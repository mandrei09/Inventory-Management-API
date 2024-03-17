using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;
using OfficeOpenXml.Style;
using System.Drawing;
using Optima.Fais.Dto;
using Optima.Fais.Model.Utils;
using System.Collections;
using Optima.Fais.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharp.Pdf.Filters;
using PdfSharp;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace Optima.Fais.Api.Controllers
{
	[Authorize]
    [Route("api/dashboards")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class DashboardsController : GenericApiController<Model.Dashboard, Dto.Dashboard>
    {
        public DashboardsController(ApplicationDbContext context, IDashboardsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("inventorystatus/{administrationId}/{inventoryId}/{departmentId}/{divisionId}/{costCenterId}")]
        public virtual async Task<IActionResult> GetInventoryData(int administrationId, int inventoryId, int departmentId, int divisionId, int costCenterId)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string userName = HttpContext.User.Identity.Name;
                string role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                string empId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                Model.Inventory inventory = await _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId && a.IsDeleted == false).FirstOrDefaultAsync();

                //List<Model.InventoryTotal> items = _context.Set<Model.InventoryTotal>().FromSql("InventoryTotal {0}, {1}, {2}, {3}, {4}, {5}, {6}", inventoryId, role, employeeId, departmentId, divisionId, costCenterId, typeId).ToList();

                List<Model.InventoryTotal> items = _context.Set<Model.InventoryTotal>().FromSql("GET_InventoryStatusDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                                 administrationId, inventoryId, inventory.AccMonthId.Value, null, role, costCenterId, departmentId, divisionId).ToList();


                return Ok(items);
			}
			else
			{
                List<Model.InventoryTotal> items = new List<Model.InventoryTotal>();

                items.Add(new Model.InventoryTotal()
                {
                    Id = 0,
                    CurrentAPC = 0,
                    CurrBkValue = 0,
                    AccumulDep = 0,
                    Initial = 0,
                    Scanned = 0,
                    Procentage = 0
                });

                return Ok(items);
			}
        }

        [HttpGet]
        [Route("costcenterstatus/{administrationId}/{inventoryId}/{departmentId}/{divisionId}/{costCenterId}")]
        public virtual async Task<IActionResult> GetCostCenterData(int administrationId, int inventoryId, int departmentId, int divisionId, int costCenterId)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string userName = HttpContext.User.Identity.Name;
                string role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                string empId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				Model.Inventory inventory = await _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId && a.IsDeleted == false).FirstOrDefaultAsync();

				List<Model.CostCenterTotal> items = _context.Set<Model.CostCenterTotal>().FromSql("GET_CostCenterDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
				 administrationId, inventoryId, inventory.AccMonthId.Value, null, role, costCenterId, departmentId, divisionId).ToList();

                return Ok(items);
            }
            else
            {
                List<Model.CostCenterTotal> items = new List<Model.CostCenterTotal>();
                items.Add(new Model.CostCenterTotal()
                {
                    Id = 0,
                    ValueInv = 0,
                    ValueRem = 0,
                    Initial = 0,
                    Scanned = 0,
                    Procentage = 0
                });

                return Ok(items);
            }
        }

		[HttpGet]
		[Route("administrationstatus/{administrationId}/{inventoryId}/{departmentId}/{divisionId}/{costCenterId}")]
		public virtual async Task<IActionResult> GetAdministrationData(int administrationId, int inventoryId, int departmentId, int divisionId, int costCenterId)
		{
			if (HttpContext.User.Identity.Name != null)
			{
				string userName = HttpContext.User.Identity.Name;
				string role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				string empId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				Model.Inventory inventory = await _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId && a.IsDeleted == false).FirstOrDefaultAsync();

				List<Model.AdministrationTotal> items = _context.Set<Model.AdministrationTotal>().FromSql("GET_AdministrationDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
				 administrationId, inventoryId, inventory.AccMonthId.Value, null, role, costCenterId, departmentId, divisionId).ToList();

                /*
                //int sumInitialIT = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.InitialIT);
				//int sumScannedIT = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.ScannedIT);
				//int sumNotScannedIT = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.NotScannedIT);
				//decimal sumInitialITCurrentAPC = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.InitialITCurrentAPC);
				//decimal sumInitialITCurrBk = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.InitialITCurrBkValue);
				//decimal sumInitialITAccumulDep = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.InitialITAccumulDep);

				//decimal sumScannedITCurrentAPC = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.ScannedITCurrentAPC);
				//decimal sumScannedITCurrBk = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.ScannedITCurrBkValue);
				//decimal sumScannedITAccumulDep = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.ScannedITAccumulDep);

				//decimal sumNotScannedITCurrentAPC = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.NotScannedITCurrentAPC);
				//decimal sumNotScannedITCurrBk = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.NotScannedITCurrBkValue);
				//decimal sumNotScannedITAccumulDep = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.NotScannedITAccumulDep);

				//int sumInitialFacility = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.InitialFacility);
				//int sumScannedFacility = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.ScannedFacility);
				//int sumNotScannedFacility = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.NotScannedFacility);
				//decimal sumInitialFacilityCurrentAPC = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.InitialFacilityCurrentAPC);
				//decimal sumInitialFacilityCurrBk = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.InitialFacilityCurrBkValue);
				//decimal sumInitialFacilityAccumulDep = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.InitialFacilityAccumulDep);

				//decimal sumScannedFacilityCurrentAPC = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.ScannedFacilityCurrentAPC);
				//decimal sumScannedFacilityCurrBk = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.ScannedFacilityCurrBkValue);
				//decimal sumScannedFacilityAccumulDep = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.ScannedFacilityAccumulDep);

				//decimal sumNotScannedFacilityCurrentAPC = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.NotScannedFacilityCurrentAPC);
				//decimal sumNotScannedFacilityCurrBk = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.NotScannedFacilityCurrBkValue);
				//decimal sumNotScannedFacilityAccumulDep = items.Where(a => a.Code == "IT" || a.Code == "Facility").Sum(a => a.NotScannedFacilityAccumulDep);
                */

                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Code == "WFH")
					{
						int initial = _context.Set<Model.Asset>().Where(a => a.InvNo.StartsWith("WFH2023")).ToList().Count;
						int scanned = _context.Set<Model.Asset>().Where(a => a.IsDeleted == true && a.InvNo.StartsWith("WFH2023")).ToList().Count;
						int notScanned = _context.Set<Model.Asset>().Where(a => a.IsDeleted == false && a.InvNo.StartsWith("WFH2023")).ToList().Count;
						decimal sumScannedCurrentAPC = _context.Set<Model.InventoryAsset>().Include(a => a.Asset).Where(a => a.InventoryId == inventoryId && a.Asset.IsDeleted == false && a.Asset.Validated == true && a.Asset.IsPlus == true && a.TempReco.StartsWith("WFH2023")).AsNoTracking().Sum(a => a.CurrentAPC);
						decimal sumScannedCurrBk = _context.Set<Model.InventoryAsset>().Include(a => a.Asset).Where(a => a.InventoryId == inventoryId && a.Asset.IsDeleted == false && a.Asset.Validated == true && a.Asset.IsPlus == true && a.TempReco.StartsWith("WFH2023")).AsNoTracking().Sum(a => a.CurrBkValue);
						decimal sumScannedCAccumulDep = _context.Set<Model.InventoryAsset>().Include(a => a.Asset).Where(a => a.InventoryId == inventoryId && a.Asset.IsDeleted == false && a.Asset.Validated == true && a.Asset.IsPlus == true && a.TempReco.StartsWith("WFH2023")).AsNoTracking().Sum(a => Math.Abs(a.AccumulDep));
						items[i].Initial = initial;
						items[i].Scanned = scanned;
						items[i].NotScanned = notScanned;

						//items[i].InitialCurrentAPC = sumInitialFacilityCurrentAPC;
						//items[i].InitialCurrBkValue = sumInitialFacilityCurrBk;
						//items[i].InitialAccumulDep = sumInitialFacilityAccumulDep;

						items[i].ScannedCurrentAPC = sumScannedCurrentAPC;
						items[i].ScannedCurrBkValue = sumScannedCurrBk;
						items[i].ScannedAccumulDep = sumScannedCAccumulDep;

						//items[i].NotScannedCurrentAPC = sumNotScannedFacilityCurrentAPC;
						//items[i].NotScannedCurrBkValue = sumNotScannedFacilityCurrBk;
						//items[i].NotScannedAccumulDep = sumNotScannedFacilityAccumulDep;
					}

				}

                return Ok(items);
			}
			else
			{
				List<Model.AdministrationTotal> items = new List<Model.AdministrationTotal>();
				items.Add(new Model.AdministrationTotal()
				{
					Id = 0,
					Code = "",
                    Name = "",
					Initial = 0,
                    //InitialIT = 0,
                    //InitialFacility = 0,
                    InitialCurrentAPC = 0,
                    InitialCurrBkValue = 0,
                    InitialAccumulDep= 0,
                    //InitialITCurrentAPC = 0,
                    //InitialITCurrBkValue = 0,
                    //InitialITAccumulDep = 0,
                    //InitialFacilityCurrentAPC= 0,
                    //InitialFacilityCurrBkValue = 0,
                    //InitialFacilityAccumulDep = 0,
					Scanned = 0,
					//ScannedIT = 0,
					//ScannedFacility = 0,
					ScannedCurrentAPC = 0,
					ScannedCurrBkValue = 0,
					ScannedAccumulDep = 0,
					//ScannedITCurrentAPC = 0,
					//ScannedITCurrBkValue = 0,
					//ScannedITAccumulDep = 0,
					//ScannedFacilityCurrentAPC = 0,
					//ScannedFacilityCurrBkValue = 0,
					//ScannedFacilityAccumulDep = 0,
					NotScanned = 0,
					//NotScannedIT = 0,
					//NotScannedFacility = 0,
					NotScannedCurrentAPC = 0,
					NotScannedCurrBkValue = 0,
					NotScannedAccumulDep = 0,
					//NotScannedITCurrentAPC = 0,
					//NotScannedITCurrBkValue = 0,
					//NotScannedITAccumulDep = 0,
					//NotScannedFacilityCurrentAPC = 0,
					//NotScannedFacilityCurrBkValue = 0,
					//NotScannedFacilityAccumulDep = 0,
                    Temp = 0,
					Procentage = 0
				});

				return Ok(items);
			}
		}

        [HttpGet]
        [Route("departmentstatus/{administrationId}/{inventoryId}/{departmentId}/{divisionId}/{costCenterId}")]
        public virtual async Task<IActionResult> GetDepartmentData(int administrationId, int inventoryId, int departmentId, int divisionId, int costCenterId)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string userName = HttpContext.User.Identity.Name;
                string role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                string empId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				Model.Inventory inventory = await _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId && a.IsDeleted == false).FirstOrDefaultAsync();

				List<Model.DepartmentTotal> items = _context.Set<Model.DepartmentTotal>().FromSql("GET_DepartmentDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
					administrationId, inventoryId, inventory.AccMonthId.Value, null, role, costCenterId, departmentId, divisionId).ToList();

                return Ok(items);
            }
            else
            {
                List<Model.DepartmentTotal> items = new List<Model.DepartmentTotal>();
                items.Add(new Model.DepartmentTotal()
                {
                    Id = 0,
                    Initial = 0,
                    Scanned = 0,
                    Procentage = 0,
                    ValueInv = 0,
                    ValueRem = 0,
                    Code = "",
                    Name = "",
                });

                return Ok(items);
            }
        }

        [HttpGet]
        [Route("divisionstatus/{administrationId}/{inventoryId}/{departmentId}/{divisionId}/{costCenterId}")]
        public virtual async Task<IActionResult> GetDivisionData(int administrationId, int inventoryId, int departmentId, int divisionId, int costCenterId)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                string userName = HttpContext.User.Identity.Name;
                string role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                string empId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				Model.Inventory inventory = await _context.Set<Model.Inventory>().Where(a => a.Id == inventoryId && a.IsDeleted == false).FirstOrDefaultAsync();

				List<Model.DivisionTotal> items = _context.Set<Model.DivisionTotal>().FromSql("GET_DivisionDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
				   administrationId, inventoryId, inventory.AccMonthId.Value, null, role, costCenterId, departmentId, divisionId).ToList();

                return Ok(items);
            }
            else
            {
                List<Model.DivisionTotal> items = new List<Model.DivisionTotal>();
                items.Add(new Model.DivisionTotal()
                {
                    Id = 0,
                    Initial = 0,
                    Scanned = 0,
                    Procentage = 0,
                    ValueInv =0,
                    ValueRem = 0
                });

                return Ok(items);
            }
        }

        [HttpGet]
        [Route("companydynamicgroup/{accMonthId}/{companyId}/{countryId}/{projectId}/{admCenterId}/{regionId}/{assetTypeId}/{activityId}")]
        public virtual IActionResult GetCompanyDynamicGroupDetails(int accMonthId, int companyId, int countryId, int projectId, int admCenterId, int regionId, int assetTypeId, int activityId)
        {
            List<Model.CompanyDynamicGroup> items = _context.Set<Model.CompanyDynamicGroup>().FromSql("GET_CompanyGroupDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", accMonthId, companyId, countryId, projectId, admCenterId, regionId, assetTypeId, activityId).ToList();

            if (items.Count == 0)
            {
				var item = new Model.CompanyDynamicGroup
				{
					Code = "-",
					Name = "-",
					Fy2019Actual = 0,
					Fy2020Actual = 0,
                    Fy2021Actual = 0,
                    Fy2022BudgetBase = 0,
                    Fy2022BudgetDynamic = 0,
                    VarView12MonthActual = 0,
                    VarView12MonthBudget = 0 ,
                    ViewYTDBudgetBase = 0,
                    ViewYTDBudgetDynamic= 0,
                    ViewYTDActual= 0,
                    VarYTDActual= 0,
                    VarYTDBudget = 0,
                    ViewPerMonthBudgetBase = 0 ,
                    ViewPerMonthBudgetDynamic = 0,
                    ViewPerMonthActual = 0,
                    VarPerMonthActual = 0,
                    VarPerMonthBudget = 0,
                    Parent = 0,
                    Table = "Company"
				};

				items.Add(item);
            }

            return Ok(items);
        }


        [HttpGet]
        [Route("departmentdynamicgroup/{accMonthId}/{companyId}/{countryId}/{projectId}/{admCenterId}/{regionId}/{assetTypeId}/{activityId}/{parentId}")]
        public virtual IActionResult GetDepartmentDynamicGroupDetails(int accMonthId, int companyId, int countryId, int projectId, int admCenterId, int regionId, int assetTypeId, int activityId, int parentId)
        {
            List<Model.DepartmentDynamicGroup> items = _context.Set<Model.DepartmentDynamicGroup>().FromSql("GET_DepartmentGroupDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", accMonthId, companyId, countryId, projectId, admCenterId, regionId, assetTypeId, activityId, parentId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.DepartmentDynamicGroup
                {
                    Code = "-",
                    Name = "-",
                    Fy2019Actual = 0,
                    Fy2020Actual = 0,
                    Fy2021Actual = 0,
                    Fy2022BudgetBase = 0,
                    Fy2022BudgetDynamic = 0,
                    VarView12MonthActual = 0,
                    VarView12MonthBudget = 0,
                    ViewYTDBudgetBase = 0,
                    ViewYTDBudgetDynamic = 0,
                    ViewYTDActual = 0,
                    VarYTDActual = 0,
                    VarYTDBudget = 0,
                    ViewPerMonthBudgetBase = 0,
                    ViewPerMonthBudgetDynamic = 0,
                    ViewPerMonthActual = 0,
                    VarPerMonthActual = 0,
                    VarPerMonthBudget = 0,
                    Parent = parentId,
                    Table = "Department"
                };

                items.Add(item);
            }

            return Ok(items);
        }

        [HttpGet]
        [Route("divisiondynamicgroup/{accMonthId}/{companyId}/{countryId}/{projectId}/{admCenterId}/{regionId}/{assetTypeId}/{activityId}/{parentId}")]
        public virtual IActionResult GetDiviisonDynamicGroupDetails(int accMonthId, int companyId, int countryId, int projectId, int admCenterId, int regionId, int assetTypeId, int activityId, int parentId)
        {
            List<Model.DivisionDynamicGroup> items = _context.Set<Model.DivisionDynamicGroup>().FromSql("GET_DivisionGroupDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", accMonthId, companyId, countryId, projectId, admCenterId, regionId, assetTypeId, activityId, parentId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.DivisionDynamicGroup
                {
                    Code = "",
                    Name = "",
                    Division = "-",
                    Fy2019Actual = 0,
                    Fy2020Actual = 0,
                    Fy2021Actual = 0,
                    Fy2022BudgetBase = 0,
                    Fy2022BudgetDynamic = 0,
                    VarView12MonthActual = 0,
                    VarView12MonthBudget = 0,
                    ViewYTDBudgetBase = 0,
                    ViewYTDBudgetDynamic = 0,
                    ViewYTDActual = 0,
                    VarYTDActual = 0,
                    VarYTDBudget = 0,
                    ViewPerMonthBudgetBase = 0,
                    ViewPerMonthBudgetDynamic = 0,
                    ViewPerMonthActual = 0,
                    VarPerMonthActual = 0,
                    VarPerMonthBudget = 0,
                    Parent = 0,
                    Table = "Division"
                };

                items.Add(item);
            }

            return Ok(items);
        }


        [HttpGet]
        [Route("regiondynamicgroup/{accMonthId}/{companyId}/{countryId}/{departmentId}/{divisionId}/{projectId}/{admCenterId}/{regionId}/{assetTypeId}/{activityId}/{parentId}")]
        public virtual IActionResult GetRegionDynamicGroupDetails(int accMonthId, int companyId, int countryId, int departmentId, int divisionId, int projectId, int admCenterId, int regionId, int assetTypeId, int activityId, int parentId)
        {
            List<Model.RegionDynamicGroup> items = _context.Set<Model.RegionDynamicGroup>().FromSql("GET_RegionGroupDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}", accMonthId, companyId, countryId, departmentId, divisionId, projectId, admCenterId, regionId, assetTypeId, activityId, parentId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.RegionDynamicGroup
                {
                    Code = "",
                    Name = "",
                    Region = "-",
                    Fy2019Actual = 0,
                    Fy2020Actual = 0,
                    Fy2021Actual = 0,
                    Fy2022BudgetBase = 0,
                    Fy2022BudgetDynamic = 0,
                    VarView12MonthActual = 0,
                    VarView12MonthBudget = 0,
                    ViewYTDBudgetBase = 0,
                    ViewYTDBudgetDynamic = 0,
                    ViewYTDActual = 0,
                    VarYTDActual = 0,
                    VarYTDBudget = 0,
                    ViewPerMonthBudgetBase = 0,
                    ViewPerMonthBudgetDynamic = 0,
                    ViewPerMonthActual = 0,
                    VarPerMonthActual = 0,
                    VarPerMonthBudget = 0,
                    Parent = 0,
                    Table = "Region"
                };

                items.Add(item);
            }

            return Ok(items);
        }

        [HttpGet]
        [Route("admcenterdynamicgroup/{accMonthId}/{companyId}/{countryId}/{departmentId}/{divisionId}/{projectId}/{admCenterId}/{regionId}/{assetTypeId}/{activityId}/{parentId}")]
        public virtual IActionResult GetAdmCenterDynamicGroupDetails(int accMonthId, int companyId, int countryId, int departmentId, int divisionId, int projectId, int admCenterId, int regionId, int assetTypeId, int activityId, int parentId)
        {
            List<Model.AdmCenterDynamicGroup> items = _context.Set<Model.AdmCenterDynamicGroup>().FromSql("GET_AdmCenterGroupDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}", accMonthId, companyId, countryId, departmentId, divisionId, projectId, admCenterId, regionId, assetTypeId, activityId, parentId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.AdmCenterDynamicGroup
                {
                    Code = "",
                    Name = "",
                    AdmCenter = "-",
                    Fy2019Actual = 0,
                    Fy2020Actual = 0,
                    Fy2021Actual = 0,
                    Fy2022BudgetBase = 0,
                    Fy2022BudgetDynamic = 0,
                    VarView12MonthActual = 0,
                    VarView12MonthBudget = 0,
                    ViewYTDBudgetBase = 0,
                    ViewYTDBudgetDynamic = 0,
                    ViewYTDActual = 0,
                    VarYTDActual = 0,
                    VarYTDBudget = 0,
                    ViewPerMonthBudgetBase = 0,
                    ViewPerMonthBudgetDynamic = 0,
                    ViewPerMonthActual = 0,
                    VarPerMonthActual = 0,
                    VarPerMonthBudget = 0,
                    Parent = 0,
                    Table = "AdmCenter"
                };

                items.Add(item);
            }

            return Ok(items);
        }

        [HttpGet]
        [Route("assettypedynamicgroup/{accMonthId}/{companyId}/{countryId}/{departmentId}/{divisionId}/{projectId}/{admCenterId}/{regionId}/{assetTypeId}/{activityId}/{parentId}")]
        public virtual IActionResult GetAssetTypeDynamicGroupDetails(int accMonthId, int companyId, int countryId, int departmentId, int divisionId, int projectId, int admCenterId, int regionId, int assetTypeId, int activityId, int parentId)
        {
            List<Model.AssetTypeDynamicGroup> items = _context.Set<Model.AssetTypeDynamicGroup>().FromSql("GET_AssetTypeGroupDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}", accMonthId, companyId, countryId, departmentId, divisionId, projectId, admCenterId, regionId, assetTypeId, activityId, parentId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.AssetTypeDynamicGroup
                {
                    Code = "",
                    Name = "",
                    AssetType = "-",
                    Fy2019Actual = 0,
                    Fy2020Actual = 0,
                    Fy2021Actual = 0,
                    Fy2022BudgetBase = 0,
                    Fy2022BudgetDynamic = 0,
                    VarView12MonthActual = 0,
                    VarView12MonthBudget = 0,
                    ViewYTDBudgetBase = 0,
                    ViewYTDBudgetDynamic = 0,
                    ViewYTDActual = 0,
                    VarYTDActual = 0,
                    VarYTDBudget = 0,
                    ViewPerMonthBudgetBase = 0,
                    ViewPerMonthBudgetDynamic = 0,
                    ViewPerMonthActual = 0,
                    VarPerMonthActual = 0,
                    VarPerMonthBudget = 0,
                    Parent = 0,
                    Table = "AssetType"
                };

                items.Add(item);
            }

            return Ok(items);
        }


        [HttpGet]
        [Route("projectdynamicgroup/{accMonthId}/{companyId}/{countryId}/{projectId}/{admCenterId}/{regionId}/{assetTypeId}/{activityId}/{parentId}")]
        public virtual IActionResult GetProjectDynamicGroupDetails(int accMonthId, int companyId, int countryId, int projectId, int admCenterId, int regionId, int assetTypeId, int activityId, int parentId)
        {
            List<Model.ProjectDynamicGroup> items = _context.Set<Model.ProjectDynamicGroup>().FromSql("GET_ProjectGroupDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", accMonthId, companyId, countryId, projectId, admCenterId, regionId, assetTypeId, activityId, parentId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.ProjectDynamicGroup
                {
                    Code = "",
                    Name = "",
                    Project = "-",
                    Fy2019Act = 0,
                    Fy2020Act = 0,
                    Fy2021Act = 0,
                    Fy2022Budget = 0,
                    Fy2022BudgetRem = 0,
                    Parent = 0,
                    Table = "Project"
                };

                items.Add(item);
            }

            return Ok(items);
        }


        [HttpGet]
        [Route("projecttypedynamicgroup/{accMonthId}/{companyId}/{countryId}/{departmentId}/{divisionId}/{projectId}/{admCenterId}/{regionId}/{assetTypeId}/{activityId}/{parentId}")]
        public virtual IActionResult GetProjectTypeDynamicGroupDetails(int accMonthId, int companyId, int countryId, int departmentId, int divisionId, int projectId, int admCenterId, int regionId, int assetTypeId, int activityId, int parentId)
        {
            List<Model.ProjectTypeDynamicGroup> items = _context.Set<Model.ProjectTypeDynamicGroup>().FromSql("GET_ProjectTypeGroupDynamicSearch {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}", accMonthId, companyId, countryId, departmentId, divisionId, projectId, admCenterId, regionId, assetTypeId, activityId, parentId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.ProjectTypeDynamicGroup
                {
                    Code = "",
                    Name = "",
                    ProjectType = "-",
                    Fy2019Actual = 0,
                    Fy2020Actual = 0,
                    Fy2021Actual = 0,
                    Fy2022BudgetBase = 0,
                    Fy2022BudgetDynamic = 0,
                    VarView12MonthActual = 0,
                    VarView12MonthBudget = 0,
                    ViewYTDBudgetBase = 0,
                    ViewYTDBudgetDynamic = 0,
                    ViewYTDActual = 0,
                    VarYTDActual = 0,
                    VarYTDBudget = 0,
                    ViewPerMonthBudgetBase = 0,
                    ViewPerMonthBudgetDynamic = 0,
                    ViewPerMonthActual = 0,
                    VarPerMonthActual = 0,
                    VarPerMonthBudget = 0,
                    Parent = 0,
                    Table = "ProjectType"
                };

                items.Add(item);
            }

            return Ok(items);
        }

        [HttpGet]
        [Route("dynamicgroupmonth/{accMonthId}/{companyId}/{departmentId}/{divisionId}/{countryId}/{projectId}/{admCenterId}/{regionId}/{assetTypeId}/{activityId}/{projectTypeId}")]
        public virtual IActionResult GetCompanyDynamicGroupMonthDetails(int accMonthId, int companyId, int departmentId, int divisionId, int countryId, int projectId, int admCenterId, int regionId, int assetTypeId, int activityId, int projectTypeId)
        {
            List<Model.CompanyDynamicGroupMonth> items = _context.Set<Model.CompanyDynamicGroupMonth>().FromSql("GET_CompanyGroupDynamicSearchValues {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}", accMonthId, companyId, departmentId, divisionId, countryId, projectId, admCenterId, regionId, assetTypeId, activityId, projectTypeId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.CompanyDynamicGroupMonth
                {
                    Code = "-",
                    Name = "-",
                    Value = 0,
                    ValueDep = 0,
                    Month = "-"
                };

                items.Add(item);
            }

            return Ok(items);
        }

		//[HttpGet]
		//[Route("requestListStatus")]
		//public virtual IActionResult GetListRequests(string jsonFilter)
		//{
		//	DashboardFilter assetFilter = null;
		//	assetFilter = GetAssetFilter(jsonFilter);

		//	var items = (_itemsRepository as IDashboardsRepository)
		//		.GetListRequests(assetFilter).ToList();
		//	//var itemsResource = _mapper.Map<List<Model.RequestKanban>, List<m.RequestKanban>>(items);

		//	return Ok(items);
		//}

	}
}