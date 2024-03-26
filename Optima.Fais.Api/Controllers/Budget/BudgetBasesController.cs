using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using Optima.Fais.Api.Helpers;
using Optima.Fais.Api.Services;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.EfRepository;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/budgetbases")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class BudgetBasesController : GenericApiController<Model.BudgetBase, Dto.BudgetBase>
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<Model.ApplicationUser> userManager;

        private readonly IEmailSender _emailSender;
		private readonly IRequestsService _requestsService;

		public BudgetBasesController(ApplicationDbContext context,
            IBudgetBasesRepository itemsRepository, 
            //IRepository<Model.Budget> itemsRepository,
            IMapper mapper, IWebHostEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager, IEmailSender emailSender, IRequestsService requestsService)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            this._emailSender = emailSender;
			this._requestsService = requestsService;
		}

		[HttpGet]
		[Route("", Order = -1)]
		public virtual async Task<IActionResult> GetDepDetails(int page, int pageSize, string sortColumn, string sortDirection,
			string includes, string jsonFilter)
		{
			AssetDepTotal depTotal = null;
			AssetCategoryTotal catTotal = null;
			BudgetFilter assetFilter = null;
			Paging paging = null;
			Sorting sorting = null;

            // includes ??= "BudgetBase.AssetType,BudgetBase.ProjectType,BudgetBase.Region,BudgetBase.AdmCenter,BudgetBase.Activity,BudgetBase.Country,BudgetBase.Company,BudgetBase.Employee,BudgetBase.AccMonth,BudgetBase.AppState,";
            //includes = "Company,Project,Administration,CostCenter,SubType.Type.MasterType,SubType.Type,SubType,Employee,AccMonth,InterCompany,Partner,Account,AppState";

            if (sortDirection == null)
                sortDirection = "asc";

			if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
				|| (page <= 0) || (pageSize <= 0))
				return BadRequest();

			paging = new Paging() { Page = page, PageSize = pageSize };
			sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();


			includes = includes + "BudgetBase.BudgetMonthBase,BudgetBase.Project";

            var items = (_itemsRepository as IBudgetBasesRepository)
               .GetBuget(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();

			var itemsResource = _mapper.Map<List<Model.BudgetBaseDetail>, List<Dto.BudgetBase>>(items);


			var result = new BudgetBasePagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal, catTotal);

			return Ok(result);
		}

        [HttpGet]
        [Route("detailui", Order = -1)]
        public virtual async Task<IActionResult> GetDepDetailUIs(int page, int pageSize, string sortColumn, string sortDirection,
            string includes, string jsonFilter, string filter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            BudgetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            sortColumn = "BudgetBase.Code";

            // var count = _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgets").ToList();

            includes = includes + ",BudgetBase.Company,BudgetBase.Project,BudgetBase.AdmCenter,BudgetBase.CostCenter,BudgetBase.Region,BudgetBase.AssetType,BudgetBase.AppState,BudgetBase.Uom,BudgetBase.BudgetMonthBase";

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();


            if (filter != null)
            {
                assetFilter.Filter = filter;
            }

            var items = (_itemsRepository as IBudgetBasesRepository)
                .GetBugetUI(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.BudgetBaseDetail>, List<Dto.BudgetBaseUI>>(items);

            var result = new BudgetBaseUIPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }

        [HttpGet]
        [Route("detailfreezeui", Order = -1)]
        public virtual async Task<IActionResult> GetDepDetailFreezeUIs(int page, int pageSize, string sortColumn, string sortDirection,
            string includes, string jsonFilter, string filter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            BudgetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            sortColumn = "BudgetBase.Code";

            // var count = _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgets").ToList();

            includes = includes + ",BudgetBase.Project";

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();


            if (filter != null)
            {
                assetFilter.Filter = filter;
            }

            var items = (_itemsRepository as IBudgetBasesRepository)
                .GetBugetFreezeUI(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.BudgetBaseDetail>, List<Dto.BudgetBaseFreezeUI>>(items);

            var result = new BudgetBaseFreezeUIPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }

        [HttpGet("detail/{id:int}")]
        public virtual IActionResult GetDetail(int id, string includes)
        {
            var budgetBase = (_itemsRepository as IBudgetBasesRepository).GetDetailsById(id, includes);
            var result = _mapper.Map<Dto.BudgetBase>(budgetBase);

            return Ok(result);
        }


        //[Authorize]
        //[HttpPost]
        //[Route("import")]
        //public virtual IActionResult Import([FromBody] BudgetImport assetImport)
        //{
        //    if (HttpContext.User.Identity.Name != null)
        //    {
        //        string userName = HttpContext.User.Identity.Name;

        //        var user = _context.Users.Include(r => r.Claims).Where(u => u.Id == "92E74C4F-A79A-4C83-A7D0-A3202BD2507F").SingleOrDefault();
        //        assetImport.UserId = user.Id;
        //        //if (userName != null)
        //        //{
                    
                   
        //        //}
        //    }

        //    int budgetId = 0;
        //    budgetId = (_itemsRepository as IBudgetsRepository).BudgetImport(assetImport);

        //    var budget = _context.Set<Model.Budget>().Include(b =>b.BudgetMonths).Where(b => b.Id == budgetId).Single();

        //    budget.ValueIni = budget.BudgetMonths.Sum(a => a.Value);
        //    budget.ValueFin = budget.BudgetMonths.Sum(a => a.Value);

        //    _context.SaveChanges();

        //    return Ok(budgetId);
        //}

        [Authorize]
        [HttpPost]
        [Route("import")]
        public async virtual Task<ImportBudgetResult> Import([FromBody] BudgetBaseImport assetImport)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string userName = HttpContext.User.Identity.Name;

                var user = _context.Users.Include(r => r.Claims).Where(u => u.Id == "92E74C4F-A79A-4C83-A7D0-A3202BD2507F").SingleOrDefault();
                assetImport.UserId = user.Id;

				var result = await (_itemsRepository as IBudgetBasesRepository).BudgetBaseImport(assetImport);

				return new Model.ImportBudgetResult { Success = result.Success, Message = result.Message, Id = result.Id };
			}
			else
			{
				return new Model.ImportBudgetResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}

			
		}

        [HttpGet("templateImport")]
        public async Task<IActionResult> Template()
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                // add a new worksheet to the empty workbook

                int rowIndex = 0;

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Template_Budget");

                string[] columnNames = {
                    "Project ID",
                    "Budget Owner (email address)",
                    "WBS",
                    "Tara Name",
                    "Tara Code",
                    "Activity",
                    "Business Unit Name",
                    "Business Unit Code",
                    "Profit Center",
                    "PC Details",
                    "Departament Name",
                    "Departament Code",
                    "Project Name",
                    "Project Code",
                    "Project Details",
                    "Cost Type Name",
                    "Cost Type Code",
                    "ACQ type",
                    "Implementation Date (month&year)",
                    "Dep Per BGT (month)",
                    "Dep Per ACT (month)",
                    "mar.23",
                    "apr.23",
                    "mai.23",
                    "iun.23",
                    "iul.23",
                    "aug.23",
                    "sept.23",
                    "oct.23",
                    "nov.23",
                    "dec.23",
                    "ian.24",
                    "feb.24",
                    "mar.24"
                };

                foreach (var columnName in columnNames)
                {
                    worksheet.Cells[4, ++rowIndex].Value = columnName;
                }

                worksheet.Cells["V3:AH3"].Value = "RON";

                worksheet.View.ZoomScale = 100;

                worksheet.Cells.AutoFitColumns();

                worksheet.Row(4).Height = 30.00;

                worksheet.Cells["A1:AH4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:AH4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["A4:AH4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:AH4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 176, 240));

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Template_Import_Far_Dante.xlsx"
                };

                return result;
            }
        }

        [HttpGet("export")]
        public IActionResult ExporteMAG(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {

            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            BudgetFilter assetFilter = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            int rowNumber = 0;
            Paging paging = null;
            Sorting sorting = null;
            string userName = string.Empty;
            string role = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();


            includes = includes + "BudgetBase.BudgetBaseAsset.Asset,BudgetBase.BudgetForecast,BudgetBase.BudgetMonthBase,BudgetBase.Project,BudgetBase.Employee,BudgetBase.Country,BudgetBase.Project,BudgetBase.Activity,BudgetBase.Project,BudgetBase.CostCenter,BudgetBase.Division,BudgetBase.Department,BudgetBase.AssetType,BudgetBase.AppState,BudgetBase.ProjectType,BudgetBase.StartMonth,BudgetBase.AdmCenter,";

            var items = (_itemsRepository as IBudgetBasesRepository)
                .GetBuget(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();

            int fiscalYear = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true && a.IsDeleted == false).SingleOrDefault().FiscalYear;

            using (ExcelPackage package = new ExcelPackage())
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Buget");

                int rowCell = 2;
                int rowTCell = 5;
                int rowTotal = items.Count() + 5;
                //First add the headers

                worksheet.Cells[3, 21].Value = "ACT";
                worksheet.Cells[3, 22].Value = "ACT";
                worksheet.Cells[3, 23].Value = "ACT";
                worksheet.Cells[3, 24].Value = "ACT";
                worksheet.Cells[3, 25].Value = "ACT";
                worksheet.Cells[3, 26].Value = "ACT";
                worksheet.Cells[3, 27].Value = "ACT";
                worksheet.Cells[3, 28].Value = "FC2";
                worksheet.Cells[3, 29].Value = "FC2";
                worksheet.Cells[3, 30].Value = "FC2";
                worksheet.Cells[3, 31].Value = "FC2";
                worksheet.Cells[3, 32].Value = "FC2";
                worksheet.Cells[3, 33].Value = "FC2";


                worksheet.Cells[3, 35].Value = "ACT";
                worksheet.Cells[3, 36].Value = "ACT";
                worksheet.Cells[3, 37].Value = "ACT";
                worksheet.Cells[3, 38].Value = "ACT";
                worksheet.Cells[3, 39].Value = "ACT";
                worksheet.Cells[3, 40].Value = "ACT";
                worksheet.Cells[3, 41].Value = "ACT";
                worksheet.Cells[3, 42].Value = "FC2";
                worksheet.Cells[3, 43].Value = "FC2";
                worksheet.Cells[3, 44].Value = "FC2";
                worksheet.Cells[3, 45].Value = "FC2";
                worksheet.Cells[3, 46].Value = "FC2";
                worksheet.Cells[3, 47].Value = "FC2";


                worksheet.Cells[3, 49].Value = "ACT";
                worksheet.Cells[3, 50].Value = "ACT";
                worksheet.Cells[3, 51].Value = "ACT";
                worksheet.Cells[3, 52].Value = "ACT";
                worksheet.Cells[3, 53].Value = "ACT";
                worksheet.Cells[3, 54].Value = "ACT";
                worksheet.Cells[3, 55].Value = "ACT";
                worksheet.Cells[3, 56].Value = "FC2";
                worksheet.Cells[3, 57].Value = "FC2";
                worksheet.Cells[3, 58].Value = "FC2";
                worksheet.Cells[3, 59].Value = "FC2";
                worksheet.Cells[3, 60].Value = "FC2";
                worksheet.Cells[3, 61].Value = "FC2";


                worksheet.Cells[3, 63].Value = "ACT";
                worksheet.Cells[3, 64].Value = "ACT";
                worksheet.Cells[3, 65].Value = "ACT";
                worksheet.Cells[3, 66].Value = "ACT";
                worksheet.Cells[3, 67].Value = "ACT";
                worksheet.Cells[3, 68].Value = "ACT";
                worksheet.Cells[3, 69].Value = "ACT";
                worksheet.Cells[3, 70].Value = "FC2";
                worksheet.Cells[3, 71].Value = "FC2";
                worksheet.Cells[3, 72].Value = "FC2";
                worksheet.Cells[3, 73].Value = "FC2";
                worksheet.Cells[3, 74].Value = "FC2";
                worksheet.Cells[3, 75].Value = "FC2";

                worksheet.Cells[4, 21].Value = "RON";
                worksheet.Cells[4, 22].Value = "RON";
                worksheet.Cells[4, 23].Value = "RON";
                worksheet.Cells[4, 24].Value = "RON";
                worksheet.Cells[4, 25].Value = "RON";
                worksheet.Cells[4, 26].Value = "RON";
                worksheet.Cells[4, 27].Value = "RON";
                worksheet.Cells[4, 28].Value = "RON";
                worksheet.Cells[4, 29].Value = "RON";
                worksheet.Cells[4, 30].Value = "RON";
                worksheet.Cells[4, 31].Value = "RON";
                worksheet.Cells[4, 32].Value = "RON";
                worksheet.Cells[4, 33].Value = "RON";

                worksheet.Cells[4, 35].Value = "RON";
                worksheet.Cells[4, 36].Value = "RON";
                worksheet.Cells[4, 37].Value = "RON";
                worksheet.Cells[4, 38].Value = "RON";
                worksheet.Cells[4, 39].Value = "RON";
                worksheet.Cells[4, 40].Value = "RON";
                worksheet.Cells[4, 41].Value = "RON";
                worksheet.Cells[4, 42].Value = "RON";
                worksheet.Cells[4, 43].Value = "RON";
                worksheet.Cells[4, 44].Value = "RON";
                worksheet.Cells[4, 45].Value = "RON";
                worksheet.Cells[4, 46].Value = "RON";
                worksheet.Cells[4, 47].Value = "RON";

                worksheet.Cells[4, 49].Value = "RON";
                worksheet.Cells[4, 50].Value = "RON";
                worksheet.Cells[4, 51].Value = "RON";
                worksheet.Cells[4, 52].Value = "RON";
                worksheet.Cells[4, 53].Value = "RON";
                worksheet.Cells[4, 54].Value = "RON";
                worksheet.Cells[4, 55].Value = "RON";
                worksheet.Cells[4, 56].Value = "RON";
                worksheet.Cells[4, 57].Value = "RON";
                worksheet.Cells[4, 58].Value = "RON";
                worksheet.Cells[4, 59].Value = "RON";
                worksheet.Cells[4, 60].Value = "RON";
                worksheet.Cells[4, 61].Value = "RON";

                worksheet.Cells[4, 63].Value = "RON";
                worksheet.Cells[4, 64].Value = "RON";
                worksheet.Cells[4, 65].Value = "RON";
                worksheet.Cells[4, 66].Value = "RON";
                worksheet.Cells[4, 67].Value = "RON";
                worksheet.Cells[4, 68].Value = "RON";
                worksheet.Cells[4, 69].Value = "RON";
                worksheet.Cells[4, 70].Value = "RON";
                worksheet.Cells[4, 71].Value = "RON";
                worksheet.Cells[4, 72].Value = "RON";
                worksheet.Cells[4, 73].Value = "RON";
                worksheet.Cells[4, 74].Value = "RON";
                worksheet.Cells[4, 75].Value = "RON";


                worksheet.Cells[5, 1].Value = "Budget Owner\r\n(email adress)";
                worksheet.Cells[5, 2].Value = "WBS";
                worksheet.Cells[5, 3].Value = "Tara Name";
                worksheet.Cells[5, 4].Value = "Tara Code";
                worksheet.Cells[5, 5].Value = "Activity";
                worksheet.Cells[5, 6].Value = "Business Unit Name";
                worksheet.Cells[5, 7].Value = "Business Unit Code";
                worksheet.Cells[5, 8].Value = "Profit Center";
                worksheet.Cells[5, 9].Value = "";
                worksheet.Cells[5, 10].Value = "Departament Name";
                worksheet.Cells[5, 11].Value = "Departament Code";
                worksheet.Cells[5, 12].Value = "Project Name";
                worksheet.Cells[5, 13].Value = "Project Code";
                worksheet.Cells[5, 14].Value = "Project Details";
                worksheet.Cells[5, 15].Value = "Cost Type Name";
                worksheet.Cells[5, 16].Value = "Cost Type Code";
                worksheet.Cells[5, 17].Value = "ACQ type";
                worksheet.Cells[5, 18].Value = "Implementation Date (month&year)";
                worksheet.Cells[5, 19].Value = "Dep Per\r\nBGT(month)";
                worksheet.Cells[5, 20].Value = "Dep Per\r\nACT(month)";

                string pastYear = (fiscalYear -1).ToString();
                string currentYear = (fiscalYear).ToString();

                worksheet.Cells[5, 21].Value = "4/1/" + pastYear;
                worksheet.Cells[5, 22].Value = "5/1/" + pastYear;
                worksheet.Cells[5, 23].Value = "6/1/" + pastYear;
                worksheet.Cells[5, 24].Value = "7/1/" + pastYear;
                worksheet.Cells[5, 25].Value = "8/1/" + pastYear;
                worksheet.Cells[5, 26].Value = "9/1/" + pastYear;
                worksheet.Cells[5, 27].Value = "10/1/" + pastYear;
                worksheet.Cells[5, 28].Value = "11/1/" + pastYear;
                worksheet.Cells[5, 29].Value = "12/1/" + pastYear;
                worksheet.Cells[5, 30].Value = "1/1/" + currentYear;
                worksheet.Cells[5, 31].Value = "2/1/" + currentYear;
                worksheet.Cells[5, 32].Value = "3/1/" + currentYear;
                worksheet.Cells[5, 33].Value = "TY FY22";

                worksheet.Cells[5, 35].Value = "4/1/" + pastYear;
                worksheet.Cells[5, 36].Value = "5/1/" + pastYear;
                worksheet.Cells[5, 37].Value = "6/1/" + pastYear;
                worksheet.Cells[5, 38].Value = "7/1/" + pastYear;
                worksheet.Cells[5, 39].Value = "8/1/" + pastYear;
                worksheet.Cells[5, 40].Value = "9/1/" + pastYear;
                worksheet.Cells[5, 41].Value = "10/1/" + pastYear;
                worksheet.Cells[5, 42].Value = "11/1/" + pastYear;
                worksheet.Cells[5, 43].Value = "12/1/" + pastYear;
                worksheet.Cells[5, 44].Value = "1/1/" + currentYear;
                worksheet.Cells[5, 45].Value = "2/1/" + currentYear;
                worksheet.Cells[5, 46].Value = "3/1/" + currentYear;
                worksheet.Cells[5, 47].Value = "TY FY22";

                worksheet.Cells[5, 49].Value = "4/1/" + pastYear;
                worksheet.Cells[5, 50].Value = "5/1/" + pastYear;
                worksheet.Cells[5, 51].Value = "6/1/" + pastYear;
                worksheet.Cells[5, 52].Value = "7/1/" + pastYear;
                worksheet.Cells[5, 53].Value = "8/1/" + pastYear;
                worksheet.Cells[5, 54].Value = "9/1/" + pastYear;
                worksheet.Cells[5, 55].Value = "10/1/" + pastYear;
                worksheet.Cells[5, 56].Value = "11/1/" + pastYear;
                worksheet.Cells[5, 57].Value = "12/1/" + pastYear;
                worksheet.Cells[5, 58].Value = "1/1/" + currentYear;
                worksheet.Cells[5, 59].Value = "2/1/" + currentYear;
                worksheet.Cells[5, 60].Value = "3/1/" + currentYear;
                worksheet.Cells[5, 61].Value = "TY FY22";

                worksheet.Cells[5, 63].Value = "4/1/" + pastYear;
                worksheet.Cells[5, 64].Value = "5/1/" + pastYear;
                worksheet.Cells[5, 65].Value = "6/1/" + pastYear;
                worksheet.Cells[5, 66].Value = "7/1/" + pastYear;
                worksheet.Cells[5, 67].Value = "8/1/" + pastYear;
                worksheet.Cells[5, 68].Value = "9/1/" + pastYear;
                worksheet.Cells[5, 69].Value = "10/1/" + pastYear;
                worksheet.Cells[5, 70].Value = "11/1/" + pastYear;
                worksheet.Cells[5, 71].Value = "12/1/" + pastYear;
                worksheet.Cells[5, 72].Value = "1/1/" + currentYear;
                worksheet.Cells[5, 73].Value = "2/1/" + currentYear;
                worksheet.Cells[5, 74].Value = "3/1/" + currentYear;
                worksheet.Cells[5, 75].Value = "TY FY22";

                int recordIndex = 6;
                int count = items.Count();

                foreach (var item in items)
                {
                    rowNumber++;
                    rowTCell++;
                    int diff = recordIndex - count;

                    if (diff > 0)
                    {
                        diff = 0;
                    }

                    worksheet.Cells[recordIndex, 1].Value = item.BudgetBase.Employee != null ? item.BudgetBase.Employee.Email : "";
                    worksheet.Cells[recordIndex, 2].Value = item.BudgetBase.Project != null ? item.BudgetBase.Project.Code : "";
                    worksheet.Cells[recordIndex, 3].Value = item.BudgetBase.Country != null ? item.BudgetBase.Country.Name : "" ;
                    worksheet.Cells[recordIndex, 4].Value = item.BudgetBase.Country != null ? item.BudgetBase.Country.Code : "" ;
                    worksheet.Cells[recordIndex, 5].Value = item.BudgetBase.Activity != null ? item.BudgetBase.Activity.Name : "";
                    worksheet.Cells[recordIndex, 6].Value = item.BudgetBase.Department != null ? item.BudgetBase.Department.Name : "";
                    worksheet.Cells[recordIndex, 7].Value = item.BudgetBase.Department != null ? item.BudgetBase.Department.Code : "";
                    worksheet.Cells[recordIndex, 8].Value = item.BudgetBase.AdmCenter != null ? item.BudgetBase.AdmCenter.Name : "";
                    worksheet.Cells[recordIndex, 9].Value = "";
                    worksheet.Cells[recordIndex, 10].Value = item.BudgetBase.Division != null ? item.BudgetBase.Division.Name : "";
                    worksheet.Cells[recordIndex, 11].Value = item.BudgetBase.Division != null ? item.BudgetBase.Division.Code : "";
                    worksheet.Cells[recordIndex, 12].Value = item.BudgetBase.ProjectType != null ? item.BudgetBase.ProjectType.Name : "";
                    worksheet.Cells[recordIndex, 13].Value = item.BudgetBase.ProjectType != null ? item.BudgetBase.ProjectType.Code : "";
                    worksheet.Cells[recordIndex, 14].Value = item.BudgetBase.Info;
                    worksheet.Cells[recordIndex, 15].Value = item.BudgetBase.AssetType != null ? item.BudgetBase.AssetType.Name : "";
                    worksheet.Cells[recordIndex, 16].Value = item.BudgetBase.AssetType != null ? item.BudgetBase.AssetType.Code : "";
                    worksheet.Cells[recordIndex, 17].Value = item.BudgetBase.AppState != null ? item.BudgetBase.AppState.Name : "";
                    //worksheet.Cells[recordIndex, 17].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[recordIndex, 18].Value = item.BudgetBase.StartMonth != null ? item.BudgetBase.StartMonth.Month : "";
                    //worksheet.Cells[recordIndex, 18].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[recordIndex, 19].Value = item.BudgetBase.DepPeriod;
                    //worksheet.Cells[recordIndex, 19].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[recordIndex, 20].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Max(a => a.Asset?.DepPeriod);
                    //worksheet.Cells[recordIndex, 20].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 21].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.April);
                    worksheet.Cells[recordIndex, 21].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 22].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.May);
                    worksheet.Cells[recordIndex, 22].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 23].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.June);
                    worksheet.Cells[recordIndex, 23].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 24].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.July);
                    worksheet.Cells[recordIndex, 24].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 25].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.August);
                    worksheet.Cells[recordIndex, 25].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 26].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.September);
                    worksheet.Cells[recordIndex, 26].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 27].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.Octomber);
                    worksheet.Cells[recordIndex, 27].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 28].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.November);
                    worksheet.Cells[recordIndex, 28].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 29].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.December);
                    worksheet.Cells[recordIndex, 29].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 30].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.January);
                    worksheet.Cells[recordIndex, 30].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 31].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.February);
                    worksheet.Cells[recordIndex, 31].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 32].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.March);
                    worksheet.Cells[recordIndex, 32].Style.Numberformat.Format = "#,##0.00";
                    //worksheet.Cells[recordIndex, 33].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast).Select(a => a.Total);
                    //worksheet.Cells[recordIndex, 33].Style.Numberformat.Format = "#,##0.00";

                    worksheet.Cells[recordIndex, 35].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.April);
                    worksheet.Cells[recordIndex, 35].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 36].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.May);
                    worksheet.Cells[recordIndex, 36].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 37].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.June);
                    worksheet.Cells[recordIndex, 37].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 38].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.July);
                    worksheet.Cells[recordIndex, 38].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 39].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.August);
                    worksheet.Cells[recordIndex, 39].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 40].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.September);
                    worksheet.Cells[recordIndex, 40].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 41].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.Octomber);
                    worksheet.Cells[recordIndex, 41].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 42].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.November);
                    worksheet.Cells[recordIndex, 42].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 43].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.December);
                    worksheet.Cells[recordIndex, 43].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 44].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.January);
                    worksheet.Cells[recordIndex, 44].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 45].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.February);
                    worksheet.Cells[recordIndex, 45].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 46].Value = item.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.March);
                    worksheet.Cells[recordIndex, 46].Style.Numberformat.Format = "#,##0.00";

                    worksheet.Cells[recordIndex, 49].Value = item.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.April);
                    worksheet.Cells[recordIndex, 49].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 50].Value = item.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.May);
                    worksheet.Cells[recordIndex, 50].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 51].Value = item.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.June);
                    worksheet.Cells[recordIndex, 51].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 52].Value = item.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.July);
                    worksheet.Cells[recordIndex, 52].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 53].Value = item.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.August);
                    worksheet.Cells[recordIndex, 53].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 54].Value = item.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.September);
                    worksheet.Cells[recordIndex, 54].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 55].Value = item.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.Octomber);
                    worksheet.Cells[recordIndex, 55].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 56].Value = item.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.November);
                    worksheet.Cells[recordIndex, 56].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 57].Value = item.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.December);
                    worksheet.Cells[recordIndex, 57].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 58].Value = item.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.January);
                    worksheet.Cells[recordIndex, 58].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 59].Value = item.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.February);
                    worksheet.Cells[recordIndex, 59].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 60].Value = item.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.March);
                    worksheet.Cells[recordIndex, 60].Style.Numberformat.Format = "#,##0.00";



                    worksheet.Cells[recordIndex, 63].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.April);
                    worksheet.Cells[recordIndex, 63].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 64].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.May);
                    worksheet.Cells[recordIndex, 64].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 65].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.June);
                    worksheet.Cells[recordIndex, 65].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 66].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.July);
                    worksheet.Cells[recordIndex, 66].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 67].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.August);
                    worksheet.Cells[recordIndex, 67].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 68].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.September);
                    worksheet.Cells[recordIndex, 68].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 69].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.Octomber);
                    worksheet.Cells[recordIndex, 69].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 70].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.November);
                    worksheet.Cells[recordIndex, 70].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 71].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.December);
                    worksheet.Cells[recordIndex, 71].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 72].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.January);
                    worksheet.Cells[recordIndex, 72].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 73].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.February);
                    worksheet.Cells[recordIndex, 73].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 74].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.March);
                    worksheet.Cells[recordIndex, 74].Style.Numberformat.Format = "#,##0.00";


                    worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Formula = "SUM(U" + rowTCell + ":AF" + rowTCell + ")";
                    worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Font.Bold = true;
                    worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

                    worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Formula = "SUM(AI" + rowTCell + ":AT" + rowTCell + ")";
                    worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Font.Bold = true;
                    worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

                    worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Formula = "SUM(AW" + rowTCell + ":BH" + rowTCell + ")";
                    worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Font.Bold = true;
                    worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

                    worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Formula = "SUM(BK" + rowTCell + ":BV" + rowTCell + ")";
                    worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Font.Bold = true;
                    worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

                    if (diff == 0)
                    {
                        for (int i = 1; i < 76; i++)
                        {
                            worksheet.Cells[5, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[5, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[5, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[5, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[5, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[5, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[5, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[5, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            worksheet.Cells[5, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        worksheet.Row(5).Height = 35.00;
                        worksheet.Row(5).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Row(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.View.FreezePanes(6, 1);

                        using (var cells = worksheet.Cells[5, 1, items.Count() + 5, 75])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        using (var cells = worksheet.Cells[3, 21, 3, 75])
                        {
                            cells.Style.Font.Bold = true;
                            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(198, 224, 180));
                            cells.Style.Font.Color.SetColor(Color.Black);
                        }

                        using (var cells = worksheet.Cells[4, 21, 4, 75])
                        {
                            cells.Style.Font.Bold = true;
                            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 204));
                            cells.Style.Font.Color.SetColor(Color.Black);
                        }

                        using (var cells = worksheet.Cells[5, 1, 5, 75])
                        {
                            cells.Style.Font.Bold = true;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
                            cells.Style.Font.Color.SetColor(Color.Black);
                        }

                        using (var cells = worksheet.Cells[6, 1, items.Count() + 3, 75])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                        }

                        using (var cells = worksheet.Cells[6, 1, items.Count() + 3, 75])
                        {
                            for (int i = 6; i < items.Count() + 2; i++)
                            {
                                worksheet.Row(i).Height = 15.00;
                                worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[$"A{i}"].Style.WrapText = true;
                                worksheet.Cells[$"B{i}"].Style.WrapText = true;
                                worksheet.Cells[$"C{i}"].Style.WrapText = true;
                                worksheet.Cells[$"D{i}"].Style.WrapText = true;
                                worksheet.Cells[$"E{i}"].Style.WrapText = true;
                                worksheet.Cells[$"F{i}"].Style.WrapText = true;
                                worksheet.Cells[$"G{i}"].Style.WrapText = true;
                                worksheet.Cells[$"H{i}"].Style.WrapText = true;
                                worksheet.Cells[$"I{i}"].Style.WrapText = true;
                                worksheet.Cells[$"J{i}"].Style.WrapText = true;
                                worksheet.Cells[$"K{i}"].Style.WrapText = true;
                                worksheet.Cells[$"L{i}"].Style.WrapText = true;
                                worksheet.Cells[$"M{i}"].Style.WrapText = true;


                            }



                        }


                        worksheet.View.ShowGridLines = false;
                        worksheet.View.ZoomScale = 100;

                        for (int i = 1; i < 76; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        worksheet.Column(1).Width = 30.00;
                        worksheet.Column(2).Width = 20.00;
                        worksheet.Column(3).Width = 15.00;
                        worksheet.Column(4).Width = 15.00;
                        worksheet.Column(5).Width = 20.00;
                        worksheet.Column(6).Width = 25.00;
                        worksheet.Column(7).Width = 21.00;
                        worksheet.Column(8).Width = 20.00;
                        worksheet.Column(9).Width = 0.00;
                        worksheet.Column(10).Width = 30.00;
                        worksheet.Column(11).Width = 20.00;
                        worksheet.Column(12).Width = 35.00;
                        worksheet.Column(13).Width = 15.00;
                        worksheet.Column(14).Width = 30.00;
                        worksheet.Column(15).Width = 30.00;
                        worksheet.Column(16).Width = 15.00;
                        worksheet.Column(17).Width = 15.00;
                        worksheet.Column(18).Width = 25.00;
                        worksheet.Column(19).Width = 20.00;
                        worksheet.Column(20).Width = 20.00;
                        worksheet.Column(21).Width = 15.00;
                        worksheet.Column(22).Width = 15.00;
                        worksheet.Column(23).Width = 15.00;
                        worksheet.Column(24).Width = 15.00;
                        worksheet.Column(25).Width = 15.00;
                        worksheet.Column(26).Width = 15.00;
                        worksheet.Column(27).Width = 15.00;
                        worksheet.Column(28).Width = 15.00;
                        worksheet.Column(29).Width = 15.00;
                        worksheet.Column(30).Width = 15.00;
                        worksheet.Column(31).Width = 15.00;
                        worksheet.Column(32).Width = 15.00;
                        worksheet.Column(33).Width = 15.00;
                        worksheet.Column(34).Width = 15.00;
                        worksheet.Column(35).Width = 15.00;
                        worksheet.Column(36).Width = 15.00;
                        worksheet.Column(37).Width = 15.00;
                        worksheet.Column(38).Width = 15.00;
                        worksheet.Column(39).Width = 15.00;
                        worksheet.Column(40).Width = 15.00;
                        worksheet.Column(41).Width = 15.00;
                        worksheet.Column(42).Width = 15.00;
                        worksheet.Column(43).Width = 15.00;
                        worksheet.Column(44).Width = 15.00;
                        worksheet.Column(45).Width = 15.00;
                        worksheet.Column(46).Width = 15.00;
                        worksheet.Column(47).Width = 15.00;
                        worksheet.Column(48).Width = 15.00;
                        worksheet.Column(49).Width = 15.00;
                        worksheet.Column(50).Width = 15.00;
                        worksheet.Column(51).Width = 15.00;
                        worksheet.Column(52).Width = 15.00;
                        worksheet.Column(53).Width = 15.00;
                        worksheet.Column(54).Width = 15.00;
                        worksheet.Column(55).Width = 15.00;
                        worksheet.Column(56).Width = 15.00;
                        worksheet.Column(57).Width = 15.00;
                        worksheet.Column(58).Width = 15.00;
                        worksheet.Column(59).Width = 15.00;
                        worksheet.Column(60).Width = 15.00;
                        worksheet.Column(61).Width = 15.00;
                        worksheet.Column(62).Width = 15.00;
                        worksheet.Column(63).Width = 15.00;
                        worksheet.Column(64).Width = 15.00;
                        worksheet.Column(65).Width = 15.00;
                        worksheet.Column(66).Width = 15.00;
                        worksheet.Column(67).Width = 15.00;
                        worksheet.Column(68).Width = 15.00;
                        worksheet.Column(69).Width = 15.00;
                        worksheet.Column(70).Width = 15.00;
                        worksheet.Column(71).Width = 15.00;
                        worksheet.Column(72).Width = 15.00;
                        worksheet.Column(73).Width = 15.00;
                        worksheet.Column(74).Width = 15.00;
                        worksheet.Column(75).Width = 15.00;

                        //worksheet.Cells["A" + rowCell + ":K" + rowCell].Merge = true;
                        //worksheet.Cells["A" + rowCell + ":K" + rowCell].Value = "TOTAL";
                        //worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        //worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.Font.Bold = true;
                        //worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        //worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));


                        worksheet.Cells["U" + rowCell + ":U" + rowCell].Formula = "SUM(U6:U" + rowTotal + ")";
                        worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["V" + rowCell + ":V" + rowCell].Formula = "SUM(V6:V" + rowTotal + ")";
						worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["W" + rowCell + ":W" + rowCell].Formula = "SUM(W6:W" + rowTotal + ")";
						worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["X" + rowCell + ":X" + rowCell].Formula = "SUM(X6:X" + rowTotal + ")";
						worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Formula = "SUM(Y6:Y" + rowTotal + ")";
						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Formula = "SUM(Z6:Z" + rowTotal + ")";
                        worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Formula = "SUM(AA6:AA" + rowTotal + ")";
                        worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Formula = "SUM(AB6:AB" + rowTotal + ")";
                        worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Formula = "SUM(AC6:AC" + rowTotal + ")";
                        worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Formula = "SUM(AD6:AD" + rowTotal + ")";
                        worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Formula = "SUM(AE6:AE" + rowTotal + ")";
                        worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Formula = "SUM(AF6:AF" + rowTotal + ")";
                        worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Formula = "SUM(AG6:AG" + rowTotal + ")";
                        worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49));

                        worksheet.Cells["U6" + ":U" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["V6" + ":V" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["W6" + ":W" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["X6" + ":X" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["Y6" + ":Y" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["Z6" + ":Z" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AA6" + ":AA" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AB6" + ":AB" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AC6" + ":AC" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AD6" + ":AD" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AE6" + ":AE" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AF6" + ":AF" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AG6" + ":AG" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(237, 125, 49));


                        // 2 //

                        worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Formula = "SUM(AI6:AI" + rowTotal + ")";
                        worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Formula = "SUM(AJ6:AJ" + rowTotal + ")";
                        worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Formula = "SUM(AK6:AK" + rowTotal + ")";
                        worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Formula = "SUM(AL6:AL" + rowTotal + ")";
                        worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Formula = "SUM(AM6:AM" + rowTotal + ")";
                        worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Formula = "SUM(AN6:AN" + rowTotal + ")";
                        worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Formula = "SUM(AO6:AO" + rowTotal + ")";
                        worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Formula = "SUM(AP6:AP" + rowTotal + ")";
                        worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Formula = "SUM(AQ6:AQ" + rowTotal + ")";
                        worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Formula = "SUM(AR6:AR" + rowTotal + ")";
                        worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Formula = "SUM(AS6:AS" + rowTotal + ")";
                        worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Formula = "SUM(AT6:AT" + rowTotal + ")";
                        worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Formula = "SUM(AU6:AU" + rowTotal + ")";
                        worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49));

                        worksheet.Cells["AI6" + ":AI" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AJ6" + ":AJ" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AK6" + ":AK" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AL6" + ":AL" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AM6" + ":AM" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AN6" + ":AN" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AO6" + ":AO" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AP6" + ":AP" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AQ6" + ":AQ" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AR6" + ":AR" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AS6" + ":AS" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AT6" + ":AT" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AU6" + ":AU" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(237, 125, 49));

                        // 2 //

                        // 3 //

                        worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Formula = "SUM(AW6:AW" + rowTotal + ")";
                        worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Formula = "SUM(AX6:AX" + rowTotal + ")";
                        worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Formula = "SUM(AY6:AY" + rowTotal + ")";
                        worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Formula = "SUM(AZ6:AZ" + rowTotal + ")";
                        worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Formula = "SUM(BA6:BA" + rowTotal + ")";
                        worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Formula = "SUM(BB6:BB" + rowTotal + ")";
                        worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Formula = "SUM(BC6:BC" + rowTotal + ")";
                        worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Formula = "SUM(BD6:BD" + rowTotal + ")";
                        worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Formula = "SUM(BE6:BE" + rowTotal + ")";
                        worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Formula = "SUM(BF6:BF" + rowTotal + ")";
                        worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Formula = "SUM(BG6:BG" + rowTotal + ")";
                        worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Formula = "SUM(BH6:BH" + rowTotal + ")";
                        worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Formula = "SUM(BI6:BI" + rowTotal + ")";
                        worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49));

                        worksheet.Cells["AW6" + ":AW" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AX6" + ":AX" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AY6" + ":AY" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["AZ6" + ":AZ" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BA6" + ":BA" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BB6" + ":BB" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BC6" + ":BC" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BD6" + ":BD" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BE6" + ":BE" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BF6" + ":BF" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BG6" + ":BG" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BH6" + ":BH" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BI6" + ":BI" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(237, 125, 49));

                        // 3 //

                        // 4 //

                        worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Formula = "SUM(BK6:BK" + rowTotal + ")";
                        worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Formula = "SUM(BL6:BL" + rowTotal + ")";
                        worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Formula = "SUM(BM6:BM" + rowTotal + ")";
                        worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Formula = "SUM(BN6:BN" + rowTotal + ")";
                        worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Formula = "SUM(BO6:BO" + rowTotal + ")";
                        worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Formula = "SUM(BP6:BP" + rowTotal + ")";
                        worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Formula = "SUM(BQ6:BQ" + rowTotal + ")";
                        worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Formula = "SUM(BR6:BR" + rowTotal + ")";
                        worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Formula = "SUM(BS6:BS" + rowTotal + ")";
                        worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Formula = "SUM(BT6:BT" + rowTotal + ")";
                        worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Formula = "SUM(BU6:BU" + rowTotal + ")";
                        worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Formula = "SUM(BV6:BV" + rowTotal + ")";
                        worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

                        worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Formula = "SUM(BW6:BW" + rowTotal + ")";
                        worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Font.Bold = true;
                        worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49));

                        worksheet.Cells["BK6" + ":BK" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BL6" + ":BL" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BM6" + ":BM" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BN6" + ":BN" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BO6" + ":BO" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BP6" + ":BP" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BQ6" + ":BQ" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BR6" + ":BR" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BS6" + ":BS" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BT6" + ":BT" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BU6" + ":BU" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BV6" + ":BV" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
                        worksheet.Cells["BW6" + ":BW" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(237, 125, 49));

                        // 4 //

                        //package.Workbook.CalcMode = ExcelCalcMode.Manual;

                        package.Workbook.Calculate();

                        worksheet.Cells["A5:AG5"].AutoFilter = true;

                    }
                    recordIndex++;
                }

                using (var cells = worksheet.Cells[5, 1, 5, 75])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Export.xlsx"
                };

                return result;

            }
        }

        [Authorize]
        [HttpPost("add")]
        public async virtual Task<RequestResult> PutDetail([FromBody] BudgetSave budget)
        {
            Model.RequestResult requestResult = null;
            var userName = HttpContext.User.Identity.Name;

            if (userName != null && userName != "")
            {
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }

                if (user != null)
                {
                    budget.UserId = user.Id;

                    int id = (_itemsRepository as IBudgetBasesRepository).CreateBudget(budget);

                    if(id > 0)
					{

                        return new Model.RequestResult { Success = true, Message = $"Bugetul a fost creat cu succes!" };
                    }
					else
					{
                        return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
                    }
                }
                else
                {
                    return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
                }

            }
            else
            {
                return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
            }
        }

        [Authorize]
        [HttpPost("addRequest")]
        public async virtual Task<RequestResult> AddRequest([FromBody] BudgetAddSave budget)
        {
            Model.RequestResult requestResult = null;
            var userName = HttpContext.User.Identity.Name;

            if (userName != null && userName != "")
            {
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }

                if (user != null)
                {
                    budget.UserId = user.Id;

                    var requestCheck = _context.Set<Model.Request>().Where(i => i.Id == budget.RequestId).SingleOrDefault();
                    if (requestCheck == null) return new RequestResult { Success = false, Message = "Nu a fost selectat un P.R.", RequestId = 0 };

                    var total = budget.AprilForecastNew +
                        budget.MayForecastNew +
                        budget.JuneForecastNew +
                        budget.JulyForecastNew +
                        budget.AugustForecastNew +
                        budget.SeptemberForecastNew +
                        budget.OctomberForecastNew +
                        budget.NovemberForecastNew +
                        budget.DecemberForecastNew +
                        budget.JanuaryForecastNew +
                        budget.FebruaryForecastNew +
                        budget.MarchForecastNew;

                    if (requestCheck.BudgetValueNeed < total)
                    {
                        return new Model.RequestResult { Success = false, Message = $"Valoarea bugetului adaugat depaseste valoarea solicitata!" };
                    }

                    var res = await (_itemsRepository as IBudgetBasesRepository).CreateRequestBudget(budget);

                    if (res != null && res.Success)
                    {
                        var bgtForecast = _context.Set<Model.BudgetForecast>().Where(a => a.Id == res.Id).Single();
                        var request = _context.Set<Model.Request>().Where(a => a.Id == budget.RequestId).SingleOrDefault();


                        if(request != null)
						{
                            var result = await this._requestsService.SendRequestResponseNeedBudget(request.Id, bgtForecast.BudgetBaseId);

                            if (result)
                            {
                                request.AppStateId = 33;
                                // request.BudgetBaseId = id;
                                request.ModifiedAt = DateTime.Now;
                                request.BudgetForecastId = res.Id;

                                _context.Update(request);
                                _context.SaveChanges();

                                return new Model.RequestResult { Success = true, Message = $"Bugetul a fost creat cu succes!" };
                            }
                            else
                            {
                                return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
                            }
                        }
                        else
                        {
                            return new Model.RequestResult { Success = false, Message = $"Ticketul nu exista!" };
                        }

                    }
                    else
                    {
                        return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
                    }
                }
                else
                {
                    return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
                }

            }
            else
            {
                return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
            }
        }


        //[Authorize]
        //[HttpPost("update")]
        //public async virtual Task<RequestResult> UpdateDetail([FromBody] BudgetSave budget)
        //{
        //    var userName = HttpContext.User.Identity.Name;

        //    if (userName != null && userName != "")
        //    {
        //        var user = await userManager.FindByEmailAsync(userName);
        //        if (user == null)
        //        {
        //            user = await userManager.FindByNameAsync(userName);
        //        }

        //        if (user != null)
        //        {
        //            budget.UserId = user.Id;

        //            var orderCheck = _context.Set<Model.Order>().Where(i => i.Id == budget.OrderId).SingleOrDefault();

        //            var total = budget.AprilForecastNew +
        //                budget.MayForecastNew +
        //                budget.JuneForecastNew +
        //                budget.JulyForecastNew +
        //                budget.AugustForecastNew +
        //                budget.SeptemberForecastNew +
        //                budget.OctomberForecastNew +
        //                budget.NovemberForecastNew +
        //                budget.DecemberForecastNew +
        //                budget.JanuaryForecastNew +
        //                budget.FebruaryForecastNew +
        //                budget.MarchForecastNew;

        //            //if (budget.ValueRem + orderCheck.BudgetValueNeed < total)
        //            //{
        //            //    return new Model.RequestResult { Success = false, Message = $"Valoarea bugetului adaugat depaseste valoarea solicitata!" };
        //            //}

        //            int id = (_itemsRepository as IBudgetBasesRepository).UpdateBudget(budget);

        //            if (id > 0 && budget.OrderId != null && budget.OrderId > 0)
        //            {
        //                var order = _context.Set<Model.Order>().Where(a => a.Id == budget.OrderId).SingleOrDefault();

        //                if (order != null)
        //                {

        //                    var result = await NeedBudgetResponse(order.Id);

        //                    if (result)
        //                    {
        //                        order.AppStateId = 12;
        //                        order.ModifiedAt = DateTime.Now;

        //                        _context.Update(order);
        //                        _context.SaveChanges();

        //                        return new Model.RequestResult { Success = true, Message = $"Bugetul a fost creat cu succes!" };
        //                    }
        //                    else
        //                    {
        //                        return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
        //                    }
        //                }
        //                else
        //                {
        //                    return new Model.RequestResult { Success = false, Message = $"Ticketul nu exista!" };
        //                }

        //            }
        //            else
        //            {
        //                return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
        //            }
        //        }
        //        else
        //        {
        //            return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
        //        }

        //    }
        //    else
        //    {
        //        return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
        //    }
        //}

        [Route("needbgtresponse")]
        public async Task<bool> NeedBudgetResponse(int orderId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            var files = new FormFileCollection();
            List<Model.OrderMaterial> orderMaterials = null;
            Model.AppState appState = null;
            Model.Order order = null;
            order = await _context.Set<Model.Order>()
                .Include(o => o.Offer).ThenInclude(r => r.Request)
                .Include(o => o.Offer).ThenInclude(r => r.AssetType)
                .Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
                .Include(o => o.Offer).ThenInclude(r => r.Region)
                .Include(c => c.Uom)
                 .Include(c => c.Partner)
                .Include(c => c.Company)
                .Include(c => c.CostCenter)
                .Include(c => c.BudgetBase).ThenInclude(e => e.Employee)
                .Include(c => c.BudgetBase).ThenInclude(e => e.Project)
                .Include(c => c.BudgetBase).ThenInclude(e => e.Activity)
                .Include(c => c.BudgetBase).ThenInclude(e => e.Division)
                .Include(c => c.BudgetBase).ThenInclude(e => e.Department)
                .Include(c => c.BudgetBase).ThenInclude(e => e.Project)
                .Include(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
                .Include(c => c.Employee)
                .Include(c => c.Project)
                .Include(c => c.OrderType).AsNoTracking().Where(a => a.Id == orderId).SingleOrDefaultAsync();
            var htmlMessage = "";
            var subject = "";
            var htmlBodyEmail1 = "";
            var htmlBodyEnd = "";
            var htmlBodyCompany1 = "";
            var htmlBodyCompany2 = "";
            var htmlBody1 = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    OPTIMA
                                                </title>
                                                <style type=""text/css"">
                                                    table.redTable {
                                                          border: 2px solid #04327d;
                                                          background-color: #FFFFFF;
                                                          width: 100%;
                                                          text-align: center;
                                                          border-collapse: collapse;
                                                        }
                                                        table.redTable td, table.redTable th {
                                                          border: 1px solid #04327d;
                                                          padding: 3px 2px;
                                                        }
                                                        table.redTable tbody td {
                                                          font-size: 13px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 16px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 13px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 13px;
                                                        }
                                                        table.redTable tfoot .links {
                                                          text-align: right;
                                                        }
                                                        table.redTable tfoot .links a{
                                                          display: inline-block;
                                                          background: #FFFFFF;
                                                          color: #04327d;
                                                          padding: 2px 8px;
                                                          border-radius: 5px;
                                                        }
													.button {
                                                              background-color: #04327d;
                                                              border: none;
                                                              color: #ffffff;
                                                              padding: 5px 10px;
                                                              text-align: center;
                                                              text-decoration: none;
                                                              display: inline-block;
                                                              font-size: 16px;
                                                              margin: 4px 2px;
                                                              cursor: pointer;
                                                            }
                                                   .button-no {
                                                              background-color: #6491D9;
                                                              border: none;
                                                              color: white;
                                                              padding: 5px 10px;
                                                              text-align: center;
                                                              text-decoration: none;
                                                              display: inline-block;
                                                              font-size: 16px;
                                                              margin: 4px 2px;
                                                              cursor: pointer;
                                                            }
                                                </style>
                                            </head>
                                            <body>
                                                <table class=""redTable"">
                                                    <thead>
                                                    
                                        ";
            var htmlHeader = @"        
                                    <tr style=""background-color: #04327d;font-color: #ffffff; font-weight: bold"">
                                        <th colspan=""11"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">CONFIRMARE INCARCARE SUPLIMENTARE BUGET</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Solicitant</th>
                                        <th colspan=""2"">Buget initial RON</th>
                                        <th colspan=""2"">Buget disponibil incarcat RON</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", order.CreatedAt)}</th>
										<th>{String.Format("{0:MM}", order.CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", order.CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Cod ticket</th>
									    <th style=""color: #ffffff;"">Tip comanda</th>
										<th style=""color: #ffffff;"">WIP</th>
										<th style=""color: #ffffff;"">Cod Produs</th>
										<th style=""color: #ffffff;"">Cantitate</th>
                                        <th style=""color: #ffffff;"">Moneda</th>
										<th style=""color: #ffffff;"">P.U. Valuta</th>
                                        <th style=""color: #ffffff;"">Total comanda Valuta</th>
                                        <th style=""color: #ffffff;"">P.U. RON</th>
                                        <th style=""color: #ffffff;"">Total comanda RON</th>
									</tr>";

            var htmlHeader3 = "";

            var htmlHeader4 = @"<tr>
										<th colspan = ""11""></th>
								</tr>";

            var htmlHeader5 = "";
            var htmlHeader6 = "";
            var htmlHeader7 = "";
            var htmlHeader8 = "";
            var htmlHeader9 = "";
            var htmlHeader10 = "";

            var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            orderMaterials = await _context.Set<Model.OrderMaterial>().Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request).Include(o => o.Order).ThenInclude(a => a.Uom).Include(a => a.Material).Include(o => o.Order).ThenInclude(a => a.OrderType).Where(a => a.IsDeleted == false && a.OrderId == order.Id).ToListAsync();

            int index = 0;

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/OFAuat";

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "NEED_BUDGET").SingleAsync();



            string empIni = order.Employee != null ? order.Employee.FirstName + " " + order.Employee.LastName : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{order.Employee.Email}</th>
                                                <th rowspan=""2"" colspan=""2"">{String.Format("{0:#,##0.##}", order.BudgetBase.ValueIni)}</th>
                                                <th rowspan=""2"" colspan=""2"">{String.Format("{0:#,##0.##}", order.BudgetValueNeed)}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""11"" style=""color: #ffffff;"">Detalii OFERTA</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Furnizor</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Companie</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Tip</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">PC</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">PC Det</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"">{order.Partner.Name}</th>
												<th>{order.Company.Code}</th>
												<th>{order.CostCenter.Code}</th>
												<th>{order.Offer.AssetType.Name}</th>
                                                <th>{order.Offer.AdmCenter.Code}</th>
                                                <th colspan=""2"">{order.Offer.Region.Code}</th>
												</tr>";

            htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""11"" style=""color: #ffffff;"">Detalii BUGET</th>
												</tr>";

            htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Cod OPTIMA</th>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">WBS</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Activity</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Buget total</th>
												<th rowspan=""2"" colspan=""2"">
												</th>
												</tr>";

            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th >{order.BudgetBase.Code}</th>
												<th colspan=""2"">{order.BudgetBase.Employee.Email}</th>
												<th>{order.BudgetBase.Project.Code}</th>
                                                <th>{order.BudgetBase.Activity.Name}</th>
												<th>{order.BudgetBase.Info}</th>
                                                <th>{String.Format("{0:#,##0.##}", order.BudgetBase.ValueIni)}</th>
												</tr>";


            subject = "Suplimentare buget pentru comanda: " + order.Code + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{order.Code}</th>";



            for (int i = 0; i < orderMaterials.Count; i++)
            {
                index++;
                var wip = orderMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th>{orderMaterials[i].Order.OrderType.Name}</th>
									<th>{wip}</th>
									<th>{orderMaterials[i].Material.Code}</th>
                                    <th>{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th>{orderMaterials[i].Order.Uom.Code}</th>
									<th>{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th>{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th>{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th>{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
                if (index == orderMaterials.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""4""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
								</tr>";
                }
            };

            //    htmlHeader3 = htmlHeader3 + $@"      
            //                        <tr>
            //                            <th>{index}</th>
            //	<th colspan=""2"">{order.OrderType.Name}</th>
            //	<th>{order.Offer.Code}</th>
            //	<th>{order.Offer.Request.Code}</th>
            //                            <th colspan=""2"">{order.Uom.Code}</th>
            //	<th>{String.Format("{0:#,##0.##}", order.Price)}</th>
            //</tr>";


            emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == order.EmployeeId).Select(a => a.Guid).ToList();

            if (emails1.Count > 0)
            {
                emails.Add(emails1.ElementAt(0));
            }

            for (int e = 0; e < emails.Count; e++)
            {
                var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

                if (emp.Email != null && emp.Email != "")
                {
                    to.Add(emp.Email);
                }
                else
                {
                    to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
                }
            }

            to = new List<string>();

            to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
            //to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
            //to.Add("silvia.damian@emag.ro");

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
