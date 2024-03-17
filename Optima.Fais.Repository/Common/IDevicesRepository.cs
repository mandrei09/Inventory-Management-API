using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IDevicesRepository : IRepository<Device>
    {
        int GetCountByFilters(string filter, List<int> deviceTypeIds);
        IEnumerable<Device> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> deviceTypeIds);
        IEnumerable<Model.Device> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
