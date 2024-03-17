using System;

namespace Optima.Fais.Model
{
    public class AssetMonthDetailExport
    {
        public string InvNo { get; set; }
		public string SubNo { get; set; }
		public string Account { get; set; }
		public string ExpAccount { get; set; }
		public string AssetCategory { get; set; }
		public string Description { get; set; }
		public string ErpCode { get; set; }
		public string SerialNumber { get; set; }
		public string Partner { get; set; }
		public string PartnerName { get; set; }
		public string Manufacturier { get; set; }
		public string Location { get; set; }
		public string CostCenterCode { get; set; }
		public string CostCenterName { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeInternalCode { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeCostCenterCode { get; set; }
		public string ProfitCenterCode { get; set; }
        public DateTime? PurchaseDate { get; set; }
		public DateTime? InvoiceDate { get; set; }
		public DateTime? RemovalDate { get; set; }
		public int UsefulLife { get; set; }
		public int TotLifeInpPeriods { get; set; }
		public int ExpLifeInPeriods { get; set; }
		public int RemLifeInPeriods { get; set; }
		public decimal APCFYStart { get; set; }
		public decimal DepFYStart { get; set; }
		public decimal BkValFYStart { get; set; }
		public decimal Acquisition { get; set; }
		public decimal DepForYear { get; set; }
		public decimal Retirement { get; set; }
		public decimal DepRetirement { get; set; }
		public decimal CurrBkValue { get; set; }
		public decimal Transfer { get; set; }
		public decimal DepTransfer { get; set; }
		public decimal PosCap { get; set; }
		public decimal DepPostCap { get; set; }
		public decimal InvestSupport { get; set; }
		public decimal WriteUps { get; set; }
		public decimal CurrentAPC { get; set; }
		public decimal AccumulDep { get; set; }
		public string Material { get; set; }
		public string ProjectCode { get; set; }
		public string BudgetBaseCode { get; set; }
        public string RequestCode { get; set; }
		public string OfferCode { get; set; }
		public string OrderCode { get; set; }
		public string Source { get; set; }
		public string ReceptionEmail { get; set; }

		public string ContractID { get; set; }
		public DateTime? RequestDate { get; set; } = null;
		public DateTime? OfferDate { get; set; } = null;
		public DateTime? OrderDate { get; set; } = null;
		public DateTime? OrderEndDate { get; set; } = null;
		public DateTime? ReceptionDate { get; set; } = null;
	}
}
