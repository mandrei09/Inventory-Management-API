using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class DashboardPagedResult : PagedResult<DashboardReport>
    {
        public DashboardTotal Totals { get; set; }

        public DashboardPagedResult(IEnumerable<DashboardReport> items, PagingInfo pagingInfo, DashboardTotal totals)
            : base(items, pagingInfo)
        {
            Totals = totals;
        }
    }
}
