using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class OrderUIPagedResult : PagedResult<Dto.OrderUI>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetCategoryTotal AssetCategoryTotals { get; set; }

        public OrderUIPagedResult(IEnumerable<Dto.OrderUI> items, PagingInfo pagingInfo, AssetDepTotal totals, AssetCategoryTotal assetCategoryTotals)
            : base(items, pagingInfo)
        {
            Totals = totals;
            AssetCategoryTotals = assetCategoryTotals;
        }
    }
}
