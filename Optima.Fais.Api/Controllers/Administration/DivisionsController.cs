using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharp;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/divisions")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class DivisionsController : GenericApiController<Model.Division, Dto.Division>
    {
        public DivisionsController(ApplicationDbContext context, IDivisionsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string departmentIds, string locationIds,  string includes, bool showAll = false)
        {
            List<Model.Division> items = null;
            IEnumerable<Dto.Division> itemsResult = null;
            List<int?> dIds = null;
            List<int?> divIds = null;
            List<int?> lIds = null;
            string userName = string.Empty;
            string role = string.Empty;
            string employeeId = string.Empty;

            includes = includes ?? "Department,Location";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                if (role != "administrator" && !showAll)
                {
                    divIds = _context.Set<Model.EmployeeCostCenter>().Include(c => c.CostCenter).AsNoTracking().Where(l => l.EmployeeId == int.Parse(employeeId) && l.IsDeleted == false).Select(e => e.CostCenter.DivisionId).ToList();
                }
            }

			if (departmentIds != null && !departmentIds.StartsWith("["))
			{
				departmentIds = "[" + departmentIds + "]";
			}

			if ((departmentIds != null) && (departmentIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(departmentIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((locationIds != null) && (locationIds.Length > 0)) lIds = JsonConvert.DeserializeObject<string[]>(locationIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IDivisionsRepository).GetByFilters(filter, dIds, divIds, lIds, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Division>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IDivisionsRepository).GetCountByFilters(filter, dIds, divIds, lIds);
                var pagedResult = new Dto.PagedResult<Dto.Division>(itemsResult, new Dto.PagingInfo()
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

        [HttpGet]
        [Route("dashboard", Order = -1)]
        public virtual IActionResult GetDashboard(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string departmentIds, string locationIds, string includes,string jsonFilter)
        {
            List<Model.Division> items = null;
            IEnumerable<Dto.Division> itemsResult = null;
            DashboardFilter dashboardFilter = null;
            //List<int?> dIds = null;
            List<int?> divIds = null;
            List<int?> lIds = null;
            string userName = string.Empty;
            string role = string.Empty;
            string employeeId = string.Empty;

            includes = includes ?? "Department,Location";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                if (role != "administrator")
                {
                    divIds = _context.Set<Model.EmployeeCostCenter>().Include(c => c.CostCenter).AsNoTracking().Where(l => l.EmployeeId == int.Parse(employeeId) && l.IsDeleted == false).Select(e => e.CostCenter.DivisionId).ToList();
                }
            }

            dashboardFilter = jsonFilter != null ? JsonConvert.DeserializeObject<DashboardFilter>(jsonFilter) : new DashboardFilter();

            //if ((departmentIds != null) && (departmentIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(departmentIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((locationIds != null) && (locationIds.Length > 0)) lIds = JsonConvert.DeserializeObject<string[]>(locationIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IDivisionsRepository).GetByFilters(filter, dashboardFilter.DepartmentIds, divIds, lIds, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Division>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IDivisionsRepository).GetCountByFilters(filter, dashboardFilter.DepartmentIds, divIds, lIds);
                var pagedResult = new Dto.PagedResult<Dto.Division>(itemsResult, new Dto.PagingInfo()
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
        [Route("sync")]
        public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.Division> items = (_itemsRepository as IDivisionsRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<DivisionSync>(i));
            var pagedResult = new Dto.PagedResult<DivisionSync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }

		[HttpGet("export")]
		public IActionResult Export(string filter)
		{
			List<Model.Division> divisions = null;
			int rowNumber = 0;
			using (ExcelPackage package = new ExcelPackage())
			{
				divisions = (_itemsRepository as IDivisionsRepository).GetByFilters(filter, null, null, null, null, null, null, null, null).ToList();
				divisions = divisions.OrderBy(l => l.Name).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Departamente");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod";
				worksheet.Cells[1, 2].Value = "Descriere";


				int recordIndex = 2;
				foreach (var item in divisions)
				{
					rowNumber++;
					worksheet.Cells[recordIndex, 1].Value = item.Code;
					worksheet.Cells[recordIndex, 2].Value = item.Name;



					recordIndex++;
				}

				worksheet.View.FreezePanes(2, 1);

				worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();



				using (var cells = worksheet.Cells[1, 1, 1, 2])
				{
					cells.Style.Font.Bold = true;
					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
					cells.Style.Fill.BackgroundColor.SetColor(Color.Red);
				}

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				//HttpContext.Response.ContentType = entityFile.FileType;
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "Locatii.xlsx"
				};

				return result;

			}
		}
	}
}
