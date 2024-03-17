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
    [Route("api/routeroles")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class RouteRolesController : GenericApiController<Model.RouteRole, Dto.RouteRole>
    {
		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<ApplicationRole> roleManager;

		public RouteRolesController(ApplicationDbContext context, IRouteRolesRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager, RoleManager<Model.ApplicationRole> roleManager)
            : base(context, itemsRepository, mapper)
        {
			this.userManager = userManager;
			this.roleManager = roleManager;
		}

        [HttpGet("data/{username}")]
        public virtual async Task<IActionResult> GetDownloadData(string username)
        {
            List<string> rIds = new List<string>();
            List<Route> routes = new List<Route>();
            if(HttpContext.User.Identity.Name != null)
			{
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByNameAsync(userName);

                if (user == null)
                {
                    user = await userManager.FindByEmailAsync(userName);
                }

                var roles = await userManager.GetRolesAsync(user);

                for (int i = 0; i < roles.Count; i++)
				{
                    var role = await roleManager.FindByNameAsync(roles[i]);
					rIds.Add(role.Id);
                }

                IRouteRolesRepository repo = _itemsRepository as IRouteRolesRepository;
                routes = await repo.GetAllIncludingRouteChildrensAsync(rIds);
            }

            return Ok(routes.OrderBy(r => r.Position).Select(i => _mapper.Map<Dto.RouteData>(i)));
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string routeIds, string roleIds, string includes)
        {
            List<Model.RouteRole> items = null;
            IEnumerable<Dto.RouteRole> itemsResult = null;
            List<int?> dIds = null;
            List<string> rIds = null;
            includes = "Route,Role";

            if (roleIds != null)
            {
                rIds = new List<string>();
                rIds.Add(roleIds);
            }

            if ((routeIds != null) && (routeIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(routeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();



            items = (_itemsRepository as IRouteRolesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, dIds, rIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.RouteRole>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IRouteRolesRepository).GetCountByFilters(filter, dIds, rIds);
                var pagedResult = new Dto.PagedResult<Dto.RouteRole>(itemsResult, new Dto.PagingInfo()
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
    }
}
