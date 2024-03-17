using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class BudgetForecastPagedResult : PagedResult<Dto.BudgetForecast>
    {
        public ForecastTotal Totals { get; set; }

        public BudgetForecastPagedResult(IEnumerable<Dto.BudgetForecast> data, PagingInfo pagingInfo, ForecastTotal totals)
            : base(data, pagingInfo)
        {
            Totals = totals;
        }
    }
}
