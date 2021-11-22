using System;

namespace EasyConfig
{
	internal class GenericAttr : IAttributeType
	{
		private readonly Func<string, object> _fromString;

		public GenericAttr(string typeName, Func<string, object> fromString)
		{
			TypeName = typeName;
			_fromString = fromString;
		}

		public string TypeName { get; }
		public object FromString(string s) => _fromString(s);
		public string ToString(object value) => value.ToString();
	}

	internal class UInt16Attr : GenericAttr
	{
		public UInt16Attr() : base("ushort", s => ushort.Parse(s)) { }
	}

	internal class Int16Attr : GenericAttr
	{
		public Int16Attr() : base("short", s => short.Parse(s)) { }
	}

	internal class ByteAttr : GenericAttr
	{
		public ByteAttr() : base("byte", s => byte.Parse(s)) { }
	}

	internal class StringAttr : IAttributeType
	{
		public object FromString(string S) => S;
		public string ToString(object Value) => Convert.ToString(Value);

		public string TypeName => "string";
	}

	internal class IntAttr : IAttributeType
	{
		public object FromString(string S) => int.Parse(S);
		public string ToString(object Value) => Value.ToString();

		public string TypeName => "int";
	}

	internal class Int64Attr : IAttributeType
	{
		public object FromString(string S) => long.Parse(S);
		public string ToString(object Value) => Value.ToString();

		public string TypeName => "long";
	}

	internal class SingleAttr : IAttributeType
	{
		public object FromString(string S) => float.Parse(S);
		public string ToString(object Value) => Value.ToString();

		public string TypeName => "float";
	}

	internal class CharAttr : IAttributeType
	{
		public object FromString(string S) => S[0];
		public string ToString(object Value) => Value.ToString();

		public string TypeName => "char";
	}

	internal class BoolAttr : IAttributeType
	{
		public string ToString(object Value) => (bool)Value ? "Yes" : "No";
		public object FromString(string S) => StringComparer.OrdinalIgnoreCase.Equals(S, "Yes");

		public string TypeName => "Yes/No";
	}

	internal class DoubleAttr : IAttributeType
	{
		public object FromString(string S) => double.Parse(S);
		public string ToString(object Value) => Value.ToString();

		public string TypeName => "double";
	}

	internal class VersionAttr : IAttributeType
	{
		public object FromString(string S) => new Version(S);
		public string ToString(object Value) => Convert.ToString(Value);

		public string TypeName => "Version";
	}
}
