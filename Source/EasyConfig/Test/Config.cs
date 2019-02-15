using System;
using System.Collections;
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

	internal struct Point
	{
		public int x, y;

		public override string ToString() => $"({x}, {y})";
	}

	internal class Config
	{
		public Version Version;
		private static readonly Person Adam = new Person("Adam", 10000000)/* { Spouse = null }*/;

		public int Num;
		public string Text;

		[Name("Person")]
		public List<Person> Persons;

		public int? NullableInt;

		public Point? P;

		public override string ToString() => $"Num = {Num}, Text = {Text}, Version = {Version}, int? = {NullableInt}, P = {P}, Persons = [{string.Join(", ", Persons ?? new List<Person>())}]";

		[AllFieldsNecessary]
		public class Person : IEnumerable<Person>
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
			public IReadOnlyList<Person> Children;

			[Optional]
			public Person Spouse = Adam;

			[Optional]
			public long Long = 45;

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

			IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
			public IEnumerator<Person> GetEnumerator() { throw new NotImplementedException(); }
		}
	}
}
