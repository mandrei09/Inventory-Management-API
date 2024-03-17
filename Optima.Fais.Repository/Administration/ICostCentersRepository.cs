using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface ICostCentersRepository : IRepository<CostCenter>
    {
        int GetCountByFilters(string filter, List<int> administrationIds, List<int> admCenterIds, List<int?> divisionIds, List<int?> departmentIds, List<int> locationIds, List<int> costCenterIds, List<int?> exceptEmployeeIds, bool fromStock);
        IEnumerable<CostCenter> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> administrationIds, List<int> admCenterIds, List<int?> divisionIds, List<int?> departmentIds, List<int> locationIds, List<int> costCenterIds, List<int?> exceptEmployeeIds, bool fromStock);
        IEnumerable<Model.CostCenter> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
		Task<Model.ImportCostCenterResult> Import(Dto.CostCenterImport budgetImport);
	}
}
