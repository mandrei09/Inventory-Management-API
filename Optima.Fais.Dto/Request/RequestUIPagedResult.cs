using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class RequestUIPagedResult : PagedResult<Dto.RequestUI>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetCategoryTotal AssetCategoryTotals { get; set; }

        public RequestUIPagedResult(IEnumerable<Dto.RequestUI> items, PagingInfo pagingInfo, AssetDepTotal totals, AssetCategoryTotal assetCategoryTotals)
            : base(items, pagingInfo)
        {
            Totals = totals;
            AssetCategoryTotals = assetCategoryTotals;
        }
    }
}
