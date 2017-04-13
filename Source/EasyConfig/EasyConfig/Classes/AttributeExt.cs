using XmlExt;
using System.Xml;

namespace EasyConfig
{
	internal static class AttributeExt
	{
		public static void WriteDeclaration(this Schema.Attribute A, IndentedStreamWriter SW) =>
			SW.Declare(A.Name, A.Type == "yn" ? "bool" : A.Type, false, A.Desc);

		public static void WriteRead(this Schema.Attribute A, IndentedStreamWriter SW)
		{
			string Prefix;
			switch (A.Type[0])
			{
			case 'i': Prefix = "i"; break;
			case 'f': Prefix = "f"; break;
			case 'y': Prefix = "yn"; break;
			case 'V': Prefix = "ver"; break;
			default: Prefix = ""; break;
			}

			string D = A.Default;
			if (D != null)
				D = ", " + D;

			SW.WriteLine("{0} = Node.{1}Attr(\"{0}\"{2});", A.Name, Prefix, D);
		}

		public static void WriteSample(this Schema.Attribute A, XmlNode Node) => Node.AddAttr(A.Name, A.Default);
	}
}
