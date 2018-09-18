using System.Windows.Controls;

namespace EasyConfig.Editor
{
	internal interface IEditor
	{
		Control Control { get; }

		object Value { get; }

		/// <summary>
		/// Must throw <see cref="ValidationException"/> in case of validation error
		/// </summary>
		void Validate();

		/// <summary>
		/// Used in conjunction with <see cref="Validate"/> to show the validation error
		/// </summary>
		void ShowItem(object Item);
	}
}
