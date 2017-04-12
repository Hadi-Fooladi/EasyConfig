using System;
using System.IO;

namespace EasyConfig
{
	/// <summary>
	/// StreamWriter which supports Indentation
	/// </summary>
	internal class IndentedStreamWriter : StreamWriter
	{
		public int IndentationCount;

		/// <summary>
		/// true if there is a blank line before current line
		/// </summary>
		private bool BlankLine;

		/// <summary>
		/// Used to remove blank line after '{'
		/// </summary>
		private bool OpenBrace;

		public IndentedStreamWriter(string path) : base(path) { }

		/// <summary>
		/// Prevent double blank line
		/// </summary>
		public override void WriteLine() => BlankLine = true;

		public override void WriteLine(string value)
		{
			if (BlankLine)
			{
				if (!OpenBrace)
					base.WriteLine();

				BlankLine = false;
			}
			Indent();
			OpenBrace = false;
			base.WriteLine(value);
		}

		public override void WriteLine(string format, object arg0) => WriteLine(string.Format(format, arg0));
		public override void WriteLine(string format, params object[] arg) => WriteLine(string.Format(format, arg));
		public override void WriteLine(string format, object arg0, object arg1) => WriteLine(string.Format(format, arg0, arg1));
		public override void WriteLine(string format, object arg0, object arg1, object arg2) => WriteLine(string.Format(format, arg0, arg1, arg2));

		private void Indent()
		{
			for (int i = 0; i < IndentationCount; i++)
				Write('\t');
		}

		public void Block(Action A)
		{
			WriteLine("{");
			OpenBrace = true;
			Inside(A);
			BlankLine = false; // Removing blank line before '}'
			WriteLine("}");
			WriteLine();
		}

		public void Inside(Action A)
		{
			IndentationCount++;
			A();
			IndentationCount--;
		}

		public void Declare(string Name, string Type, bool isList, string Desc)
		{
			WriteDesc(Desc);
			var Format = string.Format("public readonly {0} {{1}};", isList ? "List<{0}>" : "{0}");
			WriteLine(Format, Type, Name);

			// We put a line after declaration if it has description
			if (Desc != null) WriteLine();
		}

		public void WriteDesc(string Desc)
		{
			if (Desc == null) return;

			WriteLine();
			WriteLine("/// <summary>");
			WriteLine("/// {0}", Desc);
			WriteLine("/// </summary>");
		}

		public void RemoveBlankLine() => BlankLine = false;
	}
}
