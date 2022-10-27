using System;
using System.Collections.Generic;

namespace Editor_Test
{
	internal class LinkedList
	{
		public string Text { get; set; } = "";

		public List<ListItem> List;

		public override string ToString() => Text;
	}
}
