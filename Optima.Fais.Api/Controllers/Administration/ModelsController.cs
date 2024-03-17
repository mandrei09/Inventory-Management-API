using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
	[Authorize]
	[Route("api/models")]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public partial class ModelsController : GenericApiController<Model.Model, Dto.Model>
	{
        private readonly UserManager<Model.ApplicationUser> userManager;

        public ModelsController(ApplicationDbContext context, IModelsRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
			: base(context, itemsRepository, mapper)
		{
            this.userManager = userManager;
        }

		[HttpGet]
		[Route("", Order = -1)]
		public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string brandIds, string includes)
		{
			List<Model.Model> items = null;
			IEnumerable<Dto.Model> itemsResult = null;
			List<int> bIds = null;

			includes = includes + ",Brand";

			if (brandIds != null && !brandIds.StartsWith("["))
			{
				brandIds = "[" + brandIds + "]";
			}

			if ((brandIds != null) && (brandIds.Length > 0)) bIds = JsonConvert.DeserializeObject<string[]>(brandIds).ToList().Select(int.Parse).ToList();

			items = (_itemsRepository as IModelsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, bIds).ToList();
			itemsResult = items.Select(i => _mapper.Map<Dto.Model>(i));



			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as IModelsRepository).GetCountByFilters(filter, bIds);
				var pagedResult = new Dto.PagedResult<Dto.Model>(itemsResult, new Dto.PagingInfo()
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
		[Route("allowAnonymous", Order = -1)]
		public virtual IActionResult GetAllowAnonymous(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string brandIds, string includes)
		{
			List<Model.Model> items = null;
			IEnumerable<Dto.Model> itemsResult = null;
			List<int> bIds = null;

			includes = includes + ",Brand";

			if (brandIds != null && !brandIds.StartsWith("["))
			{
				brandIds = "[" + brandIds + "]";
			}

			if ((brandIds != null) && (brandIds.Length > 0)) bIds = JsonConvert.DeserializeObject<string[]>(brandIds).ToList().Select(int.Parse).ToList();

			items = (_itemsRepository as IModelsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, bIds).ToList();
			itemsResult = items.Select(i => _mapper.Map<Dto.Model>(i));



			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as IModelsRepository).GetCountByFilters(filter, bIds);
				var pagedResult = new Dto.PagedResult<Dto.Model>(itemsResult, new Dto.PagingInfo()
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

        [HttpPost]
        [Route("import")]
        public async Task<ImportITModelResult> Import([FromBody] ImportModel data)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }

                _context.UserId = user.Id.ToString();
                try
                {
                    if(data.Index == 0)
					{
                        var IdBrand = await _context.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Brand").SingleAsync();
                        var IdModel = await _context.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Model").SingleAsync();

                        return await (_itemsRepository as IModelsRepository).ImportModel(data);
                    }
					else
					{
                        return await (_itemsRepository as IModelsRepository).ImportModel(data);
                    }

                }
				catch (System.Exception ex)
				{

                    return new Model.ImportITModelResult { Success = false, Message = ex.Message };
                }
            }
            else
            {
                return new Model.ImportITModelResult { Success = false, Message = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpGet("export")]
        public IActionResult Export(string filter)
        {
            List<Model.Model> models = null;

            using (ExcelPackage package = new ExcelPackage())
            {
                models = (_itemsRepository as IModelsRepository).GetByFilters(filter, ",Brand.DictionaryItem", null, null, null, null, null, true).ToList();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("mobile_phones");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Echipament";
                worksheet.Cells[1, 2].Value = "Marca";
                worksheet.Cells[1, 3].Value = "Model";
                worksheet.Cells[1, 4].Value = "Lungime Serie";
                worksheet.Cells[1, 5].Value = "Lungime IMEI";
                worksheet.Cells[1, 6].Value = "Activ";

                int recordIndex = 2;
                foreach (var item in models)
                {
                    worksheet.Cells[recordIndex, 1].Value = item.Name == "Altele" ? "Altele" : item.Brand != null && item.Brand.DictionaryItem != null ? item.Brand.DictionaryItem.Name : "";
                    worksheet.Cells[recordIndex, 2].Value = item.Name == "Altele" ? "Altele" : item.Brand != null ? item.Brand.Name : "";
                    worksheet.Cells[recordIndex, 3].Value = item.Name;
                    worksheet.Cells[recordIndex, 4].Value = item.SNLength;
                    worksheet.Cells[recordIndex, 5].Value = item.IMEILength;
                    worksheet.Cells[recordIndex, 6].Value = item.IsDeleted ? "NU" : "DA";
                    recordIndex++;
                }

                worksheet.Cells[recordIndex, 1].Value = "END";

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
                    cells.Style.Fill.BackgroundColor.SetColor(Color.DarkOrange);
                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "modele-brand.xlsx"
                };

                return result;

            }
        }
    }
}
