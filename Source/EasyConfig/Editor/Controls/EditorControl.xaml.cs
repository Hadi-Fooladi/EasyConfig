using System;
using System.Xml;

using XmlExt;

namespace EasyConfig.Editor
{
	public partial class EditorControl
	{
		#region Constructors
		public EditorControl() => InitializeComponent();

		public EditorControl(object obj) : this() => Value = obj;
		public EditorControl(XmlDocument doc, Type t) : this() => Fill(doc, t);
		public EditorControl(string xmlPath, Type t) : this(Fn.LoadXml(xmlPath), t) { }

		public static EditorControl New<T>(string xmlPath) => New<T>(Fn.LoadXml(xmlPath));
		public static EditorControl New<T>(XmlDocument doc) => new EditorControl(doc, typeof(T));
		#endregion

		private CompoundEditor CE;

		#region Public Members
		public object Value
		{
			get => CE.Value;
			set => SV.Content = CE = new CompoundEditor(value.GetType(), value);
		}

		public bool IsValid(bool showError)
		{
			try
			{
				CE.Validate();
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
				CE.Validate();
				Msg.Info("Validation Succeeded");
			}
			catch (ValidationException VE)
			{
				for (;;)
				{
					VE.ShowItemInEditor();

					var E = VE.InnerException;
					if (E is ValidationException Ex)
						VE = Ex;
					else
					{
						Msg.Error(E.Message);
						break;
					}
				}
			}
		}

		public XmlDocument GetXml(string RootTagName)
		{
			if (CE.Ignored) return null;

			var Doc = new XmlDocument();

			var Root = Doc.AppendNode(RootTagName);
			CE.SaveToXmlNode(Root, null);

			return Doc;
		}

		public void Fill<T>(XmlDocument doc) => Fill(doc, typeof(T));
		public void Fill(XmlDocument doc, Type t) => SV.Content = CE = new CompoundEditor(t, doc.DocumentElement);
		#endregion
	}
}
