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
	[Route("api/categories")]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public partial class CategoriesController : GenericApiController<Model.Category, Dto.Category>
	{
		public CategoriesController(ApplicationDbContext context, ICategoriesRepository itemsRepository, IMapper mapper)
			: base(context, itemsRepository, mapper)
		{
		}

		[HttpGet]
		[Route("", Order = -1)]
		public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
		{
			List<Model.Category> items = null;
			IEnumerable<Dto.Category> itemsResult = null;

			includes = ",InterCompany,CategoryEN";

			items = (_itemsRepository as ICategoriesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
			itemsResult = items.Select(i => _mapper.Map<Dto.Category>(i));

           

			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as ICategoriesRepository).GetCountByFilters(filter);
				var pagedResult = new Dto.PagedResult<Dto.Category>(itemsResult, new Dto.PagingInfo()
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
            List<Model.Category> interCompanies = null;

            using (OfficeOpenXml.ExcelPackage package = new ExcelPackage())
            {
                interCompanies = (_itemsRepository as ICategoriesRepository).GetByFilters(filter, "InterCompany,CategoryEN", null, null, null, null).ToList();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Categories");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Cod Categorie";
                worksheet.Cells[1, 2].Value = "Categorie";

				int recordIndex = 2;
                foreach (var item in interCompanies)
                {
                    worksheet.Cells[recordIndex, 1].Value = item.Code;
                    worksheet.Cells[recordIndex, 2].Value = item.Name;
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
                    cells.Style.Fill.BackgroundColor.SetColor(Color.Aqua);
                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Categories.xlsx"
				};

                return result;

            }
        }
	}
}
