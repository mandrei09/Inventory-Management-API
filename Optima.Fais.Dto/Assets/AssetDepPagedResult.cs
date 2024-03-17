using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class AssetDepPagedResult : PagedResult<Asset>
    {
        public AssetDepTotal Totals { get; set; }

        public AssetDepPagedResult(IEnumerable<Asset> items, PagingInfo pagingInfo, AssetDepTotal totals)
            : base(items, pagingInfo)
        {
            Totals = totals;
        }
    }
}
