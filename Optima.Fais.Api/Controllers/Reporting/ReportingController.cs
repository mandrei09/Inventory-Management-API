
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optima.Fais.Data;
using Optima.Fais.Dto.Reporting;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Optima.Fais.Api.Controllers.Reporting
{
    [Route("api/reporting")]
    public class ReportingController : Controller
    {
        IReportingRepository _repository = null;
        private readonly ApplicationDbContext _context;

        public ReportingController(IReportingRepository repository, ApplicationDbContext context)
        {
            this._repository = repository;
            this._context = context;
        }

        //[Route("inventorylistv3")]
        //public IActionResult GetInventoryListV3Data(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int employeeId, string reportType, bool? custody, string internalCode)
        //{
        //    var inventoryListV3Data = this._repository.GetInventoryListV2ByFilters(inventoryId, admCenterId, costCenterId, regionId, locationId, null, employeeId, reportType, custody, internalCode);

        //    return Ok(inventoryListV3Data);
        //}

        //[Route("inventorylistv2")]
        //public IActionResult GetInventoryListV2Data(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, string reportType, bool? custody)
        //{
        //    var inventoryListV2Data = this._repository.GetInventoryListV2ByFilters(inventoryId, admCenterId, costCenterId, regionId, locationId, divisionId, null, reportType, custody, null);

        //    return Ok(inventoryListV2Data);
        //}

        [Route("inventorylistemployees")]
        public IActionResult GetInventoryListEmployeesData(int? inventoryId, int? admCenterId)
        {
            var inventoryListV3Data = this._repository.GetInventoryListEmployees(inventoryId, admCenterId);

            if (inventoryListV3Data == null) return BadRequest();

            return Ok(inventoryListV3Data);
        }

        [Route("inventorylistreconciliations")]
        public IActionResult GetInventoryListReconciliationsData(int? inventoryId, int? admCenterId)
        {
            var inventoryListV3Data = this._repository.GetInventoryListReconciliations(inventoryId, admCenterId);

            if (inventoryListV3Data == null) return BadRequest();

            return Ok(inventoryListV3Data);
        }

        [Route("inventorylistuserscans")]
        public IActionResult GetInventoryListUserScansData(int? inventoryId, int? employeeId, int? admCenterId)
        {
            var inventoryListV3Data = this._repository.GetInventoryListUserScans(inventoryId, employeeId, admCenterId);

            if (inventoryListV3Data == null) return BadRequest();

            return Ok(inventoryListV3Data);
        }

        [Route("inventorylistuserbuildingscans")]
        public IActionResult GetInventoryListUserBuildingScansData(int? inventoryId)
        {
            var inventoryListV3Data = this._repository.GetInventoryListUserBuildingScans(inventoryId);

            if (inventoryListV3Data == null) return BadRequest();

            return Ok(inventoryListV3Data);
        }

        [Route("inventorylistv3")]
        public IActionResult GetInventoryListV3Data(int? inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int employeeId, string reportType, bool? custody, string internalCode)
        {

            var inventoryListV3Data = this._repository.GetInventoryListV2ByFilters(inventoryId, admCenterId, costCenterId, regionId, locationId, employeeId, reportType, custody, internalCode);

            if (inventoryListV3Data == null) return BadRequest();

            return Ok(inventoryListV3Data);
        }

        //[Route("inventorylistv2")]
        //public IActionResult GetInventoryListV2Data(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, string reportType, bool? custody)
        //{

        //    var inventoryListV2Data = this._repository.GetInventoryListV2ByFilters(inventoryId, admCenterId, costCenterId, regionId, null, locationId, reportType, custody);

        //    return Ok(inventoryListV2Data);
        //}

        [Route("inventorylistv3GROUP")]
        public IActionResult GetInventoryListV3GROUPData(int? inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int employeeId, string reportType, bool? custody, string internalCode)
        {

            var inventoryListV3Data = this._repository.GetInventoryListV2ByFilters(inventoryId, admCenterId, costCenterId, regionId, locationId, employeeId, reportType, custody, internalCode);

            if (inventoryListV3Data == null) return BadRequest();

            return Ok(inventoryListV3Data);
        }

        [Route("inventorylistv2")]
        public IActionResult GetInventoryListV2Data(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, string reportType, bool? custody)
        {

            var inventoryListV2Data = this._repository.GetInventoryListV2ByFilters(inventoryId, admCenterId, costCenterId, regionId, locationId, null, reportType, custody, null);

            return Ok(inventoryListV2Data);
        }

        [Route("inventorylistv2multiple")]
        public IActionResult GetInventoryListV2MultipleData(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, string reportType, bool? custody, string reportId)
        {
            
            var inventoryListV2Data = this._repository.GetInventoryListV2MultipleByFilters(inventoryId, admCenterId, regionId, locationId, reportType);

            return Ok(inventoryListV2Data);
        }

        [Route("inventorylistthales")]
        public IActionResult GetInventoryListWGData(int inventoryId, int? locationId, string reportType)
        {
            var inventoryListV2Data = this._repository.GetInventoryListWGByFilters(inventoryId, locationId, reportType);

            return Ok(inventoryListV2Data);
        }

        [Route("generalList")]
        public IActionResult GetGeneralListData(int? accMonthId, int inventoryId, int regionId, string reportType)
        {
            var generalListData = this._repository.GetGeneralList(accMonthId, inventoryId, regionId, reportType);

            return Ok(generalListData);
        }

        [Route("employeeReport")]
        public IActionResult GetEmployeeReportData(string reportType, string internalCode)
        {
            var employeeReportData = this._repository.GetEmployeeReportData(reportType, internalCode);

            return Ok(employeeReportData);
        }

        [Route("assetOperations")]
        public IActionResult GetAssetOperationsData(int? locationId)
        {
            var assetOperationsData = this._repository.GetAssetOperationData(locationId);

            return Ok(assetOperationsData);
        }

        [Route("inventorylistv2Total")]
        public IActionResult GetInventoryListV2Total(int inventoryId)
        {
            var inventoryListV2Data = this._repository.GetInventoryListV2Total(inventoryId);

            return Ok(inventoryListV2Data);
        }

        [Route("transferoutv1")]
        public IActionResult GetTransferOutV1Data(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, bool? custody)
        {

            var transferOutV1Data = this._repository.GetTransferOutV1ByFilters(inventoryId, admCenterId, costCenterId, regionId, locationId, custody);

            return Ok(transferOutV1Data);
        }

        [Route("transferoutv1multiple")]
        public IActionResult GetTransferOutV1MultipleData(int inventoryId, int? admCenterId, int? regionId, int? locationId)
        {
          
            var transferOutV1MultipleData = this._repository.GetTransferOutV1MultipleByFilters(inventoryId, admCenterId, regionId, locationId);

            return Ok(transferOutV1MultipleData);
        }

        [Route("transferinv1")]
        public IActionResult GetTransferInV1Data(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, string reportType, bool? custody)
        //public IActionResult GetTransferInV1Data(int inventoryId, int locationId)
        {
            var transferInV1Data = this._repository.GetTransferInV1ByFilters(inventoryId, admCenterId, costCenterId, regionId, locationId, reportType, custody);
            //var transferInV1Data = this._repository.GetTransferInV1ByFilters(inventoryId, locationId);

            return Ok(transferInV1Data);
        }

        [Route("transferinv1multiple")]
        public IActionResult GetTransferInV1MultipleData(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, string reportType, bool? custody)
        //public IActionResult GetTransferInV1Data(int inventoryId, int locationId)
        {
           
            var transferInV1Data = this._repository.GetTransferInV1MultipleByFilters(inventoryId, admCenterId, costCenterId, regionId, locationId, reportType, custody);
            //var transferInV1Data = this._repository.GetTransferInV1ByFilters(inventoryId, locationId);

            return Ok(transferInV1Data);
        }

        //[Route("inventorylist")]
        //public IActionResult GetInventoryListData(int locationId)
        //{
        //    var inventoryListData = this._repository.GetInventoryListData(locationId);

        //    return Ok(inventoryListData);
        //}

        //[Route("accnotice/{documentId}")]
        //public IActionResult GetAccNoticeData(int documentId)
        //{
        //    var reportData = this._repository.GetAccNoticeData(documentId);
        //    return Ok(reportData);
        //}

        //[Route("invnoregistry")]
        //public ActionResult GetInvNoRegistryData()
        //{
        //    var reportData = this._repository.GetInvNoRegistryData();
        //    return Ok(reportData);
        //}

        //[Route("api/~/api/reports/accnotice/{documentId}")]
        //public AccNotice GetAccNoticeData(int documentId)
        //{
        //    return this._repository.GetAccNoticeData(documentId);
        //}

        //[Route("annulement/{documentId}")]
        //public IActionResult GetAnnulementData(int documentId)
        //{
        //    var data = this._repository.GetAnnulementData(documentId);
        //    return Ok(data);
        //}

        //[Route("api/~/api/reports/commissioning")]
        //public Commissioning GetCommissioningData(string documentId)
        //{
        //    return this._repository.GetCommissioningData(documentId);
        //}

        //[Route("api/~/api/reports/diflist")]
        //public DifferenceList GetDifferenceListData(string documentId)
        //{
        //    return this._repository.GetDifferenceListData(documentId);
        //}

        //[Route("api/~/api/reports/externalentrynote/{documentId}")]
        //public ExternalEntryNote GetExternalEntryNoteData(int documentId)
        //{
        //    return this._repository.GetExternalEntryNoteData(documentId);
        //}

        //[Route("api/~/api/reports/internalentrynote/{documentId}")]
        //public InternalEntryNote GetInternalEntryNoteData(int documentId)
        //{
        //    return this._repository.GetInternalEntryNoteData(documentId);
        //}

        //[Route("api/~/api/reports/asset")]
        //public FixedAssetReport GetFixedAssetReportData(string invNo)
        //{
        //    return this._repository.GetFixedAssetReportData(invNo);
        //}

        [Route("movementproviding/{documentId}/{assetOpId?}")]
        public ActionResult GetMovementProvidingData(int documentId, int? assetOpId)
        {
            var data = this._repository.GetMovementProvidingData(documentId, assetOpId);
            return Ok(data);
        }


		[Route("scrabproviding/{documentId}/{assetOpId?}")]
		public ActionResult GetScrabProvidingData(int documentId, int? assetOpId)
		{
			var data = this._repository.GetScrabProvidingData(documentId, assetOpId);
			return Ok(data);
		}

		[Route("pif")]
        public ActionResult GetPifData(Guid reportId, int assetId)
        {
            var reportType = "pif";
            var data = this._repository.GetReportAssetData(reportId, assetId);
            return Ok(data);
        }


		[Route("nir")]
		public ActionResult GetNIRData(Guid reportId, int assetId)
		{
			var data = this._repository.GetReportNIRData(reportId, assetId);
			return Ok(data);
		}

		[Route("pv")]
        public ActionResult GetPvData(Guid reportId, int assetId)
        {
            var reportType = "pv";
            var data = this._repository.GetReportAssetData(reportId, assetId);
            return Ok(data);
        }

        [Route("inventorylistapanova")]
        public IActionResult GetInventoryListApaNovaInternData(int? inventoryId, int? locationId)
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
            //            //join room1 in _context.Rooms on IA.RoomIdFinal equals room1.Id

            //            where IA.InventoryId == inventoryId && ((IA.RoomIdFinal == room.Id) || (IA.RoomIdInitial == room.Id && IA.RoomIdFinal == null))

            //            select new InventoryListDetailApaNova
            //            {
            //                AssetId = asset.Id,
            //                InvNo = asset.ERPCode,
            //                InvNoNew = asset.InvNo,
            //                Description = asset.Name,
            //                DescriptionNew = asset.Name,
            //                SerialNumber = asset.SerialNumber,
            //                SerialNumberNew = IA.SerialNumber,
            //                PifDate = asset.PurchaseDate,
            //                InventoryDate = IA.ModifiedAt.Value,
            //                InventoryType = IA.Asset.AssetCategory.Name,
            //                AssetCategory = assetInv.Model,
            //                Model = IA.Producer,
            //                Dimension = IA.Model,
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



            //var list = query.OrderBy(i => i.InvNoNew).ToList();

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



            Directory.CreateDirectory(string.Format(location.Code));


            ExcelPackage _pck = new ExcelPackage(new FileInfo(location.Code + "\\" + location.Code + ".xlsx"));

            var ws = _pck.Workbook.Worksheets.Add(location.Code);
            ws.Cells[1, 1].Value = "Id";
            ws.Cells[1, 2].Value = "Nr inv alocat Optima/reconciliat";
            ws.Cells[1, 3].Value = "Nr de alocat 2018";
            ws.Cells[1, 4].Value = "Denumire noua( cf inventar 2018)_ data de MI";
            ws.Cells[1, 5].Value = "denumire finala- colectata Optima";
            ws.Cells[1, 6].Value = "SN scriptic";
            ws.Cells[1, 7].Value = "SN fizic";
            ws.Cells[1, 8].Value = "Data PIF";
            ws.Cells[1, 9].Value = "Data inventariere 2018";
            ws.Cells[1, 10].Value = "TIP inventariere";
            ws.Cells[1, 11].Value = "Tip echipament";
            ws.Cells[1, 12].Value = "Model";
            ws.Cells[1, 13].Value = "Dimensiune";
            ws.Cells[1, 14].Value = "User";
            ws.Cells[1, 15].Value = "QtyInitial";
            ws.Cells[1, 16].Value = "QtyFinal";
            ws.Cells[1, 17].Value = "Observatie Optima";
            ws.Cells[1, 18].Value = "LocationCodeInitial2018";
            ws.Cells[1, 19].Value = "LocationNameInitial2018";
            ws.Cells[1, 20].Value = "LocationCodeFinal2018";
            ws.Cells[1, 21].Value = "LocationNameFinal2018";
            ws.Cells[1, 22].Value = "InfoNew";
            ws.Cells[1, 23].Value = "Imagini";


            int recordIndex = 2;
            int assetCount = 0;
            int sameAssetCount = 0;
            InventoryListDetailApaNova prevItem = null;

            foreach (var item in invListApaNova.InventoryListDetailInternMod)
            {
                if ((prevItem == null) || (prevItem.InvNo != item.InvNo))
                {
                    if (sameAssetCount > 1)
                    {
                        for (int i = 1; i < 23; i++)
                        {
                            ws.Cells[recordIndex - sameAssetCount, i, recordIndex - 1, i].Merge = true;
                        }
                    }

                    sameAssetCount = 1;
                    assetCount++;
                    ws.Cells[recordIndex, 1].Value = assetCount;// ws.Dimension.End.Row;
                    ws.Cells[recordIndex, 2].Value = item.InvNo;
                    //ws.Cells[recordIndex, 3].Value = item.InvNoNew;
                    //ws.Cells[recordIndex, 4].Value = item.Description;
                    //ws.Cells[recordIndex, 5].Value = item.DescriptionNew;
                    //ws.Cells[recordIndex, 6].Value = item.SerialNumber;
                    //ws.Cells[recordIndex, 7].Value = item.SerialNumberNew;
                    //ws.Cells[recordIndex, 8].Value = item.PifDate;
                    //ws.Cells[recordIndex, 8].Style.Numberformat.Format = "yyyy-MM-dd";
                    //ws.Cells[recordIndex, 9].Value = item.InventoryDate;
                    //ws.Cells[recordIndex, 9].Style.Numberformat.Format = "yyyy-MM-dd";
                    //ws.Cells[recordIndex, 10].Value = item.InventoryType;
                    //ws.Cells[recordIndex, 11].Value = item.AssetCategory;
                    //ws.Cells[recordIndex, 12].Value = item.Model;
                    //ws.Cells[recordIndex, 13].Value = item.Dimension;
                    //ws.Cells[recordIndex, 14].Value = item.UserName;
                    //ws.Cells[recordIndex, 15].Value = item.Qinitial;
                    //ws.Cells[recordIndex, 16].Value = item.QFinal;
                    //ws.Cells[recordIndex, 17].Value = item.Info;
                    //ws.Cells[recordIndex, 18].Value = item.LocationCodeInitial;
                    //ws.Cells[recordIndex, 19].Value = item.LocationNameInitial;
                    //ws.Cells[recordIndex, 20].Value = item.LocationCodeFinal;
                    //ws.Cells[recordIndex, 21].Value = item.LocationNameFinal;
                    //ws.Cells[recordIndex, 22].Value = item.InfoNew;
                }
                else
                {
                    sameAssetCount++;

                }

                ws.Cells[recordIndex, 23].Hyperlink = new Uri(item.ImageLink, UriKind.Relative);
                ws.Cells[recordIndex, 23].Value = "Foto " + sameAssetCount.ToString();

                prevItem = item;
                recordIndex++;
            }

            if (sameAssetCount > 1)
            {
                for (int i = 1; i < 23; i++)
                {
                    ws.Cells[recordIndex - sameAssetCount, i, recordIndex - 1, i].Merge = true;
                }
            }

            for (int i = 1; i < 24; i++)
            {
                ws.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.Tan);
                ws.Cells[1, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));

            }

            ws.Row(1).Height = 45.75;
            ws.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.View.FreezePanes(2, 1);

            using (var cells = ws.Cells[2, 1, invListApaNova.InventoryListDetailInternMod.Count() + 1, 23])
            {
                cells.Style.Font.Bold = true;
                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cells.Style.Fill.BackgroundColor.SetColor(Color.Khaki);
                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cells.Style.Font.SetFromFont(new Font("Arial Narrow", 12));

            }



            ws.Column(1).AutoFit();
            ws.Column(2).AutoFit();
            ws.Column(3).AutoFit();
            ws.Column(4).AutoFit();
            ws.Column(5).AutoFit();
            ws.Column(6).AutoFit();
            ws.Column(7).AutoFit();
            ws.Column(8).AutoFit();
            ws.Column(9).AutoFit();
            ws.Column(10).AutoFit();
            ws.Column(11).AutoFit();
            ws.Column(12).AutoFit();
            ws.Column(13).AutoFit();
            ws.Column(14).AutoFit();
            ws.Column(15).AutoFit();
            ws.Column(16).AutoFit();
            ws.Column(17).AutoFit();
            ws.Column(18).AutoFit();
            ws.Column(19).AutoFit();
            ws.Column(20).AutoFit();
            ws.Column(21).AutoFit();
            ws.Column(22).AutoFit();
            ws.Column(23).AutoFit();
            ws.Column(23).Style.WrapText = true;



            _pck.Save();



            var inventoryListV3Data = this._repository.GetInventoryListApaNovaImages(inventoryId, locationId);

            if (inventoryListV3Data == null) return BadRequest();

            return Ok(inventoryListV3Data);
        }

        [HttpGet("locationdownload/{regionId}/{locationId}/{roomId}")]
        public IActionResult LocationDownload(int regionId, int locationId, int roomId)
        {
            var zip = GetLocationDownloadMemoryStream(regionId, locationId, roomId);

            HttpContext.Response.ContentType = "application/zip";
            FileContentResult result = new FileContentResult(zip, "application/zip")
            {
                FileDownloadName = "poze.zip"
            };

            return result;
        }

        private byte[] GetLocationDownloadMemoryStream(int regionId, int locationId, int roomId)
        {
            Model.EntityType entityType = _context.Set<Model.EntityType>().Where(e => e.Code == "LOCATION").Single();
            Model.EntityType entityTypeAsset = _context.Set<Model.EntityType>().Where(e => e.Code == "ASSET").Single();
            Model.Region region = _context.Regions.Where(r => r.Id == regionId).FirstOrDefault();
            Model.Location location = _context.Locations.Where(r => r.Id == locationId).FirstOrDefault();
            Model.Room room = _context.Rooms.Where(r => r.Id == roomId).FirstOrDefault();


            //  Model.Room room = _context.Rooms.Where(r => r.LocationId == locationId).FirstOrDefault();
            //  List<Model.EntityFile> locationFiles = _context.Set<Model.EntityFile>().Where(e => e.EntityId == locationId && e.EntityTypeId == entityType.Id && e.IsDeleted == false).ToList();

            MemoryStream outputMemStream = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);
            byte[] buffer = null;
            ZipEntry newEntry = null;
            //List<string> missingFiles = new List<string>();
            System.Text.StringBuilder sb = new StringBuilder();

            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

            //foreach (Model.EntityFile locationFile in locationFiles)
            //{
            //    string path = @"..\FaisAnb\upload\" + locationFile.StoredAs;
            //   // string path = @"C:\Adrian\ApaNova\upload\" + locationFile.StoredAs;
            //    if (System.IO.File.Exists(path))
            //    {
            //        FileInfo fi = new FileInfo(path);

            //        string entryName = @"location\" + locationFile.StoredAs;
            //        entryName = ZipEntry.CleanName(entryName);
            //        newEntry = new ZipEntry(entryName);
            //        newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity
            //        newEntry.Size = fi.Length;

            //        zipStream.PutNextEntry(newEntry);

            //        // Zip the file in buffered chunks
            //        // the "using" will close the stream even if an exception occurs
            //        buffer = new byte[4096];
            //        //using (FileStream streamReader = File.OpenRead(filename))
            //        using (FileStream streamReader = System.IO.File.OpenRead(path))
            //        {
            //            StreamUtils.Copy(streamReader, zipStream, buffer);
            //        }
            //        zipStream.CloseEntry();
            //        //break;
            //    }
            //    else
            //    {
            //        sb.AppendLine(path);
            //    }
            //}



            var query = from ia in _context.InventoryAssets
                        join entityFile in _context.EntityFiles on ia.AssetId equals entityFile.EntityId
                        where ia.RoomIdFinal == room.Id && entityFile.IsDeleted == false
                        select entityFile.StoredAs;
            List<string> assetFiles = query.ToList();

            foreach (string assetFile in assetFiles)
            {
                string path = @"..\FaisDemoMobile\upload\" + assetFile;
                //string path = @"c:\Adrian\Demo\upload\" + assetFile;

                //FileInfo fi = System.IO.File.Exists(path) ? new FileInfo(path) : null;

                if (System.IO.File.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);
                    string entryName = @"assets\" + assetFile;
                    entryName = ZipEntry.CleanName(entryName);
                    newEntry = new ZipEntry(entryName);
                    newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                    newEntry.Size = fi.Length;

                    zipStream.PutNextEntry(newEntry);

                    // Zip the file in buffered chunks
                    // the "using" will close the stream even if an exception occurs
                    buffer = new byte[4096];
                    //using (FileStream streamReader = File.OpenRead(filename))
                    using (FileStream streamReader = System.IO.File.OpenRead(path))
                    {
                        StreamUtils.Copy(streamReader, zipStream, buffer);
                    }
                    zipStream.CloseEntry();
                }
                else
                {
                    sb.AppendLine(path);
                }

                //break;
            }

            MemoryStream ms = null;
            var excelReport = GetMegaImageReportData(8, regionId, locationId, roomId);
            ms = new MemoryStream(excelReport);
            buffer = new byte[4096];

            string reportEntryName = region.Code + ".xlsx";
            reportEntryName = ZipEntry.CleanName(reportEntryName);
            newEntry = new ZipEntry(reportEntryName);
            newEntry.DateTime = DateTime.Now;
            zipStream.PutNextEntry(newEntry);

            StreamUtils.Copy(ms, zipStream, buffer);
            zipStream.CloseEntry();

            ms = new MemoryStream(Encoding.ASCII.GetBytes(sb.ToString()));
            buffer = new byte[4096];
            string reportEntryMissingFiles = "files.txt";
            reportEntryMissingFiles = ZipEntry.CleanName(reportEntryMissingFiles);
            newEntry = new ZipEntry(reportEntryMissingFiles);
            newEntry.DateTime = DateTime.Now;
            zipStream.PutNextEntry(newEntry);

            StreamUtils.Copy(ms, zipStream, buffer);
            zipStream.CloseEntry();

            zipStream.IsStreamOwner = false;    // False stops the Close also Closing the underlying stream.
            //zipStream.Close();          // Must finish the ZipOutputStream before using outputMemStream.
            zipStream.Finish();

            outputMemStream.Position = 0;
            return outputMemStream.ToArray();
        }

        private byte[] GetMegaImageReportData(int inventoryId, int regionId, int locationId, int roomId)
        {
            InventoryListApaNova invListApaNova = new InventoryListApaNova();
            Model.Inventory inventory = null;
            Model.Region region = null;
            Model.Location location = null;
            Model.Room room = null;
            Model.EntityType entityTypeAsset = _context.Set<Model.EntityType>().Where(e => e.Code == "ASSET").Single();
            Model.EntityType entityTypeLocation = _context.Set<Model.EntityType>().Where(e => e.Code == "LOCATION").Single();

            inventory = _context.Set<Model.Inventory>().Where(i => i.Id == inventoryId).FirstOrDefault();
            region = _context.Set<Model.Region>().Where(i => i.Id == regionId).FirstOrDefault();
            location = _context.Set<Model.Location>().Where(i => i.Id == locationId).FirstOrDefault();
            room = _context.Set<Model.Room>().Where(i => i.Id == roomId).FirstOrDefault();
            var entityFilesAsset = _context.EntityFiles.Where(e => ((e.EntityTypeId == entityTypeAsset.Id) && (e.IsDeleted == false)));
            var entityFilesLocation = _context.EntityFiles.Where(e => ((e.EntityTypeId == entityTypeLocation.Id) && (e.IsDeleted == false)));

            List<Model.InventoryListApn> items = _context.Set<Model.InventoryListApn>().FromSql("ApnReportImages {0}, {1}", inventoryId, locationId).ToList();

            invListApaNova.InventoryListDetailInternMod = new List<InventoryListDetailApaNova>();

            //var query =
            //            from IA in _context.InventoryAssets

            //            join asset in _context.Assets on IA.AssetId equals asset.Id
            //            join assetInv in _context.AssetInvs on asset.Id equals assetInv.AssetId
            //            join roomInitial in _context.Rooms on IA.RoomIdInitial equals roomInitial.Id
            //            join assetCategory in _context.AssetCategories on asset.AssetCategoryId equals assetCategory.Id
            //            join employee in _context.Employees on asset.EmployeeId equals employee.Id
            //            join ef in entityFilesAsset on asset.Id equals ef.EntityId into joined
            //            from entityFile in joined.DefaultIfEmpty()

            //            where IA.InventoryId == inventoryId && ((IA.RoomIdFinal == room.Id) || (IA.RoomIdInitial == room.Id && IA.RoomIdFinal == null))



            //            select new InventoryListDetailApaNova
            //            {
            //                AssetId = asset.Id,
            //                InvNo = asset.ERPCode,
            //                InvNoNew = asset.InvNo,
            //                Description = asset.Name,
            //                DescriptionNew = asset.Name,
            //                SerialNumber = asset.SerialNumber,
            //                SerialNumberNew = IA.SerialNumber,
            //                PifDate = asset.PurchaseDate,
            //           //     InventoryDate = IA.ModifiedAt.Value,
            //                InventoryType = IA.Asset.AssetCategory.Name,
            //                AssetCategory = assetInv.Model,
            //                Model = IA.Producer,
            //                Dimension = IA.Model,
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
            //                ImageLink = entityFile != null ? entityFile.StoredAs : "",
            //              //  ModifiedAt = IA.ModifiedAt.Value,
            //               // IsDeleted = entityFile != null ? entityFile.IsDeleted : false
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
                    ImageLink = item.StoredAs,
                    GpsCoordinates = item.GpsCoordinates
                   

                });
            }



            //var list = query.OrderBy(i => i.InvNoNew).ToList();

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



            //  Directory.CreateDirectory(string.Format(location.Code));


            //ExcelPackage _pck = new ExcelPackage(new FileInfo(location.Code + "\\" + location.Code + ".xlsx"));
            int recordIndex = 0;
            ExcelPackage _pck = new ExcelPackage();

            var ws = _pck.Workbook.Worksheets.Add(location.Code);

            ws.Cells[1, 1].Value = "Nr. crt.";
            ws.Cells[1, 2].Value = "Locatia (Structura) initial";
            ws.Cells[1, 3].Value = "Locatia (Structura) inventar";
            ws.Cells[1, 4].Value = "Categoria";
            ws.Cells[1, 5].Value = "Numar inventar";
            ws.Cells[1, 6].Value = "Nr. inventar parinte";
            ws.Cells[1, 7].Value = "Denumire";
            ws.Cells[1, 8].Value = "Cantitate scriptic";
            ws.Cells[1, 9].Value = "Cantitate faptic";
            ws.Cells[1, 10].Value = "UM";
            ws.Cells[1, 11].Value = "Centru de cost initial";
            ws.Cells[1, 12].Value = "Centru cost inventar";
            ws.Cells[1, 13].Value = "Detalii stare initial";
            ws.Cells[1, 14].Value = "Detalii stare inventar";
            ws.Cells[1, 15].Value = "Utilizator initial";
            ws.Cells[1, 16].Value = "Marca utilizator initial";
            ws.Cells[1, 17].Value = "Utilizator inventar";
            ws.Cells[1, 18].Value = "Marca utilizator inventar";
            ws.Cells[1, 19].Value = "Cod strada initial";
            ws.Cells[1, 20].Value = "Adresa initial";
            ws.Cells[1, 21].Value = "Cod strada inventar";
            ws.Cells[1, 22].Value = "Adresa inventar";

            ws.Cells[1, 23].Value = "Cod punct lucru initial";
            ws.Cells[1, 24].Value = "Denumire Punct de lucru initial";
            ws.Cells[1, 25].Value = "Cod punct lucru inventar";
            ws.Cells[1, 26].Value = "Denumire Punct de lucru inventar";
            ws.Cells[1, 27].Value = "Nr de serie";
            ws.Cells[1, 28].Value = "Observatii";
            ws.Cells[1, 29].Value = "Coordonate GPS";
            ws.Cells[1, 30].Value = "Link Poze";


            recordIndex = 2;
            int assetCount = 0;
            int sameAssetCount = 0;
            InventoryListDetailApaNova prevItem = null;

            foreach (var item in invListApaNova.InventoryListDetailInternMod)
            {
                if ((prevItem == null) || (prevItem.InvNo != item.InvNo))
                {
                    if (sameAssetCount > 1)
                    {
                        for (int i = 1; i < 30; i++)
                        {
                            ws.Cells[recordIndex - sameAssetCount, i, recordIndex - 1, i].Merge = true;
                        }
                    }

                    sameAssetCount = 1;
                    assetCount++;
                    ws.Cells[recordIndex, 1].Value = assetCount;// ws.Dimension.End.Row;
                    ws.Cells[recordIndex, 2].Value = item.LocationNameInitial;
                    ws.Cells[recordIndex, 3].Value = item.LocationNameFinal;
                    ws.Cells[recordIndex, 4].Value = item.AssetCategory;
                    ws.Cells[recordIndex, 5].Value = item.InvNo;
                    ws.Cells[recordIndex, 6].Value = item.InvNoParent;
                    ws.Cells[recordIndex, 7].Value = item.Description;
                    ws.Cells[recordIndex, 8].Value = item.Qinitial;
                    ws.Cells[recordIndex, 9].Value = item.QFinal;
                    ws.Cells[recordIndex, 10].Value = item.Um;
                    ws.Cells[recordIndex, 11].Value = item.CostCenterNameInitial;
                    ws.Cells[recordIndex, 12].Value = item.CostCenterNameFinal;
                    ws.Cells[recordIndex, 13].Value = item.AssetStateInitial;
                    ws.Cells[recordIndex, 14].Value = item.AssetStateFinal;
                    ws.Cells[recordIndex, 15].Value = item.UserEmployeeFullNameInitial;
                    ws.Cells[recordIndex, 16].Value = item.UserEmployeeInternalCodeInitial;
                    ws.Cells[recordIndex, 17].Value = item.UserEmployeeFullNameFinal;
                    ws.Cells[recordIndex, 18].Value = item.UserEmployeeInternalCodeFinal;
                    ws.Cells[recordIndex, 19].Value = item.StreetCodeInitial;
                    ws.Cells[recordIndex, 20].Value = item.StreetNameInitial;
                    ws.Cells[recordIndex, 21].Value = item.StreetCodeFinal;
                    ws.Cells[recordIndex, 22].Value = item.StreetNameFinal;
                    ws.Cells[recordIndex, 23].Value = item.RoomCodeInitial;
                    ws.Cells[recordIndex, 24].Value = item.RoomNameInitial;
                    ws.Cells[recordIndex, 25].Value = item.RoomCodeFinal;
                    ws.Cells[recordIndex, 26].Value = item.RoomNameFinal;

                    ws.Cells[recordIndex, 27].Value = item.SerialNumber;
                    ws.Cells[recordIndex, 28].Value = item.Info;
                    ws.Cells[recordIndex, 29].Value = item.GpsCoordinates;
                    ws.Cells[recordIndex, 30].Value = item.ImageLink;
                   

                   // ws.Cells[recordIndex, 8].Style.Numberformat.Format = "yyyy-MM-dd";
                }
                else
                {
                    sameAssetCount++;
                }

                if (item.ImageLink.Length > 0)
                {
                    ws.Cells[recordIndex, 30].Hyperlink = new Uri(@"assets\" + item.ImageLink, UriKind.Relative);
                    ws.Cells[recordIndex, 30].Value = "Foto " + sameAssetCount.ToString();
                }
                ws.Cells[recordIndex, 30].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[recordIndex, 30].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 150, 221));
                ws.Cells[recordIndex, 30].Style.Font.Color.SetColor(Color.White);

                prevItem = item;
                recordIndex++;
            }

            if (sameAssetCount > 1)
            {
                for (int i = 1; i < 30; i++)
                {
                    ws.Cells[recordIndex - sameAssetCount, i, recordIndex - 1, i].Merge = true;
                }
            }

            for (int i = 1; i < 31; i++)
            {
                ws.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(16, 74, 132));
                ws.Cells[1, i].Style.Font.Color.SetColor(Color.White);
                ws.Cells[1, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));

            }

            ws.Row(1).Height = 45.75;
            ws.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.View.FreezePanes(2, 1);

            using (var cells = ws.Cells[2, 1, invListApaNova.InventoryListDetailInternMod.Count() + 1, 30])
            {
                cells.Style.Font.Bold = true;
                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 150, 221));
                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cells.Style.Font.SetFromFont(new Font("Arial Narrow", 12));
                cells.Style.Font.Color.SetColor(Color.White);

            }



            ws.Column(1).AutoFit();
            ws.Column(2).AutoFit();
            ws.Column(3).AutoFit();
            ws.Column(4).AutoFit();
            ws.Column(5).AutoFit();
            ws.Column(6).AutoFit();
            ws.Column(7).AutoFit();
            ws.Column(8).AutoFit();
            ws.Column(9).AutoFit();
            ws.Column(10).AutoFit();
            ws.Column(11).AutoFit();
            ws.Column(12).AutoFit();
            ws.Column(13).AutoFit();
            ws.Column(14).AutoFit();
            ws.Column(15).AutoFit();
            ws.Column(16).AutoFit();
            ws.Column(17).AutoFit();
            ws.Column(18).AutoFit();
            ws.Column(19).AutoFit();
            ws.Column(20).AutoFit();
            ws.Column(21).AutoFit();
            ws.Column(22).AutoFit();
            ws.Column(23).AutoFit();
            ws.Column(24).AutoFit();
            ws.Column(25).AutoFit();
            ws.Column(26).AutoFit();
            ws.Column(27).AutoFit();
            ws.Column(28).AutoFit();
            ws.Column(29).AutoFit();
            ws.Column(30).AutoFit();
            ws.Column(30).Style.WrapText = true;

            var queryLocationFiles =
                from entityFile in entityFilesLocation
                where entityFile.EntityId == location.Id
                select entityFile.StoredAs;
            List<string> locationFiles = queryLocationFiles.ToList();

            int fileCount = 0;
            int hOffset = 2;
            int vOffset = 2;
            int columns = 10;
            int rowIndex = 0;
            int columnIndex = 0;
            var wsLocationImages = _pck.Workbook.Worksheets.Add("foto magazin");
            foreach (string locationFile in locationFiles)
            {
                columnIndex = fileCount % columns;
                columnIndex = (columnIndex + 1) * hOffset;

                rowIndex = fileCount / columns;
                rowIndex = (rowIndex + 1) * vOffset;

                fileCount++;

                wsLocationImages.Cells[rowIndex, columnIndex].Hyperlink = new Uri(@"location\" + locationFile, UriKind.Relative);
                wsLocationImages.Cells[rowIndex, columnIndex].Value = "Foto " + fileCount.ToString();
                wsLocationImages.Cells[rowIndex, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsLocationImages.Cells[rowIndex, columnIndex].Style.Fill.BackgroundColor.SetColor(Color.Khaki);
            }

            return _pck.GetAsByteArray();
        }

        [Route("inventoryReportByAdmCenter")]
        public IActionResult ExportMail(int inventoryId)
        {

            List<Model.InventoryReportByAdmCenter> items = _context.Set<Model.InventoryReportByAdmCenter>().FromSql("InventoryReportByAdmCenter {0}", inventoryId).ToList();

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Situatie_scanari");

                worksheet.Cells[1, 1].Value = "Nr. Crt.";
                worksheet.Cells[1, 2].Value = "Regiune";
                worksheet.Cells[1, 3].Value = "Localitate";
                worksheet.Cells[1, 4].Value = "Nr. total de repere";
                worksheet.Cells[1, 5].Value = "Nr. de repere scanate";
                worksheet.Cells[1, 6].Value = "Procent";


                int recordIndex = 2;
                int assetCount = 0;
                foreach (var item in items)
                {
                    assetCount++;

                    worksheet.Cells[recordIndex, 1].Value = assetCount;
                    worksheet.Cells[recordIndex, 2].Value = item.AdmCenterCode;
                    worksheet.Cells[recordIndex, 3].Value = item.AdmCenterName;
                    worksheet.Cells[recordIndex, 4].Value = item.Total;
                    worksheet.Cells[recordIndex, 5].Value = item.Scanned;
                    worksheet.Cells[recordIndex, 6].Value = item.Procentage + "%";
                    recordIndex++;
                }

                using (var range = worksheet.Cells[1, 1, items.Count() + 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(216, 228, 188));
                    range.Style.Font.Color.SetColor(Color.Black);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                worksheet.Cells.AutoFitColumns();

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Situatie_scanari.xlsx"
                };

                return result;

            }
        }

        [Route("exportSAP")]
        public ActionResult ExportSAP(int inventoryId, int? admCenterId, int reportType)
        {
            InventorySAP invSAP = new InventorySAP();
            Model.Inventory inventory = null;
            Model.AdmCenter admCenter = null;

            inventory = _context.Set<Model.Inventory>().Where(i => i.Id == inventoryId).FirstOrDefault();


            invSAP.SAPDetail = new List<InventorySAPDetail>();


            if (reportType == 1)
            {

                invSAP.SAPDetail = new List<InventorySAPDetail>();

                var query = _context.InventoryAssets
                    .Include(a => a.Asset)
                        .Include(ri => ri.RoomInitial)
                            .Include(i => i.EmployeeInitial)
                        .Include(i => i.CostCenterInitial)
                            .ThenInclude(c => c.AdmCenter)
                        .Include(i => i.RoomFinal)
                        .Include(i => i.EmployeeFinal)
                        .Include(i => i.CostCenterFinal)
                            .ThenInclude(c => c.AdmCenter)
                            .Where(i => i.InventoryId == inventoryId && (i.StateIdFinal != 1 && i.StateIdFinal != null))
                            .Select(item => new InventorySAPDetail()
                            {
                                InvNo = item.Asset.InvNo,
                                Description = item.Asset.Name,
                                DescriptionPlus = "",
                                CostCenterCode = item.CostCenterIdFinal != null ? item.CostCenterFinal.Code : string.Empty,
                                CostCenterName = item.CostCenterIdFinal != null ? item.CostCenterFinal.Name : string.Empty,
                                AdmCenterName = item.CostCenterIdFinal != null ? item.CostCenterFinal.AdmCenterId != null ? item.CostCenterFinal.AdmCenter.Name : string.Empty : string.Empty,
                                EmployeeInternalCode = item.EmployeeIdFinal != null ? item.EmployeeFinal.InternalCode : string.Empty,
                                EmployeeFirstName = item.EmployeeIdFinal != null ? item.EmployeeFinal.FirstName : string.Empty,
                                EmployeeLastName = item.EmployeeIdFinal != null ? item.EmployeeFinal.LastName : string.Empty,
                                SerialNumber = item.SerialNumber,
                                RoomName = item.RoomIdFinal != null ? item.RoomFinal.Name : string.Empty,
                                InventoryName = inventory.Description,
                                InventoryDate = inventory.Start.Value,
                                CostCenterNameInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.Name : string.Empty,
                                CostCenterNameFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.Name : string.Empty
                            });

                if (admCenterId != null)
                {
                    admCenter = _context.Set<Model.AdmCenter>().Where(i => i.Id == admCenterId).FirstOrDefault();
                    query = query.Where(a => a.AdmCenterName == admCenter.Name);
                }

                invSAP.SAPDetail = query.OrderBy(a => a.AdmCenterName).ToList();

                invSAP.InventoryName = inventory.Description.Trim();


                StringBuilder sb = new StringBuilder();
                sb.Append("Numar inventar" + "\t"
                        + "Descriere mijloc fix" + "\t"
                        + "Descriere suplimentara" + "\t"
                        + "Centru de cost" + "\t"
                        + "Descriere centru de cost" + "\t"
                        + "Marca personal" + "\t"
                        + "Nume salariat" + "\t"
                        + "Nr serie mijloc fix" + "\t"
                        + "Camera" + "\t"
                        + "Decizie" + "\t"
                        + "Data inventar" + "\t"
                        + "Numar mijloc fix" + "\t"
                        + "Subnumar mijloc fix" + "\r\n");

                foreach (var item in invSAP.SAPDetail)
                {
                    sb.Append(item.InvNo).Append("\t");
                    sb.Append(item.Description).Append("\t");
                    sb.Append("").Append("\t");
                    sb.Append(item.CostCenterCode).Append("\t");
                    sb.Append(item.CostCenterName).Append("\t");
                    //  sb.Append(item.AdmCenterName).Append("\t");
                    sb.Append(item.EmployeeInternalCode).Append("\t");
                    sb.Append(item.EmployeeFirstName + " " + item.EmployeeLastName).Append("\t");
                    sb.Append(item.SerialNumber).Append("\t");
                    sb.Append(item.RoomName).Append("\t");
                    sb.Append("").Append("\t");
                    sb.Append(string.Format("{0}.{1}.{2}", item.InventoryDate.Day.ToString().Length == 1 ? "0" + item.InventoryDate.Day : item.InventoryDate.Day.ToString(),
                                                           item.InventoryDate.Month.ToString().Length == 1 ? "0" + item.InventoryDate.Month : item.InventoryDate.Month.ToString(),
                                                           item.InventoryDate.Year)).Append("\t");
                    sb.Append("").Append("\t");
                    sb.Append("").Append("\t");

                    sb.AppendLine();
                }

                return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "Casari.txt");


            }

            else if (reportType == 2)

            {
                invSAP.SAPDetail = new List<InventorySAPDetail>();

                var query = _context.InventoryAssets
                    .Include(a => a.Asset)
                        .Include(ri => ri.RoomInitial)
                            .Include(i => i.EmployeeInitial)
                        .Include(i => i.CostCenterInitial)
                            .ThenInclude(c => c.AdmCenter)
                        .Include(i => i.RoomFinal)
                        .Include(i => i.EmployeeFinal)
                        .Include(i => i.CostCenterFinal)
                            .ThenInclude(c => c.AdmCenter)
                            .Where(i => i.InventoryId == inventoryId && (i.RoomIdFinal == null))
                            .Select(item => new InventorySAPDetail()
                            {
                                InvNo = item.Asset.InvNo,
                                Description = item.Asset.Name,
                                DescriptionPlus = "",
                                CostCenterCode = item.CostCenterInitial.Code,
                                CostCenterName = item.CostCenterInitial.Name,
                                AdmCenterName = item.CostCenterInitial.AdmCenter.Name,
                                EmployeeInternalCode = item.EmployeeInitial.InternalCode,
                                EmployeeFirstName = item.EmployeeInitial.FirstName,
                                EmployeeLastName = item.EmployeeInitial.LastName,
                                SerialNumber = item.SerialNumber,
                                RoomName = item.RoomInitial.Name,
                                InventoryName = inventory.Description,
                                InventoryDate = inventory.Start.Value,
                                CostCenterNameInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.Name : string.Empty,
                                CostCenterNameFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.Name : string.Empty
                            });

                if (admCenterId != null)
                {
                    admCenter = _context.Set<Model.AdmCenter>().Where(i => i.Id == admCenterId).FirstOrDefault();
                    query = query.Where(a => a.AdmCenterName == admCenter.Name);
                }

                invSAP.SAPDetail = query.OrderBy(a => a.AdmCenterName).ToList();

                invSAP.InventoryName = inventory.Description.Trim();


                StringBuilder sb = new StringBuilder();
                sb.Append("Numar inventar" + "\t"
                        + "Descriere mijloc fix" + "\t"
                        + "Descriere suplimentara" + "\t"
                        + "Centru de cost" + "\t"
                        + "Descriere centru de cost" + "\t"
                        + "Marca personal" + "\t"
                        + "Nume salariat" + "\t"
                        + "Nr serie mijloc fix" + "\t"
                        + "Camera" + "\t"
                        + "Decizie" + "\t"
                        + "Data inventar" + "\t"
                        + "Numar mijloc fix" + "\r\n");

                foreach (var item in invSAP.SAPDetail)
                {
                    sb.Append(item.InvNo).Append("\t");
                    sb.Append(item.Description).Append("\t");
                    sb.Append("").Append("\t");
                    sb.Append(item.CostCenterCode).Append("\t");
                    sb.Append(item.CostCenterName).Append("\t");
                    //  sb.Append(item.AdmCenterName).Append("\t");
                    sb.Append(item.EmployeeInternalCode).Append("\t");
                    sb.Append(item.EmployeeFirstName + " " + item.EmployeeLastName).Append("\t");
                    sb.Append(item.SerialNumber).Append("\t");
                    sb.Append(item.RoomName).Append("\t");
                    sb.Append("").Append("\t");
                    //sb.Append(item.InventoryDate.ToString("dd.MM.yyyy")).Append("\t");
                    sb.Append(string.Format("{0}.{1}.{2}", item.InventoryDate.Day.ToString().Length == 1 ? "0" + item.InventoryDate.Day : item.InventoryDate.Day.ToString(),
                                                           item.InventoryDate.Month.ToString().Length == 1 ? "0" + item.InventoryDate.Month : item.InventoryDate.Month.ToString(),
                                                           item.InventoryDate.Year)).Append("\t");
                    sb.Append("").Append("\t");

                    sb.AppendLine();
                }

                return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "Minusuri.txt");
            }

            else if (reportType == 3)

            {
                invSAP.SAPDetail = new List<InventorySAPDetail>();

                var query = _context.InventoryAssets
                    .Include(a => a.Asset)
                        .Include(ri => ri.RoomInitial)
                            .Include(i => i.EmployeeInitial)
                        .Include(i => i.CostCenterInitial)
                            .ThenInclude(c => c.AdmCenter)
                        .Include(i => i.RoomFinal)
                        .Include(i => i.EmployeeFinal)
                        .Include(i => i.CostCenterFinal)
                            .ThenInclude(c => c.AdmCenter)
                            .Where(i => i.InventoryId == inventoryId && ((i.RoomIdInitial != i.RoomIdFinal) || (i.EmployeeIdInitial != i.EmployeeIdFinal) || (i.CostCenterIdInitial != i.CostCenterIdFinal)) && i.RoomIdFinal != null && ((i.CostCenterIdFinal != null) && (i.CostCenterInitial.AdmCenterId == i.CostCenterFinal.AdmCenterId)))
                            .Select(item => new InventorySAPDetail()
                            {
                                InvNo = item.Asset.InvNo,
                                Description = item.Asset.Name,
                                DescriptionPlus = "",
                                CostCenterCodeInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.Code : string.Empty,
                                AdmCenterNameInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.AdmCenterId != null ? item.CostCenterInitial.AdmCenter.Name : string.Empty : string.Empty,
                                CostCenterCodeFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.Code : string.Empty,
                                CostCenterNameInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.Name : string.Empty,
                                CostCenterNameFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.Name : string.Empty,
                                AdmCenterNameFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.AdmCenterId != null ? item.CostCenterFinal.AdmCenter.Name : string.Empty : string.Empty,
                                EmployeeInternalCode = item.EmployeeIdFinal != null ? item.EmployeeFinal.InternalCode : string.Empty,
                                EmployeeFirstName = item.EmployeeIdFinal != null ? item.EmployeeFinal.FirstName : string.Empty,
                                EmployeeLastName = item.EmployeeIdFinal != null ? item.EmployeeFinal.LastName : string.Empty,
                                SerialNumber = item.SerialNumber,
                                RoomName = item.RoomIdFinal != null ? item.RoomFinal.Name : string.Empty,
                                InventoryName = inventory.Description,
                                InventoryDate = inventory.Start.Value
                            });

                if (admCenterId != null)
                {
                    admCenter = _context.Set<Model.AdmCenter>().Where(i => i.Id == admCenterId).FirstOrDefault();
                    query = query.Where(a => a.AdmCenterNameInitial == admCenter.Name || a.AdmCenterNameFinal == admCenter.Name);
                }

                invSAP.SAPDetail = query.OrderBy(a => a.AdmCenterName).ToList();

                invSAP.InventoryName = inventory.Description.Trim();


                StringBuilder sb = new StringBuilder();
                sb.Append("Numar inventar" + "\t"
                        + "Descriere mijloc fix" + "\t"
                        + "Descriere suplimentara" + "\t"
                        + "Centru de cost final" + "\t"
                        + "Descriere centru de cost final" + "\t"
                        + "Centru de cost" + "\t"
                        + "Descriere centru de cost" + "\t"
                        + "Marca personal" + "\t"
                        + "Nume salariat" + "\t"
                        + "Nr serie mijloc fix" + "\t"
                        + "Camera" + "\t"
                        + "Decizie" + "\t"
                        + "Data inventar" + "\t"
                        + "Numar mijloc fix" + "\r\n");

                foreach (var item in invSAP.SAPDetail)
                {
                    sb.Append(item.InvNo).Append("\t");
                    sb.Append(item.Description).Append("\t");
                    sb.Append("").Append("\t");
                    sb.Append(item.CostCenterCodeFinal).Append("\t");
                    //  sb.Append(item.AdmCenterNameFinal).Append("\t");
                    sb.Append(item.CostCenterNameFinal).Append("\t");
                    sb.Append(item.CostCenterCodeInitial).Append("\t");
                    sb.Append(item.CostCenterNameInitial).Append("\t");
                    //  sb.Append(item.AdmCenterNameInitial).Append("\t");
                    sb.Append(item.EmployeeInternalCode).Append("\t");
                    sb.Append(item.EmployeeFirstName + " " + item.EmployeeLastName).Append("\t");
                    sb.Append(item.SerialNumber).Append("\t");
                    sb.Append(item.RoomName).Append("\t");
                    sb.Append("").Append("\t");
                    sb.Append(string.Format("{0}.{1}.{2}", item.InventoryDate.Day.ToString().Length == 1 ? "0" + item.InventoryDate.Day : item.InventoryDate.Day.ToString(),
                                                            item.InventoryDate.Month.ToString().Length == 1 ? "0" + item.InventoryDate.Month : item.InventoryDate.Month.ToString(),
                                                            item.InventoryDate.Year)).Append("\t");
                    sb.Append("").Append("\t");

                    sb.AppendLine();
                }

                return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "Modificate in unitati logistice.txt");
            }
            else if (reportType == 4)
            {
                invSAP.SAPDetail = new List<InventorySAPDetail>();

                var query = _context.InventoryAssets
                    .Include(a => a.Asset)
                        .Include(ri => ri.RoomInitial)
                            .Include(i => i.EmployeeInitial)
                        .Include(i => i.CostCenterInitial)
                            .ThenInclude(c => c.AdmCenter)
                        .Include(i => i.RoomFinal)
                        .Include(i => i.EmployeeFinal)
                        .Include(i => i.CostCenterFinal)
                            .ThenInclude(c => c.AdmCenter)
                            .Where(i => i.InventoryId == inventoryId && ((i.RoomIdInitial != i.RoomIdFinal) || (i.EmployeeIdInitial != i.EmployeeIdFinal) || (i.CostCenterIdInitial != i.CostCenterIdFinal)) && i.RoomIdFinal != null && ((i.CostCenterIdFinal != null) && (i.CostCenterInitial.AdmCenterId != i.CostCenterFinal.AdmCenterId)))
                            .Select(item => new InventorySAPDetail()
                            {
                                InvNo = item.Asset.InvNo,
                                Description = item.Asset.Name,
                                DescriptionPlus = "",
                                CostCenterCodeInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.Code : string.Empty,
                                AdmCenterNameInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.AdmCenterId != null ? item.CostCenterInitial.AdmCenter.Name : string.Empty : string.Empty,
                                CostCenterCodeFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.Code : string.Empty,
                                AdmCenterNameFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.AdmCenterId != null ? item.CostCenterFinal.AdmCenter.Name : string.Empty : string.Empty,
                                EmployeeInternalCode = item.EmployeeIdFinal != null ? item.EmployeeFinal.InternalCode : string.Empty,
                                EmployeeFirstName = item.EmployeeIdFinal != null ? item.EmployeeFinal.FirstName : string.Empty,
                                EmployeeLastName = item.EmployeeIdFinal != null ? item.EmployeeFinal.LastName : string.Empty,
                                SerialNumber = item.SerialNumber,
                                RoomName = item.RoomIdFinal != null ? item.RoomFinal.Name : string.Empty,
                                InventoryName = inventory.Description,
                                InventoryDate = inventory.Start.Value,
                                CostCenterNameInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.Name : string.Empty,
                                CostCenterNameFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.Name : string.Empty,
                            });

                if (admCenterId != null)
                {
                    admCenter = _context.Set<Model.AdmCenter>().Where(i => i.Id == admCenterId).FirstOrDefault();
                    query = query.Where(a => a.AdmCenterNameFinal == admCenter.Name);
                }

                invSAP.SAPDetail = query.OrderBy(a => a.AdmCenterName).ToList();

                invSAP.InventoryName = inventory.Description.Trim();


                StringBuilder sb = new StringBuilder();
                sb.Append("Numar inventar" + "\t"
                        + "Descriere mijloc fix" + "\t"
                        + "Descriere suplimentara" + "\t"
                        + "Centru de cost final" + "\t"
                        + "Descriere centru de cost final" + "\t"
                        + "Centru de cost" + "\t"
                        + "Descriere centru de cost" + "\t"
                        + "Marca personal" + "\t"
                        + "Nume salariat" + "\t"
                        + "Nr serie mijloc fix" + "\t"
                        + "Camera" + "\t"
                        + "Decizie" + "\t"
                        + "Data inventar" + "\t"
                        + "Numar mijloc fix" + "\r\n");

                foreach (var item in invSAP.SAPDetail)
                {
                    sb.Append(item.InvNo).Append("\t");
                    sb.Append(item.Description).Append("\t");
                    sb.Append("").Append("\t");
                    sb.Append(item.CostCenterCodeFinal).Append("\t");
                    sb.Append(item.CostCenterNameFinal).Append("\t");
                    // sb.Append(item.AdmCenterNameFinal).Append("\t");
                    sb.Append(item.CostCenterCodeInitial).Append("\t");
                    sb.Append(item.CostCenterNameInitial).Append("\t");
                    //  sb.Append(item.AdmCenterNameInitial).Append("\t");
                    sb.Append(item.EmployeeInternalCode).Append("\t");
                    sb.Append(item.EmployeeFirstName + " " + item.EmployeeLastName).Append("\t");
                    sb.Append(item.SerialNumber).Append("\t");
                    sb.Append(item.RoomName).Append("\t");
                    sb.Append("").Append("\t");
                    sb.Append(string.Format("{0}.{1}.{2}", item.InventoryDate.Day.ToString().Length == 1 ? "0" + item.InventoryDate.Day : item.InventoryDate.Day.ToString(),
                                                           item.InventoryDate.Month.ToString().Length == 1 ? "0" + item.InventoryDate.Month : item.InventoryDate.Month.ToString(),
                                                           item.InventoryDate.Year)).Append("\t");
                    sb.Append("").Append("\t");

                    sb.AppendLine();
                }

                return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "Modificate intre unitati logistice.txt");
            }


            return Ok();

        }

        [Route("exportSAPSN")]
        public ActionResult ExportSAPSN(int inventoryId, int? admCenterId)
        {
            InventorySAP invSAP = new InventorySAP();
            Model.Inventory inventory = null;
            Model.AdmCenter admCenter = null;

            inventory = _context.Set<Model.Inventory>().Where(i => i.Id == inventoryId).FirstOrDefault();


            invSAP.SAPDetail = new List<InventorySAPDetail>();

            var query = _context.InventoryAssets
                .Include(a => a.Asset)
                    .Include(ri => ri.RoomInitial)
                        .Include(i => i.EmployeeInitial)
                    .Include(i => i.CostCenterInitial)
                        .ThenInclude(c => c.AdmCenter)
                    .Include(i => i.RoomFinal)
                    .Include(i => i.EmployeeFinal)
                    .Include(i => i.CostCenterFinal)
                        .ThenInclude(c => c.AdmCenter)
                        .Where(i => i.InventoryId == inventoryId && ((i.RoomIdInitial == i.RoomIdFinal && i.EmployeeIdInitial == i.EmployeeIdFinal && i.CostCenterIdInitial == i.CostCenterIdFinal && i.SerialNumber != i.Asset.SerialNumber)) && i.RoomIdFinal != null)
                        .Select(item => new InventorySAPDetail()
                        {
                            InvNo = item.Asset.InvNo,
                            Description = item.Asset.Name,
                            DescriptionPlus = "",
                            CostCenterCodeInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.Code : string.Empty,
                            CostCenterNameInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.Name : string.Empty,
                            AdmCenterNameInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.AdmCenterId != null ? item.CostCenterInitial.AdmCenter.Name : string.Empty : string.Empty,
                            CostCenterCodeFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.Code : string.Empty,
                            CostCenterNameFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.Name : string.Empty,
                            AdmCenterNameFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.AdmCenterId != null ? item.CostCenterFinal.AdmCenter.Name : string.Empty : string.Empty,
                            EmployeeInternalCode = item.EmployeeIdFinal != null ? item.EmployeeFinal.InternalCode : string.Empty,
                            EmployeeFirstName = item.EmployeeIdFinal != null ? item.EmployeeFinal.FirstName : string.Empty,
                            EmployeeLastName = item.EmployeeIdFinal != null ? item.EmployeeFinal.LastName : string.Empty,
                            SerialNumber = item.SerialNumber,
                            RoomName = item.RoomIdFinal != null ? item.RoomFinal.Name : string.Empty,
                            InventoryName = inventory.Description,
                            InventoryDate = inventory.Start.Value
                        });

            if (admCenterId != null)
            {
                admCenter = _context.Set<Model.AdmCenter>().Where(i => i.Id == admCenterId).FirstOrDefault();
                query = query.Where(a => a.AdmCenterNameInitial == admCenter.Name || a.AdmCenterNameFinal == admCenter.Name);
            }

            invSAP.SAPDetail = query.OrderBy(a => a.AdmCenterNameFinal).ToList();

            invSAP.InventoryName = inventory.Description.Trim();


            StringBuilder sb = new StringBuilder();
            sb.Append("Numar inventar" + "\t"
                    + "Descriere mijloc fix" + "\t"
                    + "Descriere suplimentara" + "\t"
                    + "Centru de cost final" + "\t"
                    + "Descriere centru de cost final" + "\t"
                    + "Centru de cost" + "\t"
                    + "Descriere centru de cost" + "\t"
                    + "Marca personal" + "\t"
                    + "Nume salariat" + "\t"
                    + "Nr serie mijloc fix" + "\t"
                    + "Camera" + "\t"
                    + "Decizie" + "\t"
                    + "Data inventar" + "\t"
                    + "Numar mijloc fix" + "\r\n");

            foreach (var item in invSAP.SAPDetail)
            {
                sb.Append(item.InvNo).Append("\t");
                sb.Append(item.Description).Append("\t");
                sb.Append("").Append("\t");
                sb.Append(item.CostCenterCodeFinal).Append("\t");
                sb.Append(item.CostCenterNameFinal).Append("\t");
                //  sb.Append(item.AdmCenterNameFinal).Append("\t");
                sb.Append(item.CostCenterCodeInitial).Append("\t");
                sb.Append(item.CostCenterNameInitial).Append("\t");
                //  sb.Append(item.AdmCenterNameInitial).Append("\t");
                sb.Append(item.EmployeeInternalCode).Append("\t");
                sb.Append(item.EmployeeFirstName + " " + item.EmployeeLastName).Append("\t");
                sb.Append(item.SerialNumber).Append("\t");
                sb.Append(item.RoomName).Append("\t");
                sb.Append("").Append("\t");
                sb.Append(string.Format("{0}.{1}.{2}", item.InventoryDate.Day.ToString().Length == 1 ? "0" + item.InventoryDate.Day : item.InventoryDate.Day.ToString(),
                                                           item.InventoryDate.Month.ToString().Length == 1 ? "0" + item.InventoryDate.Month : item.InventoryDate.Month.ToString(),
                                                           item.InventoryDate.Year)).Append("\t");
                sb.Append("").Append("\t");

                sb.AppendLine();
            }

            return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "Modificate SN.txt");

        }

        [Route("inventorylisttotals")]
        public IActionResult Export(int inventoryId)
        {
            Model.Inventory inventory = null;
            inventory = _context.Set<Model.Inventory>().Where(i => i.Id == inventoryId).FirstOrDefault();


            using (ExcelPackage package = new ExcelPackage())
            {
                InventoryListTotal inventoryTotals = new InventoryListTotal();

                //IQueryable<Model.InventoryAsset> query = null;
                //query = _context.InventoryAssets.AsNoTracking();



                inventoryTotals.InventoryListTotalDetail = new List<InventoryListTotalDetail>();

                var query = _context.InventoryAssets
                        .Include(a => a.Asset)
                            .ThenInclude(u => u.Uom)
                        .Include(ri => ri.RoomInitial)
                            .ThenInclude(l => l.Location)
                            .Include(i => i.EmployeeInitial)
                        .Include(i => i.CostCenterInitial)
                            .ThenInclude(c => c.AdmCenter)
                        .Include(i => i.RoomFinal)
                            .ThenInclude(r => r.Location)
                        .Include(i => i.EmployeeFinal)
                        .Include(i => i.CostCenterFinal)
                            .ThenInclude(c => c.AdmCenter)
                            .Where(i => i.InventoryId == inventoryId)
                            .Select(item => new InventoryListTotalDetail()
                            {
                                InvNo = item.Asset.InvNo,
                                Description = item.Asset.Name,
                                SerialNumber = item.SerialNumber,
                                CostCenterCodeInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.Code : string.Empty,
                                AdmCenterNameInitial = item.CostCenterIdInitial != null ? item.CostCenterInitial.AdmCenter.Name : string.Empty,
                                LocationNameInitial = item.RoomIdInitial != null ? item.RoomInitial.Location.Name : string.Empty,
                                RoomNameInitial = item.RoomIdInitial != null ? item.RoomInitial.Name : string.Empty,
                                PurchaseDate = item.Asset.PurchaseDate.Value,
                                ValueInv = item.Asset.ValueInv,
                                ValueDep = item.Asset.ValueRem,
                                UomName = item.Asset.Uom.Name,
                                QInitial = Convert.ToDecimal(item.QInitial),
                                Custody = item.Asset.Custody.Value,
                                InternalCodeInitial = item.EmployeeIdInitial != null ? item.EmployeeInitial.InternalCode : string.Empty,
                                FirstNameInitial = item.EmployeeIdInitial != null ? item.EmployeeInitial.FirstName : string.Empty,
                                LastNameInitial = item.EmployeeIdInitial != null ? item.EmployeeInitial.LastName : string.Empty,
                                CostCenterCodeFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.Code : string.Empty,
                                AdmCenterNameFinal = item.CostCenterIdFinal != null ? item.CostCenterFinal.AdmCenter.Name : string.Empty,
                                LocationNameFinal = item.RoomIdFinal != null ? item.RoomFinal.Location.Name : string.Empty,
                                RoomNameFinal = item.RoomIdFinal != null ? item.RoomFinal.Name : string.Empty,
                                InternalCodeFinal = item.EmployeeIdFinal != null ? item.EmployeeFinal.InternalCode : string.Empty,
                                FirstNameFinal = item.EmployeeIdFinal != null ? item.EmployeeFinal.FirstName : string.Empty,
                                LastNameFinal = item.EmployeeIdFinal != null ? item.EmployeeFinal.LastName : string.Empty,
                                QFinal = item.EmployeeIdFinal != null ? Convert.ToDecimal(item.QFinal) : 0
                            });

                inventoryTotals.InventoryListTotalDetail = query.ToList();

                var assetni = _context.AssetNis
                        .Include(ri => ri.Room)
                            .ThenInclude(c => c.Location)
                        .Include(i => i.Employee)
                        .Include(i => i.CostCenter)
                        .ThenInclude(c => c.AdmCenter)
                        .Where(i => i.InventoryId == inventoryId && i.AssetId == null && i.IsDeleted == false)

                           .Select(item => new InventoryListTotalDetail()
                           {
                               InvNo = item.Code1,
                               Description = item.Name1,
                               SerialNumber = item.SerialNumber,
                               CostCenterCodeInitial = item.CostCenterId != null ? item.CostCenter.Code : string.Empty,
                               AdmCenterNameInitial = item.CostCenterId != null ? item.CostCenter.AdmCenter.Name : string.Empty,
                               LocationNameInitial = item.RoomId != null ? item.Room.Location.Name : string.Empty,
                               RoomNameInitial = item.RoomId != null ? item.Room.Name : string.Empty,
                               //PurchaseDate = item.Asset.PurchaseDate.Value,
                               ValueInv = 0,
                               ValueDep = 0,
                               // UomName = item.Asset.Uom.Name,
                               QInitial = Convert.ToDecimal(item.Quantity),
                               //Custody = item.Asset.Custody.Value,
                               InternalCodeInitial = item.EmployeeId != null ? item.Employee.InternalCode : string.Empty,
                               FirstNameInitial = item.EmployeeId != null ? item.Employee.FirstName : string.Empty,
                               LastNameInitial = item.EmployeeId != null ? item.Employee.LastName : string.Empty,
                               CostCenterCodeFinal = item.CostCenterId != null ? item.CostCenter.Code : string.Empty,
                               AdmCenterNameFinal = item.CostCenterId != null ? item.CostCenter.AdmCenter.Name : string.Empty,
                               LocationNameFinal = item.RoomId != null ? item.Room.Location.Name : string.Empty,
                               RoomNameFinal = item.RoomId != null ? item.Room.Name : string.Empty,
                               InternalCodeFinal = item.EmployeeId != null ? item.Employee.InternalCode : string.Empty,
                               FirstNameFinal = item.EmployeeId != null ? item.Employee.FirstName : string.Empty,
                               LastNameFinal = item.EmployeeId != null ? item.Employee.LastName : string.Empty,
                               QFinal = item.EmployeeId != null ? Convert.ToDecimal(item.Quantity) : 0
                           });

                var assetliList = assetni.ToList();


                //  inventoryTotals.InventoryListTotalDetail.Add(assetliList);



                foreach (var assetNi in assetliList)
                {
                    inventoryTotals.InventoryListTotalDetail.Add(assetNi);
                }

                inventoryTotals.InventoryListTotalDetail = inventoryTotals.InventoryListTotalDetail
                   .OrderBy(a => a.AdmCenterNameInitial)
                   .ThenBy(a => a.LocationCodeInitial)
                   .ThenBy(a => a.CostCenterCodeInitial)
                   .ThenBy(a => a.RoomNameInitial)
                   .ThenBy(a => a.InternalCodeInitial)
                   .ThenBy(a => a.AdmCenterNameFinal)
                   .ThenBy(a => a.LocationNameFinal)
                   .ThenBy(a => a.CostCenterCodeFinal)
                   .ThenBy(a => a.RoomNameFinal)
                   .ThenBy(a => a.InternalCodeFinal).ToList();
                inventoryTotals.InventoryName = inventory.Description.Trim();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Inventar");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Numar inventar";
                worksheet.Cells[1, 2].Value = "Descriere mijloc fix";
                worksheet.Cells[1, 3].Value = "Nr serie mijloc fix";
                worksheet.Cells[1, 4].Value = "Centru de cost initial";
                worksheet.Cells[1, 5].Value = "Unitate logistica initial";
                worksheet.Cells[1, 6].Value = "Cladire initial";
                worksheet.Cells[1, 7].Value = "Camera initial";
                worksheet.Cells[1, 8].Value = "Data capitalizare";
                worksheet.Cells[1, 9].Value = "Valoare mijloc fix";
                worksheet.Cells[1, 10].Value = "Amortizarea acumulata";
                worksheet.Cells[1, 11].Value = "UM";
                worksheet.Cells[1, 12].Value = "Scriptic";
                worksheet.Cells[1, 13].Value = "Custodie";
                worksheet.Cells[1, 14].Value = "Marca personal intial";
                worksheet.Cells[1, 15].Value = "Nume salariat initial";
                worksheet.Cells[1, 16].Value = "Centru cost final";
                worksheet.Cells[1, 17].Value = "Unitate logistica final";
                worksheet.Cells[1, 18].Value = "Cladire final";
                worksheet.Cells[1, 19].Value = "Camera finala";
                worksheet.Cells[1, 20].Value = "Marca personal final";
                worksheet.Cells[1, 21].Value = "Nume salariat final";
                worksheet.Cells[1, 22].Value = "Faptic";

                int recordIndex = 2;

                foreach (var item in inventoryTotals.InventoryListTotalDetail)
                {
                    worksheet.Cells[recordIndex, 1].Value = item.InvNo;
                    worksheet.Cells[recordIndex, 2].Value = item.Description;
                    worksheet.Cells[recordIndex, 3].Value = item.SerialNumber;
                    worksheet.Cells[recordIndex, 4].Value = item.CostCenterCodeInitial;
                    worksheet.Cells[recordIndex, 5].Value = item.AdmCenterNameInitial;
                    worksheet.Cells[recordIndex, 6].Value = item.LocationNameInitial;
                    worksheet.Cells[recordIndex, 7].Value = item.RoomNameInitial;
                    worksheet.Cells[recordIndex, 8].Value = item.PurchaseDate;
                    worksheet.Cells[recordIndex, 9].Value = item.ValueInv;
                    worksheet.Cells[recordIndex, 10].Value = item.ValueInv - item.ValueDep;
                    worksheet.Cells[recordIndex, 11].Value = item.UomName;
                    worksheet.Cells[recordIndex, 12].Value = item.QInitial;
                    worksheet.Cells[recordIndex, 13].Value = item.Custody == false ? "NU" : "DA";
                    worksheet.Cells[recordIndex, 14].Value = item.InternalCodeInitial;
                    worksheet.Cells[recordIndex, 15].Value = item.FirstNameInitial + " " + item.LastNameInitial;
                    worksheet.Cells[recordIndex, 16].Value = item.CostCenterCodeFinal;
                    worksheet.Cells[recordIndex, 17].Value = item.AdmCenterNameFinal;
                    worksheet.Cells[recordIndex, 18].Value = item.LocationNameFinal;
                    worksheet.Cells[recordIndex, 19].Value = item.RoomNameFinal;
                    worksheet.Cells[recordIndex, 20].Value = item.InternalCodeFinal;
                    worksheet.Cells[recordIndex, 21].Value = item.FirstNameFinal + " " + item.LastNameFinal;
                    worksheet.Cells[recordIndex, 22].Value = item.QFinal;

                    recordIndex++;
                }

                //worksheet.Column(1).AutoFit();
                //worksheet.Column(2).AutoFit();
                //worksheet.Column(3).AutoFit();
                //worksheet.Column(4).AutoFit();
                //worksheet.Column(5).AutoFit();
                //worksheet.Column(6).AutoFit();
                //worksheet.Column(7).AutoFit();
                //worksheet.Column(8).AutoFit();
                //worksheet.Column(9).AutoFit();
                //worksheet.Column(10).AutoFit();
                //worksheet.Column(11).AutoFit();
                //worksheet.Column(12).AutoFit();
                //worksheet.Column(13).AutoFit();
                //worksheet.Column(14).AutoFit();
                //worksheet.Column(15).AutoFit();
                //worksheet.Column(16).AutoFit();
                //worksheet.Column(17).AutoFit();
                //worksheet.Column(18).AutoFit();
                //worksheet.Column(19).AutoFit();
                //worksheet.Column(20).AutoFit();
                worksheet.View.ShowGridLines = true;
                worksheet.View.FreezePanes(2, 1);

                using (var cells = worksheet.Cells[1, 1, 1, 22])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(100, 145, 217));
                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Backup_inventar.xlsx"
                };

                return result;

            }
        }

        [Route("inventorylistemag")]
        public IActionResult GetInventoryListEmagData(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? companyId, int? administrationId, int? roomId, string reportFilter)
        {
            var inventoryListV2Data = this._repository.GetInventoryListEmagByFilters(inventoryId, admCenterId, costCenterId, regionId, locationId, divisionId, companyId, administrationId, roomId, reportFilter);

            return Ok(inventoryListV2Data);
        }

        [Route("inventorylistempemag")]
        public IActionResult GetInventoryListEmpPetromData(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? companyId, int? administrationId, int? roomId)
        {
            var inventoryListV2Data = this._repository.GetInventoryListEmpEmagByFilters(inventoryId, admCenterId, costCenterId, regionId, locationId, divisionId, companyId, administrationId, roomId);

            return Ok(inventoryListV2Data);
        }

        [Route("inventorylistempfinalemag")]
        public IActionResult GetInventoryListEmpFinalPetromData(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? companyId, int? administrationId, int? roomId)
        {
            var inventoryListV2Data = this._repository.GetInventoryListEmpFinalEmagByFilters(inventoryId, admCenterId, costCenterId, regionId, locationId, divisionId, companyId, administrationId, roomId);

            return Ok(inventoryListV2Data);
        }

        [Route("inventoryresultemag")]
        public IActionResult GetInventoryResultData(int inventoryId, int? admCenterId, int? costCenterId, int? regionId, int? locationId, int? divisionId, int? companyId, int? administrationId, int? roomId, string reportFilter)
        {

            var inventoryResultData = this._repository.GetInventoryResultByFilters(inventoryId, admCenterId, costCenterId, regionId, locationId, divisionId, companyId, administrationId, roomId, reportFilter);

            return Ok(inventoryResultData);
        }

        [Route("inventorylabel")]
        public IActionResult GetInventoryLabelData(int inventoryId, int? costCenterId)
        {

            var inventoryResultData = this._repository.GetInventoryLabelByFilters(inventoryId, costCenterId);

            return Ok(inventoryResultData);
        }

        //[Route("api/~/api/reports/movement/{documentId}")]
        //public MovementReport GetMovementReportData(int documentId)
        //{
        //    return this._repository.GetMovementReportData(documentId);
        //}
    }
}
