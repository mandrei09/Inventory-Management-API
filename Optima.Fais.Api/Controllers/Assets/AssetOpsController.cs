using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Dto.Sync;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static IdentityServer4.Models.IdentityResources;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/assetops")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AssetOpsController : GenericApiController<Model.AssetOp, Dto.AssetOp>
    {
        private readonly IHostingEnvironment hostingEnvironment;

        private readonly UserManager<Model.ApplicationUser> _userManager;

        public AssetOpsController(ApplicationDbContext context, IAssetOpsRepository itemsRepository, IMapper mapper, IHostingEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        //[HttpGet]
        //[Route("filtered")]
        //public virtual IActionResult GetOperationDetails(string assetOpState, DateTime startDate, DateTime endDate )
        //{
        //    List<Dto.AssetOpSd> items = (_itemsRepository as IAssetOpsRepository).GetFiltered(assetOpState, startDate, endDate).ToList();
        //    return Ok(items);
        //}

        [HttpGet]
        [Route("details")]
        public virtual IActionResult GetOperationDetailsFull(int? page, int? pageSize, string sortColumn, string sortDirection, 
            string includes, int? assetId, string documentTypeCode, string assetOpState,  DateTime startDate, DateTime endDate, string jsonFilter)
        {
            AssetFilter assetFilter = null;
            int totalItems = 0;
            string userName = string.Empty;
            string userId = null;
            string employeeId = string.Empty;
            string role = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + "Asset,AssetOpState,DimensionFinal";

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
                    assetFilter.EmployeeIds.Add(int.Parse("100000000"));
                }
            }
            else
            {
                assetFilter.EmployeeIds = null;
                assetFilter.EmployeeIds = new List<int?>();
                assetFilter.EmployeeIds.Add(int.Parse("100000000"));
            }

            List<Model.AssetOp> items = (_itemsRepository as IAssetOpsRepository)
                .GetFiltered(assetFilter, includes, assetId, documentTypeCode, assetOpState, startDate, endDate, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var itemsResource = _mapper.Map<List<Model.AssetOp>, List<Dto.AssetOp>>(items);
            var result = new PagedResult<Dto.AssetOp>(itemsResource, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });

            return Ok(result);
        }

        [HttpGet("exportoperation")]
        public async Task<IActionResult> ExporteMAGOperation(string jsonFilter, string propertyFilters, int? bfId, string assetOpState)
        {
            List<Model.AssetOpExport> items = null;
            AssetFilter assetFilter = null;
            List<PropertyFilter> propFilters = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            int rowNumber = 0;
            string userName = string.Empty;
            string role = string.Empty;
            int? costCenterId = null;

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

                    costCenterId = _context.Set<Model.Employee>().Where(e => e.Id == int.Parse(employeeId)).FirstOrDefault().CostCenterId;

                    assetFilter.EmpCostCenterIds = null;
                    assetFilter.EmpCostCenterIds = new List<int?>();
                    assetFilter.EmpCostCenterIds.Add(costCenterId);
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

            items = (_itemsRepository as IAssetOpsRepository)
               .ExportAssetOp(assetFilter, propFilters, assetOpState).ToList();

            using (ExcelPackage package = new ExcelPackage())
            {

                assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Emag");
                worksheet.Cells[1, 1].Value = "Numar bon";
                worksheet.Cells[1, 2].Value = "Type";
                worksheet.Cells[1, 3].Value = "Date transfer";
                worksheet.Cells[1, 4].Value = "Stare";
                worksheet.Cells[1, 5].Value = "Description";
                worksheet.Cells[1, 6].Value = "Asset";
                worksheet.Cells[1, 7].Value = "From Date temporary";
                worksheet.Cells[1, 8].Value = "To Date temporary";
                worksheet.Cells[1, 9].Value = "Email predator";
                worksheet.Cells[1, 10].Value = "Email primitor";
                worksheet.Cells[1, 11].Value = "Manager primitor";
                worksheet.Cells[1, 12].Value = "CC predator";
                worksheet.Cells[1, 13].Value = "CC primitor";
                

                int recordIndex = 2;
                int count = items.Count();
                int rowCell = items.Count() + 2;
                int rowTotal = items.Count() + 1;

                if (items.Count == 0 && assetFilter.Export == "scrap")
                {
                    return new StatusCodeResult(418);
                }
                else
                {
                    for (int a = 0; a < items.Count; a++)
                    {
                        rowNumber++;
                        int diff = a - count;

                        worksheet.Cells[recordIndex, 1].Value = items[a].Nbon;
                        worksheet.Cells[recordIndex, 2].Value = items[a].Type;
                        worksheet.Cells[recordIndex, 3].Value = items[a].DateTransfer;
                        worksheet.Cells[recordIndex, 4].Value = items[a].Stare;
                        worksheet.Cells[recordIndex, 5].Value = items[a].Description;
                        worksheet.Cells[recordIndex, 6].Value = items[a].Asset;
                        worksheet.Cells[recordIndex, 7].Value = items[a].FromDateTemporary;
                        worksheet.Cells[recordIndex, 8].Value = items[a].ToDateTemporary;
                        worksheet.Cells[recordIndex, 9].Value = items[a].EmailPredator;
                        worksheet.Cells[recordIndex, 10].Value = items[a].EmailTaker;
                        worksheet.Cells[recordIndex, 11].Value = items[a].ManageTaker;
                        worksheet.Cells[recordIndex, 12].Value = items[a].CCPredator;
                        worksheet.Cells[recordIndex, 13].Value = items[a].CCTaker;

                        if (diff == -1)
                        {
                            for (int i = 1; i < (assetFilter.Export == "stockhistory" ? 14 : 13); i++)
                            {
                                worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                                worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 9));

                            }

                            worksheet.Row(1).Height = 35.00;
                            worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.View.FreezePanes(2, 1);

                            using (var cells = worksheet.Cells[1, 1, 1, (assetFilter.Export == "stockhistory" ? 14 : 13)])
                            {
                                cells.Style.Font.Bold = true;
                                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                                cells.Style.Font.Color.SetColor(Color.Black);
                            }

                            using (var cells = worksheet.Cells[2, 1, items.Count() + 1, (assetFilter.Export == "stockhistory" ? 14 : 13)])
                            {
                                cells.Style.Font.Bold = false;
                                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cells.Style.Font.SetFromFont(new Font("Times New Roman", 9));
                            }

                            worksheet.View.ShowGridLines = false;
                            worksheet.View.ZoomScale = 100;

                            worksheet.Column(1).Width = 12.00;
                            worksheet.Column(2).Width = 10.00;
                            worksheet.Column(3).Width = 14.00;
                            worksheet.Column(4).Width = 20.00;
                            worksheet.Column(5).Width = 15.00;
                            worksheet.Column(6).Width = 55.00;
                            worksheet.Column(7).Width = 60.00;
                            worksheet.Column(8).Width = 30.00;
                            worksheet.Column(9).Width = 25.00;
                            worksheet.Column(10).Width = 25.00;
                            worksheet.Column(11).Width = 12.00;
                            worksheet.Column(12).Width = 40.00;
                            worksheet.Column(13).Width = 20.00;
                            worksheet.Column(14).Width = 30.00;
                            worksheet.Column(15).Width = 15.00;

                            package.Workbook.Calculate();

                        }
                        recordIndex++;
                    }
                }
                        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Export.xlsx"
                };

                return result;

            }
        }

        [HttpGet]
        [Route("recos")]
        public virtual IActionResult GetOperationRecoDetailsFull(int? page, int? pageSize, string sortColumn, string sortDirection,
          string includes, int? assetId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string jsonFilter)
        {
            AssetFilter assetFilter = null;
            int totalItems = 0;
            string userName = string.Empty;
            string userId = null;
            string employeeId = string.Empty;
            string role = string.Empty;
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            //includes = includes + ",AssetOpState,BudgetManagerInitial,ProjectInitial,DimensionInitial,AssetNatureInitial,BudgetManagerFinal,ProjectFinal,DimensionFinal,AssetNatureFinal";
            includes = includes + "Asset,AssetOpState,DimensionFinal";

            //if (HttpContext.User.Identity.Name != null)
            //{
            //    userName = HttpContext.User.Identity.Name;
            //    var user = _context.Users.Include(r => r.Claims).Where(u => u.UserName == userName).SingleOrDefault();


            //    userId = user != null ? user.Id : null;
            //    employeeId = user != null ? user.EmployeeId.ToString() : null;
            //    _context.UserId = userId;

            //    if (user != null)
            //    {
            //        var claims = user.Claims.ToList();
            //        if (claims[2].ClaimValue == "administrator")
            //        {
            //            employeeId = null;
            //        }


            //    }

            //    if (employeeId != null && employeeId != "")
            //    {
            //        if (assetFilter.EmployeeIds.Count() > 0)
            //        {
            //            var claimss = user.Claims.ToList();
            //            if (claimss[2].ClaimValue == "user")
            //            {

            //                assetFilter.EmployeeIds = null;
            //                assetFilter.EmployeeIds = new List<int?>();
            //                assetFilter.EmployeeIds.Add(int.Parse(employeeId));
            //            }
            //            else
            //            {
            //                employeeId = null;
            //            }

            //        }
            //        else
            //        {
            //            if ((assetFilter).EmployeeIds == null) assetFilter.EmployeeIds = new List<int?>();
            //            assetFilter.EmployeeIds.Add(int.Parse(employeeId));
            //        }

            //    }

            //    if (user == null)
            //    {
            //        assetFilter.EmployeeIds = null;
            //        assetFilter.EmployeeIds = new List<int?>();
            //        assetFilter.EmployeeIds.Add(int.Parse("100000000"));
            //    }
            //}else
            //{
            //    if ((assetFilter).EmployeeIds == null) assetFilter.EmployeeIds = new List<int?>();
            //    assetFilter.EmployeeIds.Add(int.Parse("1234567"));
            //}

            //if (HttpContext != null)
            //{
            //    userName = HttpContext.User.Identity.Name;
            //    if (userName != null)
            //    {
            //        Model.ApplicationUser userClaim = _context.Set<Model.ApplicationUser>().Include(c => c.Claims).Where(u => u.UserName == userName).Single();

            //        if (userClaim != null)
            //        {

            //            var claimsRole = userClaim.Claims.Where(c => c.ClaimType == "role").Select(c => c.ClaimValue).Single();

            //            if (claimsRole != "administrator")
            //            {
            //                var claimsType = userClaim.Claims.Where(c => c.ClaimType == "admCenter").Select(c => c.ClaimValue).FirstOrDefault();

            //                claimStrings = claimsType.Split('|').ToList();

            //                if ((assetFilter).AdmCenterIds == null) assetFilter.AdmCenterIds = new List<int?>();

            //                for (int i = 0; i < claimStrings.Count(); i++)
            //                {
            //                    var admCenter = _context.Set<Model.AdmCenter>().Where(u => u.Name == claimStrings[i]).Single();
            //                    assetFilter.AdmCenterIds.Add(int.Parse(admCenter.Id.ToString()));
            //                }
            //            }

            //        }
            //    }
            //    else
            //    {
            //        if ((assetFilter).AdmCenterIds == null) assetFilter.AdmCenterIds = new List<int?>();
            //        assetFilter.AdmCenterIds.Add(int.Parse("1234567"));
            //    }
            //}

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
                    assetFilter.EmployeeIds.Add(int.Parse("100000000"));
                }
            }
            else
            {
                assetFilter.EmployeeIds = null;
                assetFilter.EmployeeIds = new List<int?>();
                assetFilter.EmployeeIds.Add(int.Parse("100000000"));
            }


            List<Model.AssetOp> items = (_itemsRepository as IAssetOpsRepository)
                .GetRecoFiltered(assetFilter, includes, assetId, documentTypeCode, assetOpState, startDate, endDate, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var itemsResource = _mapper.Map<List<Model.AssetOp>, List<Dto.AssetOp>>(items);
            var result = new PagedResult<Dto.AssetOp>(itemsResource, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });



            return Ok(result);
        }

        [HttpGet]
        [Route("simpledetail/filtered")]
        public virtual IActionResult GetDetailsByAsset(int assetId)
        {
            //List<Dto.AssetOpSd> items = (_itemsRepository as IAssetOpsRepository).GetByAsset(assetId).ToList();
            //return Ok(items);

            List<Model.AssetOp> items = (_itemsRepository as IAssetOpsRepository).GetByAsset(assetId).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetOp>, List<Dto.AssetOp>>(items);
            return Ok(itemsResource);
        }

        [HttpGet]
        [Route("simpledetailnotValidate")]
        public virtual IActionResult GetDetails()
        {
            List<Dto.AssetOpSd> items = (_itemsRepository as IAssetOpsRepository).GetAll().ToList();
            return Ok(items);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sync")]
        public virtual IActionResult GetSyncDetails(int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.AssetOp> items = (_itemsRepository as IAssetOpsRepository).GetSyncDetails(pageSize, lastId, modifiedAt, out totalItems).ToList();

            var itemsResult = items.Select(i => _mapper.Map<AssetOpSync>(i));

            var pagedResult = new PagedResult<AssetOpSync>(itemsResult, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sync")]
        public async virtual Task<IActionResult> PutAssetOpDetail([FromBody] Dto.AssetOpSyncMobile assetOp)
        {

            Model.Asset asset = null;
            Model.AssetInv assetInv = null;
            Model.AssetAdmMD assetAdmMD = null;
            Model.AssetOp assetOpNew = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.Inventory inventory = null;
            //Model.DictionaryItem dictionaryItem = null;
            bool success = false;
            Model.ApplicationUser user = null;
            //Model.AssetChangeSAP assetChangeSAP = null;
            Model.CostCenter costCenterInventory = null;
			Model.Employee employeeInventory = null;

            //using (var errorfile = System.IO.File.CreateText("assetOp_" + DateTime.Now.Ticks + ".txt"))
            //{
            //    errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(assetOp, Formatting.Indented));
            //};

            if (assetOp.UserId != null && assetOp.UserId != "")
			{
                user = await _userManager.FindByNameAsync(assetOp.UserId);

                if(user == null)
				{
                    user = await _userManager.FindByEmailAsync(assetOp.UserId);
                }
			}
			else
			{
                user = await _userManager.FindByIdAsync(assetOp.UserId);
            }

            if(user == null)
			{
                user = await _userManager.FindByIdAsync("92E74C4F-A79A-4C83-A7D0-A3202BD2507F");
            }

            if (assetOp.AssetId > 0)
            {
                asset = _context.Set<Model.Asset>()
                    .Include(a => a.Stock)
					.Include(a => a.Company)
					.Include(a => a.CostCenter)
					.Include(a => a.Employee)
						.ThenInclude(a => a.CostCenter)
					.Include(a => a.ExpAccount)
                    .Include(a => a.AssetCategory)
					.Include(a => a.Document)
						.ThenInclude(a => a.Partner)
					.Include(a => a.BudgetForecast)
						.ThenInclude(a => a.BudgetBase)
						    .ThenInclude(a => a.Project)
					.SingleOrDefault(a => a.Id == assetOp.AssetId);
                assetInv = _context.Set<Model.AssetInv>().SingleOrDefault(a => a.AssetId == assetOp.AssetId);
                inventory = _context.Set<Model.Inventory>().AsNoTracking().SingleOrDefault(a => a.Active == true);
                assetAdmMD = _context.Set<Model.AssetAdmMD>().SingleOrDefault(a => a.AccMonthId == inventory.AccMonthId && a.AssetId == assetOp.AssetId);
                inventoryAsset = _context.Set<Model.InventoryAsset>().SingleOrDefault(a => a.InventoryId == inventory.Id && a.AssetId == assetOp.AssetId);



                //int administrationId = asset.AdministrationId.GetValueOrDefault();
                int? roomId = asset.RoomId.GetValueOrDefault() > 0 ? asset.RoomId.GetValueOrDefault() : null;
                int? invStateId = asset.InvStateId.GetValueOrDefault(1);
                int? employeeId = asset.EmployeeId.GetValueOrDefault() > 0 ? asset.EmployeeId.GetValueOrDefault() : null;
                int? costCenterId = asset.CostCenterId.GetValueOrDefault() > 0 ? asset.CostCenterId.GetValueOrDefault() : null;
                //int dimensionId = asset.DimensionId.GetValueOrDefault();
                //int uomId = asset.UomId.GetValueOrDefault();
                //int assetCategoryId = asset.AssetCategoryId.GetValueOrDefault();
                //int assetTypeId = asset.AssetTypeId.GetValueOrDefault();

                //int assetStateId = asset.AssetStateId.GetValueOrDefault();
                //int companyId = asset.CompanyId.GetValueOrDefault();
                //int assetNatureId = asset.AssetNatureId.GetValueOrDefault();
                //int interCategoryId = asset.InterCompanyId.GetValueOrDefault();
                //int insuranceCategoryId = asset.InsuranceCategoryId.GetValueOrDefault();

                //asset.AdministrationId = assetOp.AdministrationId.GetValueOrDefault() > 0 ? assetOp.AdministrationId : null;
                asset.RoomId = assetOp.RoomId != null && assetOp.RoomId > 0 ? assetOp.RoomId : null;
                asset.InvStateId = assetOp.InvStateId != null && assetOp.InvStateId > 0 ? assetOp.InvStateId : null;
                asset.EmployeeId = assetOp.EmployeeId != null && assetOp.EmployeeId > 0 ? assetOp.EmployeeId : null;
                asset.CostCenterId = assetOp.CostCenterId != null && assetOp.CostCenterId > 0 ? assetOp.CostCenterId : null;

                Model.CostCenter costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == asset.CostCenterId).SingleOrDefault();

                asset.DepartmentId = costCenter.Division.DepartmentId;
                asset.DivisionId = costCenter.DivisionId;

                asset.SerialNumber = assetOp.SerialNumber;
                asset.Info = assetOp.Info;

                asset.AllowLabel = Convert.ToBoolean(assetOp.AllowLabel);
                //asset.DimensionId = assetOp.DimensionId.GetValueOrDefault() > 0 ? assetOp.DimensionId : null;
                //asset.UomId = assetOp.UomId.GetValueOrDefault() > 0 ? assetOp.UomId : null;
                //asset.IsMinus = assetOp.IsMinus;
                //asset.InfoMinus = assetOp.InfoMinus;
                asset.Quantity = assetOp.Quantity != null ? assetOp.Quantity.Value : 1;
                //asset.AssetCategoryId = assetOp.AssetCategoryId.GetValueOrDefault() > 0 ? assetOp.AssetCategoryId : null;
                //asset.AssetTypeId = assetOp.AssetTypeId.GetValueOrDefault() > 0 ? assetOp.AssetTypeId : null;
                

                //asset.CompanyId = assetOp.CompanyId.GetValueOrDefault() > 0 ? assetOp.CompanyId : null;
                //asset.AssetNatureId = assetOp.AssetNatureId.GetValueOrDefault() > 0 ? assetOp.AssetNatureId : null;
                //asset.InterCompanyId = assetOp.InterCompanyId.GetValueOrDefault() > 0 ? assetOp.InterCompanyId : null;
                //asset.InsuranceCategoryId = assetOp.InsuranceCategoryId.GetValueOrDefault() > 0 ? assetOp.InsuranceCategoryId : null;

                //asset.ModifiedAt = DateTime.Now;
                asset.IsDuplicate = true;

                asset.InfoPlus = assetOp.UserId;

                //assetAdmMD.AdministrationId = assetOp.AdministrationId.GetValueOrDefault() > 0 ? assetOp.AdministrationId : null;
                assetAdmMD.RoomId = assetOp.RoomId != null && assetOp.RoomId > 0 ? assetOp.RoomId : null;
                assetAdmMD.EmployeeId = assetOp.EmployeeId != null && assetOp.EmployeeId > 0 ? assetOp.EmployeeId : null;
                assetAdmMD.CostCenterId = assetOp.CostCenterId != null && assetOp.CostCenterId > 0 ? assetOp.CostCenterId : null;

                assetAdmMD.DepartmentId = costCenter.Division.DepartmentId;
                assetAdmMD.DivisionId = costCenter.DivisionId;

                //assetAdmMD.AssetCategoryId = assetOp.AssetCategoryId.GetValueOrDefault() > 0 ? assetOp.AssetCategoryId : null;
                //assetAdmMD.AssetTypeId = assetOp.AssetTypeId.GetValueOrDefault() > 0 ? assetOp.AssetTypeId : null;
                //assetAdmMD.AssetNatureId = assetOp.AssetNatureId.GetValueOrDefault() > 0 ? assetOp.AssetNatureId : null;
                //assetAdmMD.InterCompanyId = assetOp.InterCompanyId.GetValueOrDefault() > 0 ? assetOp.InterCompanyId : null;
                //assetAdmMD.InsuranceCategoryId = assetOp.InsuranceCategoryId.GetValueOrDefault() > 0 ? assetOp.InsuranceCategoryId : null;


                assetInv.AllowLabel = Convert.ToBoolean(assetOp.AllowLabel);
                assetInv.Info = assetOp.Info;
                assetInv.InvStateId = assetOp.InvStateId != null && assetOp.InvStateId > 0 ? assetOp.InvStateId.Value : 1;

                inventoryAsset.AdministrationIdFinal = inventoryAsset.AdministrationIdInitial;
                inventoryAsset.RoomIdFinal = assetOp.RoomId != null && assetOp.RoomId > 0 ? assetOp.RoomId : null;
                inventoryAsset.StateIdFinal = assetOp.InvStateId != null && assetOp.InvStateId > 0 ? assetOp.InvStateId : null;
                inventoryAsset.EmployeeIdFinal = assetOp.EmployeeId != null && assetOp.EmployeeId > 0 ? assetOp.EmployeeId : null;
                inventoryAsset.CostCenterIdFinal = assetOp.CostCenterId != null && assetOp.CostCenterId > 0 ? assetOp.CostCenterId : null;
                inventoryAsset.SerialNumber = assetOp.SerialNumber;
                inventoryAsset.Info = assetOp.Info;
                //inventoryAsset.Info2019 = assetOp.Info2;
                
                inventoryAsset.DimensionIdFinal = asset.DimensionId;
                inventoryAsset.UomIdFinal = asset.UomId;
                //inventoryAsset.IsMinus = assetOp.IsMinus;
                //inventoryAsset.InfoMinus = assetOp.InfoMinus;
                inventoryAsset.ModifiedAt = assetOp.ModifiedAt?.AddHours(2);
                //inventoryAsset.SysModifiedAt = DateTime.UtcNow;
                inventoryAsset.ModifiedBy = user.Id;
                inventoryAsset.QFinal = assetOp.Quantity != null ? assetOp.Quantity.Value : 1;
                //inventoryAsset.ScanDate = assetOp.ModifiedAt?.AddHours(2);
                //inventoryAsset.ScanBy = user.Id;
                //inventoryAsset.UpdatedAt = assetOp.ModifiedAt?.AddHours(2);

                assetOpNew = new Model.AssetOp
                {
                    AssetId = assetOp.AssetId,
                    CreatedBy = user.Id,
                    RoomIdInitial = roomId,
                    RoomIdFinal = assetOp.RoomId != null && assetOp.RoomId > 0 ? assetOp.RoomId : null,
                    EmployeeIdInitial = employeeId,
                    EmployeeIdFinal = assetOp.EmployeeId != null && assetOp.EmployeeId > 0 ? assetOp.EmployeeId : null,
                    CostCenterIdInitial = costCenterId,
                    CostCenterIdFinal = assetOp.CostCenterId != null && assetOp.CostCenterId > 0 ? assetOp.CostCenterId : null,
                    AssetOpStateId = 3,
                    SrcConfBy = user.Id,
                    SrcConfAt = DateTime.Now,
                    Info = assetOp.Info,
                    Info2019 = string.Empty,
                    InvStateIdInitial = invStateId,
                    InvStateIdFinal = assetOp.InvStateId != null && assetOp.InvStateId > 0 ? assetOp.InvStateId : null,
                    IsDeleted = false,
                    AssetTypeIdInitial = asset.AssetTypeId,
                    AssetTypeIdFinal = asset.AssetTypeId,
                    DepartmentIdInitial = asset.DepartmentId,
                    DepartmentIdFinal = asset.DepartmentId,
                    AssetCategoryIdInitial = asset.AssetCategoryId,
                    AssetCategoryIdFinal = asset.AssetCategoryId,
                    AdministrationIdInitial = asset.AdministrationId,
                    AdministrationIdFinal = asset.AdministrationId,
                    AccSystemId = 3,
                    AssetStateIdInitial = asset.AssetStateId,
                    AssetStateIdFinal = asset.AssetStateId,
                    UomId = asset.UomId,
                    CompanyId = asset.CompanyId,
                    AssetNatureIdInitial = asset.AssetNatureId,
                    AssetNatureIdFinal = asset.AssetNatureId,
                    BudgetManagerIdInitial = asset.BudgetManagerId,
                    BudgetManagerIdFinal = asset.BudgetManagerId,
                    ProjectIdInitial = asset.ProjectId,
                    ProjectIdFinal = asset.ProjectId,
                    InterCompanyId = asset.InterCompanyId,
                    InsuranceCategoryId = asset.InsuranceCategoryId,
                    //UomIdFinal = assetOp.UomId.GetValueOrDefault() > 0 ? assetOp.UomId : null,
                    DimensionIdInitial = asset.DimensionId,
                    DimensionIdFinal = asset.DimensionId,
                    InvName = asset.Name,
                    SerialNumber = assetOp.SerialNumber,
                    Quantity = assetOp.Quantity != null ? assetOp.Quantity.Value : 1,
                    AllowLabel = Convert.ToBoolean(assetOp.AllowLabel),
                    DocumentId = inventory.DocumentId
                };


                if (asset.IsTemp == true)
                {
                    //dictionaryItem = _context.Set<Model.DictionaryItem>().Include(c => c.AssetCategory).AsNoTracking().Where(d => d.Id == asset.DictionaryItemId).SingleOrDefault();

                    //if (dictionaryItem != null)
                    //{

                    //    asset.AssetCategoryId = dictionaryItem.AssetCategoryId;
                    //    assetAdmMD.AssetCategoryId = dictionaryItem.AssetCategoryId;

                    //    //asset.AssetTypeId = dictionaryItem.AssetCategory.AssetTypeId;
                    //    //assetAdmMD.AssetTypeId = dictionaryItem.AssetCategory.AssetTypeId;

                    //    assetOpNew.AssetCategoryIdFinal = dictionaryItem.AssetCategoryId;
                    //    //assetOpNew.AssetTypeIdFinal = dictionaryItem.AssetCategory.AssetTypeId;
                    //}
                    asset.Name = assetOp.InvName;
                    

                }

                _context.UserId = user.Id;
                _context.Update(asset);
                _context.Update(assetAdmMD);
                _context.Update(inventoryAsset);

                _context.Set<Model.AssetOp>().Add(assetOpNew);

				if ((inventoryAsset.IsTemp == false) && ((inventoryAsset.CostCenterIdInitial != inventoryAsset.CostCenterIdFinal) || (inventoryAsset.SNInitial != inventoryAsset.SerialNumber)))
				{
                    costCenterInventory = await _context.Set<Model.CostCenter>().Where(c => c.Id == assetOp.CostCenterId).FirstOrDefaultAsync();
					employeeInventory = await _context.Set<Model.Employee>().Include(c => c.CostCenter).Where(c => c.Id == assetOp.EmployeeId).FirstOrDefaultAsync();

                    if (employeeInventory == null)
                    {
						employeeInventory = await _context.Set<Model.Employee>().Include(c => c.CostCenter).Where(c => c.Id == asset.EmployeeId).FirstOrDefaultAsync();
					}

					//var names = SplitToLines(asset.Name, 50);
					//var countNames = names.Count();

					//assetChangeSAP = new Model.AssetChangeSAP()
					//{
					//	COMPANYCODE = asset.Company.Code,
					//	ASSET = asset.InvNo,
					//	SUBNUMBER = asset.SubNo,
					//	ASSETCLASS = asset.ExpAccount != null ? asset.ExpAccount.Name : "",
					//	POSTCAP = "",
					//	DESCRIPT = countNames > 0 ? names.ElementAt(0) : "",
					//	DESCRIPT2 = countNames > 1 ? names.ElementAt(1) : "",
					//	INVENT_NO = asset.ERPCode ?? "",
					//	SERIAL_NO = asset.SerialNumber ?? "",
					//	QUANTITY = (int)asset.Quantity,
					//	BASE_UOM = "ST",
					//	LAST_INVENTORY_DATE = "00000000",
					//	LAST_INVENTORY_DOCNO = "",
					//	CAP_DATE = "00000000",
					//	COSTCENTER = costCenterInventory != null ? costCenterInventory.Code : "",
					//	RESP_CCTR = employeeInventory != null && employeeInventory.CostCenter != null ? employeeInventory.CostCenter.Code : "",
					//	INTERN_ORD = "",
					//	PLANT = "RO02",
					//	LOCATION = "",
					//	ROOM = "",
					//	PERSON_NO = asset.Employee != null ? asset.Employee.InternalCode : "",
					//	PLATE_NO = asset.AgreementNo ?? string.Empty,
					//	ZZCLAS = asset.AssetCategory != null ? asset.AssetCategory.Code : "",
					//	IN_CONSERVATION = "",
					//	PROP_IND = "1",
					//	OPTIMA_ASSET_NO = "",
					//	OPTIMA_ASSET_PARENT_NO = "",
					//	VENDOR_NO = asset.Document.Partner != null ? asset.Document.Partner.RegistryNumber : "",
					//	FROM_DATE = "00000000",
					//	AccMonthId = inventory.AccMonthId.Value,
					//	AssetId = asset.Id,
					//	BudgetManagerId = asset.BudgetManagerId.Value,
					//	NotSync = true,
					//	CreatedAt = DateTime.Now,
					//	CreatedBy = _context.UserId,
					//	ModifiedAt = DateTime.Now,
					//	ModifiedBy = _context.UserId,
					//	INVOICE = asset.Stock != null && asset.Stock.Invoice != null ? asset.Stock.Invoice : (asset.Document.DocNo1 != null ? asset.Document.DocNo1 : ""),
					//	DOC_YEAR = "00000000",
					//	MAT_DOC = "",
					//	WBS_ELEMENT = asset.BudgetForecast != null && asset.BudgetForecast.BudgetBase != null && asset.BudgetForecast.BudgetBase.Project != null ? asset.BudgetForecast.BudgetBase.Project.Code : ""
					//};

					//_context.Add(assetChangeSAP);
				}

				_context.SaveChanges();

                
                success = true;
            }
            if (success)
            {
                return Ok(StatusCode(200));
            }
            else
            {
                return NotFound();
            }
        }

        [AllowAnonymous]
        [HttpDelete("syncdelete/{assetId}")]
        public async Task<int> DeleteAssetOpAsync(int assetId)
        {
            Model.Inventory inventory = null;
            Model.AssetAdmMD assetAdmMD = null;
            List<Model.AssetOp> assetOps = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.AssetInv assetInv = null;

            inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Active == true).SingleAsync();

            List<Model.Asset> assets = await _context.Set<Model.Asset>().AsNoTracking().Where(a => a.Id == assetId).ToListAsync();

            for (int i = 0; i < assets.Count; i++)
            {
                assetInv = await _context.Set<Model.AssetInv>().Where(a => a.AssetId == assets[i].Id).SingleAsync();
                assetOps = await _context.Set<Model.AssetOp>().Where(a => a.AssetId == assets[i].Id && a.AssetOpStateId == 3 && a.IsDeleted == false && a.DocumentId == inventory.DocumentId).ToListAsync();

                List<Model.EntityFile> entityFiles = await _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.EntityId == assets[i].Id && e.EntityType.Name == inventory.DocumentId.ToString() && e.EntityType.Code == "ASSET").ToListAsync();

                if (entityFiles.Count() > 0)
                {
                    for (int k = 0; k < entityFiles.Count; k++)
                    {
                        entityFiles[k].IsDeleted = true;
                        _context.Update(entityFiles[k]);
                        _context.SaveChanges();
                    }
                }

                inventoryAsset = await _context.Set<Model.InventoryAsset>().Where(a => a.AssetId == assets[i].Id && a.InventoryId == inventory.Id).SingleAsync();

                if (assetInv != null)
                {
                    assetInv.Info = string.Empty;
                    assetInv.InvStateId = inventoryAsset.StateIdInitial.Value;
                }

                if (assets[i].InvNo.StartsWith("T"))
                {
                    assets[i].Name = string.Empty;
                    assets[i].ModifiedAt = DateTime.Now;
                    assets[i].ImageCount = 0;
                    assets[i].RoomId = inventoryAsset.RoomIdInitial;
                    assets[i].EmployeeId = inventoryAsset.EmployeeIdInitial;
                    //assets[i].AdministrationId = inventoryAsset.AdministrationIdInitial;
                    assets[i].CostCenterId = inventoryAsset.CostCenterIdInitial;

                    Model.CostCenter costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == assets[i].CostCenterId).SingleOrDefault();

                    assets[i].DepartmentId = costCenter.Division.DepartmentId;
                    assets[i].DivisionId = costCenter.DivisionId;

                    //assets[i].UomId = inventoryAsset.UomIdInitial;
                    //assets[i].DimensionId = inventoryAsset.DimensionIdInitial;
                    assets[i].Quantity = inventoryAsset.QInitial;
                    assets[i].SerialNumber = inventoryAsset.SNInitial;
                    assets[i].Info = string.Empty;
                    //assets[i].AssetCategoryId = 1635;
                    //assets[i].AssetTypeId = 1013;

                }
                else
                {
                    assets[i].ModifiedAt = DateTime.Now;
                    assets[i].ImageCount = 0;
                    assets[i].RoomId = inventoryAsset.RoomIdInitial;
                    assets[i].EmployeeId = inventoryAsset.EmployeeIdInitial;
                    //assets[i].AdministrationId = inventoryAsset.AdministrationIdInitial;
                    assets[i].CostCenterId = inventoryAsset.CostCenterIdInitial;

                    Model.CostCenter costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == assets[i].CostCenterId).SingleOrDefault();

                    assets[i].DepartmentId = costCenter.Division.DepartmentId;
                    assets[i].DivisionId = costCenter.DivisionId;

                    //assets[i].UomId = inventoryAsset.UomIdInitial;
                    //assets[i].DimensionId = inventoryAsset.DimensionIdInitial;
                    assets[i].Quantity = inventoryAsset.QInitial;
                    assets[i].SerialNumber = inventoryAsset.SNInitial;
                    assets[i].Info = string.Empty;
                }

                assets[i].IsDuplicate = false;

                assetAdmMD = await _context.Set<Model.AssetAdmMD>().Where(a => a.AssetId == assets[i].Id && a.AccMonthId == inventory.AccMonthId).SingleAsync();

                if (assetAdmMD != null)
                {
                    assetAdmMD.RoomId = inventoryAsset.RoomIdInitial;
                    assetAdmMD.EmployeeId = inventoryAsset.EmployeeIdInitial;
                    assetAdmMD.CostCenterId = inventoryAsset.CostCenterIdInitial;

                    Model.CostCenter costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == assetAdmMD.CostCenterId).SingleOrDefault();

                    assetAdmMD.DepartmentId = costCenter.Division.DepartmentId;
                    assetAdmMD.DivisionId = costCenter.DivisionId;
                    //assetAdmMD.AdministrationId = inventoryAsset.AdministrationIdInitial;
                }

                inventoryAsset.CostCenterIdFinal = null;
                inventoryAsset.EmployeeIdFinal = null;
                //inventoryAsset.Model = string.Empty;
                //inventoryAsset.Producer = string.Empty;
                inventoryAsset.SerialNumber = string.Empty;
                inventoryAsset.Info = string.Empty;
                //inventoryAsset.Info2019 = null;
                inventoryAsset.RoomIdFinal = null;
                inventoryAsset.StateIdFinal = null;
                inventoryAsset.AdministrationIdFinal = null;
                inventoryAsset.ModifiedAt = null;
                inventoryAsset.ModifiedBy = null;
                inventoryAsset.UomIdFinal = null;
                inventoryAsset.DimensionIdFinal = null;
                inventoryAsset.IsMinus = false;
                inventoryAsset.InfoMinus = "";
                inventoryAsset.ImageCount = 0;
                inventoryAsset.QFinal = 0;

                _context.Update(assets[i]);

                _context.SaveChanges();

                if (assetOps.Count > 0)
                {

                    for (int m = 0; m < assetOps.Count; m++)
                    {
                        assetOps[m].IsDeleted = true;
                        assetOps[m].ModifiedAt = DateTime.Now;
                        _context.SaveChanges();
                    }

                }


            }


            if (assets.Count > 0)
            {
                return assets[0].Id;
            }
            else
            {
                return 0;
            }


        }


        //[HttpPost]
        //[Authorize]
        //[Route("sync")]
        //public IActionResult SaveAssetInvOps([FromBody] Dto.AssetInvOp op)
        //{
        //    Model.Asset asset = null;
        //    Model.AssetOp assetOp = null;
        //    Model.AssetOp assetOpPrev = null;
        //    Model.InventoryAsset inventoryAsset = null;
        //    Model.Inventory inventory = null;
        //    Model.Employee employee = null;
        //    string userName = string.Empty;
        //    string userId = null;

        //    if (HttpContext != null)
        //    {
        //        userName = HttpContext.User.Identity.Name;
        //        var user = _context.Users.Where(u => u.UserName == userName).SingleOrDefault();
        //        userId = user != null ? user.Id : null;

        //        _context.UserId = userId;
        //    }

        //    if (op.InventoryId.HasValue)
        //    {
        //        //return Unauthorized();

        //        inventory = _context.Set<Model.Inventory>().Where(i => i.Id == op.InventoryId).Single();
        //        //if (!inventory.Active) return Unauthorized();
        //        inventoryAsset = _context.Set<Model.InventoryAsset>().Where(i => ((i.InventoryId == inventory.Id) && (i.AssetId == op.AssetId))).Single();
        //    }

        //    if (op.Id > 0)
        //    {
        //        assetOp = _context.Set<Model.AssetOp>().Where(a => a.Id == op.Id).Single();

        //        assetOp.Info = op.Info;
        //        assetOp.RoomIdInitial = op.RoomIdInitial;
        //        assetOp.RoomIdFinal = op.RoomIdFinal;
        //        assetOp.EmployeeIdInitial = op.EmployeeIdInitial;
        //        assetOp.EmployeeIdFinal = op.EmployeeIdFinal;

        //        _context.Set<Model.AssetOp>().Update(assetOp);

        //        if (assetOp.IsDeleted != op.IsDeleted)
        //        {
        //            assetOp.IsDeleted = op.IsDeleted;

        //            if (op.InventoryId.HasValue)
        //            {
        //                assetOpPrev = _context.Set<Model.AssetOp>().Where(a => ((a.DocumentId == inventory.DocumentId) && (a.AssetId == op.AssetId) && (!a.IsDeleted))).OrderByDescending(a => a.Id).Take(1).SingleOrDefault();

        //                if (assetOpPrev != null)
        //                {
        //                    inventoryAsset.EmployeeIdFinal = assetOpPrev.EmployeeIdFinal != null ? assetOpPrev.EmployeeIdFinal : null;
        //                    inventoryAsset.RoomIdFinal = assetOpPrev.RoomIdFinal != null ? assetOpPrev.RoomIdFinal : null;
        //                    inventoryAsset.StateIdFinal = assetOpPrev.InvStateIdFinal != null ? assetOpPrev.InvStateIdFinal : null;

        //                    employee = _context.Set<Model.Employee>().Where(e => e.Id == assetOpPrev.EmployeeIdFinal).Single();
        //                    inventoryAsset.CostCenterIdFinal = ((employee != null) && (employee.CostCenterId != null)) ? employee.CostCenterId : inventoryAsset.CostCenterIdInitial;
        //                }
        //                else
        //                {
        //                    inventoryAsset.EmployeeIdFinal = null;
        //                    inventoryAsset.RoomIdFinal = null;
        //                    inventoryAsset.StateIdFinal = null;
        //                    inventoryAsset.QFinal = 0;
        //                    inventoryAsset.CostCenterIdFinal = null;
        //                }

        //                inventoryAsset.Info = op.Info;
        //                inventoryAsset.ModifiedBy = userId;
        //                inventoryAsset.ModifiedAt = DateTime.Now;
        //                _context.Set<Model.InventoryAsset>().Update(inventoryAsset);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        assetOp = new Model.AssetOp();

        //        assetOp.AssetId = op.AssetId;

        //        assetOp.CreatedBy = "";
        //        assetOp.RoomIdInitial = op.RoomIdInitial;
        //        assetOp.RoomIdFinal = op.RoomIdFinal;
        //        assetOp.EmployeeIdInitial = op.EmployeeIdInitial;
        //        assetOp.EmployeeIdFinal = op.EmployeeIdFinal;

        //        assetOp.AssetOpStateId = 3;
        //        assetOp.SrcConfBy = userId;
        //        assetOp.SrcConfAt = DateTime.Now;
        //        assetOp.Info = op.Info;

        //        _context.Set<Model.AssetOp>().Add(assetOp);

        //        if (op.InventoryId.HasValue)
        //        {
        //            assetOp.DocumentId = inventory.DocumentId;

        //            if (op.EmployeeIdFinal != null) inventoryAsset.EmployeeIdFinal = op.EmployeeIdFinal;
        //            if (op.RoomIdFinal != null) inventoryAsset.RoomIdFinal = op.RoomIdFinal;
        //            if (op.StateIdFinal != null) inventoryAsset.StateIdFinal = op.StateIdFinal;

        //            //inventoryAsset.QFinal = op.QFinal;
        //            asset = _context.Set<Model.Asset>().Where(a => a.Id == op.AssetId).Single();
        //            inventoryAsset.QInitial = asset.Quantity;
        //            inventoryAsset.QFinal = asset.Quantity;

        //            if (inventoryAsset.EmployeeIdInitial != inventoryAsset.EmployeeIdFinal)
        //            {
        //                employee = _context.Set<Model.Employee>().Where(e => e.Id == op.EmployeeIdFinal).Single();
        //                inventoryAsset.CostCenterIdFinal = ((employee != null) && (employee.CostCenterId != null)) ? employee.CostCenterId : inventoryAsset.CostCenterIdInitial;
        //            }
        //            else
        //            {
        //                inventoryAsset.CostCenterIdFinal = inventoryAsset.CostCenterIdInitial;
        //            }

        //            inventoryAsset.Info = op.Info;
        //            inventoryAsset.ModifiedBy = userId;
        //            inventoryAsset.ModifiedAt = DateTime.Now;
        //            _context.Set<Model.InventoryAsset>().Update(inventoryAsset);
        //        }
        //        else
        //        {
        //            string documentTypeCode = "TRANSFER";
        //            Model.Document document = new Model.Document();
        //            Model.DocumentType documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();

        //            DateTime creationDate = DateTime.Now;
        //            DateTime documentDate = new DateTime(creationDate.Year, creationDate.Month, creationDate.Day);

        //            document.DocumentType = documentType;
        //            document.DocNo1 = string.Empty;
        //            document.DocNo2 = string.Empty;
        //            document.DocumentDate = documentDate;
        //            document.RegisterDate = documentDate;
        //            document.Approved = true;
        //            document.Exported = false;
        //            document.CreationDate = creationDate;
        //            document.Details = string.Empty;

        //            _context.Set<Model.Document>().Add(document);

        //            assetOp.Document = document;
        //        }
        //    }

        //    //asset = _context.Set<Model.Asset>().Where(i => i.Id == op.AssetId).Single();
        //    //if (op.EmployeeIdFinal != null) asset.EmployeeId = op.EmployeeIdFinal;
        //    //if (op.RoomIdFinal != null) asset.RoomId = op.RoomIdFinal;
        //    //if (op.StateIdFinal != null) asset.InvStateId = op.StateIdFinal;

        //    //_context.Set<Model.Asset>().Update(asset);

        //    _context.SaveChanges();

        //    op.Id = assetOp.Id;

        //    //return Ok(assetOp);
        //    return Ok(_mapper.Map<Dto.AssetOp>(assetOp));
        //}

        [HttpPost]
        [Route("process")]
        [Authorize]
        public async virtual Task<IActionResult> AssetOpProcess([FromBody] int[] assetOpIds)
        {
            Model.Asset assetPrev = null;
            Model.InvState invState = null;
            Model.AssetState assetState = null;
            Model.AccMonth accMonth = null;
            Model.AssetOp assetOpPrev = null;
            Model.Inventory inventory = null;
            Model.InventoryAsset inventoryAssetPrev = null;
            Model.AssetAdmMD assetAdmMDPrev = null;
            assetOpIds = assetOpIds.Distinct().ToArray();
            List<string> emailIni = new List<string>();
            List<string> emailCC = new List<string>();
            var operationsCount = 0;
            var subject = "Transfer validated";
            string htmlBody = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    Upcoming topics
                                                </title>
                                                <style type=""text/css"">
                                                    HTML{background-color: #e8e8e8;}
                                                    .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
                                                    .courses-table .description{color: #505050;}
                                                    .red{background-color: #003880;}
                                                    .courses-table td{border: 1px solid #D1D1D1; background-color: #F3F3F3; padding: 0 10px;}
                                                    .courses-table th{border: 1px solid #424242; color: #FFFFFF;text-align: left; padding: 0 10px;}
                                                    .red{background-color: #003880;}
                                                    .green{background-color: #EBC9B3;}
                                                </style>
                                            </head>
                                            <body>
                                                <h2>Dear user, the following transfer have  been validated:</h2>
                                                <table class=""courses-table"">
                                                    <thead>
                                                        <tr>
                                                            <th class=""red""></th>
                                                            <th class=""red"">Asset number</th>
                                                            <th class=""red"">Description</th>
                                                            <th class=""red"">Employee</th>
                                                            <th class=""red"">Building</th>
                                                            <th class=""red"">Room</th>
                                                            <th class=""red"">Category</th>
                                                            <th class=""red"">Asset value</th>
                                                           
                                                          
                                                        </tr>
                                                    </thead>
                                                    <tbody>";

            var htmlBodyEnd = @" </tbody>
                                                </table>

                                        <h5>This is an automated message – Please do not replay directly to this email!</h5>
                                        <h5>For more details please contact the IT Administration at following email: adrian.cirnaru@optima.ro</h5>

                                            </body>
                                        </html>
                                        ";
            var htmlBodyFinal = "";

            foreach (var assetOpId in assetOpIds)
            {
                operationsCount++;
                inventory = _context.Set<Model.Inventory>().Where(i => i.Active == true).Single();
                accMonth = _context.Set<Model.AccMonth>().Where(i => i.IsActive == true).Single();
                assetOpPrev = _context.Set<Model.AssetOp>().Include(a => a.Asset).Where(a => a.Id == assetOpId).SingleOrDefault();
                assetPrev = _context.Set<Model.Asset>().Where(a => a.Id == assetOpPrev.AssetId).SingleOrDefault();
                //inventoryAssetPrev = _context.Set<Model.InventoryAsset>().Where(a => a.AssetId == assetOpPrev.AssetId && a.InventoryId == inventory.Id).SingleOrDefault();
                assetAdmMDPrev = _context.Set<Model.AssetAdmMD>().Where(a => a.AssetId == assetOpPrev.AssetId && a.AccMonthId == accMonth.Id).SingleOrDefault();
                invState = _context.Set<Model.InvState>().Where(a => a.Id == assetOpPrev.InvStateIdFinal).SingleOrDefault();
                assetState = _context.Set<Model.AssetState>().Where(a => a.Name == invState.Name).SingleOrDefault();

                if (assetOpPrev.AssetOpStateId < 5)
                {
                   

                    //roomInitialEmployee = _context.Set<Model.Room>().Where(r => r.Id == assetOpPrev.RoomIdInitial).FirstOrDefault();
                    //roomFinalEmployee = _context.Set<Model.Room>().Where(r => r.Id == assetOpPrev.RoomIdFinal).FirstOrDefault();

                    //locationInitialEmployee = _context.Set<Model.Location>().Where(r => r.Id == roomInitialEmployee.LocationId).FirstOrDefault();
                    //locationFinalEmployee = _context.Set<Model.Location>().Where(r => r.Id == roomFinalEmployee.LocationId).FirstOrDefault();

                    //admCenterInitialEmployee = _context.Set<Model.AdmCenter>().Where(r => r.Id == locationInitialEmployee.AdmCenterId).FirstOrDefault();
                    //admCenterFinalEmployee = _context.Set<Model.AdmCenter>().Where(r => r.Id == locationFinalEmployee.AdmCenterId).FirstOrDefault();

                    string userName = HttpContext.User.Identity.Name;
                    var user = _context.Users.Where(u => u.UserName == userName).Single();

                    //var claims = await _userManager.GetClaimsAsync(user);
                    
                    //var claim = claims.FirstOrDefault(c => c.Type == "admCenter");

                    switch (assetOpPrev.AssetOpStateId)
                    {
                        case 1:
                            assetOpPrev.ReleaseConfAt = DateTime.Now;
                            assetOpPrev.ReleaseConfBy = user.Id;
                            break;
                        case 2:
                            assetOpPrev.SrcConfAt = DateTime.Now;
                            assetOpPrev.SrcConfBy = user.Id;
                            break;
                        case 3:
                            //assetOpPrev.DstConfAt = DateTime.Now;
                            //assetOpPrev.DstConfBy = user.Id;
                            //assetPrev.RoomId = assetOpPrev.RoomIdFinal;
                            //assetPrev.EmployeeId = assetOpPrev.EmployeeIdFinal;
                            //assetPrev.AssetStateId = assetState.Id;
                            //assetPrev.InvStateId = assetOpPrev.InvStateIdFinal;
                            //inventoryAssetPrev.RoomIdInitial = assetOpPrev.RoomIdFinal;
                            //inventoryAssetPrev.EmployeeIdInitial = assetOpPrev.EmployeeIdFinal;
                            //inventoryAssetPrev.StateIdInitial = assetOpPrev.InvStateIdFinal;
                            //assetAdmMDPrev.RoomId = assetOpPrev.RoomIdFinal;
                            //assetAdmMDPrev.AssetStateId = assetState.Id;
                            //assetAdmMDPrev.EmployeeId = assetOpPrev.EmployeeIdFinal;



                            // Transfer //

                            Model.Employee employeeIni = null;
                            Model.Employee employeeFin = null;
                            Model.Room roomIni = null;
                            Model.Room roomFin = null;
                            Model.Location locationIni = null;
                            Model.Location locationFin = null;
                            Model.AssetCategory assetCategoryIni = null;
                            Model.AssetCategory assetCategoryFin = null;


                            var assetCategoryNameIni = "-";
                            var assetCategoryNameFin = "-";
                            var roomNameIni = "";
                            var roomNameFin = "";
                            var roomCodeIni = "";
                            var roomCodeFin = "";
                            var locationCodeIni = "";
                            var locationCodeFin = "";
                            var employeeNameIni = "";
                            var employeeNameFin = "";

                            if (assetOpPrev.RoomIdInitial != null)
                            {
                                roomIni = _context.Set<Model.Room>().Where(a => a.Id == assetOpPrev.RoomIdInitial).FirstOrDefault();

                                if (roomIni != null)
                                {
                                    locationIni = _context.Set<Model.Location>().Where(a => a.Id == roomIni.LocationId).FirstOrDefault();
                                }
                            }

                            if (assetOpPrev.RoomIdFinal != null)
                            {
                                roomFin = _context.Set<Model.Room>().Where(a => a.Id == assetOpPrev.RoomIdFinal).FirstOrDefault();

                                if (roomFin != null)
                                {
                                    locationFin = _context.Set<Model.Location>().Where(a => a.Id == roomFin.LocationId).FirstOrDefault();
                                }
                            }


                            if (assetOpPrev.EmployeeIdInitial != null)
                            {
                                employeeIni = _context.Set<Model.Employee>().Where(a => a.Id == assetOpPrev.EmployeeIdInitial).FirstOrDefault();
                            }

                            if (assetOpPrev.EmployeeIdFinal != null)
                            {
                                employeeFin = _context.Set<Model.Employee>().Where(a => a.Id == assetOpPrev.EmployeeIdFinal).FirstOrDefault();
                            }

                            if (assetOpPrev.AssetCategoryIdInitial != null)
                            {
                                assetCategoryIni = _context.Set<Model.AssetCategory>().Where(a => a.Id == assetOpPrev.AssetCategoryIdInitial).FirstOrDefault();
                            }

                            if (assetOpPrev.AssetCategoryIdFinal != null)
                            {
                                assetCategoryFin = _context.Set<Model.AssetCategory>().Where(a => a.Id == assetOpPrev.AssetCategoryIdFinal).FirstOrDefault();
                            }

                            assetOpPrev.DstConfAt = DateTime.Now;
                            assetOpPrev.DstConfBy = user.Id;
                            assetPrev.RoomId = roomFin.Id;
                            assetPrev.EmployeeId = employeeFin.Id;
                            assetPrev.AssetStateId = assetState.Id;
                            assetPrev.InvStateId = assetOpPrev.InvStateIdFinal;
                            assetPrev.IsInTransfer = false;

                            assetAdmMDPrev.RoomId = roomFin.Id;
                            assetAdmMDPrev.EmployeeId = employeeFin.Id;
                            assetAdmMDPrev.AssetStateId = assetState.Id;


                            var assetCategIni = assetCategoryIni != null ? assetCategoryIni.Name : assetCategoryNameIni;
                            var assetCategFin = assetCategoryFin != null ? assetCategoryFin.Name : assetCategoryNameFin;
                            var rIniCode = roomIni != null ? roomIni.Name : roomCodeIni;
                            var rFinCode = roomFin != null ? roomFin.Name : roomCodeFin;
                            var lIniName = locationIni != null ? locationIni.Name : locationCodeIni;
                            var lFinName = locationFin != null ? locationFin.Name : locationCodeFin;
                            var rIniName = roomIni != null ? roomIni.Name : roomNameIni;
                            var rFinName = roomFin != null ? roomFin.Name : roomNameFin;
                            var eIni = employeeIni != null ? employeeIni.FirstName + " " + employeeIni.LastName : employeeNameIni;
                            var eFin = employeeFin != null ? employeeFin.FirstName + " " + employeeFin.LastName : employeeNameFin;

                            emailIni.Add(employeeIni != null ? employeeIni.Email != null ? employeeIni.Email : "adrian.cirnaru@optima.ro" : "adrian.cirnaru@optima.ro");
							//var emailCC = "dvladoiu@stanleybet.ro";

							//var emailIni = "adrian.cirnaru@optima.ro";
							//var emailFin = "ALIN.CERNATESCU@ALLIANZ.COM";
							emailCC.Add("adrian.cirnaru@optima.ro");
							emailCC.Add("cosmina.pricop@optima.ro");

                            //assets.Add(" - " + assetTransfer.InvNo + " " + assetTransfer.Name + " from " + assetTransfer.AssetAdm.Employee.InternalCode + " " + "(" + assetTransfer.AssetAdm.Employee.FirstName + " " + assetTransfer.AssetAdm.Employee.LastName + ")" + "\r\n");






                            //var result = String.Join(Environment.NewLine,
                            //             assets.Select(a => String.Join("\r\n", a)));

                            //  var password = base64Decode2(user.PasswordHash);

                            //var email = "adrian.cirnaru@optima.ro";
                            //var email1 = "mdeaconescu@stanleybet.ro";
                            //var email2 = "cosmina.cretu@optima.ro";




                            htmlBody = htmlBody + @"
                                                        <tr>
                                                           <td class=""description"">Initial</ td >
                                                            <td class=""description"">" + assetOpPrev.Asset.InvNo + @" </ td >
                                                            <td class=""description"">" + assetOpPrev.Asset.Name + @" </ td >
                                                            <td class=""description"">" + eIni + @" </ td >
                                                            <td class=""description"">" + lIniName + @" </ td >
                                                            <td class=""description"">" + rIniName + @" </ td >
                                                            <td class=""description"">" + assetCategIni + @" </ td >
                                                            <td class=""description"">" + assetOpPrev.Asset.ValueInv + @" </ td >
                                                           
                                                       
                                                        </tr>";

                            if(assetOpIds.Count() == operationsCount)
                            {
                                htmlBody = htmlBody + @"
                                                  <table class=""courses-table"">
                                                    <thead>
                                                        <tr>
                                                            <th class=""red""></th>
                                                       
                                                            <th class=""red"">Employee</th>
                                                            <th class=""red"">Building</th>
                                                            <th class=""red"">Room</th>
                                                          
                                                           
                                                          
                                                        </tr>
                                                  
                                                           <tr>
                                                            <td class=""description"">Final</ td >
                                                            <td class=""description"">" + eFin + @" </ td >
                                                            <td class=""description"">" + lFinName + @" </ td >
                                                            <td class=""description"">" + rFinName + @" </ td >
                                                           
                                                        </tr>
  </ thead >
                                                   </ table>"

;
                            }
                           

                            // Transfer //


                            break;
                        case 4:
                            assetOpPrev.RegisterConfAt = DateTime.Now;
                            assetOpPrev.RegisterConfBy = user.Id;
                            break;
                        default:
                            break;
                    }

                    assetOpPrev.AssetOpStateId += 1;
                    _context.UserName = userName;
                    _context.SaveChanges();

                }
            }

            // STANLEYBET //

           

            var eCC = "";
            var eInis = "";

            foreach (var item in emailCC)
            {
                eCC = item;
            }


            var emailMessage = new MimeMessage();

            if (emailIni.Count  > 0)
            {
                InternetAddressList list = new InternetAddressList();

                foreach (var item in emailIni)
                {
                    list.Add(new MailboxAddress(item));
                }

                emailMessage.To.AddRange(list);
            }
            else
            {
                emailMessage.To.Add(new MailboxAddress("", "adrian.cirnaru@optima.ro"));
            }

            emailMessage.From.Add(new MailboxAddress("Transferuri", "ofa@optima.ro"));
            //emailMessage.From.Add(new MailboxAddress("Transferuri", "inventar@stanleybet.ro"));
            //emailMessage.To.Add(new MailboxAddress("", eInis));
            //emailMessage.To.Add(new MailboxAddress("", email1));
            emailMessage.To.Add(new MailboxAddress("", "allianztechnology-ro-it@allianz.com")); // trebuie comentata
            emailMessage.To.Add(new MailboxAddress("", eCC));
            

            emailMessage.Subject = subject;
            //emailMessage.Body = new TextPart("plain") { Text = message };

            var builder = new BodyBuilder { TextBody = htmlBody, HtmlBody = htmlBody };
            //  builder.Attachments.Add(attachmentName, attachment, ContentType.Parse(attachmentType));
            emailMessage.Body = builder.ToMessageBody();

            //using (var client = new SmtpClient())
            //{

            //    await client.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
            //    client.AuthenticationMechanisms.Remove("XOAUTH2");

            //    await client.AuthenticateAsync("ofa@optima.ro", "Inventory2019");
            //    await client.SendAsync(emailMessage).ConfigureAwait(false);
            //    await client.DisconnectAsync(true).ConfigureAwait(false);


            //}


            // STANLEYBET

            return Ok(assetOpIds);
        }


        [HttpPost]
        [Route("exportAssetOps")]
        [Authorize]
        public virtual IActionResult ExportAssetOps([FromBody] int assetOpId)
        {
            
            Model.AssetOp assetOp = null;
          
                assetOp = _context.Set<Model.AssetOp>().Where(a => a.Id == assetOpId).SingleOrDefault();
               


                if (assetOp.AssetOpStateId < 5)
                {
                    string userName = HttpContext.User.Identity.Name;
                    var user = _context.Users.Where(u => u.UserName == userName).Single();

                    switch (assetOp.AssetOpStateId)
                    {
                        case 1:
                            assetOp.ReleaseConfAt = DateTime.Now;
                            assetOp.ReleaseConfBy = user.Id;
                            break;
                        case 2:
                            assetOp.SrcConfAt = DateTime.Now;
                            assetOp.SrcConfBy = user.Id;
                            break;
                        case 3:
                            assetOp.DstConfAt = DateTime.Now;
                            assetOp.DstConfBy = user.Id;
                            break;
                        case 4:
                            assetOp.RegisterConfAt = DateTime.Now;
                            assetOp.RegisterConfBy = user.Id;
                            break;
                        default:
                            break;
                    }

                    assetOp.AssetOpStateId += 1;
                    _context.UserName = userName;
                    _context.SaveChanges();
                }
            



            return Ok(assetOpId);
        }

        [HttpPost("importConfirmation")]
        [Authorize]
        public virtual IActionResult ImportMailConfirmation([FromBody] AssetOpConfirmUpload assetOpConfirm)
        {
            if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;

            return Ok((_itemsRepository as IAssetOpsRepository).AssetOpConfImport(assetOpConfirm, _context.UserName));

        }

        [HttpPost("upload")]
        public IActionResult Import(IFormFile file)
        {
            

            MemoryStream ms = null;
          //  ApplicationDbContext context = new ApplicationDbContext();
          //  EfRepository.AssetOpsRepository repo = new EfRepository.AssetOpsRepository(context);

            string companyCode = "PIRAEUS";

            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");

            ms = new MemoryStream();
            file.CopyTo(ms);

            switch(companyCode)
            {
                case "BNR":
                    ImportBnr(ms);
                    break;
                case "PIRAEUS":
                    ImportPiraeus(ms);
                    break;
            }



            return Ok();
        }

        private void ImportBnr(MemoryStream ms)
        {
            List<Dto.AssetOpConfirmUpload> assetsOpConfirm = null;
            assetsOpConfirm = new List<AssetOpConfirmUpload>();

            using (ExcelPackage package = new ExcelPackage(ms))
            {
                for (int j = 1; j < package.Workbook.Worksheets.Count + 1; j++)
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[j];

                    int rows = worksheet.Dimension.End.Row;
                    for (int i = 2; i <= rows; i++)
                    {
                        Dto.AssetOpConfirmUpload assetOpConfirm = new AssetOpConfirmUpload();


                        assetOpConfirm.Name = worksheet.Cells[i, 1].Text;
                        assetOpConfirm.Quantity = float.Parse(worksheet.Cells[i, 2].Text);
                        assetOpConfirm.InvNo = worksheet.Cells[i, 3].Text;
                        assetOpConfirm.ValueInv = decimal.Parse(worksheet.Cells[i, 4].Text);
                        assetOpConfirm.LocationNameInitial = worksheet.Cells[i, 5].Text;
                        assetOpConfirm.RoomNameInitial = worksheet.Cells[i, 6].Text;
                        assetOpConfirm.EmployeeFullNameInitial = worksheet.Cells[i, 7].Text;
                        assetOpConfirm.EmployeeInternalCodeInitial = worksheet.Cells[i, 8].Text;
                        assetOpConfirm.LocationNameFinal = worksheet.Cells[i, 9].Text;
                        assetOpConfirm.RoomNameFinal = worksheet.Cells[i, 10].Text;
                        assetOpConfirm.EmployeeFullNameFinal = worksheet.Cells[i, 11].Text;
                        assetOpConfirm.EmployeeInternalCodeFinal = worksheet.Cells[i, 12].Text;
                        assetOpConfirm.Confirm = worksheet.Cells[i, 13].Text;
                        assetOpConfirm.AssetOpId = int.Parse(worksheet.Cells[i, 14].Text);
                        //asset.InvNo = worksheet.Cells[i, 1].Text;
                        //asset.Name = worksheet.Cells[i, 3].Text;
                        //asset.Administration = worksheet.Cells[i, 4].Text;
                        //asset.AssetCategory = worksheet.Cells[i, 5].Text;
                        //asset.AssetType = worksheet.Cells[i, 6].Text;

                        //asset.Employee = worksheet.Cells[i, 8].Text;

                        //asset.RegionCode = worksheet.Cells[i, 9].Text;
                        //asset.LocationCode = worksheet.Cells[i, 10].Text;
                        //asset.RoomCode = worksheet.Cells[i, 11].Text;

                        //asset.CostCenterCode = worksheet.Cells[i, 12].Text;
                        //asset.AdmCenterCode = worksheet.Cells[i, 13].Text;

                        //asset.Account = worksheet.Cells[i, 15].Text;
                        //asset.PurchaseDate = worksheet.Cells[i, 16].GetValue<DateTime>();

                        //asset.ValueInv = worksheet.Cells[i, 18].GetValue<decimal>();
                        //asset.ValueDep = 0 - worksheet.Cells[i, 19].GetValue<decimal>();
                        //asset.ValueRem = worksheet.Cells[i, 20].GetValue<decimal>();

                        //asset.ValueDepYTD = worksheet.Cells[i, 24].GetValue<decimal>();
                        //asset.Quantity = 1;

                        //(_itemsRepository as IAssetsRepository).AssetImportV4(asset);
                        //repo.AssetImportV4(asset);

                        assetsOpConfirm.Add(assetOpConfirm);
                    }
                }
                

                
            }

            foreach (Dto.AssetOpConfirmUpload assetOpConfirm in assetsOpConfirm)
            {
                //repo.AssetImportV4(asset);
                if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
                Ok((_itemsRepository as IAssetOpsRepository).AssetOpConfImportBnr(assetOpConfirm, _context.UserName));
            }
        }

        private void ImportPiraeus(MemoryStream ms)
        {
            List<Dto.AssetOpConfirmUpload> assetsOpConfirm = null;
            assetsOpConfirm = new List<AssetOpConfirmUpload>();

            using (ExcelPackage package = new ExcelPackage(ms))
            {
                for (int j = 1; j < package.Workbook.Worksheets.Count + 1; j++)
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[j];

                    //int rows = worksheet.Dimension.End.Row;
                    //for (int i = 2; i <= rows; i++)
                    //{
                    //    Dto.AssetOpConfirmUpload assetOpConfirm = new AssetOpConfirmUpload();

                    //    assetOpConfirm.Confirm = worksheet.Cells[i, 12].Text;
                    //    assetOpConfirm.AssetOpId = int.Parse(worksheet.Cells[i, 13].Text);
                    //    assetsOpConfirm.Add(assetOpConfirm);
                    //}  BNR
                    
                    int rows = worksheet.Dimension.End.Row - 4;

                    if (j != 1)
                    {
                        for (int i = 25; i <= rows; i++)
                        {
                           
                            Dto.AssetOpConfirmUpload assetOpConfirm = new AssetOpConfirmUpload();

                            assetOpConfirm.Confirm = worksheet.Cells[i, 28].Text;
                            assetOpConfirm.AssetOpId = int.Parse(worksheet.Cells[i, 29].Text);
                            assetsOpConfirm.Add(assetOpConfirm);
                        }
                    }
                    else
                    {
                        for (int i = 26; i <= rows; i++)
                        {
                          
                            Dto.AssetOpConfirmUpload assetOpConfirm = new AssetOpConfirmUpload();

                            assetOpConfirm.Confirm = worksheet.Cells[i, 28].Text;
                            assetOpConfirm.AssetOpId = int.Parse(worksheet.Cells[i, 29].Text);
                            assetsOpConfirm.Add(assetOpConfirm);
                           

                        }
                    }

                }



            }

            foreach (Dto.AssetOpConfirmUpload assetOpConfirm in assetsOpConfirm)
            {
                //repo.AssetImportV4(asset);
                if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
                Ok((_itemsRepository as IAssetOpsRepository).AssetOpConfImportPiraeus(assetOpConfirm, _context.UserName));
            }
        }

        [HttpPost]
        [Route("sendEmail")]
        public async Task SendEmailAsync([FromBody] AssetOpConf[] operations)
        {
            Export();

               void Export()
            {
                //string sWebRootFolder = hostingEnvironment.WebRootPath;
                string sWebRootFolder = hostingEnvironment.WebRootPath;
                Console.WriteLine(sWebRootFolder);
                string sFileName = @"demo.xlsx";
                string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));



                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                }
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    //var package = new ExcelPackage();

                    


                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Nota_transfer");
                    //First add the headers
                    worksheet.Cells[1, 1].Value = "OptimaId";
                    worksheet.Cells[1, 2].Value = "Numar inventar plecare";
                    worksheet.Cells[1, 3].Value = "Denumire";
                    worksheet.Cells[1, 4].Value = "Centru de cost plecare";
                    worksheet.Cells[1, 5].Value = "Cladire plecare";
                    worksheet.Cells[1, 6].Value = "Centru de cost destinatie";
                    worksheet.Cells[1, 7].Value = "Cladire destinatie";
                    worksheet.Cells[1, 8].Value = "Confirmat";
                    worksheet.Cells[1, 9].Value = "Numar inventar primit";
                    worksheet.Cells[1, 10].Value = "Observatii";
                    worksheet.Cells[1, 11].Value = "Instructiuni";
                    worksheet.Cells[12, 11].Value = "Primite in plus";
                    worksheet.Cells[13, 11].Value = "Denumire produs";
                    worksheet.Cells[13, 14].Value = "Descriere obiect";
                    worksheet.Cells[13, 17].Value = "Numar inventar";


                    int recordIndex = 2;
                    foreach (var item in operations)
                    {
                        worksheet.Cells[recordIndex, 1].Value = item.AssetOpId;
                        worksheet.Cells[recordIndex, 2].Value = item.InvNo;
                        worksheet.Cells[recordIndex, 3].Value = item.AssetName;
                        worksheet.Cells[recordIndex, 4].Value = item.RoomCodeIni;
                        worksheet.Cells[recordIndex, 5].Value = item.LocationCodeIni;
                        worksheet.Cells[recordIndex, 6].Value = item.RoomCodeFin;
                        worksheet.Cells[recordIndex, 7].Value = item.LocationCodeFin;
                        worksheet.Cells[recordIndex, 8].Value = "Da";
                        worksheet.Cells[recordIndex, 9].Value = item.InvNo;
                        worksheet.Cells[recordIndex, 11].Value = @"Pe coloana Confirmat se va completa cu :
                                         'NU' daca numarul de inventar nu a ajuns la destinatie si cu 
                                         'Da' daca a ajuns.
                                          Implicit este completat cu 'Da'. 
                              In situatia in care numarul de inventar nu corespunde cu cel initial 
                              se va completa pe coloana 'Numar de inventar primit' numarul existent.
                              Pe coloana Observatii se va completa 
                              daca obiectul este functional, defect, etc.
                              Daca exista obiecte care nu se regasesc in lista  
                              acestea se vor completa in zona 'Primite in plus'";
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
                    worksheet.Column(10).AutoFit();
                    worksheet.Cells["K1:R1"].Merge = true;
                    worksheet.Cells["K1:R1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    worksheet.Cells["K2:R10"].Merge = true;
                    worksheet.Cells["K2:R10"].Style.WrapText = true;




                    worksheet.Cells["K12:R12"].Merge = true;
                    worksheet.Cells["K12:R12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


                    worksheet.Cells["K13:M13"].Merge = true;
                    worksheet.Cells["K13:R13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    worksheet.Cells["N13:P13"].Merge = true;
                    worksheet.Cells["N13:P13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    worksheet.Cells["Q13:R13"].Merge = true;
                    worksheet.Cells["Q13:R13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


                    for (int i = 14; i < 1000; i++)
                    {
                        worksheet.Cells[i, 11, i, 13].Merge = true;
                        worksheet.Cells[i, 14, i, 16].Merge = true;
                        worksheet.Cells[i, 17, i, 18].Merge = true;
                    }

                    worksheet.Protection.IsProtected = true;
                    worksheet.Column(8).Style.Locked = false;
                    worksheet.Column(9).Style.Locked = false;
                    worksheet.Column(10).Style.Locked = false;
                    worksheet.Cells["$K$14:$K$1000"].Style.Locked = false;
                    worksheet.Cells["$L$14:$L$1000"].Style.Locked = false;
                    worksheet.Cells["$M$14:$M$1000"].Style.Locked = false;
                    worksheet.Cells["$N$14:$N$1000"].Style.Locked = false;
                    worksheet.Cells["$O$14:$O$1000"].Style.Locked = false;
                    worksheet.Cells["$P$14:$P$1000"].Style.Locked = false;
                    worksheet.Cells["$Q$14:$Q$1000"].Style.Locked = false;
                    worksheet.Cells["$R$14:$R$1000"].Style.Locked = false;


                    //worksheet.Cells["13", "K", "1000", "K"].Merge = true;

                    worksheet.Protection.SetPassword("piraeus");


                    var dropdownlist = worksheet.DataValidations.AddListValidation("$H$2:$H$10000");
                    dropdownlist.Formula.Values.Add("Da");
                    dropdownlist.Formula.Values.Add("Nu");

                    var dropdownlistObs = worksheet.DataValidations.AddListValidation("$J$2:$J$10000");
                    dropdownlistObs.Formula.Values.Add("DEFECT");
                    dropdownlistObs.Formula.Values.Add("-");

                    //dropdownlist.Formula.ExcelFormula = "=$H$2:$H$1000000!Da";
                    //dropdownlist.ShowErrorMessage = true;
                    ////dropdownlist.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                    ////dropdownlist.ErrorTitle = "Error";
                    //dropdownlist.Error = "Error Text";


                    //var validation = worksheet.DataValidations.AddListValidation("H2");
                    //validation.Formula.Values.Add("Da");
                    //validation.Formula.Values.Add("Nu");
                    //validation.ShowErrorMessage = true;
                    //validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                    //validation.ErrorTitle = "Error";
                    //validation.Error = "Error Text";
                    //// sheet with a name : DropDownLists 
                    //// from DropDownLists sheet, get values from cells: !$A$1:$A$10
                    //var formula = "=Nota_transfer!$H$2:$H$2";
                    ////Applying Formula to the range
                    //validation.Formula.ExcelFormula = formula;
                    //Process.Start("mailto:hello@test.com&subject=This is the subject&body=This is the body");




                    using (var cells = worksheet.Cells[1, 1, 1, 10])
                    {
                        cells.Style.Font.Bold = true;
                        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cells.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    }

                    using (var cells = worksheet.Cells[2, 11, 10, 18])
                    {
                        cells.Style.Font.Bold = true;
                        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cells.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    }

                    using (var cells = worksheet.Cells[1, 11, 1, 18])
                    {
                        cells.Style.Font.Bold = true;
                        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cells.Style.Fill.BackgroundColor.SetColor(Color.Red);

                    }

                    using (var cells = worksheet.Cells[12, 11, 12, 18])
                    {
                        cells.Style.Font.Bold = true;
                        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cells.Style.Fill.BackgroundColor.SetColor(Color.Red);

                    }

                    using (var cells = worksheet.Cells[13, 11, 13, 18])
                    {
                        cells.Style.Font.Bold = true;
                        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cells.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    }

                    //using (FileStream aFile = new FileStream(@"D:\asdf.xlsx", FileMode.Create))
                    //{
                    //    aFile.Seek(0, SeekOrigin.Begin);
                    //    package.SaveAs(aFile);

                    //}

                   // package.Save();
                    //package.SaveAs(new FileInfo(sFileName));
                    
                    Response.Clear();
                    Response.Headers.Add("content-disposition", "attachment;  filename=demo.xlsx");
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var bytes = package.GetAsByteArray();
                    Response.Body.WriteAsync(bytes, 0, bytes.Length);

                    
                }
            }

            

            var attachmentName = "Transferuri.xlsx";
            //var package = new ExcelPackage();

            //ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Nota_transfer");
            ////First add the headers
            //worksheet.Cells[1, 1].Value = "OptimaId";
            //worksheet.Cells[1, 2].Value = "Numar inventar plecare";
            //worksheet.Cells[1, 3].Value = "Denumire";
            //worksheet.Cells[1, 4].Value = "Centru de cost plecare";
            //worksheet.Cells[1, 5].Value = "Cladire plecare";
            //worksheet.Cells[1, 6].Value = "Centru de cost destinatie";
            //worksheet.Cells[1, 7].Value = "Cladire destinatie";
            //worksheet.Cells[1, 8].Value = "Confirmat";
            //worksheet.Cells[1, 9].Value = "Numar inventar primit";
            //worksheet.Cells[1, 10].Value = "Observatii";
            //worksheet.Cells[1, 11].Value = "Instructiuni";
            //worksheet.Cells[12, 11].Value = "Primite in plus";
            //worksheet.Cells[13, 11].Value = "Denumire produs";
            //worksheet.Cells[13, 14].Value = "Descriere obiect";
            //worksheet.Cells[13, 17].Value = "Numar inventar";


            //int recordIndex = 2;
            //foreach (var item in operations)
            //{
            //    worksheet.Cells[recordIndex, 1].Value = item.AssetOpId;
            //    worksheet.Cells[recordIndex, 2].Value = item.InvNo;
            //    worksheet.Cells[recordIndex, 3].Value = item.AssetName;
            //    worksheet.Cells[recordIndex, 4].Value = item.RoomCodeIni;
            //    worksheet.Cells[recordIndex, 5].Value = item.LocationCodeIni;
            //    worksheet.Cells[recordIndex, 6].Value = item.RoomCodeFin;
            //    worksheet.Cells[recordIndex, 7].Value = item.LocationCodeFin;
            //    worksheet.Cells[recordIndex, 8].Value = "Da";
            //    worksheet.Cells[recordIndex, 9].Value = item.InvNo;
            //    worksheet.Cells[recordIndex, 11].Value = @"Pe coloana Confirmat se va completa cu :
            //                             'NU' daca numarul de inventar nu a ajuns la destinatie si cu 
            //                             'Da' daca a ajuns.
            //                              Implicit este completat cu 'Da'. 
            //                  In situatia in care numarul de inventar nu corespunde cu cel initial 
            //                  se va completa pe coloana 'Numar de inventar primit' numarul existent.
            //                  Pe coloana Observatii se va completa 
            //                  daca obiectul este functional, defect, etc.
            //                  Daca exista obiecte care nu se regasesc in lista  
            //                  acestea se vor completa in zona 'Primite in plus'";
            //    recordIndex++;
            //}

            //worksheet.Column(1).AutoFit();
            //worksheet.Column(2).AutoFit();
            //worksheet.Column(3).AutoFit();
            //worksheet.Column(4).AutoFit();
            //worksheet.Column(5).AutoFit();
            //worksheet.Column(6).AutoFit();
            //worksheet.Column(7).AutoFit();
            //worksheet.Column(8).AutoFit();
            //worksheet.Column(9).AutoFit();
            //worksheet.Column(10).AutoFit();
            //worksheet.Cells["K1:R1"].Merge = true;
            //worksheet.Cells["K1:R1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            //worksheet.Cells["K2:R10"].Merge = true;
            //worksheet.Cells["K2:R10"].Style.WrapText = true;




            //worksheet.Cells["K12:R12"].Merge = true;
            //worksheet.Cells["K12:R12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


            //worksheet.Cells["K13:M13"].Merge = true;
            //worksheet.Cells["K13:R13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            //worksheet.Cells["N13:P13"].Merge = true;
            //worksheet.Cells["N13:P13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            //worksheet.Cells["Q13:R13"].Merge = true;
            //worksheet.Cells["Q13:R13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


            //for (int i = 14; i < 1000; i++)
            //{
            //    worksheet.Cells[i, 11, i, 13].Merge = true;
            //    worksheet.Cells[i, 14, i, 16].Merge = true;
            //    worksheet.Cells[i, 17, i, 18].Merge = true;
            //}

            //worksheet.Protection.IsProtected = true;
            //worksheet.Column(8).Style.Locked = false;
            //worksheet.Column(9).Style.Locked = false;
            //worksheet.Column(10).Style.Locked = false;
            //worksheet.Cells["$K$14:$K$1000"].Style.Locked = false;
            //worksheet.Cells["$L$14:$L$1000"].Style.Locked = false;
            //worksheet.Cells["$M$14:$M$1000"].Style.Locked = false;
            //worksheet.Cells["$N$14:$N$1000"].Style.Locked = false;
            //worksheet.Cells["$O$14:$O$1000"].Style.Locked = false;
            //worksheet.Cells["$P$14:$P$1000"].Style.Locked = false;
            //worksheet.Cells["$Q$14:$Q$1000"].Style.Locked = false;
            //worksheet.Cells["$R$14:$R$1000"].Style.Locked = false;


            ////worksheet.Cells["13", "K", "1000", "K"].Merge = true;

            //worksheet.Protection.SetPassword("");


            //var dropdownlist = worksheet.DataValidations.AddListValidation("$H$2:$H$10000");
            //dropdownlist.Formula.Values.Add("Da");
            //dropdownlist.Formula.Values.Add("Nu");

            //var dropdownlistObs = worksheet.DataValidations.AddListValidation("$J$2:$J$10000");
            //dropdownlistObs.Formula.Values.Add("DEFECT");
            //dropdownlistObs.Formula.Values.Add("-");
     
            ////dropdownlist.Formula.ExcelFormula = "=$H$2:$H$1000000!Da";
            ////dropdownlist.ShowErrorMessage = true;
            //////dropdownlist.ErrorStyle = ExcelDataValidationWarningStyle.stop;
            //////dropdownlist.ErrorTitle = "Error";
            ////dropdownlist.Error = "Error Text";


            ////var validation = worksheet.DataValidations.AddListValidation("H2");
            ////validation.Formula.Values.Add("Da");
            ////validation.Formula.Values.Add("Nu");
            ////validation.ShowErrorMessage = true;
            ////validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
            ////validation.ErrorTitle = "Error";
            ////validation.Error = "Error Text";
            ////// sheet with a name : DropDownLists 
            ////// from DropDownLists sheet, get values from cells: !$A$1:$A$10
            ////var formula = "=Nota_transfer!$H$2:$H$2";
            //////Applying Formula to the range
            ////validation.Formula.ExcelFormula = formula;
            ////Process.Start("mailto:hello@test.com&subject=This is the subject&body=This is the body");




            //using (var cells = worksheet.Cells[1, 1, 1, 10])
            //{
            //    cells.Style.Font.Bold = true;
            //    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    cells.Style.Fill.BackgroundColor.SetColor(Color.Orange);
                
            //}

            //using (var cells = worksheet.Cells[2, 11, 10, 18])
            //{
            //    cells.Style.Font.Bold = true;
            //    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    cells.Style.Fill.BackgroundColor.SetColor(Color.Orange);

            //}

            //using (var cells = worksheet.Cells[1, 11, 1, 18])
            //{
            //    cells.Style.Font.Bold = true;
            //    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    cells.Style.Fill.BackgroundColor.SetColor(Color.Red);

            //}

            //using (var cells = worksheet.Cells[12, 11, 12, 18])
            //{
            //    cells.Style.Font.Bold = true;
            //    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    cells.Style.Fill.BackgroundColor.SetColor(Color.Red);

            //}

            //using (var cells = worksheet.Cells[13, 11, 13, 18])
            //{
            //    cells.Style.Font.Bold = true;
            //    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    cells.Style.Fill.BackgroundColor.SetColor(Color.Orange);

            //}



            //var text = package.GetAsByteArray();

            //byte[] attachment = text;
            var attachmentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //var email = "Anca.Timofti@piraeusbank.ro";
            var email = "adrian.cirnaru@optima.ro";
            //var emailBnr = "Mihaela.Matei@bnro.ro";
            // var email1 = "Daniel.Enache@piraeusbank.ro";
            //var email2 = "Vlad.Matei@piraeusbank.ro";
            //var email3 = "Emanuel.Odobescu@piraeusbank.ro";
            var subject = "Transferuri pentru confirmare";
            var message= @"
            Va rugam sa deschideti fisierul atasat.
            Verificati daca ati receptionat toate obiectele trimise si confirmati cu DA / NU.
            Salvati fisierul si trimiteti forward catre mijloacefixe@piraeusbank.ro";

            var emailMessage = new MimeMessage();

            //mailMessage.From.Add(new MailboxAddress("Adrian Cirnaru", "transferuri@piraeusbank.ro"));
            emailMessage.From.Add(new MailboxAddress("Adrian Cirnaru", "adrian.cirnaru@optima.ro"));
            emailMessage.To.Add(new MailboxAddress("", email));
           // emailMessage.To.Add(new MailboxAddress("", emailBnr));


            //emailMessage.To.Add(new MailboxAddress("", email1));
            //emailMessage.To.Add(new MailboxAddress("", email2));
            //emailMessage.To.Add(new MailboxAddress("", email3));
            emailMessage.Subject = subject;
            //emailMessage.Body = new TextPart("plain") { Text = message };

            var builder = new BodyBuilder { TextBody = message };
            //builder.Attachments.Add(attachmentName, attachment, ContentType.Parse(attachmentType));
            emailMessage.Body = builder.ToMessageBody();

            //using (FileStream aFile = new FileStream(@"D:\asdf.xlsx", FileMode.Create))
            //{


            //    //package.SaveAs(aFile);

            //}

            // See here - I can still work with the spread sheet.



            // Gmail

            //using (var client = new SmtpClient())
            //{
            //    client.LocalDomain = "smtp.office365.com";
            //    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
            //    await client.AuthenticateAsync("carnaruadrian@gmail.com", "cth589589LG");
            //    await client.SendAsync(emailMessage).ConfigureAwait(false);
            //    await client.DisconnectAsync(true).ConfigureAwait(false);
            //}

            //var emailMessage1 = new MimeMessage();

            //emailMessage1.From.Add(new MailboxAddress("Joe Bloggs", "jbloggs@example.com"));
            //emailMessage1.To.Add(new MailboxAddress("", email));
            //emailMessage1.Subject = subject;
            //emailMessage1.Body = new TextPart("plain") { Text = message };

            //using (StreamWriter data = System.IO.File.CreateText("D:\\Install.txt"))
            //{
            //    emailMessage.WriteTo(data.BaseStream);
            //    Process.Start("cmd", "/C start mailto:hello@test.com?subject=Thisisthesubject?body=?attachment=" + "D:\\Install.txt");
            //}


            // Set the file name and get the output directory







            // Office365




            //using (var client = new SmtpClient())
            //{
            //    //Process.Start(String.Format(
            //    //     "mailto: someone @example.com? Subject = Hello % 20again"));
            //    await client.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
            //    client.AuthenticationMechanisms.Remove("XOAUTH2");

            //    await client.AuthenticateAsync("adrian.cirnaru@optima.ro", "Adcr3386");
            //    await client.SendAsync(emailMessage).ConfigureAwait(false);
            //    await client.DisconnectAsync(true).ConfigureAwait(false);
            //    //Process.Start("cmd", "/C start mailto:");
            //}

            // Piraeus intern

            //using (var client = new SmtpClient())
            //{
            //    client.LocalDomain = "dag.pbr.ro";
            //    await client.ConnectAsync("dag.pbr.ro", 25, SecureSocketOptions.Auto).ConfigureAwait(false);
            //    client.AuthenticationMechanisms.Remove("XOAUTH2");
            //    //await client.AuthenticateAsync("transferuri@piraeusbank.ro", "");
            //    await client.SendAsync(emailMessage).ConfigureAwait(false);
            //    await client.DisconnectAsync(true).ConfigureAwait(false);


            //}


            //using (var client = new ImapClient())
            //{

            //    client.Connect("outlook.office365.com", 993, true);
            //    client.AuthenticationMechanisms.Remove("XOAUTH2");
            //    client.Authenticate("adrian.cirnaru@optima.ro", "Adcr3386");

            //    var inbox = client.Inbox;
            //    inbox.Open(FolderAccess.ReadOnly);
            //    inbox.Search(SearchQuery.All);


            //    using (var clientSmtp = new SmtpClient())
            //    {



            //        await clientSmtp.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
            //        client.AuthenticationMechanisms.Remove("XOAUTH2");

            //        await clientSmtp.AuthenticateAsync("adrian.cirnaru@optima.ro", "Adcr3386");
            //        await clientSmtp.SendAsync(emailMessage).ConfigureAwait(false);
            //        await clientSmtp.DisconnectAsync(true).ConfigureAwait(false);
            //    }

            //    //Console.WriteLine("Total messages: {0}", inbox.Count);
            //    //Console.WriteLine("Recent messages: {0}", inbox.Recent);
            //    client.Disconnect(true);
            //}



        }

        [HttpPost]
        [Route("sendEmailBnr")]
        public async Task SendEmailAsyncBnr([FromBody] AssetOpConf[] operations)
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"Transferuri.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));



            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }
           
            var package = new ExcelPackage();

            Dictionary<string, int> locationIndexes = new Dictionary<string, int>();
            Dictionary<string, int> locationNames = new Dictionary<string, int>();
            int recordIndex = 0;

            foreach (var item in operations)
            {
                string sheetName = item.LocationCodeFin;

                ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];

                worksheet = package.Workbook.Worksheets[sheetName];


                if (worksheet == null)
                {
                    worksheet = package.Workbook.Worksheets.Add(sheetName);
                    recordIndex = 2;
                    locationIndexes.Add(sheetName, recordIndex);

                    locationNames.Add(item.LocationCodeFin, recordIndex);


                    worksheet.Cells[1, 1].Value = "Denumire bun";
                    worksheet.Cells[1, 2].Value = "Cantitate";
                    worksheet.Cells[1, 3].Value = "Numar inventar";
                    worksheet.Cells[1, 4].Value = "Valoare de inventar";
                    worksheet.Cells[1, 5].Value = "Directia predatoare";
                    worksheet.Cells[1, 6].Value = "Camera predatoare";
                    worksheet.Cells[1, 7].Value = "Nume predator";
                    worksheet.Cells[1, 8].Value = "Numar legitimatie predator";
                    worksheet.Cells[1, 9].Value = "Directia primitoare";
                    worksheet.Cells[1, 10].Value = "Camera primitoare";
                    worksheet.Cells[1, 11].Value = "Nume primitor";
                    worksheet.Cells[1, 12].Value = "Numar legitimatie primitor";
                    worksheet.Cells[1, 13].Value = "Confirm";
                    worksheet.Cells[1, 14].Value = "OptimaId";
                }
                else
                {
                    recordIndex = locationIndexes[sheetName];
                    recordIndex--;
                }

                worksheet.Cells[recordIndex, 1].Value = item.AssetName;
                worksheet.Cells[recordIndex, 2].Value = item.Quantity;
                worksheet.Cells[recordIndex, 3].Value = item.InvNo;
                worksheet.Cells[recordIndex, 4].Value = item.ValueInv;
                worksheet.Cells[recordIndex, 5].Value = item.LocationCodeIni;
                worksheet.Cells[recordIndex, 6].Value = item.RoomCodeIni;
                worksheet.Cells[recordIndex, 7].Value = item.EmployeeFirstNameInitial + " " + item.EmployeeLastNameInitial;
                worksheet.Cells[recordIndex, 8].Value = item.EmployeeInternalCodeInitial;
                worksheet.Cells[recordIndex, 9].Value = item.LocationCodeFin;
                worksheet.Cells[recordIndex, 10].Value = item.RoomCodeFin;
                worksheet.Cells[recordIndex, 11].Value = item.EmployeeFirstNameFinal + " " + item.EmployeeLastNameFinal;
                worksheet.Cells[recordIndex, 12].Value = item.EmployeeInternalCodeFinal;
                worksheet.Cells[recordIndex, 13].Value = "Nu";
                worksheet.Cells[recordIndex, 14].Value = item.AssetOpId;
                //worksheet.Cells["C3:F3"].Value = item.LocationCodeIni;
                //worksheet.Cells["C4:F4"].Value = item.LocationCodeFin;
                worksheet.View.ShowGridLines = false;
                worksheet.Cells[recordIndex, 1].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 2].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 3].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 4].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 5].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 6].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 7].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 8].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 9].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 10].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 11].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 12].Style.Numberformat.Format = "@";



                for (int i = 1; i < 13; i++)
                {
                    worksheet.Cells[recordIndex, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[recordIndex, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[recordIndex, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[recordIndex, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 255, 191));
                    worksheet.Cells[recordIndex, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));
                }

                for (int i = 13; i < 14; i++)
                {
                    worksheet.Cells[recordIndex, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[recordIndex, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[recordIndex, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[recordIndex, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(178, 148, 83));
                    worksheet.Cells[recordIndex, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));
                }

                for (int i = 14; i < 15; i++)
                {
                    worksheet.Cells[recordIndex, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[recordIndex, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[recordIndex, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[recordIndex, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 232, 184));
                    worksheet.Cells[recordIndex, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));

                }

                for (int i = 1; i < 14; i++)
                {
                    worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(159, 178, 78));
                    worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));
                    // worksheet.Cells.Style.Font.Bold = true;
                }

                var startRow = 2;
                var startColumn = 1;
                var endRow = operations.Length;
                var endColumn = 12;
                var sortColumn = 9;
                using (ExcelRange excelRange = worksheet.Cells[startRow, startColumn, endRow, endColumn])
                {
                    excelRange.Sort(sortColumn, true);
                }

                worksheet.View.FreezePanes(2, 1);


                recordIndex++;

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                worksheet.Column(10).AutoFit();
                worksheet.Column(11).AutoFit();
                worksheet.Column(12).AutoFit();
                worksheet.Column(13).AutoFit();
                worksheet.Column(14).AutoFit();

                //worksheet.Protection.IsProtected = true;
                //worksheet.Column(11).Style.Locked = false;
                //worksheet.Column(12).Style.Locked = false;
                worksheet.Column(14).Hidden = true;
                worksheet.Row(1).Height = 20;
                worksheet.Column(4).Style.Numberformat.Format = "0.00";

                //worksheet.Cells["$K$14:$K$1000"].Style.Locked = false;
                //worksheet.Cells["$L$14:$L$1000"].Style.Locked = false;
                //worksheet.Cells["$M$14:$M$1000"].Style.Locked = false;
                //worksheet.Cells["$N$14:$N$1000"].Style.Locked = false;
                //worksheet.Cells["$O$14:$O$1000"].Style.Locked = false;
                //worksheet.Cells["$P$14:$P$1000"].Style.Locked = false;
                //worksheet.Cells["$Q$14:$Q$1000"].Style.Locked = false;
                //worksheet.Cells["$R$14:$R$1000"].Style.Locked = false;


                //worksheet.Cells["13", "K", "1000", "K"].Merge = true;

                //  worksheet.Protection.SetPassword("bnr");

                var validations = worksheet.DataValidations;
                if (validations.Count > 0)
                {

                }
                else
                {
                    int rowNumber = recordIndex - 1;
                    //var dropdownlist = worksheet.DataValidations.AddListValidation("$M$" + rowNumber + ":$M$" + operations.Length);
                    //dropdownlist.Formula.Values.Add("Da");
                    //dropdownlist.Formula.Values.Add("Nu");
                }

               

                recordIndex++;
                locationIndexes[sheetName] = recordIndex;


            }

           

            var text = package.GetAsByteArray();

            byte[] attachment = text;
            var attachmentName = sFileName;
            var attachmentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //var email = "Anca.Timofti@piraeusbank.ro";
            var email = "adrian.cirnaru@optima.ro";
            //var email1 = "Daniel.Enache@piraeusbank.ro";
            //var email2 = "Vlad.Matei@piraeusbank.ro";
            //var email3 = "Emanuel.Odobescu@piraeusbank.ro";
            var subject = "Transferuri pentru confirmare";
            var message = @"
            Va rugam sa deschideti fisierul atasat.
            Verificati daca ati receptionat toate obiectele trimise si confirmati cu DA / NU.
            Salvati fisierul si trimiteti forward catre mijloacefixe@piraeusbank.ro";

            var emailMessage = new MimeMessage();

            //mailMessage.From.Add(new MailboxAddress("Adrian Cirnaru", "transferuri@piraeusbank.ro"));
            emailMessage.From.Add(new MailboxAddress("Adrian Cirnaru", "adrian.cirnaru@optima.ro"));
            emailMessage.To.Add(new MailboxAddress("", email));


            //emailMessage.To.Add(new MailboxAddress("", email1));
            //emailMessage.To.Add(new MailboxAddress("", email2));
            //emailMessage.To.Add(new MailboxAddress("", email3));
            emailMessage.Subject = subject;
            //emailMessage.Body = new TextPart("plain") { Text = message };

            var builder = new BodyBuilder { TextBody = message };
            builder.Attachments.Add(attachmentName, attachment, ContentType.Parse(attachmentType));
            emailMessage.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
              
                //await client.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
                //client.AuthenticationMechanisms.Remove("XOAUTH2");

                //await client.AuthenticateAsync("adrian.cirnaru@optima.ro", "Adcr3386");
                //await client.SendAsync(emailMessage).ConfigureAwait(false);
                //await client.DisconnectAsync(true).ConfigureAwait(false);
               
            }

           


        }

        [HttpPost]
        [Route("sendEmailPiraeus")]
        public async Task SendEmailAsyncPiraeus([FromBody] AssetOpConf[] operations)
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"Transferuri.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));



            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }

            var package = new ExcelPackage();

            Dictionary<string, int> locationIndexes = new Dictionary<string, int>();
            Dictionary<string, int> locationNames = new Dictionary<string, int>();
            int recordIndex = 0;

            foreach (var item in operations)
            {
                string sheetName = item.LocationCodeFin;

                ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];

                worksheet = package.Workbook.Worksheets[sheetName];


                if (worksheet == null)
                {
                    worksheet = package.Workbook.Worksheets.Add(sheetName);
                    recordIndex = 2;
                    locationIndexes.Add(sheetName, recordIndex);

                    locationNames.Add(item.LocationCodeFin, recordIndex);


                    worksheet.Cells[1, 1].Value = "Denumire";
                    worksheet.Cells[1, 2].Value = "Cantitate";
                    worksheet.Cells[1, 3].Value = "Numar inventar";
                    worksheet.Cells[1, 4].Value = "Valoare de inventar";
                    worksheet.Cells[1, 5].Value = "Cladire plecare";
                    worksheet.Cells[1, 6].Value = "Centru de cost plecare";
                    worksheet.Cells[1, 7].Value = "Nume predator";
                    worksheet.Cells[1, 8].Value = "Numar legitimatie predator";
                    worksheet.Cells[1, 9].Value = "Cladire destinatie";
                    worksheet.Cells[1, 10].Value = "Centru de cost destinatie";
                    worksheet.Cells[1, 11].Value = "Nume primitor";
                    worksheet.Cells[1, 12].Value = "Numar legitimatie primitor";
                    worksheet.Cells[1, 13].Value = "Confirm";
                    worksheet.Cells[1, 14].Value = "OptimaId";
                }
                else
                {
                    recordIndex = locationIndexes[sheetName];
                    recordIndex--;
                }

                worksheet.Cells[recordIndex, 1].Value = item.AssetName;
                worksheet.Cells[recordIndex, 2].Value = item.Quantity;
                worksheet.Cells[recordIndex, 3].Value = item.InvNo;
                worksheet.Cells[recordIndex, 4].Value = item.ValueInv;
                worksheet.Cells[recordIndex, 5].Value = item.LocationCodeIni;
                worksheet.Cells[recordIndex, 6].Value = item.RoomCodeIni;
                worksheet.Cells[recordIndex, 7].Value = item.EmployeeFirstNameInitial + " " + item.EmployeeLastNameInitial;
                worksheet.Cells[recordIndex, 8].Value = item.EmployeeInternalCodeInitial;
                worksheet.Cells[recordIndex, 9].Value = item.LocationCodeFin;
                worksheet.Cells[recordIndex, 10].Value = item.RoomCodeFin;
                worksheet.Cells[recordIndex, 11].Value = item.EmployeeFirstNameFinal + " " + item.EmployeeLastNameFinal;
                worksheet.Cells[recordIndex, 12].Value = item.EmployeeInternalCodeFinal;
                worksheet.Cells[recordIndex, 13].Value = "Nu";
                worksheet.Cells[recordIndex, 14].Value = item.AssetOpId;
                //worksheet.Cells["C3:F3"].Value = item.LocationCodeIni;
                //worksheet.Cells["C4:F4"].Value = item.LocationCodeFin;
                worksheet.View.ShowGridLines = false;
                worksheet.Cells[recordIndex, 1].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 2].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 3].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 4].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 5].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 6].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 7].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 8].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 9].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 10].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 11].Style.Numberformat.Format = "@";
                worksheet.Cells[recordIndex, 12].Style.Numberformat.Format = "@";



                for (int i = 1; i < 13; i++)
                {
                    worksheet.Cells[recordIndex, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[recordIndex, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[recordIndex, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[recordIndex, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 255, 191));
                    worksheet.Cells[recordIndex, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));
                }

                for (int i = 13; i < 14; i++)
                {
                    worksheet.Cells[recordIndex, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[recordIndex, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[recordIndex, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[recordIndex, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(178, 148, 83));
                    worksheet.Cells[recordIndex, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));
                }

                for (int i = 14; i < 15; i++)
                {
                    worksheet.Cells[recordIndex, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[recordIndex, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[recordIndex, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[recordIndex, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[recordIndex, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 232, 184));
                    worksheet.Cells[recordIndex, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));

                }

                for (int i = 1; i < 14; i++)
                {
                    worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(159, 178, 78));
                    worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));
                    // worksheet.Cells.Style.Font.Bold = true;
                }

                var startRow = 2;
                var startColumn = 1;
                var endRow = operations.Length;
                var endColumn = 12;
                var sortColumn = 9;
                using (ExcelRange excelRange = worksheet.Cells[startRow, startColumn, endRow, endColumn])
                {
                    excelRange.Sort(sortColumn, true);
                }

                worksheet.View.FreezePanes(2, 1);


                recordIndex++;

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                worksheet.Column(10).AutoFit();
                worksheet.Column(11).AutoFit();
                worksheet.Column(12).AutoFit();
                worksheet.Column(13).AutoFit();
                worksheet.Column(14).AutoFit();

                worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                //worksheet.Protection.IsProtected = true;
                //worksheet.Column(11).Style.Locked = false;
                //worksheet.Column(12).Style.Locked = false;
                worksheet.Column(14).Hidden = true;
                worksheet.Row(1).Height = 20;
                worksheet.Column(4).Style.Numberformat.Format = "0.00";

                //worksheet.Cells["$K$14:$K$1000"].Style.Locked = false;
                //worksheet.Cells["$L$14:$L$1000"].Style.Locked = false;
                //worksheet.Cells["$M$14:$M$1000"].Style.Locked = false;
                //worksheet.Cells["$N$14:$N$1000"].Style.Locked = false;
                //worksheet.Cells["$O$14:$O$1000"].Style.Locked = false;
                //worksheet.Cells["$P$14:$P$1000"].Style.Locked = false;
                //worksheet.Cells["$Q$14:$Q$1000"].Style.Locked = false;
                //worksheet.Cells["$R$14:$R$1000"].Style.Locked = false;


                //worksheet.Cells["13", "K", "1000", "K"].Merge = true;

                //  worksheet.Protection.SetPassword("bnr");

                var validations = worksheet.DataValidations;
                if (validations.Count > 0)
                {

                }
                else
                {
                    int rowNumber = recordIndex - 1;
                    if (operations.Length == 1)
                    {
                        var dropdownlist = worksheet.DataValidations.AddListValidation("$M$" + rowNumber + ":$M$" + operations.Length + 1);
                        dropdownlist.Formula.Values.Add("Da");
                        dropdownlist.Formula.Values.Add("Nu");
                    }
                    else
                    {
                        var dropdownlist = worksheet.DataValidations.AddListValidation("$M$" + rowNumber + ":$M$" + operations.Length);
                        dropdownlist.Formula.Values.Add("Da");
                        dropdownlist.Formula.Values.Add("Nu");
                    }
                   
                }



                recordIndex++;
                locationIndexes[sheetName] = recordIndex;


            }



            var text = package.GetAsByteArray();

            byte[] attachment = text;
            var attachmentName = sFileName;
            var attachmentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //var email = "Anca.Timofti@piraeusbank.ro";
            var email = "adrian.cirnaru@optima.ro";
            var email1 = "Daniel.Enache@piraeusbank.ro";
            //var email2 = "Vlad.Matei@piraeusbank.ro";
            //var email3 = "Emanuel.Odobescu@piraeusbank.ro";
            var subject = "Transferuri pentru confirmare";
            var message = @"
            Va rugam sa deschideti fisierul atasat.
            Verificati daca ati receptionat toate obiectele trimise si confirmati cu DA / NU.
            Salvati fisierul si trimiteti forward catre mijloacefixe@piraeusbank.ro";

            var emailMessage = new MimeMessage();

            //mailMessage.From.Add(new MailboxAddress("Adrian Cirnaru", "transferuri@piraeusbank.ro"));
            emailMessage.From.Add(new MailboxAddress("Adrian Cirnaru", "adrian.cirnaru@optima.ro"));
            emailMessage.To.Add(new MailboxAddress("", email));


            emailMessage.To.Add(new MailboxAddress("", email1));
            //emailMessage.To.Add(new MailboxAddress("", email2));
            //emailMessage.To.Add(new MailboxAddress("", email3));
            emailMessage.Subject = subject;
            //emailMessage.Body = new TextPart("plain") { Text = message };

            var builder = new BodyBuilder { TextBody = message };
            builder.Attachments.Add(attachmentName, attachment, ContentType.Parse(attachmentType));
            emailMessage.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {

                //await client.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
                //client.AuthenticationMechanisms.Remove("XOAUTH2");

                //await client.AuthenticateAsync("adrian.cirnaru@optima.ro", "Adcr3386");
                //await client.SendAsync(emailMessage).ConfigureAwait(false);
                //await client.DisconnectAsync(true).ConfigureAwait(false);

            }

            // Piraeus intern

            using (var client = new SmtpClient())
            {
                //client.LocalDomain = "dag.pbr.ro";
                //await client.ConnectAsync("dag.pbr.ro", 25, SecureSocketOptions.Auto).ConfigureAwait(false);
                //client.AuthenticationMechanisms.Remove("XOAUTH2");
                ////await client.AuthenticateAsync("transferuri@piraeusbank.ro", "");
                //await client.SendAsync(emailMessage).ConfigureAwait(false);
                //await client.DisconnectAsync(true).ConfigureAwait(false);


            }


        }

        //public void Export()
        //{
        //    string sWebRootFolder = hostingEnvironment.WebRootPath;
        //    string sFileName = @"demo.xlsx";
        //    string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
        //    FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));


        //    if (file.Exists)
        //    {
        //        file.Delete();
        //        file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //    }
        //    using (ExcelPackage package = new ExcelPackage(file))
        //    {
        //        var costcenters = _context.CostCenters.ToList();
        //        // add a new worksheet to the empty workbook
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Costcenters");
        //        //First add the headers
        //        worksheet.Cells[1, 1].Value = "Id";
        //        worksheet.Cells[1, 2].Value = "Code";
        //        worksheet.Cells[1, 3].Value = "Name";

        //        int recordIndex = 2;
        //        //foreach (var item in costcenters)
        //        //{
        //        //    worksheet.Cells[recordIndex, 1].Value = item.Id;
        //        //    worksheet.Cells[recordIndex, 2].Value = item.Code;
        //        //    worksheet.Cells[recordIndex, 3].Value = item.Name;
        //        //    recordIndex++;
        //        //}

        //        worksheet.Column(1).AutoFit();
        //        worksheet.Column(2).AutoFit();
        //        worksheet.Column(3).AutoFit();

        //        using (var cells = worksheet.Cells[1, 1, 1, 4])
        //        {
        //            cells.Style.Font.Bold = true;
        //            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            cells.Style.Fill.BackgroundColor.SetColor(Color.Aqua);
        //        }

        //        package.Save();
        //        Response.Clear();
        //        Response.Headers.Add("content-disposition", "attachment;  filename=Costcenters.xlsx");
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        var bytes = package.GetAsByteArray();
        //        Response.Body.WriteAsync(bytes, 0, bytes.Length);

        //    }
        //}


        //[HttpPost]
        //[Route("downloadMailOps")]
        //[EnableCors("MyPolicy")]
        //public IActionResult Export1([FromBody] AssetOpConf[] operations)
        //{


        //    string sWebRootFolder = hostingEnvironment.WebRootPath;
        //    //string sWebRootFolder = "D:\\";

        //    string sFileName = @"Nota_transfer.xlsx";
        //    string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
        //    FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));



        //    if (file.Exists)
        //    {
        //        file.Delete();
        //        file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //    }

        //    using (ExcelPackage package = new ExcelPackage(file))
        //    {
        //        //var package = new ExcelPackage();

        //        var path = sWebRootFolder + "\\logo.png";


        //        Dictionary<string, int> locationIndexes = new Dictionary<string, int>();
        //        int recordIndex = 0;

        //        foreach (var item in operations)
        //        {
        //            string sheetName = item.LocationCodeFin + "_Nota_transfer";

        //            ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];
        //            if (worksheet == null)
        //            {
        //                worksheet = package.Workbook.Worksheets.Add(sheetName);
        //                recordIndex = 2;
        //                locationIndexes.Add(sheetName, recordIndex);

        //                //First add the headers
        //                worksheet.Cells[1, 1].Value = "OptimaId";
        //                worksheet.Cells[1, 2].Value = "Numar inventar plecare";
        //                worksheet.Cells[1, 3].Value = "Denumire";
        //                worksheet.Cells[1, 4].Value = "Centru de cost plecare";
        //                worksheet.Cells[1, 5].Value = "Cladire plecare";
        //                worksheet.Cells[1, 6].Value = "Centru de cost destinatie";
        //                worksheet.Cells[1, 7].Value = "Cladire destinatie";
        //                worksheet.Cells[1, 8].Value = "Confirmat";
        //                worksheet.Cells[1, 9].Value = "Numar inventar primit";
        //                worksheet.Cells[1, 10].Value = "Observatii";
        //                worksheet.Cells[1, 11].Value = "Instructiuni";
        //                worksheet.Cells[12, 11].Value = "Primite in plus";
        //                worksheet.Cells[13, 11].Value = "Denumire produs";
        //                worksheet.Cells[13, 14].Value = "Descriere obiect";
        //                worksheet.Cells[13, 17].Value = "Numar inventar";


        //                worksheet.Cells[15, 4].Value = "Fixed Assets & Other Inventories Transfer Form";
        //                worksheet.Cells[16, 2].Value = "Transferred :";
        //                worksheet.Cells[17, 3].Value = "FROM :";
        //                worksheet.Cells[17, 4].Value = "";
        //                worksheet.Cells[18, 3].Value = "TO :";
        //                worksheet.Cells[18, 4].Value = "";

        //                Image logo = Image.FromFile(path);



        //                var picture = worksheet.Drawings.AddPicture("logo.png", logo);
        //                picture.SetPosition(14, 2, 0, 2);

        //            }
        //            else
        //            {
        //                recordIndex = locationIndexes[sheetName];
        //                recordIndex--;
        //            }

        //            worksheet.Cells[recordIndex, 1].Value = item.AssetOpId;
        //            worksheet.Cells[recordIndex, 2].Value = item.InvNo;
        //            worksheet.Cells[recordIndex, 3].Value = item.AssetName;
        //            worksheet.Cells[recordIndex, 4].Value = item.RoomCodeIni;
        //            worksheet.Cells[recordIndex, 5].Value = item.LocationCodeIni;
        //            worksheet.Cells[recordIndex, 6].Value = item.RoomCodeFin;
        //            worksheet.Cells[recordIndex, 7].Value = item.LocationCodeFin;
        //            worksheet.Cells[recordIndex, 8].Value = "Da";
        //            worksheet.Cells[recordIndex, 9].Value = item.InvNo;

        //            for (int i = 2; i < recordIndex; i++)
        //            {
        //                worksheet.Cells[2, i].Style.WrapText = true;
        //            }

        //            worksheet.Cells[recordIndex, 11].Value = @"
        //          Pe coloana Confirmat se va completa cu :
        //                                 'NU' daca numarul de inventar nu a ajuns la destinatie si cu 
        //                                 'Da' daca a ajuns.
        //                                  Implicit este completat cu 'Da'. 
        //          In situatia in care numarul de inventar nu corespunde cu cel initial se va completa 
        //          pe coloana 'Numar de inventar primit' numarul existent.
        //          Pe coloana Observatii se va completa daca obiectul este functional, defect, etc.
        //          Daca exista obiecte care nu se regasesc in lista acestea se vor completa in 
        //          zona 'Primite in plus.'";
        //            recordIndex++;

        //            worksheet.Column(1).AutoFit();
        //            worksheet.Column(2).AutoFit();
        //            worksheet.Column(3).AutoFit();
        //            worksheet.Column(4).AutoFit();
        //            worksheet.Column(5).AutoFit();
        //            worksheet.Column(6).AutoFit();
        //            worksheet.Column(7).AutoFit();
        //            worksheet.Column(8).AutoFit();
        //            worksheet.Column(9).AutoFit();
        //            worksheet.Column(10).AutoFit();
        //            worksheet.Cells["K1:R1"].Merge = true;
        //            worksheet.Cells["K1:R1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

        //            worksheet.Cells["D15:G15"].Merge = true;
        //            worksheet.Cells["D15:G15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
        //            worksheet.Cells["D17:G17"].Merge = true;
        //            worksheet.Cells["D17:G17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
        //            worksheet.Cells["D17:G17"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //            worksheet.Cells["D18:G18"].Merge = true;
        //            worksheet.Cells["D18:G18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
        //            worksheet.Cells["D18:G18"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //            worksheet.Cells["K2:R10"].Merge = true;
        //            worksheet.Cells["K2:R10"].Style.WrapText = true;
        //            worksheet.Cells["K2:R10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
        //            worksheet.Cells["K2:R10"].Style.Font.Size = 10;




        //            worksheet.Cells["K12:R12"].Merge = true;
        //            worksheet.Cells["K12:R12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


        //            worksheet.Cells["K13:M13"].Merge = true;
        //            worksheet.Cells["K13:R13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

        //            worksheet.Cells["N13:P13"].Merge = true;
        //            worksheet.Cells["N13:P13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

        //            worksheet.Cells["Q13:R13"].Merge = true;
        //            worksheet.Cells["Q13:R13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


        //            for (int i = 14; i < 1000; i++)
        //            {
        //                worksheet.Cells[i, 11, i, 13].Merge = true;
        //                worksheet.Cells[i, 14, i, 16].Merge = true;
        //                worksheet.Cells[i, 17, i, 18].Merge = true;
        //            }

        //            worksheet.Protection.IsProtected = true;
        //            worksheet.Column(8).Style.Locked = false;
        //            worksheet.Column(9).Style.Locked = false;
        //            worksheet.Column(10).Style.Locked = false;
        //            worksheet.Cells["$K$14:$K$1000"].Style.Locked = false;
        //            worksheet.Cells["$L$14:$L$1000"].Style.Locked = false;
        //            worksheet.Cells["$M$14:$M$1000"].Style.Locked = false;
        //            worksheet.Cells["$N$14:$N$1000"].Style.Locked = false;
        //            worksheet.Cells["$O$14:$O$1000"].Style.Locked = false;
        //            worksheet.Cells["$P$14:$P$1000"].Style.Locked = false;
        //            worksheet.Cells["$Q$14:$Q$1000"].Style.Locked = false;
        //            worksheet.Cells["$R$14:$R$1000"].Style.Locked = false;


        //            //worksheet.Cells["13", "K", "1000", "K"].Merge = true;

        //            worksheet.Protection.SetPassword("piraeus");

        //            var validations = worksheet.DataValidations;
        //            if (validations.Count > 0)
        //            {

        //            }
        //            else
        //            {
        //                var dropdownlist = worksheet.DataValidations.AddListValidation("$H$" + recordIndex + ":$H$10000");
        //                dropdownlist.Formula.Values.Add("Da");
        //                dropdownlist.Formula.Values.Add("Nu");

        //                var dropdownlistObs = worksheet.DataValidations.AddListValidation("$J$" + recordIndex + ":$J$10000");
        //                dropdownlistObs.Formula.Values.Add("DEFECT");
        //                dropdownlistObs.Formula.Values.Add("DETERIORAT");
        //            }




        //            worksheet.Row(1).Height = 45.75;
        //            worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //            worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            worksheet.View.FreezePanes(14, 19);
        //            worksheet.Cells["K11"].Value = "";
        //            worksheet.View.ZoomScale = 65;


        //            for (int i = 1; i < 11; i++)
        //            {
        //                worksheet.Cells[1, i].Style.WrapText = true;
        //            }

        //            //dropdownlist.Formula.ExcelFormula = "=$H$2:$H$1000000!Da";
        //            //dropdownlist.ShowErrorMessage = true;
        //            ////dropdownlist.ErrorStyle = ExcelDataValidationWarningStyle.stop;
        //            ////dropdownlist.ErrorTitle = "Error";
        //            //dropdownlist.Error = "Error Text";


        //            //var validation = worksheet.DataValidations.AddListValidation("H2");
        //            //validation.Formula.Values.Add("Da");
        //            //validation.Formula.Values.Add("Nu");
        //            //validation.ShowErrorMessage = true;
        //            //validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
        //            //validation.ErrorTitle = "Error";
        //            //validation.Error = "Error Text";
        //            //// sheet with a name : DropDownLists 
        //            //// from DropDownLists sheet, get values from cells: !$A$1:$A$10
        //            //var formula = "=Nota_transfer!$H$2:$H$2";
        //            ////Applying Formula to the range
        //            //validation.Formula.ExcelFormula = formula;
        //            //Process.Start("mailto:hello@test.com&subject=This is the subject&body=This is the body");




        //            using (var cells = worksheet.Cells[1, 1, 1, 10])
        //            {
        //                cells.Style.Font.Bold = true;
        //                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                cells.Style.Fill.BackgroundColor.SetColor(Color.Silver);

        //            }

        //            using (var cells = worksheet.Cells[2, 11, 10, 18])
        //            {
        //                cells.Style.Font.Bold = true;
        //                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                cells.Style.Fill.BackgroundColor.SetColor(Color.Silver);

        //            }

        //            using (var cells = worksheet.Cells[1, 11, 1, 18])
        //            {
        //                cells.Style.Font.Bold = true;
        //                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                cells.Style.Fill.BackgroundColor.SetColor(Color.Red);

        //            }

        //            using (var cells = worksheet.Cells[12, 11, 12, 18])
        //            {
        //                cells.Style.Font.Bold = true;
        //                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                cells.Style.Fill.BackgroundColor.SetColor(Color.Red);

        //            }

        //            using (var cells = worksheet.Cells[13, 11, 13, 18])
        //            {
        //                cells.Style.Font.Bold = true;
        //                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                cells.Style.Fill.BackgroundColor.SetColor(Color.Silver);

        //            }

        //            recordIndex++;
        //            locationIndexes[sheetName] = recordIndex;
        //        }

        //        //using (FileStream aFile = new FileStream(@"D:\asdf.xlsx", FileMode.Create))
        //        //{
        //        //    aFile.Seek(0, SeekOrigin.Begin);
        //        //    package.SaveAs(aFile);

        //        //}
        //        package.Save();
        //        //package.SaveAs(new FileInfo(sFileName));


        //        //package.SaveAs(new FileInfo(sFileName));

        //        Response.Clear();
        //        //Response.Headers.Add("content-disposition", "attachment;  filename=transferuri.xlsx");
        //        // Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        //var bytes = package.GetAsByteArray();
        //        // Response.Body.WriteAsync(bytes, 0, bytes.Length);

        //        //Process.Start("cmd", "/C start " + file.FullName.ToString());

        //        //var filePath = @"D:\Adrian\Fais\trunk\fais-api\Optima.Fais.Api\wwwroot\transferuri.xlsx";
        //        //var fileName = "transferuri.xlsx";
        //        //var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        //        //FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(filePath), mimeType)
        //        //{
        //        //    FileDownloadName = fileName
        //        //};

        //        // Bnr 

        //        string sWebRootFolderBnr = hostingEnvironment.WebRootPath;
        //        //string sWebRootFolder = "D:\\";

        //        string sFileNameBnr = @"Nota_transfer.xlsx";
        //        string URLBnr = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileNameBnr);
        //        FileInfo fileBnr = new FileInfo(Path.Combine(sWebRootFolderBnr, sFileNameBnr));

        //        using (ExcelPackage excelPackageBnr = new ExcelPackage(fileBnr))
        //        {
        //            ExcelWorkbook excelWorkBookBnr = excelPackageBnr.Workbook;
        //            ExcelWorksheet excelWorksheetBnr = excelWorkBookBnr.Worksheets.First();


        //            excelPackageBnr.Save();
        //        }



        //        return Ok();


        //    }

        //    // ----original-----//

        //    //using (ExcelPackage package = new ExcelPackage(file))
        //    //{
        //    //    //var package = new ExcelPackage();




        //    //    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Nota_transfer");
        //    //    //First add the headers
        //    //    worksheet.Cells[1, 1].Value = "OptimaId";
        //    //    worksheet.Cells[1, 2].Value = "Numar inventar plecare";
        //    //    worksheet.Cells[1, 3].Value = "Denumire";
        //    //    worksheet.Cells[1, 4].Value = "Centru de cost plecare";
        //    //    worksheet.Cells[1, 5].Value = "Cladire plecare";
        //    //    worksheet.Cells[1, 6].Value = "Centru de cost destinatie";
        //    //    worksheet.Cells[1, 7].Value = "Cladire destinatie";
        //    //    worksheet.Cells[1, 8].Value = "Confirmat";
        //    //    worksheet.Cells[1, 9].Value = "Numar inventar primit";
        //    //    worksheet.Cells[1, 10].Value = "Observatii";
        //    //    worksheet.Cells[1, 11].Value = "Instructiuni";
        //    //    worksheet.Cells[12, 11].Value = "Primite in plus";
        //    //    worksheet.Cells[13, 11].Value = "Denumire produs";
        //    //    worksheet.Cells[13, 14].Value = "Descriere obiect";
        //    //    worksheet.Cells[13, 17].Value = "Numar inventar";


        //    //    int recordIndex = 2;
        //    //    foreach (var item in operations)
        //    //    {
        //    //        worksheet.Cells[recordIndex, 1].Value = item.AssetOpId;
        //    //        worksheet.Cells[recordIndex, 2].Value = item.InvNo;
        //    //        worksheet.Cells[recordIndex, 3].Value = item.AssetName;
        //    //        worksheet.Cells[recordIndex, 4].Value = item.RoomCodeIni;
        //    //        worksheet.Cells[recordIndex, 5].Value = item.LocationCodeIni;
        //    //        worksheet.Cells[recordIndex, 6].Value = item.RoomCodeFin;
        //    //        worksheet.Cells[recordIndex, 7].Value = item.LocationCodeFin;
        //    //        worksheet.Cells[recordIndex, 8].Value = "Da";
        //    //        worksheet.Cells[recordIndex, 9].Value = item.InvNo;
        //    //        worksheet.Cells[recordIndex, 11].Value = @"
        //    //      Pe coloana Confirmat se va completa cu :
        //    //                             'NU' daca numarul de inventar nu a ajuns la destinatie si cu 
        //    //                             'Da' daca a ajuns.
        //    //                              Implicit este completat cu 'Da'. 
        //    //      In situatia in care numarul de inventar nu corespunde cu cel initial se va completa 
        //    //      pe coloana 'Numar de inventar primit' numarul existent.
        //    //      Pe coloana Observatii se va completa daca obiectul este functional, defect, etc.
        //    //      Daca exista obiecte care nu se regasesc in lista acestea se vor completa in 
        //    //      zona 'Primite in plus.'";
        //    //        recordIndex++;
        //    //    }

        //    //    worksheet.Column(1).AutoFit();
        //    //    worksheet.Column(2).AutoFit();
        //    //    worksheet.Column(3).AutoFit();
        //    //    worksheet.Column(4).AutoFit();
        //    //    worksheet.Column(5).AutoFit();
        //    //    worksheet.Column(6).AutoFit();
        //    //    worksheet.Column(7).AutoFit();
        //    //    worksheet.Column(8).AutoFit();
        //    //    worksheet.Column(9).AutoFit();
        //    //    worksheet.Column(10).AutoFit();
        //    //    worksheet.Cells["K1:R1"].Merge = true;
        //    //    worksheet.Cells["K1:R1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
        //    //    worksheet.Cells["K2:R10"].Merge = true;
        //    //    worksheet.Cells["K2:R10"].Style.WrapText = true;
        //    //    worksheet.Cells["K2:R10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
        //    //    worksheet.Cells["K2:R10"].Style.Font.Size = 10;




        //    //    worksheet.Cells["K12:R12"].Merge = true;
        //    //    worksheet.Cells["K12:R12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


        //    //    worksheet.Cells["K13:M13"].Merge = true;
        //    //    worksheet.Cells["K13:R13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

        //    //    worksheet.Cells["N13:P13"].Merge = true;
        //    //    worksheet.Cells["N13:P13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

        //    //    worksheet.Cells["Q13:R13"].Merge = true;
        //    //    worksheet.Cells["Q13:R13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


        //    //    for (int i = 14; i < 1000; i++)
        //    //    {
        //    //        worksheet.Cells[i, 11, i, 13].Merge = true;
        //    //        worksheet.Cells[i, 14, i, 16].Merge = true;
        //    //        worksheet.Cells[i, 17, i, 18].Merge = true;
        //    //    }

        //    //    worksheet.Protection.IsProtected = true;
        //    //    worksheet.Column(8).Style.Locked = false;
        //    //    worksheet.Column(9).Style.Locked = false;
        //    //    worksheet.Column(10).Style.Locked = false;
        //    //    worksheet.Cells["$K$14:$K$1000"].Style.Locked = false;
        //    //    worksheet.Cells["$L$14:$L$1000"].Style.Locked = false;
        //    //    worksheet.Cells["$M$14:$M$1000"].Style.Locked = false;
        //    //    worksheet.Cells["$N$14:$N$1000"].Style.Locked = false;
        //    //    worksheet.Cells["$O$14:$O$1000"].Style.Locked = false;
        //    //    worksheet.Cells["$P$14:$P$1000"].Style.Locked = false;
        //    //    worksheet.Cells["$Q$14:$Q$1000"].Style.Locked = false;
        //    //    worksheet.Cells["$R$14:$R$1000"].Style.Locked = false;


        //    //    //worksheet.Cells["13", "K", "1000", "K"].Merge = true;

        //    //    worksheet.Protection.SetPassword("piraeus");


        //    //    var dropdownlist = worksheet.DataValidations.AddListValidation("$H$2:$H$10000");
        //    //    dropdownlist.Formula.Values.Add("Da");
        //    //    dropdownlist.Formula.Values.Add("Nu");

        //    //    var dropdownlistObs = worksheet.DataValidations.AddListValidation("$J$2:$J$10000");
        //    //    dropdownlistObs.Formula.Values.Add("DEFECT");
        //    //    dropdownlistObs.Formula.Values.Add("DETERIORAT");

        //    //    worksheet.Row(1).Height = 45.75;
        //    //    worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //    //    worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    //    worksheet.View.FreezePanes(14, 19);
        //    //    worksheet.Cells["K11"].Value = "";
        //    //    worksheet.View.ZoomScale = 70;

        //    //  //dropdownlist.Formula.ExcelFormula = "=$H$2:$H$1000000!Da";
        //    //  //dropdownlist.ShowErrorMessage = true;
        //    //  ////dropdownlist.ErrorStyle = ExcelDataValidationWarningStyle.stop;
        //    //  ////dropdownlist.ErrorTitle = "Error";
        //    //  //dropdownlist.Error = "Error Text";


        //    //  //var validation = worksheet.DataValidations.AddListValidation("H2");
        //    //  //validation.Formula.Values.Add("Da");
        //    //  //validation.Formula.Values.Add("Nu");
        //    //  //validation.ShowErrorMessage = true;
        //    //  //validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
        //    //  //validation.ErrorTitle = "Error";
        //    //  //validation.Error = "Error Text";
        //    //  //// sheet with a name : DropDownLists 
        //    //  //// from DropDownLists sheet, get values from cells: !$A$1:$A$10
        //    //  //var formula = "=Nota_transfer!$H$2:$H$2";
        //    //  ////Applying Formula to the range
        //    //  //validation.Formula.ExcelFormula = formula;
        //    //  //Process.Start("mailto:hello@test.com&subject=This is the subject&body=This is the body");




        //    //  using (var cells = worksheet.Cells[1, 1, 1, 10])
        //    //    {
        //    //        cells.Style.Font.Bold = true;
        //    //        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //    //        cells.Style.Fill.BackgroundColor.SetColor(Color.Silver);

        //    //    }

        //    //    using (var cells = worksheet.Cells[2, 11, 10, 18])
        //    //    {
        //    //        cells.Style.Font.Bold = true;
        //    //        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //    //        cells.Style.Fill.BackgroundColor.SetColor(Color.Silver);

        //    //    }

        //    //    using (var cells = worksheet.Cells[1, 11, 1, 18])
        //    //    {
        //    //        cells.Style.Font.Bold = true;
        //    //        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //    //        cells.Style.Fill.BackgroundColor.SetColor(Color.Red);

        //    //    }

        //    //    using (var cells = worksheet.Cells[12, 11, 12, 18])
        //    //    {
        //    //        cells.Style.Font.Bold = true;
        //    //        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //    //        cells.Style.Fill.BackgroundColor.SetColor(Color.Red);

        //    //    }

        //    //    using (var cells = worksheet.Cells[13, 11, 13, 18])
        //    //    {
        //    //        cells.Style.Font.Bold = true;
        //    //        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //    //        cells.Style.Fill.BackgroundColor.SetColor(Color.Silver);

        //    //    }

        //    //    //using (FileStream aFile = new FileStream(@"D:\asdf.xlsx", FileMode.Create))
        //    //    //{
        //    //    //    aFile.Seek(0, SeekOrigin.Begin);
        //    //    //    package.SaveAs(aFile);

        //    //    //}
        //    //    package.Save();
        //    //    //package.SaveAs(new FileInfo(sFileName));


        //    //    //package.SaveAs(new FileInfo(sFileName));

        //    //    Response.Clear();
        //    //    //Response.Headers.Add("content-disposition", "attachment;  filename=transferuri.xlsx");
        //    //    // Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    //    //var bytes = package.GetAsByteArray();
        //    //    // Response.Body.WriteAsync(bytes, 0, bytes.Length);

        //    //    //Process.Start("cmd", "/C start " + file.FullName.ToString());

        //    //    //var filePath = @"D:\Adrian\Fais\trunk\fais-api\Optima.Fais.Api\wwwroot\transferuri.xlsx";
        //    //    //var fileName = "transferuri.xlsx";
        //    //    //var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        //    //    //FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(filePath), mimeType)
        //    //    //{
        //    //    //    FileDownloadName = fileName
        //    //    //};

        //    //    // Bnr 

        //    //    string sWebRootFolderBnr = hostingEnvironment.WebRootPath;
        //    //    //string sWebRootFolder = "D:\\";

        //    //    string sFileNameBnr = @"RBM.xlsx";
        //    //    string URLBnr = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileNameBnr);
        //    //    FileInfo fileBnr = new FileInfo(Path.Combine(sWebRootFolderBnr, sFileNameBnr));

        //    //    using (ExcelPackage excelPackageBnr = new ExcelPackage(fileBnr))
        //    //    {
        //    //        ExcelWorkbook excelWorkBookBnr = excelPackageBnr.Workbook;
        //    //        ExcelWorksheet excelWorksheetBnr = excelWorkBookBnr.Worksheets.First();
        //    //        excelWorksheetBnr.Cells[1, 1].Value = "Test";
        //    //        excelWorksheetBnr.Cells[3, 2].Value = "Test2";
        //    //        excelWorksheetBnr.Cells[3, 3].Value = "Test3";

        //    //        excelPackageBnr.Save();
        //    //    }



        //    //    return Ok();


        //    //} 

        //    // ----original-----//

        //}


        [HttpPost]
        [Route("downloadMailOps")]
        [EnableCors("MyPolicy")]
        public IActionResult Export1([FromBody] AssetOpConf[] operations)
        {


            string sWebRootFolder = hostingEnvironment.WebRootPath;
            //string sWebRootFolder = "D:\\";

            string sFileName = @"Nota_transfer.xlsx";
            string sFileNameTemplate = @"TransferTemplate.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

         

                if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }

            using (ExcelPackage package = new ExcelPackage(file))
            {
                //var package = new ExcelPackage();

              //  var path = sWebRootFolder + "\\logo.png";


                Dictionary<string, int> locationIndexes = new Dictionary<string, int>();
                Dictionary<string, int> locationNames = new Dictionary<string, int>();
                int recordIndex = 0;

                foreach (var item in operations)
                {

                   // string sheetName = item.LocationCodeFin + "_Nota_transfer";  // PIRAEUS
                    string sheetName = item.LocationCodeFin;  // BNR



                    ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];

                    //var checkLocationExist = locationNames.TryGetValue(item.LocationCodeIni, out int value);

                    //if (checkLocationExist)
                    //{
                    //    worksheet = package.Workbook.Worksheets[sheetName];
                    //}
                    //else
                    //{
                    //    sheetName = item.LocationCodeFin + "_Nota_transfer" + "<--" + item.LocationCodeIni;
                    //    worksheet = package.Workbook.Worksheets[sheetName + "<--" + item.LocationCodeIni];
                    //}

                    worksheet = package.Workbook.Worksheets[sheetName];


                    if (worksheet == null)
                    {
                        worksheet = package.Workbook.Worksheets.Add(sheetName);
                        recordIndex = 2;
                        locationIndexes.Add(sheetName, recordIndex);


                        //  if (!checkLocationExist) locationNames.Add(item.LocationCodeIni, recordIndex);

                        locationNames.Add(item.LocationCodeFin, recordIndex);

                        //First add the headers
                        worksheet.Cells[1, 1].Value = "Fixed Assets Class";
                        worksheet.Cells[1, 2].Value = "Inventory Item ";
                        worksheet.Cells[1, 3].Value = "Inventory Number (Barcode if implemented)";
                        worksheet.Cells[1, 4].Value = "Date of Entrance ";
                        worksheet.Cells[1, 5].Value = "Type / Mark";
                        worksheet.Cells[1, 6].Value = "Serial Number   (if applicable)";
                        worksheet.Cells[1, 7].Value = "Old cost center";
                        worksheet.Cells[1, 8].Value = "Old Location";
                        worksheet.Cells[1, 9].Value = "New cost center";
                        worksheet.Cells[1, 10].Value = "New Location";
                        worksheet.Cells[1, 11].Value = "Confirm";
                        worksheet.Cells[1, 12].Value = "Obs.";
                        worksheet.Cells[1, 13].Value = "OptimaId";
                        //worksheet.Cells[1, 11].Value = "Instructiuni";
                        //worksheet.Cells[12, 11].Value = "Primite in plus";
                        //worksheet.Cells[13, 11].Value = "Denumire produs";
                        //worksheet.Cells[13, 14].Value = "Descriere obiect";
                        //worksheet.Cells[13, 17].Value = "Numar inventar";

                        // NEW REPORT //

                        //worksheet.Cells[1, 1].Value = "Fixed Assets & Other Inventories Transfer Form";
                        //worksheet.Cells[2, 1].Value = "Transferred :";
                        //worksheet.Cells[3, 2].Value = "FROM :";
                        //worksheet.Cells[3, 3].Value = "";
                        //worksheet.Cells[4, 2].Value = "TO :";
                        //worksheet.Cells[4, 3].Value = "";


                        worksheet.Cells[operations.Length + 5, 1].Value = "Fixed Asset Area Administrator's Signature";
                        worksheet.Cells[operations.Length + 6, 1].Value = "Old  User / Responsible of Old Location";
                        worksheet.Cells[operations.Length + 7, 1].Value = "New  User / Responsible of New Location";

                        worksheet.Cells[operations.Length + 5, 6].Value = "Date: ";
                        worksheet.Cells[operations.Length + 6, 6].Value = "Date: ";
                        worksheet.Cells[operations.Length + 7, 6].Value = "Date: ";

                        //int operationLenghtSecondSheets = operations.Length + 12;

                        //worksheet.Cells["D" + operationLenghtSecondSheets + ":F" + operationLenghtSecondSheets].Merge = true;
                        //worksheet.Cells["D" + operationLenghtSecondSheets + ":F" + operationLenghtSecondSheets].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        //worksheet.Cells["D" + operationLenghtSecondSheets + ":F" + operationLenghtSecondSheets].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        //worksheet.Cells["D" + operations.Length + 13 + ":F" + operations.Length + 13].Merge = true;
                        //worksheet.Cells["D" + operations.Length + 13 + ":F" + operations.Length + 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        //worksheet.Cells["D" + operations.Length + 13 + ":F" + operations.Length + 13].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        //worksheet.Cells["D" + operations.Length + 14 + ":F" + operations.Length + 14].Merge = true;
                        //worksheet.Cells["D" + operations.Length + 14 + ":F" + operations.Length + 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        //worksheet.Cells["D" + operations.Length + 14 + ":F" + operations.Length + 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        //Image logo = Image.FromFile(path);



                        //var picture = worksheet.Drawings.AddPicture("logo.png", logo);
                        //picture.SetPosition(2, 2, 0, 2);


                        //using (ExcelRange Rng = worksheet.Cells["B20:I20"])
                        //{
                        //    Rng.Value = "Everyday Be Coding - Format Table using EPPlus .Net Library - Part 15(B)";
                        //    Rng.Merge = true;
                        //    Rng.Style.Font.Size = 16;
                        //    Rng.Style.Font.Bold = true;
                        //    Rng.Style.Font.Italic = true;
                        //}

                        //using (ExcelRange Rng1 = worksheet.Cells["B30:G40"])
                        //{
                        //    ExcelTableCollection tblcollection = worksheet.Tables;



                        //    ExcelTable table = tblcollection.Add(Rng1, "tblSalesman" + item.LocationCodeIni);
                        //    //Set Columns position & name  
                        //    table.Columns[0].Name = "Id";
                        //    table.Columns[1].Name = "Salesman Name";
                        //    table.Columns[2].Name = "Sales Amount";
                        //    table.Columns[3].Name = "Profits";
                        //    table.Columns[4].Name = "Country";
                        //    table.Columns[5].Name = "Date";
                        //    //table.ShowHeader = false;  
                        //    table.ShowFilter = true;
                        //    table.ShowTotal = true;
                        //    //Add TotalsRowFormula into Excel table Columns  
                        //    table.Columns[0].TotalsRowLabel = "Total Rows";
                        //    table.Columns[1].TotalsRowFormula = "SUBTOTAL(102,[Id])"; //102 = Count  
                        //    table.Columns[2].TotalsRowFormula = "SUBTOTAL(109,[Sales Amount])"; //109 = Sum  
                        //    table.Columns[3].TotalsRowFormula = "SUBTOTAL(101,[Profits])"; //101 = Average  
                        //                                                                   //Add TotalsRowFunction into Excel table Columns  
                        //                                                                   //table.Columns[0].TotalsRowLabel = "Total Rows";  
                        //                                                                   //able.Columns[1].TotalsRowFunction = RowFunctions.Count;  
                        //                                                                   //table.Columns[2].TotalsRowFunction = RowFunctions.Sum;  
                        //                                                                   //table.Columns[3].TotalsRowFunction = RowFunctions.Average;  
                        //    table.TableStyle = TableStyles.Dark9;
                        //}


                    }
                    else
                    {
                        recordIndex = locationIndexes[sheetName];
                        recordIndex--;

                        for (int i = 0; i < operations.Length; i++)
                        {
                            if (i > 2) break;
                            int operationLenghtSecondSheets = operations.Length + 5 + i;

                            worksheet.Cells["C" + operationLenghtSecondSheets + ":D" + operationLenghtSecondSheets].Merge = true;
                            worksheet.Cells["C" + operationLenghtSecondSheets + ":D" + operationLenghtSecondSheets].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            worksheet.Cells["C" + operationLenghtSecondSheets + ":D" + operationLenghtSecondSheets].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells["G" + operationLenghtSecondSheets + ":J" + operationLenghtSecondSheets].Merge = true;
                            worksheet.Cells["G" + operationLenghtSecondSheets + ":J" + operationLenghtSecondSheets].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            worksheet.Cells["G" + operationLenghtSecondSheets + ":J" + operationLenghtSecondSheets].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            
                        }

                        
                       
                        
     
                      

                        //worksheet.Cells["C" + operationLenghtSecondSheets + ":D" + operationLenghtSecondSheets].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //worksheet.Cells["C" + operationLenghtSecondSheets + ":D" + operationLenghtSecondSheets].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(204, 255, 204));



                        //  worksheet.Column(1).Width = 45.75;
                        //  worksheet.View.ZoomScale = 65;
                    }

                    worksheet.Cells[recordIndex, 1].Value = item.AssetTypeName;
                    worksheet.Cells[recordIndex, 2].Value = item.AssetName;
                    worksheet.Cells[recordIndex, 3].Value = item.InvNo;
                    worksheet.Cells[recordIndex, 4].Value = item.PurchaseDate;
                  //  worksheet.Cells[recordIndex, 5].Value = item.LocationCodeIni;
                    worksheet.Cells[recordIndex, 6].Value = item.SerialNumber;
                    worksheet.Cells[recordIndex, 7].Value = item.RoomCodeIni;
                    worksheet.Cells[recordIndex, 8].Value = item.LocationCodeIni;
                    worksheet.Cells[recordIndex, 9].Value = item.RoomCodeFin;
                    worksheet.Cells[recordIndex, 10].Value = item.LocationCodeFin;
                    worksheet.Cells[recordIndex, 11].Value = "Nu";
                    worksheet.Cells[recordIndex, 13].Value = item.AssetOpId;
                    //worksheet.Cells["C3:F3"].Value = item.LocationCodeIni;
                    //worksheet.Cells["C4:F4"].Value = item.LocationCodeFin;
                    worksheet.View.ShowGridLines = false;
                    worksheet.Cells[recordIndex, 4].Style.Numberformat.Format = "mm-dd-yy";

                    for (int i = 1; i < 13; i++)
                    {
                        worksheet.Cells[recordIndex, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[recordIndex, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        worksheet.Cells[recordIndex, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[recordIndex, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(204, 255, 204));
                      //  worksheet.Cells[recordIndex, i].Style.WrapText = true;

                        worksheet.Cells[recordIndex, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));


                    }

                    for (int i = 2; i < recordIndex; i++)
                    {
                        //worksheet.Cells[2, i].Style.WrapText = true;
                    }

                    
                    recordIndex++;
                  //  recordIndex = 2;
                  //  worksheet.Cells[2, 11].Value = @"
                  //Pe coloana Confirmat se va completa cu :
                  //                       'NU' daca numarul de inventar nu a ajuns la destinatie si cu 
                  //                       'Da' daca a ajuns.
                  //                        Implicit este completat cu 'Da'. 
                  //In situatia in care numarul de inventar nu corespunde cu cel initial se va completa 
                  //pe coloana 'Numar de inventar primit' numarul existent.
                  //Pe coloana Observatii se va completa daca obiectul este functional, defect, etc.
                  //Daca exista obiecte care nu se regasesc in lista acestea se vor completa in 
                  //zona 'Primite in plus.'";

                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();
                    worksheet.Column(5).AutoFit();
                    worksheet.Column(6).AutoFit();
                    worksheet.Column(7).AutoFit();
                    worksheet.Column(8).AutoFit();
                    worksheet.Column(9).AutoFit();
                    worksheet.Column(10).AutoFit();
                    worksheet.Column(11).AutoFit();
                    worksheet.Column(12).AutoFit();

                  //  worksheet.Cells["A1:J1"].Merge = true;
                   // worksheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    //worksheet.Cells["K1:R1"].Merge = true;
                    //worksheet.Cells["K1:R1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    //worksheet.Cells["D15:G15"].Merge = true;
                    ////worksheet.Cells["D15:G15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    //worksheet.Cells["C3:F3"].Merge = true;
                    //worksheet.Cells["C3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    //worksheet.Cells["C3:F3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    //worksheet.Cells["C4:F4"].Merge = true;
                    //worksheet.Cells["C4:F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    //worksheet.Cells["C4:F4"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    for (int i = 0; i < operations.Length; i++)
                    {
                        if (i > 2) break;
                        int operationLenght = operations.Length + 5 + i;

                        worksheet.Cells["C" + operationLenght + ":D" + operationLenght].Merge = true;
                        worksheet.Cells["C" + operationLenght + ":D" + operationLenght].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        worksheet.Cells["C" + operationLenght + ":D" + operationLenght].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells["G" + operationLenght + ":J" + operationLenght].Merge = true;
                        worksheet.Cells["G" + operationLenght + ":J" + operationLenght].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        worksheet.Cells["G" + operationLenght + ":J" + operationLenght].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    


                   

                    //worksheet.Cells["K2:R10"].Merge = true;
                    //worksheet.Cells["K2:R10"].Style.WrapText = true;
                    //worksheet.Cells["K2:R10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                    //worksheet.Cells["K2:R10"].Style.Font.Size = 10;




                    //worksheet.Cells["K12:R12"].Merge = true;
                    //worksheet.Cells["K12:R12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


                    //worksheet.Cells["K13:M13"].Merge = true;
                    //worksheet.Cells["K13:R13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    //worksheet.Cells["N13:P13"].Merge = true;
                    //worksheet.Cells["N13:P13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    //worksheet.Cells["Q13:R13"].Merge = true;
                    //worksheet.Cells["Q13:R13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


                    //for (int i = 14; i < 1000; i++)
                    //{
                    //    worksheet.Cells[i, 11, i, 13].Merge = true;
                    //    worksheet.Cells[i, 14, i, 16].Merge = true;
                    //    worksheet.Cells[i, 17, i, 18].Merge = true;
                    //}

                    worksheet.Protection.IsProtected = true;
                    worksheet.Column(11).Style.Locked = false;
                    worksheet.Column(12).Style.Locked = false;
                    worksheet.Column(13).Hidden = true;

                    //worksheet.Cells["$K$14:$K$1000"].Style.Locked = false;
                    //worksheet.Cells["$L$14:$L$1000"].Style.Locked = false;
                    //worksheet.Cells["$M$14:$M$1000"].Style.Locked = false;
                    //worksheet.Cells["$N$14:$N$1000"].Style.Locked = false;
                    //worksheet.Cells["$O$14:$O$1000"].Style.Locked = false;
                    //worksheet.Cells["$P$14:$P$1000"].Style.Locked = false;
                    //worksheet.Cells["$Q$14:$Q$1000"].Style.Locked = false;
                    //worksheet.Cells["$R$14:$R$1000"].Style.Locked = false;


                    //worksheet.Cells["13", "K", "1000", "K"].Merge = true;

                    worksheet.Protection.SetPassword("piraeus");

                    var validations = worksheet.DataValidations;
                    if (validations.Count > 0)
                    {

                    }
                    else
                    {
                        int rowNumber = recordIndex - 1;
                        var dropdownlist = worksheet.DataValidations.AddListValidation("$K$" + rowNumber + ":$K$10000");
                        dropdownlist.Formula.Values.Add("Da");
                        dropdownlist.Formula.Values.Add("Nu");

                        var dropdownlistObs = worksheet.DataValidations.AddListValidation("$L$" + rowNumber + ":$L$10000");
                        dropdownlistObs.Formula.Values.Add("DEFECT");
                        dropdownlistObs.Formula.Values.Add("DETERIORAT");
                    }




                    //worksheet.Row(1).Height = 45.75;
                    //worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                 //   worksheet.View.FreezePanes(14, 19);
                  //  worksheet.Cells["K11"].Value = "";
                  
                    worksheet.View.ZoomScale = 73;


                    for (int i = 1; i < 13; i++)
                    {
                     //   worksheet.Cells[1, i].Style.WrapText = true;
                     //   worksheet.Cells[7, i].Style.WrapText = true;
                        worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(204, 255, 204));
                        worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));
                        worksheet.Cells[1, i].Style.Font.Bold = true;

                        worksheet.Row(i).Height = 30;
                       


                        //worksheet.Cells[recordIndex, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //worksheet.Cells[recordIndex, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(204, 255, 204));
                      //  worksheet.Cells[recordIndex, 1].Style.WrapText = true;


                    }

                    


                    //dropdownlist.Formula.ExcelFormula = "=$H$2:$H$1000000!Da";
                    //dropdownlist.ShowErrorMessage = true;
                    ////dropdownlist.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                    ////dropdownlist.ErrorTitle = "Error";
                    //dropdownlist.Error = "Error Text";


                    //var validation = worksheet.DataValidations.AddListValidation("H2");
                    //validation.Formula.Values.Add("Da");
                    //validation.Formula.Values.Add("Nu");
                    //validation.ShowErrorMessage = true;
                    //validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                    //validation.ErrorTitle = "Error";
                    //validation.Error = "Error Text";
                    //// sheet with a name : DropDownLists 
                    //// from DropDownLists sheet, get values from cells: !$A$1:$A$10
                    //var formula = "=Nota_transfer!$H$2:$H$2";
                    ////Applying Formula to the range
                    //validation.Formula.ExcelFormula = formula;
                    //Process.Start("mailto:hello@test.com&subject=This is the subject&body=This is the body");




                    using (var cells = worksheet.Cells[1, 1, 1, 10])
                    {
                        cells.Style.Font.Bold = true;
                        //cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //cells.Style.Fill.BackgroundColor.SetColor(Color.White);

                    }

                    using (var cells = worksheet.Cells[2, 1, recordIndex, 2])
                    {
                        cells.Style.Font.Bold = true;
                        cells.Style.WrapText = true;
                       



                    }

                    //using (var cells = worksheet.Cells[2, 11, 10, 18])
                    //{
                    //    cells.Style.Font.Bold = true;
                    //    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //    cells.Style.Fill.BackgroundColor.SetColor(Color.Silver);

                    //}

                    //using (var cells = worksheet.Cells[1, 11, 1, 18])
                    //{
                    //    cells.Style.Font.Bold = true;
                    //    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //    cells.Style.Fill.BackgroundColor.SetColor(Color.Red);

                    //}

                    //using (var cells = worksheet.Cells[12, 11, 12, 18])
                    //{
                    //    cells.Style.Font.Bold = true;
                    //    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //    cells.Style.Fill.BackgroundColor.SetColor(Color.Red);

                    //}

                    //using (var cells = worksheet.Cells[13, 11, 13, 18])
                    //{
                    //    cells.Style.Font.Bold = true;
                    //    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //    cells.Style.Fill.BackgroundColor.SetColor(Color.Silver);

                    //}

                    recordIndex++;
                    locationIndexes[sheetName] = recordIndex;

                   
                }

                //using (FileStream aFile = new FileStream(@"D:\asdf.xlsx", FileMode.Create))
                //{
                //    aFile.Seek(0, SeekOrigin.Begin);
                //    package.SaveAs(aFile);

                //}

              

                package.Save();
                //package.SaveAs(new FileInfo(sFileName));


                //package.SaveAs(new FileInfo(sFileName));

                Response.Clear();
                //Response.Headers.Add("content-disposition", "attachment;  filename=transferuri.xlsx");
                // Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //var bytes = package.GetAsByteArray();
                // Response.Body.WriteAsync(bytes, 0, bytes.Length);

                //Process.Start("cmd", "/C start " + file.FullName.ToString());

                //var filePath = @"D:\Adrian\Fais\trunk\fais-api\Optima.Fais.Api\wwwroot\transferuri.xlsx";
                //var fileName = "transferuri.xlsx";
                //var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                //FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(filePath), mimeType)
                //{
                //    FileDownloadName = fileName
                //};

                // Bnr 

                string sWebRootFolderBnr = hostingEnvironment.WebRootPath;
                //string sWebRootFolder = "D:\\";

                string sFileNameBnr = @"Nota_transfer.xlsx";
                string URLBnr = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileNameBnr);
                FileInfo fileBnr = new FileInfo(Path.Combine(sWebRootFolderBnr, sFileNameBnr));

               
                using (ExcelPackage excelPackageBnr = new ExcelPackage(fileBnr))
                {
                    ExcelWorkbook excelWorkBookBnr = excelPackageBnr.Workbook;
                    ExcelWorksheet excelWorksheetBnr = excelWorkBookBnr.Worksheets.First();


                    excelPackageBnr.Save();
                }



                   
           



                return Ok();
            }

            // ----original-----//

            //using (ExcelPackage package = new ExcelPackage(file))
            //{
            //    //var package = new ExcelPackage();




            //    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Nota_transfer");
            //    //First add the headers
            //    worksheet.Cells[1, 1].Value = "OptimaId";
            //    worksheet.Cells[1, 2].Value = "Numar inventar plecare";
            //    worksheet.Cells[1, 3].Value = "Denumire";
            //    worksheet.Cells[1, 4].Value = "Centru de cost plecare";
            //    worksheet.Cells[1, 5].Value = "Cladire plecare";
            //    worksheet.Cells[1, 6].Value = "Centru de cost destinatie";
            //    worksheet.Cells[1, 7].Value = "Cladire destinatie";
            //    worksheet.Cells[1, 8].Value = "Confirmat";
            //    worksheet.Cells[1, 9].Value = "Numar inventar primit";
            //    worksheet.Cells[1, 10].Value = "Observatii";
            //    worksheet.Cells[1, 11].Value = "Instructiuni";
            //    worksheet.Cells[12, 11].Value = "Primite in plus";
            //    worksheet.Cells[13, 11].Value = "Denumire produs";
            //    worksheet.Cells[13, 14].Value = "Descriere obiect";
            //    worksheet.Cells[13, 17].Value = "Numar inventar";


            //    int recordIndex = 2;
            //    foreach (var item in operations)
            //    {
            //        worksheet.Cells[recordIndex, 1].Value = item.AssetOpId;
            //        worksheet.Cells[recordIndex, 2].Value = item.InvNo;
            //        worksheet.Cells[recordIndex, 3].Value = item.AssetName;
            //        worksheet.Cells[recordIndex, 4].Value = item.RoomCodeIni;
            //        worksheet.Cells[recordIndex, 5].Value = item.LocationCodeIni;
            //        worksheet.Cells[recordIndex, 6].Value = item.RoomCodeFin;
            //        worksheet.Cells[recordIndex, 7].Value = item.LocationCodeFin;
            //        worksheet.Cells[recordIndex, 8].Value = "Da";
            //        worksheet.Cells[recordIndex, 9].Value = item.InvNo;
            //        worksheet.Cells[recordIndex, 11].Value = @"
            //      Pe coloana Confirmat se va completa cu :
            //                             'NU' daca numarul de inventar nu a ajuns la destinatie si cu 
            //                             'Da' daca a ajuns.
            //                              Implicit este completat cu 'Da'. 
            //      In situatia in care numarul de inventar nu corespunde cu cel initial se va completa 
            //      pe coloana 'Numar de inventar primit' numarul existent.
            //      Pe coloana Observatii se va completa daca obiectul este functional, defect, etc.
            //      Daca exista obiecte care nu se regasesc in lista acestea se vor completa in 
            //      zona 'Primite in plus.'";
            //        recordIndex++;
            //    }

            //    worksheet.Column(1).AutoFit();
            //    worksheet.Column(2).AutoFit();
            //    worksheet.Column(3).AutoFit();
            //    worksheet.Column(4).AutoFit();
            //    worksheet.Column(5).AutoFit();
            //    worksheet.Column(6).AutoFit();
            //    worksheet.Column(7).AutoFit();
            //    worksheet.Column(8).AutoFit();
            //    worksheet.Column(9).AutoFit();
            //    worksheet.Column(10).AutoFit();
            //    worksheet.Cells["K1:R1"].Merge = true;
            //    worksheet.Cells["K1:R1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            //    worksheet.Cells["K2:R10"].Merge = true;
            //    worksheet.Cells["K2:R10"].Style.WrapText = true;
            //    worksheet.Cells["K2:R10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
            //    worksheet.Cells["K2:R10"].Style.Font.Size = 10;




            //    worksheet.Cells["K12:R12"].Merge = true;
            //    worksheet.Cells["K12:R12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


            //    worksheet.Cells["K13:M13"].Merge = true;
            //    worksheet.Cells["K13:R13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            //    worksheet.Cells["N13:P13"].Merge = true;
            //    worksheet.Cells["N13:P13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            //    worksheet.Cells["Q13:R13"].Merge = true;
            //    worksheet.Cells["Q13:R13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


            //    for (int i = 14; i < 1000; i++)
            //    {
            //        worksheet.Cells[i, 11, i, 13].Merge = true;
            //        worksheet.Cells[i, 14, i, 16].Merge = true;
            //        worksheet.Cells[i, 17, i, 18].Merge = true;
            //    }

            //    worksheet.Protection.IsProtected = true;
            //    worksheet.Column(8).Style.Locked = false;
            //    worksheet.Column(9).Style.Locked = false;
            //    worksheet.Column(10).Style.Locked = false;
            //    worksheet.Cells["$K$14:$K$1000"].Style.Locked = false;
            //    worksheet.Cells["$L$14:$L$1000"].Style.Locked = false;
            //    worksheet.Cells["$M$14:$M$1000"].Style.Locked = false;
            //    worksheet.Cells["$N$14:$N$1000"].Style.Locked = false;
            //    worksheet.Cells["$O$14:$O$1000"].Style.Locked = false;
            //    worksheet.Cells["$P$14:$P$1000"].Style.Locked = false;
            //    worksheet.Cells["$Q$14:$Q$1000"].Style.Locked = false;
            //    worksheet.Cells["$R$14:$R$1000"].Style.Locked = false;


            //    //worksheet.Cells["13", "K", "1000", "K"].Merge = true;

            //    worksheet.Protection.SetPassword("piraeus");


            //    var dropdownlist = worksheet.DataValidations.AddListValidation("$H$2:$H$10000");
            //    dropdownlist.Formula.Values.Add("Da");
            //    dropdownlist.Formula.Values.Add("Nu");

            //    var dropdownlistObs = worksheet.DataValidations.AddListValidation("$J$2:$J$10000");
            //    dropdownlistObs.Formula.Values.Add("DEFECT");
            //    dropdownlistObs.Formula.Values.Add("DETERIORAT");

            //    worksheet.Row(1).Height = 45.75;
            //    worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //    worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //    worksheet.View.FreezePanes(14, 19);
            //    worksheet.Cells["K11"].Value = "";
            //    worksheet.View.ZoomScale = 70;

            //  //dropdownlist.Formula.ExcelFormula = "=$H$2:$H$1000000!Da";
            //  //dropdownlist.ShowErrorMessage = true;
            //  ////dropdownlist.ErrorStyle = ExcelDataValidationWarningStyle.stop;
            //  ////dropdownlist.ErrorTitle = "Error";
            //  //dropdownlist.Error = "Error Text";


            //  //var validation = worksheet.DataValidations.AddListValidation("H2");
            //  //validation.Formula.Values.Add("Da");
            //  //validation.Formula.Values.Add("Nu");
            //  //validation.ShowErrorMessage = true;
            //  //validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
            //  //validation.ErrorTitle = "Error";
            //  //validation.Error = "Error Text";
            //  //// sheet with a name : DropDownLists 
            //  //// from DropDownLists sheet, get values from cells: !$A$1:$A$10
            //  //var formula = "=Nota_transfer!$H$2:$H$2";
            //  ////Applying Formula to the range
            //  //validation.Formula.ExcelFormula = formula;
            //  //Process.Start("mailto:hello@test.com&subject=This is the subject&body=This is the body");




            //  using (var cells = worksheet.Cells[1, 1, 1, 10])
            //    {
            //        cells.Style.Font.Bold = true;
            //        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //        cells.Style.Fill.BackgroundColor.SetColor(Color.Silver);

            //    }

            //    using (var cells = worksheet.Cells[2, 11, 10, 18])
            //    {
            //        cells.Style.Font.Bold = true;
            //        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //        cells.Style.Fill.BackgroundColor.SetColor(Color.Silver);

            //    }

            //    using (var cells = worksheet.Cells[1, 11, 1, 18])
            //    {
            //        cells.Style.Font.Bold = true;
            //        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //        cells.Style.Fill.BackgroundColor.SetColor(Color.Red);

            //    }

            //    using (var cells = worksheet.Cells[12, 11, 12, 18])
            //    {
            //        cells.Style.Font.Bold = true;
            //        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //        cells.Style.Fill.BackgroundColor.SetColor(Color.Red);

            //    }

            //    using (var cells = worksheet.Cells[13, 11, 13, 18])
            //    {
            //        cells.Style.Font.Bold = true;
            //        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //        cells.Style.Fill.BackgroundColor.SetColor(Color.Silver);

            //    }

            //    //using (FileStream aFile = new FileStream(@"D:\asdf.xlsx", FileMode.Create))
            //    //{
            //    //    aFile.Seek(0, SeekOrigin.Begin);
            //    //    package.SaveAs(aFile);

            //    //}
            //    package.Save();
            //    //package.SaveAs(new FileInfo(sFileName));


            //    //package.SaveAs(new FileInfo(sFileName));

            //    Response.Clear();
            //    //Response.Headers.Add("content-disposition", "attachment;  filename=transferuri.xlsx");
            //    // Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //    //var bytes = package.GetAsByteArray();
            //    // Response.Body.WriteAsync(bytes, 0, bytes.Length);

            //    //Process.Start("cmd", "/C start " + file.FullName.ToString());

            //    //var filePath = @"D:\Adrian\Fais\trunk\fais-api\Optima.Fais.Api\wwwroot\transferuri.xlsx";
            //    //var fileName = "transferuri.xlsx";
            //    //var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            //    //FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(filePath), mimeType)
            //    //{
            //    //    FileDownloadName = fileName
            //    //};

            //    // Bnr 

            //    string sWebRootFolderBnr = hostingEnvironment.WebRootPath;
            //    //string sWebRootFolder = "D:\\";

            //    string sFileNameBnr = @"RBM.xlsx";
            //    string URLBnr = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileNameBnr);
            //    FileInfo fileBnr = new FileInfo(Path.Combine(sWebRootFolderBnr, sFileNameBnr));

            //    using (ExcelPackage excelPackageBnr = new ExcelPackage(fileBnr))
            //    {
            //        ExcelWorkbook excelWorkBookBnr = excelPackageBnr.Workbook;
            //        ExcelWorksheet excelWorksheetBnr = excelWorkBookBnr.Worksheets.First();
            //        excelWorksheetBnr.Cells[1, 1].Value = "Test";
            //        excelWorksheetBnr.Cells[3, 2].Value = "Test2";
            //        excelWorksheetBnr.Cells[3, 3].Value = "Test3";

            //        excelPackageBnr.Save();
            //    }



            //    return Ok();


            //} 

            // ----original-----//

        }

        [HttpPost]
        [Route("downloadMailOpsBnr")]
        [EnableCors("MyPolicy")]
        public IActionResult ExportBnr([FromBody] AssetOpConf[] operations)
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"Transferuri.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));



            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }

            using (ExcelPackage package = new ExcelPackage(file))
            {

                Dictionary<string, int> locationIndexes = new Dictionary<string, int>();
                Dictionary<string, int> locationNames = new Dictionary<string, int>();
                int recordIndex = 0;

                foreach (var item in operations)
                {
                    string sheetName = item.LocationCodeFin;

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];

                    worksheet = package.Workbook.Worksheets[sheetName];


                    if (worksheet == null)
                    {
                        worksheet = package.Workbook.Worksheets.Add(sheetName);
                        recordIndex = 2;
                        locationIndexes.Add(sheetName, recordIndex);

                        locationNames.Add(item.LocationCodeFin, recordIndex);


                        worksheet.Cells[1, 1].Value = "Fixed Assets Class";
                        worksheet.Cells[1, 2].Value = "Inventory Item ";
                        worksheet.Cells[1, 3].Value = "Inventory Number (Barcode if implemented)";
                        worksheet.Cells[1, 4].Value = "Date of Entrance ";
                        worksheet.Cells[1, 5].Value = "Type / Mark";
                        worksheet.Cells[1, 6].Value = "Serial Number   (if applicable)";
                        worksheet.Cells[1, 7].Value = "Old cost center";
                        worksheet.Cells[1, 8].Value = "Old Location";
                        worksheet.Cells[1, 9].Value = "New cost center";
                        worksheet.Cells[1, 10].Value = "New Location";
                        worksheet.Cells[1, 11].Value = "Confirm";
                        worksheet.Cells[1, 12].Value = "Obs.";
                        worksheet.Cells[1, 13].Value = "OptimaId";
                    }
                    else
                    {
                        recordIndex = locationIndexes[sheetName];
                        recordIndex--;
                    }

                    worksheet.Cells[recordIndex, 1].Value = item.AssetTypeName;
                    worksheet.Cells[recordIndex, 2].Value = item.AssetName;
                    worksheet.Cells[recordIndex, 3].Value = item.InvNo;
                    worksheet.Cells[recordIndex, 4].Value = item.PurchaseDate;
                    //  worksheet.Cells[recordIndex, 5].Value = item.LocationCodeIni;
                    worksheet.Cells[recordIndex, 6].Value = item.SerialNumber;
                    worksheet.Cells[recordIndex, 7].Value = item.RoomCodeIni;
                    worksheet.Cells[recordIndex, 8].Value = item.LocationCodeIni;
                    worksheet.Cells[recordIndex, 9].Value = item.RoomCodeFin;
                    worksheet.Cells[recordIndex, 10].Value = item.LocationCodeFin;
                    worksheet.Cells[recordIndex, 11].Value = "Nu";
                    worksheet.Cells[recordIndex, 13].Value = item.AssetOpId;
                    //worksheet.Cells["C3:F3"].Value = item.LocationCodeIni;
                    //worksheet.Cells["C4:F4"].Value = item.LocationCodeFin;
                    worksheet.View.ShowGridLines = false;
                    worksheet.Cells[recordIndex, 1].Style.Numberformat.Format = "@";
                    worksheet.Cells[recordIndex, 2].Style.Numberformat.Format = "@";
                    worksheet.Cells[recordIndex, 3].Style.Numberformat.Format = "@";
                    worksheet.Cells[recordIndex, 4].Style.Numberformat.Format = "mm-dd-yy";
                    worksheet.Cells[recordIndex, 5].Style.Numberformat.Format = "@";
                    worksheet.Cells[recordIndex, 6].Style.Numberformat.Format = "@";
                    worksheet.Cells[recordIndex, 7].Style.Numberformat.Format = "@";
                    worksheet.Cells[recordIndex, 8].Style.Numberformat.Format = "@";
                    worksheet.Cells[recordIndex, 9].Style.Numberformat.Format = "@";
                    worksheet.Cells[recordIndex, 10].Style.Numberformat.Format = "@";
                    worksheet.Cells[recordIndex, 11].Style.Numberformat.Format = "@";


                    for (int i = 1; i < 11; i++)
                    {
                        worksheet.Cells[recordIndex, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[recordIndex, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[recordIndex, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[recordIndex, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 255, 191));
                        worksheet.Cells[recordIndex, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));
                    }

                    for (int i = 11; i < 12; i++)
                    {
                        worksheet.Cells[recordIndex, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[recordIndex, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        worksheet.Cells[recordIndex, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[recordIndex, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(178, 148, 83));
                        worksheet.Cells[recordIndex, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));
                    }

                    for (int i = 12; i < 13; i++)
                    {
                        worksheet.Cells[recordIndex, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[recordIndex, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[recordIndex, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[recordIndex, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[recordIndex, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 232, 184));
                        worksheet.Cells[recordIndex, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));

                    }

                    for (int i = 1; i < 13; i++)
                    {
                        worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(159, 178, 78));
                        worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Arial Narrow", 12));
                       // worksheet.Cells.Style.Font.Bold = true;
                    }
                    recordIndex++;

                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();
                    worksheet.Column(5).AutoFit();
                    worksheet.Column(6).AutoFit();
                    worksheet.Column(7).AutoFit();
                    worksheet.Column(8).AutoFit();
                    worksheet.Column(9).AutoFit();
                    worksheet.Column(10).AutoFit();
                    worksheet.Column(11).AutoFit();
                    worksheet.Column(12).AutoFit();

                    //worksheet.Protection.IsProtected = true;
                    //worksheet.Column(11).Style.Locked = false;
                    //worksheet.Column(12).Style.Locked = false;
                    worksheet.Column(13).Hidden = true;
                    worksheet.Row(1).Height = 20;

                    //worksheet.Cells["$K$14:$K$1000"].Style.Locked = false;
                    //worksheet.Cells["$L$14:$L$1000"].Style.Locked = false;
                    //worksheet.Cells["$M$14:$M$1000"].Style.Locked = false;
                    //worksheet.Cells["$N$14:$N$1000"].Style.Locked = false;
                    //worksheet.Cells["$O$14:$O$1000"].Style.Locked = false;
                    //worksheet.Cells["$P$14:$P$1000"].Style.Locked = false;
                    //worksheet.Cells["$Q$14:$Q$1000"].Style.Locked = false;
                    //worksheet.Cells["$R$14:$R$1000"].Style.Locked = false;


                    //worksheet.Cells["13", "K", "1000", "K"].Merge = true;

                    //  worksheet.Protection.SetPassword("bnr");

                    var validations = worksheet.DataValidations;
                    if (validations.Count > 0)
                    {

                    }
                    else
                    {
                        int rowNumber = recordIndex - 1;
                        var dropdownlist = worksheet.DataValidations.AddListValidation("$K$" + rowNumber + ":$K$10000");
                        dropdownlist.Formula.Values.Add("Da");
                        dropdownlist.Formula.Values.Add("Nu");

                        var dropdownlistObs = worksheet.DataValidations.AddListValidation("$L$" + rowNumber + ":$L$10000");
                        dropdownlistObs.Formula.Values.Add("DEFECT");
                        dropdownlistObs.Formula.Values.Add("DETERIORAT");
                    }

                    recordIndex++;
                    locationIndexes[sheetName] = recordIndex;

                   
                }


                package.Save();
                Response.Clear();

               
                return Ok();

            }


           

        }


        [HttpGet("download")]
        public async Task<IActionResult> Download()
     
        {
            await Task.Delay(5000);
            var sWebRootFolder = hostingEnvironment.WebRootPath;
            var fileName = "Nota_transfer.xlsx";
            FileInfo filePath = new FileInfo(Path.Combine(sWebRootFolder, fileName));
            
            var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            HttpContext.Response.ContentType = mimeType;
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(filePath.ToString()), mimeType)
            {
                FileDownloadName = fileName
            };
            
            return result;
        }

        [HttpGet("downloadBnr")]
        public async Task<IActionResult> DownloadBnr()

        {
            await Task.Delay(5000);
            var sWebRootFolder = hostingEnvironment.WebRootPath;
            var fileName = "Transferuri.xlsx";
            FileInfo filePath = new FileInfo(Path.Combine(sWebRootFolder, fileName));

            var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            HttpContext.Response.ContentType = mimeType;
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(filePath.ToString()), mimeType)
            {
                FileDownloadName = fileName
            };

            return result;
        }

        [HttpDelete("deleteAssetOp/{assetOpId}")]
        public void DeleteAsset(int assetOpId)
        {

           var deleteAssetOpId = (_itemsRepository as IAssetOpsRepository).DeleteAssetOp(assetOpId);
            _context.SaveChanges();
        }

        [HttpDelete("validateAssetOpTemp/{assetOpId}")]
        public async Task ValidateAsset(int assetOpId)
        {

            var deleteAssetOpId = (_itemsRepository as IAssetOpsRepository).ValidateAssetOp(assetOpId);

            _context.SaveChanges();

            Model.AssetOp assetOp = null;

            assetOp = _context.Set<Model.AssetOp>().Include(a => a.Asset).Where(op => op.Id == assetOpId).FirstOrDefault();

            // Transfer //

            Model.Employee employeeIni = null;
            Model.Employee employeeFin = null;
            Model.Room roomIni = null;
            Model.Room roomFin = null;
            Model.Location locationIni = null;
            Model.Location locationFin = null;
            Model.AssetCategory assetCategoryIni = null;
            Model.AssetCategory assetCategoryFin = null;


            var assetCategoryNameIni = "-";
            var assetCategoryNameFin = "-";

            var roomNameIni = "";
            var roomNameFin = "";
            var locationCodeIni = "";
            var locationCodeFin = "";
            var employeeNameIni = "";
            var employeeNameFin = "";

            if (assetOp.RoomIdInitial != null)
            {
                roomIni = _context.Set<Model.Room>().Where(a => a.Id == assetOp.RoomIdInitial).FirstOrDefault();

                if (roomIni != null)
                {
                    locationIni = _context.Set<Model.Location>().Where(a => a.Id == roomIni.LocationId).FirstOrDefault();
                }
            }

            if (assetOp.RoomIdFinal != null)
            {
                roomFin = _context.Set<Model.Room>().Where(a => a.Id == assetOp.RoomIdFinal).FirstOrDefault();

                if (roomFin != null)
                {
                    locationFin = _context.Set<Model.Location>().Where(a => a.Id == roomFin.LocationId).FirstOrDefault();
                }
            }
            else
            {
                if (assetOp.RoomIdInitial != null)
                {
                    roomFin = _context.Set<Model.Room>().Where(a => a.Id == assetOp.RoomIdInitial).FirstOrDefault();

                    if (roomFin != null)
                    {
                        locationFin = _context.Set<Model.Location>().Where(a => a.Id == roomFin.LocationId).FirstOrDefault();
                    }
                }
            }


            if (assetOp.EmployeeIdInitial != null)
            {

                employeeIni = _context.Set<Model.Employee>().Where(a => a.Id == assetOp.EmployeeIdInitial).FirstOrDefault();
            }

            if (assetOp.EmployeeIdFinal != null)
            {

                employeeFin = _context.Set<Model.Employee>().Where(a => a.Id == assetOp.EmployeeIdFinal).FirstOrDefault();
            }

            if (assetOp.AssetCategoryIdInitial != null)
            {
                assetCategoryIni = _context.Set<Model.AssetCategory>().Where(a => a.Id == assetOp.AssetCategoryIdInitial).FirstOrDefault();
            }

            if (assetOp.AssetCategoryIdFinal != null)
            {
                assetCategoryFin = _context.Set<Model.AssetCategory>().Where(a => a.Id == assetOp.AssetCategoryIdFinal).FirstOrDefault();
            }

            var assetCategIni = assetCategoryIni != null ? assetCategoryIni.Name : assetCategoryNameIni;
            var assetCategFin = assetCategoryFin != null ? assetCategoryFin.Name : assetCategoryNameFin;
            var lIniName = locationIni != null ? locationIni.Name : locationCodeIni;
            var lFinName = locationFin != null ? locationFin.Name : locationCodeFin;
            var rIniName = roomIni != null ? roomIni.Name : roomNameIni;
            var rFinName = roomFin != null ? roomFin.Name : roomNameFin;
            var eIni = employeeIni != null ? employeeIni.FirstName + " " + employeeIni.LastName : employeeNameIni;
            var eFin = employeeFin != null ? employeeFin.FirstName + " " + employeeFin.LastName : employeeNameFin;

            var emailIni = employeeIni != null ? employeeIni.Email != null ? employeeIni.Email : "adrian.cirnaru@optima.ro" : "adrian.cirnaru@optima.ro";
            //var emailCC = "dvladoiu@stanleybet.ro";

            //var emailIni = "adrian.cirnaru@optima.ro";
            //var emailFin = "adrian.cirnaru@optima.ro";
            var emailCC = "adrian.cirnaru@optima.ro";

            //assets.Add(" - " + assetTransfer.InvNo + " " + assetTransfer.Name + " from " + assetTransfer.AssetAdm.Employee.InternalCode + " " + "(" + assetTransfer.AssetAdm.Employee.FirstName + " " + assetTransfer.AssetAdm.Employee.LastName + ")" + "\r\n");






            //var result = String.Join(Environment.NewLine,
            //             assets.Select(a => String.Join("\r\n", a)));

            //  var password = base64Decode2(user.PasswordHash);

            //var email = "adrian.cirnaru@optima.ro";
            //var email1 = "mdeaconescu@stanleybet.ro";
            //var email2 = "cosmina.cretu@optima.ro";


            var subject = "Temporary transfer has been confirmed";

            string htmlBody = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    Upcoming topics
                                                </title>
                                                <style type=""text/css"">
                                                    HTML{background-color: #e8e8e8;}
                                                    .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
                                                    .courses-table .description{color: #505050;}
                                                    .courses-table td{border: 1px solid #D1D1D1; background-color: #F3F3F3; padding: 0 10px;}
                                                    .courses-table th{border: 1px solid #424242; color: #FFFFFF;text-align: left; padding: 0 10px;}
                                                    .red{background-color: #003880;}
                                                    .green{background-color: #6B9852;}
                                                </style>
                                            </head>
                                            <body>
                                                <h2>Dear user, the temporary transfer have been validated:</h2>
                                                <table class=""courses-table"">
                                                    <thead>
                                                        <tr>
                                                            <th class=""red""></th>
                                                            <th class=""red"">Asset number</th>
                                                            <th class=""red"">Description</th>
                                                            <th class=""red"">Employee</th>
                                                            <th class=""red"">Building</th>
                                                            <th class=""red"">Room</th>
                                                            <th class=""red"">Category</th>
                                                            <th class=""red"">Asset value</th>
                                                           
                                                          
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                      
                                                        <tr>
                                                           <td class=""description"">Initial</ td >
                                                            <td class=""description"">" + assetOp.Asset.InvNo + @" </ td >
                                                            <td class=""description"">" + assetOp.Asset.Name + @" </ td >
                                                            <td class=""description"">" + eIni + @" </ td >
                                                            <td class=""description"">" + lIniName + @" </ td >
                                                            <td class=""description"">" + rIniName + @" </ td >
                                                            <td class=""description"">" + assetCategIni + @" </ td >
                                                            <td class=""description"">" + assetOp.Asset.ValueInv + @" </ td >
                                                           
                                                       
                                                        </tr>
                                                          <tr>
                                                             <td class=""description"">Final</ td >
                                                            <td class=""description"">" + assetOp.Asset.InvNo + @" </ td >
                                                            <td class=""description"">" + assetOp.Asset.Name + @" </ td >
                                                            <td class=""description"">" + eFin + @" </ td >
                                                            <td class=""description"">" + lFinName + @" </ td >
                                                            <td class=""description"">" + rFinName + @" </ td >
                                                            <td class=""description"">" + assetCategFin + @" </ td >
                                                            <td class=""description"">" + assetOp.Asset.ValueInv + @" </ td >
                                                           
                                                        </tr>
                                                    </tbody>
                                                </table>

                                        <h5>This is an automated message – Please do not replay directly to this email!</h5>
                                        <h5>For more details please contact the IT Administration at following email: adrian.cirnaru@optima.ro</h5>

                                            </body>
                                        </html>
                                        ";

            // STANLEYBET //

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Transferuri", "ofa@optima.ro"));
            //emailMessage.From.Add(new MailboxAddress("Transferuri", "inventar@stanleybet.ro"));
            emailMessage.To.Add(new MailboxAddress("", emailIni));
            //emailMessage.To.Add(new MailboxAddress("", email1));
            //emailMessage.To.Add(new MailboxAddress("", emailFin));
            emailMessage.To.Add(new MailboxAddress("", emailCC));

            emailMessage.Subject = subject;
            //emailMessage.Body = new TextPart("plain") { Text = message };

            var builder = new BodyBuilder { TextBody = htmlBody, HtmlBody = htmlBody };
            //  builder.Attachments.Add(attachmentName, attachment, ContentType.Parse(attachmentType));
            emailMessage.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {

                //await client.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
                //client.AuthenticationMechanisms.Remove("XOAUTH2");

                //await client.AuthenticateAsync("ofa@optima.ro", "Inventory2019");
                //await client.SendAsync(emailMessage).ConfigureAwait(false);
                //await client.DisconnectAsync(true).ConfigureAwait(false);

            }

            // STANLEYBET



            // Transfer //
        }

        [HttpPost]
        [Route("recoprocess/{isManagerTransfer}")]
        public async virtual Task<IActionResult> RecoAssetOpProcess([FromBody] int[] assetOpIds, int isManagerTransfer)
        {
            Model.Asset assetPrev = null;
            Model.Asset assetTemp = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.InventoryAsset inventoryAssetTemp = null;
            Model.Inventory inventory = null;
            // Model.AccMonth accMonth = null;
            Model.AssetOp assetOpPrev = null;
            Model.AssetAdmMD assetAdmMDPrev = null;
            //IQueryable<Model.AssetOp> assetOps2 = null;
            //IQueryable<Model.AssetOp> assetOps3 = null;
            assetOpIds = assetOpIds.Distinct().ToArray();
            Model.AssetOp assetOpNew = null;
            //assetOps2 = _context.AssetOps.AsNoTracking();
            //assetOps3 = _context.AssetOps.AsNoTracking();

            var ops = assetOpIds.ToList();

            for (int i = 0; i < ops.Count; i++)
            {

                // accMonth = _context.Set<Model.AccMonth>().Where(acc => acc.IsActive == true).Single();
                inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(acc => acc.Active == true).Single();
                assetOpPrev = _context.Set<Model.AssetOp>().Where(a => a.Id == ops[i]).SingleOrDefault();
                assetPrev = _context.Set<Model.Asset>().Where(a => a.Id == assetOpPrev.AssetId).SingleOrDefault();
                assetAdmMDPrev = _context.Set<Model.AssetAdmMD>().Where(a => a.AssetId == assetOpPrev.AssetId && a.AccMonthId == inventory.AccMonthId).SingleOrDefault();
                inventoryAsset = _context.Set<Model.InventoryAsset>().Where(a => a.AssetId == assetOpPrev.AssetId && a.InventoryId == inventory.Id).SingleOrDefault();
                var tempInvnNo = inventoryAsset.TempReco;

                string[] split = tempInvnNo.Split(";");

                for (int t = 0; t < split.Length; t++)
                {
                    assetTemp = _context.Set<Model.Asset>().Where(a => a.InvNo == split[t] && a.IsDeleted == false && a.AssetRecoStateId == 79).SingleOrDefault();

                    if (assetTemp != null && assetAdmMDPrev != null && inventoryAsset != null)
                    {
                        inventoryAssetTemp = _context.Set<Model.InventoryAsset>().Include(a => a.Asset).Where(a => a.AssetId == assetTemp.Id && a.InventoryId == inventory.Id).SingleOrDefault();

                        if (assetOpPrev.AssetOpStateId < 81)
                        {


                            string userName = HttpContext.User.Identity.Name;
                            var user = _context.Users.Where(u => u.Id == "92E74C4F-A79A-4C83-A7D0-A3202BD2507F").Single();

                            if (assetOpPrev.AssetOpStateId == isManagerTransfer)
                            {
                                switch (assetOpPrev.AssetOpStateId)
                                {
                                    case 1:
                                        assetOpPrev.ReleaseConfAt = DateTime.Now;
                                        assetOpPrev.ReleaseConfBy = user != null ? user.Id : "92E74C4F-A79A-4C83-A7D0-A3202BD2507F";
                                        break;
                                    case 79:

                                        inventoryAsset.RoomIdFinal = split[t].StartsWith("WFH") ? 34072 : assetOpPrev.RoomIdFinal;
                                        inventoryAsset.EmployeeIdFinal = assetOpPrev.EmployeeIdFinal;
                                        inventoryAsset.SerialNumber = assetOpPrev.SerialNumber;
                                        inventoryAsset.StateIdFinal = assetOpPrev.InvStateIdFinal;
                                        inventoryAsset.Model = assetOpPrev.Model;
                                        inventoryAsset.Producer = assetOpPrev.Producer;
                                        inventoryAsset.CostCenterIdFinal = split[t].StartsWith("WFH") ? 600 : assetOpPrev.CostCenterIdFinal;
                                        inventoryAsset.QFinal = assetOpPrev.Quantity;
                                        inventoryAsset.Info = assetOpPrev.Info;
                                        inventoryAsset.Info2019 = assetOpPrev.Info2019;
                                        inventoryAsset.AdministrationIdFinal = split[t].StartsWith("WFH") ? 149 : assetOpPrev.AdministrationIdFinal;
                                        inventoryAsset.ModifiedAt = inventoryAssetTemp.Asset.CreatedAt;
                                        inventoryAsset.ModifiedBy = inventoryAssetTemp.Asset.CreatedBy != null ? inventoryAssetTemp.Asset.CreatedBy : "92E74C4F-A79A-4C83-A7D0-A3202BD2507F";
                                        inventoryAsset.SerialNumber = inventoryAssetTemp.Asset.SerialNumber;



										assetPrev.RoomId = split[t].StartsWith("WFH") ? 34072 : assetOpPrev.RoomIdFinal;
                                        assetPrev.EmployeeId = assetOpPrev.EmployeeIdFinal;
                                        assetPrev.InvStateId = assetOpPrev.InvStateIdFinal;
                                        assetPrev.CostCenterId = split[t].StartsWith("WFH") ? 600 :assetOpPrev.CostCenterIdFinal;

                                        Model.CostCenter costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == assetPrev.CostCenterId).SingleOrDefault();

                                        assetPrev.DepartmentId = costCenter.Division.DepartmentId;
                                        assetPrev.DivisionId = costCenter.DivisionId;

                                        assetPrev.AdministrationId = split[t].StartsWith("WFH") ? 149 : assetOpPrev.AdministrationIdFinal;
                                        assetPrev.ModifiedAt = DateTime.Now;
                                        assetPrev.ModifiedBy = inventoryAssetTemp.Asset.CreatedBy != null ? inventoryAssetTemp.Asset.CreatedBy : "92E74C4F-A79A-4C83-A7D0-A3202BD2507F";
                                        assetPrev.SerialNumber = assetTemp.SerialNumber;

                                        if (assetTemp.InvNo.StartsWith("WFH"))
                                        {
											assetPrev.Imei = assetTemp.Imei;
											assetPrev.PhoneNumber = assetTemp.PhoneNumber;
											assetPrev.DictionaryItemId = assetTemp.DictionaryItemId;
											assetPrev.BrandId = assetTemp.BrandId;
											assetPrev.ModelId = assetTemp.ModelId;
											assetPrev.SAPCode = assetTemp.SAPCode;
										}

                                        assetAdmMDPrev.RoomId = split[t].StartsWith("WFH") ? 34072 : assetOpPrev.RoomIdFinal;
                                        assetAdmMDPrev.EmployeeId = assetOpPrev.EmployeeIdFinal;
                                        assetAdmMDPrev.CostCenterId = split[t].StartsWith("WFH") ? 600 : assetOpPrev.CostCenterIdFinal;

                                        costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == assetAdmMDPrev.CostCenterId).SingleOrDefault();

                                        assetAdmMDPrev.DepartmentId = costCenter.Division.DepartmentId;
                                        assetAdmMDPrev.DivisionId = costCenter.DivisionId;

                                        assetAdmMDPrev.AdministrationId = split[t].StartsWith("WFH") ? 149 : assetOpPrev.AdministrationIdFinal;

                                        assetTemp.IsDeleted = true;
                                        assetTemp.ModifiedAt = DateTime.Now;
                                        assetTemp.ModifiedBy = inventoryAssetTemp.Asset.CreatedBy != null ? inventoryAssetTemp.Asset.CreatedBy : "92E74C4F-A79A-4C83-A7D0-A3202BD2507F";

										assetOpNew = new Model.AssetOp
										{
											AssetId = assetOpPrev.AssetId,
											CreatedBy = user.Id,
											RoomIdInitial = assetOpPrev.RoomIdInitial,
											RoomIdFinal = split[t].StartsWith("WFH") ? 34072 : assetOpPrev.RoomIdFinal,
											EmployeeIdInitial = assetOpPrev.EmployeeIdInitial,
											EmployeeIdFinal = assetOpPrev.EmployeeIdFinal,
											CostCenterIdInitial = inventoryAsset.CostCenterIdFinal,
											CostCenterIdFinal = assetOpPrev.CostCenterIdFinal,
											AssetOpStateId = 3,
											SrcConfBy = user.Id,
											SrcConfAt = DateTime.Now,
											Info = assetOpPrev.Info,
											Info2019 = assetOpPrev.Info2019,
											InvStateIdInitial = assetOpPrev.InvStateIdInitial,
											InvStateIdFinal = assetOpPrev.InvStateIdFinal,
											IsDeleted = false,
											AssetTypeIdInitial = assetOpPrev.AssetTypeIdInitial,
											AssetTypeIdFinal = assetOpPrev.AssetTypeIdFinal,
											DepartmentIdInitial = assetOpPrev.DepartmentIdInitial,
											DepartmentIdFinal = assetOpPrev.DepartmentIdFinal,
											AssetCategoryIdInitial = assetOpPrev.AssetCategoryIdInitial,
											AssetCategoryIdFinal = assetOpPrev.AssetCategoryIdFinal,
											AdministrationIdInitial = assetOpPrev.AdministrationIdInitial,
											AdministrationIdFinal = split[t].StartsWith("WFH") ? 149 : assetOpPrev.AdministrationIdFinal,
											AccSystemId = 3,
											AssetStateIdInitial = assetOpPrev.AssetStateIdInitial,
											AssetStateIdFinal = assetOpPrev.AssetStateIdFinal,
											UomId = assetOpPrev.UomId,
											CompanyId = assetOpPrev.CompanyId,
											AssetNatureIdInitial = assetOpPrev.AssetNatureIdInitial,
											AssetNatureIdFinal = assetOpPrev.AssetNatureIdFinal,
											BudgetManagerIdInitial = assetOpPrev.BudgetManagerIdInitial,
											BudgetManagerIdFinal = assetOpPrev.BudgetManagerIdFinal,
											ProjectIdInitial = assetOpPrev.ProjectIdInitial,
											ProjectIdFinal = assetOpPrev.ProjectIdFinal,
											InterCompanyId = assetOpPrev.InterCompanyId,
											InsuranceCategoryId = assetOpPrev.InsuranceCategoryId,
											//UomIdFinal = assetOp.UomId.GetValueOrDefault() > 0 ? assetOp.UomId : null,
											DimensionIdInitial = assetOpPrev.DimensionIdInitial,
											DimensionIdFinal = assetOpPrev.DimensionIdFinal,
											InvName = assetOpPrev.InvName,
											SerialNumber = assetOpPrev.SerialNumber,
											Quantity = assetOpPrev.Quantity,
											AllowLabel = Convert.ToBoolean(assetOpPrev.AllowLabel),
											DocumentId = inventory.DocumentId,
										};

                                        _context.Add(assetOpNew);


										var entityFiles = _context.EntityFiles.Where(e => e.EntityId == assetTemp.Id).ToList();

                                        foreach (Model.EntityFile entityFile in entityFiles)
                                        {
                                            Model.EntityFile newEntityFile = new Model.EntityFile
                                            {
                                                EntityId = inventoryAsset.AssetId,
                                                EntityTypeId = entityFile.EntityTypeId,
                                                FileType = entityFile.FileType,
                                                Info = entityFile.Info,
                                                Name = entityFile.Name,
                                                Size = entityFile.Size,
                                                StoredAs = entityFile.StoredAs,
                                                CostCenterId = entityFile.CostCenterId

											};

                                            _context.EntityFiles.Add(newEntityFile);
                                        }



                                        break;
                                    default:
                                        break;
                                }

                                assetOpPrev.AssetOpStateId = 80;
                                assetPrev.AssetRecoStateId = 80;
                                assetTemp.AssetRecoStateId = 80;
                                inventoryAsset.AssetRecoStateId = 80;
                                inventoryAssetTemp.AssetRecoStateId = 80;
                                _context.UserName = userName;
                                _context.SaveChanges();
                            }


                        }
                    }
                }

            }



            return Ok(ops);
        }

		[HttpPut("deleteAssetOpReco/{assetOpId}")]
        public int DeleteAssetReco(int assetOpId)
        {

            var deleteAssetOpId = (_itemsRepository as IAssetOpsRepository).DeleteAssetOpReco(assetOpId);
            _context.SaveChanges();

            return deleteAssetOpId;
        }

		[HttpGet("exportBM")]
		public IActionResult ExportBMSimple(int? page, int? pageSize, string sortColumn, string sortDirection,
			string includes, int? assetId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string jsonFilter)
		{

			AssetFilter assetFilter = null;
			int totalItems = 0;
			string userName = string.Empty;
			string userId = null;
			string employeeId = string.Empty;
			string roomId = string.Empty;
			string admCenterId = string.Empty;
            string role = string.Empty;

			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

			includes = includes ?? "ReleaseConfUser.Employee,RegisterConfUser.Employee,DstConfUser.Employee,SrcConfUser.Employee,Asset.Company,Document.DocumentType,AssetOpState,Asset,CostCenterInitial.Division.Department,CostCenterFinal.Division.Department,AssetOpState,EmployeeInitial,EmployeeFinal";


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


			List<Model.AssetOp> items = (_itemsRepository as IAssetOpsRepository)
				.GetFiltered(assetFilter, includes, assetId, documentTypeCode, assetOpState, startDate, endDate, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

			if (items.Count > 0)
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					string sheetName = "Reconcilieri";

					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

					int recordIndex = 4;
					int rowNumber = 0;
					int count = items.Count();
					foreach (var item in items)
					{

						rowNumber++;
						int diff = recordIndex - count;

						if (diff > 0)
						{
							diff = 0;
						}
						worksheet.Cells[2, 8].Value = "Centru de cost";
						worksheet.Cells[2, 9].Value = "Departament";
						// worksheet.Cells[2, 11].Value = "ERP";
						worksheet.Cells[2, 10].Value = "B.U.";
						worksheet.Cells[2, 11].Value = "Centru de cost";
						// worksheet.Cells[2, 14].Value = "Judet";
						//worksheet.Cells[2, 15].Value = "Manager Zonal";
						//worksheet.Cells[2, 16].Value = "Manager Regional";
						//worksheet.Cells[2, 17].Value = "Shop Code";
						worksheet.Cells[2, 12].Value = "Departament";
						// worksheet.Cells[2, 19].Value = "ERP CODE";
						worksheet.Cells[2, 13].Value = "B.U.";
						//worksheet.Cells[2, 14].Value = "Oras";
						//worksheet.Cells[2, 22].Value = "Judet";
						worksheet.Cells[2, 14].Value = "Angajat initial";
						worksheet.Cells[2, 15].Value = "Angajat transfer";
						worksheet.Cells[2, 16].Value = "Status";


						worksheet.Cells[3, 1].Value = "1";
						worksheet.Cells[3, 2].Value = "2";
						worksheet.Cells[3, 3].Value = "3";
						worksheet.Cells[3, 4].Value = "4";
						worksheet.Cells[3, 5].Value = "5";
						worksheet.Cells[3, 6].Value = "6";
						worksheet.Cells[3, 7].Value = "7";
						worksheet.Cells[3, 8].Value = "8";
						worksheet.Cells[3, 9].Value = "9";
						worksheet.Cells[3, 10].Value = "10";
						worksheet.Cells[3, 11].Value = "11";
						worksheet.Cells[3, 12].Value = "12";
						worksheet.Cells[3, 13].Value = "13";
						worksheet.Cells[3, 14].Value = "14";
						worksheet.Cells[3, 15].Value = "15";
						worksheet.Cells[3, 16].Value = "16";
						//worksheet.Cells[3, 17].Value = "17";
						//worksheet.Cells[3, 18].Value = "18";
						//worksheet.Cells[3, 19].Value = "19";
						//worksheet.Cells[3, 20].Value = "20";
						//worksheet.Cells[3, 21].Value = "21";
						//worksheet.Cells[3, 22].Value = "22";
						//worksheet.Cells[3, 23].Value = "23";
						//worksheet.Cells[3, 24].Value = "24";
						//worksheet.Cells[3, 25].Value = "25";

						worksheet.Cells[recordIndex, 1].Value = rowNumber;
						worksheet.Cells[recordIndex, 2].Value = item.SrcConfAt != null ? item.SrcConfAt.Value.ToString("dd/MM/yyyy") : ""; // DATA PROPUNERE
						worksheet.Cells[recordIndex, 3].Value = item.DstConfAt != null ? item.DstConfAt.Value.ToString("dd/MM/yyyy") : ""; // DATA VALIDARE
						worksheet.Cells[recordIndex, 4].Value = item.Asset.Name;
						worksheet.Cells[recordIndex, 5].Value = item.SrcConfUser != null && item.SrcConfUser.Employee != null ? item.SrcConfUser.Employee.Email : "";
						// worksheet.Cells[recordIndex, 5].Value = item.Asset.Quantity;
						worksheet.Cells[recordIndex, 6].Value = item.Asset.InvNo;
						//worksheet.Cells[recordIndex, 7].Value = item.Asset.ERPCode;
						worksheet.Cells[recordIndex, 7].Value = item.Asset.ValueInv;
						//worksheet.Cells[recordIndex, 9].Value = item.RoomInitial != null ? item.RoomInitial.Code : "";
						worksheet.Cells[recordIndex, 8].Value = item.CostCenterInitial != null ? item.CostCenterInitial.Code : "";
						// worksheet.Cells[recordIndex, 11].Value = item.RoomInitial != null && item.RoomInitial.Location != null && item.RoomInitial.Location.LocationType != null ? item.RoomInitial.Location.LocationType.Code : "";
						worksheet.Cells[recordIndex, 9].Value = item.CostCenterInitial != null && item.CostCenterInitial.Division != null ? item.CostCenterInitial.Division.Name : "";
						worksheet.Cells[recordIndex, 10].Value = item.CostCenterInitial != null && item.CostCenterInitial.Division != null && item.CostCenterInitial.Division.Department != null ? item.CostCenterInitial.Division.Department.Name : "";
						//worksheet.Cells[recordIndex, 14].Value = item.RoomInitial != null && item.RoomInitial.Location != null && item.RoomInitial.Location.City != null && item.RoomInitial.Location.City.County != null ? item.RoomInitial.Location.City.County.Name : "";
						//worksheet.Cells[recordIndex, 15].Value = item.RoomInitial != null && item.RoomInitial.Location != null && item.RoomInitial.Location.Employee != null ? item.RoomInitial.Location.Employee.FirstName + " " + item.RoomInitial.Location.Employee.LastName : "";
						//worksheet.Cells[recordIndex, 16].Value = item.RoomInitial != null && item.RoomInitial.Location != null && item.RoomInitial.Location.Employee != null && item.RoomInitial.Location.Employee.Division != null ? item.RoomInitial.Location.Employee.Division.FirstName + " " + item.RoomInitial.Location.Employee.Division.LastName : "";

						//worksheet.Cells[recordIndex, 17].Value = item.RoomFinal != null ? item.RoomFinal.Code : "";
						worksheet.Cells[recordIndex, 11].Value = item.CostCenterFinal != null ? item.CostCenterFinal.Code : "";
						//worksheet.Cells[recordIndex, 19].Value = item.RoomFinal != null && item.RoomFinal.Location != null && item.RoomFinal.Location.LocationType != null ? item.RoomFinal.Location.LocationType.Code : "";
						worksheet.Cells[recordIndex, 12].Value = item.CostCenterFinal != null && item.CostCenterFinal.Division != null ? item.CostCenterFinal.Division.Name : "";
						worksheet.Cells[recordIndex, 13].Value = item.CostCenterFinal != null && item.CostCenterFinal.Division != null && item.CostCenterFinal.Division.Department != null ? item.CostCenterFinal.Division.Department.Name : "";
						//worksheet.Cells[recordIndex, 22].Value = item.RoomFinal != null && item.RoomFinal.Location != null && item.RoomFinal.Location.City != null && item.RoomFinal.Location.City.County != null ? item.RoomFinal.Location.City.County.Name : "";
						worksheet.Cells[recordIndex, 14].Value = item.EmployeeInitial != null ? item.EmployeeInitial.Email : "";
						worksheet.Cells[recordIndex, 15].Value = item.EmployeeFinal != null ? item.EmployeeFinal.Email : "";


						worksheet.Cells[recordIndex, 16].Value = item.AssetOpState.Name;


						//worksheet.Cells[recordIndex, 16].Value = item.AssetOpStateId != 4 ? "" : (item.DstConfAt != null ? item.DstConfAt.Value.AddDays(3).ToString("dd/MM/yyyy") : "");
						//worksheet.Cells[recordIndex, 17].Value = "";
						//worksheet.Cells[recordIndex, 21].Value = item.AssetOpStateId != 4 ? (item.SrcConfUser != null ? (item.SrcConfUser.Employee != null ? (item.SrcConfUser.Employee.FirstName + " " + item.SrcConfUser.Employee.LastName) : "") : "") : (item.DstConfUser != null ? (item.DstConfUser.Employee != null ? (item.DstConfUser.Employee.FirstName + " " + item.DstConfUser.Employee.LastName) : "") : "");
						////worksheet.Cells[recordIndex, 13].Value = item.PurchaseDate.Value.ToString("dd-MMMM-yyyy", new CultureInfo("ro-RO"));
						//worksheet.Cells[recordIndex, 14].Value = item.OutDate.Value.ToString("dd-MMMM-yyyy", new CultureInfo("ro-RO"));
						//worksheet.Cells[recordIndex, 15].Value = item.Quantity;
						//worksheet.Cells[recordIndex, 16].Value = item.ValueInv;
						//worksheet.Cells[recordIndex, 17].Value = item.Dep.ValueDepYTD;
						//worksheet.Cells[recordIndex, 18].Value = item.ValueInv - item.Dep.ValueDepYTD;



						if (diff == 0)
						{

							for (int i = 1; i < 17; i++)
							{
								worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
								worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
								worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
								worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 153, 153));
								worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

							}

							worksheet.Row(1).Height = 35.00;
							worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Row(2).Height = 35.00;
							worksheet.Row(2).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Row(3).Height = 15.00;
							worksheet.Row(3).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Row(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.View.FreezePanes(4, 1);

							using (var cells = worksheet.Cells[1, 1, items.Count() + 3, 16])
							{
								cells.Style.Font.Bold = false;
								cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
								cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 153, 153));
								cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
								cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
								cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
								cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
								cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
								cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
								cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
								cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

							}




							worksheet.Cells["A1:A2"].Merge = true;
							worksheet.Cells["A1:A2"].Value = "Nr. Crt";
							worksheet.Cells["B1:B2"].Merge = true;
							worksheet.Cells["B1:B2"].Value = "Data transfer";
							worksheet.Cells["C1:C2"].Merge = true;
							worksheet.Cells["C1:C2"].Value = "Data validare";
							worksheet.Cells["D1:D2"].Merge = true;
							worksheet.Cells["D1:D2"].Value = "Denumire";
							worksheet.Cells["E1:E2"].Merge = true;
							worksheet.Cells["E1:E2"].Value = "Username";
							worksheet.Cells["F1:F2"].Merge = true;
							worksheet.Cells["F1:F2"].Value = "Asset";
							worksheet.Cells["G1:G2"].Merge = true;
							worksheet.Cells["G1:G2"].Value = "Valoare";
							worksheet.Cells["H1:J1"].Merge = true;
							worksheet.Cells["H1:J1"].Value = "Initial";
							worksheet.Cells["K1:P1"].Merge = true;
							worksheet.Cells["K1:P1"].Value = "Final";
							//worksheet.Cells["Q1:X1"].Merge = true;
							//worksheet.Cells["Q1:X1"].Value = "Final";

							//worksheet.Cells["D3:G3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
							//worksheet.Cells["D3:G3"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 213, 180));



							//worksheet.Cells["A3:A4"].Merge = true;
							//worksheet.Cells["B3:B4"].Merge = true;
							//worksheet.Cells["C3:C4"].Merge = true;
							//worksheet.Cells["D3:G3"].Merge = true;
							//worksheet.Cells["D3:G3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
							//worksheet.Cells["D3:G3"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 213, 180));
							//worksheet.Cells["D3:G3"].Value = "Imobilizare";
							//worksheet.Cells["H3:K3"].Merge = true;
							//worksheet.Cells["H3:K3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
							//worksheet.Cells["H3:K3"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(182, 222, 232));
							//worksheet.Cells["H3:K3"].Value = "Predător";
							//worksheet.Cells["L3:O3"].Merge = true;
							//worksheet.Cells["L3:O3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
							//worksheet.Cells["L3:O3"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(216, 228, 188));
							//worksheet.Cells["L3:O3"].Value = "Primitor";
							//worksheet.Cells["P3:U3"].Merge = true;
							//worksheet.Cells["P3:U3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
							//worksheet.Cells["P3:U3"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));
							//worksheet.Cells["V3:V3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
							//worksheet.Cells["V3:V3"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 213, 180));

							using (var cells = worksheet.Cells[3, 1, 3, 16])
							{
								cells.Style.Font.Bold = true;
								cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
								cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 219, 219));
							}

							using (var cells = worksheet.Cells[1, 1, 2, 16])
							{
								cells.Style.Font.Bold = true;
							}

							using (var cells = worksheet.Cells[4, 1, items.Count() + 5, 16])
							{
								cells.Style.Font.Bold = false;
								cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
								cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
							}

							//using (var cells = worksheet.Cells[4, 8, items.Count() + 5, 11])
							//{
							//    cells.Style.Font.Bold = true;
							//    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							//    cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(182, 222, 232));
							//}

							//using (var cells = worksheet.Cells[4, 12, items.Count() + 5, 15])
							//{
							//    cells.Style.Font.Bold = true;
							//    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							//    cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(216, 228, 188));
							//}

							//using (var cells = worksheet.Cells[4, 16, items.Count() + 5, 21])
							//{
							//    cells.Style.Font.Bold = true;
							//    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							//    cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));
							//}

							//using (var cells = worksheet.Cells[4, 18, items.Count() + 5, 18])
							//{
							//    cells.Style.Font.Bold = true;
							//    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							//    cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 213, 180));
							//}

							//using (var cells = worksheet.Cells[1, 1, 1, 18])
							//{
							//    cells.Style.Font.Bold = true;
							//    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							//    cells.Style.Fill.BackgroundColor.SetColor(Color.Khaki);

							//}

							//using (var cells = worksheet.Cells[5, 1, 5, 18])
							//{
							//    cells.Style.Font.Bold = true;
							//    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							//    cells.Style.Fill.BackgroundColor.SetColor(Color.Khaki);


							//}

							using (var cells = worksheet.Cells[3, 1, items.Count() + 5, 16])
							{
								for (int i = 4; i < items.Count() + 4; i++)
								{
									worksheet.Row(i).Height = 20.00;
									worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
									worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

									worksheet.Cells[$"A{i}"].Style.WrapText = true;
									worksheet.Cells[$"B{i}"].Style.WrapText = true;
									worksheet.Cells[$"C{i}"].Style.WrapText = true;
									worksheet.Cells[$"D{i}"].Style.WrapText = true;
									worksheet.Cells[$"E{i}"].Style.WrapText = true;
									worksheet.Cells[$"F{i}"].Style.WrapText = true;
									worksheet.Cells[$"G{i}"].Style.WrapText = true;
									worksheet.Cells[$"H{i}"].Style.WrapText = true;
									worksheet.Cells[$"I{i}"].Style.WrapText = true;
									worksheet.Cells[$"J{i}"].Style.WrapText = true;
									worksheet.Cells[$"K{i}"].Style.WrapText = true;
									worksheet.Cells[$"L{i}"].Style.WrapText = true;
									worksheet.Cells[$"M{i}"].Style.WrapText = true;
									worksheet.Cells[$"N{i}"].Style.WrapText = true;
									worksheet.Cells[$"O{i}"].Style.WrapText = true;
									worksheet.Cells[$"P{i}"].Style.WrapText = true;
									//worksheet.Cells[$"Q{i}"].Style.WrapText = true;
									//worksheet.Cells[$"R{i}"].Style.WrapText = true;
									//worksheet.Cells[$"S{i}"].Style.WrapText = true;
									//worksheet.Cells[$"T{i}"].Style.WrapText = true;
									//worksheet.Cells[$"U{i}"].Style.WrapText = true;
									//worksheet.Cells[$"V{i}"].Style.WrapText = true;

								}



							}


							worksheet.View.ShowGridLines = false;
							worksheet.View.ZoomScale = 100;

							for (int i = 1; i < 17; i++)
							{
								worksheet.Column(i).AutoFit();
							}

							worksheet.Column(1).Width = 11.00;
							worksheet.Column(2).Width = 11.00;
							worksheet.Column(3).Width = 11.00;
							worksheet.Column(4).Width = 30.00;
							worksheet.Column(5).Width = 30.00;
							worksheet.Column(6).Width = 15.00;
							worksheet.Column(7).Width = 10.00;
							worksheet.Column(8).Width = 35.00;
							worksheet.Column(9).Width = 65.00;
							worksheet.Column(10).Width = 15.00;
							worksheet.Column(11).Width = 40.00;
							worksheet.Column(12).Width = 110.00;
							worksheet.Column(13).Width = 20.00;
							worksheet.Column(14).Width = 40.00;
							worksheet.Column(15).Width = 40.00;
							worksheet.Column(16).Width = 15.00;
							//worksheet.Column(17).Width = 20.00;
							//worksheet.Column(18).Width = 20.00;
							//worksheet.Column(19).Width = 20.00;
							//worksheet.Column(20).Width = 80.00;
							//worksheet.Column(21).Width = 10.00;
							//worksheet.Column(22).Width = 30.00;
							//worksheet.Column(23).Width = 30.00;
							//worksheet.Column(24).Width = 30.00;
							//worksheet.Column(25).Width = 30.00;

						}


						//  worksheet.Column(3).Style.Numberformat.Format = "MMMM";
						//  worksheet.Column(13).Style.Numberformat.Format = "yyyy-mm-dd";
						//  worksheet.Column(14).Style.Numberformat.Format = "yyyy-MM-dd";


						//  worksheet.Cells.AutoFitColumns();


						recordIndex++;
					}




					string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					//HttpContext.Response.ContentType = entityFile.FileType;
					HttpContext.Response.ContentType = contentType;
					FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
					{
						FileDownloadName = "Registru Operatii.xlsx"
					};

					return result;

				}


			}

			using (ExcelPackage package = new ExcelPackage())
			{
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Nu exista bonuri");

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "Registru BM.xlsx"
				};

				return result;

			}

		}

		public IEnumerable<string> SplitToLines(string stringToSplit, int maximumLineLength)
		{
			var words = stringToSplit.Split(' ').Concat(new[] { "" });
			return
				words
					.Skip(1)
					.Aggregate(
						words.Take(1).ToList(),
						(a, w) =>
						{
							var last = a.Last();
							while (last.Length > maximumLineLength)
							{
								a[a.Count() - 1] = last.Substring(0, maximumLineLength);
								last = last.Substring(maximumLineLength);
								a.Add(last);
							}
							var test = last + " " + w;
							if (test.Length > maximumLineLength)
							{
								a.Add(w);
							}
							else
							{
								a[a.Count() - 1] = test;
							}
							return a;
						});
		}

	}


}
