using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MigraDoc.Rendering;
using Optima.Fais.Api.Services;
using Optima.Fais.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class EmailStatusService : IEmailStatusService
    {
		private readonly ApplicationDbContext _context;
		private readonly IEmailSender _emailSender;
        private IAppendix1Generator _appendix1Generator = null;
        public IServiceProvider _services { get; }

        private readonly string _basePath;
        private readonly string _resourcesFolder;
        private readonly string _resourcesPath;

        public EmailStatusService(ApplicationDbContext context, IEmailSender emailSender, IConfiguration configuration, IAppendix1Generator appendix1Generator, IServiceProvider services)
		{
			this._context = context;
			this._emailSender = emailSender;
            _services = services;
            _appendix1Generator = appendix1Generator;
            this._basePath = configuration.GetSection("BasePath").GetValue<string>("Base");
            this._resourcesFolder = configuration.GetSection("BasePath").GetValue<string>("Resources");
            this._resourcesPath = $"{this._basePath}{this._resourcesFolder}";
        }
        public async Task<bool> SendDstEmployeeNotification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<int> emails1 = new List<int>();
            List<int> emails2 = new List<int>();
            List<int> emails = new List<int>();
            var files = new FormFileCollection();
            List<Model.EmailStatus> emailStatus = new List<Model.EmailStatus>();
            emailStatus = await _context.Set<Model.EmailStatus>()
                .Include(o => o.Asset)
                .Include(e => e.EmployeeInitial).ThenInclude(c => c.CostCenter).ThenInclude(c => c.Division).ThenInclude(c => c.Department)
				.Include(e => e.EmployeeFinal).ThenInclude(c => c.CostCenter).ThenInclude(c => c.Division).ThenInclude(c => c.Department)
				.Include(e => e.CostCenterInitial).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .Include(e => e.CostCenterFinal).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotDstEmployeeSync == true && a.DstEmployeeEmailSend == false && a.DocumentNumber == documentNumberId).ToListAsync();
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
                                                          font-size: 9px;;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 12px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 9px;;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 9px;;
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
                                                              font-size: 12px;
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
                                                              font-size: 12px;
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
                                        <th colspan=""8"" style=""color: #ffffff; font-weight: bold;font-size: 1.0em;"">TRANSFER NOU</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar document</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Predator</th>
                                        <th colspan=""2"">Primitor</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:MM}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", emailStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""8""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Nr. inventar</th>
										<th colspan = ""2"" style=""color: #ffffff;"">Descriere</th>
										<th style=""color: #ffffff;"">Data achizitie</th>
										<th style=""color: #ffffff;"">Serie</th>
                                        <th style=""color: #ffffff;"">Cantitate</th>
										<th style=""color: #ffffff;"">Valoare</th>
									</tr>";

            var htmlHeader3 = "";

            var htmlHeader4 = @"<tr>
										<th colspan = ""8""></th>
								</tr>";

            var htmlHeader5 = "";
            var htmlHeader6 = "";
            var htmlHeader7 = "";
            var htmlHeader8 = "";
            var htmlHeader9 = "";
            var htmlHeader10 = "";
			var htmlHeader30 = "";
			var htmlHeader35 = "";
			var htmlHeader40 = "";
			var htmlHeader50 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            int index = 0;

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

			//var linkYes = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeevalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;
            //var linkNo = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeenotvalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;

            var link = "https://optima.emag.network/ofa";



            string empIni = emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.FirstName + " " + emailStatus[0].EmployeeInitial.LastName : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.Email : "")}</th>
                                                <th rowspan=""2"" colspan=""2"">{(emailStatus[0].EmployeeFinal.Email)}</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;text-align: left;"">DETALII PREDATOR</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;""></th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th>{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.InternalCode : "")}</th>
												<th>{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.FirstName : "")}</th>
												<th>{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.LastName :"")}</th>
												<th>{(emailStatus[0].EmployeeInitial != null && emailStatus[0].EmployeeInitial.CostCenter != null ? emailStatus[0].EmployeeInitial.CostCenter.Code :"")}</th>
                                                <th>{(emailStatus[0].EmployeeInitial != null && emailStatus[0].EmployeeInitial.CostCenter != null && emailStatus[0].EmployeeInitial.CostCenter.Division != null ? emailStatus[0].EmployeeInitial.CostCenter.Division.Name : "")}</th>
                                                <th>{(emailStatus[0].EmployeeInitial != null && emailStatus[0].EmployeeInitial.CostCenter != null && emailStatus[0].EmployeeInitial.CostCenter.Division != null && emailStatus[0].EmployeeInitial.CostCenter.Division.Department != null ? emailStatus[0].EmployeeInitial.CostCenter.Division.Department.Name : "")}</th>
                                                <th colspan=""2""></th>
												</tr>";

            htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;text-align: left;"">DETALII PRIMITOR</th>
												</tr>";

            htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;""></th>
												</tr>";

            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th>{emailStatus[0].EmployeeFinal.InternalCode}</th>
												<th>{emailStatus[0].EmployeeFinal.FirstName}</th>
												<th>{emailStatus[0].EmployeeFinal.LastName}</th>
												<th>{(emailStatus[0].EmployeeFinal.CostCenter != null ? emailStatus[0].EmployeeFinal.CostCenter.Code : "")}</th>
                                                <th>{(emailStatus[0].EmployeeFinal.CostCenter != null && emailStatus[0].EmployeeFinal.CostCenter.Division != null ? emailStatus[0].EmployeeFinal.CostCenter.Division.Name : "")}</th>
                                                <th>{(emailStatus[0].EmployeeFinal.CostCenter != null && emailStatus[0].EmployeeFinal.CostCenter.Division != null && emailStatus[0].EmployeeFinal.CostCenter.Division.Department != null ? emailStatus[0].EmployeeFinal.CostCenter.Division.Department.Name : "")}</th>
                                                <th colspan=""2""></th>
												</tr>";

			htmlHeader30 = htmlHeader30 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;text-align: left;"">DETALII TRANSFER</th>
												</tr>";

			htmlHeader35 = htmlHeader35 + $@"     
												<tr>
												<th colspan=""3"" style=""background-color: #6491D9;color: #ffffff;"">ACTUAL</th>
                                                <th colspan=""3"" style=""background-color: #6491D9;color: #ffffff;"">DESTINATIE</th>
												</tr>";

			htmlHeader40 = htmlHeader40 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th rowspan=""2"" colspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >VALIDEAZA TRANSFERUL</a>" + "&nbsp;" + @"
												</th>
												</tr>";

			htmlHeader50 = htmlHeader50 + $@"    
												<tr>
                                                <th>{emailStatus[0].CostCenterInitial.Code}</th>
                                                <th>{(emailStatus[0].CostCenterInitial.Division != null ? emailStatus[0].CostCenterInitial.Division.Name : "")}</th>
                                                <th>{(emailStatus[0].CostCenterInitial.Division != null && emailStatus[0].CostCenterInitial.Division.Department != null ? emailStatus[0].CostCenterInitial.Division.Department.Name : "")}</th>
												<th>{(emailStatus[0].CostCenterFinal.Code)}</th>
												<th>{(emailStatus[0].CostCenterFinal != null && emailStatus[0].CostCenterFinal.Division != null ? emailStatus[0].CostCenterFinal.Division.Name : "")}</th>
												<th>{(emailStatus[0].CostCenterFinal != null && emailStatus[0].CostCenterFinal.Division != null && emailStatus[0].CostCenterFinal.Division.Department != null ? emailStatus[0].CostCenterFinal.Division.Department.Name: "")}</th>
												</tr>";


			subject = "Solicitare validare transfer numarul " + emailStatus[0].DocumentNumber + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{emailStatus[0].DocumentNumber}</th>";



            for (int i = 0; i < emailStatus.Count; i++)
            {
                index++;
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{emailStatus[i].Asset.InvNo + "|" + emailStatus[i].Asset.SubNo}</th>
									<th colspan=""2"">{emailStatus[i].Asset.Name}</th>
									<th>{String.Format("{0:dd/MM/yyyy}", emailStatus[i].Asset.PurchaseDate)}</th>
                                    <th>{emailStatus[i].Asset.SerialNumber}</th>
                                    <th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.Quantity)}</th>
									<th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.ValueInv)}</th>
								</tr>";
                if (index == emailStatus.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{emailStatus.Sum(a => a.Asset.Quantity)}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", emailStatus.Sum(a => a.Asset.ValueInv))}</th>
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


            emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdFinal).Select(a => a.Id).ToListAsync();

            if (emails1.Count > 0)
            {
                emails.Add(emails1.ElementAt(0));
            }

            for (int e = 0; e < emails.Count; e++)
            {
                var emp = await _context.Set<Model.Employee>().Where(employee => employee.Id == emails.ElementAt(e)).SingleAsync();

                if (emp.Email != null && emp.Email != "")
                {
                    to.Add(emp.Email);
                }
                else
                {
                    to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
					to.Add("cosmina.pricop@optima.ro");
				}
            }

            //to = new List<string>();
            bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
			bcc.Add("cosmina.pricop@optima.ro");
			//to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
			//to.Add("silvia.damian@emag.ro");

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + 
                htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + 
                htmlHeader30 + htmlHeader35 + htmlHeader40 + htmlHeader50 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
				for (int i = 0; i < emailStatus.Count; i++)
				{
                    //Model.Employee employee = _context.Set<Model.Employee>().Where(e => e.Id == )
                    emailStatus[i].NotDstEmployeeSync = false;
                    emailStatus[i].DstEmployeeEmailSend = true;
                    //emailStatus[i].NotDstManagerSync = true;
                    //emailStatus[i].DstEmployeeValidateAt = DateTime.Now;
                    //emailStatus[i].DstEmployeeValidateBy = _context.UserId;

                    _context.Update(emailStatus[i]);
                    _context.SaveChanges();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendDstEmployeeReminder1Notification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            var files = new FormFileCollection();
            List<Model.EmailStatus> emailStatus = new List<Model.EmailStatus>();
            DateTime reminderDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0).AddDays(1);
            emailStatus = await _context.Set<Model.EmailStatus>()
                .Include(o => o.Asset)
                .Include(e => e.EmployeeInitial)
                .Include(e => e.EmployeeFinal)
                .Include(e => e.CostCenterInitial).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .Include(e => e.CostCenterFinal).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .AsNoTracking()
                .Where(com => 
                com.IsDeleted == false && 
                com.NotDstEmployeeSync == false && 
                com.DstEmployeeEmailSend == true && 
                com.DstEmployeeReminder1EmailSend == false && 
                (reminderDate < DateTime.Now) && 
                com.SyncErrorCount < 3 && 
                com.DocumentNumber == documentNumberId)
                .ToListAsync();
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
                                                          width: 0%;
                                                          text-align: center;
                                                          border-collapse: collapse;
                                                        }
                                                        table.redTable td, table.redTable th {
                                                          border: 1px solid #04327d;
                                                          padding: 3px 2px;
                                                        }
                                                        table.redTable tbody td {
                                                          font-size: 9px;;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 12px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 9px;;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 9px;;
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
                                                              font-size: 12px;
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
                                                              font-size: 12px;
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
                                        <th colspan=""8"" style=""color: #ffffff; font-weight: bold;font-size: 1.0em;"">TRANSFER NOU</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar document</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Predator</th>
                                        <th colspan=""2"">Primitor</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:MM}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", emailStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""8""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Nr. inventar</th>
										<th colspan = ""2"" style=""color: #ffffff;"">Descriere</th>
										<th style=""color: #ffffff;"">Data achizitie</th>
										<th style=""color: #ffffff;"">Serie</th>
                                        <th style=""color: #ffffff;"">Cantitate</th>
										<th style=""color: #ffffff;"">Valoare</th>
									</tr>";

            var htmlHeader3 = "";

            var htmlHeader4 = @"<tr>
										<th colspan = ""8""></th>
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

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            //var linkYes = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeevalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;
            //var linkNo = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeenotvalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;

            var link = "https://optima.emag.network/ofa";



            string empIni = emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.FirstName + " " + emailStatus[0].EmployeeInitial.LastName : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeInitial.Email}</th>
                                                <th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeFinal.Email}</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;"">DETALII PREDATOR</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;""></th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th>{emailStatus[0].EmployeeInitial.InternalCode}</th>
												<th>{emailStatus[0].EmployeeInitial.FirstName}</th>
												<th>{emailStatus[0].EmployeeInitial.LastName}</th>
												<th>{emailStatus[0].CostCenterInitial.Code}</th>
                                                <th>{emailStatus[0].CostCenterInitial.Division.Name}</th>
                                                <th>{emailStatus[0].CostCenterInitial.Division.Department.Name}</th>
                                                <th colspan=""2""></th>
												</tr>";

            htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;"">DETALII PRIMITOR</th>
												</tr>";

            htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th rowspan=""2"" colspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >VALIDEAZA TRANSFERUL</a>" + "&nbsp;" + @"
												</th>
												</tr>";

            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th>{emailStatus[0].EmployeeFinal.InternalCode}</th>
												<th>{emailStatus[0].EmployeeFinal.FirstName}</th>
												<th>{emailStatus[0].EmployeeFinal.LastName}</th>
												<th>{emailStatus[0].CostCenterFinal.Code}</th>
                                                <th>{emailStatus[0].CostCenterFinal.Division.Name}</th>
                                                <th>{emailStatus[0].CostCenterFinal.Division.Department.Name}</th>
												</tr>";


            subject = "Solicitare validare transfer numarul " + emailStatus[0].DocumentNumber + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{emailStatus[0].DocumentNumber}</th>";



            for (int i = 0; i < emailStatus.Count; i++)
            {
                index++;
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{emailStatus[i].Asset.InvNo + "|" + emailStatus[i].Asset.SubNo}</th>
									<th colspan=""2"">{emailStatus[i].Asset.Name}</th>
									<th>{String.Format("{0:dd/MM/yyyy}", emailStatus[i].Asset.PurchaseDate)}</th>
                                    <th>{emailStatus[i].Asset.SerialNumber}</th>
                                    <th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.Quantity)}</th>
									<th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.ValueInv)}</th>
								</tr>";
                if (index == emailStatus.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{emailStatus.Sum(a => a.Asset.Quantity)}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", emailStatus.Sum(a => a.Asset.ValueInv))}</th>
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


            emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdFinal).Select(a => a.Guid).ToList();

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

            //to = new List<string>();
            bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
            //to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
            //to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
            //to.Add("silvia.damian@emag.ro");

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
                for (int i = 0; i < emailStatus.Count; i++)
                {
                    //Model.Employee employee = _context.Set<Model.Employee>().Where(e => e.Id == )
                    //emailStatus[i].NotDstEmployeeReminder1Sync = false;
                    emailStatus[i].DstEmployeeReminder1EmailSend = true;
                    emailStatus[i].NotDstEmployeeSync = false;
                    emailStatus[i].DstEmployeeEmailSend = true;
                    //emailStatus[i].NotDstManagerSync = true;
                    //emailStatus[i].DstEmployeeValidateAt = DateTime.Now;
                    //emailStatus[i].DstEmployeeValidateBy = _context.UserId;

                    _context.Update(emailStatus[i]);
                    _context.SaveChanges();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendDstEmployeeReminder2Notification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            var files = new FormFileCollection();
            List<Model.EmailStatus> emailStatus = new List<Model.EmailStatus>();
            emailStatus = await _context.Set<Model.EmailStatus>()
                .Include(o => o.Asset)
                .Include(e => e.EmployeeInitial)
                .Include(e => e.EmployeeFinal)
                .Include(e => e.CostCenterInitial).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .Include(e => e.CostCenterFinal).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotDstEmployeeSync == true && a.DstEmployeeEmailSend == false && a.NotDstEmployeeReminder2Sync == true && a.DocumentNumber == documentNumberId).ToListAsync();
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
                                                          width: 0%;
                                                          text-align: center;
                                                          border-collapse: collapse;
                                                        }
                                                        table.redTable td, table.redTable th {
                                                          border: 1px solid #04327d;
                                                          padding: 3px 2px;
                                                        }
                                                        table.redTable tbody td {
                                                          font-size: 9px;;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 12px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 9px;;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 9px;;
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
                                                              font-size: 12px;
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
                                                              font-size: 12px;
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
                                        <th colspan=""8"" style=""color: #ffffff; font-weight: bold;font-size: 1.0em;"">TRANSFER NOU</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar document</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Predator</th>
                                        <th colspan=""2"">Primitor</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:MM}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", emailStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""8""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Nr. inventar</th>
										<th colspan = ""2"" style=""color: #ffffff;"">Descriere</th>
										<th style=""color: #ffffff;"">Data achizitie</th>
										<th style=""color: #ffffff;"">Serie</th>
                                        <th style=""color: #ffffff;"">Cantitate</th>
										<th style=""color: #ffffff;"">Valoare</th>
									</tr>";

            var htmlHeader3 = "";

            var htmlHeader4 = @"<tr>
										<th colspan = ""8""></th>
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

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            //var linkYes = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeevalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;
            //var linkNo = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeenotvalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;

            var link = "https://optima.emag.network/ofa";



            string empIni = emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.FirstName + " " + emailStatus[0].EmployeeInitial.LastName : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeInitial.Email}</th>
                                                <th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeFinal.Email}</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;"">DETALII PREDATOR</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;""></th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th>{emailStatus[0].EmployeeInitial.InternalCode}</th>
												<th>{emailStatus[0].EmployeeInitial.FirstName}</th>
												<th>{emailStatus[0].EmployeeInitial.LastName}</th>
												<th>{emailStatus[0].CostCenterInitial.Code}</th>
                                                <th>{emailStatus[0].CostCenterInitial.Division.Name}</th>
                                                <th>{emailStatus[0].CostCenterInitial.Division.Department.Name}</th>
                                                <th colspan=""2""></th>
												</tr>";

            htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;"">DETALII PRIMITOR</th>
												</tr>";

            htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th rowspan=""2"" colspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >VALIDEAZA TRANSFERUL</a>" + "&nbsp;" + @"
												</th>
												</tr>";

            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th>{emailStatus[0].EmployeeFinal.InternalCode}</th>
												<th>{emailStatus[0].EmployeeFinal.FirstName}</th>
												<th>{emailStatus[0].EmployeeFinal.LastName}</th>
												<th>{emailStatus[0].CostCenterFinal.Code}</th>
                                                <th>{emailStatus[0].CostCenterFinal.Division.Name}</th>
                                                <th>{emailStatus[0].CostCenterFinal.Division.Department.Name}</th>
												</tr>";


            subject = "Solicitare validare transfer numarul " + emailStatus[0].DocumentNumber + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{emailStatus[0].DocumentNumber}</th>";



            for (int i = 0; i < emailStatus.Count; i++)
            {
                index++;
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{emailStatus[i].Asset.InvNo + "|" + emailStatus[i].Asset.SubNo}</th>
									<th colspan=""2"">{emailStatus[i].Asset.Name}</th>
									<th>{String.Format("{0:dd/MM/yyyy}", emailStatus[i].Asset.PurchaseDate)}</th>
                                    <th>{emailStatus[i].Asset.SerialNumber}</th>
                                    <th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.Quantity)}</th>
									<th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.ValueInv)}</th>
								</tr>";
                if (index == emailStatus.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{emailStatus.Sum(a => a.Asset.Quantity)}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", emailStatus.Sum(a => a.Asset.ValueInv))}</th>
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


            emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdFinal).Select(a => a.Guid).ToList();

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

            //to = new List<string>();
            bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
            //to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
            //to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
            //to.Add("silvia.damian@emag.ro");

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
                for (int i = 0; i < emailStatus.Count; i++)
                {
                    //Model.Employee employee = _context.Set<Model.Employee>().Where(e => e.Id == )
                    emailStatus[i].NotDstEmployeeReminder2Sync = false;
                    emailStatus[i].DstEmployeeReminder2EmailSend = true;
                    emailStatus[i].NotDstEmployeeSync = false;
                    emailStatus[i].DstEmployeeEmailSend = true;
                    //emailStatus[i].NotDstManagerSync = true;
                    //emailStatus[i].DstEmployeeValidateAt = DateTime.Now;
                    //emailStatus[i].DstEmployeeValidateBy = _context.UserId;

                    _context.Update(emailStatus[i]);
                    _context.SaveChanges();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendDstEmployeeReminder3Notification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            var files = new FormFileCollection();
            List<Model.EmailStatus> emailStatus = new List<Model.EmailStatus>();
            emailStatus = await _context.Set<Model.EmailStatus>()
                .Include(o => o.Asset)
                .Include(e => e.EmployeeInitial)
                .Include(e => e.EmployeeFinal)
                .Include(e => e.CostCenterInitial).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .Include(e => e.CostCenterFinal).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotDstEmployeeSync == true && a.DstEmployeeEmailSend == false && a.NotDstEmployeeReminder3Sync == true && a.DocumentNumber == documentNumberId).ToListAsync();
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
                                                          width: 0%;
                                                          text-align: center;
                                                          border-collapse: collapse;
                                                        }
                                                        table.redTable td, table.redTable th {
                                                          border: 1px solid #04327d;
                                                          padding: 3px 2px;
                                                        }
                                                        table.redTable tbody td {
                                                          font-size: 9px;;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 12px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 9px;;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 9px;;
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
                                                              font-size: 12px;
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
                                                              font-size: 12px;
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
                                        <th colspan=""8"" style=""color: #ffffff; font-weight: bold;font-size: 1.0em;"">TRANSFER NOU</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar document</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Predator</th>
                                        <th colspan=""2"">Primitor</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:MM}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", emailStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""8""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Nr. inventar</th>
										<th colspan = ""2"" style=""color: #ffffff;"">Descriere</th>
										<th style=""color: #ffffff;"">Data achizitie</th>
										<th style=""color: #ffffff;"">Serie</th>
                                        <th style=""color: #ffffff;"">Cantitate</th>
										<th style=""color: #ffffff;"">Valoare</th>
									</tr>";

            var htmlHeader3 = "";

            var htmlHeader4 = @"<tr>
										<th colspan = ""8""></th>
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

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            //var linkYes = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeevalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;
            //var linkNo = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeenotvalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;

            var link = "https://optima.emag.network/ofa";



            string empIni = emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.FirstName + " " + emailStatus[0].EmployeeInitial.LastName : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeInitial.Email}</th>
                                                <th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeFinal.Email}</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;"">DETALII PREDATOR</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;""></th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th>{emailStatus[0].EmployeeInitial.InternalCode}</th>
												<th>{emailStatus[0].EmployeeInitial.FirstName}</th>
												<th>{emailStatus[0].EmployeeInitial.LastName}</th>
												<th>{emailStatus[0].CostCenterInitial.Code}</th>
                                                <th>{emailStatus[0].CostCenterInitial.Division.Name}</th>
                                                <th>{emailStatus[0].CostCenterInitial.Division.Department.Name}</th>
                                                <th colspan=""2""></th>
												</tr>";

            htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;"">DETALII PRIMITOR</th>
												</tr>";

            htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th rowspan=""2"" colspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >VALIDEAZA TRANSFERUL</a>" + "&nbsp;" + @"
												</th>
												</tr>";

            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th>{emailStatus[0].EmployeeFinal.InternalCode}</th>
												<th>{emailStatus[0].EmployeeFinal.FirstName}</th>
												<th>{emailStatus[0].EmployeeFinal.LastName}</th>
												<th>{emailStatus[0].CostCenterFinal.Code}</th>
                                                <th>{emailStatus[0].CostCenterFinal.Division.Name}</th>
                                                <th>{emailStatus[0].CostCenterFinal.Division.Department.Name}</th>
												</tr>";


            subject = "Solicitare validare transfer numarul " + emailStatus[0].DocumentNumber + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{emailStatus[0].DocumentNumber}</th>";



            for (int i = 0; i < emailStatus.Count; i++)
            {
                index++;
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{emailStatus[i].Asset.InvNo + "|" + emailStatus[i].Asset.SubNo}</th>
									<th colspan=""2"">{emailStatus[i].Asset.Name}</th>
									<th>{String.Format("{0:dd/MM/yyyy}", emailStatus[i].Asset.PurchaseDate)}</th>
                                    <th>{emailStatus[i].Asset.SerialNumber}</th>
                                    <th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.Quantity)}</th>
									<th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.ValueInv)}</th>
								</tr>";
                if (index == emailStatus.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{emailStatus.Sum(a => a.Asset.Quantity)}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", emailStatus.Sum(a => a.Asset.ValueInv))}</th>
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


            emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdFinal).Select(a => a.Guid).ToList();

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

            //to = new List<string>();
            bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
            //to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
            //to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
            //to.Add("silvia.damian@emag.ro");

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
                for (int i = 0; i < emailStatus.Count; i++)
                {
                    //Model.Employee employee = _context.Set<Model.Employee>().Where(e => e.Id == )
                    emailStatus[i].NotDstEmployeeReminder3Sync = false;
                    emailStatus[i].DstEmployeeReminder3EmailSend = true;
                    emailStatus[i].NotDstEmployeeSync = false;
                    emailStatus[i].DstEmployeeEmailSend = true;
                    //emailStatus[i].NotDstManagerSync = true;
                    //emailStatus[i].DstEmployeeValidateAt = DateTime.Now;
                    //emailStatus[i].DstEmployeeValidateBy = _context.UserId;

                    _context.Update(emailStatus[i]);
                    _context.SaveChanges();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendDstEmployeeReminder4Notification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            var files = new FormFileCollection();
            List<Model.EmailStatus> emailStatus = new List<Model.EmailStatus>();
            emailStatus = await _context.Set<Model.EmailStatus>()
                .Include(o => o.Asset)
                .Include(e => e.EmployeeInitial)
                .Include(e => e.EmployeeFinal)
                .Include(e => e.CostCenterInitial).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .Include(e => e.CostCenterFinal).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotDstEmployeeSync == true && a.DstEmployeeEmailSend == false && a.NotDstEmployeeReminder4Sync == true && a.DocumentNumber == documentNumberId).ToListAsync();
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
                                                          width: 0%;
                                                          text-align: center;
                                                          border-collapse: collapse;
                                                        }
                                                        table.redTable td, table.redTable th {
                                                          border: 1px solid #04327d;
                                                          padding: 3px 2px;
                                                        }
                                                        table.redTable tbody td {
                                                          font-size: 9px;;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 12px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 9px;;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 9px;;
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
                                                              font-size: 12px;
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
                                                              font-size: 12px;
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
                                        <th colspan=""8"" style=""color: #ffffff; font-weight: bold;font-size: 1.0em;"">TRANSFER NOU</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar document</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Predator</th>
                                        <th colspan=""2"">Primitor</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:MM}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", emailStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""8""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Nr. inventar</th>
										<th colspan = ""2"" style=""color: #ffffff;"">Descriere</th>
										<th style=""color: #ffffff;"">Data achizitie</th>
										<th style=""color: #ffffff;"">Serie</th>
                                        <th style=""color: #ffffff;"">Cantitate</th>
										<th style=""color: #ffffff;"">Valoare</th>
									</tr>";

            var htmlHeader3 = "";

            var htmlHeader4 = @"<tr>
										<th colspan = ""8""></th>
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

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            //var linkYes = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeevalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;
            //var linkNo = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeenotvalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;

            var link = "https://optima.emag.network/ofa";



            string empIni = emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.FirstName + " " + emailStatus[0].EmployeeInitial.LastName : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeInitial.Email}</th>
                                                <th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeFinal.Email}</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;"">DETALII PREDATOR</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;""></th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th>{emailStatus[0].EmployeeInitial.InternalCode}</th>
												<th>{emailStatus[0].EmployeeInitial.FirstName}</th>
												<th>{emailStatus[0].EmployeeInitial.LastName}</th>
												<th>{emailStatus[0].CostCenterInitial.Code}</th>
                                                <th>{emailStatus[0].CostCenterInitial.Division.Name}</th>
                                                <th>{emailStatus[0].CostCenterInitial.Division.Department.Name}</th>
                                                <th colspan=""2""></th>
												</tr>";

            htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;"">DETALII PRIMITOR</th>
												</tr>";

            htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th rowspan=""2"" colspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >VALIDEAZA TRANSFERUL</a>" + "&nbsp;" + @"
												</th>
												</tr>";

            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th>{emailStatus[0].EmployeeFinal.InternalCode}</th>
												<th>{emailStatus[0].EmployeeFinal.FirstName}</th>
												<th>{emailStatus[0].EmployeeFinal.LastName}</th>
												<th>{emailStatus[0].CostCenterFinal.Code}</th>
                                                <th>{emailStatus[0].CostCenterFinal.Division.Name}</th>
                                                <th>{emailStatus[0].CostCenterFinal.Division.Department.Name}</th>
												</tr>";


            subject = "Solicitare validare transfer numarul " + emailStatus[0].DocumentNumber + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{emailStatus[0].DocumentNumber}</th>";



            for (int i = 0; i < emailStatus.Count; i++)
            {
                index++;
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{emailStatus[i].Asset.InvNo + "|" + emailStatus[i].Asset.SubNo}</th>
									<th colspan=""2"">{emailStatus[i].Asset.Name}</th>
									<th>{String.Format("{0:dd/MM/yyyy}", emailStatus[i].Asset.PurchaseDate)}</th>
                                    <th>{emailStatus[i].Asset.SerialNumber}</th>
                                    <th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.Quantity)}</th>
									<th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.ValueInv)}</th>
								</tr>";
                if (index == emailStatus.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{emailStatus.Sum(a => a.Asset.Quantity)}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", emailStatus.Sum(a => a.Asset.ValueInv))}</th>
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


            emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdFinal).Select(a => a.Guid).ToList();

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

            //to = new List<string>();
            bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
            //to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
            //to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
            //to.Add("silvia.damian@emag.ro");

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
                for (int i = 0; i < emailStatus.Count; i++)
                {
                    //Model.Employee employee = _context.Set<Model.Employee>().Where(e => e.Id == )
                    emailStatus[i].NotDstEmployeeReminder4Sync = false;
                    emailStatus[i].DstEmployeeReminder4EmailSend = true;
                    emailStatus[i].NotDstEmployeeSync = false;
                    emailStatus[i].DstEmployeeEmailSend = true;
                    //emailStatus[i].NotDstManagerSync = true;
                    //emailStatus[i].DstEmployeeValidateAt = DateTime.Now;
                    //emailStatus[i].DstEmployeeValidateBy = _context.UserId;

                    _context.Update(emailStatus[i]);
                    _context.SaveChanges();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendDstManagerNotification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            var files = new FormFileCollection();
            List<Model.EmailStatus> emailStatus = new List<Model.EmailStatus>();
            emailStatus = await _context.Set<Model.EmailStatus>()
                .Include(o => o.Asset)
                .Include(e => e.EmployeeInitial)
                .Include(e => e.EmployeeFinal).ThenInclude(e => e.Manager)
                .Include(e => e.CostCenterInitial).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .Include(e => e.CostCenterFinal).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .Include(e => e.SrcEmployeeValidateUser)
                .Include(e => e.DstEmployeeValidateUser)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotDstManagerSync == true && a.DstManagerEmailSend == false && a.SyncDstManagerErrorCount < 5 && a.DocumentNumber == documentNumberId).ToListAsync();
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
                                                          font-size: 9px;;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 12px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 9px;;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 9px;;
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
                                                              font-size: 12px;
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
                                                              font-size: 12px;
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
                                        <th colspan=""8"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">TRANSFER NOU</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar document</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Predator</th>
                                        <th colspan=""2"">Primitor</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:MM}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", emailStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""8""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Nr. inventar</th>
										<th colspan = ""2"" style=""color: #ffffff;"">Descriere</th>
										<th style=""color: #ffffff;"">Data achizitie</th>
										<th style=""color: #ffffff;"">Serie</th>
                                        <th style=""color: #ffffff;"">Cantitate</th>
										<th style=""color: #ffffff;"">Valoare</th>
									</tr>";

            var htmlHeader3 = "";

            var htmlHeader4 = @"<tr>
										<th colspan = ""8""></th>
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

            var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            int index = 0;

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            //var linkYes = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeevalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;
            //var linkNo = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeenotvalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;

            var link = "https://optima.emag.network/ofa";



            string empIni = emailStatus[0].EmployeeInitial != null  ? emailStatus[0].EmployeeInitial.FirstName + " " + emailStatus[0].EmployeeInitial.LastName : "";
            string empIniEmail = emailStatus[0].SrcEmployeeValidateUser != null ? emailStatus[0].SrcEmployeeValidateUser.Email : emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.Email : "";
            string managerFin = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.FirstName + " " + emailStatus[0].EmployeeFinal.Manager.LastName : "";
            string managerInternalCode = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.InternalCode : "";
            string managerFirstName = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.FirstName : "";
            string managerLastName = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null  ? emailStatus[0].EmployeeFinal.Manager.LastName : "";
            string managerEmail = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeInitial.Email}</th>
                                                <th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeFinal.Email}</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;"">Detalii Predator</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Semnatura</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th>{emailStatus[0].EmployeeInitial.InternalCode}</th>
												<th>{emailStatus[0].EmployeeInitial.FirstName}</th>
												<th>{emailStatus[0].EmployeeInitial.LastName}</th>
												<th>{emailStatus[0].CostCenterInitial.Code}</th>
                                                <th>{emailStatus[0].CostCenterInitial.Division.Name}</th>
                                                <th>{emailStatus[0].CostCenterInitial.Division.Department.Name}</th>
                                                <th colspan=""2"">{empIniEmail + "<br>" + emailStatus[0].SrcEmployeeValidateAt.Value.ToString("dd/MM/yyyy HH:mm:ss")}</th>
												</tr>";

            htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;"">Detalii Primitor</th>
												</tr>";

            htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Semnatura</th>
												</tr>";

            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th>{emailStatus[0].EmployeeFinal.InternalCode}</th>
												<th>{emailStatus[0].EmployeeFinal.FirstName}</th>
												<th>{emailStatus[0].EmployeeFinal.LastName}</th>
												<th>{emailStatus[0].CostCenterFinal.Code}</th>
                                                <th>{emailStatus[0].CostCenterFinal.Division.Name}</th>
                                                <th>{emailStatus[0].CostCenterFinal.Division.Department.Name}</th>
                                                <th colspan=""2"">{emailStatus[0].DstEmployeeValidateUser.Email + "<br>" + emailStatus[0].DstEmployeeValidateAt.Value.ToString("dd/MM/yyyy HH:mm:ss")}</th>
												</tr>";


            htmlHeader14 = htmlHeader14 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""6"" style=""color: #ffffff;"">Detalii Manager Primitor</th>
                                                <th colspan=""2"">Semnatura</th>
												</tr>";

            htmlHeader15 = htmlHeader15 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th colspan=""3"" style=""background-color: #6491D9;color: #ffffff;"">Email</th>
												<th rowspan=""2"" colspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >APROBA TRANSFERUL</a>" + "&nbsp;" + @"
												</th>
												</tr>";

            htmlHeader16 = htmlHeader16 + $@"    
												<tr>
												<th>{managerInternalCode}</th>
												<th>{managerFirstName}</th>
												<th>{managerLastName}</th>
												<th  colspan=""3"">{managerEmail}</th>
												</tr>";

            subject = "Solicitare validare transfer numarul " + emailStatus[0].DocumentNumber + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{emailStatus[0].DocumentNumber}</th>";



            for (int i = 0; i < emailStatus.Count; i++)
            {
                index++;
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{emailStatus[i].Asset.InvNo + "|" + emailStatus[i].Asset.SubNo}</th>
									<th colspan=""2"">{emailStatus[i].Asset.Name}</th>
									<th>{String.Format("{0:dd/MM/yyyy}", emailStatus[i].Asset.PurchaseDate)}</th>
                                    <th>{emailStatus[i].Asset.SerialNumber}</th>
                                    <th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.Quantity)}</th>
									<th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.ValueInv)}</th>
								</tr>";
                if (index == emailStatus.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{emailStatus.Sum(a => a.Asset.Quantity)}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", emailStatus.Sum(a => a.Asset.ValueInv))}</th>
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


            emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdFinal).Select(a => a.Guid).ToList();

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

            //to = new List<string>();

            //to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
            //to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
            //to.Add("silvia.damian@emag.ro");

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeader14 + htmlHeader15 + htmlHeader16 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
                for (int i = 0; i < emailStatus.Count; i++)
                {
                    emailStatus[i].NotDstManagerSync = false;
                    emailStatus[i].DstManagerEmailSend = true;
                    _context.Update(emailStatus[i]);
                    _context.SaveChanges();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendFinalNotification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<int> emails1 = new List<int>();
            List<int> emails2 = new List<int>();
			List<int> emailsManager1 = new List<int>();
			List<int> emailsManager2 = new List<int>();
			List<int> emails = new List<int>();
			List<int> emailManagers = new List<int>();
			var files = new FormFileCollection();
            Model.EntityType entityType = null;
            List<Model.EmailStatus> emailStatus = new List<Model.EmailStatus>();
            emailStatus = await _context.Set<Model.EmailStatus>()
                .Include(o => o.Asset)
                .Include(e => e.EmployeeInitial).ThenInclude(e => e.Manager)
                .Include(e => e.EmployeeFinal).ThenInclude(e => e.Manager)
				.Include(e => e.EmployeeInitial).ThenInclude(c => c.CostCenter).ThenInclude(c => c.Division).ThenInclude(c => c.Department)
				.Include(e => e.EmployeeFinal).ThenInclude(c => c.CostCenter).ThenInclude(c => c.Division).ThenInclude(c => c.Department)
				.Include(e => e.CostCenterInitial).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .Include(e => e.CostCenterFinal).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
                .Include(e => e.SrcEmployeeValidateUser)
                .Include(e => e.DstEmployeeValidateUser)
                //.Include(e => e.DstManagerValidateUser)
                .AsNoTracking()
                .Where(a => a.IsDeleted == false && a.NotSync == true && a.NotCompletedSync == true && a.SyncErrorCount < 3 && a.DocumentNumber == documentNumberId)
                .ToListAsync();
            entityType = await _context.Set<Model.EntityType>().AsNoTracking().Where(e => e.Code == "TRANSFER").SingleAsync();
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
                                                          border: 1px solid #04327d;
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
                                                          font-size: 6px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 6px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 6px;
                                                        }
                                                        table.redTable tfoot .links {
                                                          text-align: right;
                                                        }
                                                        table.redTable tfoot .links a{
                                                          display: inline-block;
                                                          background: #FFFFFF;
                                                          color: #04327d;
                                                          padding: 2px 8px;
                                                          border-radius: 1px;
                                                        }
													.button {
                                                              background-color: #2f75b5;
                                                              border: none;
                                                              color: #ffffff;
                                                              padding: 5px 10px;
                                                              text-align: center;
                                                              text-decoration: none;
                                                              display: inline-block;
                                                              font-size: 12px;
                                                              margin: 4px 2px;
                                                              cursor: pointer;
                                                            }
                                                   .button-no {
                                                              background-color: #cf1140;
                                                              border: none;
                                                              color: white;
                                                              padding: 5px 10px;
                                                              text-align: center;
                                                              text-decoration: none;
                                                              display: inline-block;
                                                              font-size: 12px;
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
                                        <th colspan=""8"" style=""color: #ffffff; font-weight: bold;font-size: 1.0em;"">TRANSFER VALIDAT</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar document</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Predator</th>
                                        <th colspan=""2"">Primitor</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:MM}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", emailStatus[0].CreatedAt)}</th>
									</tr>";
            var htmlHeader14 = "";
            var htmlHeader15 = "";
            var htmlHeader16 = "";
            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""8""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Nr. inventar</th>
										<th colspan = ""2"" style=""color: #ffffff;"">Descriere</th>
										<th style=""color: #ffffff;"">Data achizitie</th>
										<th style=""color: #ffffff;"">Serie</th>
                                        <th style=""color: #ffffff;"">Cantitate</th>
										<th style=""color: #ffffff;"">Valoare</th>
									</tr>";

            var htmlHeader3 = "";

            var htmlHeader4 = @"<tr>
										<th colspan = ""8""></th>
								</tr>";

            var htmlHeader5 = "";
            var htmlHeader6 = "";
            var htmlHeader7 = "";
            var htmlHeader8 = "";
            var htmlHeader9 = "";
            var htmlHeader10 = "";
            var htmlHeader30 = "";
			var htmlHeader35 = "";
			var htmlHeader40 = "";
			var htmlHeader50 = "";


			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            int index = 0;

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

            //var linkYes = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeevalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;
            //var linkNo = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeenotvalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;

            // var link = "https://optima.emag.network/ofa";



            string empIni = emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.FirstName + " " + emailStatus[0].EmployeeInitial.LastName : "";
            string empIniEmail = emailStatus[0].SrcEmployeeValidateUser != null ? emailStatus[0].SrcEmployeeValidateUser.Email : emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.Email : "";
            //string managerFin = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.FirstName + " " + emailStatus[0].EmployeeFinal.Manager.LastName : "";
            //string managerInternalCode = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.InternalCode : "";
            //string managerFirstName = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.FirstName : "";
            //string managerLastName = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.LastName : "";
            //string managerEmail = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.Email : "-")}</th>
                                                <th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeFinal.Email}</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;text-align: left;"">DETALII PREDATOR</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Aprobat</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th>{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.InternalCode : "")}</th>
												<th>{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.FirstName : "")}</th>
												<th>{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.LastName : "")}</th>
												<th>{(emailStatus[0].EmployeeInitial != null && emailStatus[0].EmployeeInitial.CostCenter != null ? emailStatus[0].EmployeeInitial.CostCenter.Code : "")}</th>
                                                <th>{(emailStatus[0].EmployeeInitial != null && emailStatus[0].EmployeeInitial.CostCenter != null && emailStatus[0].EmployeeInitial.CostCenter.Division != null ? emailStatus[0].EmployeeInitial.CostCenter.Division.Name : "")}</th>
                                                <th>{(emailStatus[0].EmployeeInitial != null && emailStatus[0].EmployeeInitial.CostCenter != null && emailStatus[0].EmployeeInitial.CostCenter.Division != null && emailStatus[0].EmployeeInitial.CostCenter.Division.Department != null ? emailStatus[0].EmployeeInitial.CostCenter.Division.Department.Name : "")}</th>
                                                <th colspan=""2"">{empIniEmail + "<br>" + emailStatus[0].SrcEmployeeValidateAt.Value.ToString("dd/MM/yyyy HH:mm:ss")}</th>
												</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;text-align: left;"">DETALII PRIMITOR</th>
												</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Aprobat</th>
												</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th>{emailStatus[0].EmployeeFinal.InternalCode}</th>
												<th>{emailStatus[0].EmployeeFinal.FirstName}</th>
												<th>{emailStatus[0].EmployeeFinal.LastName}</th>
												<th>{(emailStatus[0].EmployeeFinal.CostCenter != null ? emailStatus[0].EmployeeFinal.CostCenter.Code : "")}</th>
                                                <th>{(emailStatus[0].EmployeeFinal.CostCenter != null && emailStatus[0].EmployeeFinal.CostCenter.Division != null ? emailStatus[0].EmployeeFinal.CostCenter.Division.Name : "")}</th>
                                                <th>{(emailStatus[0].EmployeeFinal.CostCenter != null && emailStatus[0].EmployeeFinal.CostCenter.Division != null && emailStatus[0].EmployeeFinal.CostCenter.Division.Department != null ? emailStatus[0].EmployeeFinal.CostCenter.Division.Department.Name : "")}</th>
                                                <th colspan=""2"">{emailStatus[0].DstEmployeeValidateUser.Email + "<br>" + emailStatus[0].DstEmployeeValidateAt.Value.ToString("dd/MM/yyyy HH:mm:ss")}</th>
												</tr>";


			htmlHeader30 = htmlHeader30 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;text-align: left;"">DETALII TRANSFER</th>
												</tr>";

			htmlHeader35 = htmlHeader35 + $@"     
												<tr>
												<th colspan=""4"" style=""background-color: #6491D9;color: #ffffff;"">ACTUAL</th>
                                                <th colspan=""4"" style=""background-color: #6491D9;color: #ffffff;"">DESTINATIE</th>
												</tr>";

			htmlHeader40 = htmlHeader40 + $@"  
												<tr>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												</tr>";

			htmlHeader50 = htmlHeader50 + $@"    
												<tr>
                                                <th colspan=""2"">{emailStatus[0].CostCenterInitial.Code}</th>
                                                <th>{(emailStatus[0].CostCenterInitial.Division != null ? emailStatus[0].CostCenterInitial.Division.Name : "")}</th>
                                                <th>{(emailStatus[0].CostCenterInitial.Division != null && emailStatus[0].CostCenterInitial.Division.Department != null ? emailStatus[0].CostCenterInitial.Division.Department.Name : "")}</th>
												<th colspan=""2"">{(emailStatus[0].CostCenterFinal.Code)}</th>
												<th>{(emailStatus[0].CostCenterFinal != null && emailStatus[0].CostCenterFinal.Division != null ? emailStatus[0].CostCenterFinal.Division.Name : "")}</th>
												<th>{(emailStatus[0].CostCenterFinal != null && emailStatus[0].CostCenterFinal.Division != null && emailStatus[0].CostCenterFinal.Division.Department != null ? emailStatus[0].CostCenterFinal.Division.Department.Name : "")}</th>
												</tr>";

			subject = "Bonul de transfer cu numarul " + emailStatus[0].DocumentNumber + " a fost aprobat!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{emailStatus[0].DocumentNumber}</th>";



            for (int i = 0; i < emailStatus.Count; i++)
            {
                index++;
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{emailStatus[i].Asset.InvNo + "/" + emailStatus[i].Asset.SubNo}</th>
									<th colspan=""2"">{emailStatus[i].Asset.Name}</th>
									<th>{String.Format("{0:dd/MM/yyyy}", emailStatus[i].Asset.PurchaseDate)}</th>
                                    <th>{emailStatus[i].Asset.SerialNumber}</th>
                                    <th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.Quantity)}</th>
									<th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.ValueInv)}</th>
								</tr>";
                if (index == emailStatus.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{emailStatus.Sum(a => a.Asset.Quantity)}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", emailStatus.Sum(a => a.Asset.ValueInv))}</th>
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


           
            

            if(emailStatus[0].EmployeeInitial != null && emailStatus[0].EmployeeInitial.Manager != null)
            {
				emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdInitial).Select(a => a.Id).ToList();
				emailsManager1 = _context.Set<Model.Employee>().Include(m => m.Manager).AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdInitial).Select(a => a.ManagerId.Value).ToList();
			}

			if (emailStatus[0].EmployeeFinal.Manager != null)
			{
				emails2 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdFinal).Select(a => a.Id).ToList();
				emailsManager2 = _context.Set<Model.Employee>().Include(m => m.Manager).AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdFinal).Select(a => a.ManagerId.Value).ToList();
			}


			

			if (emails1.Count > 0)
            {
                for (int i = 0; i < emails1.Count; i++)
                {
					emails.Add(emails1.ElementAt(i));
				}
                
            }

            if (emails2.Count > 0)
            {
                for (int i = 0; i < emails2.Count; i++)
                {
					emails.Add(emails2.ElementAt(i));
				}
                
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


			// Managers // 

			if (emailsManager1.Count > 0)
			{
				for (int i = 0; i < emailsManager1.Count; i++)
				{
					emailManagers.Add(emailsManager1.ElementAt(i));
				}

			}

			if (emailsManager2.Count > 0)
			{
				for (int i = 0; i < emailsManager2.Count; i++)
				{
					emailManagers.Add(emailsManager2.ElementAt(i));
				}

			}


			for (int e = 0; e < emailManagers.Count; e++)
			{
				var manager = _context.Set<Model.Manager>().Where(m => m.Id == emailManagers.ElementAt(e)).Single();

				if (manager.Email != null && manager.Email != "")
				{
					cc.Add(manager.Email);
				}
				else
				{
					cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
				}
			}


			// Managers //

			//to = new List<string>();
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
            //cc.Add("alina.poturlu@emag.ro");
            //to.Add("adriancirnaru@yahoo.com");
            //to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
            //to.Add("silvia.damian@emag.ro");

            var path = "\\bonuri\\2022\\";

            List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>()
                .Include(e => e.EntityType)
                .Where(e => e.EntityTypeId == entityType.Id && e.IsDeleted == false && e.EntityId == documentNumberId)
                .ToList();

            if (entityFiles.Count == 0)
            {
                return false;
            }

            MemoryStream ms = new();
            

            List<(string, string, string)> filePaths = new List<(string, string, string)>();

            for (int i = 0; i < entityFiles.Count; i++)
            {
                string filePath = $"{this._basePath}{path}{entityFiles[i].StoredAs}";
                filePaths.Add(new(filePath, entityFiles[i].Name, entityFiles[i].FileType));
            }

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + 
                htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 +
				htmlHeader30 + htmlHeader35 + htmlHeader40 + htmlHeader50 + htmlHeader14 + htmlHeader15 + htmlHeader16 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, filePaths);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            if (successEmail)
            {
				for (int i = 0; i < emailStatus.Count; i++)
				{
					emailStatus[i].NotSync = false;
					emailStatus[i].NotCompletedSync = false;
					emailStatus[i].FinalValidateAt = DateTime.Now;
					_context.Update(emailStatus[i]);
					_context.SaveChanges();
				}

				return true;
            }
            else
            {
                return false;
            }
        }

		public async Task<bool> SendRejectedNotification(int documentNumberId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			List<int> emails1 = new List<int>();
			List<int> emails2 = new List<int>();
			List<int> emailsManager1 = new List<int>();
			List<int> emailsManager2 = new List<int>();
			List<int> emails = new List<int>();
			List<int> emailManagers = new List<int>();
			var files = new FormFileCollection();
			Model.EntityType entityType = null;
			List<Model.EmailStatus> emailStatus = new List<Model.EmailStatus>();
			emailStatus = await _context.Set<Model.EmailStatus>()
				.Include(o => o.Asset)
				.Include(o => o.AssetOp).ThenInclude(e => e.EmployeeInitial).ThenInclude(e => e.Manager)
				.Include(o => o.AssetOp).ThenInclude(e => e.EmployeeInitial).ThenInclude(e => e.CostCenter).ThenInclude(c => c.Division).ThenInclude(c => c.Department)
				.Include(e => e.EmployeeInitial).ThenInclude(e => e.Manager)
				.Include(e => e.EmployeeFinal).ThenInclude(e => e.Manager)
				.Include(e => e.EmployeeInitial).ThenInclude(c => c.CostCenter).ThenInclude(c => c.Division).ThenInclude(c => c.Department)
				.Include(e => e.EmployeeFinal).ThenInclude(c => c.CostCenter).ThenInclude(c => c.Division).ThenInclude(c => c.Department)
				.Include(e => e.CostCenterInitial).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
				.Include(e => e.CostCenterFinal).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
				.Include(e => e.SrcEmployeeValidateUser)
				.Include(e => e.DstEmployeeValidateUser)
				//.Include(e => e.DstManagerValidateUser)
				.AsNoTracking()
				.Where(a => a.IsDeleted == false && a.NotSync == true && a.NotCompletedSync == true && a.SyncErrorCount < 3 && a.DocumentNumber == documentNumberId)
				.ToListAsync();
			entityType = await _context.Set<Model.EntityType>().AsNoTracking().Where(e => e.Code == "TRANSFER").SingleAsync();
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
                                                          border: 1px solid #04327d;
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
                                                          font-size: 6px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 6px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 6px;
                                                        }
                                                        table.redTable tfoot .links {
                                                          text-align: right;
                                                        }
                                                        table.redTable tfoot .links a{
                                                          display: inline-block;
                                                          background: #FFFFFF;
                                                          color: #04327d;
                                                          padding: 2px 8px;
                                                          border-radius: 1px;
                                                        }
													.button {
                                                              background-color: #2f75b5;
                                                              border: none;
                                                              color: #ffffff;
                                                              padding: 5px 10px;
                                                              text-align: center;
                                                              text-decoration: none;
                                                              display: inline-block;
                                                              font-size: 12px;
                                                              margin: 4px 2px;
                                                              cursor: pointer;
                                                            }
                                                   .button-no {
                                                              background-color: #cf1140;
                                                              border: none;
                                                              color: white;
                                                              padding: 5px 10px;
                                                              text-align: center;
                                                              text-decoration: none;
                                                              display: inline-block;
                                                              font-size: 12px;
                                                              margin: 4px 2px;
                                                              cursor: pointer;
                                                            }
                                                </style>
                                            </head>
                                            <body>
                                                <table class=""redTable"">
                                                    <thead>
                                                    
                                        ";
			var htmlHeader = $@"        
                                    <tr style=""background-color: #DC143C;font-color: #ffffff; font-weight: bold"">
                                        <th colspan=""8"" style=""color: #ffffff; font-weight: bold;font-size: 1.0em;text-align: left"">Transfer refuzat.Motiv: {emailStatus[0].Info}</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar document</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Predator</th>
                                        <th colspan=""2"">Primitor</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:MM}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", emailStatus[0].CreatedAt)}</th>
									</tr>";
			var htmlHeader14 = "";
			var htmlHeader15 = "";
			var htmlHeader16 = "";
			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""8""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Nr. inventar</th>
										<th colspan = ""2"" style=""color: #ffffff;"">Descriere</th>
										<th style=""color: #ffffff;"">Data achizitie</th>
										<th style=""color: #ffffff;"">Serie</th>
                                        <th style=""color: #ffffff;"">Cantitate</th>
										<th style=""color: #ffffff;"">Valoare</th>
									</tr>";

			var htmlHeader3 = "";

			var htmlHeader4 = @"<tr>
										<th colspan = ""8""></th>
								</tr>";

			var htmlHeader5 = "";
			var htmlHeader6 = "";
			var htmlHeader7 = "";
			var htmlHeader8 = "";
			var htmlHeader9 = "";
			var htmlHeader10 = "";
			var htmlHeader30 = "";
			var htmlHeader35 = "";
			var htmlHeader40 = "";
			var htmlHeader50 = "";


			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			int index = 0;

			//var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

			////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

			//var linkYes = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeevalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;
			//var linkNo = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeenotvalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;

			// var link = "https://optima.emag.network/ofa";



			string empIni = emailStatus[0].EmployeeInitial != null ? (emailStatus[0].EmployeeInitial.FirstName + " " + emailStatus[0].EmployeeInitial.LastName) : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null ? (emailStatus[0].AssetOp.EmployeeInitial.FirstName + " " + emailStatus[0].AssetOp.EmployeeInitial.LastName) : "";
			string empIniEmail = emailStatus[0].SrcEmployeeValidateUser != null ? emailStatus[0].SrcEmployeeValidateUser.Email : emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.Email : "";
			//string managerFin = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.FirstName + " " + emailStatus[0].EmployeeFinal.Manager.LastName : "";
			//string managerInternalCode = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.InternalCode : "";
			//string managerFirstName = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.FirstName : "";
			//string managerLastName = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.LastName : "";
			//string managerEmail = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.Email : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null ? (emailStatus[0].AssetOp.EmployeeInitial.Email) : "-")}</th>
                                                <th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeFinal.Email}</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;text-align: left;"">DETALII PREDATOR</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Aprobat</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th>{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.InternalCode : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null ? (emailStatus[0].AssetOp.EmployeeInitial.InternalCode) : "")}</th>
												<th>{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.FirstName : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null ? (emailStatus[0].AssetOp.EmployeeInitial.FirstName) :"")}</th>
												<th>{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.LastName : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null ? (emailStatus[0].AssetOp.EmployeeInitial.LastName) :"")}</th>
												<th>{(emailStatus[0].EmployeeInitial != null && emailStatus[0].EmployeeInitial.CostCenter != null ? emailStatus[0].EmployeeInitial.CostCenter.Code : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null && emailStatus[0].AssetOp.EmployeeInitial.CostCenter != null ? (emailStatus[0].AssetOp.EmployeeInitial.CostCenter.Code) : "")}</th>
                                                <th>{(emailStatus[0].EmployeeInitial != null && emailStatus[0].EmployeeInitial.CostCenter != null && emailStatus[0].EmployeeInitial.CostCenter.Division != null ? emailStatus[0].EmployeeInitial.CostCenter.Division.Name : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null && emailStatus[0].AssetOp.EmployeeInitial.CostCenter != null && emailStatus[0].AssetOp.EmployeeInitial.CostCenter.Division != null ? (emailStatus[0].AssetOp.EmployeeInitial.CostCenter.Division.Name) : "")}</th>
                                                <th>{(emailStatus[0].EmployeeInitial != null && emailStatus[0].EmployeeInitial.CostCenter != null && emailStatus[0].EmployeeInitial.CostCenter.Division != null && emailStatus[0].EmployeeInitial.CostCenter.Division.Department != null ? emailStatus[0].EmployeeInitial.CostCenter.Division.Department.Name : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null && emailStatus[0].AssetOp.EmployeeInitial.CostCenter != null && emailStatus[0].AssetOp.EmployeeInitial.CostCenter.Division != null && emailStatus[0].AssetOp.EmployeeInitial.CostCenter.Division.Department != null ? (emailStatus[0].AssetOp.EmployeeInitial.CostCenter.Division.Department.Name) : "")}</th>
                                                <th colspan=""2"">{empIniEmail + "<br>" + emailStatus[0].SrcEmployeeValidateAt.Value.ToString("dd/MM/yyyy HH:mm:ss")}</th>
												</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;text-align: left;"">DETALII PRIMITOR</th>
												</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Marca</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Nume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Prenume</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Refuzat</th>
												</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th>{emailStatus[0].EmployeeFinal.InternalCode}</th>
												<th>{emailStatus[0].EmployeeFinal.FirstName}</th>
												<th>{emailStatus[0].EmployeeFinal.LastName}</th>
												<th>{(emailStatus[0].EmployeeFinal.CostCenter != null ? emailStatus[0].EmployeeFinal.CostCenter.Code : "")}</th>
                                                <th>{(emailStatus[0].EmployeeFinal.CostCenter != null && emailStatus[0].EmployeeFinal.CostCenter.Division != null ? emailStatus[0].EmployeeFinal.CostCenter.Division.Name : "")}</th>
                                                <th>{(emailStatus[0].EmployeeFinal.CostCenter != null && emailStatus[0].EmployeeFinal.CostCenter.Division != null && emailStatus[0].EmployeeFinal.CostCenter.Division.Department != null ? emailStatus[0].EmployeeFinal.CostCenter.Division.Department.Name : "")}</th>
                                                <th colspan=""2"">{emailStatus[0].DstEmployeeValidateUser.Email + "<br>" + emailStatus[0].DstEmployeeValidateAt.Value.ToString("dd/MM/yyyy HH:mm:ss")}</th>
												</tr>";


			htmlHeader30 = htmlHeader30 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""8"" style=""color: #ffffff;text-align: left;"">DETALII TRANSFER</th>
												</tr>";

			htmlHeader35 = htmlHeader35 + $@"     
												<tr>
												<th colspan=""4"" style=""background-color: #6491D9;color: #ffffff;"">ACTUAL</th>
                                                <th colspan=""4"" style=""background-color: #6491D9;color: #ffffff;"">DESTINATIE</th>
												</tr>";

			htmlHeader40 = htmlHeader40 + $@"  
												<tr>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">B.U.</th>
												</tr>";

			htmlHeader50 = htmlHeader50 + $@"    
												<tr>
                                                <th colspan=""2"">{(emailStatus[0].CostCenterInitial != null ? emailStatus[0].CostCenterInitial.Code : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null && emailStatus[0].AssetOp.EmployeeInitial.CostCenter != null ? (emailStatus[0].AssetOp.EmployeeInitial.CostCenter.Code) : "")}</th>
                                                <th>{(emailStatus[0].CostCenterInitial != null && emailStatus[0].CostCenterInitial.Division != null ? emailStatus[0].CostCenterInitial.Division.Name : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null && emailStatus[0].AssetOp.EmployeeInitial.CostCenter != null && emailStatus[0].AssetOp.EmployeeInitial.CostCenter.Division != null ? (emailStatus[0].AssetOp.EmployeeInitial.CostCenter.Division.Name) : "")}</th>
                                                <th>{(emailStatus[0].CostCenterInitial != null && emailStatus[0].CostCenterInitial.Division != null && emailStatus[0].CostCenterInitial.Division.Department != null ? emailStatus[0].CostCenterInitial.Division.Department.Name : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null && emailStatus[0].AssetOp.EmployeeInitial.CostCenter != null && emailStatus[0].AssetOp.EmployeeInitial.CostCenter.Division != null && emailStatus[0].AssetOp.EmployeeInitial.CostCenter.Division.Department != null ? (emailStatus[0].AssetOp.EmployeeInitial.CostCenter.Division.Department.Name) : "")}</th>
												<th colspan=""2"">{(emailStatus[0].CostCenterFinal != null ? emailStatus[0].CostCenterFinal.Code : "")}</th>
												<th>{(emailStatus[0].CostCenterFinal != null && emailStatus[0].CostCenterFinal.Division != null ? emailStatus[0].CostCenterFinal.Division.Name : "")}</th>
												<th>{(emailStatus[0].CostCenterFinal != null && emailStatus[0].CostCenterFinal.Division != null && emailStatus[0].CostCenterFinal.Division.Department != null ? emailStatus[0].CostCenterFinal.Division.Department.Name : "")}</th>
												</tr>";

			subject = "Bonul de transfer cu numarul " + emailStatus[0].DocumentNumber + " a fost refuzat!";

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{emailStatus[0].DocumentNumber}</th>";



			for (int i = 0; i < emailStatus.Count; i++)
			{
				index++;
				htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{emailStatus[i].Asset.InvNo + "/" + emailStatus[i].Asset.SubNo}</th>
									<th colspan=""2"">{emailStatus[i].Asset.Name}</th>
									<th>{String.Format("{0:dd/MM/yyyy}", emailStatus[i].Asset.PurchaseDate)}</th>
                                    <th>{emailStatus[i].Asset.SerialNumber}</th>
                                    <th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.Quantity)}</th>
									<th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.ValueInv)}</th>
								</tr>";
				if (index == emailStatus.Count)
				{
					htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{emailStatus.Sum(a => a.Asset.Quantity)}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", emailStatus.Sum(a => a.Asset.ValueInv))}</th>
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





			if (emailStatus[0].EmployeeInitial != null && emailStatus[0].EmployeeInitial.Manager != null)
			{
				emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdInitial).Select(a => a.Id).ToList();
				emailsManager1 = _context.Set<Model.Employee>().Include(m => m.Manager).AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdInitial).Select(a => a.ManagerId.Value).ToList();
            }
            else
            {
                if(emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null && emailStatus[0].AssetOp.EmployeeInitial.Manager != null)
                {
					emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].AssetOp.EmployeeIdInitial).Select(a => a.Id).ToList();
					emailsManager1 = _context.Set<Model.Employee>().Include(m => m.Manager).AsNoTracking().Where(a => a.Id == emailStatus[0].AssetOp.EmployeeIdInitial).Select(a => a.ManagerId.Value).ToList();
				}
            }

			if (emailStatus[0].EmployeeFinal.Manager != null)
			{
				emails2 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdFinal).Select(a => a.Id).ToList();
				emailsManager2 = _context.Set<Model.Employee>().Include(m => m.Manager).AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdFinal).Select(a => a.ManagerId.Value).ToList();
			}




			if (emails1.Count > 0)
			{
				for (int i = 0; i < emails1.Count; i++)
				{
					emails.Add(emails1.ElementAt(i));
				}

			}

			if (emails2.Count > 0)
			{
				for (int i = 0; i < emails2.Count; i++)
				{
					emails.Add(emails2.ElementAt(i));
				}

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


			// Managers // 

			if (emailsManager1.Count > 0)
			{
				for (int i = 0; i < emailsManager1.Count; i++)
				{
					emailManagers.Add(emailsManager1.ElementAt(i));
				}

			}

			if (emailsManager2.Count > 0)
			{
				for (int i = 0; i < emailsManager2.Count; i++)
				{
					emailManagers.Add(emailsManager2.ElementAt(i));
				}

			}


			for (int e = 0; e < emailManagers.Count; e++)
			{
				var manager = _context.Set<Model.Manager>().Where(m => m.Id == emailManagers.ElementAt(e)).Single();

				if (manager.Email != null && manager.Email != "")
				{
					cc.Add(manager.Email);
				}
				else
				{
					cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
				}
			}


			// Managers //

			//to = new List<string>();
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
            //cc.Add("alina.poturlu@emag.ro");
            //to.Add("adriancirnaru@yahoo.com");
            //to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
            //to.Add("silvia.damian@emag.ro");

#if DEBUG
            to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
            cc = new List<string>();
#endif

			//var path = "\\bonuri\\2022\\";

			//List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>()
			//	.Include(e => e.EntityType)
			//	.Where(e => e.EntityTypeId == entityType.Id && e.IsDeleted == false && e.EntityId == documentNumberId)
			//	.ToList();

			//if (entityFiles.Count == 0)
			//{
			//	return false;
			//}

			MemoryStream ms = new();


			List<(string, string, string)> filePaths = new List<(string, string, string)>();

			//for (int i = 0; i < entityFiles.Count; i++)
			//{
			//	string filePath = $"{this._basePath}{path}{entityFiles[i].StoredAs}";
			//	filePaths.Add(new(filePath, entityFiles[i].Name, entityFiles[i].FileType));
			//}

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 +
				htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 +
				htmlHeader30 + htmlHeader35 + htmlHeader40 + htmlHeader50 + htmlHeader14 + htmlHeader15 + htmlHeader16 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, filePaths);

			successEmail = await _emailSender.SendEmailAsync(emailMessage);

			if (successEmail)
			{
				for (int i = 0; i < emailStatus.Count; i++)
				{
					emailStatus[i].NotSync = false;
					emailStatus[i].NotCompletedSync = false;
					emailStatus[i].FinalValidateAt = DateTime.Now;
					_context.Update(emailStatus[i]);
					_context.SaveChanges();
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		public async Task<bool> SendRejectedAccountingNotification(int documentNumberId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			List<int> emails1 = new List<int>();
			List<int> emails2 = new List<int>();
			List<int> emails = new List<int>();
			List<int> emailManagers = new List<int>();
			var files = new FormFileCollection();
			Model.EntityType entityType = null;
			List<Model.EmailStatus> emailStatus = new List<Model.EmailStatus>();
			emailStatus = await _context.Set<Model.EmailStatus>()
				.Include(o => o.Asset)
				.Include(o => o.AssetOp).ThenInclude(e => e.EmployeeInitial).ThenInclude(e => e.Manager)
				.Include(o => o.AssetOp).ThenInclude(e => e.EmployeeInitial).ThenInclude(e => e.CostCenter).ThenInclude(c => c.Division).ThenInclude(c => c.Department)
				.Include(e => e.EmployeeInitial).ThenInclude(e => e.Manager)
				.Include(e => e.EmployeeFinal).ThenInclude(e => e.Manager)
				.Include(e => e.EmployeeInitial).ThenInclude(c => c.CostCenter).ThenInclude(c => c.Division).ThenInclude(c => c.Department)
				.Include(e => e.EmployeeFinal).ThenInclude(c => c.CostCenter).ThenInclude(c => c.Division).ThenInclude(c => c.Department)
				.Include(e => e.CostCenterInitial).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
				.Include(e => e.CostCenterFinal).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
				.Include(e => e.SrcEmployeeValidateUser)
				.Include(e => e.DstEmployeeValidateUser)
				//.Include(e => e.DstManagerValidateUser)
				.AsNoTracking()
				.Where(a => a.IsDeleted == false && a.NotSync == true && a.NotCompletedSync == true && a.SyncErrorCount < 3 && a.DocumentNumber == documentNumberId)
				.ToListAsync();
			entityType = await _context.Set<Model.EntityType>().AsNoTracking().Where(e => e.Code == "REJECTASSET").SingleAsync();
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
                                                          border: 1px solid #04327d;
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
                                                          font-size: 6px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 6px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 6px;
                                                        }
                                                        table.redTable tfoot .links {
                                                          text-align: right;
                                                        }
                                                        table.redTable tfoot .links a{
                                                          display: inline-block;
                                                          background: #FFFFFF;
                                                          color: #04327d;
                                                          padding: 2px 8px;
                                                          border-radius: 1px;
                                                        }
													.button {
                                                              background-color: #2f75b5;
                                                              border: none;
                                                              color: #ffffff;
                                                              padding: 5px 10px;
                                                              text-align: center;
                                                              text-decoration: none;
                                                              display: inline-block;
                                                              font-size: 12px;
                                                              margin: 4px 2px;
                                                              cursor: pointer;
                                                            }
                                                   .button-no {
                                                              background-color: #cf1140;
                                                              border: none;
                                                              color: white;
                                                              padding: 5px 10px;
                                                              text-align: center;
                                                              text-decoration: none;
                                                              display: inline-block;
                                                              font-size: 12px;
                                                              margin: 4px 2px;
                                                              cursor: pointer;
                                                            }
                                                </style>
                                            </head>
                                            <body>
                                                <table class=""redTable"">
                                                    <thead>
                                                    
                                        ";
			var htmlHeader = $@"        
                                    <tr style=""background-color: #DC143C;font-color: #ffffff; font-weight: bold"">
                                        <th colspan=""8"" style=""color: #ffffff; font-weight: bold;font-size: 1.0em;text-align: left"">Validare refuzata.Motiv: {emailStatus[0].Info}</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar document</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Contabilitate</th>
                                        <th colspan=""2"">Solicitant</th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:MM}", emailStatus[0].CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", emailStatus[0].CreatedAt)}</th>
									</tr>";
			var htmlHeader14 = "";
			var htmlHeader15 = "";
			var htmlHeader16 = "";
			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""8""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Nr. inventar</th>
										<th colspan = ""2"" style=""color: #ffffff;"">Descriere</th>
										<th style=""color: #ffffff;"">Data achizitie</th>
										<th style=""color: #ffffff;"">Serie</th>
                                        <th style=""color: #ffffff;"">Cantitate</th>
										<th style=""color: #ffffff;"">Valoare</th>
									</tr>";

			var htmlHeader3 = "";

            var htmlHeader4 = @"<tr>
										<th colspan = ""8""></th>
								</tr>";


			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			int index = 0;

			//var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

			////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + emailStatus[0].Guid + "/" + emailStatus[0].Id;

			//var linkYes = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeevalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;
			//var linkNo = "https://optima.emag.network/ofaapi/api/emailstatus/dstemployeenotvalidate/" + emailStatus[0].EmployeeFinal.Guid + "/" + emailStatus[0].Guid;

			// var link = "https://optima.emag.network/ofa";



			string empIni = emailStatus[0].EmployeeInitial != null ? (emailStatus[0].EmployeeInitial.FirstName + " " + emailStatus[0].EmployeeInitial.LastName) : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null ? (emailStatus[0].AssetOp.EmployeeInitial.FirstName + " " + emailStatus[0].AssetOp.EmployeeInitial.LastName) : "";
			string empIniEmail = emailStatus[0].SrcEmployeeValidateUser != null ? emailStatus[0].SrcEmployeeValidateUser.Email : emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.Email : "";
			//string managerFin = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.FirstName + " " + emailStatus[0].EmployeeFinal.Manager.LastName : "";
			//string managerInternalCode = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.InternalCode : "";
			//string managerFirstName = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.FirstName : "";
			//string managerLastName = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.LastName : "";
			//string managerEmail = emailStatus[0].EmployeeFinal != null && emailStatus[0].EmployeeFinal.Manager != null ? emailStatus[0].EmployeeFinal.Manager.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{(emailStatus[0].EmployeeInitial != null ? emailStatus[0].EmployeeInitial.Email : emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null ? (emailStatus[0].AssetOp.EmployeeInitial.Email) : "-")}</th>
                                                <th rowspan=""2"" colspan=""2"">{emailStatus[0].EmployeeFinal.Email}</th>";

			subject = "Numarul de inventar" + emailStatus[0].Asset.InvNo + "|" + emailStatus[0].Asset.SubNo + " a fost refuzat de contabilitate!";

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{emailStatus[0].DocumentNumber}</th>";



			for (int i = 0; i < emailStatus.Count; i++)
			{
				index++;
				htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{emailStatus[i].Asset.InvNo + "/" + emailStatus[i].Asset.SubNo}</th>
									<th colspan=""2"">{emailStatus[i].Asset.Name}</th>
									<th>{String.Format("{0:dd/MM/yyyy}", emailStatus[i].Asset.PurchaseDate)}</th>
                                    <th>{emailStatus[i].Asset.SerialNumber}</th>
                                    <th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.Quantity)}</th>
									<th>{String.Format("{0:#,##0.##}", emailStatus[i].Asset.ValueInv)}</th>
								</tr>";
				if (index == emailStatus.Count)
				{
					htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{emailStatus.Sum(a => a.Asset.Quantity)}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", emailStatus.Sum(a => a.Asset.ValueInv))}</th>
								</tr>";
				}
			};

			if (emailStatus[0].EmployeeInitial != null && emailStatus[0].EmployeeInitial.Manager != null)
			{
				emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdInitial).Select(a => a.Id).ToList();
				//emailsManager1 = _context.Set<Model.Employee>().Include(m => m.Manager).AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdInitial).Select(a => a.ManagerId.Value).ToList();
			}
			else
			{
				if (emailStatus[0].AssetOp != null && emailStatus[0].AssetOp.EmployeeInitial != null && emailStatus[0].AssetOp.EmployeeInitial.Manager != null)
				{
					emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].AssetOp.EmployeeIdInitial).Select(a => a.Id).ToList();
					//emailsManager1 = _context.Set<Model.Employee>().Include(m => m.Manager).AsNoTracking().Where(a => a.Id == emailStatus[0].AssetOp.EmployeeIdInitial).Select(a => a.ManagerId.Value).ToList();
				}
			}

			if (emailStatus[0].EmployeeFinal.Manager != null)
			{
				emails2 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdFinal).Select(a => a.Id).ToList();
				//emailsManager2 = _context.Set<Model.Employee>().Include(m => m.Manager).AsNoTracking().Where(a => a.Id == emailStatus[0].EmployeeIdFinal).Select(a => a.ManagerId.Value).ToList();
			}




			if (emails1.Count > 0)
			{
				for (int i = 0; i < emails1.Count; i++)
				{
					emails.Add(emails1.ElementAt(i));
				}

			}

			if (emails2.Count > 0)
			{
				for (int i = 0; i < emails2.Count; i++)
				{
					emails.Add(emails2.ElementAt(i));
				}

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


			// Managers // 


			for (int e = 0; e < emailManagers.Count; e++)
			{
				var manager = _context.Set<Model.Manager>().Where(m => m.Id == emailManagers.ElementAt(e)).Single();

				if (manager.Email != null && manager.Email != "")
				{
					cc.Add(manager.Email);
				}
				else
				{
					cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
				}
			}


			// Managers //

			//to = new List<string>();
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
			//cc.Add("alina.poturlu@emag.ro");
			//to.Add("adriancirnaru@yahoo.com");
			//to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
			//to.Add("silvia.damian@emag.ro");

#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			cc = new List<string>();
#endif

			//var path = "\\bonuri\\2022\\";

			//List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>()
			//	.Include(e => e.EntityType)
			//	.Where(e => e.EntityTypeId == entityType.Id && e.IsDeleted == false && e.EntityId == documentNumberId)
			//	.ToList();

			//if (entityFiles.Count == 0)
			//{
			//	return false;
			//}

			MemoryStream ms = new();


			List<(string, string, string)> filePaths = new List<(string, string, string)>();

			//for (int i = 0; i < entityFiles.Count; i++)
			//{
			//	string filePath = $"{this._basePath}{path}{entityFiles[i].StoredAs}";
			//	filePaths.Add(new(filePath, entityFiles[i].Name, entityFiles[i].FileType));
			//}

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 +
				htmlHeader2 + htmlHeader3 + htmlHeader4  + htmlHeader14 + htmlHeader15 + htmlHeader16 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, filePaths);

			successEmail = await _emailSender.SendEmailAsync(emailMessage);

			if (successEmail)
			{
				for (int i = 0; i < emailStatus.Count; i++)
				{
					emailStatus[i].NotSync = false;
					emailStatus[i].NotCompletedSync = false;
					emailStatus[i].FinalValidateAt = DateTime.Now;
					_context.Update(emailStatus[i]);
					_context.SaveChanges();
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		public async Task<bool> GenerateAppendixAsync(int documentNumber)
        {
            Model.EntityType entityType = null;
            Model.EntityFile entityFile = null;
            List<Model.EntityFile> entityFiles = null;
            var path = "\\bonuri\\2022\\";
            var entityTypeCode = "TRANSFER";
            // var documentType = await this._documentTypesRepository.GetByCodeAsync(this._documentTypeTransferProposal);
            string fileName = documentNumber + "-" + Guid.NewGuid().ToString() + ".pdf";
            string filePath = $"{this._basePath}{path}{fileName}";
            PdfDocumentResult document = null;

			try
			{
				document = await this._appendix1Generator.GenerateDocumentAsync(documentNumber, this._resourcesPath);

				DocumentRenderer docRenderer = new DocumentRenderer(document.Document);
				docRenderer.PrepareDocument();

				PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
				renderer.Document = document.Document;

				renderer.RenderDocument();

				try
				{
					renderer.PdfDocument.Save(filePath);

					using (var scope = _services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						entityType = await dbContext.Set<Model.EntityType>().Where(e => e.Code == entityTypeCode).SingleAsync();

						entityFiles = await dbContext.Set<Model.EntityFile>()
							.Where(e => e.EntityId == documentNumber && e.IsDeleted == false && e.EntityTypeId == entityType.Id)
							.ToListAsync();

						for (int i = 0; i < entityFiles.Count; i++)
						{
							entityFiles[i].IsDeleted = true;
							dbContext.Update(entityFiles[i]);
						}

						entityFile = new Model.EntityFile()
						{
							EntityId = documentNumber,
							FileType = "application/pdf",
							EntityTypeId = entityType.Id,
							Info = string.Empty,
							Name = "Bon de miscare nr. " + documentNumber + ".pdf",
							Size = renderer.PdfDocument.FileSize,
							StoredAs = fileName,
							IsDeleted = false
						};

						dbContext.Add(entityFile);
						dbContext.SaveChanges();
					}

					return true;
				}
				catch (Exception ex)
				{

					using (var errorfile = System.IO.File.CreateText("SAVE_PDF_ERROR_NR: " + documentNumber + "_" + DateTime.Now.Ticks + ".txt"))
					{
						errorfile.WriteLine(ex.StackTrace);
						errorfile.WriteLine(ex.ToString());
						errorfile.WriteLine(filePath.ToString());
					};

					return false;
				}
			}
			catch (Exception)
			{

				//using (var errorfile = System.IO.File.CreateText("DOCUMENT-" + DateTime.Now.Ticks + ".txt"))
				//{
				//    errorfile.WriteLine(ex.StackTrace);
				//    errorfile.WriteLine(ex.ToString());
				//};

				List<Model.EmailStatus> emailStatus = await _context.Set<Model.EmailStatus>().Where(e => e.DocumentNumber == documentNumber).ToListAsync();

				for (int k = 0; k < emailStatus.Count; k++)
				{
					emailStatus[k].GenerateBookErrorCount++;
					_context.Update(emailStatus[k]);
					_context.SaveChanges();
				}

				return false;
			}
		}
    }
}
