using System;

namespace _001_Simple
{
	class Program
	{
		static void Main()
		{
			Console.WriteLine("Welcome to EasyConfig!");
			Console.WriteLine("----------------------");
			Console.WriteLine();

			// Reading Config File
			var C = new Config("Config.xml");

			// Writing Data to the Console
			Console.WriteLine("Port: " + C.Port);
			Console.WriteLine("Hostname: " + C.Hostname);

			Console.WriteLine();
			Console.WriteLine();
			Console.Write("Press any key to exit...");
			Console.ReadKey(true);
		}
	}
}
