using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using AutoMapper;
using Optima.Fais.Repository;
using Newtonsoft.Json;
using Optima.Fais.Model.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Optima.Fais.Api.Controllers
{
    [Route("api/assetsni")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AssetNiController : GenericApiController<Model.AssetNi, Dto.AssetNiInvDet>
    {
        public AssetNiController(ApplicationDbContext context, IAssetNiRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {

        }

        [HttpPost]
        [Route("sync", Order = -1)]
        public IActionResult SaveAssetNi([FromBody] Dto.AssetNi ani)
        {
            Model.AssetNi assetNi = null;

            assetNi = _context.Set<Model.AssetNi>().SingleOrDefault(a => a.Code1 == ani.Code1);

            if (assetNi == null)
            {
                assetNi = new Model.AssetNi();

                assetNi.Code1 = ani.Code1;

                _context.Set<Model.AssetNi>().Add(assetNi);
            }
            else
            {
                _context.Set<Model.AssetNi>().Update(assetNi);
            }

            assetNi.InventoryId = ani.InventoryId;

            assetNi.AssetId = ani.AssetId;

            assetNi.RoomId = ani.RoomId;
            assetNi.EmployeeId = ani.EmployeeId;

            assetNi.InvStateId = ani.InvStateId;

            assetNi.Code2 = ani.Code2;
            assetNi.Name1 = ani.Name1;
            assetNi.Name2 = ani.Name2;

            assetNi.SerialNumber = ani.SerialNumber;

            assetNi.Model = ani.Model;
            assetNi.Producer = ani.Producer;

            assetNi.Quantity = ani.Quantity;

            assetNi.AllowLabel = ani.AllowLabel;
            assetNi.Info = ani.Info;

            assetNi.IsDeleted = ani.IsDeleted;

            _context.SaveChanges();

            return Ok(assetNi);
        }

        [HttpGet]
        [Route("filters")]
        public virtual IActionResult GetInvDetails(int? page, int? pageSize, string sortColumn, string sortDirection,
            string filter, string filters, string conditionType, int? inventoryId, int? assetId,
            string departmentIds, string employeeIds, string locationIds, string roomIds, string regionIds, string admCenterIds)
        {
            int totalItems = 0;
            List<int> dIds = null;
            List<int> eIds = null;
            List<int> lIds = null;
            List<int> rIds = null;
            List<int> regIds = null;
            List<int> admIds = null;

            List<string> fs = null;

            if ((filters != null) && (filters.Length > 0)) fs = JsonConvert.DeserializeObject<string[]>(filters).ToList();
            if ((departmentIds != null) && (departmentIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(departmentIds).ToList().Select(int.Parse).ToList();
            if ((employeeIds != null) && (employeeIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(employeeIds).ToList().Select(int.Parse).ToList();
            if ((locationIds != null) && (locationIds.Length > 0)) lIds = JsonConvert.DeserializeObject<string[]>(locationIds).ToList().Select(int.Parse).ToList();
            if ((regionIds != null) && (regionIds.Length > 0)) lIds = JsonConvert.DeserializeObject<string[]>(regionIds).ToList().Select(int.Parse).ToList();
            if ((admCenterIds != null) && (admCenterIds.Length > 0)) admIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();
            if ((roomIds != null) && (roomIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(roomIds).ToList().Select(int.Parse).ToList();


            List<Dto.AssetNi> items = (_itemsRepository as IAssetNiRepository).GetAssetNiByFilters(
                fs, conditionType, inventoryId, assetId, dIds, eIds, lIds, rIds, regIds, admIds,
                sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var result = new Dto.PagedResult<Dto.AssetNi>(items, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });

            return Ok(result);
        }

        [HttpPost]
        [Route("filters/reco")]
        public virtual IActionResult SaveReco([FromBody] Dto.AssetNiRecoSave reco)
        {
            Model.AssetNi assetNi = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.Asset asset = null;
            Model.AssetOp assetOp = null;
            Model.Document document = null;

            string documentTypeCode = "RECONCILIATION";
                                       

            assetNi = _context.Set<Model.AssetNi>().SingleOrDefault(a => a.Id == reco.AssetNiId);
            inventoryAsset = _context.Set<Model.InventoryAsset>().SingleOrDefault(a => a.AssetId == reco.AssetId);
            asset = _context.Set<Model.Asset>().Where(a => a.Id == reco.AssetId).OrderByDescending(a => a.ModifiedAt).Take(1).SingleOrDefault();

            var documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();

            document = new Model.Document();
            document.Approved = true;
            document.DocumentType = documentType;
            document.DocNo1 = string.Empty;
            document.DocNo2 = string.Empty;
            document.DocumentDate = DateTime.Now;
            document.RegisterDate = DateTime.Now;
            document.Partner = null;

            _context.Add(document);

            assetOp = new Model.AssetOp()
            {
                Asset = asset,
                DocumentId = document.Id,
                RoomIdInitial = asset.RoomId,
                RoomIdFinal = assetNi.RoomId,
                EmployeeIdInitial = asset.EmployeeId,
                EmployeeIdFinal = assetNi.EmployeeId,
                CostCenterIdInitial = asset.CostCenterId,
                CostCenterIdFinal = asset.CostCenterId,
                AssetCategoryIdInitial = asset.AssetCategoryId,
                AssetCategoryIdFinal = asset.AssetCategoryId,
                InvStateIdInitial = asset.InvStateId,
                InvStateIdFinal = assetNi.InvStateId,
                AdministrationIdInitial = asset.AdministrationId,
                AdministrationIdFinal = asset.AdministrationId,
                AccSystemId = 1
            };

            _context.Add(assetOp);

            assetNi.AssetId = reco.AssetId;
            assetNi.IsDeleted = true;
            asset.TempReco = assetNi.Code1;
            asset.TempName = assetNi.Name1;
            asset.SerialNumber = assetNi.SerialNumber.Trim().Length > 0 ? assetNi.SerialNumber : asset.SerialNumber;
            inventoryAsset.RoomIdFinal = assetNi.RoomId;
            inventoryAsset.EmployeeIdFinal = assetNi.EmployeeId;
            inventoryAsset.SerialNumber = assetNi.SerialNumber;
            inventoryAsset.StateIdFinal = assetNi.InvStateId;
            inventoryAsset.Model = assetNi.Model;
            inventoryAsset.Producer = assetNi.Producer;
            inventoryAsset.CostCenterIdFinal = inventoryAsset.CostCenterIdInitial;
            inventoryAsset.QFinal = inventoryAsset.QInitial;
            inventoryAsset.Info = assetNi.Info;
            inventoryAsset.ModifiedAt = assetNi.ModifiedAt;
            inventoryAsset.ModifiedBy = assetNi.ModifiedBy;


            var entityFiles = _context.EntityFiles.Where(e => e.EntityId == reco.AssetNiId).ToList();
            foreach (Model.EntityFile entityFile in entityFiles)
            {
                Model.EntityFile newEntityFile = new Model.EntityFile
                {
                    EntityId = reco.AssetId,
                    EntityTypeId = entityFile.EntityTypeId,
                    FileType = entityFile.FileType,
                    Info = entityFile.Info,
                    Name = entityFile.Name,
                    Size = entityFile.Size,
                    StoredAs = entityFile.StoredAs
                };

                _context.EntityFiles.Add(newEntityFile);
            }


            _context.SaveChanges();

            return Ok(reco);
        }

        [HttpPost]
        [Route("filters/transfer")]
        public virtual IActionResult TransferAssetNi([FromBody] Dto.AssetNiTransferSave transfer)
        {
            Model.AssetNi assetNi = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.Asset asset = null;
            Model.AssetOp assetOp = null;
            Model.Document document = null;
            Model.AssetType assetType = null;
            Model.AssetClass assetClass = null;
            Model.AssetClassType assetClassType = null;
            Model.Administration administration = null;
            Model.AssetAC assetAc = null;
            Model.AssetDep assetDep = null;
            Model.AssetInv assetInv = null;
            Model.AccSystem accSystem = null;
            Model.AccMonth accMonth = null;
            Model.AssetDepMD assetDepMD = null;
            Model.AssetAdmMD assetAdmMD = null;
            Model.AssetState assetState = null;
            Model.AssetCategory assetCategory = null;
            Model.Inventory inventory = null;
            Model.Partner partner = null;

            string documentTypeCode = "TRANSFERASSETNI";
            string codeDefault = "_NSP";
            string nameDefault = "_Nespecificat";
            string assetClassTypeDefault = "RAS";
            string assetStateCodeDefault = "FUNCTION";
            string assetStateNameDefault = "In functiune";


            assetNi = _context.Set<Model.AssetNi>().SingleOrDefault(a => a.Id == transfer.AssetNiId);

            inventory = _context.Set<Model.Inventory>().Where(i => !i.IsDeleted && i.Active).OrderByDescending(i => i.Id).FirstOrDefault();
            assetType = _context.Set<Model.AssetType>().Where(a => (a.Code == codeDefault.Trim())).FirstOrDefault();

            if (assetType == null)
            {
                assetType = new Model.AssetType
                {
                    Code = codeDefault,
                    Name = nameDefault,
                    IsDeleted = false
                };

                _context.Set<Model.AssetType>().Add(assetType);

            }

            

            assetClassType = _context.Set<Model.AssetClassType>().Where(a => (a.Code == assetClassTypeDefault)).FirstOrDefault();

            if (assetClassType == null)
            {
                assetClassType = new Model.AssetClassType
                {
                    Code = assetClassTypeDefault,
                    Name = assetClassTypeDefault,
                    IsDeleted = false
                };
                _context.Set<Model.AssetClassType>().Add(assetClassType);
            }

            assetClass = _context.Set<Model.AssetClass>().Where(a => (a.Code == assetClassTypeDefault.Trim())).FirstOrDefault();

            if (assetClass == null)
            {
                assetClass = new Model.AssetClass
                {
                    Code = assetClassTypeDefault.Trim(),
                    Name = assetClassTypeDefault.Trim(),
                    IsDeleted = false,
                    AssetClassType = assetClassType
                };
                _context.Set<Model.AssetClass>().Add(assetClass);
            }

            accSystem = _context.Set<Model.AccSystem>().Where(a => (a.Code == assetClassTypeDefault)).FirstOrDefault();

            if (accSystem == null)
            {
                accSystem = new Model.AccSystem
                {
                    AssetClassType = assetClassType,
                    Code = assetClassTypeDefault,
                    Name = assetClassTypeDefault,
                    IsDeleted = false
                };
                _context.Set<Model.AccSystem>().Add(accSystem);
            }

            administration = _context.Set<Model.Administration>().Where(a => (a.Code == assetClassTypeDefault.Trim())).FirstOrDefault();

            if (administration == null)
            {
                administration = new Model.Administration
                {
                    Code = assetClassTypeDefault.Trim(),
                    Name = assetClassTypeDefault.Trim(),
                    IsDeleted = false
                };

                _context.Set<Model.Administration>().Add(administration);
            }

            assetState = _context.Set<Model.AssetState>().Where(a => (a.Code == assetStateCodeDefault)).FirstOrDefault();

            if (assetState == null)
            {
                assetState = new Model.AssetState
                {
                    Code = assetStateCodeDefault,
                    Name = assetStateNameDefault,
                    IsDeleted = false
                };
                _context.Set<Model.AssetState>().Add(assetState);
            }

            assetCategory = _context.Set<Model.AssetCategory>().Where(a => a.Code == codeDefault.Trim()).FirstOrDefault();
            if (assetCategory == null)
            {
                assetCategory = new Model.AssetCategory
                {
                    Code = codeDefault,
                    Name = nameDefault,
                    IsDeleted = false
                };
                _context.Set<Model.AssetCategory>().Add(assetCategory);
            }

            partner = _context.Set<Model.Partner>().Where(a => a.FiscalCode == codeDefault).FirstOrDefault();
            if (partner == null)
            {
                partner = new Model.Partner
                {
                    Name = nameDefault,
                    FiscalCode = codeDefault,
                    RegistryNumber = string.Empty,
                    IsDeleted = false
                };

                _context.Set<Model.Partner>().Add(partner);
            }


            var documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();

            document = new Model.Document
            {
                Approved = true,
                DocumentType = documentType,
                DocNo1 = string.Empty,
                DocNo2 = string.Empty,
                DocumentDate = DateTime.Now,
                RegisterDate = DateTime.Now,
                Partner = partner
            };

            _context.Add(document);

            asset = new Model.Asset()
            {
                InvNo = assetNi.Code1.Trim(),
                Name= assetNi.Name1.Trim(),
                Document = document,
                AssetTypeId = assetType.Id,
                PurchaseDate = DateTime.Now,
                Quantity = assetNi.Quantity,
                Validated = true,
                Administration = administration,
                AssetState = assetState,
                AssetCategory = assetCategory,
                SerialNumber = assetNi.SerialNumber

            };

            asset.EmployeeId = assetNi.EmployeeId;
            asset.RoomId = assetNi.RoomId;
            asset.InvStateId = assetNi.InvStateId;
            asset.CostCenterId = assetNi.RoomId;



            _context.Add(asset);

            inventoryAsset = new Model.InventoryAsset()
            {
                Asset = asset,
                Inventory = inventory,
                QInitial = assetNi.Quantity,
                QFinal = assetNi.Quantity,
                SerialNumber = assetNi.SerialNumber,
                Info = assetNi.Info,
                Model= assetNi.Model,
                Producer = assetNi.Producer

            };

            inventoryAsset.EmployeeIdInitial = assetNi.EmployeeId;
            inventoryAsset.EmployeeIdFinal = assetNi.EmployeeId;
            inventoryAsset.RoomIdInitial = assetNi.RoomId;
            inventoryAsset.RoomIdFinal = assetNi.RoomId;
            inventoryAsset.StateIdInitial = assetNi.InvStateId;
            inventoryAsset.StateIdFinal = assetNi.InvStateId;
            inventoryAsset.CostCenterIdInitial = assetNi.RoomId;
            inventoryAsset.CostCenterIdFinal = assetNi.RoomId;

            _context.Add(inventoryAsset);


            assetAc = new Model.AssetAC
            {
                AssetClassType = assetClassType,
                Asset = asset,
                AssetClass = assetClass,
                AssetClassIn = assetClass
            };


            _context.Set<Model.AssetAC>().Add(assetAc);

            var monthSum = 0;


            assetDep = new Model.AssetDep
            {
                AccSystem = accSystem,
                Asset = asset,
                DepPeriod = (int)monthSum,
                DepPeriodIn = (int)monthSum,
                DepPeriodMonth = 1,
                DepPeriodMonthIn = 0,
                DepPeriodRem = (int)monthSum,
                DepPeriodRemIn = (int)monthSum,
                UsageStartDate = DateTime.Now,
                ValueDep = 0,
                ValueDepIn = 0,
                ValueDepPU = 0,
                ValueDepPUIn = 0,
                ValueDepYTD = 0,
                ValueDepYTDIn = 0,
                ValueInv = 0,
                ValueInvIn = 0,
                ValueRem = 0,
                ValueRemIn = 0
            };

            _context.Set<Model.AssetDep>().Add(assetDep);


            assetInv = new Model.AssetInv
            {
                Asset = asset,
                AllowLabel = assetNi.AllowLabel.ToString() == "ETICHETABIL" ? true : false,
                Barcode = assetNi.Code1,
                Info = assetNi.Info,
                InvName = assetNi.Name1,
                InvNoOld = assetNi.Name1,
                Model = assetNi.Model,
                Producer = assetNi.Producer
            };

            _context.Set<Model.AssetInv>().Add(assetInv);

            accMonth = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).FirstOrDefault();

            assetDepMD = new Model.AssetDepMD
            {
                AccMonth = accMonth,
                AccSystem = accSystem,
                Asset = asset,
                UsefulLife = (int)monthSum,
                TotLifeInpPeriods = 1,
                RemLifeInPeriods = 1,
                AccumulDep = 0,
                BkValFYStart = 0,
                DepForYear = 0,
                CurrentAPC = 0,
                PosCap = 0
            };

            _context.Set<Model.AssetDepMD>().Add(assetDepMD);

            assetAdmMD = new Model.AssetAdmMD
            {
                AccMonth = accMonth,
                Asset = asset,
                Administration = administration,
                AssetCategory = assetCategory,
                AssetState = assetState,
                AssetType = assetType,
                DepartmentId = null,
               
            };

            assetAdmMD.EmployeeId = assetNi.EmployeeId;
            assetAdmMD.RoomId = assetNi.RoomId;
            assetAdmMD.AssetStateId = assetNi.InvStateId;
            assetAdmMD.CostCenterId = assetNi.RoomId;

            _context.Set<Model.AssetAdmMD>().Add(assetAdmMD);



            assetOp = new Model.AssetOp()
            {
                Asset = asset,
                DocumentId = document.Id,
                AssetCategoryInitial = assetCategory,
                AssetCategoryFinal = assetCategory,
                AdministrationIdInitial = asset.AdministrationId,
                AdministrationIdFinal = asset.AdministrationId,
                AccSystem = accSystem,
                AdministrationInitial = administration,
                AdministrationFinal = administration,
                AssetStateInitial = assetState,
                AssetStateFinal = assetState,
                Document = document,
                Info = assetNi.Info
            };

            assetOp.EmployeeIdInitial = assetNi.EmployeeId;
            assetOp.EmployeeIdFinal = assetNi.EmployeeId;
            assetOp.RoomIdInitial = assetNi.RoomId;
            assetOp.RoomIdFinal = assetNi.RoomId;
            assetOp.InvStateIdInitial = assetNi.InvStateId;
            assetOp.InvStateIdFinal = assetNi.InvStateId;
            assetOp.CostCenterIdInitial = assetNi.RoomId;
            assetOp.CostCenterIdFinal = assetNi.RoomId;

            _context.Add(assetOp);

            assetNi.Asset = asset;

            _context.Update(assetNi);

            _context.SaveChanges();

            return Ok(transfer);
        }

        [HttpGet]
        [Route("filters/exportAsetNiOtp")]
        public virtual IActionResult ExportAssetNi(int? page, int? pageSize, string sortColumn, string sortDirection,
           string filter, string filters, string conditionType, int? inventoryId, int? assetId,
           string departmentIds, string employeeIds, string locationIds, string roomIds)
        {
            int totalItems = 0;
            List<int> dIds = null;
            List<int> eIds = null;
            List<int> lIds = null;
            List<int> rIds = null;
            List<int> regIds = null;

            List<string> fs = null;

            if ((filters != null) && (filters.Length > 0)) fs = JsonConvert.DeserializeObject<string[]>(filters).ToList();
            if ((departmentIds != null) && (departmentIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(departmentIds).ToList().Select(int.Parse).ToList();
            if ((employeeIds != null) && (employeeIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(employeeIds).ToList().Select(int.Parse).ToList();
            if ((locationIds != null) && (locationIds.Length > 0)) lIds = JsonConvert.DeserializeObject<string[]>(locationIds).ToList().Select(int.Parse).ToList();
            if ((roomIds != null) && (roomIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(roomIds).ToList().Select(int.Parse).ToList();


            List<Dto.AssetNi> items = (_itemsRepository as IAssetNiRepository).GetAssetNiByFilters(
                fs, conditionType, inventoryId, assetId, dIds, eIds, lIds, rIds, regIds, null,
                sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            using (ExcelPackage package = new ExcelPackage())
            {

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("OTP");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Nr. crt";
                worksheet.Cells[1, 2].Value = "Temporara";
                worksheet.Cells[1, 3].Value = "Denumire";
                worksheet.Cells[1, 4].Value = "Serial number";
                worksheet.Cells[1, 5].Value = "Cod regiune";
                worksheet.Cells[1, 6].Value = "Regiune";
                worksheet.Cells[1, 7].Value = "Cod directie";
                worksheet.Cells[1, 8].Value = "Directie";
                worksheet.Cells[1, 9].Value = "Cod camera";
                worksheet.Cells[1, 10].Value = "Camera";
                worksheet.Cells[1, 11].Value = "Marca";
                worksheet.Cells[1, 12].Value = "Nume";
                worksheet.Cells[1, 13].Value = "Prenume";
                worksheet.Cells[1, 14].Value = "Cantitate";
                worksheet.Cells[1, 15].Value = "Etichetabil";
                worksheet.Cells[1, 16].Value = "Observatii";
                worksheet.Cells[1, 17].Value = "Producator";
                worksheet.Cells[1, 18].Value = "Model";
              
                int recordIndex = 2;
                foreach (var item in items)
                {
                    worksheet.Cells[recordIndex, 1].Value = worksheet.Dimension.End.Row;
                    worksheet.Cells[recordIndex, 2].Value = item.Code1;
                    worksheet.Cells[recordIndex, 3].Value = item.Name1;
                    worksheet.Cells[recordIndex, 4].Value = item.SerialNumber;
                    worksheet.Cells[recordIndex, 5].Value = item.RegionCode;
                    worksheet.Cells[recordIndex, 6].Value = item.RegionName;
                    worksheet.Cells[recordIndex, 7].Value = item.LocationCode;
                    worksheet.Cells[recordIndex, 8].Value = item.LocationName;
                    worksheet.Cells[recordIndex, 9].Value = item.RoomCode;
                    worksheet.Cells[recordIndex, 10].Value = item.RoomName;
                    worksheet.Cells[recordIndex, 11].Value = item.InternalCode;
                    worksheet.Cells[recordIndex, 12].Value = item.LastName;
                    worksheet.Cells[recordIndex, 13].Value = item.FirstName;
                    worksheet.Cells[recordIndex, 14].Value = item.Quantity;
                    worksheet.Cells[recordIndex, 15].Value = item.AllowLabel == true ? "DA" : "NU";
                    worksheet.Cells[recordIndex, 16].Value = item.Info;
                    worksheet.Cells[recordIndex, 17].Value = item.Producer;
                    worksheet.Cells[recordIndex, 18].Value = item.Model;
                    recordIndex++;
                }






                //  worksheet.Column(3).Style.Numberformat.Format = "MMMM";
                //   worksheet.Column(12).Style.Numberformat.Format = "yyyy-mm-dd";





                using (var cells = worksheet.Cells[1, 1, 1, 18])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.Khaki);

                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Export OTP.xlsx"
                };

                return result;

            }
        }

        [HttpPut("filters/recoverAssetNi/{assetId}/{inventoryId}")]
        public void RecoverAssetNi(int assetId, int inventoryId)
        {

            var recoverAssetNiId = (_itemsRepository as IAssetNiRepository).RecoverAssetNi(assetId, inventoryId);
            _context.SaveChanges();
        }
    }
}
