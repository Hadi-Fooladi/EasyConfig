﻿using XmlExt;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace EasyConfig
{
	internal static class Program
	{
		internal static void Main(string[] args)
		{
			try
			{
				string InputPath, OutputPath;

				// Adding Parameters
				OneStringParameter
					pSamplePath = new OneStringParameter("gs", "path", "Generate Sample xml file"),
					pNameSpace = new OneStringParameter("ns", "name", "namespace name (Default: no namespace)"),
					pOutputPath = new OneStringParameter("o", "path", "Output file (Default: Same name as file with '.cs' extension in the current folder)"),
					pDefaultType = new OneStringParameter("dt", "type", "class/struct (Default: class)") { CustomProcess = Value => Global.DefaultType = Value };

				P.Add(pOutputPath);
				P.Add(pNameSpace);
				P.Add(pSamplePath);
				P.Add(pDefaultType);

				#region Arguments Analysis
				try
				{
					int
						ndx = 0,
						n = args.Length - 1;

					while (ndx < n)
					{
						string Option = args[ndx++];
						if (Option[0] != '-')
							throw new Exception();

						IParameter Param = null;
						string Code = Option.Substring(1);
						foreach (var X in P)
							if (X.Code == Code)
							{
								Param = X;
								break;
							}

						if (Param == null)
							throw new Exception("Unknown Parameter");

						Param.Process(args, ref ndx);
					}

					InputPath = args[ndx];
					OutputPath = pOutputPath.Value ?? Path.GetFileNameWithoutExtension(InputPath) + ".cs";
				}
				catch (Exception E)
				{
					PrintUsage("Error: " + E.Message);
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
					SW.WriteLine("using System;");
					SW.WriteLine("using System.Xml;");
					SW.WriteLine("using System.Collections.Generic;");
					
					SW.WriteLine();

					if (pNameSpace.Value == null)
						RootNode.WriteImplementation(SW);
					else
					{
						SW.WriteLine("namespace {0}", pNameSpace.Value);
						SW.Block(() => RootNode.WriteImplementation(SW));
					}
				}

				if (pSamplePath.Value != null)
				{
					var SampleDoc = new XmlDocument();
					RootNode.WriteSample(SampleDoc.AppendNode(RootNode.Name));
					SampleDoc.Save(pSamplePath.Value);
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

		private static readonly List<IParameter> P = new List<IParameter>();

		internal static void PrintUsage(string Message = null)
		{
			if (Message != null)
			{
				Console.WriteLine();
				Console.WriteLine(Message);
			}

			Console.WriteLine();
			Console.WriteLine("Usage: ");
			Console.WriteLine("   EasyConfig [Options] File");
			Console.WriteLine();
			Console.WriteLine("Options:");

			int MaxLen = 0;
			foreach (var X in P)
				MaxLen = Math.Max(MaxLen, (X.Code + X.CodeParams).Length);

			var Format = string.Format("{{0,-{0}}}", MaxLen + 1);
			foreach (var X in P)
			{
				Console.Write("   -");
				Console.Write(Format, X.Code + " " + X.CodeParams);
				Console.WriteLine(" | " + X.Desc);
			}
		}
	}
}
