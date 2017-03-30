using System;
using System.IO;
using System.Xml;

namespace EasyConfig
{
	internal static class Program
	{
		internal static void Main(string[] args)
		{
			int n = args.Length;
			if (n % 2 != 1)
			{
				PrintUsage();
				return;
			}

			try
			{
				string
					InputPath,
					NameSpace = null,
					OutputPath = null;

				#region Arguments Analysis
				try
				{
					int i = 0;

					n--;
					while (i < n)
					{
						string Option = args[i];
						if (Option[0] != '-')
							throw new Exception();

						i++;
						switch (Option.Substring(1))
						{
						case "o": OutputPath = args[i]; break;
						case "ns": NameSpace = args[i]; break;
						default: throw new Exception();
						}

						i++;
					}

					InputPath = args[i];
					if (OutputPath == null)
						OutputPath = Path.GetFileNameWithoutExtension(InputPath) + ".cs";
				}
				catch
				{
					PrintUsage();
					return;
				}
				#endregion

				var Doc = new XmlDocument();
				Doc.Load(InputPath);

				var Root = Doc.SelectSingleNode("Root");
				var RootNode = new RootNode(Root);

				using (var SW = new IndentatedStreamWriter(OutputPath))
				{
					SW.WriteLine("using XmlExt;");
					SW.WriteLine("using System.Xml;");
					SW.WriteLine("using System.Collections.Generic;");
					
					SW.WriteLine();

					if (NameSpace == null)
						RootNode.WriteImplementation(SW);
					else
					{
						SW.WriteLine("namespace {0}", NameSpace);
						SW.Block(() => RootNode.WriteImplementation(SW));
					}
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

		internal static void PrintUsage()
		{
			Console.WriteLine();
			Console.WriteLine("Usage: ");
			Console.WriteLine("   EasyConfig [Options] File");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("\t-o\t Output file (Default: Same name as file with '.cs' extension in the current folder)");
			Console.WriteLine("\t-ns\t namespace name (Default: no namespace)");
		}
	}
}
