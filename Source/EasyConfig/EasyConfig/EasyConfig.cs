using XmlExt;
using System;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace EasyConfig
{
	using Attributes;
	using Exceptions;

	public class EasyConfig
	{
		public delegate void dlgVersion(XmlElement Root);

		#region Options
		public bool
			UseFields = true,
			UseProperties = false;
		#endregion

		#region Public Methods
		public void Save(object Config, string FilePath, string RootTagName)
		{
			var Doc = new XmlDocument();
			var Root = Doc.AppendNode(RootTagName);

			SaveVersion?.Invoke(Root);

			FillNode(Root, Config);

			Doc.Save(FilePath);
		}

		public T Load<T>(string FilePath) => (T)Load(FilePath, typeof(T));
		public object Load(string FilePath, Type T)
		{
			var Doc = new XmlDocument();
			Doc.Load(FilePath);

			var Root = Doc.DocumentElement;

			CheckVersion?.Invoke(Root);

			return Load(Root, T);
		}

		public dlgVersion
			CheckVersion,
			SaveVersion;
		#endregion

		#region Constants
		private const BindingFlags PUBLIC_INSTANCE_FLAG = BindingFlags.Instance | BindingFlags.Public;

		private static readonly IReadOnlyDictionary<Type, IAttributeType> AttributeMap = new Dictionary<Type, IAttributeType>
		{
			{ typeof(int), new IntAttr() },
			{ typeof(long), new Int64Attr() },
			{ typeof(bool), new BoolAttr() },
			{ typeof(char), new CharAttr() },
			{ typeof(float), new SingleAttr() },
			{ typeof(double), new DoubleAttr() },
			{ typeof(string), new StringAttr() },
			{ typeof(Version), new VersionAttr() }
		};
		#endregion

		#region Private Methods
		private void FillNode(XmlNode Tag, object Value)
		{
			var T = Value.GetType();
			bool AllFieldsNecessary = T.HasAttribute<AllFieldsNecessaryAttribute>();

			if (UseFields)
				foreach (var F in T.GetFields(PUBLIC_INSTANCE_FLAG))
					if (!F.HasAttribute<IgnoreAttribute>())
						Do(F, F.FieldType, F.GetValue(Value));

			if (UseProperties)
				foreach (var P in T.GetProperties(PUBLIC_INSTANCE_FLAG))
					if (P.CanRead && P.CanWrite && P.GetIndexParameters().Length == 0 && !P.HasAttribute<IgnoreAttribute>())
						Do(P, P.PropertyType, P.GetValue(Value));

			void Do(MemberInfo Member, Type MemberType, object MemberValue)
			{
				try
				{
					if (MemberValue == null)
					{
						if (!MemberType.IsCollection() && Member.IsNecessary(AllFieldsNecessary))
							throw new NecessaryFieldIsNullException();

						return;
					}

					var Name = Member.GetConfigName();

					if (MemberType.IsEnum)
					{
						Tag.AddAttr(Name, MemberValue);
						return;
					}

					// T? => T
					var NullableUnderlyingType = Nullable.GetUnderlyingType(MemberType);
					if (NullableUnderlyingType != null)
						MemberType = NullableUnderlyingType;

					var AttributeType = AttributeMap.GetValueOrNull(MemberType);
					if (AttributeType != null)
					{
						Tag.AddAttr(Name, AttributeType.ToString(MemberValue));
						return;
					}

					if (MemberType.IsCollection())
					{
						var C = (IEnumerable)MemberValue;
						foreach (var X in C)
						{
							if (X != null)
								CreateAndFillNode(Tag, Name, X);
						}
					}
					else
						CreateAndFillNode(Tag, Name, MemberValue);
				}
				catch (Exception E)
				{
					throw new SaveFailedException(Tag, Member, E);
				}
			}
		}

		private void CreateAndFillNode(XmlNode Container, string TagName, object Value)
			=> FillNode(Container.AppendNode(TagName), Value);

		private object Load(XmlNode Tag, Type T)
		{
			var Result = Activator.CreateInstance(T);
			bool AllFieldsNecessary = T.HasAttribute<AllFieldsNecessaryAttribute>();

			if (UseFields)
				foreach (var F in T.GetFields(PUBLIC_INSTANCE_FLAG))
					if (!F.HasAttribute<IgnoreAttribute>())
						Do(F, F.FieldType, obj => F.SetValue(Result, obj));

			if (UseProperties)
				foreach (var P in T.GetProperties(PUBLIC_INSTANCE_FLAG))
					if (P.CanRead && P.CanWrite && P.GetIndexParameters().Length == 0 && !P.HasAttribute<IgnoreAttribute>())
						Do(P, P.PropertyType, obj => P.SetValue(Result, obj));

			void Do(MemberInfo Member, Type MemberType, Action<object> Setter)
			{
				try
				{
					var Name = Member.GetConfigName();

					#region Enum Or Primitive
					// Check field is enum
					if (MemberType.IsEnum)
					{
						SetValue(Value => Enum.Parse(MemberType, Value));
						return;
					}

					// T? => T
					var NullableUnderlyingType = Nullable.GetUnderlyingType(MemberType);
					if (NullableUnderlyingType != null)
						MemberType = NullableUnderlyingType;

					// Check field is primitive
					var AttributeType = AttributeMap.GetValueOrNull(MemberType);
					if (AttributeType != null)
					{
						SetValue(Value => AttributeType.FromString(Value));
						return;
					}

					// Nested Method
					void SetValue(Func<string, object> Converter)
					{
						var Attr = Tag.Attributes[Name];
						if (Attr != null)
							// Set value by config
							Setter(Converter(Attr.Value));
						else
						{
							var DefaultAttr = Member.GetCustomAttribute<DefaultAttribute>();
							if (DefaultAttr != null)
								// Set value by default
								Setter(DefaultAttr.Value);
							else
								// Throw exception if field is necessary
								if (Member.IsNecessary(AllFieldsNecessary))
									throw new NecessaryFieldNotFoundException();
						}
					}
					#endregion

					if (MemberType.IsCollection())
					{
						Type
							ElementType = MemberType.GetCollectionElementType(),
							CollectionType = typeof(List<>).MakeGenericType(ElementType);

						var L = (IList)Activator.CreateInstance(CollectionType);

						foreach (XmlNode Node in Tag.SelectNodes($"*[local-name()='{Name}']"))
							L.Add(Load(Node, ElementType));

						Setter(L);
					}
					else
					{
						var Node = Tag.SelectSingleNode($"*[local-name()='{Name}']");
						if (Node != null)
							Setter(Load(Node, MemberType));
						else
							if (Member.IsNecessary(AllFieldsNecessary))
								throw new NecessaryFieldNotFoundException();
					}
				}
				catch (Exception E)
				{
					throw new LoadFailedException(Tag, Member, E);
				}
			}

			return Result;
		}
		#endregion
	}
}
