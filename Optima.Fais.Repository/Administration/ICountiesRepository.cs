using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface ICountiesRepository : IRepository<County>
    {
        int GetCountByFilters(string filter, List<int> countryIds);
        IEnumerable<County> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> countryIds);
        IEnumerable<Model.County> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
