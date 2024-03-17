using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class Request : Entity
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

        public int? AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool Validated { get; set; }

        public int? BudgetManagerId { get; set; }

        public virtual BudgetManager BudgetManager { get; set; }

        public bool IsAccepted { get; set; }

        public string Info { get; set; }

        public int? BudgetId { get; set; }

        public virtual Budget Budget { get; set; }

        public int? BudgetBaseId { get; set; }

        public virtual BudgetBase BudgetBase { get; set; }

        public int? AssetTypeId { get; set; }

        public virtual AssetType AssetType { get; set; }

        public int? ProjectTypeId { get; set; }

        public virtual ProjectType ProjectType { get; set; }

		public Guid Guid { get; set; }

        public int? OwnerId { get; set; }

        public virtual Employee Owner { get; set; }

        public decimal BudgetValueNeed { get; set; }

        public int? StartAccMonthId { get; set; }

        public virtual AccMonth StartAccMonth { get; set; }

        public int? BudgetForecastId { get; set; }

        public virtual BudgetForecast BudgetForecast { get; set; }

        public int? DivisionId { get; set; }

        public virtual Division Division { get; set; }

        public DateTime? StartExecution { get; set; }

        public DateTime? EndExecution { get; set; }

        public int? RequestTypeId { get; set; }

        public virtual RequestType RequestType { get; set; }

		public decimal Quantity { get; set; }
	}
}
