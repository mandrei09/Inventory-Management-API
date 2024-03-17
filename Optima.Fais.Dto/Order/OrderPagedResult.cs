using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class OrderPagedResult : PagedResult<Dto.Order>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetCategoryTotal AssetCategoryTotals { get; set; }

        public OrderPagedResult(IEnumerable<Dto.Order> items, PagingInfo pagingInfo, AssetDepTotal totals, AssetCategoryTotal assetCategoryTotals)
            : base(items, pagingInfo)
        {
            Totals = totals;
            AssetCategoryTotals = assetCategoryTotals;
        }
    }
}
