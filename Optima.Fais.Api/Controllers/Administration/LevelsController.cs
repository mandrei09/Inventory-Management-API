using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharp;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/levels")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class LevelsController : GenericApiController<Model.Level, Dto.Level>
    {
        public LevelsController(ApplicationDbContext context, ILevelsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }


        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
        {
            List<Model.Level> items = null;
            IEnumerable<Dto.Level> itemsResult = null;

            items = (_itemsRepository as ILevelsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Level>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as ILevelsRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.Level>(itemsResult, new Dto.PagingInfo()
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
			List<int?> aIds = null;
			List<Model.Level> levels = null;
			using (ExcelPackage package = new ExcelPackage())
			{
				levels = (_itemsRepository as ILevelsRepository).GetByFilters(filter, null, null, null, null, null).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("level");
				//First add the headers

				worksheet.Cells[1, 1].Value = "Code";
				worksheet.Cells[1, 2].Value = "Name";

				int recordIndex = 2;
				foreach (var item in levels)
				{
					worksheet.Cells[recordIndex, 1].Value = item.Code;
					worksheet.Cells[recordIndex, 2].Value = item.Name;
					recordIndex++;
				}

				worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();

				using (var cells = worksheet.Cells[1, 1, 1, 2])
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
					FileDownloadName = "types.xlsx"
				};

				return result;

			}
		}
	}
}
