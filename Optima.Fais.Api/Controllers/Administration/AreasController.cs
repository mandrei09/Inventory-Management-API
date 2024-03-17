using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharp;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/areas")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AreasController : GenericApiController<Model.Area, Dto.Area>
    {
        public AreasController(ApplicationDbContext context, IAreasRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }


        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
        {
            List<Model.Area> items = null;
            IEnumerable<Dto.Area> itemsResult = null;

            items = (_itemsRepository as IAreasRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Area>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IAreasRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.Area>(itemsResult, new Dto.PagingInfo()
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
			List<Model.Area> areas = null;
			using (ExcelPackage package = new ExcelPackage())
			{
				areas = (_itemsRepository as IAreasRepository).GetByFilters(filter, null, null, null, null, null).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("area");
				//First add the headers

				worksheet.Cells[1, 1].Value = "Code";
				worksheet.Cells[1, 2].Value = "Name";

				int recordIndex = 2;
				foreach (var item in areas)
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
