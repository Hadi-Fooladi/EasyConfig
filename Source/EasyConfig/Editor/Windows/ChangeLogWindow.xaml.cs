using System;
using System.Collections.Generic;

namespace Editor
{
	internal partial class ChangeLogWindow
	{
		public ChangeLogWindow(TreeNode Root)
		{
			InitializeComponent();

			var L = new List<Item>();
			FindChanges(Root);
			DG.ItemsSource = L;

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
						L.Add(new Item(A) { Path = Node.Path });

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

		public string Message
		{
			get => lbl.Text;
			set => lbl.Text = value;
		}

		private class Item
		{
			public string Path { get; set; }
			public string Attribute { get; }
			public string Before { get; }
			public string After { get; }

			public Item(AttributeValue AV)
			{
				Attribute = AV.Name;
				After = AV.CurValue;
				Before = AV.PrevValue;
			}
		}
	}
}
