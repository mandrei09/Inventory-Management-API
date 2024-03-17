using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class RequestPagedResult : PagedResult<Dto.Request>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetCategoryTotal AssetCategoryTotals { get; set; }

        public RequestPagedResult(IEnumerable<Dto.Request> items, PagingInfo pagingInfo, AssetDepTotal totals, AssetCategoryTotal assetCategoryTotals)
            : base(items, pagingInfo)
        {
            Totals = totals;
            AssetCategoryTotals = assetCategoryTotals;
        }
    }
}
