using System;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Org.XmlUnit.Builder;
using Org.XmlUnit.Diff;

namespace Tests
{
	[TestClass]
	public class PrimitivesTests
	{
		private const string XML = "<Config S='Test' Num='-123' L='170265246306366' F='-14.5' D='7.1234567' ch='C' B='Yes' Ver='9.1' />";

		private static readonly XmlDocument Doc = XML.ToXmlDocument();

		[TestMethod]
		public void Load()
		{
			var C = Global.ecBasic.Load<Config>(Doc);

			Assert.AreEqual(C.S, "Test");
			Assert.AreEqual(C.Num, -123);
			Assert.AreEqual(C.L, 170265246306366);
			Assert.AreEqual(C.F, -14.5f);
			Assert.AreEqual(C.D, 7.1234567);
			Assert.AreEqual(C.ch, 'C');
			Assert.AreEqual(C.B, true);
			Assert.AreEqual(C.Ver.Major, 9);
			Assert.AreEqual(C.Ver.Minor, 1);
		}

		[TestMethod]
		public void Save()
		{
			var C = new Config
			{
				S = "Test",
				Num = -123,
				L = 170265246306366,
				F = -14.5f,
				D = 7.1234567,
				ch = 'C',
				B = true,
				Ver = new Version(9, 1)
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
			public string S;

			public int Num;

			public long L;

			public float F;

			public double D;

			public char ch;

			public bool B;

			public Version Ver;
		}
	}

}
