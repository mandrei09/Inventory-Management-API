using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IEmployeesRepository : IRepository<Model.Employee>
    {
        IEnumerable<Dto.EmployeeDetail> GetDetailsByFilters(int? departmentId, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
        Task<IEnumerable<Dto.EmployeeDetail>> GetDetailsByFiltersAsync(int? departmentId, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        IEnumerable<Model.Employee> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
        void EmployeeImport(Dto.EmployeeImport employeeImport, out bool updated, out bool userExist, out Model.ApplicationUser user, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut);
        IEnumerable<Model.Employee> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> companyIds, List<int?> costCenterIds, string email, bool deletedEmployees, bool IsBudgetOwner, bool teamStatus);
        int GetCountByFilters(string filter, List<int?> companyIds, List<int?> costCenterIds, string email, bool deletedEmployees, bool IsBudgetOwner, bool teamStatus);
        Model.Employee GetDetailsById(int employeeId, string includes);
        IEnumerable<Model.Employee> GetTransferEmployeesInUseWithAssets(AssetFilter assetFilter, List<PropertyFilter> propFilters);
    }
}
