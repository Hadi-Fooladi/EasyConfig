using System;
using System.Xml;
using System.Reflection;

namespace EasyConfig.Exceptions
{
	public class NecessaryFieldNotFoundException : Exception
	{
		public NecessaryFieldNotFoundException() : base("Necessary field not found") { }
	}

	public class NecessaryFieldIsNullException : Exception
	{
		public NecessaryFieldIsNullException() : base("Necessary field is null") { }
	}

	public class LoadFailedException : Exception
	{
		public readonly XmlNode Tag;
		public readonly MemberInfo Member;

		public LoadFailedException(XmlNode Tag, MemberInfo Member, Exception Inner) : base("Load failed", Inner)
		{
			this.Tag = Tag;
			this.Member = Member;
		}
	}

	public class SaveFailedException : Exception
	{
		public readonly XmlNode Tag;
		public readonly MemberInfo Member;

		public SaveFailedException(XmlNode Tag, MemberInfo Member, Exception Inner) : base("Save failed", Inner)
		{
			this.Tag = Tag;
			this.Member = Member;
		}
	}
}
