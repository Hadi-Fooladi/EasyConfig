using System;
using System.IO;

namespace EasyConfig
{
	internal partial class Base
	{
		public bool HasDesc => Desc != null || MultiLineDesc != null;

		public void WriteDesc(StreamWriter SW)
		{
			if (!HasDesc) return;

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

		public virtual void Declare(StreamWriter SW) => throw new NotImplementedException();

		public void Declare(StreamWriter SW, string Type, bool isList, bool ReadOnly)
		{
			WriteDesc(SW);

			var Format = string.Format("public{{0}} {0} {{2}};", isList ? "List<{1}>" : "{1}");
			SW.WriteLine(Format, ReadOnly ? " readonly" : "", Type, Name);

			// We put a line after declaration if it has description
			if (HasDesc) SW.WriteLine();
		}
	}
}