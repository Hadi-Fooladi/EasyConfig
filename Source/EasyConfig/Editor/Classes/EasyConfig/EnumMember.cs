namespace EasyConfig
{
	internal partial class EnumMember
	{
		public EnumMember(string S) : base(GetMemberName(S))
		{
			int ndx = S.IndexOf('=');
			if (ndx != -1)
				Value = int.Parse(S.Substring(ndx + 1));
		}

		private static string GetMemberName(string S)
		{
			int ndx = S.IndexOf('=');
			return ndx == -1 ? S : S.Substring(0, ndx);
		}
	}
}
