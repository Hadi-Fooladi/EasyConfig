using System;
using System.Collections.Generic;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			Version VER = new Version(1, 0);
			const string FILEPATH = "Test.xml";

			var Hadi = new Config.Person("Hadi", 34);
			var Payam = new Config.Person("Payam", 32);
			Payam.Children = new List<Config.Person> { new Config.Person("Faraeen", 0) };

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
				Text = "Easy<Config> \"@@!!~~'\"AA\nNew Line!!!",
				Persons = new List<Config.Person>
				{
					Hadi,
					Payam,
					A,
					B,
					C
				}
			};

			var EasyConfig = new EasyConfig.EasyConfig();

			EasyConfig.Save(Config, FILEPATH, "Config", VER);

			try
			{
				var C2 = EasyConfig.Load<Config>(FILEPATH, VER);

				Console.WriteLine(C2);
			}
			catch (Exception Ex)
			{
				Console.WriteLine($"Error: {Ex.Message}");
			}

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}
	}
}
