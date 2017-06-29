using XmlExt;
using System.Xml;

namespace EasyConfig
{
	internal partial class Attribute
	{
		public override void Declare() => Declare(Type == "yn" ? "bool" : Type, false, ReadOnly);

		public void WriteRead()
		{
			string Prefix;
			switch (Type[0])
			{
			case 'i': Prefix = "i"; break;
			case 'f': Prefix = "f"; break;
			case 'c': Prefix = "ch"; break;
			case 'y': Prefix = "yn"; break;
			case 'V': Prefix = "ver"; break;
			default: Prefix = ""; break;
			}

			string D = Default;
			if (D != null)
				D = ", " + D;

			Global.SW.WriteLine("{0} = Node.{1}Attr(\"{0}\"{2});", Name, Prefix, D);
		}

		public void WriteSample(XmlNode Node) => Node.AddAttr(Name, Default);

		public void WriteSave() => Global.SW.WriteLine("Node.AddAttr(\"{0}\", {0});", Name);
	}
}
