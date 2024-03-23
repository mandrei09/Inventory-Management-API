using AutoMapper;
using IdentityModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Utils;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static IdentityServer4.Models.IdentityResources;
using Employee = Optima.Fais.Model.Employee;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/employees")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class EmployeesController : GenericApiController<Model.Employee, Dto.Employee>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public  IEmailSender _emailSender { get; }

        public EmployeesController(ApplicationDbContext context, IEmployeesRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager, IEmailSender emailSender)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        [Route("details")]
        public virtual IActionResult GetDetails(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, int? departmentId)
        {
            int totalItems = 0;

            List<Dto.EmployeeDetail> items = (_itemsRepository as IEmployeesRepository).GetDetailsByFilters(departmentId, filter, null, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var result = new PagedResult<Dto.EmployeeDetail>(items, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });

            return Ok(result);
        }

        [HttpGet("detail/{id:int}")]
        public virtual async Task<IActionResult> GetDetail(int id, string includes)
        {
            var employee = (_itemsRepository as IEmployeesRepository).GetDetailsById(id, includes);
            var result = _mapper.Map<Dto.Employee>(employee);

            return Ok(result);
        }

        [HttpPost("updateEmployee")]
        public async Task<IActionResult> UpdateEmployee([FromBody] Employee employee)
        {
            
            Model.Employee modelEmployee = _context.Set<Model.Employee>().Where(e => e.Id == employee.Id).Single();

            modelEmployee.FirstName = employee.FirstName;
            modelEmployee.LastName = employee.LastName;
            modelEmployee.InternalCode = employee.InternalCode;
            modelEmployee.Email = employee.Email;
            modelEmployee.Department = employee.Department;
            modelEmployee.ERPCode = employee.ERPCode;
            modelEmployee.Division = employee.Division;
            modelEmployee.NotifyLast = employee.NotifyLast;
            modelEmployee.IsConfirmed = employee.IsConfirmed;
            modelEmployee.IsEmailSend = employee.IsEmailSend;
            modelEmployee.ERPId = employee.ERPId;
            modelEmployee.Guid = employee.Guid;
            modelEmployee.AssetCount = employee.AssetCount;
            modelEmployee.IsBudgetOwner = employee.IsBudgetOwner;
            if(employee.CostCenter != null)
            {
                modelEmployee.CostCenter = _context.CostCenters.Find(employee.CostCenter.Id);
            }
            if(employee.DepartmentId != null)
            {
                modelEmployee.Department = _context.Departments.Find(employee.DepartmentId);
            }

            _context.Update(modelEmployee);
            _context.SaveChanges();

            return Ok(employee.Id);
        }

        [AllowAnonymous]
        [HttpPost("deleteEmployee")]
        public async Task<IActionResult> RemoveEmployee([FromBody] int employeeId)
        {

            Model.Employee employee = _context.Set<Model.Employee>().Where(e => e.Id == employeeId).FirstOrDefault();
            employee.IsDeleted = true;
            _context.Update(employee);
            _context.SaveChanges();
            return Ok(employeeId);
        }

        [HttpGet]
        [Route("", Order = -1)]
        public async virtual Task<IActionResult> Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string companyIds, string costCenterIds, string includes, bool deleted = false, bool isBudgetOwner = false, bool teamStatus = false)
        {
            List<Model.Employee> items = null;
            IEnumerable<Dto.Employee> itemsResult = null;
            List<int?> compIds = null;
            List<int?> cIds = null;
            string email = string.Empty;

            includes = includes ?? "CostCenter,Company,Manager,Department";


            if ((companyIds != null) && (companyIds.Length > 0)) compIds = JsonConvert.DeserializeObject<string[]>(companyIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((costCenterIds != null) && (costCenterIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(costCenterIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

			if (HttpContext.User.Identity.Name != null)
			{
				var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

				if (user == null)
				{
					user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                }

				if (user != null)
                {
					email = user.Email;
				}
			}
			else
			{
				email = "N/A";
			}

			items = (_itemsRepository as IEmployeesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, compIds, cIds, email, deleted, isBudgetOwner, teamStatus).ToList();


            itemsResult = items.Select(i => _mapper.Map<Dto.Employee>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IEmployeesRepository).GetCountByFilters(filter, compIds, cIds, email, deleted, isBudgetOwner, teamStatus);
                var pagedResult = new Dto.PagedResult<Dto.Employee>(itemsResult, new Dto.PagingInfo()
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
		[Route("teamstatus", Order = -1)]
		public async Task<IActionResult> GetTeamStatus(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string companyIds, string costCenterIds, string includes, bool deleted = false, bool isBudgetOwner = false, bool teamStatus = false)
		{
			List<Model.Employee> items = null;
			IEnumerable<Dto.Employee> itemsResult = null;
			List<int?> compIds = null;
			List<int?> cIds = null;
			string employeeId = string.Empty;
			string admCenterId = string.Empty;
			string userName = string.Empty;
			string role = string.Empty;
            string email = string.Empty;

			includes = includes ?? "CostCenter,Company,Manager";


			if ((companyIds != null) && (companyIds.Length > 0)) compIds = JsonConvert.DeserializeObject<string[]>(companyIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((costCenterIds != null) && (costCenterIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(costCenterIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

			if (HttpContext.User.Identity.Name != null)
			{
				var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

				if (user == null)
				{
					user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				}

                if(user != null)
                {
                    email = user.Email;
                    //email = "Sergiu.Davidescu@emag.ro";
                    //email = "ioana.cristea";

                }
			}
			else
			{
                email = "N/A";
			}

			items = (_itemsRepository as IEmployeesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, compIds, cIds, email, deleted, isBudgetOwner, teamStatus).ToList();


			itemsResult = items.Select(i => _mapper.Map<Dto.Employee>(i));



			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as IEmployeesRepository).GetCountByFilters(filter, compIds, cIds, email, deleted, isBudgetOwner, teamStatus);
				var pagedResult = new Dto.PagedResult<Dto.Employee>(itemsResult, new Dto.PagingInfo()
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

		[HttpPost("updateAllEmployees")]
		public async Task<int> SyncAllEmployees(int companyId)
		{
			int countChanges = 0;

			var count = _context.Set<Model.RecordCount>().FromSql("UpdateAllEmployees").ToList();

			countChanges = count[0].Count;
			return countChanges;
		}

		[HttpGet("export")]
		public IActionResult Export(string filter, string departmentIds, bool deleted = false, bool isBudgetOwner = false)
		{
			List<int> dIds = null;
			List<Model.Employee> employees = null;
            int rowNumber = 0;

            int columnsCount = 10;

            if ((departmentIds != null) && (departmentIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(departmentIds).ToList().Select(int.Parse).ToList();

			using (ExcelPackage package = new ExcelPackage())
			{
				employees = (_itemsRepository as IEmployeesRepository).GetByFilters(filter, "CostCenter,Manager,Company", null, null, null, 5000, null, null, "N/A", deleted, isBudgetOwner, false).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Angajati");
				//First add the headers
				//worksheet.Cells[1, 1].Value = "Trimis email";
				//worksheet.Cells[1, 2].Value = "Confirmat";
				worksheet.Cells[1, 1].Value = "Marca";
				worksheet.Cells[1, 2].Value = "Nume";
				worksheet.Cells[1, 3].Value = "Prenume";
				worksheet.Cells[1, 4].Value = "Email";
				worksheet.Cells[1, 5].Value = "Cod CC";
				worksheet.Cells[1, 6].Value = "Centru de cost";
                worksheet.Cells[1, 7].Value = "Manager";
                worksheet.Cells[1, 8].Value = "Cod Companie";
                worksheet.Cells[1, 9].Value = "Companie";
                worksheet.Cells[1, 10].Value = "Activ";

                int recordIndex = 2;
                int count = employees.Count();

                foreach (var item in employees)
				{
                    rowNumber++;

                    int diff = recordIndex - count;

                    if (diff > 0)
                    {
                        diff = 0;
                    }

                    //worksheet.Cells[recordIndex, 1].Value = item.IsEmailSend == true ? "DA" : "NU";
                    //worksheet.Cells[recordIndex, 2].Value = item.IsConfirmed == true ? "DA" : "NU";
                    worksheet.Cells[recordIndex, 1].Value = item.InternalCode;
					worksheet.Cells[recordIndex, 2].Value = item.FirstName;
					worksheet.Cells[recordIndex, 3].Value = item.LastName;
					worksheet.Cells[recordIndex, 4].Value = item.Email;
					worksheet.Cells[recordIndex, 5].Value = item.CostCenter != null ? item.CostCenter.Code : "";
					worksheet.Cells[recordIndex, 6].Value = item.CostCenter != null ? item.CostCenter.Name : "";
                    worksheet.Cells[recordIndex, 7].Value = item.Manager != null ? item.Manager.FirstName : "";
                    worksheet.Cells[recordIndex, 8].Value = item.Company != null ? item.Company.Code : "";
                    worksheet.Cells[recordIndex, 9].Value = item.Company != null ? item.Company.Name : "";
                    worksheet.Cells[recordIndex, 10].Value = item.IsConfirmed ? "DA" : "NU";

                    if (diff == 0)
                    {

                        for (int i = 1; i <= columnsCount ; i++)
                        {
                            worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        worksheet.Row(1).Height = 35.00;
                        worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.View.FreezePanes(2, 1);

                        using (var cells = worksheet.Cells[1, 1, employees.Count() + 1, columnsCount])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        using (var cells = worksheet.Cells[1, 1, 1, columnsCount])
                        {
                            cells.Style.Font.Bold = true;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
                            cells.Style.Font.Color.SetColor(Color.Black);
                        }

                        using (var cells = worksheet.Cells[2, 1, employees.Count() + 3, columnsCount])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                        }

                        using (var cells = worksheet.Cells[2, 1, employees.Count() + 3, columnsCount])
                        {
                            for (int i = 2; i < employees.Count() + 2; i++)
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
                            }



                        }


                        worksheet.View.ShowGridLines = false;
                        worksheet.View.ZoomScale = 100;

                        for (int i = 1; i <= columnsCount; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        //worksheet.Column(1).Width = 15.00;
                        //worksheet.Column(2).Width = 15.00;
                        worksheet.Column(2).Width = 10.00;
                        worksheet.Column(2).Width = 20.00;
                        worksheet.Column(3).Width = 40.00;
                        worksheet.Column(4).Width = 31.00;
                        worksheet.Column(5).Width = 15.00;
                        worksheet.Column(6).Width = 32.00;
                        worksheet.Column(7).Width = 33.00;
                        worksheet.Column(8).Width = 20.00;
                        worksheet.Column(9).Width = 20.00;

                        worksheet.Cells["A1:J1"].AutoFilter = true;

                    }


                    recordIndex++;
				}

                using (var cells = worksheet.Cells[1, 1, 1, columnsCount])
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
					FileDownloadName = "employees.xlsx"
				};

				return result;

			}
		}

        [AllowAnonymous]
        [HttpGet]
        [Route("sync")]
        public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.Employee> items = (_itemsRepository as IEmployeesRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<EmployeeSync>(i));
            var pagedResult = new Dto.PagedResult<EmployeeSync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }
        //[HttpPost("details/importEmployees")]
        //public virtual IActionResult PostImportEmployees([FromBody] EmployeeImport employeeImport)
        //{
        //    (_itemsRepository as IEmployeesRepository).EmployeeImport(employeeImport);

        //    return Ok(employeeImport);
        //}

        [HttpPost("importEmployees")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            MemoryStream ms = null;
            bool updated = false;
            bool userExist = true;
            Model.ApplicationUser user = null;
            Model.Employee emp = null;
            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");

            ms = new MemoryStream();
            file.CopyTo(ms);

            using (ExcelPackage package = new ExcelPackage(ms))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int rows = worksheet.Dimension.End.Row;
                for (int i = 2; i <= rows; i++)
                {
                    
                    if(worksheet.Cells[i, 2].Text != "")
                    {
                        Dto.EmployeeImport employee = new EmployeeImport
                        {
                            FullName = worksheet.Cells[i, 2].Text,
                            Email = worksheet.Cells[i, 3].Text,
                            InternalCode = worksheet.Cells[i, 4].Text,
                            Status = worksheet.Cells[i, 6].Text
                        };

                        //if(employee.InternalCode == "ICZ3AE6")
                        //{
                        //    Console.WriteLine();
                        //}

                        //Console.WriteLine(employee.InternalCode);

                        (_itemsRepository as IEmployeesRepository).EmployeeImport(employee, out updated, out userExist, out user, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut);

                        if (!updated || !userExist)
                        {
                            user.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.GivenName, ClaimValue = user.GivenName });
                            user.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.FamilyName, ClaimValue = user.FamilyName });
                            string[] roles = { "user" };
                            foreach (string role in roles)
                            {
                                user.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Role, ClaimValue = role });
                            }
                            var password = Guid.NewGuid().ToString("n").Substring(0, 6).ToUpper() + "T3";

                            emp = _context.Set<Model.Employee>().Where(a => a.InternalCode == employee.InternalCode.Trim()).FirstOrDefault();

                            if (emp != null)
                            {
                                emp.ERPCode = password;
                                user.EmployeeId = emp.Id;
                            }

                            var result = await _userManager.CreateAsync(user, password);

                            if (!updated)
                            {
                                var emailMessage = new MimeMessage();

                                emailMessage.From.Add(new MailboxAddress("Operations", "ofa@optima.ro"));
                                if (emailIniOut != "")
                                {
                                    emailMessage.To.Add(new MailboxAddress("", emailIniOut));
                                }
                                else
                                {
                                    emailMessage.To.Add(new MailboxAddress("", "ALIN.CERNATESCU@ALLIANZ.COM"));
                                }
                                emailMessage.To.Add(new MailboxAddress("", "allianztechnology-ro-it@allianz.com")); // trebuie comentata

                                emailMessage.To.Add(new MailboxAddress("", emailIniOut));
                                emailMessage.To.Add(new MailboxAddress("", emailCCOut));

                                emailMessage.Subject = subjectOut;

                                var builder = new BodyBuilder { TextBody = bodyHtmlOut, HtmlBody = bodyHtmlOut };
                                emailMessage.Body = builder.ToMessageBody();

                                using (var client = new MailKit.Net.Smtp.SmtpClient())
                                {

                                    await client.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
                                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                                    await client.AuthenticateAsync("ofa@optima.ro", "Inventory2019");
                                    await client.SendAsync(emailMessage).ConfigureAwait(false);
                                    await client.DisconnectAsync(true).ConfigureAwait(false);
                                }
                            }

                            updated = true;

                        }


                       
                    }

                }
            }

            if (updated)
            {
                return Ok(StatusCode(200));
            }
            else
            {
                return Ok(StatusCode(404));
            }

        }

        [HttpPost("sendEmail/{employeeId}")]
        public async Task<IActionResult> SendMail (int employeeId)
        {

            var files = new FormFileCollection();

            //Model.Document document = null;
            //Model.DocumentType documentType = null;
            Model.EmailManager emailManager = null;
            Model.EmailManager emailManagerComponent = null;
            Model.EmailType emailTypeAsset = null;
            Model.EmailType emailTypeAssetComponent = null;
            Model.Asset asset = null;
            Model.AssetComponent assetComponent = null;
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            Dictionary<int, string> dictCC = new Dictionary<int, string>();
            Dictionary<int, Guid> guidIds = new Dictionary<int, Guid>();
            int number = 0;
            int guidNumber = 0;
            var htmlBodyEmail1 = "";
            var htmlBodyEmailComponent = "";
            var htmlBodyEnd = "";
            var htmlBodyCompany1 = "";
            var htmlBodyCompany2 = "";
            var company1 = "";
            var company2 = "";
            var link1 = "";
            var link2 = "";
            var htmlBody1 = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    Upcoming topics
                                                </title>
                                                <style type=""text/css"">
                                                    HTML{background-color: #e8e8e8;}
                                                    .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
                                                    .courses-table .description {color: #fefefe !important;}
                                                    .courses-table .description a{color: #ffffff !important; font: bold 11px Arial;
                                                      text-decoration: none;
                                                      background-color: #fe0000;
                                                      padding: 2px 6px 2px 6px;
                                                      border-top: 1px solid #CCCCCC;
                                                      border-right: 1px solid #333333;
                                                      border-bottom: 1px solid #333333;
                                                      border-left: 1px solid #CCCCCC;}
                                                    .courses-table td{border: 1px solid #ffffff; background-color: #000000; text-align: center; padding: 8px;}
                                                    .courses-table th{border: 1px solid #ffffff; color: #030804;text-align: center; padding: 8px;}
                                                    .red{background-color: #FFDD04;}
                                                    .header{background-color: #fe0000; color: #000000 !important;}
                                                    .msg{background-color: #000000; color: #fefefe !important;}
                                                    .table-header{background-color: #000000; color: #fefefe !important;}
                                                    .table-body{background-color: #000000; color: #fefefe !important;}
                                                    .green{background-color: #6B9852;}
                                                   
                                                </style>
                                            </head>
                                            <body>
                
                                    <h2>Buna ziua,</h2>

                        <br>
                                              
                                                <table class=""courses-table"">
                                                    <thead>
                                                    
                                        ";
           
            var htmlBody2 = @"
                                       
                                                
                                                        <tr>
                                                            <th class=""header"">Numar Inventar</th>
                                                            <th class=""header"">Denumire</th>
                                                         
                                                            <th class=""header"">Locatie</th>
                                                            <th class=""header"" colspan=""2"">Valid</th>
                                                            
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                        ";
            //documentType = _context.Set<Model.DocumentType>().Where(d => d.Id == 16).Single();
            emailTypeAsset = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE ASSET").Single();
            emailTypeAssetComponent = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE COMPONENT").Single();


            List<Model.Asset> assets = _context.Set<Model.Asset>().Include(r => r.Room).Where(a => a.EmployeeId == employeeId).ToList();
            List<Model.AssetComponent> assetComponents = _context.Set<Model.AssetComponent>().Where(a => a.EmployeeId == employeeId).ToList();


            // var itemsResource = _mapper.Map<List<Model.Asset>, List<Dto.AssetUI>>(assets);


            EmailUI emailUIs = null;

            emailUIs = new EmailUI
            {
                AssetComponents = new List<Dto.AssetComponent>(),
                Assets = new List<AssetUI>()
            };

            foreach (var item in assetComponents)
            {
                emailUIs.AssetComponents.Add(new Dto.AssetComponent() {

                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name
                });
            }


            foreach (var item in assets)
            {
                emailUIs.Assets.Add(new AssetUI()
                {

                    Id = item.Id,
                    InvNo = item.InvNo,
                    Name = item.Name,
                    Room = new CodeNameEntity {Id = item.Room.Id, Code = item.Room.Code, Name = item.Room.Name }
                });
            }



            //document = new Model.Document
            //{
            //    DocumentType = documentType,
            //    DocNo1 = string.Empty,
            //    DocNo2 = string.Empty,
            //    DocumentDate = DateTime.Now,
            //    RegisterDate = DateTime.Now,
            //    Approved = true,
            //    Exported = false,
            //    CreationDate = DateTime.Now,
            //    Details = string.Empty
            //};

            //_context.Set<Model.Document>().Add(document);


           // _context.SaveChanges();

            var subject = "Validare Lista Active + Accesorii";


            if (emailUIs.Assets.Count > 0)
            {
                for (int i = 0; i < emailUIs.Assets.Count(); i++)
                {


                    if (emailUIs.Assets[i].Id > 0)
                    {

                        Model.Room roomIni = null;
                        Model.Location locationIni = null;
                        Model.Employee eIni = null;
                        eIni = _context.Set<Model.Employee>().Include(d => d.Department).Where(e => e.Id == employeeId).SingleOrDefault();

                        if (emailUIs.AssetComponents.Count > 0 && emailUIs.AssetComponents.Count > i)
                        {
                            emailManagerComponent = new Model.EmailManager
                            {
                                EmailType = emailTypeAssetComponent,
                                EmployeeInitial = eIni,
                                EmployeeFinal = eIni,
                                AssetId = null,
                                AssetComponentId = emailUIs.AssetComponents[i].Id,
                                Guid = Guid.NewGuid(),
                                IsAccepted = true
                            };

                            _context.Set<Model.EmailManager>().Add(emailManagerComponent);
                            _context.SaveChanges();
                        }


                        emailManager = new Model.EmailManager
                        {
                            EmailType = emailTypeAsset,
                            EmployeeInitial = eIni,
                            EmployeeFinal = eIni,
                            AssetId = emailUIs.Assets[i].Id,
                            AssetComponentId = null,
                            Guid = Guid.NewGuid(),
                            IsAccepted = true
                        };

                        _context.Set<Model.EmailManager>().Add(emailManager);

                        if (emailUIs.Assets[i].Room != null)
                        {

                            roomIni = _context.Set<Model.Room>().Where(a => a.Id == emailUIs.Assets[i].Room.Id).FirstOrDefault();

                            if (roomIni != null)
                            {
                                locationIni = _context.Set<Model.Location>().Where(a => a.Id == roomIni.LocationId).FirstOrDefault();

                                emailManager.RoomIdInitial = roomIni.Id;
                                emailManager.RoomIdFinal = roomIni.Id;
                            }
                        }

                        _context.SaveChanges();

                        if(eIni.Email != null && eIni.Email != "" && !dictCC.ContainsValue(eIni.Email)) {
                            dictCC.Add(number, eIni.Email);
                            number++;
                        }

                        if (eIni != null && eIni.Department != null && eIni.Department.Name != null && eIni.Department.Name != "" && !dictCC.ContainsValue(eIni.Department.Name))
                        {
                            dictCC.Add(number, eIni.Department.Name);
                            number++;
                        }

                        if (emailManager != null && emailManager.Guid != Guid.Empty && !guidIds.ContainsValue(emailManager.Guid))
                        {
                            guidIds.Add(guidNumber, emailManager.Guid);
                            guidNumber++;
                        }

                        if (emailManagerComponent != null && emailManagerComponent.Guid != Guid.Empty && !guidIds.ContainsValue(emailManagerComponent.Guid))
                        {
                            guidIds.Add(guidNumber, emailManagerComponent.Guid);
                            guidNumber++;
                        }


                        if (emailUIs.Assets.Count() > 1)
                        {
                            if (i == 0)
                            {

                                company1 = asset != null ? asset.Company.Name : "LISTA ACTIVE";
                                company2 = asset != null ? asset.Company.Name : "LISTA ACCESORII";

                                htmlBodyCompany1 = htmlBodyCompany1 + @"<tr><th class=""msg"" colspan=""5"">" + company1 + "</th></tr>"; ;
                                

                                if (emailUIs.AssetComponents.Count > 0)
                                {
                                    htmlBodyCompany2 = htmlBodyCompany2 + @"<tr><th class=""msg"" colspan=""5"">" + company2 + "</th></tr>";

                                    htmlBodyEnd = htmlBodyEnd + htmlBodyCompany2 + @"
                                                        <tr>
                                                            <th class=""header"" colspan=""3"">Denumire</th>
                                                            <th class=""header"" colspan=""2"">Valid</th>
                                                            
                                                        </tr>
                                             ";
                                }
                               

                               

                                //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
                                link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validatemultiple/" + emailManager.Guid + "/2";
                                //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
                                link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;

                                htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                            <td class=""description"">" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td class=""description"">" + emailUIs.Assets[i].Name + @" </ td >
                                                         
                                                            <td class=""description"">" + emailUIs.Assets[i].Room.Code + @" </ td >
                                                           
                                                            <td class=""description"" colspan=""2""><a href='" + link2 + "'" + "' >NU</a>" + @" </ td >
                                                        </tr>
                                                       
                                        ";


                            }
                            else
                            {

                                link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManager.Guid + "/" + emailUIs.Assets[i].InvNo + "/2";
                                link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;

                                htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                             <td class=""description"">" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td class=""description"">" + emailUIs.Assets[i].Name + @" </ td >
                                                          
                                                            <td class=""description"">" + emailUIs.Assets[i].Room.Code + @" </ td >
                                                          
                                                            <td class=""description"" colspan=""2""><a href='" + link2 + "'" + "' >NU</a>" + @" </ td >
                                                        </tr>
                                                       
                                        ";


                            }
                        }
                        else
                        {
                            company1 = asset != null ? asset.Company.Name : "LISTA ACTIVE";
                            company2 = asset != null ? asset.Company.Name : "LISTA ACCESORII";

                            htmlBodyCompany1 = htmlBodyCompany1 + @"<tr> <th class=""msg"" colspan=""5"">" + company1 + "</th></tr>"; ;

                            if (emailUIs.AssetComponents.Count > 0)
                            {
                                htmlBodyCompany2 = htmlBodyCompany2 + @"<tr><th class=""msg"" colspan=""5"">" + company2 + "</th></tr>";

                                htmlBodyEnd = htmlBodyEnd + htmlBodyCompany2 + @"
                                                        <tr>
                                                            <th class=""header"" colspan=""3"">Denumire</th>
                                                            <th class=""header"" colspan=""2"">Valid</th>
                                                            
                                                        </tr>
                                             ";
                            }
                           
                            

                            //htmlBodyEnd = htmlBodyEnd + @"</tbody>
                            //                    </table>
                            //                        <h3> Pentru validarea acestora, acceseaza urmatorul link  https://service.inventare.ro/BOFO/#/operations
                            //                        <br>
                            //                        <h3> Multumesc, </ h3 >
                            //                        <br>
                            //                        <h3> Referent " + eIni + @" </ h3 >

                            //                </body>
                            //            </html> ";

                          

                            link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManager.Guid + "/" + emailUIs.Assets[i].InvNo + "/2";
                            link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;

                            htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                            <td class=""description"">" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td class=""description"">" + emailUIs.Assets[i].Name + @" </ td >
                                                          
                                                            <td class=""description"">" + emailUIs.Assets[i].Room.Code + @" </ td >
                                                           
                                                            <td class=""description"" colspan=""2""><a href='" + link2 + "'" + "' >NU</a>" + @" </ td >
                                                        </tr>
                                                       
                                        ";


                        }


                    }

                }
            }


            if (emailUIs.AssetComponents.Count > 0)
            {
                for (int i = 0; i < emailUIs.AssetComponents.Count; i++)
                {
                    link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManagerComponent.Guid;

                    htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                            <td class=""description"" colspan=""3"">" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                         
                                                           
                                                            <td class=""description"" colspan=""2""><a href='" + link2 + "'" + "' >NU</a>" + @" </ td >
                                                        </tr>
                                                       
                                         ";

                }
            }

            string imageLink = string.Empty;

            string FullFormatPath = Path.Combine(Environment.CurrentDirectory, "upload", "Logo.jpg");
            //string[] ImgPaths = Directory.GetFiles(Path.Combine(FullFormatPath, "upload"));
            //var bodyBuilder = new BodyBuilder { HtmlBody = "" };

            //foreach (string imgpath in ImgPaths)
            //{
            //    var image = bodyBuilder.LinkedResources.Add(imgpath);
            //    image.ContentId = MimeUtils.GenerateMessageId();
            //    //HtmlFormat = HtmlFormat.Replace(Path.GetFileName(imgpath), string.Format("cid:{0}", image.ContentId));
            //    imageLink = string.Format("cid:{0}", image.ContentId);
            //}

            var end = @"</tbody>
                                                </table>
                                                    <h3> Pentru stergerea unui activ din lista se foloseste butonul NU </h3>
                                                   
                                                    <img style=""display:block"" width=""100"" heigth=""100""  src=" + FullFormatPath + @">
                                                    <img style=""display:block"" width=""100"" heigth=""100""  src=" + FullFormatPath + @">

                                                    <br>
                                                    <h3> Multumesc, </h3>      
                                            </body>
                                        </html>";

            foreach (var item in dictCC)
            {
                cc.Add(item.Value);
            }

           if (cc.Count == 0)
            {
                cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
            }
            var bodyHtmlOut = htmlBody1 + htmlBodyCompany1 + htmlBody2 + htmlBodyEmail1 + htmlBodyEnd + htmlBodyEmailComponent + end;
            // var messageAttach = new Message(cc, subject, bodyHtmlOut, files);
            var messageAttach = new Message(cc, cc, bcc, subject, bodyHtmlOut, null);





            // var message = new Message(new string[] { "adrian.cirnaru@optima.ro" }, "Test email", "This is the content from our email.");

            // new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };


            //_emailSender.SendEmail(message);


            var success = await _emailSender.SendEmailAsync(messageAttach);

            if (!success)
            {
                foreach (var item in guidIds)
                {
                    Model.EmailManager eManager = _context.Set<Model.EmailManager>().Where(e => e.Guid == item.Value).SingleOrDefault();

                    if (eManager != null)
                    {
                        eManager.IsDeleted = true;
                        _context.Update(eManager);
                        _context.SaveChanges();
                    }
                }
            }

            return Ok(success);
        }

        [HttpPost("sendBookEmail/{employeeId}")]
        public async Task<IActionResult> SendBookMail(int employeeId)
        {

            var files = new FormFileCollection();

            //Model.Document document = null;
            //Model.DocumentType documentType = null;
            //Model.EmailManager emailManager = null;
            //Model.EmailManager emailManagerComponent = null;
            Model.EmailType emailTypeAsset = null;
            Model.EmailType emailTypeAssetComponent = null;
            Model.Asset asset = null;
            Model.AssetComponent assetComponent = null;
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            Dictionary<int, string> dictCC = new Dictionary<int, string>();
            //Dictionary<int, Guid> guidIds = new Dictionary<int, Guid>();
            Dictionary<string,int> aCats = new Dictionary<string, int>();
            Dictionary<string, int> aCompCats = new Dictionary<string, int>();
            int number = 0;
            int rowSpan = 1;
            int span = 1;
            int rowSpanComp = 1;
            int spanComp = 1;
            string categoryCode = "";
            string categoryComponentCode = "";
            //int guidNumber = 0;
            var htmlBodyEmail1 = "";
            var htmlBodyEmailComponent = "";
            var htmlBodyEnd = "";
            var htmlBodyCompany1 = "";
            var htmlBodyCompany2 = "";
            var company1 = "";
            //string imageLink = "https://service.inventare.ro/socgenupload/";
            string imageLink = "C://Work//github//fais1.0//Emag//api5.0//Optima.Fais.Api//upload//";
            string defaultImage = "blank.jpg";
            //var company2 = "";
            //var link1 = "";
            //var link2 = "";
            var headerMsg = "";
            var footerMsg = "";
            var end = @"</tbody>
                                                </table>";
            var htmlBody11 = @"
                                            <br>
                                              
                                                <table class=""minimalistBlack"">
                                                    <thead>";
            var htmlBody1 = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    Upcoming topics
                                                </title>
                                                <style type=""text/css"">
                                                    HTML{background-color: #e8e8e8;}
                                                    .courses-table{border: 3px solid #000000;
                                                      width: 100%;
                                                      text-align: center;
                                                      border-collapse: collapse}
                                                    .courses-table .description {color: #fefefe !important;}
                                                    .courses-table .description a{color: #ffffff !important; font: bold 11px Arial;
                                                      text-decoration: none;
                                                      background-color: #fe0000;
                                                      padding: 2px 6px 2px 6px;
                                                      border-top: 1px solid #CCCCCC;
                                                      border-right: 1px solid #333333;
                                                      border-bottom: 1px solid #333333;
                                                      border-left: 1px solid #CCCCCC;}
                                                    .courses-table td{border: 1px solid #ffffff; background-color: #000000; text-align: center; padding: 8px;}
                                                    .courses-table th{border: 1px solid #ffffff; color: #030804;text-align: center; padding: 8px;}
                                                    .red{background-color: #FFDD04;}
                                                    .header{background-color: #fe0000; color: #000000 !important;}
                                                    .msg{background-color: #000000; color: #fefefe !important;}
                                                    .redmsg{color: #FF0000 !important;text-align: center;}
                                                    .message{color: #000000 !important;text-align: center;}
                                                    .table-header{background-color: #000000; color: #fefefe !important;}
                                                    .table-body{background-color: #000000; color: #fefefe !important;}
                                                    .green{background-color: #6B9852;}
                                                    table.minimalistBlack {
                                                      border: 3px solid #000000;
                                                      width: 100%;
                                                      text-align: center;
                                                      border-collapse: collapse;
                                                    }
                                                    table.minimalistBlack td, table.minimalistBlack th {
                                                      border: 1px solid #000000;
                                                      padding: 5px 4px;
                                                    }
                                                    table.minimalistBlack tbody td {
                                                      font-size: 13px;
                                                    }
                                                    table.minimalistBlack thead {
                                                      background: #CFCFCF;
                                                      background: -moz-linear-gradient(top, #dbdbdb 0%, #d3d3d3 66%, #CFCFCF 100%);
                                                      background: -webkit-linear-gradient(top, #dbdbdb 0%, #d3d3d3 66%, #CFCFCF 100%);
                                                      background: linear-gradient(to bottom, #dbdbdb 0%, #d3d3d3 66%, #CFCFCF 100%);
                                                      border-bottom: 3px solid #000000;
                                                    }
                                                    table.minimalistBlack thead th {
                                                      font-size: 15px;
                                                      font-weight: bold;
                                                      color: #000000;
                                                      text-align: center;
                                                    }
                                                    table.minimalistBlack tfoot td {
                                                      font-size: 14px;
                                                    }

                                                   
                                                </style>
                                            </head>
                                            <body>        
                                        ";

            var htmlBody2 = @"
                                       
                                                       
                                                        <tr>
                                                            <th>Category</th>
                                                            <th>Asset Number</th>
                                                            <th>Description</th>
                                                            <th>Serial Number</th>
                                                            <th>Photo</th>
                                                            
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                        ";
            //documentType = _context.Set<Model.DocumentType>().Where(d => d.Id == 16).Single();
            emailTypeAsset = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE ASSET").Single();
            emailTypeAssetComponent = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE COMPONENT").Single();
            headerMsg = String.Format("{0}", emailTypeAsset.HeaderMsg);
            footerMsg = String.Format("{0}", emailTypeAsset.FooterMsg);
            Model.Inventory inventory = _context.Set<Model.Inventory>().Where(i => i.Active == true).SingleOrDefault();
            List<Model.Asset> assets = _context.Set<Model.Asset>().Include(s => s.SubType).Include(d => d.Document).Where(a => a.EmployeeId == employeeId && a.Document.ParentDocumentId == inventory.DocumentId).OrderBy(a => a.SubType.TypeId).ToList();
            List<Model.AssetComponent> assetComponents = _context.Set<Model.AssetComponent>().Include(s => s.SubType).Where(a => a.EmployeeId == employeeId).OrderBy(a => a.SubType.TypeId).ToList();


            // var itemsResource = _mapper.Map<List<Model.Asset>, List<Dto.AssetUI>>(assets);


            EmailUI emailUIs = null;

            emailUIs = new EmailUI
            {
                AssetComponents = new List<Dto.AssetComponent>(),
                Assets = new List<AssetUI>()
            };

            foreach (var item in assetComponents)
            {
                List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.EntityId == item.SubTypeId && e.EntityType.Code == "SUBTYPE").ToList();

                emailUIs.AssetComponents.Add(new Dto.AssetComponent()
                {

                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    ImageName = entityFiles.Count > 0 ? entityFiles[0].StoredAs : "",
                    Type = item.SubType != null ?  new CodeNameEntity { Id = item.SubType.Id, Code = item.SubType.Code } : null

                });


            }


			for (int k = 0; k < assets.Count; k++)
			{
                List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.EntityId == assets[k].SubTypeId && e.EntityType.Code == "SUBTYPE").ToList();

                emailUIs.Assets.Add(new AssetUI()
                {

                    Id = assets[k].Id,
                    InvNo = assets[k].InvNo,
                    Name = assets[k].Name,
                    SerialNumber = assets[k].SerialNumber,
                    // Room = new CodeNameEntity { Id = item.Room.Id, Code = item.Room.Code, Name = item.Room.Name },
                    // Location = new CodeNameEntity { Id = item.Room.Location.Id, Code = item.Room.Location.Code, Name = item.Room.Location.Name },
                    // Region = new CodeNameEntity { Id = item.Room.Location.Region.Id, Code = item.Room.Location.Region.Code, Name = item.Room.Location.Region.Name },
                    ImageName = entityFiles.Count > 0 ? entityFiles[0].StoredAs : "",
                    Type = assets[k].SubType != null ? new CodeNameEntity { Id = assets[k].SubType.Id, Code = assets[k].SubType.Code } : null
                });

            }




            var itemsByGroupWithCount = assets.GroupBy(i => i.SubType != null ? i.SubType.Code : "")
                 .Select(item => new  {
                     Code = item.Key,
                     Total = item.Count(),
                 }).ToList();


            foreach (var item in itemsByGroupWithCount)
            {
                if (!aCats.ContainsKey(item.Code))
                {
                    aCats.Add(item.Code, item.Total);
                    rowSpan++;
                }
                
            }

            var itemsByGroupWithCountComp = assetComponents.GroupBy(i => i.SubType != null ? i.SubType.Code : "")
                .Select(item => new {
                    Code = item.Key,
                    Total = item.Count(),
                }).ToList();


            foreach (var item in itemsByGroupWithCountComp)
            {
                if (!aCompCats.ContainsKey(item.Code))
                {
                    aCompCats.Add(item.Code, item.Total);
                    rowSpanComp++;
                }

            }
            //document = new Model.Document
            //{
            //    DocumentType = documentType,
            //    DocNo1 = string.Empty,
            //    DocNo2 = string.Empty,
            //    DocumentDate = DateTime.Now,
            //    RegisterDate = DateTime.Now,
            //    Approved = true,
            //    Exported = false,
            //    CreationDate = DateTime.Now,
            //    Details = string.Empty
            //};

            //_context.Set<Model.Document>().Add(document);


            // _context.SaveChanges();

            var subject = "Fisa Lichidare";


            if (emailUIs.Assets.Count > 0)
            {
                for (int i = 0; i < emailUIs.Assets.Count(); i++)
                {


                    if (emailUIs.Assets[i].Id > 0)
                    {

                        //Model.Room roomIni = null;
                        //Model.Location locationIni = null;
                        Model.Employee eIni = null;
                        eIni = _context.Set<Model.Employee>().Include(d => d.Department).Where(e => e.Id == employeeId).SingleOrDefault();


                        if (emailUIs.Assets[i].Room != null)
                        {

                            //roomIni = _context.Set<Model.Room>().Where(a => a.Id == emailUIs.Assets[i].Room.Id).FirstOrDefault();

                            //if (roomIni != null)
                            //{
                            //    locationIni = _context.Set<Model.Location>().Where(a => a.Id == roomIni.LocationId).FirstOrDefault();
                            //}
                        }

                        _context.SaveChanges();

                        if (eIni.Email != null && eIni.Email != "" && !dictCC.ContainsValue(eIni.Email))
                        {
                            dictCC.Add(number, eIni.Email);
                            number++;
                        }

                        if (eIni != null && eIni.Department != null && eIni.Department.Name != null && eIni.Department.Name != "" && !dictCC.ContainsValue(eIni.Department.Name))
                        {
                            dictCC.Add(number, eIni.Department.Name);
                            number++;
                        }


                        if (emailUIs.Assets.Count() > 1)
                        {
                            if (i == 0)
                            {

                                company1 = asset != null ? asset.Company.Name : "Items";

                                htmlBodyCompany1 = htmlBodyCompany1 + @"<tr><th colspan=""5"">" + company1 + "</th></tr>"; ;

                                aCats.TryGetValue(emailUIs.Assets[i].Type.Code, out rowSpan);

                                categoryCode = emailUIs.Assets[i].Type.Code;

                                if (emailUIs.Assets[i].ImageName != "")
                                {
                                    htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                            <td rowspan=""" + rowSpan + @""">" + emailUIs.Assets[i].Type.Code + @" </ td >
                                                            <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                             <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                           <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
                                                        </tr>
                                                       
                                        ";
                                }
                                else
                                {
                                    htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                            <td rowspan=""" + rowSpan + @""">" + emailUIs.Assets[i].Type.Code + @" </ td >
                                                            <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                            <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                           <td>" + @" </td>
                                                        </tr>
                                                       
                                        ";
                                }

                             
                               


                            }
                            else
                            {

                                aCats.TryGetValue(emailUIs.Assets[i].Type.Code, out rowSpan);

                                // var lastTypeId = emailUIs.Assets[i].Type.Id;

                                if (categoryCode != emailUIs.Assets[i].Type.Code)
                                {
                                    span = rowSpan;
                                    categoryCode = emailUIs.Assets[i].Type.Code;

                                    if (emailUIs.Assets[i].ImageName != "")
                                    {
                                        htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                             <td rowspan=""" + rowSpan + @""">" + emailUIs.Assets[i].Type.Code + @" </td>
                                                             <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                            <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                            <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
                                                        </tr>
                                                       
                                        ";
                                    }
                                    else
                                    {
                                        htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                             <td rowspan=""" + rowSpan + @""">" + emailUIs.Assets[i].Type.Code + @" </td>
                                                             <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                             <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                            <td>" + @" </td>
                                                        </tr>
                                                       
                                        ";
                                    }
                                      
                                }
                                else
                                {
                                    if (emailUIs.Assets[i].ImageName != "")
                                    {
                                        htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                           
                                                             <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                             <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                            <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
                                                        </tr>
                                                       
                                        ";
                                    }
                                    else
                                    {
                                        htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                           
                                                             <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                             <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                            <td>" + @" </td>
                                                        </tr>
                                                       
                                        ";
                                    }
                                       
                                }

                               




                            }
                        }
                        else
                        {
                            company1 = asset != null ? asset.Company.Name : "Items";

                            htmlBodyCompany1 = htmlBodyCompany1 + @"<tr><th colspan=""5"">" + company1 + "</th></tr>";

                            if (emailUIs.Assets[i].ImageName != "")
                            {
                                htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                             <td>" + emailUIs.Assets[i].Type.Code + @" </td>
                                                             <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                            <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                            <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
                                                        </tr>
                                                       
                                        ";
                            }
                            else
                            {
                                htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                             <td>" + emailUIs.Assets[i].Type.Code + @" </td>
                                                             <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                            <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                            <td>" + @" </td>
                                                        </tr>
                                                       
                                        ";
                            }

                              


                        }


                    }

                }
            }


            if (emailUIs.AssetComponents.Count > 0)
            {
               

                for (int i = 0; i < emailUIs.AssetComponents.Count; i++)
                {

                    if (i == 0)
                    {
                        aCompCats.TryGetValue(emailUIs.AssetComponents[i].Type.Code, out rowSpanComp);

                        categoryComponentCode = emailUIs.AssetComponents[i].Type.Code;


                        if (emailUIs.AssetComponents[i].ImageName != "")
                        {
                           
                            htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                            <td rowspan=""" + rowSpanComp + @""">" + emailUIs.AssetComponents[i].Type.Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                         
                                                            <td>" + @" </ td >
                                                           <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.AssetComponents[i].ImageName + @"""  >" + @" </td>
                                                        </tr>
                                                       
                                         ";
                        }
                        else
                        {
                            htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                             <td rowspan=""" + rowSpanComp + @""">" + emailUIs.AssetComponents[i].Type.Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                            <td>" + @" </ td >
                                                           <td>" + @" </td>
                                                        </tr>
                                                       
                                         ";
                        }
                           
                    }
                    else
                    {

                        aCompCats.TryGetValue(emailUIs.AssetComponents[i].Type.Code, out rowSpanComp);

                        if (categoryComponentCode != emailUIs.AssetComponents[i].Type.Code)
                        {

                            spanComp = rowSpanComp;
                            categoryComponentCode = emailUIs.AssetComponents[i].Type.Code;


                            if (emailUIs.AssetComponents[i].ImageName != "")
                            {
                                htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                     
                                                            <td rowspan=""" + rowSpanComp + @""">" + emailUIs.AssetComponents[i].Type.Code + @" </td>
                                                             <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                            <td>" + @" </ td >
                                                            <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.AssetComponents[i].ImageName + @"""  >" + @" </td>
                                                             </tr>
                                                       
                                         ";
                            }
                            else
                            {
                                htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                           
                                                             <td rowspan=""" + rowSpanComp + @""">" + emailUIs.AssetComponents[i].Type.Code + @" </td>
                                                             <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                            <td>" + @" </ td >
                                                            <td>" + @" </td>
                                                        </tr>
                                                       
                                         ";
                            }
                        }
                        else
                        {
                            if (emailUIs.AssetComponents[i].ImageName != "")
                            {
                                htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                           
                                                             <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                            <td>" + @" </ td >
                                                            <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.AssetComponents[i].ImageName + @"""  >" + @" </td>
                                                        </tr>
                                                       
                                         ";
                            }
                            else
                            {
                                htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                           
                                                           
                                                             <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                            <td>" + @" </ td >
                                                            <td>" + @" </td>
                                                        </tr>
                                                       
                                         ";
                            }
                        }


                           
                           
                    }
                   

                }
            }

            //string imageLink = string.Empty;

            //string FullFormatPath = Path.Combine(Environment.CurrentDirectory, "upload", "Logo.jpg");

           
            var end1 = @" </body>
                                        </html>";

            foreach (var item in dictCC)
            {
                cc.Add(item.Value);
            }

			//if (cc.Count == 0)
			//{
			//    cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
			//}

			cc = new List<string>();

			cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");


			var bodyHtmlOut = htmlBody1 + headerMsg + htmlBody11 + htmlBodyCompany1 + htmlBody2 + htmlBodyEmail1 + htmlBodyEnd + htmlBodyEmailComponent + end + footerMsg + end1;
            // var messageAttach = new Message(cc, subject, bodyHtmlOut, files);
            var messageAttach = new Message(cc, cc, bcc, subject, bodyHtmlOut, null);





            // var message = new Message(new string[] { "adrian.cirnaru@optima.ro" }, "Test email", "This is the content from our email.");

            // new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };


            //_emailSender.SendEmail(message);


            var success = await _emailSender.SendEmailAsync(messageAttach);

            return Ok(success);
        }

        [HttpGet("sendBookEmailPreview/{employeeId}")]
        public async Task<string> BookEmailPreview(int employeeId)
        {

            var files = new FormFileCollection();

            Model.EmailType emailTypeAsset = null;
            Model.EmailType emailTypeAssetComponent = null;
            Model.Asset asset = null;
            Model.AssetComponent assetComponent = null;
            List<string> cc = new List<string>();
            Dictionary<int, string> dictCC = new Dictionary<int, string>();
            //Dictionary<int, Guid> guidIds = new Dictionary<int, Guid>();
            Dictionary<string, int> aCats = new Dictionary<string, int>();
            Dictionary<string, int> aCompCats = new Dictionary<string, int>();
            int number = 0;
            int rowSpan = 1;
            int span = 1;
            int rowSpanComp = 1;
            int spanComp = 1;
            string categoryCode = "";
            string categoryComponentCode = "";
            //int guidNumber = 0;
            var htmlBodyEmail1 = "";
            var htmlBodyEmailComponent = "";
            var htmlBodyEnd = "";
            var htmlBodyCompany1 = "";
            var htmlBodyCompany2 = "";
            var company1 = "";
            string imageLink = "https://service.inventare.ro/socgenupload/";
            //string imageLink = "C:/Work/github/fais1.0/SocieteGeneraleDemo/api2.0/Optima.Fais.Api/upload/";
            string defaultImage = "blank.jpg";
            //var company2 = "";
            //var link1 = "";
            //var link2 = "";
            var headerMsg = "";
            var footerMsg = "";
            var end = @"</tbody>
                                                </table>";
            var htmlBody11 = @"
                                            <br>
                                              
                                                <table class=""minimalistBlack"">
                                                    <thead>";
            var htmlBody1 = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    OPTIMA
                                                </title>
                                                <style type=""text/css"">
                                                    HTML{background-color: #ffffff;}
                                                    .courses-table{border: 3px solid #000000;
                                                      width: 100%;
                                                      text-align: center;
                                                      border-collapse: collapse}
                                                    .courses-table .description {color: #fefefe !important;}
                                                    .courses-table .description a{color: #ffffff !important; font: bold 11px Arial;
                                                      text-decoration: none;
                                                      background-color: #fe0000;
                                                      padding: 2px 6px 2px 6px;
                                                      border-top: 1px solid #CCCCCC;
                                                      border-right: 1px solid #333333;
                                                      border-bottom: 1px solid #333333;
                                                      border-left: 1px solid #CCCCCC;}
                                                    .courses-table td{border: 1px solid #ffffff; background-color: #000000; text-align: center; padding: 8px;}
                                                    .courses-table th{border: 1px solid #ffffff; color: #030804;text-align: center; padding: 8px;}
                                                    .red{background-color: #FFDD04;}
                                                    .header{background-color: #fe0000; color: #000000 !important;}
                                                    .msg{background-color: #000000; color: #fefefe !important;}
                                                    .redmsg{color: #FF0000 !important;text-align: center;}
                                                    .message{color: #000000 !important;text-align: center;}
                                                    .table-header{background-color: #000000; color: #fefefe !important;}
                                                    .table-body{background-color: #000000; color: #fefefe !important;}
                                                    .green{background-color: #6B9852;}
                                                    table.minimalistBlack {
                                                      border: 3px solid #000000;
                                                      width: 100%;
                                                      text-align: center;
                                                      border-collapse: collapse;
                                                    }
                                                    table.minimalistBlack td, table.minimalistBlack th {
                                                      border: 1px solid #000000;
                                                      padding: 5px 4px;
                                                    }
                                                    table.minimalistBlack tbody td {
                                                      font-size: 12px;
                                                    }
                                                    table.minimalistBlack thead {
                                                      background: #CFCFCF;
                                                      background: -moz-linear-gradient(top, #dbdbdb 0%, #d3d3d3 66%, #CFCFCF 100%);
                                                      background: -webkit-linear-gradient(top, #dbdbdb 0%, #d3d3d3 66%, #CFCFCF 100%);
                                                      background: linear-gradient(to bottom, #dbdbdb 0%, #d3d3d3 66%, #CFCFCF 100%);
                                                      border-bottom: 3px solid #000000;
                                                    }
                                                    table.minimalistBlack thead th {
                                                      font-size: 13px;
                                                      font-weight: bold;
                                                      color: #000000;
                                                      text-align: center;
                                                    }
                                                    table.minimalistBlack tfoot td {
                                                      font-size: 12px;
                                                    }

                                                   
                                                </style>
                                            </head>
                                            <body>        
                                        ";

            var htmlBody2 = @"
                                       
                                                       
                                                        <tr>
                                                            <th>Category</th>
                                                            <th>Asset Number</th>
                                                            <th>Description</th>
                                                            <th>Serial Number</th>
                                                            <th>Photo</th>
                                                            
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                        ";
            //documentType = _context.Set<Model.DocumentType>().Where(d => d.Id == 16).Single();
            emailTypeAsset = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE ASSET").Single();
            emailTypeAssetComponent = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE COMPONENT").Single();
            headerMsg = String.Format("{0}", emailTypeAsset.HeaderMsg);
            footerMsg = String.Format("{0}", emailTypeAsset.FooterMsg);
            Model.Inventory inventory = _context.Set<Model.Inventory>().Where(i => i.Active == true).SingleOrDefault();
            List<Model.Asset> assets = _context.Set<Model.Asset>().Include(s => s.SubType).Include(d => d.Document).Where(a => a.EmployeeId == employeeId && a.Document.ParentDocumentId == inventory.DocumentId).OrderBy(a => a.SubType.TypeId).ToList();
            List<Model.AssetComponent> assetComponents = _context.Set<Model.AssetComponent>().Include(s => s.SubType).Where(a => a.EmployeeId == employeeId).OrderBy(a => a.SubType.TypeId).ToList();


            // var itemsResource = _mapper.Map<List<Model.Asset>, List<Dto.AssetUI>>(assets);


            EmailUI emailUIs = null;

            emailUIs = new EmailUI
            {
                AssetComponents = new List<Dto.AssetComponent>(),
                Assets = new List<AssetUI>()
            };

            foreach (var item in assetComponents)
            {
                List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.EntityId == item.SubTypeId && e.EntityType.Code == "SUBTYPE").ToList();

                emailUIs.AssetComponents.Add(new Dto.AssetComponent()
                {

                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    ImageName = entityFiles.Count > 0 ? entityFiles[0].StoredAs : "",
                    Type = item.SubType != null ? new CodeNameEntity { Id = item.SubType.Id, Code = item.SubType.Code } : null

                });


            }


            for (int k = 0; k < assets.Count; k++)
            {
                List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.EntityId == assets[k].SubTypeId && e.EntityType.Code == "SUBTYPE").ToList();

                emailUIs.Assets.Add(new AssetUI()
                {

                    Id = assets[k].Id,
                    InvNo = assets[k].InvNo,
                    Name = assets[k].Name,
                    SerialNumber = assets[k].SerialNumber,
                    // Room = new CodeNameEntity { Id = item.Room.Id, Code = item.Room.Code, Name = item.Room.Name },
                    // Location = new CodeNameEntity { Id = item.Room.Location.Id, Code = item.Room.Location.Code, Name = item.Room.Location.Name },
                    // Region = new CodeNameEntity { Id = item.Room.Location.Region.Id, Code = item.Room.Location.Region.Code, Name = item.Room.Location.Region.Name },
                    ImageName = entityFiles.Count > 0 ? entityFiles[0].StoredAs : "",
                    Type = assets[k].SubType != null ? new CodeNameEntity { Id = assets[k].SubType.Id, Code = assets[k].SubType.Code } : null
                });

            }




            var itemsByGroupWithCount = assets.GroupBy(i => i.SubType != null ? i.SubType.Code : "")
                 .Select(item => new {
                     Code = item.Key,
                     Total = item.Count(),
                 }).ToList();


            foreach (var item in itemsByGroupWithCount)
            {
                if (!aCats.ContainsKey(item.Code))
                {
                    aCats.Add(item.Code, item.Total);
                    rowSpan++;
                }

            }

            var itemsByGroupWithCountComp = assetComponents.GroupBy(i => i.SubType != null ? i.SubType.Code : "")
                .Select(item => new {
                    Code = item.Key,
                    Total = item.Count(),
                }).ToList();


            foreach (var item in itemsByGroupWithCountComp)
            {
                if (!aCompCats.ContainsKey(item.Code))
                {
                    aCompCats.Add(item.Code, item.Total);
                    rowSpanComp++;
                }

            }
            //document = new Model.Document
            //{
            //    DocumentType = documentType,
            //    DocNo1 = string.Empty,
            //    DocNo2 = string.Empty,
            //    DocumentDate = DateTime.Now,
            //    RegisterDate = DateTime.Now,
            //    Approved = true,
            //    Exported = false,
            //    CreationDate = DateTime.Now,
            //    Details = string.Empty
            //};

            //_context.Set<Model.Document>().Add(document);


            // _context.SaveChanges();

            var subject = "Fisa Lichidare";


            if (emailUIs.Assets.Count > 0)
            {
                for (int i = 0; i < emailUIs.Assets.Count(); i++)
                {


                    if (emailUIs.Assets[i].Id > 0)
                    {

                        //Model.Room roomIni = null;
                        //Model.Location locationIni = null;
                        Model.Employee eIni = null;
                        eIni = _context.Set<Model.Employee>().Include(d => d.Department).Where(e => e.Id == employeeId).SingleOrDefault();


                        if (emailUIs.Assets[i].Room != null)
                        {

                            //roomIni = _context.Set<Model.Room>().Where(a => a.Id == emailUIs.Assets[i].Room.Id).FirstOrDefault();

                            //if (roomIni != null)
                            //{
                            //    locationIni = _context.Set<Model.Location>().Where(a => a.Id == roomIni.LocationId).FirstOrDefault();
                            //}
                        }

                        _context.SaveChanges();

                        if (eIni.Email != null && eIni.Email != "" && !dictCC.ContainsValue(eIni.Email))
                        {
                            dictCC.Add(number, eIni.Email);
                            number++;
                        }

                        if (eIni != null && eIni.Department != null && eIni.Department.Name != null && eIni.Department.Name != "" && !dictCC.ContainsValue(eIni.Department.Name))
                        {
                            dictCC.Add(number, eIni.Department.Name);
                            number++;
                        }


                        if (emailUIs.Assets.Count() > 1)
                        {
                            if (i == 0)
                            {

                                company1 = asset != null ? asset.Company.Name : "Items";

                                htmlBodyCompany1 = htmlBodyCompany1 + @"<tr><th colspan=""5"">" + company1 + "</th></tr>"; ;

                                if (emailUIs.Assets[i].Type != null)
								{
                                    aCats.TryGetValue(emailUIs.Assets[i].Type.Code, out rowSpan);

                                    categoryCode = emailUIs.Assets[i].Type.Code;
								}
								else
								{
                                    emailUIs.Assets[i].Type = new CodeNameEntity();
								}
                               

                                if (emailUIs.Assets[i].ImageName != "")
                                {
                                    htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                            <td rowspan=""" + rowSpan + @""">" + emailUIs.Assets[i].Type.Code + @" </ td >
                                                            <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                             <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                           <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
                                                        </tr>
                                                       
                                        ";
                                }
                                else
                                {

                                    htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                            <td rowspan=""" + rowSpan + @""">" + emailUIs.Assets[i].Type.Code + @" </ td >
                                                            <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                            <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                           <td>" + @" </td>
                                                        </tr>
                                                       
                                        ";
                                }





                            }
                            else
                            {
                                if (emailUIs.Assets[i].Type != null)
								{
                                    aCats.TryGetValue(emailUIs.Assets[i].Type.Code, out rowSpan);
                                }
								else
								{
                                    emailUIs.Assets[i].Type = new CodeNameEntity();
                                }
                                

                                // var lastTypeId = emailUIs.Assets[i].Type.Id;

                                if (categoryCode != emailUIs.Assets[i].Type.Code)
                                {
                                    span = rowSpan;
                                    categoryCode = emailUIs.Assets[i].Type.Code;

                                    if (emailUIs.Assets[i].ImageName != "")
                                    {
                                        htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                             <td rowspan=""" + rowSpan + @""">" + emailUIs.Assets[i].Type.Code + @" </td>
                                                             <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                            <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                            <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
                                                        </tr>
                                                       
                                        ";
                                    }
                                    else
                                    {
                                        htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                             <td rowspan=""" + rowSpan + @""">" + emailUIs.Assets[i].Type.Code + @" </td>
                                                             <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                             <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                            <td>" + @" </td>
                                                        </tr>
                                                       
                                        ";
                                    }

                                }
                                else
                                {
                                    if (emailUIs.Assets[i].ImageName != "")
                                    {
                                        htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                           
                                                             <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                             <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                            <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
                                                        </tr>
                                                       
                                        ";
                                    }
                                    else
                                    {
                                        htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                           
                                                             <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                             <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                            <td>" + @" </td>
                                                        </tr>
                                                       
                                        ";
                                    }

                                }






                            }
                        }
                        else
                        {
                            company1 = asset != null ? asset.Company.Name : "Items";

                            htmlBodyCompany1 = htmlBodyCompany1 + @"<tr><th colspan=""5"">" + company1 + "</th></tr>";

                            if (emailUIs.Assets[i].ImageName != "")
                            {
                                htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                             <td>" + emailUIs.Assets[i].Type.Code + @" </td>
                                                             <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                            <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                            <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
                                                        </tr>
                                                       
                                        ";
                            }
                            else
                            {
                                htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                             <td>" + emailUIs.Assets[i].Type.Code + @" </td>
                                                             <td>" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                            <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
                                                            <td>" + @" </td>
                                                        </tr>
                                                       
                                        ";
                            }




                        }


                    }

                }
            }


            if (emailUIs.AssetComponents.Count > 0)
            {


                for (int i = 0; i < emailUIs.AssetComponents.Count; i++)
                {

                    if (i == 0)
                    {
                        aCompCats.TryGetValue(emailUIs.AssetComponents[i].Type.Code, out rowSpanComp);

                        categoryComponentCode = emailUIs.AssetComponents[i].Type.Code;


                        if (emailUIs.AssetComponents[i].ImageName != "")
                        {

                            htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                            <td rowspan=""" + rowSpanComp + @""">" + emailUIs.AssetComponents[i].Type.Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                         
                                                            <td>" + @" </ td >
                                                           <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.AssetComponents[i].ImageName + @"""  >" + @" </td>
                                                        </tr>
                                                       
                                         ";
                        }
                        else
                        {
                            htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                             <td rowspan=""" + rowSpanComp + @""">" + emailUIs.AssetComponents[i].Type.Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                            <td>" + @" </ td >
                                                           <td>" + @" </td>
                                                        </tr>
                                                       
                                         ";
                        }

                    }
                    else
                    {

                        aCompCats.TryGetValue(emailUIs.AssetComponents[i].Type.Code, out rowSpanComp);

                        if (categoryComponentCode != emailUIs.AssetComponents[i].Type.Code)
                        {

                            spanComp = rowSpanComp;
                            categoryComponentCode = emailUIs.AssetComponents[i].Type.Code;


                            if (emailUIs.AssetComponents[i].ImageName != "")
                            {
                                htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                     
                                                            <td rowspan=""" + rowSpanComp + @""">" + emailUIs.AssetComponents[i].Type.Code + @" </td>
                                                             <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                            <td>" + @" </ td >
                                                            <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.AssetComponents[i].ImageName + @"""  >" + @" </td>
                                                             </tr>
                                                       
                                         ";
                            }
                            else
                            {
                                htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                           
                                                             <td rowspan=""" + rowSpanComp + @""">" + emailUIs.AssetComponents[i].Type.Code + @" </td>
                                                             <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                            <td>" + @" </ td >
                                                            <td>" + @" </td>
                                                        </tr>
                                                       
                                         ";
                            }
                        }
                        else
                        {
                            if (emailUIs.AssetComponents[i].ImageName != "")
                            {
                                htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                           
                                                             <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                            <td>" + @" </ td >
                                                            <td><img style = ""display: block;padding: 0px"" width = ""300"" heigth = ""300""  src = """ + imageLink + emailUIs.AssetComponents[i].ImageName + @"""  >" + @" </td>
                                                        </tr>
                                                       
                                         ";
                            }
                            else
                            {
                                htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                           
                                                           
                                                             <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
                                                            <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                            <td>" + @" </ td >
                                                            <td>" + @" </td>
                                                        </tr>
                                                       
                                         ";
                            }
                        }




                    }


                }
            }

            var end1 = @" </body>
                                        </html>";

            foreach (var item in dictCC)
            {
                cc.Add(item.Value);
            }

            var bodyHtmlOut = htmlBody1 + headerMsg + htmlBody11 + htmlBodyCompany1 + htmlBody2 + htmlBodyEmail1 + htmlBodyEnd + htmlBodyEmailComponent + end + footerMsg + end1;

            return bodyHtmlOut;
        }

        [HttpPost("sendITBookEmail/{id}")]
        public async Task<bool> CheckInventory(int id)
        {
            bool result = false;
            if (id > 0)
            {
                result = await SendNewAssetLinkMail(id);

            }

            return result;

        }

        public async Task<bool> SendNewAssetLinkMail(int employeeId)
        {
            var files = new FormFileCollection();
            Model.EmailType emailTypeAsset = null;
            Model.Asset asset = null;
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            Dictionary<int, string> dictCC = new Dictionary<int, string>();
            Dictionary<int, Guid> guidIds = new Dictionary<int, Guid>();
            Dictionary<string, int> aCats = new Dictionary<string, int>();
            int number = 0;
            var headerMsg = "";
            var htmlBody1 = @"<!DOCTYPE html>
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    Inventariem echipamentele IT
                                                </title>
                         
                                            </head>
                                            <body>         
                                        ";


            emailTypeAsset = _context.Set<Model.EmailType>().Where(d => d.Code == "EMPLOYEE NEW ASSET").Single();
            headerMsg = emailTypeAsset.HeaderMsg;
            // footerMsg = emailTypeAsset.FooterMsg;
            Model.Employee emp = _context.Set<Model.Employee>().AsNoTracking().Where(e => e.Id == employeeId).SingleOrDefault();
            //var employeeLink = "https://service.inventare.ro/SameDayValidate/#/newassetemployee/" + emp.Guid.ToString();
            // var employeeLink = "http://localhost:4200/#/wfh/validate/";
            //var link = @"<h4><span style=""color: rgb(66, 149, 208)"">Pentru introducerea datelor va rugam sa accesati: <a style=""color: red; font-size: 20px;"" href = '" + employeeLink + "'" + "' >Adauga bunuri angajat</a>" + @"</span></h4>";
            //var linkInfo = @"<h4><span style=""color: rgb(66, 149, 208)"">In cazul in care link-ul nu poate fi accesat din Internet Explorer, va rugam sa-l accesati din Google Chrome" + @"</span></h4>";
            //var linkInfo2 = @"<h4><span style=""color: rgb(66, 149, 208)"">Datele tale cu caracter personal vor fi prelucrate in interesul legitim al Sameday de a realiza si mentine o evidenta a bunurilor predate fiecarui angajat iar la aceste date va avea acces, pentru o perioada de 4 ani, compania Optima Group, in calitate de furnizor al aplicatiei Optimal Fixed Assets ." + @"</span></h4>";
            // var GuidAll = Guid.NewGuid();

            if (emp.Email != null && emp.Email != "" && !dictCC.ContainsValue(emp.Email))
            {
                dictCC.Add(number, emp.Email);
                number++;
            }

            foreach (var item in dictCC)
            {
                cc.Add(item.Value);
            }


            var subject = "Inventariem echipamentele IT";



            var end1 = @"  </body>
                                        </html>";

            if (cc.Count == 0)
            {
                cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
			}
			//else
			//{
   //             cc = new List<string>();
   //             cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
   //         }
            //else
            //{
            //	//cc.Add("DragosGabriel.Surcel@bcr.ro");

            //	cc = new List<string>();
            //	cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
            //	//cc.Add("DragosGabriel.Surcel@bcr.ro");
            //	//cc.Add("sorina.nanciu@optima.ro");
            //}


            var bodyHtmlOut = htmlBody1 + headerMsg + end1;
            var messageAttach = new Message(cc, cc, bcc, subject, bodyHtmlOut, null);

            var success = await _emailSender.SendEmailAsync(messageAttach);

            if (success)
            {

                Model.Employee employee = _context.Set<Model.Employee>().Where(e => e.Id == employeeId).SingleOrDefault();

                if (employee != null)
                {
                    emailTypeAsset.NotifyLast = DateTime.Now;
                    // emailTypeAssetComponent.NotifyLast = DateTime.Now;
                    employee.NotifyLast = DateTime.Now;
                    employee.IsEmailSend = true;
                    _context.Update(employee);
                    _context.Update(emailTypeAsset);
                    // _context.Update(emailTypeAssetComponent);
                    _context.SaveChanges();
                }

            }

            return true;


        }

        [HttpGet]
        [Route("inuse", Order = -1)]
        public virtual async Task<IActionResult> GetInUse(string jsonFilter, string propertyFilters)
        {
            AssetFilter assetFilter = null;
            List<PropertyFilter> propFilters = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();
            propFilters = propertyFilters != null ? JsonConvert.DeserializeObject<List<PropertyFilter>>(propertyFilters) : new List<PropertyFilter>();

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    assetFilter.EmployeeIds = null;
                    assetFilter.EmployeeIds = new List<int?>();
                    assetFilter.EmployeeIds.Add(int.Parse("-1"));
                }
            }
            else
            {
                assetFilter.EmployeeIds = null;
                assetFilter.EmployeeIds = new List<int?>();
                assetFilter.EmployeeIds.Add(int.Parse("-1"));
            }

            var items = (_itemsRepository as IEmployeesRepository)
                .GetTransferEmployeesInUseWithAssets(assetFilter, propFilters).ToList();

            return Ok(items);

        }

    }
}
