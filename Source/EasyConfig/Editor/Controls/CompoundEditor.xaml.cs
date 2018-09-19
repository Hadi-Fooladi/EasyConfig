using System;
using System.Xml;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using System.Collections.Generic;

namespace EasyConfig.Editor
{
	internal partial class CompoundEditor : IEditor
	{
		private readonly Type T;

		#region Constructors
		public CompoundEditor() => InitializeComponent();

		public CompoundEditor(Type T, object Value) : this()
		{
			this.T = T;

			if (Value == null)
				Delete();
			else New(Value);
		}

		public CompoundEditor(Type T, XmlNode Node) : this()
		{
			this.T = T;

			if (Node == null)
				Delete();
			else
			{
				New();
				PopulateFields(Node);
			}
		}
		#endregion

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

		#region Private Methods
		private void Delete()
		{
			bNewDel.Content = "New";
			LB.Visibility = Visibility.Hidden;
			FieldEditorContainer.Content = null;
		}

		private void New()
		{
			bNewDel.Content = "Delete";
			LB.Visibility = Visibility.Visible;
		}

		private void New(object Value)
		{
			New();
			PopulateFields(Value);
		}

		private void PopulateFields(object Value)
		{
			var Items = LB.Items;

			Items.Clear();
			foreach (var F in T.GetFields(PUBLIC_INSTANCE_FLAG))
				Items.Add(new FieldItem(F, F.GetValue(Value)));
		}

		private void PopulateFields(XmlNode Node)
		{
			var Items = LB.Items;

			Items.Clear();
			foreach (var F in T.GetFields(PUBLIC_INSTANCE_FLAG))
				Items.Add(new FieldItem(F, Node));
		}
		#endregion

		#region IEditor Members
		public Control Control => this;

		public object Value
		{
			get
			{
				if (LB.Visibility != Visibility.Visible) return null;

				var Result = Activator.CreateInstance(T);

				foreach (FieldItem FI in LB.Items)
					FI.Save(Result);

				return Result;
			}
		}

		public void Validate()
		{
			foreach (FieldItem FI in LB.Items)
				try
				{
					FI.Editor.Validate();
				}
				catch (Exception E)
				{
					throw new ValidationException(this, FI, E);
				}
		}

		public void ShowItem(object Item) => LB.SelectedItem = Item;
		#endregion

		#region Nested Class
		private class FieldItem
		{
			private readonly FieldInfo FI;

			public readonly IEditor Editor;

			public FieldItem(FieldInfo FI, object Value)
			{
				this.FI = FI;

				var Type = FI.FieldType;

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

			public FieldItem(FieldInfo FI, XmlNode Node)
			{
				this.FI = FI;

				var Type = FI.FieldType;
				var Name = FI.GetConfigName();

				if (Type.IsEnum)
				{
					Editor = new EnumEditor(Type, Node.Attributes[Name]);
					return;
				}

				var AttributeType = AttributeMap.GetValueOrNull(Type);
				if (AttributeType != null)
				{
					Editor = new PrimitiveEditor(AttributeType, Node.Attributes[Name]);
					return;
				}

				if (Type.IsCollection())
					Editor = new CollectionEditor(Type, Node.SelectNodes(Name));
				else
					Editor = new CompoundEditor(Type, Node.SelectSingleNode(Name));
			}

			public override string ToString() => FI.Name;

			public void Save(object Obj) => FI.SetValue(Obj, Editor.Value);
		}
		#endregion

		#region Event Handlers
		private void bNewDel_OnClick(object sender, RoutedEventArgs e)
		{
			if (LB.Visibility == Visibility.Visible)
				Delete();
			else
				New(Activator.CreateInstance(T));
		}

		private void LB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var FI = LB.SelectedItem as FieldItem;
			FieldEditorContainer.Content = FI?.Editor.Control;
		}
		#endregion
	}
}
