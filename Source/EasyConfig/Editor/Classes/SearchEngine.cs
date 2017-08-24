using System.Collections.Generic;

namespace Editor
{
	internal class SearchEngine
	{
		public static List<Result> Search(TreeNode Node, string Word)
		{
			var L = new List<Result>();
			SearchNode(Node);
			return L;

			void SearchNode(TreeNode TN)
			{
				if (TN.Path.icContains(Word))
					L.Add(new Result(TN));

				foreach (var A in TN.Attributes)
					if (A.Name.icContains(Word))
						L.Add(new Result(TN, A));

				foreach (var N in TN.Nodes)
					SearchNode(N);
			}
		}

		#region Nested Classes
		public class Result
		{
			public readonly TreeNode Node;
			public readonly AttributeValue AV;

			public Result(TreeNode Node, AttributeValue AV = null)
			{
				this.AV = AV;
				this.Node = Node;
			}

			public override string ToString() => $"{Node.Path}/{AV?.Name}";
		}
		#endregion
	}
}
