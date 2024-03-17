using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class BudgetForecast
    {
		public int Id { get; set; }

        public BudgetBase BudgetBase { get; set; }

        public CodeNameEntity BudgetType { get; set; }

        public CodeNameEntity BudgetManager { get; set; }

        public CodeNameEntity AppState { get; set; }

        public CodeNameEntity Project { get; set; }

        public MonthEntity AccMonth { get; set; }

        public MonthEntity StartMonth { get; set; }

        public decimal January { get; set; }

        public decimal February { get; set; }

        public decimal March { get; set; }

        public decimal April { get; set; }

        public decimal May { get; set; }

        public decimal June { get; set; }

        public decimal July { get; set; }

        public decimal August { get; set; }

        public decimal September { get; set; }

        public decimal Octomber { get; set; }

        public decimal November { get; set; }

        public decimal December { get; set; }

        public decimal Total { get; set; }

        public decimal DepPeriod { get; set; }

        public decimal DepPeriodRem { get; set; }

        public decimal ValueOrder { get; set; }

        public decimal ValueAsset { get; set; }

        public decimal TotalRem { get; set; }

		public bool IsFirst { get; set; }

        public bool HasChangeApril { get; set; }

        public bool HasChangeMay { get; set; }

        public bool HasChangeJune { get; set; }

        public bool HasChangeJuly { get; set; }

        public bool HasChangeAugust { get; set; }

        public bool HasChangeSeptember { get; set; }

        public bool HasChangeOctomber { get; set; }

        public bool HasChangeNovember { get; set; }

        public bool HasChangeDecember { get; set; }

        public bool HasChangeJanuary { get; set; }

        public bool HasChangeFebruary { get; set; }

        public bool HasChangeMarch { get; set; }

        public decimal ImportValueOrder { get; set; }

		public decimal ValueAssetYTD { get; set; }

		public decimal ValueAssetYTG { get; set; }

		public decimal ValueOrderPending { get; set; }

		public decimal ValueOrderApproved { get; set; }

		public decimal ValueRequest { get; set; }



		public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public CodeNameEntity Company { get; set; }

        public int? ProjectId { get; set; }

        //public int? AdministrationId { get; set; }

        //public CodeNameEntity Administration { get; set; }

        //public int? CostCenterId { get; set; }

        //public CodeNameEntity CostCenter { get; set; }

        //public int? SubTypeId { get; set; }

        //public CodeNameEntity MasterType { get; set; }

        //public CodeNameEntity Type { get; set; }

        //public CodeNameEntity SubType { get; set; }

        [MaxLength(450)]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        // public int? EmployeeId { get; set; }

        public EmployeeResource Employee { get; set; }

        // public int? AccMonthId { get; set; }


        //public int? InterCompanyId { get; set; }

        //public CodeNameEntity InterCompany { get; set; }

        //public int? PartnerId { get; set; }

        //public CodePartnerEntity Partner { get; set; }

        //public int? AccountId { get; set; }

        //public CodeNameEntity Account { get; set; }

        public int? AppStateId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool Validated { get; set; }

        public float Quantity { get; set; }

        public float QuantityRem { get; set; }

        public decimal ValueIni { get; set; }

        public decimal ValueFin { get; set; }

        // public int? BudgetManagerId { get; set; }

        public bool IsAccepted { get; set; }

        public string Info { get; set; }

        public string InfoMinus { get; set; }

        public bool IsDeleted { get; set; }

        public CodeNameEntity Country { get; set; }

        public CodeNameEntity Activity { get; set; }

        public CodeNameEntity Region { get; set; }

        public CodeNameEntity AdmCenter { get; set; }

        public CodeNameEntity AssetType { get; set; }

        public CodeNameEntity ProjectType { get; set; }

        public CodeNameEntity Department { get; set; }

        public CodeNameEntity Division { get; set; }

        public ICollection<BudgetMonthBase> BudgetMonthBases { get; set; }

		public bool InTransfer { get; set; }
	}
}
