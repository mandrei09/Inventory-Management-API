using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class BudgetPagedResultTree : PagedResultTree<Dto.BudgetTree>
    {
        public BudgetPagedResultTree(IEnumerable<Dto.BudgetTree> data, PagingInfo pagingInfo)
            : base(data, pagingInfo)
        {
        }
    }
}
