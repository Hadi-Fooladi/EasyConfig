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
		#region Public Methods
		public void Save(object Config, string FilePath, string RootTagName)
		{
			var Doc = new XmlDocument();
			var Root = Doc.AppendNode(RootTagName);

			FillNode(Root, Config);

			Doc.Save(FilePath);
		}

		public T Load<T>(string FilePath) => (T)Load(FilePath, typeof(T));
		public object Load(string FilePath, Type T)
		{
			var Doc = new XmlDocument();
			Doc.Load(FilePath);

			var Root = Doc.DocumentElement;

			return Load(Root, T);
		}
		#endregion

		#region Constants
		private const BindingFlags PUBLIC_INSTANCE_FLAG = BindingFlags.Instance | BindingFlags.Public;

		private static readonly IReadOnlyDictionary<Type, IAttributeType> AttributeMap = new Dictionary<Type, IAttributeType>
		{
			{ typeof(int), new IntAttr() },
			{ typeof(bool), new BoolAttr() },
			{ typeof(char), new CharAttr() },
			{ typeof(float), new SingleAttr() },
			{ typeof(double), new DoubleAttr() },
			{ typeof(string), new StringAttr() },
			{ typeof(Version), new VersionAttr() }
		};
		#endregion

		#region Private Methods
		private static void FillNode(XmlNode Tag, object Value)
		{
			var T = Value.GetType();
			bool AllFieldsNecessary = T.HasAttribute<AllFieldsNecessaryAttribute>();

			foreach (var F in T.GetFields(PUBLIC_INSTANCE_FLAG))
			{
				var FieldType = F.FieldType;
				var FieldValue = F.GetValue(Value);

				if (FieldValue == null)
				{
					if (!FieldType.IsCollection() && F.IsNecessary(AllFieldsNecessary))
						throw new NecessaryFieldIsNullException();

					continue;
				}

				var Name = F.GetConfigName();

				if (FieldType.IsEnum)
				{
					Tag.AddAttr(Name, FieldValue);
					continue;
				}

				var AttributeType = AttributeMap.GetValueOrNull(FieldType);
				if (AttributeType != null)
				{
					Tag.AddAttr(Name, AttributeType.ToString(FieldValue));
					continue;
				}

				if (FieldValue is ICollection C)
					foreach (var X in C)
						CreateAndFillNode(Tag, Name, X);
				else
					CreateAndFillNode(Tag, Name, FieldValue);
			}
		}

		private static void CreateAndFillNode(XmlNode Container, string TagName, object Value)
			=> FillNode(Container.AppendNode(TagName), Value);

		private static object Load(XmlNode Tag, Type T)
		{
			var Result = Activator.CreateInstance(T);
			bool AllFieldsNecessary = T.HasAttribute<AllFieldsNecessaryAttribute>();

			foreach (var F in T.GetFields(PUBLIC_INSTANCE_FLAG))
			{
				var Name = F.GetConfigName();
				var FieldType = F.FieldType;

				#region Enum Or Primitive
				// Check field is enum
				if (FieldType.IsEnum)
				{
					SetValue(Value => Enum.Parse(FieldType, Value));
					continue;
				}

				// Check field is primitive
				var AttributeType = AttributeMap.GetValueOrNull(FieldType);
				if (AttributeType != null)
				{
					SetValue(Value => AttributeType.FromString(Value));
					continue;
				}

				// Nested Method
				void SetValue(Func<string, object> Converter)
				{
					var Attr = Tag.Attributes[Name];
					if (Attr != null)
						// Set value by config
						F.SetValue(Result, Converter(Attr.Value));
					else
					{
						var DefaultAttr = F.GetCustomAttribute<DefaultAttribute>();
						if (DefaultAttr != null)
							// Set value by default
							F.SetValue(Result, DefaultAttr.Value);
						else
							// Throw exception if field is necessary
							if (F.IsNecessary(AllFieldsNecessary))
								throw new NecessaryFieldNotFoundException();
					}
				}
				#endregion

				if (FieldType.IsCollection())
				{
					Type
						ElementType = FieldType.GetCollectionElementType(),
						CollectionType = typeof(List<>).MakeGenericType(ElementType);

					var L = (IList)Activator.CreateInstance(CollectionType);

					foreach (XmlNode Node in Tag.SelectNodes($"*[local-name()='{Name}']"))
						L.Add(Load(Node, ElementType));

					F.SetValue(Result, L);
				}
				else
				{
					var Node = Tag.SelectSingleNode(Name);
					if (Node != null)
						F.SetValue(Result, Load(Node, FieldType));
					else
						if (F.IsNecessary(AllFieldsNecessary))
							throw new NecessaryFieldNotFoundException();
				}
			}

			return Result;
		}
		#endregion
	}
}
