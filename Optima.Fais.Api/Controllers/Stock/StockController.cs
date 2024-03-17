using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
using System.Drawing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharp;
using System.Drawing.Printing;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/stocks")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class StocksController : GenericApiController<Model.Stock, Dto.Stock>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public StocksController(ApplicationDbContext context, IStocksRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public async virtual Task<IActionResult> Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string categoryIds, string plantInitialIds, string plantActualIds, string exceptmaterialIds, string includes, bool showStock = false)
        {
            List<Model.Stock> items = null;
            IEnumerable<Dto.Stock> itemsResult = null;
            List<int?> rIds = null;
            List<int?> exceptIds = null;
            List<int?> pIniIds = null;
            List<int?> pFinIds = null;
            List<int?> storageIds = null;
            includes = includes ?? "Category,Company,Uom,Material,Brand,Partner,Error,PlantInitial,PlantActual,Storage,StorageInitial";

            //if (!showStock)
            //{
            //    categoryIds = "[" + -1 + "]";

            //}

            var userName = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(userName);
            }

            storageIds = _context.Set<Model.EmployeeStorage>().AsNoTracking().Where(e => e.EmployeeId == user.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.StorageId).ToList();

            if (storageIds.Count == 0)
            {
                storageIds = new List<int?>();
                storageIds.Add(-1);
            }

            if ((plantActualIds != null) && (plantActualIds.Length > 0)) pIniIds = JsonConvert.DeserializeObject<string[]>(plantActualIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((plantInitialIds != null) && (plantInitialIds.Length > 0)) pFinIds = JsonConvert.DeserializeObject<string[]>(plantInitialIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((categoryIds != null) && (categoryIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(categoryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((exceptmaterialIds != null) && (exceptmaterialIds.Length > 0)) exceptIds = JsonConvert.DeserializeObject<string[]>(exceptmaterialIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();


            items = (_itemsRepository as IStocksRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, rIds, pIniIds, pFinIds, exceptIds, storageIds, showStock).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Stock>(i));
			itemsResult = itemsResult.Where(i => i.Last_Incoming_Date > new DateTime(2022, 12, 31));
			itemsResult = itemsResult.Where(i => i.Last_Incoming_Date != new DateTime(2023, 10, 10)).Where(i => i.Code != "0024286023");
            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IStocksRepository).GetCountByFilters(filter, rIds, pIniIds, pFinIds, exceptIds, storageIds, showStock);
                var pagedResult = new Dto.PagedResult<Dto.Stock>(itemsResult, new Dto.PagingInfo()
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

		[HttpGet]
		[Route("initial", Order = -1)]
		public async virtual Task<IActionResult> GetInitial(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string categoryIds, string plantInitialIds, string plantActualIds, string exceptmaterialIds, string includes, bool showStock = false)
		{
			List<Model.Stock> items = null;
			IEnumerable<Dto.Stock> itemsResult = null;
			List<int?> rIds = null;
			List<int?> exceptIds = null;
			List<int?> pIniIds = null;
			List<int?> pFinIds = null;
			List<int?> storageIds = null;

			includes = includes ?? "Category,Company,Uom,Material,Brand,Partner,Error,PlantInitial,PlantActual,Storage,StorageInitial";


			var userName = HttpContext.User.Identity.Name;
			var user = await _userManager.FindByEmailAsync(userName);
			if (user == null)
			{
				user = await _userManager.FindByNameAsync(userName);
			}

			storageIds = _context.Set<Model.EmployeeStorage>().AsNoTracking().Where(e => e.EmployeeId == user.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.StorageId).ToList();

			if (storageIds.Count == 0)
			{
				storageIds = new List<int?>();
				storageIds.Add(-1);
			}

			if ((plantActualIds != null) && (plantActualIds.Length > 0)) pIniIds = JsonConvert.DeserializeObject<string[]>(plantActualIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((plantInitialIds != null) && (plantInitialIds.Length > 0)) pFinIds = JsonConvert.DeserializeObject<string[]>(plantInitialIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((categoryIds != null) && (categoryIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(categoryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((exceptmaterialIds != null) && (exceptmaterialIds.Length > 0)) exceptIds = JsonConvert.DeserializeObject<string[]>(exceptmaterialIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();


			items = (_itemsRepository as IStocksRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, rIds, pIniIds, pFinIds, exceptIds, storageIds, true).ToList();
			itemsResult = items.Select(i => _mapper.Map<Dto.Stock>(i));

			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as IStocksRepository).GetCountByFilters(filter, rIds, pIniIds, pFinIds, exceptIds, storageIds, true);
				var pagedResult = new Dto.PagedResult<Dto.Stock>(itemsResult, new Dto.PagingInfo()
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
		public async virtual Task<IActionResult> Export(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string categoryIds, string plantInitialIds, string plantActualIds, string exceptmaterialIds, string includes, bool showStock = false)
		{
			List<Model.Stock> items = null;
			IEnumerable<Dto.Stock> itemsResult = null;
			List<int?> rIds = null;
			List<int?> exceptIds = null;
			List<int?> pIniIds = null;
			List<int?> pFinIds = null;
			List<int?> storageIds = null;

			includes = includes ?? "Category,Company,Uom,Material,Brand,Partner,Error,PlantInitial,PlantActual,Storage,StorageInitial";


			var userName = HttpContext.User.Identity.Name;
			var user = await _userManager.FindByEmailAsync(userName);
			if (user == null)
			{
				user = await _userManager.FindByNameAsync(userName);
			}

			storageIds = _context.Set<Model.EmployeeStorage>().AsNoTracking().Where(e => e.EmployeeId == user.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.StorageId).ToList();

			if (storageIds.Count == 0)
			{
				storageIds = new List<int?>();
				storageIds.Add(-1);
			}

			if ((plantActualIds != null) && (plantActualIds.Length > 0)) pIniIds = JsonConvert.DeserializeObject<string[]>(plantActualIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((plantInitialIds != null) && (plantInitialIds.Length > 0)) pFinIds = JsonConvert.DeserializeObject<string[]>(plantInitialIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((categoryIds != null) && (categoryIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(categoryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((exceptmaterialIds != null) && (exceptmaterialIds.Length > 0)) exceptIds = JsonConvert.DeserializeObject<string[]>(exceptmaterialIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();


			using (ExcelPackage package = new ExcelPackage())
			{
				items = (_itemsRepository as IStocksRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, rIds, pIniIds, pFinIds, exceptIds, storageIds, true).ToList();
				itemsResult = items.Select(i => _mapper.Map<Dto.Stock>(i));

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("stock");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Batch";
				worksheet.Cells[1, 2].Value = "Cod categorie";
				worksheet.Cells[1, 3].Value = "Categorie";
				worksheet.Cells[1, 4].Value = "Companie";
				worksheet.Cells[1, 5].Value = "Moneda";
				worksheet.Cells[1, 6].Value = "Last_Incoming_Date";
				worksheet.Cells[1, 7].Value = "Cod Material";
				worksheet.Cells[1, 8].Value = "Plant Initial";
				worksheet.Cells[1, 9].Value = "Plant";
				worksheet.Cells[1, 10].Value = "Storage Initial";
				worksheet.Cells[1, 11].Value = "Storage";
				worksheet.Cells[1, 12].Value = "Brand";
				worksheet.Cells[1, 13].Value = "Cantitate";
				worksheet.Cells[1, 14].Value = "Short_Description";
				worksheet.Cells[1, 15].Value = "Long_Description";
				worksheet.Cells[1, 16].Value = "Supplier ID";
				worksheet.Cells[1, 17].Value = "Supplier";
				worksheet.Cells[1, 18].Value = "Value";
				worksheet.Cells[1, 19].Value = "UM";

				int recordIndex = 2;
				foreach (var item in items)
				{
					worksheet.Cells[recordIndex, 1].Value = item.Code;
					worksheet.Cells[recordIndex, 2].Value = item.Category != null ? item.Category.Code : "";
					worksheet.Cells[recordIndex, 3].Value = item.Category != null ? item.Category.Name : "";
					worksheet.Cells[recordIndex, 4].Value = item.Company != null ? item.Company.Code : "";
					worksheet.Cells[recordIndex, 5].Value = item.Uom != null ? item.Uom.Code : "";
					worksheet.Cells[recordIndex, 6].Value = item.Last_Incoming_Date;
					worksheet.Cells[recordIndex, 6].Style.Numberformat.Format = "mm/dd/yyyy";
					worksheet.Cells[recordIndex, 7].Value = item.Material != null ? item.Material.Code : "";
					worksheet.Cells[recordIndex, 8].Value = item.PlantInitial != null ? item.PlantInitial.Name : "";
					worksheet.Cells[recordIndex, 9].Value = item.PlantActual != null ? item.PlantActual.Code : "";
					worksheet.Cells[recordIndex, 10].Value = item.StorageInitial != null ? item.StorageInitial.Code : "";
					worksheet.Cells[recordIndex, 11].Value = item.Storage != null ? item.Storage.Code : "";
					worksheet.Cells[recordIndex, 12].Value = item.Brand != null ? item.Brand.Name : "";
					worksheet.Cells[recordIndex, 13].Value = item.Quantity;
					worksheet.Cells[recordIndex, 14].Value = item.Name;
					worksheet.Cells[recordIndex, 15].Value = item.LongName;
					worksheet.Cells[recordIndex, 16].Value = item.Partner != null ? item.Partner.RegistryNumber : "";
					worksheet.Cells[recordIndex, 17].Value = item.Partner != null ? item.Partner.Name : "";
					worksheet.Cells[recordIndex, 18].Value = item.Value;
					worksheet.Cells[recordIndex, 18].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 19].Value = item.UM;

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
				worksheet.Column(14).AutoFit();
				worksheet.Column(15).AutoFit();
				worksheet.Column(16).AutoFit();
				worksheet.Column(17).AutoFit();
				worksheet.Column(18).AutoFit();
				worksheet.Column(19).AutoFit();

				using (var cells = worksheet.Cells[1, 1, 1, 19])
				{
					cells.Style.Font.Bold = true;
					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
					cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
					cells.Style.Font.Color.SetColor(Color.Black);
				}

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				//HttpContext.Response.ContentType = entityFile.FileType;
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "stocks.xlsx"
				};

				return result;

			}
		}
	}
}
