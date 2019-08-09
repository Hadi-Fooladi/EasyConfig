using Microsoft.VisualStudio.TestTools.UnitTesting;

using EasyConfig.Attributes;
using EasyConfig.Exceptions;

namespace Tests
{
	[TestClass]
	public class NecessaryAttributeTests
	{
		[TestMethod]
		public void NecessaryFieldNotFoundException()
		{
			const string Xml = "<Config />";

			var E = Assert.ThrowsException<LoadFailedException>(() => Load(Xml));

			Assert.IsInstanceOfType(E.GetInnerMost(), typeof(NecessaryFieldNotFoundException));
		}

		[TestMethod]
		public void NecessaryFieldIsNullException()
		{
			var C = new Config();

			var E = Assert.ThrowsException<SaveFailedException>(() => Global.ecBasic.GetXmlDocument(C));

			Assert.IsInstanceOfType(E.GetInnerMost(), typeof(NecessaryFieldIsNullException));
		}

		private static Config Load(string Xml) => Global.ecBasic.Load<Config>(Xml.ToXmlDocument());

		private class Config
		{
			[Necessary]
			public string Value;
		}
	}
}
