using Optima.Fais.Model;
using System;

namespace Optima.Fais.Dto
{
    public class Permission
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public CodeNameEntity PermissionType { get; set; }

    }
}
