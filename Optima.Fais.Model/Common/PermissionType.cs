using System;

namespace Optima.Fais.Model
{
    public class PermissionType : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public Guid Guid { get; set; }
    }
}
