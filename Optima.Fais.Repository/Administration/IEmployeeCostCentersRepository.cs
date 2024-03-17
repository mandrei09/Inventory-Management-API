using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IEmployeeCostCentersRepository : IRepository<EmployeeCostCenter>
    {
        IEnumerable<EmployeeCostCenter> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> costCenterIds, List<int?> employeeIds);
        int GetCountByFilters(string filter, List<int?> costCenterIds, List<int?> employeeIds);
        IEnumerable<Model.EmployeeCostCenter> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
        Task<List<Model.EmployeeCostCenter>> GetAllByCostCenter(ReportFilter reportFilter);
    }
}
