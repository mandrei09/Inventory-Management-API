using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Optima.Fais.Api.Services;
using Optima.Fais.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class OrdersService : IOrdersService
    {
		private readonly ApplicationDbContext _context;
		private readonly IEmailSender _emailSender;

		public OrdersService(ApplicationDbContext context, IEmailSender emailSender)
		{
			this._context = context;
			this._emailSender = emailSender;
		}
        public async Task<bool> SendOrderNeedBudget(int orderId)
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
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Employee)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Activity)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Division)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Department)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
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
            var link = "https://optima.emag.network/ofa/#/procurement/offer/status";
            //var link = "https://optima.emag.network/ofa";

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "NEED_BUDGET").SingleAsync();



            string empIni = order.Employee != null ? order.Employee.FirstName + " " + order.Employee.LastName : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{order.Employee.Email}</th>
                                                <th rowspan=""2"" colspan=""4"">{String.Format("{0:#,##0.##}", order.BudgetValueNeed)}</th>
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
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Buget disponibil RON</th>
												<th rowspan=""2"" colspan=""2"">
												</th>
												</tr>";

            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th >{order.BudgetForecast.BudgetBase.Code}</th>
												<th colspan=""2"">{order.BudgetForecast.BudgetBase.Employee.Email}</th>
												<th>{order.BudgetForecast.BudgetBase.Project.Code}</th>
                                                <th>{order.BudgetForecast.BudgetBase.Activity.Name}</th>
												<th>{order.BudgetForecast.BudgetBase.Info}</th>
                                                <th>{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon) - order.BudgetValueNeed)}</th>
												</th>
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

            List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.OrderId == orderId).ToList();

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
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendOrderResponseNeedBudget(int requestId, int budgetBaseId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            var files = new FormFileCollection();
            Model.Request request = null;
            Model.BudgetBase budgetBase = null;
            request = await _context.Set<Model.Request>()
                .Include(o => o.AssetType)
                .Include(c => c.Company)
                .Include(c => c.CostCenter).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
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
            //var linkYes = "https://optima.emag.network/ofa";
            var linkYes = "https://optima.emag.network/procurement/order";
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
            var region = budgetBase.Region != null ? budgetBase.Region.Code : budgetBase.AdmCenter.Code;
            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
                                                <th >{budgetBase.Project.Code}</th>
												<th >{request.CostCenter.Division.Department.Code + "|" + request.CostCenter.Division.Department.Name}</th>
												<th>{request.CostCenter.Division.Code + "|" + request.CostCenter.Division.Name}</th>
                                                <th>{request.ProjectType.Code + "|" + request.ProjectType.Name}</th>
                                                <th>{request.AssetType.Code + "|" + request.AssetType.Name}</th>
                                                <th>{budgetBase.AdmCenter.Code}</th>
                                                <th>{region}</th>
                                                <th>{budgetBase.Activity.Code}</th>
                                                <th>{budgetBase.Country.Code}</th>
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


            emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == request.EmployeeId).Select(a => a.Guid).ToList();

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
            to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
            //to.Add("silvia.damian@emag.ro");

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
    }
}
