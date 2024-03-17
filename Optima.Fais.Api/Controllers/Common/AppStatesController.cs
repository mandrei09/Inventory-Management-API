using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize]
    [Route("api/appstates")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AppStatesController : GenericApiController<Model.AppState, Dto.AppState>
    {
        public AppStatesController(ApplicationDbContext context, IAppStatesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string parentCode, string includes)
        {
            List<Model.AppState> items = null;
            IEnumerable<Dto.AppState> itemsResult = null;


            items = (_itemsRepository as IAppStatesRepository).GetByFilters(filter, parentCode, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.AppState>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IAppStatesRepository).GetCountByFilters(filter, parentCode);
                var pagedResult = new Dto.PagedResult<Dto.AppState>(itemsResult, new Dto.PagingInfo()
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

        [HttpGet("filtered")]
        public IActionResult GetAppStatesByFilters(string parentCode)
        {
            IEnumerable<Model.AppState> items = (_itemsRepository as IAppStatesRepository).GetAppStatesByFilters(parentCode);

            return Ok(items.Select(i => _mapper.Map<Dto.AppState>(i)));
        }

        [HttpGet("parentCode/{parentCode}")]
        public IActionResult GetAppStatesByPrefix(string parentCode)
        {
            IEnumerable<Model.AppState> items = (_itemsRepository as IAppStatesRepository).GetAppStatesByParentCode(parentCode);

            return Ok(items.Select(i => _mapper.Map<Dto.AppState>(i)));
        }

		[HttpGet("export")]
		public IActionResult Export(string filter)
		{
			List<Model.AppState> appStates = null;
			int rowNumber = 0;
			using (ExcelPackage package = new ExcelPackage())
			{
				appStates = (_itemsRepository as IAppStatesRepository).GetByFilters(filter, null, null, null, null, null, null).ToList();
				appStates = appStates.OrderBy(l => l.Name).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("stari");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod";
				worksheet.Cells[1, 2].Value = "Descriere";
				worksheet.Cells[1, 3].Value = "Culoare";
				worksheet.Cells[1, 4].Value = "Iconita";


				int recordIndex = 2;
				foreach (var item in appStates)
				{
					rowNumber++;
					worksheet.Cells[recordIndex, 1].Value = item.Code;
					worksheet.Cells[recordIndex, 2].Value = item.Name;
					worksheet.Cells[recordIndex, 3].Value = item.BadgeColor;
					worksheet.Cells[recordIndex, 4].Value = item.BadgeIcon;

					recordIndex++;
				}

				worksheet.View.FreezePanes(2, 1);

				worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();
				worksheet.Column(3).AutoFit();
				worksheet.Column(4).AutoFit();


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
					FileDownloadName = "stari.xlsx"
				};

				return result;

			}
		}
	}
}
