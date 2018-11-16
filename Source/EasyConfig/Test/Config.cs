using System;
using EasyConfig.Attributes;
using System.Collections.Generic;

namespace Test
{
	internal enum eBloodType
	{
		Unknown,
		A,
		B,
		O,
		AB
	}

	internal class Config
	{
		public Version Version;

		public int Num;
		public string Text;

		[Name("Person")]
		public List<Person> Persons;

		public override string ToString() => $"Num = {Num}, Text = {Text}, Version = {Version}, Persons = [{string.Join(", ", Persons ?? new List<Person>())}]";

		[AllFieldsNecessary]
		public class Person
		{
			[Ignore]
			public int SSN;

			public int Age;
			public string Name;

			[Default(5)]
			public int DefaultTest;

			[Optional]
			[Name("Test")]
			public string Necessary = "a";

			[Name("Child")]
			public List<Person> Children;

			[Optional]
			public eBloodType BloodType;

			public Person() { }

			public Person(string Name, int Age)
			{
				this.Age = Age;
				this.Name = Name;
			}

			public override string ToString()
			{
				var S = $"{Name} ({Age}, {DefaultTest}, {BloodType})";

				if (Children != null && Children.Count > 0)
					S += $"<{string.Join(", ", Children)}>";

				return S;
			}
		}
	}
}
