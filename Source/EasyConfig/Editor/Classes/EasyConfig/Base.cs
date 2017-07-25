using System;

namespace EasyConfig
{
	internal partial class Base
	{
		public Base(string Name) { this.Name = Name; }

		private string m_Description;

		/// <summary>
		/// Used to convert multi line descriptions into a string
		/// </summary>
		public string Description
		{
			get
			{
				if (m_Description == null)
					m_Description = MultiLineDesc == null ? Desc : string.Join(Environment.NewLine, MultiLineDesc.Lines);
				return m_Description;
			}
		}
	}
}
