using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IDevicesTypesRepository : IRepository<DeviceType>
    {
        int GetCountByFilters(string filter);
        IEnumerable<DeviceType> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        IEnumerable<Model.DeviceType> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
