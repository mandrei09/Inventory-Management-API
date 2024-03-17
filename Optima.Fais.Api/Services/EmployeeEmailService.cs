
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public class EmployeeEmailService : BackgroundService
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmailSender _emailSender;

        public EmployeeEmailService(IServiceProvider services, IEmployeeService employeeService, IEmailSender emailSender)
        {
            Services = services;
            _employeeService = employeeService;
            _emailSender = emailSender;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Model.EmployeeEmailResult> employeesList = new List<Model.EmployeeEmailResult>();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
					//using (var scope = Services.CreateScope())
					//{
					//	var dbContext =
					//		scope.ServiceProvider
					//			.GetRequiredService<ApplicationDbContext>();

					//	List<Model.SyncStatus> syncStatusList = await dbContext.Set<Model.SyncStatus>().Where(s => !s.IsDeleted && s.SyncEnabled).ToListAsync();

					//	foreach (var syncStatus in syncStatusList)
					//	{
					//		var lastSync = syncStatus.SyncLast;

					//		if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.UtcNow)))
					//		{
					//			List<Model.EmailStatus> notificationList = await dbContext.Set<Model.EmailStatus>().AsNoTracking()
					//				.Where(p => p.EmailType.Code == "TRANSFER" && !p.IsDeleted && 
					//				((!p.DstEmployeeEmailSend && p.DstEmployeeEmailSend) || (p.NotSync && p.NotCompletedSync)) && p.SyncErrorCount < 3).ToListAsync();

					//			syncStatus.SyncStart = DateTime.UtcNow;

					//			foreach (var notification in notificationList)
					//			{
					//				bool result = false;

					//				switch (notification)
					//				{
					//					case "FINISHED_INVENTORIES":
					//						result = await messageService.SendFinishedInventoriesNotification(notification.Id);
					//						break;
					//					case "VALIDATED_INVENTORIES":
					//						result = await messageService.SendValidatedInventoriesNotification(notification.Id);
					//						break;
					//					case "COMPLETED_INVENTORIES":
					//						result = await messageService.SendCompletedInventoriesNotification(notification.Id);
					//						break;
					//					default:
					//						result = await messageService.SendRecoNotification(notification.Id);
					//						break;
					//				}

					//				if (result)
					//					return new OperationResult { Success = true, MessageCode = "Notificarea a fost trimisa cu succes!" };
					//			}

					//			syncStatus.SyncEnd = DateTime.UtcNow;
					//			syncStatus.SyncLast = DateTime.UtcNow;
					//			dbContext.Update(syncStatus);
					//			dbContext.SaveChanges();
					//		}
					//	}
					//}
						

					

					await _employeeService.SearchNewEmployeeTransferAsync();
					//await _employeeService.SearchNewEmployeeReminder1TransferAsync();
					//await _employeeService.SearchNewEmployeeReminder2TransferAsync();
					//await _employeeService.SearchNewEmployeeReminder3TransferAsync();
					//await _employeeService.SearchNewEmployeeReminder4TransferAsync();
					//await _employeeService.SearchNewManagerTransferAsync();
					await _employeeService.SearchNewAppendixTransferAsync();
					await _employeeService.SearchNewRejectedByStockTransferAsync();
					await _employeeService.SearchNewRejectedAccountingValidationAsync();
				}
                catch (Exception ex)
                {
                    string s = ex.ToString();
                }

               
                // await Task.Delay(86400000 * (employeesList.Count > 0 ? employeesList[0].NotifyInterval : 1), stoppingToken);
                await Task.Delay(300000, stoppingToken);
            }


        }


  //      public async Task<bool> SendMail(int employeeId)
  //      {
  //          using (IServiceScope scope = Services.CreateScope())
  //          {
  //             var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

  //              var files = new FormFileCollection();

  //              //Model.Document document = null;
  //              //Model.DocumentType documentType = null;
  //              Model.EmailManager emailManager = null;
  //              Model.EmailManager emailManagerComponent = null;
  //              Model.EmailType emailTypeAsset = null;
  //              Model.EmailType emailTypeAssetComponent = null;
  //              Model.Asset asset = null;
  //              Model.AssetComponent assetComponent = null;
  //              List<string> cc = new List<string>();
  //              Dictionary<int, string> dictCC = new Dictionary<int, string>();
  //              Dictionary<int, Guid> guidIds = new Dictionary<int, Guid>();
  //              Dictionary<string, int> aCats = new Dictionary<string, int>();
  //              Dictionary<string, int> aCompCats = new Dictionary<string, int>();
  //              string categoryCode = "";
  //              string categoryComponentCode = "";
  //              int span = 1;
  //              int spanComp = 1;
  //              int rowSpan = 1;
  //              int rowSpanComp = 1;
  //              int number = 0;
  //              int guidNumber = 0;
  //              var htmlBodyEmail1 = "";
  //              var htmlBodyEmailComponent = "";
  //              var htmlBodyEnd = "";
  //              var htmlBodyCompany1 = "";
  //              var htmlBodyCompany2 = "";
  //              var company1 = "";
  //              var company2 = "";
  //              var link1 = "";
  //              var link2 = "";
  //              var link3 = "";
  //              string imageLink = "https://service.inventare.ro/socgenupload/";
  //              string defaultImage = "blank.jpg";
  //              var headerMsg = "";
  //              var footerMsg = "";
  //              var end = @"</tbody>
  //                                              </table>";
  //              var htmlBody11 = @"
  //                                          <br>
                                              
  //                                              <table class=""minimalistBlack"">
  //                                                  <thead>";
  //              var htmlBody1 = @"
  //                                      <html lang=""en"">
  //                                          <head>    
  //                                              <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
  //                                              <title>
  //                                                  Upcoming topics
  //                                              </title>
  //                                              <style type=""text/css"">
  //                                                  HTML{background-color: #e8e8e8;}
  //                                                  .courses-table{border: 3px solid #000000;
  //                                                    width: 100%;
  //                                                    text-align: center;
  //                                                    border-collapse: collapse}
  //                                                  .courses-table .description {color: #fefefe !important;}
  //                                                  .courses-table .description a{color: #ffffff !important; font: bold 11px Arial;
  //                                                    text-decoration: none;
  //                                                    background-color: #fe0000;
  //                                                    padding: 2px 6px 2px 6px;
  //                                                    border-top: 1px solid #CCCCCC;
  //                                                    border-right: 1px solid #333333;
  //                                                    border-bottom: 1px solid #333333;
  //                                                    border-left: 1px solid #CCCCCC;}
  //                                                  .courses-table td{border: 1px solid #ffffff; background-color: #000000; text-align: center; padding: 8px;}
  //                                                  .courses-table th{border: 1px solid #ffffff; color: #030804;text-align: center; padding: 8px;}
  //                                                  .red{background-color: #FFDD04;}
  //                                                  .header{background-color: #fe0000; color: #000000 !important;}
  //                                                  .msg{background-color: #000000; color: #fefefe !important;}
  //                                                  .redmsg{color: #FF0000 !important;text-align: center;}
  //                                                  .message{color: #000000 !important;text-align: center;}
  //                                                  .table-header{background-color: #000000; color: #fefefe !important;}
  //                                                  .table-body{background-color: #000000; color: #fefefe !important;}
  //                                                  .green{background-color: #6B9852;}
  //                                                  table.minimalistBlack {
  //                                                    border: 3px solid #000000;
  //                                                    width: 100%;
  //                                                    text-align: center;
  //                                                    border-collapse: collapse;
  //                                                  }
  //                                                  table.minimalistBlack td, table.minimalistBlack th {
  //                                                    border: 1px solid #000000;
  //                                                    padding: 5px 4px;
  //                                                  }
  //                                                  table.minimalistBlack tbody td {
  //                                                    font-size: 13px;
  //                                                  }
  //                                                  table.minimalistBlack thead {
  //                                                    background: #CFCFCF;
  //                                                    background: -moz-linear-gradient(top, #dbdbdb 0%, #d3d3d3 66%, #CFCFCF 100%);
  //                                                    background: -webkit-linear-gradient(top, #dbdbdb 0%, #d3d3d3 66%, #CFCFCF 100%);
  //                                                    background: linear-gradient(to bottom, #dbdbdb 0%, #d3d3d3 66%, #CFCFCF 100%);
  //                                                    border-bottom: 3px solid #000000;
  //                                                  }
  //                                                  table.minimalistBlack thead th {
  //                                                    font-size: 15px;
  //                                                    font-weight: bold;
  //                                                    color: #000000;
  //                                                    text-align: center;
  //                                                  }
  //                                                  table.minimalistBlack tfoot td {
  //                                                    font-size: 14px;
  //                                                  }
  //                                                 .button {
  //                                                            background-color: #4CAF50;
  //                                                            border: none;
  //                                                            color: black;
  //                                                            padding: 15px 32px;
  //                                                            text-align: center;
  //                                                            text-decoration: none;
  //                                                            display: inline-block;
  //                                                            font-size: 16px;
  //                                                            margin: 4px 2px;
  //                                                            cursor: pointer;
  //                                                          }
  //                                                 .button-no {
  //                                                            background-color: #cf1140;
  //                                                            border: none;
  //                                                            color: white;
  //                                                            padding: 15px 32px;
  //                                                            text-align: center;
  //                                                            text-decoration: none;
  //                                                            display: inline-block;
  //                                                            font-size: 16px;
  //                                                            margin: 4px 2px;
  //                                                            cursor: pointer;
  //                                                          }
                                                   
  //                                              </style>
  //                                          </head>
  //                                          <body>         
  //                                      ";

  //              var htmlBody2 = @"
                                       
                                                
  //                                                      <tr>
  //                                                          <th>Category</th>
  //                                                          <th>AssetNumber</th>
  //                                                          <th>Description</th>
  //                                                          <th>Model</th>
  //                                                          <th>Serial Number</th>
  //                                                          <th>Photo</th>
  //                                                          <th colspan=""2"">Validate</th>
  //                                                      </tr>
  //                                                  </thead>
  //                                                  <tbody>
  //                                      ";
  //              //documentType = _context.Set<Model.DocumentType>().Where(d => d.Id == 16).Single();
  //              emailTypeAsset = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE ASSET INTERVAL").Single();
  //              emailTypeAssetComponent = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE COMPONENT INTERVAL").Single();
  //              headerMsg = emailTypeAsset.HeaderMsg;
  //              footerMsg = emailTypeAsset.FooterMsg;
  //              Model.Inventory inventory = _context.Set<Model.Inventory>().Where(i => i.Active == true).SingleOrDefault();

  //              List<Model.Asset> assets = _context.Set<Model.Asset>().Include(s => s.SubType).ThenInclude(t => t.Type).Include(r => r.Model).Include(d => d.Document).Where(a => a.EmployeeId == employeeId && a.Document.ParentDocumentId == inventory.DocumentId).OrderBy(a => a.SubType.TypeId).ToList();
  //              List<Model.AssetComponent> assetComponents = _context.Set<Model.AssetComponent>().Include(s => s.SubType).ThenInclude(t => t.Type).Where(a => a.EmployeeId == employeeId).OrderBy(a => a.SubType.TypeId).ToList();


  //              if (assets.Count > 1)
  //              {
  //                  htmlBody2 = @"
                                       
                                                
  //                                                      <tr>
  //                                                          <th>Category</th>
  //                                                          <th>AssetNumber</th>
  //                                                          <th>Description</th>
  //                                                          <th>Model</th>
  //                                                          <th>Serial Number</th>
  //                                                          <th>Photo</th>
  //                                                          <th colspan=""2"">Validate</th>
  //                                                          <th>All</th>
  //                                                      </tr>
  //                                                  </thead>
  //                                                  <tbody>
  //                                      ";
  //              }


  //              EmailUI emailUIs = null;

  //              emailUIs = new EmailUI
  //              {
  //                  AssetComponents = new List<AssetComponent>(),
  //                  Assets = new List<AssetUI>()
  //              };

  //              var GuidAll = Guid.NewGuid();

  //              foreach (var item in assetComponents)
  //              {
  //                  List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.EntityId == item.SubType.Id && e.EntityType.Code == "ASSETCOMPONENT").ToList();

  //                  emailUIs.AssetComponents.Add(new AssetComponent()
  //                  {

  //                      Id = item.Id,
  //                      Code = item.Code,
  //                      Name = item.Name,
  //                      ImageName = entityFiles.Count > 0 ? entityFiles[0].StoredAs : "",
  //                      Type = new CodeNameEntity { Id = item.SubType != null && item.SubType.Type != null ? item.SubType.Type.Id : 0, Code = item.SubType != null && item.SubType.Type != null ? item.SubType.Type.Code : "" }
  //                  });
  //              }


  //              foreach (var item in assets)
  //              {
  //                  List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.EntityId == item.SubType.Id && e.EntityType.Code == "ASSET").ToList();

  //                  emailUIs.Assets.Add(new AssetUI()
  //                  {

  //                      Id = item.Id,
  //                      InvNo = item.InvNo,
  //                      Name = item.Name,
  //                      SerialNumber = item.SerialNumber,
  //                      Model = new CodeNameEntity {Id = item.Model != null ? item.Model.Id : 0, Code = item.Model != null ? item.Model.Code : "", Name = item.Model != null ? item.Model.Name : "" },
  //                      //Room = new CodeNameEntity { Id = item.Room.Id, Code = item.Room.Code, Name = item.Room.Name },
  //                      //Location = new CodeNameEntity { Id = item.Room.Location.Id, Code = item.Room.Location.Code, Name = item.Room.Location.Name },
  //                      //Region = new CodeNameEntity { Id = item.Room.Location.Region.Id, Code = item.Room.Location.Region.Code, Name = item.Room.Location.Region.Name },
  //                      ImageName = entityFiles.Count > 0 ? entityFiles[0].StoredAs : "",
  //                      Type = new CodeNameEntity { Id = item.SubType != null && item.SubType.Type != null ? item.SubType.Type.Id : 0, Code = item.SubType != null && item.SubType.Type != null ? item.SubType.Type.Code : "" }
  //                  });
  //              }


  //              var itemsByGroupWithCount = assets.GroupBy(i => i.SubType != null && i.SubType.Type != null ? i.SubType.Type.Code : "")
  //              .Select(item => new {
  //                  Code = item.Key,
  //                  Total = item.Count(),
  //              }).ToList();


  //              foreach (var item in itemsByGroupWithCount)
  //              {
  //                  if (!aCats.ContainsKey(item.Code))
  //                  {
  //                      aCats.Add(item.Code, item.Total);
  //                      rowSpan++;
  //                  }

  //              }

  //              var itemsByGroupWithCountComp = assetComponents.GroupBy(i => i.SubType != null && i.SubType.Type != null ? i.SubType.Type.Code : "")
  //                  .Select(item => new {
  //                      Code = item.Key,
  //                      Total = item.Count(),
  //                  }).ToList();


  //              foreach (var item in itemsByGroupWithCountComp)
  //              {
  //                  if (!aCompCats.ContainsKey(item.Code))
  //                  {
  //                      aCompCats.Add(item.Code, item.Total);
  //                      rowSpanComp++;
  //                  }

  //              }
  //              //document = new Model.Document
  //              //{
  //              //    DocumentType = documentType,
  //              //    DocNo1 = string.Empty,
  //              //    DocNo2 = string.Empty,
  //              //    DocumentDate = DateTime.Now,
  //              //    RegisterDate = DateTime.Now,
  //              //    Approved = true,
  //              //    Exported = false,
  //              //    CreationDate = DateTime.Now,
  //              //    Details = string.Empty
  //              //};

  //              //_context.Set<Model.Document>().Add(document);


  //              // _context.SaveChanges();

  //              var subject = "Validate Asset List + Accessories";


  //              if (emailUIs.Assets.Count > 0)
  //              {
  //                  for (int i = 0; i < emailUIs.Assets.Count(); i++)
  //                  {


  //                      if (emailUIs.Assets[i].Id > 0)
  //                      {

  //                          // Model.Room roomIni = null;
  //                          // Model.Location locationIni = null;
  //                          Model.Employee eIni = null;
  //                          eIni = _context.Set<Model.Employee>().Include(d => d.Department).Where(e => e.Id == employeeId).SingleOrDefault();

  //                          if (emailUIs.AssetComponents.Count > 0 && emailUIs.AssetComponents.Count > i)
  //                          {
  //                              emailManagerComponent = new Model.EmailManager
  //                              {
  //                                  EmailType = emailTypeAssetComponent,
  //                                  EmployeeInitial = eIni,
  //                                  EmployeeFinal = eIni,
  //                                  AssetId = null,
  //                                  AssetComponentId = emailUIs.AssetComponents[i].Id,
  //                                  Guid = Guid.NewGuid(),
  //                                  GuidAll = GuidAll,
  //                                  IsAccepted = false,
  //                                  AppStateId = 7
  //                              };

  //                              _context.Set<Model.EmailManager>().Add(emailManagerComponent);
  //                              _context.SaveChanges();
  //                          }


  //                          emailManager = new Model.EmailManager
  //                          {
  //                              EmailType = emailTypeAsset,
  //                              EmployeeInitial = eIni,
  //                              EmployeeFinal = eIni,
  //                              AssetId = emailUIs.Assets[i].Id,
  //                              AssetComponentId = null,
  //                              Guid = Guid.NewGuid(),
  //                              GuidAll = GuidAll,
  //                              IsAccepted = true,
  //                              AppStateId = 6

  //                          };

  //                          _context.Set<Model.EmailManager>().Add(emailManager);

  //                          if (emailUIs.Assets[i].Room != null)
  //                          {

  //                              //roomIni = _context.Set<Model.Room>().Where(a => a.Id == emailUIs.Assets[i].Room.Id).FirstOrDefault();

  //                              //if (roomIni != null)
  //                              //{
  //                              //    locationIni = _context.Set<Model.Location>().Where(a => a.Id == roomIni.LocationId).FirstOrDefault();

  //                              //    emailManager.RoomIdInitial = roomIni.Id;
  //                              //    emailManager.RoomIdFinal = roomIni.Id;
  //                              //}
  //                          }

  //                          _context.SaveChanges();

  //                          if (eIni.Email != null && eIni.Email != "" && !dictCC.ContainsValue(eIni.Email))
  //                          {
  //                              dictCC.Add(number, eIni.Email);
  //                              number++;
  //                          }

  //                          //if (eIni != null && eIni.Department != null && eIni.Department.Name != null && eIni.Department.Name != "" && !dictCC.ContainsValue(eIni.Department.Name))
  //                          //{
  //                          //    dictCC.Add(number, eIni.Department.Name);
  //                          //    number++;
  //                          //}

  //                          if (emailManager != null && emailManager.Guid != Guid.Empty && !guidIds.ContainsValue(emailManager.Guid))
  //                          {
  //                              guidIds.Add(guidNumber, emailManager.Guid);
  //                              guidNumber++;
  //                          }

  //                          if (emailManagerComponent != null && emailManagerComponent.Guid != Guid.Empty && !guidIds.ContainsValue(emailManagerComponent.Guid))
  //                          {
  //                              guidIds.Add(guidNumber, emailManagerComponent.Guid);
  //                              guidNumber++;
  //                          }


  //                          if (emailUIs.Assets.Count() > 1)
  //                          {
  //                              if (i == 0)
  //                              {

  //                                  company1 = asset != null ? asset.Company.Name : "ITEMS";
  //                                  company2 = asset != null ? asset.Company.Name : "LISTA ACCESORII";

  //                                  htmlBodyCompany1 = htmlBodyCompany1 + @"<tr><th colspan=""9"">" + company1 + "</th></tr>";

  //                                  aCats.TryGetValue(emailUIs.Assets[i].Type.Code, out rowSpan);

  //                                  categoryCode = emailUIs.Assets[i].Type.Code;

  //                                  if (emailUIs.Assets[i].ImageName != "")
  //                                  {
  //                                      //if (emailUIs.AssetComponents.Count > 0)
  //                                      //{
  //                                      //    htmlBodyCompany2 = htmlBodyCompany2 + @"<tr><th class=""msg"" colspan=""6"">" + company2 + "</th></tr>";

  //                                      //    htmlBodyEnd = htmlBodyEnd + htmlBodyCompany2 + @"
  //                                      //                <tr>
  //                                      //                    <th class=""header"" colspan=""3"">Denumire</th>
  //                                      //                    <th class=""header"">Photo</th>
  //                                      //                    <th class=""header"" colspan=""2"">Valid</th>
                                                            
  //                                      //                </tr>
  //                                      //     ";
  //                                      //}




  //                                      //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
  //                                      link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManager.Guid;
  //                                      //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
  //                                      link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;
  //                                      link3 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/all/" + emailManager.GuidAll;

  //                                      htmlBodyEmail1 = htmlBodyEmail1 + @"
  //                                                        <tr>
  //                                                          <td rowspan=""" + rowSpan + @""">" + emailUIs.Assets[i].Type.Code + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].InvNo + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                         
  //                                                          <td>" + emailUIs.Assets[i].Model.Name + @" </ td >
  //                                                           <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
  //                                                         <td><img style = ""display: block"" width = ""100"" heigth = ""100""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
  //                                                          <td class=""description""><a class=""button"" href='" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                                          <td class=""description""><a class=""button-no"" href='" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                                                          <td class=""description"" rowspan=""" + emailUIs.Assets.Count + @""" ><a class=""button"" href='" + link3 + "'" + "' >ALL</a>" + @" </ td >
  //                                                      </tr>
                                                       
  //                                      ";

  //                                  }
  //                                  else
  //                                  {
  //                                      //if (emailUIs.AssetComponents.Count > 0)
  //                                      //{
  //                                      //    htmlBodyCompany2 = htmlBodyCompany2 + @"<tr><th class=""msg"" colspan=""8"">" + company2 + "</th></tr>";

  //                                      //    htmlBodyEnd = htmlBodyEnd + htmlBodyCompany2 + @"
  //                                      //                <tr>
  //                                      //                    <th class=""header"" colspan=""3"">Denumire</th>
  //                                      //                    <th class=""header"">Photo</th>
  //                                      //                    <th class=""header"" colspan=""2"">Valid</th>
                                                            
  //                                      //                </tr>
  //                                      //     ";
  //                                      //}




  //                                      //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
  //                                      link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManager.Guid;
  //                                      //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
  //                                      link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;
  //                                      link3 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/all/" + emailManager.GuidAll;

  //                                      htmlBodyEmail1 = htmlBodyEmail1 + @"
  //                                                        <tr>
  //                                                           <td rowspan=""" + rowSpan + @""">" + emailUIs.Assets[i].Type.Code + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].InvNo + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Name + @" </ td >
                                                         
  //                                                          <td>" + emailUIs.Assets[i].Model.Name + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
  //                                                         <td>" + @" </td>
  //                                                          <td class=""description""><a class=""button"" href='" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                                          <td class=""description""><a class=""button-no"" href='" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                                                          <td class=""description"" rowspan=""" + emailUIs.Assets.Count + @""" ><a class=""button"" href='" + link3 + "'" + "' >ALL</a>" + @" </ td >
  //                                                      </tr>
                                                       
  //                                      ";

  //                                  }




  //                              }
  //                              else
  //                              {

  //                                  //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
  //                                  link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManager.Guid;
  //                                  //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
  //                                  link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;

  //                                  aCats.TryGetValue(emailUIs.Assets[i].Type.Code, out rowSpan);

  //                                  if (categoryCode != emailUIs.Assets[i].Type.Code)
  //                                  {
  //                                      span = rowSpan;
  //                                      categoryCode = emailUIs.Assets[i].Type.Code;

  //                                      if (emailUIs.Assets[i].ImageName != "")
  //                                      {
  //                                          htmlBodyEmail1 = htmlBodyEmail1 + @"
  //                                                        <tr>
  //                                                           <td rowspan=""" + rowSpan + @""">" + emailUIs.Assets[i].Type.Code + @" </td>
  //                                                           <td>" + emailUIs.Assets[i].InvNo + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Name + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Model.Name + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
  //                                                          <td><img style = ""display: block"" width = ""100"" heigth = ""100""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
  //                                                          <td class=""description""><a class=""button"" href='" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                                          <td class=""description""><a class=""button-no"" href='" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                                                      </tr>
                                                       
  //                                      ";
  //                                      }
  //                                      else
  //                                      {
  //                                          htmlBodyEmail1 = htmlBodyEmail1 + @"
  //                                                        <tr>
  //                                                           <td rowspan=""" + rowSpan + @""">" + emailUIs.Assets[i].Type.Code + @" </td>
  //                                                           <td>" + emailUIs.Assets[i].InvNo + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Name + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Model.Name + @" </ td >
  //                                                           <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
  //                                                          <td>" + @" </td>
  //                                                          <td class=""description""><a class=""button"" href='" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                                          <td class=""description""><a class=""button-no"" href='" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                                                      </tr>
                                                       
  //                                      ";
  //                                      }
  //                                  }
  //                                  else
  //                                  {
  //                                      if (emailUIs.Assets[i].ImageName != "")
  //                                      {
  //                                          htmlBodyEmail1 = htmlBodyEmail1 + @"
  //                                                        <tr>
  //                                                           <td>" + emailUIs.Assets[i].InvNo + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Name + @" </ td >
  //                                                           <td>" + emailUIs.Assets[i].Model.Name + @" </ td >
  //                                                           <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
  //                                                          <td><img style = ""display: block"" width = ""100"" heigth = ""100""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
  //                                                          <td class=""description""><a class=""button"" href='" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                                          <td class=""description""><a class=""button-no"" href='" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                                                      </tr>
                                                       
  //                                      ";
  //                                      }
  //                                      else
  //                                      {
  //                                          htmlBodyEmail1 = htmlBodyEmail1 + @"
  //                                                        <tr>
  //                                                          <td>" + emailUIs.Assets[i].InvNo + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Name + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Model.Name + @" </ td >
  //                                                           <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
  //                                                          <td>" + @" </td>
  //                                                          <td class=""description""><a class=""button"" href='" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                                          <td class=""description""><a class=""button-no"" href='" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                                                      </tr>
                                                       
  //                                      ";
  //                                      }
  //                                  }
                                   

                                      


  //                              }
  //                          }
  //                          else
  //                          {
  //                              company1 = asset != null ? asset.Company.Name : "ITEMS";
  //                              company2 = asset != null ? asset.Company.Name : "LISTA ACCESORII";

  //                              htmlBodyCompany1 = htmlBodyCompany1 + @"<tr> <th colspan=""8"">" + company1 + "</th></tr>";

  //                              if (emailUIs.Assets[i].ImageName != "")
  //                              {
  //                                  //if (emailUIs.AssetComponents.Count > 0)
  //                                  //{
  //                                  //    htmlBodyCompany2 = htmlBodyCompany2 + @"<tr><th  colspan=""8"">" + company2 + "</th></tr>";

  //                                  //    htmlBodyEnd = htmlBodyEnd + htmlBodyCompany2 + @"
  //                                  //                    <tr>
  //                                  //                        <th class=""header"" colspan=""3"">Denumire</th>
  //                                  //                        <th class=""header"" >Photo</th>
  //                                  //                        <th class=""header"" colspan=""2"">Valid</th>
                                                            
  //                                  //                    </tr>
  //                                  //         ";
  //                                  //}



  //                                  //htmlBodyEnd = htmlBodyEnd + @"</tbody>
  //                                  //                    </table>
  //                                  //                        <h3> Pentru validarea acestora, acceseaza urmatorul link  https://service.inventare.ro/BOFO/#/operations
  //                                  //                        <br>
  //                                  //                        <h3> Multumesc, </ h3 >
  //                                  //                        <br>
  //                                  //                        <h3> Referent " + eIni + @" </ h3 >

  //                                  //                </body>
  //                                  //            </html> ";



  //                                  //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
  //                                  link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManager.Guid;
  //                                  //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
  //                                  link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;

  //                                  htmlBodyEmail1 = htmlBodyEmail1 + @"
  //                                                        <tr>
  //                                                           <td>" + emailUIs.Assets[i].Type.Code + @" </td>
  //                                                           <td>" + emailUIs.Assets[i].InvNo + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Name + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Model.Name + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
  //                                                          <td><img style = ""display: block"" width = ""100"" heigth = ""100""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
  //                                                          <td class=""description""><a class=""button"" href='" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                                          <td class=""description""><a class=""button-no"" href='" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                                                      </tr>
                                                       
  //                                      ";

  //                              }
  //                              else
  //                              {
  //                                  //if (emailUIs.AssetComponents.Count > 0)
  //                                  //{
  //                                  //    htmlBodyCompany2 = htmlBodyCompany2 + @"<tr><th class=""msg"" colspan=""5"">" + company2 + "</th></tr>";

  //                                  //    htmlBodyEnd = htmlBodyEnd + htmlBodyCompany2 + @"
  //                                  //                    <tr>
  //                                  //                        <th class=""header"" colspan=""3"">Denumire</th>
  //                                  //                        <th class=""header"" >Photo</th>
  //                                  //                        <th class=""header"" colspan=""2"">Valid</th>
                                                            
  //                                  //                    </tr>
  //                                  //         ";
  //                                  //}



  //                                  //htmlBodyEnd = htmlBodyEnd + @"</tbody>
  //                                  //                    </table>
  //                                  //                        <h3> Pentru validarea acestora, acceseaza urmatorul link  https://service.inventare.ro/BOFO/#/operations
  //                                  //                        <br>
  //                                  //                        <h3> Multumesc, </ h3 >
  //                                  //                        <br>
  //                                  //                        <h3> Referent " + eIni + @" </ h3 >

  //                                  //                </body>
  //                                  //            </html> ";



  //                                  //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
  //                                  link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManager.Guid;
  //                                  //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
  //                                  link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;

  //                                  htmlBodyEmail1 = htmlBodyEmail1 + @"
  //                                                        <tr>
  //                                                           <td>" + emailUIs.Assets[i].Type.Code + @" </td>
  //                                                           <td>" + emailUIs.Assets[i].InvNo + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Name + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].Model.Name + @" </ td >
  //                                                          <td>" + emailUIs.Assets[i].SerialNumber + @" </ td >
  //                                                          <td>" + @" </td>
  //                                                          <td class=""description""><a class=""button"" href='" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                                          <td class=""description""><a class=""button-no"" href='" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                                                      </tr>
                                                       
  //                                      ";

  //                              }



  //                          }


  //                      }

  //                  }
  //              }


  //              if (emailUIs.AssetComponents.Count > 0)
  //              {
                    

  //                  for (int i = 0; i < emailUIs.AssetComponents.Count; i++)
  //                  {

  //                      if (i == 0)
  //                      {
  //                          if (emailUIs.Assets.Count == 0)
  //                          {
  //                              Model.Employee eIni = null;
  //                              eIni = _context.Set<Model.Employee>().Include(d => d.Department).Where(e => e.Id == employeeId).SingleOrDefault();

  //                              emailManagerComponent = new Model.EmailManager
  //                              {
  //                                  EmailType = emailTypeAssetComponent,
  //                                  EmployeeInitial = eIni,
  //                                  EmployeeFinal = eIni,
  //                                  AssetId = null,
  //                                  AssetComponentId = emailUIs.AssetComponents[i].Id,
  //                                  Guid = Guid.NewGuid(),
  //                                  IsAccepted = false,
  //                                  AppStateId = 7
  //                              };

  //                              _context.Set<Model.EmailManager>().Add(emailManagerComponent);
  //                              _context.SaveChanges();
  //                          }


  //                          //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
  //                          link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManagerComponent.Guid;
  //                          //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
  //                          link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManagerComponent.Guid;

  //                          aCompCats.TryGetValue(emailUIs.AssetComponents[i].Type.Code, out rowSpanComp);

  //                          categoryComponentCode = emailUIs.AssetComponents[i].Type.Code;


  //                          if (emailUIs.AssetComponents[i].ImageName != "")
  //                          {
  //                              htmlBodyEmailComponent = htmlBodyEmailComponent + @"
  //                                                        <tr>
  //                                                          <td rowspan=""" + rowSpanComp + @""">" + emailUIs.AssetComponents[i].Type.Code + @" </ td >
  //                                                          <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
  //                                                          <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                         
  //                                                          <td>" + @" </ td >
  //                                                          <td>" + @" </ td >
  //                                                         <td><img style = ""display: block"" width = ""100"" heigth = ""100""  src = """ + imageLink + emailUIs.AssetComponents[i].ImageName + @"""  >" + @" </td>
                                                           
  //                                                      </tr>
                                                       
  //                                       ";
  //                                //<td class=""description""><a href = '" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                //<td class=""description""><a href = '" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                          }
  //                          else
  //                          {
  //                              htmlBodyEmailComponent = htmlBodyEmailComponent + @"
  //                                                        <tr>
  //                                                          <td rowspan=""" + rowSpanComp + @""">" + emailUIs.AssetComponents[i].Type.Code + @" </ td >
  //                                                          <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
  //                                                          <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
  //                                                          <td>" + @" </ td >
  //                                                          <td>" + @" </ td >
  //                                                         <td>" + @" </td>
                                                           
  //                                                      </tr>
                                                       
  //                                       ";

  //                               //<td class=""description""><a href = '" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                               //<td class=""description""><a href = '" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                          }
  //                      }
  //                      else
  //                      {
  //                          if (emailUIs.Assets.Count == 0)
  //                          {
  //                              Model.Employee eIni = null;
  //                              eIni = _context.Set<Model.Employee>().Include(d => d.Department).Where(e => e.Id == employeeId).SingleOrDefault();

  //                              emailManagerComponent = new Model.EmailManager
  //                              {
  //                                  EmailType = emailTypeAssetComponent,
  //                                  EmployeeInitial = eIni,
  //                                  EmployeeFinal = eIni,
  //                                  AssetId = null,
  //                                  AssetComponentId = emailUIs.AssetComponents[i].Id,
  //                                  Guid = Guid.NewGuid(),
  //                                  IsAccepted = true,
  //                                  AppStateId = 7
  //                              };

  //                              _context.Set<Model.EmailManager>().Add(emailManagerComponent);
  //                              _context.SaveChanges();
  //                          }


  //                          //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
  //                          link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManagerComponent.Guid;
  //                          //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
  //                          link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManagerComponent.Guid;

  //                          aCompCats.TryGetValue(emailUIs.AssetComponents[i].Type.Code, out rowSpanComp);

  //                          if (categoryComponentCode != emailUIs.AssetComponents[i].Type.Code)
  //                          {
  //                              spanComp = rowSpanComp;
  //                              categoryComponentCode = emailUIs.AssetComponents[i].Type.Code;


  //                              if (emailUIs.AssetComponents[i].ImageName != "")
  //                              {
  //                                  htmlBodyEmailComponent = htmlBodyEmailComponent + @"
  //                                                        <tr>
  //                                                          <td rowspan=""" + rowSpanComp + @""">" + emailUIs.AssetComponents[i].Type.Code + @" </ td >
  //                                                          <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
  //                                                          <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                         
  //                                                          <td>" + @" </ td >
  //                                                          <td>" + @" </ td >
  //                                                         <td><img style = ""display: block"" width = ""100"" heigth = ""100""  src = """ + imageLink + emailUIs.AssetComponents[i].ImageName + @"""  >" + @" </td>
                                                           
  //                                                      </tr>
                                                       
  //                                       ";

  //                                   //<td class=""description""><a href = '" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                   // <td class=""description""><a href = '" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                              }
  //                              else
  //                              {
  //                                  htmlBodyEmailComponent = htmlBodyEmailComponent + @"
  //                                                        <tr>
  //                                                          <td rowspan=""" + rowSpanComp + @""">" + emailUIs.AssetComponents[i].Type.Code + @" </ td >
  //                                                          <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
  //                                                          <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
  //                                                          <td>" + @" </ td >
  //                                                          <td>" + @" </ td >
  //                                                         <td>" + @" </td>
                                                           
  //                                                      </tr>
                                                       
  //                                       ";

  //                                   //< td class=""description""><a href = '" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                   //<td class=""description""><a href = '" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                              }
  //                          }
  //                          else
  //                          {
  //                              if (emailUIs.AssetComponents[i].ImageName != "")
  //                              {
  //                                  htmlBodyEmailComponent = htmlBodyEmailComponent + @"
  //                                                        <tr>
  //                                                           <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
  //                                                          <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
  //                                                          <td>" + @" </ td >
  //                                                          <td>" + @" </ td >
  //                                                          <td><img style = ""display: block"" width = ""100"" heigth = ""100""  src = """ + imageLink + emailUIs.AssetComponents[i].ImageName + @"""  >" + @" </td>
                                                          
  //                                                      </tr>
                                                       
  //                                       ";

  //                                    //< td class=""description""><a href = '" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                    //<td class=""description""><a href = '" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                              }
  //                              else
  //                              {
  //                                  htmlBodyEmailComponent = htmlBodyEmailComponent + @"
  //                                                        <tr>
  //                                                              <td>" + emailUIs.AssetComponents[i].Code + @" </ td >
  //                                                          <td>" + emailUIs.AssetComponents[i].Name + @" </ td >
  //                                                          <td>" + @" </ td >
  //                                                          <td>" + @" </ td >
  //                                                          <td>" + @" </td>
                                                           
  //                                                      </tr>
                                                       
  //                                       ";

  //                                   //<td class=""description""><a href = '" + link1 + "'" + "' >YES</a>" + @" </ td >
  //                                   // <td class=""description""><a href = '" + link2 + "'" + "' >NO</a>" + @" </ td >
  //                              }
  //                          }

                           
  //                      }
                      

                         

  //                  }
  //              }

  //              //string[] ImgPaths = Directory.GetFiles(Path.Combine(FullFormatPath, "upload"));
  //              //var bodyBuilder = new BodyBuilder { HtmlBody = "" };

  //              //foreach (string imgpath in ImgPaths)
  //              //{
  //              //    var image = bodyBuilder.LinkedResources.Add(imgpath);
  //              //    image.ContentId = MimeUtils.GenerateMessageId();
  //              //    //HtmlFormat = HtmlFormat.Replace(Path.GetFileName(imgpath), string.Format("cid:{0}", image.ContentId));
  //              //    imageLink = string.Format("cid:{0}", image.ContentId);
  //              //}

  //              var end1 = @"  </body>
  //                                      </html>";

  //              foreach (var item in dictCC)
  //              {
  //                  cc.Add(item.Value);
  //              }

		//		//if (cc.Count == 0)
		//		//{
		//		//    cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
		//		//}

		//		cc = new List<string>();
		//		cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");

		//		var bodyHtmlOut = htmlBody1 + headerMsg + htmlBody11 + htmlBodyCompany1 + htmlBody2 + htmlBodyEmail1 + htmlBodyEnd + htmlBodyEmailComponent + end + footerMsg + end1;
  //              // var messageAttach = new Message(cc, subject, bodyHtmlOut, files);
  //              var messageAttach = new Message(cc, subject, bodyHtmlOut, null);





  //              // var message = new Message(new string[] { "adrian.cirnaru@optima.ro" }, "Test email", "This is the content from our email.");

  //              // new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };


  //              //_emailSender.SendEmail(message);

  //              if (emailUIs.AssetComponents.Count > 0 || emailUIs.Assets.Count > 0)
  //              {
  //                  var success = await _emailSender.SendEmailAsync(messageAttach);

  //                  if (!success)
  //                  {
  //                      foreach (var item in guidIds)
  //                      {
  //                          Model.EmailManager eManager = _context.Set<Model.EmailManager>().Where(e => e.Guid == item.Value).SingleOrDefault();

  //                          if (eManager != null)
  //                          {
  //                              eManager.IsDeleted = true;
  //                              _context.Update(eManager);
  //                              _context.SaveChanges();
  //                          }
  //                      }


  //                  }
  //                  else
  //                  {
  //                      Model.Employee employee = _context.Set<Model.Employee>().Where(e => e.Id == employeeId).SingleOrDefault();

  //                      if (employee != null)
  //                      {
  //                          emailTypeAsset.NotifyLast = DateTime.Now;
  //                          emailTypeAssetComponent.NotifyLast = DateTime.Now;
  //                          employee.NotifyLast = DateTime.Now;
  //                          _context.Update(employee);
  //                          _context.Update(emailTypeAsset);
  //                          _context.Update(emailTypeAssetComponent);
  //                          _context.SaveChanges();
  //                      }
  //                  }

  //                  return true;
  //              }
  //              else
  //              {
  //                  foreach (var item in guidIds)
  //                  {
  //                      Model.EmailManager eManager = _context.Set<Model.EmailManager>().Where(e => e.Guid == item.Value).SingleOrDefault();

  //                      if (eManager != null)
  //                      {
  //                          eManager.IsDeleted = true;
  //                          _context.Update(eManager);
  //                          _context.SaveChanges();
  //                      }
  //                  }


  //                  return false;
  //              }


               

               

  //          }

            
  //      }

		//public async Task<bool> SenLinkdMail(int employeeId)
		//{
		//	using (IServiceScope scope = Services.CreateScope())
		//	{
		//		var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		//		var files = new FormFileCollection();
		//		Model.EmailType emailTypeAsset = null;
		//		Model.Asset asset = null;
		//		List<string> cc = new List<string>();
		//		Dictionary<int, string> dictCC = new Dictionary<int, string>();
		//		Dictionary<int, Guid> guidIds = new Dictionary<int, Guid>();
		//		Dictionary<string, int> aCats = new Dictionary<string, int>();
		//		int number = 0;
		//		var headerMsg = "";
		//		var footerMsg = "";
		//		var htmlBody1 = @"<!DOCTYPE html>
  //                                      <html lang=""en"">
  //                                          <head>    
  //                                              <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
  //                                              <title>
  //                                                  Upcoming topics
  //                                              </title>
                         
  //                                          </head>
  //                                          <body>         
  //                                      ";


		//		emailTypeAsset = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE ASSET INTERVAL").Single();
		//		headerMsg = emailTypeAsset.HeaderMsg;
		//		footerMsg = emailTypeAsset.FooterMsg;
		//		Model.Employee emp = _context.Set<Model.Employee>().AsNoTracking().Where(e => e.Id == employeeId).SingleOrDefault();
		//		var employeeLink = "https://service.inventare.ro/SocGen/#/employeevalidates/" + emp.Guid.ToString();
		//		//var employeeLink = "http://localhost:3100/#/employeevalidates/" + emp.Guid.ToString();
		//		var link = @"<h4><span style=""font-family: Roboto,Montserrat,helvetica neue,Helvetica,Arial,sans-serif;font-size: 12px;color: rgb(115, 115, 115)"">To review and validate your inventory list, please access the following link: <a style=""color: red; font-size: 16px;"" href = '" + employeeLink + "'" + "' >  Inventory list 2020</a>" + @"</span></h4>";
		//		var linkInfo = @"<h4><span style=""font-family: Roboto,Montserrat,helvetica neue,Helvetica,Arial,sans-serif;font-size: 12px;color: rgb(115, 115, 115)"">(If the link cannot be accessed with IE, please try using Chrome)" + @"</span></h4>";
		//		// var GuidAll = Guid.NewGuid();

		//		if (emp.Email != null && emp.Email != "" && !dictCC.ContainsValue(emp.Email))
		//		{
		//			dictCC.Add(number, emp.Email);
		//			number++;
		//		}

		//		foreach (var item in dictCC)
		//		{
		//			cc.Add(item.Value);
		//		}


		//		var subject = "Confirm inventory list 2020";



		//		var end1 = @"  </body>
  //                                      </html>";

				

		//		//if (cc.Count == 0)
		//		//{
		//		//	cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
		//		//}
		//		//else
		//		//{
		//		//	//cc.Add("DragosGabriel.Surcel@bcr.ro");
		//		//}

		//		//cc = new List<string>();

		//		//cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
		//		//cc.Add("andreea-raluca.podeanu@socgen.com");


		//		var bodyHtmlOut = htmlBody1 + headerMsg + link + linkInfo + footerMsg + end1;
		//		// var messageAttach = new Message(cc, subject, bodyHtmlOut, files);
  //              var messageAttach = new Message(cc, subject, bodyHtmlOut, null);

  //              var success = await _emailSender.SendEmailAsync(messageAttach);

		//		if (success)
		//		{

		//			Model.Employee employee = _context.Set<Model.Employee>().Where(e => e.Id == employeeId).SingleOrDefault();

		//			if (employee != null)
		//			{
		//				emailTypeAsset.NotifyLast = DateTime.Now;
		//				employee.NotifyLast = DateTime.Now;
		//				employee.IsEmailSend = true;
		//				_context.Update(employee);
		//				_context.Update(emailTypeAsset);
		//				_context.SaveChanges();
		//			}

		//		}

		//		return true;


		//	}


		//}

  //      public async Task<bool> SendNewAssetLinkMail(int employeeId)
  //      {
  //          using (IServiceScope scope = Services.CreateScope())
  //          {
  //              var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

  //              var files = new FormFileCollection();
  //              Model.EmailType emailTypeAsset = null;
  //              Model.Asset asset = null;
  //              List<string> cc = new List<string>();
  //              Dictionary<int, string> dictCC = new Dictionary<int, string>();
  //              Dictionary<int, Guid> guidIds = new Dictionary<int, Guid>();
  //              Dictionary<string, int> aCats = new Dictionary<string, int>();
  //              int number = 0;
  //              var headerMsg = "";
  //              var htmlBody1 = @"<!DOCTYPE html>
  //                                      <html lang=""en"">
  //                                          <head>    
  //                                              <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
  //                                              <title>
  //                                                  Inventariem echipamentele IT
  //                                              </title>
                         
  //                                          </head>
  //                                          <body>         
  //                                      ";


  //              emailTypeAsset = _context.Set<Model.EmailType>().Where(d => d.Code == "EMPLOYEE NEW ASSET").Single();
  //              headerMsg = emailTypeAsset.HeaderMsg;
  //              // footerMsg = emailTypeAsset.FooterMsg;
  //              Model.Employee emp = _context.Set<Model.Employee>().AsNoTracking().Where(e => e.Id == employeeId).SingleOrDefault();
  //              //var employeeLink = "https://service.inventare.ro/SameDayValidate/#/newassetemployee/" + emp.Guid.ToString();
  //              // var employeeLink = "http://localhost:4200/#/wfh/validate/";
  //              //var link = @"<h4><span style=""color: rgb(66, 149, 208)"">Pentru introducerea datelor va rugam sa accesati: <a style=""color: red; font-size: 20px;"" href = '" + employeeLink + "'" + "' >Adauga bunuri angajat</a>" + @"</span></h4>";
  //              //var linkInfo = @"<h4><span style=""color: rgb(66, 149, 208)"">In cazul in care link-ul nu poate fi accesat din Internet Explorer, va rugam sa-l accesati din Google Chrome" + @"</span></h4>";
  //              //var linkInfo2 = @"<h4><span style=""color: rgb(66, 149, 208)"">Datele tale cu caracter personal vor fi prelucrate in interesul legitim al Sameday de a realiza si mentine o evidenta a bunurilor predate fiecarui angajat iar la aceste date va avea acces, pentru o perioada de 4 ani, compania Optima Group, in calitate de furnizor al aplicatiei Optimal Fixed Assets ." + @"</span></h4>";
  //              // var GuidAll = Guid.NewGuid();

  //              if (emp.Email != null && emp.Email != "" && !dictCC.ContainsValue(emp.Email))
  //              {
  //                  dictCC.Add(number, emp.Email);
  //                  number++;
  //              }

  //              foreach (var item in dictCC)
  //              {
  //                  cc.Add(item.Value);
  //              }


  //              var subject = "Reminder - Inventariem echipamentele IT";



  //              var end1 = @"  </body>
  //                                      </html>";

  //              if (cc.Count == 0)
  //              {
  //                  cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
  //              }
		//		//else
		//		//{
		//		//	//cc.Add("DragosGabriel.Surcel@bcr.ro");

		//		//	cc = new List<string>();
		//		//	cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
		//		//	//cc.Add("DragosGabriel.Surcel@bcr.ro");
		//		//	//cc.Add("sorina.nanciu@optima.ro");
		//		//}


		//		var bodyHtmlOut = htmlBody1 + headerMsg + end1;
  //              var messageAttach = new Message(cc, subject, bodyHtmlOut, null);

  //              var success = await _emailSender.SendEmailAsync(messageAttach);

  //              if (success)
  //              {

  //                  Model.Employee employee = _context.Set<Model.Employee>().Where(e => e.Id == employeeId).SingleOrDefault();

  //                  if (employee != null)
  //                  {
  //                      emailTypeAsset.NotifyLast = DateTime.Now;
  //                      // emailTypeAssetComponent.NotifyLast = DateTime.Now;
  //                      employee.NotifyLast = DateTime.Now;
  //                      employee.IsEmailSend = true;
  //                      _context.Update(employee);
  //                      _context.Update(emailTypeAsset);
  //                      // _context.Update(emailTypeAssetComponent);
  //                      _context.SaveChanges();
  //                  }

  //              }

  //              return true;


  //          }


  //      }
    }
}
