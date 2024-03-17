using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public class Offer : Entity
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
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ValueIni { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ValueFin { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ValueUsed { get; set; }

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

        public bool IsEnabled { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Price { get; set; }

        public int? AdmCenterId { get; set; }

        public virtual AdmCenter AdmCenter { get; set; }

        public int? RegionId { get; set; }

        public virtual Region Region { get; set; }

        public int? AssetTypeId { get; set; }

        public virtual AssetType AssetType { get; set; }

        public int? ProjectTypeId { get; set; }

        public virtual ProjectType ProjectType { get; set; }

        public virtual ICollection<OfferMaterial> OfferMaterials { get; set; }

        public int? RateId { get; set; }

        public virtual Rate Rate { get; set; }

        public int? RequestId { get; set; }

        public virtual Request Request { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal PreAmount { get; set; }

        public int? OrderTypeId { get; set; }

        public virtual OrderType OrderType { get; set; }

		public bool WIP { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal PriceRon { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal PreAmountRon { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ValueIniRon { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ValueFinRon { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ValueUsedRon { get; set; }

        public int? OfferTypeId { get; set; }

        public virtual OfferType OfferType { get; set; }

        public int? BudgetForecastId { get; set; }

        public virtual BudgetForecast BudgetForecast { get; set; }

        public int? DivisionId { get; set; }

        public virtual Division Division { get; set; }

    }
}
