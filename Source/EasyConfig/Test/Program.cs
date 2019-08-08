using System;
using System.Xml;
using System.Collections.Generic;

using EasyConfig.Exceptions;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			const string FILEPATH = "Test.xml";

			var Hadi = new Config.Person("Hadi", 34);
			var Payam = new Config.Person("Payam", 32)
			{
				Children = new List<Config.Person> { new Config.Person("Faraeen", 0) }
			};

			var A = new Config.Person("A", 1);
			var B = new Config.Person("B", 2);
			var C = new Config.Person("C", 3);
			var D = new Config.Person("D", 4);
			var E = new Config.Person("E", 5);
			var G = new Config.Person("G", 6);
			var F = new Config.Person("F", 7);
			B.Children = new List<Config.Person> { D, E, G };
			D.Children = new List<Config.Person> { F };

			var Config = new Config
			{
				Num = 23,
				Text = "123",
				NullableInt = 12,
				P = new Point { x = 4, y = 5 },
				Persons = new List<Config.Person>
				{
					Hadi,
					Payam,
					A,
					B,
					C
				},
				Version = new Version(2, 4)
			};

			Hadi.Necessary = null;

			var EasyConfig = new EasyConfig.EasyConfig
			{
				UseFields = true,
				UseProperties = true
			};

			try
			{
				EasyConfig.Save(Config, FILEPATH, "Config");

				var C2 = EasyConfig.Load<Config>(FILEPATH);

				Console.WriteLine(C2);
			}
			catch (LoadFailedException Ex)
			{
				//Console.WriteLine($"Error: {Ex.Message}");
				//Console.WriteLine(GetPath(Ex.Tag));

				Show(Ex);

			}

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}

		private static void Show(Exception Ex)
		{
			var E = Ex;
			var Path = new List<string>();

			LoadFailedException Last = null;
			for (;;)
			{
				if (E is LoadFailedException LFE)
				{
					Last = LFE;
					Path.Add($"{LFE.Tag.LocalName}[{FindIndex(LFE.Tag) + 1}]");
				}

				if (E.InnerException == null)
					break;

				E = E.InnerException;
			}

			Console.WriteLine($"Path: {string.Join(" / ", Path)}");

			if (Last != null)
				Console.WriteLine($"Field: {Last.Member.Name}");

			Console.WriteLine($"Message: {E.Message}");
		}

		private static IEnumerable<XmlNode> IterateNodes(XmlNode Node)
		{
			foreach (XmlNode X in Node.ChildNodes)
				if (X.NodeType == XmlNodeType.Element)
					yield return X;
		}

		private static int FindIndex(XmlNode Node)
		{
			XmlNode P = Node.ParentNode;

			if (P == null) return -1;

			int Num = 0;
			foreach (var X in IterateNodes(P))
			{
				if (X == Node)
					return Num;

				Num++;
			}

			return -1;
		}
	}
}
