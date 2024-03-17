using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharp;
using System.Drawing.Printing;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/dimensions")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class DimensionsController : GenericApiController<Model.Dimension, Dto.Dimension>
    {
        public DimensionsController(ApplicationDbContext context, IDimensionsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string assetCategoryIds, string includes)
        {
            List<Model.Dimension> items = null;
            IEnumerable<Dto.Dimension> itemsResult = null;
            List<int?> aCategoryIds = null;

            includes = includes ?? "AssetCategory";

            if (assetCategoryIds != null && !assetCategoryIds.StartsWith("["))
            {
                assetCategoryIds = "[" + assetCategoryIds + "]";
            }

            if ((assetCategoryIds != null) && (assetCategoryIds.Length > 0)) aCategoryIds = JsonConvert.DeserializeObject<string[]>(assetCategoryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IDimensionsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, aCategoryIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Dimension>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IDimensionsRepository).GetCountByFilters(filter, aCategoryIds);
                var pagedResult = new Dto.PagedResult<Dto.Dimension>(itemsResult, new Dto.PagingInfo()
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


        [AllowAnonymous]
        [HttpGet]
        [Route("sync")]
        public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.Dimension> items = (_itemsRepository as IDimensionsRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<Dto.DimensionSync>(i));
            var pagedResult = new Dto.PagedResult<Dto.DimensionSync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }

		[HttpGet("export")]
		public IActionResult Export(string filter)
		{
			List<Model.Dimension> departments = null;
			int rowNumber = 0;
			using (ExcelPackage package = new ExcelPackage())
			{
				departments = (_itemsRepository as IDimensionsRepository).GetByFilters(filter, null, null, null, null, null, null).ToList();
				departments = departments.OrderBy(l => l.Width).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Dimenssiuni");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod";
				worksheet.Cells[1, 2].Value = "Descriere";


				int recordIndex = 2;
				foreach (var item in departments)
				{
					rowNumber++;
					worksheet.Cells[recordIndex, 1].Value = item.Width;
					worksheet.Cells[recordIndex, 2].Value = item.Height;



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
