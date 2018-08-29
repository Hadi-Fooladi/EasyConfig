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
		public void Save(object Config, string FilePath, string RootTagName, Version Version)
		{
			var Doc = new XmlDocument();
			var Root = Doc.AppendNode(RootTagName);

			Root.AddAttr("Version", Version);
			FillNode(Root, Config);

			Doc.Save(FilePath);
		}

		public T Load<T>(string FilePath, Version ExpectedVersion) => (T)Load(FilePath, typeof(T), ExpectedVersion);
		public object Load(string FilePath, Type T, Version ExpectedVersion)
		{
			var Doc = new XmlDocument();
			Doc.Load(FilePath);

			var Root = Doc.DocumentElement;
			var V = Root.verAttr("Version");

			if (V.Major != ExpectedVersion.Major || V.Minor > ExpectedVersion.Minor)
				throw new VersionMismatchException();

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
			{ typeof(string), new StringAttr() }
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
					if (!FieldType.IsCollection() && IsNecessary(F, AllFieldsNecessary))
						throw new NecessaryFieldIsNullException();

					continue;
				}

				var Name = GetConfigName(F);

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
				var Name = GetConfigName(F);
				var FieldType = F.FieldType;

				// Check field is primitive
				var AttributeType = AttributeMap.GetValueOrNull(FieldType);
				if (AttributeType != null)
				{
					var Attr = Tag.Attributes[Name];
					if (Attr != null)
						// Set value by config
						F.SetValue(Result, AttributeType.FromString(Attr.Value));
					else
					{
						var DefaultAttr = F.GetCustomAttribute<DefaultAttribute>();
						if (DefaultAttr != null)
							// Set value by default
							F.SetValue(Result, DefaultAttr.Value);
						else
							// Throw exception if field is necessary
							if (IsNecessary(F, AllFieldsNecessary))
								throw new NecessaryFieldNotFoundException();
					}

					continue;
				}

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
						if (IsNecessary(F, AllFieldsNecessary))
							throw new NecessaryFieldNotFoundException();
				}
			}

			return Result;
		}

		private static string GetConfigName(MemberInfo MI)
		{
			var A = MI.GetCustomAttribute<NameAttribute>();
			return A == null ? MI.Name : A.Name;
		}

		private static bool IsNecessary(MemberInfo MI, bool Necessary)
			=> Necessary ?
				!MI.HasAttribute<OptionalAttribute>() :
				MI.HasAttribute<NecessaryAttribute>();
		#endregion
	}
}
