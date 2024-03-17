using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class InventoryPlanBaseMain
    {
        public int InventoryId { get; set; }
        public int InvCommitteeId { get; set; }
        public int? CostCenterId { get; set; }
        public int? AdministrationId { get; set; }
        public DateTime? DateStarted { get; set; }
        public DateTime? DateFinished { get; set; }
    }

    public class InventoryPlan : InventoryPlanBaseMain
    {
        public int Id { get; set; }
        public Inventory Inventory { get; set; }
        public InvCommittee InvCommittee { get; set; }
        public virtual CostCenter CostCenter { get; set; }
        public virtual Administration Administration { get; set; }
    }
}
