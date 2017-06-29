using System;
using System.Xml;

namespace XmlExt
{
	/// <summary>
	/// Useful extension methods for working with XML documents
	/// </summary>
	public static class XmlExt
	{
		public static string Attr(this XmlNode Node, string Name) => Node.Attributes[Name].Value;
		public static int iAttr(this XmlNode Node, string Name) => Convert.ToInt32(Node.Attr(Name));
		public static char chAttr(this XmlNode Node, string Name) => Convert.ToChar(Node.Attr(Name));
		public static float fAttr(this XmlNode Node, string Name) => Convert.ToSingle(Node.Attr(Name));
		public static bool ynAttr(this XmlNode Node, string Name) => string.Equals(Node.Attr(Name), "Yes", StringComparison.OrdinalIgnoreCase);

		public static Version verAttr(this XmlNode Node, string Name)
		{
			var A = Node.Attributes[Name];
			return A == null ? null : new Version(A.Value);
		}

		public static string Attr(this XmlNode Node, string Name, string Default)
		{
			var A = Node.Attributes[Name];
			return A == null ? Default : A.Value;
		}

		public static int iAttr(this XmlNode Node, string Name, int Default)
		{
			var A = Node.Attributes[Name];
			return A == null ? Default : Convert.ToInt32(A.Value);
		}

		public static float fAttr(this XmlNode Node, string Name, float Default)
		{
			var A = Node.Attributes[Name];
			return A == null ? Default : Convert.ToSingle(A.Value);
		}

		public static bool ynAttr(this XmlNode Node, string Name, bool Default)
		{
			var A = Node.Attributes[Name];
			return A == null ? Default : string.Equals(A.Value, "Yes", StringComparison.OrdinalIgnoreCase);
		}

		public static char chAttr(this XmlNode Node, string Name, char Default)
		{
			var A = Node.Attributes[Name];
			return A == null ? Default : Convert.ToChar(A.Value);
		}

		public static XmlElement AppendNode(this XmlNode Node, string Name)
		{
			var E = Node.OwnerDocument.CreateElement(Name);
			Node.AppendChild(E);
			return E;
		}

		public static XmlElement AppendNode(this XmlDocument Doc, string Name)
		{
			var E = Doc.CreateElement(Name);
			Doc.AppendChild(E);
			return E;
		}

		public static XmlAttribute AddAttr(this XmlNode Node, string Name, string Value)
		{
			XmlAttribute A = Node.OwnerDocument.CreateAttribute(Name);
			A.Value = Value;
			Node.Attributes.Append(A);
			return A;
		}

		public static XmlAttribute AddAttr(this XmlNode Node, string Name, object Value) => Node.AddAttr(Name, Value.ToString());
		public static XmlAttribute AddAttr(this XmlNode Node, string Name, bool Value) => Node.AddAttr(Name, Value ? "Yes" : "No");
	}
}