using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Optima.Fais.Model;

namespace Optima.Fais.Api.Controllers
{
	[Authorize]
	[Route("api/routes")]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public partial class RoutesController : GenericApiController<Model.Route, Dto.Route>
	{
		private readonly UserManager<ApplicationUser> userManager;

		public RoutesController(ApplicationDbContext context, IRoutesRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
			: base(context, itemsRepository, mapper)
		{
			this.userManager = userManager;
		}


		[HttpGet("data/{role}")]
		public virtual async Task<IActionResult> GetDownloadData(string role)
		{
			IRoutesRepository repo = _itemsRepository as IRoutesRepository;
			var items = await repo.GetAllIncludingRouteChildrensAsync(role);

			return Ok(items.OrderBy(r => r.Position).Select(i => _mapper.Map<Dto.RouteData>(i)));
		}

		[HttpGet]
		[Route("", Order = -1)]
		public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string roleIds, string includes)
		{
			List<Model.Route> items = null;
			IEnumerable<Dto.Route> itemsResult = null;
			List<string> rIds = null;

			if (roleIds != null)
			{
				rIds = new List<string>();
				rIds.Add(roleIds);
			}

			items = (_itemsRepository as IRoutesRepository).GetByFilters(filter, "Role,Badge", sortColumn, sortDirection, page, pageSize, rIds).ToList();


			itemsResult = items.Where(r => r.IsDeleted == false).Select(i => _mapper.Map<Dto.Route>(i));


			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as IRoutesRepository).GetCountByFilters(filter, rIds);
				var pagedResult = new Dto.PagedResult<Dto.Route>(itemsResult, new Dto.PagingInfo()
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
            bool result = false;

            var userName = HttpContext.User.Identity.Name;
            var user = await userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await userManager.FindByNameAsync(userName);
            }

            if (cloneAll)
            {
                routes = await _context.Set<Model.Route>().Where(a => a.IsDeleted == false).ToListAsync();


                for (int t = 0; t < routes.Count; t++)
                {
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
                                    RouteId = routes[t].Id,
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
