using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto.Common;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/permissionroles")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class PermissionRolesController : GenericApiController<Model.PermissionRole, Dto.PermissionRole>
    {
		private readonly UserManager<ApplicationUser> userManager;

		public PermissionRolesController(ApplicationDbContext context, IPermissionRolesRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
			this.userManager = userManager;
		}

        [HttpGet("data/{role}")]
        public virtual async Task<string[]> GetData(string role)
        {
            IPermissionRolesRepository repo = _itemsRepository as IPermissionRolesRepository;
            var items = await repo.GetPermissionByRoleAsync(role);

            var permission = new List<string>();

            for (int i = 0; i < items.Count; i++)
            {

                permission.Add(items[i].Permission.Code + "|" + items[i].PermissionType.Code);
            }

            var result = permission.Distinct().ToArray();

            return result;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string permissionTypeIds, string permissionIds, string roleIds, string includes)
        {
            List<Model.PermissionRole> items = null;
            IEnumerable<Dto.PermissionRole> itemsResult = null;
            List<int?> dIds = null;
            List<int?> pIds = null;
            List<string> rIds = null;
            includes = "Permission,PermissionType,Role";

            if (roleIds != null)
            {
                rIds = new List<string>();
                rIds.Add(roleIds);
            }

            if ((permissionTypeIds != null) && (permissionTypeIds.Length > 0)) pIds = JsonConvert.DeserializeObject<string[]>(permissionTypeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((permissionIds != null) && (permissionIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(permissionIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();



            items = (_itemsRepository as IPermissionRolesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, pIds, dIds, rIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.PermissionRole>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IPermissionRolesRepository).GetCountByFilters(filter, pIds, dIds, rIds);
                var pagedResult = new Dto.PagedResult<Dto.PermissionRole>(itemsResult, new Dto.PagingInfo()
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


        [Route("accept/{id}")]
        public async virtual Task<bool> ValidateOffer(int id)
        {
            Model.EmailManager emailManager = null;
            bool success = false;

            if (id  > 0)
            {
                emailManager = await _context.Set<Model.EmailManager>().Include(e => e.EmailType).Where(a => a.Id == id).SingleAsync();

                if (emailManager != null && emailManager.EmailType.Code == "NEW_OFFER")
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
                    emailManager.AppStateId = 7;
                    offer.IsAccepted = true;
                    offer.AppStateId = 7;
                    offer.PartnerId = emailManager.PartnerId;
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
                List<Model.EmailManager> emailManagers = _context.Set<Model.EmailManager>().Where(e => e.SubTypeId == emailManager.SubTypeId && e.AppStateId == 6).ToList();

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

        [HttpPost]
        [Route("clone/{permissionType}/{permisionTypeNew}")]
        public async virtual Task<bool> SaveReco(int permissionType, int permisionTypeNew)
        {
            List<Model.PermissionRole> permissionRoles = null;
            Model.PermissionRole permissionRoleNew = null;
            bool result = false;

            permissionRoles = await _context.Set<Model.PermissionRole>().Where(a => a.PermissionTypeId == permissionType && a.IsDeleted == false).ToListAsync();

            if(permissionRoles.Count > 0)
			{
                var userName = HttpContext.User.Identity.Name;


                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }


                if (user != null)
				{
                    for (int i = 0; i < permissionRoles.Count; i++)
                    {
                        permissionRoleNew = new Model.PermissionRole()
                        {
                            Code = permissionRoles[i].Code,
                            CreatedAt = DateTime.Now,
                            CreatedBy = user.Id,
                            IsDeleted = false,
                            ModifiedAt = DateTime.Now,
                            ModifiedBy = user.Id,
                            Name = permissionRoles[i].Name,
                            PermissionId = permissionRoles[i].PermissionId,
                            RoleId = permissionRoles[i].RoleId,
                            PermissionTypeId = permisionTypeNew
                        };

                        _context.Add(permissionRoleNew);
                        _context.SaveChanges();
                        result = true;
                    }
                }
			}
            

            return result;
        }



        //[HttpPost]
        //[Route("cloneAll/{permissionTypeId}/{roleId}/{cloneRoleId}/{cloneAll}")]
        //public async virtual Task<bool> SaveReco(int permissionTypeId, string roleId, string cloneRoleId, bool cloneAll)
        //{
        //    List<Model.PermissionRole> permissionRoles = null;
        //    List<Model.Route> routes = null;
        //    Model.RouteChildren routeChildren = null;
        //    Model.Route route = null;
        //    bool result = false;

        //    var userName = HttpContext.User.Identity.Name1;
        //    var user = await _context.Users.Where(u => u.UserName == username).SingleOrDefaultAsync();

        //    if (cloneAll)
        //    {
        //        routes = await _context.Set<Model.Route>().Where(a => a.IsDeleted == false && a.RoleId == roleId).ToListAsync();


        //        for (int t = 0; t < routes.Count; t++)
        //        {
        //            route = await _context.Set<Model.Route>().Where(a => a.Id == routes[t].Id && a.RoleId == cloneRoleId && a.IsDeleted == false).SingleOrDefaultAsync();

        //            if (route == null)
        //            {
        //                route = new Model.Route()
        //                {
        //                    Class = routes[t].Class,
        //                    CreatedAt = DateTime.Now,
        //                    CreatedBy = user.Id,
        //                    Divider = routes[t].Divider,
        //                    Href = routes[t].Href,
        //                    Icon = routes[t].Icon,
        //                    IsDeleted = false,
        //                    ModifiedAt = DateTime.Now,
        //                    ModifiedBy = user.Id,
        //                    Name = routes[t].Name,
        //                    RoleId = cloneRoleId,
        //                    Title = routes[t].Title,
        //                    Url = routes[t].Url,
        //                    Active = true,
        //                    BadgeId = routes[t].BadgeId,
        //                    IconRouteId = routes[t].IconRouteId,
        //                    Variant = routes[t].Variant,
        //                    Position = routes[t].Position
        //                };

        //                _context.Add(route);
        //                _context.SaveChanges();
        //            }

        //            permissionRoles = await _context.Set<Model.RouteChildren>().Where(a => a.RouteId == routes[t].Id && a.IsDeleted == false && a.RoleId == roleId).ToListAsync();

        //            if (permissionRoles.Count > 0 && roleId != "" && cloneRoleId != "")
        //            {
        //                if (user != null)
        //                {
        //                    for (int i = 0; i < permissionRoles.Count; i++)
        //                    {
        //                        routeChildren = new Model.RouteChildren()
        //                        {
        //                            Class = permissionRoles[i].Class,
        //                            CreatedAt = DateTime.Now,
        //                            CreatedBy = user.Id,
        //                            Divider = permissionRoles[i].Divider,
        //                            Href = permissionRoles[i].Href,
        //                            Icon = permissionRoles[i].Icon,
        //                            IsDeleted = false,
        //                            ModifiedAt = DateTime.Now,
        //                            ModifiedBy = user.Id,
        //                            Name = permissionRoles[i].Name,
        //                            RoleId = cloneRoleId,
        //                            RouteId = route.Id,
        //                            Title = permissionRoles[i].Title,
        //                            Url = permissionRoles[i].Url,
        //                            Active = true,
        //                            BadgeId = permissionRoles[i].BadgeId,
        //                            IconRouteId = permissionRoles[i].IconRouteId,
        //                            Variant = permissionRoles[i].Variant,
        //                            Position = permissionRoles[i].Position
        //                        };

        //                        _context.Add(routeChildren);
        //                        _context.SaveChanges();
        //                        result = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        permissionRoles = await _context.Set<Model.RouteChildren>().Where(a => a.RouteId == routeId && a.IsDeleted == false && a.RoleId == roleId).ToListAsync();

        //        if (permissionRoles.Count > 0 && roleId != "" && cloneRoleId != "")
        //        {
        //            if (user != null)
        //            {
        //                for (int i = 0; i < permissionRoles.Count; i++)
        //                {
        //                    routeChildren = new Model.RouteChildren()
        //                    {
        //                        Class = permissionRoles[i].Class,
        //                        CreatedAt = DateTime.Now,
        //                        CreatedBy = user.Id,
        //                        Divider = permissionRoles[i].Divider,
        //                        Href = permissionRoles[i].Href,
        //                        Icon = permissionRoles[i].Icon,
        //                        IsDeleted = false,
        //                        ModifiedAt = DateTime.Now,
        //                        ModifiedBy = user.Id,
        //                        Name = permissionRoles[i].Name,
        //                        RoleId = cloneRoleId,
        //                        RouteId = routeId,
        //                        Title = permissionRoles[i].Title,
        //                        Url = permissionRoles[i].Url,
        //                        Active = true,
        //                        BadgeId = permissionRoles[i].BadgeId,
        //                        IconRouteId = permissionRoles[i].IconRouteId,
        //                        Variant = permissionRoles[i].Variant,
        //                        Position = permissionRoles[i].Position
        //                    };

        //                    _context.Add(routeChildren);
        //                    _context.SaveChanges();
        //                    result = true;
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}
    }
}
