using Microsoft.VisualStudio.TestTools.UnitTesting;

using EasyConfig.Attributes;
using EasyConfig.Exceptions;

namespace Tests
{
	[TestClass]
	public class AllFieldsNecessaryAndOptionalAttributeTests
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

		[TestMethod]
		public void LoadWithMissingOptionalField()
		{
			const string Xml = "<Config Normal='Test' />";

			var C = Load(Xml);

			Assert.AreEqual(C.Normal, "Test");
			Assert.AreEqual(C.Optional, null);
		}
		private static Config Load(string Xml) => Global.ecBasic.Load<Config>(Xml.ToXmlDocument());

		[AllFieldsNecessary]
		private class Config
		{
			public string Normal;

			[Optional]
			public string Optional;
		}
	}
}
