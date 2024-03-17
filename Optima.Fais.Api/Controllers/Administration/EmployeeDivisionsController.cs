using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/employeedivisions")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class EmployeeDivisionsController : GenericApiController<Model.EmployeeDivision, Dto.EmployeeDivision>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public EmployeeDivisionsController(ApplicationDbContext context, IEmployeeDivisionsRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string divisionIds, string employeeIds, string includes)
        {
            List<Model.EmployeeDivision> items = null;
            IEnumerable<Model.EmployeeDivision> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;
            
			includes = "Employee,Division";


			if (divisionIds != null && !divisionIds.StartsWith("["))
			{
                divisionIds = "[" + divisionIds + "]";
			}

			if (employeeIds != null && !employeeIds.StartsWith("["))
			{
                employeeIds = "[" + employeeIds + "]";
			}

			if ((divisionIds != null) && (divisionIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(divisionIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((employeeIds != null) && (employeeIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(employeeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            //items = (_itemsRepository as IEmployeeDivisionsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, eIds).ToList();
            items = (_itemsRepository as IEmployeeDivisionsRepository).GetCustomQuery(Convert.ToInt32(eIds[0]), sortColumn, sortDirection).ToList();

             itemsResult = items.Select(i => _mapper.Map<Model.EmployeeDivision>(i)); 


            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IEmployeeDivisionsRepository).GetCountByFilters(filter, cIds, eIds);
                var pagedResult = new Dto.PagedResult<Model.EmployeeDivision>(itemsResult, new Dto.PagingInfo()
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
        public async virtual Task<IActionResult> AddEmployeeDivision([FromBody] Dto.EmployeeDivisionAdd empDivisionAdd)
        {
            Model.EmployeeDivision employeeDivision = null;
            Model.Department department = await _context.Set<Model.Department>().Where(a => a.Id == empDivisionAdd.DepartmentIds[0] && a.IsDeleted == false).FirstOrDefaultAsync();
            var user = await _context.Set<Model.ApplicationUser>().Where(u => u.EmployeeId == empDivisionAdd.EmployeeId).FirstOrDefaultAsync();

            for (int i = 0; i < empDivisionAdd.DivisionIds.Length; i++)
            {
                //division = await _context.Set<Model.Division>().Where(u => u.Id == empDivisionAdd.DivisionIds[i]).FirstOrDefaultAsync();
                //department = await _context.Set<Model.Department>().Where(u => u.Id == empDivisionAdd.DepartmentIds[i]).FirstOrDefaultAsync();


                employeeDivision = new Model.EmployeeDivision()
                {
                    EmployeeId = empDivisionAdd.EmployeeId,
                    DivisionId = empDivisionAdd.DivisionIds[i],
                    DepartmentId = department.Id
                };
                _context.Add(employeeDivision);
                _context.SaveChanges();

                var costCenterIds = (from ed in _context.EmployeeDivisions
                                     join d in _context.Divisions on ed.DepartmentId equals d.DepartmentId
                                     join cc in _context.CostCenters on d.Id equals cc.DivisionId
                                     where ed.EmployeeId == empDivisionAdd.EmployeeId && !ed.IsDeleted && ed.DepartmentId == empDivisionAdd.DepartmentIds[i] && !d.IsDeleted && !cc.IsDeleted
                                     select cc.Id).ToList();

                foreach (var costCenterId in costCenterIds)
                {
                    var employeeCostCenter = new Model.EmployeeCostCenter()
                    {
                        EmployeeId = empDivisionAdd.EmployeeId,
                        CostCenterId = costCenterId
                    };

                    _context.Add(employeeCostCenter);
                }


                _context.SaveChanges();
            }

            return Ok(StatusCode(200));
        }

        [HttpDelete("remove/{id}")]
        public virtual IActionResult DeleteEmployeeDivision(int id)
        {

            Model.EmployeeDivision employeeDivision = _context.Set<Model.EmployeeDivision>().Where(a => a.Id == id).Single();

            if (employeeDivision != null)
            {

                employeeDivision.IsDeleted = true;
                employeeDivision.ModifiedAt = DateTime.Now;
                _context.Update(employeeDivision);
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
            List<Model.EmployeeDivision> items = (_itemsRepository as IEmployeeDivisionsRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<EmployeeDivisionSync>(i));
            var pagedResult = new Dto.PagedResult<EmployeeDivisionSync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }
    }
}
