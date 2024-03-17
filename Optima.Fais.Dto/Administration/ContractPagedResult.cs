using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class ContractPagedResult : PagedResult<Dto.Contract>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetCategoryTotal AssetCategoryTotals { get; set; }

        public ContractPagedResult(IEnumerable<Dto.Contract> items, PagingInfo pagingInfo, AssetDepTotal totals, AssetCategoryTotal assetCategoryTotals)
            : base(items, pagingInfo)
        {
            Totals = totals;
            AssetCategoryTotals = assetCategoryTotals;
        }
    }
}
