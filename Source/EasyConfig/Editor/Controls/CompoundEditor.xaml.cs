using System;
using System.Xml;
using System.Windows;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

using XmlExt;
using EasyConfig.Attributes;

namespace EasyConfig.Editor
{
	internal partial class CompoundEditor : IEditor
	{
		private readonly Type T;

		#region Constructors
		private CompoundEditor(Type T)
		{
			this.T = T;
			InitializeComponent();
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

		private void PopulateFields(object obj)
		{
			var items = LB.Items;

			items.Clear();
			foreach (var mi in T.GetConfigMembers())
				items.Add(new MemberItem(mi, mi.GetValue(obj)));

			LB.SelectedIndex = 0;
		}

		private void PopulateFields(XmlNode node)
		{
			var items = LB.Items;

			items.Clear();
			foreach (var mi in T.GetConfigMembers())
				items.Add(new MemberItem(mi, node));

			LB.SelectedIndex = 0;
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

				foreach (MemberItem FI in LB.Items)
					FI.Save(Result);

				return Result;
			}
		}

		public bool Ignored => LB.Visibility != Visibility.Visible;

		public void Validate()
		{
			foreach (MemberItem FI in LB.Items)
				try
				{
					var E = FI.Editor;

					if (E.Ignored)
					{
						if (FI.Necessary)
							throw new NecessaryFieldIgnoredException();
					}
					else
						E.Validate();
				}
				catch (Exception Ex)
				{
					throw new ValidationException(this, FI, Ex);
				}
		}

		public void ShowItem(object Item) => LB.SelectedItem = Item;

		public void SaveToXmlNode(XmlNode Node, string Name)
		{
			foreach (MemberItem FI in LB.Items)
			{
				var E = FI.Editor;
				if (E.Ignored) continue;

				if (E is CompoundEditor)
				{
					var ChildNode = Node.AppendNode(FI.ConfigName);
					E.SaveToXmlNode(ChildNode, null);
				}
				else
					E.SaveToXmlNode(Node, FI.ConfigName);
			}
		}
		#endregion

		#region Nested Class
		private class MemberItem
		{
			private readonly MemberInfo MI;

			public readonly bool Necessary;
			public readonly IEditor Editor;

			public string Name => MI.Name;
			public string ConfigName => MI.GetConfigName();

			public Brush Color => Necessary ? Brushes.DarkRed : Brushes.Black;
			public FontWeight FontWeight => Necessary ? FontWeights.SemiBold : FontWeights.Normal;

			private object Default => MI.GetCustomAttribute<DefaultAttribute>()?.Value;

			#region Constructors
			private MemberItem(MemberInfo mi) => Necessary = (MI = mi).IsNecessary();

			public MemberItem(MemberInfo mi, object Value) : this(mi)
			{
				var Type = mi.GetMemberType();

				// T? => T
				Type = Nullable.GetUnderlyingType(Type) ?? Type;

				if (Type.IsEnum)
				{
					Editor = new EnumEditor(Type, Value, Default);
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

			public MemberItem(MemberInfo mi, XmlNode Node) : this(mi)
			{
				var Type = mi.GetMemberType();

				// T? => T
				Type = Nullable.GetUnderlyingType(Type) ?? Type;

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
					MI.SetValue(Obj, Editor.Value);
				else
				{
					var D = Default;
					if (D != null)
						MI.SetValue(Obj, D);
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
			var FI = LB.SelectedItem as MemberItem;
			FieldEditorContainer.Content = FI?.Editor.Control;
		}
		#endregion
	}
}
