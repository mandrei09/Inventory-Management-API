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
    [Route("api/counties")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    //[ResponseCache(Location = ResponseCacheLocation.Client, NoStore = false, Duration = 60)]
    public partial class CountiesController : GenericApiController<Model.County, Dto.County>
    {
        public CountiesController(ApplicationDbContext context, ICountiesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string countryIds, string includes)
        {
            List<Model.County> items = null;
            IEnumerable<Dto.County> itemsResult = null;
            List<int> rIds = null;


            includes = includes ?? "Country";

            if ((countryIds != null) && (countryIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(countryIds).ToList().Select(int.Parse).ToList();


            items = (_itemsRepository as ICountiesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, rIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.County>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as ICountiesRepository).GetCountByFilters(filter, rIds);
                var pagedResult = new Dto.PagedResult<Dto.County>(itemsResult, new Dto.PagingInfo()
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
            List<Model.County> items = (_itemsRepository as ICountiesRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<CountySync>(i));
            var pagedResult = new Dto.PagedResult<CountySync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }

        [HttpGet("export")]
        public IActionResult Export(string filter, string countryIds)
        {
            List<int> rIds = null;
            List<Model.County> counties = null;
            int rowNumber = 0;
            var includes = "Country";

            if ((countryIds != null) && (countryIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(countryIds).ToList().Select(int.Parse).ToList();


            using (ExcelPackage package = new ExcelPackage())
            {
                counties = (_itemsRepository as ICountiesRepository).GetByFilters(filter, includes, null, null, null, null, rIds).ToList();

                counties = counties.OrderBy(l => l.Name).ToList();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Judete");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Nr. Crt";
                worksheet.Cells[1, 2].Value = "Oras";
                worksheet.Cells[1, 3].Value = "Judet";


                int recordIndex = 2;
                foreach (var item in counties)
                {
                    rowNumber++;
                    worksheet.Cells[recordIndex, 1].Value = rowNumber;
                    worksheet.Cells[recordIndex, 2].Value = item.Name;
                    worksheet.Cells[recordIndex, 3].Value = item.Country != null ? item.Country.Name : "";



                    recordIndex++;
                }

                worksheet.View.FreezePanes(2, 1);

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();



                using (var cells = worksheet.Cells[1, 1, 1, 3])
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
                    FileDownloadName = "orase.xlsx"
                };

                return result;

            }
        }
    }
}
