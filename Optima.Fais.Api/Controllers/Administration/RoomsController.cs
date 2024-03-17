using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/rooms")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class RoomsController : GenericApiController<Model.Room, Dto.Room>
    {
        public RoomsController(ApplicationDbContext context, IRoomsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        //[HttpGet]
        //[Route("details")]
        //public virtual IActionResult GetDetails(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, int? locationId)
        //{
        //    int totalItems = 0;

        //    List<int> location

        //    //List<Dto.RoomDetail> items = (_itemsRepository as IRoomsRepository).GetDetailsByFilters(locationId, filter, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();
        //    List<Model.Room> items = (_itemsRepository as IRoomsRepository).GetByFilters(filter, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

        //    var result = new PagedResult<Dto.RoomDetail>(items, new PagingInfo()
        //    {
        //        TotalItems = totalItems,
        //        CurrentPage = page.Value,
        //        PageSize = pageSize.Value
        //    });

        //    return Ok(result);
        //}

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string locationIds, string regionIds, string admCenterIds, string includes)
        {
            List<Model.Room> items = null;
            IEnumerable<Dto.Room> itemsResult = null;
            List<int> lIds = null;
            List<int> rIds = null;
            List<int> admIds = null;

            includes = includes ?? "Location.City.County";

            if (locationIds != null && !locationIds.StartsWith("["))
            {
                locationIds = "[" + locationIds + "]";
            }


            if ((locationIds != null) && (locationIds.Length > 0)) lIds = JsonConvert.DeserializeObject<string[]>(locationIds).ToList().Select(int.Parse).ToList();
            if ((regionIds != null) && (regionIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(regionIds).ToList().Select(int.Parse).ToList();
            if ((admCenterIds != null) && (admCenterIds.Length > 0)) admIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();

            items = (_itemsRepository as IRoomsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, lIds, rIds, admIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Room>(i));

            

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IRoomsRepository).GetCountByFilters(filter, lIds, rIds, admIds);
                var pagedResult = new Dto.PagedResult<Dto.Room>(itemsResult, new Dto.PagingInfo()
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

        [HttpPut]
        [Route("", Order = -1)]
        public void PutCustom([FromBody] Dto.Room vm)
        {
            //var room = _mapper.Map<Model.Room>(vm);
            Model.Room room = new Model.Room
            {
                Id = vm.Id,
                Code = vm.Code,
                Name = vm.Name,
                LocationId = vm.Location.Id,
                IsFinished = vm.IsFinished
            };

            _itemsRepository.Update(room);

            if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
            _context.SaveChanges();

            //var updatedItem = this.GetById(item.Id);

            //if (_notifyChange) _wsService.NotifyNewData<V>(string.Empty, typeof(T).Name.ToUpper(), "UPDATE", updatedItem);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sync")]
        public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.Room> items = (_itemsRepository as IRoomsRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<RoomSync>(i));
            var pagedResult = new Dto.PagedResult<RoomSync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }

        [HttpGet("export")]
        public IActionResult Export(string filter, string locationIds, string regionIds, string admCenterIds)
        {
            List<int> lIds = null;
            List<int> rIds = null;
            List<int> admIds = null;
            List<Model.Room> rooms = null;
            int rowNumber = 0;
            var includes = "Location,Location.Region";

            if ((locationIds != null) && (locationIds.Length > 0)) lIds = JsonConvert.DeserializeObject<string[]>(locationIds).ToList().Select(int.Parse).ToList();
            if ((regionIds != null) && (regionIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(regionIds).ToList().Select(int.Parse).ToList();
            if ((admCenterIds != null) && (admCenterIds.Length > 0)) admIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();

            using (ExcelPackage package = new ExcelPackage())
            {
                rooms = (_itemsRepository as IRoomsRepository).GetByFilters(filter, includes, null, null, null, null, lIds, rIds, admIds).ToList();


                rooms = rooms.OrderBy(l => l.Code).ToList();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("zones");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Nr. Crt";
                worksheet.Cells[1, 2].Value = "ZoneCode";
                worksheet.Cells[1, 3].Value = "ZoneName";
                worksheet.Cells[1, 4].Value = "BuildingCode";
                worksheet.Cells[1, 5].Value = "BuildingName";
                worksheet.Cells[1, 6].Value = "Location";
                //worksheet.Cells[1, 7].Value = "Gestionar Marca";
                //worksheet.Cells[1, 8].Value = "Gestionar Nume";
                //worksheet.Cells[1, 9].Value = "Gestionar Prenume";
                //worksheet.Cells[1, 10].Value = "Gestionar Email";
                //worksheet.Cells[1, 11].Value = "Finalizat?";

                int recordIndex = 2;
                foreach (var item in rooms)
                {
                    rowNumber++;
                    worksheet.Cells[recordIndex, 1].Value = rowNumber;
                    worksheet.Cells[recordIndex, 2].Value = item.Code;
                    worksheet.Cells[recordIndex, 3].Value = item.Name;
                    worksheet.Cells[recordIndex, 4].Value = item.Location != null ? item.Location.Code : "";
                    worksheet.Cells[recordIndex, 5].Value = item.Location != null ? item.Location.Name : "";
                    worksheet.Cells[recordIndex, 6].Value = item.Location != null ? item.Location.Region != null ? item.Location.Region.Name : "" : "";
                    //worksheet.Cells[recordIndex, 6].Value = item.Location != null ? item.Location.AdmCenter != null ?item.Location.AdmCenter.Name : "" : "";
                    //worksheet.Cells[recordIndex, 7].Value = item.Location != null ? item.Location.AdmCenter != null ? item.Location.AdmCenter.Employee != null ? item.Location.AdmCenter.Employee.InternalCode : "" : "" : "";
                    //worksheet.Cells[recordIndex, 8].Value = item.Location != null ? item.Location.AdmCenter != null ? item.Location.AdmCenter.Employee != null ? item.Location.AdmCenter.Employee.FirstName : "" : "" : "";
                    //worksheet.Cells[recordIndex, 9].Value = item.Location != null ? item.Location.AdmCenter != null ? item.Location.AdmCenter.Employee != null ? item.Location.AdmCenter.Employee.LastName : "" : "" : "";
                    //worksheet.Cells[recordIndex, 10].Value = item.Location != null ? item.Location.AdmCenter != null ? item.Location.AdmCenter.Employee != null ? item.Location.AdmCenter.Employee.Email : "" : "" : "";
                    //worksheet.Cells[recordIndex, 11].Value = item.IsFinished == true ? "Finalizat" : "De inventariat";
                    recordIndex++;
                }

                worksheet.View.FreezePanes(2, 1);

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                //worksheet.Column(7).AutoFit();
                //worksheet.Column(8).AutoFit();
                //worksheet.Column(9).AutoFit();
                //worksheet.Column(10).AutoFit();
                //worksheet.Column(11).AutoFit();

                using (var cells = worksheet.Cells[1, 1, 1, 6])
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
                    FileDownloadName = "zones.xlsx"
                };

                return result;

            }
        }
    }
}
