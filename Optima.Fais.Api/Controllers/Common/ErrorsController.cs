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
using System.Globalization;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/errors")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class ErrorsController : GenericApiController<Model.Error, Dto.Error>
    {
        public ErrorsController(ApplicationDbContext context, IErrorsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string errorTypeIds, string includes)
        {
            List<Model.Error> items = null;
            IEnumerable<Dto.Error> itemsResult = null;
            List<int> cIds = null;

            if (errorTypeIds != null && !errorTypeIds.StartsWith("["))
            {
                errorTypeIds = "[" + errorTypeIds + "]";
            }



            includes = includes ?? "ErrorType,Asset";

            if ((errorTypeIds != null) && (errorTypeIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(errorTypeIds).ToList().Select(int.Parse).ToList();


            items = (_itemsRepository as IErrorsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Error>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IErrorsRepository).GetCountByFilters(filter, cIds);
                var pagedResult = new Dto.PagedResult<Dto.Error>(itemsResult, new Dto.PagingInfo()
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

        [HttpGet("export")]
        public IActionResult Export(string filter)
        {
            List<Model.Error> errors = null;

            using (ExcelPackage package = new ExcelPackage())
            {
                errors = (_itemsRepository as IErrorsRepository).GetByFilters(filter, "ErrorType", null, null, null, null, null).ToList();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("errors");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Asset";
                worksheet.Cells[1, 2].Value = "Reason";
                worksheet.Cells[1, 3].Value = "Type";
                worksheet.Cells[1, 4].Value = "Date";

                int recordIndex = 2;
                foreach (var item in errors)
                {
                    worksheet.Cells[recordIndex, 1].Value = item.Code;
                    worksheet.Cells[recordIndex, 2].Value = item.Name;
                    worksheet.Cells[recordIndex, 3].Value = item.ErrorType != null ? item.ErrorType.Name : "";
                    worksheet.Cells[recordIndex, 4].Value = item.CreatedAt;
                    worksheet.Cells[recordIndex, 4].Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                    recordIndex++;
                }

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();

                using (var cells = worksheet.Cells[1, 1, 1, 4])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.Salmon);
                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "erorrs.xlsx"
                };

                return result;

            }
        }
    }
}
