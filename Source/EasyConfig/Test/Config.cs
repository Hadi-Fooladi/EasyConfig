using EasyConfig.Attributes;
using System.Collections.Generic;

namespace Test
{
	internal class Config
	{
		public int Num;
		public string Text;

		[Name("Person")]
		public List<Person> Persons;

		public override string ToString() => $"Num = {Num}, Text = {Text}, Persons = [{string.Join(", ", Persons ?? new List<Person>())}]";

		[AllFieldsNecessary]
		public class Person
		{
			public int Age;
			public string Name;

			[Default(5)]
			public int DefaultTest;

			[Optional]
			[Name("Test")]
			public string Necessary = "a";

			[Name("Child")]
			public List<Person> Children;

			public Person() { }

			public Person(string Name, int Age)
			{
				this.Age = Age;
				this.Name = Name;
			}

			public override string ToString()
			{
				var S = $"{Name} ({Age}, {DefaultTest})";

				if (Children != null && Children.Count > 0)
					S += $"<{string.Join(", ", Children)}>";

				return S;
			}
		}
	}
}
