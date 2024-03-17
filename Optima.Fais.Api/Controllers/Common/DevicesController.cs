using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharp;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/devices")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class DevicesController : GenericApiController<Model.Device, Dto.Device>
    {
        public DevicesController(ApplicationDbContext context, IDevicesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string infoTypeIds, string includes)
        {
            List<Model.Device> items = null;
            IEnumerable<Dto.Device> itemsResult = null;
            List<int> cIds = null;

            if (infoTypeIds != null && !infoTypeIds.StartsWith("["))
            {
                infoTypeIds = "[" + infoTypeIds + "]";
            }



            includes = includes ?? "DeviceType,Employee";

            if ((infoTypeIds != null) && (infoTypeIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(infoTypeIds).ToList().Select(int.Parse).ToList();


            items = (_itemsRepository as IDevicesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Device>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IDevicesRepository).GetCountByFilters(filter, cIds);
                var pagedResult = new Dto.PagedResult<Dto.Device>(itemsResult, new Dto.PagingInfo()
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
        [Route("syncmobile")]
        public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.Device> items = (this._itemsRepository as IDevicesRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => this._mapper.Map<Dto.Device>(i));
            var pagedResult = new Dto.PagedResult<Dto.Device>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }

		[HttpGet("export")]
		public IActionResult Export(string filter, string countyIds)
		{
			List<int> rIds = null;
			List<Model.Device> devices = null;
			int rowNumber = 0;
			var includes = "DeviceType";

			if ((countyIds != null) && (countyIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(countyIds).ToList().Select(int.Parse).ToList();
			using (ExcelPackage package = new ExcelPackage())
			{
				devices = (_itemsRepository as IDevicesRepository).GetByFilters(filter, includes, null, null, null, null, null).ToList();
				devices = devices.OrderBy(l => l.Name).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("dispozitive");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod";
				worksheet.Cells[1, 2].Value = "Descriere";
				worksheet.Cells[1, 3].Value = "Serie";


				int recordIndex = 2;
				foreach (var item in devices)
				{
					rowNumber++;
					worksheet.Cells[recordIndex, 1].Value = item.Code;
					worksheet.Cells[recordIndex, 2].Value = item.Name;
					worksheet.Cells[recordIndex, 3].Value = item.Serial;



					recordIndex++;
				}

				worksheet.View.FreezePanes(2, 1);

				worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();
				worksheet.Column(3).AutoFit();



				using (var cells = worksheet.Cells[1, 1, 1, 3])
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
					FileDownloadName = "dispozitive.xlsx"
				};

				return result;

			}
		}
	}
}
