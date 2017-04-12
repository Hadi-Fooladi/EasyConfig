using System;
using System.IO;

namespace EasyConfig
{
	/// <summary>
	/// StreamWriter which supports Indentation
	/// </summary>
	internal class IndentatedStreamWriter : StreamWriter
	{
		public int IndentationCount;

		public IndentatedStreamWriter(string path) : base(path) { }

		public override void WriteLine(string value)
		{
			Indent();
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
			Inside(A);
			WriteLine("}");
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
		}

		public void WriteDesc(string Desc)
		{
			if (Desc == null) return;

			WriteLine("/// <summary>");
			WriteLine("/// {0}", Desc);
			WriteLine("/// </summary>");
		}
	}
}
