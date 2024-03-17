using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class RouteRole : Entity
    {
        public RouteRole()
        {
        }

        public int RouteId { get; set; }

        public virtual Route Route { get; set; }

        public string RoleId { get; set; }

        public virtual ApplicationRole Role { get; set; }

        public Guid Guid { get; set; }

    }
}
