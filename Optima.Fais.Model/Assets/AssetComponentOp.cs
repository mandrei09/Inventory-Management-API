using System;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class AssetComponentOp : Entity
    {
        public int AssetComponentId { get; set; }

        public virtual AssetComponent AssetComponent { get; set; }

        public int DocumentId { get; set; }

        public virtual Document Document { get; set; }

        public int? EmployeeIdInitial { get; set; }

        public virtual Employee EmployeeInitial { get; set; }

        public int? EmployeeIdFinal { get; set; }

        public virtual Employee EmployeeFinal { get; set; }

        public float Quantity { get; set; }

        public int? InvStateId { get; set; }

        public virtual InvState InvState { get; set; }


    }
}
