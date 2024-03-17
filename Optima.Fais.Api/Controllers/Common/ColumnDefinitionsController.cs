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
    [Route("api/columndefinitions")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class ColumnDefinitionsController : GenericApiController<Model.ColumnDefinition, Dto.ColumnDefinition>
    {
		private readonly UserManager<ApplicationUser> userManager;

		public ColumnDefinitionsController(ApplicationDbContext context, IColumnDefinitionsRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
			this.userManager = userManager;
		}

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string tableDefinitionIds, string roleIds, string roleName, string filter)
        {
            List<Model.ColumnDefinition> items = null;
            IEnumerable<Dto.ColumnDefinition> itemsResult = null;
            List<int> tdIds = null;
			List<string> rdIds = null;

			if (roleIds != null)
			{
				rdIds = new List<string>();
				rdIds.Add(roleIds);
			}

			if ((tableDefinitionIds != null) && (tableDefinitionIds.Length > 0)) tdIds = JsonConvert.DeserializeObject<string[]>(tableDefinitionIds).ToList().Select(int.Parse).ToList();

			items = (_itemsRepository as IColumnDefinitionsRepository).GetByFilters(filter, tdIds, rdIds, roleName, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.ColumnDefinition>(i));

            if (HttpContext.User.Identity.Name != null)
            {
                string userName = HttpContext.User.Identity.Name;
                string role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                string employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                //var count = _context.Set<Model.RecordCount>().FromSql("UpdateBadgeCount {0}, {1}", role, employeeId).ToList();
            }

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IColumnDefinitionsRepository).GetCountByFilters(filter, tdIds, rdIds, roleName);
                var pagedResult = new Dto.PagedResult<Dto.ColumnDefinition>(itemsResult, new Dto.PagingInfo()
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

        [AllowAnonymous]
        [HttpGet]
        [Route("allowAnonymous", Order = -1)]
        public virtual IActionResult GetAllowAnonymous(int? page, int? pageSize, string sortColumn, string sortDirection, string tableDefinitionIds, string roleIds, string roleName, string filter)
        {
            List<Model.ColumnDefinition> items = null;
            IEnumerable<Dto.ColumnDefinition> itemsResult = null;
            List<int> tdIds = null;
            List<string> rdIds = null;

            _context.Database.SetCommandTimeout(120);

            if (roleIds != null)
            {
                rdIds = new List<string>();
                rdIds.Add(roleIds);
            }

            if ((tableDefinitionIds != null) && (tableDefinitionIds.Length > 0)) tdIds = JsonConvert.DeserializeObject<string[]>(tableDefinitionIds).ToList().Select(int.Parse).ToList();

            items = (_itemsRepository as IColumnDefinitionsRepository).GetByFilters(filter, tdIds, rdIds, roleName, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.ColumnDefinition>(i));

            if (HttpContext.User.Identity.Name != null)
            {
                string userName = HttpContext.User.Identity.Name;
                string role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                string employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                var count = _context.Set<Model.RecordCount>().FromSql("UpdateBadgeCount {0}, {1}", role, employeeId).ToList();
            }

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IColumnDefinitionsRepository).GetCountByFilters(filter, tdIds, rdIds, roleName);
                var pagedResult = new Dto.PagedResult<Dto.ColumnDefinition>(itemsResult, new Dto.PagingInfo()
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
        [Route("clone/{tableDefinitionId}/{roleId}/{cloneRoleId}/{cloneAll}")]
        public async virtual Task<bool> SaveReco(int tableDefinitionId, string roleId, string cloneRoleId, bool cloneAll)
        {
            List<Model.ColumnDefinition> columnDefinitions = null;
            List<Model.TableDefinition> tableDefinitions = null;
            Model.ColumnDefinition columnDefinition = null;
            bool result = false;

            var userName = HttpContext.User.Identity.Name;
            var user = await userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await userManager.FindByNameAsync(userName);
            }

            if (cloneAll)
			{
                tableDefinitions = await _context.Set<Model.TableDefinition>().Where(a => a.IsDeleted == false).ToListAsync();


                for (int t = 0; t < tableDefinitions.Count; t++)
				{
                    columnDefinitions = await _context.Set<Model.ColumnDefinition>().Where(a => a.TableDefinitionId == tableDefinitions[t].Id && a.IsDeleted == false && a.RoleId == roleId).ToListAsync();

                    if (columnDefinitions.Count > 0 && roleId != "" && cloneRoleId != "")
                    {
                        if (user != null)
                        {
                            for (int i = 0; i < columnDefinitions.Count; i++)
                            {
                                columnDefinition = new Model.ColumnDefinition()
                                {
                                    Active = true,
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = user.Id,
                                    Format = columnDefinitions[i].Format,
                                    HeaderCode = columnDefinitions[i].HeaderCode,
                                    Include = columnDefinitions[i].Include,
                                    IsDeleted = columnDefinitions[i].IsDeleted,
                                    ModifiedAt = DateTime.Now,
                                    ModifiedBy = user.Id,
                                    Pipe = columnDefinitions[i].Pipe,
                                    Position = columnDefinitions[i].Position,
                                    Property = columnDefinitions[i].Property,
                                    SortBy = columnDefinitions[i].SortBy,
                                    TableDefinitionId = tableDefinitions[t].Id,
                                    TextAlign = columnDefinitions[i].TextAlign,
                                    RoleId = cloneRoleId,
                                };

                                _context.Add(columnDefinition);
                                _context.SaveChanges();
                                result = true;
                            }
                        }
                    }
                }
			}
			else
			{
                columnDefinitions = await _context.Set<Model.ColumnDefinition>().Where(a => a.TableDefinitionId == tableDefinitionId && a.IsDeleted == false && a.RoleId == roleId).ToListAsync();

                if (columnDefinitions.Count > 0 && roleId != "" && cloneRoleId != "")
                {
                    if (user != null)
                    {
                        for (int i = 0; i < columnDefinitions.Count; i++)
                        {
                            columnDefinition = new Model.ColumnDefinition()
                            {
                                Active = true,
                                CreatedAt = DateTime.Now,
                                CreatedBy = user.Id,
                                Format = columnDefinitions[i].Format,
                                HeaderCode = columnDefinitions[i].HeaderCode,
                                Include = columnDefinitions[i].Include,
                                IsDeleted = columnDefinitions[i].IsDeleted,
                                ModifiedAt = DateTime.Now,
                                ModifiedBy = user.Id,
                                Pipe = columnDefinitions[i].Pipe,
                                Position = columnDefinitions[i].Position,
                                Property = columnDefinitions[i].Property,
                                SortBy = columnDefinitions[i].SortBy,
                                TableDefinitionId = tableDefinitionId,
                                TextAlign = columnDefinitions[i].TextAlign,
                                RoleId = cloneRoleId,
                            };

                            _context.Add(columnDefinition);
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
