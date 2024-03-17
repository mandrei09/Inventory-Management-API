using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class PagedResultTree<V>
    {
        public IEnumerable<V> Data { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public PagedResultTree(IEnumerable<V> data, PagingInfo pagingInfo)
        {
            Data = data;
            PagingInfo = pagingInfo;
        }
    }
}
