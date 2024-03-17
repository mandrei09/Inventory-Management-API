using Optima.Fais.Dto.Common;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class RequestSave
    {
        public int Id { get; set; }
        // public int? CostCenterId { get; set; }
        // public int? BudgetId { get; set; }
        // public int? BudgetBaseId { get; set; }
        // public int? BudgetForecastId { get; set; }
        public string Info { get; set; }
        public bool Validated { get; set; }
		public string UserId { get; set; }
		public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? OwnerId { get; set; }
        public decimal? BudgetValueNeed { get; set; }
        public int? StartAccMonthId { get; set; }
        public RequestBudgetForecastSave[] BudgetForecastIds { get; set; }
        public int? AssetTypeId { get; set; }
        public int? ProjectTypeId { get; set; }
        public int? DivisionId { get; set; }
		// public DateTime?[] RangeDates { get; set; }
		public DateTime? StartPeriodDate { get; set; }
		public DateTime? EndPeriodDate { get; set; }
		// public decimal Quantity { get; set; }
	}
}

public class RequestBudgetForecastSave
{
    public int Id { get; set; }

    public decimal? NeedBudgetValue { get; set; } = 0;
}
