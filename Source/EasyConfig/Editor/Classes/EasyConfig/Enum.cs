namespace EasyConfig
{
	internal partial class Enum
	{
		private string[] m_MembersArray;
		public string[] MembersArray => m_MembersArray ?? (m_MembersArray = Members.Replace(" ", "").Split(','));
	}
}
