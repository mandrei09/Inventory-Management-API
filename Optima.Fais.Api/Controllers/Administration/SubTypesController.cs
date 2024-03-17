using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/subtypes")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class SubTypesController : GenericApiController<Model.SubType, Dto.SubType>
    {
        public SubTypesController(ApplicationDbContext context, ISubTypesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string typeIds, string includes)
        {
            List<Model.SubType> items = null;
            IEnumerable<Dto.SubType> itemsResult = null;
            List<int?> dIds = null;

            includes = includes ?? "Type.MasterType";

            if (typeIds != null && !typeIds.StartsWith("["))
            {
                typeIds = "[" + typeIds + "]";
            }

            if ((typeIds != null) && (typeIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(typeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as ISubTypesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, dIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.SubType>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as ISubTypesRepository).GetCountByFilters(filter, dIds);
                var pagedResult = new Dto.PagedResult<Dto.SubType>(itemsResult, new Dto.PagingInfo()
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
        public IActionResult Export(string filter, string typeIds)
        {
            List<int?> aIds = null;
            List<Model.SubType> subTypes = null;

            if ((typeIds != null) && (typeIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(typeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            using (ExcelPackage package = new ExcelPackage())
            {
                subTypes = (_itemsRepository as ISubTypesRepository).GetByFilters(filter, null, null, null, null, null, aIds).ToList();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("subtypes");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Code";
                worksheet.Cells[1, 3].Value = "Name";

                int recordIndex = 2;
                foreach (var item in subTypes)
                {
                    worksheet.Cells[recordIndex, 1].Value = item.Id;
                    worksheet.Cells[recordIndex, 2].Value = item.Code;
                    worksheet.Cells[recordIndex, 3].Value = item.Name;
                    recordIndex++;
                }

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();

                using (var cells = worksheet.Cells[1, 1, 1, 4])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.Aqua);
                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "subtypes.xlsx"
                };

                return result;

            }
        }
    }
}
