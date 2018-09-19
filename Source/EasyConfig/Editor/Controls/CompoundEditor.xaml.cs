﻿using System;
using System.Xml;
using System.Windows;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

using EasyConfig.Attributes;

namespace EasyConfig.Editor
{
	internal partial class CompoundEditor : IEditor
	{
		private readonly Type T;
		private readonly bool AllFieldsNecessary;

		#region Constructors
		public CompoundEditor(Type T)
		{
			this.T = T;
			InitializeComponent();
			AllFieldsNecessary = T.HasAttribute<AllFieldsNecessaryAttribute>();
		}

		public CompoundEditor(Type T, object Value) : this(T)
		{
			if (Value == null)
				Delete();
			else New(Value);
		}

		public CompoundEditor(Type T, XmlNode Node) : this(T)
		{
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
				Items.Add(new FieldItem(F, F.GetValue(Value), AllFieldsNecessary));
		}

		private void PopulateFields(XmlNode Node)
		{
			var Items = LB.Items;

			Items.Clear();
			foreach (var F in T.GetFields(PUBLIC_INSTANCE_FLAG))
				Items.Add(new FieldItem(F, Node, AllFieldsNecessary));
		}
		#endregion

		#region IEditor Members
		public Control Control => this;

		public object Value
		{
			get
			{
				if (Ignored) return null;

				var Result = Activator.CreateInstance(T);

				foreach (FieldItem FI in LB.Items)
					FI.Save(Result);

				return Result;
			}
		}

		public bool Ignored => LB.Visibility != Visibility.Visible;

		public void Validate()
		{
			foreach (FieldItem FI in LB.Items)
				try
				{
					var E = FI.Editor;

					if (FI.Necessary && E.Ignored)
						throw new NecessaryFieldIgnoredException();

					E.Validate();
				}
				catch (Exception Ex)
				{
					throw new ValidationException(this, FI, Ex);
				}
		}

		public void ShowItem(object Item) => LB.SelectedItem = Item;
		#endregion

		#region Nested Class
		private class FieldItem
		{
			private readonly FieldInfo FI;

			public readonly bool Necessary;
			public readonly IEditor Editor;

			public string Name => FI.Name;
			public Brush Color => Necessary ? Brushes.DarkRed : Brushes.Black;
			public FontWeight FontWeight => Necessary ? FontWeights.SemiBold : FontWeights.Normal;

			private object Default => FI.GetCustomAttribute<DefaultAttribute>()?.Value;

			#region Constructors
			private FieldItem(FieldInfo FI, bool Necessary)
			{
				this.FI = FI;
				this.Necessary = FI.IsNecessary(Necessary);
			}

			public FieldItem(FieldInfo FI, object Value, bool Necessary) : this(FI, Necessary)
			{
				var Type = FI.FieldType;

				if (Type.IsEnum)
				{
					Editor = new EnumEditor(Value, Default);
					return;
				}

				var AttributeType = AttributeMap.GetValueOrNull(Type);
				if (AttributeType != null)
				{
					Editor = new PrimitiveEditor(Value, AttributeType, Default);
					return;
				}

				if (Type.IsCollection())
					Editor = new CollectionEditor(Type, Value);
				else
					Editor = new CompoundEditor(Type, Value);
			}

			public FieldItem(FieldInfo FI, XmlNode Node, bool Necessary) : this(FI, Necessary)
			{
				var Type = FI.FieldType;
				var ConfigName = FI.GetConfigName();

				if (Type.IsEnum)
				{
					Editor = new EnumEditor(Type, Node.Attributes[ConfigName], Default);
					return;
				}

				var AttributeType = AttributeMap.GetValueOrNull(Type);
				if (AttributeType != null)
				{
					Editor = new PrimitiveEditor(AttributeType, Node.Attributes[ConfigName], Default);
					return;
				}

				if (Type.IsCollection())
					Editor = new CollectionEditor(Type, Node.SelectNodes(ConfigName));
				else
					Editor = new CompoundEditor(Type, Node.SelectSingleNode(ConfigName));
			}
			#endregion

			public void Save(object Obj)
			{
				if (!Editor.Ignored)
					FI.SetValue(Obj, Editor.Value);
				else
				{
					var D = Default;
					if (D != null)
						FI.SetValue(Obj, D);
				}
			}
		}
		#endregion

		#region Event Handlers
		private void bNewDel_OnClick(object sender, RoutedEventArgs e)
		{
			if (Ignored)
				New(Activator.CreateInstance(T));
			else
				Delete();
		}

		private void LB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var FI = LB.SelectedItem as FieldItem;
			FieldEditorContainer.Content = FI?.Editor.Control;
		}
		#endregion
	}
}
