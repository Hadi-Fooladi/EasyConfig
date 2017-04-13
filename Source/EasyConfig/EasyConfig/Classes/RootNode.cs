using XmlExt;
using System.Xml;

namespace EasyConfig
{
	internal class RootNode : Node
	{
		private readonly string Version;

		public RootNode(XmlNode N) : base(N) { Version = N.Attr("Version", null); }

		protected override string TypeName => ProposedTypeName ?? Name;

		protected override string ConstructorParameters => "string Filename";

		protected override void ConstructorPre(IndentedStreamWriter SW)
		{
			SW.WriteLine("var Doc = new XmlDocument();");
			SW.WriteLine("Doc.Load(Filename);");
			SW.WriteLine();
			SW.WriteLine("var Node = Doc.SelectSingleNode(\"{0}\");", Name);
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
			if (Version != null)
			{
				SW.WriteLine("public readonly Version Version;");
				SW.WriteLine("public static readonly Version ExpectedVersion = new Version(\"{0}\");", Version);

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
