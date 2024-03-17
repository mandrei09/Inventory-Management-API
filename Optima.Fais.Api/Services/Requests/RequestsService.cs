using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Optima.Fais.Api.Services;
using Optima.Fais.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class RequestsService : IRequestsService
	{
		private readonly ApplicationDbContext _context;
		private readonly IEmailSender _emailSender;

		public RequestsService(ApplicationDbContext context, IEmailSender emailSender)
		{
			this._context = context;
			this._emailSender = emailSender;
		}
        public async Task<bool> SendRequestNeedBudget(int requestId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<int> emails1 = new List<int>();
            List<int> emails2 = new List<int>();
            List<int> emails = new List<int>();
            var files = new FormFileCollection();
            Model.RequestBudgetForecast requestBudgetForecast = null; 
            Model.Request request = null;
            request = await _context.Set<Model.Request>()
                .Include(o => o.AssetType)
                .Include(c => c.Company)
                .Include(d => d.Division).ThenInclude(d => d.Department)
                //.Include(c => c.CostCenter).ThenInclude(d => d.AdmCenter)
                // .Include(c => c.CostCenter).ThenInclude(d => d.Region)
                .Include(c => c.Employee)
                .Include(c => c.ProjectType)
                .Include(c => c.AppState)
                .Include(c => c.Owner)
                .Include(c => c.StartAccMonth)
                .AsNoTracking().Where(a => a.Id == requestId).SingleOrDefaultAsync();
            requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
                .Include(r => r.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(e => e.Employee)
				.Include(r => r.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(e => e.Project)
				.Where(a => a.RequestId == request.Id && a.IsDeleted == false)
                .FirstOrDefaultAsync();
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
                                                          font-size: 10px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 13px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 10px;
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
                                                              font-size: 13px;
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
                                                              font-size: 13px;
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
                                        <th colspan=""9"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">SOLICITARE BUGET NOU</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar Ticket</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Solicitant</th>
                                        <th colspan=""1"">Buget solicitat(RON)</th>
                                        <th colspan=""1"">Luna executie</th>
                                        <th colspan=""1"">Luna implementare</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", request.CreatedAt)}</th>
										<th>{String.Format("{0:MM}", request.CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", request.CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""9""></th>
									</tr>";

            var htmlHeader3 = "";

            var htmlHeader4 = @"<tr>
										<th colspan = ""9""></th>
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

            int index = 0;

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/requestbudgetvalidate/" + request.Guid + "/" + request.Id;
            //var linkNo = "http://OFAUAT/#/requestbudgetnotvalidate/" + request.Guid + "/" + request.Id;

            //var linkYes = "http://OFAAPIUAT/api/requests/requestbudgetvalidate/" + request.Guid + "/" + request.Id;
            var linkYes = "https://optima.emag.network/ofa";
			//var linkNo = "http://OFAUAT/#/requestbudgetnotvalidate/" + request.Guid + "/" + request.Id;



			string empIni = request.Employee != null ? request.Employee.FirstName + " " + request.Employee.LastName : "";

            string owner = request.Owner != null ? request.Owner.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{(request.Employee != null ? request.Employee.Email: "")}</th>
                                                <th rowspan=""2"" colspan=""1"">{String.Format("{0:#,##0.##}", request.BudgetValueNeed)}</th>
                                                <th rowspan=""2"" colspan=""1"">{String.Format("{0:MM/yyyy}", request.EndDate != null ? request.EndDate : request.StartDate)}</th>
                                                <th rowspan=""2"" colspan=""1"">{String.Format("{0:MM/yyyy}", request.StartAccMonth != null ? request.StartAccMonth.Month + "|" + request.StartAccMonth.Year : "")}</th>";



            htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""9"" style=""color: #ffffff;"">Detalii</th>
												</tr>";

            htmlHeader9 = htmlHeader9 + $@"  
												<tr>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Tip proiect</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Referinta BGT</th>
												<th rowspan=""2"" colspan=""1"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + linkYes + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>
												</tr>";

            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
                                                <th >{owner}</th>
												<th >{(request.Division != null && request.Division.Department != null ? (request.Division.Department.Code + "|" + request.Division.Department.Name) : "")}</th>
												<th>{(request.Division != null ? (request.Division.Code + "|" + request.Division.Name) : "")}</th>
                                                <th>{(request.ProjectType != null ? (request.ProjectType.Code + "|" + request.ProjectType.Name) : "")}</th>
                                                <th>{(request.AssetType != null ? (request.AssetType.Code + "|" + request.AssetType.Name) : "")}</th>
                                                <th colspan=""2"">{request.Info}</th></th>
                                                <th>{(requestBudgetForecast != null && requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Code : "")}</th>
												</tr>";


            subject = "Solicitare buget pentru P.R. - ul: " + request.Code + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{request.Code}</th>";


            emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == request.EmployeeId).Select(a => a.Id).ToListAsync();

            if (emails1.Count > 0)
            {
                emails.Add(emails1.ElementAt(0));
            }

            if (requestBudgetForecast != null && requestBudgetForecast.BudgetForecast !=null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.Employee != null)
            {
				emails2 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == requestBudgetForecast.BudgetForecast.BudgetBase.EmployeeId).Select(a => a.Id).ToListAsync();


				if (emails2.Count > 0)
				{
					emails.Add(emails2.ElementAt(0));
				}
			}

			
			for (int e = 0; e < emails.Count; e++)
            {
                var emp = await _context.Set<Model.Employee>().Where(employee => employee.Id == emails.ElementAt(e)).SingleAsync();

                if (emp.Email != null && emp.Email != "")
                {
                    cc.Add(emp.Email);
                }
                else
                {
                    cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
                }
            }

            

            bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
			to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
			to.Add("capex.opex@emag.ro");

#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			cc = new List<string>();
			cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
#endif



			List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.RequestId == requestId && e.IsDeleted == false).ToList();

            List<(string, string, string)> filePaths = new List<(string, string, string)>();

            for (int i = 0; i < entityFiles.Count; i++)
            {
                var filePath = Path.Combine("request", entityFiles[i].StoredAs);
                filePaths.Add(new(filePath, entityFiles[i].Name, entityFiles[i].FileType));
            }

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, filePaths);

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

        public async Task<bool> SendRequestResponseNeedBudget(int requestId, int budgetBaseId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<int> emails1 = new List<int>();
            List<int> emails2 = new List<int>();
            List<int> emails = new List<int>();
            var files = new FormFileCollection();
            Model.Request request = null;
            Model.BudgetBase budgetBase = null;
            request = await _context.Set<Model.Request>()
                .Include(o => o.AssetType)
                .Include(c => c.Company)
                .Include(d => d.Division).ThenInclude(d => d.Department)
                .Include(c => c.Employee)
                .Include(c => c.ProjectType)
                .Include(c => c.AppState)
                .Include(c => c.Owner)
                .Include(c => c.StartAccMonth)
                .AsNoTracking().Where(a => a.Id == requestId).SingleOrDefaultAsync();
            budgetBase = await _context.Set<Model.BudgetBase>()
               .Include(o => o.BudgetForecast)
               .Include(c => c.Project)
               .Include(c => c.AdmCenter)
               .Include(c => c.Region)
               .Include(c => c.Activity)
               .Include(c => c.Country)
               .AsNoTracking().Where(a => a.Id == budgetBaseId).SingleOrDefaultAsync();
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
                                                          width: 80%;
                                                          text-align: center;
                                                          border-collapse: collapse;
                                                        }
                                                        table.redTable td, table.redTable th {
                                                          border: 1px solid #04327d;
                                                          padding: 3px 2px;
                                                        }
                                                        table.redTable tbody td {
                                                          font-size: 10px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 13px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 10px;
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
                                                              font-size: 13px;
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
                                                              font-size: 13px;
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
                                        <th colspan=""10"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">INCARCARE BUGET</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar Ticket</th>
									    <th colspan=""3"">Owner</th>
										<th colspan=""2"">Solicitant</th>
                                        <th colspan=""1"">Buget solicitat(RON)</th>
                                        <th colspan=""1"">Luna executie</th>
                                        <th colspan=""1"">Luna implementare</th>
                                        <th colspan=""1""></th>
									</tr>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = "";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""10""></th>
									</tr>";

            var htmlHeader3 = "";

            var htmlHeader4 = @"<tr>
										<th colspan = ""10""></th>
								</tr>";

            var htmlHeader5 = "";
            var htmlHeader6 = "";
            var htmlHeader7 = "";
            var htmlHeader8 = "";
            var htmlHeader9 = "";
            var htmlHeader10 = "";
            var htmlHeader14 = "";
            var htmlHeader15 = "";
            var htmlHeader16 = "";
            var htmlHeader17 = "";
            var htmlHeader18 = "";
            var htmlHeader19 = "";
            var htmlHeader20 = "";
            var htmlHeader21 = "";
            var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            int index = 0;

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/requestbudgetvalidate/" + request.Guid + "/" + request.Id;
            //var linkNo = "http://OFAUAT/#/requestbudgetnotvalidate/" + request.Guid + "/" + request.Id;

            //var linkYes = "http://OFAAPIUAT/api/requests/requestbudgetvalidate/" + request.Guid + "/" + request.Id;
            var linkYes = "https://optima.emag.network/ofa/#/budget/forecast";
            //var linkNo = "http://OFAUAT/#/requestbudgetnotvalidate/" + request.Guid + "/" + request.Id;



            string empIni = request.Employee != null ? request.Employee.FirstName + " " + request.Employee.LastName : "";

            string owner = request.Owner != null ? request.Owner.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
                                                <th rowspan=""2"" colspan=""3"">{owner}</th>
												<th rowspan=""2"" colspan=""2"">{request.Employee.Email}</th>
                                                <th rowspan=""2"" colspan=""1"">{String.Format("{0:#,##0.##}", request.BudgetValueNeed)}</th>
                                                <th rowspan=""2"" colspan=""1"">{String.Format("{0:MM/yyyy}", request.EndDate != null ? request.EndDate : request.StartDate)}</th>
                                                <th rowspan=""2"" colspan=""1"">{String.Format("{0:MM/yyyy}", request.StartAccMonth != null ? request.StartAccMonth.Month + "|" + request.StartAccMonth.Year : request.StartDate)}</th>
                                                <th rowspan=""2"" colspan=""1"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + linkYes + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";


            htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""9"" style=""color: #ffffff;"">Detalii solicitate</th>
												</tr>";

            htmlHeader9 = htmlHeader9 + $@"  
												<tr>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">WBS</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Tip proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Profit Center</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">PC Detaliu</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Activity</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Tara</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
												</tr>";
            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
                                                <th >{(budgetBase.Project != null ? budgetBase.Project.Code : "")}</th>
												<th >{(request.Division != null && request.Division.Department != null ? request.Division.Department.Code + "|" + request.Division.Department.Name : "")}</th>
												<th>{(request.Division != null ? request.Division.Code + "|" + request.Division.Name : "")}</th>
                                                <th>{(request.ProjectType != null ? request.ProjectType.Code + "|" + request.ProjectType.Name : "")}</th>
                                                <th>{(request.AssetType != null ? request.AssetType.Code + "|" + request.AssetType.Name : "")}</th>
                                                <th>{(budgetBase.AdmCenter != null ? budgetBase.AdmCenter.Code : "")}</th>
                                                <th>{(budgetBase.Region != null ? budgetBase.Region.Code : "")}</th>
                                                <th>{(budgetBase.Activity != null ? budgetBase.Activity.Code : "")}</th>
                                                <th>{(budgetBase.Country != null ? budgetBase.Country.Code : "")}</th>
                                                <th>{request.Info}</th>
												</tr>";

            //htmlHeader14 = htmlHeader14 + $@"     
												//<tr style=""background-color: #04327d;"">
												//<th colspan=""9"" style=""color: #ffffff;"">Detalii incarcate</th>
												//</tr>";

            //htmlHeader15 = htmlHeader15 + $@"  
												//<tr>
            //                                    <th style=""background-color: #6491D9;color: #ffffff;"">WBS</th>
												//<th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												//<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
												//<th style=""background-color: #6491D9;color: #ffffff;"">Proiect</th>
            //                                    <th style=""background-color: #6491D9;color: #ffffff;"">Tip proiect</th>
            //                                    <th style=""background-color: #6491D9;color: #ffffff;"">Profit Center</th>
            //                                    <th style=""background-color: #6491D9;color: #ffffff;"">Activity</th>
            //                                    <th style=""background-color: #6491D9;color: #ffffff;"">Tara</th>
            //                                    <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
												
												//</tr>";

            //htmlHeader16 = htmlHeader16 + $@"    
												//<tr>
            //                                    <th ></th>
												//<th >{request.CostCenter.Division.Department.Code + "|" + request.CostCenter.Division.Department.Name}</th>
												//<th>{request.CostCenter.Division.Code + "|" + request.CostCenter.Division.Name}</th>
            //                                    <th>{request.ProjectType.Code + "|" + request.ProjectType.Name}</th>
            //                                    <th>{request.AssetType.Code + "|" + request.AssetType.Name}</th>
            //                                    <th></th>
            //                                    <th></th>
            //                                    <th></th>
            //                                    <th>{request.Info}</th>
												//</tr>";

            htmlHeader17 = htmlHeader17 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""9"" style=""color: #ffffff;"">Detalii Buget incarcare / luni</th>
												</tr>";

            htmlHeader18 = htmlHeader18 + $@"  
												<tr>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Aprilie</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Mai</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;""></th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Iunie</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Iulie</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">August</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Septembrie</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;""></th>
												</tr>";

            htmlHeader19 = htmlHeader19 + $@"    
												<tr>
                                                <th>{String.Format("{0:#,##0.##}", budgetBase.BudgetForecast.ElementAtOrDefault(0).April)}</th>
												<th>{String.Format("{0:#,##0.##}", budgetBase.BudgetForecast.ElementAtOrDefault(0).May)}</th>
                                                <th></th>
												<th>{String.Format("{0:#,##0.##}", budgetBase.BudgetForecast.ElementAtOrDefault(0).June)}</th>
                                                <th>{String.Format("{0:#,##0.##}", budgetBase.BudgetForecast.ElementAtOrDefault(0).July)}</th>
                                                <th></th>
                                                <th>{String.Format("{0:#,##0.##}", budgetBase.BudgetForecast.ElementAtOrDefault(0).August)}</th>
                                                <th>{String.Format("{0:#,##0.##}", budgetBase.BudgetForecast.ElementAtOrDefault(0).September)}</th>
                                                <th></th>
												</tr>";


            htmlHeader20 = htmlHeader20 + $@"  
												<tr>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Octombrie</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Noiembrie</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;""></th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Decembrie</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Ianuarie</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Februarie</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Martie</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;""></th>
												</tr>";

            htmlHeader21 = htmlHeader21 + $@"    
												<tr>
                                                <th>{String.Format("{0:#,##0.##}", budgetBase.BudgetForecast.ElementAtOrDefault(0).Octomber)}</th>
												<th>{String.Format("{0:#,##0.##}", budgetBase.BudgetForecast.ElementAtOrDefault(0).November)}</th>
                                                <th></th>
												<th>{String.Format("{0:#,##0.##}", budgetBase.BudgetForecast.ElementAtOrDefault(0).December)}</th>
                                                <th>{String.Format("{0:#,##0.##}", budgetBase.BudgetForecast.ElementAtOrDefault(0).January)}</th>
                                                <th></th>
                                                <th>{String.Format("{0:#,##0.##}", budgetBase.BudgetForecast.ElementAtOrDefault(0).February)}</th>
                                                <th>{String.Format("{0:#,##0.##}", budgetBase.BudgetForecast.ElementAtOrDefault(0).March)}</th>
                                                <th></th>
												</tr>";


            subject = "Incarcare buget pentru ticketul: " + request.Code + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{request.Code}</th>";


            emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == request.EmployeeId).Select(a => a.Id).ToList();

            if (emails1.Count > 0)
            {
                emails.Add(emails1.ElementAt(0));
            }

            for (int e = 0; e < emails.Count; e++)
            {
                var emp = _context.Set<Model.Employee>().Where(employee => employee.Id == emails.ElementAt(e)).Single();

                if (emp.Email != null && emp.Email != "")
                {
                    to.Add(emp.Email);
                }
                else
                {
                    to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
                }
            }


			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
			to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
			cc.Add("capex.opex@emag.ro");
#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
			cc = new List<string>();
			cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
#endif
			htmlMessage = 
                htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + 
                htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + 
                htmlHeader14 + htmlHeader15 + htmlHeader16 + htmlHeader17 + htmlHeader18 + htmlHeader19 + 
                htmlHeader20 + htmlHeader21 + htmlBodyEnd;

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


		public async Task<bool> SendRequestBudgetForecastNeedBudget(int requestBudgetForecastId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			List<int> emails1 = new List<int>();
			List<int> emails2 = new List<int>();
			List<int> emails = new List<int>();
			var files = new FormFileCollection();
			Model.RequestBudgetForecast requestBudgetForecast = null;
			requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
				.Include(a => a.Request).ThenInclude(o => o.AssetType)
				.Include(a => a.Request).ThenInclude(c => c.Company)
				.Include(a => a.Request).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .Include(c => c.BudgetForecast).ThenInclude(b=> b.BudgetBase).ThenInclude(p => p.Project)
				//.Include(c => c.CostCenter).ThenInclude(d => d.AdmCenter)
				// .Include(c => c.CostCenter).ThenInclude(d => d.Region)
				.Include(a => a.Request).ThenInclude(c => c.Employee)
				.Include(a => a.Request).ThenInclude(c => c.ProjectType)
				.Include(a => a.Request).ThenInclude(c => c.AppState)
				.Include(a => a.Request).ThenInclude(c => c.Owner)
				.Include(a => a.Request).ThenInclude(c => c.StartAccMonth)
				.AsNoTracking().Where(a => a.Id == requestBudgetForecastId).SingleOrDefaultAsync();
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
                                                          font-size: 10px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 13px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 10px;
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
                                                              font-size: 13px;
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
                                                              font-size: 13px;
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
                                        <th colspan=""9"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">SOLICITARE SUPLIMENTARE BUGET</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar Ticket</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""1"">Solicitant</th>
                                        <th colspan=""1"">Buget initial(RON)</th>
                                        <th colspan=""1"">Buget solicitat(RON)</th>
                                        <th colspan=""1"">Luna executie</th>
                                        <th colspan=""1"">Luna implementare</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", requestBudgetForecast.CreatedAt)}</th>
										<th>{String.Format("{0:MM}", requestBudgetForecast.CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", requestBudgetForecast.CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""9""></th>
									</tr>";

			var htmlHeader3 = "";

			var htmlHeader4 = @"<tr>
										<th colspan = ""9""></th>
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

			int index = 0;

			//var linkYes = "http://OFAAPIUAT/api/budgetbases/requestbudgetvalidate/" + request.Guid + "/" + request.Id;
			//var linkNo = "http://OFAUAT/#/requestbudgetnotvalidate/" + request.Guid + "/" + request.Id;

			//var linkYes = "http://OFAAPIUAT/api/requests/requestbudgetvalidate/" + request.Guid + "/" + request.Id;
			var linkYes = "https://optima.emag.network/ofa";
			//var linkNo = "http://OFAUAT/#/requestbudgetnotvalidate/" + request.Guid + "/" + request.Id;



			string empIni = requestBudgetForecast.Request != null && requestBudgetForecast.Request.Employee != null ? requestBudgetForecast.Request.Employee.Email : "";

			string owner = requestBudgetForecast.Request != null && requestBudgetForecast.Request.Owner != null ? requestBudgetForecast.Request.Owner.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""1"">{(requestBudgetForecast.Request != null && requestBudgetForecast.Request.Employee != null ? requestBudgetForecast.Request.Employee.Email : "")}</th>
                                                <th rowspan=""2"" colspan=""1"">{String.Format("{0:#,##0.##}", requestBudgetForecast.BudgetForecast.TotalRem)}</th>            
                                                <th rowspan=""2"" colspan=""1"">{String.Format("{0:#,##0.##}", requestBudgetForecast.NeedBudgetValue)}</th>
                                                <th rowspan=""2"" colspan=""1"">{String.Format("{0:MM/yyyy}", requestBudgetForecast.Request.EndDate != null ? requestBudgetForecast.Request.EndDate : requestBudgetForecast.Request.StartDate)}</th>
                                                <th rowspan=""2"" colspan=""1"">{String.Format("{0:MM/yyyy}", requestBudgetForecast.Request.StartAccMonth != null ? requestBudgetForecast.Request.StartAccMonth.Month + "|" + requestBudgetForecast.Request.StartAccMonth.Year : "")}</th>";



			htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""9"" style=""color: #ffffff;"">Detalii</th>
												</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
												<tr>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Tip proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Referinta BGT</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">COD WBS</th>
												<th rowspan=""2"" colspan=""1"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + linkYes + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>
												</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
												<tr>
                                                <th >{owner}</th>
												<th >{(requestBudgetForecast.Request != null &&  requestBudgetForecast.Request.Division != null && requestBudgetForecast.Request.Division.Department != null ? requestBudgetForecast.Request.Division.Department.Code + "|" + requestBudgetForecast.Request.Division.Department.Name : "")}</th>
												<th>{(requestBudgetForecast.Request != null && requestBudgetForecast.Request.Division != null ? requestBudgetForecast.Request.Division.Code + "|" + requestBudgetForecast.Request.Division.Name : "")}</th>
                                                <th>{(requestBudgetForecast.Request != null && requestBudgetForecast.Request.ProjectType != null ? requestBudgetForecast.Request.ProjectType.Code + "|" + requestBudgetForecast.Request.ProjectType.Name : "")}</th>
                                                <th>{(requestBudgetForecast.Request != null && requestBudgetForecast.Request.AssetType != null ? requestBudgetForecast.Request.AssetType.Code + "|" + requestBudgetForecast.Request.AssetType.Name : "")}</th>
                                                <th>{(requestBudgetForecast.Request != null ? requestBudgetForecast.Request.Info : "")}</th>
                                                <th>{(requestBudgetForecast.BudgetForecast != null &&  requestBudgetForecast.BudgetForecast.BudgetBase != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Code : "")}</th>
                                                <th>{(requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.Project != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Project.Code : "")}</th>
                                                <th></th>
                                                <th></th>
												</th>
												</tr>";


			subject = "Solicitare buget pentru ticketul: " + requestBudgetForecast.Request.Code + "!!";

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{(requestBudgetForecast.Request != null ? requestBudgetForecast.Request.Code : "")}</th>";


			emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == requestBudgetForecast.Request.EmployeeId).Select(a => a.Id).ToList();

			if (emails1.Count > 0)
			{
				emails.Add(emails1.ElementAt(0));
			}

			for (int e = 0; e < emails.Count; e++)
			{
				var emp = _context.Set<Model.Employee>().Where(employee => employee.Id == emails.ElementAt(e)).Single();

				if (emp.Email != null && emp.Email != "")
				{
					to.Add(emp.Email);
				}
				else
				{
					to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
				}
			}



			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
			to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
			cc.Add("capex.opex@emag.ro");
#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
			cc = new List<string>();
			cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
#endif



			List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.RequestId == requestBudgetForecast.RequestId && e.IsDeleted == false).ToList();

			List<(string, string, string)> filePaths = new List<(string, string, string)>();

			for (int i = 0; i < entityFiles.Count; i++)
			{
				var filePath = Path.Combine("request", entityFiles[i].StoredAs);
				filePaths.Add(new(filePath, entityFiles[i].Name, entityFiles[i].FileType));
			}

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, filePaths);

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

		public async Task<bool> SendRequestTransferBudget(int budgetBaseOpId, string requester)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			List<int> emails1 = new List<int>();
			List<int> emails2 = new List<int>();
			List<int> emails = new List<int>();
			var files = new FormFileCollection();
			Model.BudgetBaseOp budgetBaseOp = null;
			budgetBaseOp = await _context.Set<Model.BudgetBaseOp>()
				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(o => o.AssetType)
				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.Company)
				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
				.Include(a => a.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(p => p.Project)
				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.Employee)
				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.ProjectType)
				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.AppState)

				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(o => o.AssetType)
				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.Company)
				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
				.Include(a => a.BudgetForecastFin).ThenInclude(b => b.BudgetBase).ThenInclude(p => p.Project)
				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.Employee)
				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.ProjectType)
				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.AppState)

				.Include(a => a.Document).ThenInclude(a => a.DocumentType)

				.AsNoTracking().Where(a => a.Id == budgetBaseOpId).SingleOrDefaultAsync();
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
                                                          font-size: 10px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 13px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 10px;
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
                                                              font-size: 13px;
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
                                                              font-size: 13px;
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
                                        <th colspan=""9"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">SOLICITARE TRANSFER BUGET</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Solicitare</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""1"">Solicitant</th>
                                        <th colspan=""1"">Sursa</th>
                                        <th colspan=""1"">Valoare sursa(RON)</th>
                                        <th colspan=""1"">Destinatie</th>
                                        <th colspan=""1"">Valoare destinatie(RON)</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", budgetBaseOp.CreatedAt)}</th>
										<th>{String.Format("{0:MM}", budgetBaseOp.CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", budgetBaseOp.CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""9""></th>
									</tr>";

			var htmlHeader3 = "";

			var htmlHeader4 = @"<tr>
										<th colspan = ""9""></th>
								</tr>";

			var htmlHeader5 = "";
			var htmlHeader6 = "";
			var htmlHeader7 = "";
			var htmlHeader8 = "";
			var htmlHeader9 = "";
			var htmlHeader10 = "";
			var htmlHeader61 = "";
			var htmlHeader71 = "";
			var htmlHeader81 = "";
			var htmlHeader91 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			int index = 0;

			//var linkYes = "http://OFAAPIUAT/api/budgetbases/requestbudgetvalidate/" + request.Guid + "/" + request.Id;
			//var linkNo = "http://OFAUAT/#/requestbudgetnotvalidate/" + request.Guid + "/" + request.Id;

			//var linkYes = "http://OFAAPIUAT/api/requests/requestbudgetvalidate/" + request.Guid + "/" + request.Id;
			var linkYes = "https://optima.emag.network/ofa";
			//var linkNo = "http://OFAUAT/#/requestbudgetnotvalidate/" + request.Guid + "/" + request.Id;



			string empIni = budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.Employee != null ? budgetBaseOp.BudgetForecast.BudgetBase.Employee.Email : "";
			string empFin = budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null && budgetBaseOp.BudgetForecastFin.BudgetBase.Employee != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Employee.Email : "";

			//string owner = budgetBaseOp.Request != null && budgetBaseOp.Request.Owner != null ? budgetBaseOp.Request.Owner.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""1"">{requester}</th>
                                                <th rowspan=""2"" colspan=""1"">{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null ? budgetBaseOp.BudgetForecast.BudgetBase.Code : "")}</th>            
                                                <th rowspan=""2"" colspan=""1"" >{"-" + String.Format("{0:#,##0.##}", (budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null ? Convert.ToDecimal(budgetBaseOp.AprilFin + budgetBaseOp.MayFin + budgetBaseOp.JuneFin + budgetBaseOp.JulyFin + budgetBaseOp.AugustFin + budgetBaseOp.SeptemberFin + budgetBaseOp.OctomberFin + budgetBaseOp.NovemberFin + budgetBaseOp.DecemberFin + budgetBaseOp.JanuaryFin + budgetBaseOp.FebruaryFin + budgetBaseOp.MarchFin) : ""))}</ th >												
                                                <th rowspan=""2"" colspan =""1"">{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Code : "")}</ th >
                                                <th rowspan=""2"" colspan=""1"" >{"+" + String.Format("{0:#,##0.##}", (budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null ? Convert.ToDecimal(budgetBaseOp.AprilFin + budgetBaseOp.MayFin + budgetBaseOp.JuneFin + budgetBaseOp.JulyFin + budgetBaseOp.AugustFin + budgetBaseOp.SeptemberFin + budgetBaseOp.OctomberFin + budgetBaseOp.NovemberFin+ budgetBaseOp.DecemberFin + budgetBaseOp.JanuaryFin + budgetBaseOp.FebruaryFin + budgetBaseOp.MarchFin) : ""))}</th>";


			
			

			htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""9"" style=""color: #ffffff;"">Detalii SURSA</th>
												</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
												<tr>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Tip proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Referinta BGT</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">COD WBS</th>
												<th rowspan=""2"" colspan=""1"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + linkYes + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>
												</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
												<tr>
                                                <th >{empIni}</th>
												<th >{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.Division != null && budgetBaseOp.BudgetForecast.BudgetBase.Division.Department != null ? budgetBaseOp.BudgetForecast.BudgetBase.Division.Department.Code + "|" + budgetBaseOp.BudgetForecast.BudgetBase.Division.Department.Name : "")}</th>
												<th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.Division != null ? budgetBaseOp.BudgetForecast.BudgetBase.Division.Code + "|" + budgetBaseOp.BudgetForecast.BudgetBase.Division.Name : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.ProjectType != null ? budgetBaseOp.BudgetForecast.BudgetBase.ProjectType.Code + "|" + budgetBaseOp.BudgetForecast.BudgetBase.ProjectType.Name : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.AssetType != null ? budgetBaseOp.BudgetForecast.BudgetBase.AssetType.Code + "|" + budgetBaseOp.BudgetForecast.BudgetBase.AssetType.Name : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null ? budgetBaseOp.BudgetForecast.BudgetBase.Info : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null ? budgetBaseOp.BudgetForecast.BudgetBase.Code : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.Project != null ? budgetBaseOp.BudgetForecast.BudgetBase.Project.Code : "")}</th>
                                                <th></th>
                                                <th></th>
												</th>
												</tr>";

			htmlHeader61 = @"      
									<tr>
                                        <th colspan=""9""></th>
									</tr>";


			htmlHeader71 = htmlHeader71 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""9"" style=""color: #ffffff;"">Detalii DESTINATIE</th>
												</tr>";

			htmlHeader81 = htmlHeader81 + $@"  
												<tr>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Tip proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Referinta BGT</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">COD WBS</th>
												<th rowspan=""2"" colspan=""1"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + linkYes + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>
												</tr>";

			htmlHeader91 = htmlHeader91 + $@"    
												<tr>
                                                <th >{empFin}</th>
												<th >{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null && budgetBaseOp.BudgetForecastFin.BudgetBase.Division != null && budgetBaseOp.BudgetForecastFin.BudgetBase.Division.Department != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Division.Department.Code + "|" + budgetBaseOp.BudgetForecastFin.BudgetBase.Division.Department.Name : "")}</th>
												<th>{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null && budgetBaseOp.BudgetForecastFin.BudgetBase.Division != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Division.Code + "|" + budgetBaseOp.BudgetForecastFin.BudgetBase.Division.Name : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null && budgetBaseOp.BudgetForecastFin.BudgetBase.ProjectType != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.ProjectType.Code + "|" + budgetBaseOp.BudgetForecastFin.BudgetBase.ProjectType.Name : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null && budgetBaseOp.BudgetForecastFin.BudgetBase.AssetType != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.AssetType.Code + "|" + budgetBaseOp.BudgetForecastFin.BudgetBase.AssetType.Name : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Info : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Code : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null && budgetBaseOp.BudgetForecastFin.BudgetBase.Project != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Project.Code : "")}</th>
                                                <th></th>
                                                <th></th>
												</th>
												</tr>";




			subject = "Solicitare transfer de la : " + budgetBaseOp.BudgetForecast.BudgetBase.Code + " catre " + budgetBaseOp.BudgetForecastFin.BudgetBase.Code + " !!";

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{(budgetBaseOp.Document.DocumentType != null ? budgetBaseOp.Document.DocumentType.Name : "")}</th>";


			emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == budgetBaseOp.BudgetForecast.BudgetBase.EmployeeId).Select(a => a.Id).ToList();

			if (emails1.Count > 0)
			{
				emails.Add(emails1.ElementAt(0));
			}

			emails2 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == budgetBaseOp.BudgetForecastFin.BudgetBase.EmployeeId).Select(a => a.Id).ToList();

			if (emails2.Count > 0)
			{
				emails.Add(emails2.ElementAt(0));
			}

			for (int e = 0; e < emails.Count; e++)
			{
				var emp = _context.Set<Model.Employee>().Where(employee => employee.Id == emails.ElementAt(e)).Single();

				if (emp.Email != null && emp.Email != "")
				{
					cc.Add(emp.Email);
				}
				else
				{
					cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
				}

                cc.Add(requester);
			}



			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
			to.Add("capex.opex@emag.ro");
#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");

			cc = new List<string>();
			//cc.Add(requester);
#endif



			List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.BudgetBaseOpId == budgetBaseOp.Id && e.IsDeleted == false).ToList();

			List<(string, string, string)> filePaths = new List<(string, string, string)>();

			for (int i = 0; i < entityFiles.Count; i++)
			{
				var filePath = Path.Combine("request", entityFiles[i].StoredAs);
				filePaths.Add(new(filePath, entityFiles[i].Name, entityFiles[i].FileType));
			}

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + 
                htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + 
                htmlHeader61 + htmlHeader71 + htmlHeader81 + htmlHeader91 +
                htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, filePaths);

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

		public async Task<bool> SendRequestResponseTransferBudget(int budgetBaseOpId, string requester)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			List<int> emails1 = new List<int>();
			List<int> emails2 = new List<int>();
			List<int> emails = new List<int>();
			var files = new FormFileCollection();
			Model.BudgetBaseOp budgetBaseOp = null;
			budgetBaseOp = await _context.Set<Model.BudgetBaseOp>()
				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(o => o.AssetType)
				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.Company)
				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
				.Include(a => a.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(p => p.Project)
				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.Employee)
				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.ProjectType)
				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.AppState)

				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(o => o.AssetType)
				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.Company)
				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
				.Include(a => a.BudgetForecastFin).ThenInclude(b => b.BudgetBase).ThenInclude(p => p.Project)
				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.Employee)
				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.ProjectType)
				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.AppState)

				.Include(a => a.Document).ThenInclude(a => a.DocumentType)

				.AsNoTracking().Where(a => a.Id == budgetBaseOpId).SingleOrDefaultAsync();
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
                                                          font-size: 10px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 13px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 10px;
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
                                                              font-size: 13px;
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
                                                              font-size: 13px;
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
                                        <th colspan=""9"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">VALIDARE TRANSFER BUGET</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Solicitare</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""1"">Solicitant</th>
                                        <th colspan=""1"">Sursa</th>
                                        <th colspan=""1"">Valoare sursa(RON)</th>
                                        <th colspan=""1"">Destinatie</th>
                                        <th colspan=""1"">Valoare destinatie(RON)</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", budgetBaseOp.CreatedAt)}</th>
										<th>{String.Format("{0:MM}", budgetBaseOp.CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", budgetBaseOp.CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""9""></th>
									</tr>";

			var htmlHeader3 = "";

			var htmlHeader4 = @"<tr>
										<th colspan = ""9""></th>
								</tr>";

			var htmlHeader5 = "";
			var htmlHeader6 = "";
			var htmlHeader7 = "";
			var htmlHeader8 = "";
			var htmlHeader9 = "";
			var htmlHeader10 = "";
			var htmlHeader61 = "";
			var htmlHeader71 = "";
			var htmlHeader81 = "";
			var htmlHeader91 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			int index = 0;

			//var linkYes = "http://OFAAPIUAT/api/budgetbases/requestbudgetvalidate/" + request.Guid + "/" + request.Id;
			//var linkNo = "http://OFAUAT/#/requestbudgetnotvalidate/" + request.Guid + "/" + request.Id;

			//var linkYes = "http://OFAAPIUAT/api/requests/requestbudgetvalidate/" + request.Guid + "/" + request.Id;
			var linkYes = "https://optima.emag.network/ofa";
			//var linkNo = "http://OFAUAT/#/requestbudgetnotvalidate/" + request.Guid + "/" + request.Id;



			string empIni = budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.Employee != null ? budgetBaseOp.BudgetForecast.BudgetBase.Employee.Email : "";
			string empFin = budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null && budgetBaseOp.BudgetForecastFin.BudgetBase.Employee != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Employee.Email : "";

			//string owner = budgetBaseOp.Request != null && budgetBaseOp.Request.Owner != null ? budgetBaseOp.Request.Owner.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""1"">{requester}</th>
                                                <th rowspan=""2"" colspan=""1"">{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null ? budgetBaseOp.BudgetForecast.BudgetBase.Code : "")}</th>            
                                                <th rowspan=""2"" colspan=""1"" >{"-" + String.Format("{0:#,##0.##}", (budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null ? Convert.ToDecimal(budgetBaseOp.AprilFin + budgetBaseOp.MayFin + budgetBaseOp.JuneFin + budgetBaseOp.JulyFin + budgetBaseOp.AugustFin + budgetBaseOp.SeptemberFin + budgetBaseOp.OctomberFin + budgetBaseOp.NovemberFin + budgetBaseOp.DecemberFin + budgetBaseOp.JanuaryFin + budgetBaseOp.FebruaryFin + budgetBaseOp.MarchFin) : ""))}</ th >												
                                                <th rowspan=""2"" colspan =""1"">{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Code : "")}</ th >
                                                <th rowspan=""2"" colspan=""1"" >{"+" + String.Format("{0:#,##0.##}", (budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null ? Convert.ToDecimal(budgetBaseOp.AprilFin + budgetBaseOp.MayFin + budgetBaseOp.JuneFin + budgetBaseOp.JulyFin + budgetBaseOp.AugustFin + budgetBaseOp.SeptemberFin + budgetBaseOp.OctomberFin + budgetBaseOp.NovemberFin + budgetBaseOp.DecemberFin + budgetBaseOp.JanuaryFin + budgetBaseOp.FebruaryFin + budgetBaseOp.MarchFin) : ""))}</th>";





			htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""9"" style=""color: #ffffff;"">Detalii SURSA</th>
												</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
												<tr>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Tip proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Referinta BGT</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">COD WBS</th>
												<th rowspan=""2"" colspan=""1"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + linkYes + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>
												</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
												<tr>
                                                <th >{empIni}</th>
												<th >{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.Division != null && budgetBaseOp.BudgetForecast.BudgetBase.Division.Department != null ? budgetBaseOp.BudgetForecast.BudgetBase.Division.Department.Code + "|" + budgetBaseOp.BudgetForecast.BudgetBase.Division.Department.Name : "")}</th>
												<th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.Division != null ? budgetBaseOp.BudgetForecast.BudgetBase.Division.Code + "|" + budgetBaseOp.BudgetForecast.BudgetBase.Division.Name : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.ProjectType != null ? budgetBaseOp.BudgetForecast.BudgetBase.ProjectType.Code + "|" + budgetBaseOp.BudgetForecast.BudgetBase.ProjectType.Name : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.AssetType != null ? budgetBaseOp.BudgetForecast.BudgetBase.AssetType.Code + "|" + budgetBaseOp.BudgetForecast.BudgetBase.AssetType.Name : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null ? budgetBaseOp.BudgetForecast.BudgetBase.Info : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null ? budgetBaseOp.BudgetForecast.BudgetBase.Code : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.Project != null ? budgetBaseOp.BudgetForecast.BudgetBase.Project.Code : "")}</th>
                                                <th></th>
                                                <th></th>
												</th>
												</tr>";

			htmlHeader61 = @"      
									<tr>
                                        <th colspan=""9""></th>
									</tr>";


			htmlHeader71 = htmlHeader71 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""9"" style=""color: #ffffff;"">Detalii DESTINATIE</th>
												</tr>";

			htmlHeader81 = htmlHeader81 + $@"  
												<tr>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Tip proiect</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Referinta BGT</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">COD WBS</th>
												<th rowspan=""2"" colspan=""1"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + linkYes + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>
												</tr>";

			htmlHeader91 = htmlHeader91 + $@"    
												<tr>
                                                <th >{empFin}</th>
												<th >{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null && budgetBaseOp.BudgetForecastFin.BudgetBase.Division != null && budgetBaseOp.BudgetForecastFin.BudgetBase.Division.Department != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Division.Department.Code + "|" + budgetBaseOp.BudgetForecastFin.BudgetBase.Division.Department.Name : "")}</th>
												<th>{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null && budgetBaseOp.BudgetForecastFin.BudgetBase.Division != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Division.Code + "|" + budgetBaseOp.BudgetForecastFin.BudgetBase.Division.Name : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null && budgetBaseOp.BudgetForecastFin.BudgetBase.ProjectType != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.ProjectType.Code + "|" + budgetBaseOp.BudgetForecastFin.BudgetBase.ProjectType.Name : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null && budgetBaseOp.BudgetForecastFin.BudgetBase.AssetType != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.AssetType.Code + "|" + budgetBaseOp.BudgetForecastFin.BudgetBase.AssetType.Name : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Info : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Code : "")}</th>
                                                <th>{(budgetBaseOp.BudgetForecastFin != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null && budgetBaseOp.BudgetForecastFin.BudgetBase.Project != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Project.Code : "")}</th>
                                                <th></th>
                                                <th></th>
												</th>
												</tr>";




			subject = "Transfer buget validat de la : " + budgetBaseOp.BudgetForecast.BudgetBase.Code + " catre " + budgetBaseOp.BudgetForecastFin.BudgetBase.Code + " !!";

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{(budgetBaseOp.Document.DocumentType != null ? budgetBaseOp.Document.DocumentType.Name : "")}</th>";


			emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == budgetBaseOp.BudgetForecast.BudgetBase.EmployeeId).Select(a => a.Id).ToList();

			if (emails1.Count > 0)
			{
				emails.Add(emails1.ElementAt(0));
			}

			emails2 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == budgetBaseOp.BudgetForecastFin.BudgetBase.EmployeeId).Select(a => a.Id).ToList();

			if (emails2.Count > 0)
			{
				emails.Add(emails2.ElementAt(0));
			}

			for (int e = 0; e < emails.Count; e++)
			{
				var emp = _context.Set<Model.Employee>().Where(employee => employee.Id == emails.ElementAt(e)).Single();

				if (emp.Email != null && emp.Email != "")
				{
					cc.Add(emp.Email);
				}
				else
				{
					cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
				}

				cc.Add(requester);
			}



			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
			to.Add("capex.opex@emag.ro");
#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");

			cc = new List<string>();
			//cc.Add(requester);
#endif



			List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.BudgetBaseOpId == budgetBaseOp.Id && e.IsDeleted == false).ToList();

			List<(string, string, string)> filePaths = new List<(string, string, string)>();

			for (int i = 0; i < entityFiles.Count; i++)
			{
				var filePath = Path.Combine("request", entityFiles[i].StoredAs);
				filePaths.Add(new(filePath, entityFiles[i].Name, entityFiles[i].FileType));
			}

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 +
				htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 +
				htmlHeader61 + htmlHeader71 + htmlHeader81 + htmlHeader91 +
				htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, filePaths);

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

//		public async Task<bool> SendRequestResponseTransferBudget(int budgetBaseOpId, string requester)
//		{
//			bool successEmail = false;
//			List<string> to = new List<string>();
//			List<string> cc = new List<string>();
//			List<string> bcc = new List<string>();
//			List<int> emails1 = new List<int>();
//			List<int> emails2 = new List<int>();
//			List<int> emails = new List<int>();
//			var files = new FormFileCollection();
//			Model.BudgetBaseOp budgetBaseOp = null;
//			budgetBaseOp = await _context.Set<Model.BudgetBaseOp>()
//				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(o => o.AssetType)
//				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.Company)
//				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
//				.Include(a => a.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(p => p.Project)
//				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.Employee)
//				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.ProjectType)
//				.Include(a => a.BudgetForecast).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.AppState)

//				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(o => o.AssetType)
//				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.Company)
//				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
//				.Include(a => a.BudgetForecastFin).ThenInclude(b => b.BudgetBase).ThenInclude(p => p.Project)
//				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.Employee)
//				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.ProjectType)
//				.Include(a => a.BudgetForecastFin).ThenInclude(a => a.BudgetBase).ThenInclude(c => c.AppState)

//				.Include(a => a.Document).ThenInclude(a => a.DocumentType)

//				.AsNoTracking().Where(a => a.Id == budgetBaseOpId).SingleOrDefaultAsync();
//			var htmlMessage = "";
//			var subject = "";
//			var htmlBodyEmail1 = "";
//			var htmlBodyEnd = "";
//			var htmlBodyCompany1 = "";
//			var htmlBodyCompany2 = "";
//			var htmlBody1 = @"
//                                        <html lang=""en"">
//                                            <head>    
//                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
//                                                <title>
//                                                    OPTIMA
//                                                </title>
//                                                <style type=""text/css"">
//                                                    table.redTable {
//                                                          border: 2px solid #04327d;
//                                                          background-color: #FFFFFF;
//                                                          width: 100%;
//                                                          text-align: center;
//                                                          border-collapse: collapse;
//                                                        }
//                                                        table.redTable td, table.redTable th {
//                                                          border: 1px solid #04327d;
//                                                          padding: 3px 2px;
//                                                        }
//                                                        table.redTable tbody td {
//                                                          font-size: 10px;
//                                                        }
//                                                        table.redTable tr:nth-child(even) {
//                                                          background: #F5C8BF;
//                                                        }
//                                                        table.redTable thead {
//                                                          background: #ffffff;
//                                                        }
//                                                        table.redTable thead th {
//                                                          font-size: 13px;
//                                                          font-weight: bold;
//                                                          color: #04327d;
//                                                          text-align: center;
//                                                          border-left: 2px solid #04327d;
//                                                        }

//                                                        table.redTable tfoot {
//                                                          font-size: 10px;
//                                                          font-weight: bold;
//                                                          color: #04327d;
//                                                          background: #ffffff;
//                                                        }
//                                                        table.redTable tfoot td {
//                                                          font-size: 10px;
//                                                        }
//                                                        table.redTable tfoot .links {
//                                                          text-align: right;
//                                                        }
//                                                        table.redTable tfoot .links a{
//                                                          display: inline-block;
//                                                          background: #FFFFFF;
//                                                          color: #04327d;
//                                                          padding: 2px 8px;
//                                                          border-radius: 5px;
//                                                        }
//													.button {
//                                                              background-color: #04327d;
//                                                              border: none;
//                                                              color: #ffffff;
//                                                              padding: 5px 10px;
//                                                              text-align: center;
//                                                              text-decoration: none;
//                                                              display: inline-block;
//                                                              font-size: 13px;
//                                                              margin: 4px 2px;
//                                                              cursor: pointer;
//                                                            }
//                                                   .button-no {
//                                                              background-color: #6491D9;
//                                                              border: none;
//                                                              color: white;
//                                                              padding: 5px 10px;
//                                                              text-align: center;
//                                                              text-decoration: none;
//                                                              display: inline-block;
//                                                              font-size: 13px;
//                                                              margin: 4px 2px;
//                                                              cursor: pointer;
//                                                            }
//                                                </style>
//                                            </head>
//                                            <body>
//                                                <table class=""redTable"">
//                                                    <thead>
                                                    
//                                        ";
//			var htmlHeader = @"        
//                                    <tr style=""background-color: #04327d;font-color: #ffffff; font-weight: bold"">
//                                        <th colspan=""9"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">TRANSFER BUGET VALIDAT</th>                                      
//									</tr>";

//			var htmlHeader1 = $@"        
//                                    <tr>
//                                        <th rowspan=""2"">Solicitare</th>
//									    <th colspan=""3"">Data solicitare</th>
//										<th colspan=""1"">Solicitant</th>
//                                        <th colspan=""2"">Buget sursa(RON)</th>
//                                        <th colspan=""2"">Buget destinatie(RON)</th>
//									</tr>
//									<tr>
//                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
//									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
//										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

//			var htmlHeader11 = "";

//			var htmlHeader12 = "";
//			var htmlHeader13 = $@"        
//									    <th>{String.Format("{0:dd}", budgetBaseOp.CreatedAt)}</th>
//										<th>{String.Format("{0:MM}", budgetBaseOp.CreatedAt)}</th>
//										<th>{String.Format("{0:yyyy}", budgetBaseOp.CreatedAt)}</th>
//									</tr>";

//			var htmlHeader2 = @"      
//									<tr>
//                                        <th colspan=""9""></th>
//									</tr>";

//			var htmlHeader3 = "";

//			var htmlHeader4 = @"<tr>
//										<th colspan = ""9""></th>
//								</tr>";

//			var htmlHeader5 = "";
//			var htmlHeader6 = "";
//			var htmlHeader7 = "";
//			var htmlHeader8 = "";
//			var htmlHeader9 = "";
//			var htmlHeader10 = "";

//			var htmlHeaderEnd = @"
//                                </thead>
//                                <tbody>";

//			int index = 0;

//			//var linkYes = "http://OFAAPIUAT/api/budgetbases/requestbudgetvalidate/" + request.Guid + "/" + request.Id;
//			//var linkNo = "http://OFAUAT/#/requestbudgetnotvalidate/" + request.Guid + "/" + request.Id;

//			//var linkYes = "http://OFAAPIUAT/api/requests/requestbudgetvalidate/" + request.Guid + "/" + request.Id;
//			var linkYes = "https://optima.emag.network/ofa";
//			//var linkNo = "http://OFAUAT/#/requestbudgetnotvalidate/" + request.Guid + "/" + request.Id;



//			string empIni = budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.Employee != null ? budgetBaseOp.BudgetForecast.BudgetBase.Employee.Email : "";

//			//string owner = budgetBaseOp.Request != null && budgetBaseOp.Request.Owner != null ? budgetBaseOp.Request.Owner.Email : "";


//			htmlHeader11 = htmlHeader11 + $@"        
//												<th rowspan=""2"" colspan=""1"">{requester}</th>
//                                                <th rowspan=""2"" colspan=""2"">{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null ? budgetBaseOp.BudgetForecast.BudgetBase.Code : "")}</th>            
//                                                <th rowspan=""2"" colspan=""2"">{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecastFin.BudgetBase != null ? budgetBaseOp.BudgetForecastFin.BudgetBase.Code : "")}</th>";



//			htmlHeader8 = htmlHeader8 + $@"     
//												<tr style=""background-color: #04327d;"">
//												<th colspan=""9"" style=""color: #ffffff;"">Detalii</th>
//												</tr>";

//			htmlHeader9 = htmlHeader9 + $@"  
//												<tr>
//                                                <th style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
//												<th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
//												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
//												<th style=""background-color: #6491D9;color: #ffffff;"">Proiect</th>
//                                                <th style=""background-color: #6491D9;color: #ffffff;"">Tip proiect</th>
//                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
//                                                <th style=""background-color: #6491D9;color: #ffffff;"">Referinta BGT</th>
//                                                <th style=""background-color: #6491D9;color: #ffffff;"">COD WBS</th>
//												<th rowspan=""2"" colspan=""1"">
//                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + linkYes + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
//												</th>
//												</tr>";

//			htmlHeader10 = htmlHeader10 + $@"    
//												<tr>
//                                                <th >{empIni}</th>
//												<th >{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.Division != null && budgetBaseOp.BudgetForecast.BudgetBase.Division.Department != null ? budgetBaseOp.BudgetForecast.BudgetBase.Division.Department.Code + "|" + budgetBaseOp.BudgetForecast.BudgetBase.Division.Department.Name : "")}</th>
//												<th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.Division != null ? budgetBaseOp.BudgetForecast.BudgetBase.Division.Code + "|" + budgetBaseOp.BudgetForecast.BudgetBase.Division.Name : "")}</th>
//                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.ProjectType != null ? budgetBaseOp.BudgetForecast.BudgetBase.ProjectType.Code + "|" + budgetBaseOp.BudgetForecast.BudgetBase.ProjectType.Name : "")}</th>
//                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.AssetType != null ? budgetBaseOp.BudgetForecast.BudgetBase.AssetType.Code + "|" + budgetBaseOp.BudgetForecast.BudgetBase.AssetType.Name : "")}</th>
//                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null ? budgetBaseOp.BudgetForecast.BudgetBase.Info : "")}</th>
//                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null ? budgetBaseOp.BudgetForecast.BudgetBase.Code : "")}</th>
//                                                <th>{(budgetBaseOp.BudgetForecast != null && budgetBaseOp.BudgetForecast.BudgetBase != null && budgetBaseOp.BudgetForecast.BudgetBase.Project != null ? budgetBaseOp.BudgetForecast.BudgetBase.Project.Code : "")}</th>
//                                                <th></th>
//                                                <th></th>
//												</th>
//												</tr>";


//			subject = "Transfer buget validat de la : " + budgetBaseOp.BudgetForecast.BudgetBase.Code + " catre " + budgetBaseOp.BudgetForecastFin.BudgetBase.Code + " !!";

//			htmlHeader12 = htmlHeader12 + $@"        
//							</tr>
//							<tr>
//                                <th>{(budgetBaseOp.Document.DocumentType != null ? budgetBaseOp.Document.DocumentType.Name : "")}</th>";


//			emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == budgetBaseOp.BudgetForecast.BudgetBase.EmployeeId).Select(a => a.Id).ToList();

//			if (emails1.Count > 0)
//			{
//				emails.Add(emails1.ElementAt(0));
//			}

//			emails2 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == budgetBaseOp.BudgetForecastFin.BudgetBase.EmployeeId).Select(a => a.Id).ToList();

//			if (emails2.Count > 0)
//			{
//				emails.Add(emails2.ElementAt(0));
//			}

//			for (int e = 0; e < emails.Count; e++)
//			{
//				var emp = _context.Set<Model.Employee>().Where(employee => employee.Id == emails.ElementAt(e)).Single();

//				if (emp.Email != null && emp.Email != "")
//				{
//					cc.Add(emp.Email);
//				}
//				else
//				{
//					cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
//				}
//			}



//			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

//			to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
//			to.Add("capex.opex@emag.ro");
//#if DEBUG
//			to = new List<string>();
//			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
//			//to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");

//			cc = new List<string>();
//#endif



//			List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.BudgetBaseOpId == budgetBaseOp.Id && e.IsDeleted == false).ToList();

//			List<(string, string, string)> filePaths = new List<(string, string, string)>();

//			for (int i = 0; i < entityFiles.Count; i++)
//			{
//				var filePath = Path.Combine("request", entityFiles[i].StoredAs);
//				filePaths.Add(new(filePath, entityFiles[i].Name, entityFiles[i].FileType));
//			}

//			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

//			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, filePaths);

//			successEmail = await _emailSender.SendEmailAsync(emailMessage);

//			to = new List<string>();

//			if (successEmail)
//			{
//				return true;
//			}
//			else
//			{
//				return false;
//			}
//		}
	}
}
