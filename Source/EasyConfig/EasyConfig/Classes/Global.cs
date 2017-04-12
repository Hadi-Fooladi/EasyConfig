using System.Collections.Generic;

namespace EasyConfig
{
	internal static class Global
	{
		/// <summary>
		/// Used to determine default type of nodes<br />
		/// Expected values: class / struct
		/// </summary>
		public static string DefaultType = "class";

		public static readonly HashSet<string> Structures = new HashSet<string>();
		public static readonly Dictionary<string, Node> Name2Node = new Dictionary<string, Node>();
	}
}
