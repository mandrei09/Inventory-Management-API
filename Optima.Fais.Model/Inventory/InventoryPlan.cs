using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
    public partial class InventoryPlan : Entity
    {
        public InventoryPlan() { }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public int InvCommitteeId { get; set; }
        public InvCommittee InvCommittee { get; set; }
        public int? CostCenterId { get; set; }
        public virtual CostCenter CostCenter { get; set; }
        public int? AdministrationId { get; set; }
        public virtual Administration Administration { get; set; }
        public DateTime? DateStarted { get; set; }
        public DateTime? DateFinished { get; set; }
    }
}
