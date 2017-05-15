using Editor;
using System.Collections.Generic;

namespace EasyConfig
{
	internal partial class DataType
	{
		public IEnumerable<Field> AllFields
		{
			get
			{
				if (Inherit != null)
					foreach (var F in Global.DataTypeMap[Inherit].AllFields)
						yield return F;

				foreach (var F in Fields)
					yield return F;
			}
		}
	}
}
