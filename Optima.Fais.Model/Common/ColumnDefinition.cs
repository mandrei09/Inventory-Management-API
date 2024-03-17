using System;

namespace Optima.Fais.Model
{
    public class ColumnDefinition : Entity
    {
        public int TableDefinitionId { get; set; }
        public virtual TableDefinition TableDefinition { get; set; }

        public string HeaderCode { get; set; }
        public string Property { get; set; }
        public string Include { get; set; }
        public string SortBy { get; set; }
        public string Pipe { get; set; }
        public string Format { get; set; }
        public string TextAlign { get; set; }
        public bool Active { get; set; }
        public int Position { get; set; }

        public string RoleId { get; set; }
		public ApplicationRole AspNetRole { get; set; }

		public int? ColumnFilterId { get; set; }
		public virtual ColumnFilter ColumnFilter { get; set; }

		public Guid Guid { get; set; }
	}
}
