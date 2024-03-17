using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IDictionaryItemsRepository : IRepository<DictionaryItem>
    {
        IEnumerable<DictionaryItem> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> dictionaryTypeIds, List<int?> assetCategoryIds, bool showWFH);
        int GetCountByFilters(string filter, List<int?> dictionaryTypeIds, List<int?> assetCategoryIds, bool showWFH);
        IEnumerable<DictionaryItem> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
