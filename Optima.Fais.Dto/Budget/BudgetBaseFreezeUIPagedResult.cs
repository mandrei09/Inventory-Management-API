using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class BudgetBaseFreezeUIPagedResult : PagedResult<Dto.BudgetBaseFreezeUI>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetCategoryTotal AssetCategoryTotals { get; set; }

        public BudgetBaseFreezeUIPagedResult(IEnumerable<Dto.BudgetBaseFreezeUI> items, PagingInfo pagingInfo, AssetDepTotal totals, AssetCategoryTotal assetCategoryTotals)
            : base(items, pagingInfo)
        {
            Totals = totals;
            AssetCategoryTotals = assetCategoryTotals;
        }
    }
}
