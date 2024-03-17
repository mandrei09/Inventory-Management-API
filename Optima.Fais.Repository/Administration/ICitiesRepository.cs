using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface ICitiesRepository : IRepository<City>
    {
        int GetCountByFilters(string filter, List<int> countyIds);
        IEnumerable<City> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> countyIds);
        IEnumerable<Model.City> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
