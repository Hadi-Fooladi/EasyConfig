using EasyConfig.Attributes;
using System.Collections.Generic;

namespace Test
{
	internal class Config
	{
		public int Num;
		public string Text;

		[EasyConfig(Tag = "Person")]
		public List<Person> Persons;

		public override string ToString() => $"Num = {Num}, Text = {Text}, Persons = [{string.Join(", ", Persons ?? new List<Person>())}]";

		public class Person
		{
			public int Age;
			public string Name;

			public Person() { }

			public Person(string Name, int Age)
			{
				this.Age = Age;
				this.Name = Name;
			}

			public override string ToString() => $"{Name} ({Age})";
		}
	}
}
