using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class ColumnDefinitionRole : Entity
    {
        public ColumnDefinitionRole()
        {
        }

        public int ColumnDefinitionId { get; set; }

        public virtual ColumnDefinition ColumnDefinition { get; set; }

        public string RoleId { get; set; }

        public virtual ApplicationRole Role { get; set; }

        public Guid Guid { get; set; }

    }
}
