using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.Style;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Dto.Common;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/emailmanagers")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class EmailManagersController : GenericApiController<Model.EmailManager, Dto.EmailManager>
    {
		private readonly UserManager<Model.ApplicationUser> _userManager;

		public EmailManagersController(ApplicationDbContext context, IEmailManagersRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
			this._userManager = userManager;
		}

        [HttpGet]
        [Route("", Order = -1)]
        public async virtual Task<IActionResult> Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string emailTypeIds, string appStateIds, string assetCategoryIds, string includes, string type)
        {
            List<Model.EmailManager> items = null;
            IEnumerable<Dto.EmailManager> itemsResult = null;
            List<int?> dIds = null;
            List<int?> aIds = null;
            List<int?> appIds = null;
            List<int> divisionIds = null;
            string employeeId = string.Empty;
            string role = string.Empty;
            bool inInventory = false;

            var userName = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(userName);
            }

            includes = "EmailType,EmployeeInitial,EmployeeFinal,RoomInitial,RoomFinal,Asset,AssetComponent,AppState,SubType,Offer.Request,Order,Budget,Partner";

            //if (!user.InInventory)
            //{
            //    divisionIds = await _context.Set<Model.EmployeeDivision>()
            //        .Where(a => a.EmployeeId == user.EmployeeId)
            //        .Select(a => a.DivisionId)
            //        .ToListAsync();
            //}
            //else
            //{
            //    divisionIds = await _context.Set<Model.EmployeeCostCenter>().Where(a => a.EmployeeId == user.EmployeeId).Select(a => a.CostCenter != null && a.CostCenter.DivisionId != null ? a.CostCenter.DivisionId.Value : 0).ToListAsync();
            //}

            role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
            employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;
            inInventory = user.InInventory;

            if ((emailTypeIds != null) && (emailTypeIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(emailTypeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((appStateIds != null) && (appStateIds.Length > 0)) appIds = JsonConvert.DeserializeObject<string[]>(appStateIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((assetCategoryIds != null) && (assetCategoryIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(assetCategoryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IEmailManagersRepository).GetByFilters(employeeId, role, inInventory, filter, includes, sortColumn, sortDirection, page, pageSize, dIds, appIds, aIds, divisionIds, type).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.EmailManager>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IEmailManagersRepository).GetCountByFilters(employeeId, role, inInventory, filter, dIds, appIds, aIds, divisionIds, type);
                var pagedResult = new Dto.PagedResult<Dto.EmailManager>(itemsResult, new Dto.PagingInfo()
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

        [Route("notvalidate/{guid}")]
        public virtual IActionResult NotValidate(Guid guid)
        {
            return Redirect("https://service.inventare.ro/BOSGDEMO/#/notvalidate/" + guid.ToString());
        }

        [HttpPost("declined")]
        public async virtual Task<IActionResult> Success([FromBody] EmailManagerReason reason)
        {

            bool success = false;
            Guid guid = reason.Guid;

            if (guid != Guid.Empty)
            {
                Model.EmailManager emailManager = _context.Set<Model.EmailManager>().Include(e => e.EmailType).Where(a => a.Guid == guid).Single();

                if (emailManager != null && emailManager.EmailType.Code == "VALIDATE ASSET")
                {
                    Model.Asset asset = _context.Set<Model.Asset>().Where(a => a.Id == emailManager.AssetId).SingleOrDefault();

                    emailManager.IsAccepted = false;
                    emailManager.AppStateId = 8;
                    emailManager.Info = reason.Reason;
                    asset.IsAccepted = false;
                    _context.Update(asset);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
                else if (emailManager != null && emailManager.EmailType.Code == "VALIDATE COMPONENT")
                {
                    Model.AssetComponent assetComponent = _context.Set<Model.AssetComponent>().Where(a => a.Id == emailManager.AssetComponentId).SingleOrDefault();

                    emailManager.IsAccepted = false;
                    emailManager.AppStateId = 8;
                    emailManager.Info = reason.Reason;
                    assetComponent.IsAccepted = false;
                    _context.Update(assetComponent);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
                else if (emailManager != null && emailManager.EmailType.Code == "VALIDATE ASSET INTERVAL")
                {
                    Model.Asset asset = _context.Set<Model.Asset>().Where(a => a.Id == emailManager.AssetId).SingleOrDefault();

                    emailManager.IsAccepted = false;
                    emailManager.AppStateId = 8;
                    emailManager.Info = reason.Reason;
                    asset.IsAccepted = false;
                    _context.Update(asset);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
                else if (emailManager != null && emailManager.EmailType.Code == "VALIDATE COMPONENT INTERVAL")
                {
                    Model.AssetComponent assetComponent = _context.Set<Model.AssetComponent>().Where(a => a.Id == emailManager.AssetComponentId).SingleOrDefault();

                    emailManager.IsAccepted = false;
                    emailManager.AppStateId = 8;
                    emailManager.Info = reason.Reason;
                    assetComponent.IsAccepted = false;
                    _context.Update(assetComponent);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
            }

            if (success)
            {
                return Redirect("https://service.inventare.ro/BOSGDEMO/#/success");
            }
            else
            {
                return Redirect("https://service.inventare.ro/BOSGDEMO/#/error");
            }



        }


        [Route("validate/{guid}")]
        public async virtual Task<IActionResult> Validate(Guid guid)
        {

            bool success = false;

            if (guid != Guid.Empty)
            {
                Model.EmailManager emailManager = _context.Set<Model.EmailManager>().Include(e => e.EmailType).Where(a => a.Guid == guid).Single();

                if (emailManager != null && emailManager.EmailType.Code == "VALIDATE ASSET")
                {
                    Model.Asset asset = _context.Set<Model.Asset>().Where(a => a.Id == emailManager.AssetId).SingleOrDefault();

                    emailManager.IsAccepted = true;
                    emailManager.AppStateId = 7;
                    asset.IsAccepted = true;
                    _context.Update(asset);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
                else if (emailManager != null && emailManager.EmailType.Code == "VALIDATE COMPONENT")
                {
                    Model.AssetComponent assetComponent = _context.Set<Model.AssetComponent>().Where(a => a.Id == emailManager.AssetComponentId).SingleOrDefault();

                    emailManager.IsAccepted = true;
                    emailManager.AppStateId = 7;
                    assetComponent.IsAccepted = true;
                    _context.Update(assetComponent);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
                else if (emailManager != null && emailManager.EmailType.Code == "VALIDATE ASSET INTERVAL")
                {
                    Model.Asset asset = _context.Set<Model.Asset>().Where(a => a.Id == emailManager.AssetId).SingleOrDefault();
                    Model.Employee employee = _context.Set<Model.Employee>().Where(a => a.Id == emailManager.EmployeeIdInitial).SingleOrDefault();

                    employee.IsConfirmed = true;
                    emailManager.IsAccepted = true;
                    emailManager.AppStateId = 7;
                    asset.IsAccepted = true;
                    _context.Update(employee);
                    _context.Update(asset);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
                else if (emailManager != null && emailManager.EmailType.Code == "VALIDATE COMPONENT INTERVAL")
                {
                    Model.AssetComponent assetComponent = _context.Set<Model.AssetComponent>().Where(a => a.Id == emailManager.AssetComponentId).SingleOrDefault();
                    Model.Employee employee = _context.Set<Model.Employee>().Where(a => a.Id == emailManager.EmployeeIdInitial).SingleOrDefault();

                    employee.IsConfirmed = true;
                    emailManager.IsAccepted = true;
                    emailManager.AppStateId = 7;
                    assetComponent.IsAccepted = true;
                    _context.Update(employee);
                    _context.Update(assetComponent);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
            }

            if (success)
            {
                return Redirect("https://service.inventare.ro/BOSGDEMO/#/validate");
            }
            else
            {
                return Redirect("https://service.inventare.ro/BOSGDEMO/#/error");
            }



        }

        [Route("validate/all/{guid}")]
        public async virtual Task<IActionResult> ValidateAll(Guid guid)
        {

            bool success = false;

            if (guid != Guid.Empty)
            {
                List<Model.EmailManager> emailManager = _context.Set<Model.EmailManager>().Include(e => e.EmailType).Where(a => a.GuidAll == guid).ToList();

                for (int i = 0; i < emailManager.Count; i++)
                {
                    if (emailManager != null && emailManager[i].EmailType.Code == "VALIDATE ASSET")
                    {
                        Model.Asset asset = _context.Set<Model.Asset>().Where(a => a.Id == emailManager[i].AssetId).SingleOrDefault();

                        emailManager[i].IsAccepted = true;
                        emailManager[i].AppStateId = 7;
                        asset.IsAccepted = true;
                        _context.Update(asset);
                        _context.Update(emailManager[i]);
                        _context.SaveChanges();
                        success = true;
                    }
                    else if (emailManager != null && emailManager[i].EmailType.Code == "VALIDATE COMPONENT")
                    {
                        Model.AssetComponent assetComponent = _context.Set<Model.AssetComponent>().Where(a => a.Id == emailManager[i].AssetComponentId).SingleOrDefault();

                        emailManager[i].IsAccepted = true;
                        emailManager[i].AppStateId = 7;
                        assetComponent.IsAccepted = true;
                        _context.Update(assetComponent);
                        _context.Update(emailManager[i]);
                        _context.SaveChanges();
                        success = true;
                    }
                    else if (emailManager != null && emailManager[i].EmailType.Code == "VALIDATE ASSET INTERVAL")
                    {
                        Model.Asset asset = _context.Set<Model.Asset>().Where(a => a.Id == emailManager[i].AssetId).SingleOrDefault();
                        Model.Employee employee = _context.Set<Model.Employee>().Where(a => a.Id == emailManager[i].EmployeeIdInitial).SingleOrDefault();

                        employee.IsConfirmed = true;
                        emailManager[i].IsAccepted = true;
                        emailManager[i].AppStateId = 7;
                        asset.IsAccepted = true;
                        _context.Update(employee);
                        _context.Update(asset);
                        _context.Update(emailManager[i]);
                        _context.SaveChanges();
                        success = true;
                    }
                    else if (emailManager != null && emailManager[i].EmailType.Code == "VALIDATE COMPONENT INTERVAL")
                    {
                        Model.AssetComponent assetComponent = _context.Set<Model.AssetComponent>().Where(a => a.Id == emailManager[i].AssetComponentId).SingleOrDefault();
                        Model.Employee employee = _context.Set<Model.Employee>().Where(a => a.Id == emailManager[i].EmployeeIdInitial).SingleOrDefault();

                        employee.IsConfirmed = true;
                        emailManager[i].IsAccepted = true;
                        emailManager[i].AppStateId = 7;
                        assetComponent.IsAccepted = true;
                        _context.Update(employee);
                        _context.Update(assetComponent);
                        _context.Update(emailManager[i]);
                        _context.SaveChanges();
                        success = true;
                    }
                }

         
            }

            if (success)
            {
                return Redirect("https://service.inventare.ro/BOSGDEMO/#/validate");
            }
            else
            {
                return Redirect("https://service.inventare.ro/BOSGDEMO/#/error");
            }



        }


        [Route("accept")]
        public async virtual Task<bool> ValidateOffer([FromBody] OfferUpdate offerUpdate)
        {
            Model.EmailManager emailManager = null;
            List<Model.OfferMaterial> offerMaterials = null;
            bool success = false;
            Guid guid = Guid.NewGuid();

            if (offerUpdate.Id > 0)
            {
                emailManager = await _context.Set<Model.EmailManager>().Include(e => e.EmailType).Where(a => a.Id == offerUpdate.Id).SingleAsync();

                if (emailManager != null && emailManager.EmailType.Code == "NEW_OFFER" || emailManager.EmailType.Code == "VALIDATED_OFFER")
                {
                    Model.Offer offer = await _context.Set<Model.Offer>().Where(a => a.Id == emailManager.OfferId).SingleAsync();

                    offerMaterials = await _context.Set<Model.OfferMaterial>().Where(a => a.OfferId == emailManager.OfferId && a.EmailManagerId == emailManager.Id && a.IsDeleted == false).ToListAsync();

                    Model.DocumentType documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "VALIDATE_OFFER").SingleOrDefault();

                    Model.Document document = new Model.Document()
                    {
                        Approved = true,
                        CompanyId = offer.CompanyId,
                        CostCenterId = offer.CostCenterId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = offer.UserId,
                        CreationDate = DateTime.Now,
                        Details = offer.Info != null ? offer.Info : string.Empty,
                        DocNo1 = offer.Info != null ? offer.Info : string.Empty,
                        DocNo2 = offer.Info != null ? offer.Info : string.Empty,
                        DocumentDate = DateTime.Now,
                        DocumentTypeId = documentType.Id,
                        Exported = true,
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = offer.UserId,
                        ParentDocumentId = null,
                        PartnerId = emailManager.PartnerId,
                        RegisterDate = DateTime.Now,
                        ValidationDate = DateTime.Now
                    };

                    _context.Add(document);


                    Model.OfferOp budgetOp = new Model.OfferOp()
                    {
                        AccMonthId = offer.AccMonthId,
                        AccSystemId = null,
                        AccountIdInitial = offer.AccountId,
                        AccountIdFinal = offer.AccountId,
                        AdministrationIdInitial = offer.AdministrationId,
                        AdministrationIdFinal = offer.AdministrationId,
                        Offer = offer,
                        BudgetManagerIdInitial = null,
                        BudgetManagerIdFinal = null,
                        BudgetStateId = 7,
                        CompanyIdInitial = offer.CompanyId,
                        CompanyIdFinal = offer.CompanyId,
                        CostCenterIdInitial = offer.CostCenterId,
                        CostCenterIdFinal = offer.CostCenterId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = offer.UserId,
                        Document = document,
                        DstConfAt = DateTime.Now,
                        DstConfBy = offer.UserId,
                        EmployeeIdInitial = offer.EmployeeId,
                        EmployeeIdFinal = offer.EmployeeId,
                        InfoIni = offer.Info,
                        InfoFin = offer.Info,
                        IsAccepted = false,
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = offer.UserId,
                        PartnerIdInitial = offer.PartnerId,
                        PartnerIdFinal = emailManager.PartnerId,
                        ProjectIdInitial = offer.ProjectId,
                        ProjectIdFinal = offer.ProjectId,
                        QuantityIni = offer.Quantity,
                        QuantityFin = offer.Quantity,
                        SubTypeIdInitial = offer.SubTypeId,
                        SubTypeIdFinal = offer.SubTypeId,
                        Validated = true,
                        ValueFin1 = offer.ValueFin,
                        ValueIni1 = offer.ValueIni,
                        ValueFin2 = offer.ValueFin,
                        ValueIni2 = offer.ValueIni,
                        Guid = Guid.NewGuid()
                    };

                    _context.Add(budgetOp);

                    emailManager.IsAccepted = true;
                    emailManager.AppStateId = 7;
                    emailManager.Info = offerUpdate.Reason;
                    offer.IsAccepted = true;
                    offer.AppStateId = 7;
                    offer.PartnerId = emailManager.PartnerId;
                    offer.Guid = guid;

					for (int i = 0; i < offerMaterials.Count; i++)
					{
                        offerMaterials[i].AppStateId = 7;
                        offerMaterials[i].Validated = true;
                        offerMaterials[i].Guid = guid;

                        _context.Update(offerMaterials[i]);

                    }

                    
                    _context.Update(offer);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
                else if (emailManager != null && emailManager.EmailType.Code == "VALIDATE COMPONENT")
                {
                    Model.AssetComponent assetComponent = _context.Set<Model.AssetComponent>().Where(a => a.Id == emailManager.AssetComponentId).SingleOrDefault();

                    emailManager.IsAccepted = true;
                    emailManager.AppStateId = 7;
                    assetComponent.IsAccepted = true;
                    _context.Update(assetComponent);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
                else if (emailManager != null && emailManager.EmailType.Code == "VALIDATE ASSET INTERVAL")
                {
                    Model.Asset asset = _context.Set<Model.Asset>().Where(a => a.Id == emailManager.AssetId).SingleOrDefault();
                    Model.Employee employee = _context.Set<Model.Employee>().Where(a => a.Id == emailManager.EmployeeIdInitial).SingleOrDefault();

                    employee.IsConfirmed = true;
                    emailManager.IsAccepted = true;
                    emailManager.AppStateId = 7;
                    asset.IsAccepted = true;
                    _context.Update(employee);
                    _context.Update(asset);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
                else if (emailManager != null && emailManager.EmailType.Code == "VALIDATE COMPONENT INTERVAL")
                {
                    Model.AssetComponent assetComponent = _context.Set<Model.AssetComponent>().Where(a => a.Id == emailManager.AssetComponentId).SingleOrDefault();
                    Model.Employee employee = _context.Set<Model.Employee>().Where(a => a.Id == emailManager.EmployeeIdInitial).SingleOrDefault();

                    employee.IsConfirmed = true;
                    emailManager.IsAccepted = true;
                    emailManager.AppStateId = 7;
                    assetComponent.IsAccepted = true;
                    _context.Update(employee);
                    _context.Update(assetComponent);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
            }

            if (success)
            {
                List<Model.EmailManager> emailManagers = _context.Set<Model.EmailManager>().Where(e => e.OfferId == emailManager.OfferId && e.AppStateId == 6).ToList();

				for (int i = 0; i < emailManagers.Count; i++)
				{
					emailManagers[i].AppStateId = 8;
					emailManagers[i].IsDeleted = true;
					_context.Update(emailManager);
					_context.SaveChanges();
				}

                return true;
            }
            else
            {
                return false;
            }



        }

        [Route("decline")]
        public async virtual Task<bool> DeclineOffer([FromBody] OfferUpdate offerUpdate)
        {
            Model.EmailManager emailManager = null;
            bool success = false;

            if (offerUpdate.Id > 0)
            {
                emailManager = await _context.Set<Model.EmailManager>().Include(e => e.EmailType).Where(a => a.Id == offerUpdate.Id).SingleAsync();

                if (emailManager != null && emailManager.EmailType.Code == "NEW_OFFER" || emailManager.EmailType.Code == "VALIDATED_OFFER")
                {
                    Model.Offer offer = await _context.Set<Model.Offer>().Where(a => a.Id == emailManager.OfferId).SingleAsync();

                    Model.DocumentType documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "VALIDATE_OFFER").SingleOrDefault();

                    Model.Document document = new Model.Document()
                    {
                        Approved = true,
                        CompanyId = offer.CompanyId,
                        CostCenterId = offer.CostCenterId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = offer.UserId,
                        CreationDate = DateTime.Now,
                        Details = offer.Info != null ? offer.Info : string.Empty,
                        DocNo1 = offer.Info != null ? offer.Info : string.Empty,
                        DocNo2 = offer.Info != null ? offer.Info : string.Empty,
                        DocumentDate = DateTime.Now,
                        DocumentTypeId = documentType.Id,
                        Exported = true,
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = offer.UserId,
                        ParentDocumentId = null,
                        PartnerId = emailManager.PartnerId,
                        RegisterDate = DateTime.Now,
                        ValidationDate = DateTime.Now
                    };

                    _context.Add(document);


                    Model.OfferOp budgetOp = new Model.OfferOp()
                    {
                        AccMonthId = offer.AccMonthId,
                        AccSystemId = null,
                        AccountIdInitial = offer.AccountId,
                        AccountIdFinal = offer.AccountId,
                        AdministrationIdInitial = offer.AdministrationId,
                        AdministrationIdFinal = offer.AdministrationId,
                        Offer = offer,
                        BudgetManagerIdInitial = null,
                        BudgetManagerIdFinal = null,
                        BudgetStateId = 7,
                        CompanyIdInitial = offer.CompanyId,
                        CompanyIdFinal = offer.CompanyId,
                        CostCenterIdInitial = offer.CostCenterId,
                        CostCenterIdFinal = offer.CostCenterId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = offer.UserId,
                        Document = document,
                        DstConfAt = DateTime.Now,
                        DstConfBy = offer.UserId,
                        EmployeeIdInitial = offer.EmployeeId,
                        EmployeeIdFinal = offer.EmployeeId,
                        InfoIni = offer.Info,
                        InfoFin = offer.Info,
                        IsAccepted = false,
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = offer.UserId,
                        PartnerIdInitial = offer.PartnerId,
                        PartnerIdFinal = emailManager.PartnerId,
                        ProjectIdInitial = offer.ProjectId,
                        ProjectIdFinal = offer.ProjectId,
                        QuantityIni = offer.Quantity,
                        QuantityFin = offer.Quantity,
                        SubTypeIdInitial = offer.SubTypeId,
                        SubTypeIdFinal = offer.SubTypeId,
                        Validated = true,
                        ValueFin1 = offer.ValueFin,
                        ValueIni1 = offer.ValueIni,
                        ValueFin2 = offer.ValueFin,
                        ValueIni2 = offer.ValueIni,
                        Guid = Guid.NewGuid()
                    };

                    _context.Add(budgetOp);

                    emailManager.IsAccepted = true;
                    emailManager.AppStateId = 8;
                    emailManager.Info = offerUpdate.Reason;
                    //offer.IsAccepted = true;
                    //offer.AppStateId = 7;
                    //offer.PartnerId = emailManager.PartnerId;
                    //_context.Update(offer);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
                else if (emailManager != null && emailManager.EmailType.Code == "VALIDATE COMPONENT")
                {
                    Model.AssetComponent assetComponent = _context.Set<Model.AssetComponent>().Where(a => a.Id == emailManager.AssetComponentId).SingleOrDefault();

                    emailManager.IsAccepted = true;
                    emailManager.AppStateId = 7;
                    assetComponent.IsAccepted = true;
                    _context.Update(assetComponent);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
                else if (emailManager != null && emailManager.EmailType.Code == "VALIDATE ASSET INTERVAL")
                {
                    Model.Asset asset = _context.Set<Model.Asset>().Where(a => a.Id == emailManager.AssetId).SingleOrDefault();
                    Model.Employee employee = _context.Set<Model.Employee>().Where(a => a.Id == emailManager.EmployeeIdInitial).SingleOrDefault();

                    employee.IsConfirmed = true;
                    emailManager.IsAccepted = true;
                    emailManager.AppStateId = 7;
                    asset.IsAccepted = true;
                    _context.Update(employee);
                    _context.Update(asset);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
                else if (emailManager != null && emailManager.EmailType.Code == "VALIDATE COMPONENT INTERVAL")
                {
                    Model.AssetComponent assetComponent = _context.Set<Model.AssetComponent>().Where(a => a.Id == emailManager.AssetComponentId).SingleOrDefault();
                    Model.Employee employee = _context.Set<Model.Employee>().Where(a => a.Id == emailManager.EmployeeIdInitial).SingleOrDefault();

                    employee.IsConfirmed = true;
                    emailManager.IsAccepted = true;
                    emailManager.AppStateId = 7;
                    assetComponent.IsAccepted = true;
                    _context.Update(employee);
                    _context.Update(assetComponent);
                    _context.Update(emailManager);
                    _context.SaveChanges();
                    success = true;
                }
            }

            if (success)
            {
    //            List<Model.EmailManager> emailManagers = _context.Set<Model.EmailManager>().Where(e => e.OfferId == emailManager.OfferId && e.AppStateId == 6).ToList();

    //            for (int i = 0; i < emailManagers.Count; i++)
    //            {
				//	emailManagers[i].AppStateId = 8;
				//	emailManagers[i].IsDeleted = true;
				//	_context.Update(emailManager);
				//	_context.SaveChanges();

				//}

                return true;
            }
            else
            {
                return false;
            }



        }

        [HttpPost("offerMaterialUpdate")]
        public async virtual Task<OfferResult> OfferMaterialUpdate([FromBody] List<OfferMaterialUpdate> offerMaterialToUpdates)
        {
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
			List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.OfferMaterial offerMaterial = null;
			//List<Model.RequestBFMaterialCostCenter> requestBFMaterialCostCenters = null;
			Model.Offer offer = null;
            Model.Order order = null;
            Model.Rate rate = null;
            int offerId = 0;
            decimal rateValue = 0;
			int uomId = 0;
            string rateDate = null;

			for (int i = 0; i < offerMaterialToUpdates.Count; i++)
			{
				offerMaterial = await _context.Set<Model.OfferMaterial>()
					.Include(o => o.Offer)
					//.Include(a => a.Rate).ThenInclude(u => u.Uom)
					.Where(a => a.Id == offerMaterialToUpdates[i].Id)
					.FirstOrDefaultAsync();
                if (offerMaterial == null) return new OfferResult { Success = false, Message= "Nu a fost selectata nicio oferta!", OfferId = 0 };

				offerId = offerMaterial.OfferId;

				offer = await _context.Set<Model.Offer>().Include(f => f.OfferType).Include(r => r.Rate).Where(o => o.Id == offerId).SingleAsync();

				rateValue = offerMaterialToUpdates[i].RateValue;

				if (offerMaterialToUpdates[i].RateDate == null) return new OfferResult { Success = false, Message = "Nu a fost selectata o data CURS!", OfferId = 0 };

				rateDate = offerMaterialToUpdates[i].RateDate.Value.ToString("yyyy-MM-dd");
				uomId = offerMaterialToUpdates[i].UomId;

				rate = await _context.Set<Model.Rate>()
                    .Include(u => u.Uom)
                    .AsNoTracking()
                    .Where(i => i.Value == rateValue && i.UomId == uomId && i.Code == rateDate && i.IsDeleted == false)
                    .FirstOrDefaultAsync();

				if (rate == null)
				{
					rate = new Model.Rate()
					{
						AccMonthId = 50,
						Code = rateDate,
						CompanyId = null,
						CreatedAt = DateTime.Now,
						CreatedBy = _context.UserId,
						ModifiedAt = DateTime.Now,
						ModifiedBy = _context.UserId,
						Name = rateDate,
						UomId = offerMaterialToUpdates[i].UomId,
						Value = offerMaterialToUpdates[i].RateValue,
						IsLast = false,
						Multiplier = 1
					};

					_context.Add(rate);

                    offerMaterial.Rate = rate;
                    offer.Rate = rate;

                    _context.Update(offerMaterial);
					_context.Update(offer);
					_context.SaveChanges();
                }
                else
                {
					offerMaterial.RateId = rate.Id;
					offer.RateId = rate.Id;

					_context.Update(offerMaterial);
					_context.Update(offer);
					_context.SaveChanges();
				}


				
				//rateValue = offerMaterial.Rate.Value;

				offerMaterial.Price = offerMaterialToUpdates[i].Price;
				offerMaterial.PriceRon = offerMaterialToUpdates[i].Price * rateValue;
				offerMaterial.PriceIni = offerMaterialToUpdates[i].Price;
				offerMaterial.PriceIniRon = offerMaterialToUpdates[i].Price * rateValue;
				offerMaterial.Value = offerMaterialToUpdates[i].Price * offerMaterialToUpdates[i].Quantity;
				offerMaterial.ValueRon = offerMaterialToUpdates[i].Price * offerMaterialToUpdates[i].Quantity * rateValue;
				offerMaterial.QuantityIni = offerMaterialToUpdates[i].Quantity;
				offerMaterial.Quantity = offerMaterialToUpdates[i].Quantity;
				offerMaterial.ValueIni = offerMaterialToUpdates[i].Price * offerMaterialToUpdates[i].Quantity;
				offerMaterial.ValueIniRon = offerMaterialToUpdates[i].Price * offerMaterialToUpdates[i].Quantity * rateValue;
				offerMaterial.RequestId = offerMaterial.Offer.RequestId;
				offerMaterial.WIP = offerMaterialToUpdates[i].WIP;

				order = await _context.Set<Model.Order>().Include(o => o.Offer).Where(a => a.OfferId == offerId).FirstOrDefaultAsync();

				if (order != null)
				{
					offerMaterial.OrdersPrice = offerMaterialToUpdates[i].Price;
					offerMaterial.OrdersPriceRon = offerMaterialToUpdates[i].Price * rateValue;
					offerMaterial.OrdersQuantity = offerMaterialToUpdates[i].Quantity;
					offerMaterial.OrdersValue = offerMaterialToUpdates[i].Price * offerMaterialToUpdates[i].Quantity;
					offerMaterial.OrdersValueRon = offerMaterialToUpdates[i].Price * offerMaterialToUpdates[i].Quantity * rateValue;

					_context.Update(order);

				}

				_context.Update(offerMaterial);
				_context.SaveChanges();
			}

			if (offerId > 0)
			{
                decimal sumQuantity = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.Quantity);
                decimal sumQuantityIni = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.QuantityIni);
                decimal sumPrice = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.Price);
                decimal sumPriceIni = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false).Sum(a => a.PriceIni);
                decimal sumValue = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.Value);
                decimal sumValueIni = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.ValueIni);

                offer = await _context.Set<Model.Offer>().Include(f => f.OfferType).Include(r => r.Rate).Where(o => o.Id == offerId).SingleAsync();

                offer.QuantityRem = (float)sumQuantity;
                offer.Quantity = (float)sumQuantity;
                offer.Price = offerMaterial.Price;
                offer.PriceRon = offerMaterial.PriceRon;
                offer.ValueFin = sumValue;
                offer.ValueFinRon = sumValue * offer.Rate.Value;
                offer.ValueIni = sumValueIni;
                offer.ValueIniRon = sumValueIni * offer.Rate.Value;

                _context.Update(offer);
                _context.SaveChanges();

				requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>()
                    .Include(o => o.OfferType)
                    .Where(r => r.RequestId == offer.RequestId && r.IsDeleted == false)
                    .ToListAsync();

				for (int i = 0; i < requestBudgetForecasts.Count; i++)
				{
					requestBudgetForecasts[i].MaxQuantity = (int)sumQuantity;

     //               if(offer.OfferType.Code == "O-V")
					//{
     //                   requestBudgetForecasts[i].MaxQuantity = 1;
     //               }
					requestBudgetForecasts[i].MaxValue = sumValue;
					requestBudgetForecasts[i].MaxValueRon = sumValue * offer.Rate.Value;
                    requestBudgetForecasts[i].OfferTypeId = offer.OfferTypeId;
                    requestBudgetForecasts[i].Price = offerMaterial.Price;
                    requestBudgetForecasts[i].PriceRon = offerMaterial.PriceRon;

                    _context.Update(requestBudgetForecasts[i]);
					_context.SaveChanges();
				}
			}

            order = await _context.Set<Model.Order>().Include(o => o.Offer).Where(a => a.OfferId == offerId).FirstOrDefaultAsync();

            if(order != null)
            {
                order.Price = offer.Price;
				order.PriceRon = offer.PriceRon;

                order.ValueFin = offer.ValueFin;
				order.ValueFinRon = offer.ValueFinRon;

				order.Quantity = offer.Quantity;
				order.QuantityRem = offer.QuantityRem;

				order.ValueIni = offer.ValueIni;
				order.ValueIniRon = offer.ValueIniRon;

				decimal sumQuantity = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.Quantity);
				decimal sumQuantityIni = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.QuantityIni);
				decimal sumPrice = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.Price);
				decimal sumPriceIni = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false).Sum(a => a.PriceIni);
				decimal sumValue = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.Value);
				decimal sumValueRon = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.ValueRon);
				decimal sumValueIni = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.ValueIni);

				requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>()
					.Include(o => o.Contract).ThenInclude(c => c.ContractAmount)
					.Include(o => o.BudgetForecast)
					.Include(o => o.OfferType)
					.Where(r => r.RequestId == offer.RequestId && r.IsDeleted == false)
					.ToListAsync();

				for (int i = 0; i < requestBudgetForecasts.Count; i++)
				{
					requestBudgetForecasts[i].MaxQuantity = (int)sumQuantity;
					requestBudgetForecasts[i].Quantity = (int)offer.Quantity;


					requestBudgetForecasts[i].MaxValue = sumValue;
					requestBudgetForecasts[i].MaxValueRon = sumValue * offer.Rate.Value;
					requestBudgetForecasts[i].OfferTypeId = offer.OfferTypeId;

					requestBudgetForecasts[i].Value = offer.Price * (decimal)offer.Quantity;
					requestBudgetForecasts[i].ValueRon = offer.PriceRon * (decimal)offer.Quantity;
					requestBudgetForecasts[i].TotalOrderValue = offer.Price * (decimal)offer.Quantity;
					requestBudgetForecasts[i].TotalOrderValueRon = offer.PriceRon * (decimal)offer.Quantity;
					requestBudgetForecasts[i].TotalOrderQuantity = (int)offer.Quantity;
					
					requestBudgetForecasts[i].Price = offerMaterial.Price;
					requestBudgetForecasts[i].PriceRon = offerMaterial.PriceRon;

					//if (offer.OfferType.Code == "O-V")
					//{
					//	requestBudgetForecasts[i].MaxQuantity = 1;
					//}


					if (requestBudgetForecasts[i].BudgetForecast.TotalRem > requestBudgetForecasts[i].MaxValueRon)
					{
						requestBudgetForecasts[i].NeedBudget = false;
						requestBudgetForecasts[i].NeedBudgetValue = 0;
					}
					else
					{
						requestBudgetForecasts[i].NeedBudget = true;
						requestBudgetForecasts[i].NeedBudgetValue = requestBudgetForecasts[i].MaxValueRon - requestBudgetForecasts[i].BudgetForecast.TotalRem;
					}

                    if(requestBudgetForecasts[i].Contract != null)
                    {
						if (requestBudgetForecasts[i].Contract.ContractAmount.AmountRonRem > requestBudgetForecasts[i].MaxValueRon)
						{
							requestBudgetForecasts[i].NeedContract = false;
							requestBudgetForecasts[i].NeedContractValue = 0;
						}
						else
						{
							requestBudgetForecasts[i].NeedContract = true;
							requestBudgetForecasts[i].NeedContractValue = requestBudgetForecasts[i].MaxValueRon - requestBudgetForecasts[i].Contract.ContractAmount.AmountRonRem;
						}
					}

					

					_context.Update(requestBudgetForecasts[i]);

					requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
					.Include(o => o.OfferType)
					.Where(r => r.RequestBudgetForecastId == requestBudgetForecasts[i].Id && r.IsDeleted == false)
					.ToListAsync();

					for (int j = 0; j < requestBudgetForecastMaterials.Count;j++)
					{
						requestBudgetForecastMaterials[j].MaxQuantity = (int)sumQuantity;

						//if (offer.OfferType.Code == "O-V")
						//{
						//	requestBudgetForecastMaterials[j].MaxQuantity = 1;
						//}
						requestBudgetForecastMaterials[j].MaxValue = sumValue;
						requestBudgetForecastMaterials[j].MaxValueRon = sumValue * offer.Rate.Value;
						requestBudgetForecastMaterials[j].MaxQuantity = (int)offer.Quantity;

						requestBudgetForecastMaterials[j].OfferTypeId = offer.OfferTypeId;

						requestBudgetForecastMaterials[j].Value = offer.Price * (decimal)offer.Quantity;
						requestBudgetForecastMaterials[j].ValueRon = offer.PriceRon * (decimal)offer.Quantity;

						requestBudgetForecastMaterials[j].ValueRem = offer.Price * (decimal)offer.Quantity;
						requestBudgetForecastMaterials[j].ValueRemRon = offer.PriceRon * (decimal)offer.Quantity;

						requestBudgetForecastMaterials[j].Quantity = (int)offerMaterial.Quantity;
						requestBudgetForecastMaterials[j].QuantityRem = (int)offerMaterial.Quantity;

						requestBudgetForecastMaterials[i].Price = offerMaterial.Price;
						requestBudgetForecastMaterials[i].PriceRon = offerMaterial.PriceRon;

						_context.Update(requestBudgetForecastMaterials[j]);
						_context.SaveChanges();
					}

					//requestBFMaterialCostCenters = await _context.Set<Model.RequestBFMaterialCostCenter>()
					//.Include(o => o.OfferType)
					//.Where(r => r.OrderId == order.Id && r.IsDeleted == false)
					//.ToListAsync();

					//for (int k = 0; k < requestBFMaterialCostCenters.Count; k++)
					//{
					//	requestBFMaterialCostCenters[k].MaxQuantity = (int)sumQuantity;

					//	if (offer.OfferType.Code == "O-V")
					//	{
					//		requestBFMaterialCostCenters[k].MaxQuantity = 1;
					//	}
					//	requestBFMaterialCostCenters[k].MaxValue = sumValue;
					//	requestBFMaterialCostCenters[k].MaxValueRon = sumValue * offer.Rate.Value;
					//	requestBFMaterialCostCenters[k].OfferTypeId = offer.OfferTypeId;
					//	//requestBudgetForecasts[i].Price = sumPrice;
					//	//requestBudgetForecasts[i].PriceRon = sumPrice * offer.Rate.Value;

					//	_context.Update(requestBFMaterialCostCenters[k]);
					//	_context.SaveChanges();
					//}


					//_context.SaveChanges();
				}

				var countBudgetBase = await _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToListAsync();
				var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials").ToListAsync();

				_context.Update(order);
				_context.SaveChanges();
            }
            else
            {
				decimal sumQuantity = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.Quantity);
				decimal sumQuantityIni = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.QuantityIni);
				decimal sumPrice = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.Price);
				decimal sumPriceIni = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false).Sum(a => a.PriceIni);
				decimal sumValue = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.Value);
				decimal sumValueIni = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.ValueIni);

				requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>()
					.Include(o => o.Contract).ThenInclude(c => c.ContractAmount)
					.Include(o => o.BudgetForecast)
					.Include(o => o.OfferType)
					.Where(r => r.RequestId == offer.RequestId && r.IsDeleted == false)
					.ToListAsync();

				for (int i = 0; i < requestBudgetForecasts.Count; i++)
				{
					requestBudgetForecasts[i].MaxQuantity = (int)sumQuantity;

					//if (offer.OfferType.Code == "O-V")
					//{
					//	requestBudgetForecasts[i].MaxQuantity = 1;
					//}
					requestBudgetForecasts[i].MaxValue = sumValue;
					requestBudgetForecasts[i].MaxValueRon = sumValue * offer.Rate.Value;
					requestBudgetForecasts[i].OfferTypeId = offer.OfferTypeId;

					requestBudgetForecasts[i].MaxQuantity = (int)offer.Quantity;
					requestBudgetForecasts[i].Value = offer.Price * (decimal)offer.Quantity;
					requestBudgetForecasts[i].ValueRon = offer.PriceRon * (decimal)offer.Quantity;
					requestBudgetForecasts[i].TotalOrderValue = 0;
					requestBudgetForecasts[i].TotalOrderValueRon = 0;
					requestBudgetForecasts[i].TotalOrderQuantity = 0;
					requestBudgetForecasts[i].Quantity = (int)offer.Quantity;
					requestBudgetForecasts[i].Price = offerMaterial.Price;
					requestBudgetForecasts[i].PriceRon = offerMaterial.PriceRon;

                    if(requestBudgetForecasts[i].BudgetForecast.TotalRem > requestBudgetForecasts[i].MaxValueRon)
                    {
                        requestBudgetForecasts[i].NeedBudget = false;
                        requestBudgetForecasts[i].NeedBudgetValue = 0;
                    }
                    else
                    {
						requestBudgetForecasts[i].NeedBudget = true;
						requestBudgetForecasts[i].NeedBudgetValue = requestBudgetForecasts[i].MaxValueRon - requestBudgetForecasts[i].BudgetForecast.TotalRem;
					}

                    if(requestBudgetForecasts[i].Contract != null)
                    {
						if (requestBudgetForecasts[i].Contract.ContractAmount.AmountRonRem > requestBudgetForecasts[i].MaxValueRon)
						{
							requestBudgetForecasts[i].NeedContract = false;
							requestBudgetForecasts[i].NeedContractValue = 0;
						}
						else
						{
							requestBudgetForecasts[i].NeedContract = true;
							requestBudgetForecasts[i].NeedContractValue = requestBudgetForecasts[i].MaxValueRon - requestBudgetForecasts[i].Contract.ContractAmount.AmountRonRem;
						}
					}

					

					_context.Update(requestBudgetForecasts[i]);

					requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
					.Include(o => o.OfferType)
					.Where(r => r.RequestBudgetForecastId == requestBudgetForecasts[i].Id && r.IsDeleted == false)
					.ToListAsync();

					for (int j = 0; j < requestBudgetForecastMaterials.Count; j++)
					{
						requestBudgetForecastMaterials[j].MaxQuantity = (int)sumQuantity;

						//if (offer.OfferType.Code == "O-V")
						//{
						//	requestBudgetForecastMaterials[j].MaxQuantity = 1;
						//}
						requestBudgetForecastMaterials[j].MaxValue = sumValue;
						requestBudgetForecastMaterials[j].MaxValueRon = sumValue * offer.Rate.Value;
						requestBudgetForecastMaterials[j].MaxQuantity = (int)offer.Quantity;

						requestBudgetForecastMaterials[j].OfferTypeId = offer.OfferTypeId;

						requestBudgetForecastMaterials[j].Value = offer.Price * (decimal)offer.Quantity;
						requestBudgetForecastMaterials[j].ValueRon = offer.PriceRon * (decimal)offer.Quantity;

						requestBudgetForecastMaterials[j].ValueRem = offer.Price * (decimal)offer.Quantity;
						requestBudgetForecastMaterials[j].ValueRemRon = offer.PriceRon * (decimal)offer.Quantity;

						requestBudgetForecastMaterials[j].Quantity = (int)offer.Quantity;
						requestBudgetForecastMaterials[j].QuantityRem = (int)offer.Quantity;

						requestBudgetForecastMaterials[i].Price = offerMaterial.Price;
						requestBudgetForecastMaterials[i].PriceRon = offerMaterial.PriceRon;

                        _context.Update(requestBudgetForecastMaterials[j]);
						_context.SaveChanges();
					}

					//requestBFMaterialCostCenters = await _context.Set<Model.RequestBFMaterialCostCenter>()
					//.Include(o => o.OfferType)
					//.Where(r => r.OrderId == order.Id && r.IsDeleted == false)
					//.ToListAsync();

					//for (int k = 0; k < requestBFMaterialCostCenters.Count; k++)
					//{
					//	requestBFMaterialCostCenters[k].MaxQuantity = (int)sumQuantity;

					//	if (offer.OfferType.Code == "O-V")
					//	{
					//		requestBFMaterialCostCenters[k].MaxQuantity = 1;
					//	}
					//	requestBFMaterialCostCenters[k].MaxValue = sumValue;
					//	requestBFMaterialCostCenters[k].MaxValueRon = sumValue * offer.Rate.Value;
					//	requestBFMaterialCostCenters[k].OfferTypeId = offer.OfferTypeId;
					//	//requestBudgetForecasts[i].Price = sumPrice;
					//	//requestBudgetForecasts[i].PriceRon = sumPrice * offer.Rate.Value;

					//	_context.Update(requestBFMaterialCostCenters[k]);
					//	_context.SaveChanges();
					//}


					_context.SaveChanges();
				}

                var countBudgetBase = await _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToListAsync();
                var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials").ToListAsync();

                //_context.Update(order);
                //_context.SaveChanges();
            }

			return new OfferResult { Success = true, Message = "Datele au fost actualizate cu sucess!"};
        }

        [HttpPost("orderMaterialUpdate")]
        public async virtual Task<bool> OrderMaterialUpdate([FromBody] List<OfferMaterialUpdate> offerMaterialToUpdates)
        {
            bool success = false;

            for (int i = 0; i < offerMaterialToUpdates.Count; i++)
            {
                Model.OfferMaterial offerMaterial = _context.Set<Model.OfferMaterial>().Where(a => a.Id == offerMaterialToUpdates[i].Id).Single();

                if (offerMaterial != null)
                {
                    offerMaterial.OrdersQuantity = offerMaterialToUpdates[i].Quantity;
                    offerMaterial.OrdersPrice = offerMaterialToUpdates[i].Price;
                    offerMaterial.OrdersValue = offerMaterialToUpdates[i].Price * offerMaterialToUpdates[i].Quantity;
                    _context.Update(offerMaterial);
                    _context.SaveChanges();
                    success = true;
                }
            }

            return success;
        }

        [HttpDelete("remove/{id}")]
        public virtual IActionResult DeleteEOfferMaterial(int id)
        {

            Model.OfferMaterial offerMaterial = _context.Set<Model.OfferMaterial>().Where(a => a.Id == id).Single();

            if (offerMaterial != null)
            {

                offerMaterial.IsDeleted = true;
                offerMaterial.ModifiedAt = DateTime.Now;
                _context.Update(offerMaterial);
                _context.SaveChanges();
            }
            else
            {
                return Ok(StatusCode(404));
            }

            return Ok(StatusCode(200));
        }

        [HttpPost("offerITMaterialUpdate")]
        public async virtual Task<bool> OfferITMaterialUpdate([FromBody] List<OfferMaterialUpdate> offerMaterialToUpdates)
        {
            bool success = false;
            int offerId = 0;
            for (int i = 0; i < offerMaterialToUpdates.Count; i++)
            {
                Model.OfferMaterial offerMaterial = await _context.Set<Model.OfferMaterial>().Include(o => o.Offer).Include(a => a.Rate).ThenInclude(u => u.Uom).Where(a => a.Id == offerMaterialToUpdates[i].Id).SingleAsync();
                offerId = offerMaterial.OfferId;

                if (offerMaterial != null)
                {

                    offerMaterial.Price = offerMaterialToUpdates[i].Price;
                    offerMaterial.PriceRon = offerMaterialToUpdates[i].Price * offerMaterial.Rate.Value;
                    offerMaterial.PriceIni = offerMaterialToUpdates[i].Price;
                    offerMaterial.PriceIniRon = offerMaterialToUpdates[i].Price * offerMaterial.Rate.Value;
                    offerMaterial.Value = offerMaterialToUpdates[i].Price * offerMaterialToUpdates[i].Quantity;
                    offerMaterial.ValueRon = offerMaterialToUpdates[i].Price * offerMaterialToUpdates[i].Quantity * offerMaterial.Rate.Value;
                    offerMaterial.QuantityIni = offerMaterialToUpdates[i].Quantity;
                    offerMaterial.Quantity = offerMaterialToUpdates[i].Quantity;
                    offerMaterial.ValueIni = offerMaterialToUpdates[i].Price * offerMaterialToUpdates[i].Quantity;
                    offerMaterial.ValueIniRon = offerMaterialToUpdates[i].Price * offerMaterialToUpdates[i].Quantity * offerMaterial.Rate.Value;
                    offerMaterial.RequestId = offerMaterial.Offer.RequestId;
                    offerMaterial.WIP = offerMaterialToUpdates[i].WIP;

                    _context.Update(offerMaterial);
                    _context.SaveChanges();
                    success = true;
                }
            }

            if (success && offerId > 0)
            {
                decimal sumQuantity = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.Quantity);
                decimal sumQuantityIni = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.QuantityIni);
                decimal sumPrice = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.Price);
                decimal sumPriceIni = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false).Sum(a => a.PriceIni);
                decimal sumValue = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.Value);
                decimal sumValueIni = _context.Set<Model.OfferMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.OfferId == offerId).Sum(a => a.ValueIni);

                Model.Offer offer = await _context.Set<Model.Offer>().Include(r => r.Rate).Where(o => o.Id == offerId).SingleAsync();

                offer.QuantityRem = (float)sumQuantity;
                offer.Quantity = (float)sumQuantity;
                offer.Price = sumPrice;
                offer.PriceRon = sumPrice * offer.Rate.Value;
                offer.ValueFin = sumValue;
                offer.ValueFinRon = sumValue * offer.Rate.Value;
                offer.ValueIni = sumValueIni;
                offer.ValueIniRon = sumValueIni * offer.Rate.Value;

                _context.Update(offer);
                _context.SaveChanges();
            }

            return success;
        }

		[HttpGet("export")]
		public async Task<IActionResult> Export(string filter, string emailTypeIds, string appStateIds, string assetCategoryIds, string includes, string type)
		{
			List<Model.EmailManager> items = null;
			IEnumerable<Dto.EmailManager> itemsResult = null;
			List<int?> dIds = null;
			List<int?> aIds = null;
			List<int?> appIds = null;
			List<int> divisionIds = null;
			string employeeId = string.Empty;
			string role = string.Empty;
			bool inInventory = false;

			var userName = HttpContext.User.Identity.Name;
			var user = await _userManager.FindByEmailAsync(userName);
			if (user == null)
			{
				user = await _userManager.FindByNameAsync(userName);
			}

			includes = "EmailType,EmployeeInitial,EmployeeFinal,RoomInitial,RoomFinal,Asset,AssetComponent,AppState,SubType,Offer.Request,Order,Budget,Partner";

			//if (!user.InInventory)
			//{
			//    divisionIds = await _context.Set<Model.EmployeeDivision>()
			//        .Where(a => a.EmployeeId == user.EmployeeId)
			//        .Select(a => a.DivisionId)
			//        .ToListAsync();
			//}
			//else
			//{
			//    divisionIds = await _context.Set<Model.EmployeeCostCenter>().Where(a => a.EmployeeId == user.EmployeeId).Select(a => a.CostCenter != null && a.CostCenter.DivisionId != null ? a.CostCenter.DivisionId.Value : 0).ToListAsync();
			//}

			role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
			employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;
			inInventory = user.InInventory;

			if ((emailTypeIds != null) && (emailTypeIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(emailTypeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((appStateIds != null) && (appStateIds.Length > 0)) appIds = JsonConvert.DeserializeObject<string[]>(appStateIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((assetCategoryIds != null) && (assetCategoryIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(assetCategoryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

			
			using (ExcelPackage package = new ExcelPackage())
			{
				items = (_itemsRepository as IEmailManagersRepository).GetByFilters(employeeId, role, inInventory, filter, includes, null, null, null, null, dIds, appIds, aIds, divisionIds, type).ToList();
				itemsResult = items.Select(i => _mapper.Map<Dto.EmailManager>(i));

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Oferte-Produse");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod Oferta";
				worksheet.Cells[1, 2].Value = "Status";
				worksheet.Cells[1, 3].Value = "Date creare";
				worksheet.Cells[1, 4].Value = "Data modificare";
				worksheet.Cells[1, 5].Value = "Owner";
				worksheet.Cells[1, 6].Value = "Cod P.R.";
				worksheet.Cells[1, 7].Value = "Supplier CUI";
				worksheet.Cells[1, 8].Value = "Supplier";
				worksheet.Cells[1, 9].Value = "Tip";
				worksheet.Cells[1, 10].Value = "Info";

				int recordIndex = 2;
				foreach (var item in itemsResult)
				{
                    worksheet.Cells[recordIndex, 1].Value = item.Offer!= null ? item.Offer.Code : "";
                    worksheet.Cells[recordIndex, 2].Value = item.State != null ? item.State.Name : "";
                    worksheet.Cells[recordIndex, 3].Value = item.CreatedAt;
                    worksheet.Cells[recordIndex, 3].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 4].Value = item.ModifiedAt;
                    worksheet.Cells[recordIndex, 4].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 5].Value = item.Offer != null && item.Offer.Employee != null ? item.Offer.Employee.Email : "";
                    worksheet.Cells[recordIndex, 6].Value = item.Offer != null && item.Offer.Request != null ? item.Offer.Request.Code : "";
                    worksheet.Cells[recordIndex, 7].Value = item.Partner != null ? item.Partner.RegistryNumber : "";
                    worksheet.Cells[recordIndex, 8].Value = item.Partner != null ? item.Partner.Name : "";
                    worksheet.Cells[recordIndex, 9].Value = item.EmailType != null ? item.EmailType.Name : "";
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
				worksheet.Column(10).AutoFit();

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
					FileDownloadName = "Oferte-Produse.xlsx"
				};

				return result;

			}
		}
	}
}
