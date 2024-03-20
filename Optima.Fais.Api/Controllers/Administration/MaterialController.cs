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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Org.BouncyCastle.Utilities;
using PdfSharp;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/materials")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class MaterialsController : GenericApiController<Model.Material, Dto.Material>
    {
        public MaterialsController(ApplicationDbContext context, IMaterialsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string countyIds, string exceptmaterialIds, string materialIds, string includes, bool hasSubCategory = false)
        {
            List<Model.Material> items = null;
            IEnumerable<Dto.Material> itemsResult = null;
            List<int> rIds = null;
            List<int?> exceptIds = null;
            List<int?> mIds = null;

            includes = includes ?? "Account,ExpAccount,AssetCategory,SubCategory,SubCategoryEN,SubType";

            if (materialIds != null && !materialIds.StartsWith("["))
            {
                materialIds = "[" + materialIds + "]";
            }

            if ((countyIds != null) && (countyIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(countyIds).ToList().Select(int.Parse).ToList();
            if ((exceptmaterialIds != null) && (exceptmaterialIds.Length > 0)) exceptIds = JsonConvert.DeserializeObject<string[]>(exceptmaterialIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((materialIds != null) && (materialIds.Length > 0)) mIds = JsonConvert.DeserializeObject<string[]>(materialIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IMaterialsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, rIds, mIds, exceptIds, hasSubCategory).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Material>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IMaterialsRepository).GetCountByFilters(filter, rIds, mIds, exceptIds, hasSubCategory);
                var pagedResult = new Dto.PagedResult<Dto.Material>(itemsResult, new Dto.PagingInfo()
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
            List<Model.Material> items = (_itemsRepository as IMaterialsRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<Dto.MaterialSync>(i));
            var pagedResult = new Dto.PagedResult<Dto.MaterialSync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }

		[HttpGet("export")]
		public IActionResult Export(string filter, string administrationIds, string admCenterIds)
		{
			List<int> aIds = null;
			List<int> admIds = null;
			List<Model.Material> materials = null;

			if ((admCenterIds != null) && (admCenterIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();
			if ((administrationIds != null) && (administrationIds.Length > 0)) admIds = JsonConvert.DeserializeObject<string[]>(administrationIds).ToList().Select(int.Parse).ToList();
			using (ExcelPackage package = new ExcelPackage())
			{
				materials = (_itemsRepository as IMaterialsRepository).GetByFilters(filter, ",SubCategory", null, null, null, null, null, null, null, false).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("produse");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod";
				worksheet.Cells[1, 2].Value = "Descriere";
				//worksheet.Cells[1, 3].Value = "Cod SubCategorie";
				//worksheet.Cells[1, 4].Value = "SubCategorie";

				int recordIndex = 2;
				foreach (var item in materials)
				{
					worksheet.Cells[recordIndex, 1].Value = item.Code;
					worksheet.Cells[recordIndex, 2].Value = item.Name;
					//worksheet.Cells[recordIndex, 3].Value = item.SubCategory != null ? item.SubCategory.Code : "";
					//worksheet.Cells[recordIndex, 4].Value = item.SubCategory != null ? item.SubCategory.Name : "";
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
					FileDownloadName = "produse.xlsx"
				};

				return result;

			}
		}
	}
}
