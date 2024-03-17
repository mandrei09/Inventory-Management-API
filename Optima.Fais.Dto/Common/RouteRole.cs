using Optima.Fais.Model;
using System;

namespace Optima.Fais.Dto
{
    public class RouteRole
    {
        public int Id { get; set; }
        public CodeNameEntity Route { get; set; }
        public RoleEntity Role { get; set; }

    }
}
