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
    [Route("api/columndefinitionroles")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class ColumnDefinitionRolesController : GenericApiController<Model.ColumnDefinitionRole, Dto.ColumnDefinitionRole>
    {
		private readonly UserManager<ApplicationUser> userManager;

		public ColumnDefinitionRolesController(ApplicationDbContext context, IColumnDefinitionRolesRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
			this.userManager = userManager;
		}

        [HttpGet("data/{role}")]
        public virtual async Task<string[]> GetData(string role)
        {
            IColumnDefinitionRolesRepository repo = _itemsRepository as IColumnDefinitionRolesRepository;
            var items = await repo.GetColumnDefinitionByRoleAsync(role);

            var permission = new List<string>();

            for (int i = 0; i < items.Count; i++)
            {

                //permission.Add(items[i].ColumnDefinition.Code + "|" + items[i].PermissionType.Code);
            }

            var result = permission.Distinct().ToArray();

            return result;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string columnDefinitionIds, string roleIds, string includes)
        {
            List<Model.ColumnDefinitionRole> items = null;
            IEnumerable<Dto.ColumnDefinitionRole> itemsResult = null;
            List<int?> dIds = null;
            List<string> rIds = null;
            includes = "ColumnDefinition,Role";

            if (roleIds != null)
            {
                rIds = new List<string>();
                rIds.Add(roleIds);
            }

            if ((columnDefinitionIds != null) && (columnDefinitionIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(columnDefinitionIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();



            items = (_itemsRepository as IColumnDefinitionRolesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, dIds, rIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.ColumnDefinitionRole>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IColumnDefinitionRolesRepository).GetCountByFilters(filter, dIds, rIds);
                var pagedResult = new Dto.PagedResult<Dto.ColumnDefinitionRole>(itemsResult, new Dto.PagingInfo()
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
