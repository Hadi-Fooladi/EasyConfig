using Editor;
using System.Collections.Generic;

namespace EasyConfig
{
	internal partial class DataType
	{
		public DataType Base => Inherit == null ? null : Global.DataTypeMap[Inherit];

		public IEnumerable<Field> AllFields
		{
			get
			{
				if (Inherit != null)
					foreach (var F in Base.AllFields)
						yield return F;

				foreach (var F in Fields)
					yield return F;
			}
		}

		public IEnumerable<Attribute> AllAttributes
		{
			get
			{
				if (Inherit != null)
					foreach (var A in Base.AllAttributes)
						yield return A;

				foreach (var A in Attributes)
					yield return A;
			}
		}
	}
}
