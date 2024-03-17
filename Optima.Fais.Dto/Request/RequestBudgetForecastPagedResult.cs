using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class RequestBudgetForecastPagedResult : PagedResult<Dto.RequestBudgetForecast>
    {
        public RequestBudgetForecastTotal Totals { get; set; }

        public RequestBudgetForecastPagedResult(IEnumerable<Dto.RequestBudgetForecast> data, PagingInfo pagingInfo, RequestBudgetForecastTotal totals)
            : base(data, pagingInfo)
        {
            Totals = totals;
        }
    }
}
