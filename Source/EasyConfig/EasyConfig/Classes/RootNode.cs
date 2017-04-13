using XmlExt;
using System.Xml;

namespace EasyConfig
{
	internal class RootNode : Node
	{
		private readonly Schema.EasyConfig.CRoot R;

		public RootNode(Schema.EasyConfig.CRoot R) : base(R) { this.R = R; }

		protected override string TypeName => R.TypeName ?? Name;

		protected override string ConstructorParameters => "string Filename";

		protected override void ConstructorPre(IndentedStreamWriter SW)
		{
			SW.WriteLine("var Doc = new XmlDocument();");
			SW.WriteLine("Doc.Load(Filename);");
			SW.WriteLine();
			SW.WriteLine("var Node = Doc.SelectSingleNode(\"{0}\");", Name);
			SW.WriteLine();

			if (R.Version != null)
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
			var V = R.Version;
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
			if (R.Version != null)
				Node.AddAttr("Version", R.Version);

			base.WriteSample(Node);
		}
	}
}
