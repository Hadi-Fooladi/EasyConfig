using System;

namespace EasyConfig
{
	internal static class Misc
	{
		public static void Block(this IndentatedStreamWriter SW, Action A)
		{
			SW.WriteLine("{");
			SW.IndentationCount++;

			A();

			SW.IndentationCount--;
			SW.WriteLine("}");
		}
	}
}
