using Microsoft.VisualStudio.TestTools.UnitTesting;

using Org.XmlUnit.Diff;
using Org.XmlUnit.Builder;

namespace Tests
{
	[TestClass]
	public class EmptyConfigTests
	{
		[TestMethod]
		public void Save()
		{
			var C = new Config();
			var Doc = Global.ecBasic.GetXmlDocument(C);

			var D = DiffBuilder
			   .Compare(Input.FromString("<Config />"))
			   .WithTest(Input.FromDocument(Doc))
			   .CheckForIdentical()
			   .WithComparisonController(ComparisonControllers.StopWhenDifferent)
			   .Build();

			Assert.IsFalse(D.HasDifferences());
		}

		[TestMethod]
		public void SaveWithDifferentRootTagName()
		{
			var EC = new EasyConfig.EasyConfig { RootTagName = "Test" };
			var Doc = EC.GetXmlDocument(new Config());

			var D = DiffBuilder
			   .Compare(Input.FromString("<Test />"))
			   .WithTest(Input.FromDocument(Doc))
			   .CheckForIdentical()
			   .WithComparisonController(ComparisonControllers.StopWhenDifferent)
			   .Build();

			Assert.IsFalse(D.HasDifferences());
		}

		[TestMethod]
		public void Load()
		{
			var Doc = "<Config />".ToXmlDocument();
			var C = Global.ecBasic.Load<Config>(Doc);

			Assert.IsNotNull(C);
		}

		/// <summary>
		/// Empty Config Class
		/// </summary>
		private class Config
		{

		}
	}
}
