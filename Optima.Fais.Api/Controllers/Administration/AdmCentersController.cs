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
    [Route("api/admcenters")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AdmCentersController : GenericApiController<Model.AdmCenter, Dto.AdmCenter>
    {
        public AdmCentersController(ApplicationDbContext context, IAdmCentersRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }


        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string employeeIds, string includes)
        {
            List<Model.AdmCenter> items = null;
            IEnumerable<Dto.AdmCenter> itemsResult = null;
            List<int> eIds = null;

            includes = includes ?? "Employee";

            if ((employeeIds != null) && (employeeIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(employeeIds).ToList().Select(int.Parse).ToList();

            items = (_itemsRepository as IAdmCentersRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, eIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.AdmCenter>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IAdmCentersRepository).GetCountByFilters(filter, eIds);
                var pagedResult = new Dto.PagedResult<Dto.AdmCenter>(itemsResult, new Dto.PagingInfo()
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
			List<Model.AdmCenter> admCenters = null;
			int rowNumber = 0;
			using (ExcelPackage package = new ExcelPackage())
			{
				admCenters = (_itemsRepository as IAdmCentersRepository).GetByFilters(filter, null, null, null, null, null, null).ToList();
				admCenters = admCenters.OrderBy(l => l.Name).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PC");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod";
				worksheet.Cells[1, 2].Value = "Descriere";


				int recordIndex = 2;
				foreach (var item in admCenters)
				{
					rowNumber++;
					worksheet.Cells[recordIndex, 1].Value = item.Code;
					worksheet.Cells[recordIndex, 2].Value = item.Name;



					recordIndex++;
				}

				worksheet.View.FreezePanes(2, 1);

				worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();



				using (var cells = worksheet.Cells[1, 1, 1, 2])
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
					FileDownloadName = "PC.xlsx"
				};

				return result;

			}
		}
	}
}
