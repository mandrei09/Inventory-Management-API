using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class TableDefinition : Entity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<ColumnDefinition> ColumnDefinitions { get; set; }

        public Guid Guid { get; set; }
    }
}
