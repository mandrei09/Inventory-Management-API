using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/locations")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    //[ResponseCache(Location = ResponseCacheLocation.Client, NoStore = false, Duration = 60)]
    public partial class LocationsController : GenericApiController<Model.Location, Dto.Location>
    {
        public LocationsController(ApplicationDbContext context, ILocationsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string cityIds,  string regionIds, string admCenterIds, string includes)
        {
            List<Model.Location> items = null;
            IEnumerable<Dto.Location> itemsResult = null;
            List<int> rIds = null;
            List<int> aIds = null;
            List<int> cIds = null;

            includes = includes ?? "City.County";

            if ((regionIds != null) && (regionIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(regionIds).ToList().Select(int.Parse).ToList();
            if ((admCenterIds != null) && (admCenterIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();
            if ((cityIds != null) && (cityIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(cityIds).ToList().Select(int.Parse).ToList();

            items = (_itemsRepository as ILocationsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, rIds, aIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Location>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as ILocationsRepository).GetCountByFilters(filter, cIds, rIds, aIds);
                var pagedResult = new Dto.PagedResult<Dto.Location>(itemsResult, new Dto.PagingInfo()
                {
                    TotalItems = totalItems,
                    CurrentPage = page.Value,
                    PageSize = pageSize.Value
                });
                return Ok(pagedResult);
            }
            else
            {
                return Ok(itemsResult);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sync")]
        public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.Location> items = (_itemsRepository as ILocationsRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<Dto.LocationSync>(i));
            var pagedResult = new Dto.PagedResult<Dto.LocationSync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }

        [HttpGet("export")]
        public IActionResult Export(string filter, string cityIds, string regionIds, string admCenterIds)
        {
            List<int> rIds = null;
            List<int> admIds = null;
            List<int> cIds = null;
            List<Model.Location> locations = null;
            int rowNumber = 0;
            var includes = "Region,City";

            if ((cityIds != null) && (cityIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(cityIds).ToList().Select(int.Parse).ToList();
            if ((regionIds != null) && (regionIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(regionIds).ToList().Select(int.Parse).ToList();
            if ((admCenterIds != null) && (admCenterIds.Length > 0)) admIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();

            using (ExcelPackage package = new ExcelPackage())
            {
                locations = (_itemsRepository as ILocationsRepository).GetByFilters(filter, includes, null, null, null, null, cIds, rIds, admIds).ToList();

                locations = locations.OrderBy(l => l.Name).ToList();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("orase");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Nr. Crt";
                worksheet.Cells[1, 2].Value = "Building Code";
                worksheet.Cells[1, 3].Value = "Building Name";
                worksheet.Cells[1, 4].Value = "Location";
                //worksheet.Cells[1, 5].Value = "Gestionar Marca";
                //worksheet.Cells[1, 6].Value = "Gestionar Nume";
                //worksheet.Cells[1, 7].Value = "Gestionar Prenume";
                //worksheet.Cells[1, 8].Value = "Gestionar Email";

                int recordIndex = 2;
                foreach (var item in locations)
                {
                    rowNumber++;
                    worksheet.Cells[recordIndex, 1].Value = rowNumber;
                    worksheet.Cells[recordIndex, 2].Value = item.Code;
                    worksheet.Cells[recordIndex, 3].Value = item.Name;
                    worksheet.Cells[recordIndex, 4].Value = item.Region != null ? item.Region.Name : "";
                    //worksheet.Cells[recordIndex, 4].Value = item.AdmCenter != null ? item.AdmCenter.Name : "";
                    //worksheet.Cells[recordIndex, 5].Value = item.AdmCenter != null ? item.AdmCenter.Employee != null ? item.AdmCenter.Employee.InternalCode : "" : "";
                    //worksheet.Cells[recordIndex, 6].Value = item.AdmCenter != null ? item.AdmCenter.Employee != null ? item.AdmCenter.Employee.FirstName : "" : "";
                    //worksheet.Cells[recordIndex, 7].Value = item.AdmCenter != null ? item.AdmCenter.Employee != null ? item.AdmCenter.Employee.LastName : "" : "";
                    //worksheet.Cells[recordIndex, 8].Value = item.AdmCenter != null ? item.AdmCenter.Employee != null ? item.AdmCenter.Employee.Email : "" : "";

                    
                    recordIndex++;
                }

                worksheet.View.FreezePanes(2, 1);

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                //worksheet.Column(5).AutoFit();
                //worksheet.Column(6).AutoFit();
                //worksheet.Column(7).AutoFit();
                //worksheet.Column(8).AutoFit();
                

                using (var cells = worksheet.Cells[1, 1, 1, 4])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.Red);
                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Buildings.xlsx"
                };

                return result;

            }
        }
    }
}
