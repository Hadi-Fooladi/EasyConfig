using System.Xml;

namespace EasyConfig
{
	internal class RootNode : Node
	{
		public RootNode(XmlNode N) : base(N) { }

		protected override string Type => Name;

		protected override string ConstructorParameters => "string Filename";

		protected override void ConstructorPre(IndentatedStreamWriter SW)
		{
			SW.WriteLine("var Doc = new XmlDocument();");
			SW.WriteLine("Doc.Load(Filename);");
			SW.WriteLine();
			SW.WriteLine("var Node = Doc.SelectSingleNode(\"{0}\");", Name);
		}
	}
}
