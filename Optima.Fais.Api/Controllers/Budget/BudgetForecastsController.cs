using AutoMapper;
using MailKit.Search;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MimeKit;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using Optima.Fais.Api.Helpers;
using Optima.Fais.Api.Services;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.EfRepository;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/budgetforecasts")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class BudgetForecastsController : GenericApiController<Model.BudgetForecast, Dto.BudgetForecast>
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<Model.ApplicationUser> userManager;

        private readonly IEmailSender _emailSender;
		private readonly IRequestsService _requestsService;

		private readonly IMatrixRepository _matrixRepository;

		public BudgetForecastsController(ApplicationDbContext context,
			IBudgetForecastsRepository itemsRepository, 
            //IRepository<Model.Budget> itemsRepository,
            IMapper mapper, IWebHostEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager, IEmailSender emailSender, IRequestsService requestsService, IMatrixRepository matrixRepository)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            this._emailSender = emailSender;
			this._requestsService = requestsService;
			this._matrixRepository = matrixRepository;
		}

		[HttpGet]
		[Route("", Order = -1)]
		public virtual async Task<IActionResult> GetDepDetails(int page, int pageSize, string sortColumn, string sortDirection,
			string includes, string jsonFilter, string filter)
		{
			ForecastTotal depTotal = null;
			BudgetFilter budgetFilter = null;
			Paging paging = null;
			Sorting sorting = null;
			string userName = string.Empty;
			string role = string.Empty;
			string employeeId = string.Empty;

			// includes ??= "BudgetBase.AssetType,BudgetBase.ProjectType,BudgetBase.Region,BudgetBase.AdmCenter,BudgetBase.Activity,BudgetBase.Country,BudgetBase.Company,BudgetBase.Employee,BudgetBase.AccMonth,BudgetBase.AppState,";
			//includes = "Company,Project,Administration,CostCenter,SubType.Type.MasterType,SubType.Type,SubType,Employee,AccMonth,InterCompany,Partner,Account,AppState";

			if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
				|| (page <= 0) || (pageSize <= 0))
				return BadRequest();

			paging = new Paging() { Page = page, PageSize = pageSize };
			sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
			budgetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();

			if (HttpContext.User.Identity.Name != null)
			{
				var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

				if (user == null)
				{
					user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                }
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				budgetFilter.Role = role;
				budgetFilter.InInventory = user.InInventory;
				budgetFilter.UserId = user.Id;

				if (employeeId != null)
				{
					budgetFilter.EmployeeId = int.Parse(employeeId);
				}
				else
				{
					budgetFilter.EmployeeIds = null;
					budgetFilter.EmployeeIds = new List<int?>();
					budgetFilter.EmployeeIds.Add(int.Parse("-1"));
				}
			}
			else
			{
				budgetFilter.EmployeeIds = null;
				budgetFilter.EmployeeIds = new List<int?>();
				budgetFilter.EmployeeIds.Add(int.Parse("-1"));
			}


			includes = includes + "BudgetForecast.BudgetBase.Activity,BudgetForecast.BudgetBase.Country,BudgetForecast.BudgetBase.Division.Department,BudgetForecast.BudgetBase.AssetType,BudgetForecast.BudgetBase.AdmCenter,BudgetForecast.BudgetBase.Region,BudgetForecast.BudgetBase.ProjectType,BudgetForecast.BudgetBase.Project,BudgetForecast.BudgetBase.Employee,BudgetForecast.BudgetBase.AppState,BudgetForecast.AccMonth";

            var items = (_itemsRepository as IBudgetForecastsRepository)
               .GetBuget(budgetFilter, filter, includes, sorting, paging, out depTotal).ToList();

			var itemsResource = _mapper.Map<List<Model.BudgetForecastDetail>, List<Dto.BudgetForecast>>(items);

			var result = new BudgetForecastPagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal);

			return Ok(result);
		}

		[HttpGet]
		[Route("detailui", Order = -1)]
		public virtual async Task<IActionResult> GetDepDetailUIs(int page, int pageSize, string sortColumn, string sortDirection,
			string includes, string jsonFilter, string filter)
		{
			ForecastTotal depTotal = null;
			BudgetFilter assetFilter = null;
			Paging paging = null;
			Sorting sorting = null;

			sortColumn = "BudgetForecast.BudgetBase.Code";

			// var count = _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgets").ToList();

			includes = includes + ",BudgetForecast.BudgetBase.Company,BudgetForecast.BudgetBase.Project.ProjectType,BudgetForecast.BudgetBase.AdmCenter,BudgetForecast.BudgetBase.CostCenter,BudgetForecast.BudgetBase.Region,BudgetForecast.BudgetBase.AssetType,BudgetForecast.BudgetBase.Division.Department,BudgetForecast.BudgetBase.AppState,BudgetForecast.BudgetBase.Uom,BudgetForecast.BudgetBase.BudgetMonthBase,BudgetForecast.AccMonth";

			if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
				|| (page <= 0) || (pageSize <= 0))
				return BadRequest();

			paging = new Paging() { Page = page, PageSize = pageSize };
			sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();


			if (filter != null)
			{
				assetFilter.Filter = filter;
			}

			var items = (_itemsRepository as IBudgetForecastsRepository)
				.GetBugetUI(assetFilter, includes, sorting, paging, out depTotal).ToList();
			var itemsResource = _mapper.Map<List<Model.BudgetForecastDetail>, List<Dto.BudgetForecastUI>>(items);

			var result = new BudgetForecastUIPagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal);

			return Ok(result);
		}

		[HttpGet("detail/{id:int}")]
		public virtual IActionResult GetDetail(int id, string includes)
		{
			var budgetBase = (_itemsRepository as IBudgetForecastsRepository).GetDetailsById(id, includes);
			var result = _mapper.Map<Dto.BudgetForecast>(budgetBase);

			return Ok(result);
		}

        [HttpPost("update")]
        public async virtual Task<RequestResult> UpdateDetail([FromBody] BudgetSave budget)
        {

			//var result1 = await NeedBudgetResponse(549);
			Model.EntityType entityTypeNB = null;
			Model.EntityType entityType = null;
			Model.AppState appStateNB = null;
			Model.EmailType emailType = null;
			Model.EmailRequestStatus emailRequestStatus = null;
			Model.RequestBudgetForecast requestBudgetForecast = null;
			Model.Request request = null;
			Model.AppState appStateNewBudgetState = null;

			var userName = HttpContext.User.Identity.Name;

            if (userName != null && userName != "")
            {
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }

                if (user != null)
                {
                    budget.UserId = user.Id;

                    // var orderCheck = _context.Set<Model.Order>().Where(i => i.Id == budget.OrderId).SingleOrDefault();

                    //var total = budget.AprilForecastNew +
                    //    budget.MayForecastNew +
                    //    budget.JuneForecastNew +
                    //    budget.JulyForecastNew +
                    //    budget.AugustForecastNew +
                    //    budget.SeptemberForecastNew +
                    //    budget.OctomberForecastNew +
                    //    budget.NovemberForecastNew +
                    //    budget.DecemberForecastNew +
                    //    budget.JanuaryForecastNew +
                    //    budget.FebruaryForecastNew +
                    //    budget.MarchForecastNew;

					//if (budget.ValueRem + orderCheck.BudgetValueNeed < total)
					//{
					//    return new Model.RequestResult { Success = false, Message = $"Valoarea bugetului adaugat depaseste valoarea solicitata!" };
					//}

					var res = await (_itemsRepository as IBudgetForecastsRepository).UpdateBudgetForecast(budget);

                    if (res.Success && ((budget.RequestBudgetForecastId != null && budget.RequestBudgetForecastId > 0) || (budget.RequestId != null && budget.RequestId > 0)))
                    {
                        //var order = _context.Set<Model.Order>().Where(a => a.Id == budget.OrderId).SingleOrDefault();

						if(budget.RequestBudgetForecastId != null && budget.RequestBudgetForecastId > 0)
						{
							requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>().Where(a => a.Id == budget.RequestBudgetForecastId).SingleAsync();
							appStateNewBudgetState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "BUDGET_PLUS_ADDED").FirstOrDefaultAsync();
						}
						else
						{
							requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>().Where(a => a.RequestId == budget.RequestId).SingleAsync();
							appStateNewBudgetState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "BUDGET_ADDED").FirstOrDefaultAsync();
						}

                        if (requestBudgetForecast != null)
                        {

								emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "NEW_ORDER").FirstOrDefaultAsync();
								entityTypeNB = await _context.Set<Model.EntityType>().Where(c => c.Code == "ORDER").FirstOrDefaultAsync();
								appStateNB = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();

								emailRequestStatus = new Model.EmailRequestStatus()
								{
									AppStateId = appStateNB.Id,
									Completed = false,
									CreatedAt = DateTime.Now,
									CreatedBy = _context.UserId,
									DocumentNumber = int.Parse(entityTypeNB.Name),
									EmailSend = false,
									EmailTypeId = emailType.Id,
									EmployeeL1EmailSend = false,
									EmployeeL1ValidateAt = null,
									EmployeeL1ValidateBy = null,
									EmployeeL2EmailSend = false,
									EmployeeL2ValidateAt = null,
									EmployeeL2ValidateBy = null,
									EmployeeL3EmailSend = false,
									EmployeeL3ValidateAt = null,
									EmployeeL3ValidateBy = null,
									EmployeeL4EmailSend = false,
									EmployeeL4ValidateAt = null,
									EmployeeL4ValidateBy = null,
									EmployeeS1EmailSend = false,
									EmployeeS1ValidateAt = null,
									EmployeeS1ValidateBy = null,
									EmployeeS2EmailSend = false,
									EmployeeS2ValidateAt = null,
									EmployeeS2ValidateBy = null,
									EmployeeS3EmailSend = false,
									EmployeeS3ValidateAt = null,
									EmployeeS3ValidateBy = null,
									ErrorId = null,
									Exported = false,
									FinalValidateAt = null,
									FinalValidateBy = null,
									Guid = Guid.NewGuid(),
									GuidAll = Guid.NewGuid(),
									Info = string.Empty,
									IsAccepted = false,
									IsDeleted = false,
									ModifiedAt = DateTime.Now,
									ModifiedBy = _context.UserId,
									NotCompletedSync = false,
									NotEmployeeL1Sync = false,
									NotEmployeeL2Sync = false,
									NotEmployeeL3Sync = false,
									NotEmployeeL4Sync = true,
									NotEmployeeS1Sync = false,
									NotEmployeeS2Sync = false,
									NotEmployeeS3Sync = false,
									NotSync = false,
									RequestId = requestBudgetForecast.RequestId.Value,
									RequestBudgetForecastId = requestBudgetForecast.Id,
									SyncCompletedErrorCount = 0,
									SyncEmployeeL1ErrorCount = 0,
									SyncEmployeeL2ErrorCount = 0,
									SyncEmployeeL3ErrorCount = 0,
									SyncEmployeeL4ErrorCount = 0,
									SyncEmployeeS1ErrorCount = 0,
									SyncEmployeeS2ErrorCount = 0,
									SyncEmployeeS3ErrorCount = 0,
									SyncErrorCount = 0,
									EmployeeL1EmailSkip = true,
									EmployeeL2EmailSkip = true,
									EmployeeL3EmailSkip = true,
									EmployeeL4EmailSkip = false,
									EmployeeS1EmailSkip = true,
									EmployeeS2EmailSkip = true,
									EmployeeS3EmailSkip = true,
									NeedBudgetEmailSend = false,
									NotNeedBudgetSync = false,
								};

								_context.Add(emailRequestStatus);
								entityTypeNB.Name = (int.Parse(entityTypeNB.Name) + 1).ToString();
								_context.Update(entityTypeNB);


								//requestBudgetForecast.AppStateId = 15;
								//requestBudgetForecast.ModifiedAt = DateTime.Now;
								//requestBudgetForecast.NeedBudget = false;

								//_context.Update(requestBudgetForecast);
								_context.SaveChanges();

								var countBudgetBase = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
								var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();

							//return new Model.RequestResult { Success = true, Message = $"Bugetul a fost actualizat cu succes!" };

							var result = await NeedBudgetRequestResponse(requestBudgetForecast.Id);
							result = true;
							if (result)
							{
								request = await _context.Set<Model.Request>().Where(a => a.Id == requestBudgetForecast.RequestId).SingleAsync();

								requestBudgetForecast.AppStateId = 15;
								requestBudgetForecast.ModifiedAt = DateTime.Now;
								requestBudgetForecast.NeedBudget = false;

								request.AppStateId = appStateNewBudgetState.Id;

								_context.Update(request);

								_context.Update(requestBudgetForecast);


								_context.SaveChanges();

								return new Model.RequestResult { Success = true, Message = $"Bugetul a fost creat cu succes!" };
							}
							else
							{
								return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
							}
						}
                        else
                        {
                            return new Model.RequestResult { Success = false, Message = $"Ticketul nu exista!" };
                        }

                    }
                    else
                    {
                        return new Model.RequestResult { Success = res.Success, Message = res.Message };
                    }
                }
                else
                {
                    return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
                }

            }
            else
            {
                return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
            }
        }

		[HttpPost("correction")]
		public async virtual Task<BudgetForecastCorrectionResult> CorrectionDetail([FromBody] BudgetCorrectionSave budget)
		{

			//var result1 = await NeedBudgetResponse(549);
			Model.EntityType entityTypeNB = null;
			Model.EntityType entityType = null;
			Model.AppState appStateNB = null;
			Model.EmailType emailType = null;
			Model.EmailRequestStatus emailRequestStatus = null;
			Model.RequestBudgetForecast requestBudgetForecastSrc = null;
			Model.RequestBudgetForecast requestBudgetForecastDst = null;

			var userName = HttpContext.User.Identity.Name;

			if (userName != null && userName != "")
			{
				var user = await userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}

				if (user != null)
				{
					budget.UserId = user.Id;

					var res = await (_itemsRepository as IBudgetForecastsRepository).CorrectionBudgetForecast(budget);

					if(res.Success)
					{
						var countBudgetBase = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
						var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();
					}

					//if (res.Success && res.SourceId > 0 && res.DestinationId > 0)
					//{

					//	requestBudgetForecastSrc = await _context.Set<Model.RequestBudgetForecast>().Where(a => a.Id == res.SourceId).SingleAsync();
					//	requestBudgetForecastDst = await _context.Set<Model.RequestBudgetForecast>().Where(a => a.Id == res.DestinationId).SingleAsync();

					//	emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "NEW_ORDER").FirstOrDefaultAsync();
					//	entityTypeNB = await _context.Set<Model.EntityType>().Where(c => c.Code == "ORDER").FirstOrDefaultAsync();
					//	appStateNB = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();

					//	emailRequestStatus = new Model.EmailRequestStatus()
					//	{
					//		AppStateId = appStateNB.Id,
					//		Completed = false,
					//		CreatedAt = DateTime.Now,
					//		CreatedBy = _context.UserId,
					//		DocumentNumber = int.Parse(entityTypeNB.Name),
					//		EmailSend = false,
					//		EmailTypeId = emailType.Id,
					//		EmployeeL1EmailSend = false,
					//		EmployeeL1ValidateAt = null,
					//		EmployeeL1ValidateBy = null,
					//		EmployeeL2EmailSend = false,
					//		EmployeeL2ValidateAt = null,
					//		EmployeeL2ValidateBy = null,
					//		EmployeeL3EmailSend = false,
					//		EmployeeL3ValidateAt = null,
					//		EmployeeL3ValidateBy = null,
					//		EmployeeL4EmailSend = false,
					//		EmployeeL4ValidateAt = null,
					//		EmployeeL4ValidateBy = null,
					//		EmployeeS1EmailSend = false,
					//		EmployeeS1ValidateAt = null,
					//		EmployeeS1ValidateBy = null,
					//		EmployeeS2EmailSend = false,
					//		EmployeeS2ValidateAt = null,
					//		EmployeeS2ValidateBy = null,
					//		EmployeeS3EmailSend = false,
					//		EmployeeS3ValidateAt = null,
					//		EmployeeS3ValidateBy = null,
					//		ErrorId = null,
					//		Exported = false,
					//		FinalValidateAt = null,
					//		FinalValidateBy = null,
					//		Guid = Guid.NewGuid(),
					//		GuidAll = Guid.NewGuid(),
					//		Info = string.Empty,
					//		IsAccepted = false,
					//		IsDeleted = false,
					//		ModifiedAt = DateTime.Now,
					//		ModifiedBy = _context.UserId,
					//		NotCompletedSync = false,
					//		NotEmployeeL1Sync = false,
					//		NotEmployeeL2Sync = false,
					//		NotEmployeeL3Sync = false,
					//		NotEmployeeL4Sync = true,
					//		NotEmployeeS1Sync = false,
					//		NotEmployeeS2Sync = false,
					//		NotEmployeeS3Sync = false,
					//		NotSync = false,
					//		RequestId = requestBudgetForecast.RequestId.Value,
					//		RequestBudgetForecastId = requestBudgetForecast.Id,
					//		SyncCompletedErrorCount = 0,
					//		SyncEmployeeL1ErrorCount = 0,
					//		SyncEmployeeL2ErrorCount = 0,
					//		SyncEmployeeL3ErrorCount = 0,
					//		SyncEmployeeL4ErrorCount = 0,
					//		SyncEmployeeS1ErrorCount = 0,
					//		SyncEmployeeS2ErrorCount = 0,
					//		SyncEmployeeS3ErrorCount = 0,
					//		SyncErrorCount = 0,
					//		EmployeeL1EmailSkip = true,
					//		EmployeeL2EmailSkip = true,
					//		EmployeeL3EmailSkip = true,
					//		EmployeeL4EmailSkip = false,
					//		EmployeeS1EmailSkip = true,
					//		EmployeeS2EmailSkip = true,
					//		EmployeeS3EmailSkip = true,
					//		NeedBudgetEmailSend = false,
					//		NotNeedBudgetSync = false,
					//	};

					//	_context.Add(emailRequestStatus);
					//	entityTypeNB.Name = (int.Parse(entityTypeNB.Name) + 1).ToString();
					//	_context.Update(entityTypeNB);


					//	requestBudgetForecast.AppStateId = 15;
					//	requestBudgetForecast.ModifiedAt = DateTime.Now;
					//	requestBudgetForecast.NeedBudget = false;

					//	_context.Update(requestBudgetForecast);
					//	_context.SaveChanges();

					//	return new Model.RequestResult { Success = true, Message = $"Bugetul a fost actualizat cu succes!" };

					//	//var result = await NeedBudgetResponse(requestBudgetForecast.Id);

					//	//if (result)
					//	//                     {
					//	//                         requestBudgetForecast.AppStateId = 15;
					//	//                         requestBudgetForecast.ModifiedAt = DateTime.Now;
					//	//                         requestBudgetForecast.NeedBudget = false;

					//	//                         _context.Update(requestBudgetForecast);
					//	//                         _context.SaveChanges();

					//	//                         return new Model.RequestResult { Success = true, Message = $"Bugetul a fost creat cu succes!" };
					//	//                     }
					//	//                     else
					//	//                     {
					//	//                         return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
					//	//                     }

					//}
					//else
					//{
					//	return new Model.BudgetForecastCorrectionResult { Success = res.Success, Message = res.Message };
					//}

					return new Model.BudgetForecastCorrectionResult { Success = true, Message = $"Bugetele au fost actualizate cu succes!" };
				}
				else
				{
					return new Model.BudgetForecastCorrectionResult { Success = false, Message = $"Va rugam sa va autentificati!" };
				}

			}
			else
			{
				return new Model.BudgetForecastCorrectionResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}
		}

		[HttpPost("transfer")]
		public async virtual Task<BudgetForecastCorrectionResult> TransferDetail([FromBody] BudgetTransferSave budget)
		{

			//var result1 = await NeedBudgetResponse(549);
			Model.EntityType entityTypeNB = null;
			Model.EntityType entityType = null;
			Model.AppState appStateNB = null;
			Model.EmailType emailType = null;
			Model.EmailRequestStatus emailRequestStatus = null;
			Model.RequestBudgetForecast requestBudgetForecastSrc = null;
			Model.RequestBudgetForecast requestBudgetForecastDst = null;

			var userName = HttpContext.User.Identity.Name;

			if (userName != null && userName != "")
			{
				var user = await userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}

				if (user != null)
				{
					budget.UserId = user.Id;

					var res = await (_itemsRepository as IBudgetForecastsRepository).TransferBudgetForecast(budget);

					if (res.Success)
					{
						var countBudgetBase = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
						var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();
					}

					//if (res.Success && res.SourceId > 0 && res.DestinationId > 0)
					//{

					//	requestBudgetForecastSrc = await _context.Set<Model.RequestBudgetForecast>().Where(a => a.Id == res.SourceId).SingleAsync();
					//	requestBudgetForecastDst = await _context.Set<Model.RequestBudgetForecast>().Where(a => a.Id == res.DestinationId).SingleAsync();

					//	emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "NEW_ORDER").FirstOrDefaultAsync();
					//	entityTypeNB = await _context.Set<Model.EntityType>().Where(c => c.Code == "ORDER").FirstOrDefaultAsync();
					//	appStateNB = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();

					//	emailRequestStatus = new Model.EmailRequestStatus()
					//	{
					//		AppStateId = appStateNB.Id,
					//		Completed = false,
					//		CreatedAt = DateTime.Now,
					//		CreatedBy = _context.UserId,
					//		DocumentNumber = int.Parse(entityTypeNB.Name),
					//		EmailSend = false,
					//		EmailTypeId = emailType.Id,
					//		EmployeeL1EmailSend = false,
					//		EmployeeL1ValidateAt = null,
					//		EmployeeL1ValidateBy = null,
					//		EmployeeL2EmailSend = false,
					//		EmployeeL2ValidateAt = null,
					//		EmployeeL2ValidateBy = null,
					//		EmployeeL3EmailSend = false,
					//		EmployeeL3ValidateAt = null,
					//		EmployeeL3ValidateBy = null,
					//		EmployeeL4EmailSend = false,
					//		EmployeeL4ValidateAt = null,
					//		EmployeeL4ValidateBy = null,
					//		EmployeeS1EmailSend = false,
					//		EmployeeS1ValidateAt = null,
					//		EmployeeS1ValidateBy = null,
					//		EmployeeS2EmailSend = false,
					//		EmployeeS2ValidateAt = null,
					//		EmployeeS2ValidateBy = null,
					//		EmployeeS3EmailSend = false,
					//		EmployeeS3ValidateAt = null,
					//		EmployeeS3ValidateBy = null,
					//		ErrorId = null,
					//		Exported = false,
					//		FinalValidateAt = null,
					//		FinalValidateBy = null,
					//		Guid = Guid.NewGuid(),
					//		GuidAll = Guid.NewGuid(),
					//		Info = string.Empty,
					//		IsAccepted = false,
					//		IsDeleted = false,
					//		ModifiedAt = DateTime.Now,
					//		ModifiedBy = _context.UserId,
					//		NotCompletedSync = false,
					//		NotEmployeeL1Sync = false,
					//		NotEmployeeL2Sync = false,
					//		NotEmployeeL3Sync = false,
					//		NotEmployeeL4Sync = true,
					//		NotEmployeeS1Sync = false,
					//		NotEmployeeS2Sync = false,
					//		NotEmployeeS3Sync = false,
					//		NotSync = false,
					//		RequestId = requestBudgetForecast.RequestId.Value,
					//		RequestBudgetForecastId = requestBudgetForecast.Id,
					//		SyncCompletedErrorCount = 0,
					//		SyncEmployeeL1ErrorCount = 0,
					//		SyncEmployeeL2ErrorCount = 0,
					//		SyncEmployeeL3ErrorCount = 0,
					//		SyncEmployeeL4ErrorCount = 0,
					//		SyncEmployeeS1ErrorCount = 0,
					//		SyncEmployeeS2ErrorCount = 0,
					//		SyncEmployeeS3ErrorCount = 0,
					//		SyncErrorCount = 0,
					//		EmployeeL1EmailSkip = true,
					//		EmployeeL2EmailSkip = true,
					//		EmployeeL3EmailSkip = true,
					//		EmployeeL4EmailSkip = false,
					//		EmployeeS1EmailSkip = true,
					//		EmployeeS2EmailSkip = true,
					//		EmployeeS3EmailSkip = true,
					//		NeedBudgetEmailSend = false,
					//		NotNeedBudgetSync = false,
					//	};

					//	_context.Add(emailRequestStatus);
					//	entityTypeNB.Name = (int.Parse(entityTypeNB.Name) + 1).ToString();
					//	_context.Update(entityTypeNB);


					//	requestBudgetForecast.AppStateId = 15;
					//	requestBudgetForecast.ModifiedAt = DateTime.Now;
					//	requestBudgetForecast.NeedBudget = false;

					//	_context.Update(requestBudgetForecast);
					//	_context.SaveChanges();

					//	return new Model.RequestResult { Success = true, Message = $"Bugetul a fost actualizat cu succes!" };

					//	//var result = await NeedBudgetResponse(requestBudgetForecast.Id);

					//	//if (result)
					//	//                     {
					//	//                         requestBudgetForecast.AppStateId = 15;
					//	//                         requestBudgetForecast.ModifiedAt = DateTime.Now;
					//	//                         requestBudgetForecast.NeedBudget = false;

					//	//                         _context.Update(requestBudgetForecast);
					//	//                         _context.SaveChanges();

					//	//                         return new Model.RequestResult { Success = true, Message = $"Bugetul a fost creat cu succes!" };
					//	//                     }
					//	//                     else
					//	//                     {
					//	//                         return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
					//	//                     }

					//}
					//else
					//{
					//	return new Model.BudgetForecastCorrectionResult { Success = res.Success, Message = res.Message };
					//}

					return new Model.BudgetForecastCorrectionResult { Success = true, Message = $"Propunerea a fost salvata cu succes!", SourceId = budget.BudgetForecastId, DestinationId = budget.BudgetForecastDestinationId, BudgetBaseOpId= res.BudgetBaseOpId };
				}
				else
				{
					return new Model.BudgetForecastCorrectionResult { Success = false, Message = $"Va rugam sa va autentificati!" };
				}

			}
			else
			{
				return new Model.BudgetForecastCorrectionResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}
		}

        [Authorize]
		[HttpPost]
		[Route("import")]
		public async virtual Task<ImportBudgetResult> Import([FromBody] BudgetForecastImport budgetImport)
		{
			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);

				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}

				_context.UserId = user.Id.ToString();

				var result = await (_itemsRepository as IBudgetForecastsRepository).BudgetForecastImport(budgetImport);

				if (budgetImport.CurrentIndex == (budgetImport.CountLines - 1))
				{
					// var inventory = await _context.Set<Model.Inventory>().Where(a => a.IsDeleted == false && a.Active == true).FirstOrDefaultAsync();
					var firstBudgetType = await _context.Set<Model.BudgetForecast>().Include(b => b.BudgetType).Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefaultAsync();
					var lastBudgetType = await _context.Set<Model.BudgetForecast>().Include(b => b.BudgetType).Where(a => a.IsDeleted == false && a.IsLast == true).LastOrDefaultAsync();

					var month = lastBudgetType.BudgetType.Name.Substring(0, 2);
					var year = lastBudgetType.BudgetType.Name.Substring(2, 4);

					var currentMonth = await _context.Set<Model.AccMonth>().Where(a => a.IsDeleted == false && a.Month == int.Parse(month) && a.Year == int.Parse(year)).FirstOrDefaultAsync();

					List<Model.BudgetForecast> budgetForecasts = await _context.Set<Model.BudgetForecast>().Where(a => a.BudgetTypeId == firstBudgetType.BudgetTypeId).ToListAsync();

					//SE DECOMMENTEAZA IN CAZ CA SE VREA IMPORT PE TOT FISIERUL.

					//for (int i = 0; i < budgetForecasts.Count; i++)
					//{
					//	budgetForecasts[i].IsLast = false;
					//	_context.Update(budgetForecasts[i]);
					//}

					//

					//List<Model.BudgetBase> budgetBases = await _context.Set<Model.BudgetBase>().Where(a => a.BudgetTypeId == firstBudgetType.BudgetTypeId).ToListAsync();

					//for (int i = 0; i < budgetBases.Count; i++)
					//{
					//	budgetBases[i].IsLast = false;
					//	_context.Update(budgetBases[i]);
					//}

					// NEW //


					//List<Model.BudgetForecast> currentBudgetForecasts = await _context.Set<Model.BudgetForecast>().Where(a => a.BudgetTypeId == lastBudgetType.BudgetTypeId).ToListAsync();

					//for (int i = 0; i < currentBudgetForecasts.Count; i++)
					//{
					//	currentBudgetForecasts[i].AccMonthId = currentMonth.Id;
					//	_context.Update(currentBudgetForecasts[i]);
					//}

					//List<Model.BudgetBase> currentBudgetBases = await _context.Set<Model.BudgetBase>().Where(a => a.BudgetTypeId == lastBudgetType.BudgetTypeId).ToListAsync();

					//for (int i = 0; i < currentBudgetBases.Count; i++)
					//{
					//	currentBudgetBases[i].AccMonthId = currentMonth.Id;
					//	_context.Update(currentBudgetBases[i]);
					//}

					//inventory.AccMonthBudgetId = currentMonth.Id;
					//_context.Update(inventory);

					_context.SaveChanges();

					var countBudgetBase = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
					var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();
				}

				return new Model.ImportBudgetResult { Success = result.Success, Message = result.Message, Id = result.Id };
			}
			else
			{
				return new Model.ImportBudgetResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}
		}

        [Route("needbgtresponse")]
        public async Task<bool> NeedBudgetResponse(int requestBudgetForecastId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<int> emails1 = new List<int>();
            List<int> emails2 = new List<int>();
            List<int> emails = new List<int>();
            var files = new FormFileCollection();
            List<Model.RequestBudgetForecastMaterial> reqBFMaterials = null;
            Model.AppState appState = null;
            // Model.Order order = null;
            Model.RequestBudgetForecast requestBudgetForecast = null;
            //        order = await _context.Set<Model.Order>()
            //            .Include(o => o.Offer).ThenInclude(r => r.Request)
            //            .Include(o => o.Offer).ThenInclude(r => r.AssetType)
            //            .Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
            //            .Include(o => o.Offer).ThenInclude(r => r.Region)
            //            .Include(c => c.Uom)
            //             .Include(c => c.Partner)
            //            .Include(c => c.Company)
            //            .Include(c => c.CostCenter)
            //.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Employee)
            //.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
            //.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Activity)
            //.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Division)
            //.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Department)
            //            .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
            //.Include(c => c.Employee)
            //            .Include(c => c.Project)
            //            .Include(c => c.OrderType).AsNoTracking().Where(a => a.Id == orderId).SingleOrDefaultAsync();
            requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
                //.Include(o => o.Offer).ThenInclude(r => r.Request)
                //.Include(o => o.Offer).ThenInclude(r => r.AssetType)
                //.Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
                //.Include(o => o.Offer).ThenInclude(r => r.Region)
                //.Include(c => c.Uom)
                 //.Include(c => c.Partner)
                //.Include(c => c.Company)
                //.Include(c => c.CostCenter)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Employee)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Activity)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Division)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Department)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Region)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.AdmCenter)
                //.Include(c => c.Employee)
                //.Include(c => c.Project)
                //.Include(c => c.OrderType)
                .AsNoTracking().Where(a => a.Id == requestBudgetForecastId).SingleOrDefaultAsync();
            var htmlMessage = "";
            var subject = "";
            var htmlBodyEmail1 = "";
            var htmlBodyEnd = "";
            var htmlBodyCompany1 = "";
            var htmlBodyCompany2 = "";
            var htmlBody1 = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    OPTIMA
                                                </title>
                                                <style type=""text/css"">
                                                    table.redTable {
                                                          border: 2px solid #04327d;
                                                          background-color: #FFFFFF;
                                                          width: 100%;
                                                          text-align: center;
                                                          border-collapse: collapse;
                                                        }
                                                        table.redTable td, table.redTable th {
                                                          border: 1px solid #04327d;
                                                          padding: 3px 2px;
                                                        }
                                                        table.redTable tbody td {
                                                          font-size: 13px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 16px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 13px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 13px;
                                                        }
                                                        table.redTable tfoot .links {
                                                          text-align: right;
                                                        }
                                                        table.redTable tfoot .links a{
                                                          display: inline-block;
                                                          background: #FFFFFF;
                                                          color: #04327d;
                                                          padding: 2px 8px;
                                                          border-radius: 5px;
                                                        }
													.button {
                                                              background-color: #04327d;
                                                              border: none;
                                                              color: #ffffff;
                                                              padding: 5px 10px;
                                                              text-align: center;
                                                              text-decoration: none;
                                                              display: inline-block;
                                                              font-size: 16px;
                                                              margin: 4px 2px;
                                                              cursor: pointer;
                                                            }
                                                   .button-no {
                                                              background-color: #6491D9;
                                                              border: none;
                                                              color: white;
                                                              padding: 5px 10px;
                                                              text-align: center;
                                                              text-decoration: none;
                                                              display: inline-block;
                                                              font-size: 16px;
                                                              margin: 4px 2px;
                                                              cursor: pointer;
                                                            }
                                                </style>
                                            </head>
                                            <body>
                                                <table class=""redTable"">
                                                    <thead>
                                                    
                                        ";
            var htmlHeader = @"        
                                    <tr style=""background-color: #04327d;font-color: #ffffff; font-weight: bold"">
                                        <th colspan=""13"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">CONFIRMARE INCARCARE SUPLIMENTARE BUGET</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Solicitant</th>
                                        <th colspan=""2"">Buget initial RON</th>
                                        <th colspan=""2"">Buget incarcat RON</th>
                                        <th colspan=""2"">Buget total RON</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", requestBudgetForecast.CreatedAt)}</th>
										<th>{String.Format("{0:MM}", requestBudgetForecast.CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", requestBudgetForecast.CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""13""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Cod P.R.</th>
									    <th style=""color: #ffffff;"">Tip comanda</th>
										<th style=""color: #ffffff;"">WIP</th>
										<th style=""color: #ffffff;"">Cod Produs</th>
										<th style=""color: #ffffff;"">Cantitate</th>
                                        <th style=""color: #ffffff;"">Moneda</th>
										<th style=""color: #ffffff;"">P.U. Valuta</th>
                                        <th style=""color: #ffffff;"">Total comanda Valuta</th>
                                        <th style=""color: #ffffff;"">P.U. RON</th>
                                        <th style=""color: #ffffff;"">Total comanda RON</th>
									</tr>";

            var htmlHeader3 = "";

            var htmlHeader4 = @"<tr>
										<th colspan = ""13""></th>
								</tr>";

            var htmlHeader5 = "";
            var htmlHeader6 = "";
            var htmlHeader7 = "";
            var htmlHeader8 = "";
            var htmlHeader9 = "";
            var htmlHeader10 = "";

            var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

            // orderMaterials = await _context.Set<Model.OrderMaterial>().Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request).Include(o => o.Order).ThenInclude(a => a.Uom).Include(a => a.Material).Include(o => o.Order).ThenInclude(a => a.OrderType).Where(a => a.IsDeleted == false && a.OrderId == order.Id).ToListAsync();
            
            reqBFMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
               .Include(o => o.Order).ThenInclude(a => a.Uom)
               .Include(o => o.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.AssetType)
               .Include(a => a.Material)
               .Include(o => o.Order).ThenInclude(a => a.OrderType)
               .Include(o => o.Order).ThenInclude(a => a.Partner)
               .Include(o => o.Order).ThenInclude(a => a.Company)
               .Include(o => o.Order).ThenInclude(a => a.Employee)
               .Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecastId)
               .ToListAsync();
            int index = 0;

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/OFA";

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "NEED_BUDGET").SingleAsync();

			string empIni = "";
			if (reqBFMaterials.Count > 0)
			{
				empIni = reqBFMaterials[0].Order.Employee != null ? reqBFMaterials[0].Order.Employee.FirstName + " " + reqBFMaterials[0].Order.Employee.LastName : "";
			}
			else
			{
				empIni = "";
			}
            


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{(reqBFMaterials.Count > 0 && reqBFMaterials[0].Order != null && reqBFMaterials[0].Order.Employee != null && reqBFMaterials[0].Order.Employee.Email != null ? reqBFMaterials[0].Order.Employee.Email : "")}</th>
                                                <th rowspan=""2"" colspan=""2"">{String.Format("{0:#,##0.##}", requestBudgetForecast.ValueRon - requestBudgetForecast.NeedBudgetValue)}</th>
                                                <th rowspan=""2"" colspan=""2"">{String.Format("{0:#,##0.##}", requestBudgetForecast.NeedBudgetValue)}</th>
                                                <th rowspan=""2"" colspan=""2"">{String.Format("{0:#,##0.##}", requestBudgetForecast.ValueRon)}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""13"" style=""color: #ffffff;"">Detalii OFERTA</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Furnizor</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Companie</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Tip</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"">{(reqBFMaterials.Count > 0 && reqBFMaterials[0].Order != null && reqBFMaterials[0].Order.Partner != null ? reqBFMaterials[0].Order.Partner.Name : "")}</th>
												<th>{(reqBFMaterials.Count > 0 && reqBFMaterials[0].Order != null && reqBFMaterials[0].Order.Company != null ? reqBFMaterials[0].Order.Company.Code : "")}</th>
												<th>{(reqBFMaterials.Count > 0 && reqBFMaterials[0].Order != null && reqBFMaterials[0].Order.Offer != null && reqBFMaterials[0].Order.Offer.AssetType != null ? reqBFMaterials[0].Order.Offer.AssetType.Name : "")}</th>
												</tr>";

            htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""13"" style=""color: #ffffff;"">Detalii BUGET</th>
												</tr>";

            htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Cod OPTIMA</th>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">WBS</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Activity</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">PC</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">PC Det</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
												<th rowspan=""2"" colspan=""2"">
												</th>
												</tr>";

            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th >{(requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Code: "")}</th>
												<th colspan=""2"">{(requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.Employee != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Employee.Email : "")}</th>
												< th>{ (requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.Project  != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Project.Code : "")}</th>
                                                <th>{ (requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.Activity != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Activity.Name : "")}</th>
                                                <th>{ (requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.Region != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Region.Name : "")}</th>
                                                <th>{ (requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.AdmCenter != null ? requestBudgetForecast.BudgetForecast.BudgetBase.AdmCenter.Name : "")}</th>
												< th>{ (requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Info: "")}</th>
												</tr>";


            subject = "Suplimentare buget pentru comanda: " + (reqBFMaterials.Count > 0 && reqBFMaterials[0].Order != null ? reqBFMaterials[0].Order.Code + "!!" : "");

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{(reqBFMaterials.Count > 0 && reqBFMaterials[0].Order != null ? reqBFMaterials[0].Order.Code : "")}</th>";



            for (int i = 0; i < reqBFMaterials.Count; i++)
            {
                index++;
                var wip = reqBFMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{(reqBFMaterials[i].Order != null && reqBFMaterials[i].Order.Offer != null && reqBFMaterials[i].Order.Offer.Request != null ? reqBFMaterials[i].Order.Offer.Request.Code : "")}</th>
                                    <th>{(reqBFMaterials[i].Order != null && reqBFMaterials[i].Order.OrderType != null ? reqBFMaterials[i].Order.OrderType.Name : "")}</th>
									<th>{wip}</th>
									<th>{(reqBFMaterials[i].Material != null ? reqBFMaterials[i].Material.Code: "")}</th>
                                    <th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].Quantity)}</th>
                                    <th>{reqBFMaterials[i].Order.Uom.Code}</th>
									<th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].Price)}</th>
									<th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].Value)}</th>
                                    <th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].PriceRon)}</th>
                                    <th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].ValueRon)}</th>
								</tr>";
                if (index == reqBFMaterials.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""4""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", reqBFMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", reqBFMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", reqBFMaterials.Sum(a => a.ValueRon))}</th>
								</tr>";
                }
            };

            //    htmlHeader3 = htmlHeader3 + $@"      
            //                        <tr>
            //                            <th>{index}</th>
            //	<th colspan=""2"">{order.OrderType.Name}</th>
            //	<th>{order.Offer.Code}</th>
            //	<th>{order.Offer.Request.Code}</th>
            //                            <th colspan=""2"">{order.Uom.Code}</th>
            //	<th>{String.Format("{0:#,##0.##}", order.Price)}</th>
            //</tr>";


            emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == reqBFMaterials[0].Order.EmployeeId).Select(a => a.Id).ToList();

            if (emails1.Count > 0)
            {
                emails.Add(emails1.ElementAt(0));
            }

            for (int e = 0; e < emails.Count; e++)
            {
                var emp = _context.Set<Model.Employee>().Where(employee => employee.Id == emails.ElementAt(e)).Single();

                if (emp.Email != null && emp.Email != "")
                {
                    to.Add(emp.Email);
                }
                else
                {
                    to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
                }
            }

			

			
			cc.Add("madalina.udrea@emag.ro");
			//to.Add("silvia.damian@emag.ro");
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");

			cc = new List<string>();
			cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
#endif

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

            var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

            successEmail = await _emailSender.SendEmailAsync(emailMessage);

            to = new List<string>();

            if (successEmail)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		[Route("needbgtresponse")]
		public async Task<bool> NeedBudgetRequestResponse(int requestBudgetForecastId)
		{
			bool successEmail = false;
			List<string> to = new List<string>();
			List<string> cc = new List<string>();
			List<string> bcc = new List<string>();
			List<int> emails1 = new List<int>();
			List<int> emails2 = new List<int>();
			List<int> emails = new List<int>();
			var files = new FormFileCollection();
			//List<Model.RequestBudgetForecastMaterial> reqBFMaterials = null;
			Model.AppState appState = null;
			// Model.Order order = null;
			Model.RequestBudgetForecast requestBudgetForecast = null;
			//        order = await _context.Set<Model.Order>()
			//            .Include(o => o.Offer).ThenInclude(r => r.Request)
			//            .Include(o => o.Offer).ThenInclude(r => r.AssetType)
			//            .Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
			//            .Include(o => o.Offer).ThenInclude(r => r.Region)
			//            .Include(c => c.Uom)
			//             .Include(c => c.Partner)
			//            .Include(c => c.Company)
			//            .Include(c => c.CostCenter)
			//.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Employee)
			//.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
			//.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Activity)
			//.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Division)
			//.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Department)
			//            .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
			//.Include(c => c.Employee)
			//            .Include(c => c.Project)
			//            .Include(c => c.OrderType).AsNoTracking().Where(a => a.Id == orderId).SingleOrDefaultAsync();
			requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
				//.Include(o => o.Offer).ThenInclude(r => r.Request)
				//.Include(o => o.Offer).ThenInclude(r => r.AssetType)
				//.Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
				//.Include(o => o.Offer).ThenInclude(r => r.Region)
				//.Include(c => c.Uom)
				//.Include(c => c.Partner)
				//.Include(c => c.Company)
				//.Include(c => c.CostCenter)
				.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Employee)
				.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
				.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Activity)
				.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Division)
				.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Department)
				.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
				.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Region)
				.Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.AdmCenter)
				.Include(c => c.Request)
				.Include(c => c.Request).ThenInclude(e => e.Employee)
				//.Include(c => c.OrderType)
				.AsNoTracking().Where(a => a.Id == requestBudgetForecastId).SingleOrDefaultAsync();
			var htmlMessage = "";
			var subject = "";
			var htmlBodyEmail1 = "";
			var htmlBodyEnd = "";
			var htmlBodyCompany1 = "";
			var htmlBodyCompany2 = "";
			var htmlBody1 = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    OPTIMA
                                                </title>
                                                <style type=""text/css"">
                                                    table.redTable {
                                                          border: 2px solid #04327d;
                                                          background-color: #FFFFFF;
                                                          width: 100%;
                                                          text-align: center;
                                                          border-collapse: collapse;
                                                        }
                                                        table.redTable td, table.redTable th {
                                                          border: 1px solid #04327d;
                                                          padding: 3px 2px;
                                                        }
                                                        table.redTable tbody td {
                                                          font-size: 13px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 16px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 13px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 13px;
                                                        }
                                                        table.redTable tfoot .links {
                                                          text-align: right;
                                                        }
                                                        table.redTable tfoot .links a{
                                                          display: inline-block;
                                                          background: #FFFFFF;
                                                          color: #04327d;
                                                          padding: 2px 8px;
                                                          border-radius: 5px;
                                                        }
													.button {
                                                              background-color: #04327d;
                                                              border: none;
                                                              color: #ffffff;
                                                              padding: 5px 10px;
                                                              text-align: center;
                                                              text-decoration: none;
                                                              display: inline-block;
                                                              font-size: 16px;
                                                              margin: 4px 2px;
                                                              cursor: pointer;
                                                            }
                                                   .button-no {
                                                              background-color: #6491D9;
                                                              border: none;
                                                              color: white;
                                                              padding: 5px 10px;
                                                              text-align: center;
                                                              text-decoration: none;
                                                              display: inline-block;
                                                              font-size: 16px;
                                                              margin: 4px 2px;
                                                              cursor: pointer;
                                                            }
                                                </style>
                                            </head>
                                            <body>
                                                <table class=""redTable"">
                                                    <thead>
                                                    
                                        ";
			var htmlHeader = @"        
                                    <tr style=""background-color: #04327d;font-color: #ffffff; font-weight: bold"">
                                        <th colspan=""13"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">CONFIRMARE INCARCARE SUPLIMENTARE BUGET</th>                                      
									</tr>";

			var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""3"">Solicitant</th>
                                        <th colspan=""2"">Buget incarcat RON</th>
                                        <th colspan=""2"">Buget total RON</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

			var htmlHeader11 = "";

			var htmlHeader12 = "";
			var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", requestBudgetForecast.CreatedAt)}</th>
										<th>{String.Format("{0:MM}", requestBudgetForecast.CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", requestBudgetForecast.CreatedAt)}</th>
									</tr>";

			var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""13""></th>
									</tr>";

			var htmlHeader3 = "";

			var htmlHeader4 = @"<tr>
										<th colspan = ""13""></th>
								</tr>";

			var htmlHeader5 = "";
			var htmlHeader6 = "";
			var htmlHeader7 = "";
			var htmlHeader8 = "";
			var htmlHeader9 = "";
			var htmlHeader10 = "";

			var htmlHeaderEnd = @"
                                </thead>
                                <tbody>";

			// orderMaterials = await _context.Set<Model.OrderMaterial>().Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request).Include(o => o.Order).ThenInclude(a => a.Uom).Include(a => a.Material).Include(o => o.Order).ThenInclude(a => a.OrderType).Where(a => a.IsDeleted == false && a.OrderId == order.Id).ToListAsync();

			//reqBFMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
			//   .Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request)
			//   .Include(o => o.Order).ThenInclude(a => a.Uom)
			//   .Include(o => o.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.AssetType)
			//   .Include(a => a.Material)
			//   .Include(o => o.Order).ThenInclude(a => a.OrderType)
			//   .Include(o => o.Order).ThenInclude(a => a.Partner)
			//   .Include(o => o.Order).ThenInclude(a => a.Company)
			//   .Include(o => o.Order).ThenInclude(a => a.Employee)
			//   .Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecastId)
			//   .ToListAsync();
			int index = 0;

			//var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
			////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
			//var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

			var link = "https://optima.emag.network/OFA";

			appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "NEED_BUDGET").SingleAsync();

			string empIni = "";
			if (requestBudgetForecast != null && requestBudgetForecast.Request != null && requestBudgetForecast.Request.Owner != null)
			{
				empIni = requestBudgetForecast.Request.Owner.Email;
			}
			else
			{
				empIni = "";
			}



			htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""3"">{(requestBudgetForecast != null && requestBudgetForecast.Request != null && requestBudgetForecast.Request.Employee != null && requestBudgetForecast.Request.Employee.Email != null ? requestBudgetForecast.Request.Employee.Email : "")}</th>
                                                <th rowspan=""2"" colspan=""2"">{String.Format("{0:#,##0.##}", requestBudgetForecast.NeedBudgetValue)}</th>
                                                <th rowspan=""2"" colspan=""2"">{String.Format("{0:#,##0.##}", requestBudgetForecast.BudgetForecast.TotalRem)}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

			//htmlHeader5 = htmlHeader5 + $@"
			//									<tr style=""background-color: #04327d;"">
			//									<th colspan=""13"" style=""color: #ffffff;"">Detalii OFERTA</th>
			//									</tr>";
			//htmlHeader6 = htmlHeader6 + $@"
			//									<tr>
			//									<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Furnizor</th>
			//									<th style=""background-color: #6491D9;color: #ffffff;"">Companie</th>
			//									<th style=""background-color: #6491D9;color: #ffffff;"">Tip</th>
			//									</tr>";

			htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2""></th>
												<th>{(requestBudgetForecast != null && requestBudgetForecast.Request != null && requestBudgetForecast.Request.Company != null ? requestBudgetForecast.Request.Company.Code : "")}</th>
												<th>{(requestBudgetForecast != null && requestBudgetForecast.Request != null && requestBudgetForecast.Request.AssetType != null ? requestBudgetForecast.Request.AssetType.Name : "")}</th>
												</tr>";

			htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""13"" style=""color: #ffffff;"">Detalii BUGET</th>
												</tr>";

			htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Cod OPTIMA</th>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">WBS</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Activity</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">PC</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">PC Det</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
												<th rowspan=""2"" colspan=""2"">
												</th>
												</tr>";

			htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th >{(requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Code : "")}</th>
												<th colspan=""2"">{(requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.Employee != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Employee.Email : "")}</th>
												<th>{(requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.Project != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Project.Code : "")}</th>
                                                <th>{(requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.Activity != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Activity.Name : "")}</th>
                                                <th>{(requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.Region != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Region.Name : "")}</th>
                                                <th>{(requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.AdmCenter != null ? requestBudgetForecast.BudgetForecast.BudgetBase.AdmCenter.Name : "")}</th>
												<th>{(requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null ? requestBudgetForecast.BudgetForecast.BudgetBase.Info : "")}</th>
												</tr>";


			subject = "Suplimentare buget pentru P.R.: " + (requestBudgetForecast != null && requestBudgetForecast.Request != null ? requestBudgetForecast.Request.Code + "!!" : "");

			htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{(requestBudgetForecast != null && requestBudgetForecast.Request != null ? requestBudgetForecast.Request.Code : "")}</th>";



			//for (int i = 0; i < reqBFMaterials.Count; i++)
			//{
			//	index++;
			//	var wip = reqBFMaterials[i].WIP ? "DA" : "NU";
			//	htmlHeader3 += $@"      
   //                             <tr>
   //                                 <th>{index}</th>
			//						<th>{(reqBFMaterials[i].Order != null && reqBFMaterials[i].Order.Offer != null && reqBFMaterials[i].Order.Offer.Request != null ? reqBFMaterials[i].Order.Offer.Request.Code : "")}</th>
   //                                 <th>{(reqBFMaterials[i].Order != null && reqBFMaterials[i].Order.OrderType != null ? reqBFMaterials[i].Order.OrderType.Name : "")}</th>
			//						<th>{wip}</th>
			//						<th>{(reqBFMaterials[i].Material != null ? reqBFMaterials[i].Material.Code : "")}</th>
   //                                 <th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].Quantity)}</th>
   //                                 <th>{reqBFMaterials[i].Order.Uom.Code}</th>
			//						<th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].Price)}</th>
			//						<th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].Value)}</th>
   //                                 <th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].PriceRon)}</th>
   //                                 <th>{String.Format("{0:#,##0.##}", reqBFMaterials[i].ValueRon)}</th>
			//					</tr>";
			//	if (index == reqBFMaterials.Count)
			//	{
			//		htmlHeader3 += $@"      
   //                             <tr>
   //                                 <th colspan=""4""></th>
   //                                 <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
   //                                 <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", reqBFMaterials.Sum(a => a.Quantity))}</th>
   //                                 <th style=""background-color: #6491D9;color: #ffffff;""></th>
   //                                 <th style=""background-color: #6491D9;color: #ffffff;""></th>
   //                                 <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", reqBFMaterials.Sum(a => a.Value))}</th>
   //                                 <th style=""background-color: #6491D9;color: #ffffff;""></th>
   //                                 <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", reqBFMaterials.Sum(a => a.ValueRon))}</th>
			//					</tr>";
			//	}
			//};

			//    htmlHeader3 = htmlHeader3 + $@"      
			//                        <tr>
			//                            <th>{index}</th>
			//	<th colspan=""2"">{order.OrderType.Name}</th>
			//	<th>{order.Offer.Code}</th>
			//	<th>{order.Offer.Request.Code}</th>
			//                            <th colspan=""2"">{order.Uom.Code}</th>
			//	<th>{String.Format("{0:#,##0.##}", order.Price)}</th>
			//</tr>";


			emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == requestBudgetForecast.Request.EmployeeId).Select(a => a.Id).ToList();

			if (emails1.Count > 0)
			{
				emails.Add(emails1.ElementAt(0));
			}

			if (requestBudgetForecast != null && requestBudgetForecast.BudgetForecast != null && requestBudgetForecast.BudgetForecast.BudgetBase != null && requestBudgetForecast.BudgetForecast.BudgetBase.Employee != null)
			{
				emails2 = await _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == requestBudgetForecast.BudgetForecast.BudgetBase.EmployeeId).Select(a => a.Id).ToListAsync();


				if (emails2.Count > 0)
				{
					emails.Add(emails2.ElementAt(0));
				}
			}

			for (int e = 0; e < emails.Count; e++)
			{
				var emp = _context.Set<Model.Employee>().Where(employee => employee.Id == emails.ElementAt(e)).Single();

				if (emp.Email != null && emp.Email != "")
				{
					to.Add(emp.Email);
				}
				else
				{
					to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
				}
			}




			cc.Add("madalina.udrea@emag.ro");
			cc.Add("capex.opex@emag.ro");
			bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

#if DEBUG
			to = new List<string>();
			to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");

			cc = new List<string>();

			cc.Add("adrian.cirnaru@optima.ro");cc.Add("cosmina.pricop@optima.ro");
			//cc.Add("madalina.udrea@emag.ro");
#endif

			htmlMessage = htmlBody1 + htmlBodyCompany1 + htmlBodyCompany2 + htmlHeader + htmlHeader1 + htmlHeader11 + htmlHeader12 + htmlHeader13 + htmlHeader2 + htmlHeader3 + htmlHeader4 + htmlHeader5 + htmlHeader6 + htmlHeader7 + htmlHeader8 + htmlHeader9 + htmlHeader10 + htmlHeaderEnd + htmlBodyEmail1 + htmlBodyEnd;

			var emailMessage = new Message(to, cc, bcc, subject, htmlMessage, null);

			successEmail = await _emailSender.SendEmailAsync(emailMessage);

			to = new List<string>();

			if (successEmail)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		[HttpPost("updateAssetBudgetForecast")]
		public async virtual Task<RequestResult> UpdateUpdateAssetBudgetForecast([FromBody] BudgetForecastUpdate budget)
		{
			var userName = HttpContext.User.Identity.Name;

			if (userName != null && userName != "")
			{
				var user = await userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}

				if (user != null)
				{
					budget.UserId = user.Id;

					var result = await (_itemsRepository as IBudgetForecastsRepository).UpdateAssetBudgetForecast(budget);

					if (result.Success)
					{

						return new Model.RequestResult { Success = true, Message = result.Message };

					}
					else
					{
						return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
					}
				}
				else
				{
					return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
				}

			}
			else
			{
				return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}
		}

		//[HttpGet("export")]
		//public async Task<IActionResult> ExporteMAG(int page, int pageSize, string sortColumn, string sortDirection,
		//	string includes, string jsonFilter, string filter)
		//{

		//	ForecastTotal depTotal = null;
		//	BudgetFilter budgetFilter = null;
		//	string employeeId = string.Empty;
		//	string admCenterId = string.Empty;
		//	int rowNumber = 0;
		//	string userName = string.Empty;
		//	string role = string.Empty;

		//	budgetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();

		//	if (HttpContext.User.Identity.Name != null)
		//	{
		//		var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

		//		if (user == null)
		//		{
		//			user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
		//		}
		//		role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
		//		employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

		//		budgetFilter.Role = role;
		//		budgetFilter.InInventory = user.InInventory;
		//		budgetFilter.UserId = user.Id;

		//		if (employeeId != null)
		//		{
		//			budgetFilter.EmployeeId = int.Parse(employeeId);
		//		}
		//		else
		//		{
		//			budgetFilter.EmployeeIds = null;
		//			budgetFilter.EmployeeIds = new List<int?>();
		//			budgetFilter.EmployeeIds.Add(int.Parse("-1"));
		//		}
		//	}
		//	else
		//	{
		//		budgetFilter.EmployeeIds = null;
		//		budgetFilter.EmployeeIds = new List<int?>();
		//		budgetFilter.EmployeeIds.Add(int.Parse("-1"));
		//	}


		//	includes = includes + "BudgetForecast.BudgetBase.StartMonth,BudgetForecast.BudgetBase.AppState,BudgetForecast.BudgetBase.ProjectType,BudgetForecast.BudgetBase.Activity,BudgetForecast.BudgetBase.Country,BudgetForecast.BudgetBase.Division.Department,BudgetForecast.BudgetBase.AssetType,BudgetForecast.BudgetBase.AdmCenter,BudgetForecast.BudgetBase.Project,BudgetForecast.BudgetBase.Employee,BudgetForecast.BudgetBase.BudgetMonthBase,BudgetForecast.BudgetBase.BudgetBaseAsset";

		//	//includes = includes + "BudgetBase.BudgetBaseAsset.Asset,BudgetBase.BudgetForecast,BudgetBase.BudgetMonthBase,BudgetBase.Project,BudgetBase.Employee,BudgetBase.Country,BudgetBase.Project,BudgetBase.Activity,BudgetBase.Project,BudgetBase.CostCenter,BudgetBase.Division,BudgetBase.Department,BudgetBase.AssetType,BudgetBase.AppState,BudgetBase.ProjectType,BudgetBase.StartMonth,BudgetBase.AdmCenter,";

		//	var items = (_itemsRepository as IBudgetForecastsRepository)
		//		.GetBuget(budgetFilter, filter, includes, null, null, out depTotal).ToList();

		//	using (ExcelPackage package = new ExcelPackage())
		//	{
		//		// add a new worksheet to the empty workbook
		//		ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Buget");

		//		int rowCell = 2;
		//		int rowTCell = 5;
		//		int rowTotal = items.Count() + 5;
		//		//First add the headers

		//		worksheet.Cells[3, 21].Value = "ACT";
		//		worksheet.Cells[3, 22].Value = "ACT";
		//		worksheet.Cells[3, 23].Value = "ACT";
		//		worksheet.Cells[3, 24].Value = "ACT";
		//		worksheet.Cells[3, 25].Value = "ACT";
		//		worksheet.Cells[3, 26].Value = "ACT";
		//		worksheet.Cells[3, 27].Value = "ACT";
		//		worksheet.Cells[3, 28].Value = "FC2";
		//		worksheet.Cells[3, 29].Value = "FC2";
		//		worksheet.Cells[3, 30].Value = "FC2";
		//		worksheet.Cells[3, 31].Value = "FC2";
		//		worksheet.Cells[3, 32].Value = "FC2";
		//		worksheet.Cells[3, 33].Value = "FC2";


		//		worksheet.Cells[3, 35].Value = "ACT";
		//		worksheet.Cells[3, 36].Value = "ACT";
		//		worksheet.Cells[3, 37].Value = "ACT";
		//		worksheet.Cells[3, 38].Value = "ACT";
		//		worksheet.Cells[3, 39].Value = "ACT";
		//		worksheet.Cells[3, 40].Value = "ACT";
		//		worksheet.Cells[3, 41].Value = "ACT";
		//		worksheet.Cells[3, 42].Value = "FC2";
		//		worksheet.Cells[3, 43].Value = "FC2";
		//		worksheet.Cells[3, 44].Value = "FC2";
		//		worksheet.Cells[3, 45].Value = "FC2";
		//		worksheet.Cells[3, 46].Value = "FC2";
		//		worksheet.Cells[3, 47].Value = "FC2";


		//		worksheet.Cells[3, 49].Value = "ACT";
		//		worksheet.Cells[3, 50].Value = "ACT";
		//		worksheet.Cells[3, 51].Value = "ACT";
		//		worksheet.Cells[3, 52].Value = "ACT";
		//		worksheet.Cells[3, 53].Value = "ACT";
		//		worksheet.Cells[3, 54].Value = "ACT";
		//		worksheet.Cells[3, 55].Value = "ACT";
		//		worksheet.Cells[3, 56].Value = "FC2";
		//		worksheet.Cells[3, 57].Value = "FC2";
		//		worksheet.Cells[3, 58].Value = "FC2";
		//		worksheet.Cells[3, 59].Value = "FC2";
		//		worksheet.Cells[3, 60].Value = "FC2";
		//		worksheet.Cells[3, 61].Value = "FC2";


		//		worksheet.Cells[3, 63].Value = "ACT";
		//		worksheet.Cells[3, 64].Value = "ACT";
		//		worksheet.Cells[3, 65].Value = "ACT";
		//		worksheet.Cells[3, 66].Value = "ACT";
		//		worksheet.Cells[3, 67].Value = "ACT";
		//		worksheet.Cells[3, 68].Value = "ACT";
		//		worksheet.Cells[3, 69].Value = "ACT";
		//		worksheet.Cells[3, 70].Value = "FC2";
		//		worksheet.Cells[3, 71].Value = "FC2";
		//		worksheet.Cells[3, 72].Value = "FC2";
		//		worksheet.Cells[3, 73].Value = "FC2";
		//		worksheet.Cells[3, 74].Value = "FC2";
		//		worksheet.Cells[3, 75].Value = "FC2";

		//		worksheet.Cells[4, 21].Value = "RON";
		//		worksheet.Cells[4, 22].Value = "RON";
		//		worksheet.Cells[4, 23].Value = "RON";
		//		worksheet.Cells[4, 24].Value = "RON";
		//		worksheet.Cells[4, 25].Value = "RON";
		//		worksheet.Cells[4, 26].Value = "RON";
		//		worksheet.Cells[4, 27].Value = "RON";
		//		worksheet.Cells[4, 28].Value = "RON";
		//		worksheet.Cells[4, 29].Value = "RON";
		//		worksheet.Cells[4, 30].Value = "RON";
		//		worksheet.Cells[4, 31].Value = "RON";
		//		worksheet.Cells[4, 32].Value = "RON";
		//		worksheet.Cells[4, 33].Value = "RON";

		//		worksheet.Cells[4, 35].Value = "RON";
		//		worksheet.Cells[4, 36].Value = "RON";
		//		worksheet.Cells[4, 37].Value = "RON";
		//		worksheet.Cells[4, 38].Value = "RON";
		//		worksheet.Cells[4, 39].Value = "RON";
		//		worksheet.Cells[4, 40].Value = "RON";
		//		worksheet.Cells[4, 41].Value = "RON";
		//		worksheet.Cells[4, 42].Value = "RON";
		//		worksheet.Cells[4, 43].Value = "RON";
		//		worksheet.Cells[4, 44].Value = "RON";
		//		worksheet.Cells[4, 45].Value = "RON";
		//		worksheet.Cells[4, 46].Value = "RON";
		//		worksheet.Cells[4, 47].Value = "RON";

		//		worksheet.Cells[4, 49].Value = "RON";
		//		worksheet.Cells[4, 50].Value = "RON";
		//		worksheet.Cells[4, 51].Value = "RON";
		//		worksheet.Cells[4, 52].Value = "RON";
		//		worksheet.Cells[4, 53].Value = "RON";
		//		worksheet.Cells[4, 54].Value = "RON";
		//		worksheet.Cells[4, 55].Value = "RON";
		//		worksheet.Cells[4, 56].Value = "RON";
		//		worksheet.Cells[4, 57].Value = "RON";
		//		worksheet.Cells[4, 58].Value = "RON";
		//		worksheet.Cells[4, 59].Value = "RON";
		//		worksheet.Cells[4, 60].Value = "RON";
		//		worksheet.Cells[4, 61].Value = "RON";

		//		worksheet.Cells[4, 63].Value = "RON";
		//		worksheet.Cells[4, 64].Value = "RON";
		//		worksheet.Cells[4, 65].Value = "RON";
		//		worksheet.Cells[4, 66].Value = "RON";
		//		worksheet.Cells[4, 67].Value = "RON";
		//		worksheet.Cells[4, 68].Value = "RON";
		//		worksheet.Cells[4, 69].Value = "RON";
		//		worksheet.Cells[4, 70].Value = "RON";
		//		worksheet.Cells[4, 71].Value = "RON";
		//		worksheet.Cells[4, 72].Value = "RON";
		//		worksheet.Cells[4, 73].Value = "RON";
		//		worksheet.Cells[4, 74].Value = "RON";
		//		worksheet.Cells[4, 75].Value = "RON";


		//		worksheet.Cells[5, 1].Value = "Budget Owner\r\n(email adress)";
		//		worksheet.Cells[5, 2].Value = "WBS";
		//		worksheet.Cells[5, 3].Value = "Tara Name";
		//		worksheet.Cells[5, 4].Value = "Tara Code";
		//		worksheet.Cells[5, 5].Value = "Activity";
		//		worksheet.Cells[5, 6].Value = "Business Unit Name";
		//		worksheet.Cells[5, 7].Value = "Business Unit Code";
		//		worksheet.Cells[5, 8].Value = "Profit Center";
		//		worksheet.Cells[5, 9].Value = "";
		//		worksheet.Cells[5, 10].Value = "Departament Name";
		//		worksheet.Cells[5, 11].Value = "Departament Code";
		//		worksheet.Cells[5, 12].Value = "Project Name";
		//		worksheet.Cells[5, 13].Value = "Project Code";
		//		worksheet.Cells[5, 14].Value = "Project Details";
		//		worksheet.Cells[5, 15].Value = "Cost Type Name";
		//		worksheet.Cells[5, 16].Value = "Cost Type Code";
		//		worksheet.Cells[5, 17].Value = "ACQ type";
		//		worksheet.Cells[5, 18].Value = "Implementation Date (month&year)";
		//		worksheet.Cells[5, 19].Value = "Dep Per\r\nBGT(month)";
		//		worksheet.Cells[5, 20].Value = "Dep Per\r\nACT(month)";
		//		worksheet.Cells[5, 21].Value = "4/1/2021";
		//		worksheet.Cells[5, 22].Value = "5/1/2021";
		//		worksheet.Cells[5, 23].Value = "6/1/2021";
		//		worksheet.Cells[5, 24].Value = "7/1/2021";
		//		worksheet.Cells[5, 25].Value = "8/1/2021";
		//		worksheet.Cells[5, 26].Value = "9/1/2021";
		//		worksheet.Cells[5, 27].Value = "10/1/2021";
		//		worksheet.Cells[5, 28].Value = "11/1/2021";
		//		worksheet.Cells[5, 29].Value = "12/1/2021";
		//		worksheet.Cells[5, 30].Value = "1/1/2022";
		//		worksheet.Cells[5, 31].Value = "2/1/2022";
		//		worksheet.Cells[5, 32].Value = "3/1/2022";
		//		worksheet.Cells[5, 33].Value = "TY FY22";


		//		worksheet.Cells[5, 35].Value = "4/1/2021";
		//		worksheet.Cells[5, 36].Value = "5/1/2021";
		//		worksheet.Cells[5, 37].Value = "6/1/2021";
		//		worksheet.Cells[5, 38].Value = "7/1/2021";
		//		worksheet.Cells[5, 39].Value = "8/1/2021";
		//		worksheet.Cells[5, 40].Value = "9/1/2021";
		//		worksheet.Cells[5, 41].Value = "10/1/2021";
		//		worksheet.Cells[5, 42].Value = "11/1/2021";
		//		worksheet.Cells[5, 43].Value = "12/1/2021";
		//		worksheet.Cells[5, 44].Value = "1/1/2022";
		//		worksheet.Cells[5, 45].Value = "2/1/2022";
		//		worksheet.Cells[5, 46].Value = "3/1/2022";
		//		worksheet.Cells[5, 47].Value = "TY FY22";

		//		worksheet.Cells[5, 49].Value = "4/1/2021";
		//		worksheet.Cells[5, 50].Value = "5/1/2021";
		//		worksheet.Cells[5, 51].Value = "6/1/2021";
		//		worksheet.Cells[5, 52].Value = "7/1/2021";
		//		worksheet.Cells[5, 53].Value = "8/1/2021";
		//		worksheet.Cells[5, 54].Value = "9/1/2021";
		//		worksheet.Cells[5, 55].Value = "10/1/2021";
		//		worksheet.Cells[5, 56].Value = "11/1/2021";
		//		worksheet.Cells[5, 57].Value = "12/1/2021";
		//		worksheet.Cells[5, 58].Value = "1/1/2022";
		//		worksheet.Cells[5, 59].Value = "2/1/2022";
		//		worksheet.Cells[5, 60].Value = "3/1/2022";
		//		worksheet.Cells[5, 61].Value = "TY FY22";

		//		worksheet.Cells[5, 63].Value = "4/1/2021";
		//		worksheet.Cells[5, 64].Value = "5/1/2021";
		//		worksheet.Cells[5, 65].Value = "6/1/2021";
		//		worksheet.Cells[5, 66].Value = "7/1/2021";
		//		worksheet.Cells[5, 67].Value = "8/1/2021";
		//		worksheet.Cells[5, 68].Value = "9/1/2021";
		//		worksheet.Cells[5, 69].Value = "10/1/2021";
		//		worksheet.Cells[5, 70].Value = "11/1/2021";
		//		worksheet.Cells[5, 71].Value = "12/1/2021";
		//		worksheet.Cells[5, 72].Value = "1/1/2022";
		//		worksheet.Cells[5, 73].Value = "2/1/2022";
		//		worksheet.Cells[5, 74].Value = "3/1/2022";
		//		worksheet.Cells[5, 75].Value = "TY FY22";


		//		int recordIndex = 6;
		//		int count = items.Count();

		//		foreach (var item in items)
		//		{
		//			rowNumber++;
		//			rowTCell++;
		//			int diff = recordIndex - count;

		//			if (diff > 0)
		//			{
		//				diff = 0;
		//			}


		//			worksheet.Cells[recordIndex, 1].Value = item.BudgetForecast.BudgetBase.Employee != null ? item.BudgetForecast.BudgetBase.Employee.Email : "";
		//			worksheet.Cells[recordIndex, 2].Value = item.BudgetForecast.BudgetBase.Project != null ? item.BudgetForecast.BudgetBase.Project.Code : "";
		//			worksheet.Cells[recordIndex, 3].Value = item.BudgetForecast.BudgetBase.Country != null ? item.BudgetForecast.BudgetBase.Country.Name : "";
		//			worksheet.Cells[recordIndex, 4].Value = item.BudgetForecast.BudgetBase.Country != null ? item.BudgetForecast.BudgetBase.Country.Code : "";
		//			worksheet.Cells[recordIndex, 5].Value = item.BudgetForecast.BudgetBase.Activity != null ? item.BudgetForecast.BudgetBase.Activity.Name : "";
		//			worksheet.Cells[recordIndex, 6].Value = item.BudgetForecast.BudgetBase.Division != null && item.BudgetForecast.BudgetBase.Division.Department != null ? item.BudgetForecast.BudgetBase.Division.Department.Name : "";
		//			worksheet.Cells[recordIndex, 7].Value = item.BudgetForecast.BudgetBase.Division != null && item.BudgetForecast.BudgetBase.Division.Department != null ? item.BudgetForecast.BudgetBase.Division.Department.Code : "";
		//			worksheet.Cells[recordIndex, 8].Value = item.BudgetForecast.BudgetBase.AdmCenter != null ? item.BudgetForecast.BudgetBase.AdmCenter.Name : "";
		//			worksheet.Cells[recordIndex, 9].Value = "";
		//			worksheet.Cells[recordIndex, 10].Value = item.BudgetForecast.BudgetBase.Division != null ? item.BudgetForecast.BudgetBase.Division.Name : "";
		//			worksheet.Cells[recordIndex, 11].Value = item.BudgetForecast.BudgetBase.Division != null ? item.BudgetForecast.BudgetBase.Division.Code : "";
		//			worksheet.Cells[recordIndex, 12].Value = item.BudgetForecast.BudgetBase.ProjectType != null ? item.BudgetForecast.BudgetBase.ProjectType.Name : "";
		//			worksheet.Cells[recordIndex, 13].Value = item.BudgetForecast.BudgetBase.ProjectType != null ? item.BudgetForecast.BudgetBase.ProjectType.Code : "";
		//			worksheet.Cells[recordIndex, 14].Value = item.BudgetForecast.BudgetBase.Info;
		//			worksheet.Cells[recordIndex, 15].Value = item.BudgetForecast.BudgetBase.AssetType != null ? item.BudgetForecast.BudgetBase.AssetType.Name : "";
		//			worksheet.Cells[recordIndex, 16].Value = item.BudgetForecast.BudgetBase.AssetType != null ? item.BudgetForecast.BudgetBase.AssetType.Code : "";
		//			worksheet.Cells[recordIndex, 17].Value = item.BudgetForecast.BudgetBase.AppState != null ? item.BudgetForecast.BudgetBase.AppState.Name : "";
		//			//worksheet.Cells[recordIndex, 17].Style.Numberformat.Format = "yyyy-mm-dd";
		//			worksheet.Cells[recordIndex, 18].Value = item.BudgetForecast.BudgetBase.StartMonth != null ? item.BudgetForecast.BudgetBase.StartMonth.Month : "";
		//			//worksheet.Cells[recordIndex, 18].Style.Numberformat.Format = "yyyy-mm-dd";
		//			worksheet.Cells[recordIndex, 19].Value = item.BudgetForecast.BudgetBase.DepPeriod;
		//			//worksheet.Cells[recordIndex, 19].Style.Numberformat.Format = "yyyy-mm-dd";
		//			worksheet.Cells[recordIndex, 20].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Max(a => a.Asset?.DepPeriod);
		//			//worksheet.Cells[recordIndex, 20].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 21].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.April);
		//			worksheet.Cells[recordIndex, 21].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 22].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.May);
		//			worksheet.Cells[recordIndex, 22].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 23].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.June);
		//			worksheet.Cells[recordIndex, 23].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 24].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.July);
		//			worksheet.Cells[recordIndex, 24].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 25].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.August);
		//			worksheet.Cells[recordIndex, 25].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 26].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.September);
		//			worksheet.Cells[recordIndex, 26].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 27].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.Octomber);
		//			worksheet.Cells[recordIndex, 27].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 28].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.November);
		//			worksheet.Cells[recordIndex, 28].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 29].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.December);
		//			worksheet.Cells[recordIndex, 29].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 30].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.January);
		//			worksheet.Cells[recordIndex, 30].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 31].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.February);
		//			worksheet.Cells[recordIndex, 31].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 32].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.March);
		//			worksheet.Cells[recordIndex, 32].Style.Numberformat.Format = "#,##0.00";
		//			//worksheet.Cells[recordIndex, 33].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast).Select(a => a.Total);
		//			//worksheet.Cells[recordIndex, 33].Style.Numberformat.Format = "#,##0.00";

		//			worksheet.Cells[recordIndex, 35].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.April);
		//			worksheet.Cells[recordIndex, 35].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 36].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.May);
		//			worksheet.Cells[recordIndex, 36].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 37].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.June);
		//			worksheet.Cells[recordIndex, 37].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 38].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.July);
		//			worksheet.Cells[recordIndex, 38].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 39].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.August);
		//			worksheet.Cells[recordIndex, 39].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 40].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.September);
		//			worksheet.Cells[recordIndex, 40].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 41].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.Octomber);
		//			worksheet.Cells[recordIndex, 41].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 42].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.November);
		//			worksheet.Cells[recordIndex, 42].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 43].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.December);
		//			worksheet.Cells[recordIndex, 43].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 44].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.January);
		//			worksheet.Cells[recordIndex, 44].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 45].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.February);
		//			worksheet.Cells[recordIndex, 45].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 46].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.March);
		//			worksheet.Cells[recordIndex, 46].Style.Numberformat.Format = "#,##0.00";

		//			worksheet.Cells[recordIndex, 49].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.April);
		//			worksheet.Cells[recordIndex, 49].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 50].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.May);
		//			worksheet.Cells[recordIndex, 50].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 51].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.June);
		//			worksheet.Cells[recordIndex, 51].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 52].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.July);
		//			worksheet.Cells[recordIndex, 52].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 53].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.August);
		//			worksheet.Cells[recordIndex, 53].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 54].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.September);
		//			worksheet.Cells[recordIndex, 54].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 55].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.Octomber);
		//			worksheet.Cells[recordIndex, 55].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 56].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.November);
		//			worksheet.Cells[recordIndex, 56].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 57].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.December);
		//			worksheet.Cells[recordIndex, 57].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 58].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.January);
		//			worksheet.Cells[recordIndex, 58].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 59].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.February);
		//			worksheet.Cells[recordIndex, 59].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 60].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.March);
		//			worksheet.Cells[recordIndex, 60].Style.Numberformat.Format = "#,##0.00";



		//			worksheet.Cells[recordIndex, 63].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.April);
		//			worksheet.Cells[recordIndex, 63].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 64].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.May);
		//			worksheet.Cells[recordIndex, 64].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 65].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.June);
		//			worksheet.Cells[recordIndex, 65].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 66].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.July);
		//			worksheet.Cells[recordIndex, 66].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 67].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.August);
		//			worksheet.Cells[recordIndex, 67].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 68].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.September);
		//			worksheet.Cells[recordIndex, 68].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 69].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.Octomber);
		//			worksheet.Cells[recordIndex, 69].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 70].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.November);
		//			worksheet.Cells[recordIndex, 70].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 71].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.December);
		//			worksheet.Cells[recordIndex, 71].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 72].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.January);
		//			worksheet.Cells[recordIndex, 72].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 73].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.February);
		//			worksheet.Cells[recordIndex, 73].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells[recordIndex, 74].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.March);
		//			worksheet.Cells[recordIndex, 74].Style.Numberformat.Format = "#,##0.00";


		//			worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Formula = "SUM(U" + rowTCell + ":AF" + rowTCell + ")";
		//			worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//			worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Font.Bold = true;
		//			worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//			worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//			worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

		//			worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Formula = "SUM(AI" + rowTCell + ":AT" + rowTCell + ")";
		//			worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//			worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Font.Bold = true;
		//			worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//			worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//			worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

		//			worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Formula = "SUM(AW" + rowTCell + ":BH" + rowTCell + ")";
		//			worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//			worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Font.Bold = true;
		//			worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//			worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//			worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

		//			worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Formula = "SUM(BK" + rowTCell + ":BV" + rowTCell + ")";
		//			worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//			worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Font.Bold = true;
		//			worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//			worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Numberformat.Format = "#,##0.00";
		//			worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//			worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

		//			if (diff == 0)
		//			{
		//				for (int i = 1; i < 76; i++)
		//				{
		//					worksheet.Cells[5, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
		//					worksheet.Cells[5, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
		//					worksheet.Cells[5, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
		//					worksheet.Cells[5, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
		//					worksheet.Cells[5, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//					worksheet.Cells[5, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
		//					worksheet.Cells[5, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//					worksheet.Cells[5, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
		//					worksheet.Cells[5, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

		//				}

		//				worksheet.Row(5).Height = 35.00;
		//				worksheet.Row(5).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
		//				worksheet.Row(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.View.FreezePanes(6, 1);

		//				using (var cells = worksheet.Cells[5, 1, items.Count() + 5, 75])
		//				{
		//					cells.Style.Font.Bold = false;
		//					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
		//					cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
		//					cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
		//					cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
		//					cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
		//					cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
		//					cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//					cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
		//					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
		//					cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

		//				}

		//				using (var cells = worksheet.Cells[3, 21, 3, 75])
		//				{
		//					cells.Style.Font.Bold = true;
		//					cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
		//					cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
		//					cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
		//					cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
		//					cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//					cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
		//					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
		//					cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(198, 224, 180));
		//					cells.Style.Font.Color.SetColor(Color.Black);
		//				}

		//				using (var cells = worksheet.Cells[4, 21, 4, 75])
		//				{
		//					cells.Style.Font.Bold = true;
		//					cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
		//					cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
		//					cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
		//					cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
		//					cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//					cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
		//					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
		//					cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 204));
		//					cells.Style.Font.Color.SetColor(Color.Black);
		//				}

		//				using (var cells = worksheet.Cells[5, 1, 5, 75])
		//				{
		//					cells.Style.Font.Bold = true;
		//					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
		//					cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
		//					cells.Style.Font.Color.SetColor(Color.Black);
		//				}

		//				using (var cells = worksheet.Cells[6, 1, items.Count() + 3, 75])
		//				{
		//					cells.Style.Font.Bold = false;
		//					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
		//					cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
		//				}

		//				using (var cells = worksheet.Cells[6, 1, items.Count() + 3, 75])
		//				{
		//					for (int i = 6; i < items.Count() + 2; i++)
		//					{
		//						worksheet.Row(i).Height = 15.00;
		//						worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
		//						worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

		//						worksheet.Cells[$"A{i}"].Style.WrapText = true;
		//						worksheet.Cells[$"B{i}"].Style.WrapText = true;
		//						worksheet.Cells[$"C{i}"].Style.WrapText = true;
		//						worksheet.Cells[$"D{i}"].Style.WrapText = true;
		//						worksheet.Cells[$"E{i}"].Style.WrapText = true;
		//						worksheet.Cells[$"F{i}"].Style.WrapText = true;
		//						worksheet.Cells[$"G{i}"].Style.WrapText = true;
		//						worksheet.Cells[$"H{i}"].Style.WrapText = true;
		//						worksheet.Cells[$"I{i}"].Style.WrapText = true;
		//						worksheet.Cells[$"J{i}"].Style.WrapText = true;
		//						worksheet.Cells[$"K{i}"].Style.WrapText = true;
		//						worksheet.Cells[$"L{i}"].Style.WrapText = true;
		//						worksheet.Cells[$"M{i}"].Style.WrapText = true;


		//					}



		//				}


		//				worksheet.View.ShowGridLines = false;
		//				worksheet.View.ZoomScale = 100;

		//				for (int i = 1; i < 76; i++)
		//				{
		//					worksheet.Column(i).AutoFit();
		//				}

		//				worksheet.Column(1).Width = 30.00;
		//				worksheet.Column(2).Width = 20.00;
		//				worksheet.Column(3).Width = 15.00;
		//				worksheet.Column(4).Width = 15.00;
		//				worksheet.Column(5).Width = 20.00;
		//				worksheet.Column(6).Width = 25.00;
		//				worksheet.Column(7).Width = 21.00;
		//				worksheet.Column(8).Width = 20.00;
		//				worksheet.Column(9).Width = 0.00;
		//				worksheet.Column(10).Width = 30.00;
		//				worksheet.Column(11).Width = 20.00;
		//				worksheet.Column(12).Width = 35.00;
		//				worksheet.Column(13).Width = 15.00;
		//				worksheet.Column(14).Width = 30.00;
		//				worksheet.Column(15).Width = 30.00;
		//				worksheet.Column(16).Width = 15.00;
		//				worksheet.Column(17).Width = 15.00;
		//				worksheet.Column(18).Width = 25.00;
		//				worksheet.Column(19).Width = 20.00;
		//				worksheet.Column(20).Width = 20.00;
		//				worksheet.Column(21).Width = 15.00;
		//				worksheet.Column(22).Width = 15.00;
		//				worksheet.Column(23).Width = 15.00;
		//				worksheet.Column(24).Width = 15.00;
		//				worksheet.Column(25).Width = 15.00;
		//				worksheet.Column(26).Width = 15.00;
		//				worksheet.Column(27).Width = 15.00;
		//				worksheet.Column(28).Width = 15.00;
		//				worksheet.Column(29).Width = 15.00;
		//				worksheet.Column(30).Width = 15.00;
		//				worksheet.Column(31).Width = 15.00;
		//				worksheet.Column(32).Width = 15.00;
		//				worksheet.Column(33).Width = 15.00;
		//				worksheet.Column(34).Width = 15.00;
		//				worksheet.Column(35).Width = 15.00;
		//				worksheet.Column(36).Width = 15.00;
		//				worksheet.Column(37).Width = 15.00;
		//				worksheet.Column(38).Width = 15.00;
		//				worksheet.Column(39).Width = 15.00;
		//				worksheet.Column(40).Width = 15.00;
		//				worksheet.Column(41).Width = 15.00;
		//				worksheet.Column(42).Width = 15.00;
		//				worksheet.Column(43).Width = 15.00;
		//				worksheet.Column(44).Width = 15.00;
		//				worksheet.Column(45).Width = 15.00;
		//				worksheet.Column(46).Width = 15.00;
		//				worksheet.Column(47).Width = 15.00;
		//				worksheet.Column(48).Width = 15.00;
		//				worksheet.Column(49).Width = 15.00;
		//				worksheet.Column(50).Width = 15.00;
		//				worksheet.Column(51).Width = 15.00;
		//				worksheet.Column(52).Width = 15.00;
		//				worksheet.Column(53).Width = 15.00;
		//				worksheet.Column(54).Width = 15.00;
		//				worksheet.Column(55).Width = 15.00;
		//				worksheet.Column(56).Width = 15.00;
		//				worksheet.Column(57).Width = 15.00;
		//				worksheet.Column(58).Width = 15.00;
		//				worksheet.Column(59).Width = 15.00;
		//				worksheet.Column(60).Width = 15.00;
		//				worksheet.Column(61).Width = 15.00;
		//				worksheet.Column(62).Width = 15.00;
		//				worksheet.Column(63).Width = 15.00;
		//				worksheet.Column(64).Width = 15.00;
		//				worksheet.Column(65).Width = 15.00;
		//				worksheet.Column(66).Width = 15.00;
		//				worksheet.Column(67).Width = 15.00;
		//				worksheet.Column(68).Width = 15.00;
		//				worksheet.Column(69).Width = 15.00;
		//				worksheet.Column(70).Width = 15.00;
		//				worksheet.Column(71).Width = 15.00;
		//				worksheet.Column(72).Width = 15.00;
		//				worksheet.Column(73).Width = 15.00;
		//				worksheet.Column(74).Width = 15.00;
		//				worksheet.Column(75).Width = 15.00;

		//				//worksheet.Cells["A" + rowCell + ":K" + rowCell].Merge = true;
		//				//worksheet.Cells["A" + rowCell + ":K" + rowCell].Value = "TOTAL";
		//				//worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				//worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.Font.Bold = true;
		//				//worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				//worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				//worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));


		//				worksheet.Cells["U" + rowCell + ":U" + rowCell].Formula = "SUM(U6:U" + rowTotal + ")";
		//				worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["V" + rowCell + ":V" + rowCell].Formula = "SUM(V6:V" + rowTotal + ")";
		//				worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["W" + rowCell + ":W" + rowCell].Formula = "SUM(W6:W" + rowTotal + ")";
		//				worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["X" + rowCell + ":X" + rowCell].Formula = "SUM(X6:X" + rowTotal + ")";
		//				worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Formula = "SUM(Y6:Y" + rowTotal + ")";
		//				worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Formula = "SUM(Z6:Z" + rowTotal + ")";
		//				worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Formula = "SUM(AA6:AA" + rowTotal + ")";
		//				worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Formula = "SUM(AB6:AB" + rowTotal + ")";
		//				worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Formula = "SUM(AC6:AC" + rowTotal + ")";
		//				worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Formula = "SUM(AD6:AD" + rowTotal + ")";
		//				worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Formula = "SUM(AE6:AE" + rowTotal + ")";
		//				worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Formula = "SUM(AF6:AF" + rowTotal + ")";
		//				worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Formula = "SUM(AG6:AG" + rowTotal + ")";
		//				worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49));

		//				worksheet.Cells["U6" + ":U" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["V6" + ":V" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["W6" + ":W" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["X6" + ":X" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["Y6" + ":Y" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["Z6" + ":Z" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AA6" + ":AA" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AB6" + ":AB" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AC6" + ":AC" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AD6" + ":AD" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AE6" + ":AE" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AF6" + ":AF" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AG6" + ":AG" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(237, 125, 49));


		//				// 2 //

		//				worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Formula = "SUM(AI6:AI" + rowTotal + ")";
		//				worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Formula = "SUM(AJ6:AJ" + rowTotal + ")";
		//				worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Formula = "SUM(AK6:AK" + rowTotal + ")";
		//				worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Formula = "SUM(AL6:AL" + rowTotal + ")";
		//				worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Formula = "SUM(AM6:AM" + rowTotal + ")";
		//				worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Formula = "SUM(AN6:AN" + rowTotal + ")";
		//				worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Formula = "SUM(AO6:AO" + rowTotal + ")";
		//				worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Formula = "SUM(AP6:AP" + rowTotal + ")";
		//				worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Formula = "SUM(AQ6:AQ" + rowTotal + ")";
		//				worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Formula = "SUM(AR6:AR" + rowTotal + ")";
		//				worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Formula = "SUM(AS6:AS" + rowTotal + ")";
		//				worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Formula = "SUM(AT6:AT" + rowTotal + ")";
		//				worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Formula = "SUM(AU6:AU" + rowTotal + ")";
		//				worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49));

		//				worksheet.Cells["AI6" + ":AI" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AJ6" + ":AJ" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AK6" + ":AK" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AL6" + ":AL" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AM6" + ":AM" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AN6" + ":AN" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AO6" + ":AO" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AP6" + ":AP" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AQ6" + ":AQ" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AR6" + ":AR" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AS6" + ":AS" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AT6" + ":AT" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AU6" + ":AU" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(237, 125, 49));

		//				// 2 //

		//				// 3 //

		//				worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Formula = "SUM(AW6:AW" + rowTotal + ")";
		//				worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Formula = "SUM(AX6:AX" + rowTotal + ")";
		//				worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Formula = "SUM(AY6:AY" + rowTotal + ")";
		//				worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Formula = "SUM(AZ6:AZ" + rowTotal + ")";
		//				worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Formula = "SUM(BA6:BA" + rowTotal + ")";
		//				worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Formula = "SUM(BB6:BB" + rowTotal + ")";
		//				worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Formula = "SUM(BC6:BC" + rowTotal + ")";
		//				worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Formula = "SUM(BD6:BD" + rowTotal + ")";
		//				worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Formula = "SUM(BE6:BE" + rowTotal + ")";
		//				worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Formula = "SUM(BF6:BF" + rowTotal + ")";
		//				worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Formula = "SUM(BG6:BG" + rowTotal + ")";
		//				worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Formula = "SUM(BH6:BH" + rowTotal + ")";
		//				worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Formula = "SUM(BI6:BI" + rowTotal + ")";
		//				worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49));

		//				worksheet.Cells["AW6" + ":AW" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AX6" + ":AX" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AY6" + ":AY" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["AZ6" + ":AZ" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BA6" + ":BA" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BB6" + ":BB" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BC6" + ":BC" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BD6" + ":BD" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BE6" + ":BE" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BF6" + ":BF" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BG6" + ":BG" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BH6" + ":BH" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BI6" + ":BI" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(237, 125, 49));

		//				// 3 //

		//				// 4 //

		//				worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Formula = "SUM(BK6:BK" + rowTotal + ")";
		//				worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Formula = "SUM(BL6:BL" + rowTotal + ")";
		//				worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Formula = "SUM(BM6:BM" + rowTotal + ")";
		//				worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Formula = "SUM(BN6:BN" + rowTotal + ")";
		//				worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Formula = "SUM(BO6:BO" + rowTotal + ")";
		//				worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Formula = "SUM(BP6:BP" + rowTotal + ")";
		//				worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Formula = "SUM(BQ6:BQ" + rowTotal + ")";
		//				worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Formula = "SUM(BR6:BR" + rowTotal + ")";
		//				worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Formula = "SUM(BS6:BS" + rowTotal + ")";
		//				worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Formula = "SUM(BT6:BT" + rowTotal + ")";
		//				worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Formula = "SUM(BU6:BU" + rowTotal + ")";
		//				worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Formula = "SUM(BV6:BV" + rowTotal + ")";
		//				worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

		//				worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Formula = "SUM(BW6:BW" + rowTotal + ")";
		//				worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		//				worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Font.Bold = true;
		//				worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
		//				worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Numberformat.Format = "#,##0.00";
		//				worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
		//				worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49));

		//				worksheet.Cells["BK6" + ":BK" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BL6" + ":BL" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BM6" + ":BM" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BN6" + ":BN" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BO6" + ":BO" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BP6" + ":BP" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BQ6" + ":BQ" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BR6" + ":BR" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BS6" + ":BS" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BT6" + ":BT" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BU6" + ":BU" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BV6" + ":BV" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
		//				worksheet.Cells["BW6" + ":BW" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(237, 125, 49));

		//				// 4 //

		//				//package.Workbook.CalcMode = ExcelCalcMode.Manual;

		//				package.Workbook.Calculate();

		//				worksheet.Cells["A5:AG5"].AutoFilter = true;

		//			}
		//			recordIndex++;
		//		}

		//		using (var cells = worksheet.Cells[5, 1, 5, 75])
		//		{
		//			cells.Style.Font.Bold = true;
		//			cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
		//			cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

		//		}

		//		string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
		//		//HttpContext.Response.ContentType = entityFile.FileType;
		//		HttpContext.Response.ContentType = contentType;
		//		FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
		//		{
		//			FileDownloadName = "Export.xlsx"
		//		};

		//		return result;

		//	}
		//}

		[HttpGet("export")]
		public async Task<IActionResult> ExporteMAG(int page, int pageSize, string sortColumn, string sortDirection,
			string includes, string jsonFilter, string filter)
		{

			ForecastTotal depTotal = null;
			BudgetFilter budgetFilter = null;
			string employeeId = string.Empty;
			string admCenterId = string.Empty;
			int rowNumber = 0;
			string userName = string.Empty;
			string role = string.Empty;

			budgetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();

			if (HttpContext.User.Identity.Name != null)
			{
				var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

				if (user == null)
				{
					user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				}
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				budgetFilter.Role = role;
				budgetFilter.InInventory = user.InInventory;
				budgetFilter.UserId = user.Id;

				if (employeeId != null)
				{
					budgetFilter.EmployeeId = int.Parse(employeeId);
				}
				else
				{
					budgetFilter.EmployeeIds = null;
					budgetFilter.EmployeeIds = new List<int?>();
					budgetFilter.EmployeeIds.Add(int.Parse("-1"));
				}
			}
			else
			{
				budgetFilter.EmployeeIds = null;
				budgetFilter.EmployeeIds = new List<int?>();
				budgetFilter.EmployeeIds.Add(int.Parse("-1"));
			}


			includes = includes + "BudgetForecast.BudgetBase.StartMonth,BudgetForecast.BudgetBase.AppState,BudgetForecast.BudgetBase.ProjectType,BudgetForecast.BudgetBase.Activity,BudgetForecast.BudgetBase.Country,BudgetForecast.BudgetBase.Division.Department,BudgetForecast.BudgetBase.AssetType,BudgetForecast.BudgetBase.AdmCenter,BudgetForecast.BudgetBase.Project,BudgetForecast.BudgetBase.Employee,BudgetForecast.BudgetBase.BudgetMonthBase,BudgetForecast.BudgetBase.BudgetBaseAsset";

			//includes = includes + "BudgetBase.BudgetBaseAsset.Asset,BudgetBase.BudgetForecast,BudgetBase.BudgetMonthBase,BudgetBase.Project,BudgetBase.Employee,BudgetBase.Country,BudgetBase.Project,BudgetBase.Activity,BudgetBase.Project,BudgetBase.CostCenter,BudgetBase.Division,BudgetBase.Department,BudgetBase.AssetType,BudgetBase.AppState,BudgetBase.ProjectType,BudgetBase.StartMonth,BudgetBase.AdmCenter,";

			var items = (_itemsRepository as IBudgetForecastsRepository)
				.GetBuget(budgetFilter, filter, includes, null, null, out depTotal).ToList();

			int fiscalYear = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true && a.IsDeleted == false).SingleOrDefault().FiscalYear;

            using (ExcelPackage package = new ExcelPackage())
			{
				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Buget");

				int rowCell = 2;
				int rowTCell = 5;
				int rowTotal = items.Count() + 5;
				//First add the headers

				worksheet.Cells[3, 21].Value = "ACT";
				worksheet.Cells[3, 22].Value = "ACT";
				worksheet.Cells[3, 23].Value = "ACT";
				worksheet.Cells[3, 24].Value = "ACT";
				worksheet.Cells[3, 25].Value = "ACT";
				worksheet.Cells[3, 26].Value = "ACT";
				worksheet.Cells[3, 27].Value = "ACT";
				worksheet.Cells[3, 28].Value = "FC2";
				worksheet.Cells[3, 29].Value = "FC2";
				worksheet.Cells[3, 30].Value = "FC2";
				worksheet.Cells[3, 31].Value = "FC2";
				worksheet.Cells[3, 32].Value = "FC2";
				worksheet.Cells[3, 33].Value = "FC2";


				worksheet.Cells[3, 35].Value = "ACT";
				worksheet.Cells[3, 36].Value = "ACT";
				worksheet.Cells[3, 37].Value = "ACT";
				worksheet.Cells[3, 38].Value = "ACT";
				worksheet.Cells[3, 39].Value = "ACT";
				worksheet.Cells[3, 40].Value = "ACT";
				worksheet.Cells[3, 41].Value = "ACT";
				worksheet.Cells[3, 42].Value = "FC2";
				worksheet.Cells[3, 43].Value = "FC2";
				worksheet.Cells[3, 44].Value = "FC2";
				worksheet.Cells[3, 45].Value = "FC2";
				worksheet.Cells[3, 46].Value = "FC2";
				worksheet.Cells[3, 47].Value = "FC2";


				worksheet.Cells[3, 49].Value = "ACT";
				worksheet.Cells[3, 50].Value = "ACT";
				worksheet.Cells[3, 51].Value = "ACT";
				worksheet.Cells[3, 52].Value = "ACT";
				worksheet.Cells[3, 53].Value = "ACT";
				worksheet.Cells[3, 54].Value = "ACT";
				worksheet.Cells[3, 55].Value = "ACT";
				worksheet.Cells[3, 56].Value = "FC2";
				worksheet.Cells[3, 57].Value = "FC2";
				worksheet.Cells[3, 58].Value = "FC2";
				worksheet.Cells[3, 59].Value = "FC2";
				worksheet.Cells[3, 60].Value = "FC2";
				worksheet.Cells[3, 61].Value = "FC2";


				worksheet.Cells[3, 63].Value = "ACT";
				worksheet.Cells[3, 64].Value = "ACT";
				worksheet.Cells[3, 65].Value = "ACT";
				worksheet.Cells[3, 66].Value = "ACT";
				worksheet.Cells[3, 67].Value = "ACT";
				worksheet.Cells[3, 68].Value = "ACT";
				worksheet.Cells[3, 69].Value = "ACT";
				worksheet.Cells[3, 70].Value = "FC2";
				worksheet.Cells[3, 71].Value = "FC2";
				worksheet.Cells[3, 72].Value = "FC2";
				worksheet.Cells[3, 73].Value = "FC2";
				worksheet.Cells[3, 74].Value = "FC2";
				worksheet.Cells[3, 75].Value = "FC2";

				worksheet.Cells[4, 21].Value = "RON";
				worksheet.Cells[4, 22].Value = "RON";
				worksheet.Cells[4, 23].Value = "RON";
				worksheet.Cells[4, 24].Value = "RON";
				worksheet.Cells[4, 25].Value = "RON";
				worksheet.Cells[4, 26].Value = "RON";
				worksheet.Cells[4, 27].Value = "RON";
				worksheet.Cells[4, 28].Value = "RON";
				worksheet.Cells[4, 29].Value = "RON";
				worksheet.Cells[4, 30].Value = "RON";
				worksheet.Cells[4, 31].Value = "RON";
				worksheet.Cells[4, 32].Value = "RON";
				worksheet.Cells[4, 33].Value = "RON";

				worksheet.Cells[4, 35].Value = "RON";
				worksheet.Cells[4, 36].Value = "RON";
				worksheet.Cells[4, 37].Value = "RON";
				worksheet.Cells[4, 38].Value = "RON";
				worksheet.Cells[4, 39].Value = "RON";
				worksheet.Cells[4, 40].Value = "RON";
				worksheet.Cells[4, 41].Value = "RON";
				worksheet.Cells[4, 42].Value = "RON";
				worksheet.Cells[4, 43].Value = "RON";
				worksheet.Cells[4, 44].Value = "RON";
				worksheet.Cells[4, 45].Value = "RON";
				worksheet.Cells[4, 46].Value = "RON";
				worksheet.Cells[4, 47].Value = "RON";

				worksheet.Cells[4, 49].Value = "RON";
				worksheet.Cells[4, 50].Value = "RON";
				worksheet.Cells[4, 51].Value = "RON";
				worksheet.Cells[4, 52].Value = "RON";
				worksheet.Cells[4, 53].Value = "RON";
				worksheet.Cells[4, 54].Value = "RON";
				worksheet.Cells[4, 55].Value = "RON";
				worksheet.Cells[4, 56].Value = "RON";
				worksheet.Cells[4, 57].Value = "RON";
				worksheet.Cells[4, 58].Value = "RON";
				worksheet.Cells[4, 59].Value = "RON";
				worksheet.Cells[4, 60].Value = "RON";
				worksheet.Cells[4, 61].Value = "RON";

				worksheet.Cells[4, 63].Value = "RON";
				worksheet.Cells[4, 64].Value = "RON";
				worksheet.Cells[4, 65].Value = "RON";
				worksheet.Cells[4, 66].Value = "RON";
				worksheet.Cells[4, 67].Value = "RON";
				worksheet.Cells[4, 68].Value = "RON";
				worksheet.Cells[4, 69].Value = "RON";
				worksheet.Cells[4, 70].Value = "RON";
				worksheet.Cells[4, 71].Value = "RON";
				worksheet.Cells[4, 72].Value = "RON";
				worksheet.Cells[4, 73].Value = "RON";
				worksheet.Cells[4, 74].Value = "RON";
				worksheet.Cells[4, 75].Value = "RON";


				worksheet.Cells[5, 1].Value = "Budget Owner\r\n(email adress)";
				worksheet.Cells[5, 2].Value = "WBS";
				worksheet.Cells[5, 3].Value = "Tara Name";
				worksheet.Cells[5, 4].Value = "Tara Code";
				worksheet.Cells[5, 5].Value = "Activity";
				worksheet.Cells[5, 6].Value = "Business Unit Name";
				worksheet.Cells[5, 7].Value = "Business Unit Code";
				worksheet.Cells[5, 8].Value = "Profit Center";
				worksheet.Cells[5, 9].Value = "";
				worksheet.Cells[5, 10].Value = "Departament Name";
				worksheet.Cells[5, 11].Value = "Departament Code";
				worksheet.Cells[5, 12].Value = "Project Name";
				worksheet.Cells[5, 13].Value = "Project Code";
				worksheet.Cells[5, 14].Value = "Project Details";
				worksheet.Cells[5, 15].Value = "Cost Type Name";
				worksheet.Cells[5, 16].Value = "Cost Type Code";
				worksheet.Cells[5, 17].Value = "ACQ type";
				worksheet.Cells[5, 18].Value = "Implementation Date (month&year)";
				worksheet.Cells[5, 19].Value = "Dep Per\r\nBGT(month)";
				worksheet.Cells[5, 20].Value = "Dep Per\r\nACT(month)";

                string pastYear = (fiscalYear - 1).ToString();
                string currentYear = (fiscalYear).ToString();

                worksheet.Cells[5, 21].Value = "4/1/" + pastYear;
                worksheet.Cells[5, 22].Value = "5/1/" + pastYear;
                worksheet.Cells[5, 23].Value = "6/1/" + pastYear;
                worksheet.Cells[5, 24].Value = "7/1/" + pastYear;
                worksheet.Cells[5, 25].Value = "8/1/" + pastYear;
                worksheet.Cells[5, 26].Value = "9/1/" + pastYear;
                worksheet.Cells[5, 27].Value = "10/1/" + pastYear;
                worksheet.Cells[5, 28].Value = "11/1/" + pastYear;
                worksheet.Cells[5, 29].Value = "12/1/" + pastYear;
                worksheet.Cells[5, 30].Value = "1/1/" + currentYear;
                worksheet.Cells[5, 31].Value = "2/1/" + currentYear;
                worksheet.Cells[5, 32].Value = "3/1/" + currentYear;
                worksheet.Cells[5, 33].Value = "TY FY22";

                worksheet.Cells[5, 34].Value = "Actuale import (freeze)";
				worksheet.Cells[5, 35].Value = "Valoare ramasa\r\n(Tot. imp. - Act. imp. - Act YTD+YTG)\r\n";
				worksheet.Cells[5, 36].Value = "Procent ramas\r\n";
				worksheet.Cells[5, 37].Value = "Actuale (dupa import) (YTD+YTG)";
				worksheet.Cells[5, 38].Value = "Actuale YTD (dupa import)";
				worksheet.Cells[5, 39].Value = "Actuale YTG (dupa import)";


				// worksheet.Cells[5, 35].Value = "4/1/2021";
				//worksheet.Cells[5, 36].Value = "5/1/2021";
				//worksheet.Cells[5, 37].Value = "6/1/2021";
				//worksheet.Cells[5, 38].Value = "7/1/2021";
				//worksheet.Cells[5, 39].Value = "8/1/2021";
				//worksheet.Cells[5, 40].Value = "9/1/2021";
				//worksheet.Cells[5, 41].Value = "10/1/2021";
				//worksheet.Cells[5, 42].Value = "11/1/2021";
				//worksheet.Cells[5, 43].Value = "12/1/2021";
				//worksheet.Cells[5, 44].Value = "1/1/2022";
				//worksheet.Cells[5, 45].Value = "2/1/2022";
				//worksheet.Cells[5, 46].Value = "3/1/2022";
				//worksheet.Cells[5, 47].Value = "TY FY22";

				//worksheet.Cells[5, 49].Value = "4/1/2021";
				//worksheet.Cells[5, 50].Value = "5/1/2021";
				//worksheet.Cells[5, 51].Value = "6/1/2021";
				//worksheet.Cells[5, 52].Value = "7/1/2021";
				//worksheet.Cells[5, 53].Value = "8/1/2021";
				//worksheet.Cells[5, 54].Value = "9/1/2021";
				//worksheet.Cells[5, 55].Value = "10/1/2021";
				//worksheet.Cells[5, 56].Value = "11/1/2021";
				//worksheet.Cells[5, 57].Value = "12/1/2021";
				//worksheet.Cells[5, 58].Value = "1/1/2022";
				//worksheet.Cells[5, 59].Value = "2/1/2022";
				//worksheet.Cells[5, 60].Value = "3/1/2022";
				//worksheet.Cells[5, 61].Value = "TY FY22";

				//worksheet.Cells[5, 63].Value = "4/1/2021";
				//worksheet.Cells[5, 64].Value = "5/1/2021";
				//worksheet.Cells[5, 65].Value = "6/1/2021";
				//worksheet.Cells[5, 66].Value = "7/1/2021";
				//worksheet.Cells[5, 67].Value = "8/1/2021";
				//worksheet.Cells[5, 68].Value = "9/1/2021";
				//worksheet.Cells[5, 69].Value = "10/1/2021";
				//worksheet.Cells[5, 70].Value = "11/1/2021";
				//worksheet.Cells[5, 71].Value = "12/1/2021";
				//worksheet.Cells[5, 72].Value = "1/1/2022";
				//worksheet.Cells[5, 73].Value = "2/1/2022";
				//worksheet.Cells[5, 74].Value = "3/1/2022";
				//worksheet.Cells[5, 75].Value = "TY FY22";


				int recordIndex = 6;
				int count = items.Count();

				foreach (var item in items)
				{
					rowNumber++;
					rowTCell++;
					int diff = recordIndex - count;

					if (diff > 0)
					{
						diff = 0;
					}


					worksheet.Cells[recordIndex, 1].Value = item.BudgetForecast.BudgetBase.Employee != null ? item.BudgetForecast.BudgetBase.Employee.Email : "";
					worksheet.Cells[recordIndex, 2].Value = item.BudgetForecast.BudgetBase.Project != null ? item.BudgetForecast.BudgetBase.Project.Code : "";
					worksheet.Cells[recordIndex, 3].Value = item.BudgetForecast.BudgetBase.Country != null ? item.BudgetForecast.BudgetBase.Country.Name : "";
					worksheet.Cells[recordIndex, 4].Value = item.BudgetForecast.BudgetBase.Country != null ? item.BudgetForecast.BudgetBase.Country.Code : "";
					worksheet.Cells[recordIndex, 5].Value = item.BudgetForecast.BudgetBase.Activity != null ? item.BudgetForecast.BudgetBase.Activity.Name : "";
					worksheet.Cells[recordIndex, 6].Value = item.BudgetForecast.BudgetBase.Division != null && item.BudgetForecast.BudgetBase.Division.Department != null ? item.BudgetForecast.BudgetBase.Division.Department.Name : "";
					worksheet.Cells[recordIndex, 7].Value = item.BudgetForecast.BudgetBase.Division != null && item.BudgetForecast.BudgetBase.Division.Department != null ? item.BudgetForecast.BudgetBase.Division.Department.Code : "";
					worksheet.Cells[recordIndex, 8].Value = item.BudgetForecast.BudgetBase.AdmCenter != null ? item.BudgetForecast.BudgetBase.AdmCenter.Name : "";
					worksheet.Cells[recordIndex, 9].Value = "";
					worksheet.Cells[recordIndex, 10].Value = item.BudgetForecast.BudgetBase.Division != null ? item.BudgetForecast.BudgetBase.Division.Name : "";
					worksheet.Cells[recordIndex, 11].Value = item.BudgetForecast.BudgetBase.Division != null ? item.BudgetForecast.BudgetBase.Division.Code : "";
					worksheet.Cells[recordIndex, 12].Value = item.BudgetForecast.BudgetBase.ProjectType != null ? item.BudgetForecast.BudgetBase.ProjectType.Name : "";
					worksheet.Cells[recordIndex, 13].Value = item.BudgetForecast.BudgetBase.ProjectType != null ? item.BudgetForecast.BudgetBase.ProjectType.Code : "";
					worksheet.Cells[recordIndex, 14].Value = item.BudgetForecast.BudgetBase.Info;
					worksheet.Cells[recordIndex, 15].Value = item.BudgetForecast.BudgetBase.AssetType != null ? item.BudgetForecast.BudgetBase.AssetType.Name : "";
					worksheet.Cells[recordIndex, 16].Value = item.BudgetForecast.BudgetBase.AssetType != null ? item.BudgetForecast.BudgetBase.AssetType.Code : "";
					worksheet.Cells[recordIndex, 17].Value = item.BudgetForecast.BudgetBase.AppState != null ? item.BudgetForecast.BudgetBase.AppState.Name : "";
					//worksheet.Cells[recordIndex, 17].Style.Numberformat.Format = "yyyy-mm-dd";
					worksheet.Cells[recordIndex, 18].Value = item.BudgetForecast.BudgetBase.StartMonth != null ? item.BudgetForecast.BudgetBase.StartMonth.Month : "";
					//worksheet.Cells[recordIndex, 18].Style.Numberformat.Format = "yyyy-mm-dd";
					worksheet.Cells[recordIndex, 19].Value = item.BudgetForecast.BudgetBase.DepPeriod;
					//worksheet.Cells[recordIndex, 19].Style.Numberformat.Format = "yyyy-mm-dd";
					worksheet.Cells[recordIndex, 20].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Max(a => a.Asset?.DepPeriod);
					//worksheet.Cells[recordIndex, 20].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 21].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.April);
					worksheet.Cells[recordIndex, 21].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 22].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.May);
					worksheet.Cells[recordIndex, 22].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 23].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.June);
					worksheet.Cells[recordIndex, 23].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 24].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.July);
					worksheet.Cells[recordIndex, 24].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 25].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.August);
					worksheet.Cells[recordIndex, 25].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 26].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.September);
					worksheet.Cells[recordIndex, 26].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 27].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.Octomber);
					worksheet.Cells[recordIndex, 27].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 28].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.November);
					worksheet.Cells[recordIndex, 28].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 29].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.December);
					worksheet.Cells[recordIndex, 29].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 30].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.January);
					worksheet.Cells[recordIndex, 30].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 31].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.February);
					worksheet.Cells[recordIndex, 31].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 32].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.March);
					worksheet.Cells[recordIndex, 32].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 34].Value = item.BudgetForecast.ImportValueOrder;
					worksheet.Cells[recordIndex, 34].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 35].Value = item.BudgetForecast.TotalRem;
					worksheet.Cells[recordIndex, 35].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 36].Value = item.BudgetForecast.Total == 0 ? 0 : (item.BudgetForecast.TotalRem/ item.BudgetForecast.Total) * 100;
					worksheet.Cells[recordIndex, 36].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 37].Value = item.BudgetForecast.ValueAsset;
					worksheet.Cells[recordIndex, 37].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 38].Value = item.BudgetForecast.ValueAssetYTD;
					worksheet.Cells[recordIndex, 38].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 39].Value = item.BudgetForecast.ValueAssetYTG;
					worksheet.Cells[recordIndex, 39].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 33].Value = item.BudgetBase.BudgetMonthBase.Where(a => a.IsLast).Select(a => a.Total);
					//worksheet.Cells[recordIndex, 33].Style.Numberformat.Format = "#,##0.00";

					//worksheet.Cells[recordIndex, 35].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.April);
					//worksheet.Cells[recordIndex, 35].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 36].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.May);
					//worksheet.Cells[recordIndex, 36].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 37].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.June);
					//worksheet.Cells[recordIndex, 37].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 38].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.July);
					//worksheet.Cells[recordIndex, 38].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 39].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.August);
					//worksheet.Cells[recordIndex, 39].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 40].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.September);
					worksheet.Cells[recordIndex, 40].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 41].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.Octomber);
					worksheet.Cells[recordIndex, 41].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 42].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.November);
					worksheet.Cells[recordIndex, 42].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 43].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.December);
					worksheet.Cells[recordIndex, 43].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 44].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.January);
					worksheet.Cells[recordIndex, 44].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 45].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.February);
					worksheet.Cells[recordIndex, 45].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 46].Value = item.BudgetForecast.BudgetBase.BudgetBaseAsset.Where(a => a.IsLast && a.IsDeleted == false).Sum(a => a.March);
					worksheet.Cells[recordIndex, 46].Style.Numberformat.Format = "#,##0.00";

					worksheet.Cells[recordIndex, 49].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.April);
					worksheet.Cells[recordIndex, 49].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 50].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.May);
					worksheet.Cells[recordIndex, 50].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 51].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.June);
					worksheet.Cells[recordIndex, 51].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 52].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.July);
					worksheet.Cells[recordIndex, 52].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 53].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.August);
					worksheet.Cells[recordIndex, 53].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 54].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.September);
					worksheet.Cells[recordIndex, 54].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 55].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.Octomber);
					worksheet.Cells[recordIndex, 55].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 56].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.November);
					worksheet.Cells[recordIndex, 56].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 57].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.December);
					worksheet.Cells[recordIndex, 57].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 58].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.January);
					worksheet.Cells[recordIndex, 58].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 59].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.February);
					worksheet.Cells[recordIndex, 59].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 60].Value = item.BudgetForecast.BudgetBase.BudgetForecast.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.March);
					worksheet.Cells[recordIndex, 60].Style.Numberformat.Format = "#,##0.00";



					worksheet.Cells[recordIndex, 63].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.April);
					worksheet.Cells[recordIndex, 63].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 64].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.May);
					worksheet.Cells[recordIndex, 64].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 65].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.June);
					worksheet.Cells[recordIndex, 65].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 66].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.July);
					worksheet.Cells[recordIndex, 66].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 67].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.August);
					worksheet.Cells[recordIndex, 67].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 68].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.September);
					worksheet.Cells[recordIndex, 68].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 69].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.Octomber);
					worksheet.Cells[recordIndex, 69].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 70].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.November);
					worksheet.Cells[recordIndex, 70].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 71].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.December);
					worksheet.Cells[recordIndex, 71].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 72].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.January);
					worksheet.Cells[recordIndex, 72].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 73].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.February);
					worksheet.Cells[recordIndex, 73].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 74].Value = item.BudgetForecast.BudgetBase.BudgetMonthBase.Where(a => a.IsLast && a.IsDeleted == false).Select(a => a.March);
					worksheet.Cells[recordIndex, 74].Style.Numberformat.Format = "#,##0.00";


					worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Formula = "SUM(U" + rowTCell + ":AF" + rowTCell + ")";
					worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Font.Bold = true;
					worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
					worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells["AG" + rowTCell + ":AG" + rowTCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

					worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Formula = "SUM(AI" + rowTCell + ":AT" + rowTCell + ")";
					worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Font.Bold = true;
					worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
					worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells["AU" + rowTCell + ":AU" + rowTCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

					worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Formula = "SUM(AW" + rowTCell + ":BH" + rowTCell + ")";
					worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Font.Bold = true;
					worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
					worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells["BI" + rowTCell + ":BI" + rowTCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

					worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Formula = "SUM(BK" + rowTCell + ":BV" + rowTCell + ")";
					worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Font.Bold = true;
					worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
					worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells["BW" + rowTCell + ":BW" + rowTCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

					if (diff == 0)
					{
						for (int i = 1; i < 76; i++)
						{
							worksheet.Cells[5, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[5, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[5, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[5, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[5, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[5, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[5, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[5, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
							worksheet.Cells[5, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

						}

						worksheet.Row(5).Height = 35.00;
						worksheet.Row(5).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Row(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.View.FreezePanes(6, 1);

						using (var cells = worksheet.Cells[5, 1, items.Count() + 5, 75])
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
							cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

						}

						using (var cells = worksheet.Cells[3, 21, 3, 75])
						{
							cells.Style.Font.Bold = true;
							cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
							cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(198, 224, 180));
							cells.Style.Font.Color.SetColor(Color.Black);
						}

						using (var cells = worksheet.Cells[4, 21, 4, 75])
						{
							cells.Style.Font.Bold = true;
							cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
							cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 204));
							cells.Style.Font.Color.SetColor(Color.Black);
						}

						using (var cells = worksheet.Cells[5, 1, 5, 75])
						{
							cells.Style.Font.Bold = true;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
							cells.Style.Font.Color.SetColor(Color.Black);
						}

						using (var cells = worksheet.Cells[6, 1, items.Count() + 3, 75])
						{
							cells.Style.Font.Bold = false;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
						}

						using (var cells = worksheet.Cells[6, 1, items.Count() + 3, 75])
						{
							for (int i = 6; i < items.Count() + 2; i++)
							{
								worksheet.Row(i).Height = 15.00;
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


							}



						}


						worksheet.View.ShowGridLines = false;
						worksheet.View.ZoomScale = 100;

						for (int i = 1; i < 76; i++)
						{
							worksheet.Column(i).AutoFit();
						}

						worksheet.Column(1).Width = 30.00;
						worksheet.Column(2).Width = 20.00;
						worksheet.Column(3).Width = 15.00;
						worksheet.Column(4).Width = 15.00;
						worksheet.Column(5).Width = 20.00;
						worksheet.Column(6).Width = 25.00;
						worksheet.Column(7).Width = 21.00;
						worksheet.Column(8).Width = 20.00;
						worksheet.Column(9).Width = 0.00;
						worksheet.Column(10).Width = 30.00;
						worksheet.Column(11).Width = 20.00;
						worksheet.Column(12).Width = 35.00;
						worksheet.Column(13).Width = 15.00;
						worksheet.Column(14).Width = 30.00;
						worksheet.Column(15).Width = 30.00;
						worksheet.Column(16).Width = 15.00;
						worksheet.Column(17).Width = 15.00;
						worksheet.Column(18).Width = 25.00;
						worksheet.Column(19).Width = 20.00;
						worksheet.Column(20).Width = 20.00;
						worksheet.Column(21).Width = 15.00;
						worksheet.Column(22).Width = 15.00;
						worksheet.Column(23).Width = 15.00;
						worksheet.Column(24).Width = 15.00;
						worksheet.Column(25).Width = 15.00;
						worksheet.Column(26).Width = 15.00;
						worksheet.Column(27).Width = 15.00;
						worksheet.Column(28).Width = 15.00;
						worksheet.Column(29).Width = 15.00;
						worksheet.Column(30).Width = 15.00;
						worksheet.Column(31).Width = 15.00;
						worksheet.Column(32).Width = 15.00;
						worksheet.Column(33).Width = 15.00;
						worksheet.Column(34).Width = 15.00;
						worksheet.Column(35).Width = 15.00;
						worksheet.Column(36).Width = 15.00;
						worksheet.Column(37).Width = 15.00;
						worksheet.Column(38).Width = 15.00;
						worksheet.Column(39).Width = 15.00;
						worksheet.Column(40).Width = 15.00;
						worksheet.Column(41).Width = 15.00;
						worksheet.Column(42).Width = 15.00;
						worksheet.Column(43).Width = 15.00;
						worksheet.Column(44).Width = 15.00;
						worksheet.Column(45).Width = 15.00;
						worksheet.Column(46).Width = 15.00;
						worksheet.Column(47).Width = 15.00;
						worksheet.Column(48).Width = 15.00;
						worksheet.Column(49).Width = 15.00;
						worksheet.Column(50).Width = 15.00;
						worksheet.Column(51).Width = 15.00;
						worksheet.Column(52).Width = 15.00;
						worksheet.Column(53).Width = 15.00;
						worksheet.Column(54).Width = 15.00;
						worksheet.Column(55).Width = 15.00;
						worksheet.Column(56).Width = 15.00;
						worksheet.Column(57).Width = 15.00;
						worksheet.Column(58).Width = 15.00;
						worksheet.Column(59).Width = 15.00;
						worksheet.Column(60).Width = 15.00;
						worksheet.Column(61).Width = 15.00;
						worksheet.Column(62).Width = 15.00;
						worksheet.Column(63).Width = 15.00;
						worksheet.Column(64).Width = 15.00;
						worksheet.Column(65).Width = 15.00;
						worksheet.Column(66).Width = 15.00;
						worksheet.Column(67).Width = 15.00;
						worksheet.Column(68).Width = 15.00;
						worksheet.Column(69).Width = 15.00;
						worksheet.Column(70).Width = 15.00;
						worksheet.Column(71).Width = 15.00;
						worksheet.Column(72).Width = 15.00;
						worksheet.Column(73).Width = 15.00;
						worksheet.Column(74).Width = 15.00;
						worksheet.Column(75).Width = 15.00;

						//worksheet.Cells["A" + rowCell + ":K" + rowCell].Merge = true;
						//worksheet.Cells["A" + rowCell + ":K" + rowCell].Value = "TOTAL";
						//worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						//worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.Font.Bold = true;
						//worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						//worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						//worksheet.Cells["A" + rowCell + ":K" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));


						worksheet.Cells["U" + rowCell + ":U" + rowCell].Formula = "SUM(U6:U" + rowTotal + ")";
						worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["U" + rowCell + ":U" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["V" + rowCell + ":V" + rowCell].Formula = "SUM(V6:V" + rowTotal + ")";
						worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["W" + rowCell + ":W" + rowCell].Formula = "SUM(W6:W" + rowTotal + ")";
						worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["X" + rowCell + ":X" + rowCell].Formula = "SUM(X6:X" + rowTotal + ")";
						worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Formula = "SUM(Y6:Y" + rowTotal + ")";
						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Formula = "SUM(Z6:Z" + rowTotal + ")";
						worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Formula = "SUM(AA6:AA" + rowTotal + ")";
						worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Formula = "SUM(AB6:AB" + rowTotal + ")";
						worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Formula = "SUM(AC6:AC" + rowTotal + ")";
						worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Formula = "SUM(AD6:AD" + rowTotal + ")";
						worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Formula = "SUM(AE6:AE" + rowTotal + ")";
						worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Formula = "SUM(AF6:AF" + rowTotal + ")";
						worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Formula = "SUM(AG6:AG" + rowTotal + ")";
						worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49));

						worksheet.Cells["U6" + ":U" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["V6" + ":V" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["W6" + ":W" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["X6" + ":X" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["Y6" + ":Y" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["Z6" + ":Z" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AA6" + ":AA" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AB6" + ":AB" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AC6" + ":AC" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AD6" + ":AD" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AE6" + ":AE" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AF6" + ":AF" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AG6" + ":AG" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(237, 125, 49));


						// 2 //

						worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Formula = "SUM(AI6:AI" + rowTotal + ")";
						worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Formula = "SUM(AJ6:AJ" + rowTotal + ")";
						worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Formula = "SUM(AK6:AK" + rowTotal + ")";
						worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Formula = "SUM(AL6:AL" + rowTotal + ")";
						worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AL" + rowCell + ":AL" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Formula = "SUM(AM6:AM" + rowTotal + ")";
						worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AM" + rowCell + ":AM" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Formula = "SUM(AN6:AN" + rowTotal + ")";
						worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AN" + rowCell + ":AN" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Formula = "SUM(AO6:AO" + rowTotal + ")";
						worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AO" + rowCell + ":AO" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Formula = "SUM(AP6:AP" + rowTotal + ")";
						worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AP" + rowCell + ":AP" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Formula = "SUM(AQ6:AQ" + rowTotal + ")";
						worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AQ" + rowCell + ":AQ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Formula = "SUM(AR6:AR" + rowTotal + ")";
						worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AR" + rowCell + ":AR" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Formula = "SUM(AS6:AS" + rowTotal + ")";
						worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AS" + rowCell + ":AS" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Formula = "SUM(AT6:AT" + rowTotal + ")";
						worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AT" + rowCell + ":AT" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Formula = "SUM(AU6:AU" + rowTotal + ")";
						worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AU" + rowCell + ":AU" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49));

						worksheet.Cells["AI6" + ":AI" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AJ6" + ":AJ" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AK6" + ":AK" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AL6" + ":AL" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AM6" + ":AM" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AN6" + ":AN" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AO6" + ":AO" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AP6" + ":AP" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AQ6" + ":AQ" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AR6" + ":AR" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AS6" + ":AS" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AT6" + ":AT" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AU6" + ":AU" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(237, 125, 49));

						// 2 //

						// 3 //

						worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Formula = "SUM(AW6:AW" + rowTotal + ")";
						worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AW" + rowCell + ":AW" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Formula = "SUM(AX6:AX" + rowTotal + ")";
						worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AX" + rowCell + ":AX" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Formula = "SUM(AY6:AY" + rowTotal + ")";
						worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AY" + rowCell + ":AY" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Formula = "SUM(AZ6:AZ" + rowTotal + ")";
						worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["AZ" + rowCell + ":AZ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Formula = "SUM(BA6:BA" + rowTotal + ")";
						worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BA" + rowCell + ":BA" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Formula = "SUM(BB6:BB" + rowTotal + ")";
						worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BB" + rowCell + ":BB" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Formula = "SUM(BC6:BC" + rowTotal + ")";
						worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BC" + rowCell + ":BC" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Formula = "SUM(BD6:BD" + rowTotal + ")";
						worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BD" + rowCell + ":BD" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Formula = "SUM(BE6:BE" + rowTotal + ")";
						worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BE" + rowCell + ":BE" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Formula = "SUM(BF6:BF" + rowTotal + ")";
						worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BF" + rowCell + ":BF" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Formula = "SUM(BG6:BG" + rowTotal + ")";
						worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BG" + rowCell + ":BG" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Formula = "SUM(BH6:BH" + rowTotal + ")";
						worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BH" + rowCell + ":BH" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Formula = "SUM(BI6:BI" + rowTotal + ")";
						worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BI" + rowCell + ":BI" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49));

						worksheet.Cells["AW6" + ":AW" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AX6" + ":AX" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AY6" + ":AY" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["AZ6" + ":AZ" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BA6" + ":BA" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BB6" + ":BB" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BC6" + ":BC" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BD6" + ":BD" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BE6" + ":BE" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BF6" + ":BF" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BG6" + ":BG" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BH6" + ":BH" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BI6" + ":BI" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(237, 125, 49));

						// 3 //

						// 4 //

						worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Formula = "SUM(BK6:BK" + rowTotal + ")";
						worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BK" + rowCell + ":BK" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Formula = "SUM(BL6:BL" + rowTotal + ")";
						worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BL" + rowCell + ":BL" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Formula = "SUM(BM6:BM" + rowTotal + ")";
						worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BM" + rowCell + ":BM" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Formula = "SUM(BN6:BN" + rowTotal + ")";
						worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BN" + rowCell + ":BN" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Formula = "SUM(BO6:BO" + rowTotal + ")";
						worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BO" + rowCell + ":BO" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Formula = "SUM(BP6:BP" + rowTotal + ")";
						worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BP" + rowCell + ":BP" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Formula = "SUM(BQ6:BQ" + rowTotal + ")";
						worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BQ" + rowCell + ":BQ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Formula = "SUM(BR6:BR" + rowTotal + ")";
						worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BR" + rowCell + ":BR" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Formula = "SUM(BS6:BS" + rowTotal + ")";
						worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BS" + rowCell + ":BS" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Formula = "SUM(BT6:BT" + rowTotal + ")";
						worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BT" + rowCell + ":BT" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Formula = "SUM(BU6:BU" + rowTotal + ")";
						worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BU" + rowCell + ":BU" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Formula = "SUM(BV6:BV" + rowTotal + ")";
						worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BV" + rowCell + ":BV" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 153));

						worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Formula = "SUM(BW6:BW" + rowTotal + ")";
						worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["BW" + rowCell + ":BW" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237, 125, 49));

						worksheet.Cells["BK6" + ":BK" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BL6" + ":BL" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BM6" + ":BM" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BN6" + ":BN" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BO6" + ":BO" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BP6" + ":BP" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BQ6" + ":BQ" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BR6" + ":BR" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BS6" + ":BS" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BT6" + ":BT" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BU6" + ":BU" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BV6" + ":BV" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(68, 114, 196));
						worksheet.Cells["BW6" + ":BW" + rowTotal].Style.Font.Color.SetColor(Color.FromArgb(237, 125, 49));

						// 4 //

						//package.Workbook.CalcMode = ExcelCalcMode.Manual;

						package.Workbook.Calculate();

						worksheet.Cells["A5:AG5"].AutoFilter = true;

					}
					recordIndex++;
				}

				using (var cells = worksheet.Cells[5, 1, 5, 75])
				{
					cells.Style.Font.Bold = true;
					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
					cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

				}

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				//HttpContext.Response.ContentType = entityFile.FileType;
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "Export.xlsx"
				};

				return result;

			}
		}

		[HttpPost("validation")]
		public async virtual Task<BudgetForecastCorrectionResult> CorrectionValidate([FromBody] BudgetCorrectionValidate budget)
		{

			//var result1 = await NeedBudgetResponse(549);
			Model.EntityType entityTypeNB = null;
			Model.EntityType entityType = null;
			Model.AppState appStateNB = null;
			Model.EmailType emailType = null;
			Model.EmailRequestStatus emailRequestStatus = null;
			Model.RequestBudgetForecast requestBudgetForecastSrc = null;
			Model.RequestBudgetForecast requestBudgetForecastDst = null;
			Model.Employee employee = null;

			var userName = HttpContext.User.Identity.Name;

			if (userName != null && userName != "")
			{
				var user = await userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}

				if (user != null)
				{
					budget.UserId = user.Id;

					employee = await _context.Set<Model.Employee>().Where(a => a.Id == user.EmployeeId).FirstOrDefaultAsync();
					if (employee == null) return new BudgetForecastCorrectionResult { Success = false , Message = "Userul nu are mapat niciun angajat!" };

					var res = await (_itemsRepository as IBudgetForecastsRepository).CorrectionValidateBudgetForecast(budget);

					if (res.Success)
					{
						var countBudgetBase = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
						var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();

						var resultNotification = await this._requestsService.SendRequestResponseTransferBudget(budget.BudgetBaseOpId, employee.Email);

						if (resultNotification)
						{
							// ORDER //

							Model.AppState appState = null;
							Model.Document document = null;
							Model.DocumentType documentType = null;
							Model.Order order = null;
							Model.BudgetBaseOp budgetBaseOp = null;
							Model.OrderOp orderOp = null;

							string documentTypeCode = "VALIDATE_ORDER_LEVEL4";
							appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").SingleAsync();
							documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).SingleAsync();
							budgetBaseOp = await _context.Set<Model.BudgetBaseOp>().Where(d => d.Id == budget.BudgetBaseOpId).SingleAsync();

							if(budgetBaseOp.OrderId != null)
							{
								document = new Model.Document
								{
									Approved = true,
									DocumentType = documentType,
									DocNo1 = string.Empty,
									DocNo2 = string.Empty,
									DocumentDate = DateTime.Now,
									RegisterDate = DateTime.Now,
									Partner = null
								};

								_context.Add(document);

								order = await _context.Set<Model.Order>().Include(o => o.OrderMaterials).Include(a => a.OrderType).Where(a => a.Id == budgetBaseOp.OrderId).SingleOrDefaultAsync();

								Model.Offer offer = await _context.Set<Model.Offer>().Where(c => c.Id == order.OfferId).SingleAsync();
								decimal valueRon = order.OrderMaterials.Sum(a => a.ValueRon);

								var result = await GetMatrixData(offer.DivisionId.Value, valueRon);

								if (result.Count > 0)
								{

									order.EmployeeL4Id = result.ElementAt(0).EmployeeL4.Validate ? result.ElementAt(0).EmployeeL4.Id : null;
									order.EmployeeL3Id = result.ElementAt(0).EmployeeL3.Validate ? result.ElementAt(0).EmployeeL3.Id : null;
									order.EmployeeL2Id = result.ElementAt(0).EmployeeL2.Validate ? result.ElementAt(0).EmployeeL2.Id : null;
									order.EmployeeL1Id = result.ElementAt(0).EmployeeL1.Validate ? result.ElementAt(0).EmployeeL1.Id : null;
									order.EmployeeS3Id = result.ElementAt(0).EmployeeS3.Validate ? result.ElementAt(0).EmployeeS3.Id : null;
									order.EmployeeS2Id = result.ElementAt(0).EmployeeS2.Validate ? result.ElementAt(0).EmployeeS2.Id : null;
									order.EmployeeS1Id = result.ElementAt(0).EmployeeS1.Validate ? result.ElementAt(0).EmployeeS1.Id : null;


									order.AppStateId = appState.Id;
									order.IsAccepted = true;

									orderOp = new Model.OrderOp()
									{
										AccMonthId = order.AccMonthId,
										AccSystemId = null,
										AccountIdInitial = order.AccountId,
										AccountIdFinal = order.AccountId,
										AdministrationIdInitial = order.AdministrationId,
										AdministrationIdFinal = order.AdministrationId,
										Order = order,
										BudgetManagerIdInitial = null,
										BudgetManagerIdFinal = null,
										BudgetStateId = appState.Id,
										CompanyIdInitial = order.CompanyId,
										CompanyIdFinal = order.CompanyId,
										CostCenterIdInitial = order.CostCenterId,
										CostCenterIdFinal = order.CostCenterId,
										CreatedAt = DateTime.Now,
										CreatedBy = order.UserId,
										Document = document,
										DstConfAt = DateTime.Now,
										DstConfBy = order.UserId,
										EmployeeIdInitial = order.EmployeeId,
										EmployeeIdFinal = order.EmployeeId,
										InfoIni = order.Info,
										InfoFin = order.Info,
										InterCompanyIdInitial = order.InterCompanyId,
										InterCompanyIdFinal = order.InterCompanyId,
										IsAccepted = false,
										IsDeleted = false,
										ModifiedAt = DateTime.Now,
										ModifiedBy = order.UserId,
										PartnerIdInitial = order.PartnerId,
										PartnerIdFinal = order.PartnerId,
										ProjectIdInitial = order.ProjectId,
										ProjectIdFinal = order.ProjectId,
										QuantityIni = order.Quantity,
										QuantityFin = order.Quantity,
										SubTypeIdInitial = order.SubTypeId,
										SubTypeIdFinal = order.SubTypeId,
										Validated = true,
										ValueFin1 = order.ValueIni,
										ValueIni1 = order.ValueIni,
										ValueFin2 = order.ValueIni,
										ValueIni2 = order.ValueIni,
										Guid = Guid.NewGuid(),
										BudgetIdInitial = order.BudgetId,
										BudgetIdFinal = order.BudgetId,
										OfferIdInitial = order.OfferId,
										OfferIdFinal = order.OfferId,
										UomId = order.UomId
									};

									_context.Add(orderOp);

									emailType = _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL4").Single();
									entityType = _context.Set<Model.EntityType>().Where(c => c.Code == "ORDER").Single();


									Model.EmailOrderStatus emailOrderStatus = new Model.EmailOrderStatus()
									{
										AppStateId = appState.Id,
										Completed = false,
										CreatedAt = DateTime.Now,
										CreatedBy = user.Id,
										DocumentNumber = int.Parse(entityType.Name),
										EmailSend = false,
										EmailTypeId = emailType.Id,
										EmployeeL1EmailSend = false,
										EmployeeL1ValidateAt = null,
										EmployeeL1ValidateBy = null,
										EmployeeL2EmailSend = false,
										EmployeeL2ValidateAt = null,
										EmployeeL2ValidateBy = null,
										EmployeeL3EmailSend = false,
										EmployeeL3ValidateAt = null,
										EmployeeL3ValidateBy = null,
										EmployeeL4EmailSend = false,
										EmployeeL4ValidateAt = null,
										EmployeeL4ValidateBy = null,
										EmployeeS1EmailSend = false,
										EmployeeS1ValidateAt = null,
										EmployeeS1ValidateBy = null,
										EmployeeS2EmailSend = false,
										EmployeeS2ValidateAt = null,
										EmployeeS2ValidateBy = null,
										EmployeeS3EmailSend = false,
										EmployeeS3ValidateAt = null,
										EmployeeS3ValidateBy = null,
										ErrorId = null,
										Exported = false,
										FinalValidateAt = null,
										FinalValidateBy = null,
										Guid = Guid.NewGuid(),
										GuidAll = Guid.NewGuid(),
										Info = string.Empty,
										IsAccepted = false,
										IsDeleted = false,
										MatrixId = result[0].Id,
										ModifiedAt = DateTime.Now,
										ModifiedBy = user.Id,
										NotCompletedSync = false,
										NotEmployeeL1Sync = false,
										NotEmployeeL2Sync = false,
										NotEmployeeL3Sync = false,
										NotEmployeeL4Sync = true,
										NotEmployeeS1Sync = false,
										NotEmployeeS2Sync = false,
										NotEmployeeS3Sync = false,
										NotSync = false,
										OrderId = order.Id,
										SyncCompletedErrorCount = 0,
										SyncEmployeeL1ErrorCount = 0,
										SyncEmployeeL2ErrorCount = 0,
										SyncEmployeeL3ErrorCount = 0,
										SyncEmployeeL4ErrorCount = 0,
										SyncEmployeeS1ErrorCount = 0,
										SyncEmployeeS2ErrorCount = 0,
										SyncEmployeeS3ErrorCount = 0,
										SyncErrorCount = 0,
										EmployeeL1EmailSkip = !result.ElementAt(0).EmployeeL1.Validate,
										EmployeeL2EmailSkip = !result.ElementAt(0).EmployeeL2.Validate,
										EmployeeL3EmailSkip = !result.ElementAt(0).EmployeeL3.Validate,
										EmployeeL4EmailSkip = !result.ElementAt(0).EmployeeL4.Validate,
										EmployeeS1EmailSkip = !result.ElementAt(0).EmployeeS1.Validate,
										EmployeeS2EmailSkip = !result.ElementAt(0).EmployeeS2.Validate,
										EmployeeS3EmailSkip = !result.ElementAt(0).EmployeeS3.Validate,
									};

									_context.Add(emailOrderStatus);
									_context.SaveChanges();
								}
							}
							// ORDER //
						}
					}

					//if (res.Success && res.SourceId > 0 && res.DestinationId > 0)
					//{

					//	requestBudgetForecastSrc = await _context.Set<Model.RequestBudgetForecast>().Where(a => a.Id == res.SourceId).SingleAsync();
					//	requestBudgetForecastDst = await _context.Set<Model.RequestBudgetForecast>().Where(a => a.Id == res.DestinationId).SingleAsync();

					//	emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "NEW_ORDER").FirstOrDefaultAsync();
					//	entityTypeNB = await _context.Set<Model.EntityType>().Where(c => c.Code == "ORDER").FirstOrDefaultAsync();
					//	appStateNB = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();

					//	emailRequestStatus = new Model.EmailRequestStatus()
					//	{
					//		AppStateId = appStateNB.Id,
					//		Completed = false,
					//		CreatedAt = DateTime.Now,
					//		CreatedBy = _context.UserId,
					//		DocumentNumber = int.Parse(entityTypeNB.Name),
					//		EmailSend = false,
					//		EmailTypeId = emailType.Id,
					//		EmployeeL1EmailSend = false,
					//		EmployeeL1ValidateAt = null,
					//		EmployeeL1ValidateBy = null,
					//		EmployeeL2EmailSend = false,
					//		EmployeeL2ValidateAt = null,
					//		EmployeeL2ValidateBy = null,
					//		EmployeeL3EmailSend = false,
					//		EmployeeL3ValidateAt = null,
					//		EmployeeL3ValidateBy = null,
					//		EmployeeL4EmailSend = false,
					//		EmployeeL4ValidateAt = null,
					//		EmployeeL4ValidateBy = null,
					//		EmployeeS1EmailSend = false,
					//		EmployeeS1ValidateAt = null,
					//		EmployeeS1ValidateBy = null,
					//		EmployeeS2EmailSend = false,
					//		EmployeeS2ValidateAt = null,
					//		EmployeeS2ValidateBy = null,
					//		EmployeeS3EmailSend = false,
					//		EmployeeS3ValidateAt = null,
					//		EmployeeS3ValidateBy = null,
					//		ErrorId = null,
					//		Exported = false,
					//		FinalValidateAt = null,
					//		FinalValidateBy = null,
					//		Guid = Guid.NewGuid(),
					//		GuidAll = Guid.NewGuid(),
					//		Info = string.Empty,
					//		IsAccepted = false,
					//		IsDeleted = false,
					//		ModifiedAt = DateTime.Now,
					//		ModifiedBy = _context.UserId,
					//		NotCompletedSync = false,
					//		NotEmployeeL1Sync = false,
					//		NotEmployeeL2Sync = false,
					//		NotEmployeeL3Sync = false,
					//		NotEmployeeL4Sync = true,
					//		NotEmployeeS1Sync = false,
					//		NotEmployeeS2Sync = false,
					//		NotEmployeeS3Sync = false,
					//		NotSync = false,
					//		RequestId = requestBudgetForecast.RequestId.Value,
					//		RequestBudgetForecastId = requestBudgetForecast.Id,
					//		SyncCompletedErrorCount = 0,
					//		SyncEmployeeL1ErrorCount = 0,
					//		SyncEmployeeL2ErrorCount = 0,
					//		SyncEmployeeL3ErrorCount = 0,
					//		SyncEmployeeL4ErrorCount = 0,
					//		SyncEmployeeS1ErrorCount = 0,
					//		SyncEmployeeS2ErrorCount = 0,
					//		SyncEmployeeS3ErrorCount = 0,
					//		SyncErrorCount = 0,
					//		EmployeeL1EmailSkip = true,
					//		EmployeeL2EmailSkip = true,
					//		EmployeeL3EmailSkip = true,
					//		EmployeeL4EmailSkip = false,
					//		EmployeeS1EmailSkip = true,
					//		EmployeeS2EmailSkip = true,
					//		EmployeeS3EmailSkip = true,
					//		NeedBudgetEmailSend = false,
					//		NotNeedBudgetSync = false,
					//	};

					//	_context.Add(emailRequestStatus);
					//	entityTypeNB.Name = (int.Parse(entityTypeNB.Name) + 1).ToString();
					//	_context.Update(entityTypeNB);


					//	requestBudgetForecast.AppStateId = 15;
					//	requestBudgetForecast.ModifiedAt = DateTime.Now;
					//	requestBudgetForecast.NeedBudget = false;

					//	_context.Update(requestBudgetForecast);
					//	_context.SaveChanges();

					//	return new Model.RequestResult { Success = true, Message = $"Bugetul a fost actualizat cu succes!" };

					//	//var result = await NeedBudgetResponse(requestBudgetForecast.Id);

					//	//if (result)
					//	//                     {
					//	//                         requestBudgetForecast.AppStateId = 15;
					//	//                         requestBudgetForecast.ModifiedAt = DateTime.Now;
					//	//                         requestBudgetForecast.NeedBudget = false;

					//	//                         _context.Update(requestBudgetForecast);
					//	//                         _context.SaveChanges();

					//	//                         return new Model.RequestResult { Success = true, Message = $"Bugetul a fost creat cu succes!" };
					//	//                     }
					//	//                     else
					//	//                     {
					//	//                         return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
					//	//                     }

					//}
					//else
					//{
					//	return new Model.BudgetForecastCorrectionResult { Success = res.Success, Message = res.Message };
					//}

					return new Model.BudgetForecastCorrectionResult { Success = true, Message = $"Bugetele au fost actualizate cu succes!" };
				}
				else
				{
					return new Model.BudgetForecastCorrectionResult { Success = false, Message = $"Va rugam sa va autentificati!" };
				}

			}
			else
			{
				return new Model.BudgetForecastCorrectionResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}
		}

		public virtual async Task<List<Dto.Matrix>> GetMatrixData(int divisionId, decimal value)
		{
			var items = await _matrixRepository.GetMatchMatrixAsync(divisionId);

			var result = items.Select(i => _mapper.Map<Dto.Matrix>(i)).ToList();

			if (result.Count > 0)
			{
				result.ElementAt(0).EmployeeL4.Validate = true;

				if (value >= result.ElementAt(0).AmountL3)
				{
					result.ElementAt(0).EmployeeL3.Validate = true;
				}

				if (value >= result.ElementAt(0).AmountL2)
				{
					result.ElementAt(0).EmployeeL2.Validate = true;
				}

				if (value >= result.ElementAt(0).AmountL1)
				{
					result.ElementAt(0).EmployeeL1.Validate = true;
				}

				if (value >= result.ElementAt(0).AmountS3)
				{
					result.ElementAt(0).EmployeeS3.Validate = true;
				}

				if (value >= result.ElementAt(0).AmountS2)
				{
					result.ElementAt(0).EmployeeS2.Validate = true;
				}

				if (value >= result.ElementAt(0).AmountS1)
				{
					result.ElementAt(0).EmployeeS1.Validate = true;
				}
			}


			return result;
		}
	}
}
