using EasyConfig;
using System.Collections.Generic;

namespace Editor
{
	/// <summary>
	/// Used to retrieve data types by name
	/// </summary>
	internal class DataTypeMap
	{
		private readonly Dictionary<string, DataType> Dic = new Dictionary<string, DataType>();

		public DataTypeMap(Schema Schema)
		{
			Add(Schema.Types);
			AddNodeTypes(Schema.Root);

			void Add(IEnumerable<DataType> L)
			{
				foreach (var X in L)
					Dic.Add(X.Name, X);
			}

			void AddNodeTypes(Node N)
			{
				// Add types defined in this node
				Add(N.Types);

				Dic.Add(N.TypeName ?? N.Name + "Data", N);

				// Recursively add nested data types
				foreach (var X in N.Nodes)
					AddNodeTypes(X);
			}
		}

		public DataType this[string Name] => Dic[Name];
	}
}
