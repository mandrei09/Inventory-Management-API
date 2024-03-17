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
    [Route("api/budgetmanagers")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class BudgetManagersController : GenericApiController<Model.BudgetManager, Dto.BudgetManager>
    {
        public BudgetManagersController(ApplicationDbContext context, IBudgetManagersRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string uomIds, string includes)
        {
            List<Model.BudgetManager> items = null;
            IEnumerable<Dto.BudgetManager> itemsResult = null;
            List<int?> dIds = null;

            includes = includes ?? "Uom";

            if (uomIds != null && !uomIds.StartsWith("["))
            {
                uomIds = "[" + uomIds + "]";
            }


            if ((uomIds != null) && (uomIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(uomIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IBudgetManagersRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, dIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.BudgetManager>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IBudgetManagersRepository).GetCountByFilters(filter, dIds);
                var pagedResult = new Dto.PagedResult<Dto.BudgetManager>(itemsResult, new Dto.PagingInfo()
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
        public IActionResult Export(string filter, string uomIds)
        {
            List<int?> aIds = null;
            List<Model.BudgetManager> budgetManagers = null;

            if ((uomIds != null) && (uomIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(uomIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            using (OfficeOpenXml.ExcelPackage package = new ExcelPackage())
            {
                budgetManagers = (_itemsRepository as IBudgetManagersRepository).GetByFilters(filter, null, null, null, null, null, aIds).ToList();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("budgetManagers");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Code";
                worksheet.Cells[1, 3].Value = "Name";

                int recordIndex = 2;
                foreach (var item in budgetManagers)
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
                    FileDownloadName = "budgetManagers.xlsx"
                };

                return result;

            }
        }
    }
}
