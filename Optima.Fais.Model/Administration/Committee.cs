using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
	public class Committee : Entity
    {
        public Committee()
        {
        }


        public int? InventoryId { get; set; }

        public virtual Inventory Inventory { get; set; }

        public int? CostCenterId { get; set; }

        public virtual CostCenter CostCenter { get; set; }

        public int? AdministrationId { get; set; }

        public virtual Administration Administration { get; set; }

        public int? RoomId { get; set; }

        public virtual Room Room { get; set; }

        public int? EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        public int? EmployeeId2 { get; set; }

        public virtual Employee Employee2 { get; set; }

        public int? EmployeeId3 { get; set; }

        public virtual Employee Employee3 { get; set; }

        public int? EmployeeId4 { get; set; }

        public virtual Employee Employee4 { get; set; }

        public int? EmployeeId5 { get; set; }

        public virtual Employee Employee5 { get; set; }

        public int? EmployeeId6 { get; set; }

        public virtual Employee Employee6 { get; set; }

        public int? EmployeeId7 { get; set; }

        public virtual Employee Employee7 { get; set; }

        //public int? EmployeeId8 { get; set; }

        //public virtual Employee Employee8 { get; set; }

        //public int? EmployeeId9 { get; set; }

        //public virtual Employee Employee9 { get; set; }

        //public int? EmployeeId10 { get; set; }

        //public virtual Employee Employee10 { get; set; }

        public string Document1 { get; set; }

        public string Document2 { get; set; }
    }
}
