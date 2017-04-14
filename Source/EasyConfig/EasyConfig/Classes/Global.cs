using System.Collections.Generic;

namespace EasyConfig
{
	internal static class Global
	{
		public static string DefaultAccessModifier = "internal";

		public static readonly Dictionary<string, DataType> Name2DataType = new Dictionary<string, DataType>();
	}
}
