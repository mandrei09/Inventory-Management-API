using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/regions")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class RegionsController : GenericApiController<Model.Region, Dto.Region>
    {
        public RegionsController(ApplicationDbContext context, IRegionsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("sync")]
        public virtual IActionResult GetSyncDetails(int pageSize, int lastId, DateTime lastModifiedAt)
        {
            List<Model.Region> items = (_itemsRepository as IRegionsRepository).GetSyncDetails(pageSize, lastId, lastModifiedAt).ToList();

            return Ok(items.Select(i => _mapper.Map<Dto.Region>(i)));
        }

		[HttpGet("export")]
		public IActionResult Export(string filter)
		{
			List<Model.Region> regions = null;
			int rowNumber = 0;
			using (ExcelPackage package = new ExcelPackage())
			{
				regions = (_itemsRepository as IRegionsRepository).GetAll(null, null, null).ToList();
				regions = regions.OrderBy(l => l.Name).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PC Det");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod";
				worksheet.Cells[1, 2].Value = "Descriere";


				int recordIndex = 2;
				foreach (var item in regions)
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
					FileDownloadName = "PC_DET.xlsx"
				};

				return result;

			}
		}
	}
}
