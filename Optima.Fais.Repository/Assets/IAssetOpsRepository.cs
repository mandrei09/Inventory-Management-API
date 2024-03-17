using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAssetOpsRepository : IRepository<Model.AssetOp>
    {
        //IEnumerable<Dto.AssetOpSd> GetByAsset(int assetId);
        IEnumerable<Model.AssetOp> GetByAsset(int assetId);
        //IEnumerable<Dto.AssetOpSd> GetOperationDetails(string assetOpState, DateTime startDate, DateTime endDate);
        IEnumerable<Model.AssetOp> GetFiltered(AssetFilter assetFilter, string includes, int? assetId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
        IEnumerable<Model.AssetOp> GetRecoFiltered(AssetFilter assetFilter, string includes, int? assetId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
        IEnumerable<Dto.AssetOpSd> GetAll();
        int AssetOpConfImport(Dto.AssetOpConfirmUpload assetOpConfImport, string userName);
        int AssetOpConfImportBnr(Dto.AssetOpConfirmUpload assetOpConfImport, string userName);
        int AssetOpConfImportPiraeus(Dto.AssetOpConfirmUpload assetOpConfImport, string userName);
        int DeleteAssetOp(int assetOpId);
        int ValidateAssetOp(int assetOpId);
        int DeleteAssetOpReco(int assetOpId);

        IEnumerable<Model.AssetOpExport> ExportAssetOp(AssetFilter assetFilter, List<PropertyFilter> propFilters, string assetOpState);
        IEnumerable<Model.AssetOp> GetSyncDetails(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItem);
    }
}