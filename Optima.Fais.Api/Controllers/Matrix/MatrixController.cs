using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using Org.BouncyCastle.Utilities;
using PdfSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/matrix")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class MatrixController : GenericApiController<Model.Matrix, Dto.Matrix>
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public MatrixController(ApplicationDbContext context, IMatrixRepository itemsRepository, IMapper mapper, IHostingEnvironment hostingEnvironment)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string assetTypeIds, string projectTypeIds, string areaIds, string countryIds, string companyIds, string divisionIds,
            string costCenterIds, string projectIds, string employeeL1Ids, string employeeL2Ids, string employeeL3Ids, string employeeL4Ids, string employeeS1Ids, string employeeS2Ids, string employeeS3Ids, string exceptMatrixIds, string includes)
        {
            List<Model.Matrix> items = null;
            IEnumerable<Dto.Matrix> itemsResult = null;
            List<int?> aTypeIds = null;
            List<int?> aIds = null;
            List<int?> couIds = null;
            List<int?> comIds = null;
            List<int?> divIds = null;
            List<int?> costIds = null;
            List<int?> projIds = null;
            List<int?> pTypeIds = null;
            List<int?> empL1Ids = null;
            List<int?> empL2Ids = null;
            List<int?> empL3Ids = null;
            List<int?> empL4Ids = null;
            List<int?> empS1Ids = null;
            List<int?> empS2Ids = null;
            List<int?> empS3Ids = null;
            List<int?> exceptIds = null;
            string userName = string.Empty;
            string role = string.Empty;
            string employeeId = string.Empty;

            //if (HttpContext.User.Identity.Name != null)
            //{
            //    userName = HttpContext.User.Identity.Name;
            //    role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
            //    employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

            //    if (role != "administrator")
            //    {
            //        cIds = _context.Set<Model.EmployeeCostCenter>().AsNoTracking().Where(l => l.EmployeeId == int.Parse(employeeId) && l.IsDeleted == false).Select(e => e.CostCenterId).ToList();
            //    }
            //}

            //if (assetTypeIds != null && !assetTypeIds.StartsWith("["))
            //{
            //    assetTypeIds = "[" + assetTypeIds + "]";
            //}

            //if (areaIds != null && !areaIds.StartsWith("["))
            //{
            //    areaIds = "[" + areaIds + "]";
            //}

            //if (countryIds != null && !countryIds.StartsWith("["))
            //{
            //    countryIds = "[" + countryIds + "]";
            //}

            //if (companyIds != null && !companyIds.StartsWith("["))
            //{
            //    companyIds = "[" + companyIds + "]";
            //}

            //if (costCenterIds != null && !costCenterIds.StartsWith("["))
            //{
            //    costCenterIds = "[" + costCenterIds + "]";
            //}

            //if (projectIds != null && !projectIds.StartsWith("["))
            //{
            //    projectIds = "[" + projectIds + "]";
            //}

            //if (projectTypeIds != null && !projectTypeIds.StartsWith("["))
            //{
            //    projectTypeIds = "[" + projectTypeIds + "]";
            //}


            if (divisionIds != null && !divisionIds.StartsWith("["))
            {
                divisionIds = "[" + divisionIds + "]";
            }

            if (employeeL1Ids != null && !employeeL1Ids.StartsWith("["))
            {
                employeeL1Ids = "[" + employeeL1Ids + "]";
            }

            if (employeeL2Ids != null && !employeeL2Ids.StartsWith("["))
            {
                employeeL2Ids = "[" + employeeL2Ids + "]";
            }

            if (employeeL3Ids != null && !employeeL3Ids.StartsWith("["))
            {
                employeeL3Ids = "[" + employeeL3Ids + "]";
            }

            if (employeeL4Ids != null && !employeeL4Ids.StartsWith("["))
            {
                employeeL4Ids = "[" + employeeL4Ids + "]";
            }

            if (employeeS1Ids != null && !employeeS1Ids.StartsWith("["))
            {
                employeeS1Ids = "[" + employeeS1Ids + "]";
            }

            if (employeeS2Ids != null && !employeeS2Ids.StartsWith("["))
            {
                employeeS2Ids = "[" + employeeS2Ids + "]";
            }

            if (employeeS3Ids != null && !employeeS3Ids.StartsWith("["))
            {
                employeeS3Ids = "[" + employeeS3Ids + "]";
            }

            includes = includes ?? "Company,Division.Department,EmployeeL1,EmployeeL2,EmployeeL3,EmployeeL4,EmployeeS1,EmployeeS2,EmployeeS3";

            if ((assetTypeIds != null) && (assetTypeIds.Length > 0)) aTypeIds = JsonConvert.DeserializeObject<string[]>(assetTypeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((areaIds != null) && (areaIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(areaIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((countryIds != null) && (countryIds.Length > 0)) couIds = JsonConvert.DeserializeObject<string[]>(countryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((companyIds != null) && (companyIds.Length > 0)) comIds = JsonConvert.DeserializeObject<string[]>(companyIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((divisionIds != null) && (divisionIds.Length > 0)) divIds = JsonConvert.DeserializeObject<string[]>(divisionIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((costCenterIds != null) && (costCenterIds.Length > 0)) costIds = JsonConvert.DeserializeObject<string[]>(costCenterIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((projectIds != null) && (projectIds.Length > 0)) projIds = JsonConvert.DeserializeObject<string[]>(projectIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((projectTypeIds != null) && (projectTypeIds.Length > 0)) pTypeIds = JsonConvert.DeserializeObject<string[]>(projectTypeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((employeeL1Ids != null) && (employeeL1Ids.Length > 0)) empL1Ids = JsonConvert.DeserializeObject<string[]>(employeeL1Ids).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((employeeL2Ids != null) && (employeeL2Ids.Length > 0)) empL2Ids = JsonConvert.DeserializeObject<string[]>(employeeL2Ids).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((employeeL3Ids != null) && (employeeL3Ids.Length > 0)) empL3Ids = JsonConvert.DeserializeObject<string[]>(employeeL3Ids).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((employeeL4Ids != null) && (employeeL4Ids.Length > 0)) empL4Ids = JsonConvert.DeserializeObject<string[]>(employeeL4Ids).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((employeeS1Ids != null) && (employeeS1Ids.Length > 0)) empS1Ids = JsonConvert.DeserializeObject<string[]>(employeeS1Ids).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((employeeS2Ids != null) && (employeeS2Ids.Length > 0)) empS2Ids = JsonConvert.DeserializeObject<string[]>(employeeS2Ids).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((employeeS3Ids != null) && (employeeS3Ids.Length > 0)) empS3Ids = JsonConvert.DeserializeObject<string[]>(employeeS3Ids).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((exceptMatrixIds != null) && (exceptMatrixIds.Length > 0)) exceptIds = JsonConvert.DeserializeObject<string[]>(exceptMatrixIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IMatrixRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, aTypeIds, pTypeIds, aIds, couIds, comIds, divIds, costIds, projIds, empL2Ids, empL2Ids, empL3Ids, empL4Ids, empS1Ids, empS2Ids, empS3Ids, exceptIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Matrix>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IMatrixRepository).GetCountByFilters(filter, aTypeIds, pTypeIds, aIds, couIds, comIds, divIds, costIds, projIds, empL2Ids, empL2Ids, empL3Ids, empL4Ids, empS1Ids, empS2Ids, empS3Ids, exceptIds);
                var pagedResult = new Dto.PagedResult<Dto.Matrix>(itemsResult, new Dto.PagingInfo()
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

        [HttpGet("data/{projectId}/{costCenterId}")]
        public virtual async Task<IActionResult> GetDownloadData(int projectId, int costCenterId)
        {
            IMatrixRepository repo = _itemsRepository as IMatrixRepository;
            var items = await repo.GetAllMatrixChildrensAsync(projectId, costCenterId);

            return Ok(items.Select(i => _mapper.Map<Dto.MatrixData>(i)));
        }

        //[HttpPost]
        //[Route("import")]
        //public async virtual Task<int> MatrixImport([FromBody] Dto.MatrixImport assetImport)
        //{
        //    if (HttpContext.User.Identity.Name != null)
        //    {
        //        string userName = HttpContext.User.Identity.Name;

        //        var user = _context.Users.Include(r => r.Claims).Where(u => u.Id == "92E74C4F-A79A-4C83-A7D0-A3202BD2507F").SingleOrDefault();
        //        assetImport.UserId = user.Id;
        //    }

        //    var matrixId = (_itemsRepository as IMatrixRepository).MatrixImport(assetImport);

        //    return await matrixId;
        //}

        [HttpGet("export")]
        public IActionResult Export(string filter, string administrationIds, string admCenterIds)
        {
            List<int> aIds = null;
            List<int> admIds = null;
            List<Model.Matrix> matrix = null;

            if ((admCenterIds != null) && (admCenterIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();
            if ((administrationIds != null) && (administrationIds.Length > 0)) admIds = JsonConvert.DeserializeObject<string[]>(administrationIds).ToList().Select(int.Parse).ToList();
            using (ExcelPackage package = new ExcelPackage())
            {
                matrix = (_itemsRepository as IMatrixRepository).GetByFilters(filter, "Company,Division.Department,EmployeeL1,EmployeeL2,EmployeeL3,EmployeeL4,EmployeeS1,EmployeeS2,EmployeeS3", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null).ToList();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("matrice");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Cod Companie";
                worksheet.Cells[1, 2].Value = "Cod B.U.";
                worksheet.Cells[1, 3].Value = "B.U.";
                worksheet.Cells[1, 4].Value = "Cod Departament";
                worksheet.Cells[1, 5].Value = "Departament";
                worksheet.Cells[1, 6].Value = "L4";
                worksheet.Cells[1, 7].Value = "L3";
                worksheet.Cells[1, 8].Value = "L2";
                worksheet.Cells[1, 9].Value = "L1";
                worksheet.Cells[1, 10].Value = "S3";
                worksheet.Cells[1, 11].Value = "S2";
                worksheet.Cells[1, 12].Value = "S1";
                worksheet.Cells[1, 13].Value = "L4 Suma";
                worksheet.Cells[1, 14].Value = "L3 Suma";
                worksheet.Cells[1, 15].Value = "L2 Suma";
                worksheet.Cells[1, 16].Value = "L1 Suma";
                worksheet.Cells[1, 17].Value = "S3 Suma";
                worksheet.Cells[1, 18].Value = "S2 Suma";
                worksheet.Cells[1, 19].Value = "S1 Suma";

                int recordIndex = 2;
                foreach (var item in matrix)
                {
                    worksheet.Cells[recordIndex, 1].Value = item.Company != null ? item.Company.Code : "";
                    worksheet.Cells[recordIndex, 2].Value = item.Division != null && item.Division.Department != null ? item.Division.Department.Code : "";
                    worksheet.Cells[recordIndex, 3].Value = item.Division != null && item.Division.Department != null ? item.Division.Department.Name : "";
                    worksheet.Cells[recordIndex, 4].Value = item.Division != null ? item.Division.Code : "";
                    worksheet.Cells[recordIndex, 5].Value = item.Division != null ? item.Division.Name : "";
                    worksheet.Cells[recordIndex, 6].Value = item.EmployeeL4 != null ? item.EmployeeL4.Email : "";
                    worksheet.Cells[recordIndex, 7].Value = item.EmployeeL3 != null ? item.EmployeeL3.Email : "";
                    worksheet.Cells[recordIndex, 8].Value = item.EmployeeL2 != null ? item.EmployeeL2.Email : "";
                    worksheet.Cells[recordIndex, 9].Value = item.EmployeeL1 != null ? item.EmployeeL1.Email : "";
                    worksheet.Cells[recordIndex, 10].Value = item.EmployeeS3 != null ? item.EmployeeS3.Email : "";
                    worksheet.Cells[recordIndex, 11].Value = item.EmployeeS2 != null ? item.EmployeeS2.Email : "";
                    worksheet.Cells[recordIndex, 12].Value = item.EmployeeS1 != null ? item.EmployeeS1.Email : "";
                    worksheet.Cells[recordIndex, 13].Value = item.AmountL4;
                    worksheet.Cells[recordIndex, 13].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 14].Value = item.AmountL3;
                    worksheet.Cells[recordIndex, 14].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 15].Value = item.AmountL2;
                    worksheet.Cells[recordIndex, 15].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 16].Value = item.AmountL1;
                    worksheet.Cells[recordIndex, 16].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 17].Value = item.AmountS3;
                    worksheet.Cells[recordIndex, 17].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 18].Value = item.AmountS2;
                    worksheet.Cells[recordIndex, 18].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 19].Value = item.AmountS1;
                    worksheet.Cells[recordIndex, 19].Style.Numberformat.Format = "#,##0.00";


                    //worksheet.Cells[recordIndex, 7].Value = item.Administration != null ? item.Administration.Name : "";
                    //worksheet.Cells[recordIndex, 8].Value = item.AdmCenter != null ? item.AdmCenter.Name : "";
                    //worksheet.Cells[recordIndex, 9].Value = item.Region != null ? item.Region.Name : "";
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
                worksheet.Column(11).AutoFit();
                worksheet.Column(12).AutoFit();
                worksheet.Column(13).AutoFit();
                worksheet.Column(14).AutoFit();
                worksheet.Column(15).AutoFit();
                worksheet.Column(16).AutoFit();
                worksheet.Column(17).AutoFit();
                worksheet.Column(18).AutoFit();
                worksheet.Column(19).AutoFit();

                using (var cells = worksheet.Cells[1, 1, 1, 19])
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
                    FileDownloadName = "matrice.xlsx"
                };

                return result;

            }
        }

        [HttpPost]
        [Route("import")]
        public async virtual Task<int> Import([FromBody] Dto.MatrixImport assetImport)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string userName = HttpContext.User.Identity.Name;

                var user = _context.Users.Include(r => r.Claims).Where(u => u.Id == "92E74C4F-A79A-4C83-A7D0-A3202BD2507F").SingleOrDefault();
                assetImport.UserId = user.Id;
            }

            var matrixId = (_itemsRepository as IMatrixRepository).Import(assetImport);

            return await matrixId;
        }

        [HttpGet("matchmatrix/{divisionId}/{value}")]
        public virtual async Task<IActionResult> GetDownloadData(int divisionId, decimal value)
        {
            IMatrixRepository repo = _itemsRepository as IMatrixRepository;
            var items = await repo.GetMatchMatrixAsync(divisionId);

            var result = items.Select(i => _mapper.Map<Dto.Matrix>(i)).ToList();

            if (value > result.ElementAt(0).AmountL4 && value <= result.ElementAt(0).AmountL3)
            {
                if (value > result.ElementAt(0).AmountL3 && value <= result.ElementAt(0).AmountL2)
                {
                    if (value > result.ElementAt(0).AmountL2 && value <= result.ElementAt(0).AmountL1)
                    {
                        if (value > result.ElementAt(0).AmountL1 && value <= result.ElementAt(0).AmountS3)
                        {
                            if (value > result.ElementAt(0).AmountS3 && value <= result.ElementAt(0).AmountS2)
                            {
                                if (value > result.ElementAt(0).AmountS2)
                                {
                                    result.ElementAt(0).EmployeeS1.Validate = true;
                                }
                                else
                                {
                                    result.ElementAt(0).EmployeeS2.Validate = true;
                                }

                            }
                            else
                            {
                                result.ElementAt(0).EmployeeS3.Validate = true;
                            }

                        }
                        else
                        {
                            result.ElementAt(0).EmployeeL1.Validate = true;
                        }

                    }
                    else
                    {
                        result.ElementAt(0).EmployeeL2.Validate = true;
                    }

                }
                else
                {
                    result.ElementAt(0).EmployeeL3.Validate = true;
                }

            }
            else
            {
                result.ElementAt(0).EmployeeL4.Validate = true;

            }

            return Ok(result);
        }

        [HttpGet("matchmatrixeditpanel/{orderId}")]
        public virtual async Task<List<Dto.MatrixTreeNode>> GetEditPanelData(int orderId)
        {
            Model.Order order = null;
            Model.EmailOrderStatus emailOrderStatus = null;

            order = await _context.Set<Model.Order>().Where(a => a.Id == orderId).SingleAsync();
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>()
                .Include(e => e.Matrix).ThenInclude(e => e.EmployeeB1)
                .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL4)
                .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL3)
                .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL2)
                .Include(e => e.Matrix).ThenInclude(e => e.EmployeeL1)
                .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS3)
                .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS2)
                .Include(e => e.Matrix).ThenInclude(e => e.EmployeeS1)
                .Include(e => e.Matrix).ThenInclude(e => e.Division).ThenInclude(e => e.Department)
                .Where(a => a.OrderId == orderId && a.IsDeleted == false)
                .LastOrDefaultAsync();

            int employeeNullId = await _context.Set<Model.Employee>().Where(e => (e.InternalCode == "-" && e.IsDeleted == true)).Select(e => e.Id).FirstOrDefaultAsync();

            List<MatrixTree> childrens = new List<MatrixTree>();

            if (order.EmployeeB1Id != null && order.EmployeeB1Id != employeeNullId)
                childrens.Add(new MatrixTree { Label = "B1", Expanded = true, StyleClass = "department-cfo", Level = "B1", Children = new List<MatrixTree>() { new MatrixTree { Label = order.EmployeeB1.Email, Expanded = true, StyleClass = emailOrderStatus.EmployeeB1EmailSkip ? "skip-validate" : emailOrderStatus.EmployeeB1ValidateAt == null ? "need-to-validate" : "validated", Level = "B1", SkipValidate = emailOrderStatus.EmployeeB1EmailSkip, Validated = emailOrderStatus.EmployeeB1EmailSkip == false && emailOrderStatus.EmployeeB1ValidateAt == null ? false : true, Type = "statusPO", ValidatedDate = emailOrderStatus.EmployeeB1ValidateAt } } });
            if (order.EmployeeL4Id != null && order.EmployeeL4Id != employeeNullId)
                childrens.Add(new MatrixTree { Label = "L4", Expanded = true, StyleClass = "department-cfo", Level = "L4", Children = new List<MatrixTree>() { new MatrixTree { Label = order.EmployeeL4.Email, Expanded = true, StyleClass = emailOrderStatus.EmployeeL4EmailSkip ? "skip-validate" : emailOrderStatus.EmployeeL4ValidateAt == null ? "need-to-validate" : "validated", Level = "L4", SkipValidate = emailOrderStatus.EmployeeL4EmailSkip, Validated = emailOrderStatus.EmployeeL4EmailSkip == false && emailOrderStatus.EmployeeL4ValidateAt == null ? false : true, Type = "statusPO", ValidatedDate = emailOrderStatus.EmployeeL4ValidateAt } } });
            if (order.EmployeeS3Id != null && order.EmployeeS3Id != employeeNullId)
                childrens.Add(new MatrixTree { Label = "S3", Expanded = true, StyleClass = "department-cfo", Level = "S3", Children = new List<MatrixTree>() { new MatrixTree { Label = order.EmployeeS3.Email, Expanded = true, StyleClass = emailOrderStatus.EmployeeS3EmailSkip ? "skip-validate" : emailOrderStatus.EmployeeS3ValidateAt == null ? "need-to-validate" : "validated", Level = "S3", SkipValidate = emailOrderStatus.EmployeeS3EmailSkip, Validated = emailOrderStatus.EmployeeS3EmailSkip == false && emailOrderStatus.EmployeeS3ValidateAt == null ? false : true, Type = "statusPO", ValidatedDate = emailOrderStatus.EmployeeS3ValidateAt } } });
            if (order.EmployeeL3Id != null && order.EmployeeL3Id != employeeNullId)
                childrens.Add(new MatrixTree { Label = "L3", Expanded = true, StyleClass = "department-cfo", Level = "L3", Children = new List<MatrixTree>() { new MatrixTree { Label = order.EmployeeL3.Email, Expanded = true, StyleClass = emailOrderStatus.EmployeeL3EmailSkip ? "skip-validate" : emailOrderStatus.EmployeeL3ValidateAt == null ? "need-to-validate" : "validated", Level = "L3", SkipValidate = emailOrderStatus.EmployeeL3EmailSkip, Validated = emailOrderStatus.EmployeeL3EmailSkip == false && emailOrderStatus.EmployeeL3ValidateAt == null ? false : true, Type = "statusPO", ValidatedDate = emailOrderStatus.EmployeeL3ValidateAt } } });
            if (order.EmployeeS2Id != null && order.EmployeeS2Id != employeeNullId)
                childrens.Add(new MatrixTree { Label = "S2", Expanded = true, StyleClass = "department-cfo", Level = "S2", Children = new List<MatrixTree>() { new MatrixTree { Label = order.EmployeeS2.Email, Expanded = true, StyleClass = emailOrderStatus.EmployeeS2EmailSkip ? "skip-validate" : emailOrderStatus.EmployeeS2ValidateAt == null ? "need-to-validate" : "validated", Level = "S2", SkipValidate = emailOrderStatus.EmployeeS2EmailSkip, Validated = emailOrderStatus.EmployeeS2EmailSkip == false && emailOrderStatus.EmployeeS2ValidateAt == null ? false : true, Type = "statusPO", ValidatedDate = emailOrderStatus.EmployeeS2ValidateAt } } });
            if (order.EmployeeL2Id != null && order.EmployeeL2Id != employeeNullId)
                childrens.Add(new MatrixTree { Label = "L2", Expanded = true, StyleClass = "department-cfo", Level = "L2", Children = new List<MatrixTree>() { new MatrixTree { Label = order.EmployeeL2.Email, Expanded = true, StyleClass = emailOrderStatus.EmployeeL2EmailSkip ? "skip-validate" : emailOrderStatus.EmployeeL2ValidateAt == null ? "need-to-validate" : "validated", Level = "L2", SkipValidate = emailOrderStatus.EmployeeL2EmailSkip, Validated = emailOrderStatus.EmployeeL2EmailSkip == false && emailOrderStatus.EmployeeL2ValidateAt == null ? false : true, Type = "statusPO", ValidatedDate = emailOrderStatus.EmployeeL2ValidateAt } } });
            if (order.EmployeeL1Id != null && order.EmployeeL1Id != employeeNullId)
                childrens.Add(new MatrixTree { Label = "L1", Expanded = true, StyleClass = "department-cfo", Level = "L1", Children = new List<MatrixTree>() { new MatrixTree { Label = order.EmployeeL1.Email, Expanded = true, StyleClass = emailOrderStatus.EmployeeL1EmailSkip ? "skip-validate" : emailOrderStatus.EmployeeL1ValidateAt == null ? "need-to-validate" : "validated", Level = "L1", SkipValidate = emailOrderStatus.EmployeeL1EmailSkip, Validated = emailOrderStatus.EmployeeL1EmailSkip == false && emailOrderStatus.EmployeeL1ValidateAt == null ? false : true, Type = "statusPO", ValidatedDate = emailOrderStatus.EmployeeL1ValidateAt } } });
            if (order.EmployeeS1Id != null && order.EmployeeS1Id != employeeNullId)
                childrens.Add(new MatrixTree { Label = "S1", Expanded = true, StyleClass = "department-cfo", Level = "S1", Children = new List<MatrixTree>() { new MatrixTree { Label = order.EmployeeS1.Email, Expanded = true, StyleClass = emailOrderStatus.EmployeeS1EmailSkip ? "skip-validate" : emailOrderStatus.EmployeeS1ValidateAt == null ? "need-to-validate" : "validated", Level = "S1", SkipValidate = emailOrderStatus.EmployeeS1EmailSkip, Validated = emailOrderStatus.EmployeeS1EmailSkip == false && emailOrderStatus.EmployeeS1ValidateAt == null ? false : true, Type = "statusPO", ValidatedDate = emailOrderStatus.EmployeeS1ValidateAt } } });

            var result = new List<Dto.MatrixTreeNode>();
            var data = new Dto.MatrixTreeNode();

            data.Label = emailOrderStatus.Matrix.Division.Code + " - " + emailOrderStatus.Matrix.Division.Name + " (" + emailOrderStatus.Matrix.Division.Department.Name + " )";
            data.Expanded = true;
            data.Type = "person";
            data.StyleClass = "p-person";
            data.Children = childrens;

            result.Add(data);
            return result;
        }
    }
}
