using Optima.Fais.Dto.Reporting;
using Optima.Fais.Model.Reporting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IReportingRepository
    {
		//AccNotice GetAccNoticeData(int documentId);

		//Annulement GetAnnulementData(int documentId);

		//Commissioning GetCommissioningData(int documentId);

		//DifferenceList GetDifferenceListData(int documentId);

		//ExternalEntryNote GetExternalEntryNoteData(int documentId);

		//InternalEntryNote GetInternalEntryNoteData(int documentId);

		//FixedAssetReport GetFixedAssetReportData(string invNo);

		//InventoryList GetInventoryListData(int locationId);

		//InvNoRegistry GetInvNoRegistryData();

		TrasferSG GetMovementProvidingData(int documentId, int? assetOpId);

		SGScrab GetScrabProvidingData(int documentId, int? assetOpId);

		SGPIF GetReportAssetData(Guid reportId, int assetId);

		NIRSG GetReportNIRData(Guid reportId, int assetId);

		InventoryResult GetInventoryListEmployees(int? inventoryId, int? admCenterId);

        InventoryReconciliation GetInventoryListReconciliations(int? inventoryId, int? admCenterId);

        InventoryUserScan GetInventoryListUserScans(int? inventoryId, int? employeeId, int? admCenterId);

        InventoryUserBuildingScan GetInventoryListUserBuildingScans(int? inventoryId);

        //MovementReport GetMovementReportData(int documentId);

        //  InventoryResult GetInventoryListByFilters(int? inventoryId, int? locationId, int? employeeId, bool isTransfer, bool isCassation);

        //InventoryResult GetInventoryListV2ByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? employeeId, string reportType, bool? custody, string internalCode);
        InventoryResult GetInventoryListV2ByFilters(int? inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? employeeId, string reportType, bool? custody, string internalCode);

        InventoryResult GetInventoryListV2MultipleByFilters(int? inventoryId, int? admCenterId, int? regionId, int? locationId, string reportType);

        InventoryResult GetInventoryListWGByFilters(int inventoryId, int? locationId, string reportType);

        InventoryResult GetEmployeeReportData(string reportType, string internalCode);

        AssetOperation GetAssetOperationData(int? locationId);

        InventoryResult GetGeneralList(int? accMonthId, int inventoryId, int regionId, string reportType);

        InventoryListApaNova GetInventoryListApaNovaImages(int? inventoryId, int? locationId);


        InventoryResult GetInventoryListV2Total(int inventoryId);

        //InventoryResult GetInventoryListV3ByFilters(int inventoryId, int regionId, int? locationId, int employeeId, string reportType, bool? custody);

        TransferInV1Result GetTransferInV1ByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, string reportType, bool? custody);
        //TransferInV1Result GetTransferInV1ByFilters(int inventoryId, int locationId);

        TransferInV1Result GetTransferInV1MultipleByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, string reportType, bool? custody);

        TransferOutV1Result GetTransferOutV1ByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, bool? custody);

        TransferOutV1Result GetTransferOutV1MultipleByFilters(int inventoryId, int? admCenterId, int? regionId, int? locationId);

        InventoryResultEmag GetInventoryListEmagByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? companyId, int? administrationId, int? roomId, string reportFilter);

        InventoryResultEmployeeEmag GetInventoryListEmpEmagByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? companyId, int? administrationId, int? roomId);

        InventoryResultEmployeeEmag GetInventoryListEmpFinalEmagByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? companyId, int? administrationId, int? roomId);

        AuditInventoryResult GetInventoryResultByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? companyId, int? administrationId, int? roomId, string reportFilter);

        InventoryLabel GetInventoryLabelByFilters(int inventoryId, int? costCenterId);
    }
}
