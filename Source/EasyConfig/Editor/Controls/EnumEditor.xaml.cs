using System;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using XmlExt;

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

		public EnumEditor(Type T, XmlAttribute Attr, object Default) : this(T, Attr == null ? null : Enum.Parse(T, Attr.Value), Default) { }
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

				foreach (ToggleButton X in SP.Children)
				{
					if (X.IsChecked != true) continue;

					var Member = (string)X.Content;
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
		public void ShowItem(object Item) { }

		public void SaveToXmlNode(XmlNode Node, string Name) => Node.AddAttr(Name, Value);

		public void SetValueBy(XmlAttribute attribute)
		{
			throw new NotSupportedException();
		}

		public void SetValueBy(XmlNode containerNode, string name)
		{
			throw new NotSupportedException();
		}
		#endregion
	}
}
