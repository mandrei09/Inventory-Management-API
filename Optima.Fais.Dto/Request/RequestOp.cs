using System;

namespace Optima.Fais.Dto
{
    public class RequestOp
    {
        public int Id { get; set; }

        public Request Request { get; set; }

        public CodeNameEntity OfferFinal { get; set; }

        public CodeNameEntity BudgetInitial { get; set; }

        public CodeNameEntity Company { get; set; }

        public CodeNameEntity ProjectInitial { get; set; }

        public CodeNameEntity ProjectFinal { get; set; }

        public EmployeeResource EmployeeInitial { get; set; }

        public EmployeeResource EmployeeFinal { get; set; }

        public MonthEntity AccMonth { get; set; }

        public CodeNameEntity CostCenterInitial { get; set; }

        public CodeNameEntity CostCenterFinal { get; set; }

		public string InfoIni { get; set; }

		public string InfoFin { get; set; }

        public DateTime? DstConfAt { get; set; }

        public EmployeeResource DstConfUser { get; set; }

        public CodeNameEntity State { get; set; }

		public Document Document { get; set; }

		public CodeNameEntity DocumentType { get; set; }

		public DateTime ModifiedAt { get; set; }


	}
}
