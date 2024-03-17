using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class ContractUIPagedResult : PagedResult<Dto.ContractUI>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetCategoryTotal AssetCategoryTotals { get; set; }

        public ContractUIPagedResult(IEnumerable<Dto.ContractUI> items, PagingInfo pagingInfo, AssetDepTotal totals, AssetCategoryTotal assetCategoryTotals)
            : base(items, pagingInfo)
        {
            Totals = totals;
            AssetCategoryTotals = assetCategoryTotals;
        }
    }
}
