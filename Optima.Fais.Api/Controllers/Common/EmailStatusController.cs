using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto.Common;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/emailstatus")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class EmailStatusController : GenericApiController<Model.EmailStatus, Dto.EmailStatus>
    {
		private readonly UserManager<ApplicationUser> _userManager;

		public EmailStatusController(ApplicationDbContext context, IEmailStatusRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
			this._userManager = userManager;
		}

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string emailTypeIds, string appStateIds, string assetCategoryIds, string includes)
        {
            List<Model.EmailStatus> items = null;
            IEnumerable<Dto.EmailStatus> itemsResult = null;
            List<int?> dIds = null;
            List<int?> aIds = null;
            List<int?> appIds = null;
            includes = "EmailType,EmployeeInitial,EmployeeFinal,CostCenterInitial,CostCenterFinal,Asset,AppState";

            if ((emailTypeIds != null) && (emailTypeIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(emailTypeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((appStateIds != null) && (appStateIds.Length > 0)) appIds = JsonConvert.DeserializeObject<string[]>(appStateIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((assetCategoryIds != null) && (assetCategoryIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(assetCategoryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IEmailStatusRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, dIds, appIds, aIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.EmailStatus>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IEmailStatusRepository).GetCountByFilters(filter, dIds, appIds, aIds);
                var pagedResult = new Dto.PagedResult<Dto.EmailStatus>(itemsResult, new Dto.PagingInfo()
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
		[Route("dstemployeevalidate/{guidEmp}/{guid}")]
		public async virtual Task<IActionResult> ValidateDstEmployee(Guid guidEmp, Guid guid)
		{
			Model.AppState appState = null;
			Model.AppState newAppState = null;
			//Model.AppState sameAppState = null;
			Model.Employee employee = null;
			Model.ApplicationUser user = null;
			Model.AssetChangeSAP assetChangeSAP = null;
			Model.AccMonth accMonth = null;
			Model.BudgetManager budgetManager = null;
			Model.AssetAdmMD assetAdmMD = null;
			bool success = false;

			if (guid != Guid.Empty)
			{
				appState = _context.Set<Model.AppState>().Where(a => a.Code == "PROGRESS").SingleOrDefault();
				newAppState = _context.Set<Model.AppState>().Where(a => a.Code == "REGISTERED").SingleOrDefault();
				//sameAppState = _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_VALIDATE").SingleOrDefault();
				employee = _context.Set<Model.Employee>().Where(a => a.Guid == guidEmp).Single();
				user = await _userManager.FindByEmailAsync(employee.Email);
				accMonth = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).SingleOrDefault();
				budgetManager = _context.Set<Model.BudgetManager>().Where(a => a.Code == "2023").SingleOrDefault();
				var date = DateTime.Now.ToString("yyyyMMdd");

				List<Model.EmailStatus> emailStatuses = await _context.Set<Model.EmailStatus>()
					.Include(e => e.EmailType)
					.Include(c => c.CostCenterFinal)
					.Include(c => c.EmployeeFinal).ThenInclude(c=> c.CostCenter)
					.Where(a => a.Guid == guid && a.IsDeleted == false && a.AppStateId == appState.Id).ToListAsync();

				for (int i = 0; i < emailStatuses.Count; i++)
				{
					if (emailStatuses[i] != null && emailStatuses[i].EmailType.Code == "TRANSFER")
					{
						Model.Asset asset = _context.Set<Model.Asset>()
							.Include(c => c.Company)
							.Include(a => a.AssetCategory)
							.Include(a => a.ExpAccount)
							.Include(a => a.Document).ThenInclude(p => p.Partner)
							.Where(a => a.Id == emailStatuses[i].AssetId).SingleOrDefault();

						assetAdmMD = _context.Set<Model.AssetAdmMD>().Where(a => (a.AccMonthId == accMonth.Id || a.AccMonthId == 36) && a.AssetId == asset.Id).SingleOrDefault();


						Model.AssetOp assetOp = _context.Set<Model.AssetOp>().Where(a => a.Id == emailStatuses[i].AssetOpId).SingleOrDefault();



						var names = SplitToLines(asset.Name, 50);
						var countNames = names.Count();

						asset.AppStateId = newAppState.Id;
						asset.EmployeeId = emailStatuses[i].EmployeeIdFinal;
						asset.CostCenterId = emailStatuses[i].CostCenterIdFinal;

						Model.CostCenter costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == asset.CostCenterId).SingleOrDefault();

						asset.DepartmentId = costCenter.Division.DepartmentId;
						asset.DivisionId = costCenter.DivisionId;

                        assetAdmMD.EmployeeId = emailStatuses[i].EmployeeIdFinal;
						assetAdmMD.CostCenterId = emailStatuses[i].CostCenterIdFinal;
                        assetAdmMD.DepartmentId = costCenter.Division.DepartmentId;
                        assetAdmMD.DivisionId = costCenter.DivisionId;
                        assetOp.AssetOpStateId = newAppState.Id;
						emailStatuses[i].AppStateId = newAppState.Id;
						emailStatuses[i].DstEmployeeValidateAt = DateTime.Now;
						emailStatuses[i].DstEmployeeValidateBy = user?.Id;
						emailStatuses[i].NotSync = true;
						emailStatuses[i].SyncErrorCount = 0;

						assetChangeSAP = new AssetChangeSAP()
						{
							COMPANYCODE = asset.Company.Code,
							ASSET = asset.InvNo,
							SUBNUMBER = asset.SubNo,
							ASSETCLASS = asset.ExpAccount.Name,
							POSTCAP = "",
							DESCRIPT = countNames > 0 ? names.ElementAt(0) : "",
							DESCRIPT2 = countNames > 1 ? names.ElementAt(1) : "",
							INVENT_NO = asset.ERPCode,
							SERIAL_NO = asset.SerialNumber,
							QUANTITY = (int)asset.Quantity,
							BASE_UOM = "ST",
							LAST_INVENTORY_DATE = "00000000",
							LAST_INVENTORY_DOCNO = "",
							CAP_DATE = "00000000",
							COSTCENTER = emailStatuses[i].EmployeeFinal != null && emailStatuses[i].EmployeeFinal.CostCenter != null ? emailStatuses[i].EmployeeFinal.CostCenter.Code : emailStatuses[i].CostCenterFinal.Code,
							RESP_CCTR = emailStatuses[i].CostCenterFinal.Code,
							INTERN_ORD = "",
							PLANT = "RO02",
							LOCATION = "",
							ROOM = "",
							PERSON_NO = emailStatuses[i].EmployeeFinal.InternalCode,
							PLATE_NO = asset.AgreementNo != null ? asset.AgreementNo : "",
							ZZCLAS = asset.AssetCategory.Code,
							IN_CONSERVATION = "",
							PROP_IND = "1",
							OPTIMA_ASSET_NO = "",
							OPTIMA_ASSET_PARENT_NO = "",
							VENDOR_NO = asset.Document.Partner.RegistryNumber,
							FROM_DATE = date,
							AccMonthId = accMonth.Id,
							AssetId = asset.Id,
							BudgetManagerId = budgetManager.Id,
							NotSync = true,
							CreatedAt = DateTime.Now,
							CreatedBy = user?.Id,
							ModifiedAt= DateTime.Now,
							ModifiedBy = user?.Id,
						};


						_context.Add(assetChangeSAP);

						_context.Update(asset);
						_context.Update(assetAdmMD);
						_context.Update(assetOp);
						_context.Update(emailStatuses[i]);
						_context.SaveChanges();
						success = true;
					}
				}

				if (emailStatuses.Count == 0)
				{
					 //return Redirect("http://localhost:4200//#/alreadyvalidate");
					return Redirect("https://optima.emag.network/ofa/#/alreadyvalidate");
				}
			}

			if (success)
			{
				 //return Redirect("http://localhost:4200//#/dstemployeevalidate");
				return Redirect("https://optima.emag.network/ofa/#/dstemployeevalidate");
			}
			else
			{
				 //return Redirect("http://localhost:4200//#/error");
				return Redirect("https://optima.emag.network/ofa/#/error");
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
