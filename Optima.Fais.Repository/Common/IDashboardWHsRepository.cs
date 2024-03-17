using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IDashboardWHsRepository : IRepository<Model.Dashboard>
    {
        IEnumerable<Model.Dashboard> GetData(DashboardFilter assetFilter, string includes, Sorting sorting, Paging paging, out DashboardTotal depTotal);
        IEnumerable<Model.Dashboard> GetAssetStatusData(DashboardFilter assetFilter, string includes, Sorting sorting, Paging paging, out DashboardTotal depTotal);
        IEnumerable<Model.InventoryAsset> GetReportData(DashboardFilter assetFilter, string includes, Sorting sorting, Paging paging, out DashboardTotal depTotal);
        IEnumerable<Model.CostCenter> GetCostCentersWithAssets(DashboardFilter dashboardFilter);
        IEnumerable<Model.Division> GetDivisionsWithAssets(DashboardFilter dashboardFilter);
        IEnumerable<Model.Department> GetDepartmentsWithAssets(DashboardFilter dashboardFilter);
        IEnumerable<Model.SubType> GetSubTypesWithAssets(DashboardFilter dashboardFilter);
        IEnumerable<Model.Type> GetTypesWithAssets(DashboardFilter dashboardFilter);
        IEnumerable<Model.AssetType> GetAssetTypesWithAssets(DashboardFilter dashboardFilter);
        IEnumerable<Model.Project> GetProjectsWithAssets(DashboardFilter dashboardFilter);
		IEnumerable<Model.RequestKanban> GetRequests(DashboardFilter dashboardFilter);
		IEnumerable<Model.RequestListKanban> GetListRequests(DashboardFilter dashboardFilter);
	}
}
