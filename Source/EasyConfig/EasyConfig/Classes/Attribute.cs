using XmlExt;
using System.Xml;

namespace Schema
{
	internal partial class Attribute
	{
		public void WriteDeclaration(IndentedStreamWriter SW) => SW.Declare(Name, Type == "yn" ? "bool" : Type, false, Desc);

		public void WriteRead(IndentedStreamWriter SW)
		{
			string Prefix;
			switch (Type[0])
			{
			case 'i': Prefix = "i"; break;
			case 'f': Prefix = "f"; break;
			case 'y': Prefix = "yn"; break;
			case 'V': Prefix = "ver"; break;
			default: Prefix = ""; break;
			}

			string D = Default;
			if (D != null)
				D = ", " + D;

			SW.WriteLine("{0} = Node.{1}Attr(\"{0}\"{2});", Name, Prefix, D);
		}

		public void WriteSample(XmlNode Node) => Node.AddAttr(Name, Default);
	}
}
