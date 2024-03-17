using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class Request
    {
		public int Id { get; set; }

		public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public CodeNameEntity Company { get; set; }

        [MaxLength(450)]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int? EmployeeId { get; set; }

        public EmployeeResource Employee { get; set; }

        public EmployeeResource Owner { get; set; }

        public int? AccMonthId { get; set; }

        public MonthEntity AccMonth { get; set; }

        public int? AppStateId { get; set; }

        public AppState AppState { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool Validated { get; set; }

        public int? BudgetManagerId { get; set; }

        public CodeNameEntity BudgetManager { get; set; }

        public bool IsAccepted { get; set; }

        public string Info { get; set; }

        public string InfoMinus { get; set; }

        public bool IsDeleted { get; set; }

		public CodeNameEntity Project { get; set; }

		public CodeNameEntity ProjectType { get; set; }

        public CodeNameEntity AssetType { get; set; }

        public CodeNameEntity Division { get; set; }

        public decimal BudgetValueNeed { get; set; }

        public MonthEntity StartAccMonth { get; set; }

        public DateTime? StartExecution { get; set; }

        public DateTime? EndExecution { get; set; }

        public decimal Quantity { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? ModifiedAt { get; set; }

	}
}
