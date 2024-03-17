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
	[Route("api/subcategoriesen")]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public partial class SubCategoriesENController : GenericApiController<Model.SubCategoryEN, Dto.SubCategoryEN>
	{
		public SubCategoriesENController(ApplicationDbContext context, ISubCategoriesENRepository itemsRepository, IMapper mapper)
			: base(context, itemsRepository, mapper)
		{
		}

		[HttpGet]
		[Route("", Order = -1)]
		public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
		{
			List<Model.SubCategoryEN> items = null;
			IEnumerable<Dto.SubCategoryEN> itemsResult = null;

			includes = includes + ",CategoryEN.InterCompanyEN";

			items = (_itemsRepository as ISubCategoriesENRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
			itemsResult = items.Select(i => _mapper.Map<Dto.SubCategoryEN>(i));



			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as ISubCategoriesENRepository).GetCountByFilters(filter);
				var pagedResult = new Dto.PagedResult<Dto.SubCategoryEN>(itemsResult, new Dto.PagingInfo()
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
            List<Model.SubCategoryEN> interCompanies = null;

            using (OfficeOpenXml.ExcelPackage package = new ExcelPackage())
            {
                interCompanies = (_itemsRepository as ISubCategoriesENRepository).GetByFilters(filter, "CategoryEN.InterCompanyEN", null, null, null, null).ToList();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("SubCategoriesEN");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod";
				worksheet.Cells[1, 2].Value = "Descriere";
				worksheet.Cells[1, 3].Value = "Cod Categorie";
				worksheet.Cells[1, 4].Value = "Categorie";

				int recordIndex = 2;
                foreach (var item in interCompanies)
                {
					worksheet.Cells[recordIndex, 1].Value = item.Code;
					worksheet.Cells[recordIndex, 2].Value = item.Name;
					worksheet.Cells[recordIndex, 3].Value = item.CategoryEN != null ? item.CategoryEN.Code : "";
					worksheet.Cells[recordIndex, 4].Value = item.CategoryEN != null ? item.CategoryEN.Name : "";
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
                    FileDownloadName = "SubCategoriesEN.xlsx"
                };

                return result;

            }
        }
	}
}
