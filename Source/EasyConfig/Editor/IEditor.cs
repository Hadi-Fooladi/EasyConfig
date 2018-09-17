using System.Windows.Controls;

namespace EasyConfig.Editor
{
	internal interface IEditor
	{
		Control Control { get; }

		object Value { get; }
	}
}
