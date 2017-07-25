using System.Text;
using System.Collections.Generic;

namespace EasyConfig
{
	internal partial class Enum
	{
		private static readonly string DELIMITER = new string('-', 20);

		private List<EnumMember> m_MembersArray;
		public List<EnumMember> MembersArray
		{
			get
			{
				if (m_MembersArray == null)
				{
					var L = new List<EnumMember>();

					if (Members != null)
						foreach (var Member in Members.Replace(" ", "").Split(','))
							L.Add(new EnumMember(Member));

					L.AddRange(MembersList);
					m_MembersArray = L;
				}
				return m_MembersArray;
			}
		}

		private string m_Description;
		public new string Description
		{
			get
			{
				if (m_Description == null)
				{
					var SB = new StringBuilder();
					SB.AppendLine();
					SB.AppendLine(DELIMITER);
					SB.AppendLine("Expected Values:");
					foreach (var M in MembersArray)
					{
						SB.Append($"   + {M.Name}");
						if (M.Description == null) SB.AppendLine();
						else SB.AppendLine($": {M.Description}");
					}
					SB.Append(DELIMITER);

					m_Description = SB.ToString();
				}

				return m_Description;
			}
		}
	}
}
