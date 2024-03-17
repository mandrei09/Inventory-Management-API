using System;

namespace Optima.Fais.Model
{
    public class Inventory : Entity
    {
        public int DocumentId { get; set; }

        public virtual Document Document { get; set; }

        public string Description { get; set; }

        public int? AdministrationId { get; set; }

        public virtual Administration Administration { get; set; }

        public int? CostCenterId { get; set; }

        public virtual CostCenter CostCenter { get; set; }

        public int? EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        public int? RoomId { get; set; }

        public virtual Room Room { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public bool Active { get; set; }

        public int? AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public int? BudgetManagerId { get; set; }

        public virtual BudgetManager BudgetManager { get; set; }

		public int? AccMonthBudgetId { get; set; }

		public virtual AccMonth AccMonthBudget { get; set; }
	}
}
