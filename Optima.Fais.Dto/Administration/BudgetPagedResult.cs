using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class BudgetPagedResult : PagedResult<Dto.Budget>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetCategoryTotal AssetCategoryTotals { get; set; }

        public BudgetPagedResult(IEnumerable<Dto.Budget> data, PagingInfo pagingInfo, AssetDepTotal totals, AssetCategoryTotal assetCategoryTotals)
            : base(data, pagingInfo)
        {
            Totals = totals;
            AssetCategoryTotals = assetCategoryTotals;
        }
    }
}
