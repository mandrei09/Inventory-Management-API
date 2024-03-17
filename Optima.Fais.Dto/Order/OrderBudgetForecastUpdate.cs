using Optima.Fais.Dto.Common;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class OrderBudgetForecastUpdate
	{
        public int OrderId { get; set; }
        public int BudgetForecastId { get; set; }
		public string UserId { get; set; }
	}
}
