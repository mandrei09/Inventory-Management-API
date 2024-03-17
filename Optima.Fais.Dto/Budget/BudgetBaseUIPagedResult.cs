using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class BudgetBaseUIPagedResult : PagedResult<Dto.BudgetBaseUI>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetCategoryTotal AssetCategoryTotals { get; set; }

        public BudgetBaseUIPagedResult(IEnumerable<Dto.BudgetBaseUI> items, PagingInfo pagingInfo, AssetDepTotal totals, AssetCategoryTotal assetCategoryTotals)
            : base(items, pagingInfo)
        {
            Totals = totals;
            AssetCategoryTotals = assetCategoryTotals;
        }
    }
}
