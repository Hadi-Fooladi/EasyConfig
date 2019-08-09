using Microsoft.VisualStudio.TestTools.UnitTesting;

using Org.XmlUnit.Diff;
using Org.XmlUnit.Builder;

using EasyConfig.Attributes;

namespace Tests
{
	[TestClass]
	public class NameAttributeTests
	{
		[TestMethod]
		public void Save()
		{
			var C = new Config
			{
				Value = 7
			};

			var Doc = Global.ecBasic.GetXmlDocument(C);

			var D = DiffBuilder
			   .Compare(Input.FromString("<Config Number='7' />"))
			   .WithTest(Input.FromDocument(Doc))
			   .CheckForIdentical()
			   .WithComparisonController(ComparisonControllers.StopWhenDifferent)
			   .Build();

			Assert.IsFalse(D.HasDifferences());
		}

		[TestMethod]
		public void Load()
		{
			var C = Global.ecBasic.Load<Config>("<Config Number='7' />".ToXmlDocument());

			Assert.AreEqual(C.Value, 7);
		}

		private class Config
		{
			[Name("Number")]
			public int Value;
		}
	}
}
