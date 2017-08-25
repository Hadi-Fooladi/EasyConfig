namespace Editor
{
	internal class ValidationRecord
	{
		public readonly TreeNode Node;
		public readonly string Message;
		public readonly AttributeValue A;

		public ValidationRecord(TreeNode Node, string Message, AttributeValue A = null)
		{
			this.A = A;
			this.Node = Node;
			this.Message = Message;
		}
	}
}
