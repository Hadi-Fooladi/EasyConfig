using Microsoft.VisualStudio.TestTools.UnitTesting;

using EasyConfig.Attributes;

namespace Tests
{
	[TestClass]
	public class DefaultAttributeTests
	{
		[TestMethod]
		public void Load()
		{
			var C = Global.ecBasic.Load<Config>("<Config />".ToXmlDocument());

			Assert.AreEqual(C.Value, 5);
		}

		private class Config
		{
			[Default(5)]
			public int Value;
		}
	}
}
