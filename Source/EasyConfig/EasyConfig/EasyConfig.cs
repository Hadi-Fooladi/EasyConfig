using XmlExt;
using System;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace EasyConfig
{
	using Attributes;

	public class EasyConfig
	{
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
				throw new Exception("Version Mismatch");

			return Load(Root, T);
		}

		private const BindingFlags PUBLIC_INSTANCE_FLAG = BindingFlags.Instance | BindingFlags.Public;

		private static readonly EasyConfigAttribute DefaultAttribute = new EasyConfigAttribute();

		private static readonly IReadOnlyDictionary<Type, IAttributeType> AttributeMap = new Dictionary<Type, IAttributeType>
		{
			{ typeof(int), new IntAttr() },
			{ typeof(bool), new BoolAttr() },
			{ typeof(char), new CharAttr() },
			{ typeof(float), new SingleAttr() },
			{ typeof(double), new DoubleAttr() },
			{ typeof(string), new StringAttr() }
		};

		private static void FillNode(XmlNode Tag, object Value)
		{
			var T = Value.GetType();

			foreach (var F in T.GetFields(PUBLIC_INSTANCE_FLAG))
			{
				var FieldValue = F.GetValue(Value);
				if (FieldValue == null)

					continue;

				var AttributeType = AttributeMap.GetValueOrNull(F.FieldType);
				if (AttributeType != null)
				{
					Tag.AddAttr(F.Name, AttributeType.ToString(FieldValue));
					continue;
				}

				var A = F.GetCustomAttribute<EasyConfigAttribute>() ?? DefaultAttribute;
				var TagName = A.Tag ?? F.Name;

				if (FieldValue is ICollection C)
					foreach (var X in C)
						CreateAndFillNode(Tag, TagName, X);
				else
					CreateAndFillNode(Tag, TagName, FieldValue);
			}
		}

		private static void CreateAndFillNode(XmlNode Container, string TagName, object Value)
			=> FillNode(Container.AppendNode(TagName), Value);

		public object Load(XmlNode Tag, Type T)
		{
			var Result = Activator.CreateInstance(T);

			foreach (var F in T.GetFields(PUBLIC_INSTANCE_FLAG))
			{
				var FieldType = F.FieldType;

				var AttributeType = AttributeMap.GetValueOrNull(FieldType);
				if (AttributeType != null)
				{
					var Attr = Tag.Attributes[F.Name];
					if (Attr != null)
						F.SetValue(Result, AttributeType.FromString(Attr.Value));

					continue;
				}

				var A = F.GetCustomAttribute<EasyConfigAttribute>() ?? DefaultAttribute;
				var TagName = A.Tag ?? F.Name;

				if (FieldType.IsCollection())
				{
					Type
						ElementType = FieldType.GetCollectionElementType(),
						CollectionType = typeof(List<>).MakeGenericType(ElementType);

					var L = (IList)Activator.CreateInstance(CollectionType);

					foreach (XmlNode Node in Tag.SelectNodes($"*[local-name()='{TagName}']"))
						L.Add(Load(Node, ElementType));

					F.SetValue(Result, L);
				}
				else
				{
					var Node = Tag.SelectSingleNode(TagName);
					if (Node != null)
						F.SetValue(Result, Load(Node, FieldType));
				}
			}

			return Result;
		}
	}
}
