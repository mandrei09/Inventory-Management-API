
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public class EmployeeNotValidatedEmailService : BackgroundService
    {
        private readonly IEmployeeNotValidatedService _employeeService;
        private readonly IEmailSender _emailSender;

        public EmployeeNotValidatedEmailService(IServiceProvider services, IEmployeeNotValidatedService employeeService, IEmailSender emailSender)
        {
            Services = services;
            _employeeService = employeeService;
            _emailSender = emailSender;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Model.EmployeeNotValidatedEmailResult> employeesListNotValidated = new List<Model.EmployeeNotValidatedEmailResult>();
            List<Model.EmployeeNotValidatedEmailResult> query = new List<Model.EmployeeNotValidatedEmailResult>();
            while (!stoppingToken.IsCancellationRequested)
            {
               

                try
                {
                    employeesListNotValidated = await _employeeService.GetEmployeesNotValidatedAsync();

                    query = employeesListNotValidated.GroupBy(e => e.Id).Select(p => new Model.EmployeeNotValidatedEmailResult
                    {
                        Id = p.FirstOrDefault().Id,
                        Email = p.FirstOrDefault().Email,
                        NotifyInterval = p.FirstOrDefault().NotifyInterval,
                        NotifyLast = p.FirstOrDefault().NotifyLast
                    }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            if (query[i].NotifyLast != null && DateTime.Now > query[i].NotifyLast.Value.AddDays(query[i].NotifyInterval))
                            {
                                await SendMailNotValidated(query[i].Id);
                            }
                            else if (query[i].NotifyLast == null)
                            {
                                await SendMailNotValidated(query[i].Id);
                            }



                        }
                    }
                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                }

                // await Task.Delay(86400000 * (query.Count > 0 ? query[0].NotifyInterval : 1), stoppingToken);
                await Task.Delay(3000000, stoppingToken);
            }


        }


        public async Task<bool> SendMailNotValidated(int employeeId)
        {
            using (IServiceScope scope = Services.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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
                string imageLink = "https://service.inventare.ro/socgenupload/";
                string defaultImage = "blank.jpg";
                var htmlBody1 = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    Upcoming topics
                                                </title>
                                                <style type=""text/css"">
                                                    HTML{background-color: #e8e8e8;}
                                                      .courses-table{border: 6px solid #948473;
                                                      background-color: #FFE3C6;
                                                      width: 100%;
                                                      text-align: center;}
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
                emailTypeAsset = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE ASSET INTERVAL").Single();
                emailTypeAssetComponent = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE COMPONENT INTERVAL").Single();

                //List<Model.EmailManager> emailManagers = _context.Set<Model.EmailManager>().Include(a => a.Asset).Where(e => e.EmailTypeId == emailTypeAsset.Id && e.EmployeeIdInitial == employeeId && e.IsAccepted == false).ToList();
                //List<Model.EmailManager> emailManagerComponents = _context.Set<Model.EmailManager>().Include(a => a.AssetComponent).Where(e => e.EmailTypeId == emailTypeAssetComponent.Id && e.EmployeeIdInitial == employeeId && e.IsAccepted == false).ToList();
                Model.Inventory inventory = _context.Set<Model.Inventory>().Where(i => i.Active == true).SingleOrDefault();
                List<Model.Asset> assets = _context.Set<Model.Asset>().Include(r => r.Room).Include(d => d.Document).Where(a => a.EmployeeId == employeeId && a.IsAccepted == false && a.Document.ParentDocumentId == inventory.DocumentId).ToList();
                List<Model.AssetComponent> assetComponents = _context.Set<Model.AssetComponent>().Where(a => a.EmployeeId == employeeId && a.IsAccepted == false).ToList();


                // var itemsResource = _mapper.Map<List<Model.Asset>, List<Dto.AssetUI>>(assets);


                EmailUI emailUIs = null;

                emailUIs = new EmailUI
                {
                    AssetComponents = new List<AssetComponent>(),
                    Assets = new List<AssetUI>()
                };

                foreach (var item in assetComponents)
                {
                    List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.EntityId == item.Id && e.EntityType.Code == "ASSETCOMPONENT").ToList();

                    emailUIs.AssetComponents.Add(new AssetComponent()
                    {

                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                        ImageName = entityFiles.Count > 0 ? entityFiles[0].StoredAs : ""
                    });
                }


                foreach (var item in assets)
                {
                    List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.EntityId == item.Id && e.EntityType.Code == "ASSET").ToList();

                    emailUIs.Assets.Add(new AssetUI()
                    {

                        Id = item.Id,
                        InvNo = item.InvNo,
                        Name = item.Name,
                        Room = new CodeNameEntity { Id = item.Room.Id, Code = item.Room.Code, Name = item.Room.Name },
                        ImageName = entityFiles.Count > 0 ? entityFiles[0].StoredAs : ""
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

                var subject = "Lista Active + Accesorii Nevalidated";


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

                            if (emailUIs.Assets.Count > 0 && emailUIs.Assets.Count > i)
                            {

                                emailManager = _context.Set<Model.EmailManager>().Include(d => d.EmailType).Where(e => e.AssetId == emailUIs.Assets[i].Id && e.EmailType.Id == emailTypeAsset.Id).SingleOrDefault();

                            }

                            if (emailUIs.AssetComponents.Count > 0 && emailUIs.AssetComponents.Count > i)
                            {

                                emailManagerComponent = _context.Set<Model.EmailManager>().Include(d => d.EmailType).Where(e => e.AssetComponentId == emailUIs.AssetComponents[i].Id && e.EmailType.Id == emailTypeAssetComponent.Id).SingleOrDefault();

                            }




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

                            emailManager.ModifiedAt = DateTime.Now;
                           
                            

                            if (emailUIs.AssetComponents.Count > 0)
                            {
                                emailManagerComponent.ModifiedAt = DateTime.Now;
                                _context.Update(emailManagerComponent);
                               
                            }
                            _context.Update(emailManager);
                            _context.SaveChanges();


                            if (emailUIs.Assets.Count() > 1)
                            {
                                if (i == 0)
                                {
                                    if (emailUIs.Assets[i].ImageName != "")
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
                                                            <th class=""header"">Photo</th>
                                                            <th class=""header"" colspan=""2"">Valid</th>
                                                            
                                                        </tr>
                                             ";
                                        }




                                        //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
                                        link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManager.Guid;
                                        //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
                                        link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;

                                        htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                            <td class=""description"">" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td class=""description"">" + emailUIs.Assets[i].Name + @" </ td >
                                                         
                                                            <td class=""description"">" + emailUIs.Assets[i].Room.Code + @" </ td >
                                                            <td class=""description""><img style = ""display: block"" width = ""100"" heigth = ""100""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
                                                            <td class=""description""><a href='" + link1 + "'" + "' >DA</a>" + @" </ td >
                                                            <td class=""description""><a href='" + link2 + "'" + "' >NU</a>" + @" </ td >
                                                        </tr>
                                                       
                                        ";
                                    }
                                    else
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
                                                            <th class=""header"">Photo</th>
                                                            <th class=""header"" colspan=""2"">Valid</th>
                                                            
                                                        </tr>
                                             ";
                                        }




                                        //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
                                        link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManager.Guid;
                                        //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
                                        link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;

                                        htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                            <td class=""description"">" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td class=""description"">" + emailUIs.Assets[i].Name + @" </ td >
                                                         
                                                            <td class=""description"">" + emailUIs.Assets[i].Room.Code + @" </ td >
                                                            <td class=""description""><img style = ""display: block"" width = ""100"" heigth = ""100""  src = """ + imageLink + emailUIs.AssetComponents[i].ImageName + @"""  >" + @" </td>
                                                            <td class=""description""><a href='" + link1 + "'" + "' >DA</a>" + @" </ td >
                                                            <td class=""description""><a href='" + link2 + "'" + "' >NU</a>" + @" </ td >
                                                        </tr>
                                                       
                                        ";
                                    }
                                        


                                }
                                else
                                {

                                    //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
                                    link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManager.Guid;
                                    //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
                                    link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;

                                    htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                             <td class=""description"">" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td class=""description"">" + emailUIs.Assets[i].Name + @" </ td >
                                                          
                                                            <td class=""description"">" + emailUIs.Assets[i].Room.Code + @" </ td >
                                                            <td class=""description"">" + @" </td>
                                                            <td class=""description""><a href='" + link1 + "'" + "' >DA</a>" + @" </ td >
                                                            <td class=""description""><a href='" + link2 + "'" + "' >NU</a>" + @" </ td >
                                                        </tr>
                                                       
                                        ";


                                }
                            }
                            else
                            {
                                company1 = asset != null ? asset.Company.Name : "LISTA ACTIVE";
                                company2 = asset != null ? asset.Company.Name : "LISTA ACCESORII";

                                htmlBodyCompany1 = htmlBodyCompany1 + @"<tr> <th class=""msg"" colspan=""5"">" + company1 + "</th></tr>";

                                if (emailUIs.Assets[i].ImageName != "")
                                {
                                    if (emailUIs.AssetComponents.Count > 0)
                                    {
                                        htmlBodyCompany2 = htmlBodyCompany2 + @"<tr><th class=""msg"" colspan=""5"">" + company2 + "</th></tr>";

                                        htmlBodyEnd = htmlBodyEnd + htmlBodyCompany2 + @"
                                                        <tr>
                                                            <th class=""header"" colspan=""3"">Denumire</th>
                                                            <th class=""header"" >Photo</th>
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



                                    //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
                                    link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManager.Guid;
                                    //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
                                    link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;

                                    htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                            <td class=""description"">" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td class=""description"">" + emailUIs.Assets[i].Name + @" </ td >
                                                          
                                                            <td class=""description"">" + emailUIs.Assets[i].Room.Code + @" </ td >
                                                             <td class=""description""><img style = ""display: block"" width = ""100"" heigth = ""100""  src = """ + imageLink + emailUIs.Assets[i].ImageName + @"""  >" + @" </td>
                                                            <td class=""description""><a href='" + link1 + "'" + "' >DA</a>" + @" </ td >
                                                            <td class=""description""><a href='" + link2 + "'" + "' >NU</a>" + @" </ td >
                                                        </tr>
                                                       
                                        ";

                                }
                                else
                                {
                                    if (emailUIs.AssetComponents.Count > 0)
                                    {
                                        htmlBodyCompany2 = htmlBodyCompany2 + @"<tr><th class=""msg"" colspan=""5"">" + company2 + "</th></tr>";

                                        htmlBodyEnd = htmlBodyEnd + htmlBodyCompany2 + @"
                                                        <tr>
                                                            <th class=""header"" colspan=""3"">Denumire</th>
                                                            <th class=""header"" >Photo</th>
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



                                    //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
                                    link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManager.Guid;
                                    //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
                                    link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManager.Guid;

                                    htmlBodyEmail1 = htmlBodyEmail1 + @"
                                                          <tr>
                                                            <td class=""description"">" + emailUIs.Assets[i].InvNo + @" </ td >
                                                            <td class=""description"">" + emailUIs.Assets[i].Name + @" </ td >
                                                          
                                                            <td class=""description"">" + emailUIs.Assets[i].Room.Code + @" </ td >
                                                            <td class=""description"">" + @" </td>
                                                            <td class=""description""><a href='" + link1 + "'" + "' >DA</a>" + @" </ td >
                                                            <td class=""description""><a href='" + link2 + "'" + "' >NU</a>" + @" </ td >
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
                        if (emailUIs.Assets.Count == 0)
                        {
                            Model.Employee eIni = null;
                            eIni = _context.Set<Model.Employee>().Include(d => d.Department).Where(e => e.Id == employeeId).SingleOrDefault();

                            emailManagerComponent = new Model.EmailManager
                            {
                                EmailType = emailTypeAssetComponent,
                                EmployeeInitial = eIni,
                                EmployeeFinal = eIni,
                                AssetId = null,
                                AssetComponentId = emailUIs.AssetComponents[i].Id,
                                Guid = Guid.NewGuid(),
                                IsAccepted = false
                            };

                            _context.Set<Model.EmailManager>().Add(emailManagerComponent);
                            _context.SaveChanges();
                        }

                        //link1 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/validatemultiple/" + document.Id + "/2";
                        link1 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/validate/" + emailManagerComponent.Guid;
                        //link2 = "https://service.inventare.ro/FaisSocgen/api/entityfiles/notvalidate/" + document.Id;
                        link2 = "https://service.inventare.ro/FaisSocgen/api/emailmanagers/notvalidate/" + emailManagerComponent.Guid;


                        if (emailUIs.AssetComponents[i].ImageName != "")
                        {
                            htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                            <td class=""description"" colspan=""3"">" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                           <td class=""description""><img style = ""display: block"" width = ""100"" heigth = ""100""  src = """ + imageLink + emailUIs.AssetComponents[i].ImageName + @"""  >" + @" </td>
                                                            <td class=""description""><a href='" + link1 + "'" + "' >DA</a>" + @" </ td >
                                                            <td class=""description""><a href='" + link2 + "'" + "' >NU</a>" + @" </ td >
                                                        </tr>
                                                       
                                         ";
                        }
                        else
                        {
                            htmlBodyEmailComponent = htmlBodyEmailComponent + @"
                                                          <tr>
                                                            <td class=""description"" colspan=""3"">" + emailUIs.AssetComponents[i].Name + @" </ td >
                                                            <td class=""description"">" + @" </td>
                                                            <td class=""description""><a href='" + link1 + "'" + "' >DA</a>" + @" </ td >
                                                            <td class=""description""><a href='" + link2 + "'" + "' >NU</a>" + @" </ td >
                                                        </tr>
                                                       
                                         ";
                        }

                           

                    }
                }

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
                                                    <h3> Pentru validarea unui activ din lista se foloseste butonul DA </h3>
                                                    <h3> Pentru stergerea unui activ din lista se foloseste butonul NU </h3>

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
                //var messageAttach = new Message(cc, subject, bodyHtmlOut, files);
                var messageAttach = new Message(cc, cc, bcc, subject, bodyHtmlOut, null);





                // var message = new Message(new string[] { "adrian.cirnaru@optima.ro" }, "Test email", "This is the content from our email.");

                // new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };


                //_emailSender.SendEmail(message);

                if (emailUIs.AssetComponents.Count > 0 || emailUIs.Assets.Count > 0)
                {
                    var success = await _emailSender.SendEmailAsync(messageAttach);
                }


                return true;

            }


        }

    }
}
