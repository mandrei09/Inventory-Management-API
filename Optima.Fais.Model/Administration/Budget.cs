using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class Budget : Entity
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

        public int? PartnerId { get; set; }

        public virtual Partner Partner { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool Validated { get; set; }

        public float Quantity { get; set; }

        public float QuantityRem { get; set; }

        public float QuantityUsed { get; set; }

        public float QuantityOrder { get; set; }

        public decimal ValueIni { get; set; }

        public decimal ValueFin { get; set; }

        public decimal ValueUsed { get; set; }

        public decimal ValueOrder { get; set; }

        public int? BudgetManagerId { get; set; }

        public virtual BudgetManager BudgetManager { get; set; }

        public bool IsAccepted { get; set; }

        public string Info { get; set; }

        public Guid Guid { get; set; }

        public int? UomId { get; set; }

        public virtual Uom Uom { get; set; }

		public bool IsEnabled { get; set; }

        public decimal Price { get; set; }

        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        public int? ActivityId { get; set; }

        public virtual Activity Activity { get; set; }

        public int? AdmCenterId { get; set; }

        public virtual AdmCenter AdmCenter { get; set; }

        public int? RegionId { get; set; }

        public virtual Region Region { get; set; }

        public int? AssetTypeId { get; set; }

        public virtual AssetType AssetType { get; set; }

        public int? ProjectTypeId { get; set; }

        public virtual ProjectType ProjectType { get; set; }

        public int DepPeriod { get; set; }

        public int DepPeriodRem { get; set; }

        public decimal Total { get; set; }

        public virtual ICollection<BudgetMonth> BudgetMonths { get; set; }

        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public int? DivisionId { get; set; }

        public virtual Division Division { get; set; }

        //public bool IsLast { get; set; }

    }
}
