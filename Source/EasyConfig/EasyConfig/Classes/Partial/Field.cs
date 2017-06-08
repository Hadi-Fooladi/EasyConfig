using System.IO;

namespace EasyConfig
{
	internal partial class Field
	{
		public override void Declare(StreamWriter SW) => Declare(SW, Type, Multiple, ReadOnly);
	}
}
