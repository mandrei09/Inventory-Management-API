using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class BudgetBase : Entity
	{
		public string Code { get; set; }

		public string Name { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

		public int? ProjectId { get; set; }

		public virtual Project Project { get; set; }

		public int? CostCenterId { get; set; }

		public virtual CostCenter CostCenter { get; set; }

		[MaxLength(450)]
		public string UserId { get; set; }

		public ApplicationUser User { get; set; }

		public int? EmployeeId { get; set; }

		public virtual Employee Employee { get; set; }

		public int? AppStateId { get; set; }

		public virtual AppState AppState { get; set; }

		public int? StartMonthId { get; set; }

		public virtual AccMonth StartMonth { get; set; }

		public int? AccMonthId { get; set; }

		public virtual AccMonth AccMonth { get; set; }

		public decimal DepPeriod { get; set; }

		public decimal DepPeriodRem { get; set; }

		public decimal ValueRem { get; set; }

		public bool Validated { get; set; }

		public decimal ValueIni { get; set; }

		public decimal ValueFin { get; set; }

		public decimal ValueUsed { get; set; }

		public decimal ValueOrder { get; set; }

		public decimal ValueAsset { get; set; }

		public decimal LastYearValue { get; set; }

		public int? BudgetManagerId { get; set; }

		public virtual BudgetManager BudgetManager { get; set; }

		public bool IsAccepted { get; set; }

		public string Info { get; set; }

		public int? UomId { get; set; }

		public virtual Uom Uom { get; set; }

		public bool IsEnabled { get; set; }

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

		public decimal Total { get; set; }

		//public virtual BudgetMonthBase BudgetMonthBase { get; set; }

		//public virtual BudgetForecast BudgetForecast { get; set; }

		public virtual ICollection<BudgetMonthBase> BudgetMonthBase { get; set; }

		public virtual ICollection<BudgetForecast> BudgetForecast { get; set; }

		public virtual ICollection<BudgetBaseAsset> BudgetBaseAsset { get; set; }

		public int? DepartmentId { get; set; }

		public virtual Department Department { get; set; }

		public int? DivisionId { get; set; }

		public virtual Division Division { get; set; }

		public int BudgetTypeId { get; set; }

		public virtual BudgetType BudgetType { get; set; }

		public bool IsLast { get; set; }

		public bool IsFirst { get; set; }
	}
}
