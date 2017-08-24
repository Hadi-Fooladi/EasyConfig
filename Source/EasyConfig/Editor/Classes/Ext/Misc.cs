using System;
using System.Collections;

namespace Editor
{
	internal static class MiscExt
	{
		public static void Swap(this IList L, int ndx1, int ndx2)
		{
			var Temp = L[ndx1];
			L[ndx1] = L[ndx2];
			L[ndx2] = Temp;
		}

		public static bool isTrue(this bool? B) => B ?? false;
		public static bool isFalse(this bool? B) => !(B ?? true);

		/// <summary>
		/// Contains (Ignore Case)
		/// </summary>
		public static bool icContains(this string S, string V) => S.IndexOf(V, StringComparison.OrdinalIgnoreCase) >= 0;
	}
}
