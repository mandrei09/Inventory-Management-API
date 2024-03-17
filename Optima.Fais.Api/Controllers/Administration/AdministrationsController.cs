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
    [Route("api/administrations")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AdministrationsController : GenericApiController<Model.Administration, Dto.Administration>
    {
        public AdministrationsController(ApplicationDbContext context, IAdministrationsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string divisionIds, string includes)
        {
            List<Model.Administration> items = null;
            IEnumerable<Dto.Administration> itemsResult = null;
            List<int?> dIds = null;

            includes = "Division";

            if ((divisionIds != null) && (divisionIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(divisionIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IAdministrationsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, dIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Administration>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IAdministrationsRepository).GetCountByFilters(filter, dIds);
                var pagedResult = new Dto.PagedResult<Dto.Administration>(itemsResult, new Dto.PagingInfo()
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
			List<Model.Administration> administrations = null;
			int rowNumber = 0;
			using (ExcelPackage package = new ExcelPackage())
			{
				administrations = (_itemsRepository as IAdministrationsRepository).GetByFilters(filter, null, null, null, null, null, null).ToList();
				administrations = administrations.OrderBy(l => l.Name).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Locatii");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod";
				worksheet.Cells[1, 2].Value = "Descriere";


				int recordIndex = 2;
				foreach (var item in administrations)
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
					FileDownloadName = "Locatii.xlsx"
				};

				return result;

			}
		}
	}
}
