using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/accmonths")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AccMonthsController : GenericApiController<Model.AccMonth, Dto.AccMonth>
    {
        public AccMonthsController(ApplicationDbContext context, IAccMonthsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet("filtered")]
        public IActionResult GetAccMonth(int month, int year)
        {
            Model.AccMonth item = (_itemsRepository as IAccMonthsRepository).GetAccMonth(month, year);

            return Ok(_mapper.Map<Dto.AccMonth>(item));
        }

        [HttpPost("save")]
        public virtual IActionResult PostDetail([FromBody] AccMonth accMonth)
        {
            int accMonthId = 0;

            if(accMonth != null && accMonth.IsActive == true)
			{
                accMonthId = (_itemsRepository as IAccMonthsRepository).CreateAccMonth(accMonth);

                if (accMonthId > 0)
                {
                    var count = _context.Set<Model.RecordCount>().FromSql("AddNewAccMonth {0}", accMonthId).ToList();
                }
            }

            return Ok(accMonthId);
           
        }

		[HttpGet("export")]
		public IActionResult Export(string filter)
		{
			List<Model.AccMonth> accMonths = null;

			using (OfficeOpenXml.ExcelPackage package = new ExcelPackage())
			{
				accMonths = (_itemsRepository as IAccMonthsRepository).GetAll(null,null,null).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Luni contabile");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Luna";
				worksheet.Cells[1, 2].Value = "An";

				int recordIndex = 2;
				foreach (var item in accMonths)
				{
					worksheet.Cells[recordIndex, 1].Value = item.Month;
					worksheet.Cells[recordIndex, 2].Value = item.Year;

					recordIndex++;
				}

				worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();

				using (var cells = worksheet.Cells[1, 1, 1, 2])
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
					FileDownloadName = "luni_contabile.xlsx"
				};

				return result;

			}
		}
	}
}
