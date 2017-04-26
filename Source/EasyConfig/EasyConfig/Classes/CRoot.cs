using XmlExt;
using System.Xml;

namespace EasyConfig
{
	internal partial class CRoot
	{
		public override string DataTypeName => TypeName ?? Name;

		protected override string ConstructorParameters => "string Filename";

		protected override void ConstructorPre(IndentedStreamWriter SW)
		{
			SW.WriteLine("var Doc = new XmlDocument();");
			SW.WriteLine("Doc.Load(Filename);");
			SW.WriteLine();
			SW.WriteLine("var Node = Doc.DocumentElement;");
			SW.WriteLine();

			if (Version != null)
			{
				SW.WriteLine("// Check version");
				SW.WriteLine("Version = new Version(Node.Attr(\"Version\"));");
				SW.WriteLine("if (Version.Major != ExpectedVersion.Major || Version.Minor < ExpectedVersion.Minor)");
				SW.Inside(() => SW.WriteLine("throw new Exception(\"Version Mismatch\");"));
				SW.WriteLine();
			}
		}

		protected override void DeclareFields(IndentedStreamWriter SW)
		{
			var V = Version;
			if (V != null)
			{
				SW.WriteLine("public readonly Version Version;");
				SW.WriteLine("public static readonly Version ExpectedVersion = new Version({0}, {1});", V.Major, V.Minor);

				SW.WriteLine();
			}

			base.DeclareFields(SW);
		}

		public new void WriteSample(XmlNode Node)
		{
			if (Version != null)
				Node.AddAttr("Version", Version);

			base.WriteSample(Node);
		}
	}
}
