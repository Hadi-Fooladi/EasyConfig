// auto-generated by EasyConfig (v4.2.17)

using XmlExt;
using System;
using System.Xml;
using System.Collections.Generic;

namespace EasyConfig
{
	internal class Schema
	{
		public readonly Version Version;
		public readonly CRoot Root;
		public readonly List<DataType> Types;

		public Schema(string Filename)
		{
			var Doc = new XmlDocument();
			Doc.Load(Filename);

			var Node = Doc.DocumentElement;

			Version = Node.verAttr("Version");

			var RootNode = Node.SelectSingleNode("*[local-name()='Root']");
			if (RootNode != null)
				Root = new CRoot(RootNode);

			Types = new List<DataType>();
			foreach (XmlNode X in Node.SelectNodes("*[local-name()='DataType']"))
				Types.Add(new DataType(X));
		}
	}

	internal partial class Attribute
	{
		public readonly string Name;
		public readonly string Type;
		public readonly string Desc;
		public readonly string Default;

		public Attribute(XmlNode Node)
		{
			Name = Node.Attr("Name");
			Type = Node.Attr("Type");
			Desc = Node.Attr("Desc", null);
			Default = Node.Attr("Default", null);
		}
	}

	internal partial class Field
	{
		public readonly string Name;
		public readonly string Type;
		public readonly string Desc;

		/// <summary>
		/// If null, 'Name' will be used for tag name
		/// </summary>
		public readonly string TagName;

		public readonly bool Multiple;

		public Field(XmlNode Node)
		{
			Name = Node.Attr("Name");
			Type = Node.Attr("Type");
			Desc = Node.Attr("Desc", null);
			TagName = Node.Attr("TagName", null);
			Multiple = Node.ynAttr("Multiple", false);
		}
	}

	internal partial class DataType
	{
		public readonly string Name;
		public readonly string Desc;
		public readonly string Inherit;
		public readonly bool Partial;

		/// <summary>
		/// Access modifier for class (null means internal)
		/// </summary>
		public readonly string Access;

		public readonly List<Field> Fields;
		public readonly List<Attribute> Attributes;

		public DataType(XmlNode Node)
		{
			Name = Node.Attr("Name");
			Desc = Node.Attr("Desc", null);
			Inherit = Node.Attr("Inherit", null);
			Partial = Node.ynAttr("Partial", false);
			Access = Node.Attr("Access", null);

			Fields = new List<Field>();
			foreach (XmlNode X in Node.SelectNodes("*[local-name()='Field']"))
				Fields.Add(new Field(X));

			Attributes = new List<Attribute>();
			foreach (XmlNode X in Node.SelectNodes("*[local-name()='Attribute']"))
				Attributes.Add(new Attribute(X));
		}
	}

	internal partial class Node : DataType
	{
		public readonly bool Multiple;
		public readonly string TypeName;

		/// <summary>
		/// If null, 'Name' will be used for tag name
		/// </summary>
		public readonly string TagName;

		public readonly List<Node> Nodes;
		public readonly List<DataType> Types;

		public Node(XmlNode Node) : base(Node)
		{
			Multiple = Node.ynAttr("Multiple", false);
			TypeName = Node.Attr("TypeName", null);
			TagName = Node.Attr("TagName", null);

			Nodes = new List<Node>();
			foreach (XmlNode X in Node.SelectNodes("*[local-name()='Node']"))
				Nodes.Add(new Node(X));

			Types = new List<DataType>();
			foreach (XmlNode X in Node.SelectNodes("*[local-name()='DataType']"))
				Types.Add(new DataType(X));
		}
	}

	internal partial class CRoot : Node
	{
		public readonly Version Version;

		public CRoot(XmlNode Node) : base(Node)
		{
			Version = Node.verAttr("Version");
		}
	}
}
