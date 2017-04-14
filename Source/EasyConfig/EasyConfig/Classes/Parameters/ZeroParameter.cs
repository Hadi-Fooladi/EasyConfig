using System;

namespace EasyConfig
{
	internal class ZeroParameter : IParameter
	{
		public Action Act;

		#region IParameter
		public string Code { get; set; }
		public string Desc { get; set; }
		public string CodeParams => "";
		public void Process(string[] args, ref int ndx) => Act();
		#endregion
	}
}
