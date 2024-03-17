using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Dto.Reporting;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Optima.Fais.Dto;
using Optima.Fais.Model.Utils;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Optima.Fais.Model.Reporting;

namespace Optima.Fais.EfRepository
{
    public class ReportingRepository : IReportingRepository
    {
        protected ApplicationDbContext _context = null;
        private IAssetsRepository _assetsRepository = null;
        private IAssetNiRepository _assetNiRepository = null;

        public ReportingRepository(ApplicationDbContext context, IAssetsRepository assetsRepository, IAssetNiRepository assetNiRepository)
        {
            _context = context;
            _assetsRepository = assetsRepository;
            _assetNiRepository = assetNiRepository;
        }

        //public InventoryResult GetInventoryListV3ByFilters(int inventoryId, int regionId, int? locationId, int employeeId, string reportType, bool? custody)
        //{
        //    InventoryResult invResult = null;
        //    Model.Employee employee = null;
        //    Model.Location location = null;
        //    Model.Region region = null;

        //    IQueryable<Dto.InventoryAsset> invAssetQuery = null;

        //    List<Dto.InventoryAsset> listInventoryAssets = null;

        //    employee = _context.Set<Model.Employee>().Single(e => e.Id == employeeId);
        //    location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
        //    region = region = _context.Set<Model.Region>().Single(r => r.Id == regionId);

        //    invAssetQuery = _assetsRepository.GetInventoryAssetsQuery(inventoryId);

        //    invAssetQuery = invAssetQuery.Where(i => i.RegionIdIni == regionId || i.RegionIdFin == regionId);

        //    if (locationId.HasValue && locationId > 0)
        //    {
        //        invAssetQuery = invAssetQuery.Where(i => i.LocationIdIni == locationId || i.LocationIdFin == locationId);
        //    }

        //    if (custody.HasValue)
        //    {
        //        invAssetQuery = invAssetQuery.Where(i => i.Custody == custody.GetValueOrDefault());
        //    }

        //    invAssetQuery = invAssetQuery.Where(e => e.EmployeeIdIni == employeeId || e.EmployeeIdFin == employeeId);

        //    if (reportType != null)
        //    {
        //        switch (reportType.ToUpper())
        //        {
        //            case "MINUS":

        //                invAssetQuery = invAssetQuery.Where(i => i.QFinal == 0);

        //                break;

        //            case "PLUS":

        //                invAssetQuery = invAssetQuery.Where(i => i.QIntial == 0);

        //                break;
        //        }
        //    }

        //    listInventoryAssets = invAssetQuery.ToList();

        //    invResult = new InventoryResult();

        //    invResult.Details = new List<InventoryResultDetail>();

        //    foreach (var inventoryAsset in listInventoryAssets)
        //    {
        //        invResult.Details.Add(new InventoryResultDetail()
        //        {
        //            InvNo = inventoryAsset.InvNo,
        //            Description = inventoryAsset.Name,
        //            SerialNumber = inventoryAsset.SerialNumber,
        //            PurchaseDate = inventoryAsset.PurchaseDate,
        //            //CostCenter = inventoryAsset.CostCenterNameIni,
        //            CostCenter = inventoryAsset.LocationIdIni == locationId
        //                ? inventoryAsset.CostCenterCodeIni
        //                : inventoryAsset.CostCenterCodeFin,
        //            //Room = (inventoryAsset.RoomNameFin != null && inventoryAsset.RoomNameFin != string.Empty) ? inventoryAsset.RoomNameFin : inventoryAsset.RoomNameIni,
        //            Room = inventoryAsset.EmployeeIdIni == employeeId
        //                ? (inventoryAsset.LocationIdIni != inventoryAsset.LocationIdFin ? inventoryAsset.RoomNameIni : inventoryAsset.RoomNameFin)
        //                : inventoryAsset.RoomNameFin,
        //            Initial = inventoryAsset.EmployeeIdIni == employeeId ? inventoryAsset.QIntial : 0,
        //            Actual = inventoryAsset.EmployeeIdFin == employeeId ? inventoryAsset.QFinal : 0,
        //            Uom = inventoryAsset.Uom,
        //            Value = inventoryAsset.ValueInv,
        //            ValueInv = inventoryAsset.ValueInv,
        //            ValueDep = inventoryAsset.ValueDep,
        //            ValueDepTotal = inventoryAsset.ValueDep,
        //            Info = string.Empty,
        //            Custody = inventoryAsset.Custody.HasValue ? (inventoryAsset.Custody.Value ? "DA" : "NU") : string.Empty,
        //            InternalCode = (inventoryAsset.InternalCodeFin != null && inventoryAsset.InternalCodeFin != string.Empty) ? inventoryAsset.InternalCodeFin : inventoryAsset.InternalCodeIni,
        //            FullName = (inventoryAsset.LastNameFin != null && inventoryAsset.LastNameFin != string.Empty) ? inventoryAsset.FirstNameFin + " " + inventoryAsset.LastNameFin : inventoryAsset.FirstNameIni + " " + inventoryAsset.LastNameIni
        //        });
        //    }

        //    invResult.AdmCenterName = region.Name;
        //    invResult.CompanyName = "";
        //    invResult.EndDate = DateTime.Now;
        //    invResult.LocationName = (location != null) ? location.Name : string.Empty;

        //    invResult.EmployeeInternalCode = (employee != null) ? employee.InternalCode : string.Empty;
        //    invResult.EmployeeFirstName = (employee != null) ? employee.FirstName : string.Empty;
        //    invResult.EmployeeLastName = (employee != null) ? employee.LastName : string.Empty;

        //    return invResult;
        //}

        //public InventoryResult GetInventoryListV2ByFilters(int inventoryId, int regionId, int? locationId, string reportType, bool? custody)
        //{
        //    InventoryResult invResult = null;
        //    Model.Location location = null;
        //    Model.Region region = null;

        //    IQueryable<Dto.InventoryAsset> invAssetQuery = null;

        //    List<Dto.InventoryAsset> listInventoryAssets = null;

        //    location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
        //    region = _context.Set<Model.Region>().Single(r => r.Id == regionId);

        //    invAssetQuery = _assetsRepository.GetInventoryAssetsQuery(inventoryId);

        //    invAssetQuery = invAssetQuery.Where(i => i.RegionIdIni == regionId || i.RegionIdFin == regionId);

        //    if (locationId.HasValue && locationId > 0)
        //    {
        //        invAssetQuery = invAssetQuery.Where(i => i.LocationIdIni == locationId || i.LocationIdFin == locationId);
        //    }

        //    if (custody.HasValue)
        //    {
        //        invAssetQuery = invAssetQuery.Where(i => i.Custody == custody.GetValueOrDefault());
        //    }

        //    if (reportType != null)
        //    {
        //        switch (reportType.ToUpper())
        //        {
        //            case "MINUS":

        //                invAssetQuery = invAssetQuery.Where(i => i.QFinal == 0);

        //                break;

        //            case "PLUS":

        //                invAssetQuery = invAssetQuery.Where(i => i.QIntial == 0);

        //                break;
        //        }
        //    }

        //    listInventoryAssets = invAssetQuery.ToList();

        //    invResult = new InventoryResult();

        //    invResult.Details = new List<InventoryResultDetail>();

        //    foreach (var inventoryAsset in listInventoryAssets)
        //    {
        //        invResult.Details.Add(new InventoryResultDetail()
        //        {
        //            InvNo = inventoryAsset.InvNo,
        //            Description = inventoryAsset.Name,
        //            SerialNumber = inventoryAsset.SerialNumber,
        //            PurchaseDate = inventoryAsset.PurchaseDate,
        //            //CostCenter = inventoryAsset.CostCenterNameIni,
        //            //Room = (inventoryAsset.RoomNameFin != null && inventoryAsset.RoomNameFin != string.Empty) ? inventoryAsset.RoomNameFin : inventoryAsset.RoomNameIni,

        //            //CostCenter = inventoryAsset.CostCenterNameIni,
        //            CostCenter = inventoryAsset.LocationIdIni == locationId
        //                ? inventoryAsset.CostCenterCodeIni
        //                : inventoryAsset.CostCenterCodeFin,
        //            //Room = (inventoryAsset.RoomNameFin != null && inventoryAsset.RoomNameFin != string.Empty) ? inventoryAsset.RoomNameFin : inventoryAsset.RoomNameIni,
        //            Room = inventoryAsset.LocationIdIni == locationId
        //                ? (inventoryAsset.LocationIdIni != inventoryAsset.LocationIdFin ? inventoryAsset.RoomNameIni : inventoryAsset.RoomNameFin)
        //                : inventoryAsset.RoomNameFin,

        //            Initial = inventoryAsset.LocationIdIni == locationId ? inventoryAsset.QIntial : 0,
        //            Actual = inventoryAsset.LocationIdFin == locationId ? inventoryAsset.QFinal : 0,
        //            Uom = inventoryAsset.Uom,
        //            Value = inventoryAsset.ValueInv,
        //            ValueInv = inventoryAsset.ValueInv,
        //            ValueDep = inventoryAsset.ValueDep,
        //            ValueDepTotal = inventoryAsset.ValueDep,
        //            Info = string.Empty,
        //            Custody = inventoryAsset.Custody.HasValue ? (inventoryAsset.Custody.Value ? "DA" : "NU") : string.Empty,
        //            InternalCode = (inventoryAsset.InternalCodeFin.ToUpper().Contains("_")) ?
        //                                    string.Empty :
        //                                    (inventoryAsset.InternalCodeFin != null && inventoryAsset.InternalCodeFin != string.Empty) ? inventoryAsset.InternalCodeFin : inventoryAsset.InternalCodeIni,
        //            FullName = (inventoryAsset.InternalCodeFin.ToUpper().Contains("_")) ?
        //                                    string.Empty : (inventoryAsset.LastNameFin != null && inventoryAsset.LastNameFin != string.Empty) ?
        //                                    inventoryAsset.FirstNameFin + " " + inventoryAsset.LastNameFin : inventoryAsset.FirstNameIni + " " + inventoryAsset.LastNameIni
        //        });
        //    }

        //    invResult.AdmCenterName = region.Name;
        //    invResult.CompanyName = "";
        //    invResult.EndDate = DateTime.Now;
        //    invResult.LocationName = (location != null) ? location.Name : string.Empty;

        //    return invResult;
        //}

        //public TransferOutV1Result GetTransferOutV1ByFilters(int inventoryId, int regionId, int? locationId)
        //{
        //    TransferOutV1Result transferOutV1Result = null;
        //    Model.Location location = null;
        //    Model.Region region = null;

        //    IQueryable<Dto.InventoryAsset> invAssetQuery = null;

        //    List<Dto.InventoryAsset> listInventoryAssets = null;

        //    Model.InvState outInvState = _context.Set<Model.InvState>().First(s => s.Code == "OUT");
        //    location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
        //    region = region = _context.Set<Model.Region>().Single(r => r.Id == regionId);

        //    invAssetQuery = _assetsRepository.GetInventoryAssetsQuery(inventoryId);

        //    invAssetQuery = invAssetQuery.Where(i => i.RegionIdIni == regionId || i.RegionIdFin == regionId);

        //    if (locationId.HasValue && locationId > 0)
        //    {
        //        invAssetQuery = invAssetQuery.Where(i => i.LocationIdIni == locationId || i.LocationIdFin == locationId);
        //    }

        //    invAssetQuery = invAssetQuery.Where(i => i.InvStateIdFin == outInvState.Id);

        //    listInventoryAssets = invAssetQuery.ToList();

        //    transferOutV1Result = new TransferOutV1Result();

        //    transferOutV1Result.Details = new List<TransferOutV1Detail>();

        //    foreach (var inventoryAsset in listInventoryAssets)
        //    {
        //        transferOutV1Result.Details.Add(new TransferOutV1Detail()
        //        {
        //            InvNo = inventoryAsset.InvNo,
        //            Description = inventoryAsset.Name,
        //            PurchaseDate = inventoryAsset.PurchaseDate,
        //            RegionName = inventoryAsset.RegionNameFin,
        //            LocationName = inventoryAsset.LocationNameFin,
        //            CostCenter = inventoryAsset.CostCenterNameIni,
        //            Room = (inventoryAsset.RoomNameFin != null && inventoryAsset.RoomNameFin != string.Empty) ? inventoryAsset.RoomNameFin : inventoryAsset.RoomNameIni,
        //            Value = inventoryAsset.ValueInv,
        //            ValueDep = inventoryAsset.ValueDep,
        //            EmployeeInternalCode = (inventoryAsset.InternalCodeFin.ToUpper().Contains("_")) ? string.Empty : inventoryAsset.InternalCodeFin,
        //            EmployeeFirstName = (inventoryAsset.InternalCodeFin.ToUpper().Contains("_")) ? string.Empty : inventoryAsset.FirstNameFin,
        //            EmployeeLastName = (inventoryAsset.InternalCodeFin.ToUpper().Contains("_")) ? string.Empty : inventoryAsset.LastNameFin,
        //            Info = string.Empty
        //        });
        //    }

        //    transferOutV1Result.AdmCenterName = region.Name;
        //    transferOutV1Result.CompanyName = "";
        //    //transferOutV1Result.CompanyName = "";
        //    transferOutV1Result.EndDate = DateTime.Now;
        //    transferOutV1Result.LocationName = (location != null) ? location.Name : string.Empty;

        //    return transferOutV1Result;
        //}

        public TransferOutV1Result GetTransferOutV1ByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, bool? custody)
        {
            TransferOutV1Result transferOutV1Result = null;
            Model.AdmCenter admCenter = null;
            Model.Region region = null;
            Model.Location location = null;
            //Model.Region region = null;
            Model.Inventory inventory = null;
            Model.AccMonth accMonth = null;
            IQueryable<Model.Asset> assetQuery = null;
            IQueryable<AssetDepMD> depQuery = null;
            IQueryable<Model.InventoryAsset> invAssetQuery = null;
            List<Model.InventoryAsset> listInventoryAssets = null;
            IQueryable<Model.AssetInventoryDetail> query = null;
            List<Model.AssetInventoryDetail> listAssetInventoryDetails = null;

            int accSystemId = 1;

            if (inventoryId > 0)
            {
                inventory = _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId).SingleOrDefault();

                if (inventory != null)
                {
                    accMonth = _context.Set<Model.AccMonth>().Where(a => a.Id == inventory.AccMonthId).SingleOrDefault();
                }
            }

            //Model.InvState outInvState = _context.Set<Model.InvState>().First(s => s.Id == 22);
            //List<int?> invStateOutIds = _context.Set<Model.InvState>().Where(s => s.ParentCode == "INV_COMP_STATE" && s.Code.StartsWith("OUT")).Select(s => (int?)s.Id).ToList();
            List<int?> invStateOutIds = _context.Set<Model.InvState>().Where(s => s.Id == 22 || s.Id == 21).Select(s => (int?)s.Id).ToList();
            location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;

            admCenter = (admCenterId.HasValue && admCenterId > 0) ? _context.Set<Model.AdmCenter>().Single(a => a.Id == admCenterId) : null;
            region = (regionId.HasValue && regionId > 0) ? _context.Set<Model.Region>().Single(r => r.Id == regionId) : null;

            //invAssetQuery = _assetsRepository.GetInventoryAssetsQuery2(inventoryId, null);

            assetQuery = _context.Assets
                .Include(u => u.Uom)
               .Include(r => r.Room)
                   .ThenInclude(l => l.Location)
                        .ThenInclude(l => l.Region)
                 //.Include(r => r.Room)
                 //  .ThenInclude(l => l.Location)
                 //       .ThenInclude(l => l.AdmCenter)
               .Include(c => c.CostCenter)
                   //.ThenInclude(a => a.AdmCenter)
               .Include(e => e.Employee)
               .Include(a => a.AssetState)
               .AsQueryable();
            depQuery = _context.AssetDepMDs.AsQueryable().Where(a => a.AccSystemId == accSystemId && a.AccMonthId == accMonth.Id);

            invAssetQuery = _context.InventoryAssets
                .Include(r => r.RoomInitial)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                  //.Include(r => r.RoomInitial)
                  //  .ThenInclude(l => l.Location)
                  //     .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterInitial)
                    //.ThenInclude(a => a.AdmCenter)
                .Include(e => e.EmployeeInitial)
                .Include(s => s.StateInitial)
                .Include(r => r.RoomFinal)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                  //.Include(r => r.RoomFinal)
                  //  .ThenInclude(l => l.Location)
                  //     .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterFinal)
                    //.ThenInclude(r => r.AdmCenter)
                 .Include(e => e.EmployeeFinal)
                  .Include(s => s.StateFinal)

                .Where(i => i.InventoryId == inventoryId).AsQueryable();


            query = assetQuery.Select(asset => new Model.AssetInventoryDetail { Asset = asset });


            query = query
                .Join(invAssetQuery, q => q.Asset.Id, inv => inv.AssetId, (q, inv) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = inv });



            query = query
                .Join(depQuery, q => q.Asset.Id, dep => dep.AssetId, (q, dep) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = q.Inventory, Dep = dep });



            if (costCenterId.HasValue && (costCenterId > 0))
            {
                //invAssetQuery = invAssetQuery.Where(i => ((i.CostCenterIdInitial == costCenterId)
                //    || (i.CostCenterIdFinal == costCenterId)));
                invAssetQuery = invAssetQuery.Where(i => i.CostCenterIdFinal == costCenterId);
            }
            else
            {
                if (admCenterId.HasValue && (admCenterId > 0))
                {
                    //invAssetQuery = invAssetQuery.Where(i => i.CostCenterInitial.AdmCenterId == admCenterId
                    //    || ((i.CostCenterIdFinal != null) && (i.CostCenterFinal.AdmCenterId == admCenterId)));
                    query = query.Where(i => i.Inventory.CostCenterFinal.AdmCenterId == admCenterId);
                }
            }

            if (regionId.HasValue && regionId > 0)
            {
                //invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.Location.RegionId == regionId || i.RoomFinal.Location.RegionId == regionId);
                query = query.Where(i => i.Inventory.RoomFinal.Location.RegionId == regionId);
            }

            if (locationId.HasValue && locationId > 0)
            {
                //invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.LocationId == locationId || i.RoomFinal.LocationId == locationId);
                query = query.Where(i => i.Inventory.RoomFinal.LocationId == locationId);
            }

            if (custody.HasValue)
            {
                query = query.Where(i => i.Asset.Custody == custody.GetValueOrDefault());
            }

            query = query.Where(i => i.Inventory.StateIdFinal == 22 || i.Inventory.StateIdFinal == 21);

            //query = query.Where(i => invStateOutIds.Contains(i.Inventory.StateIdFinal));

            listAssetInventoryDetails = query.ToList();

            transferOutV1Result = new TransferOutV1Result();

            transferOutV1Result.Details = new List<TransferOutV1Detail>();

            foreach (var inventoryAsset in listAssetInventoryDetails)
            {
                TransferOutV1Detail transfer = new TransferOutV1Detail()
                {
                    InvNo = inventoryAsset.Asset.InvNo,
                    Description = inventoryAsset.Asset.Name,
                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                    LocationName = inventoryAsset.Inventory.RoomFinal.Location.Name,
                    Room = (inventoryAsset.Inventory.RoomFinal != null && inventoryAsset.Inventory.RoomFinal.Name != string.Empty) ? inventoryAsset.Inventory.RoomFinal.Name : inventoryAsset.Inventory.RoomInitial.Name,
                    Value = inventoryAsset.Dep.CurrentAPC,
                    ValueDep = inventoryAsset.Dep.AccumulDep,
                    EmployeeInternalCode = (inventoryAsset.Inventory.EmployeeFinal != null && inventoryAsset.Inventory.EmployeeFinal.InternalCode.ToUpper().Contains("_")) ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.InternalCode,
                    EmployeeFirstName = (inventoryAsset.Inventory.EmployeeFinal != null && inventoryAsset.Inventory.EmployeeFinal.InternalCode.ToUpper().Contains("_")) ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.FirstName,
                    EmployeeLastName = (inventoryAsset.Inventory.EmployeeFinal != null && inventoryAsset.Inventory.EmployeeFinal.InternalCode.ToUpper().Contains("_")) ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.LastName,
                    Info = ((inventoryAsset.Inventory.Info != null) && (inventoryAsset.Inventory.Info.Length > 0)) ? inventoryAsset.Inventory.Info : (inventoryAsset.Inventory.StateFinal != null ? inventoryAsset.Inventory.StateFinal.Name : string.Empty),
                    ModifiedAt = inventoryAsset.Inventory.ModifiedAt.Value,
                    SerialNumber = inventoryAsset.Inventory.SerialNumber,
                    RegionName = inventoryAsset.Inventory.RoomFinal.Location.Region.Name
                };

                if (inventoryAsset.Inventory.CostCenterIdFinal != null)
                {
                    transfer.CostCenter = inventoryAsset.Inventory.CostCenterFinal.Code;
                    if (inventoryAsset.Inventory.CostCenterFinal.AdmCenterId != null)
                        transfer.RegionName = inventoryAsset.Inventory.CostCenterFinal.AdmCenter.Code;
                }
                else
                {
                    if (inventoryAsset.Inventory.CostCenterIdInitial != null)
                    {
                        transfer.CostCenter = inventoryAsset.Inventory.CostCenterInitial.Code;
                        if (inventoryAsset.Inventory.CostCenterInitial.AdmCenterId != null)
                            transfer.RegionName = inventoryAsset.Inventory.CostCenterInitial.AdmCenter.Code;
                    }
                }

                transferOutV1Result.Details.Add(transfer);
            }

            inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);

            var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            transferOutV1Result.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            transferOutV1Result.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;
            transferOutV1Result.RegionName = (region != null) ? region.Name : string.Empty;

            transferOutV1Result.InventoryName = inventory.Description.Trim();
            var maxDate = transferOutV1Result.Details.Count > 0 ? transferOutV1Result.Details.Where(s => s.ModifiedAt == transferOutV1Result.Details.Max(x => x.ModifiedAt)).FirstOrDefault() : null;
            transferOutV1Result.InventoryEndDate = maxDate != null ? (maxDate.ModifiedAt != null ? maxDate.ModifiedAt.Value : DateTime.Now) : DateTime.Now;

            transferOutV1Result.LocationName = (location != null) ? location.Name : string.Empty;

            return transferOutV1Result;



        }

        //public TransferInV1Result GetTransferInV1ByFilters(int inventoryId, int regionId, int? locationId, string reportType)
        //{
        //    string strReportType = string.Empty;
        //    TransferInV1Result transferInV1Result = null;
        //    Model.Location location = null;
        //    Model.Region region = null;

        //    IQueryable<Dto.InventoryAsset> invAssetQuery = null;
        //    List<Dto.InventoryAsset> listInventoryAssets = null;

        //    location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
        //    region = region = _context.Set<Model.Region>().Single(r => r.Id == regionId);

        //    invAssetQuery = _assetsRepository.GetInventoryAssetsQuery(inventoryId);

        //    invAssetQuery = invAssetQuery.Where(i => i.RegionIdIni == regionId || i.RegionIdFin == regionId);

        //    if (locationId.HasValue && locationId > 0) invAssetQuery = invAssetQuery.Where(i => i.LocationIdIni == locationId || i.LocationIdFin == locationId);

        //    if (reportType != null)
        //    {
        //        switch (reportType.ToUpper())
        //        {
        //            case "TRANSFER_ROOM_SAME_LOCATION":
        //                invAssetQuery = invAssetQuery.Where(i => ((i.RoomIdFin != null) && (i.RoomIdIni != i.RoomIdFin) && (i.LocationIdIni == i.LocationIdFin)) || ((i.EmployeeIdFin != null) && (i.EmployeeIdIni != i.EmployeeIdFin) && (i.LocationIdIni == i.LocationIdFin)));
        //                strReportType = "intre camere/responsabili";
        //                break;
        //            case "TRANSFER_LOCATION_SAME_ADMCENTER":
        //                invAssetQuery = invAssetQuery.Where(i => ((i.LocationIdFin != null) && (i.LocationIdIni != i.LocationIdFin) && (i.RegionIdIni == i.RegionIdFin)));
        //                strReportType = "intre cladiri";
        //                break;
        //            case "TRANSFER_LOCATION_DIFF_ADMCENTER":
        //                invAssetQuery = invAssetQuery.Where(i => ((i.LocationIdFin != null) && (i.LocationIdIni != i.LocationIdFin) && (i.RegionIdIni != i.RegionIdFin)));
        //                strReportType = "intre centre logistice";
        //                break;
        //            default:
        //                invAssetQuery = invAssetQuery.Where(i => (i.LocationIdFin != null) && (i.LocationIdIni != i.LocationIdFin) && (i.RegionIdIni == i.RegionIdFin));
        //                strReportType = "intre cladiri";
        //                break;
        //        }
        //    }

        //    listInventoryAssets = invAssetQuery.ToList();

        //    transferInV1Result = new TransferInV1Result();

        //    transferInV1Result.Details = new List<TransferInV1Detail>();

        //    foreach (var inventoryAsset in listInventoryAssets)
        //    {
        //        transferInV1Result.Details.Add(new TransferInV1Detail()
        //        {
        //            InvNo = inventoryAsset.InvNo,
        //            Description = inventoryAsset.Name,
        //            SerialNumber = inventoryAsset.SerialNumber,
        //            PurchaseDate = inventoryAsset.PurchaseDate,
        //            RegionInitial = inventoryAsset.RegionNameIni,
        //            RegionFinal = inventoryAsset.RegionIdFin.HasValue ? inventoryAsset.RegionNameFin : string.Empty,
        //            LocationNameInitial = inventoryAsset.LocationNameIni,
        //            LocationNameFinal = inventoryAsset.LocationIdFin.HasValue ? inventoryAsset.LocationNameFin : string.Empty,
        //            CostCenterInitial = inventoryAsset.CostCenterNameIni,
        //            CostCenterFinal = inventoryAsset.CostCenterIdFin.HasValue ? inventoryAsset.CostCenterNameFin : string.Empty,
        //            RoomInitial = inventoryAsset.RoomNameIni,
        //            RoomFinal = inventoryAsset.RoomIdFin.HasValue ? inventoryAsset.RoomNameFin : string.Empty,
        //            InternalCodeInitial = inventoryAsset.InternalCodeIni,
        //            InternalCodeFinal = inventoryAsset.InternalCodeFin != null ? inventoryAsset.InternalCodeFin : string.Empty,
        //            FullNameInitial = inventoryAsset.FirstNameIni + " " + inventoryAsset.LastNameIni,
        //            FullNameFinal = inventoryAsset.InternalCodeFin != null ? inventoryAsset.FirstNameFin + " " + inventoryAsset.LastNameFin : string.Empty,
        //            Initial = inventoryAsset.QIntial,
        //            Actual = inventoryAsset.QFinal,
        //            Uom = inventoryAsset.Uom,
        //            Value = inventoryAsset.ValueInv,
        //            ValueDepTotal = inventoryAsset.ValueDep,
        //            Custody = inventoryAsset.Custody.HasValue ? (inventoryAsset.Custody.Value ? "DA" : "NU") : string.Empty,
        //        });
        //    }

        //    transferInV1Result.ReportType = strReportType;
        //    transferInV1Result.AdmCenterName = region.Name;
        //    transferInV1Result.CompanyName = "";
        //    //transferInV1Result.CompanyName = "";
        //    transferInV1Result.EndDate = DateTime.Now;
        //    transferInV1Result.LocationName = (location != null) ? location.Name : string.Empty;

        //    return transferInV1Result;
        //}

        public TransferInV1Result GetTransferInV1ByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, string reportType, bool? custody)
        {
            string strReportType = string.Empty;
            TransferInV1Result transferInV1Result = null;
            Model.AdmCenter admCenter = null;
            Model.Location location = null;
            Model.Region region = null;
            Model.Inventory inventory = null;
            IQueryable<Model.InventoryAsset> invAssetQuery = null;
            List<Model.InventoryAsset> listInventoryAssets = null;
            List<Model.AssetInventoryDetail> listAssetInventoryDetails = null;
            IQueryable<Model.Asset> assetQuery = null;
            IQueryable<AssetDepMD> depQuery = null;
            Model.AccMonth accMonth = null;
            IQueryable<Model.AssetInventoryDetail> query = null;

            //if (inventoryId > 0)
            //    inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);
            //else
            //    inventory = _context.Set<Model.Inventory>().OrderByDescending(i => i.Id).FirstOrDefault(i => i.Active == true);

            //if (inventory == null) return null;

            admCenter = (admCenterId.HasValue && admCenterId > 0) ? _context.Set<Model.AdmCenter>().Single(a => a.Id == admCenterId) : null;
            location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
            region = region = _context.Set<Model.Region>().Single(r => r.Id == regionId);
            region = (regionId.HasValue && regionId > 0) ? _context.Set<Model.Region>().Single(r => r.Id == regionId) : null;

            //invAssetQuery = _assetsRepository.GetInventoryAssetsQuery2(inventoryId, null);

            //if (costCenterId.HasValue && (costCenterId > 0))
            //{
            //    invAssetQuery = invAssetQuery.Where(i => ((i.CostCenterIdInitial == costCenterId)
            //        || (i.CostCenterIdFinal == costCenterId)));
            //}
            //else
            //{
            //    if (admCenterId.HasValue && (admCenterId > 0))
            //    {
            //        invAssetQuery = invAssetQuery.Where(i => i.CostCenterInitial.AdmCenterId == admCenterId
            //            || ((i.CostCenterIdFinal != null) && (i.CostCenterFinal.AdmCenterId == admCenterId)));
            //    }
            //}

            //if (locationId.HasValue && (locationId > 0))
            //{
            //    invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.LocationId == locationId
            //        || ((i.RoomIdFinal != null) && (i.RoomFinal.LocationId == locationId)));
            //}
            //else
            //{
            //    if (regionId.HasValue && (regionId > 0))
            //    {
            //        invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.Location.RegionId == regionId
            //            || ((i.RoomIdFinal != null) && (i.RoomFinal.Location.RegionId == regionId)));
            //    }
            //}

            //if (custody.HasValue)
            //{
            //    invAssetQuery = invAssetQuery.Where(i => i.Asset.Custody == custody.GetValueOrDefault());
            //}


            // NEW // 

            int accSystemId = 1;

            if (inventoryId > 0)
            {
                inventory = _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId).SingleOrDefault();

                if (inventory != null)
                {
                    accMonth = _context.Set<Model.AccMonth>().Where(a => a.Id == inventory.AccMonthId).SingleOrDefault();
                }
            }

            assetQuery = _context.Assets
                 .Include(u => u.Uom)
                .Include(r => r.Room)
                    .ThenInclude(l => l.Location)
                        .ThenInclude(l => l.Region)
                .Include(c => c.CostCenter)
                    //.ThenInclude(a => a.AdmCenter)
                .Include(e => e.Employee)
                .Include(a => a.AssetState)
                .AsQueryable();
            depQuery = _context.AssetDepMDs.AsQueryable().Where(a => a.AccSystemId == accSystemId && a.AccMonthId == accMonth.Id);

            invAssetQuery = _context.InventoryAssets
                .Include(r => r.RoomInitial)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                    //     .Include(r => r.RoomInitial)
                    //.ThenInclude(l => l.Location)
                    //   .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterInitial)
                    //.ThenInclude(a => a.AdmCenter)
                .Include(e => e.EmployeeInitial)
                .Include(s => s.StateInitial)
                .Include(r => r.RoomFinal)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                    //     .Include(r => r.RoomFinal)
                    //.ThenInclude(l => l.Location)
                    //   .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterFinal)
                    //.ThenInclude(r => r.AdmCenter)
                 .Include(e => e.EmployeeFinal)
                  .Include(s => s.StateFinal)

                .Where(i => i.InventoryId == inventoryId).AsQueryable();


            query = assetQuery.Select(asset => new Model.AssetInventoryDetail { Asset = asset });


            query = query
                .Join(invAssetQuery, q => q.Asset.Id, inv => inv.AssetId, (q, inv) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = inv });



            query = query
                .Join(depQuery, q => q.Asset.Id, dep => dep.AssetId, (q, dep) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = q.Inventory, Dep = dep });


            if (costCenterId.HasValue && (costCenterId > 0))
            {
                query = query.Where(i => ((i.Inventory.CostCenterIdInitial == costCenterId)
                    || (i.Inventory.CostCenterIdFinal == costCenterId)));
            }
            else
            {
                if (admCenterId.HasValue && (admCenterId > 0))
                {
                    query = query.Where(i => i.Inventory.CostCenterInitial.AdmCenterId == admCenterId
                        || ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)));
                }
            }

            if (locationId.HasValue && (locationId > 0))
            {
                query = query.Where(i => i.Inventory.RoomInitial.LocationId == locationId
                    || ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomFinal.LocationId == locationId)));
            }
            else
            {
                if (regionId.HasValue && (regionId > 0))
                {
                    query = query.Where(i => i.Inventory.RoomInitial.Location.RegionId == regionId
                        || ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomFinal.Location.RegionId == regionId)));
                }
            }

            if (custody.HasValue)
            {
                query = query.Where(i => i.Asset.Custody == custody.GetValueOrDefault());
            }

            if (reportType != null)
            {
                switch (reportType.ToUpper())
                {
                    case "TRANSFER_SAME_LOCATION":
                        query = query
                            .Where(i => ((i.Inventory.RoomIdFinal != null)
                                && ((i.Inventory.RoomIdInitial != i.Inventory.RoomIdFinal) || (i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))
                                && (i.Inventory.RoomInitial.LocationId == i.Inventory.RoomFinal.LocationId)));
                        strReportType = "in aceeasi localitate";
                        break;
                    case "TRANSFER_DIFF_LOCATION":
                        query = query
                            .Where(i => ((i.Inventory.RoomIdFinal != null)
                                && ((i.Inventory.RoomIdInitial != i.Inventory.RoomIdFinal) || (i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))
                                && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)));
                        strReportType = "intre adrese din aceeasi localitate";
                        break;
                    case "TRANSFER_SAME_REGION":
                        query = query
                            .Where(i => ((i.Inventory.RoomIdFinal != null)
                                && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)
                                && ((i.Inventory.RoomIdInitial != i.Inventory.RoomIdFinal) || (i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))
                                && (i.Inventory.RoomInitial.Location.RegionId == i.Inventory.RoomFinal.Location.RegionId)));
                        strReportType = "in judet";
                        break;
                    case "TRANSFER_DIFF_REGION":
                        query = query
                            .Where(i => ((i.Inventory.RoomIdFinal != null)
                                && ((i.Inventory.RoomIdInitial != i.Inventory.RoomIdFinal) || (i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))
                                && (i.Inventory.RoomInitial.Location.RegionId != i.Inventory.RoomFinal.Location.RegionId)));
                        strReportType = "intre judete";
                        break;
                    case "TRANSFER_SAME_ADMCENTER":
                        //invAssetQuery = invAssetQuery
                        //    .Where(i => ((i.CostCenterIdFinal != null)
                        //        //&& (i.CostCenterIdInitial != i.CostCenterIdFinal)
                        //        && (i.RoomInitial.LocationId != i.RoomFinal.LocationId)
                        //        && (i.CostCenterInitial.AdmCenterId == i.CostCenterFinal.AdmCenterId)));
                        query = query
                          .Where(i => ((i.Inventory.RoomIdFinal != null)
                              //&& ((i.RoomIdInitial != i.RoomIdFinal))
                              && ((i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId))
                              && (i.Inventory.RoomInitial.Location.RegionId == i.Inventory.RoomFinal.Location.RegionId)));
                        strReportType = "in judete";
                        break;
                    case "TRANSFER_DIFF_ADMCENTER":
                        query = query
                            .Where(i => ((i.Inventory.RoomIdFinal != null)
                                //&& (i.CostCenterIdInitial != i.CostCenterIdFinal)
                                && ((i.Inventory.RoomIdInitial != i.Inventory.RoomIdFinal) || (i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))
                              && (i.Inventory.RoomInitial.Location.RegionId != i.Inventory.RoomFinal.Location.RegionId)));
                        strReportType = "intre judete";
                        break;
                    case "TRANSFER_SAME_LOCATION_DIFF_COSTCENTER":
                        query = query
                            .Where(i => ((i.Inventory.RoomIdFinal != null) && (i.Inventory.CostCenterIdInitial != i.Inventory.CostCenterIdFinal)
                            && (i.Inventory.RoomIdInitial == i.Inventory.RoomIdFinal) && (i.Inventory.EmployeeIdInitial == i.Inventory.EmployeeIdFinal)
                            && (i.Inventory.RoomInitial.LocationId == i.Inventory.RoomFinal.LocationId)));
                        strReportType = "in aceeasi localitate centre de cost diferite";
                        break;
                    default:
                        break;
                }
            }

            listAssetInventoryDetails = query.ToList();

            transferInV1Result = new TransferInV1Result();

            transferInV1Result.Details = new List<TransferInV1Detail>();

            foreach (var inventoryAsset in listAssetInventoryDetails)
            {
                transferInV1Result.Details.Add(new TransferInV1Detail()
                {
                    InvNo = inventoryAsset.Asset.InvNo,
                    Description = inventoryAsset.Asset.Name,
                    SerialNumber = inventoryAsset.Asset.SerialNumber,
                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                    //RegionInitial = inventoryAsset.RoomInitial.Location.Region != null ? inventoryAsset.RoomInitial.Location.Region.Code : string.Empty,
                    //RegionFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Region != null ? inventoryAsset.RoomFinal.Location.Region.Code : string.Empty : string.Empty,
                    RegionInitial = ((inventoryAsset.Inventory.RoomIdInitial != null) && (inventoryAsset.Inventory.RoomInitial.LocationId > 0)) ? inventoryAsset.Inventory.RoomInitial.Location.Region.Code : string.Empty,
                    RegionFinal = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId > 0)) ? inventoryAsset.Inventory.RoomFinal.Location.Region.Code : string.Empty,
                    LocationHeader = inventoryAsset.Inventory.RoomInitial.Location.Name,
                    LocationNameInitial = inventoryAsset.Inventory.RoomInitial.Location.Name,
                    LocationNameFinal = inventoryAsset.Inventory.RoomFinal != null ? inventoryAsset.Inventory.RoomFinal.Location.Name : string.Empty,
                    CostCenterInitial = inventoryAsset.Inventory.CostCenterInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty,
                    CostCenterFinal = inventoryAsset.Inventory.CostCenterFinal != null ? inventoryAsset.Inventory.CostCenterFinal.Code : string.Empty,
                    RoomInitial = inventoryAsset.Inventory.RoomInitial.Name,
                    RoomFinal = inventoryAsset.Inventory.RoomFinal != null ? inventoryAsset.Inventory.RoomFinal.Name : string.Empty,

                    InternalCodeInitial = inventoryAsset.Inventory.EmployeeInitial != null ? inventoryAsset.Inventory.EmployeeInitial.InternalCode : string.Empty,
                    InternalCodeFinal = inventoryAsset.Inventory.EmployeeFinal != null ? inventoryAsset.Inventory.EmployeeFinal.InternalCode : string.Empty,

                    FullNameInitial = inventoryAsset.Inventory.EmployeeInitial != null ? inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName : string.Empty,
                    FullNameFinal = inventoryAsset.Inventory.EmployeeFinal != null ? inventoryAsset.Inventory.EmployeeFinal.FirstName + " " + inventoryAsset.Inventory.EmployeeFinal.LastName : string.Empty,

                    Initial = inventoryAsset.Inventory.QInitial,
                    Actual = inventoryAsset.Inventory.QFinal,
                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                    Value = inventoryAsset.Dep.CurrentAPC,
                    ValueDepTotal = inventoryAsset.Dep.CurrentAPC - inventoryAsset.Dep.PosCap,
                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                    ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : DateTime.MinValue
                    

                });
            }

            inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);

            var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            transferInV1Result.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            transferInV1Result.ReportType = strReportType;
            transferInV1Result.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;
            transferInV1Result.RegionName = (region != null) ? region.Name : string.Empty;

            transferInV1Result.InventoryName = inventory.Description.Trim();
            var maxDate = transferInV1Result.Details.Count > 0 ? transferInV1Result.Details.Where(s => s.ModifiedAt == transferInV1Result.Details.Max(x => x.ModifiedAt)).FirstOrDefault() : null;
            transferInV1Result.InventoryEndDate = maxDate != null ? maxDate.ModifiedAt.Value : DateTime.Now;
            transferInV1Result.LocationName = (location != null) ? location.Name : string.Empty;
           

            //if (admCenter != null)
            //{
            //    if (admCenter.Committee != null)
            //    {
            //        if (admCenter.Committee.Length > 0)
            //        {
            //            string[] committees = admCenter.Committee.Split(';');

            //            transferInV1Result.Committee1 = committees[0];
            //            transferInV1Result.Committee2 = committees[1];
            //            transferInV1Result.Committee3 = committees[2];
            //        }
            //    }


            //}

            // NEW  //

            //if (reportType != null)
            //{
            //    switch (reportType.ToUpper())
            //    {
            //        case "TRANSFER_SAME_LOCATION":
            //            invAssetQuery = invAssetQuery
            //                .Where(i => ((i.RoomIdFinal != null)
            //                    && ((i.RoomIdInitial != i.RoomIdFinal) || (i.EmployeeIdInitial != i.EmployeeIdFinal))
            //                    && (i.RoomInitial.LocationId == i.RoomFinal.LocationId)));
            //            strReportType = "in aceeasi cladire";
            //            break;
            //        case "TRANSFER_DIFF_LOCATION":
            //            invAssetQuery = invAssetQuery
            //                .Where(i => ((i.RoomIdFinal != null)
            //                    && ((i.RoomIdInitial != i.RoomIdFinal) || (i.EmployeeIdInitial != i.EmployeeIdFinal))
            //                    && (i.RoomInitial.LocationId != i.RoomFinal.LocationId)));
            //            strReportType = "intre camere din aceeasi cladire";
            //            break;
            //        case "TRANSFER_SAME_REGION":
            //            invAssetQuery = invAssetQuery
            //                .Where(i => ((i.RoomIdFinal != null)
            //                    && (i.RoomInitial.LocationId != i.RoomFinal.LocationId)
            //                    && ((i.RoomIdInitial != i.RoomIdFinal) || (i.EmployeeIdInitial != i.EmployeeIdFinal))
            //                    && (i.RoomInitial.Location.RegionId == i.RoomFinal.Location.RegionId)));
            //            strReportType = "in regiune";
            //            break;
            //        case "TRANSFER_DIFF_REGION":
            //            invAssetQuery = invAssetQuery
            //                .Where(i => ((i.RoomIdFinal != null)
            //                    && ((i.RoomIdInitial != i.RoomIdFinal) || (i.EmployeeIdInitial != i.EmployeeIdFinal))
            //                    && (i.RoomInitial.Location.RegionId != i.RoomFinal.Location.RegionId)));
            //            strReportType = "intre regiuni";
            //            break;
            //        case "TRANSFER_SAME_ADMCENTER":
            //            //invAssetQuery = invAssetQuery
            //            //    .Where(i => ((i.CostCenterIdFinal != null)
            //            //        //&& (i.CostCenterIdInitial != i.CostCenterIdFinal)
            //            //        && (i.RoomInitial.LocationId != i.RoomFinal.LocationId)
            //            //        && (i.CostCenterInitial.AdmCenterId == i.CostCenterFinal.AdmCenterId)));
            //            invAssetQuery = invAssetQuery
            //              .Where(i => ((i.CostCenterIdFinal != null)
            //                  //&& ((i.RoomIdInitial != i.RoomIdFinal))
            //                  && ((i.RoomInitial.LocationId != i.RoomFinal.LocationId))
            //                  && (i.CostCenterInitial.AdmCenterId == i.CostCenterFinal.AdmCenterId)));
            //            strReportType = "in unitatea logistica";
            //            break;
            //        case "TRANSFER_DIFF_ADMCENTER":
            //            invAssetQuery = invAssetQuery
            //                .Where(i => ((i.CostCenterIdFinal != null)
            //                    //&& (i.CostCenterIdInitial != i.CostCenterIdFinal)
            //                    && ((i.RoomIdInitial != i.RoomIdFinal) || (i.EmployeeIdInitial != i.EmployeeIdFinal))
            //                  && (i.CostCenterInitial.AdmCenterId != i.CostCenterFinal.AdmCenterId)));
            //            strReportType = "intre unitati logistice";
            //            break;
            //        case "TRANSFER_SAME_LOCATION_DIFF_COSTCENTER":
            //            invAssetQuery = invAssetQuery
            //                .Where(i => ((i.RoomIdFinal != null) && (i.CostCenterIdInitial != i.CostCenterIdFinal) && (i.RoomIdInitial == i.RoomIdFinal) && (i.EmployeeIdInitial == i.EmployeeIdFinal) && (i.RoomInitial.LocationId == i.RoomFinal.LocationId)));
            //            strReportType = "in aceeasi cladire centre de cost diferite";
            //            break;
            //        default:
            //            break;
            //    }
            //}

            //listInventoryAssets = invAssetQuery.ToList();

            //transferInV1Result = new TransferInV1Result();

            //transferInV1Result.Details = new List<TransferInV1Detail>();

            //foreach (var inventoryAsset in listInventoryAssets)
            //{
            //    transferInV1Result.Details.Add(new TransferInV1Detail()
            //    {
            //        InvNo = inventoryAsset.Asset.InvNo,
            //        Description = inventoryAsset.Asset.Name,
            //        SerialNumber = inventoryAsset.Asset.SerialNumber,
            //        PurchaseDate = inventoryAsset.Asset.PurchaseDate,

            //        //RegionInitial = inventoryAsset.RoomInitial.Location.Region != null ? inventoryAsset.RoomInitial.Location.Region.Code : string.Empty,
            //        //RegionFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Region != null ? inventoryAsset.RoomFinal.Location.Region.Code : string.Empty : string.Empty,
            //        RegionInitial = ((inventoryAsset.CostCenterIdInitial != null) && (inventoryAsset.CostCenterInitial.AdmCenterId != null)) ? inventoryAsset.CostCenterInitial.AdmCenter.Code : string.Empty,
            //        RegionFinal = ((inventoryAsset.CostCenterIdFinal != null) && (inventoryAsset.CostCenterFinal.AdmCenterId != null)) ? inventoryAsset.CostCenterFinal.AdmCenter.Code : string.Empty,

            //        LocationNameInitial = inventoryAsset.RoomInitial.Location.Name,
            //        LocationNameFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Name : string.Empty,
            //        CostCenterInitial = inventoryAsset.CostCenterInitial != null ? inventoryAsset.CostCenterInitial.Code : string.Empty,
            //        CostCenterFinal = inventoryAsset.CostCenterFinal != null ? inventoryAsset.CostCenterFinal.Code : string.Empty,
            //        RoomInitial = inventoryAsset.RoomInitial.Name,
            //        RoomFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Name : string.Empty,

            //        InternalCodeInitial = inventoryAsset.EmployeeInitial != null ? inventoryAsset.EmployeeInitial.InternalCode : string.Empty,
            //        InternalCodeFinal = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.InternalCode : string.Empty,

            //        FullNameInitial = inventoryAsset.EmployeeInitial != null ? inventoryAsset.EmployeeInitial.FirstName + " " + inventoryAsset.EmployeeInitial.LastName : string.Empty,
            //        FullNameFinal = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.FirstName + " " + inventoryAsset.EmployeeFinal.LastName : string.Empty,

            //        Initial = inventoryAsset.QInitial,
            //        Actual = inventoryAsset.QFinal,
            //        Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
            //        Value = inventoryAsset.Asset.ValueInv,
            //        ValueDepTotal = inventoryAsset.Asset.ValueRem,
            //        Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
            //        ModifiedAt = inventoryAsset.ModifiedAt != null ? inventoryAsset.ModifiedAt.Value : DateTime.MinValue
            //    });
            //}

            //inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);

            //var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            //transferInV1Result.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            //transferInV1Result.ReportType = strReportType;
            //transferInV1Result.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;

            //transferInV1Result.InventoryName = inventory.Description.Trim();
            //var maxDate = transferInV1Result.Details.Count > 0 ? transferInV1Result.Details.Where(s => s.ModifiedAt == transferInV1Result.Details.Max(x => x.ModifiedAt)).FirstOrDefault() : null;
            //transferInV1Result.InventoryEndDate = maxDate != null ? maxDate.ModifiedAt.Value : DateTime.Now;
            //transferInV1Result.LocationName = (location != null) ? location.Name : string.Empty;

            //if (admCenter != null)
            //{
            //    if (admCenter.Committee != null)
            //    {
            //        if (admCenter.Committee.Length > 0)
            //        {
            //            string[] committees = admCenter.Committee.Split(';');

            //            transferInV1Result.Committee1 = committees[0];
            //            transferInV1Result.Committee2 = committees[1];
            //            transferInV1Result.Committee3 = committees[2];
            //        }
            //    }


            //}

            return transferInV1Result;
        }

        //public InventoryResult GetInventoryListV3ByFilters(int inventoryId, int regionId, int? locationId, int employeeId, string reportType, bool? custody)
        //{
        //    InventoryResult invResult = new InventoryResult();
        //    Model.Employee employee = null;
        //    Model.Location location = null;
        //    Model.Region region = null;

        //    IQueryable<Dto.InventoryAsset> invAssetQuery = null;
        //    IQueryable<Dto.AssetNiInvDet> assetNiQuery = null;

        //    List<Dto.InventoryAsset> listInventoryAssets = null;
        //    List<Dto.AssetNiInvDet> listAssetNi = null;

        //    employee = _context.Set<Model.Employee>().Single(e => e.Id == employeeId);
        //    location = (locationId.HasValue) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
        //    region = region = _context.Set<Model.Region>().Single(r => r.Id == regionId);

        //    invAssetQuery = _assetsRepository.GetInventoryAssetsQuery(inventoryId);
        //    assetNiQuery = _assetNiRepository.GetAssetNiInvDetQuery(inventoryId);

        //    invAssetQuery = invAssetQuery.Where(i => i.RegionIdIni == regionId || i.RegionIdFin == regionId);
        //    assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);

        //    if (locationId.HasValue)
        //    {
        //        invAssetQuery = invAssetQuery.Where(i => i.LocationIdIni == locationId || i.LocationIdFin == locationId);
        //        assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
        //    }

        //    if (custody.HasValue)
        //    {
        //        invAssetQuery = invAssetQuery.Where(i => i.Custody == custody.GetValueOrDefault());
        //        assetNiQuery = assetNiQuery.Where(a => a.Custody == custody.GetValueOrDefault());
        //    }

        //    invAssetQuery = invAssetQuery.Where(e => e.EmployeeIdIni == employeeId || e.EmployeeIdFin == employeeId);
        //    assetNiQuery = assetNiQuery.Where(a => a.EmployeeId == employeeId);

        //    if (reportType != null)
        //    {
        //        switch (reportType.ToUpper())
        //        {
        //            case "MINUS":

        //                if (locationId != null)
        //                {
        //                    invAssetQuery = invAssetQuery.Where(i => (i.LocationIdFin == null) || (i.LocationIdFin != locationId));
        //                }
        //                else
        //                {
        //                    invAssetQuery = invAssetQuery.Where(i => (i.RegionIdFin == null) || (i.RegionIdFin != regionId));
        //                }

        //                assetNiQuery = null;

        //                invResult.InventoryListType = "Minusuri";

        //                break;

        //            case "PLUS":

        //                if ((locationId != null))
        //                {
        //                    invAssetQuery = invAssetQuery.Where(i => (i.LocationIdFin != null) && (i.LocationIdIni != i.LocationIdFin) && (i.LocationIdFin == locationId));
        //                }
        //                else
        //                {
        //                    invAssetQuery = invAssetQuery.Where(i => (i.RegionIdFin != null) && (i.RegionIdIni != i.RegionIdFin) && (i.RegionIdFin == regionId));
        //                }

        //                invResult.InventoryListType = "Plusuri";

        //                break;

        //            default:

        //                invResult.InventoryListType = string.Empty;

        //                break;
        //        }
        //    }

        //    listInventoryAssets = invAssetQuery.ToList();
        //    listAssetNi = (assetNiQuery != null) ? assetNiQuery.ToList() : new List<Dto.AssetNiInvDet>();

        //    invResult.Details = new List<InventoryResultDetail>();

        //    foreach (var inventoryAsset in listInventoryAssets)
        //    {
        //        invResult.Details.Add(new InventoryResultDetail()
        //        {
        //            InvNo = inventoryAsset.InvNo,
        //            Description = inventoryAsset.Name,
        //            SerialNumber = inventoryAsset.SerialNumber,
        //            PurchaseDate = inventoryAsset.PurchaseDate,

        //            //CostCenter = inventoryAsset.LocationIdIni == locationId
        //            //    ? inventoryAsset.CostCenterCodeIni
        //            //    : inventoryAsset.CostCenterCodeFin,
        //            //Room = inventoryAsset.EmployeeIdIni == employeeId
        //            //    ? (inventoryAsset.LocationIdIni != inventoryAsset.LocationIdFin ? inventoryAsset.RoomNameIni : inventoryAsset.RoomNameFin)
        //            //    : inventoryAsset.RoomNameFin,

        //            CostCenter = (locationId != null)
        //            ? (inventoryAsset.LocationIdIni == locationId ? inventoryAsset.CostCenterCodeIni : inventoryAsset.CostCenterCodeFin)
        //            : (inventoryAsset.RegionIdIni == regionId ? inventoryAsset.CostCenterCodeIni : inventoryAsset.CostCenterCodeFin),

        //            Building = (locationId != null)
        //            ? (inventoryAsset.LocationIdIni == locationId ? inventoryAsset.LocationNameIni : inventoryAsset.LocationNameFin)
        //            : (inventoryAsset.RegionIdIni == regionId ? inventoryAsset.LocationNameIni : inventoryAsset.LocationNameFin),

        //            Room = (locationId != null)
        //            ? (inventoryAsset.LocationIdIni == locationId ? ((inventoryAsset.LocationIdIni != inventoryAsset.LocationIdFin) ? inventoryAsset.RoomNameIni : inventoryAsset.RoomNameFin) : inventoryAsset.RoomNameFin)
        //            : (inventoryAsset.RegionIdIni == regionId ? ((inventoryAsset.RegionIdIni != inventoryAsset.RegionIdFin) ? inventoryAsset.RoomNameIni : inventoryAsset.RoomNameFin) : inventoryAsset.RoomNameFin),

        //            //Initial = inventoryAsset.EmployeeIdIni == employeeId ? inventoryAsset.QIntial : 0,
        //            //Actual = inventoryAsset.EmployeeIdFin == employeeId ? inventoryAsset.QFinal : 0,
        //            Initial = (locationId != null) ? ((inventoryAsset.LocationIdIni == inventoryAsset.LocationIdFin && inventoryAsset.EmployeeIdIni == employeeId) ? inventoryAsset.QIntial : (inventoryAsset.LocationIdIni == locationId ? inventoryAsset.QIntial : 0)) : (inventoryAsset.LocationIdIni == inventoryAsset.LocationIdFin && inventoryAsset.EmployeeIdIni == employeeId) ? inventoryAsset.QIntial : (inventoryAsset.RegionIdIni == regionId) ? inventoryAsset.QIntial : 0,
        //            Actual = (locationId != null) ? ((inventoryAsset.LocationIdIni == inventoryAsset.LocationIdFin && inventoryAsset.EmployeeIdIni == employeeId) ? inventoryAsset.QFinal : inventoryAsset.LocationIdFin == locationId ? inventoryAsset.QFinal : 0) : (inventoryAsset.LocationIdIni == inventoryAsset.LocationIdFin && inventoryAsset.EmployeeIdIni == employeeId) ? inventoryAsset.QFinal : (inventoryAsset.RegionIdFin == regionId) ? inventoryAsset.QFinal : 0,
        //            Uom = inventoryAsset.Uom,
        //            Value = inventoryAsset.ValueInv,
        //            ValueInv = inventoryAsset.ValueInv,
        //            ValueDep = inventoryAsset.ValueDep,
        //            ValueDepTotal = inventoryAsset.ValueDep,
        //            Info = string.Empty,
        //            Custody = inventoryAsset.Custody.HasValue ? (inventoryAsset.Custody.Value ? "DA" : "NU") : string.Empty,
        //            InternalCode = (inventoryAsset.InternalCodeFin != null && inventoryAsset.InternalCodeFin != string.Empty) ? inventoryAsset.InternalCodeFin : inventoryAsset.InternalCodeIni,
        //            FullName = (inventoryAsset.LastNameFin != null && inventoryAsset.LastNameFin != string.Empty) ? inventoryAsset.FirstNameFin + " " + inventoryAsset.LastNameFin : inventoryAsset.FirstNameIni + " " + inventoryAsset.LastNameIni
        //        });
        //    }

        //    foreach (var assetNi in listAssetNi)
        //    {
        //        invResult.Details.Add(new InventoryResultDetail()
        //        {
        //            InvNo = assetNi.Code1,
        //            Description = assetNi.Name1,
        //            SerialNumber = assetNi.SerialNumber,
        //            PurchaseDate = null,
        //            CostCenter = assetNi.CostCenterCode,
        //            Room = assetNi.RoomName,
        //            Initial = 0,
        //            Actual = assetNi.Quantity,
        //            Uom = "BUC",
        //            Value = 0,
        //            ValueInv = 0,
        //            ValueDep = 0,
        //            ValueDepTotal = 0,
        //            Info = assetNi.Info,
        //            Custody = assetNi.Custody.HasValue ? (assetNi.Custody.Value ? "DA" : "NU") : string.Empty,
        //            InternalCode = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.InternalCode,
        //            FullName = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.FirstName + " " + assetNi.LastName
        //        });
        //    }

        //    invResult.AdmCenterName = region.Name;
        //    invResult.CompanyName = "";
        //    //invResult.CompanyName = "";
        //    invResult.EndDate = DateTime.Now;
        //    invResult.LocationName = (location != null) ? location.Name : string.Empty;

        //    invResult.EmployeeInternalCode = (employee != null) ? employee.InternalCode : string.Empty;
        //    invResult.EmployeeFirstName = (employee != null) ? employee.FirstName : string.Empty;
        //    invResult.EmployeeLastName = (employee != null) ? employee.LastName : string.Empty;

        //    return invResult;
        //}

        //public InventoryResult GetInventoryListV2ByFilters(int? inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? employeeId, string reportType, bool? custody, string internalCode)
        //{
        //    InventoryResult invResult = new InventoryResult();
        //    Model.Inventory inventory = null;
        //    Model.AdmCenter admCenter = null;
        //    Model.Location location = null;
        //    Model.Region region = null;
        //    Model.Employee employee = null;

        //    IQueryable<Model.InventoryAsset> invAssetQuery = null;
        //    IQueryable<Dto.AssetNiInvDet> assetNiQuery = null;

        //    List<Model.InventoryAsset> listInventoryAssets = null;
        //    List<Dto.AssetNiInvDet> listAssetNi = null;

        //    if ((inventoryId.HasValue) && (inventoryId > 0))
        //        inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);
        //    else
        //        inventory = _context.Set<Model.Inventory>().OrderByDescending(i => i.Id).FirstOrDefault(i => i.Active == true);

        //    if (inventory == null) return null;

        //    admCenter = (admCenterId.HasValue && admCenterId > 0) ? _context.Set<Model.AdmCenter>().Single(a => a.Id == admCenterId) : null;
        //    location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
        //    region = (regionId.HasValue && regionId > 0) ? _context.Set<Model.Region>().Single(r => r.Id == regionId) : null;

        //    if ((employeeId.HasValue) && (employeeId > 0))
        //        employee = _context.Set<Model.Employee>().Single(e => e.Id == employeeId);
        //    else if ((internalCode != null) && (internalCode.Length > 0))
        //    {
        //        employee = _context.Set<Model.Employee>().FirstOrDefault(e => ((e.InternalCode == internalCode) && (e.ERPCode.ToUpper() == "PUBLIC")));

        //        if (employee == null) return null;
        //    }

        //    //if ((employee == null) && (admCenter == null) && (region == null) && (location == null)) return null;

        //    invAssetQuery = _assetsRepository.GetInventoryAssetsQuery2(inventory.Id, null);
        //    assetNiQuery = _assetNiRepository.GetAssetNiInvDetQuery(inventory.Id);

        //    //invAssetQuery = invAssetQuery.Where(i => i.RegionIdIni == regionId || i.RegionIdFin == regionId);
        //    //assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);

        //    if (costCenterId.HasValue && (costCenterId > 0))
        //    {
        //        invAssetQuery = invAssetQuery.Where(i => ((i.CostCenterIdInitial == costCenterId)
        //            || (i.CostCenterIdFinal == costCenterId)));
        //        assetNiQuery = assetNiQuery.Where(a => a.CostCenterId == costCenterId);
        //    }
        //    else
        //    {
        //        if (admCenterId.HasValue && (admCenterId > 0))
        //        {
        //            invAssetQuery = invAssetQuery.Where(i => i.CostCenterInitial.AdmCenterId == admCenterId
        //                || ((i.CostCenterIdFinal != null) && (i.CostCenterFinal.AdmCenterId == admCenterId)));
        //            assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
        //        }
        //    }

        //    if (employee != null)
        //    {
        //        employeeId = employee.Id;
        //        if (locationId.HasValue || regionId.HasValue)
        //        {
        //            if (locationId.HasValue)
        //            {
        //                invAssetQuery = invAssetQuery.Where(i => (i.EmployeeIdInitial == employeeId && i.RoomInitial.LocationId == locationId)
        //                || ((i.EmployeeIdFinal == employeeId) && (i.RoomIdFinal != null) && (i.RoomFinal.LocationId == locationId)));
        //                assetNiQuery = assetNiQuery.Where(a => (a.EmployeeId == employeeId && a.LocationId == locationId));
        //            }
        //        }
        //        else
        //        {
        //            invAssetQuery = invAssetQuery.Where(e => e.EmployeeIdInitial == employeeId || e.EmployeeIdFinal == employeeId);
        //            assetNiQuery = assetNiQuery.Where(a => a.EmployeeId == employeeId);
        //        }

        //        invResult.EmployeeInternalCode = employee.InternalCode;
        //        invResult.EmployeeFirstName = employee.FirstName;
        //        invResult.EmployeeLastName = employee.LastName;
        //    }
        //    else
        //    {
        //        if (locationId.HasValue && (locationId > 0))
        //        {
        //            invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.LocationId == locationId
        //                || ((i.RoomIdFinal != null) && (i.RoomFinal.LocationId == locationId)));
        //            assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
        //        }
        //        else
        //        {
        //            if (regionId.HasValue && (regionId > 0))
        //            {
        //                invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.Location.RegionId == regionId
        //                    || ((i.RoomIdFinal != null) && (i.RoomFinal.Location.RegionId == regionId)));
        //                assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
        //            }
        //        }
        //    }

        //    //if (locationId.HasValue && (locationId > 0))
        //    //{
        //    //    invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.LocationId == locationId
        //    //        || ((i.RoomIdFinal != null) && (i.RoomFinal.LocationId == locationId)));
        //    //    assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
        //    //}
        //    //else
        //    //{
        //    //    if (regionId.HasValue && (regionId > 0))
        //    //    {
        //    //        invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.Location.RegionId == regionId
        //    //            || ((i.RoomIdFinal != null) && (i.RoomFinal.Location.RegionId == regionId)));
        //    //        assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
        //    //    }
        //    //}

        //    if (custody.HasValue)
        //    {
        //        invAssetQuery = invAssetQuery.Where(i => i.Asset.Custody == custody.GetValueOrDefault());
        //        assetNiQuery = assetNiQuery.Where(a => a.Custody == custody.GetValueOrDefault());
        //    }

        //    if (reportType != null)
        //    {
        //        switch (reportType.ToUpper())
        //        {
        //            case "MINUS":

        //                bool filters = false;
        //                //if ((locationId.HasValue) || (regionId.HasValue))
        //                //{
        //                //    invAssetQuery = invAssetQuery.Where(i => (((i.RoomIdFinal != null) && (i.RoomInitial.LocationId != i.RoomFinal.LocationId)) || (i.RoomIdFinal == null)));
        //                //}
        //                //else
        //                //{
        //                //    invAssetQuery = invAssetQuery.Where(i => (((i.CostCenterIdFinal != null) && (i.CostCenterInitial.AdmCenterId != i.CostCenterFinal.AdmCenterId)) || (i.CostCenterIdFinal == null)));
        //                //}

        //                if (locationId.HasValue)
        //                {
        //                    //invAssetQuery = invAssetQuery.Where(i => (i.RoomInitial.LocationId == locationId));
        //                    invAssetQuery = invAssetQuery.Where(i => (i.RoomInitial.LocationId == locationId) && (((i.RoomIdFinal != null) && (i.RoomFinal.LocationId != locationId)) || (i.RoomIdFinal == null)));
        //                    filters = true;
        //                }
        //                //else
        //                //{
        //                //    if (regionId.HasValue)
        //                //    {
        //                //        invAssetQuery = invAssetQuery.Where(i => (i.RoomInitial.Location.RegionId == regionId));
        //                //    }
        //                //}

        //                if (admCenterId.HasValue)
        //                {
        //                    //invAssetQuery = invAssetQuery.Where(i => ((i.CostCenterIdInitial != null) && (i.CostCenterInitial.AdmCenterId == admCenterId)));
        //                    invAssetQuery = invAssetQuery.Where(i => (i.CostCenterIdInitial != null) && (i.CostCenterInitial.AdmCenterId == admCenterId)
        //                        && (((i.CostCenterIdFinal != null) && (i.CostCenterFinal.AdmCenterId != admCenterId)) || (i.CostCenterIdFinal == null)));
        //                    filters = true;
        //                }

        //                if (!filters)
        //                {
        //                    invAssetQuery = invAssetQuery.Where(i => i.RoomIdFinal == null);
        //                }

        //                assetNiQuery = null;
        //                invResult.InventoryListType = "Minusuri";
        //                break;

        //            case "PLUS":
        //                if (locationId.HasValue || regionId.HasValue || admCenterId.HasValue)
        //                {

        //                    if ((locationId.HasValue) || (regionId.HasValue))
        //                    {
        //                        invAssetQuery = invAssetQuery.Where(i => ((i.RoomIdFinal != null) && (i.RoomInitial.LocationId != i.RoomFinal.LocationId)));
        //                    }
        //                    else
        //                    {
        //                        if (admCenterId.HasValue)
        //                        {
        //                            invAssetQuery = invAssetQuery.Where(i => ((i.CostCenterIdFinal != null) && (i.CostCenterInitial.AdmCenterId != i.CostCenterFinal.AdmCenterId)));
        //                        }
        //                    }


        //                    if (locationId.HasValue)
        //                    {
        //                        invAssetQuery = invAssetQuery.Where(i => ((i.RoomIdFinal != null) && (i.RoomFinal.LocationId == locationId)));
        //                    }
        //                    else
        //                    {
        //                        if (regionId.HasValue)
        //                        {
        //                            invAssetQuery = invAssetQuery.Where(i => ((i.RoomIdFinal != null) && (i.RoomFinal.Location.RegionId == regionId)));
        //                        }
        //                    }

        //                    if (admCenter != null)
        //                    {
        //                        invAssetQuery = invAssetQuery.Where(i => ((i.CostCenterIdFinal != null) && (i.CostCenterFinal.AdmCenterId == admCenterId)));
        //                    }
        //                }
        //                else
        //                {
        //                    invAssetQuery = null;
        //                }

        //                invResult.InventoryListType = "Plusuri";
        //                break;

        //            case "NOT_IDENTIFIED":
        //                invAssetQuery = null;
        //                invResult.InventoryListType = "Etichete temporare";
        //                break;

        //            default:
        //                invResult.InventoryListType = string.Empty;
        //                break;
        //        }
        //    }

        //    if (invAssetQuery != null)
        //    {
        //        invAssetQuery = invAssetQuery.OrderBy(i => i.Asset.InvNo);
        //        listInventoryAssets = invAssetQuery.ToList();
        //    }
        //    else
        //    {
        //        listInventoryAssets = new List<InventoryAsset>();
        //    }

        //    if (assetNiQuery != null)
        //    {
        //        assetNiQuery = assetNiQuery.OrderBy(a => a.Code1);
        //        listAssetNi = assetNiQuery.ToList();
        //    }
        //    else
        //    {
        //        listAssetNi = new List<Dto.AssetNiInvDet>();
        //    }

        //    //listInventoryAssets = (invAssetQuery != null) ? invAssetQuery.ToList() : new List<InventoryAsset>();
        //    //listAssetNi = (assetNiQuery != null) ? assetNiQuery.ToList() : new List<Dto.AssetNiInvDet>();

        //    invResult.Details = new List<InventoryResultDetail>();

        //    foreach (var inventoryAsset in listInventoryAssets)
        //    {
        //        InventoryResultDetail inventoryResultDetail = null;
        //        inventoryResultDetail = new InventoryResultDetail()
        //        {
        //            InvNo = inventoryAsset.Asset.InvNo,
        //            Description = inventoryAsset.Asset.Name,
        //            SerialNumber = inventoryAsset.SerialNumber,
        //            PurchaseDate = inventoryAsset.Asset.PurchaseDate,

        //            Initial = 0,
        //            Actual = 0,

        //            Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
        //            Value = inventoryAsset.Asset.ValueInv,
        //            ValueInv = inventoryAsset.Asset.ValueInv,
        //            ValueDep = inventoryAsset.Asset.ValueInv - inventoryAsset.Asset.ValueRem,
        //            ValueDepTotal = inventoryAsset.Asset.ValueInv - inventoryAsset.Asset.ValueRem,
        //            Info = string.Empty,
        //            Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty
        //        };

        //        switch (reportType.ToUpper())
        //        {
        //            case "MINUS":
        //                inventoryResultDetail.Initial = inventoryAsset.QInitial;
        //                inventoryResultDetail.Actual = 0;
        //                break;
        //            case "PLUS":
        //                inventoryResultDetail.Initial = 0;
        //                inventoryResultDetail.Actual = inventoryAsset.QFinal;
        //                break;
        //            default:
        //                if (locationId.HasValue)
        //                {
        //                    inventoryResultDetail.Initial = inventoryAsset.RoomInitial.LocationId == locationId ? inventoryAsset.QInitial : 0;
        //                    inventoryResultDetail.Actual = ((inventoryAsset.RoomIdFinal != null) && (inventoryAsset.RoomFinal.LocationId == locationId)) ? inventoryAsset.QFinal : 0;
        //                }
        //                else
        //                {
        //                    inventoryResultDetail.Initial = inventoryAsset.QInitial;
        //                    inventoryResultDetail.Actual = ((inventoryAsset.RoomIdFinal != null) && (inventoryAsset.RoomFinal.LocationId == inventoryAsset.RoomInitial.LocationId)) ? inventoryAsset.QFinal : 0;
        //                }
        //                break;
        //        }

        //        //if (reportType.ToUpper() == "MINUS")
        //        if (((inventoryResultDetail.Initial > 0) && (inventoryResultDetail.Actual == 0))) //minus
        //        {
        //            inventoryResultDetail.CostCenter = inventoryAsset.CostCenterIdInitial != null ? inventoryAsset.CostCenterInitial.Code : string.Empty;
        //            inventoryResultDetail.Building = inventoryAsset.RoomInitial.Location.Name;
        //            inventoryResultDetail.Room = inventoryAsset.RoomInitial.Name;
        //            inventoryResultDetail.InternalCode = inventoryAsset.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeInitial.InternalCode;
        //            inventoryResultDetail.FullName = inventoryAsset.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeInitial.FirstName + " " + inventoryAsset.EmployeeInitial.LastName;
        //        }
        //        else
        //        {
        //            inventoryResultDetail.CostCenter =
        //                (inventoryAsset.CostCenterIdFinal == null ? (inventoryAsset.CostCenterIdInitial != null ? inventoryAsset.CostCenterInitial.Code : string.Empty) : inventoryAsset.CostCenterFinal.Code);
        //            inventoryResultDetail.Building = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Location.Name : inventoryAsset.RoomInitial.Location.Name;
        //            inventoryResultDetail.Room = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Name : inventoryAsset.RoomInitial.Name;

        //            inventoryResultDetail.InternalCode = inventoryAsset.EmployeeFinal != null
        //                ? (inventoryAsset.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeFinal.InternalCode)
        //                : (inventoryAsset.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeInitial.InternalCode);

        //            inventoryResultDetail.FullName = inventoryAsset.EmployeeFinal != null
        //                ? (inventoryAsset.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeFinal.FirstName + " " + inventoryAsset.EmployeeFinal.LastName)
        //                : (inventoryAsset.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeInitial.FirstName + " " + inventoryAsset.EmployeeInitial.LastName);
        //        }

        //        //if (employeeId.HasValue)
        //        //{
        //        //    inventoryResultDetail.Initial =
        //        //        ((inventoryAsset.EmployeeIdInitial == employeeId) || (inventoryAsset.RoomIdFinal != null && inventoryAsset.EmployeeIdFinal == employeeId && inventoryAsset.RoomInitial.LocationId == inventoryAsset.RoomFinal.LocationId))
        //        //            ? inventoryAsset.QInitial : 0;
        //        //    inventoryResultDetail.Actual = ((inventoryAsset.EmployeeIdFinal != null) && (inventoryAsset.EmployeeIdFinal == employeeId)) ? inventoryAsset.QFinal : 0;
        //        //}

        //        //if (employeeId.HasValue)
        //        //{
        //        //    inventoryResultDetail.Initial = 
        //        //        ((inventoryAsset.EmployeeIdInitial == employeeId) || (inventoryAsset.RoomIdFinal != null && inventoryAsset.EmployeeIdFinal == employeeId && inventoryAsset.RoomInitial.LocationId == inventoryAsset.RoomFinal.LocationId)) 
        //        //            ? inventoryAsset.QInitial : 0;
        //        //    inventoryResultDetail.Actual = ((inventoryAsset.EmployeeIdFinal != null) && (inventoryAsset.EmployeeIdFinal == employeeId)) ? inventoryAsset.QFinal : 0;
        //        //}
        //        //else
        //        //{
        //        //    if (locationId.HasValue)
        //        //    {
        //        //        inventoryResultDetail.Initial = inventoryAsset.RoomInitial.LocationId == locationId ? inventoryAsset.QInitial : 0;
        //        //        inventoryResultDetail.Actual = ((inventoryAsset.RoomIdFinal != null) && (inventoryAsset.RoomFinal.LocationId == locationId)) ? inventoryAsset.QFinal : 0;
        //        //    }
        //        //    else if (regionId.HasValue)
        //        //    {

        //        //    }
        //        //}

        //        bool skip = false;

        //        if ((inventoryResultDetail.Initial == 0 && inventoryResultDetail.Actual == 0) ||
        //            ((employeeId.HasValue) && (inventoryAsset.RoomIdFinal != null)
        //            && (inventoryAsset.RoomFinal.LocationId == inventoryAsset.RoomInitial.LocationId)
        //            && (inventoryAsset.EmployeeIdInitial != inventoryAsset.EmployeeIdFinal)
        //            && (inventoryAsset.EmployeeIdInitial == employeeId.Value))) skip = true;

        //        if (!skip) invResult.Details.Add(inventoryResultDetail);
        //    }

        //    foreach (var assetNi in listAssetNi)
        //    {
        //        invResult.Details.Add(new InventoryResultDetail()
        //        {
        //            InvNo = assetNi.Code1,
        //            Description = assetNi.Name1,
        //            SerialNumber = assetNi.SerialNumber,
        //            PurchaseDate = null,
        //            CostCenter = assetNi.CostCenterCode,
        //            Room = assetNi.RoomName,
        //            Building = assetNi.LocationName,
        //            Initial = 0,
        //            Actual = assetNi.Quantity,
        //            Uom = "BUC",
        //            Value = 0,
        //            ValueInv = 0,
        //            ValueDep = 0,
        //            ValueDepTotal = 0,
        //            Info = assetNi.Info,
        //            Custody = assetNi.Custody.HasValue ? (assetNi.Custody.Value ? "DA" : "NU") : string.Empty,
        //            InternalCode = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.InternalCode,
        //            FullName = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.FirstName + " " + assetNi.LastName
        //        });
        //    }

        //    var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
        //    invResult.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
        //    invResult.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;
        //    //invResult.AdmCenterName = (region != null) ? region.Name : string.Empty;
        //    invResult.EndDate = DateTime.Now;
        //    invResult.LocationName = (location != null) ? location.Name : string.Empty;

        //    return invResult;
        //}

        //public InventoryResult GetInventoryListV2ByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? employeeId, string reportType, bool? custody, string internalCode)
        //{
        //    InventoryResult invResult = new InventoryResult();
        // //   Model.AdmCenter admCenter = null;
        //    Model.Location location = null;
        //    Model.Division division = null;
        //    Model.Region region = null;
        //    Model.Employee employee = null;
        //    Model.AccMonth accMonth = null;

        //    IQueryable<Model.InventoryAsset> invAssetQuery = null;
        //    IQueryable<Model.AssetAdmMD> assetAdmMD  = null;
        //  //  IQueryable<Dto.AssetNiInvDet> assetNiQuery = null;

        //    List<Model.InventoryAsset> listInventoryAssets = null;
        //    List<Dto.AssetNiInvDet> listAssetNi = null;

        //    accMonth = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).SingleOrDefault();

        //   // admCenter = (admCenterId.HasValue && admCenterId > 0) ? _context.Set<Model.AdmCenter>().Single(a => a.Id == admCenterId) : null;
        //    location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
        //    division = (divisionId.HasValue && divisionId > 0) ? _context.Set<Model.Division>().Single(l => l.Id == divisionId) : null;

        //    region = (regionId.HasValue && regionId > 0) ? _context.Set<Model.Region>().Single(r => r.Id == regionId) : null;

        //    if ((employeeId.HasValue) && (employeeId > 0))
        //        employee = _context.Set<Model.Employee>().Single(e => e.Id == employeeId);
        //    else if ((internalCode != null) && (internalCode.Length > 0))
        //    {
        //        employee = _context.Set<Model.Employee>().FirstOrDefault(e => e.InternalCode == internalCode);

        //        if (employee == null) return null;
        //    }


        //    invAssetQuery = _assetsRepository.GetInventoryAssetsQuery2(inventoryId, null);
        //  //  assetNiQuery = _assetNiRepository.GetAssetNiInvDetQuery(inventoryId);
        //    assetAdmMD = _assetsRepository.GetAssetAdmMDsQuery(inventoryId, accMonth.Id, null);



        //  //  invAssetQuery.Where(a => a.StateIdFinal == 1);
        //    //invAssetQuery = invAssetQuery.Where(i => i.RegionIdIni == regionId || i.RegionIdFin == regionId);
        //    //assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);

        //    //if (costCenterId.HasValue && (costCenterId > 0))
        //    //{
        //    //    invAssetQuery = invAssetQuery.Where(i => ((i.CostCenterIdInitial == costCenterId)
        //    //        || (i.CostCenterIdFinal == costCenterId)));
        //    //    assetNiQuery = assetNiQuery.Where(a => a.CostCenterId == costCenterId && a.IsDeleted == false);

        //    //    assetAdmMD = assetAdmMD.Where(i => ((i.CostCenterId == costCenterId)
        //    //        || (i.CostCenterId == costCenterId)));
        //    //}
        //    //else
        //    //{
        //    //    if (admCenterId.HasValue && (admCenterId > 0))
        //    //    {
        //    //        invAssetQuery = invAssetQuery.Where(i => i.CostCenterInitial.AdmCenterId == admCenterId
        //    //            || ((i.CostCenterIdFinal != null) && (i.CostCenterFinal.AdmCenterId == admCenterId)));
        //    //        assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId && a.IsDeleted == false);
        //    //    }
        //    //}

        //    if (locationId.HasValue && (locationId > 0))
        //    {
        //        invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.LocationId == locationId 
        //            || ((i.RoomIdFinal != null) && (i.RoomFinal.LocationId == locationId)));
        //       // assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId && a.IsDeleted == false);

        //        assetAdmMD = assetAdmMD.Where(i => i.Room.LocationId == locationId
        //          || ((i.Room != null) && (i.Room.LocationId == locationId)));

        //    }
        //    else
        //    {
        //        if (regionId.HasValue && (regionId > 0))
        //        {
        //            invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.Location.RegionId == regionId 
        //                || ((i.RoomIdFinal != null) && (i.RoomFinal.Location.RegionId == regionId)));
        //          //  assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId && a.IsDeleted == false);

        //            assetAdmMD = assetAdmMD.Where(i => i.Room.Location.RegionId == regionId
        //              || ((i.Room != null) && (i.Room.Location.RegionId == regionId)));
        //        }
        //    }



        //    //if (employeeId.HasValue && employeeId != 0)
        //    //{
        //    //    employee = _context.Set<Model.Employee>().Single(e => e.Id == employee.Id);
        //    //  //    invAssetQuery = invAssetQuery.Where(e => e.EmployeeIdInitial == employeeId || e.EmployeeIdFinal == employeeId); // BNR
        //    //    invAssetQuery = invAssetQuery.Where(e => e.EmployeeIdInitial == employeeId || e.EmployeeIdFinal == employeeId); // ALLIANTZ
        //    //    assetNiQuery = assetNiQuery.Where(a => a.EmployeeId == employeeId && a.IsDeleted == false);

        //    //    invResult.EmployeeInternalCode = employee.InternalCode;
        //    //    invResult.EmployeeFirstName = employee.FirstName;
        //    //    invResult.EmployeeLastName = employee.LastName;
        //    //}

        //    //if (employee != null)
        //    //{

        //    // //   invAssetQuery = invAssetQuery.Where(e => e.EmployeeIdInitial == employee.Id || e.EmployeeIdFinal == employee.Id); // BNR
        //    //    invAssetQuery = invAssetQuery.Where(e => e.EmployeeIdInitial == employeeId || e.EmployeeIdFinal == employeeId); // ALLIANTZ
        //    //    assetNiQuery = assetNiQuery.Where(a => a.EmployeeId == employee.Id && a.IsDeleted == false);
        //    //    assetAdmMD = assetAdmMD.Where(e => e.EmployeeId == employee.Id);

        //    //    invResult.EmployeeInternalCode = employee.InternalCode;
        //    //    invResult.EmployeeFirstName = employee.FirstName;
        //    //    invResult.EmployeeLastName = employee.LastName;
        //    //}

        //    if (divisionId.HasValue && divisionId != 0)
        //    {
        //        division = _context.Set<Model.Division>().Single(e => e.Id == division.Id);
        //        //    invAssetQuery = invAssetQuery.Where(e => e.EmployeeIdInitial == employeeId || e.EmployeeIdFinal == employeeId); // BNR
        //        invAssetQuery = invAssetQuery.Where(e => e.AdministrationInitial.DivisionId == divisionId || ((e.AdministrationIdFinal != null) && (e.AdministrationFinal.DivisionId == divisionId))); // allianz
        //     //   assetNiQuery = assetNiQuery.Where(a => a.EmployeeId == employeeId && a.IsDeleted == false);

        //        invResult.EmployeeInternalCode = division.Name;
        //        invResult.EmployeeFirstName = division.Name;
        //        invResult.EmployeeLastName = division.Name;
        //    }

        //    if (division != null)
        //    {

        //        //   invAssetQuery = invAssetQuery.Where(e => e.EmployeeIdInitial == employee.Id || e.EmployeeIdFinal == employee.Id); // BNR
        //        invAssetQuery = invAssetQuery.Where(e => e.AdministrationInitial.DivisionId == divisionId || ((e.AdministrationIdFinal != null) && (e.AdministrationFinal.DivisionId == divisionId))); // allianz
        //      //  assetNiQuery = assetNiQuery.Where(a => a.EmployeeId == employee.Id && a.IsDeleted == false);
        //        assetAdmMD = assetAdmMD.Where(e => e.Administration.DivisionId == division.Id);

        //        invResult.EmployeeInternalCode = division.Name;
        //        invResult.EmployeeFirstName = division.Name;
        //        invResult.EmployeeLastName = division.Name;
        //    }

        //    //if (custody.HasValue)
        //    //{
        //    //    invAssetQuery = invAssetQuery.Where(i => i.Asset.Custody == custody.GetValueOrDefault());
        //    //    assetNiQuery = assetNiQuery.Where(a => a.Custody == custody.GetValueOrDefault() && a.IsDeleted == false);
        //    //}

        //    if (reportType != null)
        //    {
        //        switch (reportType.ToUpper())
        //        {
        //            case "MINUS":
        //                if (location != null)
        //                {
        //                    invAssetQuery = invAssetQuery.Where(i => (((i.RoomIdFinal == null) && (i.RoomInitial.LocationId == locationId))
        //                        || ((i.RoomIdFinal != null) && (i.RoomInitial.LocationId == locationId) && (i.RoomInitial.LocationId != i.RoomFinal.LocationId))));
        //                }
        //                else
        //                {
        //                    if (region != null)
        //                    {
        //                        invAssetQuery = invAssetQuery.Where(i => (((i.CostCenterIdFinal == null) && (i.CostCenterIdInitial != null) && (i.CostCenterInitial.AdmCenterId == admCenterId))
        //                            || ((i.CostCenterIdFinal != null) && (i.CostCenterInitial.AdmCenterId == admCenterId) && (i.CostCenterInitial.AdmCenterId != i.CostCenterFinal.AdmCenterId))));
        //                    }
        //                    else
        //                    {
        //                        invAssetQuery = invAssetQuery.Where(i => (i.RoomIdFinal == null));
        //                    }
        //                }
        //              //  assetNiQuery = null;
        //                invResult.InventoryListType = "Minusuri";
        //                break;

        //            case "PLUS":
        //                invAssetQuery = invAssetQuery.Where(i => ((i.RoomIdFinal != null) && (i.RoomInitial.LocationId != i.RoomFinal.LocationId) && (i.RoomFinal.LocationId == locationId)));
        //              //  invAssetQuery = invAssetQuery.Where(i => ((i.RoomIdFinal != null) && (i.RoomFinal.LocationId == locationId)));  //BNR
        //                invResult.InventoryListType = "Plusuri";
        //                break;

        //            case "NOT_IDENTIFIED":
        //                invAssetQuery = null;
        //                invResult.InventoryListType = "Etichete temporare";
        //                break;

        //            case "GENERAL":
        //                assetAdmMD = null;
        //                invResult.InventoryListType = "Centralizator";
        //                break;

        //            default:
        //                invResult.InventoryListType = string.Empty;
        //                break;
        //        }
        //    }

        //    if (invAssetQuery != null)
        //    {
        //        invAssetQuery = invAssetQuery.OrderBy(i => i.Asset.InvNo);
        //        listInventoryAssets = invAssetQuery.Where(s => s.StateIdInitial != 24).ToList();
        //    }
        //    else
        //    {
        //        listInventoryAssets = new List<Model.InventoryAsset>();
        //    }

        //    //if (assetNiQuery != null)
        //    //{
        //    //    assetNiQuery = assetNiQuery.OrderBy(a => a.Code1);

        //    //    listAssetNi = assetNiQuery.ToList();
        //    //}
        //    //else
        //    //{
        //    //    listAssetNi = new List<Dto.AssetNiInvDet>();
        //    //}

        //    //listInventoryAssets = (invAssetQuery != null) ? invAssetQuery.ToList() : new List<InventoryAsset>();
        //    //listAssetNi = (assetNiQuery != null) ? assetNiQuery.ToList() : new List<Dto.AssetNiInvDet>();



        //    invResult.Details = new List<InventoryResultDetail>();

        //      foreach (var inventoryAsset in listInventoryAssets)
        //      {
        //          InventoryResultDetail inventoryResultDetail = null;
        //          inventoryResultDetail = new InventoryResultDetail()
        //          {
        //              InvNo = inventoryAsset.Asset.InvNo,
        //              Description = inventoryAsset.Asset.Name,
        //              SerialNumber = inventoryAsset.SerialNumber,
        //              ERPCode = inventoryAsset.Asset.ERPCode,
        //              PurchaseDate = inventoryAsset.Asset.PurchaseDate,



        //              CostCenter =
        //                  //(inventoryAsset.CostCenterIdFinal == null ? inventoryAsset.CostCenterInitial.Code : inventoryAsset.CostCenterFinal.Code),
        //                //  (inventoryAsset.CostCenterIdFinal == null ? (inventoryAsset.CostCenterIdInitial != null ? inventoryAsset.CostCenterInitial.Name : string.Empty) : inventoryAsset.CostCenterFinal.Name),
        //              (inventoryAsset.AdministrationIdFinal == null ? (inventoryAsset.AdministrationIdInitial != null ? inventoryAsset.AdministrationInitial.Name : string.Empty) : inventoryAsset.AdministrationFinal.Name),
        //              Building = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Location.Name : inventoryAsset.RoomInitial.Location.Name,
        //              BuildingName = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Location.Name : inventoryAsset.RoomInitial.Location.Name,
        //              BuildingCode = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Location.Code : inventoryAsset.RoomInitial.Location.Code,

        //              Room = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Name : inventoryAsset.RoomInitial.Name,
        //              RoomCode = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Code : inventoryAsset.RoomInitial.Code,
        //              RoomName = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Name : inventoryAsset.RoomInitial.Name,

        //            //  Initial = inventoryAsset.RoomInitial.LocationId == locationId ? 1 : 0,
        //            //  Actual = ((inventoryAsset.RoomIdFinal != null) && (inventoryAsset.RoomFinal.LocationId == locationId)) ? inventoryAsset.QFinal : 0,  // BNR
        //             // Actual = ((inventoryAsset.RoomIdFinal != null) && (inventoryAsset.RoomFinal.LocationId == locationId)) ? inventoryAsset.QFinal : 0,  // OTP
        //              Initial = inventoryAsset.QInitial,  // ALLIANTZ
        //              Actual = inventoryAsset.QFinal,  // ALLIANTZ

        //              //Initial = 0,
        //              //Actual = 0,

        //              //Uom = inventoryAsset.Asset.Uom.Name,
        //              Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,


        //              Value =  inventoryAsset.Asset.ValueInv,  // ALLIANTZ
        //             // Value = inventoryAsset.Asset.ValueRem,

        //              ValueInv = inventoryAsset.Asset.ValueInv,
        //              ValueDep = inventoryAsset.Asset.ValueRem,
        //              ValueDepTotal = inventoryAsset.Asset.ValueRem,
        //              Info = inventoryAsset.Info,
        //             // Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,

        //              InternalCode = inventoryAsset.EmployeeFinal != null
        //                  ? (inventoryAsset.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeFinal.InternalCode)
        //                  : (inventoryAsset.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeInitial.InternalCode),

        //              //FullName = inventoryAsset.EmployeeFinal != null
        //              //    ? (inventoryAsset.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeFinal.FirstName + " " + inventoryAsset.EmployeeFinal.LastName)
        //              //    : (inventoryAsset.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeInitial.FirstName + " " + inventoryAsset.EmployeeInitial.LastName),

        //            //  FullName = inventoryAsset.EmployeeFinal.InternalCode, // ALLIANTZ
        //              FullName = inventoryAsset.AdministrationIdFinal != null ? inventoryAsset.AdministrationFinal.Division.Name : inventoryAsset.AdministrationInitial.Division.Name, // ALLIANTZ

        //              AssetCategory = inventoryAsset.Asset.AssetCategoryId != null ? inventoryAsset.Asset.AssetCategory.Code : string.Empty,
        //              AssetCategoryPrefix = inventoryAsset.Asset.AssetCategoryId != null ? inventoryAsset.Asset.AssetCategory.Prefix : string.Empty,
        //              AssetType = inventoryAsset.Asset.AssetTypeId != null ? inventoryAsset.Asset.AssetType.Name : string.Empty

        //          };

        //          if (employeeId.HasValue)
        //          {
        //              inventoryResultDetail.Initial = inventoryAsset.EmployeeIdInitial == employeeId ? inventoryAsset.QInitial : 0;
        //              inventoryResultDetail.Actual = ((inventoryAsset.EmployeeIdFinal != null) && (inventoryAsset.EmployeeIdFinal == employeeId)) ? inventoryAsset.QFinal : 0;


        //          }
        //          else
        //          {
        //            if (locationId.HasValue)
        //            {
        //                inventoryResultDetail.Initial = inventoryAsset.RoomInitial.LocationId == locationId ? inventoryAsset.QInitial : 0;
        //                inventoryResultDetail.Actual = ((inventoryAsset.RoomIdFinal != null) && (inventoryAsset.RoomFinal.LocationId == locationId)) ? inventoryAsset.QFinal : 0;
        //            }
        //          }

        //        if (divisionId.HasValue)
        //        {
        //            inventoryResultDetail.Initial = inventoryAsset.AdministrationInitial.DivisionId == divisionId ? inventoryAsset.QInitial : 0;
        //            inventoryResultDetail.Actual = ((inventoryAsset.AdministrationIdFinal != null) && (inventoryAsset.AdministrationFinal.DivisionId == divisionId)) ? inventoryAsset.QFinal : 0;


        //        }



        //        //   int coef = ((inventoryAsset.EmployeeIdInitial != employeeId && inventoryAsset.EmployeeIdFinal == employeeId)
        //        //       || ((inventoryAsset.EmployeeIdInitial == employeeId) && ((inventoryAsset.EmployeeIdFinal == null) || (inventoryAsset.EmployeeIdFinal == employeeId)))) ? 1 : 0;

        //        ////   int coef = ((inventoryAsset.AdministrationIdFinal != null) && (inventoryAsset.AdministrationFinal.DivisionId == divisionId)) ? 1 : 0;
        //        //   inventoryResultDetail.Value = inventoryResultDetail.Value * (decimal)coef;
        //        //   inventoryResultDetail.ValueDep = inventoryResultDetail.ValueDep * (decimal)coef;  // ALLIANTZ

        //        inventoryResultDetail.Value = inventoryResultDetail.Value * (decimal)inventoryResultDetail.Actual;
        //        inventoryResultDetail.ValueDep = inventoryResultDetail.ValueDep * (decimal)inventoryResultDetail.Actual;  // ALLIANTZ






        //        bool skip = false;

        //        if ((divisionId.HasValue) && (inventoryAsset.RoomIdFinal != null)
        //            && (inventoryAsset.RoomFinal.LocationId == inventoryAsset.RoomInitial.LocationId)
        //            && (inventoryAsset.EmployeeIdInitial != inventoryAsset.EmployeeIdFinal)
        //            && (inventoryAsset.EmployeeIdInitial == employeeId.Value)) skip = true; 

        //        //if ((employeeId.HasValue) && (inventoryAsset.EmployeeIdFinal != null)

        //        //  && (inventoryAsset.EmployeeIdInitial != inventoryAsset.EmployeeIdFinal)
        //        //  && (inventoryAsset.EmployeeIdInitial == employeeId.Value)) skip = true;


        //        // ---ALLIANTZ --- //

        //        invResult.Details.Add(inventoryResultDetail); 
        //      } 

        //   //foreach (var assetNi in listAssetNi)
        //   // {
        //   //     invResult.Details.Add(new InventoryResultDetail()
        //   //     {
        //   //         InvNo = assetNi.Code1,
        //   //         Description = assetNi.Name1,
        //   //         SerialNumber = assetNi.SerialNumber,
        //   //         PurchaseDate = null,
        //   //         CostCenter = assetNi.CostCenterCode,
        //   //         Building = assetNi.LocationName,
        //   //         BuildingCode = assetNi.LocationCode,
        //   //         BuildingName = assetNi.LocationName,
        //   //         Room = assetNi.RoomName,
        //   //         RoomCode = assetNi.RoomCode,
        //   //         RoomName = assetNi.RoomName,
        //   //         Initial = 0,
        //   //         Actual = assetNi.Quantity,
        //   //         Uom = "BUC",
        //   //         Value = 0,
        //   //         ValueInv = 0,
        //   //         ValueDep = 0,
        //   //         ValueDepTotal = 0,
        //   //         Info = assetNi.Info,
        //   //         Custody = assetNi.Custody.HasValue ? (assetNi.Custody.Value ? "DA" : "NU") : string.Empty,
        //   //         InternalCode = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.InternalCode,
        //   //         FullName = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.FirstName + " " + assetNi.LastName



        //   //     });
        //   // }   // BNR + ALLIANTZ 

        //    var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
        //    invResult.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
        // //   invResult.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;
        //    invResult.RegionName = (region != null) ? (region.Code.Length > 0) ? region.Code : ""  + region.Name : string.Empty;
        //    invResult.EndDate = DateTime.Now;
        //    invResult.LocationName = (location != null) ? location.Name : string.Empty;



        //    return invResult;
        //}


        public List<InventoryResultDetail> GetMinusPlusItems(int inventoryId, int? admCenterId, int? regionId, int? locationId, int? employeeId)
        {
            Model.Inventory inventory = null;
            Model.AdmCenter admCenter = null;
            Model.Region region = null;
            Model.Location location = null;
            Model.Employee employee = null;
            Model.AccMonth accMonth = null;
            List<InventoryResultDetail> result = new List<InventoryResultDetail>();

            IQueryable<Model.Asset> assetQuery = null;
            IQueryable<AssetDepMD> depQuery = null;
            IQueryable<Model.AssetInventoryDetail> query = null;

            IQueryable<Model.InventoryAsset> invAssetQuery = null;
            IQueryable<Dto.AssetNiInvDet> assetNiQuery = null;

            List<Dto.AssetNiInvDet> listAssetNi = null;

            inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);

            if (inventory == null) return null;

            admCenter = (admCenterId.HasValue && admCenterId > 0) ? _context.Set<Model.AdmCenter>().Single(a => a.Id == admCenterId) : null;
            region = (regionId.HasValue && regionId > 0) ? _context.Set<Model.Region>().Single(a => a.Id == regionId) : null;
            location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;

            if ((employeeId.HasValue) && (employeeId > 0))
                employee = _context.Set<Model.Employee>().Single(e => e.Id == employeeId);

            //int accMonthId = 17;
            int accSystemId = 1;

            if (inventoryId > 0)
            {
                inventory = _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId).SingleOrDefault();

                if (inventory != null)
                {
                    accMonth = _context.Set<Model.AccMonth>().Where(a => a.Id == inventory.AccMonthId).SingleOrDefault();
                }
            }

            //switch (inventoryId)
            //{
            //    case 2:
            //        accMonthId = 17;
            //        break;
            //    case 4:
            //        accMonthId = 24;
            //        break;
            //    case 5:
            //        accMonthId = 20;
            //        break;
            //    case 6:
            //        accMonthId = 26;
            //        break;
            //    case 7:
            //        accMonthId = 28;
            //        break;
            //    case 8:
            //        accMonthId = 30;
            //        break;
            //    case 9:
            //        accMonthId = 30;
            //        break;
            //    default:
            //        break;
            //}

            //accMonth = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).SingleOrDefault();

            // assetFilter.InventoryId = 6;
            assetQuery = _context.Assets
                 .Include(u => u.Uom)
                .Include(r => r.Room)
                    .ThenInclude(l => l.Location)
                        .ThenInclude(l => l.Region)
                //.Include(c => c.Room)  
                //    .ThenInclude(a => a.Location)
                //        .ThenInclude(a => a.AdmCenter)
                .Include(e => e.Employee)
                .Include(a => a.AssetState)
                .AsQueryable();
            depQuery = _context.AssetDepMDs.AsQueryable().Where(a => a.AccSystemId == accSystemId && a.AccMonthId == accMonth.Id);

            invAssetQuery = _context.InventoryAssets
                .Include(r => r.RoomInitial)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                    //    .Include(r => r.RoomInitial)
                    //.ThenInclude(l => l.Location)
                    //   .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterInitial)
                    //.ThenInclude(a => a.AdmCenter)
                .Include(e => e.EmployeeInitial)
                .Include(s => s.StateInitial)
                .Include(r => r.RoomFinal)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                    //    .Include(r => r.RoomFinal)
                    //.ThenInclude(l => l.Location)
                    //   .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterFinal)
                    //.ThenInclude(r => r.AdmCenter)
                 .Include(e => e.EmployeeFinal)
                  .Include(s => s.StateFinal)

                //.Where(i => i.InventoryId == inventoryId && ((i.StateIdFinal == null) || ((i.StateIdFinal != 4) && (i.StateIdFinal != 5)))).AsQueryable(); // _assetsRepository.GetInventoryAssetsQuery2(inventory.Id, null);
                .Where(i => i.InventoryId == inventoryId).AsQueryable(); // _assetsRepository.GetInventoryAssetsQuery2(inventory.Id, null);

            assetNiQuery = _assetNiRepository.GetAssetNiInvDetQuery(inventory.Id);

            query = assetQuery.Select(asset => new Model.AssetInventoryDetail { Asset = asset });

            query = query
                .Join(invAssetQuery, q => q.Asset.Id, inv => inv.AssetId, (q, inv) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = inv });

            query = query
                .Join(depQuery, q => q.Asset.Id, dep => dep.AssetId, (q, dep) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = q.Inventory, Dep = dep });

            if (employee != null)
            {
                employeeId = employee.Id;

                query = query.Where(e => e.Inventory.EmployeeIdInitial == employeeId || e.Inventory.EmployeeIdFinal == employeeId);

                if (assetNiQuery != null)
                {
                    assetNiQuery = assetNiQuery.Where(a => a.EmployeeId == employeeId);
                }
            }

            if (locationId.HasValue)
            {
                query = query.Where(e => e.Inventory.RoomInitial.LocationId == locationId || e.Inventory.RoomFinal.LocationId == locationId);

                if (assetNiQuery != null)
                {
                    assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
                }
            }

            if ((admCenterId.HasValue) && (admCenterId > 0))
            {
                query = query.Where(e => e.Inventory.CostCenterInitial.AdmCenterId == admCenterId || e.Inventory.CostCenterFinal.AdmCenterId == admCenterId);

                if (assetNiQuery != null)
                {
                    assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
                }
            }

            if ((regionId.HasValue) && (regionId > 0))
            {
                query = query.Where(e => e.Inventory.RoomInitial.Location.RegionId == regionId || e.Inventory.RoomFinal.Location.RegionId == regionId);

                if (assetNiQuery != null)
                {
                    assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
                }
            }

            List<Model.AssetInventoryDetail> queryList = new List<Model.AssetInventoryDetail>();

            if (query != null)
            {
                query = query.OrderBy(i => i.Asset.InvNo);
                queryList = query.ToList();
            }

            if (assetNiQuery != null)
            {
                assetNiQuery = assetNiQuery.OrderBy(a => a.Code1);
                listAssetNi = assetNiQuery.ToList();
            }
            else
            {
                listAssetNi = new List<Dto.AssetNiInvDet>();
            }

            foreach (var inventoryAsset in queryList)
            {
                InventoryResultDetail inventoryResultDetail = null;
                inventoryResultDetail = new InventoryResultDetail()
                {
                    InvNo = inventoryAsset.Asset.InvNo,
                    Description = inventoryAsset.Asset.Name,
                    SerialNumber = inventoryAsset.Inventory.SerialNumber,
                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                    Initial = 0,
                    Actual = 0,

                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                    Value = inventoryAsset.Dep.CurrentAPC,
                    ValueInv = inventoryAsset.Dep.CurrentAPC,
                    ValueDep = inventoryAsset.Dep.AccumulDep,
                    ValueDepTotal = inventoryAsset.Dep.AccumulDep,
                    Info = string.Empty,
                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                    ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : inventoryAsset.Inventory.Inventory.Start
                };

                if (employeeId.HasValue)
                {
                    inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId && inventoryAsset.Inventory.EmployeeIdInitial == employeeId ? inventoryAsset.Inventory.QInitial : 0;
                    inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId) && inventoryAsset.Inventory.EmployeeIdFinal == employeeId) ? inventoryAsset.Inventory.QFinal : 0;
                }

                //if (reportType.ToUpper() == "MINUS")
                if (((inventoryResultDetail.Initial > 0) && (inventoryResultDetail.Actual == 0))) //minus
                {
                    inventoryResultDetail.CostCenter = inventoryAsset.Inventory.CostCenterIdInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty;
                    inventoryResultDetail.Building = inventoryAsset.Inventory.RoomInitial.Location.Name;
                    inventoryResultDetail.Room = inventoryAsset.Inventory.RoomInitial.Name;
                    inventoryResultDetail.InternalCode = inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.InternalCode;
                    inventoryResultDetail.FullName = inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName;
                }
                else
                {
                    inventoryResultDetail.CostCenter =
                        (inventoryAsset.Inventory.CostCenterIdFinal == null ? (inventoryAsset.Inventory.CostCenterIdInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty) : inventoryAsset.Inventory.CostCenterFinal.Code);
                    inventoryResultDetail.Building = inventoryAsset.Inventory.RoomIdFinal != null ? inventoryAsset.Inventory.RoomFinal.Location.Name : inventoryAsset.Inventory.RoomInitial.Location.Name;
                    inventoryResultDetail.Room = inventoryAsset.Inventory.RoomIdFinal != null ? inventoryAsset.Inventory.RoomFinal.Name : inventoryAsset.Inventory.RoomInitial.Name;

                    inventoryResultDetail.InternalCode = inventoryAsset.Inventory.EmployeeFinal != null
                        ? (inventoryAsset.Inventory.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.InternalCode)
                        : (inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.InternalCode);

                    inventoryResultDetail.FullName = inventoryAsset.Inventory.EmployeeFinal != null
                        ? (inventoryAsset.Inventory.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.FirstName + " " + inventoryAsset.Inventory.EmployeeFinal.LastName)
                        : (inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName);
                }

                bool skip = inventoryResultDetail.Initial == inventoryResultDetail.Actual ? true : false;

                if (!skip) result.Add(inventoryResultDetail);
            }

            foreach (var assetNi in listAssetNi)
            {
                result.Add(new InventoryResultDetail()
                {
                    InvNo = assetNi.Code1,
                    Description = assetNi.Name1,
                    SerialNumber = assetNi.SerialNumber,
                    PurchaseDate = null,
                    CostCenter = assetNi.CostCenterCode,
                    Room = assetNi.RoomName,
                    Building = assetNi.LocationName,
                    Initial = 0,
                    Actual = assetNi.Quantity,
                    Uom = "BUC",
                    Value = 0,
                    ValueInv = 0,
                    ValueDep = 0,
                    ValueDepTotal = 0,
                    Info = assetNi.Info,
                    Custody = assetNi.Custody.HasValue ? (assetNi.Custody.Value ? "DA" : "NU") : string.Empty,
                    InternalCode = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.InternalCode,
                    FullName = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.FirstName + " " + assetNi.LastName,
                    ModifiedAt = assetNi.ModifiedAt.Value
                });
            }

            //foreach (var item in result)
            //{
            //    if (item.Initial == item.Actual) result.Remove(item);
            //}

            return result;
        }

        public InventoryResult GetMinusPlus(int inventoryId, int? admCenterId, int? regionId, int? locationId, int? employeeId)
        {
            InventoryResult invResult = new InventoryResult();

            invResult.Details = GetMinusPlusItems(inventoryId, admCenterId, regionId, locationId, employeeId);

            Model.Inventory inventory = null;
            Model.AdmCenter admCenter = null;
            Model.Region region = null;
            Model.Location location = null;
            Model.Employee employee = null;


            inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);

            if (inventory == null) return null;

            admCenter = (admCenterId.HasValue && admCenterId > 0) ? _context.Set<Model.AdmCenter>().Single(a => a.Id == admCenterId) : null;
            region = (regionId.HasValue && regionId > 0) ? _context.Set<Model.Region>().Single(a => a.Id == regionId) : null;
            location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;

            if ((employeeId.HasValue) && (employeeId > 0))
                employee = _context.Set<Model.Employee>().Single(e => e.Id == employeeId);

            if (employee != null)
            {
                invResult.EmployeeInternalCode = employee.InternalCode;
                invResult.EmployeeFirstName = employee.FirstName;
                invResult.EmployeeLastName = employee.LastName;
            }

            var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            invResult.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            invResult.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;
            invResult.RegionName = (region != null) ? region.Name : string.Empty;

            //if (admCenter != null)
            //{
            //    if (admCenter.Committee != null)
            //    {
            //        if (admCenter.Committee.Length > 0)
            //        {
            //            string[] committees = admCenter.Committee.Split(';');

            //            invResult.Committee1 = committees[0];
            //            invResult.Committee2 = committees[1];
            //            invResult.Committee3 = committees[2];
            //        }
            //    }
            //}



            invResult.InventoryName = inventory.Description.Trim();
            // var maxDate = invResult.Details.Count > 0 ?  invResult.Details.Where(s=> s.ModifiedAt == invResult.Details.Where(p => p.ModifiedAt != null).Max(p => p.ModifiedAt)).FirstOrDefault() : null;
            var maxDate = invResult.Details.Count > 0 ? invResult.Details.Where(p => p.ModifiedAt != null).Max(p => p.ModifiedAt) : DateTime.Now;
            //invResult.InventoryEndDate = maxDate != null ? (maxDate.ModifiedAt != null ? maxDate.ModifiedAt.Value : DateTime.Now) : DateTime.Now;
            invResult.InventoryEndDate = maxDate.Value;
            invResult.LocationName = (location != null) ? location.Name : string.Empty;

            return invResult;
        }

        public InventoryResult GetGroupMinusPlus(int inventoryId, int? admCenterId, int? regionId, int? locationId)
        {
            InventoryResult invResult = new InventoryResult();

            //var initial = listInventoryAssets.Select(r => r.RoomInitial.LocationId).Distinct().ToList();
            var employeeIdsInitial = _context.InventoryAssets.Include(i => i.RoomInitial).Include(i => i.RoomFinal)
                .Where(i => (i.InventoryId == inventoryId) && (((i.RoomIdInitial != null) && (i.RoomInitial.LocationId == locationId))
                || ((i.RoomIdFinal != null) && (i.RoomFinal.LocationId == locationId))))
                .Select(i => i.EmployeeIdInitial).Distinct().ToList();

            var employeeIdsFinal = _context.InventoryAssets.Include(i => i.RoomInitial).Include(i => i.RoomFinal)
                .Where(i => (i.InventoryId == inventoryId) && (((i.RoomIdInitial != null) && (i.RoomInitial.LocationId == locationId))
                || ((i.RoomIdFinal != null) && (i.RoomFinal.LocationId == locationId)))).Where(i => i.EmployeeIdFinal != null)
                .Select(i => i.EmployeeIdFinal).Distinct().ToList();

            var employeeIds = employeeIdsInitial.Concat(employeeIdsFinal).Distinct();

            //invResult.Details = GetMinusPlusItems(inventoryId, admCenterId, locationId);
            invResult.Details = new List<InventoryResultDetail>();

            foreach (int employeeId in employeeIds)
            {
                var items = GetMinusPlusItems(inventoryId, admCenterId, regionId, locationId, employeeId);
                foreach (InventoryResultDetail item in items)
                {
                    invResult.Details.Add(item);
                }
            }

            Model.Inventory inventory = null;
            Model.AdmCenter admCenter = null;
            Model.Region region = null;
            Model.Location location = null;

            inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);

            if (inventory == null) return null;

            admCenter = (admCenterId.HasValue && admCenterId > 0) ? _context.Set<Model.AdmCenter>().Single(a => a.Id == admCenterId) : null;
            region = (regionId.HasValue && regionId > 0) ? _context.Set<Model.Region>().Single(a => a.Id == regionId) : null;
            location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;

            invResult.EmployeeInternalCode = string.Empty;
            invResult.EmployeeFirstName = string.Empty;
            invResult.EmployeeLastName = string.Empty;
            invResult.InventoryListType = "Minusuri / Plusuri";

            var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            invResult.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            invResult.AdmCenterName = (region != null) ? region.Name : string.Empty;

            //if (admCenter != null)
            //{
            //    if (admCenter.Committee != null)
            //    {
            //        if (admCenter.Committee.Length > 0)
            //        {
            //            string[] committees = admCenter.Committee.Split(';');

            //            invResult.Committee1 = committees[0];
            //            invResult.Committee2 = committees[1];
            //            invResult.Committee3 = committees[2];
            //        }
            //    }
            //}

            invResult.InventoryName = inventory.Description.Trim();
            // var maxDate = invResult.Details.Count > 0 ?  invResult.Details.Where(s=> s.ModifiedAt == invResult.Details.Where(p => p.ModifiedAt != null).Max(p => p.ModifiedAt)).FirstOrDefault() : null;
            var maxDate = invResult.Details.Count > 0 ? invResult.Details.Where(p => p.ModifiedAt != null).Max(p => p.ModifiedAt) : DateTime.Now;
            //invResult.InventoryEndDate = maxDate != null ? (maxDate.ModifiedAt != null ? maxDate.ModifiedAt.Value : DateTime.Now) : DateTime.Now;
            invResult.InventoryEndDate = maxDate.Value;
            invResult.LocationName = (location != null) ? location.Name : string.Empty;

            return invResult;
        }

        public InventoryResult GetMinusPlusInventoryListV2ByFilters(int inventoryId, int? admCenterId, int? locationId, int? employeeId)
        {
            InventoryResult invResult = new InventoryResult();
            Model.Inventory inventory = null;
            Model.AdmCenter admCenter = null;
            Model.Location location = null;
            Model.Employee employee = null;
            Model.AccMonth accMonth = null;

            IQueryable<Model.Asset> assetQuery = null;
            IQueryable<AssetDepMD> depQuery = null;
            IQueryable<Model.AssetInventoryDetail> query = null;

            IQueryable<Model.InventoryAsset> invAssetQuery = null;
            IQueryable<Dto.AssetNiInvDet> assetNiQuery = null;

            // List<Model.InventoryAsset> listInventoryAssets = null;
            List<Dto.AssetNiInvDet> listAssetNi = null;

            inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);

            if (inventory == null) return null;

            admCenter = (admCenterId.HasValue && admCenterId > 0) ? _context.Set<Model.AdmCenter>().Single(a => a.Id == admCenterId) : null;
            location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;

            if ((employeeId.HasValue) && (employeeId > 0))
                employee = _context.Set<Model.Employee>().Single(e => e.Id == employeeId);



            //int accMonthId = 17;
            int accSystemId = 1;

            if (inventoryId > 0)
            {
                inventory = _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId).SingleOrDefault();

                if (inventory != null)
                {
                    accMonth = _context.Set<Model.AccMonth>().Where(a => a.Id == inventory.AccMonthId).SingleOrDefault();
                }
            }

            //switch (inventoryId)
            //{
            //    case 2:
            //        accMonthId = 17;
            //        break;
            //    case 4:
            //        accMonthId = 24;
            //        break;
            //    case 5:
            //        accMonthId = 20;
            //        break;
            //    case 6:
            //        accMonthId = 26;
            //        break;
            //    case 7:
            //        accMonthId = 28;
            //        break;
            //    case 8:
            //        accMonthId = 30;
            //        break;
            //    case 9:
            //        accMonthId = 30;
            //        break;
            //    default:
            //        break;
            //}

            //accMonth = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).SingleOrDefault();

            // assetFilter.InventoryId = 6;
            assetQuery = _context.Assets
                 .Include(u => u.Uom)
                .Include(r => r.Room)
                    .ThenInclude(l => l.Location)
                .Include(c => c.CostCenter)
                    .ThenInclude(a => a.AdmCenter)
                .Include(e => e.Employee)
                .Include(a => a.AssetState)
                .AsQueryable();
            depQuery = _context.AssetDepMDs.AsQueryable().Where(a => a.AccSystemId == accSystemId && a.AccMonthId == accMonth.Id);

            invAssetQuery = _context.InventoryAssets
                .Include(r => r.RoomInitial)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.AdmCenter)
                .Include(r => r.CostCenterInitial)
                    .ThenInclude(a => a.AdmCenter)
                .Include(e => e.EmployeeInitial)
                .Include(s => s.StateInitial)
                .Include(r => r.RoomFinal)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.AdmCenter)
                .Include(r => r.CostCenterFinal)
                    .ThenInclude(r => r.AdmCenter)
                 .Include(e => e.EmployeeFinal)
                  .Include(s => s.StateFinal)

                .Where(i => i.InventoryId == inventoryId && ((i.StateIdFinal == null) || ((i.StateIdFinal != 4) && (i.StateIdFinal != 5)))).AsQueryable(); // _assetsRepository.GetInventoryAssetsQuery2(inventory.Id, null);
            assetNiQuery = _assetNiRepository.GetAssetNiInvDetQuery(inventory.Id);


            query = assetQuery.Select(asset => new Model.AssetInventoryDetail { Asset = asset });

            query = query
                .Join(invAssetQuery, q => q.Asset.Id, inv => inv.AssetId, (q, inv) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = inv });

            query = query
                .Join(depQuery, q => q.Asset.Id, dep => dep.AssetId, (q, dep) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = q.Inventory, Dep = dep });

            if (employee != null)
            {
                employeeId = employee.Id;

                query = query.Where(e => e.Inventory.EmployeeIdInitial == employeeId || e.Inventory.EmployeeIdFinal == employeeId);

                if (assetNiQuery != null)
                {
                    assetNiQuery = assetNiQuery.Where(a => a.EmployeeId == employeeId);
                }

                invResult.EmployeeInternalCode = employee.InternalCode;
                invResult.EmployeeFirstName = employee.FirstName;
                invResult.EmployeeLastName = employee.LastName;
            }

            if (locationId.HasValue)
            {
                query = query.Where(e => e.Inventory.RoomInitial.LocationId == locationId || e.Inventory.RoomFinal.LocationId == locationId);

                if (assetNiQuery != null)
                {
                    assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
                }
            }

            if ((admCenterId.HasValue) && (admCenterId > 0))
            {
                query = query.Where(e => e.Inventory.CostCenterInitial.AdmCenterId == admCenterId || e.Inventory.CostCenterFinal.AdmCenterId == admCenterId);

                if (assetNiQuery != null)
                {
                    assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
                }
            }

            List<Model.AssetInventoryDetail> queryList = new List<Model.AssetInventoryDetail>();

            if (query != null)
            {
                query = query.OrderBy(i => i.Asset.InvNo);
                queryList = query.ToList();
            }

            if (assetNiQuery != null)
            {
                assetNiQuery = assetNiQuery.OrderBy(a => a.Code1);
                listAssetNi = assetNiQuery.ToList();
            }
            else
            {
                listAssetNi = new List<Dto.AssetNiInvDet>();
            }

            invResult.Details = new List<InventoryResultDetail>();

            foreach (var inventoryAsset in queryList)
            {
                InventoryResultDetail inventoryResultDetail = null;
                inventoryResultDetail = new InventoryResultDetail()
                {
                    InvNo = inventoryAsset.Asset.InvNo,
                    Description = inventoryAsset.Asset.Name,
                    SerialNumber = inventoryAsset.Inventory.SerialNumber,
                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                    Initial = 0,
                    Actual = 0,

                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                    Value = inventoryAsset.Dep.CurrentAPC,
                    ValueInv = inventoryAsset.Dep.CurrentAPC,
                    ValueDep = inventoryAsset.Dep.AccumulDep,
                    ValueDepTotal = inventoryAsset.Dep.AccumulDep,
                    Info = string.Empty,
                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                    ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : inventoryAsset.Inventory.Inventory.Start
                };


                if (employeeId.HasValue)
                {
                    inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId && inventoryAsset.Inventory.EmployeeIdInitial == employeeId ? inventoryAsset.Inventory.QInitial : 0;
                    inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId) && inventoryAsset.Inventory.EmployeeIdFinal == employeeId) ? inventoryAsset.Inventory.QFinal : 0;
                }

                //if (reportType.ToUpper() == "MINUS")
                if (((inventoryResultDetail.Initial > 0) && (inventoryResultDetail.Actual == 0))) //minus
                {
                    inventoryResultDetail.CostCenter = inventoryAsset.Inventory.CostCenterIdInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty;
                    inventoryResultDetail.Building = inventoryAsset.Inventory.RoomInitial.Location.Name;
                    inventoryResultDetail.Room = inventoryAsset.Inventory.RoomInitial.Name;
                    inventoryResultDetail.InternalCode = inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.InternalCode;
                    inventoryResultDetail.FullName = inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName;
                }
                else
                {
                    inventoryResultDetail.CostCenter =
                        (inventoryAsset.Inventory.CostCenterIdFinal == null ? (inventoryAsset.Inventory.CostCenterIdInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty) : inventoryAsset.Inventory.CostCenterFinal.Code);
                    inventoryResultDetail.Building = inventoryAsset.Inventory.RoomIdFinal != null ? inventoryAsset.Inventory.RoomFinal.Location.Name : inventoryAsset.Inventory.RoomInitial.Location.Name;
                    inventoryResultDetail.Room = inventoryAsset.Inventory.RoomIdFinal != null ? inventoryAsset.Inventory.RoomFinal.Name : inventoryAsset.Inventory.RoomInitial.Name;

                    inventoryResultDetail.InternalCode = inventoryAsset.Inventory.EmployeeFinal != null
                        ? (inventoryAsset.Inventory.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.InternalCode)
                        : (inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.InternalCode);

                    inventoryResultDetail.FullName = inventoryAsset.Inventory.EmployeeFinal != null
                        ? (inventoryAsset.Inventory.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.FirstName + " " + inventoryAsset.Inventory.EmployeeFinal.LastName)
                        : (inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName);
                }

                invResult.Details.Add(inventoryResultDetail);
            }

            foreach (var assetNi in listAssetNi)
            {
                invResult.Details.Add(new InventoryResultDetail()
                {
                    InvNo = assetNi.Code1,
                    Description = assetNi.Name1,
                    SerialNumber = assetNi.SerialNumber,
                    PurchaseDate = null,
                    CostCenter = assetNi.CostCenterCode,
                    Room = assetNi.RoomName,
                    Building = assetNi.LocationName,
                    Initial = 0,
                    Actual = assetNi.Quantity,
                    Uom = "BUC",
                    Value = 0,
                    ValueInv = 0,
                    ValueDep = 0,
                    ValueDepTotal = 0,
                    Info = assetNi.Info,
                    Custody = assetNi.Custody.HasValue ? (assetNi.Custody.Value ? "DA" : "NU") : string.Empty,
                    InternalCode = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.InternalCode,
                    FullName = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.FirstName + " " + assetNi.LastName,
                    ModifiedAt = assetNi.ModifiedAt.Value
                });
            }

            //  inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);
            //inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventory.Id); // CEC

            var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            invResult.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            invResult.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;

            //if (admCenter != null)
            //{
            //    if (admCenter.Committee != null)
            //    {
            //        if (admCenter.Committee.Length > 0)
            //        {
            //            string[] committees = admCenter.Committee.Split(';');

            //            invResult.Committee1 = committees[0];
            //            invResult.Committee2 = committees[1];
            //            invResult.Committee3 = committees[2];
            //        }
            //    }
            //}

            invResult.InventoryName = inventory.Description.Trim();
            // var maxDate = invResult.Details.Count > 0 ?  invResult.Details.Where(s=> s.ModifiedAt == invResult.Details.Where(p => p.ModifiedAt != null).Max(p => p.ModifiedAt)).FirstOrDefault() : null;
            var maxDate = invResult.Details.Count > 0 ? invResult.Details.Where(p => p.ModifiedAt != null).Max(p => p.ModifiedAt) : DateTime.Now;
            //invResult.InventoryEndDate = maxDate != null ? (maxDate.ModifiedAt != null ? maxDate.ModifiedAt.Value : DateTime.Now) : DateTime.Now;
            invResult.InventoryEndDate = maxDate.Value;
            invResult.LocationName = (location != null) ? location.Name : string.Empty;

            foreach (var item in invResult.Details.ToList())
            {
                if (item.Initial == item.Actual) invResult.Details.Remove(item);
            }

            return invResult;
        }

        public InventoryResult GetInventoryListV2ByFilters(int? inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? employeeId, string reportType, bool? custody, string internalCode)
        {
            if (reportType == "MINUS/PLUS") return GetMinusPlus(inventoryId.Value, admCenterId, regionId, locationId, employeeId);
            if (reportType == "GROUPEMP") return GetGroupMinusPlus(inventoryId.Value, admCenterId, regionId, locationId);

            InventoryResult invResult = new InventoryResult();
            Model.Inventory inventory = null;
            Model.AdmCenter admCenter = null;
            Model.Location location = null;
            Model.Region region = null;
            Model.Employee employee = null;
            Model.AccMonth accMonth = null;

            IQueryable<Model.Asset> assetQuery = null;
            IQueryable<AssetDepMD> depQuery = null;
            IQueryable<Model.AssetInventoryDetail> query = null;
            InventoryResult queryEmp = null;

            IQueryable<Model.InventoryAsset> invAssetQuery = null;
            IQueryable<Dto.AssetNiInvDet> assetNiQuery = null;

            // List<Model.InventoryAsset> listInventoryAssets = null;
            List<Dto.AssetNiInvDet> listAssetNi = null;

            if ((inventoryId.HasValue) && (inventoryId > 0))
                inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);
            else
                inventory = _context.Set<Model.Inventory>().OrderByDescending(i => i.Id).FirstOrDefault(i => i.Active == true);

            if (inventory == null) return null;

            admCenter = (admCenterId.HasValue && admCenterId > 0) ? _context.Set<Model.AdmCenter>().Single(a => a.Id == admCenterId) : null;
            location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
            region = (regionId.HasValue && regionId > 0) ? _context.Set<Model.Region>().Single(r => r.Id == regionId) : null;

            if ((employeeId.HasValue) && (employeeId > 0))
                employee = _context.Set<Model.Employee>().Single(e => e.Id == employeeId);
            else if ((internalCode != null) && (internalCode.Length > 0))
            {
                employee = _context.Set<Model.Employee>().FirstOrDefault(e => ((e.InternalCode == internalCode) && (e.ERPCode.ToUpper() == "PUBLIC")));

                if (employee == null) return null;
            }

            //if ((employee == null) && (admCenter == null) && (region == null) && (location == null)) return null;

            //int accMonthId = 17;
            int accSystemId = 1;

            if (inventoryId > 0)
            {
                inventory = _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId).SingleOrDefault();

                if (inventory != null)
                {
                    accMonth = _context.Set<Model.AccMonth>().Where(a => a.Id == inventory.AccMonthId).SingleOrDefault();
                }
            }



            //switch (inventoryId)
            //{
            //    case 2:
            //        accMonthId = 17;
            //        break;
            //    case 4:
            //        accMonthId = 24;
            //        break;
            //    case 5:
            //        accMonthId = 20;
            //        break;
            //    case 6:
            //        accMonthId = 26;
            //        break;
            //    case 7:
            //        accMonthId = 28;
            //        break;
            //    case 8:
            //        accMonthId = 30;
            //        break;
            //    case 9:
            //        accMonthId = 30;
            //        break;
            //    default:
            //        break;
            //}

            //accMonth = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).SingleOrDefault();

            // assetFilter.InventoryId = 6;
            assetQuery = _context.Assets
                 .Include(u => u.Uom)
                .Include(r => r.Room)
                    .ThenInclude(l => l.Location)
                        .ThenInclude(l => l.Region)
                .Include(c => c.CostCenter)
                //    .ThenInclude(a => a.AdmCenter)
                .Include(e => e.Employee)
                .Include(a => a.AssetState)
                .AsQueryable();
            depQuery = _context.AssetDepMDs.AsQueryable().Where(a => a.AccSystemId == accSystemId && a.AccMonthId == accMonth.Id);

            invAssetQuery = _context.InventoryAssets
                .Include(r => r.RoomInitial)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                //.Include(r => r.CostCenterInitial)
                //    .ThenInclude(a => a.AdmCenter)
                .Include(e => e.EmployeeInitial)
                .Include(s => s.StateInitial)
                .Include(r => r.RoomFinal)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterFinal)
                //    .ThenInclude(r => r.AdmCenter)
                 .Include(e => e.EmployeeFinal)
                  .Include(s => s.StateFinal)

                .Where(i => i.InventoryId == inventoryId).AsQueryable(); // _assetsRepository.GetInventoryAssetsQuery2(inventory.Id, null);
            assetNiQuery = _assetNiRepository.GetAssetNiInvDetQuery(inventory.Id);


            query = assetQuery.Select(asset => new Model.AssetInventoryDetail { Asset = asset });


            query = query
                .Join(invAssetQuery, q => q.Asset.Id, inv => inv.AssetId, (q, inv) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = inv });



            query = query
                .Join(depQuery, q => q.Asset.Id, dep => dep.AssetId, (q, dep) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = q.Inventory, Dep = dep });


            if (reportType != null)
            {
                switch (reportType.ToUpper())
                {
                    case "MINUS":



                        if ((locationId.HasValue) || (locationId != null))
                        {
                            //query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && ((i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId || (i.Inventory.RoomIdInitial  == i.Inventory.RoomIdFinal && i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))) ));
                            query = query.Where(i => ((i.Inventory.RoomInitial.LocationId == locationId) && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)));

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
                            }
                        }
                        else
                        {
                            //if (admCenterId.HasValue)
                            //{
                            //    query = query.Where(i => ((i.Inventory.CostCenterInitial.AdmCenterId == admCenterId) && (i.Inventory.CostCenterInitial.AdmCenterId != i.Inventory.CostCenterFinal.AdmCenterId)));

                            //    if (assetNiQuery != null)
                            //    {
                            //        assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
                            //    }
                            //}

                            if (regionId.HasValue)
                            {
                                query = query.Where(i => ((i.Inventory.RoomInitial.Location.RegionId == regionId) && (i.Inventory.RoomInitial.Location.RegionId != i.Inventory.RoomFinal.Location.RegionId)));

                                if (assetNiQuery != null)
                                {
                                    assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
                                }
                            }
                        }

                        //if (admCenterId == null && locationId == null)
                        //{
                        //    query = query.Where(i => ((i.Inventory.CostCenterIdFinal == null)));
                        //}

                        if (regionId == null && locationId == null)
                        {
                            query = query.Where(i => ((i.Inventory.RoomIdFinal == null)));
                        }

                        assetNiQuery = null;
                        invResult.InventoryListType = "Minusuri";
                        break;


                    case "MINUS/PLUS":

                        if (employee != null)
                        {
                            employeeId = employee.Id;

                            query = query.Where(e => e.Inventory.EmployeeIdInitial == employeeId || e.Inventory.EmployeeIdFinal == employeeId);

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.EmployeeId == employeeId);
                            }

                            invResult.EmployeeInternalCode = employee.InternalCode;
                            invResult.EmployeeFirstName = employee.FirstName;
                            invResult.EmployeeLastName = employee.LastName;
                        }

                        if (locationId.HasValue)
                        {

                            query = query.Where(e => e.Inventory.RoomInitial.LocationId == locationId || e.Inventory.RoomFinal.LocationId == locationId);

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
                            }

                        }

                        //if ((admCenterId.HasValue) && (admCenterId > 0))
                        //{
                        //    query = query.Where(e => e.Inventory.CostCenterInitial.AdmCenterId == admCenterId || e.Inventory.CostCenterFinal.AdmCenterId == admCenterId);

                        //    if (assetNiQuery != null)
                        //    {
                        //        assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
                        //    }
                        //}

                        if ((regionId.HasValue) && (regionId > 0))
                        {
                            query = query.Where(e => e.Inventory.RoomInitial.Location.RegionId == regionId || e.Inventory.RoomFinal.Location.RegionId == regionId);

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
                            }
                        }



                        break;

                    case "PLUS":


                        if ((locationId.HasValue) || (locationId != null))
                        {
                            //query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && ((i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId || (i.Inventory.RoomIdInitial  == i.Inventory.RoomIdFinal && i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))) ));
                            query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomFinal.LocationId == locationId) && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)));

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
                            }
                        }
                        else
                        {
                            //if (admCenterId.HasValue)
                            //{
                            //    query = query.Where(i => ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)
                            //    && (i.Inventory.CostCenterInitial.AdmCenterId != i.Inventory.CostCenterFinal.AdmCenterId)));

                            //    if (assetNiQuery != null)
                            //    {
                            //        assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
                            //    }
                            //}

                            if (regionId.HasValue)
                            {
                                query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomFinal.Location.RegionId == regionId)
                                && (i.Inventory.RoomInitial.Location.RegionId != i.Inventory.RoomFinal.Location.RegionId)));

                                if (assetNiQuery != null)
                                {
                                    assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
                                }
                            }
                        }

                        //if (admCenterId == null && locationId == null)
                        //{
                        //    query = query.Where(i => i.Inventory.CostCenterIdFinal == 222222222);

                        //    assetNiQuery = assetNiQuery;
                        //}

                        if (regionId == null && locationId == null)
                        {
                            query = query.Where(i => i.Inventory.RoomIdFinal == 222222222);

                            assetNiQuery = assetNiQuery;
                        }


                        invResult.InventoryListType = "Plusuri";
                        break;

                    case "NOT_IDENTIFIED":

                        query = null;

                        if (locationId.HasValue)
                        {

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
                            }

                        }

                        //if ((admCenterId.HasValue) && (admCenterId > 0))
                        //{

                        //    if (assetNiQuery != null)
                        //    {
                        //        assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
                        //    }
                        //}

                        if ((regionId.HasValue) && (regionId > 0))
                        {

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
                            }
                        }


                        if (employee != null)
                        {
                            employeeId = employee.Id;

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.EmployeeId == employeeId);
                            }

                            invResult.EmployeeInternalCode = employee.InternalCode;
                            invResult.EmployeeFirstName = employee.FirstName;
                            invResult.EmployeeLastName = employee.LastName;
                        }
                        invResult.InventoryListType = "Etichete temporare";
                        break;

                    case "PLUS/MINUS":
                        invResult.InventoryListType = "Plusuri/Minusuri";
                        break;

                    case "GROUPEMP":



                        if (locationId.HasValue)
                        {

                            query = query.Where(e => e.Inventory.RoomInitial.LocationId == locationId || e.Inventory.RoomFinal.LocationId == locationId);

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
                            }

                        }

                        //if ((admCenterId.HasValue) && (admCenterId > 0))
                        //{
                        //    query = query.Where(e => e.Inventory.CostCenterInitial.AdmCenterId == admCenterId || e.Inventory.CostCenterFinal.AdmCenterId == admCenterId);

                        //    if (assetNiQuery != null)
                        //    {
                        //        assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
                        //    }
                        //}

                        if ((regionId.HasValue) && (regionId > 0))
                        {
                            query = query.Where(e => e.Inventory.RoomInitial.Location.RegionId == regionId || e.Inventory.RoomFinal.Location.RegionId == regionId);

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
                            }
                        }


                        invResult.InventoryListType = "Plusuri/Minusuri";
                        break;

                    default:
                        invResult.InventoryListType = string.Empty;

                        if (locationId.HasValue)
                        {

                            query = query.Where(e => e.Inventory.RoomInitial.LocationId == locationId || e.Inventory.RoomFinal.LocationId == locationId);

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
                            }

                        }

                        //if ((admCenterId.HasValue) && (admCenterId > 0))
                        //{
                        //    query = query.Where(e => e.Inventory.CostCenterInitial.AdmCenterId == admCenterId || e.Inventory.CostCenterFinal.AdmCenterId == admCenterId);

                        //    if (assetNiQuery != null)
                        //    {
                        //        assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
                        //    }
                        //}

                        if ((regionId.HasValue) && (regionId > 0))
                        {
                            query = query.Where(e => e.Inventory.RoomInitial.Location.RegionId == regionId || e.Inventory.RoomFinal.Location.RegionId == regionId);

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
                            }
                        }

                        if (employee != null)
                        {
                            employeeId = employee.Id;

                            query = query.Where(e => e.Inventory.EmployeeIdInitial == employeeId || e.Inventory.EmployeeIdFinal == employeeId);

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.EmployeeId == employeeId);
                            }

                            invResult.EmployeeInternalCode = employee.InternalCode;
                            invResult.EmployeeFirstName = employee.FirstName;
                            invResult.EmployeeLastName = employee.LastName;
                        }
                        break;
                }
            }



            if (custody.HasValue)
            {
                if (query != null)
                {
                    query = query.Where(i => i.Asset.Custody == custody.GetValueOrDefault());
                }

                if (assetNiQuery != null)
                {
                    assetNiQuery = assetNiQuery.Where(a => a.Custody == custody.GetValueOrDefault());
                }
            }


            List<Model.AssetInventoryDetail> queryList = new List<Model.AssetInventoryDetail>();
            List<Model.AssetInventoryDetail> queryEmpList = new List<Model.AssetInventoryDetail>();

            queryEmp = new InventoryResult();

            queryEmp.Details = new List<InventoryResultDetail>();

            if (query != null)
            {
                query = query.OrderBy(i => i.Asset.InvNo);
                queryList = query.ToList();

                if (reportType == "GROUPEMP")
                {

                    var initial = query.Select(r => r.Inventory.RoomInitial.LocationId).Distinct().ToList();
                    var final = query.Select(r => r.Inventory.RoomIdFinal != null ? r.Inventory.RoomFinal.LocationId : 0).Distinct().ToList();

                    for (int i = 0; i < initial.Count(); i++)
                    {
                        if (admCenterId != null)
                        {
                            foreach (var inventoryAsset in query.Where(r => r.Inventory.RoomInitial.LocationId == initial[i] && r.Inventory.RoomInitial.Location.AdmCenterId == admCenterId))
                            {
                                queryEmp.Details.Add(new InventoryResultDetail()
                                {
                                    InvNo = inventoryAsset.Asset.InvNo,
                                    Description = inventoryAsset.Asset.Name,
                                    SerialNumber = inventoryAsset.Inventory.SerialNumber,
                                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                                    Initial = 0,
                                    Actual = 0,

                                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                                    Value = inventoryAsset.Dep.CurrentAPC,
                                    ValueInv = inventoryAsset.Dep.CurrentAPC,
                                    ValueDep = inventoryAsset.Dep.AccumulDep,
                                    ValueDepTotal = inventoryAsset.Dep.AccumulDep,
                                    Info = string.Empty,
                                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                                    ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : inventoryAsset.Inventory.Inventory.Start
                                });
                            }
                        }
                        else
                        {
                            foreach (var inventoryAsset in query.Where(r => r.Inventory.RoomInitial.LocationId == initial[i]))
                            {
                                queryEmp.Details.Add(new InventoryResultDetail()
                                {
                                    InvNo = inventoryAsset.Asset.InvNo,
                                    Description = inventoryAsset.Asset.Name,
                                    SerialNumber = inventoryAsset.Inventory.SerialNumber,
                                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                                    Initial = 0,
                                    Actual = 0,

                                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                                    Value = inventoryAsset.Dep.CurrentAPC,
                                    ValueInv = inventoryAsset.Dep.CurrentAPC,
                                    ValueDep = inventoryAsset.Dep.AccumulDep,
                                    ValueDepTotal = inventoryAsset.Dep.AccumulDep,
                                    Info = string.Empty,
                                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                                    ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : inventoryAsset.Inventory.Inventory.Start
                                });
                            }
                        }

                    }


                    for (int i = 0; i < final.Count(); i++)
                    {
                        if (admCenterId != null)
                        {
                            foreach (var inventoryAsset in query.Where(r => r.Inventory.RoomFinal.LocationId == final[i] && r.Inventory.RoomFinal.Location.AdmCenterId == admCenterId /*&& (r.RoomInitial.LocationId != r.RoomFinal.LocationId || r.EmployeeIdInitial != r.EmployeeIdFinal) */))
                            {
                                //if (inventoryAsset.RoomInitial.LocationId != inventoryAsset.RoomFinal.LocationId || inventoryAsset.EmployeeIdInitial != inventoryAsset.EmployeeIdFinal)
                                //{
                                queryEmp.Details.Add(new InventoryResultDetail()
                                {
                                    InvNo = inventoryAsset.Asset.InvNo,
                                    Description = inventoryAsset.Asset.Name,
                                    SerialNumber = inventoryAsset.Inventory.SerialNumber,
                                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                                    Initial = 0,
                                    Actual = 0,

                                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                                    Value = inventoryAsset.Dep.CurrentAPC,
                                    ValueInv = inventoryAsset.Dep.CurrentAPC,
                                    ValueDep = inventoryAsset.Dep.AccumulDep,
                                    ValueDepTotal = inventoryAsset.Dep.AccumulDep,
                                    Info = string.Empty,
                                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                                    ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : inventoryAsset.Inventory.Inventory.Start
                                });
                                //}

                            }
                        }
                        else
                        {
                            foreach (var inventoryAsset in query.Where(r => r.Inventory.RoomFinal.LocationId == final[i]))
                            {

                                queryEmp.Details.Add(new InventoryResultDetail()
                                {
                                    InvNo = inventoryAsset.Asset.InvNo,
                                    Description = inventoryAsset.Asset.Name,
                                    SerialNumber = inventoryAsset.Inventory.SerialNumber,
                                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                                    Initial = 0,
                                    Actual = 0,

                                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                                    Value = inventoryAsset.Dep.CurrentAPC,
                                    ValueInv = inventoryAsset.Dep.CurrentAPC,
                                    ValueDep = inventoryAsset.Dep.AccumulDep,
                                    ValueDepTotal = inventoryAsset.Dep.AccumulDep,
                                    Info = string.Empty,
                                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                                    ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : inventoryAsset.Inventory.Inventory.Start
                                });

                            }
                        }

                    }
                }



            }


            if (assetNiQuery != null)
            {
                assetNiQuery = assetNiQuery.OrderBy(a => a.Code1);
                listAssetNi = assetNiQuery.ToList();
            }
            else
            {
                listAssetNi = new List<Dto.AssetNiInvDet>();
            }

            invResult.Details = new List<InventoryResultDetail>();

            if (reportType == "GROUPEMP")
            {

                foreach (var inventoryAsset in queryEmpList)
                {
                    InventoryResultDetail inventoryResultDetail = null;
                    inventoryResultDetail = new InventoryResultDetail()
                    {
                        InvNo = inventoryAsset.Asset.InvNo,
                        Description = inventoryAsset.Asset.Name,
                        SerialNumber = inventoryAsset.Inventory.SerialNumber,
                        PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                        Initial = 0,
                        Actual = 0,

                        Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                        Value = inventoryAsset.Dep.CurrentAPC,
                        ValueInv = inventoryAsset.Dep.CurrentAPC,
                        ValueDep = inventoryAsset.Dep.AccumulDep,
                        ValueDepTotal = inventoryAsset.Dep.AccumulDep,
                        Info = string.Empty,
                        Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                        ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : inventoryAsset.Inventory.Inventory.Start

                    };


                    switch (reportType.ToUpper())
                    {
                        case "MINUS":
                            inventoryResultDetail.Initial = inventoryAsset.Inventory.QInitial;
                            inventoryResultDetail.Actual = 0;
                            break;
                        case "PLUS":
                            inventoryResultDetail.Initial = 0;
                            inventoryResultDetail.Actual = inventoryAsset.Inventory.QFinal;
                            break;
                        case "MINUS/PLUS":

                            if (employeeId.HasValue)
                            {
                                inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId && inventoryAsset.Inventory.EmployeeIdInitial == employeeId ? inventoryAsset.Inventory.QInitial : 0;
                                inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId) && inventoryAsset.Inventory.EmployeeIdFinal == employeeId) ? inventoryAsset.Inventory.QFinal : 0;

                            }

                            break;

                        case "GROUPEMP":
                            if (locationId.HasValue)
                            {
                                inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId ? ((inventoryAsset.Inventory.EmployeeIdFinal != null) && (inventoryAsset.Inventory.EmployeeIdInitial != inventoryAsset.Inventory.EmployeeIdFinal)) ? 0 : inventoryAsset.Inventory.QInitial : 0;
                                inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId)) ? inventoryAsset.Inventory.QFinal : 0;
                                inventoryResultDetail.Plus = ((inventoryAsset.Inventory.RoomInitial.LocationId != locationId) || (inventoryAsset.Inventory.EmployeeIdInitial != inventoryAsset.Inventory.EmployeeIdFinal)) ? 1 : 0;
                            }
                            else
                            {
                                inventoryResultDetail.Initial = inventoryAsset.Inventory.QInitial;
                                inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == inventoryAsset.Inventory.RoomInitial.LocationId)) ? inventoryAsset.Inventory.QFinal : 0;
                            }
                            break;

                        default:

                            if (locationId.HasValue)
                            {
                                inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId ? inventoryAsset.Inventory.QInitial : 0;
                                inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId)) ? (inventoryAsset.Inventory.EmployeeIdInitial != inventoryAsset.Inventory.EmployeeIdFinal) ? 0 : inventoryAsset.Inventory.QFinal : 0;

                                if (employeeId.HasValue)
                                {
                                    inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId && inventoryAsset.Inventory.EmployeeIdInitial == employeeId ? inventoryAsset.Inventory.QInitial : 0;
                                    inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId) && inventoryAsset.Inventory.EmployeeIdFinal == employeeId) ? inventoryAsset.Inventory.QFinal : 0;
                                }
                            }
                            else
                            {
                                inventoryResultDetail.Initial = inventoryAsset.Inventory.CostCenterInitial.AdmCenterId == admCenterId ? inventoryAsset.Inventory.QInitial : 0;
                                inventoryResultDetail.Actual = ((inventoryAsset.Inventory.CostCenterIdFinal != null) && (inventoryAsset.Inventory.CostCenterFinal.AdmCenterId == admCenterId)) ? ((inventoryAsset.Inventory.RoomInitial.LocationId != inventoryAsset.Inventory.RoomFinal.LocationId) || (inventoryAsset.Inventory.EmployeeIdInitial != inventoryAsset.Inventory.EmployeeIdFinal)) ? 0 : inventoryAsset.Inventory.QFinal : 0;
                            }

                            break;

                    }

                    //if (reportType.ToUpper() == "MINUS")
                    if (((inventoryResultDetail.Initial > 0) && (inventoryResultDetail.Actual == 0))) //minus
                    {
                        inventoryResultDetail.CostCenter = inventoryAsset.Inventory.CostCenterIdInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty;
                        inventoryResultDetail.Building = inventoryAsset.Inventory.RoomInitial.Location.Name;
                        inventoryResultDetail.Room = inventoryAsset.Inventory.RoomInitial.Name;
                        inventoryResultDetail.InternalCode = inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.InternalCode;
                        inventoryResultDetail.FullName = inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName;
                    }
                    else
                    {
                        inventoryResultDetail.CostCenter =
                            (inventoryAsset.Inventory.CostCenterIdFinal == null ? (inventoryAsset.Inventory.CostCenterIdInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty) : inventoryAsset.Inventory.CostCenterFinal.Code);
                        inventoryResultDetail.Building = inventoryAsset.Inventory.RoomIdFinal != null ? inventoryAsset.Inventory.RoomFinal.Location.Name : inventoryAsset.Inventory.RoomInitial.Location.Name;
                        inventoryResultDetail.Room = inventoryAsset.Inventory.RoomIdFinal != null ? inventoryAsset.Inventory.RoomFinal.Name : inventoryAsset.Inventory.RoomInitial.Name;

                        inventoryResultDetail.InternalCode = inventoryAsset.Inventory.EmployeeFinal != null
                            ? (inventoryAsset.Inventory.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.InternalCode)
                            : (inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.InternalCode);

                        inventoryResultDetail.FullName = inventoryAsset.Inventory.EmployeeFinal != null
                            ? (inventoryAsset.Inventory.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.FirstName + " " + inventoryAsset.Inventory.EmployeeFinal.LastName)
                            : (inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName);
                    }



                    bool skip = false;

                    if (invResult.InventoryListType == "Plusuri")
                    {
                        inventoryResultDetail.Actual = 1;
                        skip = false;
                    }
                    else
                    {
                        // if ((inventoryResultDetail.Initial == 0 && inventoryResultDetail.Actual == 0) ||
                        //((employeeId.HasValue) && (inventoryAsset.Inventory.RoomIdFinal != null)
                        //&& (inventoryAsset.Inventory.RoomFinal.LocationId == inventoryAsset.Inventory.RoomInitial.LocationId)
                        //&& (inventoryAsset.Inventory.EmployeeIdInitial != inventoryAsset.Inventory.EmployeeIdFinal)
                        //&& (inventoryAsset.Inventory.EmployeeIdInitial == employeeId.Value))) skip = true;
                    }



                    if (!skip) invResult.Details.Add(inventoryResultDetail);
                }
            }
            else
            {
                foreach (var inventoryAsset in queryList)
                {
                    InventoryResultDetail inventoryResultDetail = null;
                    inventoryResultDetail = new InventoryResultDetail()
                    {
                        InvNo = inventoryAsset.Asset.InvNo,
                        Description = inventoryAsset.Asset.Name,
                        SerialNumber = inventoryAsset.Inventory.SerialNumber,
                        PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                        Initial = 0,
                        Actual = 0,

                        Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                        Value = inventoryAsset.Dep.CurrentAPC,
                        ValueInv = inventoryAsset.Dep.CurrentAPC,
                        ValueDep = inventoryAsset.Dep.AccumulDep,
                        ValueDepTotal = inventoryAsset.Dep.AccumulDep,
                        Info = string.Empty,
                        Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                        ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : inventoryAsset.Inventory.Inventory.Start

                    };


                    switch (reportType.ToUpper())
                    {
                        case "MINUS":
                            inventoryResultDetail.Initial = inventoryAsset.Inventory.QInitial;
                            inventoryResultDetail.Actual = 0;
                            break;
                        case "PLUS":
                            inventoryResultDetail.Initial = 0;
                            inventoryResultDetail.Actual = inventoryAsset.Inventory.QFinal;
                            break;
                        case "MINUS/PLUS":

                            if (employeeId.HasValue)
                            {
                                inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId && inventoryAsset.Inventory.EmployeeIdInitial == employeeId ? inventoryAsset.Inventory.QInitial : 0;
                                inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId) && inventoryAsset.Inventory.EmployeeIdFinal == employeeId) ? inventoryAsset.Inventory.QFinal : 0;

                            }

                            break;

                        case "GROUPEMP":
                            if (locationId.HasValue)
                            {
                                inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId ? ((inventoryAsset.Inventory.EmployeeIdFinal != null) && (inventoryAsset.Inventory.EmployeeIdInitial != inventoryAsset.Inventory.EmployeeIdFinal)) ? 0 : inventoryAsset.Inventory.QInitial : 0;
                                inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId)) ? inventoryAsset.Inventory.QFinal : 0;
                                inventoryResultDetail.Plus = ((inventoryAsset.Inventory.RoomInitial.LocationId != locationId) || (inventoryAsset.Inventory.EmployeeIdInitial != inventoryAsset.Inventory.EmployeeIdFinal)) ? 1 : 0;
                            }
                            else
                            {
                                inventoryResultDetail.Initial = inventoryAsset.Inventory.QInitial;
                                inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == inventoryAsset.Inventory.RoomInitial.LocationId)) ? inventoryAsset.Inventory.QFinal : 0;
                            }
                            break;

                        default:

                            if (locationId.HasValue)
                            {
                                inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId ? inventoryAsset.Inventory.QInitial : 0;
                                //inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId)) ? (inventoryAsset.Inventory.EmployeeIdInitial != inventoryAsset.Inventory.EmployeeIdFinal) ? 0 : inventoryAsset.Inventory.QFinal : 0;
                                inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId)) ? inventoryAsset.Inventory.QFinal : 0;

                                if (employeeId.HasValue)
                                {
                                    inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId && inventoryAsset.Inventory.EmployeeIdInitial == employeeId ? inventoryAsset.Inventory.QInitial : 0;
                                    inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId) && inventoryAsset.Inventory.EmployeeIdFinal == employeeId) ? inventoryAsset.Inventory.QFinal : 0;
                                }
                            }
                            else
                            {
                                inventoryResultDetail.Initial = inventoryAsset.Inventory.CostCenterInitial.AdmCenterId == admCenterId ? inventoryAsset.Inventory.QInitial : 0;
                                inventoryResultDetail.Actual = ((inventoryAsset.Inventory.CostCenterIdFinal != null) && (inventoryAsset.Inventory.CostCenterFinal.AdmCenterId == admCenterId)) ? ((inventoryAsset.Inventory.RoomInitial.LocationId != inventoryAsset.Inventory.RoomFinal.LocationId) || (inventoryAsset.Inventory.EmployeeIdInitial != inventoryAsset.Inventory.EmployeeIdFinal)) ? 0 : inventoryAsset.Inventory.QFinal : 0;
                            }

                            break;

                    }

                    //if (reportType.ToUpper() == "MINUS")
                    if (((inventoryResultDetail.Initial > 0) && (inventoryResultDetail.Actual == 0))) //minus
                    {
                        //inventoryResultDetail.CostCenter = inventoryAsset.Inventory.CostCenterIdInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty;
                        //inventoryResultDetail.Building = inventoryAsset.Inventory.RoomInitial.Location.Name;
                        //inventoryResultDetail.Room = inventoryAsset.Inventory.RoomInitial.Name;
                        //inventoryResultDetail.InternalCode = inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.InternalCode;
                        //inventoryResultDetail.FullName = inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName;
                    }
                    else
                    {
                        //inventoryResultDetail.CostCenter =
                        //    (inventoryAsset.Inventory.CostCenterIdFinal == null ? (inventoryAsset.Inventory.CostCenterIdInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty) : inventoryAsset.Inventory.CostCenterFinal.Code);
                        //inventoryResultDetail.Building = inventoryAsset.Inventory.RoomIdFinal != null ? inventoryAsset.Inventory.RoomFinal.Location.Name : inventoryAsset.Inventory.RoomInitial.Location.Name;
                        //inventoryResultDetail.Room = inventoryAsset.Inventory.RoomIdFinal != null ? inventoryAsset.Inventory.RoomFinal.Name : inventoryAsset.Inventory.RoomInitial.Name;

                        //inventoryResultDetail.InternalCode = inventoryAsset.Inventory.EmployeeFinal != null
                        //    ? (inventoryAsset.Inventory.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.InternalCode)
                        //    : (inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.InternalCode);

                        //inventoryResultDetail.FullName = inventoryAsset.Inventory.EmployeeFinal != null
                        //    ? (inventoryAsset.Inventory.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.FirstName + " " + inventoryAsset.Inventory.EmployeeFinal.LastName)
                        //    : (inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName);
                    }



                    bool skip = false;

                    if (invResult.InventoryListType == "Plusuri")
                    {
                        inventoryResultDetail.Actual = 1;
                        skip = false;
                    }
                    else
                    {
                        // if ((inventoryResultDetail.Initial == 0 && inventoryResultDetail.Actual == 0) ||
                        //((employeeId.HasValue) && (inventoryAsset.Inventory.RoomIdFinal != null)
                        //&& (inventoryAsset.Inventory.RoomFinal.LocationId == inventoryAsset.Inventory.RoomInitial.LocationId)
                        //&& (inventoryAsset.Inventory.EmployeeIdInitial != inventoryAsset.Inventory.EmployeeIdFinal)
                        //&& (inventoryAsset.Inventory.EmployeeIdInitial == employeeId.Value))) skip = true;
                    }



                    if (!skip) invResult.Details.Add(inventoryResultDetail);
                }
            }



            foreach (var assetNi in listAssetNi)
            {
                invResult.Details.Add(new InventoryResultDetail()
                {
                    InvNo = assetNi.Code1,
                    Description = assetNi.Name1,
                    SerialNumber = assetNi.SerialNumber,
                    PurchaseDate = null,
                    CostCenter = assetNi.CostCenterCode,
                    Room = assetNi.RoomName,
                    Building = assetNi.LocationName,
                    Initial = 0,
                    Actual = assetNi.Quantity,
                    Uom = "BUC",
                    Value = 0,
                    ValueInv = 0,
                    ValueDep = 0,
                    ValueDepTotal = 0,
                    Info = assetNi.Info,
                    Custody = assetNi.Custody.HasValue ? (assetNi.Custody.Value ? "DA" : "NU") : string.Empty,
                    InternalCode = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.InternalCode,
                    FullName = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.FirstName + " " + assetNi.LastName,
                    ModifiedAt = DateTime.Now
                });
            }

            //  inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);
            inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventory.Id); // CEC

            var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            invResult.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            invResult.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;
            invResult.RegionName = (region != null) ? region.Name : string.Empty;

            //if (admCenter != null)
            //{
            //    if (admCenter.Committee != null)
            //    {
            //        if (admCenter.Committee.Length > 0)
            //        {
            //            string[] committees = admCenter.Committee.Split(';');

            //            invResult.Committee1 = committees[0];
            //            invResult.Committee2 = committees[1];
            //            invResult.Committee3 = committees[2];
            //        }
            //    }


            //}




            //invResult.AdmCenterName = (region != null) ? region.Name : string.Empty;

            invResult.InventoryName = inventory.Description.Trim();
            // var maxDate = invResult.Details.Count > 0 ?  invResult.Details.Where(s=> s.ModifiedAt == invResult.Details.Where(p => p.ModifiedAt != null).Max(p => p.ModifiedAt)).FirstOrDefault() : null;
            var maxDate = invResult.Details.Count > 0 ? invResult.Details.Where(p => p.ModifiedAt != null).Max(p => p.ModifiedAt) : DateTime.Now;
            //invResult.InventoryEndDate = maxDate != null ? (maxDate.ModifiedAt != null ? maxDate.ModifiedAt.Value : DateTime.Now) : DateTime.Now;
            invResult.InventoryEndDate = maxDate.Value;
            invResult.LocationName = (location != null) ? location.Name : string.Empty;

            if (reportType.ToUpper() == "MINUS/PLUS")
            {
                foreach (var item in invResult.Details.ToList())
                {
                    if (item.Initial == item.Actual)
                    {

                        invResult.Details.Remove(item);
                    }
                }
            }

            if (reportType.ToUpper() == "GROUPEMP")
            {
                foreach (var item in invResult.Details.ToList())
                {
                    if ((item.Initial == item.Actual) && item.Plus == 0)
                    {

                        invResult.Details.Remove(item);
                    }


                }
            }



            return invResult;
        }

        public InventoryResult GetInventoryListV2MultipleByFilters(int? inventoryId, int? admCenterId, int? regionId, int? locationId, string reportType)
        {
            InventoryResult invResult = new InventoryResult();
            Model.Inventory inventory = null;
            Model.AdmCenter admCenter = null;
            Model.Location location = null;
            Model.Region region = null;
            Model.Employee employee = null;
            Model.AccMonth accMonth = null;

            IQueryable<Model.Asset> assetQuery = null;
            IQueryable<AssetDepMD> depQuery = null;
            IQueryable<Model.AssetInventoryDetail> query = null;

            IQueryable<Model.InventoryAsset> invAssetQuery = null;
            IQueryable<Dto.AssetNiInvDet> assetNiQuery = null;

            // List<Model.InventoryAsset> listInventoryAssets = null;
            List<Dto.AssetNiInvDet> listAssetNi = null;

            if ((inventoryId.HasValue) && (inventoryId > 0))
                inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);
            else
                inventory = _context.Set<Model.Inventory>().OrderByDescending(i => i.Id).FirstOrDefault(i => i.Active == true);

            if (inventory == null) return null;

            admCenter = (admCenterId.HasValue && admCenterId > 0) ? _context.Set<Model.AdmCenter>().Single(a => a.Id == admCenterId) : null;
            region = (regionId.HasValue && regionId > 0) ? _context.Set<Model.Region>().Single(a => a.Id == regionId) : null;
            location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;

            //int accMonthId = 17;
            int accSystemId = 1;

            if (inventoryId > 0)
            {
                inventory = _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId).SingleOrDefault();

                if (inventory != null)
                {
                    accMonth = _context.Set<Model.AccMonth>().Where(a => a.Id == inventory.AccMonthId).SingleOrDefault();
                }
            }

            //switch (inventoryId)
            //{
            //    case 2:
            //        accMonthId = 17;
            //        break;
            //    case 4:
            //        accMonthId = 24;
            //        break;
            //    case 5:
            //        accMonthId = 20;
            //        break;
            //    case 6:
            //        accMonthId = 26;
            //        break;
            //    case 7:
            //        accMonthId = 28;
            //        break;
            //    case 8:
            //        accMonthId = 30;
            //        break;
            //    case 9:
            //        accMonthId = 30;
            //        break;
            //    default:
            //        break;
            //}

            //accMonth = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).SingleOrDefault();

            // assetFilter.InventoryId = 6;
            assetQuery = _context.Assets
                 .Include(u => u.Uom)
                .Include(r => r.Room)
                    .ThenInclude(l => l.Location)
                        .ThenInclude(l => l.Region)
                .Include(c => c.CostCenter)
                    //.ThenInclude(a => a.AdmCenter)
                .Include(e => e.Employee)
                .Include(a => a.AssetState)
                .AsQueryable();
            depQuery = _context.AssetDepMDs.AsQueryable().Where(a => a.AccSystemId == accSystemId && a.AccMonthId == accMonth.Id);

            invAssetQuery = _context.InventoryAssets
                .Include(r => r.RoomInitial)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterInitial)
                    //.ThenInclude(a => a.AdmCenter)
                .Include(e => e.EmployeeInitial)
                .Include(s => s.StateInitial)
                .Include(r => r.RoomFinal)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterFinal)
                    //.ThenInclude(r => r.AdmCenter)
                 .Include(e => e.EmployeeFinal)
                  .Include(s => s.StateFinal)

                //.Where(i => i.InventoryId == inventoryId && ((i.StateIdFinal == null) || ((i.StateIdFinal != 4) && (i.StateIdFinal != 5)))).AsQueryable();
                .Where(i => i.InventoryId == inventoryId).AsQueryable();
            // _assetsRepository.GetInventoryAssetsQuery2(inventory.Id, null);
            assetNiQuery = _assetNiRepository.GetAssetNiInvDetQuery(inventory.Id);
            //  invAssetQuery = invAssetQuery.Where(inv => ((inv.StateIdFinal == null) || ((inv.StateIdFinal != 4) && (inv.StateIdFinal != 5))));
            //invAssetQuery = invAssetQuery.Where(inv => inv.StateIdFinal != 4).Where(inv => inv.StateIdFinal != 5);
            //invAssetQuery = invAssetQuery.Where(inv => inv.StateIdFinal != 5).Where(inv => inv.StateIdFinal != 4);
            //invAssetQuery = invAssetQuery.Where(i => i.RegionIdIni == regionId || i.RegionIdFin == regionId);
            //assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);

            //if (costCenterId.HasValue && (costCenterId > 0))
            //{
            //    invAssetQuery = invAssetQuery.Where(i => ((i.CostCenterIdInitial == costCenterId)
            //        || (i.CostCenterIdFinal == costCenterId)));
            //    assetNiQuery = assetNiQuery.Where(a => a.CostCenterId == costCenterId);
            //}
            //else
            //{
            //    if (admCenterId.HasValue && (admCenterId > 0))
            //    {
            //        //invAssetQuery = invAssetQuery.Where(i => ((i.CostCenterInitial != null) && (i.CostCenterInitial.AdmCenterId == admCenterId))
            //        //    || ((i.CostCenterFinal != null) && (i.CostCenterFinal.AdmCenterId == admCenterId)));

            //        invAssetQuery = invAssetQuery.Where(i => ((i.CostCenterInitial != null) && (i.CostCenterInitial.AdmCenterId == admCenterId))
            //            || ((i.CostCenterFinal != null) && (i.CostCenterFinal.AdmCenterId == admCenterId)));

            //        //assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
            //    }
            //}

            //if (employee != null)
            //{
            //    employeeId = employee.Id;
            //    if (locationId.HasValue || regionId.HasValue)
            //    {
            //        if (locationId.HasValue)
            //        {
            //            invAssetQuery = invAssetQuery.Where(i => (i.EmployeeIdInitial == employeeId && i.RoomInitial.LocationId == locationId)
            //            || ((i.EmployeeIdFinal == employeeId) && (i.RoomIdFinal != null) && (i.RoomFinal.LocationId == locationId)));
            //            assetNiQuery = assetNiQuery.Where(a => (a.EmployeeId == employeeId && a.LocationId == locationId));
            //        }
            //    }
            //    else
            //    {
            //        invAssetQuery = invAssetQuery.Where(e => e.EmployeeIdInitial == employeeId || e.EmployeeIdFinal == employeeId);
            //        assetNiQuery = assetNiQuery.Where(a => a.EmployeeId == employeeId);
            //    }

            //    invResult.EmployeeInternalCode = employee.InternalCode;
            //    invResult.EmployeeFirstName = employee.FirstName;
            //    invResult.EmployeeLastName = employee.LastName;
            //}
            //else
            //{
            //    if (locationId.HasValue && (locationId > 0))
            //    {
            //        invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.LocationId == locationId
            //            || ((i.RoomIdFinal != null) && (i.RoomFinal.LocationId == locationId)));
            //        assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
            //    }
            //    else
            //    {
            //        if (regionId.HasValue && (regionId > 0))
            //        {
            //            invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.Location.RegionId == regionId
            //                || ((i.RoomIdFinal != null) && (i.RoomFinal.Location.RegionId == regionId)));
            //            assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
            //        }
            //    }
            //}

            //if (custody.HasValue)
            //{
            //    invAssetQuery = invAssetQuery.Where(i => i.Asset.Custody == custody.GetValueOrDefault());
            //    assetNiQuery = assetNiQuery.Where(a => a.Custody == custody.GetValueOrDefault());
            //}

            query = assetQuery.Select(asset => new Model.AssetInventoryDetail { Asset = asset });


            query = query
                .Join(invAssetQuery, q => q.Asset.Id, inv => inv.AssetId, (q, inv) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = inv });



            query = query
                .Join(depQuery, q => q.Asset.Id, dep => dep.AssetId, (q, dep) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = q.Inventory, Dep = dep });


            if (reportType != null)
            {
                switch (reportType.ToUpper())
                {
                    case "MINUS":

                        //bool filters = false;
                        ////if ((locationId.HasValue) || (regionId.HasValue))
                        ////{
                        ////    invAssetQuery = invAssetQuery.Where(i => (((i.RoomIdFinal != null) && (i.RoomInitial.LocationId != i.RoomFinal.LocationId)) || (i.RoomIdFinal == null)));
                        ////}
                        ////else
                        ////{
                        ////    invAssetQuery = invAssetQuery.Where(i => (((i.CostCenterIdFinal != null) && (i.CostCenterInitial.AdmCenterId != i.CostCenterFinal.AdmCenterId)) || (i.CostCenterIdFinal == null)));
                        ////}

                        //if (locationId.HasValue)
                        //{
                        //    //invAssetQuery = invAssetQuery.Where(i => (i.RoomInitial.LocationId == locationId));
                        //    // query = query.Where(i => (((i.Inventory.CostCenterIdFinal == null) && (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId && i.Inventory.StateIdInitial == null))));
                        //    query = query.Where(i => (((i.Inventory.CostCenterIdFinal == null || (i.Inventory.CostCenterIdFinal != null
                        //&& (i.Inventory.CostCenterInitial.AdmCenterId != i.Inventory.CostCenterFinal.AdmCenterId) || (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)))
                        //&& (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId && i.Inventory.RoomInitial.LocationId == locationId && i.Inventory.StateIdInitial == null))));
                        //    filters = true;
                        //}
                        ////else
                        ////{
                        ////    if (regionId.HasValue)
                        ////    {
                        ////        invAssetQuery = invAssetQuery.Where(i => (i.RoomInitial.Location.RegionId == regionId));
                        ////    }
                        ////}

                        //if (admCenterId.HasValue)
                        //{
                        //    // query = query.Where(i => (i.Inventory.CostCenterIdFinal == null && i.Inventory.CostCenterInitial.AdmCenterId == admCenterId && i.Inventory.StateIdInitial == null));
                        //    //query = query.Where(i => (((i.Inventory.CostCenterIdFinal == null || (i.Inventory.CostCenterIdFinal != null 
                        //    //&& (i.Inventory.CostCenterInitial.AdmCenterId != i.Inventory.CostCenterFinal.AdmCenterId) || (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId))) 
                        //    //&& (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId && i.Inventory.StateIdInitial == null))));

                        //    query = query.Where(i => (((i.Inventory.CostCenterIdFinal == null || (i.Inventory.CostCenterIdFinal != null
                        // && (i.Inventory.CostCenterInitial.AdmCenterId != i.Inventory.CostCenterFinal.AdmCenterId))) && (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId && i.Inventory.StateIdInitial == null))));

                        //    //  query = query.Where(i => (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId) && (((i.Inventory.CostCenterIdFinal != null)

                        //    //&& (i.Inventory.CostCenterFinal.AdmCenterId != admCenterId)) || (i.Inventory.CostCenterIdFinal == null)));
                        //    //query = query.Where(i => (i.Inventory.CostCenterIdInitial != null) && (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId)
                        //    //    && (((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterFinal.AdmCenterId != admCenterId)) || (i.Inventory.CostCenterIdFinal == null)));

                        //    //      query = query.Where(i => ((i.Inventory.CostCenterIdInitial != null ? (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId) : false))
                        //    //|| ((i.Inventory.CostCenterIdFinal != null ? (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId) : false)));

                        //    filters = true;
                        //}

                        //if (!filters)
                        //{
                        //    query = query.Where(i => i.Inventory.RoomIdFinal == null);
                        //}

                        if ((locationId.HasValue) || (locationId != null))
                        {
                            //query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && ((i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId || (i.Inventory.RoomIdInitial  == i.Inventory.RoomIdFinal && i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))) ));
                            query = query.Where(i => ((i.Inventory.RoomInitial.LocationId == locationId) && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)));

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
                            }
                        }
                        else
                        {
                            //if (admCenterId.HasValue)
                            //{
                            //    query = query.Where(i => ((i.Inventory.CostCenterInitial.AdmCenterId == admCenterId) && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)));

                            //    if (assetNiQuery != null)
                            //    {
                            //        assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
                            //    }
                            //}

                            if (regionId.HasValue)
                            {
                                query = query.Where(i => ((i.Inventory.RoomInitial.Location.RegionId == regionId) && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)));

                                if (assetNiQuery != null)
                                {
                                    assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
                                }
                            }
                        }

                        assetNiQuery = null;
                        invResult.InventoryListType = "Minusuri";
                        break;

                    case "PLUS":
                        //if (locationId.HasValue)
                        //{

                        //    //if (locationId.HasValue)
                        //    //{
                        //    //    query = query.Where(i => ((i.Inventory.RoomIdFinal != null)

                        //    //    && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)
                        //    //    && (i.Inventory.RoomFinal.LocationId == locationId)));
                        //    //}
                        //    //else
                        //    //{
                        //    //    if (admCenterId.HasValue)
                        //    //    {
                        //    //        // query = query.Where(i => ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterInitial.AdmCenterId != i.Inventory.CostCenterFinal.AdmCenterId)));
                        //    //        //  query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)  && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)));

                        //    //        query = query.Where(i => ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.RoomIdFinal != null && i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)));

                        //    //        //                query = query.Where(i => ((i.Inventory.CostCenterIdInitial != null ? (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId) : false))
                        //    //        //|| ((i.Inventory.CostCenterIdFinal != null ? (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId) : false)));
                        //    //        //  query = query.Where(i =>i.Inventory.CostCenterInitial.AdmCenterId == admCenterId || ( i.Inventory.CostCenterIdFinal != null ? i.Inventory.CostCenterFinal.AdmCenterId == admCenterId : i.Inventory.CostCenterInitial.AdmCenterId == admCenterId));
                        //    //    }
                        //    //}


                        //    //if (locationId.HasValue)
                        //    //{
                        //    //    query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomFinal.LocationId == locationId)));
                        //    //}
                        //    //else
                        //    //{
                        //    //    if (regionId.HasValue)
                        //    //    {
                        //    //        query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomFinal.Location.RegionId == regionId)));
                        //    //    }
                        //    //}

                        //    //if (admCenter != null)
                        //    //{
                        //    //    query = query.Where(i => ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterInitial.AdmCenterId != i.Inventory.CostCenterFinal.AdmCenterId)));
                        //    //    //query = query.Where(i => ((i.Inventory.CostCenterIdInitial != null ? (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId) : false))
                        //    //    //         || ((i.Inventory.CostCenterIdFinal != null ? (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId) : false)));

                        //    //    // query = query.Where(i => i.Inventory.CostCenterInitial.AdmCenterId == admCenterId || (i.Inventory.CostCenterIdFinal != null ? i.Inventory.CostCenterFinal.AdmCenterId == admCenterId : i.Inventory.CostCenterInitial.AdmCenterId == admCenterId));
                        //    //}

                        //    if ((locationId.HasValue) || (admCenterId.HasValue))
                        //    {
                        //        query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && ((i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId || (i.Inventory.RoomIdInitial == i.Inventory.RoomIdFinal && i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal)))));
                        //    }
                        //    else
                        //    {
                        //        if (admCenterId.HasValue)
                        //        {
                        //            query = query.Where(i => ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterInitial.AdmCenterId != i.Inventory.CostCenterFinal.AdmCenterId)));
                        //        }
                        //    }


                        //    if (locationId.HasValue)
                        //    {
                        //        query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomFinal.LocationId == locationId) && (i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal)));
                        //    }
                        //    else
                        //    {
                        //        if (admCenterId.HasValue)
                        //        {
                        //            query = query.Where(i => ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)));
                        //        }
                        //    }

                        //    if (admCenter != null)
                        //    {
                        //        //query = query.Where(i => ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)));
                        //        query = query.Where(i => ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterInitial.AdmCenterId != i.Inventory.CostCenterFinal.AdmCenterId) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)));
                        //    }
                        //}
                        //else
                        //{

                        //    if (admCenterId.HasValue)
                        //    {
                        //        query = query.Where(i => ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterInitial.AdmCenterId != i.Inventory.CostCenterFinal.AdmCenterId) && (i.Inventory.RoomIdFinal != null && i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)));

                        //    }

                        //    //if (admCenterId.HasValue)
                        //    //{
                        //    //    query = query.Where(i => ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId) && (i.Inventory.RoomIdFinal != null && i.Inventory.RoomIdInitial != i.Inventory.RoomIdFinal)));
                        //    //}


                        //    if (admCenter != null)
                        //    {
                        //        query = query.Where(i => ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId) && (i.Inventory.RoomIdFinal != null && i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)));
                        //    }
                        //}

                        if ((locationId.HasValue) || (locationId != null))
                        {
                            //query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && ((i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId || (i.Inventory.RoomIdInitial  == i.Inventory.RoomIdFinal && i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))) ));
                            query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomFinal.LocationId == locationId) && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)));

                            if (assetNiQuery != null)
                            {
                                assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
                            }
                        }
                        else
                        {
                            //if (admCenterId.HasValue)
                            //{
                            //    query = query.Where(i => ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)
                            //     && ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId))));

                            //    if (assetNiQuery != null)
                            //    {
                            //        assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
                            //    }
                            //}

                            if (regionId.HasValue)
                            {
                                query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomFinal.Location.RegionId == regionId)
                                 && ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId))));

                                if (assetNiQuery != null)
                                {
                                    assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
                                }
                            }
                        }

                        invResult.InventoryListType = "Plusuri";
                        break;

                    case "NOT_IDENTIFIED":
                        query = null;
                        invResult.InventoryListType = "Etichete temporare";
                        break;

                    case "PLUS/MINUS":
                        invResult.InventoryListType = "Plusuri/Minusuri";
                        break;

                    case "GROUPEMP":
                        invResult.InventoryListType = "Plusuri/Minusuri";
                        break;

                    default:
                        invResult.InventoryListType = string.Empty;
                        break;
                }
            }



            if (locationId.HasValue && (locationId > 0))
            {
                if (query != null)
                {
                    //query = query.Where(i => i.Inventory.RoomInitial.LocationId == locationId

                    //|| ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomIdFinal != null ? i.Inventory.RoomFinal.LocationId == locationId : i.Inventory.RoomInitial.LocationId == locationId)));


                    switch (inventoryId)
                    {
                        //case 2:
                        //    query = query.Where(i => ((i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal) && (i.Inventory.RoomInitial.LocationId == locationId || (i.Inventory.RoomIdFinal != null ? i.Inventory.RoomFinal.LocationId == locationId : i.Inventory.RoomInitial.LocationId == locationId))));
                        //    break;
                        //      case 4:
                        //          query = query.Where(i => (i.Inventory.EmployeeIdInitial == employeeId && (i.Inventory.RoomIdInitial != null ? i.Inventory.RoomInitial.LocationId == locationId : false))
                        //     || ((i.Inventory.EmployeeIdFinal == employeeId) && (i.Inventory.RoomIdFinal != null) && ((i.Inventory.RoomIdFinal != null ? i.Inventory.RoomFinal.LocationId == locationId : false))));
                        //          break;
                        //      case 5:
                        //          query = query.Where(i => (i.Inventory.EmployeeIdInitial == employeeId && (i.Inventory.RoomIdInitial != null ? i.Inventory.RoomInitial.LocationId == locationId : false))
                        //|| ((i.Inventory.EmployeeIdFinal == employeeId) && (i.Inventory.RoomIdFinal != null) && ((i.Inventory.RoomIdFinal != null ? i.Inventory.RoomFinal.LocationId == locationId : false))));
                        //          break;
                        //      case 6:
                        //          query = query.Where(i => (i.Inventory.EmployeeIdInitial == employeeId && (i.Inventory.RoomIdInitial != null ? i.Inventory.RoomInitial.LocationId == locationId : false))
                        //|| ((i.Inventory.EmployeeIdFinal == employeeId) && (i.Inventory.RoomIdFinal != null) && ((i.Inventory.RoomIdFinal != null ? i.Inventory.RoomFinal.LocationId == locationId : false))));
                        //          break;
                        //      default:
                        //          break;
                    }

                }

                if (assetNiQuery != null)
                {
                    assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
                }

            }
            else
            {
                //if (admCenterId.HasValue && (admCenterId > 0))
                //{
                //    //query = query.Where(i => i.Inventory.CostCenterInitial.AdmCenterId == admCenterId
                //    //    || ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)));
                //    query = query.Where(i => ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)));
                //    assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
                //}
            }


            //if (admCenterId.HasValue && (admCenterId > 0))
            //{
            //    //query = query.Where(i => i.Inventory.CostCenterInitial.AdmCenterId == admCenterId
            //    //       || ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)));
            //    //query = query.Where(i => ((i.Inventory.CostCenterInitial != null) && (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId))
            //    //    || ((i.Inventory.CostCenterIdFinal != null ? (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId) : false)));
            //    if (query != null)
            //    {
            //        //       query = query.Where(i => ((i.Inventory.CostCenterIdInitial != null ? (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId) : false))
            //        //|| ((i.Inventory.CostCenterIdFinal != null ? (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId) : false)));
            //    }

            //    if (assetNiQuery != null)
            //    {
            //        assetNiQuery = assetNiQuery.Where(a => a.AdmCenterId == admCenterId);
            //    }

            //}


            if (regionId.HasValue && (regionId > 0))
            {
                //query = query.Where(i => i.Inventory.CostCenterInitial.AdmCenterId == admCenterId
                //       || ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)));
                //query = query.Where(i => ((i.Inventory.CostCenterInitial != null) && (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId))
                //    || ((i.Inventory.CostCenterIdFinal != null ? (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId) : false)));
                if (query != null)
                {
                    //       query = query.Where(i => ((i.Inventory.CostCenterIdInitial != null ? (i.Inventory.CostCenterInitial.AdmCenterId == admCenterId) : false))
                    //|| ((i.Inventory.CostCenterIdFinal != null ? (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId) : false)));
                }

                if (assetNiQuery != null)
                {
                    assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);
                }

            }

            List<Model.AssetInventoryDetail> queryList = new List<Model.AssetInventoryDetail>();

            if (query != null)
            {
                query = query.OrderBy(i => i.Asset.InvNo);
                queryList = query.ToList();
            }



            //if (invAssetQuery != null)
            //{

            //  //  listInventoryAssets = invAssetQuery.ToList();
            //}
            //else
            //{
            //    listInventoryAssets = new List<InventoryAsset>();
            //}

            if (assetNiQuery != null)
            {
                assetNiQuery = assetNiQuery.OrderBy(a => a.Code1);
                listAssetNi = assetNiQuery.ToList();
            }
            else
            {
                listAssetNi = new List<Dto.AssetNiInvDet>();
            }

            //listInventoryAssets = (invAssetQuery != null) ? invAssetQuery.ToList() : new List<InventoryAsset>();
            //listAssetNi = (assetNiQuery != null) ? assetNiQuery.ToList() : new List<Dto.AssetNiInvDet>();

            invResult.Details = new List<InventoryResultDetail>();

            foreach (var inventoryAsset in queryList)
            {
                InventoryResultDetail inventoryResultDetail = null;
                inventoryResultDetail = new InventoryResultDetail()
                {
                    InvNo = inventoryAsset.Asset.InvNo,
                    Description = inventoryAsset.Asset.Name,
                    SerialNumber = inventoryAsset.Inventory.SerialNumber,
                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                    Initial = 0,
                    Actual = 0,

                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                    Value = inventoryAsset.Dep.CurrentAPC,
                    ValueInv = inventoryAsset.Dep.CurrentAPC,
                    ValueDep = inventoryAsset.Dep.AccumulDep,
                    ValueDepTotal = inventoryAsset.Dep.AccumulDep,
                    Info = string.Empty,
                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                    ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : inventoryAsset.Inventory.Inventory.Start

                };

                switch (reportType.ToUpper())
                {
                    case "MINUS":
                        inventoryResultDetail.Initial = inventoryAsset.Inventory.QInitial;
                        inventoryResultDetail.Actual = 0;
                        break;
                    case "PLUS":
                        inventoryResultDetail.Initial = 0;
                        inventoryResultDetail.Actual = inventoryAsset.Inventory.QFinal;
                        break;
                    case "MINUS/PLUS":
                        if (locationId.HasValue)
                        {
                            inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId ? inventoryAsset.Inventory.QInitial : 0;
                            inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId)) ? inventoryAsset.Inventory.QFinal : 0;
                        }
                        else
                        {
                            inventoryResultDetail.Initial = inventoryAsset.Inventory.QInitial;
                            inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == inventoryAsset.Inventory.RoomInitial.LocationId)) ? inventoryAsset.Inventory.QFinal : 0;
                        }
                        break;

                    case "GROUPEMP":
                        if (locationId.HasValue)
                        {
                            inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId ? inventoryAsset.Inventory.QInitial : 0;
                            inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId)) ? inventoryAsset.Inventory.QFinal : 0;
                        }
                        else
                        {
                            inventoryResultDetail.Initial = inventoryAsset.Inventory.QInitial;
                            inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == inventoryAsset.Inventory.RoomInitial.LocationId)) ? inventoryAsset.Inventory.QFinal : 0;
                        }
                        break;

                    default:

                        if (locationId.HasValue)
                        {
                            inventoryResultDetail.Initial = inventoryAsset.Inventory.RoomInitial.LocationId == locationId ? inventoryAsset.Inventory.QInitial : 0;
                            inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == locationId)) ? inventoryAsset.Inventory.QFinal : 0;
                        }
                        else
                        {
                            inventoryResultDetail.Initial = inventoryAsset.Inventory.QInitial;
                            inventoryResultDetail.Actual = ((inventoryAsset.Inventory.RoomIdFinal != null) && (inventoryAsset.Inventory.RoomFinal.LocationId == inventoryAsset.Inventory.RoomInitial.LocationId)) ? inventoryAsset.Inventory.QFinal : 0;
                        }
                        break;

                }

                //if (reportType.ToUpper() == "MINUS")
                if (((inventoryResultDetail.Initial > 0) && (inventoryResultDetail.Actual == 0))) //minus
                {
                    inventoryResultDetail.CostCenter = inventoryAsset.Inventory.CostCenterIdInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty;
                    inventoryResultDetail.Building = inventoryAsset.Inventory.RoomInitial.Location.Name;
                    inventoryResultDetail.Room = inventoryAsset.Inventory.RoomInitial.Name;
                    inventoryResultDetail.InternalCode = inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.InternalCode;
                    inventoryResultDetail.FullName = inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName;
                }
                else
                {
                    inventoryResultDetail.CostCenter =
                        (inventoryAsset.Inventory.CostCenterIdFinal == null ? (inventoryAsset.Inventory.CostCenterIdInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty) : inventoryAsset.Inventory.CostCenterFinal.Code);
                    inventoryResultDetail.Building = inventoryAsset.Inventory.RoomIdFinal != null ? inventoryAsset.Inventory.RoomFinal.Location.Name : inventoryAsset.Inventory.RoomInitial.Location.Name;
                    inventoryResultDetail.Room = inventoryAsset.Inventory.RoomIdFinal != null ? inventoryAsset.Inventory.RoomFinal.Name : inventoryAsset.Inventory.RoomInitial.Name;

                    inventoryResultDetail.InternalCode = inventoryAsset.Inventory.EmployeeFinal != null
                        ? (inventoryAsset.Inventory.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.InternalCode)
                        : (inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.InternalCode);

                    inventoryResultDetail.FullName = inventoryAsset.Inventory.EmployeeFinal != null
                        ? (inventoryAsset.Inventory.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.FirstName + " " + inventoryAsset.Inventory.EmployeeFinal.LastName)
                        : (inventoryAsset.Inventory.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName);
                }

                //if (employeeId.HasValue)
                //{
                //    inventoryResultDetail.Initial =
                //        ((inventoryAsset.EmployeeIdInitial == employeeId) || (inventoryAsset.RoomIdFinal != null && inventoryAsset.EmployeeIdFinal == employeeId && inventoryAsset.RoomInitial.LocationId == inventoryAsset.RoomFinal.LocationId))
                //            ? inventoryAsset.QInitial : 0;
                //    inventoryResultDetail.Actual = ((inventoryAsset.EmployeeIdFinal != null) && (inventoryAsset.EmployeeIdFinal == employeeId)) ? inventoryAsset.QFinal : 0;
                //}

                //if (employeeId.HasValue)
                //{
                //    inventoryResultDetail.Initial = 
                //        ((inventoryAsset.EmployeeIdInitial == employeeId) || (inventoryAsset.RoomIdFinal != null && inventoryAsset.EmployeeIdFinal == employeeId && inventoryAsset.RoomInitial.LocationId == inventoryAsset.RoomFinal.LocationId)) 
                //            ? inventoryAsset.QInitial : 0;
                //    inventoryResultDetail.Actual = ((inventoryAsset.EmployeeIdFinal != null) && (inventoryAsset.EmployeeIdFinal == employeeId)) ? inventoryAsset.QFinal : 0;
                //}
                //else
                //{
                //    if (locationId.HasValue)
                //    {
                //        inventoryResultDetail.Initial = inventoryAsset.RoomInitial.LocationId == locationId ? inventoryAsset.QInitial : 0;
                //        inventoryResultDetail.Actual = ((inventoryAsset.RoomIdFinal != null) && (inventoryAsset.RoomFinal.LocationId == locationId)) ? inventoryAsset.QFinal : 0;
                //    }
                //    else if (regionId.HasValue)
                //    {

                //    }
                //}

                bool skip = false;

                if (invResult.InventoryListType == "Plusuri")
                {
                    inventoryResultDetail.Actual = 1;
                    skip = false;
                }
                //else
                //{
                //    if ((inventoryResultDetail.Initial == 0 && inventoryResultDetail.Actual == 0) ||
                //  (inventoryAsset.Inventory.RoomIdFinal != null
                //  && (inventoryAsset.Inventory.RoomFinal.LocationId == inventoryAsset.Inventory.RoomInitial.LocationId)
                //  && (inventoryAsset.Inventory.EmployeeIdInitial != inventoryAsset.Inventory.EmployeeIdFinal))) skip = true;
                //}



                if (!skip) invResult.Details.Add(inventoryResultDetail);
            }

            foreach (var assetNi in listAssetNi)
            {
                invResult.Details.Add(new InventoryResultDetail()
                {
                    InvNo = assetNi.Code1,
                    Description = assetNi.Name1,
                    SerialNumber = assetNi.SerialNumber,
                    PurchaseDate = null,
                    CostCenter = assetNi.CostCenterCode,
                    Room = assetNi.RoomName,
                    Building = assetNi.LocationName,
                    Initial = 0,
                    Actual = assetNi.Quantity,
                    Uom = "BUC",
                    Value = 0,
                    ValueInv = 0,
                    ValueDep = 0,
                    ValueDepTotal = 0,
                    Info = assetNi.Info,
                    Custody = assetNi.Custody.HasValue ? (assetNi.Custody.Value ? "DA" : "NU") : string.Empty,
                    InternalCode = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.InternalCode,
                    FullName = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.FirstName + " " + assetNi.LastName,
                    //ModifiedAt = assetNi.ModifiedAt.Value
                });
            }

            //  inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);
            inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventory.Id); // CEC

            var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            invResult.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            invResult.AdmCenterName = (region != null) ? region.Name : string.Empty;

            //if (admCenter != null)
            //{
            //    if (admCenter.Committee != null)
            //    {
            //        if (admCenter.Committee.Length > 0)
            //        {
            //            string[] committees = admCenter.Committee.Split(';');

            //            invResult.Committee1 = committees[0];
            //            invResult.Committee2 = committees[1];
            //            invResult.Committee3 = committees[2];
            //        }
            //    }


            //}




            //invResult.AdmCenterName = (region != null) ? region.Name : string.Empty;

            invResult.InventoryName = inventory.Description.Trim();
            //var maxDate = invResult.Details.Count > 0 ?  invResult.Details.Where(s=> s.ModifiedAt == invResult.Details.Where(p => p.ModifiedAt != null).Max(p => p.ModifiedAt)).FirstOrDefault() : null;
            var maxDate = invResult.Details.Count > 0 ? invResult.Details.Where(p => p.ModifiedAt != null).Max(p => p.ModifiedAt) : DateTime.Now;
            //invResult.InventoryEndDate = maxDate != null ? (maxDate.ModifiedAt != null ? maxDate.ModifiedAt.Value : DateTime.Now) : DateTime.Now;
            invResult.InventoryEndDate = maxDate.Value;
            invResult.LocationName = (location != null) ? location.Name : string.Empty;

            if (reportType.ToUpper() == "MINUS/PLUS" || reportType.ToUpper() == "GROUPEMP")
            {
                foreach (var item in invResult.Details.ToList())
                {
                    if (item.Initial == item.Actual)
                    {

                        invResult.Details.Remove(item);
                    }
                }
            }


            return invResult;
        }

        public InventoryResult GetInventoryListWGByFilters(int inventoryId, int? locationId, string reportType)
        {
            InventoryResult invResult = new InventoryResult();
            Model.Location location = null;
            Model.Inventory inventory = null;
            IQueryable<Model.InventoryAsset> invAssetQuery = null;
            List<Model.InventoryAsset> listInventoryAssets = null;

            inventory = _context.Set<Model.Inventory>().Where(i => i.Id == inventoryId).SingleOrDefault();

			location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>()
	            .Include(a => a.City)
		            .ThenInclude(a => a.County)
			         .Single(l => l.Id == locationId) : null;

	        invAssetQuery = _assetsRepository.GetInventoryAssetReportsQuery2(inventoryId, null);

            if (locationId.HasValue && (locationId > 0))
            {
                invAssetQuery = invAssetQuery.Where(i => i.RoomFinal.LocationId == locationId || i.RoomInitial.LocationId == locationId);
            }

            if (invAssetQuery != null)
            {
                invAssetQuery = invAssetQuery.OrderBy(i => i.Asset.InvNo);
                listInventoryAssets = invAssetQuery.ToList();
            }
            else
            {
                listInventoryAssets = new List<Model.InventoryAsset>();
            }


            invResult.Details = new List<InventoryResultDetail>();

            foreach (var inventoryAsset in listInventoryAssets)
            {
                InventoryResultDetail inventoryResultDetail = null;
                inventoryResultDetail = new InventoryResultDetail()
                {
                    InvNo = inventoryAsset.Asset.InvNo,
                    Description = inventoryAsset.Asset.Name,
                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,
                    SerialNumber = inventoryAsset.RoomIdFinal != null ? inventoryAsset.SerialNumber : inventoryAsset.SNInitial,
                    Producer = inventoryAsset.EmployeeIdFinal != null ? inventoryAsset.EmployeeFinal.InternalCode : inventoryAsset.EmployeeIdInitial != null ? inventoryAsset.EmployeeInitial.InternalCode : "",
                    AssetCategory = inventoryAsset.Asset.CompanyId != null ? inventoryAsset.Asset.Company.Name : string.Empty,
                    State = inventoryAsset.StateIdFinal != null ? inventoryAsset.StateFinal.Name : inventoryAsset.StateInitial.Name,
                    AllowLabel = inventoryAsset.Asset.AllowLabel.Value,
                    Value = inventoryAsset.Asset.ValueInv,
                    Info = inventoryAsset.Info,
                    Initial = inventoryAsset.QInitial,
                    Actual = inventoryAsset.QFinal,
                };

                if (locationId.HasValue)
                {
                    inventoryResultDetail.Initial = inventoryAsset.RoomInitial.LocationId == locationId ? inventoryAsset.QInitial : 0;
                    inventoryResultDetail.Actual = ((inventoryAsset.RoomIdFinal != null) && (inventoryAsset.RoomFinal.LocationId == locationId)) ? inventoryAsset.QFinal : 0;
                }

                invResult.Details.Add(inventoryResultDetail);
            }


            var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            invResult.CompanyName = _context.Set<Model.LocationType>().Where(l => l.Id == location.LocationTypeId).Select(l => l.Code).FirstOrDefault();
            invResult.EndDate = DateTime.Now;
            invResult.LocationName = (location != null) ? location.Name : string.Empty;
            invResult.LocationCode = (location != null) ? location.Name : string.Empty;
            invResult.AdmCenterName = (location != null) ? location.City.Name : "";
            invResult.AdministrationName = (location != null) ? location.City.County.Name : "";
            invResult.AdministrationCode = (location != null) ? location.Name : "";


            return invResult;
        }

        public TransferInV1Result GetTransferInV1MultipleByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, string reportType, bool? custody)
        {
            string strReportType = string.Empty;
            TransferInV1Result transferInV1Result = null;
            Model.AdmCenter admCenter = null;
            Model.Location location = null;
            Model.Region region = null;
            Model.Inventory inventory = null;
            IQueryable<Model.InventoryAsset> invAssetQuery = null;
            //List<Model.InventoryAsset> listInventoryAssets = null;
            Model.AccMonth accMonth = null;
            List<Model.AssetInventoryDetail> listAssetInventoryDetails = null;
            IQueryable<Model.Asset> assetQuery = null;
            IQueryable<AssetDepMD> depQuery = null;

            IQueryable<Model.AssetInventoryDetail> query = null;

            //if (inventoryId > 0)
            //    inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);
            //else
            //    inventory = _context.Set<Model.Inventory>().OrderByDescending(i => i.Id).FirstOrDefault(i => i.Active == true);

            //if (inventory == null) return null;

            admCenter = (admCenterId.HasValue && admCenterId > 0) ? _context.Set<Model.AdmCenter>().Single(a => a.Id == admCenterId) : null;
            location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
            //region = region = _context.Set<Model.Region>().Single(r => r.Id == regionId);
            region = (regionId.HasValue && regionId > 0) ? _context.Set<Model.Region>().Single(r => r.Id == regionId) : null;

            //invAssetQuery = _assetsRepository.GetInventoryAssetsQuery2(inventoryId, null);

            //if (costCenterId.HasValue && (costCenterId > 0))
            //{
            //    invAssetQuery = invAssetQuery.Where(i => ((i.CostCenterIdInitial == costCenterId)
            //        || (i.CostCenterIdFinal == costCenterId)));
            //}
            //else
            //{
            //    if (admCenterId.HasValue && (admCenterId > 0))
            //    {
            //        invAssetQuery = invAssetQuery.Where(i => i.CostCenterInitial.AdmCenterId == admCenterId
            //            || ((i.CostCenterIdFinal != null) && (i.CostCenterFinal.AdmCenterId == admCenterId)));
            //    }
            //}

            //if (locationId.HasValue && (locationId > 0))
            //{
            //    invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.LocationId == locationId
            //        || ((i.RoomIdFinal != null) && (i.RoomFinal.LocationId == locationId)));
            //}
            //else
            //{
            //    if (regionId.HasValue && (regionId > 0))
            //    {
            //        invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.Location.RegionId == regionId
            //            || ((i.RoomIdFinal != null) && (i.RoomFinal.Location.RegionId == regionId)));
            //    }
            //}

            //if (custody.HasValue)
            //{
            //    invAssetQuery = invAssetQuery.Where(i => i.Asset.Custody == custody.GetValueOrDefault());
            //}


            // NEW // 

            int accSystemId = 1;

            if (inventoryId > 0)
            {
                inventory = _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId).SingleOrDefault();

                if (inventory != null)
                {
                    accMonth = _context.Set<Model.AccMonth>().Where(a => a.Id == inventory.AccMonthId).SingleOrDefault();
                }
            }

            assetQuery = _context.Assets
                 .Include(u => u.Uom)
                .Include(r => r.Room)
                    .ThenInclude(l => l.Location)
                        .ThenInclude(l => l.Region)
                .Include(c => c.CostCenter)
                    //.ThenInclude(a => a.AdmCenter)
                .Include(e => e.Employee)
                .Include(a => a.AssetState)
                .AsQueryable();
            depQuery = _context.AssetDepMDs.AsQueryable().Where(a => a.AccSystemId == accSystemId && a.AccMonthId == accMonth.Id);

            invAssetQuery = _context.InventoryAssets
                .Include(r => r.RoomInitial)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterInitial)
                    //.ThenInclude(a => a.AdmCenter)
                .Include(e => e.EmployeeInitial)
                .Include(s => s.StateInitial)
                .Include(r => r.RoomFinal)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterFinal)
                    //.ThenInclude(r => r.AdmCenter)
                 .Include(e => e.EmployeeFinal)
                  .Include(s => s.StateFinal)

                .Where(i => i.InventoryId == inventoryId).AsQueryable();


            query = assetQuery.Select(asset => new Model.AssetInventoryDetail { Asset = asset });


            query = query
                .Join(invAssetQuery, q => q.Asset.Id, inv => inv.AssetId, (q, inv) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = inv });



            query = query
                .Join(depQuery, q => q.Asset.Id, dep => dep.AssetId, (q, dep) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = q.Inventory, Dep = dep });


            if (costCenterId.HasValue && (costCenterId > 0))
            {
                query = query.Where(i => ((i.Inventory.CostCenterIdInitial == costCenterId)
                    || (i.Inventory.CostCenterIdFinal == costCenterId)));
            }
            else
            {
                if (admCenterId.HasValue && (admCenterId > 0))
                {
                    query = query.Where(i => i.Inventory.CostCenterInitial.AdmCenterId == admCenterId
                        || ((i.Inventory.CostCenterIdFinal != null) && (i.Inventory.CostCenterFinal.AdmCenterId == admCenterId)));
                }
            }

            if (locationId.HasValue && (locationId > 0))
            {
                query = query.Where(i => i.Inventory.RoomInitial.LocationId == locationId
                    || ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomFinal.LocationId == locationId)));
            }
            else
            {
                if (regionId.HasValue && (regionId > 0))
                {
                    query = query.Where(i => i.Inventory.RoomInitial.Location.RegionId == regionId
                        || ((i.Inventory.RoomIdFinal != null) && (i.Inventory.RoomFinal.Location.RegionId == regionId)));
                }
            }

            if (custody.HasValue)
            {
                query = query.Where(i => i.Asset.Custody == custody.GetValueOrDefault());
            }

            if (reportType != null)
            {
                switch (reportType.ToUpper())
                {
                    case "TRANSFER_SAME_LOCATION":
                        query = query
                            .Where(i => ((i.Inventory.RoomIdFinal != null)
                                && ((i.Inventory.RoomIdInitial != i.Inventory.RoomIdFinal) || (i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))
                                && (i.Inventory.RoomInitial.LocationId == i.Inventory.RoomFinal.LocationId)));
                        strReportType = "Transferuri in aceeasi localitate";
                        break;
                    case "TRANSFER_DIFF_LOCATION":
                        query = query
                            .Where(i => ((i.Inventory.RoomIdFinal != null)
                                && ((i.Inventory.RoomIdInitial != i.Inventory.RoomIdFinal) || (i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))
                                && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)));
                        strReportType = "intre adrese din aceeasi localitate";
                        break;
                    case "TRANSFER_SAME_REGION":
                        query = query
                            .Where(i => ((i.Inventory.RoomIdFinal != null)
                                && (i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId)
                                && ((i.Inventory.RoomIdInitial != i.Inventory.RoomIdFinal) || (i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))
                                && (i.Inventory.RoomInitial.Location.RegionId == i.Inventory.RoomFinal.Location.RegionId)));
                        strReportType = "in judete";
                        break;
                    case "TRANSFER_DIFF_REGION":
                        query = query
                            .Where(i => ((i.Inventory.RoomIdFinal != null)
                                && ((i.Inventory.RoomIdInitial != i.Inventory.RoomIdFinal) || (i.Inventory.EmployeeIdInitial != i.Inventory.EmployeeIdFinal))
                                && (i.Inventory.RoomInitial.Location.RegionId != i.Inventory.RoomFinal.Location.RegionId)));
                        strReportType = "intre judete";
                        break;
                    case "TRANSFER_SAME_ADMCENTER":
                        //invAssetQuery = invAssetQuery
                        //    .Where(i => ((i.CostCenterIdFinal != null)
                        //        //&& (i.CostCenterIdInitial != i.CostCenterIdFinal)
                        //        && (i.RoomInitial.LocationId != i.RoomFinal.LocationId)
                        //        && (i.CostCenterInitial.AdmCenterId == i.CostCenterFinal.AdmCenterId)));

                        query = query
                        .Where(i => ((i.Inventory.RoomIdFinal != null)
                            && ((i.Inventory.RoomInitial.LocationId != i.Inventory.RoomFinal.LocationId))
                            && (i.Inventory.RoomInitial.Location.RegionId == i.Inventory.RoomFinal.Location.RegionId)));

                        strReportType = "Transferuri in judet";
                        break;
                    case "TRANSFER_DIFF_ADMCENTER":
                        query = query
                            .Where(i => ((i.Inventory.RoomIdFinal != null)
                                //&& (i.CostCenterIdInitial != i.CostCenterIdFinal)
                                && (i.Inventory.RoomInitial.Location.RegionId != i.Inventory.RoomFinal.Location.RegionId)));
                        strReportType = "Transferuri intre judete";
                        break;
                    default:
                        break;
                }
            }




            listAssetInventoryDetails = query.ToList();

            var initial = listAssetInventoryDetails.Select(r => r.Inventory.RoomInitial.LocationId).Distinct().ToList();
            var final = listAssetInventoryDetails.Select(r => r.Inventory.RoomFinal.LocationId).Distinct().ToList();


            transferInV1Result = new TransferInV1Result();

            transferInV1Result.Details = new List<TransferInV1Detail>();


            if (reportType == "TRANSFER_SAME_ADMCENTER")
            {
                for (int i = 0; i < initial.Count(); i++)
                {
                    if (admCenterId != null)
                    {
                        foreach (var inventoryAsset in listAssetInventoryDetails.Where(r => r.Inventory.RoomInitial.LocationId == initial[i] && r.Inventory.RoomInitial.Location.AdmCenterId == admCenterId))
                        {

                            //if (inventoryAsset.RoomInitial.LocationId != inventoryAsset.RoomFinal.LocationId || inventoryAsset.EmployeeIdInitial != inventoryAsset.EmployeeIdFinal)
                            //{
                            transferInV1Result.Details.Add(new TransferInV1Detail()
                            {
                                InvNo = inventoryAsset.Asset.InvNo,
                                Description = inventoryAsset.Asset.Name,
                                SerialNumber = inventoryAsset.Asset.SerialNumber,
                                PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                                //RegionInitial = inventoryAsset.RoomInitial.Location.Region != null ? inventoryAsset.RoomInitial.Location.Region.Code : string.Empty,
                                //RegionFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Region != null ? inventoryAsset.RoomFinal.Location.Region.Code : string.Empty : string.Empty,
                                RegionInitial = ((inventoryAsset.Inventory.CostCenterIdInitial != null) && (inventoryAsset.Inventory.CostCenterInitial.AdmCenterId != null)) ? inventoryAsset.Inventory.CostCenterInitial.AdmCenter.Code : string.Empty,
                                RegionFinal = ((inventoryAsset.Inventory.CostCenterIdFinal != null) && (inventoryAsset.Inventory.CostCenterFinal.AdmCenterId != null)) ? inventoryAsset.Inventory.CostCenterFinal.AdmCenter.Code : string.Empty,
                                LocationHeader = inventoryAsset.Inventory.RoomInitial.Location.Name,
                                LocationNameInitial = inventoryAsset.Inventory.RoomInitial.Location.Name,

                                LocationNameFinal = inventoryAsset.Inventory.RoomFinal != null ? inventoryAsset.Inventory.RoomFinal.Location.Name : string.Empty,

                                CostCenterInitial = inventoryAsset.Inventory.CostCenterInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty,
                                CostCenterFinal = inventoryAsset.Inventory.CostCenterFinal != null ? inventoryAsset.Inventory.CostCenterFinal.Code : string.Empty,
                                RoomInitial = inventoryAsset.Inventory.RoomInitial.Name,
                                RoomFinal = inventoryAsset.Inventory.RoomFinal != null ? inventoryAsset.Inventory.RoomFinal.Name : string.Empty,

                                InternalCodeInitial = inventoryAsset.Inventory.EmployeeInitial != null ? inventoryAsset.Inventory.EmployeeInitial.InternalCode : string.Empty,
                                InternalCodeFinal = inventoryAsset.Inventory.EmployeeFinal != null ? inventoryAsset.Inventory.EmployeeFinal.InternalCode : string.Empty,

                                FullNameInitial = inventoryAsset.Inventory.EmployeeInitial != null ? inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName : string.Empty,
                                FullNameFinal = inventoryAsset.Inventory.EmployeeFinal != null ? inventoryAsset.Inventory.EmployeeFinal.FirstName + " " + inventoryAsset.Inventory.EmployeeFinal.LastName : string.Empty,

                                Initial = inventoryAsset.Inventory.QInitial,
                                Actual = inventoryAsset.Inventory.QFinal,
                                Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                                Value = inventoryAsset.Dep.CurrentAPC,
                                ValueDepTotal = inventoryAsset.Dep.CurrentAPC - inventoryAsset.Dep.PosCap,
                                Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                                ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : DateTime.MinValue
                            });
                            //}


                        }
                    }
                    else
                    {
                        foreach (var inventoryAsset in listAssetInventoryDetails.Where(r => r.Inventory.RoomInitial.LocationId == initial[i]))
                        {

                            transferInV1Result.Details.Add(new TransferInV1Detail()
                            {
                                InvNo = inventoryAsset.Asset.InvNo,
                                Description = inventoryAsset.Asset.Name,
                                SerialNumber = inventoryAsset.Asset.SerialNumber,
                                PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                                //RegionInitial = inventoryAsset.RoomInitial.Location.Region != null ? inventoryAsset.RoomInitial.Location.Region.Code : string.Empty,
                                //RegionFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Region != null ? inventoryAsset.RoomFinal.Location.Region.Code : string.Empty : string.Empty,
                                RegionInitial = ((inventoryAsset.Inventory.CostCenterIdInitial != null) && (inventoryAsset.Inventory.CostCenterInitial.AdmCenterId != null)) ? inventoryAsset.Inventory.CostCenterInitial.AdmCenter.Code : string.Empty,
                                RegionFinal = ((inventoryAsset.Inventory.CostCenterIdFinal != null) && (inventoryAsset.Inventory.CostCenterFinal.AdmCenterId != null)) ? inventoryAsset.Inventory.CostCenterFinal.AdmCenter.Code : string.Empty,
                                LocationHeader = inventoryAsset.Inventory.RoomInitial.Location.Name,
                                LocationNameInitial = inventoryAsset.Inventory.RoomInitial.Location.Name,

                                LocationNameFinal = inventoryAsset.Inventory.RoomFinal != null ? inventoryAsset.Inventory.RoomFinal.Location.Name : string.Empty,

                                CostCenterInitial = inventoryAsset.Inventory.CostCenterInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty,
                                CostCenterFinal = inventoryAsset.Inventory.CostCenterFinal != null ? inventoryAsset.Inventory.CostCenterFinal.Code : string.Empty,
                                RoomInitial = inventoryAsset.Inventory.RoomInitial.Name,
                                RoomFinal = inventoryAsset.Inventory.RoomFinal != null ? inventoryAsset.Inventory.RoomFinal.Name : string.Empty,

                                InternalCodeInitial = inventoryAsset.Inventory.EmployeeInitial != null ? inventoryAsset.Inventory.EmployeeInitial.InternalCode : string.Empty,
                                InternalCodeFinal = inventoryAsset.Inventory.EmployeeFinal != null ? inventoryAsset.Inventory.EmployeeFinal.InternalCode : string.Empty,

                                FullNameInitial = inventoryAsset.Inventory.EmployeeInitial != null ? inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName : string.Empty,
                                FullNameFinal = inventoryAsset.Inventory.EmployeeFinal != null ? inventoryAsset.Inventory.EmployeeFinal.FirstName + " " + inventoryAsset.Inventory.EmployeeFinal.LastName : string.Empty,

                                Initial = inventoryAsset.Inventory.QInitial,
                                Actual = inventoryAsset.Inventory.QFinal,
                                Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                                Value = inventoryAsset.Dep.CurrentAPC,
                                ValueDepTotal = inventoryAsset.Dep.CurrentAPC - inventoryAsset.Dep.PosCap,
                                Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                                ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : DateTime.MinValue
                            });

                        }
                    }


                    //if (transferInV1Result.Details[i].LocationNameInitial == transferInV1Result.Details[i].LocationNameFinal)
                    //{
                    //    transferInV1Result.Details[i].IsDeleted = true;
                    //}

                }



                for (int i = 0; i < final.Count(); i++)
                {
                    if (admCenterId != null)
                    {
                        foreach (var inventoryAsset in listAssetInventoryDetails.Where(r => r.Inventory.RoomFinal.LocationId == final[i] && r.Inventory.RoomFinal.Location.AdmCenterId == admCenterId /*&& (r.RoomInitial.LocationId != r.RoomFinal.LocationId || r.EmployeeIdInitial != r.EmployeeIdFinal) */))
                        {
                            //if (inventoryAsset.RoomInitial.LocationId != inventoryAsset.RoomFinal.LocationId || inventoryAsset.EmployeeIdInitial != inventoryAsset.EmployeeIdFinal)
                            //{
                            transferInV1Result.Details.Add(new TransferInV1Detail()
                            {
                                InvNo = inventoryAsset.Asset.InvNo,
                                Description = inventoryAsset.Asset.Name,
                                SerialNumber = inventoryAsset.Asset.SerialNumber,
                                PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                                //RegionInitial = inventoryAsset.RoomInitial.Location.Region != null ? inventoryAsset.RoomInitial.Location.Region.Code : string.Empty,
                                //RegionFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Region != null ? inventoryAsset.RoomFinal.Location.Region.Code : string.Empty : string.Empty,
                                RegionInitial = ((inventoryAsset.Inventory.CostCenterIdInitial != null) && (inventoryAsset.Inventory.CostCenterInitial.AdmCenterId != null)) ? inventoryAsset.Inventory.CostCenterInitial.AdmCenter.Code : string.Empty,
                                RegionFinal = ((inventoryAsset.Inventory.CostCenterIdFinal != null) && (inventoryAsset.Inventory.CostCenterFinal.AdmCenterId != null)) ? inventoryAsset.Inventory.CostCenterFinal.AdmCenter.Code : string.Empty,
                                LocationHeader = inventoryAsset.Inventory.RoomFinal.Location.Name,
                                LocationNameInitial = inventoryAsset.Inventory.RoomInitial.Location.Name,

                                LocationNameFinal = inventoryAsset.Inventory.RoomFinal != null ? inventoryAsset.Inventory.RoomFinal.Location.Name : string.Empty,

                                CostCenterInitial = inventoryAsset.Inventory.CostCenterInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty,
                                CostCenterFinal = inventoryAsset.Inventory.CostCenterFinal != null ? inventoryAsset.Inventory.CostCenterFinal.Code : string.Empty,
                                RoomInitial = inventoryAsset.Inventory.RoomInitial.Name,
                                RoomFinal = inventoryAsset.Inventory.RoomFinal != null ? inventoryAsset.Inventory.RoomFinal.Name : string.Empty,

                                InternalCodeInitial = inventoryAsset.Inventory.EmployeeInitial != null ? inventoryAsset.Inventory.EmployeeInitial.InternalCode : string.Empty,
                                InternalCodeFinal = inventoryAsset.Inventory.EmployeeFinal != null ? inventoryAsset.Inventory.EmployeeFinal.InternalCode : string.Empty,

                                FullNameInitial = inventoryAsset.Inventory.EmployeeInitial != null ? inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName : string.Empty,
                                FullNameFinal = inventoryAsset.Inventory.EmployeeFinal != null ? inventoryAsset.Inventory.EmployeeFinal.FirstName + " " + inventoryAsset.Inventory.EmployeeFinal.LastName : string.Empty,

                                Initial = inventoryAsset.Inventory.QInitial,
                                Actual = inventoryAsset.Inventory.QFinal,
                                Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                                Value = inventoryAsset.Dep.CurrentAPC,
                                ValueDepTotal = inventoryAsset.Dep.CurrentAPC - inventoryAsset.Dep.PosCap,
                                Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                                ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : DateTime.MinValue
                            });
                            //}

                        }
                    }
                    else
                    {
                        foreach (var inventoryAsset in listAssetInventoryDetails.Where(r => r.Inventory.RoomFinal.LocationId == final[i]))
                        {

                            transferInV1Result.Details.Add(new TransferInV1Detail()
                            {
                                InvNo = inventoryAsset.Asset.InvNo,
                                Description = inventoryAsset.Asset.Name,
                                SerialNumber = inventoryAsset.Asset.SerialNumber,
                                PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                                //RegionInitial = inventoryAsset.RoomInitial.Location.Region != null ? inventoryAsset.RoomInitial.Location.Region.Code : string.Empty,
                                //RegionFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Region != null ? inventoryAsset.RoomFinal.Location.Region.Code : string.Empty : string.Empty,
                                RegionInitial = ((inventoryAsset.Inventory.CostCenterIdInitial != null) && (inventoryAsset.Inventory.CostCenterInitial.AdmCenterId != null)) ? inventoryAsset.Inventory.CostCenterInitial.AdmCenter.Code : string.Empty,
                                RegionFinal = ((inventoryAsset.Inventory.CostCenterIdFinal != null) && (inventoryAsset.Inventory.CostCenterFinal.AdmCenterId != null)) ? inventoryAsset.Inventory.CostCenterFinal.AdmCenter.Code : string.Empty,
                                LocationHeader = inventoryAsset.Inventory.RoomFinal.Location.Name,
                                LocationNameInitial = inventoryAsset.Inventory.RoomInitial.Location.Name,

                                LocationNameFinal = inventoryAsset.Inventory.RoomFinal != null ? inventoryAsset.Inventory.RoomFinal.Location.Name : string.Empty,

                                CostCenterInitial = inventoryAsset.Inventory.CostCenterInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty,
                                CostCenterFinal = inventoryAsset.Inventory.CostCenterFinal != null ? inventoryAsset.Inventory.CostCenterFinal.Code : string.Empty,
                                RoomInitial = inventoryAsset.Inventory.RoomInitial.Name,
                                RoomFinal = inventoryAsset.Inventory.RoomFinal != null ? inventoryAsset.Inventory.RoomFinal.Name : string.Empty,

                                InternalCodeInitial = inventoryAsset.Inventory.EmployeeInitial != null ? inventoryAsset.Inventory.EmployeeInitial.InternalCode : string.Empty,
                                InternalCodeFinal = inventoryAsset.Inventory.EmployeeFinal != null ? inventoryAsset.Inventory.EmployeeFinal.InternalCode : string.Empty,

                                FullNameInitial = inventoryAsset.Inventory.EmployeeInitial != null ? inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName : string.Empty,
                                FullNameFinal = inventoryAsset.Inventory.EmployeeFinal != null ? inventoryAsset.Inventory.EmployeeFinal.FirstName + " " + inventoryAsset.Inventory.EmployeeFinal.LastName : string.Empty,

                                Initial = inventoryAsset.Inventory.QInitial,
                                Actual = inventoryAsset.Inventory.QFinal,
                                Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                                Value = inventoryAsset.Dep.CurrentAPC,
                                ValueDepTotal = inventoryAsset.Dep.CurrentAPC - inventoryAsset.Dep.PosCap,
                                Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                                ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : DateTime.MinValue
                            });

                        }
                    }

                }
            }
            else
            {
                foreach (var inventoryAsset in listAssetInventoryDetails)
                {

                    transferInV1Result.Details.Add(new TransferInV1Detail()
                    {
                        InvNo = inventoryAsset.Asset.InvNo,
                        Description = inventoryAsset.Asset.Name,
                        SerialNumber = inventoryAsset.Asset.SerialNumber,
                        PurchaseDate = inventoryAsset.Asset.PurchaseDate,

                        //RegionInitial = inventoryAsset.RoomInitial.Location.Region != null ? inventoryAsset.RoomInitial.Location.Region.Code : string.Empty,
                        //RegionFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Region != null ? inventoryAsset.RoomFinal.Location.Region.Code : string.Empty : string.Empty,
                        RegionInitial = ((inventoryAsset.Inventory.CostCenterIdInitial != null) && (inventoryAsset.Inventory.CostCenterInitial.AdmCenterId != null)) ? inventoryAsset.Inventory.CostCenterInitial.AdmCenter.Code : string.Empty,
                        RegionFinal = ((inventoryAsset.Inventory.CostCenterIdFinal != null) && (inventoryAsset.Inventory.CostCenterFinal.AdmCenterId != null)) ? inventoryAsset.Inventory.CostCenterFinal.AdmCenter.Code : string.Empty,
                        LocationHeader = inventoryAsset.Inventory.RoomInitial.Location.AdmCenterId == admCenterId ? inventoryAsset.Inventory.RoomInitial.Location.Name : inventoryAsset.Inventory.RoomFinal.Location.Name,
                        LocationNameInitial = inventoryAsset.Inventory.RoomInitial.Location.Name,

                        LocationNameFinal = inventoryAsset.Inventory.RoomFinal != null ? inventoryAsset.Inventory.RoomFinal.Location.Name : string.Empty,

                        CostCenterInitial = inventoryAsset.Inventory.CostCenterInitial != null ? inventoryAsset.Inventory.CostCenterInitial.Code : string.Empty,
                        CostCenterFinal = inventoryAsset.Inventory.CostCenterFinal != null ? inventoryAsset.Inventory.CostCenterFinal.Code : string.Empty,
                        RoomInitial = inventoryAsset.Inventory.RoomInitial.Name,
                        RoomFinal = inventoryAsset.Inventory.RoomFinal != null ? inventoryAsset.Inventory.RoomFinal.Name : string.Empty,

                        InternalCodeInitial = inventoryAsset.Inventory.EmployeeInitial != null ? inventoryAsset.Inventory.EmployeeInitial.InternalCode : string.Empty,
                        InternalCodeFinal = inventoryAsset.Inventory.EmployeeFinal != null ? inventoryAsset.Inventory.EmployeeFinal.InternalCode : string.Empty,

                        FullNameInitial = inventoryAsset.Inventory.EmployeeInitial != null ? inventoryAsset.Inventory.EmployeeInitial.FirstName + " " + inventoryAsset.Inventory.EmployeeInitial.LastName : string.Empty,
                        FullNameFinal = inventoryAsset.Inventory.EmployeeFinal != null ? inventoryAsset.Inventory.EmployeeFinal.FirstName + " " + inventoryAsset.Inventory.EmployeeFinal.LastName : string.Empty,

                        Initial = inventoryAsset.Inventory.QInitial,
                        Actual = inventoryAsset.Inventory.QFinal,
                        Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                        Value = inventoryAsset.Dep.CurrentAPC,
                        ValueDepTotal = inventoryAsset.Dep.CurrentAPC - inventoryAsset.Dep.PosCap,
                        Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
                        ModifiedAt = inventoryAsset.Inventory.ModifiedAt != null ? inventoryAsset.Inventory.ModifiedAt.Value : DateTime.MinValue
                    });

                }
            }







            inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);

            var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            transferInV1Result.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            transferInV1Result.ReportType = strReportType;
            transferInV1Result.AdmCenterName = (region != null) ? region.Name : string.Empty;

            transferInV1Result.InventoryName = inventory.Description.Trim();
            var maxDate = transferInV1Result.Details.Count > 0 ? transferInV1Result.Details.Where(s => s.ModifiedAt == transferInV1Result.Details.Max(x => x.ModifiedAt)).FirstOrDefault() : null;
            transferInV1Result.InventoryEndDate = maxDate != null ? maxDate.ModifiedAt.Value : DateTime.Now;
            transferInV1Result.LocationName = (location != null) ? location.Name : string.Empty;

            //if (admCenter != null)
            //{
            //    if (admCenter.Committee != null)
            //    {
            //        if (admCenter.Committee.Length > 0)
            //        {
            //            string[] committees = admCenter.Committee.Split(';');

            //            transferInV1Result.Committee1 = committees[0];
            //            transferInV1Result.Committee2 = committees[1];
            //            transferInV1Result.Committee3 = committees[2];
            //        }
            //    }


            //}

            // NEW  //

            //if (reportType != null)
            //{
            //    switch (reportType.ToUpper())
            //    {
            //        case "TRANSFER_SAME_LOCATION":
            //            invAssetQuery = invAssetQuery
            //                .Where(i => ((i.RoomIdFinal != null)
            //                    && ((i.RoomIdInitial != i.RoomIdFinal) || (i.EmployeeIdInitial != i.EmployeeIdFinal))
            //                    && (i.RoomInitial.LocationId == i.RoomFinal.LocationId)));
            //            strReportType = "Transferuri in aceeasi cladire";
            //            break;
            //        case "TRANSFER_DIFF_LOCATION":
            //            invAssetQuery = invAssetQuery
            //                .Where(i => ((i.RoomIdFinal != null)
            //                    && ((i.RoomIdInitial != i.RoomIdFinal) || (i.EmployeeIdInitial != i.EmployeeIdFinal))
            //                    && (i.RoomInitial.LocationId != i.RoomFinal.LocationId)));
            //            strReportType = "intre camere din aceeasi cladire";
            //            break;
            //        case "TRANSFER_SAME_REGION":
            //            invAssetQuery = invAssetQuery
            //                .Where(i => ((i.RoomIdFinal != null)
            //                    && (i.RoomInitial.LocationId != i.RoomFinal.LocationId)
            //                    && ((i.RoomIdInitial != i.RoomIdFinal) || (i.EmployeeIdInitial != i.EmployeeIdFinal))
            //                    && (i.RoomInitial.Location.RegionId == i.RoomFinal.Location.RegionId)));
            //            strReportType = "in regiune";
            //            break;
            //        case "TRANSFER_DIFF_REGION":
            //            invAssetQuery = invAssetQuery
            //                .Where(i => ((i.RoomIdFinal != null)
            //                    && ((i.RoomIdInitial != i.RoomIdFinal) || (i.EmployeeIdInitial != i.EmployeeIdFinal))
            //                    && (i.RoomInitial.Location.RegionId != i.RoomFinal.Location.RegionId)));
            //            strReportType = "intre regiuni";
            //            break;
            //        case "TRANSFER_SAME_ADMCENTER":
            //            //invAssetQuery = invAssetQuery
            //            //    .Where(i => ((i.CostCenterIdFinal != null)
            //            //        //&& (i.CostCenterIdInitial != i.CostCenterIdFinal)
            //            //        && (i.RoomInitial.LocationId != i.RoomFinal.LocationId)
            //            //        && (i.CostCenterInitial.AdmCenterId == i.CostCenterFinal.AdmCenterId)));

            //            invAssetQuery = invAssetQuery
            //            .Where(i => ((i.CostCenterIdFinal != null)
            //                && ((i.RoomInitial.LocationId != i.RoomFinal.LocationId))
            //                && (i.CostCenterInitial.AdmCenterId == i.CostCenterFinal.AdmCenterId)));

            //            strReportType = "Transferuri in unitatea logistica";
            //            break;
            //        case "TRANSFER_DIFF_ADMCENTER":
            //            invAssetQuery = invAssetQuery
            //                .Where(i => ((i.CostCenterIdFinal != null)
            //                    //&& (i.CostCenterIdInitial != i.CostCenterIdFinal)
            //                    && (i.CostCenterInitial.AdmCenterId != i.CostCenterFinal.AdmCenterId)));
            //            strReportType = "Transferuri intre unitati logistice";
            //            break;
            //        default:
            //            break;
            //    }
            //}


            //listInventoryAssets = invAssetQuery.ToList();

            //var initial = listInventoryAssets.Select(r => r.RoomInitial.LocationId).Distinct().ToList();
            //var final = listInventoryAssets.Select(r => r.RoomFinal.LocationId).Distinct().ToList();


            //transferInV1Result = new TransferInV1Result();

            //transferInV1Result.Details = new List<TransferInV1Detail>();


            //if (reportType == "TRANSFER_SAME_ADMCENTER")
            //{
            //    for (int i = 0; i < initial.Count(); i++)
            //    {
            //        if (admCenterId != null)
            //        {
            //            foreach (var inventoryAsset in listInventoryAssets.Where(r => r.RoomInitial.LocationId == initial[i] && r.RoomInitial.Location.AdmCenterId == admCenterId))
            //            {

            //                //if (inventoryAsset.RoomInitial.LocationId != inventoryAsset.RoomFinal.LocationId || inventoryAsset.EmployeeIdInitial != inventoryAsset.EmployeeIdFinal)
            //                //{
            //                transferInV1Result.Details.Add(new TransferInV1Detail()
            //                {
            //                    InvNo = inventoryAsset.Asset.InvNo,
            //                    Description = inventoryAsset.Asset.Name,
            //                    SerialNumber = inventoryAsset.Asset.SerialNumber,
            //                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

            //                    //RegionInitial = inventoryAsset.RoomInitial.Location.Region != null ? inventoryAsset.RoomInitial.Location.Region.Code : string.Empty,
            //                    //RegionFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Region != null ? inventoryAsset.RoomFinal.Location.Region.Code : string.Empty : string.Empty,
            //                    RegionInitial = ((inventoryAsset.CostCenterIdInitial != null) && (inventoryAsset.CostCenterInitial.AdmCenterId != null)) ? inventoryAsset.CostCenterInitial.AdmCenter.Code : string.Empty,
            //                    RegionFinal = ((inventoryAsset.CostCenterIdFinal != null) && (inventoryAsset.CostCenterFinal.AdmCenterId != null)) ? inventoryAsset.CostCenterFinal.AdmCenter.Code : string.Empty,
            //                    LocationHeader = inventoryAsset.RoomInitial.Location.Name,
            //                    LocationNameInitial = inventoryAsset.RoomInitial.Location.Name,

            //                    LocationNameFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Name : string.Empty,

            //                    CostCenterInitial = inventoryAsset.CostCenterInitial != null ? inventoryAsset.CostCenterInitial.Code : string.Empty,
            //                    CostCenterFinal = inventoryAsset.CostCenterFinal != null ? inventoryAsset.CostCenterFinal.Code : string.Empty,
            //                    RoomInitial = inventoryAsset.RoomInitial.Name,
            //                    RoomFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Name : string.Empty,

            //                    InternalCodeInitial = inventoryAsset.EmployeeInitial != null ? inventoryAsset.EmployeeInitial.InternalCode : string.Empty,
            //                    InternalCodeFinal = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.InternalCode : string.Empty,

            //                    FullNameInitial = inventoryAsset.EmployeeInitial != null ? inventoryAsset.EmployeeInitial.FirstName + " " + inventoryAsset.EmployeeInitial.LastName : string.Empty,
            //                    FullNameFinal = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.FirstName + " " + inventoryAsset.EmployeeFinal.LastName : string.Empty,

            //                    Initial = inventoryAsset.QInitial,
            //                    Actual = inventoryAsset.QFinal,
            //                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
            //                    Value = inventoryAsset.Asset.ValueInv,
            //                    ValueDepTotal = inventoryAsset.Asset.ValueRem,
            //                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
            //                    ModifiedAt = inventoryAsset.ModifiedAt != null ? inventoryAsset.ModifiedAt.Value : DateTime.MinValue
            //                });
            //                //}


            //            }
            //        }
            //        else
            //        {
            //            foreach (var inventoryAsset in listInventoryAssets.Where(r => r.RoomInitial.LocationId == initial[i]))
            //            {

            //                transferInV1Result.Details.Add(new TransferInV1Detail()
            //                {
            //                    InvNo = inventoryAsset.Asset.InvNo,
            //                    Description = inventoryAsset.Asset.Name,
            //                    SerialNumber = inventoryAsset.Asset.SerialNumber,
            //                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

            //                    //RegionInitial = inventoryAsset.RoomInitial.Location.Region != null ? inventoryAsset.RoomInitial.Location.Region.Code : string.Empty,
            //                    //RegionFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Region != null ? inventoryAsset.RoomFinal.Location.Region.Code : string.Empty : string.Empty,
            //                    RegionInitial = ((inventoryAsset.CostCenterIdInitial != null) && (inventoryAsset.CostCenterInitial.AdmCenterId != null)) ? inventoryAsset.CostCenterInitial.AdmCenter.Code : string.Empty,
            //                    RegionFinal = ((inventoryAsset.CostCenterIdFinal != null) && (inventoryAsset.CostCenterFinal.AdmCenterId != null)) ? inventoryAsset.CostCenterFinal.AdmCenter.Code : string.Empty,
            //                    LocationHeader = inventoryAsset.RoomInitial.Location.Name,
            //                    LocationNameInitial = inventoryAsset.RoomInitial.Location.Name,

            //                    LocationNameFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Name : string.Empty,

            //                    CostCenterInitial = inventoryAsset.CostCenterInitial != null ? inventoryAsset.CostCenterInitial.Code : string.Empty,
            //                    CostCenterFinal = inventoryAsset.CostCenterFinal != null ? inventoryAsset.CostCenterFinal.Code : string.Empty,
            //                    RoomInitial = inventoryAsset.RoomInitial.Name,
            //                    RoomFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Name : string.Empty,

            //                    InternalCodeInitial = inventoryAsset.EmployeeInitial != null ? inventoryAsset.EmployeeInitial.InternalCode : string.Empty,
            //                    InternalCodeFinal = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.InternalCode : string.Empty,

            //                    FullNameInitial = inventoryAsset.EmployeeInitial != null ? inventoryAsset.EmployeeInitial.FirstName + " " + inventoryAsset.EmployeeInitial.LastName : string.Empty,
            //                    FullNameFinal = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.FirstName + " " + inventoryAsset.EmployeeFinal.LastName : string.Empty,

            //                    Initial = inventoryAsset.QInitial,
            //                    Actual = inventoryAsset.QFinal,
            //                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
            //                    Value = inventoryAsset.Asset.ValueInv,
            //                    ValueDepTotal = inventoryAsset.Asset.ValueRem,
            //                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
            //                    ModifiedAt = inventoryAsset.ModifiedAt != null ? inventoryAsset.ModifiedAt.Value : DateTime.MinValue
            //                });

            //            }
            //        }


            //        //if (transferInV1Result.Details[i].LocationNameInitial == transferInV1Result.Details[i].LocationNameFinal)
            //        //{
            //        //    transferInV1Result.Details[i].IsDeleted = true;
            //        //}

            //    }



            //    for (int i = 0; i < final.Count(); i++)
            //    {
            //        if (admCenterId != null)
            //        {
            //            foreach (var inventoryAsset in listInventoryAssets.Where(r => r.RoomFinal.LocationId == final[i] && r.RoomFinal.Location.AdmCenterId == admCenterId /*&& (r.RoomInitial.LocationId != r.RoomFinal.LocationId || r.EmployeeIdInitial != r.EmployeeIdFinal) */))
            //            {
            //                //if (inventoryAsset.RoomInitial.LocationId != inventoryAsset.RoomFinal.LocationId || inventoryAsset.EmployeeIdInitial != inventoryAsset.EmployeeIdFinal)
            //                //{
            //                transferInV1Result.Details.Add(new TransferInV1Detail()
            //                {
            //                    InvNo = inventoryAsset.Asset.InvNo,
            //                    Description = inventoryAsset.Asset.Name,
            //                    SerialNumber = inventoryAsset.Asset.SerialNumber,
            //                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

            //                    //RegionInitial = inventoryAsset.RoomInitial.Location.Region != null ? inventoryAsset.RoomInitial.Location.Region.Code : string.Empty,
            //                    //RegionFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Region != null ? inventoryAsset.RoomFinal.Location.Region.Code : string.Empty : string.Empty,
            //                    RegionInitial = ((inventoryAsset.CostCenterIdInitial != null) && (inventoryAsset.CostCenterInitial.AdmCenterId != null)) ? inventoryAsset.CostCenterInitial.AdmCenter.Code : string.Empty,
            //                    RegionFinal = ((inventoryAsset.CostCenterIdFinal != null) && (inventoryAsset.CostCenterFinal.AdmCenterId != null)) ? inventoryAsset.CostCenterFinal.AdmCenter.Code : string.Empty,
            //                    LocationHeader = inventoryAsset.RoomFinal.Location.Name,
            //                    LocationNameInitial = inventoryAsset.RoomInitial.Location.Name,

            //                    LocationNameFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Name : string.Empty,

            //                    CostCenterInitial = inventoryAsset.CostCenterInitial != null ? inventoryAsset.CostCenterInitial.Code : string.Empty,
            //                    CostCenterFinal = inventoryAsset.CostCenterFinal != null ? inventoryAsset.CostCenterFinal.Code : string.Empty,
            //                    RoomInitial = inventoryAsset.RoomInitial.Name,
            //                    RoomFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Name : string.Empty,

            //                    InternalCodeInitial = inventoryAsset.EmployeeInitial != null ? inventoryAsset.EmployeeInitial.InternalCode : string.Empty,
            //                    InternalCodeFinal = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.InternalCode : string.Empty,

            //                    FullNameInitial = inventoryAsset.EmployeeInitial != null ? inventoryAsset.EmployeeInitial.FirstName + " " + inventoryAsset.EmployeeInitial.LastName : string.Empty,
            //                    FullNameFinal = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.FirstName + " " + inventoryAsset.EmployeeFinal.LastName : string.Empty,

            //                    Initial = inventoryAsset.QInitial,
            //                    Actual = inventoryAsset.QFinal,
            //                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
            //                    Value = inventoryAsset.Asset.ValueInv,
            //                    ValueDepTotal = inventoryAsset.Asset.ValueRem,
            //                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
            //                    ModifiedAt = inventoryAsset.ModifiedAt != null ? inventoryAsset.ModifiedAt.Value : DateTime.MinValue
            //                });
            //                //}

            //            }
            //        }
            //        else
            //        {
            //            foreach (var inventoryAsset in listInventoryAssets.Where(r => r.RoomFinal.LocationId == final[i]))
            //            {

            //                transferInV1Result.Details.Add(new TransferInV1Detail()
            //                {
            //                    InvNo = inventoryAsset.Asset.InvNo,
            //                    Description = inventoryAsset.Asset.Name,
            //                    SerialNumber = inventoryAsset.Asset.SerialNumber,
            //                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,

            //                    //RegionInitial = inventoryAsset.RoomInitial.Location.Region != null ? inventoryAsset.RoomInitial.Location.Region.Code : string.Empty,
            //                    //RegionFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Region != null ? inventoryAsset.RoomFinal.Location.Region.Code : string.Empty : string.Empty,
            //                    RegionInitial = ((inventoryAsset.CostCenterIdInitial != null) && (inventoryAsset.CostCenterInitial.AdmCenterId != null)) ? inventoryAsset.CostCenterInitial.AdmCenter.Code : string.Empty,
            //                    RegionFinal = ((inventoryAsset.CostCenterIdFinal != null) && (inventoryAsset.CostCenterFinal.AdmCenterId != null)) ? inventoryAsset.CostCenterFinal.AdmCenter.Code : string.Empty,
            //                    LocationHeader = inventoryAsset.RoomFinal.Location.Name,
            //                    LocationNameInitial = inventoryAsset.RoomInitial.Location.Name,

            //                    LocationNameFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Name : string.Empty,

            //                    CostCenterInitial = inventoryAsset.CostCenterInitial != null ? inventoryAsset.CostCenterInitial.Code : string.Empty,
            //                    CostCenterFinal = inventoryAsset.CostCenterFinal != null ? inventoryAsset.CostCenterFinal.Code : string.Empty,
            //                    RoomInitial = inventoryAsset.RoomInitial.Name,
            //                    RoomFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Name : string.Empty,

            //                    InternalCodeInitial = inventoryAsset.EmployeeInitial != null ? inventoryAsset.EmployeeInitial.InternalCode : string.Empty,
            //                    InternalCodeFinal = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.InternalCode : string.Empty,

            //                    FullNameInitial = inventoryAsset.EmployeeInitial != null ? inventoryAsset.EmployeeInitial.FirstName + " " + inventoryAsset.EmployeeInitial.LastName : string.Empty,
            //                    FullNameFinal = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.FirstName + " " + inventoryAsset.EmployeeFinal.LastName : string.Empty,

            //                    Initial = inventoryAsset.QInitial,
            //                    Actual = inventoryAsset.QFinal,
            //                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
            //                    Value = inventoryAsset.Asset.ValueInv,
            //                    ValueDepTotal = inventoryAsset.Asset.ValueRem,
            //                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
            //                    ModifiedAt = inventoryAsset.ModifiedAt != null ? inventoryAsset.ModifiedAt.Value : DateTime.MinValue
            //                });

            //            }
            //        }

            //    }
            //}
            //else
            //{
            //    foreach (var inventoryAsset in listInventoryAssets)
            //    {

            //        transferInV1Result.Details.Add(new TransferInV1Detail()
            //        {
            //            InvNo = inventoryAsset.Asset.InvNo,
            //            Description = inventoryAsset.Asset.Name,
            //            SerialNumber = inventoryAsset.Asset.SerialNumber,
            //            PurchaseDate = inventoryAsset.Asset.PurchaseDate,

            //            //RegionInitial = inventoryAsset.RoomInitial.Location.Region != null ? inventoryAsset.RoomInitial.Location.Region.Code : string.Empty,
            //            //RegionFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Region != null ? inventoryAsset.RoomFinal.Location.Region.Code : string.Empty : string.Empty,
            //            RegionInitial = ((inventoryAsset.CostCenterIdInitial != null) && (inventoryAsset.CostCenterInitial.AdmCenterId != null)) ? inventoryAsset.CostCenterInitial.AdmCenter.Code : string.Empty,
            //            RegionFinal = ((inventoryAsset.CostCenterIdFinal != null) && (inventoryAsset.CostCenterFinal.AdmCenterId != null)) ? inventoryAsset.CostCenterFinal.AdmCenter.Code : string.Empty,
            //            LocationHeader = inventoryAsset.RoomInitial.Location.AdmCenterId == admCenterId ? inventoryAsset.RoomInitial.Location.Name : inventoryAsset.RoomFinal.Location.Name,
            //            LocationNameInitial = inventoryAsset.RoomInitial.Location.Name,

            //            LocationNameFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Location.Name : string.Empty,

            //            CostCenterInitial = inventoryAsset.CostCenterInitial != null ? inventoryAsset.CostCenterInitial.Code : string.Empty,
            //            CostCenterFinal = inventoryAsset.CostCenterFinal != null ? inventoryAsset.CostCenterFinal.Code : string.Empty,
            //            RoomInitial = inventoryAsset.RoomInitial.Name,
            //            RoomFinal = inventoryAsset.RoomFinal != null ? inventoryAsset.RoomFinal.Name : string.Empty,

            //            InternalCodeInitial = inventoryAsset.EmployeeInitial != null ? inventoryAsset.EmployeeInitial.InternalCode : string.Empty,
            //            InternalCodeFinal = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.InternalCode : string.Empty,

            //            FullNameInitial = inventoryAsset.EmployeeInitial != null ? inventoryAsset.EmployeeInitial.FirstName + " " + inventoryAsset.EmployeeInitial.LastName : string.Empty,
            //            FullNameFinal = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.FirstName + " " + inventoryAsset.EmployeeFinal.LastName : string.Empty,

            //            Initial = inventoryAsset.QInitial,
            //            Actual = inventoryAsset.QFinal,
            //            Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
            //            Value = inventoryAsset.Asset.ValueInv,
            //            ValueDepTotal = inventoryAsset.Asset.ValueRem,
            //            Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,
            //            ModifiedAt = inventoryAsset.ModifiedAt != null ? inventoryAsset.ModifiedAt.Value : DateTime.MinValue
            //        });

            //    }
            //}







            //inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);

            //var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            //transferInV1Result.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            //transferInV1Result.ReportType = strReportType;
            //transferInV1Result.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;

            //transferInV1Result.InventoryName = inventory.Description.Trim();
            //var maxDate = transferInV1Result.Details.Count > 0 ? transferInV1Result.Details.Where(s => s.ModifiedAt == transferInV1Result.Details.Max(x => x.ModifiedAt)).FirstOrDefault() : null;
            //transferInV1Result.InventoryEndDate = maxDate != null ? maxDate.ModifiedAt.Value : DateTime.Now;
            //transferInV1Result.LocationName = (location != null) ? location.Name : string.Empty;

            //if (admCenter != null)
            //{
            //    if (admCenter.Committee != null)
            //    {
            //        if (admCenter.Committee.Length > 0)
            //        {
            //            string[] committees = admCenter.Committee.Split(';');

            //            transferInV1Result.Committee1 = committees[0];
            //            transferInV1Result.Committee2 = committees[1];
            //            transferInV1Result.Committee3 = committees[2];
            //        }
            //    }


            //}

            // transferInV1Result.Details = transferInV1Result.Details.Where(i => i.IsDeleted == false).ToList();

            return transferInV1Result;
        }

        public TransferOutV1Result GetTransferOutV1MultipleByFilters(int inventoryId, int? admCenterId, int? regionId, int? locationId)
        {
            TransferOutV1Result transferOutV1Result = null;
            Model.AdmCenter admCenter = null;
            Model.Region region = null;
            Model.Location location = null;
            Model.Inventory inventory = null;
            Model.AccMonth accMonth = null;
            IQueryable<Model.InventoryAsset> invAssetQuery = null;
            List<Model.InventoryAsset> listInventoryAssets = null;

            IQueryable<Model.Asset> assetQuery = null;
            IQueryable<AssetDepMD> depQuery = null;
            List<Model.AssetInventoryDetail> listAssetInventoryDetails = null;
            IQueryable<Model.AssetInventoryDetail> query = null;

            //if (inventoryId > 0)
            //    inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);
            //else
            //    inventory = _context.Set<Model.Inventory>().OrderByDescending(i => i.Id).FirstOrDefault(i => i.Active == true);

            //if (inventory == null) return null;

            //Model.InvState outInvState = _context.Set<Model.InvState>().First(s => s.Code == "OUT");
            //List<int?> invStateOutIds = _context.Set<Model.InvState>().Where(s => s.ParentCode == "INV_COMP_STATE" && s.Code.StartsWith("OUT")).Select(s => (int?)s.Id).ToList();
            //Model.InvState outInvState = _context.Set<Model.InvState>().First(s => s.Id == 22);
            //List<int?> invStateOutIds = _context.Set<Model.InvState>().Where(s => s.ParentCode == "INV_COMP_STATE" && s.Code.StartsWith("OUT")).Select(s => (int?)s.Id).ToList();
            List<int?> invStateOutIds = _context.Set<Model.InvState>().Where(s => s.Id == 22 || s.Id == 21).Select(s => (int?)s.Id).ToList();
            location = (locationId.HasValue && locationId > 0) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
            region = (regionId.HasValue && regionId > 0) ? _context.Set<Model.Region>().Single(a => a.Id == regionId) : null;
            admCenter = (admCenterId.HasValue && admCenterId > 0) ? _context.Set<Model.AdmCenter>().Single(a => a.Id == admCenterId) : null;

            int accSystemId = 1;

            if (inventoryId > 0)
            {
                inventory = _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId).SingleOrDefault();

                if (inventory != null)
                {
                    accMonth = _context.Set<Model.AccMonth>().Where(a => a.Id == inventory.AccMonthId).SingleOrDefault();
                }
            }


            assetQuery = _context.Assets
                .Include(u => u.Uom)
               .Include(r => r.Room)
                   .ThenInclude(l => l.Location)
                    .ThenInclude(l => l.Region)
               .Include(c => c.CostCenter)
                   //.ThenInclude(a => a.AdmCenter)
               .Include(e => e.Employee)
               .Include(a => a.AssetState)
               .AsQueryable();
            depQuery = _context.AssetDepMDs.AsQueryable().Where(a => a.AccSystemId == accSystemId && a.AccMonthId == accMonth.Id);

            invAssetQuery = _context.InventoryAssets
                .Include(r => r.RoomInitial)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterInitial)
                    //.ThenInclude(a => a.AdmCenter)
                .Include(e => e.EmployeeInitial)
                .Include(s => s.StateInitial)
                .Include(r => r.RoomFinal)
                    .ThenInclude(l => l.Location)
                       .ThenInclude(a => a.Region)
                .Include(r => r.CostCenterFinal)
                    //.ThenInclude(r => r.AdmCenter)
                 .Include(e => e.EmployeeFinal)
                  .Include(s => s.StateFinal)

                .Where(i => i.InventoryId == inventoryId).AsQueryable();


            query = assetQuery.Select(asset => new Model.AssetInventoryDetail { Asset = asset });


            query = query
                .Join(invAssetQuery, q => q.Asset.Id, inv => inv.AssetId, (q, inv) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = inv });



            query = query
                .Join(depQuery, q => q.Asset.Id, dep => dep.AssetId, (q, dep) => new Model.AssetInventoryDetail { Asset = q.Asset, Inventory = q.Inventory, Dep = dep });

            //invAssetQuery = _assetsRepository.GetInventoryAssetsQuery2(inventoryId, null);

            if (admCenterId.HasValue && (admCenterId > 0))
            {
                query = query.Where(i => i.Inventory.CostCenterFinal.AdmCenterId == admCenterId);
            }

            if (regionId.HasValue && (regionId > 0))
            {
                query = query.Where(i => i.Inventory.RoomFinal.Location.RegionId == regionId);
            }

            if (locationId.HasValue && locationId > 0)
            {
                query = query.Where(i => i.Inventory.RoomFinal.LocationId == locationId);
            }

            query = query.Where(i => invStateOutIds.Contains(i.Inventory.StateIdFinal));

            listAssetInventoryDetails = query.ToList();

            transferOutV1Result = new TransferOutV1Result();

            transferOutV1Result.Details = new List<TransferOutV1Detail>();

            foreach (var inventoryAsset in listAssetInventoryDetails)
            {
                TransferOutV1Detail transfer = new TransferOutV1Detail()
                {
                    InvNo = inventoryAsset.Asset.InvNo,
                    Description = inventoryAsset.Asset.Name,
                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,
                    RegionName = inventoryAsset.Inventory.RoomFinal.Location.Region.Name,
                    LocationName = inventoryAsset.Inventory.RoomFinal.Location.Name,
                    Room = (inventoryAsset.Inventory.RoomFinal != null && inventoryAsset.Inventory.RoomFinal.Name != string.Empty) ? inventoryAsset.Inventory.RoomFinal.Name : inventoryAsset.Inventory.RoomInitial.Name,
                    Value = inventoryAsset.Dep.CurrentAPC,
                    ValueDep = inventoryAsset.Dep.AccumulDep,
                    EmployeeInternalCode = (inventoryAsset.Inventory.EmployeeFinal != null && inventoryAsset.Inventory.EmployeeFinal.InternalCode.ToUpper().Contains("_")) ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.InternalCode,
                    EmployeeFirstName = (inventoryAsset.Inventory.EmployeeFinal != null && inventoryAsset.Inventory.EmployeeFinal.InternalCode.ToUpper().Contains("_")) ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.FirstName,
                    EmployeeLastName = (inventoryAsset.Inventory.EmployeeFinal != null && inventoryAsset.Inventory.EmployeeFinal.InternalCode.ToUpper().Contains("_")) ? string.Empty : inventoryAsset.Inventory.EmployeeFinal.LastName,
                    Info = ((inventoryAsset.Inventory.Info != null) && (inventoryAsset.Inventory.Info.Length > 0)) ? inventoryAsset.Inventory.Info : (inventoryAsset.Inventory.StateFinal != null ? inventoryAsset.Inventory.StateFinal.Name : string.Empty),
                    ModifiedAt = inventoryAsset.Inventory.ModifiedAt.Value
                };

                if (inventoryAsset.Inventory.CostCenterIdFinal != null)
                {
                    transfer.CostCenter = inventoryAsset.Inventory.CostCenterFinal.Code;
                    if (inventoryAsset.Inventory.CostCenterFinal.AdmCenterId != null)
                        transfer.RegionName = inventoryAsset.Inventory.CostCenterFinal.AdmCenter.Code;
                }
                else
                {
                    if (inventoryAsset.Inventory.CostCenterIdInitial != null)
                    {
                        transfer.CostCenter = inventoryAsset.Inventory.CostCenterInitial.Code;
                        if (inventoryAsset.Inventory.CostCenterInitial.AdmCenterId != null)
                            transfer.RegionName = inventoryAsset.Inventory.CostCenterInitial.AdmCenter.Code;
                    }
                }

                transferOutV1Result.Details.Add(transfer);
            }

            inventory = _context.Set<Model.Inventory>().Single(i => i.Id == inventoryId);

            var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            transferOutV1Result.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            transferOutV1Result.AdmCenterName = (region != null) ? region.Name : string.Empty;

            transferOutV1Result.InventoryName = inventory.Description.Trim();
            var maxDate = transferOutV1Result.Details.Count > 0 ? transferOutV1Result.Details.Where(s => s.ModifiedAt == transferOutV1Result.Details.Max(x => x.ModifiedAt)).FirstOrDefault() : null;
            transferOutV1Result.InventoryEndDate = maxDate != null ? (maxDate.ModifiedAt != null ? maxDate.ModifiedAt.Value : DateTime.Now) : DateTime.Now;

            transferOutV1Result.LocationName = (location != null) ? location.Name : string.Empty;

            return transferOutV1Result;
        }

		public InventoryResult GetEmployeeReportData(string reportType, string internalCode)
		{
			InventoryResult invResult = null;
			// IEnumerable<Model.EmailManager> emailManagers = null;
			IEnumerable<Model.InventoryAsset> invQuery = null;
			Model.Employee employee = null;
			Model.Inventory inventory = null;

			inventory = _context.Set<Model.Inventory>().Where(a => a.Active == true).SingleOrDefault();
			employee = _context.Set<Model.Employee>().FirstOrDefault(e => e.Guid.ToString().ToUpper() == internalCode);
			invResult = new InventoryResult();
			invResult.Details = new List<InventoryResultDetail>();

			if (employee != null)
			{
				//emailManagers = _context.Set<Model.EmailManager>()
				//.Include(a => a.Asset)
				//.Include(a => a.RoomFinal)
				//.Include(a => a.EmployeeFinal)
				//.Where(a => a.EmployeeIdFinal == employee.Id);

				invQuery = _context.Set<Model.InventoryAsset>()
						.Include(a => a.Asset)
						.Include(a => a.RoomFinal)
							.ThenInclude(a => a.Location)
						.Include(a => a.EmployeeFinal)
						.Where(a => a.EmployeeIdFinal == employee.Id || a.EmployeeIdInitial == employee.Id);





				if (employee != null)
				{
					invResult.EmployeeInternalCode = employee.InternalCode;
					invResult.EmployeeFirstName = employee.FirstName;
					invResult.EmployeeLastName = employee.LastName;
				}

				if (reportType != null)
				{
					switch (reportType.ToUpper())
					{
						case "EMPLOYEEREPORT":
							invResult.InventoryListType = "";
							break;

						default:
							invResult.InventoryListType = string.Empty;
							break;
					}
				}



				foreach (var assetDetail in invQuery)
				{
					InventoryResultDetail inventoryResultDetail = null;
					inventoryResultDetail = new InventoryResultDetail()
					{
						InvNo = assetDetail.Asset.InvNo,
						Description = assetDetail.Asset.Name,
						SerialNumber = assetDetail.Asset.SerialNumber,
						// ERPCode = assetDetail.Asset.ERPCode,
						// PurchaseDate = assetDetail.Asset.PurchaseDate,
						// CostCenter = assetDetail.Adm.CostCenter != null ? assetDetail.Adm.CostCenter.Name : string.Empty,
						//Building = assetDetail.Adm.Room != null ? assetDetail.Adm.Room.Location.Name : string.Empty,
						//BuildingName = assetDetail.Adm.Room != null ? assetDetail.Adm.Room.Location.Name : string.Empty,
						//BuildingCode = assetDetail.Adm.Room != null ? assetDetail.Adm.Room.Location.Code : string.Empty,
						Room = assetDetail.RoomFinal != null ? assetDetail.RoomFinal.Location.Name : assetDetail.RoomInitial != null ? assetDetail.RoomInitial.Location.Name : string.Empty,
						RoomCode = assetDetail.RoomFinal != null ? assetDetail.RoomFinal.Code : assetDetail.RoomInitial != null ? assetDetail.RoomInitial.Code : string.Empty,
						//RoomName = assetDetail.Adm.Room != null ? assetDetail.Adm.Room.Name : string.Empty,
						// Initial = 0,
						// Actual = 0,
						// Uom = assetDetail.Asset.UomId != null ? assetDetail.Asset.Uom.Name : string.Empty,
						// Value = assetDetail.Asset.ValueInv,
						// ValueInv = assetDetail.Asset.ValueInv,
						// ValueDep = assetDetail.Asset.ValueRem,
						//ValueDepTotal = assetDetail.Dep.ValueDepYTD,
						//Info = assetDetail.Asset.AssetInv.Info,
						Info = assetDetail.InfoMinus,
						AppStateId = assetDetail.Asset.AppStateId != null ? assetDetail.Asset.AppStateId : 8,
						FullName = assetDetail.EmployeeFinal != null ? assetDetail.EmployeeFinal.FirstName + " " + assetDetail.EmployeeFinal.LastName : string.Empty,
						// AssetCategory = assetDetail.Adm.AssetCategory != null ? assetDetail.Adm.AssetCategory.Code : string.Empty,
						// AssetCategoryPrefix = assetDetail.Adm.AssetCategory != null ? assetDetail.Adm.AssetCategory.Prefix : string.Empty,
						// AssetType = assetDetail.Adm.AssetType != null ? assetDetail.Adm.AssetType.Name : string.Empty

					};

					invResult.Details.Add(inventoryResultDetail);
				}



				//var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
				//invResult.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
				//invResult.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;
				//invResult.RegionName = (region != null) ? (region.Code.Length > 0) ? region.Code + " - " : "" + region.Name : string.Empty;
				invResult.InventoryEndDate = DateTime.Now;
				//invResult.LocationName = (location != null) ? location.Name : string.Empty;
			}

			return invResult;
		}

		//     public InventoryResult GetEmployeeReportData(string reportType, string internalCode)
		//     {
		//         InventoryResult invResult = null;
		//         Model.AdmCenter admCenter = null;
		//         Model.Location location = null;
		//         Model.Region region = null;
		//         Model.Employee employee = null;
		//         Model.AccMonth accMonth = null;
		//         IEnumerable<Model.AssetMonthDetail> assets = null;
		//         List<Model.AssetDetail> listEmployeeAssets = null;

		//         var includes = "Dep,Adm,Adm.Room.Location,Adm.Employee,Adm.AssetCategory,Adm.AssetType,Adm.CostCenter,Asset.Uom";

		//         AssetDepTotal total = null;
		//         AssetCategoryTotal aCat = null;
		//         AssetFilter assetFilter = null;

		//         accMonth = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).SingleOrDefault();
		//employee = _context.Set<Model.Employee>().FirstOrDefault(e => e.Guid.ToString().ToUpper() == internalCode);

		//assetFilter = new AssetFilter();
		//         assetFilter.AccMonthId = accMonth.Id;
		//         assetFilter.EmployeeIds = new List<int?>() { employee.Id };

		//         assets = _assetsRepository.GetMonth(assetFilter, includes, null, null, out total, out aCat);

		//         invResult = new InventoryResult();
		//         if (employee != null)
		//         {
		//             invResult.EmployeeInternalCode = employee.InternalCode;
		//             invResult.EmployeeFirstName = employee.FirstName;
		//             invResult.EmployeeLastName = employee.LastName;
		//         }

		//         if (reportType != null)
		//         {
		//             switch (reportType.ToUpper())
		//             {
		//                 case "EMPLOYEEREPORT":
		//                     invResult.InventoryListType = "";
		//                     break;

		//                 default:
		//                     invResult.InventoryListType = string.Empty;
		//                     break;
		//             }
		//         }

		//         invResult.Details = new List<InventoryResultDetail>();

		//         foreach (var assetDetail in assets)
		//         {
		//             InventoryResultDetail inventoryResultDetail = null;
		//             inventoryResultDetail = new InventoryResultDetail()
		//             {
		//                 InvNo = assetDetail.Asset.InvNo,
		//                 Description = assetDetail.Asset.Name,
		//                 SerialNumber = assetDetail.Asset.SerialNumber,
		//                 ERPCode = assetDetail.Asset.ERPCode,
		//                 PurchaseDate = assetDetail.Asset.PurchaseDate,
		//                 CostCenter = assetDetail.Adm.CostCenter != null ? assetDetail.Adm.CostCenter.Name : string.Empty,
		//                 Building = assetDetail.Adm.Room != null ? assetDetail.Adm.Room.Location.Name : string.Empty,
		//                 BuildingName = assetDetail.Adm.Room != null ? assetDetail.Adm.Room.Location.Name : string.Empty,
		//                 BuildingCode = assetDetail.Adm.Room != null ? assetDetail.Adm.Room.Location.Code : string.Empty,
		//                 Room = assetDetail.Adm.Room != null ? assetDetail.Adm.Room.Name : string.Empty,
		//                 RoomCode = assetDetail.Adm.Room != null ? assetDetail.Adm.Room.Code : string.Empty,
		//                 RoomName = assetDetail.Adm.Room != null ? assetDetail.Adm.Room.Name : string.Empty,
		//                 Initial = 0,
		//                 Actual = 0,
		//                 Uom = assetDetail.Asset.UomId != null ? assetDetail.Asset.Uom.Name : string.Empty,
		//                 Value = assetDetail.Dep.ValueInv,
		//                 ValueInv = assetDetail.Dep.ValueInv,
		//                 ValueDep = assetDetail.Dep.ValueDep,
		//                 ValueDepTotal = assetDetail.Dep.ValueDepYTD,
		//                 //Info = assetDetail.Asset.AssetInv.Info,
		//                 Info = string.Empty,
		//                 Custody = string.Empty,
		//                 FullName = assetDetail.Adm.Employee != null ? assetDetail.Adm.Employee.FirstName + " " + assetDetail.Adm.Employee.LastName : string.Empty,
		//                 AssetCategory = assetDetail.Adm.AssetCategory != null ? assetDetail.Adm.AssetCategory.Code : string.Empty,
		//                 AssetCategoryPrefix = assetDetail.Adm.AssetCategory != null ? assetDetail.Adm.AssetCategory.Prefix : string.Empty,
		//                 AssetType = assetDetail.Adm.AssetType != null ? assetDetail.Adm.AssetType.Name : string.Empty

		//             };

		//             invResult.Details.Add(inventoryResultDetail);
		//         }



		//         var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
		//         invResult.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
		//         invResult.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;
		//         invResult.RegionName = (region != null) ? (region.Code.Length > 0) ? region.Code + " - " : "" + region.Name : string.Empty;
		//         invResult.EndDate = DateTime.Now;
		//         invResult.LocationName = (location != null) ? location.Name : string.Empty;



		//         return invResult;
		//     }

		public AssetOperation GetAssetOperationData(int? locationId)
        {
            Model.Company transferPartialNumber = null;
            Model.Company transferLastPartialNumber = null;
            Model.Location selectedLocation= null;
            Model.AssetOp assetOp = null;
            int transferNewPartialNumber = 0;
            int transferNewFinalNumber = 0;

            selectedLocation = _context.Set<Model.Location>().Where(l => l.Id == locationId).FirstOrDefault();

            AssetOperation assetOperations = new AssetOperation
            {
                Operations = new List<AssetOperationDetail>()
            };

            var query =
                        from AO in _context.AssetOps

                        join asset in _context.Assets on AO.AssetId equals asset.Id
                        // join assetni in _context.AssetNis on asset.Id equals assetni.AssetId
                        join room in _context.Rooms on AO.RoomIdInitial equals room.Id
                        join location in _context.Locations on room.LocationId equals location.Id
                       
                        join costCenter in _context.CostCenters on AO.CostCenterIdInitial equals costCenter.Id
                      
                       
                        where  AO.RoomIdFinal != null && (AO.AssetOpStateId == 3 && ((AO.RoomIdInitial != AO.RoomIdFinal) || (AO.EmployeeIdInitial != AO.EmployeeIdFinal))  || (AO.AssetOpStateId == 3 && AO.InvStateIdFinal != 1))



            select new AssetOperationDetail
                        {
                            Description = asset.Name,
                            InvNo = asset.InvNo,
                            SerialNumber = asset.SerialNumber,
                            CostCenterCodeInitial = room.Code,
                            CostCenterNameInitial = room.Name,
                            LocationCodeInitial = AO.RoomInitial.Code,
                            LocationNameInitial = AO.RoomInitial.Name,
                            CostCenterCodeFinal= AO.RoomFinal.Code,
                            CostCenterNameFinal = AO.RoomFinal.Name,
                            LocationCodeFinal = AO.RoomFinal.Location.Code,
                            LocationNameFinal = AO.RoomFinal.Location.Name,
                            AssetOpId = AO.Id,
                            OperationDate = AO.ModifiedAt.Value
                           // TransferDocumentId = Decimal.ToInt32(AO.ValueAdd.Value)
                        };

            if (selectedLocation != null)
            {
                query = query.Where(a => a.LocationNameInitial == selectedLocation.Name || a.LocationNameFinal == selectedLocation.Name);
            }




            //var query1 = (from AO in _context.AssetOps
            //              where AO.RoomIdFinal != null && (AO.AssetOpStateId == 3 && AO.RoomIdInitial != AO.RoomIdFinal) || (AO.AssetOpStateId == 3 && AO.InvStateIdFinal != 1)
            //              group AO by new { AO.RoomIdInitial, AO.ValueAdd }
            //                into grp
            //              select new
            //              {
            //                  grp.Key.RoomIdInitial,
            //                  grp.Key.ValueAdd

            //              }).ToList();

            Console.WriteLine("Operatii: " + query.Count());

            assetOperations.Operations = query.ToList();

            

            assetOperations.Confirm = "DA";
            for (int i = 0; i < assetOperations.Operations.Count; i++)
            {
                transferLastPartialNumber = _context.Set<Model.Company>().OrderByDescending(ao => ao.Id).Take(1).SingleOrDefault();
                


                if (transferLastPartialNumber == null)
                {
                    transferPartialNumber = new Model.Company
                    {
                        Code = "1",
                        CreatedAt = DateTime.Now,
                        CreatedBy = string.Empty,
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = string.Empty,
                        Name = string.Empty
                    };

                    _context.Set<Model.Company>().Add(transferPartialNumber);

                }
                else
                {
                    transferNewPartialNumber = int.Parse(transferLastPartialNumber.Code);
                    transferNewPartialNumber++;

                    transferPartialNumber = new Model.Company
                    {
                        Code = transferNewPartialNumber.ToString(),
                        CreatedAt = DateTime.Now,
                        CreatedBy = string.Empty,
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = string.Empty,
                        Name = string.Empty
                    };

                    _context.Set<Model.Company>().Add(transferPartialNumber);
                }

                _context.SaveChanges();
            }

            assetOperations.DocumentTransferNumber = transferPartialNumber != null ? int.Parse(transferPartialNumber.Code) : 0;
            assetOperations.OperationsCount = assetOperations.Operations != null ? assetOperations.Operations.Count : 0;
            return assetOperations;

        }

        public InventoryListApaNova GetInventoryListApaNovaImages(int? inventoryId, int? locationId)
        {
            InventoryListApaNova invListApaNova = new InventoryListApaNova();
            Model.Inventory inventory = null;
            Model.Location location = null;
            Model.Room room = null;

            inventory = _context.Set<Model.Inventory>().Where(i => i.Id == inventoryId).FirstOrDefault();
            location = _context.Set<Model.Location>().Where(i => i.Id == locationId).FirstOrDefault();
            room = _context.Set<Model.Room>().Where(i => i.LocationId == locationId).FirstOrDefault();

            List<Model.InventoryListApn> items = _context.Set<Model.InventoryListApn>().FromSql("ApnReport {0}", inventoryId).ToList();

            invListApaNova.InventoryListDetailInternMod = new List<InventoryListDetailApaNova>();

            //var query =
            //            from IA in _context.InventoryAssets

            //            join asset in _context.Assets on IA.AssetId equals asset.Id
            //            join assetInv in _context.AssetInvs on asset.Id equals assetInv.AssetId
            //            join entityFile in _context.EntityFiles on asset.Id equals entityFile.EntityId
            //            join roomInitial in _context.Rooms on IA.RoomIdInitial equals roomInitial.Id
            //            join assetCategory in _context.AssetCategories on asset.AssetCategoryId equals assetCategory.Id
            //            join employee in _context.Employees on asset.EmployeeId equals employee.Id


            //            where IA.InventoryId == inventoryId && ((IA.RoomIdFinal == room.Id) || (IA.RoomIdInitial == room.Id && IA.RoomIdFinal == null))

            //            select new InventoryListDetailApaNova
            //            {

            //                InvNo = asset.ERPCode,
            //                InvNoNew = asset.InvNo,
            //                Description = asset.Name,
            //                DescriptionNew = asset.Name,
            //                SerialNumber = asset.SerialNumber,
            //                SerialNumberNew = IA.SerialNumber,
            //                PifDate = asset.PurchaseDate,
            //                InventoryDate = IA.ModifiedAt.Value,
            //                InventoryType = IA.Asset.AssetCategory.Name,
            //                AssetCategory = IA.Model,
            //                Model = IA.Producer,
            //                Dimension = "",
            //                UserName = IA.EmployeeFinal.InternalCode + " " + IA.EmployeeFinal.FirstName + " " + IA.EmployeeFinal.LastName,
            //                Qinitial = IA.QInitial,
            //                QFinal = IA.QFinal,
            //                Status = true,
            //                Info = IA.Info,
            //                LocationCodeInitial = roomInitial.Code,
            //                LocationNameInitial = roomInitial.Name,
            //                RoomIdFinal = IA.RoomIdFinal,
            //                RoomIdInitial = IA.RoomIdInitial,
            //                InfoNew = IA.Info,
            //                ImageLink = entityFile.StoredAs,
            //                ModifiedAt = IA.ModifiedAt.Value
            //            };

            foreach (InventoryListApn item in items)
            {
                invListApaNova.InventoryListDetailInternMod.Add(new InventoryListDetailApaNova()
                {
                    LocationNameInitial = item.LocationNameInitial,
                    LocationNameFinal = item.LocationNameFinal,
                    AssetCategory = item.AssetCategory,
                    InvNo = item.InvNo,
                    InvNoParent = item.InvNoParent,
                    Description = item.Description,
                    Qinitial = item.Qinitial,
                    QFinal = item.QFinal,
                    Um = item.Um,
                    CostCenterNameInitial = item.CostCenterNameInitial,
                    CostCenterNameFinal = item.CostCenterNameFinal,
                    AssetStateInitial = item.AssetStateInitial,
                    AssetStateFinal = item.AssetStateFinal,
                    UserEmployeeFullNameInitial = item.UserEmployeeFullNameInitial,
                    UserEmployeeInternalCodeInitial = item.UserEmployeeInternalCodeInitial,
                    UserEmployeeFullNameFinal = item.UserEmployeeFullNameFinal,
                    UserEmployeeInternalCodeFinal = item.UserEmployeeInternalCodeFinal,
                    StreetCodeInitial = item.StreetCodeInitial,
                    StreetNameInitial = item.StreetNameInitial,
                    StreetCodeFinal = item.StreetCodeFinal,
                    StreetNameFinal = item.StreetNameFinal,
                    RoomCodeInitial = item.RoomCodeInitial,
                    RoomNameInitial = item.RoomNameInitial,
                    RoomCodeFinal = item.RoomCodeFinal,
                    RoomNameFinal = item.RoomNameFinal,
                    SerialNumber = item.SerialNumber,
                    Info = item.Info,
                    GpsCoordinates = item.GpsCoordinates

                });
            }




            //var list = query.ToList();
            //foreach (var item in list)
            //{
            //    if (item.RoomIdFinal == null)
            //    {
            //        item.QFinal = 0;
            //        item.LocationCodeFinal = string.Empty;
            //        item.LocationNameFinal = string.Empty;
            //    }
            //    else
            //    {
            //        item.LocationCodeFinal = location.Code;
            //        item.LocationNameFinal = location.Name;
            //        if (item.QFinal == 0) item.QFinal = 1;
            //    }
            //}





           // invListApaNova.InventoryListDetailApaNova = list;

          //  var maxDate = invListApaNova.InventoryListDetailApaNova.Count > 0 ? invListApaNova.InventoryListDetailApaNova.Where(p => p.ModifiedAt != null && p.RoomIdFinal != null ? (p.RoomIdFinal == location.Id) : (p.RoomIdInitial == location.Id)).Max(p => p.ModifiedAt) : DateTime.Now;

            //invListApaNova.InventoryDate = maxDate.Value;

            //invListApaNova.LocationName = location.Code;

            //return invListApaNova;

            var maxDate = DateTime.Now;
            invListApaNova.InventoryDate = maxDate != null ? maxDate : DateTime.Now;

            invListApaNova.LocationNameInitial = location.Name;

            return invListApaNova;
        }

        public InventoryResult GetGeneralList(int? accMonthId, int inventoryId, int regionId, string reportType)
        {
            InventoryResult invResult = new InventoryResult();
            Model.AdmCenter admCenter = null;
            Model.Location location = null;
            Model.Region region = null;
            Model.Employee employee = null;
            Model.AccMonth accMonth = null;

            IQueryable<Model.InventoryAsset> invAssetQuery = null;
            IQueryable<Model.AssetAdmMD> assetAdmMD = null;
            IQueryable<Dto.AssetNiInvDet> assetNiQuery = null;

            List<Model.InventoryAsset> listInventoryAssets = null;
            List<Dto.AssetNiInvDet> listAssetNi = null;

            if(accMonthId.HasValue || accMonthId > 0)
            {
                accMonth = _context.Set<Model.AccMonth>().Where(a => a.Id == accMonthId).SingleOrDefault();
            }
            else
            {
                accMonth = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).SingleOrDefault();
            }


            region = (regionId > 0) ? _context.Set<Model.Region>().Single(r => r.Id == regionId) : null;

            invAssetQuery = _assetsRepository.GetInventoryAssetsQuery2(inventoryId, null);
            assetNiQuery = _assetNiRepository.GetAssetNiInvDetQuery(inventoryId);
            assetAdmMD = _assetsRepository.GetAssetAdmMDsQuery(inventoryId, accMonth.Id, null);


            if (region != null)
            {
                invAssetQuery = invAssetQuery.Where(i => i.RoomInitial.Location.Region.Id == region.Id && i.Asset.PurchaseDate <= accMonth.EndDate);
                assetAdmMD = assetAdmMD.Where(i => i.Room.Location.Region.Id == region.Id && i.Asset.PurchaseDate <= accMonth.EndDate);

            }

            if (invAssetQuery != null)
            {
                invAssetQuery = invAssetQuery.OrderBy(i => i.Asset.InvNo);
                listInventoryAssets = invAssetQuery.ToList();
            }
            else
            {
                listInventoryAssets = new List<Model.InventoryAsset>();
            }



            if (assetNiQuery != null)
            {
                assetNiQuery = assetNiQuery.OrderBy(a => a.Code1);

                listAssetNi = assetNiQuery.ToList();
            }
            else
            {
                listAssetNi = new List<Dto.AssetNiInvDet>();
            }


            invResult.Details = new List<InventoryResultDetail>();

            foreach (var inventoryAsset in listInventoryAssets)
            {
                InventoryResultDetail inventoryResultDetail = null;
                inventoryResultDetail = new InventoryResultDetail()
                {
                    InvNo = inventoryAsset.Asset.InvNo,
                    Description = inventoryAsset.Asset.Name,
                    SerialNumber = inventoryAsset.SerialNumber,
                    ERPCode = inventoryAsset.Asset.ERPCode,
                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,
                    CostCenter =
                        //(inventoryAsset.CostCenterIdFinal == null ? inventoryAsset.CostCenterInitial.Code : inventoryAsset.CostCenterFinal.Code),
                        (inventoryAsset.CostCenterIdFinal == null ? (inventoryAsset.CostCenterIdInitial != null ? inventoryAsset.CostCenterInitial.Name : string.Empty) : inventoryAsset.CostCenterFinal.Name),
                    Building = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Location.Name : inventoryAsset.RoomInitial.Location.Name,
                    BuildingName = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Location.Name : inventoryAsset.RoomInitial.Location.Name,
                    BuildingCode = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Location.Code : inventoryAsset.RoomInitial.Location.Code,

                    Room = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Name : inventoryAsset.RoomInitial.Name,
                    RoomCode = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Code : inventoryAsset.RoomInitial.Code,
                    RoomName = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Name : inventoryAsset.RoomInitial.Name,

                  //  Initial = inventoryAsset.RoomInitial.Location.Region == locationId ? 1 : 0,
                  //  Actual = ((inventoryAsset.RoomIdFinal != null) && (inventoryAsset.RoomFinal.LocationId == locationId)) ? inventoryAsset.QFinal : 0,  // BNR
                    // Actual = ((inventoryAsset.RoomIdFinal != null) && (inventoryAsset.RoomFinal.LocationId == locationId)) ? inventoryAsset.QFinal : 0,  // OTP
                    //Initial = inventoryAsset.QInitial,  // ALLIANTZ
                    //Actual = inventoryAsset.QFinal,  // ALLIANTZ

                    //Initial = 0,
                    //Actual = 0,

                    //Uom = inventoryAsset.Asset.Uom.Name,
                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,


                    //Value =  inventoryAsset.Asset.ValueInv,  // ALLIANTZ
                    Value = inventoryAsset.Asset.ValueRem,

                    ValueInv = inventoryAsset.Asset.ValueInv,
                    ValueDep = inventoryAsset.Asset.ValueRem,
                    ValueDepTotal = inventoryAsset.Asset.ValueRem,
                    Info = inventoryAsset.Info,
                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,

                    //InternalCode = inventoryAsset.EmployeeFinal != null
                    //    ? (inventoryAsset.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeFinal.InternalCode)
                    //    : (inventoryAsset.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeInitial.InternalCode),

                    FullName = inventoryAsset.EmployeeFinal != null
                        ? (inventoryAsset.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeFinal.FirstName + " " + inventoryAsset.EmployeeFinal.LastName)
                        : (inventoryAsset.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeInitial.FirstName + " " + inventoryAsset.EmployeeInitial.LastName),

                    // FullName = inventoryAsset.EmployeeFinal.FirstName, // ALLIANTZ
                    //FullName = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.FirstName : inventoryAsset.EmployeeInitial.FirstName, // ALLIANTZ

                    AssetCategory = inventoryAsset.Asset.AssetCategoryId != null ? inventoryAsset.Asset.AssetCategory.Code : string.Empty,
                    AssetCategoryPrefix = inventoryAsset.Asset.AssetCategoryId != null ? inventoryAsset.Asset.AssetCategory.Prefix : string.Empty,
                    AssetType = inventoryAsset.Asset.AssetTypeId != null ? inventoryAsset.Asset.AssetType.Name : string.Empty

                };

                invResult.Details.Add(inventoryResultDetail);
            }

            foreach (var assetNi in listAssetNi)
            {
                invResult.Details.Add(new InventoryResultDetail()
                {
                    InvNo = assetNi.Code1,
                    Description = assetNi.Name1,
                    SerialNumber = assetNi.SerialNumber,
                    PurchaseDate = null,
                    CostCenter = assetNi.CostCenterCode,
                    Building = assetNi.LocationName,
                    BuildingCode = assetNi.LocationCode,
                    BuildingName = assetNi.LocationName,
                    Room = assetNi.RoomName,
                    RoomCode = assetNi.RoomCode,
                    RoomName = assetNi.RoomName,
                    Initial = 0,
                    Actual = assetNi.Quantity,
                    Uom = "BUC",
                    Value = 0,
                    ValueInv = 0,
                    ValueDep = 0,
                    ValueDepTotal = 0,
                    Info = assetNi.Info,
                    Custody = assetNi.Custody.HasValue ? (assetNi.Custody.Value ? "DA" : "NU") : string.Empty,
                    InternalCode = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.InternalCode,
                    FullName = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.FirstName + " " + assetNi.LastName



                });
            }   // BNR + ALLIANTZ 

            var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            invResult.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            invResult.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;
            invResult.RegionName = (region != null) ? (region.Code.Length > 0) ? region.Code + " - " + region.Name : "" + region.Name : string.Empty;
            invResult.EndDate = DateTime.Now;
            invResult.LocationName = (location != null) ? location.Name : string.Empty;



            return invResult;
        }

        public InventoryResult GetInventoryListV2Total(int inventoryId)
        {
            InventoryResult invResult = new InventoryResult();
            Model.AdmCenter admCenter = null;
            Model.Location location = null;
            Model.Region region = null;
            Model.Employee employee = null;

            IQueryable<Model.InventoryAsset> invAssetQuery = null;
            IQueryable<Dto.AssetNiInvDet> assetNiQuery = null;

            List<Model.InventoryAsset> listInventoryAssets = null;
            List<Dto.AssetNiInvDet> listAssetNi = null;

           

            invAssetQuery = _assetsRepository.GetInventoryAssetsQuery2(inventoryId, null);
            assetNiQuery = _assetNiRepository.GetAssetNiInvDetQuery(inventoryId);


            //invAssetQuery = invAssetQuery.Where(i => i.RegionIdIni == regionId || i.RegionIdFin == regionId);
            //assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);


           
            

            if (invAssetQuery != null)
            {
                invAssetQuery = invAssetQuery.OrderBy(i => i.Asset.InvNo);
                listInventoryAssets = invAssetQuery.ToList();
            }
            else
            {
                listInventoryAssets = new List<Model.InventoryAsset>();
            }

            if (assetNiQuery != null)
            {
                assetNiQuery = assetNiQuery.OrderBy(a => a.Code1);

                listAssetNi = assetNiQuery.ToList();
            }
            else
            {
                listAssetNi = new List<Dto.AssetNiInvDet>();
            }

            //listInventoryAssets = (invAssetQuery != null) ? invAssetQuery.ToList() : new List<InventoryAsset>();
            //listAssetNi = (assetNiQuery != null) ? assetNiQuery.ToList() : new List<Dto.AssetNiInvDet>();

            invResult.Details = new List<InventoryResultDetail>();

            foreach (var inventoryAsset in listInventoryAssets)
            {
                InventoryResultDetail inventoryResultDetail = null;
                inventoryResultDetail = new InventoryResultDetail()
                {
                    InvNo = inventoryAsset.Asset.InvNo,
                    Description = inventoryAsset.Asset.Name,
                    SerialNumber = inventoryAsset.SerialNumber,
                    ERPCode = inventoryAsset.Asset.ERPCode,
                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,



                    CostCenter =
                        //(inventoryAsset.CostCenterIdFinal == null ? inventoryAsset.CostCenterInitial.Code : inventoryAsset.CostCenterFinal.Code),
                        (inventoryAsset.CostCenterIdFinal == null ? (inventoryAsset.CostCenterIdInitial != null ? inventoryAsset.CostCenterInitial.Name : string.Empty) : inventoryAsset.CostCenterFinal.Name),

                    Building = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Location.Name : inventoryAsset.RoomInitial.Location.Name,
                    BuildingName = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Location.Name : inventoryAsset.RoomInitial.Location.Name,
                    BuildingCode = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Location.Code : inventoryAsset.RoomInitial.Location.Code,

                    Room = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Name : inventoryAsset.RoomInitial.Name,
                    RoomName = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Name : inventoryAsset.RoomInitial.Name,
                    RoomCode = inventoryAsset.RoomIdFinal != null ? inventoryAsset.RoomFinal.Code : inventoryAsset.RoomInitial.Code,

                    //Initial = inventoryAsset.RoomInitial.LocationId == locationId ? inventoryAsset.QInitial : 0,
                    //Actual = ((inventoryAsset.RoomIdFinal != null) && (inventoryAsset.RoomFinal.LocationId == locationId)) ? inventoryAsset.QFinal : 0,
                    Initial = 0,
                    Actual = 0,

                    //Uom = inventoryAsset.Asset.Uom.Name,
                    Uom = inventoryAsset.Asset.UomId != null ? inventoryAsset.Asset.Uom.Name : string.Empty,
                    Value = inventoryAsset.Asset.ValueInv,
                    ValueInv = inventoryAsset.Asset.ValueInv,
                    ValueDep = inventoryAsset.Asset.ValueRem,
                    ValueDepTotal = inventoryAsset.Asset.ValueRem,
                    Info = inventoryAsset.Info,
                    Custody = inventoryAsset.Asset.Custody.HasValue ? (inventoryAsset.Asset.Custody.Value ? "DA" : "NU") : string.Empty,

                    InternalCode = inventoryAsset.EmployeeFinal != null
                        ? (inventoryAsset.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeFinal.InternalCode)
                        : (inventoryAsset.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeInitial.InternalCode),

                    FullName = inventoryAsset.EmployeeFinal != null
                        ? (inventoryAsset.EmployeeFinal.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeFinal.FirstName + " " + inventoryAsset.EmployeeFinal.LastName)
                        : (inventoryAsset.EmployeeInitial.InternalCode.Contains("_") ? string.Empty : inventoryAsset.EmployeeInitial.FirstName + " " + inventoryAsset.EmployeeInitial.LastName),

                    //  FullName = inventoryAsset.EmployeeFinal != null ? inventoryAsset.EmployeeFinal.FirstName : "", // ALLIANTZ

                    AssetCategory = inventoryAsset.Asset.AssetCategoryId != null ? inventoryAsset.Asset.AssetCategory.Code : string.Empty,
                    AssetCategoryPrefix = inventoryAsset.Asset.AssetCategoryId != null ? inventoryAsset.Asset.AssetCategory.Prefix : string.Empty,
                    AssetType = inventoryAsset.Asset.AssetTypeId != null ? inventoryAsset.Asset.AssetType.Name : string.Empty

                };

               

                bool skip = false;

            

                if (!skip) invResult.Details.Add(inventoryResultDetail);
            }

            foreach (var assetNi in listAssetNi)
            {
                invResult.Details.Add(new InventoryResultDetail()
                {
                    InvNo = assetNi.Code1,
                    Description = assetNi.Name1,
                    SerialNumber = assetNi.SerialNumber,
                    PurchaseDate = null,
                    CostCenter = assetNi.CostCenterCode,
                    Building = assetNi.LocationName,
                    BuildingCode = assetNi.LocationCode,
                    BuildingName = assetNi.LocationName,
                    Room = assetNi.RoomName,
                    RoomCode = assetNi.RoomCode,
                    RoomName = assetNi.RoomName,
                    Initial = 0,
                    Actual = assetNi.Quantity,
                    Uom = "BUC",
                    Value = 0,
                    ValueInv = 0,
                    ValueDep = 0,
                    ValueDepTotal = 0,
                    Info = assetNi.Info,
                    Custody = assetNi.Custody.HasValue ? (assetNi.Custody.Value ? "DA" : "NU") : string.Empty,
                    InternalCode = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.InternalCode,
                    FullName = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.FirstName + " " + assetNi.LastName



                });
            }   // BNR + ALLIANTZ 

            var companyConfigValue = _context.Set<Model.ConfigValue>().Where(c => c.Group == "COMPANY" && c.Code == "NAME").FirstOrDefault();
            invResult.CompanyName = companyConfigValue != null ? companyConfigValue.TextValue : string.Empty;
            invResult.AdmCenterName = (admCenter != null) ? admCenter.Name : string.Empty;
            invResult.RegionName = (region != null) ? (region.Code.Length > 0) ? region.Code + " - " : "" + region.Name : string.Empty;
            invResult.EndDate = DateTime.Now;
            invResult.LocationName = (location != null) ? location.Name : string.Empty;

            //var package = new ExcelPackage();

            //Dictionary<string, int> locationIndexes = new Dictionary<string, int>();
            //int recordIndex = 0;


            //foreach (var item in invResult.Details)
            //{
            //    string sheetName = item.BuildingCode;

            //    ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];

            //    if (worksheet == null)
            //    {
            //        worksheet = package.Workbook.Worksheets.Add(sheetName);
            //        recordIndex = 2;
            //        locationIndexes.Add(sheetName, recordIndex);

            //        //First add the headers
            //        worksheet.Cells[1, 1].Value = "OptimaId";
            //        worksheet.Cells[1, 2].Value = "Numar inventar plecare";
            //        worksheet.Cells[1, 3].Value = "Denumire";
            //        worksheet.Cells[1, 4].Value = "Centru de cost plecare";
            //        worksheet.Cells[1, 5].Value = "Cladire plecare";
            //        worksheet.Cells[1, 6].Value = "Centru de cost destinatie";
            //        worksheet.Cells[1, 7].Value = "Cladire destinatie";
            //        worksheet.Cells[1, 8].Value = "Confirmat";
            //        worksheet.Cells[1, 9].Value = "Numar inventar primit";
            //        worksheet.Cells[1, 10].Value = "Observatii";
            //        worksheet.Cells[1, 11].Value = "Instructiuni";
            //        worksheet.Cells[12, 11].Value = "Primite in plus";
            //        worksheet.Cells[13, 11].Value = "Denumire produs";
            //        worksheet.Cells[13, 14].Value = "Descriere obiect";
            //        worksheet.Cells[13, 17].Value = "Numar inventar";


            //        worksheet.Cells[15, 4].Value = "Fixed Assets & Other Inventories Transfer Form";
            //        worksheet.Cells[16, 2].Value = "Transferred :";
            //        worksheet.Cells[17, 3].Value = "FROM :";
            //        worksheet.Cells[17, 4].Value = "";
            //        worksheet.Cells[18, 3].Value = "TO :";
            //        worksheet.Cells[18, 4].Value = "";



            //    }
            //    else
            //    {
            //        recordIndex = locationIndexes[sheetName];
            //        recordIndex--;
            //    }
            //}

            //using (FileStream aFile = new FileStream(@"C:\Temp\asdf.xlsx", FileMode.Create))
            //{
            //    aFile.Seek(0, SeekOrigin.Begin);
            //    package.SaveAs(aFile);
         //   }

            return invResult;
        }

		//public InventoryResult GetInventoryListV2ByFilters(int inventoryId, int regionId, int? locationId, string reportType, bool? custody)
		//{
		//    InventoryResult invResult = new InventoryResult();
		//    Model.Location location = null;
		//    Model.Region region = null;

		//    IQueryable<Dto.InventoryAsset> invAssetQuery = null;
		//    IQueryable<Dto.AssetNiInvDet> assetNiQuery = null;

		//    List<Dto.InventoryAsset> listInventoryAssets = null;
		//    List<Dto.AssetNiInvDet> listAssetNi = null;

		//    location = (locationId.HasValue) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
		//    region = _context.Set<Model.Region>().Single(r => r.Id == regionId);

		//    invAssetQuery = _assetsRepository.GetInventoryAssetsQuery(inventoryId);
		//    assetNiQuery = _assetNiRepository.GetAssetNiInvDetQuery(inventoryId);

		//    invAssetQuery = invAssetQuery.Where(i => i.RegionIdIni == regionId || i.RegionIdFin == regionId);
		//    assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);

		//    if (locationId.HasValue)
		//    {
		//        invAssetQuery = invAssetQuery.Where(i => i.LocationIdIni == locationId || i.LocationIdFin == locationId);
		//        assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
		//    }

		//    if (custody.HasValue)
		//    {
		//        invAssetQuery = invAssetQuery.Where(i => i.Custody == custody.GetValueOrDefault());
		//        assetNiQuery = assetNiQuery.Where(a => a.Custody == custody.GetValueOrDefault());
		//    }

		//    if (reportType != null)
		//    {
		//        switch (reportType.ToUpper())
		//        {
		//            case "MINUS":

		//                if (locationId != null)
		//                {
		//                    invAssetQuery = invAssetQuery.Where(i => (i.LocationIdFin == null) || (i.LocationIdFin != locationId));
		//                }
		//                else
		//                {
		//                    invAssetQuery = invAssetQuery.Where(i => (i.RegionIdFin == null) || (i.RegionIdFin != regionId));
		//                }

		//                assetNiQuery = null;

		//                invResult.InventoryListType = "Minusuri";

		//                break;

		//            case "PLUS":

		//                if ((locationId != null))
		//                {
		//                    invAssetQuery = invAssetQuery.Where(i => (i.LocationIdFin != null) && (i.LocationIdIni != i.LocationIdFin) && (i.LocationIdFin == locationId));
		//                }
		//                else
		//                {
		//                    invAssetQuery = invAssetQuery.Where(i => (i.RegionIdFin != null) && (i.RegionIdIni != i.RegionIdFin) && (i.RegionIdFin == regionId));
		//                }

		//                invResult.InventoryListType = "Plusuri";
		//                //invAssetQuery = invAssetQuery.Where(i => (locationId != null) ? 
		//                //((i.LocationIdFin != null) && (i.LocationIdIni != i.LocationIdFin)) : 
		//                //((i.RegionIdFin != null) && (i.RegionIdIni != i.RegionIdFin)));

		//                break;

		//            default:
		//                invResult.InventoryListType = string.Empty;
		//                break;
		//        }
		//    }

		//    listInventoryAssets = invAssetQuery.ToList();
		//    listAssetNi = (assetNiQuery != null) ? assetNiQuery.ToList() : new List<Dto.AssetNiInvDet>();

		//    invResult.Details = new List<InventoryResultDetail>();

		//    foreach (var inventoryAsset in listInventoryAssets)
		//    {
		//        invResult.Details.Add(new InventoryResultDetail()
		//        {
		//            InvNo = inventoryAsset.InvNo,
		//            Description = inventoryAsset.Name,
		//            SerialNumber = inventoryAsset.SerialNumber,
		//            PurchaseDate = inventoryAsset.PurchaseDate,
		//            //CostCenter = inventoryAsset.CostCenterNameIni,
		//            //Room = (inventoryAsset.RoomNameFin != null && inventoryAsset.RoomNameFin != string.Empty) ? inventoryAsset.RoomNameFin : inventoryAsset.RoomNameIni,

		//            //CostCenter = inventoryAsset.CostCenterNameIni,
		//            CostCenter = (locationId != null) 
		//            ? (inventoryAsset.LocationIdIni == locationId ? inventoryAsset.CostCenterCodeIni : inventoryAsset.CostCenterCodeFin)
		//            : (inventoryAsset.RegionIdIni == regionId ? inventoryAsset.CostCenterCodeIni : inventoryAsset.CostCenterCodeFin),
		//            //Room = (inventoryAsset.RoomNameFin != null && inventoryAsset.RoomNameFin != string.Empty) ? inventoryAsset.RoomNameFin : inventoryAsset.RoomNameIni,

		//            Building = (locationId != null)
		//            ? (inventoryAsset.LocationIdIni == locationId ? inventoryAsset.LocationNameIni : inventoryAsset.LocationNameFin)
		//            : (inventoryAsset.RegionIdIni == regionId ? inventoryAsset.LocationNameIni : inventoryAsset.LocationNameFin),

		//            Room = (locationId != null) 
		//            ? (inventoryAsset.LocationIdIni == locationId ? ((inventoryAsset.LocationIdIni != inventoryAsset.LocationIdFin) ? inventoryAsset.RoomNameIni : inventoryAsset.RoomNameFin) : inventoryAsset.RoomNameFin)
		//            : (inventoryAsset.RegionIdIni == regionId ? ((inventoryAsset.RegionIdIni != inventoryAsset.RegionIdFin) ? inventoryAsset.RoomNameIni : inventoryAsset.RoomNameFin) : inventoryAsset.RoomNameFin),

		//            Initial = (locationId != null) ? (inventoryAsset.LocationIdIni == locationId ? inventoryAsset.QIntial : 0) : (inventoryAsset.RegionIdIni == regionId) ? inventoryAsset.QIntial : 0,
		//            Actual = (locationId != null) ? (inventoryAsset.LocationIdFin == locationId ? inventoryAsset.QFinal : 0) : (inventoryAsset.RegionIdFin == regionId) ? inventoryAsset.QFinal : 0,
		//            Uom = inventoryAsset.Uom,
		//            Value = inventoryAsset.ValueInv,
		//            ValueInv = inventoryAsset.ValueInv,
		//            ValueDep = inventoryAsset.ValueDep,
		//            ValueDepTotal = inventoryAsset.ValueDep,
		//            Info = string.Empty,
		//            Custody = inventoryAsset.Custody.HasValue ? (inventoryAsset.Custody.Value ? "DA" : "NU") : string.Empty,
		//            InternalCode = (inventoryAsset.InternalCodeFin.ToUpper().Contains("_")) ?
		//                                    string.Empty :
		//                                    (inventoryAsset.InternalCodeFin != null && inventoryAsset.InternalCodeFin != string.Empty) ? inventoryAsset.InternalCodeFin : inventoryAsset.InternalCodeIni,
		//            FullName = (inventoryAsset.InternalCodeFin.ToUpper().Contains("_")) ?
		//                                    string.Empty : (inventoryAsset.LastNameFin != null && inventoryAsset.LastNameFin != string.Empty) ?
		//                                    inventoryAsset.FirstNameFin + " " + inventoryAsset.LastNameFin : inventoryAsset.FirstNameIni + " " + inventoryAsset.LastNameIni
		//        });
		//    }

		//    foreach (var assetNi in listAssetNi)
		//    {
		//        invResult.Details.Add(new InventoryResultDetail()
		//        {
		//            InvNo = assetNi.Code1,
		//            Description = assetNi.Name1,
		//            SerialNumber = assetNi.SerialNumber,
		//            PurchaseDate = null,
		//            CostCenter = assetNi.CostCenterCode,
		//            Room = assetNi.RoomName,
		//            Initial = 0,
		//            Actual = assetNi.Quantity,
		//            Uom = "BUC",
		//            Value = 0,
		//            ValueInv = 0,
		//            ValueDep = 0,
		//            ValueDepTotal = 0,
		//            Info = assetNi.Info,
		//            Custody = assetNi.Custody.HasValue ? (assetNi.Custody.Value ? "DA" : "NU") : string.Empty,
		//            InternalCode = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.InternalCode,
		//            FullName = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.FirstName + " " + assetNi.LastName
		//        });
		//    }

		//    invResult.AdmCenterName = region.Name;
		//    invResult.CompanyName = "";
		//    invResult.CompanyName = "";
		//    invResult.EndDate = DateTime.Now;
		//    invResult.LocationName = (location != null) ? location.Name : string.Empty;

		//    return invResult;
		//}

		//public TransferOutV1Result GetTransferOutV1ByFilters(int inventoryId, int regionId, int? locationId)
		//{
		//    TransferOutV1Result transferOutV1Result = null;
		//    Model.Location location = null;
		//    Model.Region region = null;

		//    IQueryable<Dto.InventoryAsset> invAssetQuery = null;
		//    IQueryable<Dto.AssetNiInvDet> assetNiQuery = null;

		//    List<Dto.InventoryAsset> listInventoryAssets = null;
		//    List<Dto.AssetNiInvDet> listAssetNi = null;

		//    Model.InvState outInvState = _context.Set<Model.InvState>().Single(s => s.Code == "OUT");
		//    location = (locationId.HasValue) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
		//    region = region = _context.Set<Model.Region>().Single(r => r.Id == regionId);

		//    invAssetQuery = _assetsRepository.GetInventoryAssetsQuery(inventoryId);
		//    assetNiQuery = _assetNiRepository.GetAssetNiInvDetQuery(inventoryId);

		//    invAssetQuery = invAssetQuery.Where(i => i.RegionIdIni == regionId || i.RegionIdFin == regionId);
		//    assetNiQuery = assetNiQuery.Where(a => a.RegionId == regionId);

		//    if (locationId.HasValue)
		//    {
		//        invAssetQuery = invAssetQuery.Where(i => i.LocationIdIni == locationId || i.LocationIdFin == locationId);
		//        assetNiQuery = assetNiQuery.Where(a => a.LocationId == locationId);
		//    }

		//    invAssetQuery = invAssetQuery.Where(i => i.InvStateIdFin == outInvState.Id);
		//    assetNiQuery = assetNiQuery.Where(a => a.InvStateId == outInvState.Id);

		//    listInventoryAssets = invAssetQuery.ToList();
		//    listAssetNi = (assetNiQuery != null) ? assetNiQuery.ToList() : new List<Dto.AssetNiInvDet>();

		//    transferOutV1Result = new TransferOutV1Result();

		//    transferOutV1Result.Details = new List<TransferOutV1Detail>();

		//    foreach (var inventoryAsset in listInventoryAssets)
		//    {
		//        transferOutV1Result.Details.Add(new TransferOutV1Detail()
		//        {
		//            InvNo = inventoryAsset.InvNo,
		//            Description = inventoryAsset.Name,
		//            PurchaseDate = inventoryAsset.PurchaseDate,
		//            AdmCenter = inventoryAsset.AdmCenterNameFin,
		//            LocationName = inventoryAsset.LocationNameFin,
		//            CostCenter = inventoryAsset.CostCenterNameIni,
		//            Room = (inventoryAsset.RoomNameFin != null && inventoryAsset.RoomNameFin != string.Empty) ? inventoryAsset.RoomNameFin : inventoryAsset.RoomNameIni,
		//            Value = inventoryAsset.ValueInv,
		//            ValueDep = inventoryAsset.ValueDep,
		//            EmployeeCode = (inventoryAsset.InternalCodeFin.ToUpper().Contains("_")) ? string.Empty : inventoryAsset.InternalCodeFin,
		//            EmployeeFirstName = (inventoryAsset.InternalCodeFin.ToUpper().Contains("_")) ? string.Empty : inventoryAsset.FirstNameFin,
		//            EmployeeLastName = (inventoryAsset.InternalCodeFin.ToUpper().Contains("_")) ? string.Empty : inventoryAsset.LastNameFin,
		//            Info = string.Empty
		//        });
		//    }

		//    foreach (var assetNi in listAssetNi)
		//    {
		//        transferOutV1Result.Details.Add(new TransferOutV1Detail()
		//        {
		//            InvNo = assetNi.Code1,
		//            Description = assetNi.Name1,
		//            PurchaseDate = null,
		//            AdmCenter = assetNi.AdmCenterName,
		//            LocationName = assetNi.LocationName,
		//            CostCenter = assetNi.CostCenterCode,
		//            Room = assetNi.RoomName,
		//            Value = 0,
		//            ValueDep = 0,
		//            EmployeeCode = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.InternalCode,
		//            EmployeeFirstName = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.FirstName,
		//            EmployeeLastName = assetNi.InternalCode.ToUpper().Contains("_") ? string.Empty : assetNi.LastName,
		//            Info = assetNi.Info
		//        });
		//    }

		//    transferOutV1Result.AdmCenterName = region.Name;
		//    transferOutV1Result.CompanyName = "";
		//    transferOutV1Result.EndDate = DateTime.Now;
		//    transferOutV1Result.LocationName = (location != null) ? location.Name : string.Empty;

		//    return transferOutV1Result;
		//}

		//public TransferInV1Result GetTransferInV1ByFilters(int inventoryId, int regionId, int? locationId, string reportType)
		//{
		//    TransferInV1Result transferInV1Result = null;
		//    Model.Location location = null;
		//    Model.Region region = null;

		//    IQueryable<Dto.InventoryAsset> invAssetQuery = null;
		//    List<Dto.InventoryAsset> listInventoryAssets = null;

		//    location = (locationId.HasValue) ? _context.Set<Model.Location>().Single(l => l.Id == locationId) : null;
		//    region = region = _context.Set<Model.Region>().Single(r => r.Id == regionId);

		//    invAssetQuery = _assetsRepository.GetInventoryAssetsQuery(inventoryId);

		//    invAssetQuery = invAssetQuery.Where(i => i.RegionIdIni == regionId || i.RegionIdFin == regionId);

		//    if (locationId.HasValue) invAssetQuery = invAssetQuery.Where(i => i.LocationIdIni == locationId || i.LocationIdFin == locationId);

		//    if (reportType != null)
		//    {
		//        switch (reportType.ToUpper())
		//        {
		//            case "TRANSFER_ROOM_SAME_LOCATION":
		//                invAssetQuery = invAssetQuery.Where(i => ((i.RoomIdFin != null) && (i.RoomIdIni != i.RoomIdFin) && (i.LocationIdIni == i.LocationIdFin)));
		//                break;
		//            case "TRANSFER_LOCATION_SAME_ADMCENTER":
		//                invAssetQuery = invAssetQuery.Where(i => ((i.LocationIdFin != null) && (i.LocationIdIni != i.LocationIdFin) && (i.AdmCenterIdIni == i.AdmCenterIdFin)));
		//                break;
		//            case "TRANSFER_LOCATION_DIFF_ADMCENTER":
		//                invAssetQuery = invAssetQuery.Where(i => ((i.LocationIdFin != null) && (i.LocationIdIni != i.LocationIdFin) && (i.AdmCenterIdIni != i.AdmCenterIdFin)));
		//                break;
		//        }
		//    }

		//    listInventoryAssets = invAssetQuery.ToList();

		//    transferInV1Result = new TransferInV1Result();

		//    transferInV1Result.Details = new List<TransferInV1Detail>();

		//    foreach (var inventoryAsset in listInventoryAssets)
		//    {
		//        transferInV1Result.Details.Add(new TransferInV1Detail()
		//        {
		//            InvNo = inventoryAsset.InvNo,
		//            Description = inventoryAsset.Name,
		//            SerialNumber = inventoryAsset.SerialNumber,
		//            PurchaseDate = inventoryAsset.PurchaseDate,
		//            AdmCenterInitial = inventoryAsset.AdmCenterNameIni,
		//            AdmCenterFinal = inventoryAsset.AdmCenterIdFin.HasValue ? inventoryAsset.AdmCenterNameFin : string.Empty,
		//            LocationNameInitial = inventoryAsset.LocationNameIni,
		//            LocationNameFinal = inventoryAsset.LocationIdFin.HasValue ? inventoryAsset.LocationNameFin : string.Empty,
		//            CostCenterInitial = inventoryAsset.CostCenterNameIni,
		//            CostCenterFinal = inventoryAsset.CostCenterIdIni.HasValue ? inventoryAsset.CostCenterNameIni : string.Empty,
		//            RoomInitial = inventoryAsset.RoomNameIni,
		//            RoomFinal = inventoryAsset.RoomIdFin.HasValue ? inventoryAsset.RoomNameFin : string.Empty,
		//            InternalCodeInitial = inventoryAsset.InternalCodeIni,
		//            InternalCodeFinal = inventoryAsset.InternalCodeFin != null ? inventoryAsset.InternalCodeFin : string.Empty,
		//            FullNameInitial = inventoryAsset.FirstNameIni + " " + inventoryAsset.LastNameIni,
		//            FullNameFinal = inventoryAsset.InternalCodeFin != null ? inventoryAsset.FirstNameFin + " " + inventoryAsset.LastNameFin : string.Empty,
		//            Initial = inventoryAsset.QIntial,
		//            Actual = inventoryAsset.QFinal,
		//            Uom = inventoryAsset.Uom,
		//            Value = inventoryAsset.ValueInv,
		//            ValueDepTotal = inventoryAsset.ValueDep,
		//            Custody = inventoryAsset.Custody.HasValue ? (inventoryAsset.Custody.Value ? "DA" : "NU") : string.Empty,
		//        });
		//    }

		//    transferInV1Result.AdmCenterName = region.Name;
		//    transferInV1Result.CompanyName = "";
		//    transferInV1Result.EndDate = DateTime.Now;
		//    transferInV1Result.LocationName = (location != null) ? location.Name : string.Empty;

		//    return transferInV1Result;
		//}

		//public InventoryResult GetInventoryListByFilters(int inventoryId, int locationId, int? employeeId, bool isTransfer, bool isCassation)
		//{
		//    InventoryResult invResult = null;

		//    IQueryable<Dto.InventoryAsset> invAssetQuery = null;
		//    Model.InvState outInvState = null;

		//    outInvState = _context.Set<Model.InvState>().Where(ois => ois.Code.Equals("OUT")).SingleOrDefault();

		//    invAssetQuery = _assetsRepository.GetInventoryAssetsQuery(inventoryId);

		//    invAssetQuery = invAssetQuery.Where(i => i.LocationIdIni == locationId || i.LocationIdFin == locationId);

		//    if (employeeId.HasValue) invAssetQuery = invAssetQuery.Where(e => e.EmployeeIdIni == employeeId || e.EmployeeIdFin == employeeId);

		//    if (isTransfer) invAssetQuery = invAssetQuery.Where(t => t.LocationIdIni != t.LocationIdFin || );

		//    if (isCassation) invAssetQuery = invAssetQuery.Where(c => c.InvDetailStateId == outInvState.Id);

		//    invResult = new InventoryResult();

		//    invResult.AdmCenterName = _context.Set<Model.AdmCenter>().W






		//}

		//public InventoryResult GetInventoryListByFilters(int? inventoryId, int? locationId, int? employeeId, bool isTransfer, bool isCassation)
		//{
		//    InventoryResult invList = null;
		//    InventoryResultDetail invDetail = null;
		//    List<InventoryResultDetail> invDetailList = null;

		//    IQueryable<Model.InventoryAsset> invAssets = _context.Set<Model.InventoryAsset>().Include("Asset")
		//                                                                                    .Include("Asset.CostCenter")
		//                                                                                    .Include("Asset.AssetCategory")
		//                                                                                    .Include("RoomInitial")
		//                                                                                    .Include("RoomInitial.Location.AdmCenter")
		//                                                                                    .Include("RoomFinal")
		//                                                                                    .Include("RoomFinal.Location.AdmCenter")
		//                                                                                    .Include("EmployeeInitial")
		//                                                                                    .Include("EmployeeFinal")
		//                                                                                    .Include("Asset.Uom");


		//    Model.InvState outInvState = _context.Set<Model.InvState>().Where(ois => ois.Code.Equals("OUT")).SingleOrDefault();

		//    Model.Location currLocation = _context.Set<Model.Location>().Include("AdmCenter").Where(l => l.Id == locationId).SingleOrDefault();

		//    if (inventoryId.HasValue) invAssets = invAssets.Where(ia => ia.InventoryId == inventoryId);

		//    //if (locationId.HasValue) invAssets = invAssets.Where(ia => ia.RoomFinal.LocationId == locationId);
		//    if (locationId.HasValue) invAssets = invAssets.Where(ia => ((ia.RoomInitial.LocationId == locationId) || (ia.RoomFinal.LocationId == locationId)) );

		//    //if (employeeId.HasValue) invAssets = invAssets.Where(ia => ia.EmployeeIdFinal == employeeId);
		//    if (employeeId.HasValue) invAssets = invAssets.Where(ia => ((ia.EmployeeIdInitial == employeeId) || (ia.EmployeeIdFinal == employeeId)));

		//    if (isTransfer) invAssets = invAssets.Where(ia => ia.RoomInitial.LocationId != ia.RoomFinal.LocationId);

		//    if (isCassation) invAssets = invAssets.Where(ia => ia.StateIdFinal == outInvState.Id);

		//    invList = new InventoryResult();

		//    invList.AdministrationCode = "Administration code";
		//    invList.AdministrationName = "Administration name";
		//    invList.CompanyAdress = "Address";
		//    invList.CompanyName = "";
		//    invList.CompanyRegistryNumber = "Company registration number";
		//    invList.CompanyUniqueID = "CUI";
		//    invList.EndDate = DateTime.Now;
		//    invList.AdmCenterName = currLocation.AdmCenter.Name;
		//    invList.LocationCode = currLocation.Code;
		//    invList.LocationName = currLocation.Name;
		//    invList.Provider = "Provider";

		//    invDetailList = new List<InventoryResultDetail>();

		//    //foreach (var invAsset in invAssets)
		//    foreach (var invAsset in invAssets.ToList())
		//    {
		//        invDetail = new InventoryResultDetail();

		//        invDetail.InvNo = invAsset.Asset.InvNo;
		//        invDetail.InvNoOld = "";
		//        invDetail.Description = invAsset.Asset.Name;

		//        invDetail.InitialSerialNumber = (invAsset.Asset.SerialNumber != null) ? invAsset.Asset.SerialNumber : "";
		//        invDetail.SerialNumber = (invAsset.SerialNumber != null) ? invAsset.SerialNumber : "";

		//        invDetail.InitialCostCenterCode = "";
		//        invDetail.InitialCostCenterName = "";

		//        invDetail.FinalCostCenterCode = (invAsset.Asset.CostCenter != null) ? invAsset.Asset.CostCenter.Code : "N/A";
		//        invDetail.FinalCostCenterName = (invAsset.Asset.CostCenter != null) ? invAsset.Asset.CostCenter.Name : "N/A";

		//        invDetail.InitialLocationId = invAsset.RoomInitial.Location.Id;
		//        invDetail.InitialLocationCode = invAsset.RoomInitial.Location.AdmCenter.Name;
		//        invDetail.InitialLocationName = invAsset.RoomInitial.Location.Name;

		//        //invDetail.FinalLocationId = invAsset.RoomFinal.Location.Id;
		//        //invDetail.FinalLocationCode = invAsset.RoomFinal.Location.AdmCenter.Name;
		//        //invDetail.FinalLocationName = invAsset.RoomFinal.Location.Name;
		//        invDetail.FinalLocationId = invAsset.RoomFinal != null ? invAsset.RoomFinal.Location.Id : 0;
		//        invDetail.FinalLocationCode = invAsset.RoomFinal != null ? invAsset.RoomFinal.Location.AdmCenter.Name : string.Empty;
		//        invDetail.FinalLocationName = invAsset.RoomFinal != null ? invAsset.RoomFinal.Location.Name : string.Empty;

		//        invDetail.InitialRoomCode = invAsset.RoomInitial.Code;
		//        invDetail.InitialRoomName = invAsset.RoomInitial.Name;

		//        //invDetail.FinalRoomCode = invAsset.RoomFinal.Code;
		//        //invDetail.FinalRoomName = invAsset.RoomFinal.Name;
		//        invDetail.FinalRoomCode = invAsset.RoomFinal != null ? invAsset.RoomFinal.Code : string.Empty;
		//        invDetail.FinalRoomName = invAsset.RoomFinal != null ? invAsset.RoomFinal.Name : string.Empty;

		//        invDetail.AssetClassCode = string.Empty;// invAsset.Asset.AssetCategory.Code;
		//        invDetail.AssetClassName = string.Empty;// invAsset.Asset.AssetCategory.Name;

		//        //if (invAsset.EmployeeInitial.InternalCode.Trim().ToUpper() == "NSP" || invAsset.EmployeeInitial.InternalCode.Trim() == string.Empty)
		//        //{
		//        //    invDetail.InitialInternalCode = "";
		//        //    invDetail.InitialFirstName = "";
		//        //    invDetail.InitialLastName = "";
		//        //}
		//        //else
		//        //{ 
		//            invDetail.InitialEmployeeId = invAsset.EmployeeInitial.Id;
		//            invDetail.InitialInternalCode = invAsset.EmployeeInitial.InternalCode;
		//            invDetail.InitialFirstName = invAsset.EmployeeInitial.FirstName;
		//            invDetail.InitialLastName = invAsset.EmployeeInitial.LastName;
		//        //}

		//        if (invAsset.EmployeeFinal == null) // || invAsset.EmployeeFinal.InternalCode.Trim().ToUpper() == "NSP" || invAsset.EmployeeFinal.InternalCode.Trim() == string.Empty)
		//        {
		//            if (employeeId.HasValue)
		//            {
		//                invDetail.FinalEmployeeId = invDetail.InitialEmployeeId;
		//                invDetail.FinalInternalCode = invDetail.InitialInternalCode;
		//                invDetail.FinalFirstName = invDetail.InitialFirstName;
		//                invDetail.FinalLastName = invDetail.InitialLastName;
		//            }
		//            else
		//            {
		//                invDetail.FinalEmployeeId = 0;
		//                invDetail.FinalInternalCode = "";
		//                invDetail.FinalFirstName = "";
		//                invDetail.FinalLastName = "";
		//            }
		//        }
		//        else
		//        {
		//            invDetail.FinalEmployeeId = invAsset.EmployeeFinal.Id;
		//            invDetail.FinalInternalCode = invAsset.EmployeeFinal.InternalCode;
		//            invDetail.FinalFirstName = invAsset.EmployeeFinal.FirstName;
		//            invDetail.FinalLastName = invAsset.EmployeeFinal.LastName;
		//        }

		//        if (employeeId.HasValue)
		//        {
		//            invDetail.InitialQuantity = Convert.ToInt32(
		//                invDetail.InitialEmployeeId == employeeId ? invAsset.QInitial : 0
		//                );
		//            invDetail.ActualQuantity = Convert.ToInt32(
		//                invDetail.FinalEmployeeId == employeeId ? invAsset.QFinal : 0
		//                );
		//        }
		//        else
		//        {
		//            //invDetail.InitialQuantity = Convert.ToInt32(invAsset.QInitial);
		//            invDetail.InitialQuantity = Convert.ToInt32(
		//                invDetail.InitialLocationId == locationId ? invAsset.QInitial : 0
		//                );
		//            //invDetail.ActualQuantity = Convert.ToInt32(invAsset.QFinal);
		//            invDetail.ActualQuantity = Convert.ToInt32(
		//                invDetail.FinalLocationId == locationId ? invAsset.QFinal : 0
		//                );
		//        }

		//        invDetail.MeasureUnit = invAsset.Asset.Uom.Code;

		//        invDetail.UnitValue = (float)invAsset.Asset.ValueInv;
		//        invDetail.InventoryValue = invDetail.InitialQuantity * invDetail.UnitValue;

		//        invDetail.AccountingValue = (float)invAsset.Asset.ValueInv;
		//        invDetail.DepreciationValue = 0;
		//        invDetail.AquisitionValue = (float)invAsset.Asset.ValueInv;
		//        invDetail.AcumulatedDepreciationValue = (float)(invAsset.Asset.ValueInv - invAsset.Asset.ValueRem);
		//        invDetail.RemainingAccoutingValue = (float)invAsset.Asset.ValueRem;

		//        invDetail.PurchaseDate = (invAsset.Asset.PurchaseDate != null) ? (DateTime)invAsset.Asset.PurchaseDate : new DateTime(0001, 01, 01);

		//        invDetail.Custody = invAsset.Asset.Custody.HasValue ? invAsset.Asset.Custody.Value ? "DA" : "NU" : "NU";

		//        invDetail.DepreciationObservation = "";
		//        invDetail.AnnulememntObservation = "";

		//        invDetailList.Add(invDetail);
		//    }

		//    invList.Details = invDetailList;

		//    return invList;
		//}

		public TrasferSG GetMovementProvidingData(int documentId, int? assetOpId)
		{
			Model.AssetOp assetOpReport = null;
			Model.Document document = null;
			TrasferSG reportData = null;
			List<TransferSGList> assets = new List<TransferSGList>();

			Model.Company transferLastPartialNumber = null;
			document = _context.Set<Model.Document>().Where(d => d.Id == documentId).AsNoTracking().SingleOrDefault();
			assetOpReport = _context.Set<Model.AssetOp>().Where(d => d.Id == assetOpId).Single();
			var valuAddInitial = assetOpReport.ValueAdd;
			var number = 0;
			var operations = _context.Set<Model.AssetOp>()
					.Include(a => a.Asset)
					.Include(r => r.RoomInitial)
					.Include(r => r.RoomFinal)
					.Include(r => r.CostCenterInitial)
					.Include(r => r.CostCenterFinal)
					.Include(r => r.EmployeeInitial)
					.Include(r => r.EmployeeFinal)
					.Include(r => r.BudgetManagerInitial)
					.Include(r => r.BudgetManagerFinal)
					.Include(s => s.SrcConfUser)
						.ThenInclude(e => e.Employee)
					.Where(a =>

					a.AssetOpStateId == 3 && a.IsDeleted == false && a.DocumentId == documentId).AsNoTracking().Select(a => new TransferSGList()
					{

						InvNo = a.Asset.InvNo,
						Description = a.Asset.Name,
						SerialNumber = a.Asset.SerialNumber,
						UserName = a.SrcConfUser.Employee != null ? a.SrcConfUser.Employee.FirstName + " " + a.SrcConfUser.Employee.LastName : "",
						RoomInitial = a.RoomInitial != null ? a.RoomInitial.Code : "",
						RoomFinal = a.RoomFinal != null ? a.RoomFinal.Code : "",
						CostCenterInitial = a.CostCenterInitial != null ? a.CostCenterInitial.Code : "",
						CostCenterFinal = a.CostCenterFinal != null ? a.CostCenterFinal.Code : "",
						EmployeeInitial = a.EmployeeInitial != null ? a.EmployeeInitial.FirstName + " " + a.EmployeeInitial.LastName : "",
						EmployeeFinal = a.EmployeeFinal != null ? a.EmployeeFinal.FirstName + " " + a.EmployeeFinal.LastName : "",
						BMInitial = a.BudgetManagerInitial != null ? a.BudgetManagerInitial.Name : "",
						BMFinal = a.BudgetManagerFinal != null ? a.BudgetManagerFinal.Name : "",

					});

			assets = operations.ToList();


			if (assetOpReport.ValueAdd == null)
			{
				transferLastPartialNumber = _context.Set<Model.Company>().Where(a => a.Code == "TRANSFER" && a.IsDeleted == true).OrderByDescending(ao => ao.Id).Take(1).SingleOrDefault();


				number = int.Parse(transferLastPartialNumber.Name);

				number = number + 1;

				assetOpReport.ValueAdd = Convert.ToDecimal(number);

				transferLastPartialNumber.Name = number.ToString();
				_context.Update(transferLastPartialNumber);
				_context.SaveChanges();

			}
			else
			{
				number = (int)assetOpReport.ValueAdd;
			}

			

			reportData = new TrasferSG
			{
				Username = assets.Count > 0 ? assets[0].UserName : "",
				DocumentNumber = number.ToString(),
				DocumentDate = document.DocumentDate.ToString(),
				TransferSGList = assets
			};

			return reportData;
		}

		public SGScrab GetScrabProvidingData(int documentId, int? assetOpId)
		{
			Model.AssetOp assetOpReport = null;
			Model.Document document = null;
			SGScrab reportData = null;
			List<SGScrabList> assets = new List<SGScrabList>();

			Model.EntityType transferLastPartialNumber = null;
			document = _context.Set<Model.Document>().Where(d => d.Id == documentId).AsNoTracking().SingleOrDefault();
			assetOpReport = _context.Set<Model.AssetOp>().Where(d => d.Id == assetOpId).Single();
			var valuAddInitial = assetOpReport.ValueAdd;
			var number = 0;
			var operations = _context.Set<Model.AssetOp>()
					.Include(a => a.Asset)
					.Include(s => s.SrcConfUser)
						.ThenInclude(e => e.Employee)
					.Where(a =>

					a.AssetOpStateId == 3 && a.IsDeleted == false && a.DocumentId == documentId).AsNoTracking().Select(a => new SGScrabList()
					{

						InvNo = a.Asset.InvNo,
						Description = a.Asset.Name,
						ValueInv = a.Asset.ValueInv,
						ValueDep = a.Asset.ValueInv,
						ValueRem = a.Asset.ValueRem,
						UserName = a.SrcConfUser.Employee != null ? a.SrcConfUser.Employee.FirstName + " " + a.SrcConfUser.Employee.LastName : "",

					});

			assets = operations.ToList();


			if (assetOpReport.ValueAdd == null)
			{
				transferLastPartialNumber = _context.Set<Model.EntityType>().Where(a => a.Code == "SCRAB" && a.IsDeleted == false).OrderByDescending(ao => ao.Id).Take(1).SingleOrDefault();


				number = int.Parse(transferLastPartialNumber.Name);

				number = number + 1;

				assetOpReport.ValueAdd = Convert.ToDecimal(number);

				transferLastPartialNumber.Name = number.ToString();
				_context.Update(transferLastPartialNumber);
				_context.SaveChanges();

			}
			else
			{
				number = (int)assetOpReport.ValueAdd;
			}



			reportData = new SGScrab
			{
				Username = assets.Count > 0 ? assets[0].UserName : "",
				DocumentNumber = number.ToString(),
				DocumentDate = document.DocumentDate.ToString(),
				SGScrabList = assets
			};

			return reportData;
		}

		public SGPIF GetReportAssetData(Guid reportId, int assetId)
        {
			Model.Document selectedDocument = null;
			Model.Asset selectedAsset = null;
			SGPIF reportData = null;
			List<SGPIFList> assets = new List<SGPIFList>();
			Model.EntityType entityType = null;
			var number = 0;

			selectedAsset = _context.Set<Model.Asset>().Where(d => d.Id == assetId).Single();
			selectedDocument = _context.Set<Model.Document>().Where(d => d.Id == selectedAsset.DocumentId).Single();

			var operations = _context.Set<Model.Asset>()
					.Include(d => d.Document)
						.ThenInclude(p => p.Partner)
					.Include(r => r.CostCenter)
					.Include(r => r.AssetNature)
					.Include(r => r.BudgetManager)
					.Include(r => r.Room)
					.Include(r => r.CreatedByUser)
					.Where(a =>

					a.Document.DocNo1.Length > 0 ? a.Document.DocNo1.Trim() == selectedDocument.DocNo1.Trim() : a.Id == assetId).AsNoTracking().Select(a => new SGPIFList()
					{

						InvNo = a.InvNo,
						Description = a.Name,
						SerialNumber = a.SerialNumber,
						CostCenter = a.CostCenter != null ? a.CostCenter.Code : "",
						AssetNature = a.Administration != null ? a.Administration.Code : "",
						Employee = a.Employee != null ? a.Employee.InternalCode : "",
						BudgetManager = a.BudgetManager != null ? a.BudgetManager.Code : "",
						Document = a.Document.DocNo1 + " / " + a.InvoiceDate != null ? a.InvoiceDate.Value.ToString("MM/dd/yyyy") : "",
						Partner = a.Document.Partner != null ? a.Document.Partner.Name : "",
						Quantity = a.Quantity,
						ValueInv = a.ValueInv,
						UserName = a.CreatedByUser.Employee != null ? a.CreatedByUser.Employee.FirstName + " " + a.CreatedByUser.Employee.LastName : "",
					
					});

			assets = operations.ToList();

			if (selectedAsset.PIFNumber == 0)
			{
				entityType = _context.Set<Model.EntityType>().Where(a => a.Code == "PIF" && a.IsDeleted == false).Take(1).SingleOrDefault();


				number = int.Parse(entityType.Name);

				number = number + 1;

				selectedAsset.PIFNumber = number;
				selectedAsset.PIFDate = DateTime.Now;
				selectedAsset.CreatedUser = reportId.ToString();
				entityType.Name = number.ToString();
				_context.SaveChanges();

			}
			else
			{
				number = selectedAsset.PIFNumber;
			}

			var userName = _context.Set<Model.ApplicationUser>().Include(e => e.Employee).AsNoTracking().Where(u => u.Id == reportId.ToString()).SingleOrDefault();

			reportData = new SGPIF
			{
				Username = userName != null ? userName.Employee.FirstName + " " + userName.Employee.LastName : "",
				DocumentNumber = number.ToString(),
				DocumentDate = selectedAsset.PIFDate.ToString(),
				SGPIFList = assets
			};

			return reportData;
		}

		public NIRSG GetReportNIRData(Guid reportId, int assetId)
		{
			Model.Document selectedDocument = null;
			Model.Asset selectedAsset = null;
			NIRSG reportData = null;
			List<NIRSGList> assets = new List<NIRSGList>();
			Model.EntityType entityType = null;
			var number = 0;

			selectedAsset = _context.Set<Model.Asset>().Where(d => d.Id == assetId).Single();
			selectedDocument = _context.Set<Model.Document>().Where(d => d.Id == selectedAsset.DocumentId).Single();

			var operations = _context.Set<Model.Asset>()
				    .Include(d => d.Document)
					.Include(r => r.Room)
						.ThenInclude(l => l.Location)
							.ThenInclude(r => r.Region)
					.Where(a =>

					a.Document.DocNo1.Length > 0 ? a.Document.DocNo1.Trim() == selectedDocument.DocNo1.Trim() : a.Id == assetId).AsNoTracking().Select(a => new NIRSGList()
					{

						InvNo = a.InvNo,
						Description = a.Name,
						SerialNumber = a.SerialNumber,
						ValueInv = a.ValueInv,
						UserName = a.CreatedByUser.Employee != null ? a.CreatedByUser.Employee.FirstName + " " + a.CreatedByUser.Employee.LastName : "",
						Location = a.Room != null ? a.Room.ERPCode : "",
						DocumentNumber = a.Document.DocNo1 + " / " + a.InvoiceDate != null ? a.InvoiceDate.Value.ToString("MM/dd/yyyy") : "",
						Partner = a.Document.Partner != null ? a.Document.Partner.Name : ""
					});

			assets = operations.ToList();

			if (selectedAsset.NIRNumber == 0)
			{
				entityType = _context.Set<Model.EntityType>().Where(a => a.Code == "NIR" && a.IsDeleted == false).Take(1).SingleOrDefault();


				number = int.Parse(entityType.Name);

				number = number + 1;

				selectedAsset.NIRNumber = number;
				selectedAsset.NIRDate = DateTime.Now;
				selectedAsset.CreatedUser = reportId.ToString();
				entityType.Name = number.ToString();
				_context.SaveChanges();

			}
			else
			{
				number = selectedAsset.NIRNumber;

			}

			var userName = _context.Set<Model.ApplicationUser>().Include(e => e.Employee).AsNoTracking().Where(u => u.Id == reportId.ToString()).SingleOrDefault();

			reportData = new NIRSG
			{
				Username = userName != null ? userName.Employee.FirstName + " " + userName.Employee.LastName : "",
				DocumentNumber = number.ToString(),
				DocumentDate = selectedAsset.NIRDate.ToString(),
				NIRSGList = assets
			};

			return reportData;
		}

		public InventoryResult GetInventoryListEmployees(int? inventoryId, int? admCenterId)
        {
            InventoryResult invResult = new InventoryResult();
            Model.Inventory inventory = null;

            inventory = _context.Set<Model.Inventory>().OrderByDescending(i => i.Id == inventoryId).FirstOrDefault();

            invResult.Details = new List<InventoryResultDetail>();

            if (admCenterId != null)
            {
                var query = from E in _context.Employees.Include(c => c.CostCenter).ThenInclude(a => a.AdmCenter)
                            join TI in (from TI in _context.InventoryAssets.Include(c => c.CostCenterInitial).ThenInclude(a => a.AdmCenter).Include(c => c.EmployeeInitial) where TI.InventoryId == inventoryId && ((TI.StateIdFinal == null) || ((TI.StateIdFinal != 4) && (TI.StateIdFinal != 5))) select TI)
                                      on E.Id equals TI.EmployeeIdInitial into join1


                            join TNS in (from TNS in _context.InventoryAssets.Include(e => e.EmployeeInitial).Include(e => e.EmployeeFinal) where TNS.EmployeeIdFinal == null && TNS.InventoryId == inventoryId && ((TNS.StateIdFinal == null) || ((TNS.StateIdFinal != 4) && (TNS.StateIdFinal != 5))) select TNS)
                                      on E.Id equals TNS.EmployeeIdInitial into join2

                            join TM in (from TM in _context.InventoryAssets.Include(e => e.EmployeeInitial).Include(e => e.EmployeeFinal) where TM.EmployeeIdInitial != TM.EmployeeIdFinal && TM.InventoryId == inventoryId && ((TM.StateIdFinal == null) || ((TM.StateIdFinal != 4) && (TM.StateIdFinal != 5))) select TM)
                                      on E.Id equals TM.EmployeeIdInitial into join3

                            join TP in (from TP in _context.InventoryAssets.Include(e => e.EmployeeInitial).Include(e => e.EmployeeFinal) where TP.EmployeeIdInitial != TP.EmployeeIdFinal && TP.InventoryId == inventoryId && ((TP.StateIdFinal == null) || ((TP.StateIdFinal != 4) && (TP.StateIdFinal != 5))) select TP)
                                      on E.Id equals TP.EmployeeIdFinal into join4

                            join TT in (from TT in _context.InventoryAssets.Include(e => e.EmployeeInitial).Include(e => e.EmployeeFinal) where ((TT.RoomIdInitial != TT.RoomIdFinal) || (TT.EmployeeIdInitial != TT.EmployeeIdFinal)) && TT.InventoryId == inventoryId && ((TT.StateIdFinal == null) || ((TT.StateIdFinal != 4) && (TT.StateIdFinal != 5))) && TT.RoomIdFinal != null select TT)
                                      on E.Id equals TT.EmployeeIdInitial into join5

                            join TEMP in (from TEMP in _context.AssetNis.Include(c => c.CostCenter).ThenInclude(a => a.AdmCenter) where TEMP.InventoryId == inventoryId select TEMP)
                                      on E.Id equals TEMP.EmployeeId into join6

                            // where (E.CostCenter.AdmCenterId == admCenterId)

                            select new InventoryResultDetail
                            {
                                InternalCode = E.InternalCode,
                                FullName = E.FirstName + " " + E.LastName,
                                Total = join1.Count(),
                                NotScanned = join2.Count(),
                                Minus = join3.Count(),
                                Plus = join4.Count(),
                                Tranfer = join5.Count(),
                                Temp = join6.Count()

                            };


                invResult.Details = query.ToList();


                invResult.InventoryName = inventory.Description.Trim();

                return invResult;
            }
            else
            {
                var query = from E in _context.Employees.Include(c => c.CostCenter).ThenInclude(a => a.AdmCenter)
                            join TI in (from TI in _context.InventoryAssets.Include(c => c.CostCenterInitial).ThenInclude(a => a.AdmCenter).Include(c => c.EmployeeInitial) where TI.InventoryId == inventoryId && ((TI.StateIdFinal == null) || ((TI.StateIdFinal != 4) && (TI.StateIdFinal != 5))) select TI)
                                      on E.Id equals TI.EmployeeIdInitial into join1


                            join TNS in (from TNS in _context.InventoryAssets.Include(e => e.EmployeeInitial).Include(e => e.EmployeeFinal) where TNS.EmployeeIdFinal == null && TNS.InventoryId == inventoryId && ((TNS.StateIdFinal == null) || ((TNS.StateIdFinal != 4) && (TNS.StateIdFinal != 5))) select TNS)
                                      on E.Id equals TNS.EmployeeIdInitial into join2

                            join TM in (from TM in _context.InventoryAssets.Include(e => e.EmployeeInitial).Include(e => e.EmployeeFinal) where TM.EmployeeIdInitial != TM.EmployeeIdFinal && TM.InventoryId == inventoryId && ((TM.StateIdFinal == null) || ((TM.StateIdFinal != 4) && (TM.StateIdFinal != 5))) select TM)
                                      on E.Id equals TM.EmployeeIdInitial into join3

                            join TP in (from TP in _context.InventoryAssets.Include(e => e.EmployeeInitial).Include(e => e.EmployeeFinal) where TP.EmployeeIdInitial != TP.EmployeeIdFinal && TP.InventoryId == inventoryId && ((TP.StateIdFinal == null) || ((TP.StateIdFinal != 4) && (TP.StateIdFinal != 5))) select TP)
                                      on E.Id equals TP.EmployeeIdFinal into join4

                            join TT in (from TT in _context.InventoryAssets.Include(e => e.EmployeeInitial).Include(e => e.EmployeeFinal) where ((TT.RoomIdInitial != TT.RoomIdFinal) || (TT.EmployeeIdInitial != TT.EmployeeIdFinal)) && TT.InventoryId == inventoryId && ((TT.StateIdFinal == null) || ((TT.StateIdFinal != 4) && (TT.StateIdFinal != 5))) && TT.RoomIdFinal != null select TT)
                                      on E.Id equals TT.EmployeeIdInitial into join5

                            join TEMP in (from TEMP in _context.AssetNis.Include(c => c.CostCenter).ThenInclude(a => a.AdmCenter) where TEMP.InventoryId == inventoryId select TEMP)
                                      on E.Id equals TEMP.EmployeeId into join6


                            select new InventoryResultDetail
                            {
                                InternalCode = E.InternalCode,
                                FullName = E.FirstName + " " + E.LastName,
                                Total = join1.Count(),
                                NotScanned = join2.Count(),
                                Minus = join3.Count(),
                                Plus = join4.Count(),
                                Tranfer = join5.Count(),
                                Temp = join6.Count()

                            };


                invResult.Details = query.ToList();


                invResult.InventoryName = inventory.Description.Trim();

                return invResult;
            }

        }

        public InventoryReconciliation GetInventoryListReconciliations(int? inventoryId, int? admCenterId)
        {
            InventoryReconciliation invReconcilation = new InventoryReconciliation();
            Model.Inventory inventory = null;


            inventory = _context.Set<Model.Inventory>().Where(i => i.Id == inventoryId).FirstOrDefault();


            invReconcilation.Reconciliations = new List<InventoryReconciliationDetail>();

            var query =
                        from IA in _context.InventoryAssets
                        join asset in _context.Assets on IA.AssetId equals asset.Id
                        join assetni in _context.AssetNis on asset.Id equals assetni.AssetId
                        //join costCenter in _context.CostCenters on IA.CostCenterIdInitial equals costCenter.Id
                        //join admCenter in _context.AdmCenters on costCenter.AdmCenterId equals admCenter.Id
                        join room in _context.Rooms on IA.RoomIdInitial equals room.Id
                        join location in _context.Locations on room.LocationId equals location.Id
                        join region in _context.Regions on location.RegionId equals region.Id
                        join employee in _context.Employees on IA.EmployeeIdInitial equals employee.Id
                        //join costCenter1 in _context.CostCenters on IA.CostCenterIdFinal equals costCenter1.Id
                        //join admCenter1 in _context.AdmCenters on costCenter1.AdmCenterId equals admCenter1.Id
                        join room1 in _context.Rooms on IA.RoomIdFinal equals room1.Id
                        join location1 in _context.Locations on room1.LocationId equals location1.Id
                        join region1 in _context.Regions on location1.RegionId equals region1.Id
                        join employee1 in _context.Employees on IA.EmployeeIdFinal equals employee1.Id
                        where IA.InventoryId == inventoryId && assetni.InventoryId == inventoryId

                        select new InventoryReconciliationDetail
                        {
                            TempInvNo = assetni.Code1,
                            TempName = assetni.Name1,
                            InvNo = asset.InvNo,
                            Description = asset.Name,
                            SerialNumberInitial = asset.SerialNumber,
                            SerialNumberFinal = assetni.SerialNumber,
                            PurchaseDate = asset.PurchaseDate.Value,
                            ValueInv = asset.ValueInv,
                            ValueDep = asset.ValueInv - asset.ValueRem,
                            AdmCenterNameInitial = region.Name,
                            AdmCenterCodeInitial = region.Code,
                            CostCenterCodeInitial = region.Code,
                            CostCenterNameInitial = region.Name,
                            EmployeeInternalCodeInitial = employee.InternalCode,
                            EmployeeFirstNameInitial = employee.FirstName,
                            EmployeeLastNameInitial = employee.LastName,
                            RoomNameInitial = room.Name,
                            LocationNameInitial = location.Name,
                            AdmCenterNameFinal = region.Name,
                            CostCenterCodeFinal = region.Code,
                            EmployeeInternalCodeFinal = employee1.InternalCode,
                            EmployeeFirstNameFinal = employee1.FirstName,
                            EmployeeLastNameFinal = employee1.LastName,
                            RoomNameFinal = room1.Name,
                            LocationNameFinal = location1.Name
                        };

            if (admCenterId != null)
            {
                Model.AdmCenter admCenterName = null;
                admCenterName = _context.Set<Model.AdmCenter>().Where(i => i.Id == admCenterId).FirstOrDefault();
                query = query.Where(a => a.AdmCenterNameInitial == admCenterName.Name || a.AdmCenterNameFinal == admCenterName.Name);
            }

            invReconcilation.Reconciliations = query.ToList();
            invReconcilation.InventoryName = inventory.Description.Trim();
            return invReconcilation;
        }

        public InventoryUserScan GetInventoryListUserScans(int? inventoryId, int? employeeId, int? admCenterId)
        {
            InventoryUserScan invUserScans = new InventoryUserScan();
            Model.Inventory inventory = null;


            inventory = _context.Set<Model.Inventory>().Where(i => i.Id == inventoryId).FirstOrDefault();


            invUserScans.UserScanDetail = new List<InventoryUserScanDetail>();

            var query =
                        from IA in _context.InventoryAssets

                        join asset in _context.Assets on IA.AssetId equals asset.Id
                        // join assetni in _context.AssetNis on asset.Id equals assetni.AssetId
                        join room in _context.Rooms on IA.RoomIdFinal equals room.Id
                        join location in _context.Locations on room.LocationId equals location.Id
                        join employee in _context.Employees on IA.EmployeeIdFinal equals employee.Id
                        join costCenter in _context.CostCenters on IA.CostCenterIdFinal equals costCenter.Id
                        join region in _context.Regions on location.RegionId equals region.Id
                        join user in _context.Users on IA.ModifiedBy equals user.Id
                        where IA.InventoryId == inventoryId
                        && user.EmployeeId == employeeId


                        select new InventoryUserScanDetail
                        {
                            InvNo = asset.InvNo,
                            Description = asset.Name,
                            AdmCenterCode = costCenter.Code,
                            AdmCenterName = region.Name,
                            CostCenterCode = costCenter.Code,
                            CostCenterName = costCenter.Name,
                            LocationName = location.Name,
                            RoomName = room.Name,
                            EmployeeInternalCode = employee.InternalCode,
                            EmployeeFirstName = employee.FirstName,
                            EmployeeLastName = employee.LastName,
                            EmployeeFullName = employee.FirstName + " " + employee.LastName,
                            SyncDate = IA.ModifiedAt.Value,
                            UserEmail = user.Email,
                            UserFirstName = user.FamilyName,
                            UserLastName = user.GivenName
                        };

            if (admCenterId != null)
            {
                Model.Region admCenterName = null;
                admCenterName = _context.Set<Model.Region>().Where(i => i.Id == admCenterId).FirstOrDefault();
                query = query.Where(a => a.AdmCenterName == admCenterName.Name);
            }

            invUserScans.UserScanDetail = query.ToList();
            invUserScans.InventoryName = inventory.Description.Trim();

            return invUserScans;
        }

        public InventoryUserBuildingScan GetInventoryListUserBuildingScans(int? inventoryId)
        {
            InventoryUserBuildingScan invUserScans = new InventoryUserBuildingScan();
            Model.Inventory inventory = null;


            inventory = _context.Set<Model.Inventory>().Where(i => i.Id == inventoryId).FirstOrDefault();

            List<Model.InventoryUserBuildingScanDetail> items = _context.Set<Model.InventoryUserBuildingScanDetail>().FromSql("GetInventoryListUserBuildingScans {0}", inventoryId).ToList();


            invUserScans.UserBuildingScanDetail = new List<Dto.Reporting.InventoryUserBuildingScanDetail>();

            //var query =
            //            from IA in _context.InventoryAssets
            //            join room in _context.Rooms on IA.RoomIdFinal equals room.Id
            //            where IA.RoomIdFinal != null && IA.InventoryId == inventoryId
            //            group room by new
            //            {
            //                room.LocationId,
            //                IA.ModifiedBy,
            //                IA.ModifiedAt,
            //            } into rooms


            //            join location in _context.Locations on rooms.Key.LocationId equals location.Id
            //            join admCenter in _context.AdmCenters on location.AdmCenterId equals admCenter.Id
            //            join user in _context.Users on rooms.Key.ModifiedBy equals user.Id
            //            orderby rooms.Key.ModifiedAt descending

            //            select new Dto.Reporting.InventoryUserBuildingScanDetail
            //            {
            //                LocationName = location.Name,
            //                RegionName = admCenter.Name,
            //                SyncDate = rooms.Key.ModifiedAt.Value,
            //                SyncHour = rooms.Key.ModifiedAt.Value,
            //                UserEmail = user.Email,
            //                UserFirstName = user.FamilyName,
            //                UserLastName = user.GivenName
            //            };



            //var query =
            //           from IA in _context.InventoryAssets
            //           join costCenter in _context.CostCenters on IA.CostCenterIdFinal equals costCenter.Id
            //           where IA.RoomIdFinal != null && IA.InventoryId == inventoryId
            //           group costCenter by new
            //           {
            //               costCenter.AdmCenterId,
            //               IA.ModifiedBy,
            //               IA.ModifiedAt,
            //               costCenter.Name

            //           } into IAGROUP

            //           join admCenter in _context.AdmCenters on IAGROUP.Key.AdmCenterId equals admCenter.Id
            //           join user in _context.Users on IAGROUP.Key.ModifiedBy equals user.Id
            //           orderby IAGROUP.Key.ModifiedAt descending

            //           select new InventoryUserBuildingScanDetail
            //           {
            //               LocationName = IAGROUP.Key.Name,
            //               RegionName = admCenter.Name,
            //               SyncDate = IAGROUP.Key.ModifiedAt.Value,
            //               SyncHour = IAGROUP.Key.ModifiedAt.Value,
            //               UserEmail = user.Email,
            //               UserFirstName = user.FamilyName,
            //               UserLastName = user.GivenName
            //           };


            foreach (var item in items)
            {
                Dto.Reporting.InventoryUserBuildingScanDetail result = null;

                result = new Dto.Reporting.InventoryUserBuildingScanDetail()
                {
                    LocationName = item.LocationName,
                    RegionName = item.RegionName,
                    SyncDate = item.Data,
                    SyncHour = item.Hour,
                    UserEmail = item.UserEmail,
                    UserFirstName = item.UserFirstName,
                    UserLastName = item.UserLastName
                };

                invUserScans.UserBuildingScanDetail.Add(result);
            }

            invUserScans.InventoryName = inventory.Description.Trim();


            return invUserScans;
        }

        public InventoryResultEmag GetInventoryListEmagByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? companyId, int? administrationId, int? roomId, string reportFilter)
        {
            InventoryResultEmag invResult = new InventoryResultEmag();
            Model.Inventory inventory = null;
            Model.CostCenter costCenter = null;
            IQueryable<Model.InventoryAsset> invAssetQuery = null;

            List<Model.InventoryAsset> listInventoryAssets = null;

            inventory = _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId).SingleOrDefault();
            costCenter = (costCenterId.HasValue && costCenterId > 0) ? _context.Set<Model.CostCenter>().Include(r => r.Room).Include(c =>c.Company).Single(l => l.Id == costCenterId) : null;
            invAssetQuery = _assetsRepository.GetInventoryAssetReportsQuery2(inventoryId, null);


			if (costCenterId.HasValue && (costCenterId > 0))
			{
				invAssetQuery = invAssetQuery.Where(i => ((i.CostCenterIdInitial == costCenterId) || (i.CostCenterIdFinal == costCenterId)));
			}

            if (invAssetQuery != null)
            {
                invAssetQuery = invAssetQuery.OrderBy(i => i.Asset.InvNo);
                listInventoryAssets = invAssetQuery.ToList();
            }
            else
            {
                listInventoryAssets = new List<Model.InventoryAsset>();
            }

            invResult.Details = new List<InventoryResultDetailEmag>();

            foreach (var inventoryAsset in listInventoryAssets)
            {
                InventoryResultDetailEmag inventoryResultDetail = null;
                inventoryResultDetail = new InventoryResultDetailEmag()
                {
                    Description = inventoryAsset.Asset.Name,
                    PurchaseDate = inventoryAsset.Asset.PurchaseDate,
                    InvNo = inventoryAsset.Asset.InvNo,
                    Key = inventoryAsset.Asset.SubNo,
                    Initial = inventoryAsset.QInitial,
                    Actual = inventoryAsset.QFinal,
                    Administration = inventoryAsset.CostCenterFinal != null && inventoryAsset.CostCenterFinal.Administration != null ? inventoryAsset.CostCenterFinal.Administration.Name : inventoryAsset.CostCenterInitial != null && inventoryAsset.CostCenterInitial.Administration != null ?  inventoryAsset.CostCenterInitial.Administration.Name : "",
                    ValueInv = inventoryAsset.Asset.ValueInv,
                    SerialNumber = inventoryAsset.CostCenterFinal != null ?  inventoryAsset.SerialNumber : inventoryAsset.SNInitial,
                    Info = inventoryAsset.Info2019,
                    AssetId = 1

                };

                invResult.Details.Add(inventoryResultDetail);
            }

            invResult.EndDate = inventory.Start.Value;
            invResult.Company = (costCenter != null && costCenter.Company != null) ? costCenter.Company.Name : "";
            invResult.Address = (costCenter != null && costCenter.Room != null) ? costCenter.Room.Name : "";
            invResult.CostCenterCode = (costCenter != null) ? costCenter.Code: "";
            invResult.CostCenterName = (costCenter != null) ? costCenter.Name : "";
            invResult.InventoryName = inventory != null ? inventory.Description : "2021";
            //if (inventoryId > 0 && administrationId != null)
            //{
            //    var committee = _context.Set<Model.Committee>()
            //        .Include(e => e.Employee)
            //        .Include(e => e.Employee2)
            //        .Include(e => e.Employee3)
            //        .Include(e => e.Employee4)
            //        .Include(e => e.Employee5)
            //        .Include(e => e.Employee6)
            //        .Include(e => e.Employee7)
            //        .Where(c => c.InventoryId == inventoryId && c.AdministrationId == administrationId && c.IsDeleted == false).FirstOrDefault();

            //    if (committee != null)
            //    {
            //        invResult.Member1 = committee.Employee != null ? committee.Employee.FirstName + " " + committee.Employee.LastName : "";
            //        invResult.Member2 = committee.Employee2 != null ? committee.Employee2.FirstName + " " + committee.Employee2.LastName : "";
            //        invResult.Member3 = committee.Employee3 != null ? committee.Employee3.FirstName + " " + committee.Employee3.LastName : "";
            //        invResult.Member4 = committee.Employee4 != null ? committee.Employee4.FirstName + " " + committee.Employee4.LastName : "";
            //        invResult.Member5 = committee.Employee5 != null ? committee.Employee5.FirstName + " " + committee.Employee5.LastName : "";
            //        invResult.Member6 = committee.Employee6 != null ? committee.Employee6.FirstName + " " + committee.Employee6.LastName : "";
            //        invResult.Member7 = committee.Employee7 != null ? committee.Employee7.FirstName + " " + committee.Employee7.LastName : "";
            //    }
            //}
            //else
            //{
            //    if (inventoryId > 0 && locationId != null)
            //    {
            //        var committee = _context.Set<Model.Committee>()
            //            .Include(e => e.Employee)
            //            .Include(e => e.Employee2)
            //            .Include(e => e.Employee3)
            //            .Include(e => e.Employee4)
            //            .Include(e => e.Employee5)
            //            .Include(e => e.Employee6)
            //            .Include(e => e.Employee7)
            //            .Where(c => c.InventoryId == inventoryId && c.LocationId == locationId && c.IsDeleted == false).FirstOrDefault();

            //        if (committee != null)
            //        {
            //            invResult.Member1 = committee.Employee != null ? committee.Employee.FirstName + " " + committee.Employee.LastName : "";
            //            invResult.Member2 = committee.Employee2 != null ? committee.Employee2.FirstName + " " + committee.Employee2.LastName : "";
            //            invResult.Member3 = committee.Employee3 != null ? committee.Employee3.FirstName + " " + committee.Employee3.LastName : "";
            //            invResult.Member4 = committee.Employee4 != null ? committee.Employee4.FirstName + " " + committee.Employee4.LastName : "";
            //            invResult.Member5 = committee.Employee5 != null ? committee.Employee5.FirstName + " " + committee.Employee5.LastName : "";
            //            invResult.Member6 = committee.Employee6 != null ? committee.Employee6.FirstName + " " + committee.Employee6.LastName : "";
            //            invResult.Member7 = committee.Employee7 != null ? committee.Employee7.FirstName + " " + committee.Employee7.LastName : "";
            //        }
            //    }
            //}


            return invResult;
        }

        public InventoryResultEmployeeEmag GetInventoryListEmpEmagByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? companyId, int? administrationId, int? roomId)
        {
            InventoryResultEmployeeEmag invResult = new InventoryResultEmployeeEmag();
            Model.CostCenter costCenter = null;
            Model.Inventory inventory = null;
            Model.EmployeeCostCenter employeeCostCenter = null;

            inventory = _context.Set<Model.Inventory>().Where(a => a.Active == true).SingleOrDefault();

            List<Model.InventoryAsset> listInventoryAssets = new List<Model.InventoryAsset>();
            invResult.Details = new List<InventoryResultDetailEmployeeEmag>();

            costCenter = (costCenterId.HasValue && costCenterId > 0) ? _context.Set<Model.CostCenter>().Include(c => c.Company).Single(l => l.Id == costCenterId) : null;
            employeeCostCenter = _context.Set<Model.EmployeeCostCenter>().Include(e => e.Employee).Where(c => c.CostCenterId == costCenterId).SingleOrDefault();

            invResult.Company = (costCenter != null && costCenter.Company != null) ? costCenter.Company.Name : "";
            invResult.Address = (costCenter != null && costCenter.Room != null) ? costCenter.Room.Name : "";
            invResult.CostCenterCode = (costCenter != null) ? costCenter.Code : "";
            invResult.CostCenterName = (costCenter != null) ? costCenter.Name : "";
            invResult.InventoryName = inventory != null ? inventory.Description : "2021";
            invResult.Employee = employeeCostCenter != null && employeeCostCenter.Employee != null ? employeeCostCenter.Employee.FirstName + " " + employeeCostCenter.Employee.LastName : "";
            invResult.Document = "2021";

            //if (inventoryId > 0 && administrationId != null)
            //{
            //    var committee = _context.Set<Model.Committee>()
            //        .Include(e => e.Employee)
            //        .Include(e => e.Employee2)
            //        .Include(e => e.Employee3)
            //        .Include(e => e.Employee4)
            //        .Include(e => e.Employee5)
            //        .Include(e => e.Employee6)
            //        .Include(e => e.Employee7)
            //        .Where(c => c.InventoryId == inventoryId && c.AdministrationId == administrationId && c.IsDeleted == false).FirstOrDefault();

            //    if (committee != null)
            //    {
            //        invResult.Document1 = committee.Document1;
            //        invResult.Member1 = committee.Employee != null ? committee.Employee.FirstName + " " + committee.Employee.LastName : "";
            //        invResult.Member2 = committee.Employee2 != null ? committee.Employee2.FirstName + " " + committee.Employee2.LastName : "";
            //        invResult.Member3 = committee.Employee3 != null ? committee.Employee3.FirstName + " " + committee.Employee3.LastName : "";
            //        invResult.Member4 = committee.Employee4 != null ? committee.Employee4.FirstName + " " + committee.Employee4.LastName : "";
            //        invResult.Member5 = committee.Employee5 != null ? committee.Employee5.FirstName + " " + committee.Employee5.LastName : "";
            //        invResult.Member6 = committee.Employee6 != null ? committee.Employee6.FirstName + " " + committee.Employee6.LastName : "";
            //        invResult.Member7 = committee.Employee7 != null ? committee.Employee7.FirstName + " " + committee.Employee7.LastName : "";
            //    }
            //}
            //else
            //{
            //    if (inventoryId > 0 && locationId != null)
            //    {
            //        var committee = _context.Set<Model.Committee>()
            //            .Include(e => e.Employee)
            //            .Include(e => e.Employee2)
            //            .Include(e => e.Employee3)
            //            .Include(e => e.Employee4)
            //            .Include(e => e.Employee5)
            //            .Include(e => e.Employee6)
            //            .Include(e => e.Employee7)
            //            .Where(c => c.InventoryId == inventoryId && c.LocationId == locationId && c.IsDeleted == false).FirstOrDefault();

            //        if (committee != null)
            //        {
            //            invResult.Document1 = committee.Document1;
            //            invResult.Member1 = committee.Employee != null ? committee.Employee.FirstName + " " + committee.Employee.LastName : "";
            //            invResult.Member2 = committee.Employee2 != null ? committee.Employee2.FirstName + " " + committee.Employee2.LastName : "";
            //            invResult.Member3 = committee.Employee3 != null ? committee.Employee3.FirstName + " " + committee.Employee3.LastName : "";
            //            invResult.Member4 = committee.Employee4 != null ? committee.Employee4.FirstName + " " + committee.Employee4.LastName : "";
            //            invResult.Member5 = committee.Employee5 != null ? committee.Employee5.FirstName + " " + committee.Employee5.LastName : "";
            //            invResult.Member6 = committee.Employee6 != null ? committee.Employee6.FirstName + " " + committee.Employee6.LastName : "";
            //            invResult.Member7 = committee.Employee7 != null ? committee.Employee7.FirstName + " " + committee.Employee7.LastName : "";
            //        }
            //    }
            //}


            return invResult;
        }

        public InventoryResultEmployeeEmag GetInventoryListEmpFinalEmagByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? companyId, int? administrationId, int? roomId)
        {
            InventoryResultEmployeeEmag invResult = new InventoryResultEmployeeEmag();
            Model.CostCenter costCenter = null;
            Model.Inventory inventory = null;
            Model.EmployeeCostCenter employeeCostCenter = null;

            inventory = _context.Set<Model.Inventory>().Where(a => a.Active == true).SingleOrDefault();

            List<Model.InventoryAsset> listInventoryAssets = new List<Model.InventoryAsset>();
            invResult.Details = new List<InventoryResultDetailEmployeeEmag>();

            costCenter = (costCenterId.HasValue && costCenterId > 0) ? _context.Set<Model.CostCenter>().Include(c => c.Room).Include(c => c.Company).Include(c =>c.Administration).Single(l => l.Id == costCenterId) : null;
            employeeCostCenter = _context.Set<Model.EmployeeCostCenter>().Include(e => e.Employee).Where(c => c.CostCenterId == costCenterId).SingleOrDefault();

            invResult.Company = (costCenter != null && costCenter.Company != null) ? costCenter.Company.Name : "";
            invResult.Address = (costCenter != null && costCenter.Room != null) ? costCenter.Room.Name : "";
            invResult.Administration = (costCenter != null && costCenter.Administration != null) ? costCenter.Administration.Name : "";
            invResult.CostCenterCode = (costCenter != null) ? costCenter.Code : "";
            invResult.CostCenterName = (costCenter != null) ? costCenter.Name : "";
            invResult.InventoryName = inventory != null ? inventory.Description : "2021";
            invResult.Employee = employeeCostCenter != null && employeeCostCenter.Employee != null ? employeeCostCenter.Employee.FirstName + " " + employeeCostCenter.Employee.LastName : "";
            invResult.Document = "2021";

            //if (inventoryId > 0 && administrationId != null)
            //{
            //    var committee = _context.Set<Model.Committee>()
            //        .Include(e => e.Employee)
            //        .Include(e => e.Employee2)
            //        .Include(e => e.Employee3)
            //        .Include(e => e.Employee4)
            //        .Include(e => e.Employee5)
            //        .Include(e => e.Employee6)
            //        .Include(e => e.Employee7)
            //        .Where(c => c.InventoryId == inventoryId && c.AdministrationId == administrationId && c.IsDeleted == false).FirstOrDefault();

            //    if (committee != null)
            //    {
            //        invResult.Document1 = committee.Document1;
            //        invResult.Member1 = committee.Employee != null ? committee.Employee.FirstName + " " + committee.Employee.LastName : "";
            //        invResult.Member2 = committee.Employee2 != null ? committee.Employee2.FirstName + " " + committee.Employee2.LastName : "";
            //        invResult.Member3 = committee.Employee3 != null ? committee.Employee3.FirstName + " " + committee.Employee3.LastName : "";
            //        invResult.Member4 = committee.Employee4 != null ? committee.Employee4.FirstName + " " + committee.Employee4.LastName : "";
            //        invResult.Member5 = committee.Employee5 != null ? committee.Employee5.FirstName + " " + committee.Employee5.LastName : "";
            //        invResult.Member6 = committee.Employee6 != null ? committee.Employee6.FirstName + " " + committee.Employee6.LastName : "";
            //        invResult.Member7 = committee.Employee7 != null ? committee.Employee7.FirstName + " " + committee.Employee7.LastName : "";
            //    }
            //}
            //else
            //{
            //    if (inventoryId > 0 && locationId != null)
            //    {
            //        var committee = _context.Set<Model.Committee>()
            //            .Include(e => e.Employee)
            //            .Include(e => e.Employee2)
            //            .Include(e => e.Employee3)
            //            .Include(e => e.Employee4)
            //            .Include(e => e.Employee5)
            //            .Include(e => e.Employee6)
            //            .Include(e => e.Employee7)
            //            .Where(c => c.InventoryId == inventoryId && c.LocationId == locationId && c.IsDeleted == false).FirstOrDefault();

            //        if (committee != null)
            //        {
            //            invResult.Document1 = committee.Document1;
            //            invResult.Member1 = committee.Employee != null ? committee.Employee.FirstName + " " + committee.Employee.LastName : "";
            //            invResult.Member2 = committee.Employee2 != null ? committee.Employee2.FirstName + " " + committee.Employee2.LastName : "";
            //            invResult.Member3 = committee.Employee3 != null ? committee.Employee3.FirstName + " " + committee.Employee3.LastName : "";
            //            invResult.Member4 = committee.Employee4 != null ? committee.Employee4.FirstName + " " + committee.Employee4.LastName : "";
            //            invResult.Member5 = committee.Employee5 != null ? committee.Employee5.FirstName + " " + committee.Employee5.LastName : "";
            //            invResult.Member6 = committee.Employee6 != null ? committee.Employee6.FirstName + " " + committee.Employee6.LastName : "";
            //            invResult.Member7 = committee.Employee7 != null ? committee.Employee7.FirstName + " " + committee.Employee7.LastName : "";
            //        }
            //    }
            //}


            return invResult;
        }

        public Dto.Reporting.AuditInventoryResult GetInventoryResultByFilters(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? companyId, int? administrationId, int? roomId, string reportFilter)
        {

            Model.Company company = null;
            Model.EmployeeCostCenter employeeCostCenter = null;
            Model.Administration administration = null;
            company = (companyId.HasValue && companyId > 0) ? _context.Set<Model.Company>().Single(l => l.Id == companyId) : null;

            // List<Model.AuditInventoryResult> inventoryResult = null;
            List<Model.AuditInventoryResult> result = new List<Model.AuditInventoryResult>();
            List<Model.AuditInventoryResult> auditInventory = new List<Model.AuditInventoryResult>();

            if (costCenterId != null)
            {
                auditInventory = _context.Set<Model.AuditInventoryResult>().FromSql("CostCenterAuditInventory {0}, {1}", costCenterId, inventoryId).ToList();
            }
            //else
            //{
            //    auditInventory = _context.Set<Model.AuditInventoryResult>().FromSql("AdmCenterAuditInventory {0}, {1}", admCenterId, inventoryId).ToList();
            //}


            if (locationId != null)
            {
                admCenterId = 0;
            }

            Dto.Reporting.AuditInventoryResult resultList = new Dto.Reporting.AuditInventoryResult();

            resultList.Details = new List<AuditInventoryResultDetail>();

			//if (inventoryId > 0 && locationId != null)
			//{
			//    var committee = _context.Set<Model.Committee>()
			//        //.Include(e => e.Employee)
			//        //.Include(e => e.Employee2)
			//        //.Include(e => e.Employee3)
			//        //.Include(e => e.Employee4)
			//        //.Include(e => e.Employee5)
			//        //.Include(e => e.Employee6)
			//        //.Include(e => e.Employee7)
			//        //.Include(e => e.Employee8)
			//        //.Include(e => e.Employee9)
			//        //.Include(e => e.Employee10)

			//        .Where(c => c.InventoryId == inventoryId && c.LocationId == locationId && c.IsDeleted == false).FirstOrDefault();

			//    if (committee != null)
			//    {

			//        resultList.Document1 = committee.Document1;
			//        resultList.Document2 = committee.Document2;

			//        resultList.Member1 = auditInventory[0].Member1;
			//        resultList.Member2 = auditInventory[0].Member2;
			//        resultList.Member3 = auditInventory[0].Member3;
			//        resultList.Member4 = auditInventory[0].Member4;
			//        resultList.Member5 = auditInventory[0].Member5;
			//        resultList.Member6 = auditInventory[0].Member6;
			//        resultList.Member7 = auditInventory[0].Member7;

			//    }
			//}

			if (costCenterId != null)
			{
                employeeCostCenter = _context.Set<Model.EmployeeCostCenter>().Include(e => e.Employee).Where(c => c.CostCenterId == costCenterId).FirstOrDefault();

				if (employeeCostCenter != null)
				{
					//resultList.Location = location.Code + "/" + location.Name;
					//resultList.Region = company.CompanyType.Name;
					resultList.Employee = employeeCostCenter.Employee.FirstName + " " + employeeCostCenter.Employee.LastName;
				}
			}

			//if (administrationId != null)
			//{
			//    administration = _context.Set<Model.Administration>().Include(e => e.Division).Where(c => c.Id == administrationId).FirstOrDefault();

			//    if (administration != null)
			//    {
			//        resultList.Employee = administration.Division.Code + " " + administration.Division.Name;
			//        resultList.Employee2 = company != null && company.CompanyType != null && company.CompanyType.Code.Trim() == "U" && administration != null && administration.DivisionId != null && administration.DivisionId == 870 ? "Nitu Gheorghe, " : company != null && company.CompanyType != null && company.CompanyType.Code.Trim() == "U" && administration != null && administration.DivisionId != null && administration.DivisionId == 872 ? ", Aldea Paula" : "";
			//        resultList.Employee3 = company != null && company.CompanyType != null && company.CompanyType.Code.Trim() == "U" && administration != null && administration.DivisionId != null && administration.DivisionId == 870 ? "Zisu Elena" : "";
			//    }
			//}




			resultList.MinDateLocation = (DateTime)auditInventory[0].InventoryStartDate;
			resultList.MaxDateLocation = (DateTime)(auditInventory[0].InventoryEndDate != null ? auditInventory[0].InventoryEndDate : DateTime.Now);
			//// resultList.Reco = countReco;
			//resultList.Cassation = countCass;
			//resultList.Cassation = auditInventory[0].Cassation;
			////resultList.MinDateAdmCenter = minDateAdm.Count > 0 ? minDateAdm[0].ScanDate.Value : DateTime.MinValue;
			////resultList.MaxDateAdmCenter = maxDateAdm.Count > 0 ? maxDateAdm[0].ScanDate.Value : DateTime.Now;
			////resultList.MinDateLocation = minDateLoc.Count > 0 ? minDateLoc[0].ScanDate.Value : DateTime.MinValue;
			////resultList.MaxDateLocation = maxDateLoc.Count > 0 ? maxDateLoc[0].ScanDate.Value : DateTime.Now;
			////resultList.MinDateAdmCenter = auditInventory[0].MinDateAdmCenter;
			////resultList.MaxDateAdmCenter = auditInventory[0].MaxDateAdmCenter;
			////resultList.MinDateLocation = auditInventory[0].MinDateLocation;
			////resultList.MaxDateLocation = auditInventory[0].MaxDateLocation;
			////resultList.Temporary = auditInventory[0].Temporary;
			resultList.Plus = auditInventory[0].Plus;
			resultList.Minus = auditInventory[0].Minus;
			resultList.ValueMinus = auditInventory[0].ValueMinus;
			resultList.ValueNotScanned = auditInventory[0].ValueNotScanned;
			////resultList.DiffAdmCenter = auditInventory[0].TranInLocation + auditInventory[0].TranBetweenAdmCenters + auditInventory[0].TranInAdmCenter;
			////resultList.DiffLocation = auditInventory[0].TranInLocation + auditInventory[0].TranBetweenAdmCenters + auditInventory[0].TranInAdmCenter;
			resultList.InventoryName = auditInventory[0].InventoryName;
            resultList.Location = auditInventory[0].Location;
            resultList.Region = auditInventory[0].Region;

            return resultList;
        }

        public Dto.Reporting.InventoryLabel GetInventoryLabelByFilters(int inventoryId, int? costCenterId)
        {
            Model.CostCenter costCenterInitial = null;

            List<Dto.Reporting.InventoryLabel> result = new List<Dto.Reporting.InventoryLabel>();

            Dto.Reporting.InventoryLabel invLabel = new Dto.Reporting.InventoryLabel
            {
                Details = new List<Dto.Reporting.InventoryLabelDetail>()
            };

            costCenterInitial = (costCenterId.HasValue && costCenterId > 0) ? _context.Set<Model.CostCenter>()
                .Include(l => l.Division)
                    .ThenInclude(c => c.Department)
                .Include(l => l.Room)
                .Single(l => l.Id == costCenterId) : null;

            var query =
                        from IA in _context.InventoryAssets
                        join asset in _context.Assets on IA.AssetId equals asset.Id
                        join costCenter in _context.CostCenters on IA.CostCenterIdInitial equals costCenter.Id
                        join inventory in _context.Inventories on IA.InventoryId equals inventory.Id
                        where IA.InventoryId == inventoryId && costCenter.Id == costCenterId && asset.AllowLabel == false

                        select new Dto.Reporting.InventoryLabelDetail
                        {
                            CostCenter = costCenter.Name,
                            InvNo = asset.InvNo,
                            Name = asset.Name,
                            Quantity = IA.QInitial
                        };

            invLabel.Details = query.ToList();

            var inventoryName = _context.Set<Model.Inventory>().Where(c => c.Id == inventoryId).FirstOrDefault();

            invLabel.Department = costCenterInitial != null && costCenterInitial.Division != null && costCenterInitial.Division.Department != null  ? costCenterInitial.Division.Department.Name : "";
            invLabel.Division = costCenterInitial != null && costCenterInitial.Division != null  ? costCenterInitial.Division.Name : "";
            invLabel.Room = costCenterInitial != null && costCenterInitial.Room != null ? costCenterInitial.Room.Name : "";
            invLabel.CostCenter = costCenterInitial.Code + " / " + costCenterInitial.Name;
            invLabel.CostCenterCode = costCenterInitial.Code;
            invLabel.Year = inventoryName.Description;

            return invLabel;
        }
    }
}

//        public AccNotice GetAccNoticeData(int documentId)
//        {
//            //Model.Document document = null;
//            //AccNotice reportData = null;
//            //AccNoticeAsset asset = null;
//            //List<AccNoticeAsset> assets = new List<AccNoticeAsset>();

//            //document = _context.Documents
//            //    .Include(d => d.Operations).ThenInclude(a => a.Asset.AssetDep)
//            //    .Include(d => d.Operations).ThenInclude(a => a.AdministrationInitial)
//            //    .Include(d => d.Operations).ThenInclude(a => a.AdministrationFinal)
//            //    .Where(d => d.Id == documentId).Single();

//            //reportData = new AccNotice();
//            //reportData.SourceDocumentNumber = document.DocNo1;
//            //reportData.DocumentDate = document.DocumentDate.ToShortDateString();

//            //foreach (Operation operation in document.Operations)
//            //{
//            //    reportData.ProviderAdministration = operation.AdministrationInitial.Name;
//            //    reportData.ReceiverAdministration = operation.AdministrationFinal.Name;

//            //    asset = new AccNoticeAsset();

//            //    asset.Name = operation.Asset.AssetName;
//            //    asset.InvNo = operation.Asset.InvNo;
//            //    asset.Barcode = string.Empty;
//            //    asset.MeasureUnit = "BUC";
//            //    asset.Quantity = 1;
//            //    asset.Value = (float)operation.Asset.AssetDep.ValueInv;

//            //    assets.Add(asset);
//            //}

//            //reportData.Assets = assets;

//            var reportData = new AccNotice()
//            {
//                ProviderAdministration = "Bucuresti",
//                ReceiverAdministration = "Timisoara",
//                SourceDocumentNumber = "5921BM01",
//                DocumentDate = DateTime.Now.ToString(),
//                Assets = new List<AccNoticeAsset>()
//                {
//                    new AccNoticeAsset()
//                    {
//                        Name = "Calculator HP",
//                        InvNo = "3200001",
//                        Barcode = "892344422",
//                        MeasureUnit = "BUC",
//                        Quantity = 2,
//                        Value = 3200
//                    },
//                    new AccNoticeAsset()
//                    {
//                        Name = "Laptop Dell",
//                        InvNo = "3200002",
//                        Barcode = "89277422",
//                        MeasureUnit = "BUC",
//                        Quantity = 5,
//                        Value = 4800
//                    },
//                    new AccNoticeAsset()
//                    {
//                        Name = "Server IBM",
//                        InvNo = "3200003",
//                        Barcode = "778934422",
//                        MeasureUnit = "BUC",
//                        Quantity = 1,
//                        Value = 8700
//                    }
//                }
//            };


//            return reportData;
//        }

//        public Annulement GetAnnulementData(int documentId)
//        {
//            Model.Document document = null;
//            Annulement reportData = null;

//            document = _context.Set<Model.Document>().Where(d => d.Id == documentId).Single();

//            var ops = from invCompOp in _context.Set<Model.InvCompOp>()
//                      join invComp in _context.Set<Model.InvComp>() on invCompOp.InvCompId equals invComp.Id
//                      join asset in _context.Set<Model.Asset>() on invComp.AssetId equals asset.Id
//                      join invState in _context.Set<Model.InvState>() on invCompOp.InvCompStateIdFinal equals invState.Id
//                      where invCompOp.DocumentId == documentId
//                      select new AnnulementAsset()
//                      {
//                          InvNo = asset.InvNo,
//                          Name = asset.AssetName,
//                          Barcode = asset.InvNo,
//                          MeasureUnit = "BUC",
//                          PricePerUnit = invComp.ValueInv,
//                          Quantity = 1,
//                          Value = invComp.ValueInv,
//                          RemainingValue = 0
//                      };

//            reportData = new Annulement();
//            reportData.CompanyAdress = "";
//            reportData.CompanyName = "";
//            reportData.CompanyRegistryNumber = "";
//            reportData.CompanyUniqueID = "";
//            reportData.DocumentDate = document.DocumentDate.ToString("dd/MM/yyyy");
//            reportData.DocumentNumber = document.Id.ToString();
//            reportData.Assets = ops.ToList();

//            return reportData;
//        }

//        public Commissioning GetCommissioningData(int documentId)
//        {
//            return new Commissioning()
//            {

//            };
//        }

//        public DifferenceList GetDifferenceListData(int documentId)
//        {
//            return new DifferenceList()
//            {

//            };
//        }

//        public ExternalEntryNote GetExternalEntryNoteData(int documentId)
//        {
//            Model.Document document = null;
//            ExternalEntryNote reportData = null;
//            ExternalEntryNoteAsset asset = null;
//            List<ExternalEntryNoteAsset> assets = new List<ExternalEntryNoteAsset>();

//            //document = _context.Documents
//            //    .Include(d => d.Operations).ThenInclude(a => a.Asset)
//            //    .Include(d => d.Operations).ThenInclude(a => a.AdministrationFinal)
//            //    .Where(d => d.Id == documentId).Single();

//            //reportData = new ExternalEntryNote();

//            //reportData.CompanyName = "";
//            //reportData.CompanyAdress = "";
//            //reportData.CompanyUniqueID = "";
//            //reportData.CompanyRegistryNumber = "";

//            //reportData.DocumentNumber = document.DocNo1;
//            //reportData.DocumentDate = document.DocumentDate.ToShortDateString();
//            //reportData.ReceiverAdministration = "";
//            //reportData.SourceCompanyName = "";
//            //reportData.SourceDocumentDate = "";
//            //reportData.SourceDocumentType = "";

//            //foreach (Operation operation in document.Operations)
//            //{
//            //    reportData.ReceiverAdministration = operation.AdministrationFinal.Name;

//            //    asset = new ExternalEntryNoteAsset();

//            //    asset.Name = operation.Asset.AssetName;
//            //    asset.Destination = operation.AdministrationFinal.Name;
//            //    asset.MeasureUnit = "BUC";
//            //    asset.SourceDocumentQuantity = "1";
//            //    asset.Quantity = "1";
//            //    asset.PricePerUnit = 0;
//            //    asset.Value = 0;

//            //    assets.Add(asset);
//            //}

//            //reportData.Assets = assets;

//            return reportData;
//        }

//        public FixedAssetReport GetFixedAssetReportData(string invNo)
//        {
//            return new FixedAssetReport()
//            {

//            };
//        }

//        public InventoryList GetInventoryListData(int locationId)
//        {
//            return new InventoryList()
//            {
//                Administration = "Gestiune",
//                CompanyAdress = "Cluj-Napoca",
//                CompanyName = "Office Depot",
//                CompanyRegistryNumber = "RO1453243",
//                CompanyUniqueID = "RO1453243",
//                Location = "Back Office",
//                EndDate = "",
//                Assets = new List<InventoryListAsset>()
//                {
//                    new InventoryListAsset()
//                    {
//                        AccountingGroup = "CC Cluj",
//                        AccountingValue = 435,
//                        Barcode = "123223432",
//                        CostCenter = "CC2 CLUJ",
//                        Depreciation = 25,
//                        HolderName = "Adrian Vasile",
//                        InvNo = "23235223",
//                        IsMinus = 0,
//                        IsPlus = 0,
//                        IsReal = 1,
//                        IsScriptic = 1,
//                        MinusDifference = 0,
//                        Name = "Laptop Lenovo",
//                        Observations = "probleme ecran",
//                        PlusDifference = 0,
//                        PricePerUnit = 235,
//                        Value = 470
//                    }
//                } 
//            };
//        }

//        public InvNoRegistry GetInvNoRegistryData()
//        {
//            IQueryable<InvNoRegistryAsset> invAssetQuery = null;
//            IQueryable<Model.Asset> assets = _context.Set<Model.Asset>().AsQueryable();
//            //IQueryable<Model.AssetInv> assetInvs = _context.Set<Model.AssetInv>().AsQueryable();
//            IQueryable<Model.AssetCategory> assetCategories = _context.Set<Model.AssetCategory>().AsQueryable();
//            IQueryable<Model.Department> departments = _context.Set<Model.Department>().AsQueryable();
//            IQueryable<Model.Employee> employees = _context.Set<Model.Employee>().AsQueryable();
//            IQueryable<Model.Location> locations = _context.Set<Model.Location>().AsQueryable();
//            IQueryable<Model.Room> rooms = _context.Set<Model.Room>().AsQueryable(); ;

//            invAssetQuery =
//                from asset in assets
//                //join assetInv in assetInvs on asset.Id equals assetInv.Id
//                join assetCategory in assetCategories on asset.AssetCategoryId equals assetCategory.Id
//                join employee in employees on asset.EmployeeId equals employee.Id
//                join department in departments on employee.DepartmentId equals department.Id
//                join room in rooms on asset.RoomId equals room.Id
//                join location in locations on room.LocationId equals location.Id

//                select new InvNoRegistryAsset()
//                {
//                    InvNo = asset.InvNo,
//                    Name = asset.AssetName,
//                    AssetCategory = assetCategory.Name,
//                    InternalCode = employee.InternalCode,
//                    FirstName = employee.FirstName,
//                    LastName = employee.LastName,
//                    LocationCode = location.Code,
//                    LocationName = location.Name,
//                    RoomCode = room.Code,
//                    RoomName = room.Name,
//                    DepartmentCode = department.Code,
//                    DepartmentName = department.Name
//                };

//            return new InvNoRegistry()
//            {
//                CompanyName = "Office Depot Service Center S.R.L.",
//                CompanyAdress = "Iulius Business Center, 53B Al.Vaida Voievod",
//                CompanyUniqueID = "400436, Cluj-Napoca",
//                CompanyRegistryNumber = "Romania",
//                Assets = invAssetQuery.ToList()
//            };
//        }

//        public InternalEntryNote GetInternalEntryNoteData(int documentId)
//        {
//            Model.Document document = null;
//            InternalEntryNote reportData = null;
//            InternalEntryNoteAsset asset = null;
//            List<InternalEntryNoteAsset> assets = new List<InternalEntryNoteAsset>();

//            //document = _context.Documents
//            //    .Include(d => d.Operations).ThenInclude(a => a.Asset).ThenInclude(a => a.AssetDep)
//            //    .Include(d => d.Operations).ThenInclude(a => a.AdministrationInitial)
//            //    .Include(d => d.Operations).ThenInclude(a => a.AdministrationFinal)
//            //    //.Include(d => d.Operations).ThenInclude(a => a.CostCenterInitial)
//            //    //.Include(d => d.Operations).ThenInclude(a => a.CostCenterFinal)
//            //    //.Include(d => d.Operations).ThenInclude(a => a.EmployeeInitial)
//            //    //.Include(d => d.Operations).ThenInclude(a => a.EmployeeFinal)
//            //    .Where(d => d.Id == documentId).Single();

//            //reportData = new InternalEntryNote();

//            //reportData.CompanyName = "";
//            //reportData.CompanyAdress = "";
//            //reportData.CompanyUniqueID = "";
//            //reportData.CompanyRegistryNumber = "";

//            //reportData.DocumentNumber = document.DocNo1;
//            //reportData.DocumentDate = document.DocumentDate.ToShortDateString();
//            //reportData.ReceiverAdministration = "";
//            //reportData.ProviderAdministration = "";
//            //reportData.SourceCompanyName = "";
//            //reportData.SourceDocumentDate = "";
//            //reportData.SourceDocumentType = "";

//            //foreach (Operation operation in document.Operations)
//            //{
//            //    reportData.ProviderAdministration = operation.AdministrationInitial.Name;
//            //    reportData.ReceiverAdministration = operation.AdministrationFinal.Name;

//            //    asset = new InternalEntryNoteAsset();

//            //    asset.Name = operation.Asset.AssetName;
//            //    asset.Destination = operation.AdministrationFinal.Name;
//            //    asset.MeasureUnit = "BUC";
//            //    asset.SourceDocumentQuantity = "1";
//            //    asset.Quantity = "1";
//            //    asset.PricePerUnit = (float)operation.Asset.AssetDep.ValueInv;
//            //    asset.Value = (float)operation.Asset.AssetDep.ValueInv;

//            //    assets.Add(asset);
//            //}

//            //reportData.Assets = assets;

//            return reportData;
//        }

//        public MovementReport GetMovementReportData(int documentId)
//        {
//            Model.Document document = null;
//            MovementReport reportData = null;
//            MovementReportAsset reportAsset = null;

//            //document = _context.Documents
//            //    .Include(d => d.Operations).ThenInclude(a => a.Asset)
//            //    .Include(d => d.Operations).ThenInclude(a => a.CostCenterInitial)
//            //    .Include(d => d.Operations).ThenInclude(a => a.CostCenterFinal)
//            //    .Include(d => d.Operations).ThenInclude(a => a.EmployeeInitial)
//            //    .Include(d => d.Operations).ThenInclude(a => a.EmployeeFinal)
//            //    .Where(d => d.Id == documentId).Single();

//            //reportData = new MovementReport();
//            //reportData.CompanyAdress = "";
//            //reportData.CompanyName = "";
//            //reportData.CompanyRegistryNumber = "";
//            //reportData.CompanyUniqueID = "";
//            //reportData.Assets = new List<MovementReportAsset>();


//            //foreach (Operation operation in document.Operations)
//            //{
//            //    reportAsset = new MovementReportAsset();

//            //    reportAsset.Barcode = operation.Asset.InvNoOld;
//            //    reportAsset.HolderName = operation.EmployeeFinal.FirstName + " " + operation.EmployeeFinal.LastName;
//            //    reportAsset.InvNo = operation.Asset.InvNo;
//            //    reportAsset.Name = operation.Asset.AssetName;

//            //    reportAsset.OldAdministration = operation.EmployeeInitial.FirstName + " " + operation.EmployeeInitial.LastName;
//            //    reportAsset.NewAdministration = operation.EmployeeFinal.FirstName + " " + operation.EmployeeFinal.LastName;

//            //    reportAsset.OldCostCenter = operation.CostCenterInitial.Name;
//            //    reportAsset.NewCostCenter = operation.CostCenterFinal.Name;

//            //    reportData.Assets.Add(reportAsset);
//            //}

//            return reportData;

//            //return new MovementReport()
//            //{
//            //    CompanyAdress = "Address",
//            //    CompanyName = "Allianz",
//            //    CompanyRegistryNumber = "RO898232",
//            //    CompanyUniqueID = "RO98882333",
//            //    Assets = new List<MovementReportAsset>()
//            //    {
//            //        new MovementReportAsset()
//            //        {
//            //            Barcode = "BC",
//            //            HolderName = "HN",
//            //            InvNo = "13233",
//            //            Name = "Laptop",
//            //            NewAdministration = "IT",
//            //            NewCostCenter = "Sediu",
//            //            NewSegment = "Segment1",
//            //            OldAdministration = "Magazie",
//            //            OldCostCenter = "Centrala",
//            //            OldSegment = "Segment2",
//            //            SAPnumber = "32553233",
//            //            SourceDocumentDate = "10/06/2016",
//            //            SourceDocumentNumber = "SAP33588823"
//            //        },
//            //        new MovementReportAsset()
//            //        {
//            //            Barcode = "BC",
//            //            HolderName = "HN",
//            //            InvNo = "13233",
//            //            Name = "Laptop",
//            //            NewAdministration = "IT",
//            //            NewCostCenter = "Sediu",
//            //            NewSegment = "Segment3",
//            //            OldAdministration = "Magazie",
//            //            OldCostCenter = "Centrala",
//            //            OldSegment = "Segment4",
//            //            SAPnumber = "32553233",
//            //            SourceDocumentDate = "10/06/2016",
//            //            SourceDocumentNumber = "SAP33588823"
//            //        }
//            //    }
//            //};
//        }
//    }
//}
