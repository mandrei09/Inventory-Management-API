using System;

namespace Optima.Fais.Dto
{
    public class BudgetOp
    {
        public int Id { get; set; }

        public Budget Budget { get; set; }

        public CodeNameEntity CompanyInitial { get; set; }

        public CodeNameEntity CompanyFinal { get; set; }

        public CodeNameEntity ProjectInitial { get; set; }

        public CodeNameEntity ProjectFinal { get; set; }

        public CodeNameEntity AdministrationInitial { get; set; }

        public CodeNameEntity AdministrationFinal { get; set; }

        public CodeNameEntity MasterTypeInitial { get; set; }

        public CodeNameEntity MasterTypeFinal { get; set; }

        public CodeNameEntity TypeInitial { get; set; }

        public CodeNameEntity TypeFinal { get; set; }

        public CodeNameEntity SubTypeInitial { get; set; }

        public CodeNameEntity SubTypeFinal { get; set; }

        public EmployeeResource EmployeeInitial { get; set; }

        public EmployeeResource EmployeeFinal { get; set; }

        public MonthEntity AccMonth { get; set; }

        public CodeNameEntity InterCompanyInitial { get; set; }

        public CodeNameEntity InterCompanyFinal { get; set; }

        public CodePartnerEntity PartnerInitial { get; set; }

        public CodePartnerEntity PartnerFinal { get; set; }

        public CodeNameEntity AccountInitial { get; set; }

        public CodeNameEntity AccountFinal { get; set; }

        public CodeNameEntity CostCenterInitial { get; set; }

        public CodeNameEntity CostCenterFinal { get; set; }

        public decimal ValueIni1 { get; set; }

        public decimal ValueIni2 { get; set; }

        public decimal ValueFin1 { get; set; }

        public decimal ValueFin2 { get; set; }

        public decimal QuantityIni { get; set; }

        public decimal QuantityFin { get; set; }

		public string InfoIni { get; set; }

		public string InfoFin { get; set; }

		//public DateTime? SrcConfAt { get; set; }

		//public EmployeeResource SrcConfUser { get; set; }

        public DateTime? DstConfAt { get; set; }

        public EmployeeResource DstConfUser { get; set; }

        public CodeNameEntity State { get; set; }

		public Document Document { get; set; }

		public CodeNameEntity DocumentType { get; set; }

		public DateTime ModifiedAt { get; set; }


	}
}
