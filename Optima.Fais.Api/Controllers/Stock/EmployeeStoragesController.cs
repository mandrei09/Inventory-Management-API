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
    [Route("api/employeestorages")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class EmployeeStoragesController : GenericApiController<Model.EmployeeStorage, Dto.EmployeeStorage>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public EmployeeStoragesController(ApplicationDbContext context, IEmployeeStoragesRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string storageIds, string employeeIds, string includes)
        {
            List<Model.EmployeeStorage> items = null;
            IEnumerable<Dto.EmployeeStorage> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;

			includes = "Employee,Storage";


			if (storageIds != null && !storageIds.StartsWith("["))
			{
                storageIds = "[" + storageIds + "]";
			}

			if (employeeIds != null && !employeeIds.StartsWith("["))
			{
                employeeIds = "[" + employeeIds + "]";
			}

			if ((storageIds != null) && (storageIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(storageIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((employeeIds != null) && (employeeIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(employeeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IEmployeeStoragesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, eIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.EmployeeStorage>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IEmployeeStoragesRepository).GetCountByFilters(filter, cIds, eIds);
                var pagedResult = new Dto.PagedResult<Dto.EmployeeStorage>(itemsResult, new Dto.PagingInfo()
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
        public async virtual Task<IActionResult> AddEmployeeStorage([FromBody] Dto.EmployeeStorageAdd empStorageAdd)
        {
            Model.EmployeeStorage employeeStorage = null;
            Model.Storage storage = null;
            var user = await _context.Set<Model.ApplicationUser>().Where(u => u.EmployeeId == empStorageAdd.EmployeeId).FirstOrDefaultAsync();

            for (int i = 0; i < empStorageAdd.StorageIds.Length; i++)
            {
                storage = await _context.Set<Model.Storage>().Where(u => u.Id == empStorageAdd.StorageIds[i]).FirstOrDefaultAsync();

                employeeStorage = new Model.EmployeeStorage()
                {
                    EmployeeId = empStorageAdd.EmployeeId,
                    StorageId = empStorageAdd.StorageIds[i]
                };

                _context.Add(employeeStorage);

                _context.SaveChanges();
            }

            return Ok(StatusCode(200));
        }

        [HttpDelete("remove/{id}")]
        public virtual IActionResult DeleteEmployeeDivision(int id)
        {

            Model.EmployeeStorage employeeStorage = _context.Set<Model.EmployeeStorage>().Where(a => a.Id == id).Single();

            if (employeeStorage != null)
            {

                employeeStorage.IsDeleted = true;
                employeeStorage.ModifiedAt = DateTime.Now;
                _context.Update(employeeStorage);
                _context.SaveChanges();
            }
            else
            {
                return Ok(StatusCode(404));
            }

            return Ok(StatusCode(200));
        }
    }
}
