using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface ILocationsRepository : IRepository<Location>
    {
        int GetCountByFilters(string filter, List<int> cityIds, List<int> regionIds, List<int> admCenterIds);
        IEnumerable<Location> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> cityIds, List<int> regionIds, List<int> admCenterIds);
        IEnumerable<Model.Location> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
