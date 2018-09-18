using System;
using EasyConfig.Attributes;
using System.Collections.Generic;

namespace Editor_Test
{
	internal enum eBloodType
	{
		Unknown,
		A,
		B,
		O,
		AB
	}

	[Flags]
	internal enum eOctal
	{
		Zero = 0,
		One = 1,
		Two = 2,
		Three = 3,
		Four = 4,
		Five,
		Six,
		Seven
	}

	internal class Config
	{
		public Version Version;

		public int Num = 7;
		public string Text = "N/A";

		public bool Boolean;

		[Name("Person")]
		public List<Person> Persons;

		public eOctal Oct;

		public override string ToString() => $"Num = {Num}, Text = {Text}, Version = {Version}, Persons = [{string.Join(", ", Persons ?? new List<Person>())}]";

		[AllFieldsNecessary]
		public class Person
		{
			public int Age;
			public string Name;

			public bool Married;

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
