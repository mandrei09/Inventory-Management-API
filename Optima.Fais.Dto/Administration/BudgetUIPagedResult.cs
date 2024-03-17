using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class BudgetUIPagedResult : PagedResult<Dto.BudgetUI>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetCategoryTotal AssetCategoryTotals { get; set; }

        public BudgetUIPagedResult(IEnumerable<Dto.BudgetUI> items, PagingInfo pagingInfo, AssetDepTotal totals, AssetCategoryTotal assetCategoryTotals)
            : base(items, pagingInfo)
        {
            Totals = totals;
            AssetCategoryTotals = assetCategoryTotals;
        }
    }
}
