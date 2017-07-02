using System;

namespace Editor
{
	internal class AttributeValue
	{
		public AttributeValue(EasyConfig.Attribute Attr)
		{
			Name = Attr.Name;
			Type = Attr.Type;
			HasDefault = Attr.Default != null;
			Default = RemoveQuotation(Attr.Default);

			if (Attr.MultiLineDesc == null) Desc = Attr.Desc;
			else Desc = string.Join(Environment.NewLine, Attr.MultiLineDesc.Lines);
		}

		public AttributeValue(string Type, string Name)
		{
			this.Type = Type;
			this.Name = Name;
		}

		public string Value { get; set; }
		public bool OverrideDefault { get; set; }

		public string Name { get; }
		public string Type { get; }
		public string Default { get; }
		public bool HasDefault { get; }

		public readonly string Desc;

		private static string RemoveQuotation(string S)
		{
			if (S == null)
				return null;

			int n = S.Length;
			if (n < 2) return S;

			if (S[0] == '"' && S[n - 1] == '"')
				return S.Substring(1, n - 2);

			return S;
		}

		public override string ToString() => Name;

		public bool Validate()
		{
			if (HasDefault && !OverrideDefault) return true;

			switch (Type)
			{
			case "string": return true;
			case "char": return Value == null ? false : Value.Length == 1;
			case "int": return int.TryParse(Value, out int _);
			case "float": return float.TryParse(Value, out float _);
			case "Version": return Version.TryParse(Value, out Version _);
			case "yn":
				if (Value == null) return false;
				switch (Value.ToLower())
				{
				case "no":
				case "yes": return true;
				}
				return false;
			default:
				throw new Exception("Unknown data type");
			}
		}
	}
}
