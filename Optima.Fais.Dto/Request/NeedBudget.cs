using Optima.Fais.Dto.Common;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class NeedBudget
    {
        public int? CostCenterId { get; set; }
        public int? ProjectTypeDivisionId { get; set; }
        public int? AssetTypeId { get; set; }
        public int? EmployeeId { get; set; }
		public int? OwnerId { get; set; }
		public string Info { get; set; }
        public string UserId { get; set; }
		public DateTime? EndDate { get; set; }
		public decimal BudgetValueNeed { get; set; }
        public int? StartAccMonthId { get; set; }
    }
}
