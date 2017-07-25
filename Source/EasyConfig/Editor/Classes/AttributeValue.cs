using System;
using System.ComponentModel;

namespace Editor
{
	internal class AttributeValue : INotifyPropertyChanged
	{
		public delegate void dlg(AttributeValue Sender);

		#region Constructors
		public AttributeValue(EasyConfig.Attribute Attr)
		{
			Name = Attr.Name;
			Type = Attr.Type;
			HasDefault = Attr.Default != null;
			Default = RemoveQuotation(Attr.Default);

			Desc = Attr.Description;
		}

		public AttributeValue(string Type, string Name)
		{
			this.Type = Type;
			this.Name = Name;
		}
		#endregion

		public readonly string Desc;

		private string m_Value;
		private bool m_OverrideDefault;

		#region Properties
		public string Value
		{
			get => m_Value;
			set
			{
				if (m_Value == value) return;

				m_Value = value;
				fireValueChanged();
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
			}
		}

		public bool OverrideDefault
		{
			get => m_OverrideDefault;
			set
			{
				if (m_OverrideDefault == value) return;

				m_OverrideDefault = value;
				fireOverrideDefaultChanged();
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OverrideDefault"));
			}
		}

		public string Name { get; }
		public string Type { get; }
		public string Default { get; }
		public bool HasDefault { get; }

		public string CurValue => CurDefault ? Default : Value;
		public string PrevValue { get; private set; }

		public bool Changed => CurDefault != PrevDefault || CurValue != PrevValue;

		public bool CurDefault => HasDefault && !OverrideDefault;
		public bool PrevDefault { get; private set; }
		#endregion

		#region Public Methods
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
				foreach (var E in Global.Schema.Enums)
					if (E.Name == Type)
					{
						foreach (var Member in E.MembersArray)
							if (Value == Member.Name)
								return true;

						return false;
					}

				throw new Exception("Unknown data type");
			}
		}

		public void ResetPrevValues()
		{
			PrevValue = CurValue;
			PrevDefault = CurDefault;
		}

		public void RevertChanges()
		{
			Value = PrevValue;
			OverrideDefault = HasDefault ? !PrevDefault : false;
		}
		#endregion

		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		public static event dlg OnValueChanged, OnOverrideDefaultChanged;

		private void fireValueChanged() => OnValueChanged?.Invoke(this);
		private void fireOverrideDefaultChanged() => OnOverrideDefaultChanged?.Invoke(this);
		#endregion

		#region Private Methods
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
		#endregion
	}
}
