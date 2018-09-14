using System;
using System.Windows;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Controls;

namespace EasyConfig.Editor
{
	internal partial class CompoundEditor : IEditor
	{
		public CompoundEditor(Type T, object Value)
		{
			this.T = T;
			InitializeComponent();

			if (Value == null)
			{
				bNewDel.Content = "New";
				LB.Visibility = Visibility.Hidden;
				return;
			}

			if (T.IsValueType)
				bNewDel.Visibility = Visibility.Collapsed;
			else
				bNewDel.Content = "Delete";

			PopulateFields(Value);
		}

		#region Constants
		private const BindingFlags PUBLIC_INSTANCE_FLAG = BindingFlags.Instance | BindingFlags.Public;

		private static readonly IReadOnlyDictionary<Type, IAttributeType> AttributeMap = new Dictionary<Type, IAttributeType>
		{
			{ typeof(int), new IntAttr() },
			{ typeof(bool), new BoolAttr() },
			{ typeof(char), new CharAttr() },
			{ typeof(float), new SingleAttr() },
			{ typeof(double), new DoubleAttr() },
			{ typeof(string), new StringAttr() },
			{ typeof(Version), new VersionAttr() }
		};
		#endregion

		private readonly Type T;

		private void PopulateFields(object Value)
		{
			var Items = LB.Items;

			Items.Clear();
			foreach (var F in T.GetFields(PUBLIC_INSTANCE_FLAG))
				Items.Add(new FieldItem(F, F.GetValue(Value)));
		}

		public Control Control => this;

		#region Nested Class
		private class FieldItem
		{
			private readonly Type Type;
			private readonly FieldInfo FI;

			public readonly IEditor Editor;

			public FieldItem(FieldInfo FI, object Value)
			{
				this.FI = FI;
				Type = FI.FieldType;

				if (Type.IsEnum)
				{
					Editor = new EnumEditor(Value);
					return;
				}

				var AttributeType = AttributeMap.GetValueOrNull(Type);
				if (AttributeType != null)
				{
					Editor = new PrimitiveEditor(Value, AttributeType);
					return;
				}

				if (Type.IsCollection())
					Editor = new CollectionEditor(Type, Value);
				else
					Editor = new CompoundEditor(Type, Value);
			}

			public override string ToString() => FI.Name;
		}
		#endregion

		#region Event Handlers
		private void bNewDel_OnClick(object sender, RoutedEventArgs e)
		{
			if (LB.Visibility == Visibility.Visible)
			{
				LB.Items.Clear();
				LB.Visibility = Visibility.Hidden;
				bNewDel.Content = "New";
				FieldEditorContainer.Content = null;
			}
			else
			{
				bNewDel.Content = "Delete";
				LB.Visibility = Visibility.Visible;
				PopulateFields(Activator.CreateInstance(T));
			}
		}

		private void LB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var FI = LB.SelectedItem as FieldItem;
			FieldEditorContainer.Content = FI?.Editor.Control;
		}
		#endregion
	}
}
