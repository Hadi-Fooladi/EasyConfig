using System.IO;

namespace EasyConfig
{
	internal partial class Base
	{
		public void WriteDesc(StreamWriter SW)
		{
			if (Desc == null && MultiLineDesc == null) return;

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

		public void Declare(StreamWriter SW, string Type, bool isList)
		{
			WriteDesc(SW);

			var Format = string.Format("public readonly {0} {{1}};", isList ? "List<{0}>" : "{0}");
			SW.WriteLine(Format, Type, Name);

			// We put a line after declaration if it has description
			if (Desc != null) SW.WriteLine();
		}
	}
}