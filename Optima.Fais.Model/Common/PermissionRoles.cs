using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class PermissionRole : Entity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int? PermissionId { get; set; }
        public virtual Permission Permission { get; set; }
        public int? PermissionTypeId { get; set; }
        public virtual PermissionType PermissionType { get; set; }
        public string RoleId { get; set; }
        public virtual ApplicationRole Role { get; set; }
        public int? RouteId { get; set; }
        public virtual Route Route { get; set; }

        public bool Active { get; set; }

        public Guid Guid { get; set; }

    }
}
