using System.Collections;
using System.Collections.Generic;

namespace Editor
{
	internal static class MiscExt
	{
		//public static void Swap<T>(this IList<T> L, int ndx1, int ndx2)
		//{
		//	var Temp = L[ndx1];
		//	L[ndx1] = L[ndx2];
		//	L[ndx2] = Temp;
		//}

		public static void Swap(this IList L, int ndx1, int ndx2)
		{
			var Temp = L[ndx1];
			L[ndx1] = L[ndx2];
			L[ndx2] = Temp;
		}
	}
}
