using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class AssetStatusWHDashboardPagedResult : PagedResult<InventoryAssetWHResource>
    {
        public DashboardTotal Totals { get; set; }

        public AssetStatusWHDashboardPagedResult(IEnumerable<InventoryAssetWHResource> items, PagingInfo pagingInfo, DashboardTotal totals)
            : base(items, pagingInfo)
        {
            Totals = totals;
        }
    }
}
