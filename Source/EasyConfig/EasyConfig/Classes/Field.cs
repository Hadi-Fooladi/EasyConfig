using XmlExt;
using System.Xml;

namespace EasyConfig
{
	internal class Field
	{
		public readonly bool Multiple;
		public readonly string Name, Desc, Type, TagName;

		public Field(XmlNode N)
		{
			Name = N.Attr("Name");
			Type = N.Attr("Type");
			Desc = N.Attr("Desc", null);
			TagName = N.Attr("TagName", Name);
			Multiple = N.ynAttr("Multiple", false);
		}

		public void WriteDeclaration(IndentedStreamWriter SW) => SW.Declare(Name, Type, Multiple, Desc);

		public void WriteRead(IndentedStreamWriter SW)
		{
			if (Multiple)
			{
				SW.WriteLine();
				SW.WriteLine("{0} = new List<{1}>();", Name, Type);
				SW.WriteLine("foreach (XmlNode X in Node.SelectNodes(\"{0}\"))", TagName);
				SW.Inside(() => SW.WriteLine("{0}.Add(new {1}(X));", Name, Type));
				SW.WriteLine();
			}
			else
			{
				SW.WriteLine();
				string NameNode = Name + "Node";
				SW.WriteLine("var {0} = Node.SelectSingleNode(\"{1}\");", NameNode, TagName);
				SW.WriteLine("if ({0} != null)", NameNode);
				SW.Inside(() => SW.WriteLine("{0} = new {1}({2});", Name, Type, NameNode));
				SW.WriteLine();
			}
		}
	}
}
