using System;
using System.Xml;

namespace EasyConfig.Editor
{
	public partial class EditorControl
	{
		#region Constructors
		public EditorControl() => InitializeComponent();

		public EditorControl(string XmlPath, Type T) : this(LoadXml(XmlPath), T) { }

		public EditorControl(XmlDocument Doc, Type T) : this()
		{
			var Root = Doc.DocumentElement;

			SV.Content = CE = new CompoundEditor(T, Root);
		}

		public EditorControl(object Obj) : this()
		{
			SV.Content = CE = new CompoundEditor(Obj.GetType(), Obj);
		}

		public static EditorControl New<T>(string XmlPath) => New<T>(LoadXml(XmlPath));
		public static EditorControl New<T>(XmlDocument Doc) => new EditorControl(Doc, typeof(T));
		#endregion

		private readonly CompoundEditor CE;

		public object Value => CE.Value;

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

		private static XmlDocument LoadXml(string Path)
		{
			var Doc = new XmlDocument();
			Doc.Load(Path);
			return Doc;
		}
	}
}
