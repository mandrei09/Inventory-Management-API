using Optima.Fais.Model;
using System;

namespace Optima.Fais.Dto
{
    public class PermissionRole
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public CodeNameEntity Permission { get; set; }
        public CodeNameEntity PermissionType { get; set; }
        public RoleEntity Role { get; set; }

    }
}
