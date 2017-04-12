using XmlExt;
using System.IO;
using System.Xml;

namespace EasyConfig
{
	internal class Attribute
	{
		public string Name, Type, Desc, Default;

		public Attribute() { }

		public Attribute(XmlNode N)
		{
			Name = N.Attr("Name");
			Type = N.Attr("Type");
			Desc = N.Attr("Desc", null);
			Default = N.Attr("Default", null);
		}

		public void WriteDeclaration(IndentedStreamWriter SW) => SW.Declare(Name, Type == "yn" ? "bool" : Type, false, Desc);

		public void WriteRead(StreamWriter SW)
		{
			string Prefix;
			switch (Type[0])
			{
			case 'i': Prefix = "i"; break;
			case 'f': Prefix = "f"; break;
			case 'y': Prefix = "yn"; break;
			default: Prefix = ""; break;
			}

			string D = Default;
			if (D != null)
			{
				if (Prefix == "")
					D = '"' + D + '"';

				D = ", " + D;
			}

			SW.WriteLine("{0} = Node.{1}Attr(\"{0}\"{2});", Name, Prefix, D);
		}

		public void WriteSample(XmlNode Node) => Node.AddAttr(Name, Default);
	}
}
