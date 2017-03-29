using System;
using System.Xml;

namespace EasyConfig
{
	internal class Program
	{
		internal static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("Usage: ");
				Console.WriteLine("   EasyConfig File");
				return;
			}

			try
			{
				var Filename = args[0];
				var Doc = new XmlDocument();
				Doc.Load(Filename);

				var Root = Doc.SelectSingleNode("Root");
				var RootNode = new RootNode(Root);

				using (var SW = new IndentatedStreamWriter("Config.cs"))
				{
					SW.WriteLine("using XmlExt;");
					SW.WriteLine("using System.Xml;");
					SW.WriteLine("using System.Collections.Generic;");
					
					SW.WriteLine();

					RootNode.WriteImplementation(SW);
				}
			}
			catch (Exception E)
			{
				Console.WriteLine("----------------------------------------");
				Console.WriteLine("Error");
				Console.WriteLine("Message" + E.Message);
				Console.WriteLine("Stack Trace: ");
				Console.WriteLine(E.StackTrace);
				Console.WriteLine("----------------------------------------");
			}
		}
	}
}
