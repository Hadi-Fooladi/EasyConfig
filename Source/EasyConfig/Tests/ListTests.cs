using System;
using System.Xml;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EasyConfig.Attributes;

using Org.XmlUnit.Diff;
using Org.XmlUnit.Builder;

namespace Tests
{
	[TestClass]
	public class ListTests
	{
		private const string XML = "<Config><Item Value='A' /><Item Value='B' /><Item Value='C' /></Config>";

		private static readonly XmlDocument Doc = XML.ToXmlDocument();

		[TestMethod]
		public void Load()
		{
			var C = Global.ecBasic.Load<Config>(Doc);

			Assert.AreEqual(C.L.Count, 3);
			Assert.AreEqual(C.L[0].Value, "A");
			Assert.AreEqual(C.L[1].Value, "B");
			Assert.AreEqual(C.L[2].Value, "C");
		}

		[TestMethod]
		public void Save()
		{
			var C = new Config
			{
				L = new List<Item>
				{
					new Item { Value = "A" },
					new Item { Value = "B" },
					new Item { Value = "C" }
				}
			};

			var D = DiffBuilder
			   .Compare(Input.FromDocument(Doc))
			   .WithTest(Input.FromDocument(Global.ecBasic.GetXmlDocument(C)))
			   .CheckForIdentical()
			   .WithComparisonController(ComparisonControllers.StopWhenDifferent)
			   .Build();

			Assert.IsFalse(D.HasDifferences());
		}

		private class Config
		{
			[Name("Item")]
			public List<Item> L;
		}

		private class Item
		{
			public string Value;
		}
	}
}
