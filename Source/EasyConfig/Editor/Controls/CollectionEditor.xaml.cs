using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

namespace EasyConfig.Editor
{
	internal partial class CollectionEditor : IEditor
	{
		public CollectionEditor(Type CollectionType, object Value)
		{
			InitializeComponent();

			Type
				ElementType = CollectionType.GetCollectionElementType(),
				ListType = typeof(List<>).MakeGenericType(ElementType);

			L = (IList)Activator.CreateInstance(ListType);
		}

		private readonly IList L;


		public Control Control => this;

	}
}
