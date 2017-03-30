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
	}
}
