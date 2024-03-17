using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class OfferUIPagedResult : PagedResult<Dto.OfferUI>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetCategoryTotal AssetCategoryTotals { get; set; }

        public OfferUIPagedResult(IEnumerable<Dto.OfferUI> items, PagingInfo pagingInfo, AssetDepTotal totals, AssetCategoryTotal assetCategoryTotals)
            : base(items, pagingInfo)
        {
            Totals = totals;
            AssetCategoryTotals = assetCategoryTotals;
        }
    }
}
