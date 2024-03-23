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
    [Route("api/bugets")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class BudgetsController : GenericApiController<Model.Budget, Dto.Budget>
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<Model.ApplicationUser> userManager;

        private readonly IEmailSender _emailSender;

        public BudgetsController(ApplicationDbContext context,
            IBudgetsRepository itemsRepository, 
            //IRepository<Model.Budget> itemsRepository,
            IMapper mapper, IWebHostEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager, IEmailSender emailSender)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            this._emailSender = emailSender;
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

			includes ??= "Budget.AssetType,Budget.ProjectType,Budget.Region,Budget.AdmCenter,Budget.Activity,Budget.Country,Budget.Company,Budget.Employee,Budget.AccMonth,Budget.AppState,";
			//includes = "Company,Project,Administration,CostCenter,SubType.Type.MasterType,SubType.Type,SubType,Employee,AccMonth,InterCompany,Partner,Account,AppState";

			if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
				|| (page <= 0) || (pageSize <= 0))
				return BadRequest();

			paging = new Paging() { Page = page, PageSize = pageSize };
			sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();


			includes = includes + "Budget.BudgetMonths,Budget.Project";

            //var items1 = await (_itemsRepository as IBudgetsRepository).GetAllByProjectIncludingBudgetMonthsAsync();

            //sortColumn = "id";
            //includes = "BudgetMonths";
            //includes = null;

            // sorting.Column = "id";

            //var items = await (_itemsRepository as IBudgetsRepository)
            //var items = await _itemsRepository
            //    .GetAsync(null, includes, sortColumn, sortDirection, page, pageSize);
            //var itemsResource = _mapper.Map<List<Model.Budget>, List<Dto.Budget>>(items.ToList());

            var items = (_itemsRepository as IBudgetsRepository)
               .GetBuget(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();


			//var Grp = items.GroupBy(item => item.Budget.ProjectId)
			//          .Select(group => new BudgetDetail()
			//          {
			//              Budget = items.First().Budget,
			//               // = group.Key,
			//              //Orders = group.ToList()
			//          })
			//          .ToList();


			//      var resultf = items
			//.Select(o => o)
			//.GroupBy(o => new
			//{
			//	UserId = o.Budget.ProjectId
			//},
			//(key, items) => new BudgetDetail()
			//                  {
			//	Budget = items.Select(p => new BudgetDetail() { p }).ToList()
			//}).
			//ToList();

			//var query =
			//            items
			//                .SelectMany(p =>
			//                    (p.Budget ?? new Model.Budget[] { })
			//                        .DefaultIfEmpty(new Model.Budget() { ProjectId = p.Budget.ProjectId }),
			//                    (p, c) => new { p, c })
			//                .GroupBy(x => x.c.Value, x => x.p);

			//items = items
	  //             .Select(o => o)
	  //             .GroupBy(a => a.Budget.ProjectId).Select(a => new BudgetDetail()
	  //             {
		 //              Budget = new Model.Budget()
		 //              {
			//               Id = a.First().Budget.Id,
			//               Code = a.First().Budget.Code,
			//               Name = a.First().Budget.Name,
			//               Company = new Model.Company() { Id = a.First().Budget.Company.Id, Code = a.First().Budget.Company.Code, Name = a.First().Budget.Company.Name },
			//               BudgetMonths = new List<Model.BudgetMonth>().Add(new Model.BudgetMonth() { })

		 //              },

	  //             }).ToList();

			var itemsResource = _mapper.Map<List<Model.BudgetDetail>, List<Dto.Budget>>(items);


			var result = new BudgetPagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal, catTotal);

			return Ok(result);
		}

		[HttpGet]
        [Route("tree", Order = -1)]
        public virtual async Task<IActionResult> GetDepTreeDetails(int page, int pageSize, string sortColumn, string sortDirection,
           string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            BudgetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            includes ??= "Budget.AssetType,Budget.ProjectType,Budget.Region,Budget.AdmCenter,Budget.Activity,Budget.Country,Budget.Company,Budget.Employee,Budget.AccMonth,Budget.AppState,";
            //includes = "Company,Project,Administration,CostCenter,SubType.Type.MasterType,SubType.Type,SubType,Employee,AccMonth,InterCompany,Partner,Account,AppState";

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();


            includes = includes + "Budget.BudgetMonths,Budget.Project";

            var items = (_itemsRepository as IBudgetsRepository)
               .GetBuget(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();

			var itemsResource = _mapper.Map<List<Model.BudgetDetail>, List<Dto.BudgetTree>>(items);




			var result = new BudgetPagedResultTree(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			});

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

            var count = _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgets").ToList();

            includes = includes + ",Budget.Company,Budget.Project,Budget.AdmCenter,Budget.CostCenter,Budget.Region,Budget.AssetType,Budget.AppState,Budget.Uom";
            // includes ??= "Budget.Company,Budget.Project,Budget.AdmCenter,Budget.CostCenter,Budget.Region,Budget.AssetType,Budget.Employee,Budget.AccMonth,Budget.InterCompany,Budget.Partner,Budget.Account,Budget.AppState,";

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

            var items = (_itemsRepository as IBudgetsRepository)
                .GetBugetUI(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.BudgetDetail>, List<Dto.BudgetUI>>(items);

            var result = new BudgetUIPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }
        // [Authorize]
        [HttpPost("detail")]
        public async Task<IActionResult> PostDetail([FromBody] BudgetSave budget)
        {
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

                    int id = (_itemsRepository as IBudgetsRepository).CreateOrUpdateBudget(budget);

                    if (id > 0)
					{
                        SendEmail(id);
                    }

                    

                    return Ok(id);
                }
                else
                {
                    return BadRequest();
                }


            }
			else
			{
                return BadRequest();
			}

           
        }
        // [Authorize]
        [HttpPut("detail")]
        public async virtual Task<IActionResult> PutDetail([FromBody] BudgetSave budget)
        {
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

                    int id = (_itemsRepository as IBudgetsRepository).CreateOrUpdateBudget(budget);
                    return Ok(id);
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                return BadRequest();
            }

            
        }
        // [Authorize]
        [HttpGet("detail/{id:int}")]
        public virtual IActionResult GetDetail(int id, string includes)
        {
            var asset = (_itemsRepository as IBudgetsRepository).GetDetailsById(id, includes);
            var result = _mapper.Map<Dto.Budget>(asset);

            return Ok(result);
        }


        [HttpGet("export")]
        public IActionResult Export(int page, int pageSize, string sortColumn, string sortDirection,
        string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            BudgetFilter assetFilter = null;
            string userName = string.Empty;
            string userId = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            int rowNumber = 0;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();


            includes = @"Budget.Company,Budget.Project,Budget.Administration,Budget.CostCenter,Budget.SubType.Type.MasterType,Budget.SubType.Type,Budget.SubType,Budget.Employee,Budget.AccMonth,Budget.InterCompany,Budget.Partner,Budget.Account,Budget.AppState,";

            List<Model.BudgetDetail> items = (_itemsRepository as IBudgetsRepository)
               .GetBuget(assetFilter, includes, null, null, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.BudgetDetail>, List<Dto.Budget>>(items);



            using (ExcelPackage package = new ExcelPackage())
            {

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Export");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Nr. crt";
                worksheet.Cells[1, 2].Value = "Company";
                worksheet.Cells[1, 3].Value = "Code";
                worksheet.Cells[1, 4].Value = "Project";
                worksheet.Cells[1, 5].Value = "Project";
                worksheet.Cells[1, 6].Value = "Activity";
                worksheet.Cells[1, 7].Value = "PC";
                worksheet.Cells[1, 8].Value = "Expense Type";
                worksheet.Cells[1, 9].Value = "Details";
                worksheet.Cells[1, 10].Value = "Owner FirstName";
                worksheet.Cells[1, 11].Value = "Owner LastName";
                worksheet.Cells[1, 12].Value = "FY";
                worksheet.Cells[1, 13].Value = "Value Ini";
                worksheet.Cells[1, 14].Value = "Value Rem";
                worksheet.Cells[1, 15].Value = "Tot. life in periods";
                worksheet.Cells[1, 16].Value = "Supplier";
                worksheet.Cells[1, 17].Value = "GL Account";
                worksheet.Cells[1, 18].Value = "CC";
                worksheet.Cells[1, 19].Value = "Comment";
                worksheet.Cells[1, 20].Value = "Status";
                worksheet.Cells[1, 21].Value = "Motiv";



                int recordIndex = 2;
                foreach (var item in itemsResource)
                {
                    rowNumber++;
                    worksheet.Cells[recordIndex, 1].Value = rowNumber;
                    worksheet.Cells[recordIndex, 2].Value = item.Company != null ? item.Company.Name : "";
                    worksheet.Cells[recordIndex, 3].Value = item.Code;
                    worksheet.Cells[recordIndex, 4].Value = item.Project != null ? item.Project.Code : "";
                    worksheet.Cells[recordIndex, 5].Value = item.Project != null ? item.Project.Name : "";
                    //worksheet.Cells[recordIndex, 6].Value = item.Administration != null ? item.Administration.Name : "";
                    //worksheet.Cells[recordIndex, 7].Value = item.MasterType != null ? item.MasterType.Name : "";
                    //worksheet.Cells[recordIndex, 8].Value = item.Type != null ? item.Type.Name : "";
                    //worksheet.Cells[recordIndex, 9].Value = item.SubType != null ? item.SubType.Name : "";
                    worksheet.Cells[recordIndex, 10].Value = item.Employee != null ? item.Employee.FirstName : "";
                    worksheet.Cells[recordIndex, 11].Value = item.Employee != null ? item.Employee.LastName : "";
                    worksheet.Cells[recordIndex, 12].Value = item.AccMonth != null ? item.AccMonth.Year : "";
                    worksheet.Cells[recordIndex, 13].Value = item.ValueIni;
                    worksheet.Cells[recordIndex, 14].Value = item.ValueFin;
                    //worksheet.Cells[recordIndex, 15].Value = item.InterCompany != null ? item.InterCompany.Name : "";
                    //worksheet.Cells[recordIndex, 16].Value = item.Partner != null ? item.Partner.Name : "";
                    //worksheet.Cells[recordIndex, 17].Value = item.Account != null ? item.Account.Name : "";
                    //worksheet.Cells[recordIndex, 18].Value = item.CostCenter != null ? item.CostCenter.Name : "";
                    worksheet.Cells[recordIndex, 19].Value = item.Info;
                    worksheet.Cells[recordIndex, 20].Value = item.AppState != null ? item.AppState.Name : "";
                    worksheet.Cells[recordIndex, 21].Value = item.Name;




                    recordIndex++;
                }






                //  worksheet.Column(3).Style.Numberformat.Format = "MMMM";
                //   worksheet.Column(12).Style.Numberformat.Format = "yyyy-mm-dd";





                using (var cells = worksheet.Cells[1, 1, 1, 43])
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


        [HttpPost("sendEmail")]
        // [Authorize]
        public async Task<IActionResult> SendEmail(int budgetId)
        {

            if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
            var item = (_itemsRepository as IBudgetsRepository).SendEmail(budgetId, _context.UserName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut);

            List<string> ccOut = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();

            ccOut.Add("adrian.cirnaru@optima.ro");
            // ccOut.Add("isabela.hurduzea@optima.ro");
            // ccOut.Add("adrian.daniu@emag.ro");
            // ccOut.Add(emailIniOut);
            //ccOut.Add(emailCCOut);

            var messageAttach = new Message(ccOut, cc, bcc, subjectOut, bodyHtmlOut, null);


            var success = await _emailSender.SendEmailAsync(messageAttach);

            if (!success)
            {
                //foreach (var val in guidIds)
                //{
                //    Model.EmailManager eManager = _context.Set<Model.EmailManager>().Where(e => e.Guid == val).SingleOrDefault();

                //    if (eManager != null)
                //    {
                //        eManager.IsDeleted = true;
                //        _context.Update(eManager);
                //        _context.SaveChanges();
                //    }
                //}
            }

            return Ok(budgetId);
        }

        [HttpPost("sendEmail")]
        // [Authorize]
        public async Task<IActionResult> SendValidatedEmail(int budgetId)
        {

            if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
            var id = (_itemsRepository as IBudgetsRepository).SendValidatedEmail(budgetId, _context.UserName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut);

            List<string> ccOut = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            var success = false;
            ccOut.Add("adrian.cirnaru@optima.ro");

            var messageAttach = new Message(ccOut, cc, bcc, subjectOut, bodyHtmlOut, null);

            if (id > 0)
			{
                success = await _emailSender.SendEmailAsync(messageAttach);
            }
 
   
            //if (!success)
            //{
            //    //foreach (var val in guidIds)
            //    //{
            //    //    Model.EmailManager eManager = _context.Set<Model.EmailManager>().Where(e => e.Guid == val).SingleOrDefault();

            //    //    if (eManager != null)
            //    //    {
            //    //        eManager.IsDeleted = true;
            //    //        _context.Update(eManager);
            //    //        _context.SaveChanges();
            //    //    }
            //    //}
            //}

            return Ok(budgetId);
        }

        [HttpGet]
        [Route("budgetvalidate")]
        [AllowAnonymous]
        public virtual IActionResult BudgetValidate(int page, int pageSize, string sortColumn, string sortDirection,
            string includes, string jsonFilter, string userId)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            BudgetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();

            var items = (_itemsRepository as IBudgetsRepository)
                .BudgetValidate(assetFilter, includes, userId, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.BudgetDetail>, List<Dto.Budget>>(items);

            var result = new BudgetPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }

        
        [HttpPost]
        [AllowAnonymous]
        [Route("budgetvalidate")]
        public async Task<IActionResult> BudgetValidate([FromBody] Dto.BudgetValidate[] employee)
        {
			await Task.Delay(0);
            Model.Budget budget = null;
            Model.BudgetOp budgetOp = null;
            Model.Document document = null;

            string documentTypeCode = "IS_MINUS";



            var documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();
            

            document = new Model.Document
            {
                Approved = true,
                DocumentType = documentType,
                DocNo1 = string.Empty,
                DocNo2 = string.Empty,
                DocumentDate = DateTime.Now,
                RegisterDate = DateTime.Now,
                Partner = null
            };

            _context.Add(document);

            for (int i = 0; i < employee.Length; i++)
            {
                budget = _context.Set<Model.Budget>().Where(a => a.Id == employee[i].BudgetId).SingleOrDefault();
                budget.IsAccepted = employee[i].Accepted;
                budget.AppStateId = employee[i].Accepted ? 7 : employee[i].Reason != null && employee[i].Reason.Trim().Length > 1 ? 8 : 1;
                budget.Name =employee[i].Reason != null && employee[i].Reason.Trim().Length > 1 && employee[i].Accepted == false ? employee[i].Reason : "";
                //int employeeId = _context.Set<Model.Employee>().Include(d => d.Division).Where(d => d.Id == budget.EmployeeId).AsNoTracking().Select(e => e.Id).SingleOrDefault();
                //string userId = _context.Set<Model.ApplicationUser>().Where(d => d.EmployeeId == employeeId).AsNoTracking().Select(e => e.Id).SingleOrDefault();


                budgetOp = new Model.BudgetOp()
                {
                    BudgetId = budget.Id,
                    
                    AccMonthId = budget.AccMonthId,
                    AdministrationIdFinal = budget.AdministrationId,
                    AdministrationIdInitial = budget.AdministrationId,
                    BudgetManagerIdFinal = budget.BudgetManagerId,
                    BudgetManagerIdInitial = budget.BudgetManagerId,
                    BudgetStateId = budget.AppStateId,
                    CompanyIdFinal = budget.CompanyId,
                    CompanyIdInitial = budget.CompanyId,
                    CostCenterIdFinal = budget.CostCenterId,
                    CostCenterIdInitial = budget.CostCenterId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = null,
                    DocumentId = document.Id,
                    DstConfAt = DateTime.UtcNow,
                    DstConfBy = null,
                    EmployeeIdFinal = budget.EmployeeId,
                    EmployeeIdInitial = budget.EmployeeId,
                    InfoIni = budget.Info,
                    InfoFin = employee[0].Reason,
                    IsAccepted = employee[0].Accepted,
                    IsDeleted = false,
                    ModifiedAt = DateTime.UtcNow,
                    ModifiedBy = null,
                    PartnerIdFinal = budget.PartnerId,
                    PartnerIdInitial = budget.PartnerId,
                    ProjectIdFinal = budget.ProjectId,
                    ProjectIdInitial = budget.ProjectId,
                    QuantityIni = budget.Quantity,
                    QuantityFin = budget.Quantity,
                    SubTypeIdFinal = budget.SubTypeId,
                    SubTypeIdInitial = budget.SubTypeId,
                    Validated = budget.Validated,
                    ValueFin1 = budget.ValueFin,
                    ValueFin2 = budget.ValueFin,
                    ValueIni1 = budget.ValueIni,
                    ValueIni2 = budget.ValueIni,
                    Guid = employee[0].Guid,
                   
                     
                };

                _context.Add(budgetOp);

                _context.SaveChanges();
            }

            //await SendValidatedEmail(budget.Id);

            return Ok(StatusCode(200));
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("total/{accMonthId}")]
        public virtual IActionResult GetPieChartDetails(int accMonthId)
        {
            List<Model.BudgetTotalProcentage> items = _context.Set<Model.BudgetTotalProcentage>().FromSql("BudgetTotal {0}", accMonthId).ToList();

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("auditBudgetChart/{accMonthId}")]
        public virtual IActionResult GetAuditLocation(int accMonthId)
        {
            List<Model.Audit> items = _context.Set<Model.Audit>().FromSql("AuditBudget {0}", accMonthId).ToList();

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("company/{accMonthId}/{companyId}")]
        public virtual IActionResult GetRegionDetails(int accMonthId, int companyId)
        {
            List<Model.BudgetCompanyProcentage> items = _context.Set<Model.BudgetCompanyProcentage>().FromSql("BudgetReportByCompany {0}, {1}", accMonthId > 0 ? accMonthId : 29, companyId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetCompanyProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("project/{accMonthId}/{projectId}")]
        public virtual IActionResult GetProjectDetails(int accMonthId, int projectId)
        {
            List<Model.BudgetProjectProcentage> items = _context.Set<Model.BudgetProjectProcentage>().FromSql("BudgetReportByProject {0}, {1}", accMonthId > 0 ? accMonthId : 29, projectId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetProjectProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("costcenter/{accMonthId}/{costCenterId}")]
        public virtual IActionResult GetCostCenterDetails(int accMonthId, int costCenterId)
        {
            List<Model.BudgetCostCenterProcentage> items = _context.Set<Model.BudgetCostCenterProcentage>().FromSql("BudgetReportByCostCenter {0}, {1}", accMonthId > 0 ? accMonthId : 29, costCenterId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetCostCenterProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("expencetype/{accMonthId}/{typeId}")]
        public virtual IActionResult GetExpenceTypeDetails(int accMonthId, int typeId)
        {
            List<Model.BudgetExpenceTypeProcentage> items = _context.Set<Model.BudgetExpenceTypeProcentage>().FromSql("BudgetReportByExpenceType {0}, {1}", accMonthId > 0 ? accMonthId : 29, typeId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetExpenceTypeProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("employee/{accMonthId}/{employeeId}")]
        public virtual IActionResult GetEmployeeDetails(int accMonthId, int employeeId)
        {
            List<Model.BudgetEmployeeProcentage> items = _context.Set<Model.BudgetEmployeeProcentage>().FromSql("BudgetReportByEmployee {0}, {1}", accMonthId > 0 ? accMonthId : 29, employeeId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetEmployeeProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("subtype/{accMonthId}/{subTypeId}")]
        public virtual IActionResult GetSubTypeDetails(int accMonthId, int subTypeId)
        {
            List<Model.BudgetSubTypeProcentage> items = _context.Set<Model.BudgetSubTypeProcentage>().FromSql("BudgetReportBySubType {0}, {1}", accMonthId > 0 ? accMonthId : 29, subTypeId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetSubTypeProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        [HttpGet("template")]
        public IActionResult ExportIT()
        {
            int i = 2;
            using (ExcelPackage package = new ExcelPackage())
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("model-import-buget");
                //First add the headers
                worksheet.Cells[1, 1].Value = "BudgetCode".ToUpper();
                worksheet.Cells[1, 2].Value = "Company".ToUpper();
                worksheet.Cells[1, 3].Value = "Country".ToUpper();
                worksheet.Cells[1, 4].Value = "Project ID (WBS)".ToUpper();
                worksheet.Cells[1, 5].Value = "Activity".ToUpper();
                worksheet.Cells[1, 6].Value = "PC".ToUpper();
                worksheet.Cells[1, 7].Value = "PC DET".ToUpper();
                worksheet.Cells[1, 8].Value = "Type".ToUpper();
                worksheet.Cells[1, 9].Value = "Project".ToUpper();
                worksheet.Cells[1, 10].Value = "Details".ToUpper();
                worksheet.Cells[1, 11].Value = "ACQ".ToUpper();
                worksheet.Cells[1, 12].Value = "Dep Per".ToUpper();
                worksheet.Cells[1, 13].Value = "Dep Per Rem".ToUpper();
                worksheet.Cells[1, 14].Value = "Value Rem".ToUpper();

                worksheet.Cells[1, 15].Value = "Month 1".ToUpper();
                worksheet.Cells[1, 16].Value = "Month 2".ToUpper();
                worksheet.Cells[1, 17].Value = "Month 3".ToUpper();
                worksheet.Cells[1, 18].Value = "Month 4".ToUpper();
                worksheet.Cells[1, 19].Value = "Month 5".ToUpper();
                worksheet.Cells[1, 20].Value = "Month 6".ToUpper();
                worksheet.Cells[1, 21].Value = "Month 7".ToUpper();
                worksheet.Cells[1, 22].Value = "Month 8".ToUpper();
                worksheet.Cells[1, 23].Value = "Month 9".ToUpper();
                worksheet.Cells[1, 24].Value = "Month 10".ToUpper();
                worksheet.Cells[1, 25].Value = "Month 11".ToUpper();
                worksheet.Cells[1, 26].Value = "Month 12".ToUpper();
                worksheet.Cells[1, 27].Value = "Month START".ToUpper();

                worksheet.Cells[2, 1].Value = "";
                worksheet.Cells[2, 2].Value = "Dante";
                worksheet.Cells[2, 3].Value = "RO";
                worksheet.Cells[2, 4].Value = "10IM_DC01_13_05";
                worksheet.Cells[2, 5].Value = "Warehouse";
                worksheet.Cells[2, 6].Value = "PC_WH";
                worksheet.Cells[2, 7].Value = "DC01";
                worksheet.Cells[2, 8].Value = "WH equipment - WIP";
                worksheet.Cells[2, 9].Value = "Echipamente WH";
                worksheet.Cells[2, 10].Value = "Sorter DC1";
                worksheet.Cells[2, 11].Value = "WIP FY21";
                worksheet.Cells[2, 12].Value = "144";
                worksheet.Cells[2, 13].Value = "36";
                worksheet.Cells[2, 14].Value = "100000";

                worksheet.Cells[2, 15].Value = "0";
                worksheet.Cells[2, 16].Value = "5000";
                worksheet.Cells[2, 17].Value = "0";
                worksheet.Cells[2, 18].Value = "1000";
                worksheet.Cells[2, 19].Value = "0";
                worksheet.Cells[2, 20].Value = "0";
                worksheet.Cells[2, 21].Value = "0";
                worksheet.Cells[2, 22].Value = "6000";
                worksheet.Cells[2, 23].Value = "0";
                worksheet.Cells[2, 24].Value = "0";
                worksheet.Cells[2, 25].Value = "0";
                worksheet.Cells[2, 26].Value = "9000";

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
                worksheet.Column(20).AutoFit();
                worksheet.Column(21).AutoFit();
                worksheet.Column(22).AutoFit();
                worksheet.Column(23).AutoFit();
                worksheet.Column(24).AutoFit();
                worksheet.Column(25).AutoFit();
                worksheet.Column(26).AutoFit();

                //worksheet.Column(1).Width = 100;
                //worksheet.Column(2).Width = 75;

                worksheet.Cells["A1:Z2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:Z2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["A1:Z2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:Z2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:Z2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:Z2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                // worksheet.Protection.IsProtected = true;
                //worksheet.Cells["A1:A3"].Style.Locked = false;

                using (var cells = worksheet.Cells[1, 1, 1, 26])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.MediumAquamarine);
                }

                for (int a = 1000 - i; i < 1000; i++)
                {
                    //Unlock non-Id fields
                    worksheet.Cells["A" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["B" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["C" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["D" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["E" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["F" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["G" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["H" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["I" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["J" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["K" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["L" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["M" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["N" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["O" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["P" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["Q" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["R" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["S" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["T" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["U" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["V" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["X" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["Y" + i.ToString()].Style.Locked = false;
                    worksheet.Cells["Z" + i.ToString()].Style.Locked = false;
                }

                //Set worksheet protection attributes
                //worksheet.Protection.AllowInsertRows = true;
                //worksheet.Protection.AllowSort = true;
                //worksheet.Protection.AllowSelectUnlockedCells = true;
                //worksheet.Protection.AllowAutoFilter = true;
                //worksheet.Protection.AllowInsertRows = true;
                //worksheet.Protection.IsProtected = true;

                var range = ExcelRange.GetAddress(2, 27, ExcelPackage.MaxRows, 27);
                var validationCell = worksheet.DataValidations.AddListValidation(range);
                
                validationCell.Formula.Values.Add("Aprilie");
                validationCell.Formula.Values.Add("Mai");
                validationCell.Formula.Values.Add("Iunie");
                validationCell.Formula.Values.Add("Iulie");
                validationCell.Formula.Values.Add("August");
                validationCell.Formula.Values.Add("Septembrie");
                validationCell.Formula.Values.Add("Octombrie");
                validationCell.Formula.Values.Add("Noiembrie");
                validationCell.Formula.Values.Add("Decembrie");
                validationCell.Formula.Values.Add("Ianuarie");
                validationCell.Formula.Values.Add("Februarie");
                validationCell.Formula.Values.Add("Martie");




                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "model-import-buget.xlsx"
                };

                return result;

            }
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
        public virtual IActionResult Import([FromBody] BudgetBaseImport assetImport)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string userName = HttpContext.User.Identity.Name;

                var user = _context.Users.Include(r => r.Claims).Where(u => u.Id == "92E74C4F-A79A-4C83-A7D0-A3202BD2507F").SingleOrDefault();
                assetImport.UserId = user.Id;
                //if (userName != null)
                //{


                //}
            }

            int budgetId = 0;
            budgetId = (_itemsRepository as IBudgetsRepository).BudgetBaseImport(assetImport);

            //var budget = _context.Set<Model.BudgetBase>().Include(b => b.BudgetMonthBase).Include(b => b.BudgetForecast).Where(b => b.Id == budgetId).Single();

            ////budget.ValueIni = budget.BudgetMonthBase.Sum(a => a.t);
            ////budget.ValueFin = budget.BudgetMonths.Sum(a => a.Value);

            //_context.SaveChanges();

            return Ok(budgetId);
        }


    }
}
