using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MigraDoc.Rendering;
using Optima.Fais.Api.Services;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class EmailOrderStatusService : IEmailOrderStatusService
    {
		private readonly ApplicationDbContext _context;
		private readonly IEmailSender _emailSender;
        public IServiceProvider _services { get; }

        private readonly string _basePath;
        private readonly string _resourcesFolder;
        private readonly string _resourcesPath;

        public EmailOrderStatusService(ApplicationDbContext context, IEmailSender emailSender, IConfiguration configuration, IServiceProvider services)
		{
			this._context = context;
			this._emailSender = emailSender;
            _services = services;
            this._basePath = configuration.GetSection("BasePath").GetValue<string>("Base");
            this._resourcesFolder = configuration.GetSection("BasePath").GetValue<string>("Resources");
            this._resourcesPath = $"{this._basePath}{this._resourcesFolder}";
        }

        public async Task<bool> SendNeedBudgetNotification(int orderId, int? requestbudgetForecastId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            var files = new FormFileCollection();
            List<Model.RequestBudgetForecastMaterial> reqBFMaterials = null;
            Model.AppState appState = null;
            // Model.Order order = null;
            Model.RequestBudgetForecast requestBudgetForecast = null;
            Model.EmailOrderStatus emailOrderStatus = null;

            //order = await _context.Set<Model.Order>()
            //    .Include(o => o.Offer).ThenInclude(r => r.Request)
            //    .Include(o => o.Offer).ThenInclude(r => r.AssetType)
            //    .Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
            //    .Include(o => o.Offer).ThenInclude(r => r.Region)
            //    .Include(c => c.Uom)
            //    .Include(c => c.Partner)
            //    .Include(c => c.Company)
            //    .Include(c => c.CostCenter)
            //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Employee)
            //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
            //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Activity)
            //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Division)
            //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Department)
            //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
            //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
            //    .Include(c => c.Employee)
            //    .Include(c => c.Project)
            //    .Include(c => c.OrderType).AsNoTracking().Where(a => a.Id == orderId).SingleOrDefaultAsync();
            requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
               //.Include(o => o.BudgetForecast).ThenInclude(r => r.Request)
               //.Include(o => o.Offer).ThenInclude(r => r.AssetType)
               //.Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
               //.Include(o => o.Offer).ThenInclude(r => r.Region)
               //.Include(c => c.Uom)
               //.Include(c => c.Partner)
               //.Include(c => c.Company)
               //.Include(c => c.CostCenter)
               .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Employee)
               .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
               .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Activity)
               .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Division)
               .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Department)
               .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
               .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
               .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Region)
               .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.AdmCenter)
               //.Include(c => c.Employee)
               //.Include(c => c.Project)
               //.Include(c => c.OrderType)
               .AsNoTracking().Where(a => a.Id == requestbudgetForecastId).SingleAsync();
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
                                        <th colspan=""11"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">SOLICITARE SUPLIMENTARE BUGET</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Solicitant</th>
                                        <th colspan=""4"">Solicitare suplimentare buget RON</th>
                                        <th></th>
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
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Cod P.R.</th>
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

            reqBFMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
                .Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
                .Include(o => o.Order).ThenInclude(a => a.Uom)
                .Include(o => o.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.AssetType)
                .Include(a => a.Material)
                .Include(o => o.Order).ThenInclude(a => a.OrderType)
                .Include(o => o.Order).ThenInclude(a => a.Partner)
                .Include(o => o.Order).ThenInclude(a => a.Company)
                .Include(o => o.Order).ThenInclude(a => a.Employee)
                .Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestbudgetForecastId)
                .ToListAsync();

            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
                .Where(a => a.OrderId == orderId && a.RequestBudgetForecastId == requestbudgetForecastId && a.IsDeleted == false && a.NeedBudgetEmailSend == false && a.NotNeedBudgetSync == true)
                .SingleAsync();


            int index = 0;

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/ofa";

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "NEED_BUDGET").SingleAsync();

            string empIni = reqBFMaterials[0].Order.Employee != null ? reqBFMaterials[0].Order.Employee.FirstName + " " + reqBFMaterials[0].Order.Employee.LastName : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{reqBFMaterials[0].Order.Employee.Email}</th>
                                                <th rowspan=""2"" colspan=""4"">{String.Format("{0:#,##0.##}", requestBudgetForecast.NeedBudgetValue)}</th>
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
												<th style=""background-color: #6491D9;color: #ffffff;"">Tip</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"">{reqBFMaterials[0].Order.Partner.Name}</th>
												<th>{reqBFMaterials[0].Order.Company.Code}</th>
												<th>{reqBFMaterials[0].Order.Offer.AssetType.Name}</th>
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
                                                 <th style=""background-color: #6491D9;color: #ffffff;"">PC</th>
                                                 <th style=""background-color: #6491D9;color: #ffffff;"">PC Det</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Buget disponibil RON</th>
												<th rowspan=""2"" colspan=""2"">
												</th>
												</tr>";

            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th >{requestBudgetForecast.BudgetForecast.BudgetBase.Code}</th>
												<th colspan=""2"">{requestBudgetForecast.BudgetForecast.BudgetBase.Employee.Email}</th>
												<th>{requestBudgetForecast.BudgetForecast.BudgetBase.Project.Code}</th>
                                                <th>{requestBudgetForecast.BudgetForecast.BudgetBase.Activity.Name}</th>
                                                <th>{requestBudgetForecast.BudgetForecast.BudgetBase.AdmCenter.Name}</th>
                                                <th>{requestBudgetForecast.BudgetForecast.BudgetBase.Region.Name}</th>
												<th>{requestBudgetForecast.BudgetForecast.BudgetBase.Info}</th>
                                                <th>{String.Format("{0:#,##0.##}", requestBudgetForecast.ValueRon - requestBudgetForecast.NeedBudgetValue)}</th>
												</tr>";


            subject = "Suplimentare buget pentru comanda: " + reqBFMaterials[0].Order.Code + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{reqBFMaterials[0].Order.Code}</th>";



            for (int i = 0; i < reqBFMaterials.Count; i++)
            {
                index++;
                var wip = reqBFMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{reqBFMaterials[i].Order.Offer.Request.Code}</th>
                                    <th>{reqBFMaterials[i].Order.OrderType.Name}</th>
									<th>{wip}</th>
									<th>{reqBFMaterials[i].Material.Code}</th>
                                    <th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].Quantity)}</th>
                                    <th>{reqBFMaterials[i].Order.Uom.Code}</th>
									<th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].Price)}</th>
									<th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].Value)}</th>
                                    <th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].PriceRon)}</th>
                                    <th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].ValueRon)}</th>
								</tr>";
                if (index == reqBFMaterials.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""4""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", reqBFMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", reqBFMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", reqBFMaterials.Sum(a => a.ValueRon))}</th>
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


            emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Email == "madalina.udrea@emag.ro").Select(a => a.Guid).ToListAsync();

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
            ////to.Add("iordache.costin@emag.ro");
            //to.Add("silvia.damian@emag.ro");

            bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

            List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.RequestBudgetForecastId == requestbudgetForecastId).ToList();

            MemoryStream ms = new();

            List<(string, string, string)> filePaths = new List<(string, string, string)>();

            for (int i = 0; i < entityFiles.Count; i++)
            {
                var filePath = Path.Combine("order", entityFiles[i].StoredAs);
                filePaths.Add(new(filePath, entityFiles[i].Name, entityFiles[i].FileType));
            }

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, filePaths);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
                emailOrderStatus.NotNeedBudgetSync = false;
                emailOrderStatus.NeedBudgetEmailSend = true;
                _context.Update(emailOrderStatus);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

		public async Task<bool> SendB1Notification(int documentNumberId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			List<Guid> emails1 = new List<Guid>();
			List<Guid> emails2 = new List<Guid>();
			List<Guid> emails = new List<Guid>();
			List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
			var files = new FormFileCollection();
			List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
			//Model.AppState appState = null;
			Model.Order order = null;
			emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
				.Include(o => o.Order).ThenInclude(p => p.Partner)
				 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
				 .Include(o => o.Order).ThenInclude(p => p.Company)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
				.AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeB1Sync == true && a.EmployeeB1EmailSkip == false && a.EmployeeB1EmailSend == false && a.SyncEmployeeB1ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();
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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare initiere flux comanda</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
			var htmlHeader120 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			orderMaterials = await _context.Set<Model.OrderMaterial>()
				.Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
				.Include(o => o.Order).ThenInclude(a => a.Uom)
				.Include(a => a.Material)
				.Include(o => o.Order).ThenInclude(a => a.OrderType)
				.Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

			order = await _context.Set<Model.Order>()
				.Include(e => e.Offer).ThenInclude(r => r.ProjectType)
				.Include(e => e.Offer).ThenInclude(r => r.Division)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
				.Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

			int index = 0;

			// var linkYesNo = "http://localhost:4200/#/ordervalidateL4/" + order.Guid;
			var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateB1/" + order.Guid;
			//var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

			var link = "https://optima.emag.network/ofa/#/procurement/order";

#if DEBUG
 link = "https://optima.emag.network/ofauat";
linkYesNo = "https://optima.emag.network/ofauat/#/ordervalidateB1/" + order.Guid;
#endif

			//appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



			string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
			string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
														<a class=""button"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >DA</a>" + "&nbsp;" + @"
														<a class=""button-no"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >NU</a>" + @"
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";

			//htmlHeader120 = htmlHeader120 + $@"    
			//									<tr>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			//                                             <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>
			//                                             <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>
			//                                             <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
			//									</tr>";


			subject = "Initiere flux comanda: " + emailOrderStatus[0].Order.Code;

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



			for (int i = 0; i < orderMaterials.Count; i++)
			{
				index++;
				var wip = orderMaterials[i].WIP ? "DA" : "NU";
				htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
				if (index == orderMaterials.Count)
				{
					htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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


			emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeB1Id).Select(a => a.Guid).ToListAsync();

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
					to.Add("cosmina.pricop@optima.ro");
				}
			}
#if DEBUG
			to = new List<string>();

			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			to.Add("radu.alexandru1@emag.ro");
			//to.Add("gabriela.dogaru@emag.ro");
			subject = "TEST - Initiere flux comanda: " + emailOrderStatus[0].Order.Code;
#endif
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 +
				htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 +
				htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeader120 +
				htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

			successEmail = await _emailSender.SendEmailAsync(emailMessage);

			to = new List<string>();

			if (successEmail)
			{
				var result = await SendB1RequesterNotification(documentNumberId);

				if (result)
				{
					for (int i = 0; i < emailOrderStatus.Count; i++)
					{
						emailOrderStatus[i].NotEmployeeB1Sync = false;
						emailOrderStatus[i].EmployeeB1EmailSend = true;
						_context.Update(emailOrderStatus[i]);

						//order.AppStateId = appState.Id;
						//_context.Update(order);


						_context.SaveChanges();
					}
				}

				return result;
			}
			else
			{
				return false;
			}
		}

		public async Task<bool> SendL4Notification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
            var files = new FormFileCollection();
            List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
            //Model.AppState appState = null;
            Model.Order order = null;
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
                .Include(o => o.Order).ThenInclude(p => p.Partner)
                 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
                 .Include(o => o.Order).ThenInclude(p => p.Company)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeL4Sync == true && a.EmployeeL4EmailSkip == false && a.EmployeeL4EmailSend == false && a.SyncEmployeeL4ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();
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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
            var htmlHeader120= "";

            var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            orderMaterials = await _context.Set<Model.OrderMaterial>()
                .Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
                .Include(o => o.Order).ThenInclude(a => a.Uom)
                .Include(a => a.Material)
                .Include(o => o.Order).ThenInclude(a => a.OrderType)
                .Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

            order = await _context.Set<Model.Order>()
                .Include(e => e.Offer).ThenInclude(r => r.ProjectType)
                .Include(e => e.Offer).ThenInclude(r => r.Division)
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
                .Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

            int index = 0;

            // var linkYesNo = "http://localhost:4200/#/ordervalidateL4/" + order.Guid;
            var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateL4/" + order.Guid;
            //var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/ofa";

            //appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();


            
            string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
            string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
														<a class=""button"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >DA</a>" + "&nbsp;" + @"
														<a class=""button-no"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >NU</a>" + @"
													</th>
												</tr>";

            //htmlHeader8 = htmlHeader8 + $@"     
            //<tr style=""background-color: #2f75b5;"">
            //<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
            //</tr>";

            htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

            htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
								<th>L3</th>
								<th>L2</th>
			                    <th>L1</th>
			                    <th>S3</th>
			                    <th colspan=""2"">S2</th>
			                    <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";

			//htmlHeader120 = htmlHeader120 + $@"    
			//									<tr>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			//                                             <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>
			//                                             <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>
			//                                             <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
			//									</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



            for (int i = 0; i < orderMaterials.Count; i++)
            {
                index++;
                var wip = orderMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
                if (index == orderMaterials.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

            
            emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeL4Id).Select(a => a.Guid).ToListAsync();

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
#if DEBUG
			to = new List<string>();

			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			to.Add("radu.alexandru1@emag.ro");
			//to.Add("silvia.damian@emag.ro");
			subject = "TEST - Aprobare comanda: " + emailOrderStatus[0].Order.Code;
#endif
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 +
                htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + 
                htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeader120 +
				htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
               var result = await SendL4RequesterNotification(documentNumberId);

                if (result)
                {
					for (int i = 0; i < emailOrderStatus.Count; i++)
					{
						emailOrderStatus[i].NotEmployeeL4Sync = false;
						emailOrderStatus[i].EmployeeL4EmailSend = true;
						_context.Update(emailOrderStatus[i]);

						//order.AppStateId = appState.Id;
						//_context.Update(order);


						_context.SaveChanges();
					}
				}

				
                return result;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendL3Notification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
            var files = new FormFileCollection();
            List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
            //Model.AppState appState = null;
            Model.Order order = null;
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
                .Include(o => o.Order).ThenInclude(p => p.Partner)
                 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
                 .Include(o => o.Order).ThenInclude(p => p.Company)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeL3Sync == true && a.EmployeeL3EmailSkip == false && a.EmployeeL3EmailSend == false && a.SyncEmployeeL3ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();


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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
            var htmlHeader120 = "";

            var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            orderMaterials = await _context.Set<Model.OrderMaterial>()
                .Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
                .Include(o => o.Order).ThenInclude(a => a.Uom)
                .Include(a => a.Material)
                .Include(o => o.Order).ThenInclude(a => a.OrderType)
                .Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

            order = await _context.Set<Model.Order>()
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
                .Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

            int index = 0;

            // var linkYesNo = "http://localhost:4200/#/ordervalidateL3/" + order.Guid;
            var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateL3/" + order.Guid;
            //var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/ofa";

            //appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



            string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
            string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
														<a class=""button"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >DA</a>" + "&nbsp;" + @"
														<a class=""button-no"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >NU</a>" + @"
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



            for (int i = 0; i < orderMaterials.Count; i++)
            {
                index++;
                var wip = orderMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
                if (index == orderMaterials.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

            //order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
            emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeL3Id).Select(a => a.Guid).ToListAsync();

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
#if DEBUG
            to = new List<string>();
            to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
            ////to.Add("iordache.costin@emag.ro");
            //to.Add("silvia.damian@emag.ro");
#endif
            bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + 
                htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + 
                htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeader120 +
                htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
                var result = await SendL3RequesterNotification(documentNumberId);

                if (result)
                {
					for (int i = 0; i < emailOrderStatus.Count; i++)
					{
						emailOrderStatus[i].NotEmployeeL3Sync = false;
						emailOrderStatus[i].EmployeeL3EmailSend = true;
						_context.Update(emailOrderStatus[i]);

						//order.AppStateId = appState.Id;
						//_context.Update(order);


						_context.SaveChanges();
					}
				}
               
                return result;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendL2Notification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
            var files = new FormFileCollection();
            List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
            //Model.AppState appState = null;
            Model.Order order = null;
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
                .Include(o => o.Order).ThenInclude(p => p.Partner)
                 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
                 .Include(o => o.Order).ThenInclude(p => p.Company)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeL2Sync == true && a.EmployeeL2EmailSkip == false && a.EmployeeL2EmailSend == false && a.SyncEmployeeL2ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();

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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
            var htmlHeader120 = "";

            var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            orderMaterials = await _context.Set<Model.OrderMaterial>()
                .Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
                .Include(o => o.Order).ThenInclude(a => a.Uom)
                .Include(a => a.Material)
                .Include(o => o.Order).ThenInclude(a => a.OrderType)
                .Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

            order = await _context.Set<Model.Order>()
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
                .Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

            int index = 0;

            // var linkYesNo = "http://localhost:4200/#/ordervalidateL2/" + order.Guid;
            var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateL2/" + order.Guid;
            //var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/ofa";

            //appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



            string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
            string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
														<a class=""button"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >DA</a>" + "&nbsp;" + @"
														<a class=""button-no"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >NU</a>" + @"
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



            for (int i = 0; i < orderMaterials.Count; i++)
            {
                index++;
                var wip = orderMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
                if (index == orderMaterials.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

            //order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
            emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeL2Id).Select(a => a.Guid).ToListAsync();

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
#if DEBUG
            to = new List<string>();
            to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
            to.Add("radu.alexandru1@emag.ro");
			//to.Add("silvia.damian@emag.ro");
			subject = "TEST - Aprobare comanda: " + emailOrderStatus[0].Order.Code;
#endif
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 +
                htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + 
                htmlHeader9 + htmlHeader10  + htmlHeader120 +
                htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
				var result = await SendL2RequesterNotification(documentNumberId);

                if (result)
                {
					for (int i = 0; i < emailOrderStatus.Count; i++)
					{
						emailOrderStatus[i].NotEmployeeL2Sync = false;
						emailOrderStatus[i].EmployeeL2EmailSend = true;
						_context.Update(emailOrderStatus[i]);

						//order.AppStateId = appState.Id;
						//_context.Update(order);


						_context.SaveChanges();
					}
				}
				
                return result;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendL1Notification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
            var files = new FormFileCollection();
            List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
            //Model.AppState appState = null;
            Model.Order order = null;
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
                .Include(o => o.Order).ThenInclude(p => p.Partner)
                 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
                 .Include(o => o.Order).ThenInclude(p => p.Company)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeL1Sync == true && a.EmployeeL1EmailSkip == false && a.EmployeeL1EmailSend == false && a.SyncEmployeeL1ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();

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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
            var htmlHeader120 = "";

            var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            orderMaterials = await _context.Set<Model.OrderMaterial>()
                .Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
                .Include(o => o.Order).ThenInclude(a => a.Uom)
                .Include(a => a.Material)
                .Include(o => o.Order).ThenInclude(a => a.OrderType)
                .Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

            order = await _context.Set<Model.Order>()
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
                .Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

            int index = 0;

            // var linkYesNo = "http://localhost:4200/#/ordervalidateL1/" + order.Guid;
            var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateL1/" + order.Guid;
            //var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/ofa";

            //appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



            string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
            string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
														<a class=""button"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >DA</a>" + "&nbsp;" + @"
														<a class=""button-no"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >NU</a>" + @"
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



            for (int i = 0; i < orderMaterials.Count; i++)
            {
                index++;
                var wip = orderMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
                if (index == orderMaterials.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

            // order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
            emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeL1Id).Select(a => a.Guid).ToListAsync();

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

#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			to.Add("radu.alexandru1@emag.ro");
			//to.Add("silvia.damian@emag.ro");
			subject = "TEST - Aprobare comanda: " + emailOrderStatus[0].Order.Code;
#endif

			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + 
                htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + 
                htmlHeader9 + htmlHeader10 + htmlHeader120 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
				var result = await SendL1RequesterNotification(documentNumberId);

                if (result)
                {
					for (int i = 0; i < emailOrderStatus.Count; i++)
					{
						emailOrderStatus[i].NotEmployeeL1Sync = false;
						emailOrderStatus[i].EmployeeL1EmailSend = true;
						_context.Update(emailOrderStatus[i]);

						//order.AppStateId = appState.Id;
						//_context.Update(order);


						_context.SaveChanges();
					}
				}

				
                return result;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendS3Notification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
            var files = new FormFileCollection();
            List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
            // Model.AppState appState = null;
            Model.Order order = null;
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
                .Include(o => o.Order).ThenInclude(p => p.Partner)
                 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
                 .Include(o => o.Order).ThenInclude(p => p.Company)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeS3Sync == true && a.EmployeeS3EmailSkip == false && a.EmployeeS3EmailSend == false && a.SyncEmployeeS3ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();

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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
            var htmlHeader120 = "";

            var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            orderMaterials = await _context.Set<Model.OrderMaterial>()
                .Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
                .Include(o => o.Order).ThenInclude(a => a.Uom)
                .Include(a => a.Material)
                .Include(o => o.Order).ThenInclude(a => a.OrderType)
                .Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

            order = await _context.Set<Model.Order>()
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
                .Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

            int index = 0;

            // var linkYesNo = "http://localhost:4200/#/ordervalidateS3/" + order.Guid;
            var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateS3/" + order.Guid;
            //var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/ofa";

            //appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



            string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
            string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
														<a class=""button"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >DA</a>" + "&nbsp;" + @"
														<a class=""button-no"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >NU</a>" + @"
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



            for (int i = 0; i < orderMaterials.Count; i++)
            {
                index++;
                var wip = orderMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
                if (index == orderMaterials.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

            //order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
            emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeS3Id).Select(a => a.Guid).ToListAsync();

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

#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			////to.Add("iordache.costin@emag.ro");
			//to.Add("silvia.damian@emag.ro");
#endif

			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + 
                htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + 
                htmlHeader9 + htmlHeader10 + htmlHeader120 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
				var result = await SendS3RequesterNotification(documentNumberId);

                if (result)
                {
					for (int i = 0; i < emailOrderStatus.Count; i++)
					{
						emailOrderStatus[i].NotEmployeeS3Sync = false;
						emailOrderStatus[i].EmployeeS3EmailSend = true;
						_context.Update(emailOrderStatus[i]);

						//order.AppStateId = appState.Id;
						//_context.Update(order);


						_context.SaveChanges();
					}
				}

				
                return result;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendS2Notification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
            var files = new FormFileCollection();
            List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
            //Model.AppState appState = null;
            Model.Order order = null;
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
                .Include(o => o.Order).ThenInclude(p => p.Partner)
                 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
                 .Include(o => o.Order).ThenInclude(p => p.Company)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeS2Sync == true && a.EmployeeS2EmailSkip == false && a.EmployeeS2EmailSend == false && a.SyncEmployeeS2ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();

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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
            var htmlHeader120 = "";

            var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            orderMaterials = await _context.Set<Model.OrderMaterial>()
                .Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
                .Include(o => o.Order).ThenInclude(a => a.Uom)
                .Include(a => a.Material)
                .Include(o => o.Order).ThenInclude(a => a.OrderType)
                .Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

            order = await _context.Set<Model.Order>()
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
                .Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

            int index = 0;

            // var linkYesNo = "http://localhost:4200/#/ordervalidateS2/" + order.Guid;
            var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateS2/" + order.Guid;
            //var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/ofa";

            //appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



            string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
            string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
														<a class=""button"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >DA</a>" + "&nbsp;" + @"
														<a class=""button-no"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >NU</a>" + @"
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



            for (int i = 0; i < orderMaterials.Count; i++)
            {
                index++;
                var wip = orderMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
                if (index == orderMaterials.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

            //order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
            emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeS2Id).Select(a => a.Guid).ToListAsync();

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

#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			////to.Add("iordache.costin@emag.ro");
			//to.Add("silvia.damian@emag.ro");
#endif

			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + 
                htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + 
                htmlHeader9 + htmlHeader10 + htmlHeader120 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
				var result = await SendS2RequesterNotification(documentNumberId);

                if (result)
                {
					for (int i = 0; i < emailOrderStatus.Count; i++)
					{
						emailOrderStatus[i].NotEmployeeS2Sync = false;
						emailOrderStatus[i].EmployeeS2EmailSend = true;
						_context.Update(emailOrderStatus[i]);

						//order.AppStateId = appState.Id;
						//_context.Update(order);


						_context.SaveChanges();
					}
				}

				
                return result;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendS1Notification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
            var files = new FormFileCollection();
            List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
            // Model.AppState appState = null;
            Model.Order order = null;
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
                .Include(o => o.Order).ThenInclude(p => p.Partner)
                 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
                 .Include(o => o.Order).ThenInclude(p => p.Company)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
                 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
                 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
                 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeS1Sync == true && a.EmployeeS1EmailSkip == false && a.EmployeeS1EmailSend == false && a.SyncEmployeeS1ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();

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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
            var htmlHeader120 = "";

            var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            orderMaterials = await _context.Set<Model.OrderMaterial>()
                .Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
                .Include(o => o.Order).ThenInclude(a => a.Uom)
                .Include(a => a.Material)
                .Include(o => o.Order).ThenInclude(a => a.OrderType)
                .Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

            order = await _context.Set<Model.Order>()
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
                .Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
                .Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

            int index = 0;

            // var linkYesNo = "http://localhost:4200/#/ordervalidateS1/" + order.Guid;
            var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateS1/" + order.Guid;
            //var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/ofa";

            //appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



            string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
            string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
														<a class=""button"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >DA</a>" + "&nbsp;" + @"
														<a class=""button-no"" style=""padding: 5px 10px"" href='" + linkYesNo + "'" + "' >NU</a>" + @"
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



            for (int i = 0; i < orderMaterials.Count; i++)
            {
                index++;
                var wip = orderMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
                if (index == orderMaterials.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

            //order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
            emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeS1Id).Select(a => a.Guid).ToListAsync();

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
#if DEBUG
             to = new List<string>();
             to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
             to.Add("radu.alexandru1@emag.ro");
			//to.Add("silvia.damian@emag.ro");
			subject = "TEST - Aprobare comanda: " + emailOrderStatus[0].Order.Code;
#endif
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

            htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + 
                htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + 
                htmlHeader9 + htmlHeader10 + htmlHeader120 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
				var result = await SendS1RequesterNotification(documentNumberId);

                if (result)
                {
					for (int i = 0; i < emailOrderStatus.Count; i++)
					{
						emailOrderStatus[i].NotEmployeeS1Sync = false;
						emailOrderStatus[i].EmployeeS1EmailSend = true;
						_context.Update(emailOrderStatus[i]);

						//order.AppStateId = appState.Id;
						//_context.Update(order);


						_context.SaveChanges();
					}
				}

				
                return result;
            }
            else
            {
                return false;
            }
        }

		public async Task<bool> SendAcceptedNotification(int documentNumberId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			//List<Guid> emails1 = new List<Guid>();
			//List<Guid> emails2 = new List<Guid>();
			//List<Guid> emails = new List<Guid>();
			List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
			var files = new FormFileCollection();
			List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
			// Model.AppState appState = null;
			Model.Order order = null;
            Model.AppState appState = null;
			appState = await _context.Set<Model.AppState>().Where(a => a.Code == "ACCEPTED").SingleAsync();
			emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
				.Include(o => o.Order).ThenInclude(p => p.Partner)
				 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
				 .Include(o => o.Order).ThenInclude(p => p.Company)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
				.AsNoTracking().Where(a => a.IsDeleted == false && a.NotCompletedSync == true && a.EmailSend == false && a.AppStateId == appState.Id && a.SyncErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();

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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">P.O. aprobat</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
			var htmlHeader120 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			orderMaterials = await _context.Set<Model.OrderMaterial>()
				.Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
				.Include(o => o.Order).ThenInclude(a => a.Uom)
				.Include(a => a.Material)
				.Include(o => o.Order).ThenInclude(a => a.OrderType)
				.Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

			order = await _context.Set<Model.Order>()
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
				.Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

			int index = 0;

			// var linkYesNo = "http://localhost:4200/#/ordervalidateS1/" + order.Guid;
			// var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateS1/" + order.Guid;
			//var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

			var link = "https://optima.emag.network/ofa/#/procurement/order";

			//appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



			string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
			string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""3"">Tip</th>
												<th colspan=""3"">Proiect</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""3"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""3"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "P.O. aprobat: " + emailOrderStatus[0].Order.Code;

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



			for (int i = 0; i < orderMaterials.Count; i++)
			{
				index++;
				var wip = orderMaterials[i].WIP ? "DA" : "NU";
				htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
				if (index == orderMaterials.Count)
				{
					htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

			//order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
			//emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeS1Id).Select(a => a.Guid).ToListAsync();

			//if (emails1.Count > 0)
			//{
			//	emails.Add(emails1.ElementAt(0));
			//}

			//for (int e = 0; e < emails.Count; e++)
			//{
			//	var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

			//	if (emp.Email != null && emp.Email != "")
			//	{
			//		to.Add(emp.Email);
			//	}
			//	else
			//	{
			//		to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//	}
			//}

			if (empOwner != null && empOwner != "" && empOwner.ToUpper().Contains("@"))
			{
				to.Add(empOwner);
			}

			if (empRequester != null && empRequester != "" && empRequester.ToUpper().Contains("@"))
			{
				to.Add(empRequester);
			}
#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			to.Add("radu.alexandru1@emag.ro");
			//to.Add("silvia.damian@emag.ro");
			subject = "TEST - P.O. aprobat: " + emailOrderStatus[0].Order.Code;
#endif
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 +
				htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 +
				htmlHeader9 + htmlHeader10 + htmlHeader120 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

			successEmail = await _emailSender.SendEmailAsync(emailMessage);

			to = new List<string>();

			if (successEmail)
			{
				for (int i = 0; i < emailOrderStatus.Count; i++)
				{
					emailOrderStatus[i].NotCompletedSync = false;
					emailOrderStatus[i].EmailSend = true;
					_context.Update(emailOrderStatus[i]);

					//order.AppStateId = appState.Id;
					//_context.Update(order);


					_context.SaveChanges();
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		public async Task<bool> SendB1RequesterNotification(int documentNumberId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			//List<Guid> emails1 = new List<Guid>();
			//List<Guid> emails2 = new List<Guid>();
			//List<Guid> emails = new List<Guid>();
			List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
			var files = new FormFileCollection();
			List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
			//Model.AppState appState = null;
			Model.Order order = null;
			emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
				.Include(o => o.Order).ThenInclude(p => p.Partner)
				 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
				 .Include(o => o.Order).ThenInclude(p => p.Company)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
				.AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeB1Sync == true && a.EmployeeB1EmailSkip == false && a.EmployeeB1EmailSend == false && a.SyncEmployeeB1ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();
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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare initiere flux comanda</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
			var htmlHeader120 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			orderMaterials = await _context.Set<Model.OrderMaterial>()
				.Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
				.Include(o => o.Order).ThenInclude(a => a.Uom)
				.Include(a => a.Material)
				.Include(o => o.Order).ThenInclude(a => a.OrderType)
				.Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

			order = await _context.Set<Model.Order>()
				.Include(e => e.Offer).ThenInclude(r => r.ProjectType)
				.Include(e => e.Offer).ThenInclude(r => r.Division)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
				.Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

			int index = 0;

			// var linkYesNo = "http://localhost:4200/#/ordervalidateL4/" + order.Guid;
			// var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateL4/" + order.Guid;
			//var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

			var link = "https://optima.emag.network/ofa";

			//appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



			string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
			string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";

			//htmlHeader120 = htmlHeader120 + $@"    
			//									<tr>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			//                                             <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>
			//                                             <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>
			//                                             <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
			//									</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



			for (int i = 0; i < orderMaterials.Count; i++)
			{
				index++;
				var wip = orderMaterials[i].WIP ? "DA" : "NU";
				htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
				if (index == orderMaterials.Count)
				{
					htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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


			//emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeL4Id).Select(a => a.Guid).ToListAsync();

			//if (emails1.Count > 0)
			//{
			//	emails.Add(emails1.ElementAt(0));
			//}

			//for (int e = 0; e < emails.Count; e++)
			//{
			//	var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

			//	if (emp.Email != null && emp.Email != "")
			//	{
			//		to.Add(emp.Email);
			//	}
			//	else
			//	{
			//		to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//	}
			//}

			if (empOwner != null && empOwner != "" && empOwner.ToUpper().Contains("@"))
			{
				to.Add(empOwner);
			}

			if (empRequester != null && empRequester != "" && empRequester.ToUpper().Contains("@"))
			{
				to.Add(empRequester);
			}
#if DEBUG
			to = new List<string>();

			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//to.Add("iordache.costin@emag.ro");
			//to.Add("silvia.damian@emag.ro");
#endif
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 +
				htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 +
				htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeader120 +
				htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

			return successEmail = await _emailSender.SendEmailAsync(emailMessage);

			//to = new List<string>();

			//if (successEmail)
			//{
			//	for (int i = 0; i < emailOrderStatus.Count; i++)
			//	{
			//		emailOrderStatus[i].NotEmployeeL4Sync = false;
			//		emailOrderStatus[i].EmployeeL4EmailSend = true;
			//		_context.Update(emailOrderStatus[i]);

			//		//order.AppStateId = appState.Id;
			//		//_context.Update(order);


			//		_context.SaveChanges();
			//	}
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}
		}

		public async Task<bool> SendL4RequesterNotification(int documentNumberId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			//List<Guid> emails1 = new List<Guid>();
			//List<Guid> emails2 = new List<Guid>();
			//List<Guid> emails = new List<Guid>();
			List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
			var files = new FormFileCollection();
			List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
			//Model.AppState appState = null;
			Model.Order order = null;
			emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
				.Include(o => o.Order).ThenInclude(p => p.Partner)
				 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
				 .Include(o => o.Order).ThenInclude(p => p.Company)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
				.AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeL4Sync == true && a.EmployeeL4EmailSkip == false && a.EmployeeL4EmailSend == false && a.SyncEmployeeL4ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();
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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
			var htmlHeader120 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			orderMaterials = await _context.Set<Model.OrderMaterial>()
				.Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
				.Include(o => o.Order).ThenInclude(a => a.Uom)
				.Include(a => a.Material)
				.Include(o => o.Order).ThenInclude(a => a.OrderType)
				.Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

			order = await _context.Set<Model.Order>()
				.Include(e => e.Offer).ThenInclude(r => r.ProjectType)
				.Include(e => e.Offer).ThenInclude(r => r.Division)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
				.Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

			int index = 0;

			// var linkYesNo = "http://localhost:4200/#/ordervalidateL4/" + order.Guid;
			// var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateL4/" + order.Guid;
			//var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

			var link = "https://optima.emag.network/ofa";

			//appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



			string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
			string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"  
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";

			//htmlHeader120 = htmlHeader120 + $@"    
			//									<tr>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			//                                             <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			//									<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>
			//                                             <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>
			//                                             <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
			//									</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



			for (int i = 0; i < orderMaterials.Count; i++)
			{
				index++;
				var wip = orderMaterials[i].WIP ? "DA" : "NU";
				htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
				if (index == orderMaterials.Count)
				{
					htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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


			//emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeL4Id).Select(a => a.Guid).ToListAsync();

			//if (emails1.Count > 0)
			//{
			//	emails.Add(emails1.ElementAt(0));
			//}

			//for (int e = 0; e < emails.Count; e++)
			//{
			//	var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

			//	if (emp.Email != null && emp.Email != "")
			//	{
			//		to.Add(emp.Email);
			//	}
			//	else
			//	{
			//		to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//	}
			//}

            if(empOwner != null && empOwner != "" && empOwner.ToUpper().Contains("@"))
            {
				to.Add(empOwner);
			}

			if (empRequester != null && empRequester != "" && empRequester.ToUpper().Contains("@"))
			{
				to.Add(empRequester);
			}
#if DEBUG
			to = new List<string>();

			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//to.Add("iordache.costin@emag.ro");
			//to.Add("silvia.damian@emag.ro");
#endif
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 +
				htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 +
				htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeader120 +
				htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

			return successEmail = await _emailSender.SendEmailAsync(emailMessage);

			//to = new List<string>();

			//if (successEmail)
			//{
			//	for (int i = 0; i < emailOrderStatus.Count; i++)
			//	{
			//		emailOrderStatus[i].NotEmployeeL4Sync = false;
			//		emailOrderStatus[i].EmployeeL4EmailSend = true;
			//		_context.Update(emailOrderStatus[i]);

			//		//order.AppStateId = appState.Id;
			//		//_context.Update(order);


			//		_context.SaveChanges();
			//	}
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}
		}

		public async Task<bool> SendL3RequesterNotification(int documentNumberId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			//List<Guid> emails1 = new List<Guid>();
			//List<Guid> emails2 = new List<Guid>();
			//List<Guid> emails = new List<Guid>();
			List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
			var files = new FormFileCollection();
			List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
			//Model.AppState appState = null;
			Model.Order order = null;
			emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
				.Include(o => o.Order).ThenInclude(p => p.Partner)
				 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
				 .Include(o => o.Order).ThenInclude(p => p.Company)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
				.AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeL3Sync == true && a.EmployeeL3EmailSkip == false && a.EmployeeL3EmailSend == false && a.SyncEmployeeL3ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();


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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
			var htmlHeader120 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			orderMaterials = await _context.Set<Model.OrderMaterial>()
				.Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
				.Include(o => o.Order).ThenInclude(a => a.Uom)
				.Include(a => a.Material)
				.Include(o => o.Order).ThenInclude(a => a.OrderType)
				.Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

			order = await _context.Set<Model.Order>()
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
				.Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

			int index = 0;

			// var linkYesNo = "http://localhost:4200/#/ordervalidateL3/" + order.Guid;
			// var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateL3/" + order.Guid;
			//var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

			var link = "https://optima.emag.network/ofa/#/procurement/order";

			//appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



			string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
			string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



			for (int i = 0; i < orderMaterials.Count; i++)
			{
				index++;
				var wip = orderMaterials[i].WIP ? "DA" : "NU";
				htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
				if (index == orderMaterials.Count)
				{
					htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

			//order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
			//emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeL4Id).Select(a => a.Guid).ToListAsync();

			//if (emails1.Count > 0)
			//{
			//	emails.Add(emails1.ElementAt(0));
			//}

			//for (int e = 0; e < emails.Count; e++)
			//{
			//	var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

			//	if (emp.Email != null && emp.Email != "")
			//	{
			//		to.Add(emp.Email);
			//	}
			//	else
			//	{
			//		to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//	}
			//}

			if (empOwner != null && empOwner != "" && empOwner.ToUpper().Contains("@"))
			{
				to.Add(empOwner);
			}

			if (empRequester != null && empRequester != "" && empRequester.ToUpper().Contains("@"))
			{
				to.Add(empRequester);
			}
#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			////to.Add("iordache.costin@emag.ro");
			//to.Add("silvia.damian@emag.ro");
#endif
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 +
				htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 +
				htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeader120 +
				htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

			return successEmail = await _emailSender.SendEmailAsync(emailMessage);

			//to = new List<string>();

			//if (successEmail)
			//{
			//	for (int i = 0; i < emailOrderStatus.Count; i++)
			//	{
			//		emailOrderStatus[i].NotEmployeeL3Sync = false;
			//		emailOrderStatus[i].EmployeeL3EmailSend = true;
			//		_context.Update(emailOrderStatus[i]);

			//		//order.AppStateId = appState.Id;
			//		//_context.Update(order);


			//		_context.SaveChanges();
			//	}
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}
		}

		public async Task<bool> SendL2RequesterNotification(int documentNumberId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			//List<Guid> emails1 = new List<Guid>();
			//List<Guid> emails2 = new List<Guid>();
			//List<Guid> emails = new List<Guid>();
			List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
			var files = new FormFileCollection();
			List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
			//Model.AppState appState = null;
			Model.Order order = null;
			emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
				.Include(o => o.Order).ThenInclude(p => p.Partner)
				 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
				 .Include(o => o.Order).ThenInclude(p => p.Company)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
				.AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeL2Sync == true && a.EmployeeL2EmailSkip == false && a.EmployeeL2EmailSend == false && a.SyncEmployeeL2ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();

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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
			var htmlHeader120 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			orderMaterials = await _context.Set<Model.OrderMaterial>()
				.Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
				.Include(o => o.Order).ThenInclude(a => a.Uom)
				.Include(a => a.Material)
				.Include(o => o.Order).ThenInclude(a => a.OrderType)
				.Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

			order = await _context.Set<Model.Order>()
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
				.Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

			int index = 0;

			// var linkYesNo = "http://localhost:4200/#/ordervalidateL2/" + order.Guid;
			// var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateL2/" + order.Guid;
			//var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

			var link = "https://optima.emag.network/ofa";

			//appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



			string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
			string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



			for (int i = 0; i < orderMaterials.Count; i++)
			{
				index++;
				var wip = orderMaterials[i].WIP ? "DA" : "NU";
				htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
				if (index == orderMaterials.Count)
				{
					htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

			//order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
			//emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeL4Id).Select(a => a.Guid).ToListAsync();

			//if (emails1.Count > 0)
			//{
			//	emails.Add(emails1.ElementAt(0));
			//}

			//for (int e = 0; e < emails.Count; e++)
			//{
			//	var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

			//	if (emp.Email != null && emp.Email != "")
			//	{
			//		to.Add(emp.Email);
			//	}
			//	else
			//	{
			//		to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//	}
			//}

			if (empOwner != null && empOwner != "" && empOwner.ToUpper().Contains("@"))
			{
				to.Add(empOwner);
			}

			if (empRequester != null && empRequester != "" && empRequester.ToUpper().Contains("@"))
			{
				to.Add(empRequester);
			}
#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			////to.Add("iordache.costin@emag.ro");
			//to.Add("silvia.damian@emag.ro");
#endif
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 +
				htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 +
				htmlHeader9 + htmlHeader10 + htmlHeader120 +
				htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

			return successEmail = await _emailSender.SendEmailAsync(emailMessage);

			//to = new List<string>();

			//if (successEmail)
			//{
			//	for (int i = 0; i < emailOrderStatus.Count; i++)
			//	{
			//		emailOrderStatus[i].NotEmployeeL2Sync = false;
			//		emailOrderStatus[i].EmployeeL2EmailSend = true;
			//		_context.Update(emailOrderStatus[i]);

			//		//order.AppStateId = appState.Id;
			//		//_context.Update(order);


			//		_context.SaveChanges();
			//	}
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}
		}

		public async Task<bool> SendL1RequesterNotification(int documentNumberId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			//List<Guid> emails1 = new List<Guid>();
			//List<Guid> emails2 = new List<Guid>();
			//List<Guid> emails = new List<Guid>();
			List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
			var files = new FormFileCollection();
			List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
			//Model.AppState appState = null;
			Model.Order order = null;
			emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
				.Include(o => o.Order).ThenInclude(p => p.Partner)
				 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
				 .Include(o => o.Order).ThenInclude(p => p.Company)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
				.AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeL1Sync == true && a.EmployeeL1EmailSkip == false && a.EmployeeL1EmailSend == false && a.SyncEmployeeL1ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();

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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
			var htmlHeader120 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			orderMaterials = await _context.Set<Model.OrderMaterial>()
				.Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
				.Include(o => o.Order).ThenInclude(a => a.Uom)
				.Include(a => a.Material)
				.Include(o => o.Order).ThenInclude(a => a.OrderType)
				.Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

			order = await _context.Set<Model.Order>()
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
				.Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

			int index = 0;

			// var linkYesNo = "http://localhost:4200/#/ordervalidateL1/" + order.Guid;
			// var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateL1/" + order.Guid;
			//var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

			var link = "https://optima.emag.network/ofa";

			//appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



			string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
			string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



			for (int i = 0; i < orderMaterials.Count; i++)
			{
				index++;
				var wip = orderMaterials[i].WIP ? "DA" : "NU";
				htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
				if (index == orderMaterials.Count)
				{
					htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

			// order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
			//emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeL4Id).Select(a => a.Guid).ToListAsync();

			//if (emails1.Count > 0)
			//{
			//	emails.Add(emails1.ElementAt(0));
			//}

			//for (int e = 0; e < emails.Count; e++)
			//{
			//	var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

			//	if (emp.Email != null && emp.Email != "")
			//	{
			//		to.Add(emp.Email);
			//	}
			//	else
			//	{
			//		to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//	}
			//}

			if (empOwner != null && empOwner != "" && empOwner.ToUpper().Contains("@"))
			{
				to.Add(empOwner);
			}

			if (empRequester != null && empRequester != "" && empRequester.ToUpper().Contains("@"))
			{
				to.Add(empRequester);
			}

#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			////to.Add("iordache.costin@emag.ro");
			//to.Add("silvia.damian@emag.ro");
#endif

			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 +
				htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 +
				htmlHeader9 + htmlHeader10 + htmlHeader120 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

			return successEmail = await _emailSender.SendEmailAsync(emailMessage);

			//to = new List<string>();

			//if (successEmail)
			//{
			//	for (int i = 0; i < emailOrderStatus.Count; i++)
			//	{
			//		emailOrderStatus[i].NotEmployeeL1Sync = false;
			//		emailOrderStatus[i].EmployeeL1EmailSend = true;
			//		_context.Update(emailOrderStatus[i]);

			//		//order.AppStateId = appState.Id;
			//		//_context.Update(order);


			//		_context.SaveChanges();
			//	}
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}
		}

		public async Task<bool> SendS3RequesterNotification(int documentNumberId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			//List<Guid> emails1 = new List<Guid>();
			//List<Guid> emails2 = new List<Guid>();
			//List<Guid> emails = new List<Guid>();
			List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
			var files = new FormFileCollection();
			List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
			// Model.AppState appState = null;
			Model.Order order = null;
			emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
				.Include(o => o.Order).ThenInclude(p => p.Partner)
				 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
				 .Include(o => o.Order).ThenInclude(p => p.Company)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
				.AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeS3Sync == true && a.EmployeeS3EmailSkip == false && a.EmployeeS3EmailSend == false && a.SyncEmployeeS3ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();

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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
			var htmlHeader120 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			orderMaterials = await _context.Set<Model.OrderMaterial>()
				.Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
				.Include(o => o.Order).ThenInclude(a => a.Uom)
				.Include(a => a.Material)
				.Include(o => o.Order).ThenInclude(a => a.OrderType)
				.Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

			order = await _context.Set<Model.Order>()
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
				.Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

			int index = 0;

			// var linkYesNo = "http://localhost:4200/#/ordervalidateS3/" + order.Guid;
			// var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateS3/" + order.Guid;
			//var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

			var link = "https://optima.emag.network/ofa";

			//appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



			string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
			string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



			for (int i = 0; i < orderMaterials.Count; i++)
			{
				index++;
				var wip = orderMaterials[i].WIP ? "DA" : "NU";
				htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
				if (index == orderMaterials.Count)
				{
					htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

			//order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
			//emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeL4Id).Select(a => a.Guid).ToListAsync();

			//if (emails1.Count > 0)
			//{
			//	emails.Add(emails1.ElementAt(0));
			//}

			//for (int e = 0; e < emails.Count; e++)
			//{
			//	var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

			//	if (emp.Email != null && emp.Email != "")
			//	{
			//		to.Add(emp.Email);
			//	}
			//	else
			//	{
			//		to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//	}
			//}

			if (empOwner != null && empOwner != "" && empOwner.ToUpper().Contains("@"))
			{
				to.Add(empOwner);
			}

			if (empRequester != null && empRequester != "" && empRequester.ToUpper().Contains("@"))
			{
				to.Add(empRequester);
			}

#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			////to.Add("iordache.costin@emag.ro");
			//to.Add("silvia.damian@emag.ro");
#endif

			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 +
				htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 +
				htmlHeader9 + htmlHeader10 + htmlHeader120 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

			return successEmail = await _emailSender.SendEmailAsync(emailMessage);

			//to = new List<string>();

			//if (successEmail)
			//{
			//	for (int i = 0; i < emailOrderStatus.Count; i++)
			//	{
			//		emailOrderStatus[i].NotEmployeeS3Sync = false;
			//		emailOrderStatus[i].EmployeeS3EmailSend = true;
			//		_context.Update(emailOrderStatus[i]);

			//		//order.AppStateId = appState.Id;
			//		//_context.Update(order);


			//		_context.SaveChanges();
			//	}
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}
		}

		public async Task<bool> SendS2RequesterNotification(int documentNumberId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			//List<Guid> emails1 = new List<Guid>();
			//List<Guid> emails2 = new List<Guid>();
			//List<Guid> emails = new List<Guid>();
			List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
			var files = new FormFileCollection();
			List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
			//Model.AppState appState = null;
			Model.Order order = null;
			emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
				.Include(o => o.Order).ThenInclude(p => p.Partner)
				 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
				 .Include(o => o.Order).ThenInclude(p => p.Company)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
				.AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeS2Sync == true && a.EmployeeS2EmailSkip == false && a.EmployeeS2EmailSend == false && a.SyncEmployeeS2ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();

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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
			var htmlHeader120 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			orderMaterials = await _context.Set<Model.OrderMaterial>()
				.Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
				.Include(o => o.Order).ThenInclude(a => a.Uom)
				.Include(a => a.Material)
				.Include(o => o.Order).ThenInclude(a => a.OrderType)
				.Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

			order = await _context.Set<Model.Order>()
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
				.Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

			int index = 0;

			// var linkYesNo = "http://localhost:4200/#/ordervalidateS2/" + order.Guid;
			// var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateS2/" + order.Guid;
			//var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

			var link = "https://optima.emag.network/ofa";

			//appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



			string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
			string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



			for (int i = 0; i < orderMaterials.Count; i++)
			{
				index++;
				var wip = orderMaterials[i].WIP ? "DA" : "NU";
				htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
				if (index == orderMaterials.Count)
				{
					htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

			//order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
			//emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeL4Id).Select(a => a.Guid).ToListAsync();

			//if (emails1.Count > 0)
			//{
			//	emails.Add(emails1.ElementAt(0));
			//}

			//for (int e = 0; e < emails.Count; e++)
			//{
			//	var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

			//	if (emp.Email != null && emp.Email != "")
			//	{
			//		to.Add(emp.Email);
			//	}
			//	else
			//	{
			//		to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//	}
			//}

			if (empOwner != null && empOwner != "" && empOwner.ToUpper().Contains("@"))
			{
				to.Add(empOwner);
			}

			if (empRequester != null && empRequester != "" && empRequester.ToUpper().Contains("@"))
			{
				to.Add(empRequester);
			}

#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			////to.Add("iordache.costin@emag.ro");
			//to.Add("silvia.damian@emag.ro");
#endif

			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 +
				htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 +
				htmlHeader9 + htmlHeader10 + htmlHeader120 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

			return successEmail = await _emailSender.SendEmailAsync(emailMessage);

			//to = new List<string>();

			//if (successEmail)
			//{
			//	for (int i = 0; i < emailOrderStatus.Count; i++)
			//	{
			//		emailOrderStatus[i].NotEmployeeS2Sync = false;
			//		emailOrderStatus[i].EmployeeS2EmailSend = true;
			//		_context.Update(emailOrderStatus[i]);

			//		//order.AppStateId = appState.Id;
			//		//_context.Update(order);


			//		_context.SaveChanges();
			//	}
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}
		}

		public async Task<bool> SendS1RequesterNotification(int documentNumberId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			//List<Guid> emails1 = new List<Guid>();
			//List<Guid> emails2 = new List<Guid>();
			//List<Guid> emails = new List<Guid>();
			List<Model.OrderMaterial> orderMaterials = new List<Model.OrderMaterial>();
			var files = new FormFileCollection();
			List<Model.EmailOrderStatus> emailOrderStatus = new List<Model.EmailOrderStatus>();
			// Model.AppState appState = null;
			Model.Order order = null;
			emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
				.Include(o => o.Order).ThenInclude(p => p.Partner)
				 .Include(o => o.Order).ThenInclude(p => p.CostCenter)
				 .Include(o => o.Order).ThenInclude(p => p.Company)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.AssetType)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.Division)
				 .Include(o => o.Order).ThenInclude(p => p.Offer).ThenInclude(a => a.ProjectType)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Employee)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Activity)
				 .Include(o => o.Order).ThenInclude(p => p.BudgetBase).ThenInclude(a => a.Project)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
				 .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
				.AsNoTracking().Where(a => a.IsDeleted == false && a.NotEmployeeS1Sync == true && a.EmployeeS1EmailSkip == false && a.EmployeeS1EmailSend == false && a.SyncEmployeeS1ErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();

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
                                                          font-size: 8px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background:#cadff2;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 1px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 8px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 8px;
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
                                                              font-size: 16px;
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
                                    <tr style=""background-color: #2f75b5;"">
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare aprobare comanda</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th>Utilizator</th>
                                        <th>Beneficiar</th>
                                        <th colspan=""4"">Solicitare aprobare comanda</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOrderStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOrderStatus[0].CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
                                        <th>Tichet</th>
									    <th>Tip</th>
										<th>WIP</th>
										<th>Produs</th>
										<th>Cantitate</th>
                                        <th>Moneda</th>
										<th>P.U.</th>
                                        <th>Total</th>
                                        <th>P.U. RON</th>
                                        <th>Total RON</th>
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
			var htmlHeader120 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			orderMaterials = await _context.Set<Model.OrderMaterial>()
				.Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
				.Include(o => o.Order).ThenInclude(a => a.Uom)
				.Include(a => a.Material)
				.Include(o => o.Order).ThenInclude(a => a.OrderType)
				.Where(a => a.IsDeleted == false && a.OrderId == emailOrderStatus[0].OrderId).ToListAsync();

			order = await _context.Set<Model.Order>()
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Employee)
				.Include(e => e.Offer).ThenInclude(r => r.Request).ThenInclude(r => r.Owner)
				.Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();

			int index = 0;

			// var linkYesNo = "http://localhost:4200/#/ordervalidateS1/" + order.Guid;
			// var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateS1/" + order.Guid;
			//var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

			var link = "https://optima.emag.network/ofa";

			//appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();



			string empOwner = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Owner != null ? order.Offer.Request.Owner.Email : "";
			string empRequester = order != null && order.Offer != null && order.Offer.Request != null && order.Offer.Request.Employee != null ? order.Offer.Request.Employee.Email : "";


			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"" colspan=""4"" style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th>Furnizor</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Company.Code}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.Division.Name}</th>
												<th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Partner.RegistryNumber + " - " + emailOrderStatus[0].Order.Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.AssetType.Name}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{emailOrderStatus[0].Order.Offer.ProjectType.Name}</th>
                                                <th colspan=""2"">
													</th>
												</tr>";

			//htmlHeader8 = htmlHeader8 + $@"     
			//<tr style=""background-color: #2f75b5;"">
			//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
			//</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
								<tr style=""background-color: #2f75b5;"">
								<th colspan=""11"" style=""color: #ffffff;text-align: left"">Flux de aprobare</th>
								</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
								<tr style=""background-color:#cadff2;color: #ffffff;"">
								<th>L4</th>
                                <th>S3</th>
                                <th>L3</th>
                                <th>S2</th>
                                <th>L2</th>
                                <th colspan=""2"">L1</th>
                                <th colspan=""2"">S1</th>
								</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL4 != null && !emailOrderStatus[0].EmployeeL4EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL4.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS3 != null && !emailOrderStatus[0].EmployeeS3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS3.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL3 != null && !emailOrderStatus[0].EmployeeL3EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL3.Email : "-")}</th>
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS2 != null && !emailOrderStatus[0].EmployeeS2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS2.Email : "-")}</th>								
                                <th style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL2 != null && !emailOrderStatus[0].EmployeeL2EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL2.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeL1 != null && !emailOrderStatus[0].EmployeeL1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeL1.Email : "-")}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{(emailOrderStatus[0].Matrix.EmployeeS1 != null && !emailOrderStatus[0].EmployeeS1EmailSkip ? emailOrderStatus[0].Matrix.EmployeeS1.Email : "-")}</th>
								</tr>";

			htmlHeader120 = htmlHeader120 + $@"    
								<tr>
								<th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL4ValidateAt != null ? emailOrderStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS3ValidateAt != null ? emailOrderStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL3ValidateAt != null ? emailOrderStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS2ValidateAt != null ? emailOrderStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>								
                                <th style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL2ValidateAt != null ? emailOrderStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeL1ValidateAt != null ? emailOrderStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			                    <th colspan=""2"" style=""font-weight: normal;"">{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOrderStatus[0].EmployeeS1ValidateAt != null ? emailOrderStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
								</tr>";


			subject = "Aprobare comanda: " + emailOrderStatus[0].Order.Code;

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOrderStatus[0].Order.Code}</th>";



			for (int i = 0; i < orderMaterials.Count; i++)
			{
				index++;
				var wip = orderMaterials[i].WIP ? "DA" : "NU";
				htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
									<th style=""font-weight: normal;"">{wip}</th>
									<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
				if (index == orderMaterials.Count)
				{
					htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color:#cadff2;""></th>
                                    <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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

			//order = await _context.Set<Model.Order>().Where(a => a.Id == emailOrderStatus[0].OrderId).SingleOrDefaultAsync();
			//emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOrderStatus[0].Matrix.EmployeeL4Id).Select(a => a.Guid).ToListAsync();

			//if (emails1.Count > 0)
			//{
			//	emails.Add(emails1.ElementAt(0));
			//}

			//for (int e = 0; e < emails.Count; e++)
			//{
			//	var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

			//	if (emp.Email != null && emp.Email != "")
			//	{
			//		to.Add(emp.Email);
			//	}
			//	else
			//	{
			//		to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			//	}
			//}

			if (empOwner != null && empOwner != "" && empOwner.ToUpper().Contains("@"))
			{
				to.Add(empOwner);
			}

			if (empRequester != null && empRequester != "" && empRequester.ToUpper().Contains("@"))
			{
				to.Add(empRequester);
			}
#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			////to.Add("iordache.costin@emag.ro");
			//to.Add("silvia.damian@emag.ro");
#endif
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 +
				htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 +
				htmlHeader9 + htmlHeader10 + htmlHeader120 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

			return successEmail = await _emailSender.SendEmailAsync(emailMessage);

			//to = new List<string>();

			//if (successEmail)
			//{
			//	for (int i = 0; i < emailOrderStatus.Count; i++)
			//	{
			//		emailOrderStatus[i].NotEmployeeS1Sync = false;
			//		emailOrderStatus[i].EmployeeS1EmailSend = true;
			//		_context.Update(emailOrderStatus[i]);

			//		//order.AppStateId = appState.Id;
			//		//_context.Update(order);


			//		_context.SaveChanges();
			//	}
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}
		}
	}
}
