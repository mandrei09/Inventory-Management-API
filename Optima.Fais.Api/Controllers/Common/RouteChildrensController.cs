using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/routechildrens")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class RouteChildrensController : GenericApiController<Model.RouteChildren, Dto.RouteChildren>
    {
		private readonly UserManager<ApplicationUser> userManager;

		public RouteChildrensController(ApplicationDbContext context, IRouteChildrensRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
			this.userManager = userManager;
		}

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string routeIds, string roleIds, string roleName, string filter)
        {
            List<Model.RouteChildren> items = null;
            IEnumerable<Dto.RouteChildren> itemsResult = null;
            List<int> tdIds = null;
            List<string> rdIds = null;

            if (roleIds != null)
            {
                rdIds = new List<string>();
                rdIds.Add(roleIds);
            }

            if ((routeIds != null) && (routeIds.Length > 0) && (routeIds[0].ToString() != "0")) tdIds = JsonConvert.DeserializeObject<string[]>(routeIds).ToList().Select(int.Parse).ToList();

            items = (_itemsRepository as IRouteChildrensRepository).GetByFilters(filter, "Route,Role,Badge", tdIds, rdIds, roleName, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Where(r => r.IsDeleted == false).Select(i => _mapper.Map<Dto.RouteChildren>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IRouteChildrensRepository).GetCountByFilters(filter, tdIds, rdIds, roleName);
                var pagedResult = new Dto.PagedResult<Dto.RouteChildren>(itemsResult, new Dto.PagingInfo()
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

        [HttpPost]
        [Route("clone/{routeId}/{roleId}/{cloneRoleId}/{cloneAll}")]
        public async virtual Task<bool> SaveReco(int routeId, string roleId, string cloneRoleId, bool cloneAll)
        {
            List<Model.RouteChildren> routeChildrens = null;
            List<Model.Route> routes = null;
            Model.RouteChildren routeChildren = null;
            Model.Route route = null;
            bool result = false;

            var userName = HttpContext.User.Identity.Name;
            var user = await userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await userManager.FindByNameAsync(userName);
            }

            if (cloneAll)
            {
                routes = await _context.Set<Model.Route>().Where(a => a.IsDeleted == false && a.RoleId == roleId).ToListAsync();


                for (int t = 0; t < routes.Count; t++)
                {
                    route = await _context.Set<Model.Route>().Where(a => a.Id == routes[t].Id && a.RoleId == cloneRoleId && a.IsDeleted == false).SingleOrDefaultAsync();

                    if(route == null)
					{
                        route = new Model.Route()
                        {
                            Class = routes[t].Class,
                            CreatedAt = DateTime.Now,
                            CreatedBy = user.Id,
                            Divider = routes[t].Divider,
                            Href = routes[t].Href,
                            Icon = routes[t].Icon,
                            IsDeleted = false,
                            ModifiedAt = DateTime.Now,
                            ModifiedBy = user.Id,
                            Name = routes[t].Name,
                            RoleId = cloneRoleId,
                            Title = routes[t].Title,
                            Url = routes[t].Url,
                            Active = true,
                            BadgeId = routes[t].BadgeId,
                            IconRouteId = routes[t].IconRouteId,
                            Variant = routes[t].Variant,
                            Position = routes[t].Position
                        };

                        _context.Add(route);
                        _context.SaveChanges();
					}

                    routeChildrens = await _context.Set<Model.RouteChildren>().Where(a => a.RouteId == routes[t].Id && a.IsDeleted == false && a.RoleId == roleId).ToListAsync();

                    if (routeChildrens.Count > 0 && roleId != "" && cloneRoleId != "")
                    {
                        if (user != null)
                        {
                            for (int i = 0; i < routeChildrens.Count; i++)
                            {
                                routeChildren = new Model.RouteChildren()
                                {
                                    Class = routeChildrens[i].Class,
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = user.Id,
                                    Divider = routeChildrens[i].Divider,
                                    Href = routeChildrens[i].Href,
                                    Icon = routeChildrens[i].Icon,
                                    IsDeleted = false,
                                    ModifiedAt = DateTime.Now,
                                    ModifiedBy = user.Id,
                                    Name = routeChildrens[i].Name,
                                    RoleId = cloneRoleId,
                                    RouteId = route.Id,
                                    Title = routeChildrens[i].Title,
                                    Url = routeChildrens[i].Url,
                                    Active = true,
                                    BadgeId = routeChildrens[i].BadgeId,
                                    IconRouteId = routeChildrens[i].IconRouteId,
                                    Variant = routeChildrens[i].Variant,
                                    Position = routeChildrens[i].Position
                                };

                                _context.Add(routeChildren);
                                _context.SaveChanges();
                                result = true;
                            }
                        }
                    }
                }
            }
			else
			{
				routeChildrens = await _context.Set<Model.RouteChildren>().Where(a => a.RouteId == routeId && a.IsDeleted == false && a.RoleId == roleId).ToListAsync();

				if (routeChildrens.Count > 0 && roleId != "" && cloneRoleId != "")
				{
					if (user != null)
					{
						for (int i = 0; i < routeChildrens.Count; i++)
						{
							routeChildren = new Model.RouteChildren()
							{
								Class = routeChildrens[i].Class,
								CreatedAt = DateTime.Now,
								CreatedBy = user.Id,
								Divider = routeChildrens[i].Divider,
								Href = routeChildrens[i].Href,
								Icon = routeChildrens[i].Icon,
								IsDeleted = false,
								ModifiedAt = DateTime.Now,
								ModifiedBy = user.Id,
								Name = routeChildrens[i].Name,
								RoleId = cloneRoleId,
								RouteId = routeId,
								Title = routeChildrens[i].Title,
								Url = routeChildrens[i].Url,
								Active = true,
								BadgeId = routeChildrens[i].BadgeId,
								IconRouteId = routeChildrens[i].IconRouteId,
								Variant = routeChildrens[i].Variant,
								Position = routeChildrens[i].Position
							};

							_context.Add(routeChildren);
							_context.SaveChanges();
							result = true;
						}
					}
				}
			}
			return result;
        }
    }
}
