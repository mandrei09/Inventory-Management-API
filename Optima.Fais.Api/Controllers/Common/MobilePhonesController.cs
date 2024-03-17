using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Utilities;
using PdfSharp;
using System.Drawing;
using Microsoft.AspNetCore.Identity;
using Optima.Fais.Dto;
using Optima.Fais.Model;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/mobilephones")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class MobilePhonesController : GenericApiController<Model.MobilePhone, Dto.MobilePhone>
    {
        private readonly UserManager<Model.ApplicationUser> userManager;

        public MobilePhonesController(ApplicationDbContext context, IMobilePhonesRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            this.userManager = userManager;
        }


        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
        {
            List<Model.MobilePhone> items = null;
            IEnumerable<Dto.MobilePhone> itemsResult = null;


            items = (_itemsRepository as IMobilePhonesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.MobilePhone>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IMobilePhonesRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.MobilePhone>(itemsResult, new Dto.PagingInfo()
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

		[HttpGet("checkUnique/{number}")]
		public async Task<bool> CheckUnique(string number)
		{
			Model.MobilePhone mobilePhone = null;
			var found = false;

			if (number != "" && number != null && number.Trim().Length == 10)
			{
				mobilePhone = await _context.Set<Model.MobilePhone>().Where(a => a.Name.Trim() == number.Trim() && a.IsDeleted == false).FirstOrDefaultAsync();
			}


			if (mobilePhone != null)
			{
				found = true;
				return found;
			}
			else
			{
				return found;
			}
		}

		[HttpGet("export")]
		public IActionResult Export(string filter, string administrationIds, string admCenterIds)
		{
			List<int> aIds = null;
			List<int> admIds = null;
			List<Model.MobilePhone> mobilePhones = null;

			if ((admCenterIds != null) && (admCenterIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();
			if ((administrationIds != null) && (administrationIds.Length > 0)) admIds = JsonConvert.DeserializeObject<string[]>(administrationIds).ToList().Select(int.Parse).ToList();
			using (ExcelPackage package = new ExcelPackage())
			{
				mobilePhones = (_itemsRepository as IMobilePhonesRepository).GetByFilters(filter, null, null, null, null, null).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("mobile_phones");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Nr de telefon";
				worksheet.Cells[1, 2].Value = "Activ";

				int recordIndex = 2;
				foreach (var item in mobilePhones)
				{
					worksheet.Cells[recordIndex, 1].Value = item.Name;
					worksheet.Cells[recordIndex, 2].Value = item.IsDeleted ? "NU" : "DA";
					recordIndex++;
				}

                worksheet.Cells[recordIndex, 1].Value = "END";

                worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();

				using (var cells = worksheet.Cells[1, 1, 1, 2])
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
					FileDownloadName = "numere_telefon.xlsx"
				};

				return result;

			}
		}

        [HttpPost]
        [Route("import")]
        public async Task<ImportITModelResult> Import([FromBody] ImportMobilePhone data)
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

                return await (_itemsRepository as IMobilePhonesRepository).Import(data);
            }
            else
            {
                return new Model.ImportITModelResult { Success = false, Message = $"Va rugam sa va autentificati!" };
            }
        }
    }
}
