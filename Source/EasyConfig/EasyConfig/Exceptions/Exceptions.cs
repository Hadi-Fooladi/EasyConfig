﻿using System;

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
}