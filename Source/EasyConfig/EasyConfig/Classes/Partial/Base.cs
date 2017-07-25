using System;

namespace EasyConfig
{
	internal partial class Base
	{
		public Base(string Name) { this.Name = Name; }

		public bool HasDesc => Desc != null || MultiLineDesc != null;

		public void WriteDesc()
		{
			if (!HasDesc) return;

			var SW = Global.SW;

			SW.WriteLine();
			SW.WriteLine("/// <summary>");

			if (MultiLineDesc == null)
				SW.WriteLine("/// {0}", Desc);
			else
			{
				int
					i = 0,
					n = MultiLineDesc.Lines.Count;

				foreach (var L in MultiLineDesc.Lines)
					SW.WriteLine("/// {0}{1}", L.Value, ++i == n ? "" : "<br />");
			}

			SW.WriteLine("/// </summary>");
		}

		public virtual void Declare() => throw new NotImplementedException();

		public void Declare(string Type, bool isList, bool ReadOnly)
		{
			WriteDesc();

			var Format = string.Format("public{{0}} {0} {{2}};", isList ? "List<{1}>" : "{1}");
			Global.SW.WriteLine(Format, ReadOnly ? " readonly" : "", Type, Name);

			// We put a line after declaration if it has description
			if (HasDesc) Global.SW.WriteLine();
		}

		public void WriteSave(string TagName, bool isList)
		{
			var SW = Global.SW;

			var Format = isList
				? "foreach (var X in {0}) X.Save(Node.AppendNode(\"{1}\"));"
				: "if ({0} != null) {0}.Save(Node.AppendNode(\"{1}\"));";

			SW.WriteLine(Format, Name, TagName);
		}
	}
}