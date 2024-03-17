using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/employeecostcenters")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class EmployeeCostCentersController : GenericApiController<Model.EmployeeCostCenter, Dto.EmployeeCostCenter>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public EmployeeCostCentersController(ApplicationDbContext context, IEmployeeCostCentersRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string costCenterIds, string employeeIds, string includes)
        {
            List<Model.EmployeeCostCenter> items = null;
            IEnumerable<Dto.EmployeeCostCenter> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;

			includes = "Employee,CostCenter.Storage";


			if (costCenterIds != null && !costCenterIds.StartsWith("["))
			{
                costCenterIds = "[" + costCenterIds + "]";
			}

			if (employeeIds != null && !employeeIds.StartsWith("["))
			{
                employeeIds = "[" + employeeIds + "]";
			}

			if ((costCenterIds != null) && (costCenterIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(costCenterIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((employeeIds != null) && (employeeIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(employeeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IEmployeeCostCentersRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, eIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.EmployeeCostCenter>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IEmployeeCostCentersRepository).GetCountByFilters(filter, cIds, eIds);
                var pagedResult = new Dto.PagedResult<Dto.EmployeeCostCenter>(itemsResult, new Dto.PagingInfo()
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
        [Route("add")]
        public async virtual Task<IActionResult> AddEmployeeCostCenter([FromBody] Dto.EmployeeCostCenterAdd empCostCenterAdd)
        {
            Model.EmployeeCostCenter employeeCostCenter = null;
            var user = _context.Set<Model.ApplicationUser>().Where(u => u.EmployeeId == empCostCenterAdd.EmployeeId).FirstOrDefault();

            var claims = await _userManager.GetClaimsAsync(user);

            var claim = claims.FirstOrDefault(c => c.Type == "costcenters");

            var costCenterClaims = "";

            for (int i = 0; i < empCostCenterAdd.CostCenterIds.Length; i++)
            {
                var costCenter = _context.Set<Model.CostCenter>().Where(u => u.Id == empCostCenterAdd.CostCenterIds[i]).FirstOrDefault();


                if (i == empCostCenterAdd.CostCenterIds.Count() - 1)
                {
                    costCenterClaims = String.Concat(costCenterClaims, costCenter.Code);
                }
                else
                {
                    costCenterClaims = String.Concat(costCenterClaims, costCenter.Code + "|");
                }


                employeeCostCenter = new Model.EmployeeCostCenter()
                {
                    EmployeeId = empCostCenterAdd.EmployeeId,
                    CostCenterId = empCostCenterAdd.CostCenterIds[i]
                };

                _context.Add(employeeCostCenter);

                _context.SaveChanges();
            }

            return Ok(StatusCode(200));
        }

        [HttpDelete("remove/{id}")]
        public virtual IActionResult DeleteEmployeeCostCenter(int id)
        {

            Model.EmployeeCostCenter employeeCostCenter = _context.Set<Model.EmployeeCostCenter>().Where(a => a.Id == id).Single();

            if (employeeCostCenter != null)
            {

                employeeCostCenter.IsDeleted = true;
                employeeCostCenter.ModifiedAt = DateTime.Now;
                _context.Update(employeeCostCenter);
                _context.SaveChanges();
            }
            else
            {
                return Ok(StatusCode(404));
            }

            return Ok(StatusCode(200));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sync")]
        public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.EmployeeCostCenter> items = (_itemsRepository as IEmployeeCostCentersRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<EmployeeCostCenterSync>(i));
            var pagedResult = new Dto.PagedResult<EmployeeCostCenterSync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }
    }
}
