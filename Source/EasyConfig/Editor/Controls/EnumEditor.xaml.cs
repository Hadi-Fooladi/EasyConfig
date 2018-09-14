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

		public Control Control => this;
	}
}
