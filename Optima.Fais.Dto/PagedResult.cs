using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class PagedResult<V>
    {
        public IEnumerable<V> Items { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public PagedResult(IEnumerable<V> items, PagingInfo pagingInfo)
        {
            Items = items;
            PagingInfo = pagingInfo;
        }
    }
}
