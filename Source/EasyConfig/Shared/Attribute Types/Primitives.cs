using System;

namespace EasyConfig
{
	internal class StringAttr : IAttributeType
	{
		public object FromString(string S) => S;
		public string ToString(object Value) => Convert.ToString(Value);
	}

	internal class IntAttr : IAttributeType
	{
		public object FromString(string S) => int.Parse(S);
		public string ToString(object Value) => Value.ToString();
	}

	internal class SingleAttr : IAttributeType
	{
		public object FromString(string S) => float.Parse(S);
		public string ToString(object Value) => Value.ToString();
	}

	internal class CharAttr : IAttributeType
	{
		public object FromString(string S) => S[0];
		public string ToString(object Value) => Value.ToString();
	}

	internal class BoolAttr : IAttributeType
	{
		public string ToString(object Value) => (bool)Value ? "Yes" : "No";
		public object FromString(string S) => StringComparer.OrdinalIgnoreCase.Equals(S, "Yes");
	}

	internal class DoubleAttr : IAttributeType
	{
		public object FromString(string S) => double.Parse(S);
		public string ToString(object Value) => Value.ToString();
	}

	internal class VersionAttr : IAttributeType
	{
		public object FromString(string S) => new Version(S);
		public string ToString(object Value) => Convert.ToString(Value);
	}
}
