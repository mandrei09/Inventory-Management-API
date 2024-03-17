using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
using System.Threading.Tasks;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/invstates")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class InvStatesController : GenericApiController<Model.InvState, Dto.InvState>
    {
        public InvStatesController(ApplicationDbContext context, IInvStatesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sync")]
        public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.InvState> items = (_itemsRepository as IInvStatesRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<Dto.InvState>(i));
            var pagedResult = new Dto.PagedResult<Dto.InvState>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }

        [HttpGet]
        [Route("inuse", Order = -1)]
        public virtual async Task<IActionResult> GetInUse(string jsonFilter, string propertyFilters)
        {
            AssetFilter assetFilter = null;
            List<PropertyFilter> propFilters = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();
            propFilters = propertyFilters != null ? JsonConvert.DeserializeObject<List<PropertyFilter>>(propertyFilters) : new List<PropertyFilter>();

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    assetFilter.EmployeeIds = null;
                    assetFilter.EmployeeIds = new List<int?>();
                    assetFilter.EmployeeIds.Add(int.Parse("-1"));
                }
            }
            else
            {
                assetFilter.EmployeeIds = null;
                assetFilter.EmployeeIds = new List<int?>();
                assetFilter.EmployeeIds.Add(int.Parse("-1"));
            }

            var items = (_itemsRepository as IInvStatesRepository)
                .GetInvStatesInUseWithAssets(assetFilter, propFilters).ToList();

            return Ok(items);

        }

		[HttpGet("export")]
		public IActionResult Export(string filter)
		{
			List<Model.InvState> admCenters = null;
			int rowNumber = 0;
			using (ExcelPackage package = new ExcelPackage())
			{
				admCenters = (_itemsRepository as IInvStatesRepository).GetByFilter(filter, null, null, null, null, null).ToList();
				admCenters = admCenters.OrderBy(l => l.Name).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Stari");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod";
				worksheet.Cells[1, 2].Value = "Descriere";


				int recordIndex = 2;
				foreach (var item in admCenters)
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
					FileDownloadName = "Stari.xlsx"
				};

				return result;

			}
		}
	}
}
