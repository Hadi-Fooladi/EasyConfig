using System.Windows;
using System.Collections.Generic;

namespace Editor
{
	internal partial class SearchWindow
	{
		private SearchWindow()
		{
			InitializeComponent();

			Closing += (s, e) =>
			{
				Hide();
				e.Cancel = true;
			};
		}

		private static SearchWindow ins;
		public static SearchWindow Instance => ins ?? (ins = new SearchWindow());

		#region Event Handlers
		private void TB_SelectionChanged(object sender, RoutedEventArgs e)
		{
			string Word = TB.Text;
			if (string.IsNullOrEmpty(Word))
			{
				DG.ItemsSource = null;
				return;
			}

			var Results = SearchEngine.Search(MainWindow.Instance.Root, Word);

			var Items = new List<Item>();
			foreach (var Result in Results)
				Items.Add(new Item(Result));

			DG.ItemsSource = Items;
		}

		private void DG_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			var Item = DG.SelectedItem as Item;
			if (Item == null) return;

			MainWindow.Instance.Reveal(Item.Result);

		}
		#endregion

		private class Item
		{
			public string Name { get; }
			public string Path { get; }

			public readonly SearchEngine.Result Result;

			public Item(SearchEngine.Result R)
			{
				Result = R;
				if (R.AV == null)
				{
					Name = $"<{R.Node.Name}>";
					Path = R.Node.Container?.Path;
				}
				else
				{
					Name = R.AV.Name;
					Path = R.Node.Path;
				}
			}
		}
	}
}
