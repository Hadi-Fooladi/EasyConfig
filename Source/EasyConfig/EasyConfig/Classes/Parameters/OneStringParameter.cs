namespace EasyConfig
{
	internal class OneStringParameter : IParameter
	{
		public OneStringParameter(string Code, string CodeParams, string Desc)
		{
			this.Code = Code;
			this.Desc = Desc;
			this.CodeParams = CodeParams;
		}

		public string Value { get; private set; }

		#region IParameter
		public string Code { get; }
		public string Desc { get; }
		public string CodeParams { get; }
		public void Process(string[] args, ref int ndx) => Value = args[ndx++];
		#endregion
	}
}
