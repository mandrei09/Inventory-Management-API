using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class RouteChildrenRole : Entity
    {
        public RouteChildrenRole()
        {
        }

        public int RouteChildrenId { get; set; }

        public virtual RouteChildren RouteChildren { get; set; }

        public string RoleId { get; set; }

        public virtual ApplicationRole Role { get; set; }

        public Guid Guid { get; set; }

    }
}
