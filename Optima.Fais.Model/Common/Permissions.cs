using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class Permission : Entity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int? PermissionTypeId { get; set; }
        public virtual PermissionType PermissionType { get; set; }

        public bool Active { get; set; }

        public Guid Guid { get; set; }

    }
}
