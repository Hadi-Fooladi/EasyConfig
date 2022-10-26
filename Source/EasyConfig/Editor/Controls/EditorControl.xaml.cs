using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

namespace EasyConfig.Editor
{
	partial class EditorControl
	{
		public EditorControl()
		{
			InitializeComponent();
			_grid.ColumnDefinitions.Clear();
		}

		private IEditor _editor;
		private readonly List<IEditor> _editors = new List<IEditor>();

		public object Value
		{
			get => _editor.Value;
			set
			{
				// Clear all editors
				RemoveEditorsAfter(-1);

				Add(_editor = value.GetType().CreateEditor(value));
				AddSelectedItemEditors(_editor);
			}
		}

		public bool IsValid(bool showError)
		{
			try
			{
				_editor.Validate();
				return true;
			}
			catch (ValidationException ve)
			{
				if (!showError) return false;

				for (; ; )
				{
					ve.ShowItemInEditor();

					var ie = ve.InnerException;
					if (ie is ValidationException ex)
					{
						ve = ex;
						continue;
					}

					Msg.Error(ie.Message);
					break;
				}
			}

			return false;
		}

		public void Validate()
		{
			try
			{
				_editor.Validate();
				Msg.Info("Validation Succeeded");
			}
			catch (ValidationException ve)
			{
				for (; ; )
				{
					ve.ShowItemInEditor();

					var e = ve.InnerException;
					if (e is ValidationException ex)
						ve = ex;
					else
					{
						Msg.Error(e.Message);
						break;
					}
				}
			}
		}

		private void Add(IEditor editor)
		{
			_editors.Add(editor);

			var cdc = _grid.ColumnDefinitions;

			var n = cdc.Count;
			if (n > 0)
				cdc.RemoveAt(--n);

			addColumn(editor.RequestedWidth ?? 200, editor.Control);
			addColumn(6, newSplitter());
			cdc.Add(new ColumnDefinition { Width = GridLength.Auto });

			editor.SelectedItemChanged += Editor_SelectedItemChanged;

			void addColumn(double width, UIElement element)
			{
				cdc.Add(new ColumnDefinition { Width = new GridLength(width) });
				_grid.Children.Add(element);
				Grid.SetColumn(element, n++);
			}

			GridSplitter newSplitter() => new GridSplitter
			{
				Width = 2,
				Background = Brushes.Black,
				Margin = new Thickness(0, 10, 0, 10),
				HorizontalAlignment = HorizontalAlignment.Center
			};
		}

		private void RemoveEditorsAfter(int ndx)
		{
			ndx++;
			int n = _editors.Count;

			for (int i = ndx; i < n; i++)
				_editors[i].SelectedItemChanged -= Editor_SelectedItemChanged;
			
			if (ndx < n)
				_editors.RemoveRange(ndx, n - ndx);

			var cdc = _grid.ColumnDefinitions;
			n = cdc.Count - 1;
			ndx *= 2;
			if (ndx < n)
			{
				var count = n - ndx;
				cdc.RemoveRange(ndx, count);
				_grid.Children.RemoveRange(ndx, count);
			}
		}

		private void AddSelectedItemEditors(IEditor editor)
		{
			for (; ; )
			{
				editor = editor.SelectedItemEditor;
				if (editor == null) break;

				Add(editor);
			}
		}

		private void Editor_SelectedItemChanged(object sender, EventArgs e)
		{
			var editor = (IEditor)sender;
			int ndx = _editors.IndexOf(editor);
			if (ndx == -1) return;

			RemoveEditorsAfter(ndx);
			AddSelectedItemEditors(editor);
		}
	}
}
