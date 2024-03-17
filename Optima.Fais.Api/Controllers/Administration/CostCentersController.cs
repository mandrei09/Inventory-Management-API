using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optima.Fais.Api.Services;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/costcenters")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class CostCentersController : GenericApiController<Model.CostCenter, Dto.CostCenter>
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly INotifyService _notifyService = null;
		private readonly UserManager<Model.ApplicationUser> _userManager;

		public CostCentersController(ApplicationDbContext context, ICostCentersRepository itemsRepository, IMapper mapper, IHostingEnvironment hostingEnvironment, INotifyService notifyService, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            this._notifyService = notifyService;
			this._userManager = userManager;
		}

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string administrationIds, string admCenterIds, string divisionIds, string locationIds, string departmentIds, string exceptCostCenterIds, string includes, bool fromStock = false)
        {
            List<Model.CostCenter> items = null;
            IEnumerable<Dto.CostCenter> itemsResult = null;
            List<int> aIds = null;
			List<int> admIds = null;
			List<int> cIds = null;
            List<int?> divIds = null;
            List<int?> depIds = null;
            List<int> locIds = null;
            List<int?> exceptIds = null;
            string userName = string.Empty;
            string role = string.Empty;
            string employeeId = string.Empty;

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                //if (role != "administrator")
                //{
                //    cIds = _context.Set<Model.EmployeeCostCenter>().AsNoTracking().Where(l => l.EmployeeId == int.Parse(employeeId) && l.IsDeleted == false).Select(e => e.CostCenterId).ToList();
                //}
            }

            if (admCenterIds != null && !admCenterIds.StartsWith("["))
            {
                admCenterIds = "[" + admCenterIds + "]";
            }

			if (administrationIds != null && !administrationIds.StartsWith("["))
			{
				administrationIds = "[" + administrationIds + "]";
			}

			if (divisionIds != null && !divisionIds.StartsWith("["))
            {
                divisionIds = "[" + divisionIds + "]";
            }

            includes = includes ??  "Division.Department,AdmCenter,Region,Room,Administration,Storage,Company";

            if ((admCenterIds != null) && (admCenterIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();
			if ((administrationIds != null) && (administrationIds.Length > 0)) admIds = JsonConvert.DeserializeObject<string[]>(administrationIds).ToList().Select(int.Parse).ToList();
			if ((divisionIds != null) && (divisionIds.Length > 0)) divIds = JsonConvert.DeserializeObject<string[]>(divisionIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((departmentIds != null) && (departmentIds.Length > 0)) depIds = JsonConvert.DeserializeObject<string[]>(departmentIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((locationIds != null) && (locationIds.Length > 0)) locIds = JsonConvert.DeserializeObject<string[]>(locationIds).ToList().Select(int.Parse).ToList();
            if ((exceptCostCenterIds != null) && (exceptCostCenterIds.Length > 0)) exceptIds = JsonConvert.DeserializeObject<string[]>(exceptCostCenterIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as ICostCentersRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, admIds, aIds, divIds, depIds, locIds, cIds, exceptIds, fromStock).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.CostCenter>(i));

            // this._notifyService.NotifyDataCreateAssetAsync(new CreateAssetSAPResult() { Success = true, ErrorMessage = "OK"});

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as ICostCentersRepository).GetCountByFilters(filter, admIds, aIds, divIds, depIds, locIds, cIds, exceptIds, fromStock);
                var pagedResult = new Dto.PagedResult<Dto.CostCenter>(itemsResult, new Dto.PagingInfo()
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
        public virtual IActionResult GetDashboard(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string administrationIds, string admCenterIds, string divisionIds, string locationIds, string departmentIds, string exceptCostCenterIds, string includes, string jsonFilter)
        {
            List<Model.CostCenter> items = null;
            IEnumerable<Dto.CostCenter> itemsResult = null;
            DashboardFilter dashboardFilter = null;
            List<int> aIds = null;
			List<int> admIds = null;
			List<int> cIds = null;
            //List<int> divIds = null;
            //List<int> depIds = null;
            List<int> locIds = null;
            List<int?> exceptIds = null;
            string userName = string.Empty;
            string role = string.Empty;
            string employeeId = string.Empty;

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                if (role != "administrator")
                {
                    cIds = _context.Set<Model.EmployeeCostCenter>().AsNoTracking().Where(l => l.EmployeeId == int.Parse(employeeId) && l.IsDeleted == false).Select(e => e.CostCenterId).ToList();
                }
            }

            if (admCenterIds != null && !admCenterIds.StartsWith("["))
            {
                admCenterIds = "[" + admCenterIds + "]";
            }

			if (administrationIds != null && !administrationIds.StartsWith("["))
			{
				administrationIds = "[" + administrationIds + "]";
			}

			dashboardFilter = jsonFilter != null ? JsonConvert.DeserializeObject<DashboardFilter>(jsonFilter) : new DashboardFilter();

            if ((admCenterIds != null) && (admCenterIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();
			if ((admCenterIds != null) && (admCenterIds.Length > 0)) admIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();
			//if ((divisionIds != null) && (divisionIds.Length > 0)) divIds = JsonConvert.DeserializeObject<string[]>(divisionIds).ToList().Select(int.Parse).ToList();
			//if ((departmentIds != null) && (departmentIds.Length > 0)) depIds = JsonConvert.DeserializeObject<string[]>(departmentIds).ToList().Select(int.Parse).ToList();
			if ((locationIds != null) && (locationIds.Length > 0)) locIds = JsonConvert.DeserializeObject<string[]>(locationIds).ToList().Select(int.Parse).ToList();
            if ((exceptCostCenterIds != null) && (exceptCostCenterIds.Length > 0)) exceptIds = JsonConvert.DeserializeObject<string[]>(exceptCostCenterIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as ICostCentersRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize,admIds, aIds, dashboardFilter.DivisionIds, dashboardFilter.DepartmentIds, locIds, cIds, exceptIds, false).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.CostCenter>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as ICostCentersRepository).GetCountByFilters(filter, admIds, aIds, dashboardFilter.DivisionIds, dashboardFilter.DepartmentIds, locIds, cIds, exceptIds, false);
                var pagedResult = new Dto.PagedResult<Dto.CostCenter>(itemsResult, new Dto.PagingInfo()
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
        public IActionResult Export(string filter, string administrationIds, string admCenterIds)
        {
            List<int> aIds = null;
			List<int> admIds = null;
			List<Model.CostCenter> costCenters = null;

            if ((admCenterIds != null) && (admCenterIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();
			if ((administrationIds != null) && (administrationIds.Length > 0)) admIds = JsonConvert.DeserializeObject<string[]>(administrationIds).ToList().Select(int.Parse).ToList();

			using (ExcelPackage package = new ExcelPackage())
            {
                costCenters = (_itemsRepository as ICostCentersRepository).GetByFilters(filter, ",Division.Department,Administration,Region,AdmCenter", null, null, null, null, admIds, aIds, null, null, null, null, null, false).ToList();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("cost_centers");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Cod CC";
                worksheet.Cells[1, 2].Value = "Descriere CC";
				worksheet.Cells[1, 3].Value = "Cod Departament";
				worksheet.Cells[1, 4].Value = "Departament";
				worksheet.Cells[1, 5].Value = "Cod B.U.";
				worksheet.Cells[1, 6].Value = "B.U.";
				worksheet.Cells[1, 7].Value = "Locatie";
				worksheet.Cells[1, 8].Value = "Profit center";
				worksheet.Cells[1, 9].Value = "PC Detaliu";

				int recordIndex = 2;
                foreach (var item in costCenters)
                {
                    worksheet.Cells[recordIndex, 1].Value = item.Code;
                    worksheet.Cells[recordIndex, 2].Value = item.Name;
                    worksheet.Cells[recordIndex, 3].Value = item.Division != null ? item.Division.Code : "";
					worksheet.Cells[recordIndex, 4].Value = item.Division != null ? item.Division.Name : "";
					worksheet.Cells[recordIndex, 5].Value = item.Division != null && item.Division.Department != null ? item.Division.Department.Code : "";
					worksheet.Cells[recordIndex, 6].Value = item.Division != null && item.Division.Department != null ? item.Division.Department.Name : "";
					worksheet.Cells[recordIndex, 7].Value = item.Administration != null ? item.Administration.Name : "";
					worksheet.Cells[recordIndex, 8].Value = item.AdmCenter != null ? item.AdmCenter.Name : "";
					worksheet.Cells[recordIndex, 9].Value = item.Region != null ? item.Region.Name : "";
					recordIndex++;
                }

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
				worksheet.Column(4).AutoFit();
				worksheet.Column(5).AutoFit();
				worksheet.Column(6).AutoFit();
				worksheet.Column(7).AutoFit();
				worksheet.Column(8).AutoFit();
				worksheet.Column(9).AutoFit();

				using (var cells = worksheet.Cells[1, 1, 1, 9])
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
                    FileDownloadName = "cost_centers.xlsx"
                };

                return result;

            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sync")]
        public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.CostCenter> items = (_itemsRepository as ICostCentersRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<CostCenterSync>(i));
            var pagedResult = new Dto.PagedResult<CostCenterSync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }

		[Authorize]
		[HttpPost]
		[Route("import")]
		public async virtual Task<ImportCostCenterResult> Import([FromBody] CostCenterImport ccImport)
		{
			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await _userManager.FindByEmailAsync(userName);

				if (user == null)
				{
					user = await _userManager.FindByNameAsync(userName);
				}

				_context.UserId = user.Id.ToString();

                if (ccImport.CurrentIndex == 1)
                {
					var IdCostCenter = await _context.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "CostCenter").SingleAsync();
					var IdAdmCenter = await _context.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "AdmCenter").SingleAsync();
					var IdRegion = await _context.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Region").SingleAsync();
					var IdCompany = await _context.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Company").SingleAsync();
				}

                var result = await (_itemsRepository as ICostCentersRepository).Import(ccImport);

				

				return new Model.ImportCostCenterResult { Success = result.Success, Message = result.Message, Id = result.Id };
			}
			else
			{
				return new Model.ImportCostCenterResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}
		}

		//[HttpPut]
		//[Route("", Order = -1)]
		//public void PutCustom([FromBody] Dto.CostCenter vm)
		//{
		//	Model.CostCenter costCenter = new Model.CostCenter
		//	{
		//		Id = vm.Id,
		//		Code = vm.Code,
		//		Name = vm.Name,
		//		IsFinished = vm.IsFinished,
  //              DateFinished = DateTime.Now,
  //              AdmCenterId = vm.AdmCenterId,
  //              RegionId = vm.RegionId,
  //              CompanyId = vm.Company != null ? vm.Company.Id : null,
  //              DivisionId = vm.DivisionId,
  //              RoomId = vm.RoomId,
                
		//	};

		//	_itemsRepository.Update(costCenter);

		//	if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
		//	_context.SaveChanges();
		//}
	}
}
