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
	[Route("api/subcategories")]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public partial class SubCategoriesController : GenericApiController<Model.SubCategory, Dto.SubCategory>
	{
		public SubCategoriesController(ApplicationDbContext context, ISubCategoriesRepository itemsRepository, IMapper mapper)
			: base(context, itemsRepository, mapper)
		{
		}

		[HttpGet]
		[Route("", Order = -1)]
		public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string categoryIds, string assetTypeIds, string orderId, string includes)
		{
			List<Model.SubCategory> items = null;
			IEnumerable<Dto.SubCategory> itemsResult = null;
            List<int> aIds = null;
            List<int> catIds = null;
            List<int> subIds = null;

            includes = includes + ",Category.InterCompany.AssetType";

            if(categoryIds != "" && categoryIds != null)
			{
                categoryIds = "[" + categoryIds + "]";
			}

            if(orderId != null && orderId.Length > 0)
			{
                subIds = _context.Set<Model.OrderMaterial>().Where(a => a.OrderId == int.Parse(orderId)).Select(a => a.Material.SubCategoryId.Value).ToList();
            }

            //if ((assetTypeIds != null) && (assetTypeIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(assetTypeIds).ToList().Select(int.Parse).ToList();
            if ((categoryIds != null) && (categoryIds.Length > 0)) catIds = JsonConvert.DeserializeObject<string[]>(categoryIds).ToList().Select(int.Parse).ToList();

            items = (_itemsRepository as ISubCategoriesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, catIds, aIds, subIds).ToList();
			itemsResult = items.Select(i => _mapper.Map<Dto.SubCategory>(i));



			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as ISubCategoriesRepository).GetCountByFilters(filter, catIds, aIds, subIds);
				var pagedResult = new Dto.PagedResult<Dto.SubCategory>(itemsResult, new Dto.PagingInfo()
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
            List<Model.SubCategory> interCompanies = null;

            using (OfficeOpenXml.ExcelPackage package = new ExcelPackage())
            {
                interCompanies = (_itemsRepository as ISubCategoriesRepository).GetByFilters(filter, "Category.InterCompany.AssetType", null, null, null, null, null, null, null).ToList();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("SubCategories");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Cod";
                worksheet.Cells[1, 2].Value = "Descriere";
                worksheet.Cells[1, 3].Value = "Cod Categorie";
				worksheet.Cells[1, 4].Value = "Categorie";
				worksheet.Cells[1, 5].Value = "Cod SupraCategorie";
				worksheet.Cells[1, 6].Value = "SupraCategorie";

				int recordIndex = 2;
                foreach (var item in interCompanies)
                {
					worksheet.Cells[recordIndex, 1].Value = item.Code;
					worksheet.Cells[recordIndex, 2].Value = item.Name;
					worksheet.Cells[recordIndex, 3].Value = item.Category != null ? item.Category.Code : "";
					worksheet.Cells[recordIndex, 4].Value = item.Category != null ? item.Category.Name : "";
					worksheet.Cells[recordIndex, 5].Value = item.Category != null && item.Category.InterCompany != null ? item.Category.InterCompany.Code : "";
					worksheet.Cells[recordIndex, 6].Value = item.Category != null && item.Category.InterCompany != null ? item.Category.InterCompany.Name : "";
					recordIndex++;
                }

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
				worksheet.Column(4).AutoFit();
				worksheet.Column(5).AutoFit();
				worksheet.Column(6).AutoFit();

				using (var cells = worksheet.Cells[1, 1, 1, 6])
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
                    FileDownloadName = "SubCategories.xlsx"
                };

                return result;

            }
        }
	}
}
