using EasyConfig;
using System.Windows.Controls;

namespace Editor
{
	internal static class Global
	{
		public static string[] args;
		public static Schema Schema;
		public static DataTypeMap DataTypeMap;
		public static readonly ContextMenu CM = new ContextMenu();
	}
}
