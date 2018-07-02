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

			var C = new Config
			{
				Num = 23,
				Text = "EasyConfig",
				Persons = new List<Config.Person>
				{
					new Config.Person("Hadi", 34),
					new Config.Person("Ali", 78)
				}
			};

			var EasyConfig = new EasyConfig.EasyConfig();

			EasyConfig.Save(C, FILEPATH, "Config", VER);

			var C2 = EasyConfig.Load<Config>(FILEPATH, VER);

			Console.WriteLine(C2);
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}
	}
}
