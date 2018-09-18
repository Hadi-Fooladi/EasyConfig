using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace EasyConfig.Editor
{
	internal partial class EnumEditor : IEditor
	{
		public EnumEditor(object Value)
		{
			InitializeComponent();

			T = Value.GetType();

			GB.Header = T.Name;
			Flags = T.HasAttribute<FlagsAttribute>();

			foreach (var Member in T.GetEnumNames())
			{
				ToggleButton B;
				object MemberValue = Enum.Parse(T, Member);

				if (Flags)
					B = new CheckBox
					{
						IsChecked = ((Enum)Value).HasFlag((Enum)MemberValue)
					};
				else
					B = new RadioButton
					{
						IsChecked = Value.Equals(MemberValue)
					};

				B.Content = Member;
				SP.Children.Add(B);
			}
		}

		private readonly Type T;
		private readonly bool Flags;

		#region IEditor Members
		public Control Control => this;

		public object Value
		{
			get
			{
				int Result = 0;

				foreach (ToggleButton X in SP.Children)
				{
					if (X.IsChecked.isFalse()) continue;

					var Member = (string)X.Content;
					Result |= (int)Enum.Parse(T, Member);

					if (!Flags)
						break;
				}

				return Enum.ToObject(T, Result);
			}
		}

		public void Validate() { }
		public void ShowItem(object Item) { }
		#endregion
	}
}
