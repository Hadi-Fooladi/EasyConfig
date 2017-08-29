using System.Windows.Controls;
using System.Collections.Generic;

namespace Editor
{
	internal static class TreeViewItemExt
	{
		public delegate void dlgItem(TreeViewItem Item);

		public static event dlgItem OnItemAdded, OnItemRemoved;

		public static void AddItem(this TreeViewItem TVI, TreeViewItem Item)
		{
			TVI.Items.Add(Item);
			OnItemAdded?.Invoke(Item);
		}

		public static void RemoveItem(this TreeViewItem TVI, TreeViewItem Item)
		{
			TVI.Items.Remove(Item);
			OnItemRemoved?.Invoke(Item);
		}

		/// <summary>
		/// All Items including itself
		/// </summary>
		public static void AddAllItems(this TreeViewItem TVI, ICollection<TreeViewItem> L)
		{
			L.Add(TVI);
			foreach (TreeViewItem X in TVI.Items)
				X.AddAllItems(L);
		}
	}
}
