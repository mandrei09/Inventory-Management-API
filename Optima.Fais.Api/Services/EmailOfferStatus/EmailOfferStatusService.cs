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
	public class EmailOfferStatusService : IEmailOfferStatusService
    {
		private readonly ApplicationDbContext _context;
		private readonly IEmailSender _emailSender;
        public IServiceProvider _services { get; }

        private readonly string _basePath;
        private readonly string _resourcesFolder;
        private readonly string _resourcesPath;

        public EmailOfferStatusService(ApplicationDbContext context, IEmailSender emailSender, IConfiguration configuration, IServiceProvider services)
		{
			this._context = context;
			this._emailSender = emailSender;
            _services = services;
            this._basePath = configuration.GetSection("BasePath").GetValue<string>("Base");
            this._resourcesFolder = configuration.GetSection("BasePath").GetValue<string>("Resources");
            this._resourcesPath = $"{this._basePath}{this._resourcesFolder}";
        }

        public async Task<bool> SendNotification(int documentNumberId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<int> emails1 = new List<int>();
            List<int> emails2 = new List<int>();
            List<int> emails = new List<int>();
            var files = new FormFileCollection();
            List<Model.EmailOfferStatus> emailOfferStatus = new List<Model.EmailOfferStatus>();
            //Model.AppState appState = null;
            // Model.Offer offer = null;
            emailOfferStatus = await _context.Set<Model.EmailOfferStatus>()
                .Include(o => o.Company)
				.Include(o => o.Offer).ThenInclude(a => a.AssetType)
				.Include(o => o.Offer).ThenInclude(a => a.Division)
				.Include(o => o.Offer).ThenInclude(a => a.ProjectType)
				.Include(o => o.Employee)
				.Include(o => o.Partner)
                .AsNoTracking().Where(a => a.IsDeleted == false && a.NotSync == true && a.EmailSkip == false && a.EmailSend == false && a.SyncErrorCount < 3 && a.DocumentNumber == documentNumberId).ToListAsync();
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
                                        <th colspan=""11"" style=""color: #ffffff;"">Solicitare oferta</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Cod oferta</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""7"">Solicitant</th>
									</tr>
									<tr>
                                        <th style=""background-color:#cadff2;"">Ziua</th>
									    <th style=""background-color:#cadff2;"">Luna</th>
										<th style=""background-color:#cadff2;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th style=""font-weight: normal;"">{String.Format("{0:dd}", emailOfferStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:MM}", emailOfferStatus[0].CreatedAt)}</th>
										<th style=""font-weight: normal;"">{String.Format("{0:yyyy}", emailOfferStatus[0].CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"";

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

    //        offer = await _context.Set<Model.Offer>()
    //            .Include(e => e.Company)
    //            .Include(e => e.Division).ThenInclude(r => r.Department)
				//.Include(e => e.AssetType)
				//.Include(e => e.ProjectType)
				//.Where(a => a.Id == emailOfferStatus[0].OfferId).SingleOrDefaultAsync();

            int index = 0;

            // var linkYesNo = "http://localhost:4200/#/ordervalidateL4/" + order.Guid;
            // var linkYesNo = "https://optima.emag.network/ofa/#/ordervalidateL4/" + offer.Guid;
            //var linkYes = "https://optima.emag.network/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "https://optima.emag.network/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            // var link = "https://optima.emag.network/ofa";

            //appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();


            
            //string empOwner = offer != null && offer != null && offer.Request != null && offer.Request.Owner != null ? offer.Request.Owner.Email : "";
            //string empRequester = offer != null && offer != null && offer.Request != null && offer.Request.Employee != null ? offer.Request.Employee.Email : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""7"" style=""font-weight: normal;"">{(emailOfferStatus[0].Employee.Email != null && emailOfferStatus[0].Employee.Email != "" ? emailOfferStatus[0].Employee.Email : "adrian.cirnaru@optima.ro")}</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Detalii Oferta</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr style=""background-color:#cadff2;color: #ffffff;"">
												<th colspan=""2"">Companie</th>
												<th colspan=""2"">Departament</th>
												<th colspan=""2"">Furnizor</th>
												<th colspan=""2"">Tip</th>
                                                <th colspan=""3"">Proiect</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOfferStatus[0].Company.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOfferStatus[0].Offer.Division.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOfferStatus[0].Partner.Name}</th>
												<th colspan=""2"" style=""font-weight: normal;"">{emailOfferStatus[0].Offer.AssetType.Name}</th>
                                                <th colspan=""3"" style=""font-weight: normal;"">{emailOfferStatus[0].Offer.ProjectType.Name}</th>
												</tr>";

			htmlHeader8 = htmlHeader8 + $@"
												<tr style=""background-color: #2f75b5;"">
												<th colspan=""11"" style=""color: #ffffff;text-align: left"">Mesaj</th>
												</tr>";

			htmlHeader9 = htmlHeader9 + $@"
												<tr>
												<th colspan=""11"" rowspan=""10"" style=""font-weight: normal;text-align: center"">{emailOfferStatus[0].Message}</th>
												</tr>";

			//htmlHeader120 = htmlHeader120 + $@"    
			//<tr>
			//<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOfferStatus[0].EmployeeL4ValidateAt != null ? emailOfferStatus[0].EmployeeL4ValidateAt : string.Empty)}</th>
			//<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOfferStatus[0].EmployeeL3ValidateAt != null ? emailOfferStatus[0].EmployeeL3ValidateAt : string.Empty)}</th>
			//<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOfferStatus[0].EmployeeL2ValidateAt != null ? emailOfferStatus[0].EmployeeL2ValidateAt : string.Empty)}</th>
			//                                    <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOfferStatus[0].EmployeeL1ValidateAt != null ? emailOfferStatus[0].EmployeeL1ValidateAt : string.Empty)}</th>
			//<th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOfferStatus[0].EmployeeS3ValidateAt != null ? emailOfferStatus[0].EmployeeS3ValidateAt : string.Empty)}</th>
			//                                    <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOfferStatus[0].EmployeeS2ValidateAt != null ? emailOfferStatus[0].EmployeeS2ValidateAt : string.Empty)}</th>
			//                                    <th>{String.Format("{0:dd/MM/yyyy HH:mm:ss}", emailOfferStatus[0].EmployeeS1ValidateAt != null ? emailOfferStatus[0].EmployeeS1ValidateAt : string.Empty)}</th>
			//</tr>";


			subject = "Solicitare oferta " + emailOfferStatus[0].Offer.Code;

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th style=""font-weight: normal;"">{emailOfferStatus[0].Offer.Code}</th>";



        //    for (int i = 0; i < orderMaterials.Count; i++)
        //    {
        //        index++;
        //        var wip = orderMaterials[i].WIP ? "DA" : "NU";
        //        htmlHeader3 += $@"      
        //                        <tr>
        //                            <th style=""font-weight: normal;"">{index}</th>
								//	<th style=""font-weight: normal;"">{orderMaterials[i].Order.Offer.Request.Code}</th>
        //                            <th style=""font-weight: normal;"">{orderMaterials[i].Order.OrderType.Name}</th>
								//	<th style=""font-weight: normal;"">{wip}</th>
								//	<th style=""font-weight: normal;"">{orderMaterials[i].Material.Name}</th>
        //                            <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
        //                            <th style=""font-weight: normal;"">{orderMaterials[i].Order.Uom.Code}</th>
								//	<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
								//	<th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
        //                            <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
        //                            <th style=""font-weight: normal;"">{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								//</tr>";
        //        if (index == orderMaterials.Count)
        //        {
        //            htmlHeader3 += $@"      
        //                        <tr>
        //                            <th colspan=""5"" style=""background-color:#cadff2;text-align: left"">Total</th>
        //                            <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
        //                            <th style=""background-color:#cadff2;""></th>
        //                            <th style=""background-color:#cadff2;""></th>
        //                            <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
        //                            <th style=""background-color:#cadff2;""></th>
        //                            <th style=""background-color:#cadff2;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
								//</tr>";
        //        }
        //    };

            //    htmlHeader3 = htmlHeader3 + $@"      
            //                        <tr>
            //                            <th>{index}</th>
            //	<th colspan=""2"">{order.OrderType.Name}</th>
            //	<th>{order.Offer.Code}</th>
            //	<th>{order.Offer.Request.Code}</th>
            //                            <th colspan=""2"">{order.Uom.Code}</th>
            //	<th>{String.Format("{0:#,##0.##}", order.Price)}</th>
            //</tr>";

            
            emails1 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == emailOfferStatus[0].EmployeeId).Select(a => a.Id).ToListAsync();
			emails2 = await _context.Set<Model.Partner>().AsNoTracking().Where(a => a.Id == emailOfferStatus[0].PartnerId).Select(a => a.Id).ToListAsync();

            for (int i = 0; i < emails1.Count; i++)
            {
				emails.Add(emails1.ElementAt(i));
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

			for (int e = 0; e < emails2.Count; e++)
			{
				var partner = _context.Set<Model.Partner>().Where(p => p.Id == emails2.ElementAt(e)).Single();

				if (partner.ContactInfo != null && partner.ContactInfo != "")
				{
					to.Add(partner.ContactInfo);
				}

				if (partner.Bank != null && partner.Bank != "")
				{
					to.Add(partner.Bank);
				}

				if (partner.BankAccount != null && partner.BankAccount != "")
				{
					to.Add(partner.BankAccount);
				}

                if(to.Count == 0)
                {
					to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
				}
			}

			//to = new List<string>();
			//cc = new List<string>();

			//to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
   //         cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
            //to.Add("silvia.damian@emag.ro");
            bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

			List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.GuidAll == emailOfferStatus[0].GuidAll).ToList();

			MemoryStream ms = new();

			List<(string, string, string)> filePaths = new List<(string, string, string)>();

			for (int i = 0; i < entityFiles.Count; i++)
			{
				var filePath = Path.Combine("offerui", entityFiles[i].StoredAs);
				filePaths.Add(new(filePath, entityFiles[i].Name, entityFiles[i].FileType));
			}

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + 
                htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + 
                htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, filePaths);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
                for (int i = 0; i < emailOfferStatus.Count; i++)
                {
                    emailOfferStatus[i].NotSync = false;
                    emailOfferStatus[i].EmailSend = true;
                    _context.Update(emailOfferStatus[i]);

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
