﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Route("api/dictionarytypes")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class DictionaryTypesController : GenericApiController<Model.DictionaryType, Dto.DictionaryType>
    {
        public DictionaryTypesController(ApplicationDbContext context, IDictionaryTypesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter,  string includes)
        {
            List<Model.DictionaryType> items = null;
            IEnumerable<Dto.DictionaryType> itemsResult = null;


            items = (_itemsRepository as IDictionaryTypesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.DictionaryType>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IDictionaryTypesRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.DictionaryType>(itemsResult, new Dto.PagingInfo()
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
            List<Model.DictionaryType> items = (_itemsRepository as IDictionaryTypesRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<DictionaryTypeSync>(i));
            var pagedResult = new Dto.PagedResult<DictionaryTypeSync>(itemsResult, new Dto.PagingInfo()
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
			List<Model.DictionaryType> dictionaryTypes = null;
			int rowNumber = 0;
			using (ExcelPackage package = new ExcelPackage())
			{
				dictionaryTypes = (_itemsRepository as IDictionaryTypesRepository).GetByFilters(filter, null, null, null, null, null).ToList();
				dictionaryTypes = dictionaryTypes.OrderBy(l => l.Name).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("tip dictionar");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod";
				worksheet.Cells[1, 2].Value = "Descriere";


				int recordIndex = 2;
				foreach (var item in dictionaryTypes)
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
					FileDownloadName = "tip_dictionar.xlsx"
				};

				return result;

			}
		}
	}
}
