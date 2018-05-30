using System.Collections.Generic;

namespace EasyConfig
{
	internal static class Global
	{
		public static readonly Dictionary<string, DataType> Name2DataType = new Dictionary<string, DataType>();

		public static IndentedStreamWriter SW;

		public static FlagParameter
			NoSaveFlag = new FlagParameter("NoSave", "Do not implement 'Save' method"),
			DefaultConstructorFlag = new FlagParameter("dc", "Implement default constructor");
	}
}
