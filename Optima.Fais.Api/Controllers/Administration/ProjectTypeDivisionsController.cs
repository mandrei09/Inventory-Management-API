using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharp;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/projecttypedivisions")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class ProjectTypeDivisionsController : GenericApiController<Model.ProjectTypeDivision, Dto.ProjectTypeDivision>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public ProjectTypeDivisionsController(ApplicationDbContext context, IProjectTypeDivisionsRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public async virtual Task<IActionResult> Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string projectTypeIds, string divIds, string includes)
        {
            List<Model.ProjectTypeDivision> items = null;
            IEnumerable<Dto.ProjectTypeDivision> itemsResult = null;
            List<int?> cIds = null;
            List<int> divisionIds = null;
			List<int?> dIds = null;
			string role = string.Empty;

            var userName = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(userName);
            if(user == null)
			{
                user = await _userManager.FindByNameAsync(userName);
            }

            role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;

            includes = "ProjectType,Division.Department";

			if (divIds != null && !divIds.StartsWith("["))
			{
				divIds = "[" + divIds + "]";
			}

			if (!user.InInventory)
			{
                if(role.ToUpper() != "ADMINISTRATOR")
                {
					if (role.ToUpper() == "PROCUREMENT")
					{
						divisionIds = _context.Set<Model.Division>().AsNoTracking().Where(e => e.Id != 1482 && e.IsDeleted == false).Select(c => c.Id).ToList();
					}
					else if (role.ToUpper() == "PROC-IT")
					{
						divisionIds = _context.Set<Model.Division>().AsNoTracking().Where(e => e.Id == 1482 && e.IsDeleted == false).Select(c => c.Id).ToList();
					}
					else
					{
						divisionIds = await _context.Set<Model.EmployeeDivision>()
					   .Where(a => a.EmployeeId == user.EmployeeId && a.IsDeleted == false)
					   .Select(a => a.DivisionId)
					   .ToListAsync();
					}
				}
                
			}
			else
			{
				divisionIds = await _context.Set<Model.EmployeeCostCenter>().Where(a => a.EmployeeId == user.EmployeeId).Select(a => a.CostCenter != null && a.CostCenter.DivisionId != null ? a.CostCenter.DivisionId.Value : 0).ToListAsync();
			}


			if (projectTypeIds != null && !projectTypeIds.StartsWith("["))
			{
                projectTypeIds = "[" + projectTypeIds + "]";
			}

			if ((projectTypeIds != null) && (projectTypeIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(projectTypeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((divIds != null) && (divIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(divIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

			items = (_itemsRepository as IProjectTypeDivisionsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, divisionIds, dIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.ProjectTypeDivision>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IProjectTypeDivisionsRepository).GetCountByFilters(filter, cIds, divisionIds, dIds);
                var pagedResult = new Dto.PagedResult<Dto.ProjectTypeDivision>(itemsResult, new Dto.PagingInfo()
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

		[HttpGet("export")]
		public IActionResult Export(string filter)
		{
			List<int?> aIds = null;
			List<Model.ProjectTypeDivision> projectTypeDivisions = null;
			using (ExcelPackage package = new ExcelPackage())
			{
				projectTypeDivisions = (_itemsRepository as IProjectTypeDivisionsRepository).GetByFilters(filter, "ProjectType,Division", null, null, null, null, null, null, null).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Project-Type-Division");
				//First add the headers

				worksheet.Cells[1, 1].Value = "Code Proiect";
				worksheet.Cells[1, 2].Value = "Proiect";
				worksheet.Cells[1, 3].Value = "Cod Departament";
				worksheet.Cells[1, 4].Value = "Departament";

				int recordIndex = 2;
				foreach (var item in projectTypeDivisions)
				{
					worksheet.Cells[recordIndex, 1].Value = item.ProjectType != null ? item.ProjectType.Code : "";
					worksheet.Cells[recordIndex, 2].Value = item.ProjectType != null ? item.ProjectType.Name : "";
					worksheet.Cells[recordIndex, 3].Value = item.Division != null ? item.Division.Code : "";
					worksheet.Cells[recordIndex, 4].Value = item.Division != null ? item.Division.Name : "";
					recordIndex++;
				}

				worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();
				worksheet.Column(3).AutoFit();
				worksheet.Column(4).AutoFit();

				using (var cells = worksheet.Cells[1, 1, 1, 4])
				{
					cells.Style.Font.Bold = true;
					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
					cells.Style.Fill.BackgroundColor.SetColor(Color.Aqua);
				}

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				//HttpContext.Response.ContentType = entityFile.FileType;
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "project-type-division.xlsx"
				};

				return result;

			}
		}
	}
}
