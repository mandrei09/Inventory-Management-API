using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optima.Fais.Api.Helpers;
using Optima.Fais.Api.Services;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.EfRepository;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/requests")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class RequestsController : GenericApiController<Model.Request, Dto.Request>
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<Model.ApplicationUser> userManager;
		private readonly IRequestsService _requestsService;

		public RequestsController(ApplicationDbContext context,
            IRequestsRepository itemsRepository, IMapper mapper, IWebHostEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager, IRequestsService requestsService)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
			this._requestsService = requestsService;
		}
        
        [HttpGet]
        [Route("", Order = -1)]
        public virtual async Task<IActionResult> GetDepDetails(int page, int pageSize, string sortColumn, string sortDirection,
            string includes, string jsonFilter, string columnFilter, string filter, string budgetForecastIds, string requestIds, bool newBudget = false, bool needBudget = false)
        {
          AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            RequestFilter requestFilter = null;
			ColumnRequestFilter colRequestFilters = null;
			Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string role = string.Empty;
			List<int?> bfIds = null;
			List<int?> reqIds = null;

			if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            requestFilter = jsonFilter != null ? JsonConvert.DeserializeObject<RequestFilter>(jsonFilter) : new RequestFilter();
			colRequestFilters = columnFilter != null ? JsonConvert.DeserializeObject<ColumnRequestFilter>(columnFilter) : new ColumnRequestFilter();

			if (filter != null)
            {
                requestFilter.Filter = filter;
            }


            if (HttpContext.User.Identity.Name != null)
            {
                var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                }
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                requestFilter.Role = role;
                requestFilter.InInventory = user.InInventory;
                requestFilter.UserId = user.Id;

                if (employeeId != null)
                {
                    requestFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    requestFilter.EmployeeIds = null;
                    requestFilter.EmployeeIds = new List<int?>();
                    requestFilter.EmployeeIds.Add(int.Parse("-1"));
                }
            }
            else
            {
                requestFilter.EmployeeIds = null;
                requestFilter.EmployeeIds = new List<int?>();
                requestFilter.EmployeeIds.Add(int.Parse("-1"));
            }

			if (budgetForecastIds != null && !budgetForecastIds.StartsWith("["))
			{
				budgetForecastIds = "[" + budgetForecastIds + "]";
			}

			if (requestIds != null && !requestIds.StartsWith("["))
			{
				requestIds = "[" + requestIds + "]";
			}

			if ((budgetForecastIds != null) && (budgetForecastIds.Length > 0)) bfIds = JsonConvert.DeserializeObject<string[]>(budgetForecastIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((requestIds != null) && (requestIds.Length > 0)) reqIds = JsonConvert.DeserializeObject<string[]>(requestIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

			var items = (_itemsRepository as IRequestsRepository)
                .GetRequest(requestFilter, colRequestFilters, includes, sorting, paging, bfIds, reqIds, newBudget, needBudget, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.RequestDetail>, List<Dto.Request>>(items);



            var result = new RequestPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }

        [HttpGet]
        [Route("detailui", Order = -1)]
        public virtual async Task<IActionResult> GetDepDetailUIs(int page, int pageSize, string sortColumn, string sortDirection,
           string includes, string jsonFilter, string filter, bool showExisting = false)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            RequestFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            includes ??= "Request.AppState,Request.Company,Request.CostCenter,Request.AccMonth,Request.Employee,";
            //includes = includes + "Request.BudgetBase.AdmCenter,Request.BudgetBase.Region,Request.BudgetBase.AssetType,Request.BudgetBase.Company,Request.BudgetBase.Project,";
            includes = includes + "Request.BudgetForecast.BudgetBase.AdmCenter,Request.BudgetForecast.BudgetBase.Region,Request.BudgetForecast.BudgetBase.AssetType,Request.BudgetForecast.BudgetBase.Company,Request.BudgetForecast.BudgetBase.Project,Request.Employee";

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<RequestFilter>(jsonFilter) : new RequestFilter();


            if (filter != null)
            {
                assetFilter.Filter = filter;
            }

            var items = (_itemsRepository as IRequestsRepository)
                .GetRequestUI(assetFilter, includes, sorting, paging, showExisting, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.RequestDetail>, List<Dto.RequestUI>>(items);

            var result = new RequestUIPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }

        [HttpGet]
        [Route("detailneedbudgetui", Order = -1)]
        public virtual async Task<IActionResult> GetDepDetailBudgetUIs(int page, int pageSize, string sortColumn, string sortDirection,
          string includes, string jsonFilter, string filter, bool showExisting = false)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            RequestFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            includes ??= "Request.AppState,Request.Budget,Request.Company,Request.CostCenter.Division.Department,Request.AccMonth,Request.Employee,";
            includes = includes + "Request.Budget.AdmCenter,Request.Budget.Region,Request.Budget.AssetType,Request.Budget.Company,Request.Budget.Project,";
            includes = includes + "Request.BudgetBase.AdmCenter,Request.BudgetBase.Region,Request.BudgetBase.AssetType,Request.BudgetBase.Company,Request.BudgetBase.Project,Request.Employee,Request.AssetType,Request.Division.Department,Request.ProjectType,Request.StartAccMonth";

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<RequestFilter>(jsonFilter) : new RequestFilter();


            if (filter != null)
            {
                assetFilter.Filter = filter;
            }

            var items = (_itemsRepository as IRequestsRepository)
                .GetRequestBudgetUI(assetFilter, includes, sorting, paging, showExisting, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.RequestDetail>, List<Dto.RequestUI>>(items);

            var result = new RequestUIPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }

        [HttpPost("detail")]
        public async Task<RequestResult> PostDetail([FromBody] RequestSave requestDto)
        {
            Model.RequestResult requestResult = null;
            Model.EmailRequestStatus emailRequestStatus = null;
            Model.EmailType emailType = null;
			Model.EntityType entityType = null;
			Model.AppState appState = null;
			Model.Request request = null;
            Model.Employee employee = null;
			Model.Employee employeeOffer = null;

			if (HttpContext.User.Identity.Name != null)
			{
                var userName = HttpContext.User.Identity.Name;
                //var userName = "daniela.sandu";
                var user = await userManager.FindByEmailAsync(userName);

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }

                if (user != null)
				{
                    _context.UserId = user.Id.ToString();
                    requestDto.UserId = user.Id;

					try
					{
                        requestResult = await (_itemsRepository as IRequestsRepository).CreateOrUpdateRequest(requestDto);

                        if (requestResult.Success)
                        {
							emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "NEW_REQUEST").FirstOrDefaultAsync();
							if (emailType == null) return new Model.RequestResult { Success = false, Message = "Lipsa tip email!", RequestId = 0 };
							entityType = await _context.Set<Model.EntityType>().Where(c => c.Code == "NEW_REQUEST").FirstOrDefaultAsync();
							if (entityType == null) return new Model.RequestResult { Success = false, Message = "Lipsa tip entitate!", RequestId = 0 };
							appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "NEW").FirstOrDefaultAsync();
							if (appState == null) return new Model.RequestResult { Success = false, Message = "Lipsa stare!", RequestId = 0 };

							//employee = await _context.Set<Model.Employee>().AsNoTracking().Where(c => c.Email == request.EmployeeId.ToString()).FirstOrDefaultAsync();
							//if (employee == null) return new Model.RequestResult { Success = false, Message = "Lipsa responsabil!", RequestId = 0 };

							//employeeOffer = await _context.Set<Model.Employee>().AsNoTracking().Where(c => c.Email == "bogdan.pirvulescu@emag.ro").FirstOrDefaultAsync();
							//if (employeeOffer == null) return new Model.RequestResult { Success = false, Message = "Lipsa notificare oferta!", RequestId = 0 };

							request = await _context.Set<Model.Request>().Where(c => c.Id == requestResult.RequestId).FirstOrDefaultAsync();
							if (request == null) return new Model.RequestResult { Success = false, Message = "Lipsa P.R.", RequestId = 0 };

							emailRequestStatus = new Model.EmailRequestStatus()
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
								RequestId = requestResult.RequestId,
								SyncCompletedErrorCount = 0,
								SyncEmployeeL1ErrorCount = 0,
								SyncEmployeeL2ErrorCount = 0,
								SyncEmployeeL3ErrorCount = 0,
								SyncEmployeeL4ErrorCount = 0,
								SyncEmployeeS1ErrorCount = 0,
								SyncEmployeeS2ErrorCount = 0,
								SyncEmployeeS3ErrorCount = 0,
								SyncErrorCount = 0,
							};

							_context.Add(emailRequestStatus);
							_context.SaveChanges();

							return new Model.RequestResult { Success = true, Message = requestResult.Message, RequestId = requestResult.RequestId };
                        }
                        else
                        {
                            return new Model.RequestResult { Success = false, Message = requestResult.Message, RequestId = 0 };
                        }
                    }
					catch (Exception ex)
					{

                        return new Model.RequestResult { Success = false, Message = ex.Message, RequestId = 0 };
                    }

                }
                else
                {
                    return new Model.RequestResult { Success = false, Message = $"Userul nu exista!", RequestId = 0 };
                }


            }
			else
			{
                return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!", RequestId = 0 };
            }

           
        }

		[HttpPost("update")]
		public async Task<RequestResult> UpdateRequest([FromBody] RequestUpdate requestDto)
		{
			Model.RequestResult requestResult = null;

			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);

				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}

				if (user != null)
				{
					_context.UserId = user.Id.ToString();
					requestDto.UserId = user.Id;

					try
					{
						requestResult = await (_itemsRepository as IRequestsRepository).UpdateRequest(requestDto);

						if (requestResult.Success)
						{
							return new Model.RequestResult { Success = true, Message = requestResult.Message, RequestId = requestResult.RequestId };
						}
						else
						{
							return new Model.RequestResult { Success = false, Message = requestResult.Message, RequestId = 0 };
						}
					}
					catch (Exception ex)
					{

						return new Model.RequestResult { Success = false, Message = ex.Message, RequestId = 0 };
					}

				}
				else
				{
					return new Model.RequestResult { Success = false, Message = $"Userul nu exista!", RequestId = 0 };
				}


			}
			else
			{
				return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!", RequestId = 0 };
			}


		}

		[HttpGet("detail/{id:int}")]
        public virtual IActionResult GetDetail(int id, string includes)
        {
            var asset = (_itemsRepository as IRequestsRepository).GetDetailsById(id, includes);
            var result = _mapper.Map<Dto.Request>(asset);

            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("validate")]
        public async Task<IActionResult> requestValidate1([FromBody] int[] requests)
        {
            await Task.Delay(0);
            Model.Request request = null;
            Model.Request userAllocation = null;
            Model.RequestOp requestOp = null;
            Model.Document document = null;
            Model.AppState appsState = await _context.Set<Model.AppState>().Where(a => a.Code == "ALLOCATED_REQUEST").FirstOrDefaultAsync();
            Model.ApplicationUser userAllocationResponsable = null;

            string documentTypeCode = "VALIDATE_REQUEST";
            var documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();
            
            var userEmail = HttpContext.User.Identity.Name;
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                user = await userManager.FindByNameAsync(userEmail);
            }

            _context.UserId = user.Id.ToString();

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

            for (int i = 0; i < requests.Length; i++)
            {
                request = _context.Set<Model.Request>().Where(a => a.Id == requests[i]).SingleOrDefault();
              
                request.UserId = user.Id;
                userAllocationResponsable = _context.Set<Model.ApplicationUser>().Where(a => a.Id == request.UserId).SingleOrDefault();

                request.AppStateId = appsState.Id;
                request.IsAccepted = true;
                request.Validated = true;
                request.EmployeeId = userAllocationResponsable.EmployeeId.GetValueOrDefault();
                request.ModifiedBy = userAllocationResponsable.Id;

                requestOp = new Model.RequestOp()
                {
                    AccMonthId = request.AccMonthId,
                    AccSystemId = null,
                    Request = request,
                    RequestStateId = appsState.Id,
                    CompanyId = request.CompanyId,
                    CostCenterIdInitial = request.CostCenterId,
                    CostCenterIdFinal = request.CostCenterId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = request.UserId,
                    Document = document,
                    DstConfAt = DateTime.Now,
                    DstConfBy = request.UserId,
                    EmployeeIdInitial = request.EmployeeId,
                    EmployeeIdFinal = request.EmployeeId,
                    InfoIni = request.Info,
                    InfoFin = request.Info,
                    IsAccepted = false,
                    IsDeleted = false,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = request.UserId,
                    ProjectIdInitial = request.ProjectId,
                    ProjectIdFinal = request.ProjectId,
                    Validated = true,
                    Guid = Guid.NewGuid(),
                    BudgetIdInitial = request.BudgetId,
                    BudgetIdFinal = request.BudgetId,
                };

                _context.Add(requestOp);

                _context.SaveChanges();
            }
            return Ok(StatusCode(200));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reject")]
        public async Task<IActionResult> RequestReject([FromBody] int[] orders)
        {
            await Task.Delay(0);
            Model.Request order = null;
            Model.RequestOp orderOp = null;
            Model.Document document = null;

            string documentTypeCode = "REJECT_REQUEST";

            var documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();


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

            for (int i = 0; i < orders.Length; i++)
            {
                order = _context.Set<Model.Request>().Where(a => a.Id == orders[i]).SingleOrDefault();
                order.AppStateId = 53;
                order.IsAccepted = true;
                order.Validated = true;


                orderOp = new Model.RequestOp()
                {
                    AccMonthId = order.AccMonthId,
                    AccSystemId = null,
                    Request = order,
                    RequestStateId = 53,
                    CompanyId = order.CompanyId,
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
                    IsAccepted = false,
                    IsDeleted = false,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = order.UserId,
                    ProjectIdInitial = order.ProjectId,
                    ProjectIdFinal = order.ProjectId,
                    Validated = true,
                    Guid = Guid.NewGuid(),
                    BudgetIdInitial = order.BudgetId,
                    BudgetIdFinal = order.BudgetId,
                };

                _context.Add(orderOp);

                _context.SaveChanges();
            }

            // await SendValidatedEmail(order.Id);

            return Ok(StatusCode(200));
        }

		[HttpPost]
		[Route("delete")]
		public async Task<RequestResult> RequestDelete([FromBody] RequestDelete requestDto)
		{
			await Task.Delay(0);
			Model.Request request = null;
			Model.RequestOp orderOp = null;
			Model.Document document = null;
            Model.AppState appState = null;
			Model.DocumentType documentType = null;

			documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == "REJECT_REQUEST").FirstOrDefaultAsync();
            if (documentType == null) return new RequestResult {  Success = false, Message = "Nu exista tip de document"};

			appState = await _context.Set<Model.AppState>().Where(d => d.Code == "REJECT_REQUEST").FirstOrDefaultAsync();
			if (appState == null) return new RequestResult { Success = false, Message = "Nu exista stare" };

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

			request = await _context.Set<Model.Request>().Where(a => a.Id == requestDto.Id).FirstOrDefaultAsync();
			if (request == null) return new RequestResult { Success = false, Message = "Nu a fost gasit PR - ul" };

			request.AppStateId = appState.Id;
			request.IsAccepted = true;
			request.Validated = true;


			orderOp = new Model.RequestOp()
			{
				AccMonthId = request.AccMonthId,
				AccSystemId = null,
				Request = request,
				RequestStateId = appState.Id,
				CompanyId = request.CompanyId,
				CostCenterIdInitial = request.CostCenterId,
				CostCenterIdFinal = request.CostCenterId,
				CreatedAt = DateTime.Now,
				CreatedBy = request.UserId,
				Document = document,
				DstConfAt = DateTime.Now,
				DstConfBy = request.UserId,
				EmployeeIdInitial = request.EmployeeId,
				EmployeeIdFinal = request.EmployeeId,
				InfoIni = request.Info,
				InfoFin = request.Info,
				IsAccepted = false,
				IsDeleted = false,
				ModifiedAt = DateTime.Now,
				ModifiedBy = request.UserId,
				ProjectIdInitial = request.ProjectId,
				ProjectIdFinal = request.ProjectId,
				Validated = true,
				Guid = Guid.NewGuid(),
				BudgetIdInitial = request.BudgetId,
				BudgetIdFinal = request.BudgetId,
			};

			_context.Add(orderOp);

			_context.SaveChanges();

			return new RequestResult { Success = true, Message = "PR - ul a fost sters cu sucess!" };
		}

		[HttpGet]
        [Route("getData")]
        public virtual IActionResult GetProjectTypeDivisions(string jsonFilter)
        {
            Model.RequestFilter assetFilter = null;
            assetFilter = GetAssetFilter(jsonFilter);

            //assetFilter.CostCenterIds = new List<int?>();
            //assetFilter.DivisionIds = new List<int?>();
            //assetFilter.DepartmentIds = new List<int?>();

            var items = (_itemsRepository as IRequestsRepository)
                .GetProjectTypeDivisionsWithBudgetBases(assetFilter).ToList();
            var itemsResource = _mapper.Map<List<Model.Division>, List<Dto.Division>>(items);

            return Ok(items);
        }

        private Model.RequestFilter GetAssetFilter(string jsonFilter)
        {
            Model.RequestFilter assetFilter = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<Model.RequestFilter>(jsonFilter) : new Model.RequestFilter();

            //if (HttpContext.User.Identity.Name != null)
            //{
            //    userName = HttpContext.User.Identity.Name;
            //    role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
            //    employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

            //    assetFilter.UserName = userName;
            //    assetFilter.Role = role;

            //    if (employeeId != null)
            //    {
            //        assetFilter.EmployeeId = int.Parse(employeeId);
            //    }
            //    else
            //    {
            //        assetFilter.EmployeeIds = null;
            //        assetFilter.EmployeeIds = new List<int?>();
            //        assetFilter.EmployeeIds.Add(int.Parse("100000000"));
            //    }
            //}
            //else
            //{
            //    assetFilter.EmployeeIds = null;
            //    assetFilter.EmployeeIds = new List<int?>();
            //    assetFilter.EmployeeIds.Add(int.Parse("100000000"));
            //}

            return assetFilter;
        }

        [HttpPost("needBudget")]
        public async Task<RequestResult> NeedBudget([FromBody] NeedBudget needBudget)
        {
            Model.RequestResult requestResult = null;

            

            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }

                if (user != null)
				{
                    _context.UserId = user.Id.ToString();
                    needBudget.UserId = user.Id.ToString();

                    requestResult = await (_itemsRepository as IRequestsRepository).NeedBudget(needBudget);

                    if (requestResult.Success)
                    {
                        //                  if (requestResult.RequestId > 0)
                        //                  {
                        //                     var res =  await _requestsService.SendRequestNeedBudget(requestResult.RequestId);

                        //	if (res)
                        //	{
                        //                          return new Model.RequestResult { Success = true, Message = $"Notificarea a fost trimisa cu succes!", RequestId = requestResult.RequestId };
                        //	}
                        //	else
                        //	{
                        //                          return new Model.RequestResult { Success = false, Message = $"Eroare trimitere notificare!", RequestId = requestResult.RequestId };
                        //                      }
                        //}
                        //else
                        //{
                        //                      return new Model.RequestResult { Success = false, Message = requestResult.Message, RequestId = 0 };
                        //                  }

                        return new Model.RequestResult { Success = true, Message = requestResult.Message, RequestId = requestResult.RequestId };
                    }
                    else
                    {
                        return new Model.RequestResult { Success = false, Message = requestResult.Message, RequestId = 0 };
                    }
                }
                else
                {
                    return new Model.RequestResult { Success = false, Message = $"Userul nu exista!", RequestId = 0 };
                }


            }
            else
            {
                return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!" };
            }

        }

        [AllowAnonymous]
        [Route("requestbudgetnotvalidate/{guidRequest}/{id}")]
        public virtual IActionResult NotValidateDstEmployee(Guid guidRequest, int id)
        {
            return Redirect("http://localhost:4200/#/requestbudgetnotvalidate/" + guidRequest + "/" + id);
            //return Redirect("http://10.5.0.65/FaisTest/#/dstemployeenotvalidate/" + guidEmp + "/" + guid.ToString());
        }

		[HttpPost("updateRequestBudgetForecast")]
		public async Task<RequestResult> UpdateRequestBudgetForecast([FromBody] RequestBudgetForecastUpdate requestDto)
		{
			Model.RequestResult orderResult = null;

			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);

				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}

				if (user != null)
				{
					_context.UserId = user.Id.ToString();
					requestDto.UserId = user.Id;

					try
					{
						orderResult = await (_itemsRepository as IRequestsRepository).RequestBudgetForecastUpdate(requestDto);

						if (orderResult.Success)
						{
							var countBudgetBase = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
							var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();

							return new Model.RequestResult { Success = true, Message = orderResult.Message, RequestId = orderResult.RequestId };
						}
						else
						{
							return new Model.RequestResult { Success = false, Message = orderResult.Message, RequestId = 0 };
						}
					}
					catch (Exception ex)
					{

						return new Model.RequestResult { Success = false, Message = ex.Message, RequestId = 0 };
					}

				}
				else
				{
					return new Model.RequestResult { Success = false, Message = $"Userul nu exista!", RequestId = 0 };
				}


			}
			else
			{
				return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!", RequestId = 0 };
			}


		}

		[HttpGet("export")]
		public async Task<IActionResult> Export(string includes, string jsonFilter, string columnFilter, string filter, string budgetForecastIds, string requestIds, bool newBudget = false, bool needBudget = false)
		{
			AssetDepTotal depTotal = null;
			AssetCategoryTotal catTotal = null;
			RequestFilter requestFilter = null;
			ColumnRequestFilter colRequestFilters = null;
			string employeeId = string.Empty;
			string role = string.Empty;
			List<int?> bfIds = null;
			List<int?> reqIds = null;

			requestFilter = jsonFilter != null ? JsonConvert.DeserializeObject<RequestFilter>(jsonFilter) : new RequestFilter();
			colRequestFilters = columnFilter != null ? JsonConvert.DeserializeObject<ColumnRequestFilter>(columnFilter) : new ColumnRequestFilter();

			if (filter != null)
			{
				requestFilter.Filter = filter;
			}


			if (HttpContext.User.Identity.Name != null)
			{
				var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

				if (user == null)
				{
					user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				}
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				requestFilter.Role = role;
				requestFilter.InInventory = user.InInventory;
				requestFilter.UserId = user.Id;

				if (employeeId != null)
				{
					requestFilter.EmployeeId = int.Parse(employeeId);
				}
				else
				{
					requestFilter.EmployeeIds = null;
					requestFilter.EmployeeIds = new List<int?>();
					requestFilter.EmployeeIds.Add(int.Parse("-1"));
				}
			}
			else
			{
				requestFilter.EmployeeIds = null;
				requestFilter.EmployeeIds = new List<int?>();
				requestFilter.EmployeeIds.Add(int.Parse("-1"));
			}

			if (budgetForecastIds != null && !budgetForecastIds.StartsWith("["))
			{
				budgetForecastIds = "[" + budgetForecastIds + "]";
			}

			if (requestIds != null && !requestIds.StartsWith("["))
			{
				requestIds = "[" + requestIds + "]";
			}

            includes = includes ?? "Request.Employee,Request.AppState,Request.Owner,Request.StartAccMonth,Request.AssetType,Request.AccMonth,Request,";

			if ((budgetForecastIds != null) && (budgetForecastIds.Length > 0)) bfIds = JsonConvert.DeserializeObject<string[]>(budgetForecastIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((requestIds != null) && (requestIds.Length > 0)) reqIds = JsonConvert.DeserializeObject<string[]>(requestIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

			

			using (ExcelPackage package = new ExcelPackage())
			{
				var items = (_itemsRepository as IRequestsRepository)
				.GetRequest(requestFilter, colRequestFilters, includes, null, null, bfIds, reqIds, newBudget, needBudget, out depTotal, out catTotal).ToList();
				var itemsResource = _mapper.Map<List<Model.RequestDetail>, List<Dto.Request>>(items);

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("P.R.");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod P.R.";
				worksheet.Cells[1, 2].Value = "Responsabil";
				worksheet.Cells[1, 3].Value = "Status";
				worksheet.Cells[1, 4].Value = "Date creare";
				worksheet.Cells[1, 5].Value = "Data modificare";
				worksheet.Cells[1, 6].Value = "Owner buget";
				worksheet.Cells[1, 7].Value = "Luna executie";
				worksheet.Cells[1, 8].Value = "Data implementare";
				worksheet.Cells[1, 9].Value = "Tip";
				worksheet.Cells[1, 10].Value = "FY";
				worksheet.Cells[1, 11].Value = "Info";
				worksheet.Cells[1, 12].Value = "Cantitate";
				worksheet.Cells[1, 13].Value = "Buget solicitat";

				int recordIndex = 2;
				foreach (var item in itemsResource)
				{
					worksheet.Cells[recordIndex, 1].Value = item.Code;
					worksheet.Cells[recordIndex, 2].Value = item.Employee != null ? item.Employee.Email : "";
					worksheet.Cells[recordIndex, 3].Value = item.AppState != null ? item.AppState.Name : "";
					worksheet.Cells[recordIndex, 4].Value = item.CreatedAt;
					worksheet.Cells[recordIndex, 4].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 5].Value = item.ModifiedAt;
					worksheet.Cells[recordIndex, 5].Style.Numberformat.Format = "mm/dd/yyyy";
					worksheet.Cells[recordIndex, 6].Value = item.Owner != null ? item.Owner.Email : "";
					worksheet.Cells[recordIndex, 7].Value = item.EndDate;
					worksheet.Cells[recordIndex, 7].Style.Numberformat.Format = "mm/dd/yyyy";
					worksheet.Cells[recordIndex, 8].Value = item.StartAccMonth != null ? item.StartAccMonth.Month : "";
					worksheet.Cells[recordIndex, 9].Value = item.AssetType != null ? item.AssetType.Name : "";
					worksheet.Cells[recordIndex, 10].Value = item.BudgetManager != null ? item.BudgetManager.Name : "";
					worksheet.Cells[recordIndex, 11].Value = item.Info;
					worksheet.Cells[recordIndex, 12].Value = item.Quantity;
					worksheet.Cells[recordIndex, 12].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 13].Value = item.BudgetValueNeed;
					worksheet.Cells[recordIndex, 13].Style.Numberformat.Format = "#,##0.00";
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

				using (var cells = worksheet.Cells[1, 1, 1, 13])
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
					FileDownloadName = "P.R..xlsx"
				};

				return result;

			}
		}
	}
}
