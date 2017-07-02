using XmlExt;
using System.Xml;

namespace EasyConfig
{
	internal partial class Attribute
	{
		public override void Declare() => Declare(Type == "yn" ? "bool" : Type, false, ReadOnly);

		public void WriteRead()
		{
			string D = Default;
			if (D != null)
				D = ", " + D;

			string Prefix;
			switch (Type)
			{
			case "int": Prefix = "i"; break;
			case "float": Prefix = "f"; break;
			case "char": Prefix = "ch"; break;
			case "yn": Prefix = "yn"; break;
			case "Version": Prefix = "ver"; break;
			case "string": Prefix = ""; break;
			default: Prefix = null; break;
			}

			if (Prefix != null)
				Global.SW.WriteLine("{0} = Node.{1}Attr(\"{0}\"{2});", Name, Prefix, D);
			else
				Global.SW.WriteLine("Node.Attr(\"{0}\", out {0}{1});", Name, D);
		}

		public void WriteSample(XmlNode Node) => Node.AddAttr(Name, Default);

		public void WriteSave() => Global.SW.WriteLine("Node.AddAttr(\"{0}\", {0});", Name);
	}
}
