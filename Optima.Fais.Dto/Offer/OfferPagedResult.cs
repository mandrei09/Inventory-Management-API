using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class OfferPagedResult : PagedResult<Dto.Offer>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetCategoryTotal AssetCategoryTotals { get; set; }

        public OfferPagedResult(IEnumerable<Dto.Offer> items, PagingInfo pagingInfo, AssetDepTotal totals, AssetCategoryTotal assetCategoryTotals)
            : base(items, pagingInfo)
        {
            Totals = totals;
            AssetCategoryTotals = assetCategoryTotals;
        }
    }
}
