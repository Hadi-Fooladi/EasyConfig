using Editor;

namespace EasyConfig
{
	internal partial class Field
	{
		public string Tag => TagName ?? Name;

		public DataType DataType => Global.DataTypeMap[Type];
	}
}
