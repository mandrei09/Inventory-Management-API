using Optima.Fais.Model;
using System;

namespace Optima.Fais.Dto
{
    public class RouteChildrenRole
    {
        public int Id { get; set; }
        public CodeNameEntity RouteChildren { get; set; }
        public RoleEntity Role { get; set; }

    }
}
