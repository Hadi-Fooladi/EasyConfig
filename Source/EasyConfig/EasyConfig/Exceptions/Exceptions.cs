using System;

namespace EasyConfig.Exceptions
{
	public class NecessaryFieldNotFoundException : Exception
	{
		private const string MSG = "Necessary field not found";

		public NecessaryFieldNotFoundException() : base(MSG) { }
		//public NecessaryFieldNotFoundException(Exception InnerException) : base(MSG, InnerException) { }
	}

	public class VersionMismatchException : Exception
	{
		public VersionMismatchException() : base("Version Mismatch") { }
	}
}
