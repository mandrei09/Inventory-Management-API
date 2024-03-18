using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optima.Fais.Api.Helpers;
using Optima.Fais.Api.Services.Flow;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.EfRepository;
using Optima.Fais.Model;
using Optima.Fais.Model.Common;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using PdfSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Optima.Fais.Api.Services.Flow;
using System.Net.NetworkInformation;


namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class OrdersController : GenericApiController<Model.Order, Dto.Order>
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<Model.ApplicationUser> userManager;

        private readonly IEmailSender _emailSender;
		private readonly IMatrixRepository _matrixRepository;
        private readonly ILevelsRepository _levelsRepository;
        private readonly IOrderFlowService _orderFlowService;

        public OrdersController(ApplicationDbContext context,
            IOrdersRepository itemsRepository, IMapper mapper, IWebHostEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager, IEmailSender emailSender, IMatrixRepository matrixRepository, ILevelsRepository levelsRepository, IOrderFlowService orderFlowService)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            this._emailSender = emailSender;
            this._matrixRepository = matrixRepository;
            _levelsRepository = levelsRepository;
            _orderFlowService = orderFlowService;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual async Task<IActionResult> GetDepDetails(int page, int pageSize, string sortColumn, string sortDirection,
            string includes, string jsonFilter, string filter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            OrderFilter orderFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string userName = string.Empty;
            string role = string.Empty;
            string employeeId = string.Empty;
            Model.ApplicationUser user = null;

            includes = includes + "Order.Offer.AssetType,Order.Offer.Request,Order.Division";

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            orderFilter = jsonFilter != null ? JsonConvert.DeserializeObject<OrderFilter>(jsonFilter) : new OrderFilter();

            if (filter != null)
            {
                orderFilter.Filter = filter;
            }


            if (HttpContext.User.Identity.Name != null)
            {
                //user = await userManager.FindByEmailAsync("gabriela.dogaru@emag.ro");
#if RELEASE
                user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
#endif
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                }
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                orderFilter.Role = role;
                orderFilter.InInventory = user.InInventory;
                orderFilter.UserId = user.Id;

                if (employeeId != null)
                {
                    orderFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    orderFilter.EmployeeIds = null;
                    orderFilter.EmployeeIds = new List<int?>();
                    orderFilter.EmployeeIds.Add(int.Parse("-1"));
                }
            }
            else
            {
                orderFilter.EmployeeIds = null;
                orderFilter.EmployeeIds = new List<int?>();
                orderFilter.EmployeeIds.Add(int.Parse("-1"));
            }

            var items = (_itemsRepository as IOrdersRepository)
                .GetOrder(orderFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.OrderDetail>, List<Dto.Order>>(items);

            var result = new OrderPagedResult(itemsResource, new PagingInfo()
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
           string includes, string jsonFilter, string filter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            OrderFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            //var count = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
            //var countA = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrderMaterials").ToList();

            includes ??= "Order.AppState,Order.Offer,Order.BudgetForecast.BudgetBase,Order.Company,Order.Project,Order.Administration,Order.CostCenter,Order.SubType.Type.MasterType,Order.AccMonth,Order.SubType.Type,Order.SubType,Order.InterCompany,Order.Employee,Order.Partner,Order.Account,";
            includes = includes + ",Order.Offer.AssetType,Order.Contract,Order.Offer.Request";
            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<OrderFilter>(jsonFilter) : new OrderFilter();


            if (filter != null)
            {
                assetFilter.Filter = filter;
            }

            var items = (_itemsRepository as IOrdersRepository)
                .GetOrderUI(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.OrderDetail>, List<Dto.OrderUI>>(items);

            var result = new OrderUIPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }

        [HttpGet]
        [Route("detailneedbudgetui", Order = -1)]
        public virtual async Task<IActionResult> GetDepNeedBudget(int page, int pageSize, string sortColumn, string sortDirection,
          string includes, string jsonFilter, string filter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            OrderFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            //var count = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
            //var countA = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrderMaterials").ToList();

            includes ??= "Order.AppState,Order.Offer,Order.Budget,Order.BudgetBase,Order.Company,Order.Project,Order.Administration,Order.CostCenter,Order.SubType.Type.MasterType,Order.AccMonth,Order.SubType.Type,Order.SubType,Order.InterCompany,Order.Employee,Order.Partner,Order.Account,";
            includes = includes + ",Order.Offer.AssetType,Order.Offer.Request.Employee,Order.Offer.Request.AssetType,Order.Offer.Request.CostCenter.Division.Department,Order.Offer.Request.ProjectType,Order.Offer.Request.CostCenter.AdmCenter,Order.BudgetBase.Country,Order.BudgetBase.Activity,Order.Offer.Request.Project";
            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<OrderFilter>(jsonFilter) : new OrderFilter();


            if (filter != null)
            {
                assetFilter.Filter = filter;
            }

            var items = (_itemsRepository as IOrdersRepository)
                .GetOrderNeedBudgetUI(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.OrderDetail>, List<Dto.OrderUI>>(items);

            var result = new OrderUIPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }

        [HttpPost("detail")]
        public async Task<OrderResult> PostDetail([FromBody] OrderSave orderSave)
        {
            Model.OrderResult orderResult = null;

            if(HttpContext.User.Identity.Name != null)
			{
                var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                }

                if (user != null)
				{
                    Model.EmailType emailType = null;
                    Model.EntityType entityType = null;
                    Model.AppState appState = null;
                    Model.Order order = null;
                    Model.Offer offer = null;
                    Model.OrderType orderType = null;
                    Model.Contract contract = null;
                    Model.EmailOrderStatus emailOrderStatus = null;
                    Model.RequestBudgetForecast requestBudgetForecast = null;
                    // bool needBudget = false;

                    _context.UserId = user.Id.ToString();
                    orderSave.UserId = user.Id;
                    orderSave.EmployeeId = user.EmployeeId;

                    offer = await _context.Set<Model.Offer>().Where(c => c.Id == orderSave.OfferId).FirstOrDefaultAsync();
                    if (offer == null) return new Model.OrderResult { Success = false, Message = "Lipsa oferta!", OrderId = 0 };
                    decimal valueRon = orderSave.OrderMaterialUpdates.Sum(a => a.ValueRon);

                    if(orderSave.RequestBudgetForecasts.Count > 0)
					{
                        requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
                            .Include(b => b.BudgetForecast).ThenInclude(b => b.BudgetBase)
                            .Where(a => a.Id == orderSave.RequestBudgetForecasts[0])
                            .SingleAsync();

						try
						{
                            var result = await GetMatrixData(offer.DivisionId.Value, valueRon);// old

							if (result.Count == 1)
                            {
                                orderSave.MatrixId = result[0].Id;

                                try
                                {
                                    orderResult = await (_itemsRepository as IOrdersRepository).CreateOrUpdateOrder(orderSave);

                                    orderType = await _context.Set<Model.OrderType>().Where(c => c.Id == orderSave.OrderTypeId).FirstOrDefaultAsync();
                                    if (orderType == null) return new Model.OrderResult { Success = false, Message = "Lipsa tip comanda!", OrderId = 0 };

                                    if (orderResult.Success && orderType.Code != "C-IT")
                                    {
                                        contract = await _context.Set<Model.Contract>().Where(c => c.Id == requestBudgetForecast.ContractId).FirstOrDefaultAsync();
                                        if (contract == null && orderType.Code != "C-LC") return new Model.OrderResult { Success = false, Message = "Lipsa contract!", OrderId = 0 };

                                        if (orderType.Code != "C-LC")
                                        {
                                            emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_BOOK").FirstOrDefaultAsync();
                                            if (emailType == null) return new Model.OrderResult { Success = false, Message = "Lipsa tip email!", OrderId = 0 };
                                            entityType = await _context.Set<Model.EntityType>().Where(c => c.Code == "ORDER").FirstOrDefaultAsync();
                                            if (entityType == null) return new Model.OrderResult { Success = false, Message = "Lipsa tip entitate!", OrderId = 0 };
                                            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELB1").FirstOrDefaultAsync();
                                            if (appState == null) return new Model.OrderResult { Success = false, Message = "Lipsa stare!", OrderId = 0 };

                                            order = await _context.Set<Model.Order>().Where(c => c.Id == orderResult.OrderId).FirstOrDefaultAsync();
                                            if (order == null) return new Model.OrderResult { Success = false, Message = "Lipsa comanda", OrderId = 0 };

											order.EmployeeB1Id = result.ElementAt(0).EmployeeB1.Validate ? result.ElementAt(0).EmployeeB1.Id : null;
											order.EmployeeL4Id = result.ElementAt(0).EmployeeL4.Validate ? result.ElementAt(0).EmployeeL4.Id : null;
                                            order.EmployeeL3Id = result.ElementAt(0).EmployeeL3.Validate ? result.ElementAt(0).EmployeeL3.Id : null;
                                            order.EmployeeL2Id = result.ElementAt(0).EmployeeL2.Validate ? result.ElementAt(0).EmployeeL2.Id : null;
                                            order.EmployeeL1Id = result.ElementAt(0).EmployeeL1.Validate ? result.ElementAt(0).EmployeeL1.Id : null;
                                            order.EmployeeS3Id = result.ElementAt(0).EmployeeS3.Validate ? result.ElementAt(0).EmployeeS3.Id : null;
                                            order.EmployeeS2Id = result.ElementAt(0).EmployeeS2.Validate ? result.ElementAt(0).EmployeeS2.Id : null;
                                            order.EmployeeS1Id = result.ElementAt(0).EmployeeS1.Validate ? result.ElementAt(0).EmployeeS1.Id : null;

                                            //if (!orderSave.NeedBudgetAmount)
                                            //{
                                            //    order.AppStateId = 15;
                                            //}

                                            emailOrderStatus = new Model.EmailOrderStatus()
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
												EmployeeB1EmailSend = false,
												EmployeeB1ValidateAt = null,
												EmployeeB1ValidateBy = null,
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
												NotEmployeeB1Sync = true,
												NotEmployeeL1Sync = false,
                                                NotEmployeeL2Sync = false,
                                                NotEmployeeL3Sync = false,
                                                NotEmployeeL4Sync = false,
                                                NotEmployeeS1Sync = false,
                                                NotEmployeeS2Sync = false,
                                                NotEmployeeS3Sync = false,
                                                NotSync = false,
                                                OrderId = orderResult.OrderId,
                                                SyncCompletedErrorCount = 0,
												SyncEmployeeB1ErrorCount = 0,
												SyncEmployeeL1ErrorCount = 0,
                                                SyncEmployeeL2ErrorCount = 0,
                                                SyncEmployeeL3ErrorCount = 0,
                                                SyncEmployeeL4ErrorCount = 0,
                                                SyncEmployeeS1ErrorCount = 0,
                                                SyncEmployeeS2ErrorCount = 0,
                                                SyncEmployeeS3ErrorCount = 0,
                                                SyncErrorCount = 0,
												EmployeeB1EmailSkip = !result.ElementAt(0).EmployeeB1.Validate,
												EmployeeL1EmailSkip = !result.ElementAt(0).EmployeeL1.Validate,
                                                EmployeeL2EmailSkip = !result.ElementAt(0).EmployeeL2.Validate,
                                                EmployeeL3EmailSkip = !result.ElementAt(0).EmployeeL3.Validate,
                                                EmployeeL4EmailSkip = !result.ElementAt(0).EmployeeL4.Validate,
                                                EmployeeS1EmailSkip = !result.ElementAt(0).EmployeeS1.Validate,
                                                EmployeeS2EmailSkip = !result.ElementAt(0).EmployeeS2.Validate,
                                                EmployeeS3EmailSkip = !result.ElementAt(0).EmployeeS3.Validate,
                                                PriorityL4 = result.ElementAt(0).PriorityL4,
												PriorityL3 = result.ElementAt(0).PriorityL3,
												PriorityL2 = result.ElementAt(0).PriorityL2,
												PriorityL1 = result.ElementAt(0).PriorityL1,
												PriorityS1 = result.ElementAt(0).PriorityS1,
												PriorityS2 = result.ElementAt(0).PriorityS2,
												PriorityS3 = result.ElementAt(0).PriorityS3,
											};

                                            _context.Update(order);
                                            _context.Add(emailOrderStatus);
                                            _context.SaveChanges();

                                            //if (orderSave.NeedBudgetAmount)
                                            //{
                                            //    var res = await NeedBudget(orderResult.OrderId);

                                            //    if (res)
                                            //    {
                                            //        return new Model.OrderResult { Success = true, Message = $"Notificarea a fost trimisa cu succes!!", OrderId = orderResult.OrderId };
                                            //    }
                                            //    else
                                            //    {
                                            //        return new Model.OrderResult { Success = false, Message = $"Eroare trimitere notificare!!", OrderId = orderResult.OrderId };
                                            //    }
                                            //}
                                            //else
                                            //{
                                            //    return new Model.OrderResult { Success = true, Message = $"Comanda a fost salvata cu succes!!", OrderId = orderResult.OrderId };
                                            //}

                                            return new Model.OrderResult { Success = true, Message = $"Comanda a fost salvata cu succes!!", OrderId = orderResult.OrderId };
                                        }
                                        else
                                        {
                                            var res = await NeedContract(orderResult.OrderId);

                                            if (res)
                                            {
                                                return new Model.OrderResult { Success = true, Message = $"Notificarea a fost trimisa cu succes!!", OrderId = orderResult.OrderId };
                                            }
                                            else
                                            {
                                                return new Model.OrderResult { Success = false, Message = $"Eroare trimitere notificare!!", OrderId = orderResult.OrderId };
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return new Model.OrderResult { Success = orderResult.Success, Message = orderResult.Message, OrderId = orderResult.OrderId };
                                    }
                                }
                                catch (Exception ex)
                                {

                                    if (orderResult != null && orderResult.OrderId > 0)
                                    {
                                        order = await _context.Set<Model.Order>().Where(c => c.Id == orderResult.OrderId).SingleAsync();

                                        order.IsDeleted = true;
                                        order.Info = ex.Message.ToString();
                                        _context.Update(order);
                                        _context.SaveChanges();

                                        return new Model.OrderResult { Success = false, Message = ex.ToString(), OrderId = 0 };
                                    }
                                    else
                                    {
                                        return new Model.OrderResult { Success = false, Message = ex.ToString(), OrderId = 0 };
                                    }
                                }
                            }
                            else
                            {
                                return new Model.OrderResult { Success = false, Message = $"Nu exista matrice de aprobare!", OrderId = 0 };
                            }
                        }
						catch (Exception ex)
						{

                            return new Model.OrderResult { Success = false, Message = ex.Message, OrderId = 0 };
                        }

					}
					else
					{
                        return new Model.OrderResult { Success = false, Message = $"Nu exista WBS!", OrderId = 0 };
                    }
                }
                else
                {
                    return new Model.OrderResult { Success = false, Message = $"Userul nu exista!", OrderId = 0 };
                }
            }
            else
            {
                return new Model.OrderResult { Success = false, Message = $"Va rugam sa va autentificati!", OrderId = 0 };
            }

            

           
        }

        [HttpPost("detailstock")]
        public async Task<OrderResult> PostDetailStock([FromBody] OrderStockSave orderSave)
        {
            Model.OrderResult orderResult = null;

            if (HttpContext.User.Identity.Name != null)
            {
                var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                }

                if (user != null)
                {
                    Model.EmailType emailType = null;
                    Model.EntityType entityType = null;
                    Model.AppState appState = null;
                    Model.Order order = null;
                    Model.Offer offer = null;
                    Model.OrderType orderType = null;
                    Model.Contract contract = null;
                    Model.EmailOrderStatus emailOrderStatus = null;
                    Model.RequestBudgetForecast requestBudgetForecast = null;
                    // bool needBudget = false;

                    _context.UserId = user.Id.ToString();
                    orderSave.UserId = user.Id;
                    orderSave.EmployeeId = user.EmployeeId;

					//offer = await _context.Set<Model.Offer>().Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();
					//if (offer == null) return new Model.OrderResult { Success = false, Message = "Lipsa oferta!", OrderId = 0 };
					//decimal valueRon = orderSave.OrderMaterialUpdates.Sum(a => a.ValueRon);

					try
					{
                        orderResult = await (_itemsRepository as IOrdersRepository).CreateOrUpdateOrderStock(orderSave);

                        return new Model.OrderResult { Success = true, Message = $"Comanda a fost salvata cu succes!!", OrderId = orderResult.OrderId };
                    }
					catch (Exception ex)
					{

                        if (orderResult != null && orderResult.OrderId > 0)
                        {
                            order = await _context.Set<Model.Order>().Where(c => c.Id == orderResult.OrderId).SingleAsync();

                            order.IsDeleted = true;
                            order.Info = ex.Message.ToString();
                            _context.Update(order);
                            _context.SaveChanges();

                            return new Model.OrderResult { Success = false, Message = ex.ToString(), OrderId = 0 };
                        }
                        else
                        {
                            return new Model.OrderResult { Success = false, Message = ex.ToString(), OrderId = 0 };
                        }
                    }

                    

                    //if (orderSave.RequestBudgetForecasts.Count > 0)
                    //{
                    //    requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
                    //        .Include(b => b.BudgetForecast).ThenInclude(b => b.BudgetBase)
                    //        .Where(a => a.Id == orderSave.RequestBudgetForecasts[0])
                    //        .SingleAsync();

                    //    try
                    //    {
                    //        var result = await GetMatrixData(offer.DivisionId.Value, valueRon);

                    //        if (result.Count == 1)
                    //        {
                    //            orderSave.MatrixId = result[0].Id;

                    //            try
                    //            {
                    //                orderResult = await (_itemsRepository as IOrdersRepository).CreateOrUpdateOrderStock(orderSave);

                    //                orderType = await _context.Set<Model.OrderType>().Where(c => c.Id == orderSave.OrderTypeId).FirstOrDefaultAsync();
                    //                if (orderType == null) return new Model.OrderResult { Success = false, Message = "Lipsa tip comanda!", OrderId = 0 };

                    //                if (orderResult.Success && orderType.Code != "C-IT")
                    //                {
                    //                    contract = await _context.Set<Model.Contract>().Where(c => c.Id == requestBudgetForecast.ContractId).FirstOrDefaultAsync();
                    //                    if (contract == null) return new Model.OrderResult { Success = false, Message = "Lipsa contract!", OrderId = 0 };

                    //                    if (contract.ContractId != "NO-C")
                    //                    {
                    //                        emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_BOOK").FirstOrDefaultAsync();
                    //                        if (emailType == null) return new Model.OrderResult { Success = false, Message = "Lipsa tip email!", OrderId = 0 };
                    //                        entityType = await _context.Set<Model.EntityType>().Where(c => c.Code == "ORDER").FirstOrDefaultAsync();
                    //                        if (entityType == null) return new Model.OrderResult { Success = false, Message = "Lipsa tip entitate!", OrderId = 0 };
                    //                        appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_BOOK").FirstOrDefaultAsync();
                    //                        if (appState == null) return new Model.OrderResult { Success = false, Message = "Lipsa stare!", OrderId = 0 };

                    //                        order = await _context.Set<Model.Order>().Where(c => c.Id == orderResult.OrderId).FirstOrDefaultAsync();
                    //                        if (order == null) return new Model.OrderResult { Success = false, Message = "Lipsa comanda", OrderId = 0 };

                    //                        order.EmployeeL4Id = result.ElementAt(0).EmployeeL4.Validate ? result.ElementAt(0).EmployeeL4.Id : null;
                    //                        order.EmployeeL3Id = result.ElementAt(0).EmployeeL3.Validate ? result.ElementAt(0).EmployeeL3.Id : null;
                    //                        order.EmployeeL2Id = result.ElementAt(0).EmployeeL2.Validate ? result.ElementAt(0).EmployeeL2.Id : null;
                    //                        order.EmployeeL1Id = result.ElementAt(0).EmployeeL1.Validate ? result.ElementAt(0).EmployeeL1.Id : null;
                    //                        order.EmployeeS3Id = result.ElementAt(0).EmployeeS3.Validate ? result.ElementAt(0).EmployeeS3.Id : null;
                    //                        order.EmployeeS2Id = result.ElementAt(0).EmployeeS2.Validate ? result.ElementAt(0).EmployeeS2.Id : null;
                    //                        order.EmployeeS1Id = result.ElementAt(0).EmployeeS1.Validate ? result.ElementAt(0).EmployeeS1.Id : null;

                    //                        //if (!orderSave.NeedBudgetAmount)
                    //                        //{
                    //                        //    order.AppStateId = 15;
                    //                        //}

                    //                        emailOrderStatus = new Model.EmailOrderStatus()
                    //                        {
                    //                            AppStateId = appState.Id,
                    //                            Completed = false,
                    //                            CreatedAt = DateTime.Now,
                    //                            CreatedBy = user.Id,
                    //                            DocumentNumber = int.Parse(entityType.Name),
                    //                            EmailSend = false,
                    //                            EmailTypeId = emailType.Id,
                    //                            EmployeeL1EmailSend = false,
                    //                            EmployeeL1ValidateAt = null,
                    //                            EmployeeL1ValidateBy = null,
                    //                            EmployeeL2EmailSend = false,
                    //                            EmployeeL2ValidateAt = null,
                    //                            EmployeeL2ValidateBy = null,
                    //                            EmployeeL3EmailSend = false,
                    //                            EmployeeL3ValidateAt = null,
                    //                            EmployeeL3ValidateBy = null,
                    //                            EmployeeL4EmailSend = false,
                    //                            EmployeeL4ValidateAt = null,
                    //                            EmployeeL4ValidateBy = null,
                    //                            EmployeeS1EmailSend = false,
                    //                            EmployeeS1ValidateAt = null,
                    //                            EmployeeS1ValidateBy = null,
                    //                            EmployeeS2EmailSend = false,
                    //                            EmployeeS2ValidateAt = null,
                    //                            EmployeeS2ValidateBy = null,
                    //                            EmployeeS3EmailSend = false,
                    //                            EmployeeS3ValidateAt = null,
                    //                            EmployeeS3ValidateBy = null,
                    //                            ErrorId = null,
                    //                            Exported = false,
                    //                            FinalValidateAt = null,
                    //                            FinalValidateBy = null,
                    //                            Guid = Guid.NewGuid(),
                    //                            GuidAll = Guid.NewGuid(),
                    //                            Info = string.Empty,
                    //                            IsAccepted = false,
                    //                            IsDeleted = false,
                    //                            MatrixId = result[0].Id,
                    //                            ModifiedAt = DateTime.Now,
                    //                            ModifiedBy = user.Id,
                    //                            NotCompletedSync = false,
                    //                            NotEmployeeL1Sync = false,
                    //                            NotEmployeeL2Sync = false,
                    //                            NotEmployeeL3Sync = false,
                    //                            NotEmployeeL4Sync = true,
                    //                            NotEmployeeS1Sync = false,
                    //                            NotEmployeeS2Sync = false,
                    //                            NotEmployeeS3Sync = false,
                    //                            NotSync = false,
                    //                            OrderId = orderResult.OrderId,
                    //                            SyncCompletedErrorCount = 0,
                    //                            SyncEmployeeL1ErrorCount = 0,
                    //                            SyncEmployeeL2ErrorCount = 0,
                    //                            SyncEmployeeL3ErrorCount = 0,
                    //                            SyncEmployeeL4ErrorCount = 0,
                    //                            SyncEmployeeS1ErrorCount = 0,
                    //                            SyncEmployeeS2ErrorCount = 0,
                    //                            SyncEmployeeS3ErrorCount = 0,
                    //                            SyncErrorCount = 0,
                    //                            EmployeeL1EmailSkip = !result.ElementAt(0).EmployeeL1.Validate,
                    //                            EmployeeL2EmailSkip = !result.ElementAt(0).EmployeeL2.Validate,
                    //                            EmployeeL3EmailSkip = !result.ElementAt(0).EmployeeL3.Validate,
                    //                            EmployeeL4EmailSkip = !result.ElementAt(0).EmployeeL4.Validate,
                    //                            EmployeeS1EmailSkip = !result.ElementAt(0).EmployeeS1.Validate,
                    //                            EmployeeS2EmailSkip = !result.ElementAt(0).EmployeeS2.Validate,
                    //                            EmployeeS3EmailSkip = !result.ElementAt(0).EmployeeS3.Validate,
                    //                        };

                    //                        _context.Update(order);
                    //                        _context.Add(emailOrderStatus);
                    //                        _context.SaveChanges();

                    //                        //if (orderSave.NeedBudgetAmount)
                    //                        //{
                    //                        //    var res = await NeedBudget(orderResult.OrderId);

                    //                        //    if (res)
                    //                        //    {
                    //                        //        return new Model.OrderResult { Success = true, Message = $"Notificarea a fost trimisa cu succes!!", OrderId = orderResult.OrderId };
                    //                        //    }
                    //                        //    else
                    //                        //    {
                    //                        //        return new Model.OrderResult { Success = false, Message = $"Eroare trimitere notificare!!", OrderId = orderResult.OrderId };
                    //                        //    }
                    //                        //}
                    //                        //else
                    //                        //{
                    //                        //    return new Model.OrderResult { Success = true, Message = $"Comanda a fost salvata cu succes!!", OrderId = orderResult.OrderId };
                    //                        //}

                    //                        return new Model.OrderResult { Success = true, Message = $"Comanda a fost salvata cu succes!!", OrderId = orderResult.OrderId };
                    //                    }
                    //                    else
                    //                    {
                    //                        var res = await NeedContract(orderResult.OrderId);

                    //                        if (res)
                    //                        {
                    //                            return new Model.OrderResult { Success = true, Message = $"Notificarea a fost trimisa cu succes!!", OrderId = orderResult.OrderId };
                    //                        }
                    //                        else
                    //                        {
                    //                            return new Model.OrderResult { Success = false, Message = $"Eroare trimitere notificare!!", OrderId = orderResult.OrderId };
                    //                        }
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    return new Model.OrderResult { Success = false, Message = $"Eroare salvare!!", OrderId = 0 };
                    //                }
                    //            }
                    //            catch (Exception ex)
                    //            {

                    //                if (orderResult != null && orderResult.OrderId > 0)
                    //                {
                    //                    order = await _context.Set<Model.Order>().Where(c => c.Id == orderResult.OrderId).SingleAsync();

                    //                    order.IsDeleted = true;
                    //                    order.Info = ex.Message.ToString();
                    //                    _context.Update(order);
                    //                    _context.SaveChanges();

                    //                    return new Model.OrderResult { Success = false, Message = ex.ToString(), OrderId = 0 };
                    //                }
                    //                else
                    //                {
                    //                    return new Model.OrderResult { Success = false, Message = ex.ToString(), OrderId = 0 };
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            return new Model.OrderResult { Success = false, Message = $"Nu exista matrice de aprobare!", OrderId = 0 };
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {

                    //        return new Model.OrderResult { Success = false, Message = ex.Message, OrderId = 0 };
                    //    }

                    //}
                    //else
                    //{
                    //    return new Model.OrderResult { Success = false, Message = $"Nu exista WBS!", OrderId = 0 };
                    //}
                }
                else
                {
                    return new Model.OrderResult { Success = false, Message = $"Userul nu exista!", OrderId = 0 };
                }
            }
            else
            {
                return new Model.OrderResult { Success = false, Message = $"Va rugam sa va autentificati!", OrderId = 0 };
            }




        }

        [HttpPost("detailCheck")]
        public async Task<Model.CreateAssetSAPResult> PostDetailCheck([FromBody] OrderStockSave orderDto)
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
                orderDto.EmployeeId = user.EmployeeId;

				try
				{
                    var createAssetSAPResult = await (_itemsRepository as IOrdersRepository).CreateOrUpdateOrderCheck(orderDto);

                    if (!createAssetSAPResult.Success)
                    {
                        Model.ErrorType errorType = await _context.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "NEWASSETCHECK").SingleOrDefaultAsync();
                        Model.Stock stock = await _context.Set<Model.Stock>().Where(a => a.Id == createAssetSAPResult.EntityId).SingleOrDefaultAsync();
                        if (errorType == null)
                        {
                            return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Error-Type-Missing" };
                        }

                        Model.Error error = new Model.Error();

                        error.AssetId = null;
                        error.ErrorTypeId = errorType.Id;
                        error.CreatedAt = DateTime.Now;
                        error.CreatedBy = _context.UserId;
                        error.ModifiedAt = DateTime.Now;
                        error.ModifiedBy = _context.UserId;
                        error.Code = "AddAssetStockCheck";
                        error.Request = JsonConvert.SerializeObject(orderDto, Formatting.Indented).ToString();
                        error.Name = createAssetSAPResult.ErrorMessage;
                        error.UserId = _context.UserId;
                        error.IsDeleted = false;

                        _context.Add(error);

                        stock.Validated = false;
                        stock.Error = error;
                        _context.Update(stock);
                        _context.SaveChanges();
                    }

                    return createAssetSAPResult;
                }
				catch (Exception ex)
				{

                    return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = ex.Message };
                }

               
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("contractUpdate")]
        public async Task<Model.CreateAssetSAPResult> ContractOrderUpdate([FromBody] OrderContractUpdate orderDto)
        {
            Model.EmailType emailType = null;
            Model.EntityType entityType = null;
            Model.AppState appState = null;

            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                Model.Order order = await _context.Set<Model.Order>().Include(o => o.OrderMaterials).Where(c => c.Id == orderDto.Id).SingleAsync();
                Model.Offer offer = await _context.Set<Model.Offer>().Where(c => c.Id == order.OfferId).SingleAsync();
                decimal valueRon = order.OrderMaterials.Sum(a => a.ValueRon);

                var result = await GetMatrixData(offer.DivisionId.Value, valueRon);

                if(result.Count > 0)
				{
                    var createAssetSAPResult = await (_itemsRepository as IOrdersRepository).OrderContractUpdate(orderDto);

                    emailType = _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_BOOK").Single();
                    entityType = _context.Set<Model.EntityType>().Where(c => c.Code == "ORDER").Single();
                    appState = _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_BOOK").Single();

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
                        NotEmployeeL1Sync = true,
                        NotEmployeeL2Sync = false,
                        NotEmployeeL3Sync = false,
                        NotEmployeeL4Sync = false,
                        NotEmployeeS1Sync = false,
                        NotEmployeeS2Sync = false,
                        NotEmployeeS3Sync = false,
                        NotSync = false,
                        OrderId = orderDto.Id,
                        SyncCompletedErrorCount = 0,
                        SyncEmployeeL1ErrorCount = 0,
                        SyncEmployeeL2ErrorCount = 0,
                        SyncEmployeeL3ErrorCount = 0,
                        SyncEmployeeL4ErrorCount = 0,
                        SyncEmployeeS1ErrorCount = 0,
                        SyncEmployeeS2ErrorCount = 0,
                        SyncEmployeeS3ErrorCount = 0,
                        SyncErrorCount = 0
                    };

                    _context.Add(emailOrderStatus);
                    _context.SaveChanges();

                    return createAssetSAPResult;
				}
				else
				{
                    return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
                }

                
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpGet("detail/{id:int}")]
        public virtual IActionResult GetDetail(int id, string includes)
        {
            var asset = (_itemsRepository as IOrdersRepository).GetDetailsById(id, includes);
            var result = _mapper.Map<Dto.Order>(asset);

            return Ok(result);
        }


        [HttpPost("sendEmail")]
        // [Authorize]
        public async Task<IActionResult> SendEmail(int orderId)
        {

            if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
            var item = (_itemsRepository as IOrdersRepository).SendEmail(orderId, _context.UserName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut);

            List<string> ccOut = new List<string>();

            ccOut.Add("adrian.cirnaru@optima.ro");
            // ccOut.Add("isabela.hurduzea@optima.ro");
            // ccOut.Add("adrian.daniu@emag.ro");
            // ccOut.Add(emailIniOut);
            //ccOut.Add(emailCCOut);

            var messageAttach = new Message(ccOut,null, null, subjectOut, bodyHtmlOut, null);


            // var success = await _emailSender.SendEmailAsync(messageAttach);
            var success = true;
            //if (success)
            //{
            //    Model.Order order = _context.Set<Model.Order>().AsNoTracking().Where(a => a.Id == orderId).SingleOrDefault();

            //    Model.EmailType eType = _context.Set<Model.EmailType>().Where(e => e.Code == "NEW_ORDER").SingleOrDefault();

            //    Model.EmailManager emailManager = new Model.EmailManager()
            //    {
            //        EmailTypeId = eType.Id,
            //        OfferId = order.OfferId,
            //        PartnerId = order.PartnerId,
            //        SubTypeId = order.SubTypeId,
            //        OrderId = orderId,
            //        BudgetId = order.BudgetId,
            //        AppStateId = 12,
            //        CreatedAt = DateTime.Now,
            //        CreatedBy = _context.UserName,
            //        ModifiedAt = DateTime.Now,
            //        ModifiedBy = _context.UserName,
            //        Guid = Guid.NewGuid(),
            //        GuidAll = Guid.NewGuid(),
            //        IsDeleted = false,
            //        Info = order.Name,
            //        EmployeeIdInitial= order.EmployeeId,
            //        EmployeeIdFinal = order.EmployeeId,
            //        CompanyId = order.CompanyId


            //    };

            //    _context.Update(emailManager);
            //    _context.SaveChanges();
            //}

            return Ok(orderId);
        }

        [HttpPost("sendValidatedEmail")]
        // [Authorize]
        public async Task<IActionResult> SendValidatedEmail(int orderId)
        {

            if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
            var id = (_itemsRepository as IOrdersRepository).SendValidatedEmail(orderId, _context.UserName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut);

            List<string> ccOut = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            var success = false;
            ccOut.Add("adrian.cirnaru@optima.ro");

            var messageAttach = new Message(ccOut, cc, bcc, subjectOut, bodyHtmlOut, null);

            if (id > 0)
			{
                // success = await _emailSender.SendEmailAsync(messageAttach);
            }

            success = true;
            if (success)
			{
                Model.Order order = _context.Set<Model.Order>().AsNoTracking().Where(a => a.Id == orderId).SingleOrDefault();

                Model.EmailType eType = _context.Set<Model.EmailType>().Where(e => e.Code == "ORDER_VALIDATE_LEVEL1").SingleOrDefault();

                Model.EmailManager emailManager = new Model.EmailManager()
                {
                    EmailTypeId = eType.Id,
                    OfferId = order.OfferId,
                    PartnerId = order.PartnerId,
                    SubTypeId = order.SubTypeId,
                    OrderId = orderId,
                    BudgetId = order.BudgetId,
                    AppStateId = 13,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserName,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = _context.UserName,
                    Guid = Guid.NewGuid(),
                    GuidAll = Guid.NewGuid(),
                    IsDeleted = false,
                    Info = order.Name,
                    EmployeeIdInitial = order.EmployeeId,
                    EmployeeIdFinal = order.EmployeeId,
                    CompanyId = order.CompanyId


                };

                _context.Update(emailManager);
                _context.SaveChanges();
            }

			return Ok(orderId);
        }

        [HttpGet]
        [Route("ordervalidate")]
        [AllowAnonymous]
        public virtual IActionResult OrderValidate(int page, int pageSize, string sortColumn, string sortDirection,
            string includes, string jsonFilter, string userId)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            OrderFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<OrderFilter>(jsonFilter) : new OrderFilter();

            var items = (_itemsRepository as IOrdersRepository)
                .BudgetValidate(assetFilter, includes, userId, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.OrderDetail>, List<Dto.Order>>(items);

            var result = new OrderPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }


        private bool GetOrderStatusSkip(Model.EmailOrderStatus emailOrderStatus, string levelCode)
        {
            switch(levelCode)
            {
                case "B1":
                    return emailOrderStatus.EmployeeB1EmailSkip;
                case "L1":
                    return emailOrderStatus.EmployeeL1EmailSkip;
                case "L2":
                    return emailOrderStatus.EmployeeL2EmailSkip;
                case "L3":
                    return emailOrderStatus.EmployeeL3EmailSkip;
                case "L4":
                    return emailOrderStatus.EmployeeL4EmailSkip;
                case "S1":
                    return emailOrderStatus.EmployeeS1EmailSkip;
                case "S2":
                    return emailOrderStatus.EmployeeS2EmailSkip;
                case "S3":
                    return emailOrderStatus.EmployeeS3EmailSkip;
                default:
                    return false;
            }
        }

        private bool IsEmployeeNull(Model.Order order, Model.Employee employeeNull,string levelCode)
        {
            switch (levelCode)
            {
                case "B1":
                    return order.EmployeeB1Id == employeeNull.Id;
                case "L1":
                    return order.EmployeeL1Id == employeeNull.Id;
                case "L2":
                    return order.EmployeeL2Id == employeeNull.Id;
                case "L3":
                    return order.EmployeeL3Id == employeeNull.Id;
                case "L4":
                    return order.EmployeeL4Id == employeeNull.Id;
                case "S1":
                    return order.EmployeeS1Id == employeeNull.Id;
                case "S2":
                    return order.EmployeeS2Id == employeeNull.Id;
                case "S3":
                    return order.EmployeeS3Id == employeeNull.Id;
                default:
                    return false;
            }
        }

        [HttpPost]
        [Route("validatelevel/{appStateId}")]
        public async Task<CreateAssetSAPResult> OrderValidate([FromBody] int[] orders, int appStateId)
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

                Model.Order order = null;
                Model.OrderOp orderOp = null;
                Model.Document document = null;
                Model.Employee employeeNull = null;
                Model.EmailOrderStatus emailOrderStatus = null;
                Model.EmailType emailType = null;
                Model.AppState appState = null;
                Model.DocumentType documentType = null;
                Model.AppState appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Id == appStateId).SingleAsync();
                List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
                List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
                Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
                Model.Level level = null;
                Model.Level nextLevel = null;

                string appStateCode = appStateRequest.Code;
                string documentTypeCode = "VALIDATE_" + appStateCode; 

                string errorMessage = $"Nu exista stare nivel {appStateCode}!";
                documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).SingleAsync();

                if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = errorMessage };

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

                employeeNull = await _context.Set<Model.Employee>().Where(e => (e.InternalCode == "-" && e.IsDeleted == true)).FirstOrDefaultAsync();

                for (int i = 0; i < orders.Length; i++)
                {
                    order = await _context.Set<Model.Order>().Where(a => a.Id == orders[i]).SingleOrDefaultAsync();
                    emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == orders[i] && a.IsDeleted == false).LastOrDefaultAsync();
                    level = await _context.Set<Model.Level>().Where(l => l.Code == appStateRequest.Name).FirstOrDefaultAsync();
                    nextLevel = await _context.Set<Model.Level>().Where(l => l.Code == level.NextLevelCode).FirstOrDefaultAsync();

                    string emailTypeCode = "VALIDATED_OFFER";
                    appStateCode = "ACCEPTED";

                    bool condition1 = GetOrderStatusSkip(emailOrderStatus, nextLevel.Code);
                    bool condition2 = IsEmployeeNull(order, employeeNull, nextLevel.Code);

                    while (condition1 || condition2)
                    {
                        if (nextLevel.NextLevelCode == "ACCEPTED")
                        {
                            break;
                        }
                        nextLevel = await _context.Set<Model.Level>().Where(l => l.Code == nextLevel.NextLevelCode).FirstOrDefaultAsync();
                        condition1 = GetOrderStatusSkip(emailOrderStatus, nextLevel.Code);
                        condition2 = IsEmployeeNull(order, employeeNull, nextLevel.Code);  
                    }

                    if(nextLevel.NextLevelCode == "ACCEPTED")
                    {
                        emailTypeCode = "VALIDATED_OFFER";
                        appStateCode = "ACCEPTED";
                    }
                    else
                        if (nextLevel.Code == "L1" || nextLevel.Code == "L2" || nextLevel.Code == "L3" || nextLevel.Code == "L4")
                        {
                            emailTypeCode = "ORDER_VALIDATE_LEVEL" + nextLevel.Code[1];
                            appStateCode = "ORDER_LEVEL" + nextLevel.Code[1];
                        }
                        else
                        {
                            emailTypeCode = "ORDER_VALIDATE_LEVEL" + nextLevel.Code;
                            appStateCode = "ORDER_LEVEL" + nextLevel.Code;
                        }

                    emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == emailTypeCode).SingleAsync();
                    appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == appStateCode).SingleAsync();

                    emailOrderStatus.NotEmployeeS3Sync = true;
                    emailOrderStatus.SyncEmployeeS3ErrorCount = 0;

                    order.AppStateId = appState.Id;
                    order.EndDate = DateTime.Now;

                    emailOrderStatus.AppStateId = appState.Id;
                    emailOrderStatus.EmailTypeId = emailType.Id;

                    switch (appStateCode)
                    {
                        case "ORDER_LEVELB1":
                            {
                                emailOrderStatus.EmployeeB1ValidateAt = DateTime.Now;
                                emailOrderStatus.EmployeeB1ValidateBy = user.Id;
                                break;
                            }

                        case "ORDER_LEVEL1":
                            {
                                emailOrderStatus.EmployeeL1ValidateAt = DateTime.Now;
                                emailOrderStatus.EmployeeL1ValidateBy = user.Id;
                                break;
                            }
                        case "ORDER_LEVEL2":
                            {
                                emailOrderStatus.EmployeeL2ValidateAt = DateTime.Now;
                                emailOrderStatus.EmployeeL2ValidateBy = user.Id;
                                break;
                            }
                        case "ORDER_LEVEL3":
                            {
                                emailOrderStatus.EmployeeL3ValidateAt = DateTime.Now;
                                emailOrderStatus.EmployeeL3ValidateBy = user.Id;
                                break;
                            }
                        case "ORDER_LEVEL4":
                            {
                                emailOrderStatus.EmployeeL4ValidateAt = DateTime.Now;
                                emailOrderStatus.EmployeeL4ValidateBy = user.Id;
                                break;
                            }
                        case "ORDER_LEVELS1":
                            {
                                emailOrderStatus.EmployeeS1ValidateAt = DateTime.Now;
                                emailOrderStatus.EmployeeS1ValidateBy = user.Id;
                                break;
                            }
                        case "ORDER_LEVELS2":
                            {
                                emailOrderStatus.EmployeeS2ValidateAt = DateTime.Now;
                                emailOrderStatus.EmployeeS2ValidateBy = user.Id;
                                break;
                            }
                        case "ORDER_LEVELS3":
                            {
                                emailOrderStatus.EmployeeS3ValidateAt = DateTime.Now;
                                emailOrderStatus.EmployeeS3ValidateBy = user.Id;
                                break;
                            }
                    }

                    emailOrderStatus.ModifiedAt = DateTime.Now;
                    emailOrderStatus.ModifiedBy = user.Id;

                    _context.Update(emailOrderStatus);

                    requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
                     .Where(a => a.OrderId == order.Id && a.AppStateId == appStateId && a.IsDeleted == false)
                     .ToListAsync();

                    for (int a = 0; a < requestBudgetForecastMaterials.Count; a++)
                    {
                        requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                            .Where(r => r.Id == requestBudgetForecastMaterials[a].Id).SingleAsync();

                        requestBudgetForecastMaterial.AppStateId = order.AppStateId;

                        _context.Update(requestBudgetForecastMaterial);
                        _context.SaveChanges();
                    }

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

                    _context.SaveChanges();
                }

                return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

		[AllowAnonymous]
		[HttpPost]
		[Route("validatemobilelevelB1/{guid}")]
		public async Task<CreateAssetSAPResult> OrderMobileValidateB1(Guid guid)
		{
			Model.Order order = null;
            Model.Employee employeeNull = null;
			Model.OrderOp orderOp = null;
			Model.Document document = null;
			Model.EmailOrderStatus emailOrderStatus = null;
			Model.EmailType emailType = null;
			Model.AppState appState = null;
			Model.DocumentType documentType = null;
			Model.Matrix matrix = null;
			Model.Employee employee = null;
			Model.ApplicationUser applicationUser = null;
			Model.AppState appStateRequest = null;
			List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
			List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
			Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;

			string documentTypeCode = String.Empty;

			documentTypeCode = "VALIDATE_ORDER_LEVELB1";
			documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).FirstOrDefaultAsync();
			if (documentType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip document!" };

			appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELB1").FirstOrDefaultAsync();
			if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel B1!" };

			appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELB1").FirstOrDefaultAsync();
			if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel B1!" };

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

			order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
			if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
			emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
			if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
			matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
			if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
			employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeL4Id).FirstOrDefaultAsync();
			if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
			applicationUser = await userManager.FindByEmailAsync(employee.Email);

			if (applicationUser == null)
			{
				applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
				if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
			}

            employeeNull = await _context.Set<Model.Employee>().Where(e => (e.InternalCode == "-" && e.IsDeleted == true)).FirstOrDefaultAsync();

            if (!emailOrderStatus.EmployeeL4EmailSkip && order.EmployeeL4Id != employeeNull.Id)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL4").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel L4!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel L4!" };

				emailOrderStatus.NotEmployeeL4Sync = true;
				emailOrderStatus.SyncEmployeeL4ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeS3EmailSkip && order.EmployeeS3Id != employeeNull.Id)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS3").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel S3!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS3").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel S3!" };

				emailOrderStatus.NotEmployeeS3Sync = true;
				emailOrderStatus.SyncEmployeeS3ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeL3EmailSkip && order.EmployeeL3Id != employeeNull.Id)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL3").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel L3!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL3").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel L3!" };

				emailOrderStatus.NotEmployeeL3Sync = true;
				emailOrderStatus.SyncEmployeeL3ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeS2EmailSkip && order.EmployeeS2Id != employeeNull.Id)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS2").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel S2!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS2").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel S2!" };

				emailOrderStatus.NotEmployeeS2Sync = true;
				emailOrderStatus.SyncEmployeeS2ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeL2EmailSkip && order.EmployeeL2Id != employeeNull.Id)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL2").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel L2!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL2").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel L2!" };

				emailOrderStatus.NotEmployeeL2Sync = true;
				emailOrderStatus.SyncEmployeeL2ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeL1EmailSkip && order.EmployeeL1Id != employeeNull.Id)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL1").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel L1!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL1").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel L1!" };

				emailOrderStatus.NotEmployeeL1Sync = true;
				emailOrderStatus.SyncEmployeeL1ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeS1EmailSkip && order.EmployeeS1Id != employeeNull.Id)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS1").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel S1!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS1").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel S1!" };

				emailOrderStatus.NotEmployeeS1Sync = true;
				emailOrderStatus.SyncEmployeeS1ErrorCount = 0;
			}
			else
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "VALIDATED_OFFER").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email validare!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ACCEPTED").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare acceptare!" };

				emailOrderStatus.EmailSend = false;
				emailOrderStatus.NotCompletedSync = true;
			}


			order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
			emailOrderStatus.EmailTypeId = emailType.Id;
			emailOrderStatus.EmployeeB1ValidateAt = DateTime.Now;
			emailOrderStatus.EmployeeB1ValidateBy = applicationUser.Id;
			emailOrderStatus.ModifiedAt = DateTime.Now;
			emailOrderStatus.ModifiedBy = applicationUser.Id;

			_context.Update(emailOrderStatus);

			requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
			   .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
			   .ToListAsync();

			for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
			{
				requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
					.Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

				requestBudgetForecastMaterial.AppStateId = appState.Id;

				_context.Update(requestBudgetForecastMaterial);
				_context.SaveChanges();
			}


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

			_context.SaveChanges();

			return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("rejectmobilelevelB1/{guid}")]
		public async Task<CreateAssetSAPResult> OrderMobileRejectB1(Guid guid)
		{
			Model.Order order = null;
			Model.OrderOp orderOp = null;
			Model.Document document = null;
			Model.EmailOrderStatus emailOrderStatus = null;
			Model.AppState appState = null;
			Model.DocumentType documentType = null;
			Model.Matrix matrix = null;
			Model.Employee employee = null;
			Model.ApplicationUser applicationUser = null;
			Model.AppState appStateRequest = null;
			List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
			List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
			Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;

			string documentTypeCode = String.Empty;

			documentTypeCode = "REJECT_ORDER_LEVELB1";
			documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).FirstOrDefaultAsync();
			if (documentType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip document!" };

			appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELB1").FirstOrDefaultAsync();
			if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel B1!" };

			appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVELB1").FirstOrDefaultAsync();
			if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel B1!" };

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

			order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
			if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
			emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
			if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
			matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
			if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
			employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeL4Id).FirstOrDefaultAsync();
			if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
			applicationUser = await userManager.FindByEmailAsync(employee.Email);

			if (applicationUser == null)
			{
				applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
				if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
			}

			appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVELB1").FirstOrDefaultAsync();
			if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel B1!" };

			order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
			emailOrderStatus.EmployeeB1ValidateAt = DateTime.Now;
			emailOrderStatus.EmployeeB1ValidateBy = applicationUser.Id;
			emailOrderStatus.ModifiedAt = DateTime.Now;
			emailOrderStatus.ModifiedBy = applicationUser.Id;

			_context.Update(emailOrderStatus);

			requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
			   .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
			   .ToListAsync();

			for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
			{
				requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
					.Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

				requestBudgetForecastMaterial.AppStateId = appState.Id;

				_context.Update(requestBudgetForecastMaterial);
				_context.SaveChanges();
			}


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

			_context.SaveChanges();

			return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
		}

		[AllowAnonymous]
        [HttpPost]
        [Route("validatemobilelevel4/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileValidateL4(Guid guid)
        {
            Model.Order order = null;
            Model.Employee employeeNull = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.EmailType emailType = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;

            string documentTypeCode = String.Empty;

            documentTypeCode = "VALIDATE_ORDER_LEVEL4";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).FirstOrDefaultAsync();
            if (documentType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip document!" };

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            employeeNull = await _context.Set<Model.Employee>().Where(e => (e.InternalCode == "-" && e.IsDeleted == true)).FirstOrDefaultAsync();

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if(order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeL4Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if(applicationUser == null)
			{
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

			if (!emailOrderStatus.EmployeeS3EmailSkip && order.EmployeeS3Id != employeeNull.Id)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS3").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel S3!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS3").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel S3!" };

				emailOrderStatus.NotEmployeeS3Sync = true;
				emailOrderStatus.SyncEmployeeS3ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeL3EmailSkip && order.EmployeeL3Id != employeeNull.Id)
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL3").FirstOrDefaultAsync();
                if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel L3!" };
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL3").FirstOrDefaultAsync();
                if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel L3!" };

                emailOrderStatus.NotEmployeeL3Sync = true;
                emailOrderStatus.SyncEmployeeL3ErrorCount = 0;
            }
			else if (!emailOrderStatus.EmployeeS2EmailSkip && order.EmployeeS2Id != employeeNull.Id)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS2").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel S2!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS2").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel S2!" };

				emailOrderStatus.NotEmployeeS2Sync = true;
				emailOrderStatus.SyncEmployeeS2ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeL2EmailSkip && order.EmployeeL2Id != employeeNull.Id)
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL2").FirstOrDefaultAsync();
                if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel L2!" };
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL2").FirstOrDefaultAsync();
                if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel L2!" };

                emailOrderStatus.NotEmployeeL2Sync = true;
                emailOrderStatus.SyncEmployeeL2ErrorCount = 0;
            }
            else if (!emailOrderStatus.EmployeeL1EmailSkip && order.EmployeeL1Id != employeeNull.Id)
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL1").FirstOrDefaultAsync();
                if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel L1!" };
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL1").FirstOrDefaultAsync();
                if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel L1!" };

                emailOrderStatus.NotEmployeeL1Sync = true;
                emailOrderStatus.SyncEmployeeL1ErrorCount = 0;
            }
            else if (!emailOrderStatus.EmployeeS1EmailSkip && order.EmployeeS1Id != employeeNull.Id)
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS1").FirstOrDefaultAsync();
                if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel S1!" };
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS1").FirstOrDefaultAsync();
                if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel S1!" };

                emailOrderStatus.NotEmployeeS1Sync = true;
                emailOrderStatus.SyncEmployeeS1ErrorCount = 0;
            }
            else
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "VALIDATED_OFFER").FirstOrDefaultAsync();
                if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email validare!" };
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ACCEPTED").FirstOrDefaultAsync();
                if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare acceptare!" };

				emailOrderStatus.EmailSend = false;
				emailOrderStatus.NotCompletedSync = true;
			}


            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmailTypeId = emailType.Id;
            emailOrderStatus.EmployeeL4ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeL4ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;

            _context.Update(emailOrderStatus);

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("rejectmobilelevel4/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileRejectL4(Guid guid)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;

            string documentTypeCode = String.Empty;

            documentTypeCode = "REJECT_ORDER_LEVEL4";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).FirstOrDefaultAsync();
            if (documentType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip document!" };

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVEL4").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeL4Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVEL4").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmployeeL4ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeL4ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;

            _context.Update(emailOrderStatus);

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("validatemobilelevel3/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileValidateL3(Guid guid)
        {
            Model.Order order = null;
            Model.Employee employeeNull = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.EmailType emailType = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            string documentTypeCode = String.Empty;

            documentTypeCode = "VALIDATE_ORDER_LEVEL3";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).SingleAsync();

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL3").FirstOrDefaultAsync();

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            employeeNull = await _context.Set<Model.Employee>().Where(e => (e.InternalCode == "-" && e.IsDeleted == true)).FirstOrDefaultAsync();

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeL3Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

			if (!emailOrderStatus.EmployeeS2EmailSkip && order.EmployeeS2Id != employeeNull.Id)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS2").SingleAsync();
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS2").SingleAsync();

				emailOrderStatus.NotEmployeeS2Sync = true;
				emailOrderStatus.SyncEmployeeS2ErrorCount = 0;
			}
			if (!emailOrderStatus.EmployeeL2EmailSkip && order.EmployeeL2Id != employeeNull.Id)
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL2").SingleAsync();
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL2").SingleAsync();

                emailOrderStatus.NotEmployeeL2Sync = true;
                emailOrderStatus.SyncEmployeeL2ErrorCount = 0;
            }
            else if (!emailOrderStatus.EmployeeL1EmailSkip && order.EmployeeL1Id != employeeNull.Id)
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL1").SingleAsync();
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL1").SingleAsync();

                emailOrderStatus.NotEmployeeL1Sync = true;
                emailOrderStatus.SyncEmployeeL1ErrorCount = 0;
            }
            else if (!emailOrderStatus.EmployeeS1EmailSkip && order.EmployeeL1Id != employeeNull.Id)
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS1").SingleAsync();
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS1").SingleAsync();

                emailOrderStatus.NotEmployeeS1Sync = true;
                emailOrderStatus.SyncEmployeeS1ErrorCount = 0;
            }
            else
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "VALIDATED_OFFER").SingleAsync();
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ACCEPTED").SingleAsync();

				emailOrderStatus.EmailSend = false;
				emailOrderStatus.NotCompletedSync = true;
			}


            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmailTypeId = emailType.Id;
            emailOrderStatus.EmployeeL3ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeL3ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }

            _context.Update(emailOrderStatus);


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("rejectmobilelevel3/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileRejectL3(Guid guid)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;

            string documentTypeCode = String.Empty;

            documentTypeCode = "REJECT_ORDER_LEVEL3";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).FirstOrDefaultAsync();
            if (documentType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip document!" };

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL3").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVEL3").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeL3Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVEL3").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 3!" };

            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmployeeL3ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeL3ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;

            _context.Update(emailOrderStatus);

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("validatemobilelevel2/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileValidateL2(Guid guid)
        {
            Model.Order order = null;
            Model.Employee employeeNull = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.EmailType emailType = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            string documentTypeCode = String.Empty;

            documentTypeCode = "VALIDATE_ORDER_LEVEL2";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).SingleAsync();

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL2").FirstOrDefaultAsync();

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            employeeNull = await _context.Set<Model.Employee>().Where(e => (e.InternalCode == "-" && e.IsDeleted == true)).FirstOrDefaultAsync();

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeL2Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

            if (!emailOrderStatus.EmployeeL1EmailSkip && order.EmployeeL1Id != employeeNull.Id)
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL1").SingleAsync();
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL1").SingleAsync();

                emailOrderStatus.NotEmployeeL1Sync = true;
                emailOrderStatus.SyncEmployeeL1ErrorCount = 0;
            }
            else if (!emailOrderStatus.EmployeeS1EmailSkip && order.EmployeeS1Id != employeeNull.Id)
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS1").SingleAsync();
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS1").SingleAsync();

                emailOrderStatus.NotEmployeeS1Sync = true;
                emailOrderStatus.SyncEmployeeS1ErrorCount = 0;
            }
            else
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "VALIDATED_OFFER").SingleAsync();
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ACCEPTED").SingleAsync();

				emailOrderStatus.EmailSend = false;
				emailOrderStatus.NotCompletedSync = true;
			}


            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmailTypeId = emailType.Id;
            emailOrderStatus.EmployeeL2ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeL2ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }

            _context.Update(emailOrderStatus);


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("rejectmobilelevel2/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileRejectL2(Guid guid)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            string documentTypeCode = String.Empty;

            documentTypeCode = "REJECT_ORDER_LEVEL2";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).FirstOrDefaultAsync();
            if (documentType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip document!" };

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL2").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 2!" };

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVEL2").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeL2Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVEL2").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmployeeL2ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeL2ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;

            _context.Update(emailOrderStatus);

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("validatemobilelevel1/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileValidateL1(Guid guid)
        {
            //var userName = HttpContext.User.Identity.Name;
            //var user = await userManager.FindByEmailAsync(userName);
            //if (user == null)
            //{
            //    user = await userManager.FindByNameAsync(userName);
            //}
            //_context.UserId = user.Id.ToString();
            Model.Order order = null;
            Model.Employee employeeNull = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.EmailType emailType = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            string documentTypeCode = String.Empty;

            documentTypeCode = "VALIDATE_ORDER_LEVEL1";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).SingleAsync();

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL1").FirstOrDefaultAsync();

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            employeeNull = await _context.Set<Model.Employee>().Where(e => (e.InternalCode == "-" && e.IsDeleted == true)).FirstOrDefaultAsync();

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeL1Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

            if (!emailOrderStatus.EmployeeS1EmailSkip && order.EmployeeS1Id != employeeNull.Id)
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS1").SingleAsync();
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS1").SingleAsync();

                emailOrderStatus.NotEmployeeS1Sync = true;
                emailOrderStatus.SyncEmployeeS1ErrorCount = 0;
            }
            else
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "VALIDATED_OFFER").SingleAsync();
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ACCEPTED").SingleAsync();

				emailOrderStatus.EmailSend = false;
				emailOrderStatus.NotCompletedSync = true;
			}


            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmailTypeId = emailType.Id;
            emailOrderStatus.EmployeeL1ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeL1ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }

            _context.Update(emailOrderStatus);


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("rejectmobilelevel1/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileRejectL1(Guid guid)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            string documentTypeCode = String.Empty;

            documentTypeCode = "REJECT_ORDER_LEVEL1";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).FirstOrDefaultAsync();
            if (documentType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip document!" };

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL1").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 1!" };

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVEL1").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeL1Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVEL1").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 1!" };

            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmployeeL1ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeL1ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;

            _context.Update(emailOrderStatus);

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("validatemobilelevelS1/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileValidateS1(Guid guid)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.EmailType emailType = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            string documentTypeCode = String.Empty;

            documentTypeCode = "VALIDATE_ORDER_LEVELS1";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).SingleAsync();

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS1").FirstOrDefaultAsync();

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeS1Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

            emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "VALIDATED_OFFER").SingleAsync();
            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ACCEPTED").SingleAsync();

            //emailOrderStatus.NotEmployeeS1Sync = true;
            //emailOrderStatus.SyncEmployeeS1ErrorCount = 0;


            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmailTypeId = emailType.Id;
            emailOrderStatus.EmployeeS1ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeS1ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;
			emailOrderStatus.EmailSend = false;
			emailOrderStatus.NotCompletedSync = true;


			_context.Update(emailOrderStatus);

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("rejectmobilelevelS1/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileRejectS1(Guid guid)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            string documentTypeCode = String.Empty;

            documentTypeCode = "REJECT_ORDER_LEVELS1";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).FirstOrDefaultAsync();
            if (documentType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip document!" };

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS1").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel S1!" };

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVELS1").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeS1Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVELS1").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel S1!" };

            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmployeeS1ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeS1ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;

            _context.Update(emailOrderStatus);

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("validatemobilelevelS2/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileValidateS2(Guid guid)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.EmailType emailType = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            string documentTypeCode = String.Empty;

            documentTypeCode = "VALIDATE_ORDER_LEVELS2";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).SingleAsync();

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS2").FirstOrDefaultAsync();

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS2").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeS2Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

			if (!emailOrderStatus.EmployeeL2EmailSkip)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL2").SingleAsync();
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL2").SingleAsync();

				emailOrderStatus.NotEmployeeL2Sync = true;
				emailOrderStatus.SyncEmployeeL2ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeL1EmailSkip)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL1").SingleAsync();
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL1").SingleAsync();

				emailOrderStatus.NotEmployeeL1Sync = true;
				emailOrderStatus.SyncEmployeeL1ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeS1EmailSkip)
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS1").SingleAsync();
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS1").SingleAsync();

                emailOrderStatus.NotEmployeeS1Sync = true;
                emailOrderStatus.SyncEmployeeS1ErrorCount = 0;
            }
            else
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "VALIDATED_OFFER").SingleAsync();
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ACCEPTED").SingleAsync();

				emailOrderStatus.EmailSend = false;
				emailOrderStatus.NotCompletedSync = true;
			}


            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmailTypeId = emailType.Id;
            emailOrderStatus.EmployeeS2ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeS2ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;



            _context.Update(emailOrderStatus);

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("rejectmobilelevelS2/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileRejectS2(Guid guid)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            string documentTypeCode = String.Empty;

            documentTypeCode = "REJECT_ORDER_LEVELS2";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).FirstOrDefaultAsync();
            if (documentType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip document!" };

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS2").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 2!" };

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVELS2").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeS2Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVELS2").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 2!" };

            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmployeeS2ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeS2ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;

            _context.Update(emailOrderStatus);

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("validatemobilelevelS3/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileValidateS3(Guid guid)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.EmailType emailType = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            string documentTypeCode = String.Empty;

            documentTypeCode = "VALIDATE_ORDER_LEVELS3";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).SingleAsync();

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS3").FirstOrDefaultAsync();

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeL4Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

			
			if (!emailOrderStatus.EmployeeL3EmailSkip)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL3").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel L3!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL3").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel L3!" };

				emailOrderStatus.NotEmployeeL3Sync = true;
				emailOrderStatus.SyncEmployeeL3ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeS2EmailSkip)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS2").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel S2!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS2").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel S2!" };

				emailOrderStatus.NotEmployeeS2Sync = true;
				emailOrderStatus.SyncEmployeeS2ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeL2EmailSkip)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL2").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel L2!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL2").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel L2!" };

				emailOrderStatus.NotEmployeeL2Sync = true;
				emailOrderStatus.SyncEmployeeL2ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeL1EmailSkip)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVEL1").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel L1!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL1").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel L1!" };

				emailOrderStatus.NotEmployeeL1Sync = true;
				emailOrderStatus.SyncEmployeeL1ErrorCount = 0;
			}
			else if (!emailOrderStatus.EmployeeS1EmailSkip)
			{
				emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_VALIDATE_LEVELS1").FirstOrDefaultAsync();
				if (emailType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip email nivel S1!" };
				appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS1").FirstOrDefaultAsync();
				if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel S1!" };

				emailOrderStatus.NotEmployeeS1Sync = true;
				emailOrderStatus.SyncEmployeeS1ErrorCount = 0;
			}
            else
            {
                emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "VALIDATED_OFFER").SingleAsync();
                appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ACCEPTED").SingleAsync();

				emailOrderStatus.EmailSend = false;
				emailOrderStatus.NotCompletedSync = true;
			}


            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmailTypeId = emailType.Id;
            emailOrderStatus.EmployeeS3ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeS3ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;



            _context.Update(emailOrderStatus);

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }


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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("rejectmobilelevelS3/{guid}")]
        public async Task<CreateAssetSAPResult> OrderMobileRejectS3(Guid guid)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            Model.Matrix matrix = null;
            Model.Employee employee = null;
            Model.ApplicationUser applicationUser = null;
            Model.AppState appStateRequest = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            string documentTypeCode = String.Empty;

            documentTypeCode = "REJECT_ORDER_LEVELS3";
            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).FirstOrDefaultAsync();
            if (documentType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista tip document!" };

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELS3").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 3!" };

            appStateRequest = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVELS3").FirstOrDefaultAsync();
            if (appStateRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 4!" };

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

            order = await _context.Set<Model.Order>().Where(a => a.Guid == guid && a.AppStateId == appState.Id).FirstOrDefaultAsync();
            if (order == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(a => a.OrderId == order.Id).LastOrDefaultAsync();
            if (emailOrderStatus == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Comanda a fost deja validata!" };
            matrix = await _context.Set<Model.Matrix>().Where(a => a.Id == emailOrderStatus.MatrixId).FirstOrDefaultAsync();
            if (matrix == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            employee = await _context.Set<Model.Employee>().Where(a => a.Id == matrix.EmployeeS3Id).FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista matrice de aprobare!" };
            applicationUser = await userManager.FindByEmailAsync(employee.Email);

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByEmailAsync("admin@optima.ro");
                if (applicationUser == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista user!" };
            }

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "REJECT_ORDER_LEVELS3").FirstOrDefaultAsync();
            if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Nu exista stare nivel 3!" };

            order.AppStateId = appState.Id;
			order.EndDate = DateTime.Now;
			emailOrderStatus.AppStateId = appState.Id;
            emailOrderStatus.EmployeeS3ValidateAt = DateTime.Now;
            emailOrderStatus.EmployeeS3ValidateBy = applicationUser.Id;
            emailOrderStatus.ModifiedAt = DateTime.Now;
            emailOrderStatus.ModifiedBy = applicationUser.Id;

            _context.Update(emailOrderStatus);

            requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
               .Where(a => a.OrderId == order.Id && a.AppStateId == appStateRequest.Id && a.IsDeleted == false)
               .ToListAsync();

            for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Where(r => r.Id == requestBudgetForecastMaterials[i].Id).SingleAsync();

                requestBudgetForecastMaterial.AppStateId = appState.Id;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();
            }

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

            _context.SaveChanges();

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"Comanda a fost validata cu success!" };
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("validatepreAmount1/{appStateId}")]
        public async Task<IActionResult> OrderValidatePreAmount1([FromBody] int[] orders, int appStateId)
        {
            await Task.Delay(0);
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;

            string documentTypeCode = "VALIDATE_ORDER_LEVEL4";



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
                order = _context.Set<Model.Order>().Include(a => a.OrderType).Where(a => a.Id == orders[i]).SingleOrDefault();
                order.AppStateId = 7;
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
                    BudgetStateId = 7,
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

                _context.SaveChanges();
            }

            //await SendValidatedEmail(order.Id);

            return Ok(StatusCode(200));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("validatepreAmount2/{appStateId}")]
        public async Task<IActionResult> OrderValidatePreAmount2([FromBody] int[] orders, int appStateId)
        {
            await Task.Delay(0);
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;

            string documentTypeCode = "VALIDATE_ORDER_LEVEL4";



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
                order = _context.Set<Model.Order>().Include(a => a.OrderType).Where(a => a.Id == orders[i]).SingleOrDefault();
                order.AppStateId =  7;
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
                    BudgetStateId = 7,
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

                _context.SaveChanges();
            }

            //await SendValidatedEmail(order.Id);

            return Ok(StatusCode(200));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("validateneedcontract/{appStateId}")]
        public async Task<IActionResult> OrderValidateNeedContract([FromBody] int[] orders, int appStateId)
        {
            await Task.Delay(0);
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.EmailType emailType = null;
            Model.EntityType entityType = null;
            Model.AppState appState = null;

            string documentTypeCode = "VALIDATE_ORDER_LEVEL4";
            var userName = HttpContext.User.Identity.Name;

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELB1").SingleAsync();

			var user = await userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await userManager.FindByNameAsync(userName);
            }
            _context.UserId = user.Id.ToString();

            var documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).SingleAsync();


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
                order = await _context.Set<Model.Order>().Include(o => o.OrderMaterials).Include(a => a.OrderType).Where(a => a.Id == orders[i]).SingleOrDefaultAsync();

                Model.Offer offer = await _context.Set<Model.Offer>().Where(c => c.Id == order.OfferId).SingleAsync();
                decimal valueRon = order.OrderMaterials.Sum(a => a.ValueRon);

                var result = await GetMatrixData(offer.DivisionId.Value, valueRon);

                if(result.Count > 0)
				{

					order.EmployeeB1Id = result.ElementAt(0).EmployeeB1.Validate ? result.ElementAt(0).EmployeeB1.Id : null;
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

                    emailType = _context.Set<Model.EmailType>().Where(c => c.Code == "ORDER_BOOK").Single();
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
						EmployeeB1EmailSend = false,
						EmployeeB1ValidateAt = null,
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
						NotEmployeeB1Sync = true,
						NotEmployeeL1Sync = false,
                        NotEmployeeL2Sync = false,
                        NotEmployeeL3Sync = false,
                        NotEmployeeL4Sync = false,
                        NotEmployeeS1Sync = false,
                        NotEmployeeS2Sync = false,
                        NotEmployeeS3Sync = false,
                        NotSync = false,
                        OrderId = order.Id,
                        SyncCompletedErrorCount = 0,
						SyncEmployeeB1ErrorCount = 0,
						SyncEmployeeL1ErrorCount = 0,
                        SyncEmployeeL2ErrorCount = 0,
                        SyncEmployeeL3ErrorCount = 0,
                        SyncEmployeeL4ErrorCount = 0,
                        SyncEmployeeS1ErrorCount = 0,
                        SyncEmployeeS2ErrorCount = 0,
                        SyncEmployeeS3ErrorCount = 0,
                        SyncErrorCount = 0,
						EmployeeB1EmailSkip = !result.ElementAt(0).EmployeeB1.Validate,
						EmployeeL1EmailSkip = !result.ElementAt(0).EmployeeL1.Validate,
                        EmployeeL2EmailSkip = !result.ElementAt(0).EmployeeL2.Validate,
                        EmployeeL3EmailSkip = !result.ElementAt(0).EmployeeL3.Validate,
                        EmployeeL4EmailSkip = !result.ElementAt(0).EmployeeL4.Validate,
                        EmployeeS1EmailSkip = !result.ElementAt(0).EmployeeS1.Validate,
                        EmployeeS2EmailSkip = !result.ElementAt(0).EmployeeS2.Validate,
                        EmployeeS3EmailSkip = !result.ElementAt(0).EmployeeS3.Validate,
						PriorityL4 = result.ElementAt(0).PriorityL4,
						PriorityL3 = result.ElementAt(0).PriorityL3,
						PriorityL2 = result.ElementAt(0).PriorityL2,
						PriorityL1 = result.ElementAt(0).PriorityL1,
						PriorityS1 = result.ElementAt(0).PriorityS1,
						PriorityS2 = result.ElementAt(0).PriorityS2,
						PriorityS3 = result.ElementAt(0).PriorityS3,
					};

                    _context.Add(emailOrderStatus);
                    _context.SaveChanges();
                }
            }

            return Ok(StatusCode(200));
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("total/{accMonthId}")]
        public virtual IActionResult GetPieChartDetails(int accMonthId)
        {
            List<Model.BudgetTotalProcentage> items = _context.Set<Model.BudgetTotalProcentage>().FromSql("OrderTotal {0}", accMonthId).ToList();

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("auditBudgetChart/{accMonthId}")]
        public virtual IActionResult GetAuditLocation(int accMonthId)
        {
            List<Model.Audit> items = _context.Set<Model.Audit>().FromSql("AuditBudget {0}", accMonthId).ToList();

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("company/{accMonthId}/{companyId}")]
        public virtual IActionResult GetRegionDetails(int accMonthId, int companyId)
        {
            List<Model.BudgetCompanyProcentage> items = _context.Set<Model.BudgetCompanyProcentage>().FromSql("OrderReportByCompany {0}, {1}", accMonthId > 0 ? accMonthId : 29, companyId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetCompanyProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("project/{accMonthId}/{projectId}")]
        public virtual IActionResult GetProjectDetails(int accMonthId, int projectId)
        {
            List<Model.BudgetProjectProcentage> items = _context.Set<Model.BudgetProjectProcentage>().FromSql("BudgetReportByProject {0}, {1}", accMonthId > 0 ? accMonthId : 29, projectId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetProjectProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }


        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("partner/{accMonthId}/{partnerId}")]
        public virtual IActionResult GetPartnerDetails(int accMonthId, int partnerId)
        {
            List<Model.BudgetProjectProcentage> items = _context.Set<Model.BudgetProjectProcentage>().FromSql("OrderReportByPartner {0}, {1}", accMonthId > 0 ? accMonthId : 29, partnerId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetProjectProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("masterType/{accMonthId}/{masterTypeId}")]
        public virtual IActionResult GetMasterTypeDetails(int accMonthId, int masterTypeId)
        {
            List<Model.BudgetCostCenterProcentage> items = _context.Set<Model.BudgetCostCenterProcentage>().FromSql("OrderReportByMasterType {0}, {1}", accMonthId > 0 ? accMonthId : 29, masterTypeId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetCostCenterProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("costcenter/{accMonthId}/{costCenterId}")]
        public virtual IActionResult GetCostCenterDetails(int accMonthId, int costCenterId)
        {
            List<Model.BudgetCostCenterProcentage> items = _context.Set<Model.BudgetCostCenterProcentage>().FromSql("BudgetReportByCostCenter {0}, {1}", accMonthId > 0 ? accMonthId : 29, costCenterId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetCostCenterProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("expencetype/{accMonthId}/{typeId}")]
        public virtual IActionResult GetExpenceTypeDetails(int accMonthId, int typeId)
        {
            List<Model.BudgetExpenceTypeProcentage> items = _context.Set<Model.BudgetExpenceTypeProcentage>().FromSql("OrderReportByExpenceType {0}, {1}", accMonthId > 0 ? accMonthId : 29, typeId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetExpenceTypeProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("employee/{accMonthId}/{employeeId}")]
        public virtual IActionResult GetEmployeeDetails(int accMonthId, int employeeId)
        {
            List<Model.BudgetEmployeeProcentage> items = _context.Set<Model.BudgetEmployeeProcentage>().FromSql("OrderReportByEmployee {0}, {1}", accMonthId > 0 ? accMonthId : 29, employeeId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetEmployeeProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        //[Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("subtype/{accMonthId}/{subTypeId}")]
        public virtual IActionResult GetSubTypeDetails(int accMonthId, int subTypeId)
        {
            List<Model.BudgetSubTypeProcentage> items = _context.Set<Model.BudgetSubTypeProcentage>().FromSql("OrderReportBySubType {0}, {1}", accMonthId > 0 ? accMonthId : 29, subTypeId).ToList();

            if (items.Count == 0)
            {
                var item = new Model.BudgetSubTypeProcentage();
                item.Name = "Nu exista active";
                item.Code = "Nu exista active";
                item.Procentage = 0;
                item.Total = 0;
                item.Approved = 0;
                item.Denied = 0;
                item.Waiting = 0;

                items.Add(item);
            }

            return Ok(items);
        }

        [Route("needbgt")]
        public async Task<bool> NeedBudget(int orderId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            var files = new FormFileCollection();
            List<Model.OrderMaterial> orderMaterials = null;
            Model.AppState appState = null;
            Model.Order order = null;
            order = await _context.Set<Model.Order>()
                .Include(o => o.Offer).ThenInclude(r => r.Request)
                .Include(o => o.Offer).ThenInclude(r => r.AssetType)
                .Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
                .Include(o => o.Offer).ThenInclude(r => r.Region)
                .Include(c => c.Uom)
                 .Include(c => c.Partner)
                .Include(c => c.Company)
                .Include(c => c.CostCenter)
                .Include(f => f.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Employee)
                .Include(f => f.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
                .Include(f => f.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Activity)
                .Include(f => f.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Division)
                .Include(f => f.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Department)
                .Include(f => f.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
                .Include(f => f.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
                .Include(c => c.Employee)
                .Include(c => c.Project)
                .Include(c => c.OrderType).AsNoTracking().Where(a => a.Id == orderId).SingleOrDefaultAsync();
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
                                                          font-size: 10px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 13px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 10px;
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
                                                              font-size: 13px;
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
                                                              font-size: 13px;
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
                                        <th colspan=""11"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">SOLICITARE SUPLIMENTARE BUGET</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Solicitant</th>
                                        <th colspan=""4"">Solicitare suplimentare buget RON</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", order.CreatedAt)}</th>
										<th>{String.Format("{0:MM}", order.CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", order.CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Cod ticket</th>
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
										<th colspan = ""11""></th>
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

            orderMaterials = await _context.Set<Model.OrderMaterial>().Include(a => a.Order).ThenInclude(a=> a.Offer).ThenInclude(a => a.Request).Include(o => o.Order).ThenInclude(a => a.Uom).Include(a => a.Material).Include(o => o.Order).ThenInclude(a => a.OrderType).Where(a => a.IsDeleted == false && a.OrderId == order.Id).ToListAsync();

            int index = 0;

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/ofa";

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "NEED_BUDGET").SingleAsync();

           

            string empIni = order.Employee != null ? order.Employee.FirstName + " " + order.Employee.LastName : "";


                htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{order.Employee.Email}</th>
                                                <th rowspan=""2"" colspan=""4"">{String.Format("{0:#,##0.##}", order.BudgetValueNeed)}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

                htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""11"" style=""color: #ffffff;"">Detalii OFERTA</th>
												</tr>";
                htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Furnizor</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Companie</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Tip</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">PC</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">PC Det</th>
												</tr>";

                htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"">{order.Partner.Name}</th>
												<th>{order.Company.Code}</th>
												<th>{order.CostCenter.Code}</th>
												<th>{order.Offer.AssetType.Name}</th>
                                                <th>{order.Offer.AdmCenter.Code}</th>
                                                <th colspan=""2"">{order.Offer.Region.Code}</th>
												</tr>";

                htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""11"" style=""color: #ffffff;"">Detalii BUGET</th>
												</tr>";

                htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Cod OPTIMA</th>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">WBS</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Activity</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Buget disponibil RON</th>
												<th rowspan=""2"" colspan=""2"">
												</th>
												</tr>";

                htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th >{order.BudgetForecast.BudgetBase.Code}</th>
												<th colspan=""2"">{order.BudgetForecast.BudgetBase.Employee.Email}</th>
												<th>{order.BudgetForecast.BudgetBase.Project.Code}</th>
                                                <th>{order.BudgetForecast.BudgetBase.Activity.Name}</th>
												<th>{order.BudgetForecast.BudgetBase.Info}</th>
                                                <th>{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon) - order.BudgetValueNeed)}</th>
												</th>
												</tr>";


            subject = "Suplimentare buget pentru comanda: " + order.Code + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{order.Code}</th>";



			for (int i = 0; i < orderMaterials.Count; i++)
			{
                index++;
                var wip = orderMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th>{orderMaterials[i].Order.OrderType.Name}</th>
									<th>{wip}</th>
									<th>{orderMaterials[i].Material.Code}</th>
                                    <th>{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th>{orderMaterials[i].Order.Uom.Code}</th>
									<th>{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th>{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th>{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th>{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
                if(index == orderMaterials.Count)
				{
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""4""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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


            emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == order.EmployeeId).Select(a => a.Guid).ToList();

            if (emails1.Count > 0)
            {
                emails.Add(emails1.ElementAt(0));
            }

            for (int e = 0; e < emails.Count; e++)
            {
                var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

                if (emp.Email != null && emp.Email != "")
                {
                    to.Add(emp.Email);
                }
                else
                {
                    to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
                }
            }

            to = new List<string>();

            to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
            //to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
            //to.Add("silvia.damian@emag.ro");

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
        public async Task<bool> NeedBudgetResponse(int orderId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            var files = new FormFileCollection();
            List<Model.OrderMaterial> orderMaterials = null;
            Model.AppState appState = null;
            Model.Order order = null;
            order = await _context.Set<Model.Order>()
                .Include(o => o.Offer).ThenInclude(r => r.Request)
                .Include(o => o.Offer).ThenInclude(r => r.AssetType)
                .Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
                .Include(o => o.Offer).ThenInclude(r => r.Region)
                .Include(c => c.Uom)
                 .Include(c => c.Partner)
                .Include(c => c.Company)
                .Include(c => c.CostCenter)
                .Include(c => c.BudgetBase).ThenInclude(e => e.Employee)
                .Include(c => c.BudgetBase).ThenInclude(e => e.Project)
                .Include(c => c.BudgetBase).ThenInclude(e => e.Activity)
                .Include(c => c.BudgetBase).ThenInclude(e => e.Division)
                .Include(c => c.BudgetBase).ThenInclude(e => e.Department)
                .Include(c => c.BudgetBase).ThenInclude(e => e.Project)
                .Include(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
                .Include(c => c.Employee)
                .Include(c => c.Project)
                .Include(c => c.OrderType).AsNoTracking().Where(a => a.Id == orderId).SingleOrDefaultAsync();
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
                                                          font-size: 10px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 13px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 10px;
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
                                                              font-size: 13px;
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
                                                              font-size: 13px;
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
                                        <th colspan=""9"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">CONFIRMARE INCARCARE SUPLIMENTARE BUGET</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Solicitant</th>
                                        <th colspan=""4"">Buget disponibil incarcat RON</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", order.CreatedAt)}</th>
										<th>{String.Format("{0:MM}", order.CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", order.CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Cod ticket</th>
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
										<th colspan = ""11""></th>
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

            orderMaterials = await _context.Set<Model.OrderMaterial>().Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request).Include(o => o.Order).ThenInclude(a => a.Uom).Include(a => a.Material).Include(o => o.Order).ThenInclude(a => a.OrderType).Where(a => a.IsDeleted == false && a.OrderId == order.Id).ToListAsync();

            int index = 0;

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/ofa";

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "NEED_BUDGET").SingleAsync();



            string empIni = order.Employee != null ? order.Employee.FirstName + " " + order.Employee.LastName : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{order.Employee.Email}</th>
                                                <th rowspan=""2"" colspan=""4"">{String.Format("{0:#,##0.##}", order.BudgetValueNeed)}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""9"" style=""color: #ffffff;"">Detalii OFERTA</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Furnizor</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Companie</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Centru de cost</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Tip</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">PC</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">PC Det</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"">{order.Partner.Name}</th>
												<th>{order.Company.Code}</th>
												<th>{order.CostCenter.Code}</th>
												<th>{order.Offer.AssetType.Name}</th>
                                                <th>{order.Offer.AdmCenter.Code}</th>
                                                <th colspan=""2"">{order.Offer.Region.Code}</th>
												</tr>";

            htmlHeader8 = htmlHeader8 + $@"     
												<tr style=""background-color: #04327d;"">
												<th colspan=""9"" style=""color: #ffffff;"">Detalii BUGET</th>
												</tr>";

            htmlHeader9 = htmlHeader9 + $@"  
												<tr>
												<th style=""background-color: #6491D9;color: #ffffff;"">Cod OPTIMA</th>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Owner</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">WBS</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Activity</th>
                                                <th style=""background-color: #6491D9;color: #ffffff;"">Detalii</th>
												<th rowspan=""2"" colspan=""2"">
												</th>
												</tr>";

            htmlHeader10 = htmlHeader10 + $@"    
												<tr>
												<th >{order.BudgetBase.Code}</th>
												<th colspan=""2"">{order.BudgetBase.Employee.Email}</th>
												<th>{order.BudgetBase.Project.Code}</th>
                                                <th>{order.BudgetBase.Activity.Name}</th>
												<th>{order.BudgetBase.Info}</th>
												</th>
												</tr>";


            subject = "Suplimentare buget pentru comanda: " + order.Code + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{order.Code}</th>";



            for (int i = 0; i < orderMaterials.Count; i++)
            {
                index++;
                var wip = orderMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th>{orderMaterials[i].Order.OrderType.Name}</th>
									<th>{wip}</th>
									<th>{orderMaterials[i].Material.Code}</th>
                                    <th>{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th>{orderMaterials[i].Order.Uom.Code}</th>
									<th>{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th>{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th>{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th>{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
                if (index == orderMaterials.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""4""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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


            emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == order.EmployeeId).Select(a => a.Guid).ToList();

            if (emails1.Count > 0)
            {
                emails.Add(emails1.ElementAt(0));
            }

            for (int e = 0; e < emails.Count; e++)
            {
                var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

                if (emp.Email != null && emp.Email != "")
                {
                    to.Add(emp.Email);
                }
                else
                {
                    to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
                }
            }

            to = new List<string>();

            to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
            to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
            //to.Add("silvia.damian@emag.ro");

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

        public virtual async Task<List<Dto.Matrix>> GetMatrixData(int divisionId, decimal value)
        {
            var items = await _matrixRepository.GetMatchMatrixAsync(divisionId);

            var result = items.Select(i => _mapper.Map<Dto.Matrix>(i)).ToList();

            /*
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
            */

            if (result.Count > 0)
			{
				result.ElementAt(0).EmployeeB1.Validate = true;
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

		[Route("needcontract")]
        public async Task<bool> NeedContract(int orderId)
        {
            bool successEmail = false;
            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<Guid> emails1 = new List<Guid>();
            List<Guid> emails2 = new List<Guid>();
            List<Guid> emails = new List<Guid>();
            var files = new FormFileCollection();
            List<Model.OrderMaterial> orderMaterials = null;
            Model.AppState appState = null;
            Model.Order order = null;
            order = await _context.Set<Model.Order>()
                .Include(o => o.Offer).ThenInclude(r => r.Request)
                .Include(o => o.Offer).ThenInclude(r => r.AssetType)
                .Include(o => o.Offer).ThenInclude(r => r.AdmCenter)
                .Include(o => o.Offer).ThenInclude(r => r.Region)
                .Include(c => c.Uom)
                .Include(c => c.Partner)
                .Include(c => c.Company)
                .Include(c => c.Division)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Employee)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Activity)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Division)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Department)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.Project)
                .Include(c => c.BudgetForecast).ThenInclude(c => c.BudgetBase).ThenInclude(e => e.ProjectType)
                .Include(c => c.Employee)
                .Include(c => c.Project)
                .Include(c => c.OrderType).AsNoTracking().Where(a => a.Id == orderId).SingleOrDefaultAsync();
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
                                                          width: 80%;
                                                          text-align: center;
                                                          border-collapse: collapse;
                                                        }
                                                        table.redTable td, table.redTable th {
                                                          border: 1px solid #04327d;
                                                          padding: 3px 2px;
                                                        }
                                                        table.redTable tbody td {
                                                          font-size: 10px;
                                                        }
                                                        table.redTable tr:nth-child(even) {
                                                          background: #F5C8BF;
                                                        }
                                                        table.redTable thead {
                                                          background: #ffffff;
                                                        }
                                                        table.redTable thead th {
                                                          font-size: 13px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          text-align: center;
                                                          border-left: 2px solid #04327d;
                                                        }

                                                        table.redTable tfoot {
                                                          font-size: 10px;
                                                          font-weight: bold;
                                                          color: #04327d;
                                                          background: #ffffff;
                                                        }
                                                        table.redTable tfoot td {
                                                          font-size: 10px;
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
                                                              font-size: 13px;
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
                                                              font-size: 13px;
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
                                        <th colspan=""11"" style=""color: #ffffff; font-weight: bold;font-size: 1.3em;"">SOLICITARE CONTRACT NOU</th>                                      
									</tr>";

            var htmlHeader1 = $@"        
                                    <tr>
                                        <th rowspan=""2"">Numar comanda</th>
									    <th colspan=""3"">Data solicitare</th>
										<th colspan=""2"">Solicitant</th>
                                        <th colspan=""4"">Suma contract nou RON</th>
                                        <th></th>
									</tr>
									<tr>
                                        <th style=""background-color: #6491D9;color: #ffffff;"">Ziua</th>
									    <th style=""background-color: #6491D9;color: #ffffff;"">Luna</th>
										<th style=""background-color: #6491D9;color: #ffffff;"">Anul</th>";

            var htmlHeader11 = "";

            var htmlHeader12 = "";
            var htmlHeader13 = $@"        
									    <th>{String.Format("{0:dd}", order.CreatedAt)}</th>
										<th>{String.Format("{0:MM}", order.CreatedAt)}</th>
										<th>{String.Format("{0:yyyy}", order.CreatedAt)}</th>
									</tr>";

            var htmlHeader2 = @"      
									<tr>
                                        <th colspan=""11""></th>
									</tr>
                                    <tr style=""background-color: #6491D9;"">
                                        <th style=""color: #ffffff;"">Nr. Crt.</th>
                                        <th style=""color: #ffffff;"">Cod ticket</th>
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
										<th colspan = ""11""></th>
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

            orderMaterials = await _context.Set<Model.OrderMaterial>().Include(a => a.Order).ThenInclude(a => a.Offer).ThenInclude(a => a.Request).Include(o => o.Order).ThenInclude(a => a.Uom).Include(a => a.Material).Include(o => o.Order).ThenInclude(a => a.OrderType).Where(a => a.IsDeleted == false && a.OrderId == order.Id).ToListAsync();

            int index = 0;

            //var linkYes = "http://OFAAPIUAT/api/budgetbases/dstmanagervalidate/" + order.Guid + "/" + order.Id;
            ////var linkNo = "http://localhost:3100/#/dstmanagernotvalidate/" + employeeGuid + "/" + key;
            //var linkNo = "http://OFAUAT/#/dstmanagernotvalidate/" + order.Guid + "/" + order.Id;

            var link = "https://optima.emag.network/ofa";

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "NEED_CONTRACT").SingleAsync();



            string empIni = order.Employee != null ? order.Employee.FirstName + " " + order.Employee.LastName : "";


            htmlHeader11 = htmlHeader11 + $@"        
												<th rowspan=""2"" colspan=""2"">{order.Employee.Email}</th>
                                                <th rowspan=""2"" colspan=""4"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
                                                <th rowspan=""2"">
                                                     <a class=""button"" style=""padding: 5px 10px"" href='" + link + "'" + "' >Vezi link</a>" + "&nbsp;" + @"
												</th>";

            htmlHeader5 = htmlHeader5 + $@"
												<tr style=""background-color: #04327d;"">
												<th colspan=""11"" style=""color: #ffffff;"">Detalii OFERTA</th>
												</tr>";
            htmlHeader6 = htmlHeader6 + $@"
												<tr>
												<th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">Furnizor</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Companie</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Departament</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">Tip</th>
												<th style=""background-color: #6491D9;color: #ffffff;"">PC</th>
                                                <th colspan=""2"" style=""background-color: #6491D9;color: #ffffff;"">PC Det</th>
												</tr>";

            htmlHeader7 = htmlHeader7 + $@" 
												<tr>
												<th colspan=""2"">{(order.Partner != null ? order.Partner.Name : "")}</th>
												<th>{(order.Company != null ? order.Company.Code : "")}</th>
												<th>{(order.Division != null ? order.Division.Code : "")}</th>
												<th>{(order.Offer != null && order.Offer.AssetType != null ? order.Offer.AssetType.Name : "")}</th>
                                                <th>{(order.Offer != null && order.Offer.AdmCenter != null ? order.Offer.AdmCenter.Code : "")}</th>
                                                <th colspan=""2"">{(order.Offer != null && order.Offer.Region != null ? order.Offer.Region.Code : "")}</th>
												</tr>";


            subject = "Contract nou pentru comanda: " + order.Code + "!!";

            htmlHeader12 = htmlHeader12 + $@"        
							</tr>
							<tr>
                                <th>{order.Code}</th>";



            for (int i = 0; i < orderMaterials.Count; i++)
            {
                index++;
                var wip = orderMaterials[i].WIP ? "DA" : "NU";
                htmlHeader3 += $@"      
                                <tr>
                                    <th>{index}</th>
									<th>{orderMaterials[i].Order.Offer.Request.Code}</th>
                                    <th>{orderMaterials[i].Order.OrderType.Name}</th>
									<th>{wip}</th>
									<th>{orderMaterials[i].Material.Code}</th>
                                    <th>{String.Format("{0:#,##0.##}", orderMaterials[i].Quantity)}</th>
                                    <th>{orderMaterials[i].Order.Uom.Code}</th>
									<th>{String.Format("{0:#,##0.##}", orderMaterials[i].Price)}</th>
									<th>{String.Format("{0:#,##0.##}", orderMaterials[i].Value)}</th>
                                    <th>{String.Format("{0:#,##0.##}", orderMaterials[i].PriceRon)}</th>
                                    <th>{String.Format("{0:#,##0.##}", orderMaterials[i].ValueRon)}</th>
								</tr>";
                if (index == orderMaterials.Count)
                {
                    htmlHeader3 += $@"      
                                <tr>
                                    <th colspan=""4""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">TOTAL</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Quantity))}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.Value))}</th>
                                    <th style=""background-color: #6491D9;color: #ffffff;""></th>
                                    <th style=""background-color: #6491D9;color: #ffffff;"">{String.Format("{0:#,##0.##}", orderMaterials.Sum(a => a.ValueRon))}</th>
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


            emails1 = _context.Set<Model.Employee>().AsNoTracking().Where(a => a.Id == order.EmployeeId).Select(a => a.Guid).ToList();

            if (emails1.Count > 0)
            {
                emails.Add(emails1.ElementAt(0));
            }

            for (int e = 0; e < emails.Count; e++)
            {
                var emp = _context.Set<Model.Employee>().Where(employee => employee.Guid == emails.ElementAt(e)).Single();

                if (emp.Email != null && emp.Email != "")
                {
                    to.Add(emp.Email);
                }
                else
                {
                    to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
                }
            }

            to = new List<string>();

            //to.Add("adrian.cirnaru@optima.ro");to.Add("cosmina.pricop@optima.ro");
            //to.Add("madalina.udrea@emag.ro");to.Add("cristian.circeanu@emag.ro");
            //to.Add("silvia.damian@emag.ro");
            to.Add("gabriela.dogaru@emag.ro");
            bcc.Add("adrian.cirnaru@optima.ro");bcc.Add("cosmina.pricop@optima.ro");

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

		[HttpPost("editOrder")]
		public async Task<OrderResult> EditOrder([FromBody] OrderEdit requestDto)
		{
			Model.OrderResult orderResult = null;

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
						orderResult = await (_itemsRepository as IOrdersRepository).OrderEdit(requestDto);

						if (orderResult.Success)
						{
							return new Model.OrderResult { Success = true, Message = orderResult.Message, OrderId = orderResult.OrderId };
						}
						else
						{
							return new Model.OrderResult { Success = false, Message = orderResult.Message, OrderId = 0 };
						}
					}
					catch (Exception ex)
					{

						return new Model.OrderResult { Success = false, Message = ex.Message, OrderId = 0 };
					}

				}
				else
				{
					return new Model.OrderResult { Success = false, Message = $"Userul nu exista!", OrderId = 0 };
				}


			}
			else
			{
				return new Model.OrderResult { Success = false, Message = $"Va rugam sa va autentificati!", OrderId = 0 };
			}


		}

		[HttpPost]
		[Route("deleteOrder")]
		public async Task<OrderResult> DeleteOrder([FromBody] OrderDelete orderDto)
		{
            return await (_orderFlowService as IOrderFlowService).DeleteOrder(orderDto);
		}

		[HttpPost("updateOrderBudgetForecast")]
		public async Task<OrderResult> UpdateOrderBudgetForecast([FromBody] OrderBudgetForecastUpdate requestDto)
		{
			Model.OrderResult orderResult = null;

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
						orderResult = await (_itemsRepository as IOrdersRepository).OrderBudgetForecastUpdate(requestDto);

						if (orderResult.Success)
						{
							var countBudgetBase = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
							var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();

							return new Model.OrderResult { Success = true, Message = orderResult.Message, OrderId = orderResult.OrderId };
						}
						else
						{
							return new Model.OrderResult { Success = false, Message = orderResult.Message, OrderId = 0 };
						}
					}
					catch (Exception ex)
					{

						return new Model.OrderResult { Success = false, Message = ex.Message, OrderId = 0 };
					}

				}
				else
				{
					return new Model.OrderResult { Success = false, Message = $"Userul nu exista!", OrderId = 0 };
				}


			}
			else
			{
				return new Model.OrderResult { Success = false, Message = $"Va rugam sa va autentificati!", OrderId = 0 };
            }
        }

        [HttpGet]
        [Route("exportorderstatuspo")]
        public async Task<ActionResult> ExportOrderStatusAsync(int? orderId, int? documentId, int? assetStateId, int? partnerId)
        {
            var fileContent = await (_itemsRepository as IOrdersRepository).ExportOrderStatusAsync(orderId, documentId, assetStateId, partnerId);

            string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Response.ContentType = fileType;
            FileContentResult result = new FileContentResult(fileContent, fileType)
            {
                FileDownloadName = "PO.xlsx"
            };

            return result;
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export(string includes, string jsonFilter, string filter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            OrderFilter orderFilter = null;
            string userName = string.Empty;
            string role = string.Empty;
            string employeeId = string.Empty;

            includes = "Order.AppState,Order.OrderType,Order.EmployeeL4,Order.EmployeeL3,Order.EmployeeL2,Order.EmployeeL1,Order.EmployeeS3,Order.EmployeeS2,Order.EmployeeS1,Order.Offer.Request,Order.Contract,Order.Partner,Order.Company,Order.Employee";

            orderFilter = jsonFilter != null ? JsonConvert.DeserializeObject<OrderFilter>(jsonFilter) : new OrderFilter();

            if (filter != null)
            {
                orderFilter.Filter = filter;
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

                orderFilter.Role = role;
                orderFilter.InInventory = user.InInventory;
                orderFilter.UserId = user.Id;

                if (employeeId != null)
                {
                    orderFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    orderFilter.EmployeeIds = null;
                    orderFilter.EmployeeIds = new List<int?>();
                    orderFilter.EmployeeIds.Add(int.Parse("-1"));
                }
            }
            else
            {
                orderFilter.EmployeeIds = null;
                orderFilter.EmployeeIds = new List<int?>();
                orderFilter.EmployeeIds.Add(int.Parse("-1"));
            }


            using (ExcelPackage package = new ExcelPackage())
            {
                var items = (_itemsRepository as IOrdersRepository)
                .GetOrder(orderFilter, includes, null, null, out depTotal, out catTotal).ToList();
                var itemsResource = _mapper.Map<List<Model.OrderDetail>, List<Dto.Order>>(items);

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("P.O.");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Cod P.O.";
                worksheet.Cells[1, 2].Value = "Tip P.O.";
                worksheet.Cells[1, 3].Value = "Status";
                worksheet.Cells[1, 4].Value = "L4";
                worksheet.Cells[1, 5].Value = "L3";
                worksheet.Cells[1, 6].Value = "L2";
                worksheet.Cells[1, 7].Value = "L1";
                worksheet.Cells[1, 8].Value = "S3";
                worksheet.Cells[1, 9].Value = "S2";
                worksheet.Cells[1, 10].Value = "S1";
                worksheet.Cells[1, 11].Value = "Date creare";
                worksheet.Cells[1, 12].Value = "Data modificare";
                worksheet.Cells[1, 13].Value = "Owner";
                worksheet.Cells[1, 14].Value = "Fux start";
                worksheet.Cells[1, 15].Value = "Flux - ultima aprobare";
                worksheet.Cells[1, 16].Value = "Cod P.R.";
                worksheet.Cells[1, 17].Value = "Cod Oferta";
                worksheet.Cells[1, 18].Value = "Contract";
                worksheet.Cells[1, 19].Value = "Companie";
                worksheet.Cells[1, 20].Value = "Supplier";
                worksheet.Cells[1, 21].Value = "Info";

                int recordIndex = 2;
                foreach (var item in itemsResource)
                {
                    worksheet.Cells[recordIndex, 1].Value = item.Code;
                    worksheet.Cells[recordIndex, 2].Value = item.OrderType != null ? item.OrderType.Name : "";
                    worksheet.Cells[recordIndex, 3].Value = item.AppState != null ? item.AppState.Name : "";
                    worksheet.Cells[recordIndex, 4].Value = item.EmployeeL4 != null ? item.EmployeeL4.Email : "";
                    worksheet.Cells[recordIndex, 5].Value = item.EmployeeL3 != null ? item.EmployeeL3.Email : "";
                    worksheet.Cells[recordIndex, 6].Value = item.EmployeeL2 != null ? item.EmployeeL2.Email : "";
                    worksheet.Cells[recordIndex, 7].Value = item.EmployeeL1 != null ? item.EmployeeL1.Email : "";
                    worksheet.Cells[recordIndex, 8].Value = item.EmployeeS3 != null ? item.EmployeeS3.Email : "";
                    worksheet.Cells[recordIndex, 9].Value = item.EmployeeS2 != null ? item.EmployeeS2.Email : "";
                    worksheet.Cells[recordIndex, 10].Value = item.EmployeeS1 != null ? item.EmployeeS1.Email : "";
                    worksheet.Cells[recordIndex, 11].Value = item.CreatedAt;
                    worksheet.Cells[recordIndex, 11].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 12].Value = item.ModifiedAt;
                    worksheet.Cells[recordIndex, 12].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 13].Value = item.Employee != null ? item.Employee.Email : "";
                    worksheet.Cells[recordIndex, 14].Value = item.StartDate;
                    worksheet.Cells[recordIndex, 14].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 15].Value = item.EndDate;
                    worksheet.Cells[recordIndex, 15].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 16].Value = item.Offer != null && item.Offer.Request != null ? item.Offer.Request.Code : "";
                    worksheet.Cells[recordIndex, 17].Value = item.Offer != null ? item.Offer.Code : "";
                    worksheet.Cells[recordIndex, 18].Value = item.Contract != null ? item.Contract.ContractID : "";
                    worksheet.Cells[recordIndex, 19].Value = item.Company != null ? item.Company.Code : "";
                    worksheet.Cells[recordIndex, 20].Value = item.Partner != null ? item.Partner.Name : "";
                    worksheet.Cells[recordIndex, 21].Value = item.Info;

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
                worksheet.Column(20).AutoFit();

                using (var cells = worksheet.Cells[1, 1, 1, 20])
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
                    FileDownloadName = "P.O..xlsx"
                };

                return result;

            }
        }
    }
}
