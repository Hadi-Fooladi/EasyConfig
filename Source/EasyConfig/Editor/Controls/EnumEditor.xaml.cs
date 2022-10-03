using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace EasyConfig.Editor
{
	internal partial class EnumEditor : IEditor
	{
		#region Constructors
		public EnumEditor(Type T, object Value, object Default)
		{
			this.T = T;
			InitializeComponent();

			GB.Header = T.Name;
			Flags = T.HasAttribute<FlagsAttribute>();

			if (Value == null)
				Value = Activator.CreateInstance(T);

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

			if (Default == null)
				DefaultPanel.Visibility = Visibility.Collapsed;
			else
				lblDefault.Text = Default.ToString();
		}
		#endregion

		private readonly Type T;
		private readonly bool Flags;

		#region IEditor Members
		public Control Control => this;

		public object Value
		{
			get
			{
				int Result = 0;

				foreach (ToggleButton tb in SP.Children)
				{
					if (tb.IsChecked != true) continue;

					var Member = (string)tb.Content;
					Result |= (int)Enum.Parse(T, Member);

					if (!Flags)
						break;
				}

				return Enum.ToObject(T, Result);
			}

			set => throw new NotSupportedException();
		}

		public bool Ignored
		{
			get => cbIgnore.IsChecked == true;
			set => cbIgnore.IsChecked = value;
		}

		public void Validate() { }
		public void ShowItem(object item) { }
		#endregion
	}
}
