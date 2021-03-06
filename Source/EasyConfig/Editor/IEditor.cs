﻿using System.Xml;
using System.Windows.Controls;

namespace EasyConfig.Editor
{
	internal interface IEditor
	{
		Control Control { get; }

		object Value { get; }

		bool Ignored { get; }

		/// <summary>
		/// Must throw <see cref="ValidationException"/> in case of validation error
		/// </summary>
		void Validate();

		/// <summary>
		/// Used in conjunction with <see cref="Validate"/> to show the validation error
		/// </summary>
		void ShowItem(object Item);

		/// <summary>
		/// Used to save editor value in an XML tag.
		/// </summary>
		/// <param name="Name">
		/// For primitive/enum values, it determines the name of the attribute.<br />
		/// For collections, it determines the name of the nested tags.<br />
		/// For compound values, it will be ignored.
		/// </param>
		void SaveToXmlNode(XmlNode Node, string Name);
	}
}
