using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;
using Optima.Fais.Api.Services;
using Optima.Fais.Data;
using Optima.Fais.EfRepository;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/inventories")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class InventoryController : GenericApiController<Model.Inventory, Dto.Inventory>
    {
        IInventoryService _inventoryService = null;

        public InventoryController(ApplicationDbContext context, IInventoryRepository itemsRepository, IMapper mapper, IInventoryService inventoryService)
            : base(context, itemsRepository, mapper)
        {
            this._inventoryService = inventoryService;
        }

        [HttpGet]
        [Route("sync")]
        public virtual IActionResult GetSyncDetails(int pageSize, int lastId, System.DateTime lastModifiedAt)
        {
            List<Model.Inventory> items = (_itemsRepository as IInventoryRepository).GetSyncDetails(pageSize, lastId, lastModifiedAt).ToList();

            return Ok(items.Select(i => _mapper.Map<Dto.Inventory>(i)));
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string accMonthIds, string includes)
        {
            List<Model.Inventory> items = null;
            IEnumerable<Dto.Inventory> itemsResult = null;
            List<int> aIds = null;


            if (includes == null)
            {
                includes = "AccMonth,AccMonthBudget,BudgetManager";
            }

			includes += ",AccMonth,AccMonthBudget,BudgetManager";
           
            if ((accMonthIds != null) && (accMonthIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(accMonthIds).ToList().Select(int.Parse).ToList();

            items = (_itemsRepository as IInventoryRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, aIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Inventory>(i));

            //var count = _context.Set<Model.RecordCount>().FromSql("UpdateInventoryAssetScans").ToList();

            //if (HttpContext.User.Identity.Name != null)
            //{
            //    string userName = HttpContext.User.Identity.Name;
            //    string role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
            //    string employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

            //    var count = _context.Set<Model.RecordCount>().FromSql("UpdateBadgeCount {0}, {1}", role, employeeId).ToList();
            //}

            

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IInventoryRepository).GetCountByFilters(filter, aIds);
                var pagedResult = new Dto.PagedResult<Dto.Inventory>(itemsResult, new Dto.PagingInfo()
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
        [Route("", Order = -1)]
        public IActionResult AddInventory([FromBody] Dto.Inventory inventory)
        {
            Model.Inventory oldInv = _context.Set<Model.Inventory>().Where(i => i.Active == true).FirstOrDefault();

            if(oldInv != null)
            {
                oldInv.Active = false;
                _context.Update(oldInv);
            }
           

            Model.Document document = new Model.Document
            {
                Approved = true,
                DocumentTypeId = 5,
                DocNo1 = string.Empty,
                DocNo2 = string.Empty,
                DocumentDate = DateTime.Now,
                RegisterDate = DateTime.Now,
                PartnerId = null,
                CreationDate = DateTime.Now

            };

            _context.Add(document);

            Model.Inventory inv = new Model.Inventory
            {
                Active = inventory.Active,
                AdministrationId = null,
                CompanyId = null,
                CostCenterId = null,
                CreatedAt = DateTime.Now,
                CreatedBy = "",
                Description = inventory.Description,
                DocumentId = document.Id,
                EmployeeId = null,
                End = inventory.End,
                IsDeleted = false,
                ModifiedAt = inventory.ModifiedAt,
                ModifiedBy = string.Empty,
                RoomId = null,
                Start = inventory.Start,
                AccMonthId = inventory.AccMonth.Id
            };

            _itemsRepository.Create(inv);

            _context.SaveChanges();

            _context.Set<Model.Inventory>().FromSql("AddNewInventory").ToList();

            return Ok(inv);
        }

        [HttpGet("audit")]
        public IActionResult Export(int inventoryId, int? admCenterId)
        {

            //List<Model.AuditInventoryV1T1> items = _context.Set<Model.AuditInventoryV1T1>().FromSql("CustomReport_V1_AuditInventory1 {0}, {1}", inventoryId, admCenterId).ToList();
              List<Model.AuditInventoryV1T1> items = _context.Set<Model.AuditInventoryV1T1>().FromSql("CustomReport_V1_Audit {0}", inventoryId).ToList();

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("audit");
                worksheet.Cells[1, 1].Value = "Judet";
                worksheet.Cells[1, 2].Value = "Localitate";
                worksheet.Cells[1, 3].Value = "Scanate";
                worksheet.Cells[1, 4].Value = "Nescanate";
                worksheet.Cells[1, 5].Value = "Minusuri";
                worksheet.Cells[1, 6].Value = "Plusuri";
                worksheet.Cells[1, 7].Value = "Etichete temporare";
                worksheet.Cells[1, 8].Value = "Transferuri in localitate";
                worksheet.Cells[1, 9].Value = "Transferuri in judet";
                worksheet.Cells[1, 10].Value = "Transferuri intre judete";

                int recordIndex = 2;
                foreach (var item in items)
                {
                    worksheet.Cells[recordIndex, 1].Value = item.Region;
                    worksheet.Cells[recordIndex, 2].Value = item.Location;
                    //worksheet.Cells[recordIndex, 3].Value = (item.Items + item.Temporary);
                    worksheet.Cells[recordIndex, 3].Value = (item.Items);
                    worksheet.Cells[recordIndex, 4].Value = item.NotScanned;
                    worksheet.Cells[recordIndex, 5].Value = item.Minus;
                    //worksheet.Cells[recordIndex, 6].Value = (item.Plus + item.Temporary);
                    worksheet.Cells[recordIndex, 6].Value = (item.Plus);
                    worksheet.Cells[recordIndex, 7].Value = item.Temporary;
                    worksheet.Cells[recordIndex, 8].Value = item.TranInLocation;
                    worksheet.Cells[recordIndex, 9].Value = item.TranInAdmCenter;
                    worksheet.Cells[recordIndex, 10].Value = item.TranBetweenAdmCenters;
                    recordIndex++;
                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Audit.xlsx"
                };

                return result;

            }
        }

        [HttpGet("previewa")]
        public async Task<IActionResult> PreviewA(int inventoryId, string jsonFilter, DateTime? inventoryDateEnd)
        {
			ReportFilter reportFilter = null;
            Model.CostCenter costCenter = null;
			Model.Division division = null;
			Model.Department department = null;
			Model.Administration administration = null;
            FileContentResult result = null;

			reportFilter = jsonFilter != null ? JsonConvert.DeserializeObject<ReportFilter>(jsonFilter) : new ReportFilter();

			var ms = await this._inventoryService.PreviewAppendixAAsync(inventoryId, reportFilter, inventoryDateEnd);

			if(ms == null) {
				return null;
			}

            var fileType = "application/octet-stream";
			HttpContext.Response.ContentType = fileType;

			if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0))
			{
				administration = await _context.Set<Model.Administration>().Where(a => a.Id == reportFilter.AdministrationIds[0]).SingleAsync();

                result = new FileContentResult(ms.ToArray(), fileType)
                {
                    FileDownloadName = administration.Name + "_Lista_inventar.pdf"
				};
			}
            else
            {
				if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0))
				{
					costCenter = await _context.Set<Model.CostCenter>().Where(a => a.Id == reportFilter.CostCenterIds[0]).SingleAsync();

					result = new FileContentResult(ms.ToArray(), fileType)
					{
						FileDownloadName = costCenter.Code + "_Lista_inventar.pdf"
					};
				}
			}

            return result;
        }

		[HttpGet("previewaminus")]
		public async Task<IActionResult> PreviewAMinus(int inventoryId, string jsonFilter, DateTime? inventoryDateEnd)
		{
			ReportFilter reportFilter = null;
			Model.CostCenter costCenter = null;
			Model.Division division = null;
			Model.Department department = null;
			Model.Administration administration = null;
			FileContentResult result = null;

			reportFilter = jsonFilter != null ? JsonConvert.DeserializeObject<ReportFilter>(jsonFilter) : new ReportFilter();

			var ms = await this._inventoryService.PreviewAppendixAMinusAsync(inventoryId, reportFilter, inventoryDateEnd);

            if (ms == null)
            {
                return null;
            }

            var fileType = "application/octet-stream";
			HttpContext.Response.ContentType = fileType;

			if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0))
			{
				administration = await _context.Set<Model.Administration>().Where(a => a.Id == reportFilter.AdministrationIds[0]).SingleAsync();

				result = new FileContentResult(ms.ToArray(), fileType)
				{
					FileDownloadName = administration.Name + "_Lista_inventar_minusuri.pdf"
				};
			}
			else
			{
				if ((reportFilter.DepartmentIds != null) && (reportFilter.DepartmentIds.Count > 0))
				{
					department = await _context.Set<Model.Department>().Where(a => a.Id == reportFilter.DepartmentIds[0]).SingleAsync();

					result = new FileContentResult(ms.ToArray(), fileType)
					{
						FileDownloadName = department.Name + "_Lista_inventar_minusuri.pdf"
					};
				}
				else
				{
					if ((reportFilter.DivisionIds != null) && (reportFilter.DivisionIds.Count > 0))
					{
						division = await _context.Set<Model.Division>().Where(a => a.Id == reportFilter.DivisionIds[0]).SingleAsync();

						result = new FileContentResult(ms.ToArray(), fileType)
						{
							FileDownloadName = division.Name + "_Lista_inventar_minusuri.pdf"
						};
					}
					else
					{
						if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0))
						{
							costCenter = await _context.Set<Model.CostCenter>().Where(a => a.Id == reportFilter.CostCenterIds[0]).SingleAsync();

							result = new FileContentResult(ms.ToArray(), fileType)
							{
								FileDownloadName = costCenter.Code + "_Lista_inventar_minusuri.pdf"
							};
						}

					}
				}
			}

			return result;
		}

		[HttpGet("previewaplus")]
		public async Task<IActionResult> PreviewAPlus(int inventoryId, string jsonFilter, DateTime? inventoryDateEnd)
		{
			ReportFilter reportFilter = null;
			Model.CostCenter costCenter = null;
			Model.Division division = null;
			Model.Department department = null;
			Model.Administration administration = null;
			FileContentResult result = null;

			reportFilter = jsonFilter != null ? JsonConvert.DeserializeObject<ReportFilter>(jsonFilter) : new ReportFilter();

			var ms = await this._inventoryService.PreviewAppendixAPlusAsync(inventoryId, reportFilter, inventoryDateEnd);

            if (ms == null)
            {
                return null;
            }

            var fileType = "application/octet-stream";
			HttpContext.Response.ContentType = fileType;

			if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0))
			{
				administration = await _context.Set<Model.Administration>().Where(a => a.Id == reportFilter.AdministrationIds[0]).SingleAsync();

				result = new FileContentResult(ms.ToArray(), fileType)
				{
					FileDownloadName = administration.Name + "_Lista_inventar_plusuri.pdf"
				};
			}
			else
			{
				if ((reportFilter.DepartmentIds != null) && (reportFilter.DepartmentIds.Count > 0))
				{
					department = await _context.Set<Model.Department>().Where(a => a.Id == reportFilter.DepartmentIds[0]).SingleAsync();

					result = new FileContentResult(ms.ToArray(), fileType)
					{
						FileDownloadName = department.Name + "_Lista_inventar_plusuri.pdf"
					};
				}
				else
				{
					if ((reportFilter.DivisionIds != null) && (reportFilter.DivisionIds.Count > 0))
					{
						division = await _context.Set<Model.Division>().Where(a => a.Id == reportFilter.DivisionIds[0]).SingleAsync();

						result = new FileContentResult(ms.ToArray(), fileType)
						{
							FileDownloadName = division.Name + "_Lista_inventar_plusuri.pdf"
						};
					}
					else
					{
						if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0))
						{
							costCenter = await _context.Set<Model.CostCenter>().Where(a => a.Id == reportFilter.CostCenterIds[0]).SingleAsync();

							result = new FileContentResult(ms.ToArray(), fileType)
							{
								FileDownloadName = costCenter.Code + "_Lista_inventar_plusuri.pdf"
							};
						}

					}
				}
			}

			return result;
		}

		[HttpGet("allowlabel")]
		public async Task<IActionResult> AllowLabel(int inventoryId, string jsonFilter, DateTime? inventoryDateEnd)
		{
			ReportFilter reportFilter = null;
			Model.CostCenter costCenter = null;
			Model.Division division = null;
			Model.Department department = null;
			Model.Administration administration = null;
			FileContentResult result = null;

			reportFilter = jsonFilter != null ? JsonConvert.DeserializeObject<ReportFilter>(jsonFilter) : new ReportFilter();

			var ms = await this._inventoryService.AllowLabelAsync(inventoryId, reportFilter, inventoryDateEnd);

            if (ms == null)
            {
                return null;
            }

            var fileType = "application/octet-stream";
			HttpContext.Response.ContentType = fileType;

			if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0))
			{
				administration = await _context.Set<Model.Administration>().Where(a => a.Id == reportFilter.AdministrationIds[0]).SingleAsync();

				result = new FileContentResult(ms.ToArray(), fileType)
				{
					FileDownloadName = administration.Name + "_Registru_neetichetabile.pdf"
				};
			}
			else
			{
				if ((reportFilter.DepartmentIds != null) && (reportFilter.DepartmentIds.Count > 0))
				{
					department = await _context.Set<Model.Department>().Where(a => a.Id == reportFilter.DepartmentIds[0]).SingleAsync();

					result = new FileContentResult(ms.ToArray(), fileType)
					{
						FileDownloadName = department.Name + "_Registru_neetichetabile.pdf"
					};
				}
				else
				{
					if ((reportFilter.DivisionIds != null) && (reportFilter.DivisionIds.Count > 0))
					{
						division = await _context.Set<Model.Division>().Where(a => a.Id == reportFilter.DivisionIds[0]).SingleAsync();

						result = new FileContentResult(ms.ToArray(), fileType)
						{
							FileDownloadName = division.Name + "_Registru_neetichetabile.pdf"
						};
					}
					else
					{
						if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0))
						{
							costCenter = await _context.Set<Model.CostCenter>().Where(a => a.Id == reportFilter.CostCenterIds[0]).SingleAsync();

							result = new FileContentResult(ms.ToArray(), fileType)
							{
								FileDownloadName = costCenter.Code + "_Registru_neetichetabile.pdf"
							};
						}

					}
				}
			}

			return result;
		}

		[HttpGet("bookBefore")]
		public async Task<IActionResult> BookBefore(int inventoryId, string jsonFilter, DateTime? inventoryDateStart)
		{
			ReportFilter reportFilter = null;
			Model.CostCenter costCenter = null;
			Model.Division division = null;
			Model.Department department = null;
			Model.Administration administration = null;
			FileContentResult result = null;

			reportFilter = jsonFilter != null ? JsonConvert.DeserializeObject<ReportFilter>(jsonFilter) : new ReportFilter();

			var ms = await this._inventoryService.BookBeforeAsync(inventoryId, reportFilter, inventoryDateStart);

            if (ms == null)
            {
                return null;
            }

            var fileType = "application/octet-stream";
			HttpContext.Response.ContentType = fileType;

			if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0))
			{
				administration = await _context.Set<Model.Administration>().Where(a => a.Id == reportFilter.AdministrationIds[0]).SingleAsync();

				result = new FileContentResult(ms.ToArray(), fileType)
				{
					FileDownloadName = administration.Name + "_Declaratie_gestionar_inainte.pdf"
				};
			}
			else
			{
				if ((reportFilter.DepartmentIds != null) && (reportFilter.DepartmentIds.Count > 0))
				{
					department = await _context.Set<Model.Department>().Where(a => a.Id == reportFilter.DepartmentIds[0]).SingleAsync();

					result = new FileContentResult(ms.ToArray(), fileType)
					{
						FileDownloadName = department.Name + "_Declaratie_gestionar_inainte.pdf"
					};
				}
				else
				{
					if ((reportFilter.DivisionIds != null) && (reportFilter.DivisionIds.Count > 0))
					{
						division = await _context.Set<Model.Division>().Where(a => a.Id == reportFilter.DivisionIds[0]).SingleAsync();

						result = new FileContentResult(ms.ToArray(), fileType)
						{
							FileDownloadName = division.Name + "_Declaratie_gestionar_inainte.pdf"
						};
					}
					else
					{
						if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0))
						{
							costCenter = await _context.Set<Model.CostCenter>().Where(a => a.Id == reportFilter.CostCenterIds[0]).SingleAsync();

							result = new FileContentResult(ms.ToArray(), fileType)
							{
								FileDownloadName = costCenter.Code + "_Declaratie_gestionar_inainte.pdf"
							};
						}

					}
				}
			}

			return result;
		}

		[HttpGet("bookAfter")]
		public async Task<IActionResult> BookAfter(int inventoryId, string jsonFilter, DateTime? inventoryDateEnd)
		{
			ReportFilter reportFilter = null;
			Model.CostCenter costCenter = null;
			Model.Division division = null;
			Model.Department department = null;
			Model.Administration administration = null;
			FileContentResult result = null;

			reportFilter = jsonFilter != null ? JsonConvert.DeserializeObject<ReportFilter>(jsonFilter) : new ReportFilter();

			var ms = await this._inventoryService.BookAfterAsync(inventoryId, reportFilter, inventoryDateEnd);

            if (ms == null)
            {
                return null;
            }

            var fileType = "application/octet-stream";
			HttpContext.Response.ContentType = fileType;


			if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0))
			{
				administration = await _context.Set<Model.Administration>().Where(a => a.Id == reportFilter.AdministrationIds[0]).SingleAsync();

				result = new FileContentResult(ms.ToArray(), fileType)
				{
					FileDownloadName = administration.Name + "_Declaratie_gestionar_dupa.pdf"
				};
			}
			else
			{
				if ((reportFilter.DepartmentIds != null) && (reportFilter.DepartmentIds.Count > 0))
				{
					department = await _context.Set<Model.Department>().Where(a => a.Id == reportFilter.DepartmentIds[0]).SingleAsync();

					result = new FileContentResult(ms.ToArray(), fileType)
					{
						FileDownloadName = department.Name + "_Declaratie_gestionar_dupa.pdf"
					};
				}
				else
				{
					if ((reportFilter.DivisionIds != null) && (reportFilter.DivisionIds.Count > 0))
					{
						division = await _context.Set<Model.Division>().Where(a => a.Id == reportFilter.DivisionIds[0]).SingleAsync();

						result = new FileContentResult(ms.ToArray(), fileType)
						{
							FileDownloadName = division.Name + "_Declaratie_gestionar_dupa.pdf"
						};
					}
					else
					{
						if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0))
						{
							costCenter = await _context.Set<Model.CostCenter>().Where(a => a.Id == reportFilter.CostCenterIds[0]).SingleAsync();

							result = new FileContentResult(ms.ToArray(), fileType)
							{
								FileDownloadName = costCenter.Code + "_Declaratie_gestionar_dupa.pdf"
							};
						}

					}
				}
			}

			return result;
		}

		[HttpGet("bookPV")]
		public async Task<IActionResult> BookPV(int inventoryId, string jsonFilter, DateTime? inventoryDateStart, DateTime? inventoryDateEnd)
		{
			ReportFilter reportFilter = null;
			Model.CostCenter costCenter = null;
			Model.Division division = null;
			Model.Department department = null;
			Model.Administration administration = null;
			FileContentResult result = null;

			reportFilter = jsonFilter != null ? JsonConvert.DeserializeObject<ReportFilter>(jsonFilter) : new ReportFilter();

			var ms = await this._inventoryService.BookPVAsync(inventoryId, reportFilter, inventoryDateStart, inventoryDateEnd);

            if (ms == null)
            {
                return null;
            }

            var fileType = "application/octet-stream";
			HttpContext.Response.ContentType = fileType;


			if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0))
			{
				administration = await _context.Set<Model.Administration>().Where(a => a.Id == reportFilter.AdministrationIds[0]).SingleAsync();

				result = new FileContentResult(ms.ToArray(), fileType)
				{
					FileDownloadName = administration.Name + "_Proces_verbal_inventariere.pdf"
				};
			}
			else
			{
				if ((reportFilter.DepartmentIds != null) && (reportFilter.DepartmentIds.Count > 0))
				{
					department = await _context.Set<Model.Department>().Where(a => a.Id == reportFilter.DepartmentIds[0]).SingleAsync();

					result = new FileContentResult(ms.ToArray(), fileType)
					{
						FileDownloadName = department.Name + "_Proces_verbal_inventariere.pdf"
					};
				}
				else
				{
					if ((reportFilter.DivisionIds != null) && (reportFilter.DivisionIds.Count > 0))
					{
						division = await _context.Set<Model.Division>().Where(a => a.Id == reportFilter.DivisionIds[0]).SingleAsync();

						result = new FileContentResult(ms.ToArray(), fileType)
						{
							FileDownloadName = division.Name + "_Proces_verbal_inventariere.pdf"
						};
					}
					else
					{
						if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0))
						{
							costCenter = await _context.Set<Model.CostCenter>().Where(a => a.Id == reportFilter.CostCenterIds[0]).SingleAsync();

							result = new FileContentResult(ms.ToArray(), fileType)
							{
								FileDownloadName = costCenter.Code + "_Proces_verbal_inventariere.pdf"
							};
						}

                    }
				}
			}

			return result;
		}

        [HttpGet("bookPVFinal")]
        public async Task<IActionResult> BookPVFinal(int inventoryId, string jsonFilter, DateTime? inventoryDateStart, DateTime? inventoryDateEnd)
        {
            ReportFilter reportFilter = null;
            FileContentResult result = null;

            reportFilter = jsonFilter != null ? JsonConvert.DeserializeObject<ReportFilter>(jsonFilter) : new ReportFilter();

            var ms = await this._inventoryService.BookPVFinalAsync(inventoryId, reportFilter, inventoryDateStart, inventoryDateEnd);

            if (ms == null)
            {
                return null;
            }

            var fileType = "application/octet-stream";
            HttpContext.Response.ContentType = fileType;

            result = new FileContentResult(ms.ToArray(), fileType)
            {
                FileDownloadName = "Proces_verbal_inventariere_final.pdf"
            };

            return result;
        }

        [HttpGet("export")]
		public IActionResult Export(string filter)
		{
			List<Model.Inventory> admCenters = null;
			int rowNumber = 0;
			using (ExcelPackage package = new ExcelPackage())
			{
				admCenters = (_itemsRepository as IInventoryRepository).GetByFilters(filter, null, null, null, null, null, null).ToList();
				admCenters = admCenters.OrderBy(l => l.Description).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Inventare");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod";
				worksheet.Cells[1, 2].Value = "Descriere";


				int recordIndex = 2;
				foreach (var item in admCenters)
				{
					rowNumber++;
					worksheet.Cells[recordIndex, 1].Value = item.Description;
					worksheet.Cells[recordIndex, 2].Value = item.Start;



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
					FileDownloadName = "PC.xlsx"
				};

				return result;

			}
		}
    }
}
