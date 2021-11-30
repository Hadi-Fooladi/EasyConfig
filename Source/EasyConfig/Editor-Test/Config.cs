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

	internal struct Point
	{
		public float x, y;
	}

	internal class Config
	{
		public Version Version;

		public int Num = 7;

		[Necessary]
		public string Text = "N/A";

		[Default(false)]
		public bool Boolean;

		[Name("Person"), Description("A list")]
		public List<Person> Persons;

		[Default(eOctal.Six)]
		public eOctal Oct;

		[Description("Root")]
		public Person Root;

		[Necessary]
		public Point Position;

		public int? NullableInt;
		public eBloodType? NullableBlood;

		public int PropertyInt { get; set; }

		public byte Byte;
		public short Short;
		public ushort UShort;

		public override string ToString() => $"Num = {Num}, Text = {Text}, Version = {Version}, Persons = [{string.Join(", ", Persons ?? new List<Person>())}]";

		[AllFieldsNecessary]
		public class Person
		{
			[Ignore]
			public int SSN;

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

			[Optional, Description("A, B, AB, O")]
			public eBloodType BloodType;

			[Name("C")]
			public Config Config { get; set; }

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
