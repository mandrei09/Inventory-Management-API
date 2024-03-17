using System;

namespace Optima.Fais.Dto
{
    public class AssetImportEmag
    {
        
        public string InvNo { get; set; }
		public int Quantity { get; set; }
        public string SubNo { get; set; }
        public string Account { get; set; }
        public string ExpAccount { get; set; }
        public string AssetCategory { get; set; }
        public string Description { get; set; }
        public string SAPCode { get; set; }
        //public string License { get; set; }
        public string SerialNumber { get; set; }
        public string CUI { get; set; }
        public string Partner { get; set; }
        //public string AgreementNo { get; set; }
        public string ManufacturerNo { get; set; }
        public string Article { get; set; }
        public string CostCenterCode { get; set; }
        public DateTime? PIFDate { get; set; }
        public DateTime? UsageStartDate { get; set; }
        public DateTime? RemovalDate { get; set; }
        public int UsefulLife { get; set; }
        public int ExpLifeInPeriods { get; set; }
        public int RemLifeInPeriods { get; set; }
        public int TotLifeInPeriods { get; set; }
        public decimal DepFYStart { get; set; }
        public decimal APCFYStart { get; set; }
        public decimal BkValFYStart { get; set; }
        public decimal CurrBkValue { get; set; }
        public decimal CurrentAPC { get; set; }
        public decimal DepRetirement { get; set; }
        public decimal DepTransfer { get; set; }
        public decimal Acquisition { get; set; }
        public decimal Retirement { get; set; }
        public decimal Transfer { get; set; }
        public decimal PosCap { get; set; }
        public decimal DepPostCap { get; set; }
        public decimal InvestSupport { get; set; }
        public decimal WriteUps { get; set; }
        public decimal DepForYear { get; set; }
        public decimal AccumulDep { get; set; }
        public string CostCenterName { get; set; }
		public string IdentityNumber { get; set; }
		public string PersonnelNumber { get; set; }
		public string ProfitCenter { get; set; }
		public int UsefullLifeInPeriods { get; set; }

		public int InventoryId { get; set; }
		public int AccMonthId { get; set; }
		public int DocumentId { get; set; }
		public int BudgetManagerId { get; set; }
		public int AssetStateId { get; set; }
		public int InvStateId { get; set; }
		public int DimensionId { get; set; }
		public int UomId { get; set; }
		public int DocumentTypeId { get; set; }
		public int CompanyId { get; set; }
		public int AssetTypeId { get; set; }
		public int AdministrationId { get; set; }
		public int AccSystemId { get; set; }
		public int AssetClassId { get; set; }
		public int AssetClassTypeId { get; set; }
	}
}
