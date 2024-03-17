using Optima.Fais.Model;
using System;

namespace Optima.Fais.Dto
{
    public class ColumnDefinitionRole
    {
        public int Id { get; set; }
        public CodeNameEntity ColumnDefinition { get; set; }
        public RoleEntity Role { get; set; }

    }
}
