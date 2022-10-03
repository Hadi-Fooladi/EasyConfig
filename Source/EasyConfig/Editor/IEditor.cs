namespace EasyConfig.Editor
{
	public interface IEditor
	{
		System.Windows.Controls.Control Control { get; }

		object Value { get; set; }

		bool Ignored { get; set; }

		/// <summary>
		/// Must throw <see cref="ValidationException"/> in case of validation error
		/// </summary>
		void Validate();

		/// <summary>
		/// Used in conjunction with <see cref="Validate"/> to show the validation error
		/// </summary>
		void ShowItem(object item);
	}
}
