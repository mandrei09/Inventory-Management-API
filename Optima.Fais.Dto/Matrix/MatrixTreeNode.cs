using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class MatrixTreeNode
    {

        public string Label { get; set; }
		//public string Data { get; set; }
		//public string Icon { get; set; }
		//public string ExpandedIcon { get; set; }
		//public string CollapsedIcon { get; set; }
		public List<MatrixTree> Children { get; set; }
		//public bool Leaf { get; set; }
		public bool Expanded { get; set; }
		public string Type { get; set; }
		//public string Parent { get; set; }
		//public bool PartialSelected { get; set; }
		public string StyleClass { get; set; }
		//public bool Draggable { get; set; }
		//public bool Droppable { get; set; }
		//public bool Selectable { get; set; }
		//public string Key { get; set; }


		//data?: T;
		//  children?: TreeNode<T>[];
		//  parent?: TreeNode<T>;
	}

	public class MatrixTree
	{
		public string Label { get; set; }
		public bool Expanded { get; set; }
		public string StyleClass { get; set; }
		public string Level { get; set; }
		public bool Validated { get; set; }
		public bool SkipValidate { get; set; }
		public string Type { get; set; }
		public DateTime? ValidatedDate { get; set; }

		public List<MatrixTree> Children { get; set; }
	}
}
