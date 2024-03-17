using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class AssetStatusDashboardPagedResult : PagedResult<DashboardView>
    {
        public DashboardTotal Totals { get; set; }

        public AssetStatusDashboardPagedResult(IEnumerable<DashboardView> items, PagingInfo pagingInfo, DashboardTotal totals)
            : base(items, pagingInfo)
        {
            Totals = totals;
        }
    }
}
