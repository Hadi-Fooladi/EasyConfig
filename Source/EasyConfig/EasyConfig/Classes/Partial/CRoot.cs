﻿using XmlExt;
using System.Xml;

namespace EasyConfig
{
	internal partial class CRoot
	{
		private const string PARAM = "string Filename";

		public override string DataTypeName => TypeName ?? Name;

		protected override string SaveMethodParameters => PARAM;
		protected override string ConstructorParameters => PARAM;

		protected override void ConstructorPre()
		{
			var SW = Global.SW;

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

		protected override void DeclareFields()
		{
			var V = Version;
			if (V != null)
			{
				Global.SW.WriteLine("public readonly Version Version;");
				Global.SW.WriteLine("public static readonly Version ExpectedVersion = new Version({0}, {1});", V.Major, V.Minor);
				Global.SW.WriteLine();
			}

			base.DeclareFields();
		}

		public new void WriteSample(XmlNode Node)
		{
			if (Version != null)
				Node.AddAttr("Version", Version);

			base.WriteSample(Node);
		}

		public override void SaveMethodPre()
		{
			var SW = Global.SW;

			SW.WriteLine("var Doc = new XmlDocument();");
			SW.WriteLine($"var Node = Doc.AppendNode(\"{TagName ?? Name}\");");
			SW.WriteLine();
		}

		public override void SaveMethodPost()
		{
			base.SaveMethodPost();
			Global.SW.WriteLine();
			Global.SW.WriteLine("Doc.Save(Filename);");
		}
	}
}
