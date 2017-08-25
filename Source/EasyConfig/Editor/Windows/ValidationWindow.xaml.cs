using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Editor
{
	internal partial class ValidationWindow
	{
		public ValidationWindow(List<ValidationRecord> Records)
		{
			InitializeComponent();

			Update(Records);
		}

		private void Update(List<ValidationRecord> Records)
		{
			var L = new List<Item>();
			foreach (var R in Records)
				L.Add(new Item(R));

			DG.ItemsSource = L;
		}

		#region Event Handlers
		private void bOK_Click(object sender, RoutedEventArgs e) => Close();

		private void DG_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var Item = DG.SelectedItem as Item;
			if (Item == null) return;

			var R = Item.Record;
			MainWindow.Instance.Reveal(R.Node, R.A);
		}

		private void bRefresh_OnClick(object sender, RoutedEventArgs e)
		{
			var Records = new List<ValidationRecord>();
			MainWindow.Instance.Root.Validate(Records);
			Update(Records);
		}
		#endregion

		#region Nested Class
		private class Item
		{
			public string Path { get; }
			public string Attribute { get; }
			public string Msg { get; }

			public readonly ValidationRecord Record;

			public Item(ValidationRecord Record)
			{
				this.Record = Record;
				Msg = Record.Message;
				Path = Record.Node.Path;
				Attribute = Record.A == null ? "-" : Record.A.Name;
			}
		}
		#endregion
	}
}
