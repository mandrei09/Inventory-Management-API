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
	public class EmailRequestStatusService : IEmailRequestStatusService
	{
		private readonly ApplicationDbContext _context;
		private readonly IEmailSender _emailSender;
        public IServiceProvider _services { get; }

        private readonly string _basePath;
        private readonly string _resourcesFolder;
        private readonly string _resourcesPath;

        public EmailRequestStatusService(ApplicationDbContext context, IEmailSender emailSender, IConfiguration configuration, IServiceProvider services)
		{
			this._context = context;
			this._emailSender = emailSender;
            _services = services;
            this._basePath = configuration.GetSection("BasePath").GetValue<string>("Base");
            this._resourcesFolder = configuration.GetSection("BasePath").GetValue<string>("Resources");
            this._resourcesPath = $"{this._basePath}{this._resourcesFolder}";
        }

		//    public async Task<bool> SendNeedBudgetNotification(int requestId, int? requestbudgetForecastId)
		//    {
		//        bool successEmail = false;
		//        List<string> to = new List<string>();
		//        List<string> cc = new List<string>();
		//        List<string> bcc = new List<string>();
		//        List<int> emails1 = new List<int>();
		//        List<int> emails2 = new List<int>();
		//        List<int> emails = new List<int>();
		//        var files = new FormFileCollection();
		//        List<Model.RequestBudgetForecastMaterial> reqBFMaterials = null;
		//        Model.AppState appState = null;
		//        // Model.Order order = null;
		//        Model.RequestBudgetForecast requestBudgetForecast = null;
		//        Model.EmailRequestStatus emailRequestStatus = null;

		//        //order = await _context.Set<Model.Order>()
		//        //    .Include(o => o.Offer).ThenInclude(r => r.Request)
		//        //    .Include(o => o.Offer).ThenInclude(r => r.AssetType)
		//        //    .Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
		//        //    .Include(o => o.Offer).ThenInclude(r => r.Region)
		//        //    .Include(c => c.Uom)
		//        //    .Include(c => c.Partner)
		//        //    .Include(c => c.Company)
		//        //    .Include(c => c.CostCenter)
		//        //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Employee)
		//        //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
		//        //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Activity)
		//        //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Division)
		//        //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Department)
		//        //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
		//        //    .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
		//        //    .Include(c => c.Employee)
		//        //    .Include(c => c.Project)
		//        //    .Include(c => c.OrderType).AsNoTracking().Where(a => a.Id == orderId).SingleOrDefaultAsync();
		//        requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
		//           //.Include(o => o.BudgetForecast).ThenInclude(r => r.Request)
		//           //.Include(o => o.Offer).ThenInclude(r => r.AssetType)
		//           //.Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
		//           //.Include(o => o.Offer).ThenInclude(r => r.Region)
		//           //.Include(c => c.Uom)
		//           //.Include(c => c.Partner)
		//           //.Include(c => c.Company)
		//           //.Include(c => c.CostCenter)
		//           .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Employee)
		//           .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
		//           .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Activity)
		//           .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Division)
		//           .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Department)
		//           .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
		//           .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
		//           .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Region)
		//           .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.AdmCenter)
		//           //.Include(c => c.Employee)
		//           //.Include(c => c.Project)
		//           //.Include(c => c.OrderType)
		//           .AsNoTracking().Where(a => a.Id == requestbudgetForecastId).SingleAsync();
		//        var htmlMessage = "";
		//        var subject = "";
		//        var htmlBodyEmail1 = "";
		//        var htmlBodyEnd = "";
		//        var htmlBodyCompany1 = "";
		//        var htmlBodyCompany2 = "";
		//        var htmlBody1 = @"
		//                                    <html lang=""en"">
		//                                        <head>    
		//                                            <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
		//                                            <title>
		//                                                OPTIMA
		//                                            </title>
		//                                            <style type=""text/css"">
		//                                                table.redTable {
		//                                                      border: 2px solid #04327d;
		//                                                      background-color: #FFFFFF;
		//                                                      width: 100%;
		//                                                      text-align: center;
		//                                                      border-collapse: collapse;
		//                                                    }
		//                                                    table.redTable td, table.redTable th {
		//                                                      border: 1px solid #04327d;
		//                                                      padding: 3px 2px;
		//                                                    }
		//                                                    table.redTable tbody td {
		//                                                      font-size: 13px;
		//                                                    }
		//                                                    table.redTable tr:nth-child(even) {
		//                                                      background: #F5C8BF;
		//                                                    }
		//                                                    table.redTable thead {
		//                                                      background: #ffffff;
		//                                                    }
		//                                                    table.redTable thead th {
		//                                                      font-size: 16px;
		//                                                      font-weight: bold;
		//                                                      color: #04327d;
		//                                                      text-align: center;
		//                                                      border-left: 2px solid #04327d;
		//                                                    }

		//                                                    table.redTable tfoot {
		//                                                      font-size: 13px;
		//                                                      font-weight: bold;
		//                                                      color: #04327d;
		//                                                      background: #ffffff;
		//                                                    }
		//                                                    table.redTable tfoot td {
		//                                                      font-size: 13px;
		//                                                    }
		//                                                    table.redTable tfoot .links {
		//                                                      text-align: right;
		//                                                    }
		//                                                    table.redTable tfoot .links a{
		//                                                      display: inline-block;
		//                                                      background: #FFFFFF;
		//                                                      color: #04327d;
		//                                                      padding: 2px 8px;
		//                                                      border-radius: 5px;
		//                                                    }
		//									.button {
		//                                                          background-color: #04327d;
		//                                                          border: none;
		//                                                          color: #ffffff;
		//                                                          padding: 5px 10px;
		//                                                          text-align: center;
		//                                                          text-decoration: none;
		//                                                          display: inline-block;
		//                                                          font-size: 16px;
		//                                                          margin: 4px 2px;
		//                                                          cursor: pointer;
		//                                                        }
		//                                               .button-no {
		//                                                          background-color: #6491D9;
		//                                                          border: none;
		//                                                          color: white;
		//                                                          padding: 5px 10px;
		//                                                          text-align: center;
		//                                                          text-decoration: none;
		//                                                          display: inline-block;
		//                                                          font-size: 16px;
		//                                                          margin: 4px 2px;
		//                                                          cursor: pointer;
		//                                                        }
		//                                            </style>
		//                                        </head>
		//                                        <body>
		//                                            <table class=""redTable"">
		//                                                <thead>

		//                                    ";
		//        var htmlHeader = @"        
		//                                <tr style=""background-color: #04327d;font-color: #ffffff; font-weight: bold"">
		//                                    <th colspan=""11"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">SOLICITARE SUPLIMENTARE BUGET</th>                                      
		//					</tr>";

		//        var htmlHeader1 = $@"        
		//                                <tr>
		//                                    <th rowspan=""2"">Numar comanda</th>
		//					    <th colspan=""3"">Data solicitare</th>
		//						<th colspan=""2"">Solicitant</th>
		//                                    <th colspan=""4"">Solicitare suplimentare buget RON</th>
		//                                    <th></th>
		//					</tr>
		//					<tr>
		//                                    <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
		//					    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
		//						<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

		//        var htmlHeader11 = "";

		//        var htmlHeader12 = "";
		//        var htmlHeader13 = $@"        
		//					    <th>{String.Format("{0:dd}", requestBudgetForecast.CreatedAt)}</th>
		//						<th>{String.Format("{0:MM}", requestBudgetForecast.CreatedAt)}</th>
		//						<th>{String.Format("{0:yyyy}", requestBudgetForecast.CreatedAt)}</th>
		//					</tr>";

		//        var htmlHeader2 = @"      
		//					<tr>
		//                                    <th colspan=""11""></th>
		//					</tr>
		//                                <tr style=""background-color: #6491D9;"">
		//                                    <th style=""color: #ffffff;"">Nr. Crt.</th>
		//                                    <th style=""color: #ffffff;"">Cod P.R.</th>
		//					    <th style=""color: #ffffff;"">Tip comanda</th>
		//						<th style=""color: #ffffff;"">WIP</th>
		//						<th style=""color: #ffffff;"">Cod Produs</th>
		//						<th style=""color: #ffffff;"">Cantitate</th>
		//                                    <th style=""color: #ffffff;"">Moneda</th>
		//						<th style=""color: #ffffff;"">P.U. Valuta</th>
		//                                    <th style=""color: #ffffff;"">Total comanda Valuta</th>
		//                                    <th style=""color: #ffffff;"">P.U. RON</th>
		//                                    <th style=""color: #ffffff;"">Total comanda RON</th>
		//					</tr>";

		//        var htmlHeader3 = "";

		//        var htmlHeader4 = @"<tr>
		//						<th colspan = ""11""></th>
		//				</tr>";

		//        var htmlHeader5 = "";
		//        var htmlHeader6 = "";
		//        var htmlHeader7 = "";
		//        var htmlHeader8 = "";
		//        var htmlHeader9 = "";
		//        var htmlHeader10 = "";

		//        var htmlHeaderEnd = @"
		//                            </thead>
		//                            <tbody>";

		//        reqBFMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
		//            .Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
		//            .Include(o => o.Order).ThenInclude(a => a.Uom)
		//            .Include(o => o.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.AssetType)
		//            .Include(a => a.Material)
		//            .Include(o => o.Order).ThenInclude(a => a.OrderType)
		//            .Include(o => o.Order).ThenInclude(a => a.Partner)
		//            .Include(o => o.Order).ThenInclude(a => a.Company)
		//            .Include(o => o.Order).ThenInclude(a => a.Employee)
		//            .Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestbudgetForecastId)
		//            .ToListAsync();

		//        emailRequestStatus = await _context.Set<Model.EmailRequestStatus>()
		//            .Where(a => a.RequestId == requestId && a.RequestBudgetForecastId == requestbudgetForecastId && a.IsDeleted == false && a.NeedBudgetEmailSend == false && a.NotNeedBudgetSync == true)
		//            .SingleAsync();


		//        int index = 0;

		//        //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
		//        ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
		//        //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

		//        var link = "https://optima.emag.network/ofa";

		//        appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "NEED_BUDGET").SingleAsync();

		//        string empIni = reqBFMaterials[0].Order.Employee != null ? reqBFMaterials[0].Order.Employee.FirstName + " " + reqBFMaterials[0].Order.Employee.LastName : "";


		//        htmlHeader11 = htmlHeader11 + $@"        
		//								<th rowspan=""2"" colspan=""2"">{reqBFMaterials[0].Order.Employee.Email}</th>
		//                                            <th rowspan=""2"" colspan=""4"">{String.Format("{0:#,##0.##}", requestBudgetForecast.NeedBudgetValue)}</th>
		//                                            <th rowspan=""2"">
		//                                                 <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
		//								</th>";

		//        htmlHeader5 = htmlHeader5 + $@"
		//								<tr style=""background-color: #04327d;"">
		//								<th colspan=""11"" style=""color: #ffffff;"">Detalii OFERTA</th>
		//								</tr>";
		//        htmlHeader6 = htmlHeader6 + $@"
		//								<tr>
		//								<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Furnizor</th>
		//								<th style=""background-color: #6491D9;color: #ffffff;"">Companie</th>
		//								<th style=""background-color: #6491D9;color: #ffffff;"">Tip</th>
		//								</tr>";

		//        htmlHeader7 = htmlHeader7 + $@" 
		//								<tr>
		//								<th colspan=""2"">{reqBFMaterials[0].Order.Partner.Name}</th>
		//								<th>{reqBFMaterials[0].Order.Company.Code}</th>
		//								<th>{reqBFMaterials[0].Order.Offer.AssetType.Name}</th>
		//								</tr>";

		//        htmlHeader8 = htmlHeader8 + $@"     
		//								<tr style=""background-color: #04327d;"">
		//								<th colspan=""11"" style=""color: #ffffff;"">Detalii BUGET</th>
		//								</tr>";

		//        htmlHeader9 = htmlHeader9 + $@"  
		//								<tr>
		//								<th style=""background-color: #6491D9;color: #ffffff;"">Cod OPTIMA</th>
		//								<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
		//								<th style=""background-color: #6491D9;color: #ffffff;"">WBS</th>
		//                                            <th style=""background-color: #6491D9;color: #ffffff;"">Activity</th>
		//                                             <th style=""background-color: #6491D9;color: #ffffff;"">PC</th>
		//                                             <th style=""background-color: #6491D9;color: #ffffff;"">PC Det</th>
		//                                            <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
		//                                            <th style=""background-color: #6491D9;color: #ffffff;"">Buget disponibil RON</th>
		//								<th rowspan=""2"" colspan=""2"">
		//								</th>
		//								</tr>";

		//        htmlHeader10 = htmlHeader10 + $@"    
		//								<tr>
		//								<th >{requestBudgetForecast.BudgetForecast.BudgetBase.Code}</th>
		//								<th colspan=""2"">{requestBudgetForecast.BudgetForecast.BudgetBase.Employee.Email}</th>
		//								<th>{requestBudgetForecast.BudgetForecast.BudgetBase.Project.Code}</th>
		//                                            <th>{requestBudgetForecast.BudgetForecast.BudgetBase.Activity.Name}</th>
		//                                            <th>{requestBudgetForecast.BudgetForecast.BudgetBase.AdmCenter.Name}</th>
		//                                            <th>{requestBudgetForecast.BudgetForecast.BudgetBase.Region.Name}</th>
		//								<th>{requestBudgetForecast.BudgetForecast.BudgetBase.Info}</th>
		//                                            <th>{String.Format("{0:#,##0.##}", requestBudgetForecast.ValueRon - requestBudgetForecast.NeedBudgetValue)}</th>
		//								</tr>";


		//        subject = "Suplimentare buget pentru comanda: " + reqBFMaterials[0].Order.Code + "!!";

		//        htmlHeader12 = htmlHeader12 + $@"        
		//			</tr>
		//			<tr>
		//                            <th>{reqBFMaterials[0].Order.Code}</th>";



		//        for (int i = 0; i < reqBFMaterials.Count; i++)
		//        {
		//            index++;
		//            var wip = reqBFMaterials[i].WIP ? "DA" : "NU";
		//            htmlHeader3 += $@"      
		//                            <tr>
		//                                <th>{index}</th>
		//					<th>{reqBFMaterials[i].Order.Offer.Request.Code}</th>
		//                                <th>{reqBFMaterials[i].Order.OrderType.Name}</th>
		//					<th>{wip}</th>
		//					<th>{reqBFMaterials[i].Material.Code}</th>
		//                                <th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].Quantity)}</th>
		//                                <th>{reqBFMaterials[i].Order.Uom.Code}</th>
		//					<th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].Price)}</th>
		//					<th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].Value)}</th>
		//                                <th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].PriceRon)}</th>
		//                                <th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].ValueRon)}</th>
		//				</tr>";
		//            if (index == reqBFMaterials.Count)
		//            {
		//                htmlHeader3 += $@"      
		//                            <tr>
		//                                <th colspan=""4""></th>
		//                                <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
		//                                <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", reqBFMaterials.Sum(a => a.Quantity))}</th>
		//                                <th style=""background-color: #6491D9;color: #ffffff;""></th>
		//                                <th style=""background-color: #6491D9;color: #ffffff;""></th>
		//                                <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", reqBFMaterials.Sum(a => a.Value))}</th>
		//                                <th style=""background-color: #6491D9;color: #ffffff;""></th>
		//                                <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", reqBFMaterials.Sum(a => a.ValueRon))}</th>
		//				</tr>";
		//            }
		//        };

		//        //    htmlHeader3 = htmlHeader3 + $@"      
		//        //                        <tr>
		//        //                            <th>{index}</th>
		//        //	<th colspan=""2"">{order.OrderType.Name}</th>
		//        //	<th>{order.Offer.Code}</th>
		//        //	<th>{order.Offer.Request.Code}</th>
		//        //                            <th colspan=""2"">{order.Uom.Code}</th>
		//        //	<th>{String.Format("{0:#,##0.##}", order.Price)}</th>
		//        //</tr>";


		//        emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Email == "madalina.udrea@emag.ro").Select(a => a.Id).ToListAsync();

		//        if (emails1.Count > 0)
		//        {
		//            emails.Add(emails1.ElementAt(0));
		//        }

		//        for (int e = 0; e < emails.Count; e++)
		//        {
		//            var emp = _context.Set<Model.Employee>().Where(employee => employee.Id == emails.ElementAt(e)).Single();

		//            if (emp.Email != null && emp.Email != "")
		//            {
		//                to.Add(emp.Email);
		//            }
		//            else
		//            {
		//                to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
		//            }
		//        }

		//        //to = new List<string>();

		//        //to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
		//        //to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
		//        ////to.Add("iordache.costin@emag.ro");
		//        //to.Add("silvia.damian@emag.ro");

		//        bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

		//        List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.RequestBudgetForecastId == requestbudgetForecastId).ToList();

		//        MemoryStream ms = new();

		//        List<(string, string, string)> filePaths = new List<(string, string, string)>();

		//        for (int i = 0; i < entityFiles.Count; i++)
		//        {
		//            var filePath = Path.Combine("order", entityFiles[i].StoredAs);
		//            filePaths.Add(new(filePath, entityFiles[i].Name, entityFiles[i].FileType));
		//        }

		//        htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

		//        var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, filePaths);

		//        successEmail = await _emailSender.SendEmailAsync(emailMessage);

		//        to = new List<string>();

		//        if (successEmail)
		//        {
		//            emailRequestStatus.NotNeedBudgetSync = false;
		//emailRequestStatus.NeedBudgetEmailSend = true;
		//            _context.Update(emailRequestStatus);
		//            _context.SaveChanges();
		//            return true;
		//        }
		//        else
		//        {
		//            return false;
		//        }
		//    }

		//public async Task<bool> SendNeedBudgetNotification(int requestId, int? requestbudgetForecastId)
		//{
		//	bool successEmail = false;
		//	List<string> to = new List<string>();
		//	List<string> cc = new List<string>();
		//	List<string> bcc = new List<string>();
		//	List<int> emails1 = new List<int>();
		//	List<int> emails2 = new List<int>();
		//	List<int> emails = new List<int>();
		//	var files = new FormFileCollection();
		//	//List<Model.OrderMaterial> orderMaterials = null;
		//	Model.AppState appState = null;
		//	Model.Request request = null;
		//	Model.RequestBudgetForecast requestBudgetForecast = null;
		//	request = await _context.Set<Model.Request>()
		//		.Include(o => o.AssetType)
		//		.Include(c => c.Company)
		//		.Include(d => d.Division).ThenInclude(d => d.Department)
		//		//.Include(c => c.CostCenter).ThenInclude(d => d.AdmCenter)
		//		// .Include(c => c.CostCenter).ThenInclude(d => d.Region)
		//		.Include(c => c.Employee)
		//		.Include(c => c.ProjectType)
		//		.Include(c => c.AppState)
		//		.Include(c => c.Owner)
		//		.Include(c => c.StartAccMonth)
		//		.AsNoTracking().Where(a => a.Id == requestId).SingleOrDefaultAsync();
		//	requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>().Where(a => a.Id == requestbudgetForecastId).FirstOrDefaultAsync();
		//	var htmlMessage = "";
		//	var subject = "";
		//	var htmlBodyEmail1 = "";
		//	var htmlBodyEnd = "";
		//	var htmlBodyCompany1 = "";
		//	var htmlBodyCompany2 = "";
		//	var htmlBody1 = @"
		//                                      <html lang=""en"">
		//                                          <head>    
		//                                              <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
		//                                              <title>
		//                                                  OPTIMA
		//                                              </title>
		//                                              <style type=""text/css"">
		//                                                  table.redTable {
		//                                                        border: 2px solid #04327d;
		//                                                        background-color: #FFFFFF;
		//                                                        width: 100%;
		//                                                        text-align: center;
		//                                                        border-collapse: collapse;
		//                                                      }
		//                                                      table.redTable td, table.redTable th {
		//                                                        border: 1px solid #04327d;
		//                                                        padding: 3px 2px;
		//                                                      }
		//                                                      table.redTable tbody td {
		//                                                        font-size: 10px;
		//                                                      }
		//                                                      table.redTable tr:nth-child(even) {
		//                                                        background: #F5C8BF;
		//                                                      }
		//                                                      table.redTable thead {
		//                                                        background: #ffffff;
		//                                                      }
		//                                                      table.redTable thead th {
		//                                                        font-size: 13px;
		//                                                        font-weight: bold;
		//                                                        color: #04327d;
		//                                                        text-align: center;
		//                                                        border-left: 2px solid #04327d;
		//                                                      }

		//                                                      table.redTable tfoot {
		//                                                        font-size: 10px;
		//                                                        font-weight: bold;
		//                                                        color: #04327d;
		//                                                        background: #ffffff;
		//                                                      }
		//                                                      table.redTable tfoot td {
		//                                                        font-size: 10px;
		//                                                      }
		//                                                      table.redTable tfoot .links {
		//                                                        text-align: right;
		//                                                      }
		//                                                      table.redTable tfoot .links a{
		//                                                        display: inline-block;
		//                                                        background: #FFFFFF;
		//                                                        color: #04327d;
		//                                                        padding: 2px 8px;
		//                                                        border-radius: 5px;
		//                                                      }
		//											.button {
		//                                                            background-color: #04327d;
		//                                                            border: none;
		//                                                            color: #ffffff;
		//                                                            padding: 5px 10px;
		//                                                            text-align: center;
		//                                                            text-decoration: none;
		//                                                            display: inline-block;
		//                                                            font-size: 13px;
		//                                                            margin: 4px 2px;
		//                                                            cursor: pointer;
		//                                                          }
		//                                                 .button-no {
		//                                                            background-color: #6491D9;
		//                                                            border: none;
		//                                                            color: white;
		//                                                            padding: 5px 10px;
		//                                                            text-align: center;
		//                                                            text-decoration: none;
		//                                                            display: inline-block;
		//                                                            font-size: 13px;
		//                                                            margin: 4px 2px;
		//                                                            cursor: pointer;
		//                                                          }
		//                                              </style>
		//                                          </head>
		//                                          <body>
		//                                              <table class=""redTable"">
		//                                                  <thead>

		//                                      ";
		//	var htmlHeader = @"        
		//                                  <tr style=""background-color: #04327d;font-color: #ffffff; font-weight: bold"">
		//                                      <th colspan=""11"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">SOLICITARE SUPLIMENTARE BUGET</th>                                      
		//							</tr>";

		//	var htmlHeader1 = $@"        
		//                                  <tr>
		//                                      <th rowspan=""2"">Numar comanda</th>
		//							    <th colspan=""3"">Data solicitare</th>
		//								<th colspan=""2"">Solicitant</th>
		//                                      <th colspan=""4"">Solicitare suplimentare buget RON</th>
		//                                      <th></th>
		//							</tr>
		//							<tr>
		//                                      <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
		//							    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
		//								<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

		//	var htmlHeader11 = "";

		//	var htmlHeader12 = "";
		//	var htmlHeader13 = $@"        
		//							    <th>{String.Format("{0:dd}", request.CreatedAt)}</th>
		//								<th>{String.Format("{0:MM}", request.CreatedAt)}</th>
		//								<th>{String.Format("{0:yyyy}", request.CreatedAt)}</th>
		//							</tr>";

		//	var htmlHeader2 = @"      
		//							<tr>
		//                                      <th colspan=""11""></th>
		//							</tr>
		//                                  <tr style=""background-color: #6491D9;"">
		//                                      <th style=""color: #ffffff;"">Nr. Crt.</th>
		//                                      <th style=""color: #ffffff;"">Cod ticket</th>
		//							    <th style=""color: #ffffff;"">Tip comanda</th>
		//								<th style=""color: #ffffff;"">WIP</th>
		//								<th style=""color: #ffffff;"">Cod Produs</th>
		//								<th style=""color: #ffffff;"">Cantitate</th>
		//                                      <th style=""color: #ffffff;"">Moneda</th>
		//								<th style=""color: #ffffff;"">P.U. Valuta</th>
		//                                      <th style=""color: #ffffff;"">Total comanda Valuta</th>
		//                                      <th style=""color: #ffffff;"">P.U. RON</th>
		//                                      <th style=""color: #ffffff;"">Total comanda RON</th>
		//							</tr>";

		//	var htmlHeader3 = "";

		//	var htmlHeader4 = @"<tr>
		//								<th colspan = ""11""></th>
		//						</tr>";

		//	var htmlHeader5 = "";
		//	var htmlHeader6 = "";
		//	var htmlHeader7 = "";
		//	var htmlHeader8 = "";
		//	var htmlHeader9 = "";
		//	var htmlHeader10 = "";

		//	var htmlHeaderEnd = @"
		//                              </thead>
		//                              <tbody>";

		//	orderMaterials = await _context.Set<Model.OrderMaterial>().Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request).Include(o => o.Order).ThenInclude(a => a.Uom).Include(a => a.Material).Include(o => o.Order).ThenInclude(a => a.OrderType).Where(a => a.IsDeleted == false && a.OrderId == order.Id).ToListAsync();

		//	int index = 0;

		//	//var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
		//	////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
		//	//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

		//	var link = "https://optima.emag.network/ofa";

		//	appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "NEED_BUDGET").SingleAsync();



		//	string empIni = request.Employee != null ? request.Employee.FirstName + " " + request.Employee.LastName : "";


		//	htmlHeader11 = htmlHeader11 + $@"        
		//										<th rowspan=""2"" colspan=""2"">{request.Employee.Email}</th>
		//                                              <th rowspan=""2"" colspan=""4"">{String.Format("{0:#,##0.##}", requestBudgetForecast.NeedBudgetValue)}</th>
		//                                              <th rowspan=""2"">
		//                                                   <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
		//										</th>";

		//	htmlHeader5 = htmlHeader5 + $@"
		//										<tr style=""background-color: #04327d;"">
		//										<th colspan=""11"" style=""color: #ffffff;"">Detalii OFERTA</th>
		//										</tr>";
		//	htmlHeader6 = htmlHeader6 + $@"
		//										<tr>
		//										<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Furnizor</th>
		//										<th style=""background-color: #6491D9;color: #ffffff;"">Companie</th>
		//										<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
		//										<th style=""background-color: #6491D9;color: #ffffff;"">Tip</th>
		//										<th style=""background-color: #6491D9;color: #ffffff;"">PC</th>
		//                                              <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">PC Det</th>
		//										</tr>";

		//	htmlHeader7 = htmlHeader7 + $@" 
		//										<tr>
		//										<th colspan=""2"">{order.Partner.Name}</th>
		//										<th>{order.Company.Code}</th>
		//										<th>{order.CostCenter.Code}</th>
		//										<th>{order.Offer.AssetType.Name}</th>
		//                                              <th>{order.Offer.AdmCenter.Code}</th>
		//                                              <th colspan=""2"">{order.Offer.Region.Code}</th>
		//										</tr>";

		//	htmlHeader8 = htmlHeader8 + $@"     
		//										<tr style=""background-color: #04327d;"">
		//										<th colspan=""11"" style=""color: #ffffff;"">Detalii BUGET</th>
		//										</tr>";

		//	htmlHeader9 = htmlHeader9 + $@"  
		//										<tr>
		//										<th style=""background-color: #6491D9;color: #ffffff;"">Cod OPTIMA</th>
		//										<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
		//										<th style=""background-color: #6491D9;color: #ffffff;"">WBS</th>
		//                                              <th style=""background-color: #6491D9;color: #ffffff;"">Activity</th>
		//                                              <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
		//                                              <th style=""background-color: #6491D9;color: #ffffff;"">Buget disponibil RON</th>
		//										<th rowspan=""2"" colspan=""2"">
		//										</th>
		//										</tr>";

		//	htmlHeader10 = htmlHeader10 + $@"    
		//										<tr>
		//										<th >{order.BudgetForecast.BudgetBase.Code}</th>
		//										<th colspan=""2"">{order.BudgetForecast.BudgetBase.Employee.Email}</th>
		//										<th>{order.BudgetForecast.BudgetBase.Project.Code}</th>
		//                                              <th>{order.BudgetForecast.BudgetBase.Activity.Name}</th>
		//										<th>{order.BudgetForecast.BudgetBase.Info}</th>
		//                                              <th>{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon) - order.BudgetValueNeed)}</th>
		//										</th>
		//										</tr>";


		//	subject = "Suplimentare buget pentru comanda: " + order.Code + "!!";

		//	htmlHeader12 = htmlHeader12 + $@"        
		//					</tr>
		//					<tr>
		//                              <th>{order.Code}</th>";



		//	for (int i = 0; i < orderMaterials.Count; i++)
		//	{
		//		index++;
		//		var wip = orderMaterials[i].WIP ? "DA" : "NU";
		//		htmlHeader3 += $@"      
		//                              <tr>
		//                                  <th>{index}</th>
		//							<th>{orderMaterials[i].Order.Offer.Request.Code}</th>
		//                                  <th>{orderMaterials[i].Order.OrderType.Name}</th>
		//							<th>{wip}</th>
		//							<th>{orderMaterials[i].Material.Code}</th>
		//                                  <th>{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
		//                                  <th>{orderMaterials[i].Order.Uom.Code}</th>
		//							<th>{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
		//							<th>{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
		//                                  <th>{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
		//                                  <th>{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
		//						</tr>";
		//		if (index == orderMaterials.Count)
		//		{
		//			htmlHeader3 += $@"      
		//                              <tr>
		//                                  <th colspan=""4""></th>
		//                                  <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
		//                                  <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
		//                                  <th style=""background-color: #6491D9;color: #ffffff;""></th>
		//                                  <th style=""background-color: #6491D9;color: #ffffff;""></th>
		//                                  <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
		//                                  <th style=""background-color: #6491D9;color: #ffffff;""></th>
		//                                  <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
		//						</tr>";
		//		}
		//	};

		//	//    htmlHeader3 = htmlHeader3 + $@"      
		//	//                        <tr>
		//	//                            <th>{index}</th>
		//	//	<th colspan=""2"">{order.OrderType.Name}</th>
		//	//	<th>{order.Offer.Code}</th>
		//	//	<th>{order.Offer.Request.Code}</th>
		//	//                            <th colspan=""2"">{order.Uom.Code}</th>
		//	//	<th>{String.Format("{0:#,##0.##}", order.Price)}</th>
		//	//</tr>";


		//	emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == order.EmployeeId).Select(a => a.Guid).ToList();

		//	if (emails1.Count > 0)
		//	{
		//		emails.Add(emails1.ElementAt(0));
		//	}

		//	for (int e = 0; e < emails.Count; e++)
		//	{
		//		var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

		//		if (emp.Email != null && emp.Email != "")
		//		{
		//			to.Add(emp.Email);
		//		}
		//		else
		//		{
		//			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
		//		}
		//	}

		//	to = new List<string>();

		//	to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
		//	//to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
		//	//to.Add("silvia.damian@emag.ro");

		//	htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

		//	var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

		//	successEmail = await _emailSender.SendEmailAsync(emailMessage);

		//	to = new List<string>();

		//	if (successEmail)
		//	{
		//		return true;
		//	}
		//	else
		//	{
		//		return false;
		//	}
		//}

		public async Task<bool> SendNeedBudgetResponseNotification(int requestId, int? requestBudgetForecastId, int documentNumber)
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
			//Model.AppState appState = null;
			// Model.Order order = null;
			Model.RequestBudgetForecast requestBudgetForecast = null;
			//        order = await _context.Set<Model.Order>()
			//            .Include(o => o.Offer).ThenInclude(r => r.Request)
			//            .Include(o => o.Offer).ThenInclude(r => r.AssetType)
			//            .Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
			//            .Include(o => o.Offer).ThenInclude(r => r.Region)
			//            .Include(c => c.Uom)
			//             .Include(c => c.Partner)
			//            .Include(c => c.Company)
			//            .Include(c => c.CostCenter)
			//.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Employee)
			//.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
			//.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Activity)
			//.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Division)
			//.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Department)
			//            .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
			//.Include(c => c.Employee)
			//            .Include(c => c.Project)
			//            .Include(c => c.OrderType).AsNoTracking().Where(a => a.Id == orderId).SingleOrDefaultAsync();
			requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
				//.Include(o => o.Offer).ThenInclude(r => r.Request)
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
				.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
				.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Region)
				.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.AdmCenter)
				//.Include(c => c.Employee)
				//.Include(c => c.Project)
				//.Include(c => c.OrderType)
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
                                        <th colspan=""13"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">CONFIRMARE INCARCARE SUPLIMENTARE BUGET</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Solicitant</th>
                                        <th colspan=""2"">Buget initial RON</th>
                                        <th colspan=""2"">Buget incarcat RON</th>
                                        <th colspan=""2"">Buget total RON</th>
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
                                        <th colspan=""13""></th>
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
										<th colspan = ""13""></th>
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

			// orderMaterials = await _context.Set<Model.OrderMaterial>().Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request).Include(o => o.Order).ThenInclude(a => a.Uom).Include(a => a.Material).Include(o => o.Order).ThenInclude(a => a.OrderType).Where(a => a.IsDeleted == false && a.OrderId == order.Id).ToListAsync();

			reqBFMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
			   .Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
			   .Include(o => o.Order).ThenInclude(a => a.Uom)
			   .Include(o => o.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.AssetType)
			   .Include(a => a.Material)
			   .Include(o => o.Order).ThenInclude(a => a.OrderType)
			   .Include(o => o.Order).ThenInclude(a => a.Partner)
			   .Include(o => o.Order).ThenInclude(a => a.Company)
			   .Include(o => o.Order).ThenInclude(a => a.Employee)
			   .Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecastId)
			   .ToListAsync();
			int index = 0;

			//var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
			////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

			var link = "https://optima.emag.network/OFA";

			//appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "NEED_BUDGET").SingleAsync();

			string empIni = "";
			if (reqBFMaterials.Count > 0)
			{
				empIni = reqBFMaterials[0].Order.Employee != null ? reqBFMaterials[0].Order.Employee.FirstName + " " + reqBFMaterials[0].Order.Employee.LastName : "";
			}
			else
			{
				empIni = "";
			}



			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{(reqBFMaterials.Count > 0 && reqBFMaterials[0].Order != null && reqBFMaterials[0].Order.Employee != null && reqBFMaterials[0].Order.Employee.Email != null ? reqBFMaterials[0].Order.Employee.Email : "")}</th>
                                                <th rowspan=""2"" colspan=""2"">{String.Format("{0:#,##0.##}", requestBudgetForecast.ValueRon - requestBudgetForecast.NeedBudgetValue)}</th>
                                                <th rowspan=""2"" colspan=""2"">{String.Format("{0:#,##0.##}", requestBudgetForecast.NeedBudgetValue)}</th>
                                                <th rowspan=""2"" colspan=""2"">{String.Format("{0:#,##0.##}", requestBudgetForecast.ValueRon)}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

			htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""13"" style=""color: #ffffff;"">Detalii OFERTA</th>
												</tr>";
			htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Furnizor</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Companie</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Tip</th>
												</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"">{(reqBFMaterials.Count > 0 && reqBFMaterials[0].Order != null && reqBFMaterials[0].Order.Partner != null ? reqBFMaterials[0].Order.Partner.Name : "")}</th>
												<th>{(reqBFMaterials.Count > 0 && reqBFMaterials[0].Order != null && reqBFMaterials[0].Order.Company != null ? reqBFMaterials[0].Order.Company.Code : "")}</th>
												<th>{(reqBFMaterials.Count > 0 && reqBFMaterials[0].Order != null && reqBFMaterials[0].Order.Offer != null && reqBFMaterials[0].Order.Offer.AssetType != null ? reqBFMaterials[0].Order.Offer.AssetType.Name : "")}</th>
												</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""13"" style=""color: #ffffff;"">Detalii BUGET</th>
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
												<th rowspan=""2"" colspan=""2"">
												</th>
												</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th >{requestBudgetForecast.BudgetForecast.BudgetBase.Code}</th>
												<th colspan=""2"">{requestBudgetForecast.BudgetForecast.BudgetBase.Employee.Email}</th>
												<th>{requestBudgetForecast.BudgetForecast.BudgetBase.Project.Code}</th>
                                                <th>{requestBudgetForecast.BudgetForecast.BudgetBase.Activity.Name}</th>
                                                <th>{requestBudgetForecast.BudgetForecast.BudgetBase.Region.Name}</th>
                                                <th>{requestBudgetForecast.BudgetForecast.BudgetBase.AdmCenter.Name}</th>
												<th>{requestBudgetForecast.BudgetForecast.BudgetBase.Info}</th>
												</tr>";


			subject = "Suplimentare buget pentru comanda: " + (reqBFMaterials.Count > 0 && reqBFMaterials[0].Order != null ? reqBFMaterials[0].Order.Code + "!!" : "");

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{(reqBFMaterials.Count > 0 && reqBFMaterials[0].Order != null ? reqBFMaterials[0].Order.Code : "")}</th>";



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


			emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == reqBFMaterials[0].Order.EmployeeId).Select(a => a.Guid).ToList();

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
#endif
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
#if RELEASE
			to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
#endif

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

		public async Task<bool> SendNeedBudgetNotification(int requestId, int? requestbudgetForecastId, int documentNumber)
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
			Model.RequestBudgetForecast requestBudgetForecast = null;
			Model.EmailRequestStatus emailRequestStatus = null;
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
			requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>().Where(a => a.Id == requestbudgetForecastId).FirstOrDefaultAsync();
			emailRequestStatus = await _context.Set<Model.EmailRequestStatus>().Where(a => a.DocumentNumber == documentNumber && a.IsDeleted == false).FirstOrDefaultAsync();
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
												<th rowspan=""2"" colspan=""2"">{request.Employee.Email}</th>
		                                              <th rowspan=""2"" colspan=""1"">{String.Format("{0:#,##0.##}", requestBudgetForecast.NeedBudgetValue)}</th>
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
		                                              <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
		                                              <th style=""background-color: #6491D9;color: #ffffff;""></th>
		                                              <th style=""background-color: #6491D9;color: #ffffff;""></th>
												<th rowspan=""2"" colspan=""1"">
		                                                   <a class=""button"" style=""padding: 5px 10px"" href='" + linkYes + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>
												</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
												<tr>
		                                              <th >{owner}</th>
												<th >{request.Division.Department.Code + "|" + request.Division.Department.Name}</th>
												<th>{request.Division.Code + "|" + request.Division.Name}</th>
		                                              <th>{request.ProjectType.Code + "|" + request.ProjectType.Name}</th>
		                                              <th>{request.AssetType.Code + "|" + request.AssetType.Name}</th>
		                                              <th>{request.Info}</th>
		                                              <th></th>
		                                              <th></th>
												</th>
												</tr>";


			subject = "Solicitare buget pentru P.R. - ul: " + request.Code + "!!";

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

#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
#endif
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
#if RELEASE
			to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
#endif

			//to.Add("silvia.damian@emag.ro");


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
				if(emailRequestStatus != null)
				{
					emailRequestStatus.NotNeedBudgetSync = false;
					emailRequestStatus.NeedBudgetEmailSend = true;
					_context.Update(emailRequestStatus);
					_context.SaveChanges();
				}

				return true;
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
            List<int> emails1 = new List<int>();
            List<int> emails2 = new List<int>();
            List<int> emails = new List<int>();
            List<Model.RequestBudgetForecast> requestBudgetForecasts = new List<Model.RequestBudgetForecast>();
            var files = new FormFileCollection();
            List<Model.EmailRequestStatus> emailRequestStatus = new List<Model.EmailRequestStatus>();
            //Model.AppState appState = null;
            Model.Request request = null;
            emailRequestStatus = await _context.Set<Model.EmailRequestStatus>()
                .Include(o => o.Request)
                 .Include(o => o.Request).ThenInclude(p => p.CostCenter)
                 .Include(o => o.Request).ThenInclude(p => p.Company)
                 .Include(o => o.Request).ThenInclude(a => a.AssetType)
                 .Include(o => o.Request).ThenInclude(a => a.Division)
                 .Include(o => o.Request).ThenInclude(a => a.ProjectType)
                 .Include(o => o.Request).ThenInclude(a => a.Employee)
                 .Include(o => o.Request).ThenInclude(a => a.Project)
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
                                        <th colspan=""11"" style=""color: #ffffff;"">Notificare P.R. nou</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar P.R.</th>
									    <th colspan=""3"">Data adaugare</th>
										<th colspan=""2"">Solicitant</th>
                                        <th colspan=""2"">Responsabil</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailRequestStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailRequestStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailRequestStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color:#cadff2;"">
                                        <th>Numar</th>
										<th colspan=""2"">Perioada implementare (de la)</th>
										<th colspan=""2"">Perioada implementare (pana la)</th>
										<th colspan=""6"">Detalii</th>
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

            requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>()
                .Include(a => a.Request)
                .Include(o => o.BudgetForecast).ThenInclude(a => a.BudgetBase)
                .Where(a => a.IsDeleted == false && a.RequestId == emailRequestStatus[0].RequestId).ToListAsync();

            request = await _context.Set<Model.Request>()
                .Include(r => r.ProjectType)
                .Include(r => r.Division)
                .Include(r => r.Employee)
                .Include(r => r.Owner)
                .Where(a => a.Id == emailRequestStatus[0].RequestId).SingleOrDefaultAsync();

            int index = 0;

            // var linkYesNo = "http://localhost:4200/#/ordervalidateL4/" + order.Guid;
            var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateL4/" + request.Guid;
            //var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/ofa/#/procurement/request";

            //appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();


            
            string empOwner = request != null && request.Owner != null ? request.Owner.Email : "";
            string empRequester = request != null && request.Employee != null ? request.Employee.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"" style=""font-weight: normal;"">{empRequester}</th>
                                                <th rowspan=""2"" colspan=""2"" style=""font-weight: normal;"">{empOwner}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii P.R.</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th colspan=""2"">Tip</th>
												<th colspan=""2"">Proiect</th>
                                                <th colspan=""2"">Aprobare</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{(emailRequestStatus[0].Request.Company != null ? emailRequestStatus[0].Request.Company.Code : "")}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{(emailRequestStatus[0].Request.Division != null ? emailRequestStatus[0].Request.Division.Name : "")}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{(emailRequestStatus[0].Request.AssetType != null ? emailRequestStatus[0].Request.AssetType.Name : "")}</th>
                                                <th colspan=""2"" style=""font-weight: normal;"">{(emailRequestStatus[0].Request.ProjectType != null ? emailRequestStatus[0].Request.ProjectType.Name: "")}</th>
                                                <th colspan=""2"">
														
													</th>
												</tr>";

            //htmlHeader8 = htmlHeader8 + $@"     
												//<tr style=""background-color: #2f75b5;"">
												//<th colspan=""11"" style=""color: #ffffff;"">Flux de aprobare</th>
												//</tr>";

            //htmlHeader120 = htmlHeader120 + $@"    
												//<tr>
												//<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailRequestStatus[0].EmployeeL4ValidateAt != null ? emailRequestStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
												//<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailRequestStatus[0].EmployeeL3ValidateAt != null ? emailRequestStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
												//<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailRequestStatus[0].EmployeeL2ValidateAt != null ? emailRequestStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
            //                                    <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailRequestStatus[0].EmployeeL1ValidateAt != null ? emailRequestStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
												//<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailRequestStatus[0].EmployeeS3ValidateAt != null ? emailRequestStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>
            //                                    <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailRequestStatus[0].EmployeeS2ValidateAt != null ? emailRequestStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>
            //                                    <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailRequestStatus[0].EmployeeS1ValidateAt != null ? emailRequestStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
												//</tr>";


            subject = "Plasare P.R. numarul : " + emailRequestStatus[0].Request.Code;

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailRequestStatus[0].Request.Code}</th>";



            for (int i = 0; i < requestBudgetForecasts.Count; i++)
            {
                index++;
                //var wip = requestBudgetForecasts[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th style=""font-weight: normal;"">{index}</th>
									 <th colspan=""2""style=""font-weight: normal;"">{String.Format("{0:dd-MM-yyyy}", emailRequestStatus[0].Request.StartExecution)}</th>
									 <th colspan=""2""style=""font-weight: normal;"">{String.Format("{0:dd-MM-yyyy}", emailRequestStatus[0].Request.EndExecution)}</th>
                                     <th colspan=""6"" style=""font-weight: normal;"">{requestBudgetForecasts[i].Request.Info}</th>
								</tr>";
        //        if (index == requestBudgetForecasts.Count)
        //        {
        //            htmlHeader3 += $@"      
        //                        <tr>
        //                            <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
        //                            <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", requestBudgetForecasts.Sum(a => a.Quantity))}</th>
        //                            <th style=""background-color:#cadff2;""></th>
        //                            <th style=""background-color:#cadff2;""></th>
        //                            <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", requestBudgetForecasts.Sum(a => a.Value))}</th>
        //                            <th style=""background-color:#cadff2;""></th>
        //                            <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", requestBudgetForecasts.Sum(a => a.ValueRon))}</th>
								//</tr>";
        //        }
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

            
            emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailRequestStatus[0].Request.EmployeeId).Select(a => a.Id).ToListAsync();

            if (emails1.Count > 0)
            {
                emails.Add(emails1.ElementAt(0));
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
            }

			to = new List<string>();
			cc = new List<string>();
			to.Add("bogdan.pirvulescu@emag.ro");
			//cc.Add("radu.alexandru1@emag.ro");
            cc.Add("procurement@emag.ro");

#if DEBUG

            to = new List<string>();
			cc = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
			to.Add("radu.alexandru1@emag.ro");

#endif
			//to.Add("iordache.costin@emag.ro");
			//to.Add("gabriela.dogaru@emag.ro");
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");
			

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
                for (int i = 0; i < emailRequestStatus.Count; i++)
                {
                    emailRequestStatus[i].NotEmployeeL4Sync = false;
                    emailRequestStatus[i].EmployeeL4EmailSend = true;
                    _context.Update(emailRequestStatus[i]);

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



    }
}
