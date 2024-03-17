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
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharp;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/accountancies")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AccountanciesController : GenericApiController<Model.Accountancy, Dto.Accountancy>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public AccountanciesController(ApplicationDbContext context, IAccountanciesRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string interCompanyIds, string accountIds, string expAccountIds, string includes)
        {
            List<Model.Accountancy> items = null;
            IEnumerable<Dto.Accountancy> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;
            List<int?> catIds = null;

            includes = "Account,AssetCategory,AssetType,ExpAccount,SubCategory.Category.InterCompany,SubCategory,";


			if (interCompanyIds != null && !interCompanyIds.StartsWith("["))
			{
                interCompanyIds = "[" + interCompanyIds + "]";
			}

			if (accountIds != null && !accountIds.StartsWith("["))
			{
                accountIds = "[" + accountIds + "]";
			}

            if (expAccountIds != null && !expAccountIds.StartsWith("["))
            {
                expAccountIds = "[" + expAccountIds + "]";
            }

            if ((interCompanyIds != null) && (interCompanyIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(interCompanyIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((accountIds != null) && (accountIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(accountIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((expAccountIds != null) && (expAccountIds.Length > 0)) catIds = JsonConvert.DeserializeObject<string[]>(expAccountIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IAccountanciesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, eIds, catIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Accountancy>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IAccountanciesRepository).GetCountByFilters(filter, cIds, eIds, catIds);
                var pagedResult = new Dto.PagedResult<Dto.Accountancy>(itemsResult, new Dto.PagingInfo()
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
        [Route("add")]
        public async virtual Task<IActionResult> AddAccountancy([FromBody] Dto.AccountancyAdd offerMatAdd)
        {
            Model.Accountancy accountancy = null;

            for (int i = 0; i < offerMatAdd.InterCompanyIds.Length; i++)
            {
                var interCompany = await _context.Set<Model.InterCompany>().Where(u => u.Id == offerMatAdd.InterCompanyIds[i]).FirstOrDefaultAsync();


                accountancy = new Model.Accountancy()
                {
                    AccountId = offerMatAdd.AccountId,
                    // InterCompanyId = offerMatAdd.InterCompanyIds[i],
                    ExpAccountId = offerMatAdd.ExpAccountId,
                    AssetCategoryId = offerMatAdd.AssetCategoryId,
                    AssetTypeId = offerMatAdd.AssetTypeId
                };

                _context.Add(accountancy);

                _context.SaveChanges();
            }

            return Ok(StatusCode(200));
        }

        [HttpDelete("remove/{id}")]
        public virtual IActionResult DeleteOfferMaterial(int id)
        {

            Model.Accountancy accountancy = _context.Set<Model.Accountancy>().Where(a => a.Id == id).Single();

            if (accountancy != null)
            {

                accountancy.IsDeleted = true;
                accountancy.ModifiedAt = DateTime.Now;
                _context.Update(accountancy);
                _context.SaveChanges();
            }
            else
            {
                return Ok(StatusCode(404));
            }

            return Ok(StatusCode(200));
        }


		[HttpGet("export")]
		public IActionResult Export(string filter)
		{
			List<int?> aIds = null;
			List<Model.Accountancy> accountancies = null;
			using (ExcelPackage package = new ExcelPackage())
			{
				accountancies = (_itemsRepository as IAccountanciesRepository).GetByFilters(filter, "Account,AssetCategory,AssetType,ExpAccount,SubCategory.Category.InterCompany,SubCategory,", null, null, null, null, null, null, null).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Mapari G.L. account");
				//First add the headers

				worksheet.Cells[1, 1].Value = "Account";
				worksheet.Cells[1, 2].Value = "Cod Clasificare";
				worksheet.Cells[1, 3].Value = "Clasificare";
				worksheet.Cells[1, 4].Value = "Cod Tip";
				worksheet.Cells[1, 5].Value = "Tip";
				worksheet.Cells[1, 6].Value = "Bal.sh.acct APC";
				worksheet.Cells[1, 7].Value = "Cod SubCategorie";
				worksheet.Cells[1, 8].Value = "SubCategorie";
				worksheet.Cells[1, 9].Value = "Cod Categorie";
				worksheet.Cells[1, 10].Value = "Categorie";
				worksheet.Cells[1, 11].Value = "Cod SupraCategorie";
				worksheet.Cells[1, 12].Value = "SupraCategorie";
				worksheet.Cells[1, 13].Value = "Value";

				int recordIndex = 2;
				foreach (var item in accountancies)
				{
					worksheet.Cells[recordIndex, 1].Value = item.Account != null ? item.Account.Code : "";
					worksheet.Cells[recordIndex, 2].Value = item.AssetCategory != null ? item.AssetCategory.Code : "";
					worksheet.Cells[recordIndex, 3].Value = item.AssetCategory != null ? item.AssetCategory.Name : "";
					worksheet.Cells[recordIndex, 4].Value = item.AssetType != null ? item.AssetType.Code : "";
					worksheet.Cells[recordIndex, 5].Value = item.AssetType != null ? item.AssetType.Name : "";
					worksheet.Cells[recordIndex, 6].Value = item.ExpAccount != null ? item.ExpAccount.Name : "";
					worksheet.Cells[recordIndex, 7].Value = item.SubCategory != null ? item.SubCategory.Code : "";
					worksheet.Cells[recordIndex, 8].Value = item.SubCategory != null ? item.SubCategory.Name : "";
					worksheet.Cells[recordIndex, 9].Value = item.SubCategory != null && item.SubCategory.Category != null ? item.SubCategory.Category.Code : "";
					worksheet.Cells[recordIndex, 10].Value = item.SubCategory != null && item.SubCategory.Category != null ? item.SubCategory.Category.Name : "";
					worksheet.Cells[recordIndex, 11].Value = item.SubCategory != null && item.SubCategory.Category != null && item.SubCategory.Category.InterCompany != null ? item.SubCategory.Category.InterCompany.Code : "";
					worksheet.Cells[recordIndex, 12].Value = item.SubCategory != null && item.SubCategory.Category != null && item.SubCategory.Category.InterCompany != null ? item.SubCategory.Category.InterCompany.Name : "";
					worksheet.Cells[recordIndex, 13].Value = item.Value;
					recordIndex++;
				}

				worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();
				worksheet.Column(3).AutoFit();
				worksheet.Column(4).AutoFit();
				worksheet.Column(5).AutoFit();
				worksheet.Column(6).AutoFit();
				worksheet.Column(7).AutoFit();
				worksheet.Column(8).AutoFit();
				worksheet.Column(9).AutoFit();
				worksheet.Column(10).AutoFit();
				worksheet.Column(11).AutoFit();
				worksheet.Column(12).AutoFit();
				worksheet.Column(13).AutoFit();

				using (var cells = worksheet.Cells[1, 1, 1, 13])
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
					FileDownloadName = "MAPARI G.L..xlsx"
				};

				return result;

			}
		}
	}
}
