using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class BudgetForecastUIPagedResult : PagedResult<Dto.BudgetForecastUI>
    {
        public ForecastTotal Totals { get; set; }

        public BudgetForecastUIPagedResult(IEnumerable<Dto.BudgetForecastUI> items, PagingInfo pagingInfo, ForecastTotal totals)
            : base(items, pagingInfo)
        {
            Totals = totals;
        }
    }
}
