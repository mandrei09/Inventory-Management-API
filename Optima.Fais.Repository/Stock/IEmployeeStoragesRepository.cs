using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IEmployeeStoragesRepository : IRepository<EmployeeStorage>
    {
        IEnumerable<EmployeeStorage> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> storageIds, List<int?> employeeIds);
        int GetCountByFilters(string filter, List<int?> storageIds, List<int?> employeeIds);
    }
}
