using System.Windows;
using System.Collections.ObjectModel;

namespace Editor
{
	internal partial class ChangeLogWindow
	{
		private readonly ObservableCollection<Item> AllChanges = new ObservableCollection<Item>();

		public ChangeLogWindow(TreeNode Root)
		{
			InitializeComponent();

			FindChanges(Root);
			DG.ItemsSource = AllChanges;

			cbNextTime.IsChecked = !ShowNextTime;

			// Event Handlers
			bOK.Click += (s, e) => { DialogResult = true; Close(); };
			bCancel.Click += (s, e) => { DialogResult = false; Close(); };

			cbNextTime.Checked += (s, e) => ShowNextTime = false;
			cbNextTime.Unchecked += (s, e) => ShowNextTime = true;

			// Local Functions
			void FindChanges(TreeNode Node)
			{
				foreach (var A in Node.Attributes)
					if (A.Changed)
						AllChanges.Add(new Item(A) { Path = Node.Path });

				foreach (var N in Node.Nodes) FindChanges(N);
			}
		}

		public bool ShowNextTime
		{
			get => Properties.Settings.Default.ChangeLog_ShowNextTime;
			set
			{
				var D = Properties.Settings.Default;
				D.ChangeLog_ShowNextTime = value;
				D.Save();
			}
		}

		#region Nested Classes
		private class Item
		{
			private readonly AttributeValue AV;

			public string Path { get; set; }
			public string Attribute { get; }

			public string Before { get; }
			public string After { get; }

			public FontWeight BeforeFontWeight { get; }
			public FontWeight AfterFontWeight { get; }

			public Item(AttributeValue AV)
			{
				this.AV = AV;
				Attribute = AV.Name;

				After = AV.CurValue;
				Before = AV.PrevValue;

				AfterFontWeight = AV.CurDefault ? FontWeights.Bold : FontWeights.Normal;
				BeforeFontWeight = AV.PrevDefault ? FontWeights.Bold : FontWeights.Normal;
			}

			public void Revert() => AV.RevertChanges();
		}
		#endregion

		#region Event Handlers
		private void bDelete_OnClick(object sender, RoutedEventArgs e)
		{
			while (DG.SelectedItem is Item X)
			{
				X.Revert();
				AllChanges.Remove(X);
			}
		}
		#endregion
	}
}
