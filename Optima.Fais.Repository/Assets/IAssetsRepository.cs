using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IAssetsRepository : IRepository<Model.Asset>
    {
        //IEnumerable<Dto.AssetInvDetail> GetInvDetailsByFilters(string filter, List<int> assetCategoryIds, List<int> assetTypeIds, List<int> partnerIds, 
        //    List<int> departmentIds, List<int> employeeIds, List<int> locationIds, List<int> roomIds, List<int> costCenterIds, bool? custody,
        //    string sortColumn, string sortDirection, int? page, int? pageSize, out int count);

        //IEnumerable<Dto.AssetDepDetail> GetDepDetailsByFilters(int accMonthId, string filter, List<int> assetTypeIds, List<int> accStateIds, 
        //    List<int> assetClassIds, List<int> costCenterIds, List<int> partnerIds,
        //    string sortColumn, string sortDirection, int? page, int? pageSize, out Dto.AssetDepTotal assetDepTotal);

        IEnumerable<Dto.AssetDepDetail> GetDepDetails(Paging paging, Sorting sorting, 
            AssetFilter assetFilter, out Dto.AssetDepTotal assetDepTotal);

        Model.Asset GetDetailsById(int assetId, string includes);
        IEnumerable<Model.Asset> GetDetails(AssetFilter assetFilter, string includes, Paging paging, Sorting sorting, out int count);
        IEnumerable<Model.Asset> GetAssetsOut(AssetFilter assetFilter, string includes, Paging paging, Sorting sorting, out int count);
        IEnumerable<Model.AssetDetail> Get(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonth(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetAcquisitionMonth(AssetFilter assetFilter, string docNo1, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthScrap(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthSold(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthClosed(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthWFH(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthInvPlus(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthSuspended(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthInUse(AssetFilter assetFilter, ColumnAssetFilter propFilters, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
		IEnumerable<Model.AssetMonthDetail> GetMonthTemps(AssetFilter assetFilter, List<PropertyFilter> propFilters, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
		IEnumerable<Model.AssetMonthDetailExport> GetMonthExportInUse(AssetFilter assetFilter, List<PropertyFilter> propFilters);
		IEnumerable<Model.AssetMonthDetailExport> GetMonthExportReceptionHistoryInUse(AssetFilter assetFilter, List<PropertyFilter> propFilters);
        IEnumerable<Model.AssetMonthDetailExport> GetMonthExportScrap(AssetFilter assetFilter, List<PropertyFilter> propFilters);
        IEnumerable<Model.AssetMonthDetail> GetMonthStockHistory(AssetFilter assetFilter, int? bfId, bool historyMeniu, List<PropertyFilter> propFilters, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
		IEnumerable<Model.AssetMonthDetail> GetMonthBudgetForecastCorrection(AssetFilter assetFilter, int? bfId, bool historyMeniu, List<PropertyFilter> propFilters, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
		IEnumerable<Model.AssetMonthDetailExport> GetMonthExportStockHistory(AssetFilter assetFilter, int? bfId, List<PropertyFilter> propFilters);
		IEnumerable<Model.AssetMonthDetail> GetMonthInEmployeePersonel(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
		IEnumerable<Model.AssetMonthDetail> GetMonthInEmployeeWFH(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
		IEnumerable<Model.AssetMonthDetail> GetMonthInEmployeePersonelValidate(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthInEmployeeManager(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthInEmployeeManagerValidate(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthInReception(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
		IEnumerable<Model.AssetMonthDetail> GetMonthInPreReception(AssetFilter assetFilter, ColumnAssetFilter propFilters, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
		IEnumerable<Model.AssetMonthDetail> GetMonthInReceptionHistory(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
		IEnumerable<Model.AssetMonthDetail> GetMonthStockIT(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthStockITMFX(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthStockITToValidate(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthStockITToValidateEmployee(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthRejection(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthValidate(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
        IEnumerable<Model.AssetMonthDetail> GetMonthReco(AssetFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);
		IEnumerable<Model.AssetMonthDetail> GetMonthExport(AssetFilter assetFilter, string includes);
        IEnumerable<Model.AssetInventoryDetail> GetInventoryExport(AssetInventoryFilter assetFilter, string includes);
        IEnumerable<Model.AssetMonthDetail> GetComponent(string includes, Sorting sorting, Paging paging, string filter, out AssetDepTotal depTotal);
		IEnumerable<Model.AssetMonthDetail> AddNewAssetValidate(string includes, Sorting sorting, Paging paging, string filter, out AssetDepTotal depTotal);

		IQueryable<Dto.InventoryAsset> GetInventoryAssetsQuery(int inventoryId);
        IQueryable<Model.InventoryAsset> GetInventoryAssetsQuery2(int inventoryId, string includes);
        IQueryable<Model.AssetAdmMD> GetAssetAdmMDsQuery(int inventoryId, int AccMonthId, string includes);
        IQueryable<Model.InventoryAsset> GetInventoryAssetsQueryTotal(int inventoryId, string includes);
        //IEnumerable<Model.AssetInventoryDetail> GetInventory(AssetInventoryFilter assetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal);

        IEnumerable<Dto.InventoryAsset> GetInventoryAssetsByFilters(int inventoryId, string filter, string reportType, string assetState, bool? custody,
            List<int> assetCategoryIds, List<int> assetTypeIds, List<int> partnerIds, 
            List<int> regionIdsIni, List<int> costCenterIdsIni, List<int> admCenterIdsIni, List<int> departmentIdsIni, List<int> employeeIdsIni, List<int> locationIdsIni, List<int> roomIdsIni,
            List<int> regionIdsFin, List<int> costCenterIdsFin, List<int> admCenterIdsFin, List<int> departmentIdsFin, List<int> employeeIdsFin, List<int> locationIdsFin, List<int> roomIdsFin,
            string sortColumn, string sortDirection, int? page, int? pageSize, out int count);

        IEnumerable<Model.InventoryAsset> GetInventoryAssetsByFilters2(int inventoryId, string includes, string filter, string reportType, string assetState, bool? custody, bool? reconcile,bool isExport, string userName, string role, string employeeId,
            List<int?> assetCategoryIds, List<int?> assetTypeIds, List<int?> partnerIds, List<int?> administrationIdsIni, List<int?> administrationIdsFin, List<int?> divisionIdsIni, List<int?> divisionIdsFin,
            List<int?> invStateIdsIni, List<int?> invStateIdsFin, List<int?> invStateIdsAll,
            List<string> userIds,
            List<int?> regionIdsIni, List<int?> costCenterIdsIni, List<int?> admCenterIdsIni, List<int?> departmentIdsIni, List<int?> employeeIdsIni, List<int?> locationIdsIni, List<int?> roomIdsIni,
            
            List<int?> regionIdsFin, List<int?> costCenterIdsFin, List<int?> admCenterIdsFin, List<int?> departmentIdsFin, List<int?> employeeIdsFin, List<int?> locationIdsFin, List<int?> roomIdsFin,
            List<int?> admCenterIdsAll, List<int?> employeeIdsAll, List<int?> locationIdsAll, List<int?> regionIdsAll, List<int?> costCenterIdsAll, List<int?> divisionIdsAll, List<int?> departmentIdsAll, List<int?> administrationIdsAll, List<int?> roomIdsAll,
            string sortColumn, string sortDirection, int? page, int? pageSize, out int count);

		IEnumerable<Model.InventoryAsset> GetInventoryAllAssetsByFilters2(int inventoryId, string includes, string filter, string reportType, string assetState, bool? custody, bool? reconcile, bool isExport, string userName, string role, string employeeId,
			List<int?> assetCategoryIds, List<int?> assetTypeIds, List<int?> partnerIds, List<int?> administrationIdsIni, List<int?> administrationIdsFin, List<int?> divisionIdsIni, List<int?> divisionIdsFin,
			List<int?> invStateIdsIni, List<int?> invStateIdsFin, List<int?> invStateIdsAll,
			List<string> userIds,
			List<int?> regionIdsIni, List<int?> costCenterIdsIni, List<int?> admCenterIdsIni, List<int?> departmentIdsIni, List<int?> employeeIdsIni, List<int?> locationIdsIni, List<int?> roomIdsIni,

			List<int?> regionIdsFin, List<int?> costCenterIdsFin, List<int?> admCenterIdsFin, List<int?> departmentIdsFin, List<int?> employeeIdsFin, List<int?> locationIdsFin, List<int?> roomIdsFin,
			List<int?> admCenterIdsAll, List<int?> employeeIdsAll, List<int?> locationIdsAll, List<int?> regionIdsAll, List<int?> costCenterIdsAll, List<int?> divisionIdsAll, List<int?> departmentIdsAll, List<int?> administrationIdsAll, List<int?> roomIdsAll,
			string sortColumn, string sortDirection, int? page, int? pageSize, out int count);

		IEnumerable<Model.InventoryAsset> GetInventoryEmail(int inventoryId, int appStateId, string includes, string filter, bool? custody,
			  List<int?> invStateIdsAll, List<int?> employeeIdsAll, List<int?> locationIdsAll, List<int?> regionIdsAll, List<int?> roomIdsAll, List<int?> companyIds,
			   string sortColumn, string sortDirection, int? page, int? pageSize, out int count);

		IEnumerable<Model.InventoryAsset> GetInventoryTempAssetsByFilters2(int inventoryId, string includes, List<string> filters, string conditionType, string reportType, bool? custody, string userName, string role, string employeeId,
			   List<int?> regionIds, List<int?> costCenterIds, // List<int?> admCenterIds, List<int?> departmentIds, 
               List<int?> employeeIds,
               List<int?> countyIds, List<int?> cityIds, List<int?> locationIds,
               List<int?> invStateIds, List<string> userIds, List<int?> companyIds, List<int?> divisionIds, List<int?> departmentIds, List<int?> uomIds, List<int?> dimensionIds,
               List<int?> roomIds,
               string sortColumn, string sortDirection, int? page, int? pageSize, out int count);

		IEnumerable<Model.InventoryAsset> EmployeeValidate(int inventoryId, int documentTypeId, string includes, string filter, string userId, string reportType, //string assetState, 
		  bool? custody,
		  bool? allowLabel, bool isExport,
		  string sortColumn, string sortDirection, int? page, int? pageSize, out int count);

		IEnumerable<Model.InventoryAsset> GetInventoryChart(out int count);

        // int AddAsset(Dto.AddAsset asset, out List<Dto.CreateAssetSAP> createAsset);
        Task<Model.CreateAssetSAPResult> CreateAssetSAP(Dto.AddAsset asset);
		Task<Model.CreateAssetSAPResult> CreatePreReceptionAssetSAP(Dto.AddAsset asset);
		Task<Model.CreateAssetSAPResult> ApprovePreReceptionAssetSAP(int asset);
		Model.CreateAssetSAPResult CreateAssetInvPlusSAP(Dto.AddAssetInvPlus asset);
        int AssetChange(Dto.AssetChangeDTO asset, out List<Dto.AssetChangeSAP> validateAsset);
        Task<Model.CreateAssetSAPResult> AcquisitionAssetSAP(Dto.AssetAcquisition asset);
		Task<Model.CreateAssetSAPResult> StornoAcquisitionAssetSAP(Dto.AssetAcquisition asset);
		Task<Model.CreateAssetSAPResult> StornoAcquisitionNoPOAssetSAP(Dto.AssetAcquisition asset);
		Task<Model.CreateAssetSAPResult> ValidateAcquisitionAssetSAP(Dto.AssetAcquisition asset);
		Task<Model.CreateAssetSAPResult> ValidateStornoAcquisitionAssetSAP(Dto.AssetAcquisition asset);
		Task<Model.CreateAssetSAPResult> ValidateStornoNoPOAcquisitionAssetSAP(Dto.AssetAcquisition asset);
		// int TransferAsset(Dto.SaveAssetTransfer asset, out List<Dto.TransferAssetSAP> acquisitionAsset);
		Model.CreateAssetSAPResult TransferAssetSAP(Dto.SaveAssetTransfer asset);
        Model.CreateAssetSAPResult TransferCloneAssetSAP(Dto.SaveAssetCloneTransfer asset);
        // int RetireAsset(Dto.SaveRetireAsset asset, out List<Dto.RetireAssetSAP> acquisitionAsset);
        Model.CreateAssetSAPResult RetireAsset(Dto.SaveRetireAsset asset);
		Task<Model.CreateAssetSAPResult> PublicRetireAsset(Dto.SaveRetireAsset asset);
		Model.CreateAssetSAPResult StornoAsset(Dto.SaveStornoAsset asset);
        Task<Model.CreateAssetSAPResult> StornoAssetMFX(Dto.SaveStornoAssetMFX asset);
        Model.CreateAssetSAPResult StornoAcquisitionAsset(Dto.SaveAssetAcquisitionStorno asset);
        Model.CreateAssetSAPResult AssetInvPlus(Dto.SaveAssetInvPlus asset);
        Model.CreateAssetSAPResult AssetInvMinus(Dto.SaveAssetInvMinus asset);
        // int AddAssetStock(Dto.AddStockAsset asset, out List<Dto.TransferInStockInput> createAsset);
        Model.CreateAssetSAPResult AddAssetStock(Dto.AddStockAsset asset);
        int CreateOrUpdateAsset(Dto.AssetSave asset);
        int CloneAsset(Dto.AssetClone asset);
		Task<Model.UpdateAssetSAPResult> UpdateAcquisition(Dto.AssetAcquisition asset);
		Task<Model.UpdateAssetSAPResult> UpdatePreAcquisition(Dto.AssetPreAcquisition asset);
		Task<Model.UpdateAssetSAPResult> UpdateParentNumber(Dto.AssetPreAcquisition asset);
		Task<Model.UpdateAssetSAPResult> UpdateAssetChange(Dto.AssetEditChange asset);
        Model.CreateAssetSAPResult UpdateAssetInvPlus(Dto.UpdateAssetInvPlus asset);
        Model.AssetResult DeleteOrderItem(int assetId);
        int DeleteAssetOp(int assetId, int inventoryId);
        int AssetImportV1(Dto.AssetImportV1 assetImport);
        int AssetImportV2(Dto.AssetImportV2 assetImport);
        int ImportThales(Dto.ImportThales assetImport);
        int ImportITThales(Dto.ImportITThales assetImport);
        Task<Model.ImportITMFXResult> ImportITMFX(Dto.ImportITMFX import);
		Task<Model.ImportPrintLabelResult> ImportPrintLabel(Dto.ImportPrintLabel import);
		int AssetImportV3(Dto.AssetImportV3 assetImport);
        IQueryable<Model.InventoryAsset> GetInventoryAssetReportsQuery2(int inventoryId, string includes);

        int DeleteAsset(int assetId);


        //Model.Asset GetAssetDetail(int assetId);
        //Dto.AssetDetail GetAssetDetail(int assetId);

        IEnumerable<Model.Asset> GetSync(string includes, int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
        IEnumerable<Dto.Sync.Asset> GetAssetInvSyncDetails(int pageSize, int lastId, DateTime lastModifiedAt);

        IEnumerable<Model.Asset> GetFilteredDetailUI(string includes, int? assetId, int? employeeId, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);

		Task<Model.WFHResult> CreateOrUpdateAssetEmployee(Dto.AssetEmployeeSave asset,int employeeId);
		Task<Model.WFHResult> DeleteAssetValidation(int assetId, int employeeId);
        IEnumerable<Model.InventoryAsset> EmployeeValidateNew(string includes, string role, int employeeId, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);

        // Task<List<Model.Reporting.InventoryListAppendixA>> GetInventoryListAsync(int inventoryId, int? costCenterId);
        Task<List<Model.Reporting.InventoryListAppendixA>> GetInventoryListAsync(int inventoryId, ReportFilter reportFilter);
		Task<List<Model.Reporting.InventoryListAppendixA>> GetInventoryMinusListAsync(int inventoryId, ReportFilter reportFilter);
		Task<List<Model.Reporting.InventoryListAppendixA>> GetInventoryPlusListAsync(int inventoryId, ReportFilter reportFilter);
		Task<List<Model.Reporting.InventoryListAppendixA>> GetInventoryListAllowLabelAsync(int inventoryId, ReportFilter reportFilter);
		Task<List<Model.AuditInventoryResult>> GetCostCenterAuditInventoryAsync(int inventoryId, ReportFilter reportFilter);
        int UpdateAssetSapValidation(AssetSapValidationSave assetSapValidation);
        Task<byte[]> ExportPrereceptionAsync(AssetFilter assetFilter, string includes);
        Task<List<AuditInventoryResult>> GetLocationAuditInventoryAsync(int inventoryId, ReportFilter reportFilter);
    }
}
