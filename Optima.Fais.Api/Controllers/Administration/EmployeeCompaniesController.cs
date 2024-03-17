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
    [Route("api/employeecompanies")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class EmployeeCompaniesController : GenericApiController<Model.EmployeeCompany, Dto.EmployeeCompany>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public EmployeeCompaniesController(ApplicationDbContext context, IEmployeeCompaniesRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string companyIds, string employeeIds, string includes)
        {
            List<Model.EmployeeCompany> items = null;
            IEnumerable<Dto.EmployeeCompany> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;

			includes = "Employee,Company";


			if (companyIds != null && !companyIds.StartsWith("["))
			{
				companyIds = "[" + companyIds + "]";
			}

			if (employeeIds != null && !employeeIds.StartsWith("["))
			{
                employeeIds = "[" + employeeIds + "]";
			}

			if ((companyIds != null) && (companyIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(companyIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((employeeIds != null) && (employeeIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(employeeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IEmployeeCompaniesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, eIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.EmployeeCompany>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IEmployeeCompaniesRepository).GetCountByFilters(filter, cIds, eIds);
                var pagedResult = new Dto.PagedResult<Dto.EmployeeCompany>(itemsResult, new Dto.PagingInfo()
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
        public async virtual Task<IActionResult> AddEmployeeCompany([FromBody] Dto.EmployeeCompanyAdd empCompanyAdd)
        {
            Model.EmployeeCompany EmployeeCompany = null;
            var user = _context.Set<Model.ApplicationUser>().Where(u => u.EmployeeId == empCompanyAdd.EmployeeId).FirstOrDefault();

            var claims = await _userManager.GetClaimsAsync(user);

            var claim = claims.FirstOrDefault(c => c.Type == "companies");

            var companyClaims = "";

            for (int i = 0; i < empCompanyAdd.CompanyIds.Length; i++)
            {
                var company = _context.Set<Model.Company>().Where(u => u.Id == empCompanyAdd.CompanyIds[i]).FirstOrDefault();


                if (i == empCompanyAdd.CompanyIds.Count() - 1)
                {
                    companyClaims = String.Concat(companyClaims, company.Code);
                }
                else
                {
                    companyClaims = String.Concat(companyClaims, company.Code + "|");
                }


                EmployeeCompany = new Model.EmployeeCompany()
                {
                    EmployeeId = empCompanyAdd.EmployeeId,
                    CompanyId = empCompanyAdd.CompanyIds[i]
                };

                _context.Add(EmployeeCompany);

                _context.SaveChanges();
            }

            return Ok(StatusCode(200));
        }

        [HttpDelete("remove/{id}")]
        public virtual IActionResult DeleteEmployeeCompany(int id)
        {

            Model.EmployeeCompany EmployeeCompany = _context.Set<Model.EmployeeCompany>().Where(a => a.Id == id).Single();

            if (EmployeeCompany != null)
            {

                EmployeeCompany.IsDeleted = true;
                EmployeeCompany.ModifiedAt = DateTime.Now;
                _context.Update(EmployeeCompany);
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
            List<Model.EmployeeCompany> items = (_itemsRepository as IEmployeeCompaniesRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<EmployeeCompanySync>(i));
            var pagedResult = new Dto.PagedResult<EmployeeCompanySync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }
    }
}
