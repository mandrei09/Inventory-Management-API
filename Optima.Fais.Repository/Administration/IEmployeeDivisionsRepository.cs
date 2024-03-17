using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IEmployeeDivisionsRepository : IRepository<EmployeeDivision>
    {
        IEnumerable<EmployeeDivision> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> divisionIds, List<int?> employeeIds);
        int GetCountByFilters(string filter, List<int?> divisionIds, List<int?> employeeIds);
        IEnumerable<Model.EmployeeDivision> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);

        IEnumerable<EmployeeDivision> GetCustomQuery(int employeeId, string SortColumn, string sortDirection);
    }
}
