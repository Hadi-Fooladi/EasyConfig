﻿using XmlExt;
using System;
using System.IO;
using System.Xml;
using System.Reflection;
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

				#region Adding Parameters
				OneStringParameter
					pSamplePath = new OneStringParameter("gs", "path", "Generate Sample xml file"),
					pNameSpace = new OneStringParameter("ns", "name", "namespace name (Default: no namespace)"),
					pOutputPath = new OneStringParameter("o", "path", "Output file (Default: Same name as file with '.cs' extension in the current folder)");

				var pPublic = new ZeroParameter
				{
					Code = "public",
					Act = () => Defaults.Access = "public",
					Desc = "Change access modifier for all classes to public (By default it is internal)"
				};

				var pWritable = new ZeroParameter
				{
					Code = "w",
					Act = () => Defaults.ReadOnly = false,
					Desc = "Makes all fields writable by default (If not specified by default fields are readonly)"
				};

				P.Add(pOutputPath);
				P.Add(pNameSpace);
				P.Add(pSamplePath);
				P.Add(pPublic);
				P.Add(pWritable);
				#endregion

				#region Arguments Analysis
				try
				{
					int
						ndx = 0,
						n = args.Length - 1;

					if (n < 0)
					{
						PrintUsage();
						return;
					}

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

				var Schema = new Schema(InputPath);

				// Check Easy-Config Version
				Version
					Ver = Schema.Version,
					AppVer = Assembly.GetExecutingAssembly().GetName().Version;

				if (Ver.Major != AppVer.Major && Ver.Minor > AppVer.Minor)
					throw new Exception("Version Mismatch");

				var Root = Schema.Root;

				// Filling 'Global.Name2DataType'
				Root.RegisterName();
				foreach (var T in Schema.Types) T.RegisterName();

				using (var SW = new IndentedStreamWriter(OutputPath))
				{
					SW.WriteLine("// auto-generated by EasyConfig (v{0})", AppVer.ToString(3));
					SW.WriteLine();

					SW.WriteLine("using XmlExt;");
					SW.WriteLine("using System;");
					SW.WriteLine("using System.Xml;");
					SW.WriteLine("using System.Collections.Generic;");
					
					SW.WriteLine();

					if (pNameSpace.Value == null)
						WriteCode();
					else
					{
						SW.WriteLine($"namespace {pNameSpace.Value}");
						SW.Block(WriteCode);
					}

					void WriteCode()
					{
						Root.WriteImplementation(SW);
						foreach (var T in Schema.Types) T.WriteImplementation(SW);
					}
				}

				if (pSamplePath.Value != null)
				{
					var SampleDoc = new XmlDocument();
					Root.WriteSample(SampleDoc.AppendNode(Root.Name));
					SampleDoc.Save(pSamplePath.Value);
				}
			}
			catch (Exception E)
			{
				Console.WriteLine("----------------------------------------");
				Console.WriteLine("Error: " + E.Message);
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
