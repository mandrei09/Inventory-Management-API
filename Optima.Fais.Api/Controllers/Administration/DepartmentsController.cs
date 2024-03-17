using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharp;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/departments")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class DepartmentsController : GenericApiController<Model.Department, Dto.Department>
    {
        public DepartmentsController(ApplicationDbContext context, IDepartmentsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("details")]
        public virtual IActionResult GetDetails(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, int? teamLeaderId)
        {
            int totalItems = 0;

            List<Dto.DepartmentDetail> items = (_itemsRepository as IDepartmentsRepository).GetDetailsByFilters(teamLeaderId, filter, null, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var result = new PagedResult<Dto.DepartmentDetail>(items, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sync")]
        public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.Department> items = (_itemsRepository as IDepartmentsRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<DepartmentSync>(i));
            var pagedResult = new Dto.PagedResult<DepartmentSync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }

		[HttpGet("export")]
		public IActionResult Export(string filter, string uomIds)
		{
			List<int?> aIds = null;
			List<Model.Department> budgetManagers = null;
			int totalItems = 0;

			if ((uomIds != null) && (uomIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(uomIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

			using (OfficeOpenXml.ExcelPackage package = new ExcelPackage())
			{
				budgetManagers = (_itemsRepository as IDepartmentsRepository).GetAll(null, null, null).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("B.U.");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Id";
				worksheet.Cells[1, 2].Value = "Code";
				worksheet.Cells[1, 3].Value = "Name";

				int recordIndex = 2;
				foreach (var item in budgetManagers)
				{
					worksheet.Cells[recordIndex, 1].Value = item.Id;
					worksheet.Cells[recordIndex, 2].Value = item.Code;
					worksheet.Cells[recordIndex, 3].Value = item.Name;
					recordIndex++;
				}

				worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();
				worksheet.Column(3).AutoFit();

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
					FileDownloadName = "B.U..xlsx"
				};

				return result;

			}
		}
	}
}
