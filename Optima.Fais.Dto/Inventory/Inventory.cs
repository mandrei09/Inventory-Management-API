using System;

namespace Optima.Fais.Dto
{
    public class Inventory
    {
        //public int id { get; set; }
        //public string description { get; set; }
        //public int? administrationId { get; set; }
        //public int? costCenterId { get; set; }
        //public int? employeeId { get; set; }
        //public int? roomId { get; set; }
        //public DateTime? start { get; set; }
        //public DateTime? end { get; set; }
        //public int? companyId { get; set; }
        //public bool active { get; set; }

        public int Id { get; set; }
        public int? DocumentId { get; set; }
        public string Description { get; set; }

        public int? AdministrationId { get; set; }
        public int? CostCenterId { get; set; }
        public int? EmployeeId { get; set; }
        public int? RoomId { get; set; }

        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        public bool Active { get; set; }
        public System.DateTime ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }

        public AccMonth AccMonth { get; set; }
		public AccMonth AccMonthBudget { get; set; }
		public BudgetManager BudgetManager { get; set; }
	}
}
