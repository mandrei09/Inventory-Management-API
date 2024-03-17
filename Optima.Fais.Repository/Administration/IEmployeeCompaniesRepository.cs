using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IEmployeeCompaniesRepository : IRepository<EmployeeCompany>
    {
        IEnumerable<EmployeeCompany> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> companyIds, List<int?> employeeIds);
        int GetCountByFilters(string filter, List<int?> companyIds, List<int?> employeeIds);
        IEnumerable<Model.EmployeeCompany> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
