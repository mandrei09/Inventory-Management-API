using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class Order : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public int? AdministrationId { get; set; }

        public virtual Administration Administration { get; set; }

        public int? CostCenterId { get; set; }

        public virtual CostCenter CostCenter { get; set; }

        public int? SubTypeId { get; set; }

        public virtual SubType SubType { get; set; }

        [MaxLength(450)]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int? EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        public int? AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public int? InterCompanyId { get; set; }

        public virtual InterCompany InterCompany { get; set; }

        public int? PartnerId { get; set; }

        public virtual Partner Partner { get; set; }

        public int? AccountId { get; set; }

        public virtual Account Account { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool Validated { get; set; }

        public float Quantity { get; set; }

        public float QuantityRem { get; set; }

        public float QuantityUsed { get; set; }

        public decimal ValueIni { get; set; }

        public decimal ValueFin { get; set; }

        public decimal ValueUsed { get; set; }

        public decimal Price { get; set; }

        public int? BudgetManagerId { get; set; }

        public virtual BudgetManager BudgetManager { get; set; }

        public bool IsAccepted { get; set; }

        public string Info { get; set; }

        public Guid Guid { get; set; }

        public int? BudgetId { get; set; }

        public virtual Budget Budget { get; set; }

        public int? BudgetBaseId { get; set; }

        public virtual BudgetBase BudgetBase { get; set; }

        public int? UomId { get; set; }

        public virtual Uom Uom { get; set; }

        public int? OfferId { get; set; }

        public virtual Offer Offer { get; set; }

        public bool IsEnabled { get; set; }

        public int? ContractId { get; set; }

        public virtual Contract Contract { get; set; }

        public int? RateId { get; set; }

        public virtual Rate Rate { get; set; }

        public virtual ICollection<OrderMaterial> OrderMaterials { get; set; }

        public int? OrderTypeId { get; set; }

        public virtual OrderType OrderType { get; set; }

		public decimal PreAmount { get; set; }

        public bool WIP { get; set; }


        public decimal ValueIniRon { get; set; }

        public decimal ValueFinRon { get; set; }

        public decimal ValueUsedRon { get; set; }

        public decimal PriceRon { get; set; }

        public decimal PreAmountRon { get; set; }

        public decimal BudgetValueNeed { get; set; }

        public decimal BudgetValueNeedOtherCurrency { get; set; }

        public int? StartAccMonthId { get; set; }

        public virtual AccMonth StartAccMonth { get; set; }

        public int? EmployeeL1Id { get; set; }

        public virtual Employee EmployeeL1 { get; set; }

        public int? EmployeeL2Id { get; set; }

        public virtual Employee EmployeeL2 { get; set; }

        public int? EmployeeL3Id { get; set; }

        public virtual Employee EmployeeL3 { get; set; }

        public int? EmployeeL4Id { get; set; }

        public virtual Employee EmployeeL4 { get; set; }

        public int? EmployeeS1Id { get; set; }

        public virtual Employee EmployeeS1 { get; set; }

        public int? EmployeeS2Id { get; set; }

        public virtual Employee EmployeeS2 { get; set; }

        public int? EmployeeS3Id { get; set; }

        public virtual Employee EmployeeS3 { get; set; }

        public int? BudgetForecastId { get; set; }

        public virtual BudgetForecast BudgetForecast { get; set; }

        public int? DivisionId { get; set; }

        public virtual Division Division { get; set; }

		public int? EmployeeB1Id { get; set; }

		public virtual Employee EmployeeB1 { get; set; }
	}
}
