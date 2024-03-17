using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IMatrixRepository : IRepository<Matrix>
    {
        int GetCountByFilters(string filter, List<int?> assetTypeIds, List<int?> projectTypeIds, List<int?> areaIds, List<int?> countryIds, List<int?> companyIds, List<int?> divisionIds, List<int?> costCenterIds, List<int?> projectIds, 
        List<int?> employeeL1Ids, List<int?> employeeL2Ids, List<int?> employeeL3Ids, List<int?> employeeL4Ids, List<int?> employeeS1Ids, List<int?> employeeS2Ids, List<int?> employeeS3Ids, List<int?> exceptEmployeeIds);
		IEnumerable<Matrix> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> assetTypeIds, List<int?> projectTypeIds, List<int?> areaIds, List<int?> countryIds, List<int?> companyIds, List<int?> divisionIds, List<int?> costCenterIds, List<int?> projectIds,
        List<int?> employeeL1Ids, List<int?> employeeL2Ids, List<int?> employeeL3Ids, List<int?> employeeL4Ids, List<int?> employeeS1Ids, List<int?> employeeS2Ids, List<int?> employeeS3Ids, List<int?> exceptEmployeeIds);
        Task<int> MatrixImport(Dto.MatrixImport matrixImport);
        Task<List<Matrix>> GetAllMatrixChildrensAsync(int projectId, int costCenterId);
        Task<List<Matrix>> GetMatchMatrixAsync(int divisionId);
        Task<int> Import(Dto.MatrixImport matrixImport);
    }
}
