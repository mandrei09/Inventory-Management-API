using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optima.Fais.Api.Helpers;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Dto.Sync;
using Optima.Fais.EfRepository;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using PdfSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/offers")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class OffersController : GenericApiController<Model.Offer, Dto.Offer>
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<Model.ApplicationUser> userManager;

        private readonly IEmailSender _emailSender;

        public OffersController(ApplicationDbContext context,
            IOffersRepository itemsRepository, IMapper mapper, IWebHostEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager, IEmailSender emailSender)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            this._emailSender = emailSender;
        }
        
        [HttpGet]
        [Route("", Order = -1)]
        public virtual async Task<IActionResult> GetDepDetails(int page, int pageSize, string sortColumn, string sortDirection,
            string includes, string jsonFilter, string filter, string columnFilter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            OfferFilter offerFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string role = string.Empty;

            includes ??= "Offer.Company,Offer.Employee,Offer.Partner,Offer.AppState";

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            offerFilter = jsonFilter != null ? JsonConvert.DeserializeObject<OfferFilter>(jsonFilter) : new OfferFilter();

            if (filter != null)
			{
                offerFilter.Filter = filter;
            }

            if (HttpContext.User.Identity.Name != null)
            {
                var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                }
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                offerFilter.Role = role;
                offerFilter.InInventory = user.InInventory;
                offerFilter.UserId = user.Id;

                if (employeeId != null)
                {
                    offerFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    offerFilter.EmployeeIds = null;
                    offerFilter.EmployeeIds = new List<int?>();
                    offerFilter.EmployeeIds.Add(int.Parse("-1"));
                }
            }
            else
            {
                offerFilter.EmployeeIds = null;
                offerFilter.EmployeeIds = new List<int?>();
                offerFilter.EmployeeIds.Add(int.Parse("-1"));
            }


            var items = (_itemsRepository as IOffersRepository)
                .GetOffer(offerFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.OfferDetail>, List<Dto.Offer>>(items);

            var result = new OfferPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }

        [HttpGet]
        [Route("detailui", Order = -1)]
        public virtual async Task<IActionResult> GetDepDetailUIs(int page, int pageSize, string sortColumn, string sortDirection,
           string includes, string jsonFilter, string filter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            OfferFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            //var countOffer = _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToList();
            //var countA = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials").ToList();

            includes = "Offer.Request.BudgetForecast.BudgetBase,Offer.AdmCenter,Offer.Region,Offer.AssetType,Offer.ProjectType,Offer.Company,Offer.Employee,Offer.Partner,Offer.AppState,Offer.BudgetForecast.BudgetBase,Offer.Uom,Offer.OfferType";

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<OfferFilter>(jsonFilter) : new OfferFilter();


            if (filter != null)
            {
                assetFilter.Filter = filter;
            }

            var items = (_itemsRepository as IOffersRepository)
                .GetOfferUI(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.OfferDetail>, List<Dto.OfferUI>>(items);

            var result = new OfferUIPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }


        [HttpGet]
        [Route("newofferdetailui", Order = -1)]
        public virtual async Task<IActionResult> GetDepNewOfferDetailUIs(int page, int pageSize, string sortColumn, string sortDirection,
           string includes, string jsonFilter, string filter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            OfferFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            //var countOffer = _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToList();
            //var countA = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials").ToList();

            includes = "Offer.Request.BudgetForecast.BudgetBase,Offer.AdmCenter,Offer.Region,Offer.AssetType,Offer.ProjectType,Offer.Company,Offer.Employee,Offer.Partner,Offer.AppState,Offer.BudgetForecast.BudgetBase,Offer.Uom,Offer.OfferType";

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<OfferFilter>(jsonFilter) : new OfferFilter();


            if (filter != null)
            {
                assetFilter.Filter = filter;
            }

            var items = (_itemsRepository as IOffersRepository)
                .GetAllOfferUI(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.OfferDetail>, List<Dto.OfferUI>>(items);

            var result = new OfferUIPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }

        [HttpPost("detail")]
        public async Task<OfferResult> PostDetail([FromBody] OfferSave offer)
        {
            Model.OfferResult offerResult = null;
            Model.EmailResult emailResult = null;

            if (HttpContext.User.Identity.Name != null)
			{
                var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                if(user == null)
				{
                    user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                }

                if(user != null)
				{
                    _context.UserId = user.Id.ToString();
                    offer.UserId = user.Id;
                    offer.EmployeeId = user.EmployeeId;

					try
					{
                        offerResult = await (_itemsRepository as IOffersRepository).CreateOrUpdateOffer(offer);

                        if (offerResult.Success)
                        {
                            for (int i = 0; i < offer.PartnerIds.Length; i++)
                            {
                                emailResult = await SendEmail(offerResult.OfferId, offer.PartnerIds[i], offer.Guid, offer.OfferTypeId);
                            }

                            if (emailResult.Success)
                            {
                                return new Model.OfferResult { Success = true, Message = $"Notificarea a fost trimisa cu succes!", OfferId = offerResult.OfferId };
                            }
                            else
                            {
                                return new Model.OfferResult { Success = false, Message = $"Eroare trimitere notificare!", OfferId = offerResult.OfferId };
                            }
                        }
                        else
                        {
                            return new Model.OfferResult { Success = false, Message = offerResult.Message, OfferId = 0 };
                        }
                    }
					catch (Exception ex)
					{

                        return new Model.OfferResult { Success = false, Message = ex.Message, OfferId = 0 };
                    }
                }
                else
                {
                    return new Model.OfferResult { Success = false, Message = $"Userul nu exista!", OfferId = 0 };
                }
            }
            else
            {
                return new Model.OfferResult { Success = false, Message = $"Va rugam sa va autentificati!", OfferId = 0 };
            }

        }

        [HttpGet("detail/{id:int}")]
        public virtual IActionResult GetDetail(int id, string includes)
        {
            var asset = (_itemsRepository as IOffersRepository).GetDetailsById(id, includes);
            var result = _mapper.Map<Dto.Offer>(asset);

            return Ok(result);
        }


    
        [HttpPost("sendBookEmail")]
        public async Task<EmailResult> SendEmail(int offerId, CodePartnerEntity partner, Guid guid, int offerTypeId)
        {
            Model.EmailResult emailResult = null;
            Model.OfferType offerType = null;
            Model.Offer offer = null;
            Model.EmailType eType = null;
            Model.AppState appState = null;
            Model.EmailManager emailManager = null;
			Model.Inventory inventory = null;
			Model.EntityType entityType = null;
            Model.EmailOfferStatus emailOfferStatus = null;
			int documentNumber = 0;
			//List<Model.EntityFile> entityFilesOFFER = new List<Model.EntityFile>();
			//List<Model.EntityFile> entityFilesREQUEST = new List<Model.EntityFile>();
			bool success = false;
            string filePath = string.Empty;

            if (HttpContext.User.Identity.Name != null)
			{
                var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                }

                if (user != null)
				{
                    _context.UserId = user.Id.ToString();
                    _context.UserName = user.UserName;

                    offerType = await _context.Set<Model.OfferType>().Where(c => c.Id == offerTypeId).FirstOrDefaultAsync();
                    if (offerType == null) return new Model.EmailResult { Success = false, Message = "Nu exista tip oferta!", EmailManagerId = 0 };
                    offer = await _context.Set<Model.Offer>().Include(r => r.Request).Where(c => c.Id == offerId).FirstOrDefaultAsync();
                    if (offer == null) return new Model.EmailResult { Success = false, Message = "Nu exista oferta!", EmailManagerId = 0 };
                    appState = await _context.Set<Model.AppState>().Where(c => c.Code == "PENDING").FirstOrDefaultAsync();
                    if (appState == null) return new Model.EmailResult { Success = false, Message = "Nu exista stare!", EmailManagerId = 0 };

                    if (partner == null) return new Model.EmailResult { Success = false, Message = "Nu exista Furnizor!", EmailManagerId = 0 };
                    offer.PartnerId = partner.Id;

					try
					{
                        emailResult = await (_itemsRepository as IOffersRepository).SendEmail(offer.Id, partner);

                        List<string> ccOut = new();
                        List<string> cc = new List<string>();
                        List<string> bcc = new List<string>();
                        ccOut.Add(emailResult.To);
                        //ccOut.Add("georgiana.nedelcu@emag.ro");
                        //ccOut.Add("adrian.daniu@emag.ro");
                        //ccOut.Add("ana.diaconu@emag.ro");
                        //ccOut.Add("cosmin.maiorescu@emag.ro");

                        if (partner.ContactInfo != null && partner.ContactInfo != "")
                        {
                            ccOut.Add(partner.ContactInfo);
                        }

                        if (partner.Bank != null && partner.Bank != "")
                        {
                            ccOut.Add(partner.Bank);
                        }

                        if (partner.BankAccount != null && partner.BankAccount != "")
                        {
                            ccOut.Add(partner.BankAccount);
                        }

                        // ccOut.Add(emailIniOut);
                        //ccOut.Add(emailCCOut);

       //                 entityFilesOFFER = await _context.Set<Model.EntityFile>()
							//.Include(e => e.EntityType)
							//.Where(e => e.GuidAll == guid && e.IsEmailSend == false && e.PartnerId == partner.Id).ToListAsync();

       //                 entityFilesREQUEST = await _context.Set<Model.EntityFile>()
       //                    .Include(e => e.EntityType)
       //                    .Where(e => e.GuidAll == guid && e.IsEmailSend == false && e.RequestId == offer.RequestId).ToListAsync();

                        List<(string, string, string)> filePaths = new List<(string, string, string)>();

						//for (int i = 0; i < entityFilesOFFER.Count; i++)
						//{
						//	var filePath = Path.Combine("offerui", entityFilesOFFER[i].StoredAs);
						//	filePaths.Add(new(filePath, entityFilesOFFER[i].Name, entityFilesOFFER[i].FileType));
						//}

      //                  for (int i = 0; i < entityFilesREQUEST.Count; i++)
      //                  {
      //                      var filePath = Path.Combine("requestbudgetforecast", entityFilesREQUEST[i].StoredAs);
      //                      filePaths.Add(new(filePath, entityFilesREQUEST[i].Name, entityFilesREQUEST[i].FileType));
      //                  }

                        for (int i = 0; i < partner.EntityFiles.Length; i++)
                        {
                            filePath = Path.Combine(partner.EntityFiles[i].EntityType.UploadFolder, partner.EntityFiles[i].StoredAs);
                            filePaths.Add(new(filePath, partner.EntityFiles[i].Name, partner.EntityFiles[i].FileType));
                        }


                        var messageAttach = new Message(ccOut, cc, bcc, emailResult.Subject, emailResult.Body, filePaths);

                        if (offerType.Code == "O-V" || offerType.Code == "O-C" || offerType.Code == "O-E")
                        {
                            success = true;

                            eType = await _context.Set<Model.EmailType>().Where(e => e.Code == "VALIDATED_OFFER").FirstOrDefaultAsync();
                            if (eType == null) return new Model.EmailResult { Success = false, Message = "Nu exista tip email O-V!", EmailManagerId = 0 };
                        }
                        else
                        {
							inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Active == true).SingleOrDefault();
							entityType = _context.Set<Model.EntityType>().AsNoTracking().Where(a => a.Code == "NEWOFFER").SingleOrDefault();

							eType = await _context.Set<Model.EmailType>().Where(e => e.Code == "NEW_OFFER").FirstOrDefaultAsync();
							if (eType == null) return new Model.EmailResult { Success = false, Message = "Nu exista tip email oferta!", EmailManagerId = 0 };

							documentNumber = int.Parse(entityType.Name);

							documentNumber++;

							appState = _context.Set<Model.AppState>().Where(a => a.Code == "OFFERUI_DOCUMENT").SingleOrDefault();

							emailOfferStatus = new Model.EmailOfferStatus()
							{
                                AppStateId = appState.Id,
								CompanyId = offer.CompanyId,
								CreatedAt = DateTime.Now,
								CreatedBy = _context.UserId,
								DocumentNumber = documentNumber,
                                EmailSend = false,
								EmailTypeId = eType.Id,
								EmployeeId = offer.EmployeeId,
                                OwnerId = null,
								ErrorId = null,
								Guid = guid,
								GuidAll = guid,
								Message = partner.Name,
								IsAccepted = false,
								IsDeleted = false,
								ModifiedAt = DateTime.Now,
								ModifiedBy = _context.UserId,
                                NotSync = true,
								OfferId = offerId,
								PartnerId = partner.Id,
								RequestId = offer.RequestId,
								SyncErrorCount = 0,

							};

							entityType.Name = documentNumber.ToString();

							_context.Add(emailOfferStatus);
							_context.Update(entityType);

                            success = true;

							//success = await _emailSender.SendEmailAsync(messageAttach);
						}

                        emailManager = new Model.EmailManager()
                        {
                            EmailTypeId = eType.Id,
                            OfferId = offerId,
                            PartnerId = partner.Id,
                            //SubTypeId = subTypeId,
                            AppStateId = appState.Id,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _context.UserName,
                            ModifiedAt = DateTime.Now,
                            ModifiedBy = _context.UserName,
                            Guid = Guid.NewGuid(),
                            GuidAll = Guid.NewGuid(),
                            IsDeleted = false,
                            CompanyId = offer.CompanyId,
                            EmployeeIdFinal = offer.EmployeeId,
                            EmployeeIdInitial = offer.EmployeeId

                        };

                        _context.Add(emailManager);


                        // Model.EntityFile entityFile = _context.Set<Model.EntityFile>().Where(e => e.GuidAll == guid && e.PartnerId == partner.Id && e.IsEmailSend == false).SingleOrDefault();

                        //for (int i = 0; i < entityFiles.Count; i++)
                        //{
                        //    entityFiles[i].IsEmailSend = true;
                        //    _context.Update(entityFiles[i]);
                        //    _context.SaveChanges();
                        //}

                        _context.SaveChanges();

                        if (success)
                        {
                            return new Model.EmailResult { Success = true, Message = emailResult.Message, EmailManagerId = emailManager != null ? emailManager.Id : 0 };

                        }
                        else
                        {
                            return new Model.EmailResult { Success = false, Message = $"Eroare trimitere email", EmailManagerId = emailManager != null ? emailManager.Id : 0 };
                        }
                    }
					catch (Exception ex)
					{

                        return new Model.EmailResult { Success = false, Message = ex.Message, EmailManagerId = 0 };
                    }

                    
                }
                else
                {
                    return new Model.EmailResult { Success = false, Message = $"Userul nu exista!", EmailManagerId = 0 };
                }
            }
            else
            {
                return new Model.EmailResult { Success = false, Message = $"Va rugam sa va autentificati!", EmailManagerId = 0 };
            }
        }

        [HttpGet("sendBookEmailPreview/{offerId}")]
        public async Task<string> SendBookEmailPreview(int offerId)
        {

            if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
            Model.Offer offer = null;
            Model.EmailType emailType = null;
            Model.Partner partner = null;
            var emailIni = "";
            var emailCC = "";
            var headerMsg = "";
            var footerMsg = "";
            var htmlBodyEmail = "";
            var htmlBodyEnd = @"</tbody>
                                                </table>
                                            </body>
                                        </html> ";
            var htmlBody = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    New Offer
                                                </title>
                                                <style type=""text/css"">
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
                                                </style>
                                            </head>
                                            <body>
                                                <h4>Detalii oferta:</h4>
                                                <table class=""courses-table"">
                                                    <thead>
                                                        <tr>
                                                            <th class=""red"">Owner</th>
                                                            <th class=""red"">Quantity</th>
                                                            <th class=""red"">Details</th>
                                                            <th class=""red"">Supplier</th>
                                                            <th class=""red"">Comment</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                        ";
            var subject = "New Offer validation";

            if (offerId > 0)
            {

                var user = _context.Users.Where(u => u.UserName == _context.UserName).Single();
                

                offer = _context.Set<Model.Offer>()
                    .Include(e => e.Company)
                    //.Include(e => e.Project)
                    //.Include(e => e.Administration)
                    //.Include(e => e.CostCenter)
                    .Include(e => e.SubType)
                        .ThenInclude(e => e.Type)
                            .ThenInclude(e => e.MasterType)
                    .Include(e => e.Employee)
                        .ThenInclude(d => d.Department)
                    // .Include(e => e.Partner)
                    //.Include(e => e.Account)
                    //.Include(e => e.AccMonth)
                    //.Include(e => e.InterCompany)
                    .Where(a => a.Id == offerId).Single();


                List<int> partners = await _context.Set<Model.SubTypePartner>().AsNoTracking().Where(u => u.SubTypeId == offer.SubTypeId).Select(p => p.Partner.Id).ToListAsync();
                partner = _context.Set<Model.Partner>().AsNoTracking().Where(u => u.Id == partners[0]).Single();

                if (offer.Employee != null && offer.Employee.Email != "" && offer.Employee.Email != null)
                {
                    emailIni = offer.Employee.Email;

                    if (offer.Employee.Department != null && offer.Employee.Department.Name != null && offer.Employee.Department.Name != "")
                    {
                        emailCC = offer.Employee.Department.Name;
                    }
                    else
                    {
                        emailCC = "adrian.cirnaru@optima.ro";
                    }
                }
                else
                {
                    emailIni = "adrian.cirnaru@optima.ro";
                    offer.Employee = new Model.Employee();
                }

                htmlBodyEmail = htmlBodyEmail + @"
                                                        <tr>
                                                            <td class=""description"">" + offer.Employee.FirstName + " " + offer.Employee.LastName + @" </td>
                                                             <td class=""description"">" + offer.Quantity + @" </td>
                                                            <td class=""description"">" + offer.SubType.Name + @" </td>
                                                            <td class=""description"">" + partner.Name + @" </td>
                                                             <td class=""description"">" + offer.Info + @" </td>
                                                        </tr>
                                        ";
            }


            emailType = _context.Set<Model.EmailType>().Where(d => d.Code == "NEW_OFFER").Single();
            headerMsg = String.Format("{0}", emailType.HeaderMsg);
            footerMsg = String.Format("{0}", emailType.FooterMsg);
            //  var budgetLink = "https://service.inventare.ro/Emag/#/budgetvalidate/" + offer.Guid.ToString();
            //var budgetLink = "http://localhost:3100/#/budgetvalidate/" + budget.Guid.ToString();
            // var link = @"<h4><span style=""font-family: Roboto,Montserrat,helvetica neue,Helvetica,Arial,sans-serif;font-size: 12px;color: rgb(115, 115, 115)"">To review and validate new budget, please access the following link: <a style=""color: red; font-size: 16px;"" href = '" + budgetLink + "'" + "' >  VALIDATE BUDGET: " + offer.Code + "</a>" + @"</span></h4>";
            // var linkInfo = @"<h4><span style=""font-family: Roboto,Montserrat,helvetica neue,Helvetica,Arial,sans-serif;font-size: 12px;color: rgb(115, 115, 115)"">(If the link cannot be accessed with IE, please try using Chrome)" + @"</span></h4>";

            // bodyHtmlOut = htmlBody + htmlBodyEmail + htmlBodyEnd;
            htmlBody = htmlBody + headerMsg + htmlBodyEmail + htmlBodyEnd + footerMsg;// + link + linkInfo;

            return htmlBody;
        }

        [HttpPost("sendEmail")]
        // [Authorize]
        public async Task<IActionResult> SendValidatedEmail(int budgetId)
        {

            if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
            var id = (_itemsRepository as IOffersRepository).SendValidatedEmail(budgetId, _context.UserName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut);

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
        [Route("Offervalidate")]
        [AllowAnonymous]
        public virtual IActionResult OfferValidate(int page, int pageSize, string sortColumn, string sortDirection,
            string includes, string jsonFilter, string userId)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            OfferFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<OfferFilter>(jsonFilter) : new OfferFilter();

            var items = (_itemsRepository as IOffersRepository)
                .BudgetValidate(assetFilter, includes, userId, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.OfferDetail>, List<Dto.Offer>>(items);

            var result = new OfferPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }

        
        [HttpPost]
        [AllowAnonymous]
        [Route("validate/{appStateId}")]
        public async Task<IActionResult> OrderValidate([FromBody] int[] employee, int appStateId)
        {
			await Task.Delay(0);
            Model.Offer budget = null;
            Model.OfferOp budgetOp = null;
            Model.Document document = null;

            string documentTypeCode = "IS_MINUS";



            //var documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();
            

            //document = new Model.Document
            //{
            //    Approved = true,
            //    DocumentType = documentType,
            //    DocNo1 = string.Empty,
            //    DocNo2 = string.Empty,
            //    DocumentDate = DateTime.Now,
            //    RegisterDate = DateTime.Now,
            //    Partner = null
            //};

            //_context.Add(document);

            //for (int i = 0; i < employee.Length; i++)
            //{
            //    budget = _context.Set<Model.Offer>().Where(a => a.Id == employee[i].BudgetId).SingleOrDefault();
            //    budget.IsAccepted = employee[i].Accepted;
            //    budget.AppStateId = employee[i].Accepted ? 7 : employee[i].Reason != null && employee[i].Reason.Trim().Length > 1 ? 8 : 1;
            //    budget.Name =employee[i].Reason != null && employee[i].Reason.Trim().Length > 1 && employee[i].Accepted == false ? employee[i].Reason : "";
            //    //int employeeId = _context.Set<Model.Employee>().Include(d => d.Division).Where(d => d.Id == budget.EmployeeId).AsNoTracking().Select(e => e.Id).SingleOrDefault();
            //    //string userId = _context.Set<Model.ApplicationUser>().Where(d => d.EmployeeId == employeeId).AsNoTracking().Select(e => e.Id).SingleOrDefault();


            //    budgetOp = new Model.OfferOp()
            //    {
            //        OfferId = budget.Id,
                    
            //        AccMonthId = budget.AccMonthId,
            //        
            //        
            //        AdministrationIdFinal = budget.AdministrationId,
            //        AdministrationIdInitial = budget.AdministrationId,
            //        BudgetManagerIdFinal = budget.BudgetManagerId,
            //        BudgetManagerIdInitial = budget.BudgetManagerId,
            //        BudgetStateId = budget.AppStateId,
            //        CompanyIdFinal = budget.CompanyId,
            //        CompanyIdInitial = budget.CompanyId,
            //        CostCenterIdFinal = budget.CostCenterId,
            //        CostCenterIdInitial = budget.CostCenterId,
            //        CreatedAt = DateTime.UtcNow,
            //        CreatedBy = null,
            //        DocumentId = document.Id,
            //        DstConfAt = DateTime.UtcNow,
            //        DstConfBy = null,
            //        EmployeeIdFinal = budget.EmployeeId,
            //        EmployeeIdInitial = budget.EmployeeId,
            //        InfoIni = budget.Info,
            //        InfoFin = employee[0].Reason,
            //        InterCompanyIdFinal = budget.InterCompanyId,
            //        InterCompanyIdInitial = budget.InterCompanyId,
            //        IsAccepted = employee[0].Accepted,
            //        IsDeleted = false,
            //        ModifiedAt = DateTime.UtcNow,
            //        ModifiedBy = null,
            //        PartnerIdFinal = budget.PartnerId,
            //        PartnerIdInitial = budget.PartnerId,
            //        ProjectIdFinal = budget.ProjectId,
            //        ProjectIdInitial = budget.ProjectId,
            //        QuantityIni = budget.Quantity,
            //        QuantityFin = budget.Quantity,
            //        SubTypeIdFinal = budget.SubTypeId,
            //        SubTypeIdInitial = budget.SubTypeId,
            //        Validated = budget.Validated,
            //        ValueFin1 = budget.ValueFin,
            //        ValueFin2 = budget.ValueFin,
            //        ValueIni1 = budget.ValueIni,
            //        ValueIni2 = budget.ValueIni,
            //        Guid = employee[0].Guid,
                   
                     
            //    };

            //    _context.Add(budgetOp);

            //    _context.SaveChanges();
            //}

            //await SendValidatedEmail(budget.Id);

            return Ok(StatusCode(200));
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("total/{accMonthId}")]
        public virtual IActionResult GetPieChartDetails(int accMonthId)
        {
            List<Model.BudgetTotalProcentage> items = _context.Set<Model.BudgetTotalProcentage>().FromSql("OfferTotal {0}", accMonthId).ToList();

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
            List<Model.BudgetCompanyProcentage> items = _context.Set<Model.BudgetCompanyProcentage>().FromSql("OfferReportByCompany {0}, {1}", accMonthId > 0 ? accMonthId : 29, companyId).ToList();

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
        [Route("partner/{accMonthId}/{partnerId}")]
        public virtual IActionResult GetProjectDetails(int accMonthId, int partnerId)
        {
            List<Model.BudgetProjectProcentage> items = _context.Set<Model.BudgetProjectProcentage>().FromSql("OfferReportByPartner {0}, {1}", accMonthId > 0 ? accMonthId : 29, partnerId).ToList();

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
        [Route("masterType/{accMonthId}/{masterTypeId}")]
        public virtual IActionResult GetCostCenterDetails(int accMonthId, int masterTypeId)
        {
            List<Model.BudgetCostCenterProcentage> items = _context.Set<Model.BudgetCostCenterProcentage>().FromSql("OfferReportByMasterType {0}, {1}", accMonthId > 0 ? accMonthId : 29, masterTypeId).ToList();

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
            List<Model.BudgetExpenceTypeProcentage> items = _context.Set<Model.BudgetExpenceTypeProcentage>().FromSql("OfferReportByExpenceType {0}, {1}", accMonthId > 0 ? accMonthId : 29, typeId).ToList();

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
            List<Model.BudgetEmployeeProcentage> items = _context.Set<Model.BudgetEmployeeProcentage>().FromSql("OfferReportByEmployee {0}, {1}", accMonthId > 0 ? accMonthId : 29, employeeId).ToList();

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
            List<Model.BudgetSubTypeProcentage> items = _context.Set<Model.BudgetSubTypeProcentage>().FromSql("OfferReportBySubType {0}, {1}", accMonthId > 0 ? accMonthId : 29, subTypeId).ToList();

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

		[HttpGet("export")]
		public async Task<IActionResult> Export(string includes, string jsonFilter, string filter)
		{
			AssetDepTotal depTotal = null;
			AssetCategoryTotal catTotal = null;
			OfferFilter offerFilter = null;
			string employeeId = string.Empty;
			string role = string.Empty;

			includes ??= "Offer.Request,Offer.Employee,Offer.Company,Offer.AssetType,Offer.Partner,Offer.AppState,";

			offerFilter = jsonFilter != null ? JsonConvert.DeserializeObject<OfferFilter>(jsonFilter) : new OfferFilter();

			if (filter != null)
			{
				offerFilter.Filter = filter;
			}

			if (HttpContext.User.Identity.Name != null)
			{
				var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

				if (user == null)
				{
					user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				}
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				offerFilter.Role = role;
				offerFilter.InInventory = user.InInventory;
				offerFilter.UserId = user.Id;

				if (employeeId != null)
				{
					offerFilter.EmployeeId = int.Parse(employeeId);
				}
				else
				{
					offerFilter.EmployeeIds = null;
					offerFilter.EmployeeIds = new List<int?>();
					offerFilter.EmployeeIds.Add(int.Parse("-1"));
				}
			}
			else
			{
				offerFilter.EmployeeIds = null;
				offerFilter.EmployeeIds = new List<int?>();
				offerFilter.EmployeeIds.Add(int.Parse("-1"));
			}


			


			using (ExcelPackage package = new ExcelPackage())
			{
				var items = (_itemsRepository as IOffersRepository)
				.GetOffer(offerFilter, includes, null, null, out depTotal, out catTotal).ToList();
				var itemsResource = _mapper.Map<List<Model.OfferDetail>, List<Dto.Offer>>(items);

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Oferte");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod Oferta";
				worksheet.Cells[1, 2].Value = "Status";
				worksheet.Cells[1, 3].Value = "Date creare";
				worksheet.Cells[1, 4].Value = "Data modificare";
				worksheet.Cells[1, 5].Value = "Owner";
				worksheet.Cells[1, 6].Value = "Cod P.R.";
				worksheet.Cells[1, 7].Value = "Companie";
				worksheet.Cells[1, 8].Value = "Supplier";
				worksheet.Cells[1, 9].Value = "Tip";
				worksheet.Cells[1, 10].Value = "Info";

				int recordIndex = 2;
				foreach (var item in itemsResource)
				{
					worksheet.Cells[recordIndex, 1].Value = item.Code;
					worksheet.Cells[recordIndex, 2].Value = item.AppState != null ? item.AppState.Name : "";
                    worksheet.Cells[recordIndex, 3].Value = item.CreatedAt;
                    worksheet.Cells[recordIndex, 3].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 4].Value = item.ModifiedAt;
                    worksheet.Cells[recordIndex, 4].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 5].Value = item.Employee != null ? item.Employee.Email : "";
					worksheet.Cells[recordIndex, 6].Value = item.Request != null ? item.Request.Code : "";
					worksheet.Cells[recordIndex, 7].Value = item.Company != null ? item.Company.Code : "";
					worksheet.Cells[recordIndex, 8].Value = item.Partner != null ? item.Partner.Name : "";
					worksheet.Cells[recordIndex, 9].Value = item.AssetType != null ? item.AssetType.Name : "";
					worksheet.Cells[recordIndex, 10].Value = item.Info;

					recordIndex++;
				}

				worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();
				worksheet.Column(3).AutoFit();
				worksheet.Column(4).AutoFit();
				worksheet.Column(5).AutoFit();
				worksheet.Column(6).AutoFit();
				worksheet.Column(7).AutoFit();
				worksheet.Column(8).AutoFit();
				worksheet.Column(9).AutoFit();

				using (var cells = worksheet.Cells[1, 1, 1, 10])
				{
					cells.Style.Font.Bold = true;
					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
					cells.Style.Fill.BackgroundColor.SetColor(Color.Aqua);
				}

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				//HttpContext.Response.ContentType = entityFile.FileType;
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "Oferte.xlsx"
				};

				return result;

			}
		}


		[HttpGet("getEmails")]
        public async Task<List<EmailReadResponse>> GetEmails()
        {

            var emails = await _emailSender.ReadEmails(10);

            return emails;
            
        }


        private byte[] GetByteArrayFromImage(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }
    }
}
