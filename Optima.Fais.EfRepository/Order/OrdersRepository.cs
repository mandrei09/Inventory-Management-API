using System.Collections.Generic;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Linq;
using Optima.Fais.Dto;
using System;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Optima.Fais.Model.Utils;
using System.Text;
using Optima.Fais.Model;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using Optima.Fais.Model.Common;
using System.Drawing;
using Optima.Fais.Dto.Sync;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Optima.Fais.EfRepository
{
    public class OrdersRepository : Repository<Model.Order>, IOrdersRepository
    {

        public OrdersRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Partner.Name.Contains(filter) || a.Offer.Code.Contains(filter) || a.Name.Contains(filter) || a.Offer.Request.Code.Contains(filter)); })
        {
           
        }

        public IEnumerable<Model.OrderDetail> GetOrder(OrderFilter orderFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Order> budgetQuery = null;
            IQueryable<OrderDetail> query = null;

            budgetQuery = _context.Orders.AsNoTracking().AsQueryable();

            if (orderFilter.Filter != "" && orderFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Name.Contains(orderFilter.Filter) || a.Code.Contains(orderFilter.Filter) || a.Partner.Name.Contains(orderFilter.Filter) || a.Offer.Code.Contains(orderFilter.Filter) || a.Offer.Request.Code.Contains(orderFilter.Filter)));


            includes = includes ?? string.Empty;

            foreach (var includeProperty in includes.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int dotIndex = includeProperty.IndexOf(".");
                string prefix = string.Empty;
                string property = string.Empty;

                if (dotIndex > 0)
                {
                    prefix = includeProperty.Substring(0, dotIndex);
                    property = includeProperty.Length > dotIndex ? includeProperty.Substring(dotIndex + 1) : string.Empty;
                }
                else
                {
                    prefix = includeProperty;
                }


                switch (prefix)
                {
                    case "Order":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new OrderDetail { Order = budget });


            if (orderFilter.Role != null && orderFilter.Role != "")
            {
                if (orderFilter.Role.ToUpper() == "ADMINISTRATOR")
                {
                    //if ((orderFilter.CostCenterIds != null) && (orderFilter.CostCenterIds.Count > 0))
                    //{
                    //    query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => { return a => ((a.Adm.CostCenterId == id)); }, orderFilter.CostCenterIds));
                    //}

                    //if ((orderFilter.EmployeeIds != null) && (orderFilter.EmployeeIds.Count > 0))
                    //{
                    //    query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => { return a => ((a.Adm.EmployeeId == id)); }, orderFilter.EmployeeIds));
                    //}
                }
                else if (orderFilter.Role.ToUpper() == "PROCUREMENT")
                {
                    List<int?> divisionIds = new List<int?>();
                    divisionIds.Add(1482);

                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => { return a => a.Order.DivisionId != id; }, divisionIds));

                    query = query.Where(a => a.Order.Offer.AssetType.Code != "STOCK_IT");
                }
                else if (orderFilter.Role.ToUpper() == "PROC-IT")
                {
                    List<int?> divisionIds = _context.Set<Model.EmployeeDivision>().AsNoTracking().Where(e => e.EmployeeId == orderFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.DivisionId).ToList();

                    if (divisionIds.Count == 0)
                    {
                        divisionIds = new List<int?>();
                        divisionIds.Add(-1);
                    }

                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => { return a => a.Order.DivisionId == id; }, divisionIds));

                    query = query.Where(a => a.Order.Offer.AssetType.Code != "STOCK_IT");
                }
                else
                {
                    if (orderFilter.Role.ToUpper() != "ADMINISTRATOR")
                    {

                        if(orderFilter.Role.ToUpper() == "APPROVERS")
						{
                            List<int?> employeeIds = new List<int?>();
                            employeeIds.Add(orderFilter.EmployeeId);


                            query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => {
                                return a => (
                            (
                            (a.Order.EmployeeB1Id == id && a.Order.AppState.Code == "ORDER_LEVELB1") ||
                            (a.Order.EmployeeL4Id == id && a.Order.AppState.Code == "ORDER_LEVEL4") ||
                            (a.Order.EmployeeL3Id == id && a.Order.AppState.Code == "ORDER_LEVEL3") ||
                            (a.Order.EmployeeL2Id == id && a.Order.AppState.Code == "ORDER_LEVEL2") ||
                            (a.Order.EmployeeL1Id == id && a.Order.AppState.Code == "ORDER_LEVEL1") ||
                            (a.Order.EmployeeS1Id == id && a.Order.AppState.Code == "ORDER_LEVELS1") ||
                            (a.Order.EmployeeS2Id == id && a.Order.AppState.Code == "ORDER_LEVELS2") ||
                            (a.Order.EmployeeS3Id == id && a.Order.AppState.Code == "ORDER_LEVELS3")) || a.Order.AppState.Code == "NEED_CONTRACT");
                            }, employeeIds));
						}
						else
						{
                            List<int?> divisionIds = _context.Set<Model.EmployeeDivision>().AsNoTracking().Where(e => e.EmployeeId == orderFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.DivisionId).ToList();

                            if (divisionIds.Count == 0)
                            {
                                divisionIds = new List<int?>();
                                divisionIds.Add(-1);
                            }

                            query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => { return a => a.Order.DivisionId == id; }, divisionIds));
                        }
                       


                        //if ((orderFilter.CostCenterIds != null) && (orderFilter.CostCenterIds.Count > 0))
                        //{
                        //    query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => { return a => ((a.Adm.CostCenterId == id)); }, orderFilter.CostCenterIds));
                        //}

                    }
                }
            }

            //if ((orderFilter.CompanyIds != null) && (orderFilter.CompanyIds.Count > 0))
            //{
            //    query = query.Where(a => orderFilter.CompanyIds.Contains(a.Order.CompanyId));
            //}

            //if ((orderFilter.EmployeeIds != null) && (orderFilter.EmployeeIds.Count > 0))
            //{
            //    query = query.Where(a => orderFilter.EmployeeIds.Contains(a.Order.EmployeeId));
            //}

            if (orderFilter.Type != null && orderFilter.Type != "")
            {
                if (orderFilter.Type.ToUpper() == "ME")
                {
                    query = query.Where(a => a.Order.EmployeeId == orderFilter.EmployeeId);
                }
            }

            if ((orderFilter.RequestIds != null) && (orderFilter.RequestIds.Count > 0))
            {
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => { return a => a.Order.Offer.RequestId == id; }, orderFilter.RequestIds));
			}

			if ((orderFilter.EmployeeIds != null) && (orderFilter.EmployeeIds.Count > 0))
			{
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => { return a => a.Order.EmployeeId == id; }, orderFilter.EmployeeIds));
			}

			if ((orderFilter.ReqEmployeeIds != null) && (orderFilter.ReqEmployeeIds.Count > 0))
			{
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => { return a => a.Order.Offer.Request.EmployeeId == id; }, orderFilter.ReqEmployeeIds));
			}

            query = query.Where(a => a.Order.IsDeleted == false);

         

            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();
            //catTotal.AssetCategoryDeskPhone = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 42 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryMonitor = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 72 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryThinClient = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 1035 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.OrderDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.OrderDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public IEnumerable<Model.OrderDetail> GetOrderUI(OrderFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Order> budgetQuery = null;
            IQueryable<OrderDetail> query = null;

            budgetQuery = _context.Orders.AsNoTracking().AsQueryable();


            int appStateId = _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "ACCEPTED").Select(a => a.Id).SingleOrDefault();

            if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Name.Contains(budgetFilter.Filter) || a.Code.Contains(budgetFilter.Filter)));


            includes = includes ?? string.Empty;

            foreach (var includeProperty in includes.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int dotIndex = includeProperty.IndexOf(".");
                string prefix = string.Empty;
                string property = string.Empty;

                if (dotIndex > 0)
                {
                    prefix = includeProperty.Substring(0, dotIndex);
                    property = includeProperty.Length > dotIndex ? includeProperty.Substring(dotIndex + 1) : string.Empty;
                }
                else
                {
                    prefix = includeProperty;
                }


                switch (prefix)
                {
                    case "Order":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new OrderDetail { Order = budget });

            query = query.Where(a => a.Order.IsDeleted == false && a.Order.AppStateId == appStateId);



            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.OrderDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.OrderDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public IEnumerable<Model.OrderDetail> GetOrderNeedBudgetUI(OrderFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Order> budgetQuery = null;
            IQueryable<OrderDetail> query = null;

            budgetQuery = _context.Orders.AsNoTracking().AsQueryable();

            if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Name.Contains(budgetFilter.Filter) || a.Code.Contains(budgetFilter.Filter)));
            int appStateId = _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "NEED_BUDGET").Select(a => a.Id).SingleOrDefault();

            includes = includes ?? string.Empty;

            foreach (var includeProperty in includes.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int dotIndex = includeProperty.IndexOf(".");
                string prefix = string.Empty;
                string property = string.Empty;

                if (dotIndex > 0)
                {
                    prefix = includeProperty.Substring(0, dotIndex);
                    property = includeProperty.Length > dotIndex ? includeProperty.Substring(dotIndex + 1) : string.Empty;
                }
                else
                {
                    prefix = includeProperty;
                }


                switch (prefix)
                {
                    case "Order":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new OrderDetail { Order = budget });

            query = query.Where(a => a.Order.IsDeleted == false && a.Order.AppStateId == appStateId);



            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.OrderDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.OrderDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        //     public async Task<Model.OrderResult> CreateOrUpdateOrder(OrderSave budgetDto)
        //     {
        //         Model.Order order = null;
        //         Model.OrderOp orderOp = null;
        //         Model.OfferOp offerOp = null;
        //         Model.Document document = null;
        //         Model.DocumentType documentType = null;
        //         Model.EntityType entityType = null;
        //         Model.Offer offer = null;
        //         //Model.Budget budgetApproved = null;
        //         // Model.BudgetBase budgetBaseApproved = null;
        //         Model.BudgetForecast budgetForecastApproved = null;
        //         Model.Contract contract = null;
        //         Model.AppState appState = null;
        //         Model.AppState appStateOrderMaterial = null;
        //         Model.Inventory inventory = null;
        //         Model.OrderType orderType = null;
        //         Model.OrderMaterial orderMaterial = null;
        //         Model.OfferMaterial offerMaterial = null;
        //         bool isValid = false;

        //         float sum = (float)budgetDto.OrderMaterialUpdates.Sum(a => a.Quantity);
        //         decimal price = budgetDto.OrderMaterialUpdates.Sum(a => a.Price);
        //         decimal priceRon = budgetDto.OrderMaterialUpdates.Sum(a => a.PriceRon);
        //         decimal value = budgetDto.OrderMaterialUpdates.Sum(a => a.Value);
        //         decimal valueRon = budgetDto.OrderMaterialUpdates.Sum(a => a.ValueRon);
        //         //decimal valueInOtherCurrency = budgetDto.SumOfTotalInOtherCurrency;
        //         decimal budgetValueNeed = 0;
        //         decimal budgetValueNeedInOtherCurrency = 0;

        //         orderType = await _context.Set<Model.OrderType>().Where(c => c.Id == budgetDto.OrderTypeId).SingleAsync();
        //         contract = await _context.Set<Model.Contract>().Include(c => c.ContractAmount).Where(c => c.Id == budgetDto.ContractId).SingleAsync();
        //         offer = await _context.Set<Model.Offer>().Include(e => e.Request).Include(r => r.Rate).ThenInclude(u => u.Uom).Where(c => c.Id == budgetDto.OfferId).SingleAsync();
        //         //budgetApproved = _context.Set<Model.Budget>().Where(c => c.Id == budgetDto.BudgetId).SingleOrDefault();
        //         budgetForecastApproved = await _context.Set<Model.BudgetForecast>().Include(b => b.BudgetBase).Where(c => c.Id == budgetDto.BudgetForecastId).SingleAsync();


        //if (budgetDto.NeedBudgetAmount)
        //{
        //             budgetValueNeed =  value - budgetForecastApproved.TotalRem;
        //             budgetValueNeedInOtherCurrency = ( value - budgetForecastApproved.TotalRem) / offer.Rate.Value;
        //         }

        ////         if (contract.ContractAmount.AmountRonRem >= value && budgetApproved.ValueFin >= value && offer.QuantityRem >= sum)
        ////{
        ////             isValid = true;

        ////             // offer.QuantityRem = offer.QuantityRem > 0 ? offer.QuantityRem - sum : offer.Quantity;
        ////             // offer.ValueFin = offer.ValueFin > 0 ? offer.ValueFin - budgetDto.ValueIni : offer.ValueFin;

        ////             // budgetApproved.QuantityRem = budgetApproved.QuantityRem > 0 ? budgetApproved.QuantityRem - sum : budgetApproved.QuantityRem;
        ////             // budgetApproved.ValueFin = budgetApproved.ValueFin > 0 ? budgetApproved.ValueFin - value : budgetApproved.ValueFin;


        ////             //contract.ContractAmount.AmountRonUsed += value;
        ////             //contract.ContractAmount.AmountUsed += valueInOtherCurrency;
        ////         }

        //         if (orderType != null && orderType.Code != "C-IT" && (contract.ContractAmount.AmountRonRem >= valueRon && budgetForecastApproved.TotalRem >= valueRon && offer.QuantityRem >= sum))
        //         {
        //             isValid = true;

        //	// offer.QuantityRem = offer.QuantityRem > 0 ? offer.QuantityRem - sum : offer.Quantity;
        //	// offer.ValueFin = offer.ValueFin > 0 ? offer.ValueFin - budgetDto.ValueIni : offer.ValueFin;

        //	// budgetApproved.QuantityRem = budgetApproved.QuantityRem > 0 ? budgetApproved.QuantityRem - sum : budgetApproved.QuantityRem;
        //	// budgetApproved.ValueFin = budgetApproved.ValueFin > 0 ? budgetApproved.ValueFin - value : budgetApproved.ValueFin;


        //	//contract.ContractAmount.AmountRonUsed += value;
        //	//contract.ContractAmount.AmountUsed += valueInOtherCurrency;
        //}
        //else if (contract != null && contract.ContractId == "NO-C" && (budgetForecastApproved.TotalRem >= value && offer.QuantityRem >= sum))
        //{
        //             isValid = true;
        //}


        //         if (isValid || budgetDto.NeedBudgetAmount)
        //{
        //             entityType = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWORDER").SingleAsync();

        //             if(orderType.Code == "C-IT")
        //	{
        //                 appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ACCEPTED").SingleAsync();
        //             }
        //             else if (contract.ContractId == "NO-C")
        //             {
        //                 appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "NEED_CONTRACT").SingleAsync();
        //             }
        //             else
        //	{
        //                 appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL1").SingleAsync();
        //             }


        //	if (budgetDto.NeedBudgetAmount)
        //	{
        //                 appState = await _context.Set<Model.AppState>().Where(c => c.Code == "ORDER_BOOK").SingleAsync();
        //             }

        //             appStateOrderMaterial = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "PENDING").SingleAsync();

        //             if (budgetDto.NeedBudgetAmount)
        //             {
        //                 appStateOrderMaterial = await _context.Set<Model.AppState>().Where(c => c.Code == "ORDER_BOOK").SingleAsync();
        //             }


        //             var lastCode = int.Parse(entityType.Name);
        //             var newBudgetCode = string.Empty;

        //             if (lastCode.ToString().Length == 1)
        //             {
        //                 newBudgetCode = "ORD000000" + entityType.Name;
        //             }
        //             else if (lastCode.ToString().Length == 2)
        //             {
        //                 newBudgetCode = "ORD00000" + entityType.Name;
        //             }
        //             else if (lastCode.ToString().Length == 3)
        //             {
        //                 newBudgetCode = "ORD0000" + entityType.Name;
        //             }
        //             else if (lastCode.ToString().Length == 4)
        //             {
        //                 newBudgetCode = "ORD000" + entityType.Name;
        //             }
        //             else if (lastCode.ToString().Length == 5)
        //             {
        //                 newBudgetCode = "ORD00" + entityType.Name;
        //             }
        //             else if (lastCode.ToString().Length == 6)
        //             {
        //                 newBudgetCode = "ORD0" + entityType.Name;
        //             }
        //             else if (lastCode.ToString().Length == 7)
        //             {
        //                 newBudgetCode = "ORD" + entityType.Name;
        //             }


        //             documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_ORDER").SingleAsync();
        //             inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).SingleAsync();

        //             document = new Model.Document()
        //             {
        //                 Approved = true,
        //                 CompanyId = offer.CompanyId,
        //                 CostCenterId = offer.CostCenterId,
        //                 CreatedAt = DateTime.Now,
        //                 CreatedBy = _context.UserId,
        //                 CreationDate = DateTime.Now,
        //                 Details = budgetDto.Info != null ? budgetDto.Info : string.Empty,
        //                 DocNo1 = budgetDto.Info != null ? budgetDto.Info : string.Empty,
        //                 DocNo2 = budgetDto.Info != null ? budgetDto.Info : string.Empty,
        //                 DocumentDate = DateTime.Now,
        //                 DocumentTypeId = documentType.Id,
        //                 Exported = true,
        //                 IsDeleted = false,
        //                 ModifiedAt = DateTime.Now,
        //                 ModifiedBy = budgetDto.UserId,
        //                 ParentDocumentId = null,
        //                 PartnerId = offer.PartnerId,
        //                 RegisterDate = DateTime.Now,
        //                 ValidationDate = DateTime.Now
        //             };

        //             _context.Add(document);


        //             order = new Model.Order()
        //             {
        //                 AccMonthId = inventory.AccMonthId,
        //                 AccountId = offer.AccountId,
        //                 AdministrationId = offer.AdministrationId,
        //                 AppStateId = appState.Id,
        //                 BudgetManagerId = inventory.BudgetManagerId,
        //                 Code = newBudgetCode,
        //                 CompanyId = offer.CompanyId,
        //                 CostCenterId = offer.CostCenterId,
        //                 CreatedAt = DateTime.Now,
        //                 CreatedBy = _context.UserId,
        //                 EmployeeId = offer.EmployeeId,
        //                 EndDate = budgetDto.EndDate,
        //                 StartDate = DateTime.Now,
        //                 Info = offer.Info,
        //                 InterCompanyId = offer.InterCompanyId,
        //                 IsAccepted = false,
        //                 IsDeleted = false,
        //                 ModifiedAt = DateTime.Now,
        //                 ModifiedBy = _context.UserId,
        //                 Name = budgetDto.Name,
        //                 PartnerId = offer.PartnerId,
        //                 ProjectId = budgetForecastApproved.BudgetBase.ProjectId,
        //                 Quantity = sum,
        //                 SubTypeId = offer.SubTypeId,
        //                 UserId = budgetDto.UserId,
        //                 Validated = true,
        //                 ValueFin = value,
        //                 ValueFinRon = valueRon,
        //                 ValueIni = value,
        //                 ValueIniRon = valueRon,
        //                 Guid = Guid.NewGuid(),
        //                 QuantityRem = sum,
        //                 Price = price,
        //                 PriceRon = priceRon,
        //                 OfferId = budgetDto.OfferId,
        //                 // BudgetId = budgetDto.BudgetId,
        //                 // UomId = contract.ContractAmount.UomId,
        //                 ContractId = budgetDto.ContractId,
        //                 // RateId = contract.ContractAmount.RateId,
        //                 RateId = offer.RateId,
        //                 // BudgetBaseId = budgetDto.BudgetBaseId,
        //                 BudgetForecastId = budgetDto.BudgetForecastId,
        //                 UomId = offer.UomId,
        //                 PreAmount = budgetDto.PreAmount,
        //                 OrderTypeId = budgetDto.OrderTypeId,
        //                 BudgetValueNeed = budgetValueNeed,
        //                 BudgetValueNeedOtherCurrency = budgetValueNeedInOtherCurrency,
        //                 StartAccMonthId = budgetDto.StartAccMonthId

        //             };
        //             _context.Add(order);

        //             orderOp = new Model.OrderOp()
        //             {
        //                 AccMonthId = inventory.AccMonthId,
        //                 AccSystemId = null,
        //                 //AccountIdInitial = budgetBaseApproved.AccountId,
        //                 //AccountIdFinal = budgetBaseApproved.AccountId,
        //                 //AdministrationIdInitial = budgetBaseApproved.AdministrationId,
        //                 //AdministrationIdFinal = budgetBaseApproved.AdministrationId,
        //                 Order = order,
        //                 BudgetManagerIdInitial = inventory.BudgetManagerId,
        //                 BudgetManagerIdFinal = inventory.BudgetManagerId,
        //                 BudgetStateId = appState.Id,
        //                 CompanyIdInitial = budgetForecastApproved.BudgetBase.CompanyId,
        //                 CompanyIdFinal = budgetForecastApproved.BudgetBase.CompanyId,
        //                 CostCenterIdInitial = budgetForecastApproved.BudgetBase.CostCenterId,
        //                 CostCenterIdFinal = budgetForecastApproved.BudgetBase.CostCenterId,
        //                 CreatedAt = DateTime.Now,
        //                 CreatedBy = _context.UserId,
        //                 Document = document,
        //                 DstConfAt = DateTime.Now,
        //                 DstConfBy = budgetDto.UserId,
        //                 EmployeeIdInitial = budgetForecastApproved.BudgetBase.EmployeeId,
        //                 EmployeeIdFinal = budgetForecastApproved.BudgetBase.EmployeeId,
        //                 InfoIni = budgetDto.Info,
        //                 InfoFin = budgetDto.Info,
        //                 //InterCompanyIdInitial = budgetBaseApproved.InterCompanyId,
        //                 //InterCompanyIdFinal = budgetBaseApproved.InterCompanyId,
        //                 IsAccepted = false,
        //                 IsDeleted = false,
        //                 ModifiedAt = DateTime.Now,
        //                 ModifiedBy = _context.UserId,
        //                 PartnerIdInitial = offer.PartnerId,
        //                 PartnerIdFinal = offer.PartnerId,
        //                 ProjectIdInitial = budgetForecastApproved.BudgetBase.ProjectId,
        //                 ProjectIdFinal = budgetForecastApproved.BudgetBase.ProjectId,
        //                 QuantityIni = sum,
        //                 QuantityFin = sum,
        //                 SubTypeIdInitial = offer.SubTypeId,
        //                 SubTypeIdFinal = offer.SubTypeId,
        //                 Validated = true,
        //                 ValueFin1 = value,
        //                 ValueIni1 = value,
        //                 ValueFin2 = value,
        //                 ValueIni2 = value,
        //                 Guid = Guid.NewGuid(),
        //                 //BudgetIdInitial = budgetDto.BudgetId,
        //                 //BudgetIdFinal = budgetDto.BudgetId,
        //                 OfferIdInitial = budgetDto.OfferId,
        //                 OfferIdFinal = budgetDto.OfferId,
        //                 UomId = contract.ContractAmount.UomId
        //             };

        //             _context.Add(orderOp);


        //             offerOp = new Model.OfferOp()
        //             {
        //                 AccMonthId = inventory.AccMonthId,
        //                 AccSystemId = null,
        //                 AccountIdInitial = offer.AccountId,
        //                 AccountIdFinal = offer.AccountId,
        //                 AdministrationIdInitial = offer.AdministrationId,
        //                 AdministrationIdFinal = offer.AdministrationId,
        //                 Offer = offer,
        //                 BudgetManagerIdInitial = inventory.BudgetManagerId,
        //                 BudgetManagerIdFinal = inventory.BudgetManagerId,
        //                 BudgetStateId = offer.AppStateId,
        //                 CompanyIdInitial = offer.CompanyId,
        //                 CompanyIdFinal = offer.CompanyId,
        //                 CostCenterIdInitial = offer.CostCenterId,
        //                 CostCenterIdFinal = offer.CostCenterId,
        //                 CreatedAt = DateTime.Now,
        //                 CreatedBy = _context.UserId,
        //                 Document = document,
        //                 DstConfAt = DateTime.Now,
        //                 DstConfBy = offer.UserId,
        //                 EmployeeIdInitial = offer.EmployeeId,
        //                 EmployeeIdFinal = offer.EmployeeId,
        //                 InfoIni = offer.Info,
        //                 InfoFin = offer.Info,
        //                 InterCompanyIdInitial = offer.InterCompanyId,
        //                 InterCompanyIdFinal = offer.InterCompanyId,
        //                 IsAccepted = false,
        //                 IsDeleted = false,
        //                 ModifiedAt = DateTime.Now,
        //                 ModifiedBy = _context.UserId,
        //                 PartnerIdInitial = offer.PartnerId,
        //                 PartnerIdFinal = offer.PartnerId,
        //                 ProjectIdInitial = offer.ProjectId,
        //                 ProjectIdFinal = offer.ProjectId,
        //                 QuantityIni = offer.Quantity,
        //                 QuantityFin = offer.QuantityRem,
        //                 SubTypeIdInitial = offer.SubTypeId,
        //                 SubTypeIdFinal = offer.SubTypeId,
        //                 Validated = true,
        //                 ValueFin1 = offer.ValueFin,
        //                 ValueIni1 = offer.ValueIni,
        //                 ValueFin2 = offer.ValueFin,
        //                 ValueIni2 = offer.ValueIni,
        //                 Guid = Guid.NewGuid()
        //             };

        //             _context.Add(offerOp);

        //             entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
        //             _context.Update(entityType);

        //             if(orderType.Code == "C-IT")
        //	{
        //                 order.IsAccepted = true;
        //	}

        //             _context.SaveChanges();

        //             for (int i = 0; i < budgetDto.OrderMaterialUpdates.Count; i++)
        //             {
        //                 if (orderType.Code == "C-IT")
        //                 {
        //                     for (int q = 0; q < budgetDto.OrderMaterialUpdates[i].Quantity; q++)
        //                     {
        //                         Dto.AddStockAsset newAssetFromStock = new AddStockAsset();
        //                         newAssetFromStock.Id = 0;
        //                         newAssetFromStock.Name = "";
        //                         newAssetFromStock.SerialNumber = "";
        //                         newAssetFromStock.Name2 = "";
        //                         newAssetFromStock.StockId = budgetDto.OrderMaterialUpdates[i].Id;
        //                         newAssetFromStock.SubCategoryId = null;

        //                         await AddAssetStock(newAssetFromStock);
        //                     }
        //                 }

        //                 offerMaterial = await _context.Set<Model.OfferMaterial>().Include(r => r.Rate).AsNoTracking().Where(a => a.Id == budgetDto.OrderMaterialUpdates[i].Id).SingleOrDefaultAsync();

        //                 if (offerMaterial == null)
        //                 {
        //                     offerMaterial = new Model.OfferMaterial()
        //                     {
        //                         CreatedAt = DateTime.Now,
        //                         CreatedBy = _context.UserId,
        //                         ModifiedAt = DateTime.Now,
        //                         ModifiedBy = _context.UserId,
        //                         OfferId = budgetDto.OfferId.Value,
        //                         MaterialId = 627,
        //                         EmailManagerId = 2076,
        //                         AppStateId = 7,
        //                         Value = value,
        //                         ValueRon = valueRon,
        //                         Price = price,
        //                         PriceRon = priceRon,
        //                         Quantity = (decimal)sum,
        //                         RequestId = offer.RequestId,
        //                         RateId = 2669
        //                     };

        //                     _context.Add(offerMaterial);

        //                 }

        //                 //if (offerMaterial != null)
        //                 //{
        //                 //    offerMaterial.OrdersQuantity += budgetDto.OrderMaterialUpdates[i].Quantity;
        //                 //    offerMaterial.Quantity -= budgetDto.OrderMaterialUpdates[i].Quantity;
        //                 //    offerMaterial.OrdersPrice += budgetDto.OrderMaterialUpdates[i].Price;
        //                 //    offerMaterial.OrdersValue += budgetDto.OrderMaterialUpdates[i].Price * budgetDto.OrderMaterialUpdates[i].Quantity;
        //                 //    offerMaterial.Value -= budgetDto.OrderMaterialUpdates[i].Price * budgetDto.OrderMaterialUpdates[i].Quantity;
        //                 //    _context.Update(offerMaterial);

        //                 //}

        //                 orderMaterial = new Model.OrderMaterial()
        //                 {
        //                     AppStateId = appStateOrderMaterial.Id,
        //                     CreatedAt = DateTime.Now,
        //                     CreatedBy = _context.UserId,
        //                     IsDeleted = false,
        //                     MaterialId = offerMaterial.MaterialId,
        //                     ModifiedAt = DateTime.Now,
        //                     ModifiedBy = _context.UserId,
        //                     Order = order,
        //                     Price = budgetDto.OrderMaterialUpdates[i].Price,
        //                     PriceRon = budgetDto.OrderMaterialUpdates[i].PriceRon,
        //                     PriceIni = budgetDto.OrderMaterialUpdates[i].Price,
        //                     PriceIniRon = budgetDto.OrderMaterialUpdates[i].PriceRon,
        //                     QuantityIni = (decimal)budgetDto.OrderMaterialUpdates[i].Quantity,
        //                     ValueIni = budgetDto.OrderMaterialUpdates[i].Value,
        //                     ValueIniRon = budgetDto.OrderMaterialUpdates[i].ValueRon,
        //                     OrdersQuantity = budgetDto.OrderMaterialUpdates[i].Quantity,
        //                     Quantity = (decimal)budgetDto.OrderMaterialUpdates[i].Quantity,
        //                     OrdersPrice = budgetDto.OrderMaterialUpdates[i].Price,
        //                     OrdersPriceRon = budgetDto.OrderMaterialUpdates[i].PriceRon,
        //                     OrdersValue = budgetDto.OrderMaterialUpdates[i].Value,
        //                     OrdersValueRon = budgetDto.OrderMaterialUpdates[i].ValueRon,
        //                     Value = budgetDto.OrderMaterialUpdates[i].Value,
        //                     ValueRon = budgetDto.OrderMaterialUpdates[i].ValueRon,
        //                     // RateId = contract.ContractAmount.RateId,
        //                     RateId = offerMaterial.RateId,
        //                     RequestId = offerMaterial.RequestId,
        //                     OfferMaterialId = offerMaterial.Id,
        //                     PreAmount = budgetDto.OrderMaterialUpdates[i].PreAmount,
        //                     PreAmountRon = budgetDto.OrderMaterialUpdates[i].PreAmount * offerMaterial.Rate.Value,
        //                     Guid = offerMaterial.Guid

        //                 };

        //                 _context.Add(orderMaterial);
        //                 _context.SaveChanges();
        //             }

        //             var countOffer = await _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToListAsync();
        //             ////var countOrd = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
        //             var countContract = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToListAsync();
        //             var countContractAmount = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToListAsync();
        //             var countBudget = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
        //             // var countOfferMaterials = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials").ToList();
        //             var countOfferMaterials2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials2").ToList();


        //             return new Model.OrderResult { Success = true, Message = "Oferta a fost salvata cu succes!", OrderId = order.Id };
        //         }
        //else
        //{
        //             return new Model.OrderResult { Success = false, Message = $"Comanda nu a fost plasata. Valoare comenzii depaseste valoarea disponibila!", OrderId = order.Id };
        //         }


        //     }

        public async Task<Model.OrderResult> CreateOrUpdateOrder(OrderSave orderDto)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.OfferOp offerOp = null;
            Model.Document document = null;
            Model.DocumentType documentType = null;
            Model.EntityType entityType = null;
            Model.EntityType entityTypeRequestBF = null;
            Model.Offer offer = null;
            //Model.Budget budgetApproved = null;
            // Model.BudgetBase budgetBaseApproved = null;
            //Model.BudgetForecast budgetForecastApproved = null;
            //Model.Contract contract = null;
            Model.AppState appState = null;
            Model.AppState appStateOrderMaterial = null;
            Model.AppState appStateRequestBudgetForecast = null;
            Model.Inventory inventory = null;
            Model.OrderType orderType = null;
            Model.OrderMaterial orderMaterial = null;
            Model.OfferMaterial offerMaterial = null;
            Model.RequestBudgetForecast requestBudgetForecast = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.EmailType emailType = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;

            bool isValid = false;

            float sum = 0;
            decimal price = 0;
            decimal priceRon = 0;
            decimal value = 0;
            decimal valueRon = 0;

            appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "NEW").FirstOrDefaultAsync();

            for (int m = 0; m < orderDto.RequestBudgetForecasts.Count; m++)
			{
                requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Include(f => f.OfferType)
                    .Where(a => a.RequestBudgetForecastId == orderDto.RequestBudgetForecasts[m] && a.AppStateId == appState.Id)
                    .ToListAsync();

                if(requestBudgetForecastMaterials.Count == 0)
                {
                    return new OrderResult { Success = false, Message = "Va rugam adaugati produse in P.O.", OrderId = orderDto.Id };
                }

                for (int i = 0; i < requestBudgetForecastMaterials.Count; i++)
                {
                    requestBudgetForecastMaterials[i].Multiple = orderDto.Multiple[m];
                    _context.Update(requestBudgetForecastMaterials[i]);
                    _context.SaveChanges();
                }

                    sum += requestBudgetForecastMaterials.Count > 0 ? (float)requestBudgetForecastMaterials.Sum(a => a.Quantity): 0;

                price += requestBudgetForecastMaterials.Count > 0 ? requestBudgetForecastMaterials.Sum(a => a.Price) : 0;
                priceRon += requestBudgetForecastMaterials.Count > 0 ? requestBudgetForecastMaterials.Sum(a => a.PriceRon) : 0;
                value += requestBudgetForecastMaterials.Count > 0 ? requestBudgetForecastMaterials.Sum(a => a.Value) : 0;
                valueRon += requestBudgetForecastMaterials.Count > 0 ? requestBudgetForecastMaterials.Sum(a => a.ValueRon) : 0;
            }
            //decimal valueInOtherCurrency = budgetDto.SumOfTotalInOtherCurrency;
            //decimal budgetValueNeed = 0;
            //decimal budgetValueNeedInOtherCurrency = 0;

            orderType = await _context.Set<Model.OrderType>().Where(c => c.Id == orderDto.OrderTypeId).FirstOrDefaultAsync();
            if (orderType == null) return new Model.OrderResult { Success = false, Message = "Lipsa tip comanda!", OrderId = 0 };
            //contract = await _context.Set<Model.Contract>().Include(c => c.ContractAmount).Where(c => c.Id == budgetDto.ContractId).SingleAsync();
            offer = await _context.Set<Model.Offer>()
                .Include(f => f.OfferType)
                .Include(e => e.Request)
                .Include(r => r.Rate).ThenInclude(u => u.Uom)
                .Where(c => c.Id == orderDto.OfferId)
                .FirstOrDefaultAsync();

            if (offer == null) return new Model.OrderResult { Success = false, Message = "Lipsa oferta!", OrderId = 0 };
            if (offer != null && offer.OfferType == null) return new Model.OrderResult { Success = false, Message = "Lipsa tip oferta!", OrderId = 0 };
            //budgetApproved = _context.Set<Model.Budget>().Where(c => c.Id == budgetDto.BudgetId).SingleOrDefault();
            //budgetForecastApproved = await _context.Set<Model.BudgetForecast>().Include(b => b.BudgetBase).Where(c => c.Id == budgetDto.BudgetForecastId).SingleAsync();

            /*
            //if (budgetDto.NeedBudgetAmount)
            //{
            //    budgetValueNeed = value - budgetForecastApproved.TotalRem;
            //    budgetValueNeedInOtherCurrency = (value - budgetForecastApproved.TotalRem) / offer.Rate.Value;
            //}

            //         if (contract.ContractAmount.AmountRonRem >= value && budgetApproved.ValueFin >= value && offer.QuantityRem >= sum)
            //{
            //             isValid = true;

            //             // offer.QuantityRem = offer.QuantityRem > 0 ? offer.QuantityRem - sum : offer.Quantity;
            //             // offer.ValueFin = offer.ValueFin > 0 ? offer.ValueFin - budgetDto.ValueIni : offer.ValueFin;

            //             // budgetApproved.QuantityRem = budgetApproved.QuantityRem > 0 ? budgetApproved.QuantityRem - sum : budgetApproved.QuantityRem;
            //             // budgetApproved.ValueFin = budgetApproved.ValueFin > 0 ? budgetApproved.ValueFin - value : budgetApproved.ValueFin;


            //             //contract.ContractAmount.AmountRonUsed += value;
            //             //contract.ContractAmount.AmountUsed += valueInOtherCurrency;
            //         }

            //if (orderType != null && orderType.Code != "C-IT" && (contract.ContractAmount.AmountRonRem >= valueRon && budgetForecastApproved.TotalRem >= valueRon && offer.QuantityRem >= sum))
            //{
            //    isValid = true;

            //    // offer.QuantityRem = offer.QuantityRem > 0 ? offer.QuantityRem - sum : offer.Quantity;
            //    // offer.ValueFin = offer.ValueFin > 0 ? offer.ValueFin - budgetDto.ValueIni : offer.ValueFin;

            //    // budgetApproved.QuantityRem = budgetApproved.QuantityRem > 0 ? budgetApproved.QuantityRem - sum : budgetApproved.QuantityRem;
            //    // budgetApproved.ValueFin = budgetApproved.ValueFin > 0 ? budgetApproved.ValueFin - value : budgetApproved.ValueFin;


            //    //contract.ContractAmount.AmountRonUsed += value;
            //    //contract.ContractAmount.AmountUsed += valueInOtherCurrency;
            //}
            //else if (contract != null && contract.ContractId == "NO-C" && (budgetForecastApproved.TotalRem >= value && offer.QuantityRem >= sum))
            //{
            //    isValid = true;
            //}
            // */

            isValid = true;

            if(offer.OfferType.Code == "O-V")
			{
                sum = 1;
			}

            if (isValid)
            {
                entityType = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWORDER").FirstOrDefaultAsync();
                if (entityType == null) return new Model.OrderResult { Success = false, Message = "Lipsa entitate!", OrderId = 0 };

                if (orderType.Code == "C-IT")
                {
                    appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ACCEPTED").FirstOrDefaultAsync();
                    if (appState == null) return new Model.OrderResult { Success = false, Message = "Lipsa stare!", OrderId = 0 };
                    appStateOrderMaterial = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "PENDING").FirstOrDefaultAsync();
                    if (appStateOrderMaterial == null) return new Model.OrderResult { Success = false, Message = "Lipsa stare 2!", OrderId = 0 };
                }
				else if (orderType.Code == "C-LC")
				{
					appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "NEED_CONTRACT").SingleAsync();
                    appStateOrderMaterial = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "NEED_CONTRACT").SingleAsync();
                }
				else
                {
                    appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVELB1").SingleAsync();
                    if (appState == null) return new Model.OrderResult { Success = false, Message = "Lipsa stare!", OrderId = 0 };
                    appStateOrderMaterial = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "PENDING").SingleAsync();
                    if (appStateOrderMaterial == null) return new Model.OrderResult { Success = false, Message = "Lipsa stare 2!", OrderId = 0 };
                }

                documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_ORDER").FirstOrDefaultAsync();
                if (documentType == null) return new Model.OrderResult { Success = false, Message = "Lipsa tip document!", OrderId = 0 };
                inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).SingleAsync();
                if (inventory == null) return new Model.OrderResult { Success = false, Message = "Lipsa inventar!", OrderId = 0 };

                //if (orderDto.NeedBudgetAmount)
                //{
                //    appState = await _context.Set<Model.AppState>().Where(c => c.Code == "ORDER_BOOK").SingleAsync();
                //    appStateOrderMaterial = await _context.Set<Model.AppState>().Where(c => c.Code == "ORDER_BOOK").SingleAsync();
                //}

                var lastCode = int.Parse(entityType.Name);
                var newBudgetCode = "ORD";

                int lastCodeStringLength = 7 - lastCode.ToString().Length;
                for (int i = 0; i < lastCodeStringLength; i++)
                    newBudgetCode += "0";

                newBudgetCode += entityType.Name;

                document = new Model.Document()
                {
                    Approved = true,
                    CompanyId = offer.CompanyId,
                    CostCenterId = offer.CostCenterId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserId,
                    CreationDate = DateTime.Now,
                    Details = orderDto.Info != null ? orderDto.Info : string.Empty,
                    DocNo1 = orderDto.Info != null ? orderDto.Info : string.Empty,
                    DocNo2 = orderDto.Info != null ? orderDto.Info : string.Empty,
                    DocumentDate = DateTime.Now,
                    DocumentTypeId = documentType.Id,
                    Exported = true,
                    IsDeleted = false,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = orderDto.UserId,
                    ParentDocumentId = null,
                    PartnerId = offer.PartnerId,
                    RegisterDate = DateTime.Now,
                    ValidationDate = DateTime.Now
                };

                _context.Add(document);

                order = new Model.Order()
                {
                    AccMonthId = inventory.AccMonthId,
                    AccountId = offer.AccountId,
                    AdministrationId = offer.AdministrationId,
                    AppStateId = appState.Id,
                    BudgetManagerId = inventory.BudgetManagerId,
                    Code = newBudgetCode,
                    CompanyId = offer.CompanyId,
                    CostCenterId = offer.CostCenterId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserId,
                    EmployeeId = orderDto.EmployeeId,
                    EndDate = null,
                    StartDate = DateTime.Now,
                    Info = offer.Info,
                    InterCompanyId = offer.InterCompanyId,
                    IsAccepted = false,
                    IsDeleted = false,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = _context.UserId,
                    Name = orderDto.Name,
                    PartnerId = offer.PartnerId,
                    //ProjectId = budgetForecastApproved.BudgetBase.ProjectId,
                    Quantity = sum,
                    SubTypeId = offer.SubTypeId,
                    UserId = orderDto.UserId,
                    Validated = true,
                    ValueFin = value,
                    ValueFinRon = valueRon,
                    ValueIni = value,
                    ValueIniRon = valueRon,
                    Guid = Guid.NewGuid(),
                    QuantityRem = sum,
                    Price = price,
                    PriceRon = priceRon,
                    OfferId = orderDto.OfferId,
                    // BudgetId = budgetDto.BudgetId,
                    // UomId = contract.ContractAmount.UomId,
                    //ContractId = budgetDto.ContractId,
                    // RateId = contract.ContractAmount.RateId,
                    RateId = offer.RateId,
                    // BudgetBaseId = budgetDto.BudgetBaseId,
                    //BudgetForecastId = budgetDto.BudgetForecastId,
                    UomId = offer.UomId,
                    PreAmount = orderDto.PreAmount,
                    OrderTypeId = orderDto.OrderTypeId,
                    //BudgetValueNeed = budgetValueNeed,
                    //BudgetValueNeedOtherCurrency = budgetValueNeedInOtherCurrency,
                    StartAccMonthId = orderDto.StartAccMonthId,
                    DivisionId = offer.DivisionId

                };
                _context.Add(order);

                /*
                orderop = new model.orderop()
                {
                    accmonthid = inventory.accmonthid,
                    accsystemid = null,
                    //accountidinitial = budgetbaseapproved.accountid,
                    //accountidfinal = budgetbaseapproved.accountid,
                    //administrationidinitial = budgetbaseapproved.administrationid,
                    //administrationidfinal = budgetbaseapproved.administrationid,
                    order = order,
                    budgetmanageridinitial = inventory.budgetmanagerid,
                    budgetmanageridfinal = inventory.budgetmanagerid,
                    budgetstateid = appstate.id,
                    companyidinitial = budgetforecastapproved.budgetbase.companyid,
                    companyidfinal = budgetforecastapproved.budgetbase.companyid,
                    costcenteridinitial = budgetforecastapproved.budgetbase.costcenterid,
                    costcenteridfinal = budgetforecastapproved.budgetbase.costcenterid,
                    createdat = datetime.now,
                    createdby = _context.userid,
                    document = document,
                    dstconfat = datetime.now,
                    dstconfby = budgetdto.userid,
                    employeeidinitial = budgetforecastapproved.budgetbase.employeeid,
                    employeeidfinal = budgetforecastapproved.budgetbase.employeeid,
                    infoini = budgetdto.info,
                    infofin = budgetdto.info,
                    //intercompanyidinitial = budgetbaseapproved.intercompanyid,
                    //intercompanyidfinal = budgetbaseapproved.intercompanyid,
                    isaccepted = false,
                    isdeleted = false,
                    modifiedat = datetime.now,
                    modifiedby = _context.userid,
                    partneridinitial = offer.partnerid,
                    partneridfinal = offer.partnerid,
                    projectidinitial = budgetforecastapproved.budgetbase.projectid,
                    projectidfinal = budgetforecastapproved.budgetbase.projectid,
                    quantityini = sum,
                    quantityfin = sum,
                    subtypeidinitial = offer.subtypeid,
                    subtypeidfinal = offer.subtypeid,
                    validated = true,
                    valuefin1 = value,
                    valueini1 = value,
                    valuefin2 = value,
                    valueini2 = value,
                    guid = guid.newguid(),
                    //budgetidinitial = budgetdto.budgetid,
                    //budgetidfinal = budgetdto.budgetid,
                    offeridinitial = budgetdto.offerid,
                    offeridfinal = budgetdto.offerid,
                    uomid = contract.contractamount.uomid
                };

                _context.add(orderop);


                offerop = new model.offerop()
                {
                    accmonthid = inventory.accmonthid,
                    accsystemid = null,
                    accountidinitial = offer.accountid,
                    accountidfinal = offer.accountid,
                    administrationidinitial = offer.administrationid,
                    administrationidfinal = offer.administrationid,
                    offer = offer,
                    budgetmanageridinitial = inventory.budgetmanagerid,
                    budgetmanageridfinal = inventory.budgetmanagerid,
                    budgetstateid = offer.appstateid,
                    companyidinitial = offer.companyid,
                    companyidfinal = offer.companyid,
                    costcenteridinitial = offer.costcenterid,
                    costcenteridfinal = offer.costcenterid,
                    createdat = datetime.now,
                    createdby = _context.userid,
                    document = document,
                    dstconfat = datetime.now,
                    dstconfby = offer.userid,
                    employeeidinitial = offer.employeeid,
                    employeeidfinal = offer.employeeid,
                    infoini = offer.info,
                    infofin = offer.info,
                    intercompanyidinitial = offer.intercompanyid,
                    intercompanyidfinal = offer.intercompanyid,
                    isaccepted = false,
                    isdeleted = false,
                    modifiedat = datetime.now,
                    modifiedby = _context.userid,
                    partneridinitial = offer.partnerid,
                    partneridfinal = offer.partnerid,
                    projectidinitial = offer.projectid,
                    projectidfinal = offer.projectid,
                    quantityini = offer.quantity,
                    quantityfin = offer.quantityrem,
                    subtypeidinitial = offer.subtypeid,
                    subtypeidfinal = offer.subtypeid,
                    validated = true,
                    valuefin1 = offer.valuefin,
                    valueini1 = offer.valueini,
                    valuefin2 = offer.valuefin,
                    valueini2 = offer.valueini,
                    guid = guid.newguid()
                };

                _context.add(offerop);
                //
                */

                entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
                _context.Update(entityType);

                if (orderType.Code == "C-IT")
                {
                    order.IsAccepted = true;
                }

                _context.SaveChanges();

                for (int i = 0; i < orderDto.OrderMaterialUpdates.Count; i++)
                {
                    if (orderType.Code == "C-IT")
                    {
                        for (int q = 0; q < orderDto.OrderMaterialUpdates[i].Quantity; q++)
                        {
                            Dto.AddStockAsset newAssetFromStock = new AddStockAsset();
                            newAssetFromStock.Id = 0;
                            newAssetFromStock.Name = "";
                            newAssetFromStock.SerialNumber = "";
                            newAssetFromStock.Name2 = "";
                            newAssetFromStock.StockId = orderDto.OrderMaterialUpdates[i].Id;
                            newAssetFromStock.SubCategoryId = null;

                            await AddAssetStock(newAssetFromStock);
                        }
                    }

                    offerMaterial = await _context.Set<Model.OfferMaterial>()
                        .Include(r => r.Rate).AsNoTracking()
                        .Where(a => a.Id == orderDto.OrderMaterialUpdates[i].Id)
                        .FirstOrDefaultAsync();

                    if (offerMaterial == null)
                    {
                        offerMaterial = new Model.OfferMaterial()
                        {
                            CreatedAt = DateTime.Now,
                            CreatedBy = _context.UserId,
                            ModifiedAt = DateTime.Now,
                            ModifiedBy = _context.UserId,
                            OfferId = orderDto.OfferId.Value,
                            MaterialId = 627,
                            EmailManagerId = 2076,
                            AppStateId = 7,
                            Value = value,
                            ValueRon = valueRon,
                            Price = price,
                            PriceRon = priceRon,
                            Quantity = (decimal)sum,
                            RequestId = offer.RequestId,
                            RateId = 2669
                        };

                        _context.Add(offerMaterial);

                    }

                    /*
                    if (offerMaterial != null)
                    {
                        offerMaterial.OrdersQuantity += budgetDto.OrderMaterialUpdates[i].Quantity;
                        offerMaterial.Quantity -= budgetDto.OrderMaterialUpdates[i].Quantity;
                        offerMaterial.OrdersPrice += budgetDto.OrderMaterialUpdates[i].Price;
                        offerMaterial.OrdersValue += budgetDto.OrderMaterialUpdates[i].Price * budgetDto.OrderMaterialUpdates[i].Quantity;
                        offerMaterial.Value -= budgetDto.OrderMaterialUpdates[i].Price * budgetDto.OrderMaterialUpdates[i].Quantity;
                        _context.Update(offerMaterial);

                    }
                    */

                    if (orderType.Code == "C-IT")
					{
                        orderMaterial = new Model.OrderMaterial()
                        {
                            AppStateId = appStateOrderMaterial.Id,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _context.UserId,
                            IsDeleted = false,
                            MaterialId = offerMaterial.MaterialId,
                            ModifiedAt = DateTime.Now,
                            ModifiedBy = _context.UserId,
                            Order = order,
                            Price = orderDto.OrderMaterialUpdates[i].Price,
                            PriceRon = orderDto.OrderMaterialUpdates[i].PriceRon,
                            PriceIni = orderDto.OrderMaterialUpdates[i].Price,
                            PriceIniRon = orderDto.OrderMaterialUpdates[i].PriceRon,
                            QuantityIni = (decimal)orderDto.OrderMaterialUpdates[i].Quantity,
                            ValueIni = orderDto.OrderMaterialUpdates[i].Value,
                            ValueIniRon = orderDto.OrderMaterialUpdates[i].ValueRon,
                            OrdersQuantity = orderDto.OrderMaterialUpdates[i].Quantity,
                            Quantity = (decimal)orderDto.OrderMaterialUpdates[i].Quantity,
                            OrdersPrice = orderDto.OrderMaterialUpdates[i].Price,
                            OrdersPriceRon = orderDto.OrderMaterialUpdates[i].PriceRon,
                            OrdersValue = orderDto.OrderMaterialUpdates[i].Value,
                            OrdersValueRon = orderDto.OrderMaterialUpdates[i].ValueRon,
                            Value = orderDto.OrderMaterialUpdates[i].Value,
                            ValueRon = orderDto.OrderMaterialUpdates[i].ValueRon,
                            // RateId = contract.ContractAmount.RateId,
                            RateId = offerMaterial.RateId,
                            RequestId = offerMaterial.RequestId,
                            OfferMaterialId = offerMaterial.Id,
                            PreAmount = orderDto.OrderMaterialUpdates[i].PreAmount,
                            PreAmountRon = orderDto.OrderMaterialUpdates[i].PreAmount * offerMaterial.Rate.Value,
                            Guid = offerMaterial.Guid
                        };

                        _context.Add(orderMaterial);
                    }
                    
                    _context.SaveChanges();
                }

				for (int i = 0; i < orderDto.RequestBudgetForecasts.Count; i++)
				{
                    requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
                        //.Include(b => b.RequestBudgetForecast).ThenInclude(b => b.BudgetForecast)
                        .Where(r => r.RequestBudgetForecastId == orderDto.RequestBudgetForecasts[i] && r.IsDeleted == false && r.AppStateId == 1)
                        .ToListAsync();

                    for (int r = 0; r < requestBudgetForecastMaterials.Count; r++)
                    {
                        requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                            .Include(b => b.RequestBudgetForecast).ThenInclude(b => b.BudgetForecast)
                            .Where(a => a.Id == requestBudgetForecastMaterials[r].Id).SingleAsync();

                        requestBudgetForecastMaterial.OrderId = order.Id;

                        //orderMaterial = await _context.Set<Model.OrderMaterial>()
                        //    .Where(r => r.Id == requestBudgetForecastMaterial.OrderMaterialId)
                        //    .SingleAsync();

                        //orderMaterial.BudgetForecastId = requestBudgetForecastMaterial.RequestBudgetForecast.BudgetForecastId;
                        order.ContractId = requestBudgetForecastMaterial.RequestBudgetForecast.ContractId;

                        
                        requestBudgetForecastMaterial.AppStateId = appState.Id;
                        //requestBudgetForecastMaterial.BudgetForecastTimeStamp = requestBudgetForecastMaterial.RequestBudgetForecast.BudgetForecast.TotalRem;
                        //requestBudgetForecastMaterial.ContractTimeStamp = requestBudgetForecastMaterial.RequestBudgetForecast.Contract.ContractAmount.AmountRonRem;

                        orderMaterial = new Model.OrderMaterial()
                        {
                            AppStateId = appStateOrderMaterial.Id,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _context.UserId,
                            IsDeleted = false,
                            MaterialId = requestBudgetForecastMaterial.MaterialId,
                            ModifiedAt = DateTime.Now,
                            ModifiedBy = _context.UserId,
                            OrderId = order.Id,
                            Price = requestBudgetForecastMaterial.Price,
                            PriceRon = requestBudgetForecastMaterial.PriceRon,
                            PriceIni = requestBudgetForecastMaterial.Price,
                            PriceIniRon = requestBudgetForecastMaterial.PriceRon,
                            QuantityIni = (decimal)requestBudgetForecastMaterial.Quantity,
                            ValueIni = requestBudgetForecastMaterial.Value,
                            ValueIniRon = requestBudgetForecastMaterial.ValueRon,
                            OrdersQuantity = requestBudgetForecastMaterial.Quantity,
                            Quantity = (decimal)requestBudgetForecastMaterial.Quantity,
                            OrdersPrice = requestBudgetForecastMaterial.Price,
                            OrdersPriceRon = requestBudgetForecastMaterial.PriceRon,
                            OrdersValue = requestBudgetForecastMaterial.Value,
                            OrdersValueRon = requestBudgetForecastMaterial.ValueRon,
                            Value = requestBudgetForecastMaterial.Value,
                            ValueRon = requestBudgetForecastMaterial.ValueRon,
                            // RateId = contract.ContractAmount.RateId,
                            RateId = offerMaterial != null ? offerMaterial.RateId : null,
                            RequestId = offerMaterial != null ? offerMaterial.RequestId : null,
                            OfferMaterialId = requestBudgetForecastMaterial.OfferMaterialId,
                            PreAmount = requestBudgetForecastMaterial.PreAmount,
                            PreAmountRon = offerMaterial != null ?  requestBudgetForecastMaterial.PreAmount * offerMaterial.Rate.Value : 0,
                            Guid = offerMaterial != null ? offerMaterial.Guid : Guid.Empty,
                            BudgetForecastId = requestBudgetForecastMaterial.RequestBudgetForecast.BudgetForecastId
                        };

                        requestBudgetForecastMaterial.OrderMaterial = orderMaterial;

                        _context.Add(orderMaterial);

                        _context.Update(order);
                        _context.Update(requestBudgetForecastMaterial);
                    }

                    requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>().Where(r => r.Id == orderDto.RequestBudgetForecasts[i]).SingleAsync();

					if (requestBudgetForecast.NeedBudget && requestBudgetForecast.NeedBudgetValue > 0)
					{
                        emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "NEED_BUDGET").SingleAsync();
                        entityTypeRequestBF = await _context.Set<Model.EntityType>().Where(c => c.Code == "NEED_BUDGET").SingleAsync();
                        appStateRequestBudgetForecast = await _context.Set<Model.AppState>().Where(c => c.Code == "ORDER_BOOK").SingleAsync();

                        emailOrderStatus = new Model.EmailOrderStatus()
                        {
                            AppStateId = appStateRequestBudgetForecast.Id,
                            Completed = false,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _context.UserId,
                            DocumentNumber = int.Parse(entityTypeRequestBF.Name),
                            EmailSend = false,
                            EmailTypeId = emailType.Id,
							EmployeeB1EmailSend = false,
							EmployeeB1ValidateAt = null,
							EmployeeB1ValidateBy = null,
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
                            MatrixId = orderDto.MatrixId.Value,
                            ModifiedAt = DateTime.Now,
                            ModifiedBy = _context.UserId,
                            NotCompletedSync = false,
							NotEmployeeB1Sync = false,
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
							EmployeeB1EmailSkip = false,
							EmployeeL1EmailSkip = true,
                            EmployeeL2EmailSkip = true,
                            EmployeeL3EmailSkip = true,
                            EmployeeL4EmailSkip = false,
                            EmployeeS1EmailSkip = true,
                            EmployeeS2EmailSkip = true,
                            EmployeeS3EmailSkip = true,
                            NeedBudgetEmailSend = false,
                            NotNeedBudgetSync = false,
                            SyncNeedBudgetErrorCount = 0,
                            NeedContractEmailSend = false,
                            NotNeedContractSync = false,
                            SyncNeedContractErrorCount = 0,
                            RequestBudgetForecastId = requestBudgetForecast.Id,
                            ContractId = order.ContractId
                        };

                        entityTypeRequestBF.Name = (int.Parse(entityTypeRequestBF.Name) + 1).ToString();
                        _context.Update(entityTypeRequestBF);

                        _context.Add(emailOrderStatus);

                        order.AppStateId = appStateRequestBudgetForecast.Id;
                        requestBudgetForecast.AppStateId = appStateRequestBudgetForecast.Id;
                    }

                    if (requestBudgetForecast.NeedContract && requestBudgetForecast.NeedContractValue > 0)
                    {
                        emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "NEED_CONTRACT").SingleAsync();
                        appStateRequestBudgetForecast = await _context.Set<Model.AppState>().Where(c => c.Code == "NEED_CONTRACT").SingleAsync();

                        emailOrderStatus = new Model.EmailOrderStatus()
                        {
                            AppStateId = appStateRequestBudgetForecast.Id,
                            Completed = false,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _context.UserId,
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
                            MatrixId = orderDto.MatrixId.Value,
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
                            EmployeeL1EmailSkip = true,
                            EmployeeL2EmailSkip = true,
                            EmployeeL3EmailSkip = true,
                            EmployeeL4EmailSkip = false,
                            EmployeeS1EmailSkip = true,
                            EmployeeS2EmailSkip = true,
                            EmployeeS3EmailSkip = true,
                            NeedBudgetEmailSend = false,
                            NotNeedBudgetSync = false,
                            SyncNeedBudgetErrorCount = 0,
                            NeedContractEmailSend = false,
                            NotNeedContractSync = false,
                            SyncNeedContractErrorCount = 0,
                            RequestBudgetForecastId = requestBudgetForecast.Id,
                            ContractId = order.ContractId
                        };

                        _context.Add(emailOrderStatus);

                        order.AppStateId = appStateRequestBudgetForecast.Id;
                        requestBudgetForecast.AppStateId = appStateRequestBudgetForecast.Id;
                    }

                    _context.SaveChanges();
                }

                var countOffer = await _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToListAsync();
                ////var countOrd = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
                var countContract = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToListAsync();
                var countContractAmount = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToListAsync();
                var countBudget = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
                // var countOfferMaterials = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials").ToList();
                var countOfferMaterials2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials2").ToList();
                var UpdateAllRequestBudgetForecastMaterials = _context.Set<Model.RecordCount>().FromSql("UpdateAllRequestBudgetForecastMaterials").ToList();

                return new Model.OrderResult { Success = true, Message = "Oferta a fost salvata cu succes!", OrderId = order.Id };
            }
            else
            {
                return new Model.OrderResult { Success = false, Message = $"Comanda nu a fost plasata. Valoare comenzii depaseste valoarea disponibila!", OrderId = order.Id };
            }


        }

        public async Task<Model.OrderResult> CreateOrUpdateOrderStock(OrderStockSave orderDto)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.OfferOp offerOp = null;
            Model.Document document = null;
            Model.DocumentType documentType = null;
            Model.EntityType entityType = null;
            Model.EntityType entityTypeRequestBF = null;
            Model.Offer offer = null;
            //Model.Budget budgetApproved = null;
            // Model.BudgetBase budgetBaseApproved = null;
            //Model.BudgetForecast budgetForecastApproved = null;
            //Model.Contract contract = null;
            Model.AppState appState = null;
            Model.AppState appStateOrderMaterial = null;
            Model.AppState appStateRequestBudgetForecast = null;
            Model.Inventory inventory = null;
            Model.OrderType orderType = null;
            Model.OrderMaterial orderMaterial = null;
            //Model.OfferMaterial offerMaterial = null;
            //Model.RequestBudgetForecast requestBudgetForecast = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            Model.EmailType emailType = null;
            //Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            //List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            Model.Request request = null;
            Model.EntityType entityTypeRequest = null;
            Model.EntityType entityTypeOffer = null;
            Model.Company company = null;
            Model.ProjectType projectType = null;
            Model.AssetType assetType = null;
            Model.Division division = null;
            Model.OfferType offerType = null;
            Model.Project project = null;
            Model.ErrorType errorType = null;
            Model.Stock stock = null;
            Model.SubCategory subCategory = null;
            Model.Category category = null;
            Model.ContractAmount contractAmount = null;
            Model.Uom uom = null;
            Model.Rate rate = null;
            Model.Contract contract = null;
            Model.EmailManager emailManager = null;
            List<Model.SubCategory> subCategories = null;
            bool isValid = false;
            bool newRequest = false;
            bool newOffer = false;
            bool newContract = false;

            //float sum = (float)orderDto.OrderMaterialUpdates.Sum(a => a.Quantity);
            //decimal price = orderDto.OrderMaterialUpdates.Sum(a => a.Price);
            //decimal priceRon = orderDto.OrderMaterialUpdates.Sum(a => a.PriceRon);
            //decimal value = orderDto.OrderMaterialUpdates.Sum(a => a.Value);
            //decimal valueRon = orderDto.OrderMaterialUpdates.Sum(a => a.ValueRon);

            float sum = 0;
            decimal price = 0;
            decimal priceRon = 0;
            decimal value = 0;
            decimal valueRon = 0;

            orderType = await _context.Set<Model.OrderType>().Where(c => c.Id == orderDto.OrderTypeId).FirstOrDefaultAsync();
            if (orderType == null) return new Model.OrderResult { Success = false, Message = "Lipsa tip comanda!" };

            documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_ORDER_CHECK").SingleOrDefault();
            if (documentType == null) return new Model.OrderResult { Success = false, Message = "Document-Type-Missing" };

            inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).FirstOrDefault();
            if (inventory == null) return new Model.OrderResult { Success = false, Message = "Inventory-Missing" };

            //order = await _context.Set<Model.Order>()
            //    .Include(o => o.Offer)
            //   .Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();

            if (order == null)
            {
                //request = await _context.Set<Model.Request>().Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();

                if (request == null)
                {
                    newRequest = true;

                    projectType = await _context.Set<Model.ProjectType>().Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();
                    if (projectType == null) return new Model.OrderResult { Success = false, Message = "Nu exista Tip Proiect!" };

                    assetType = await _context.Set<Model.AssetType>().Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();
                    if (assetType == null) return new Model.OrderResult { Success = false, Message = "Nu exista Cost Type!" };

                    division = await _context.Set<Model.Division>().Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();
                    if (division == null) return new Model.OrderResult { Success = false, Message = "Nu exista Divizie!" };

                    company = await _context.Set<Model.Company>().Where(c => c.Code == "RO10").FirstOrDefaultAsync();
                    if (company == null) return new Model.OrderResult { Success = false, Message = "Nu exista compania!" };

                    inventory = await _context.Set<Model.Inventory>().Where(c => c.Active == true).FirstOrDefaultAsync();
                    if (inventory == null) return new Model.OrderResult { Success = false, Message = "Nu exista inventar activ!" };

                    appState = await _context.Set<Model.AppState>().Where(c => c.Code == "ACCEPTED").FirstOrDefaultAsync();
                    if (appState == null) return new Model.OrderResult { Success = false, Message = "Nu exista stare!" };

                    offerType = await _context.Set<Model.OfferType>().Where(c => c.Code == "O-C").FirstOrDefaultAsync();
                    if (offerType == null) return new Model.OrderResult { Success = false, Message = "Lipsa tip oferta!" };

                    uom = await _context.Set<Model.Uom>().Where(com => com.Code == "RON").FirstOrDefaultAsync();
                    if (uom == null) return new Model.OrderResult { Success = false, Message = "Lipsa Moneda!" };
                    rate = await _context.Set<Model.Rate>().Include(r => r.Uom).Where(com => com.Uom.Code == "RON" && com.IsLast == true).LastOrDefaultAsync();
                    if (rate == null)
                    {
                        rate = await _context.Set<Model.Rate>().Include(r => r.Uom).Where(com => com.Uom.Code == "RON" && com.IsLast == false).LastOrDefaultAsync();
                    }
                    if (rate == null) return new Model.OrderResult { Success = false, Message = "Lipsa Curs BNR!" };

                    entityTypeRequest = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWREQUEST").FirstOrDefaultAsync();
                    if (entityTypeRequest == null) return new Model.OrderResult { Success = false, Message = "Nu exista entityType!" };

					var lastCodeRequest = int.Parse(entityTypeRequest.Name);
					var newRequestCode = string.Empty;

					if (lastCodeRequest.ToString().Length == 1)
					{
						newRequestCode = "PR000000" + entityTypeRequest.Name;
					}
					else if (lastCodeRequest.ToString().Length == 2)
					{
						newRequestCode = "PR00000" + entityTypeRequest.Name;
					}
					else if (lastCodeRequest.ToString().Length == 3)
					{
						newRequestCode = "PR0000" + entityTypeRequest.Name;
					}
					else if (lastCodeRequest.ToString().Length == 4)
					{
						newRequestCode = "PR000" + entityTypeRequest.Name;
					}
					else if (lastCodeRequest.ToString().Length == 5)
					{
						newRequestCode = "PR00" + entityTypeRequest.Name;
					}
					else if (lastCodeRequest.ToString().Length == 6)
					{
						newRequestCode = "PR0" + entityTypeRequest.Name;
					}
					else if (lastCodeRequest.ToString().Length == 7)
					{
						newRequestCode = "PR" + entityTypeRequest.Name;
					}

					request = new Model.Request()
                    {
                        AccMonthId = inventory.AccMonthId,
                        AppStateId = appState.Id,
                        BudgetManagerId = inventory.BudgetManagerId,
                        Code = newRequestCode,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _context.UserId,
                        EndDate = null,
                        StartDate = DateTime.Now,
                        Info = "STOCK IT",
                        IsAccepted = false,
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId,
                        Name = "STOCK_IT",
                        UserId = _context.UserId,
                        Validated = true,
                        ProjectTypeId = projectType.Id,
                        AssetTypeId = assetType.Id,
                        EmployeeId = orderDto.EmployeeId,
                        OwnerId = orderDto.EmployeeId,
                        BudgetValueNeed = 0,
                        CompanyId = company.Id,
                        StartAccMonthId = null,
                        Guid = Guid.NewGuid(),
                        DivisionId = division.Id

                    };

                    _context.Add(request);

                    entityTypeRequest.Name = (int.Parse(entityTypeRequest.Name) + 1).ToString();
                    _context.Update(entityTypeRequest);
                }

                //offer = await _context.Set<Model.Offer>()
                //    .Include(e => e.Request)
                //    .Include(e => e.OfferType)
                //    .Include(r => r.Rate).ThenInclude(u => u.Uom)
                //    .Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();

                if (offer == null)
                {
                    newOffer = true;
					entityTypeOffer = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWOFFER").FirstOrDefaultAsync();
					if (entityTypeOffer == null) return new Model.OrderResult { Success = false, Message = "Nu exista entityType!" };

					var lastOfferCode = int.Parse(entityTypeOffer.Name);
					var newOfferCode = string.Empty;

					if (lastOfferCode.ToString().Length == 1)
					{
						newOfferCode = "OFFER000000" + entityTypeOffer.Name;
					}
					else if (lastOfferCode.ToString().Length == 2)
					{
						newOfferCode = "OFFER00000" + entityTypeOffer.Name;
					}
					else if (lastOfferCode.ToString().Length == 3)
					{
						newOfferCode = "OFFER0000" + entityTypeOffer.Name;
					}
					else if (lastOfferCode.ToString().Length == 4)
					{
						newOfferCode = "OFFER000" + entityTypeOffer.Name;
					}
					else if (lastOfferCode.ToString().Length == 5)
					{
						newOfferCode = "OFFER00" + entityTypeOffer.Name;
					}
					else if (lastOfferCode.ToString().Length == 6)
					{
						newOfferCode = "OFFER0" + entityTypeOffer.Name;
					}
					else if (lastOfferCode.ToString().Length == 7)
					{
						newOfferCode = "OFFER" + entityTypeOffer.Name;
					}


					offer = new Model.Offer()
                    {
                        AccMonthId = inventory.AccMonthId,
                        AccountId = null,
                        AdministrationId = null,
                        AppStateId = appState.Id,
                        BudgetManagerId = inventory.BudgetManagerId,
                        Code = newOfferCode,
                        CompanyId = request.CompanyId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _context.UserId,
                        EmployeeId = orderDto.EmployeeId,
                        EndDate = request.StartDate,
                        StartDate = request.EndDate,
                        Info = request.Info,
                        InterCompanyId = null,
                        IsAccepted = false,
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId,
                        PartnerId = null,
                        Quantity = 0,
                        SubTypeId = null,
                        UserId = _context.UserId,
                        Validated = true,
                        ValueFin = 0,
                        ValueIni = 0,
                        Guid = Guid.NewGuid(),
                        QuantityRem = 0,
                        AdmCenterId = null,
                        RegionId = null,
                        Request = request,
                        AssetTypeId = request.AssetTypeId,
                        ProjectTypeId = request.ProjectTypeId,
                        OfferTypeId = offerType.Id,
                        DivisionId = request.DivisionId

                    };

                    _context.Add(offer);

                    entityTypeOffer.Name = (int.Parse(entityTypeOffer.Name) + 1).ToString();
                    _context.Update(entityTypeOffer);
                }

                contract = await _context.Set<Model.Contract>().Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();

                if (contract == null)
                {
                    newContract = true;

                    contractAmount = new Model.ContractAmount()
                    {
                        Amount = 0,
                        Uom = uom,
                        Rate = rate,
                        IsDeleted = false,
                        Code = string.Empty,
                        Name = string.Empty,
                        AmountRem = 0
                    };

                    _context.Add(contractAmount);

                    contract = new Model.Contract()
                    {
                        ContractId = "STOCK_IT",
                        Title = "STOCK_IT",
                        Name = "STOCK_IT",
                        AppState = appState,
                        EffectiveDate = null,
                        AgreementDate = null,
                        ExpirationDate = null,
                        CreationDate = null,
                        Version = "",
                        TemplateId = "",
                        AmendmentType = "",
                        AmendmentReason = "",
                        Origin = 0,
                        HierarchicalType = "",
                        ExpirationTermType = "",
                        RelatedId = "",
                        MaximumNumberOfRenewals = 0,
                        AutoRenewalInterval = 0,
                        IsTestProject = true,
                        Owner = null,
                        Partner = null,
                        BusinessSystem = null,
                        ContractAmount = contractAmount,
                        Code = "STOCK_IT",
                        PaymentTerms = 0,
                        Company = company
                    };

                    _context.Add(contract);
                };

                if(newRequest || newOffer || newContract)
				{
                    _context.SaveChanges();
                }
            }

            isValid = true;

            if (offer.OfferType.Code == "O-V")
            {
                sum = 1;
            }

            if (isValid)
            {
                entityType = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWORDER").FirstOrDefaultAsync();
                if (entityType == null) return new Model.OrderResult { Success = false, Message = "Lipsa entitate!", OrderId = 0 };

                if (orderType.Code == "C-IT")
                {
                    appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ACCEPTED").FirstOrDefaultAsync();
                    if (appState == null) return new Model.OrderResult { Success = false, Message = "Lipsa stare!", OrderId = 0 };
                    appStateOrderMaterial = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "PENDING").FirstOrDefaultAsync();
                    if (appStateOrderMaterial == null) return new Model.OrderResult { Success = false, Message = "Lipsa stare 2!", OrderId = 0 };
                }
                //else if (contract.ContractId == "NO-C")
                //{
                //    appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "NEED_CONTRACT").SingleAsync();
                //}
                else
                {
                    appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL4").SingleAsync();
                    if (appState == null) return new Model.OrderResult { Success = false, Message = "Lipsa stare!", OrderId = 0 };
                    appStateOrderMaterial = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "PENDING").SingleAsync();
                    if (appStateOrderMaterial == null) return new Model.OrderResult { Success = false, Message = "Lipsa stare 2!", OrderId = 0 };
                }

                documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_ORDER").FirstOrDefaultAsync();
                if (documentType == null) return new Model.OrderResult { Success = false, Message = "Lipsa tip document!", OrderId = 0 };
                inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).SingleAsync();
                if (inventory == null) return new Model.OrderResult { Success = false, Message = "Lipsa inventar!", OrderId = 0 };

                emailType = await _context.Set<Model.EmailType>().Where(e => e.Code == "VALIDATED_OFFER").FirstOrDefaultAsync();
                if (emailType == null) return new Model.OrderResult { Success = false, Message = "Nu exista tip email O-V!" };

                //if (orderDto.NeedBudgetAmount)
                //{
                //    appState = await _context.Set<Model.AppState>().Where(c => c.Code == "ORDER_BOOK").SingleAsync();
                //    appStateOrderMaterial = await _context.Set<Model.AppState>().Where(c => c.Code == "ORDER_BOOK").SingleAsync();
                //}

                var lastCode = int.Parse(entityType.Name);
                var newBudgetCode = string.Empty;

                if (lastCode.ToString().Length == 1)
                {
                    newBudgetCode = "ORD000000" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 2)
                {
                    newBudgetCode = "ORD00000" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 3)
                {
                    newBudgetCode = "ORD0000" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 4)
                {
                    newBudgetCode = "ORD000" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 5)
                {
                    newBudgetCode = "ORD00" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 6)
                {
                    newBudgetCode = "ORD0" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 7)
                {
                    newBudgetCode = "ORD" + entityType.Name;
                }

                document = new Model.Document()
                {
                    Approved = true,
                    CompanyId = offer.CompanyId,
                    CostCenterId = offer.CostCenterId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserId,
                    CreationDate = DateTime.Now,
                    Details = orderDto.Info != null ? orderDto.Info : string.Empty,
                    DocNo1 = orderDto.Info != null ? orderDto.Info : string.Empty,
                    DocNo2 = orderDto.Info != null ? orderDto.Info : string.Empty,
                    DocumentDate = DateTime.Now,
                    DocumentTypeId = documentType.Id,
                    Exported = true,
                    IsDeleted = false,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = orderDto.UserId,
                    ParentDocumentId = null,
                    PartnerId = offer.PartnerId,
                    RegisterDate = DateTime.Now,
                    ValidationDate = DateTime.Now
                };

                _context.Add(document);


                order = new Model.Order()
                {
                    AccMonthId = inventory.AccMonthId,
                    AccountId = offer.AccountId,
                    AdministrationId = offer.AdministrationId,
                    AppStateId = appState.Id,
                    BudgetManagerId = inventory.BudgetManagerId,
                    Code = newBudgetCode,
                    CompanyId = offer.CompanyId,
                    CostCenterId = offer.CostCenterId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserId,
                    EmployeeId = offer.EmployeeId,
                    // EndDate = orderDto.EndDate,
                    StartDate = DateTime.Now,
                    Info = offer.Info,
                    InterCompanyId = offer.InterCompanyId,
                    IsAccepted = false,
                    IsDeleted = false,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = _context.UserId,
                    Name = orderDto.Name == null || orderDto.Name == "" ? "STOCK_IT_MFX" : orderDto.Name,
                    PartnerId = offer.PartnerId,
                    //ProjectId = budgetForecastApproved.BudgetBase.ProjectId,
                    Quantity = sum,
                    SubTypeId = offer.SubTypeId,
                    UserId = orderDto.UserId,
                    Validated = true,
                    ValueFin = value,
                    ValueFinRon = valueRon,
                    ValueIni = value,
                    ValueIniRon = valueRon,
                    Guid = Guid.NewGuid(),
                    QuantityRem = sum,
                    Price = price,
                    PriceRon = priceRon,
                    Offer = offer,
                    // BudgetId = budgetDto.BudgetId,
                    // UomId = contract.ContractAmount.UomId,
                    //ContractId = budgetDto.ContractId,
                    // RateId = contract.ContractAmount.RateId,
                    RateId = offer.RateId,
                    // BudgetBaseId = budgetDto.BudgetBaseId,
                    //BudgetForecastId = budgetDto.BudgetForecastId,
                    UomId = offer.UomId,
                    // PreAmount = orderDto.PreAmount,
                    OrderTypeId = orderDto.OrderTypeId,
                    //BudgetValueNeed = budgetValueNeed,
                    //BudgetValueNeedOtherCurrency = budgetValueNeedInOtherCurrency,
                    //StartAccMonthId = orderDto.StartAccMonthId,
                    DivisionId = offer.DivisionId,
                    Contract = contract

                };
                _context.Add(order);

                //orderOp = new Model.OrderOp()
                //{
                //    AccMonthId = inventory.AccMonthId,
                //    AccSystemId = null,
                //    //AccountIdInitial = budgetBaseApproved.AccountId,
                //    //AccountIdFinal = budgetBaseApproved.AccountId,
                //    //AdministrationIdInitial = budgetBaseApproved.AdministrationId,
                //    //AdministrationIdFinal = budgetBaseApproved.AdministrationId,
                //    Order = order,
                //    BudgetManagerIdInitial = inventory.BudgetManagerId,
                //    BudgetManagerIdFinal = inventory.BudgetManagerId,
                //    BudgetStateId = appState.Id,
                //    CompanyIdInitial = budgetForecastApproved.BudgetBase.CompanyId,
                //    CompanyIdFinal = budgetForecastApproved.BudgetBase.CompanyId,
                //    CostCenterIdInitial = budgetForecastApproved.BudgetBase.CostCenterId,
                //    CostCenterIdFinal = budgetForecastApproved.BudgetBase.CostCenterId,
                //    CreatedAt = DateTime.Now,
                //    CreatedBy = _context.UserId,
                //    Document = document,
                //    DstConfAt = DateTime.Now,
                //    DstConfBy = budgetDto.UserId,
                //    EmployeeIdInitial = budgetForecastApproved.BudgetBase.EmployeeId,
                //    EmployeeIdFinal = budgetForecastApproved.BudgetBase.EmployeeId,
                //    InfoIni = budgetDto.Info,
                //    InfoFin = budgetDto.Info,
                //    //InterCompanyIdInitial = budgetBaseApproved.InterCompanyId,
                //    //InterCompanyIdFinal = budgetBaseApproved.InterCompanyId,
                //    IsAccepted = false,
                //    IsDeleted = false,
                //    ModifiedAt = DateTime.Now,
                //    ModifiedBy = _context.UserId,
                //    PartnerIdInitial = offer.PartnerId,
                //    PartnerIdFinal = offer.PartnerId,
                //    ProjectIdInitial = budgetForecastApproved.BudgetBase.ProjectId,
                //    ProjectIdFinal = budgetForecastApproved.BudgetBase.ProjectId,
                //    QuantityIni = sum,
                //    QuantityFin = sum,
                //    SubTypeIdInitial = offer.SubTypeId,
                //    SubTypeIdFinal = offer.SubTypeId,
                //    Validated = true,
                //    ValueFin1 = value,
                //    ValueIni1 = value,
                //    ValueFin2 = value,
                //    ValueIni2 = value,
                //    Guid = Guid.NewGuid(),
                //    //BudgetIdInitial = budgetDto.BudgetId,
                //    //BudgetIdFinal = budgetDto.BudgetId,
                //    OfferIdInitial = budgetDto.OfferId,
                //    OfferIdFinal = budgetDto.OfferId,
                //    UomId = contract.ContractAmount.UomId
                //};

                //_context.Add(orderOp);


                //offerOp = new Model.OfferOp()
                //{
                //    AccMonthId = inventory.AccMonthId,
                //    AccSystemId = null,
                //    AccountIdInitial = offer.AccountId,
                //    AccountIdFinal = offer.AccountId,
                //    AdministrationIdInitial = offer.AdministrationId,
                //    AdministrationIdFinal = offer.AdministrationId,
                //    Offer = offer,
                //    BudgetManagerIdInitial = inventory.BudgetManagerId,
                //    BudgetManagerIdFinal = inventory.BudgetManagerId,
                //    BudgetStateId = offer.AppStateId,
                //    CompanyIdInitial = offer.CompanyId,
                //    CompanyIdFinal = offer.CompanyId,
                //    CostCenterIdInitial = offer.CostCenterId,
                //    CostCenterIdFinal = offer.CostCenterId,
                //    CreatedAt = DateTime.Now,
                //    CreatedBy = _context.UserId,
                //    Document = document,
                //    DstConfAt = DateTime.Now,
                //    DstConfBy = offer.UserId,
                //    EmployeeIdInitial = offer.EmployeeId,
                //    EmployeeIdFinal = offer.EmployeeId,
                //    InfoIni = offer.Info,
                //    InfoFin = offer.Info,
                //    InterCompanyIdInitial = offer.InterCompanyId,
                //    InterCompanyIdFinal = offer.InterCompanyId,
                //    IsAccepted = false,
                //    IsDeleted = false,
                //    ModifiedAt = DateTime.Now,
                //    ModifiedBy = _context.UserId,
                //    PartnerIdInitial = offer.PartnerId,
                //    PartnerIdFinal = offer.PartnerId,
                //    ProjectIdInitial = offer.ProjectId,
                //    ProjectIdFinal = offer.ProjectId,
                //    QuantityIni = offer.Quantity,
                //    QuantityFin = offer.QuantityRem,
                //    SubTypeIdInitial = offer.SubTypeId,
                //    SubTypeIdFinal = offer.SubTypeId,
                //    Validated = true,
                //    ValueFin1 = offer.ValueFin,
                //    ValueIni1 = offer.ValueIni,
                //    ValueFin2 = offer.ValueFin,
                //    ValueIni2 = offer.ValueIni,
                //    Guid = Guid.NewGuid()
                //};

                //_context.Add(offerOp);

                entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
                _context.Update(entityType);

                if (orderType.Code == "C-IT")
                {
                    order.IsAccepted = true;
                }

                _context.SaveChanges();

                for (int i = 0; i < orderDto.StockMaterialUpdates.Count; i++)
                {
                    if (orderType.Code == "C-IT")
                    {
                        stock = await _context.Set<Model.Stock>().Where(s => s.Id == orderDto.StockMaterialUpdates[i]).SingleAsync();

                        for (int q = 0; q < stock.Quantity; q++)
                        {
                            //Dto.AddStockAsset newAssetFromStock = new AddStockAsset();
                            //newAssetFromStock.Id = 0;
                            //newAssetFromStock.Name = "";
                            //newAssetFromStock.SerialNumber = "";
                            //newAssetFromStock.Name2 = "";
                            //newAssetFromStock.StockId = orderDto.StockMaterialUpdates[i];
                            //newAssetFromStock.SubCategoryId = null;
                            //newAssetFromStock.OrderId = order.Id;

                            Dto.AddStockAsset newAssetFromStock = new AddStockAsset
                            {
                                Id = 0,
                                Name = stock.Name,
                                SerialNumber = "",
                                Name2 = stock.LongName,
                                StockId = orderDto.StockMaterialUpdates[i],
                                OrderId = order.Id
                            };

                            subCategories = await _context.Set<Model.SubCategory>()
                                .Where(a => a.CategoryId == stock.CategoryId && a.IsDeleted == false)
                                .ToListAsync();


                            if (subCategories.Count == 0)
                            {
                                category = await _context.Set<Model.Category>().AsNoTracking().Where(a => a.Id == stock.CategoryId).SingleAsync();

                                subCategory = new Model.SubCategory()
                                {
                                    CategoryENId = null,
                                    CategoryId = stock.CategoryId,
                                    Code = category.Code,
                                    CompanyId = stock.CompanyId,
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = _context.UserId,
                                    IsDeleted = false,
                                    ModifiedAt = DateTime.Now,
                                    ModifiedBy = _context.UserId,
                                    Name = category.Name
                                };

                                _context.Add(subCategory);

                                _context.SaveChanges();

                                // return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Sub-Category-Missing", EntityId = stock.Id };
                            }

                            newAssetFromStock.SubCategoryId = subCategories.Count > 0 ? subCategories[0].Id : subCategory.Id;

                            var result = await AddAssetStock(newAssetFromStock);

							if (result.Success)
							{
                                emailManager = await _context.Set<Model.EmailManager>()
                                    .Where(e => e.OrderId == order.Id && e.PartnerId == stock.PartnerId && e.IsDeleted == false)
                                    .FirstOrDefaultAsync();

                                if(emailManager == null)
								{
                                    emailManager = new Model.EmailManager()
                                    {
                                        EmailTypeId = emailType.Id,
                                        Offer = offer,
                                        OrderId = order.Id,
                                        PartnerId = stock.PartnerId,
                                        //SubTypeId = subTypeId,
                                        AppStateId = appState.Id,
                                        CreatedAt = DateTime.Now,
                                        CreatedBy = _context.UserName,
                                        ModifiedAt = DateTime.Now,
                                        ModifiedBy = _context.UserName,
                                        Guid = Guid.NewGuid(),
                                        GuidAll = Guid.NewGuid(),
                                        IsDeleted = false,
                                        CompanyId = offer.CompanyId,
                                        EmployeeIdFinal = offer.EmployeeId,
                                        EmployeeIdInitial = offer.EmployeeId,
                                        Info = "STOCK_IT_MFX"

                                    };

                                    _context.Add(emailManager);
                                }

                              Model.OfferMaterial offerMaterial = await _context.Set<Model.OfferMaterial>()
                                    .Where(e => e.OfferId == offer.Id && e.MaterialId == stock.MaterialId && e.IsDeleted == false)
                                    .FirstOrDefaultAsync();

                                if(offerMaterial == null)
								{
                                    offerMaterial = new Model.OfferMaterial()
                                    {
                                        CreatedAt = DateTime.Now,
                                        CreatedBy = _context.UserId,
                                        ModifiedAt = DateTime.Now,
                                        ModifiedBy = _context.UserId,
                                        Offer = offer,
                                        MaterialId = stock.MaterialId != null ? stock.MaterialId.Value : 627,
                                        EmailManager = emailManager,
                                        AppStateId = 7,
                                        Value = stock.Value,
                                        ValueRon = stock.Value,
                                        ValueIniRon = stock.Value,
                                        ValueIni = stock.Value,
                                        OrdersPrice = stock.Value,
                                        OrdersPriceRon  =stock.Value,
                                        OrdersValue = stock.Value,
                                        OrdersValueRon = stock.Value,
                                        OrdersQuantity = 1,
                                        PriceIni = stock.Value,
                                        PriceIniRon = stock.Value,
                                        Price = stock.Value,
                                        PriceRon = stock.Value,
                                        Quantity = 1,
                                        QuantityIni = 1,
                                        RequestId = offer.RequestId,
                                        RateId = rate.Id,
                                        OrderTypeId = order.OrderTypeId
                                    };

                                    _context.Add(offerMaterial);
								}
								else
								{
                                    offerMaterial.Value += stock.Value;
                                    offerMaterial.ValueRon += stock.Value;
                                    offerMaterial.ValueIni += stock.Value;
                                    offerMaterial.ValueIniRon += stock.Value;
                                    offerMaterial.OrdersValue += stock.Value;
                                    offerMaterial.OrdersValueRon += stock.Value;
                                    offerMaterial.OrdersQuantity++;
                                    offerMaterial.Quantity++;
                                    offerMaterial.QuantityIni++;

                                    _context.Update(offerMaterial);
                                }

                                orderMaterial = await _context.Set<Model.OrderMaterial>()
                                    .Where(e => e.OrderId == order.Id && e.MaterialId == stock.MaterialId && e.RequestId == request.Id && e.IsDeleted == false)
                                    .FirstOrDefaultAsync();

                                if(orderMaterial == null)
								{
                                    orderMaterial = new Model.OrderMaterial()
                                    {
                                        AppStateId = appStateOrderMaterial.Id,
                                        CreatedAt = DateTime.Now,
                                        CreatedBy = _context.UserId,
                                        IsDeleted = false,
                                        MaterialId = stock.MaterialId != null ? stock.MaterialId.Value : offerMaterial.MaterialId,
                                        ModifiedAt = DateTime.Now,
                                        ModifiedBy = _context.UserId,
                                        Order = order,
                                        Price = stock.Value,
                                        PriceRon = stock.Value,
                                        PriceIni = stock.Value,
                                        PriceIniRon = stock.Value,
                                        QuantityIni = 1,
                                        ValueIni = stock.Value,
                                        ValueIniRon = stock.Value,
                                        OrdersQuantity = 1,
                                        Quantity = 1,
                                        OrdersPrice = stock.Value,
                                        OrdersPriceRon = stock.Value,
                                        OrdersValue = stock.Value,
                                        OrdersValueRon = stock.Value,
                                        Value = stock.Value,
                                        ValueRon = stock.Value,
                                        // RateId = contract.ContractAmount.RateId,
                                        RateId = offerMaterial.RateId,
                                        RequestId = offerMaterial.RequestId,
                                        OfferMaterial = offerMaterial,
                                        //PreAmount = offerMaterial.PreAmount,
                                        //PreAmountRon = offerMaterial.PreAmount * offerMaterial.Rate.Value,
                                        Guid = offerMaterial.Guid,
                                        OrderTypeId = order.OrderTypeId

                                    };

                                    _context.Add(orderMaterial);
								}
								else
								{
                                    orderMaterial.Value += stock.Value;
                                    orderMaterial.ValueRon += stock.Value;
                                    orderMaterial.ValueIni += stock.Value;
                                    orderMaterial.ValueIniRon += stock.Value;
                                    orderMaterial.OrdersValue += stock.Value;
                                    orderMaterial.OrdersValueRon += stock.Value;
                                    orderMaterial.OrdersQuantity++;
                                    orderMaterial.Quantity++;
                                    orderMaterial.QuantityIni++;

                                    _context.Update(orderMaterial);
                                }

                                //requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
                                //   .Where(e => e.RequestId == order.Offer.RequestId && e.ContractId == order.ContractId && e.OfferTypeId == order.Offer.OfferTypeId && e.IsDeleted == false)
                                //   .FirstOrDefaultAsync();

                                //if (requestBudgetForecast == null)
                                //{
                                //    requestBudgetForecast = new Model.RequestBudgetForecast()
                                //    {
                                //        AccMonthId = inventory.AccMonthId.Value,
                                //        AppStateId = appStateOrderMaterial.Id,
                                //        BudgetForecastId = null,
                                //        BudgetManagerId = inventory.BudgetManagerId.Value,
                                //        Contract = contract,
                                //        CreatedAt = DateTime.Now,
                                //        CreatedBy = _context.UserId,
                                //        Guid = offerMaterial.Guid,
                                //        IsDeleted = false,
                                //        Materials = "",
                                //        MaxQuantity = 1,
                                //        MaxValue = stock.Value,
                                //        MaxValueRon = stock.Value,
                                //        ModifiedAt = DateTime.Now,
                                //        ModifiedBy = _context.UserId,
                                //        NeedBudget = false,
                                //        NeedContract = false,
                                //        Price = stock.Value,
                                //        PriceRon = stock.Value,
                                //        Quantity = 1,
                                //        RequestId = offerMaterial.RequestId,
                                //        TotalOrderQuantity = 1,
                                //        TotalOrderValue = stock.Value,
                                //        TotalOrderValueRon = stock.Value,
                                //        Value = stock.Value,
                                //        ValueRon = stock.Value,
                                //        NeedBudgetValue = 0,
                                //        NeedContractValue = 0,
                                //        OfferTypeId = order.Offer.OfferTypeId

                                //    };

                                //    _context.Add(requestBudgetForecast);
                                //}
                                //else
                                //{
                                //    requestBudgetForecast.MaxQuantity++;
                                //    requestBudgetForecast.MaxValue += stock.Value;
                                //    requestBudgetForecast.MaxValueRon += stock.Value;
                                //    requestBudgetForecast.Quantity++;
                                //    requestBudgetForecast.TotalOrderQuantity ++;
                                //    requestBudgetForecast.TotalOrderValue += stock.Value;
                                //    requestBudgetForecast.TotalOrderValueRon += stock.Value;
                                //    requestBudgetForecast.Value += stock.Value;
                                //    requestBudgetForecast.ValueRon += stock.Value;

                                //    _context.Update(requestBudgetForecast);
                                //}

                                //requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                                //  .Where(e => e.RequestBudgetForecastId == requestBudgetForecast.Id && e.OrderId == order.Id && e.MaterialId == stock.MaterialId && e.OfferTypeId == order.Offer.OfferTypeId && e.IsDeleted == false)
                                //  .FirstOrDefaultAsync();

                                //if (requestBudgetForecastMaterial == null)
                                //{
                                //    requestBudgetForecastMaterial = new Model.RequestBudgetForecastMaterial()
                                //    {
                                //        AppStateId = appStateOrderMaterial.Id,
                                //        BudgetForecastTimeStamp = 0,
                                //        BudgetValueNeed = 0,
                                //        CreatedAt = DateTime.Now,
                                //        CreatedBy = _context.UserId,
                                //        Guid = offerMaterial.Guid,
                                //        IsDeleted = false,
                                //        MaterialId = stock.MaterialId != null ? stock.MaterialId.Value : 627,
                                //        OfferMaterial = offerMaterial,
                                //        OrderId = order.Id,
                                //        OrderMaterial = orderMaterial,
                                //        PreAmount = 0,
                                //        PreAmountRon = 0,
                                //        Price = stock.Value,
                                //        PriceRon = stock.Value,
                                //        Quantity = 1,
                                //        QuantityRem = 1,
                                //        RequestBudgetForecast = requestBudgetForecast,
                                //        Value = stock.Value,
                                //        ValueRon = stock.Value,
                                //        ValueRem = stock.Value,
                                //        ValueRemRon = stock.Value,
                                //        MaxQuantity = 1,
                                //        MaxValue = stock.Value,
                                //        MaxValueRon = stock.Value,
                                //        ModifiedAt = DateTime.Now,
                                //        ModifiedBy = _context.UserId,
                                //        OfferTypeId = order.Offer.OfferTypeId

                                //    };

                                //    _context.Add(requestBudgetForecastMaterial);
                                //}
                                //else
                                //{
                                //    requestBudgetForecastMaterial.MaxQuantity ++;
                                //    requestBudgetForecastMaterial.MaxValue += stock.Value;
                                //    requestBudgetForecastMaterial.MaxValueRon += stock.Value;
                                //    requestBudgetForecastMaterial.Quantity++;
                                //    requestBudgetForecastMaterial.QuantityRem++;
                                //    requestBudgetForecastMaterial.ValueRemRon += stock.Value;
                                //    requestBudgetForecastMaterial.ValueRem += stock.Value;
                                //    requestBudgetForecastMaterial.Value += stock.Value;
                                //    requestBudgetForecastMaterial.ValueRon += stock.Value;

                                //    _context.Update(requestBudgetForecastMaterial);
                                //}

                                offer.IsAccepted = true;
                                offer.Name = "STOCK_IT_MFX";
                                offer.PartnerId = emailManager.PartnerId;
                                offer.Quantity++;
                                offer.QuantityRem++;
                                offer.QuantityUsed++;
                                offer.UomId = rate.UomId;
                                offer.ValueFin += stock.Value;
                                offer.ValueFinRon += stock.Value;
                                offer.ValueIni += stock.Value;
                                offer.ValueIniRon += stock.Value;
                                offer.ValueUsed += stock.Value;
                                offer.ValueUsedRon += stock.Value;
                                offer.Price += stock.Value;
                                offer.PriceRon += stock.Value;
                                offer.RateId = rate.Id;
                                offer.OrderTypeId = order.OrderTypeId;

                                order.IsAccepted = true;
                                order.Name = "STOCK_IT_MFX";
                                order.PartnerId = emailManager.PartnerId;
                                order.Quantity++;
                                order.QuantityRem++;
                                //order.QuantityUsed++;
                                order.UomId = rate.UomId;
                                order.ValueFin += stock.Value;
                                order.ValueFinRon += stock.Value;
                                order.ValueIni += stock.Value;
                                order.ValueIniRon += stock.Value;
                                //order.ValueUsed += stock.Value;
                                //order.ValueUsedRon += stock.Value;
                                order.Price += stock.Value;
                                order.PriceRon += stock.Value;
                                order.RateId = rate.Id;

                                stock.Imported = true;
                                _context.Update(stock);
                                _context.SaveChanges();
                            }
                        }
                    }

                    //offerMaterial = await _context.Set<Model.OfferMaterial>()
                    //    .Include(r => r.Rate).AsNoTracking()
                    //    .Where(a => a.Id == orderDto.OrderMaterialUpdates[i].Id)
                    //    .FirstOrDefaultAsync();

                    //if (offerMaterial == null)
                    //{
                        

                    //}

                    //if (offerMaterial != null)
                    //{
                    //    offerMaterial.OrdersQuantity += budgetDto.OrderMaterialUpdates[i].Quantity;
                    //    offerMaterial.Quantity -= budgetDto.OrderMaterialUpdates[i].Quantity;
                    //    offerMaterial.OrdersPrice += budgetDto.OrderMaterialUpdates[i].Price;
                    //    offerMaterial.OrdersValue += budgetDto.OrderMaterialUpdates[i].Price * budgetDto.OrderMaterialUpdates[i].Quantity;
                    //    offerMaterial.Value -= budgetDto.OrderMaterialUpdates[i].Price * budgetDto.OrderMaterialUpdates[i].Quantity;
                    //    _context.Update(offerMaterial);

                    //}
                }

                //for (int i = 0; i < orderDto.RequestBudgetForecasts.Count; i++)
                //{
                //    requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
                //        //.Include(b => b.RequestBudgetForecast).ThenInclude(b => b.BudgetForecast)
                //        .Where(r => r.RequestBudgetForecastId == orderDto.RequestBudgetForecasts[i] && r.IsDeleted == false)
                //        .ToListAsync();

                //    for (int r = 0; r < requestBudgetForecastMaterials.Count; r++)
                //    {
                //        requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                //            .Include(b => b.RequestBudgetForecast).ThenInclude(b => b.BudgetForecast)
                //            .Where(a => a.Id == requestBudgetForecastMaterials[r].Id).SingleAsync();

                //        requestBudgetForecastMaterial.OrderId = order.Id;

                //        orderMaterial = await _context.Set<Model.OrderMaterial>()
                //            .Where(r => r.OrderId == order.Id && r.MaterialId == requestBudgetForecastMaterial.MaterialId && r.IsDeleted == false)
                //            .SingleAsync();

                //        orderMaterial.BudgetForecastId = requestBudgetForecastMaterial.RequestBudgetForecast.BudgetForecastId;
                //        order.ContractId = requestBudgetForecastMaterial.RequestBudgetForecast.ContractId;

                //        requestBudgetForecastMaterial.OrderMaterialId = orderMaterial.Id;
                //        //requestBudgetForecastMaterial.BudgetForecastTimeStamp = requestBudgetForecastMaterial.RequestBudgetForecast.BudgetForecast.TotalRem;
                //        //requestBudgetForecastMaterial.ContractTimeStamp = requestBudgetForecastMaterial.RequestBudgetForecast.Contract.ContractAmount.AmountRonRem;

                //        _context.Update(order);
                //        _context.Update(orderMaterial);
                //        _context.Update(requestBudgetForecastMaterial);
                //    }

                //    requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>().Where(r => r.Id == orderDto.RequestBudgetForecasts[i]).SingleAsync();

                //    if (requestBudgetForecast.NeedBudget && requestBudgetForecast.NeedBudgetValue > 0)
                //    {
                //        emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "NEED_BUDGET").SingleAsync();
                //        entityTypeRequestBF = await _context.Set<Model.EntityType>().Where(c => c.Code == "NEED_BUDGET").SingleAsync();
                //        appStateRequestBudgetForecast = await _context.Set<Model.AppState>().Where(c => c.Code == "ORDER_BOOK").SingleAsync();

                //        emailOrderStatus = new Model.EmailOrderStatus()
                //        {
                //            AppStateId = appStateRequestBudgetForecast.Id,
                //            Completed = false,
                //            CreatedAt = DateTime.Now,
                //            CreatedBy = _context.UserId,
                //            DocumentNumber = int.Parse(entityTypeRequestBF.Name),
                //            EmailSend = false,
                //            EmailTypeId = emailType.Id,
                //            EmployeeL1EmailSend = false,
                //            EmployeeL1ValidateAt = null,
                //            EmployeeL1ValidateBy = null,
                //            EmployeeL2EmailSend = false,
                //            EmployeeL2ValidateAt = null,
                //            EmployeeL2ValidateBy = null,
                //            EmployeeL3EmailSend = false,
                //            EmployeeL3ValidateAt = null,
                //            EmployeeL3ValidateBy = null,
                //            EmployeeL4EmailSend = false,
                //            EmployeeL4ValidateAt = null,
                //            EmployeeL4ValidateBy = null,
                //            EmployeeS1EmailSend = false,
                //            EmployeeS1ValidateAt = null,
                //            EmployeeS1ValidateBy = null,
                //            EmployeeS2EmailSend = false,
                //            EmployeeS2ValidateAt = null,
                //            EmployeeS2ValidateBy = null,
                //            EmployeeS3EmailSend = false,
                //            EmployeeS3ValidateAt = null,
                //            EmployeeS3ValidateBy = null,
                //            ErrorId = null,
                //            Exported = false,
                //            FinalValidateAt = null,
                //            FinalValidateBy = null,
                //            Guid = Guid.NewGuid(),
                //            GuidAll = Guid.NewGuid(),
                //            Info = string.Empty,
                //            IsAccepted = false,
                //            IsDeleted = false,
                //            MatrixId = orderDto.MatrixId.Value,
                //            ModifiedAt = DateTime.Now,
                //            ModifiedBy = _context.UserId,
                //            NotCompletedSync = false,
                //            NotEmployeeL1Sync = false,
                //            NotEmployeeL2Sync = false,
                //            NotEmployeeL3Sync = false,
                //            NotEmployeeL4Sync = false,
                //            NotEmployeeS1Sync = false,
                //            NotEmployeeS2Sync = false,
                //            NotEmployeeS3Sync = false,
                //            NotSync = false,
                //            OrderId = order.Id,
                //            SyncCompletedErrorCount = 0,
                //            SyncEmployeeL1ErrorCount = 0,
                //            SyncEmployeeL2ErrorCount = 0,
                //            SyncEmployeeL3ErrorCount = 0,
                //            SyncEmployeeL4ErrorCount = 0,
                //            SyncEmployeeS1ErrorCount = 0,
                //            SyncEmployeeS2ErrorCount = 0,
                //            SyncEmployeeS3ErrorCount = 0,
                //            SyncErrorCount = 0,
                //            EmployeeL1EmailSkip = true,
                //            EmployeeL2EmailSkip = true,
                //            EmployeeL3EmailSkip = true,
                //            EmployeeL4EmailSkip = false,
                //            EmployeeS1EmailSkip = true,
                //            EmployeeS2EmailSkip = true,
                //            EmployeeS3EmailSkip = true,
                //            NeedBudgetEmailSend = false,
                //            NotNeedBudgetSync = false,
                //            SyncNeedBudgetErrorCount = 0,
                //            NeedContractEmailSend = false,
                //            NotNeedContractSync = false,
                //            SyncNeedContractErrorCount = 0,
                //            RequestBudgetForecastId = requestBudgetForecast.Id,
                //            ContractId = order.ContractId
                //        };

                //        entityTypeRequestBF.Name = (int.Parse(entityTypeRequestBF.Name) + 1).ToString();
                //        _context.Update(entityTypeRequestBF);

                //        _context.Add(emailOrderStatus);

                //        order.AppStateId = appStateRequestBudgetForecast.Id;
                //        requestBudgetForecast.AppStateId = appStateRequestBudgetForecast.Id;
                //    }

                //    if (requestBudgetForecast.NeedContract && requestBudgetForecast.NeedContractValue > 0)
                //    {
                //        emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "NEED_CONTRACT").SingleAsync();
                //        appStateRequestBudgetForecast = await _context.Set<Model.AppState>().Where(c => c.Code == "NEED_CONTRACT").SingleAsync();

                //        emailOrderStatus = new Model.EmailOrderStatus()
                //        {
                //            AppStateId = appStateRequestBudgetForecast.Id,
                //            Completed = false,
                //            CreatedAt = DateTime.Now,
                //            CreatedBy = _context.UserId,
                //            DocumentNumber = int.Parse(entityType.Name),
                //            EmailSend = false,
                //            EmailTypeId = emailType.Id,
                //            EmployeeL1EmailSend = false,
                //            EmployeeL1ValidateAt = null,
                //            EmployeeL1ValidateBy = null,
                //            EmployeeL2EmailSend = false,
                //            EmployeeL2ValidateAt = null,
                //            EmployeeL2ValidateBy = null,
                //            EmployeeL3EmailSend = false,
                //            EmployeeL3ValidateAt = null,
                //            EmployeeL3ValidateBy = null,
                //            EmployeeL4EmailSend = false,
                //            EmployeeL4ValidateAt = null,
                //            EmployeeL4ValidateBy = null,
                //            EmployeeS1EmailSend = false,
                //            EmployeeS1ValidateAt = null,
                //            EmployeeS1ValidateBy = null,
                //            EmployeeS2EmailSend = false,
                //            EmployeeS2ValidateAt = null,
                //            EmployeeS2ValidateBy = null,
                //            EmployeeS3EmailSend = false,
                //            EmployeeS3ValidateAt = null,
                //            EmployeeS3ValidateBy = null,
                //            ErrorId = null,
                //            Exported = false,
                //            FinalValidateAt = null,
                //            FinalValidateBy = null,
                //            Guid = Guid.NewGuid(),
                //            GuidAll = Guid.NewGuid(),
                //            Info = string.Empty,
                //            IsAccepted = false,
                //            IsDeleted = false,
                //            MatrixId = orderDto.MatrixId.Value,
                //            ModifiedAt = DateTime.Now,
                //            ModifiedBy = _context.UserId,
                //            NotCompletedSync = false,
                //            NotEmployeeL1Sync = false,
                //            NotEmployeeL2Sync = false,
                //            NotEmployeeL3Sync = false,
                //            NotEmployeeL4Sync = true,
                //            NotEmployeeS1Sync = false,
                //            NotEmployeeS2Sync = false,
                //            NotEmployeeS3Sync = false,
                //            NotSync = false,
                //            OrderId = order.Id,
                //            SyncCompletedErrorCount = 0,
                //            SyncEmployeeL1ErrorCount = 0,
                //            SyncEmployeeL2ErrorCount = 0,
                //            SyncEmployeeL3ErrorCount = 0,
                //            SyncEmployeeL4ErrorCount = 0,
                //            SyncEmployeeS1ErrorCount = 0,
                //            SyncEmployeeS2ErrorCount = 0,
                //            SyncEmployeeS3ErrorCount = 0,
                //            SyncErrorCount = 0,
                //            EmployeeL1EmailSkip = true,
                //            EmployeeL2EmailSkip = true,
                //            EmployeeL3EmailSkip = true,
                //            EmployeeL4EmailSkip = false,
                //            EmployeeS1EmailSkip = true,
                //            EmployeeS2EmailSkip = true,
                //            EmployeeS3EmailSkip = true,
                //            NeedBudgetEmailSend = false,
                //            NotNeedBudgetSync = false,
                //            SyncNeedBudgetErrorCount = 0,
                //            NeedContractEmailSend = false,
                //            NotNeedContractSync = false,
                //            SyncNeedContractErrorCount = 0,
                //            RequestBudgetForecastId = requestBudgetForecast.Id,
                //            ContractId = order.ContractId
                //        };

                //        _context.Add(emailOrderStatus);

                //        order.AppStateId = appStateRequestBudgetForecast.Id;
                //        requestBudgetForecast.AppStateId = appStateRequestBudgetForecast.Id;
                //    }

                //    _context.SaveChanges();
                //}

                var countOffer = await _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToListAsync();
                ////var countOrd = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
                var countContract = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToListAsync();
                var countContractAmount = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToListAsync();
                var countBudget = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
                // var countOfferMaterials = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials").ToList();
                var countOfferMaterials2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials2").ToList();
                var UpdateAllRequestBudgetForecastMaterials = _context.Set<Model.RecordCount>().FromSql("UpdateAllRequestBudgetForecastMaterials").ToList();


                return new Model.OrderResult { Success = true, Message = "Oferta a fost salvata cu succes!", OrderId = order.Id };
            }
            else
            {
                return new Model.OrderResult { Success = false, Message = $"Comanda nu a fost plasata. Valoare comenzii depaseste valoarea disponibila!", OrderId = order.Id };
            }


        }

        public async Task<Model.CreateAssetSAPResult> OrderContractUpdate(OrderContractUpdate orderDto)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.DocumentType documentType = null;
            Model.BudgetBase budgetBaseApproved = null;
            Model.Contract contract = null;
            Model.AppState appState = null;
            Model.Inventory inventory = null;
            Model.BudgetManager budgetManager = null;

            order = await _context.Set<Model.Order>().Include(a => a.Offer).Include(o => o.OrderMaterials).Where(c => c.Id == orderDto.Id).SingleAsync();
            contract = await _context.Set<Model.Contract>().Include(c => c.ContractAmount).Where(c => c.Id == orderDto.ContractId).SingleAsync();
            float sum = (float)order.OrderMaterials.Sum(a => a.Quantity);
            decimal value = order.OrderMaterials.Sum(a => a.Value);
            decimal valueRon = order.OrderMaterials.Sum(a => a.ValueRon);

            if (contract.ContractAmount.AmountRonRem >= valueRon && contract.ContractAmount.AmountRem >= value)
            {
                documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ORDER_CONTRACT_UPDATE").SingleOrDefault();
                inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).FirstOrDefault();
                budgetManager = _context.Set<Model.BudgetManager>().Where(c => c.Code == "2023").Single();

                appState = _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_LEVEL1").FirstOrDefault();
                budgetBaseApproved = _context.Set<Model.BudgetBase>().Where(c => c.Id == order.BudgetBaseId).SingleOrDefault();

                document = new Model.Document()
                {
                    Approved = true,
                    CompanyId = order.Offer.CompanyId,
                    CostCenterId = order.Offer.CostCenterId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserId,
                    CreationDate = DateTime.Now,
                    Details = order.Info != null ? order.Info : string.Empty,
                    DocNo1 = order.Info != null ? order.Info : string.Empty,
                    DocNo2 = order.Info != null ? order.Info : string.Empty,
                    DocumentDate = DateTime.Now,
                    DocumentTypeId = documentType.Id,
                    Exported = true,
                    IsDeleted = true,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = _context.UserId,
                    ParentDocumentId = null,
                    PartnerId = order.Offer.PartnerId,
                    RegisterDate = DateTime.Now,
                    ValidationDate = DateTime.Now,
                };

                _context.Add(document);

                orderOp = new Model.OrderOp()
                {
                    AccMonthId = inventory.AccMonthId,
                    AccSystemId = null,
                    Order = order,
                    BudgetManagerIdInitial = budgetManager.Id,
                    BudgetManagerIdFinal = budgetManager.Id,
                    BudgetStateId = appState.Id,
                    CompanyIdInitial = budgetBaseApproved.CompanyId,
                    CompanyIdFinal = budgetBaseApproved.CompanyId,
                    CostCenterIdInitial = budgetBaseApproved.CostCenterId,
                    CostCenterIdFinal = budgetBaseApproved.CostCenterId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserId,
                    Document = document,
                    DstConfAt = DateTime.Now,
                    DstConfBy = _context.UserId,
                    EmployeeIdInitial = budgetBaseApproved.EmployeeId,
                    EmployeeIdFinal = budgetBaseApproved.EmployeeId,
                    InfoIni = order.Info,
                    InfoFin = order.Info,
                    IsAccepted = false,
                    IsDeleted = true,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = _context.UserId,
                    PartnerIdInitial = order.Offer.PartnerId,
                    PartnerIdFinal = order.Offer.PartnerId,
                    ProjectIdInitial = budgetBaseApproved.ProjectId,
                    ProjectIdFinal = budgetBaseApproved.ProjectId,
                    QuantityIni = sum,
                    QuantityFin = sum,
                    SubTypeIdInitial = order.Offer.SubTypeId,
                    SubTypeIdFinal = order.Offer.SubTypeId,
                    Validated = true,
                    ValueFin1 = value,
                    ValueIni1 = value,
                    ValueFin2 = value,
                    ValueIni2 = value,
                    Guid = Guid.NewGuid(),
                    OfferIdInitial = order.OfferId,
                    OfferIdFinal = order.OfferId,
                    UomId = contract.ContractAmount.UomId
                };

                _context.Add(orderOp);

                order.ContractId = orderDto.ContractId;
                order.AppStateId = appState.Id;
                _context.Update(order);

                _context.SaveChanges();

				var countContracts = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToListAsync();
				var countOffer = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToListAsync();


				return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = order.Id.ToString() };
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = order.Id.ToString() };
            }
        }

        public async Task<Model.CreateAssetSAPResult> CreateOrUpdateOrderCheck(OrderStockSave orderDto)
        {
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.OfferOp offerOp = null;
            Model.Document document = null;
            Model.DocumentType documentType = null;
            Model.EntityType entityType = null;
            Model.Offer offer = null;
            //Model.Budget budgetApproved = null;
            //Model.BudgetBase budgetBaseApproved = null;
            //Model.BudgetForecast budgetForecastApproved = null;
            Model.Contract contract = null;
            Model.AppState appState = null;
            Model.AppState appStateOrderMaterial = null;
            Model.Inventory inventory = null;
            //Model.BudgetManager budgetManager = null;
            Model.OrderType orderType = null;
            //Model.OrderMaterial orderMaterial = null;
            //Model.OfferMaterial offerMaterial = null;
            Model.Request request = null;
            Model.EntityType entityTypeRequest = null;
            Model.EntityType entityTypeOffer = null;
            Model.Company company = null;
            Model.ProjectType projectType = null;
            Model.AssetType assetType = null;
            Model.Division division = null;
            Model.OfferType offerType = null;
            Model.Project project = null;
            Model.ErrorType errorType = null;
            Model.Stock stock = null;
            Model.SubCategory subCategory = null;
            Model.Category category = null;
            Model.ContractAmount contractAmount = null;
            Model.Uom uom = null;
            Model.Rate rate = null;
            List<Model.SubCategory> subCategories = null;
            bool isValid = false;

            //float sum = (float)orderDto.OrderMaterialUpdates.Sum(a => a.Quantity);
            //decimal price = orderDto.OrderMaterialUpdates.Sum(a => a.Price);
            //decimal priceRon = orderDto.OrderMaterialUpdates.Sum(a => a.PriceRon);
            //decimal value = orderDto.OrderMaterialUpdates.Sum(a => a.Value);
            //decimal valueRon = orderDto.OrderMaterialUpdates.Sum(a => a.ValueRon);
            //decimal budgetValueNeed = 0;
            //decimal budgetValueNeedInOtherCurrency = 0;

            orderType = await _context.Set<Model.OrderType>().Where(c => c.Id == orderDto.OrderTypeId).FirstOrDefaultAsync();
            if (orderType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Lipsa tip comanda!" };

            documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_ORDER_CHECK").SingleOrDefault();

            if (documentType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Document-Type-Missing" };

            inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).FirstOrDefault();

            if (inventory == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Inventory-Missing" };

            //order = await _context.Set<Model.Order>()
            //    .Include(o => o.Offer)
            //   .Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();

            if (order == null)
            {
                request = await _context.Set<Model.Request>().Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();

                if(request == null)
				{
                    projectType = await _context.Set<Model.ProjectType>().Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();
                    if (projectType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Nu exista Tip Proiect!" };

                    assetType = await _context.Set<Model.AssetType>().Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();
                    if (assetType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Nu exista Cost Type!" };

                    division = await _context.Set<Model.Division>().Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();
                    if (division == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Nu exista Divizie!" };

                    company = await _context.Set<Model.Company>().Where(c => c.Code == "RO10").FirstOrDefaultAsync();
                    if (company == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Nu exista compania!" };

                    inventory = await _context.Set<Model.Inventory>().Where(c => c.Active == true).FirstOrDefaultAsync();
                    if (inventory == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Nu exista inventar activ!" };

                    appState = await _context.Set<Model.AppState>().Where(c => c.Code == "ACCEPTED").FirstOrDefaultAsync();
                    if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Nu exista stare!" };

                    offerType = await _context.Set<Model.OfferType>().Where(c => c.Code == "O-C").FirstOrDefaultAsync();
                    if (offerType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Lipsa tip oferta!" };

                    uom = await _context.Set<Model.Uom>().Where(com => com.Code == "RON").FirstOrDefaultAsync();
                    if (uom == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Lipsa Moneda!" };
                    rate = await _context.Set<Model.Rate>().Include(r => r.Uom).Where(com => com.Uom.Code == "RON" && com.IsLast == true).LastOrDefaultAsync();
                    if(rate == null)
					{
                        rate = await _context.Set<Model.Rate>().Include(r => r.Uom).Where(com => com.Uom.Code == "RON" && com.IsLast == false).LastOrDefaultAsync();
                    }
                    if (rate == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Lipsa Curs BNR!" };

                    //entityTypeRequest = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWREQUEST").FirstOrDefaultAsync();
                    //if (entityTypeRequest == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Nu exista entityType!" };

                    //var lastCodeRequest = int.Parse(entityTypeRequest.Name);
                    //var newRequestCode = string.Empty;

                    //if (lastCodeRequest.ToString().Length == 1)
                    //{
                    //    newRequestCode = "PR000000" + entityTypeRequest.Name;
                    //}
                    //else if (lastCodeRequest.ToString().Length == 2)
                    //{
                    //    newRequestCode = "PR00000" + entityTypeRequest.Name;
                    //}
                    //else if (lastCodeRequest.ToString().Length == 3)
                    //{
                    //    newRequestCode = "PR0000" + entityTypeRequest.Name;
                    //}
                    //else if (lastCodeRequest.ToString().Length == 4)
                    //{
                    //    newRequestCode = "PR000" + entityTypeRequest.Name;
                    //}
                    //else if (lastCodeRequest.ToString().Length == 5)
                    //{
                    //    newRequestCode = "PR00" + entityTypeRequest.Name;
                    //}
                    //else if (lastCodeRequest.ToString().Length == 6)
                    //{
                    //    newRequestCode = "PR0" + entityTypeRequest.Name;
                    //}
                    //else if (lastCodeRequest.ToString().Length == 7)
                    //{
                    //    newRequestCode = "PR" + entityTypeRequest.Name;
                    //}

                    request = new Model.Request()
                    {
                        AccMonthId = inventory.AccMonthId,
                        AppStateId = appState.Id,
                        BudgetManagerId = inventory.BudgetManagerId,
                        Code = "STOCK_IT",
                        CreatedAt = DateTime.Now,
                        CreatedBy = _context.UserId,
                        EndDate = null,
                        StartDate = DateTime.Now,
                        Info = "STOCK IT",
                        IsAccepted = false,
                        IsDeleted = true,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId,
                        Name = "STOCK_IT",
                        UserId = _context.UserId,
                        Validated = true,
                        ProjectTypeId = projectType.Id,
                        AssetTypeId = assetType.Id,
                        EmployeeId = orderDto.EmployeeId,
                        OwnerId = orderDto.EmployeeId,
                        BudgetValueNeed = 0,
                        CompanyId = company.Id,
                        StartAccMonthId = null,
                        Guid = Guid.NewGuid(),
                        DivisionId = division.Id

                    };

                    _context.Add(request);
                }

				offer = await _context.Set<Model.Offer>()
					.Include(e => e.Request)
					.Include(r => r.Rate).ThenInclude(u => u.Uom)
					.Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();

                if(offer == null)
				{
                    //entityTypeOffer = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWOFFER").FirstOrDefaultAsync();
                    //if (entityTypeOffer == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Nu exista entityType!" };

                    //var lastOfferCode = int.Parse(entityTypeOffer.Name);
                    //var newOfferCode = string.Empty;

                    //if (lastOfferCode.ToString().Length == 1)
                    //{
                    //    newOfferCode = "OFFER000000" + entityTypeOffer.Name;
                    //}
                    //else if (lastOfferCode.ToString().Length == 2)
                    //{
                    //    newOfferCode = "OFFER00000" + entityTypeOffer.Name;
                    //}
                    //else if (lastOfferCode.ToString().Length == 3)
                    //{
                    //    newOfferCode = "OFFER0000" + entityTypeOffer.Name;
                    //}
                    //else if (lastOfferCode.ToString().Length == 4)
                    //{
                    //    newOfferCode = "OFFER000" + entityTypeOffer.Name;
                    //}
                    //else if (lastOfferCode.ToString().Length == 5)
                    //{
                    //    newOfferCode = "OFFER00" + entityTypeOffer.Name;
                    //}
                    //else if (lastOfferCode.ToString().Length == 6)
                    //{
                    //    newOfferCode = "OFFER0" + entityTypeOffer.Name;
                    //}
                    //else if (lastOfferCode.ToString().Length == 7)
                    //{
                    //    newOfferCode = "OFFER" + entityTypeOffer.Name;
                    //}


                    offer = new Model.Offer()
                    {
                        AccMonthId = inventory.AccMonthId,
                        AccountId = null,
                        AdministrationId = null,
                        AppStateId = appState.Id,
                        BudgetManagerId = inventory.BudgetManagerId,
                        Code = "STOCK_IT",
                        CompanyId = request.CompanyId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _context.UserId,
                        EmployeeId = orderDto.EmployeeId,
                        EndDate = request.StartDate,
                        StartDate = request.EndDate,
                        Info = request.Info,
                        InterCompanyId = null,
                        IsAccepted = false,
                        IsDeleted = true,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId,
                        PartnerId = null,
                        Quantity = 0,
                        SubTypeId = null,
                        UserId = _context.UserId,
                        Validated = true,
                        ValueFin = 0,
                        ValueIni = 0,
                        Guid = Guid.NewGuid(),
                        QuantityRem = 0,
                        AdmCenterId = null,
                        RegionId = null,
                        Request = request,
                        AssetTypeId = request.AssetTypeId,
                        ProjectTypeId = request.ProjectTypeId,
                        OfferTypeId = offerType.Id,
                        DivisionId = request.DivisionId

                    };

                    _context.Add(offer);
                }

                contract = await _context.Set<Model.Contract>().Where(c => c.Code == "STOCK_IT").FirstOrDefaultAsync();

                if(contract == null)
				{
                    contractAmount = new Model.ContractAmount()
                    {
                        Amount = 0,
                        Uom = uom,
                        Rate = rate,
                        IsDeleted = true,
                        Code = string.Empty,
                        Name = string.Empty,
                        AmountRem = 0
                    };

                    _context.Add(contractAmount);

                    contract = new Model.Contract()
                    {
                        ContractId = "STOCK_IT",
                        Title = "STOCK_IT",
                        Name = "STOCK_IT",
                        AppState = appState,
                        EffectiveDate = null,
                        AgreementDate = null,
                        ExpirationDate = null,
                        CreationDate = null,
                        Version = "",
                        TemplateId = "",
                        AmendmentType = "",
                        AmendmentReason = "",
                        Origin = 0,
                        HierarchicalType = "",
                        ExpirationTermType = "",
                        RelatedId = "",
                        MaximumNumberOfRenewals = 0,
                        AutoRenewalInterval = 0,
                        IsTestProject = true,
                        Owner = null,
                        Partner = null,
                        BusinessSystem = null,
                        ContractAmount = contractAmount,
                        Code = "STOCK_IT",
                        PaymentTerms = 0,
                        Company = company,
                        IsDeleted = true
                    };

                    _context.Add(contract);
                };

                _context.SaveChanges();
            }

            //budgetForecastApproved = _context.Set<Model.BudgetForecast>().Include(b => b.BudgetBase).Where(c => c.Id == orderDto.BudgetForecastId).SingleOrDefault();

            //if (budgetForecastApproved == null)
            //{
            //    return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Budget-Base-Missing" };
            //}

            //if (orderDto.NeedBudgetAmount)
            //{
            //    budgetValueNeed = value - budgetForecastApproved.TotalRem;
            //    budgetValueNeedInOtherCurrency = (value - budgetForecastApproved.TotalRem) / offer.Rate.Value;
            //}

            //         if (orderType != null && orderType.Code != "C-IT" && (contract.ContractAmount.AmountRonRem >= valueRon && budgetForecastApproved.TotalRem >= valueRon && offer.QuantityRem >= sum))
            //         {
            //             isValid = true;
            //}
            //else
            //{
            //             if(budgetForecastApproved.TotalRem >= valueRon)
            //	{
            //                 isValid = true;
            //             }
            //	else
            //	{
            //                 return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Budget-Base-NOT-OK" };
            //             }

            //         }

            isValid = true;

            if (isValid)
            {
                entityType = _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWORDERCHECK").FirstOrDefault();
                if (entityType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Entity-Type-Missing" };

                appState = _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ACCEPTED").FirstOrDefault();
                if (appState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "App-State-Accepted-Missing" };

                project = _context.Set<Model.Project>().AsNoTracking().Where(c => c.Code == "STOCK_IT").FirstOrDefault();
                if (project == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Lipsa project" };

                errorType = await _context.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "NEWASSETCHECK").SingleOrDefaultAsync();
                if (errorType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Error-Type-Missing" };

                //if (orderDto.NeedBudgetAmount)
                //{
                //    appState = _context.Set<Model.AppState>().Where(c => c.Code == "NEED_BUDGET").FirstOrDefault();

                //    if (appState == null)
                //    {
                //        return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "App-State-Need-BGT-Missing" };
                //    }
                //}

                appStateOrderMaterial = _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "PENDING").FirstOrDefault();

                if (appStateOrderMaterial == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "App-State-Pending-Missing" };

                //if (orderDto.NeedBudgetAmount)
                //{
                //    appStateOrderMaterial = _context.Set<Model.AppState>().Where(c => c.Code == "NEED_BUDGET").FirstOrDefault();

                //    if (appStateOrderMaterial == null)
                //    {
                //        return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "App-State-Need-BGT-Missing" };
                //    }
                //}

                var lastCode = int.Parse(entityType.Name);
                var newBudgetCode = string.Empty;

                if (lastCode.ToString().Length == 1)
                {
                    newBudgetCode = "ORDC000000" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 2)
                {
                    newBudgetCode = "ORDC00000" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 3)
                {
                    newBudgetCode = "ORDC0000" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 4)
                {
                    newBudgetCode = "ORDC000" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 5)
                {
                    newBudgetCode = "ORDC00" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 6)
                {
                    newBudgetCode = "ORDC0" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 7)
                {
                    newBudgetCode = "ORDC" + entityType.Name;
                }

                document = new Model.Document()
                {
                    Approved = true,
                    CompanyId = offer.CompanyId,
                    CostCenterId = offer.CostCenterId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserId,
                    CreationDate = DateTime.Now,
                    Details = orderDto.Info != null ? orderDto.Info : string.Empty,
                    DocNo1 = orderDto.Info != null ? orderDto.Info : string.Empty,
                    DocNo2 = orderDto.Info != null ? orderDto.Info : string.Empty,
                    DocumentDate = DateTime.Now,
                    DocumentTypeId = documentType.Id,
                    Exported = true,
                    IsDeleted = true,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = orderDto.UserId,
                    ParentDocumentId = null,
                    PartnerId = offer.PartnerId,
                    RegisterDate = DateTime.Now,
                    ValidationDate = DateTime.Now,
                };

                _context.Add(document);


                order = new Model.Order()
                {
                    AccMonthId = inventory.AccMonthId,
                    AccountId = offer.AccountId,
                    AdministrationId = offer.AdministrationId,
                    AppStateId = appState.Id,
                    BudgetManagerId = inventory.BudgetManagerId,
                    Code = newBudgetCode,
                    CompanyId = offer.CompanyId,
                    CostCenterId = offer.CostCenterId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserId,
                    EmployeeId = offer.EmployeeId,
                    EndDate = null,
                    StartDate = DateTime.Now,
                    Info = offer.Info,
                    InterCompanyId = offer.InterCompanyId,
                    IsAccepted = true,
                    IsDeleted = true,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = _context.UserId,
                    Name = "STOCK_IT",
                    PartnerId = offer.PartnerId,
                    ProjectId = project.Id,
                    Quantity = 0,
                    SubTypeId = offer.SubTypeId,
                    UserId = _context.UserId,
                    Validated = true,
                    ValueFin = 0,
                    ValueFinRon = 0,
                    ValueIni = 0,
                    ValueIniRon = 0,
                    Guid = Guid.NewGuid(),
                    QuantityRem = 0,
                    Price = 0,
                    PriceRon = 0,
                    Offer = offer,
                    Contract = contract,
                    Rate = rate,
                    // BudgetBaseId = budgetDto.BudgetBaseId,
                    //BudgetForecastId = orderDto.BudgetForecastId,
                    Uom = uom,
                    PreAmount = 0,
                    OrderTypeId = orderType.Id,
                    BudgetValueNeed = 0,
                    BudgetValueNeedOtherCurrency = 0,
                    StartAccMonthId = null,
                };

                _context.Add(order);

                //orderOp = new Model.OrderOp()
                //{
                //    AccMonthId = inventory.AccMonthId,
                //    AccSystemId = null,
                //    //AccountIdInitial = budgetBaseApproved.AccountId,
                //    //AccountIdFinal = budgetBaseApproved.AccountId,
                //    //AdministrationIdInitial = budgetBaseApproved.AdministrationId,
                //    //AdministrationIdFinal = budgetBaseApproved.AdministrationId,
                //    Order = order,
                //    BudgetManagerIdInitial = budgetManager.Id,
                //    BudgetManagerIdFinal = budgetManager.Id,
                //    BudgetStateId = appState.Id,
                //    CompanyIdInitial = budgetForecastApproved.BudgetBase.CompanyId,
                //    CompanyIdFinal = budgetForecastApproved.BudgetBase.CompanyId,
                //    CostCenterIdInitial = budgetForecastApproved.BudgetBase.CostCenterId,
                //    CostCenterIdFinal = budgetForecastApproved.BudgetBase.CostCenterId,
                //    CreatedAt = DateTime.Now,
                //    CreatedBy = _context.UserId,
                //    Document = document,
                //    DstConfAt = DateTime.Now,
                //    DstConfBy = orderDto.UserId,
                //    EmployeeIdInitial = budgetForecastApproved.BudgetBase.EmployeeId,
                //    EmployeeIdFinal = budgetForecastApproved.BudgetBase.EmployeeId,
                //    InfoIni = orderDto.Info,
                //    InfoFin = orderDto.Info,
                //    //InterCompanyIdInitial = budgetBaseApproved.InterCompanyId,
                //    //InterCompanyIdFinal = budgetBaseApproved.InterCompanyId,
                //    IsAccepted = false,
                //    IsDeleted = true,
                //    ModifiedAt = DateTime.Now,
                //    ModifiedBy = _context.UserId,
                //    PartnerIdInitial = offer.PartnerId,
                //    PartnerIdFinal = offer.PartnerId,
                //    ProjectIdInitial = budgetForecastApproved.BudgetBase.ProjectId,
                //    ProjectIdFinal = budgetForecastApproved.BudgetBase.ProjectId,
                //    QuantityIni = sum,
                //    QuantityFin = sum,
                //    SubTypeIdInitial = offer.SubTypeId,
                //    SubTypeIdFinal = offer.SubTypeId,
                //    Validated = true,
                //    ValueFin1 = value,
                //    ValueIni1 = value,
                //    ValueFin2 = value,
                //    ValueIni2 = value,
                //    Guid = Guid.NewGuid(),
                //    //BudgetIdInitial = budgetDto.BudgetId,
                //    //BudgetIdFinal = budgetDto.BudgetId,
                //    OfferIdInitial = orderDto.OfferId,
                //    OfferIdFinal = orderDto.OfferId,
                //    UomId = contract.ContractAmount.UomId
                //};

                //_context.Add(orderOp);


                //offerOp = new Model.OfferOp()
                //{
                //    AccMonthId = inventory.AccMonthId,
                //    AccSystemId = null,
                //    AccountIdInitial = offer.AccountId,
                //    AccountIdFinal = offer.AccountId,
                //    AdministrationIdInitial = offer.AdministrationId,
                //    AdministrationIdFinal = offer.AdministrationId,
                //    Offer = offer,
                //    BudgetManagerIdInitial = budgetManager.Id,
                //    BudgetManagerIdFinal = budgetManager.Id,
                //    BudgetStateId = offer.AppStateId,
                //    CompanyIdInitial = offer.CompanyId,
                //    CompanyIdFinal = offer.CompanyId,
                //    CostCenterIdInitial = offer.CostCenterId,
                //    CostCenterIdFinal = offer.CostCenterId,
                //    CreatedAt = DateTime.Now,
                //    CreatedBy = _context.UserId,
                //    Document = document,
                //    DstConfAt = DateTime.Now,
                //    DstConfBy = offer.UserId,
                //    EmployeeIdInitial = offer.EmployeeId,
                //    EmployeeIdFinal = offer.EmployeeId,
                //    InfoIni = offer.Info,
                //    InfoFin = offer.Info,
                //    InterCompanyIdInitial = offer.InterCompanyId,
                //    InterCompanyIdFinal = offer.InterCompanyId,
                //    IsAccepted = false,
                //    IsDeleted = true,
                //    ModifiedAt = DateTime.Now,
                //    ModifiedBy = _context.UserId,
                //    PartnerIdInitial = offer.PartnerId,
                //    PartnerIdFinal = offer.PartnerId,
                //    ProjectIdInitial = offer.ProjectId,
                //    ProjectIdFinal = offer.ProjectId,
                //    QuantityIni = offer.Quantity,
                //    QuantityFin = offer.QuantityRem,
                //    SubTypeIdInitial = offer.SubTypeId,
                //    SubTypeIdFinal = offer.SubTypeId,
                //    Validated = true,
                //    ValueFin1 = offer.ValueFin,
                //    ValueIni1 = offer.ValueIni,
                //    ValueFin2 = offer.ValueFin,
                //    ValueIni2 = offer.ValueIni,
                //    Guid = Guid.NewGuid()
                //};

                //_context.Add(offerOp);

                entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
                _context.Update(entityType);

                 _context.SaveChanges();

                //          for (int i = 0; i < orderDto.OrderMaterialUpdates.Count; i++)
                //          {
                //              for (int q = 0; q < orderDto.OrderMaterialUpdates[i].Quantity; q++)
                //              {
                //                  Model.Stock stock = await _context.Set<Model.Stock>().Where(a => a.Id == orderDto.OrderMaterialUpdates[i].Id).SingleOrDefaultAsync();


                //                  if (stock == null)
                //                  {
                //                      return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Stock-Missing" };
                //                  }

                //                  Dto.AddStockAsset newAssetFromStock = new AddStockAsset();
                //                  newAssetFromStock.Id = 0;
                //                  newAssetFromStock.Name = stock.Name;
                //                  newAssetFromStock.SerialNumber = "";
                //                  newAssetFromStock.Name2 = stock.LongName;
                //                  newAssetFromStock.StockId = orderDto.OrderMaterialUpdates[i].Id;

                //                  List<Model.SubCategory> subCategories = await _context.Set<Model.SubCategory>().Where(a => a.CategoryId == stock.CategoryId && a.IsDeleted == false).ToListAsync();
                //                  Model.SubCategory subCategory = null;
                //                  Model.Category category = null;

                //                  if (subCategories.Count == 0)
                //                  {
                //	category = _context.Set<Model.Category>().AsNoTracking().Where(a => a.Id == stock.CategoryId).SingleOrDefault();
                //                      subCategory = new Model.SubCategory()
                //                      {
                //                          CategoryENId = null,
                //                          CategoryId = stock.CategoryId,
                //                          Code = category.Code,
                //                          CompanyId = stock.CompanyId,
                //                          CreatedAt = DateTime.Now,
                //                          CreatedBy = _context.UserId,
                //                          IsDeleted = false,
                //                          ModifiedAt = DateTime.Now,
                //                          ModifiedBy = _context.UserId,
                //                          Name = category.Name
                //                      };

                //                      _context.Add(subCategory);
                //                      _context.SaveChanges();

                //                      // return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Sub-Category-Missing", EntityId = stock.Id };
                //                  }

                //                  newAssetFromStock.SubCategoryId = subCategories.Count > 0 ? subCategories[0].Id : subCategory.Id;

                //                 var res = await AddAssetStockCheck(newAssetFromStock);

                //if (!res.Success)
                //{
                //                      Model.Error error = new Model.Error();

                //                      error.AssetId = null;
                //                      error.ErrorTypeId = errorType.Id;
                //                      error.CreatedAt = DateTime.Now;
                //                      error.CreatedBy = _context.UserId;
                //                      error.ModifiedAt = DateTime.Now;
                //                      error.ModifiedBy = _context.UserId;
                //                      error.Code = "AddAssetStockCheck";
                //                      error.Request = JsonConvert.SerializeObject(newAssetFromStock, Formatting.Indented).ToString();
                //                      error.Name = res.ErrorMessage;
                //                      error.UserId = _context.UserId;
                //                      error.IsDeleted = false;

                //                      _context.Add(error);

                //                      stock.Validated = false;
                //                      stock.Error = error;
                //                      _context.Update(stock);
                //                      _context.SaveChanges();
                //}
                //else
                //{
                //                      stock.Validated = true;
                //                      stock.ErrorId = null;
                //                      _context.Update(stock);
                //                      _context.SaveChanges();
                //}
                //              }


                //              offerMaterial = _context.Set<Model.OfferMaterial>().Include(r => r.Rate).AsNoTracking().Where(a => a.Id == orderDto.OrderMaterialUpdates[i].Id).SingleOrDefault();

                //              if (offerMaterial == null)
                //              {
                //                  offerMaterial = new Model.OfferMaterial()
                //                  {
                //                      CreatedAt = DateTime.Now,
                //                      CreatedBy = _context.UserId,
                //                      ModifiedAt = DateTime.Now,
                //                      ModifiedBy = _context.UserId,
                //                      OfferId = orderDto.OfferId.Value,
                //                      MaterialId = 627,
                //                      EmailManagerId = 2076,
                //                      AppStateId = 7,
                //                      Value = value,
                //                      ValueRon = valueRon,
                //                      Price = price,
                //                      PriceRon = priceRon,
                //                      Quantity = (decimal)sum,
                //                      RequestId = offer.RequestId,
                //                      RateId = 2669,
                //                      IsDeleted = true
                //                  };

                //                  _context.Add(offerMaterial);

                //              }

                //              orderMaterial = new Model.OrderMaterial()
                //              {
                //                  AppStateId = appStateOrderMaterial.Id,
                //                  CreatedAt = DateTime.Now,
                //                  CreatedBy = _context.UserId,
                //                  IsDeleted = true,
                //                  MaterialId = offerMaterial.MaterialId,
                //                  ModifiedAt = DateTime.Now,
                //                  ModifiedBy = _context.UserId,
                //                  Order = order,
                //                  Price = orderDto.OrderMaterialUpdates[i].Price,
                //                  PriceRon = orderDto.OrderMaterialUpdates[i].PriceRon,
                //                  PriceIni = orderDto.OrderMaterialUpdates[i].Price,
                //                  PriceIniRon = orderDto.OrderMaterialUpdates[i].Price,
                //                  QuantityIni = (decimal)orderDto.OrderMaterialUpdates[i].Quantity,
                //                  ValueIni = orderDto.OrderMaterialUpdates[i].Value,
                //                  ValueIniRon = orderDto.OrderMaterialUpdates[i].ValueRon,
                //                  OrdersQuantity = orderDto.OrderMaterialUpdates[i].Quantity,
                //                  Quantity = (decimal)orderDto.OrderMaterialUpdates[i].Quantity,
                //                  OrdersPrice = orderDto.OrderMaterialUpdates[i].Price,
                //                  OrdersPriceRon = orderDto.OrderMaterialUpdates[i].PriceRon,
                //                  OrdersValue = orderDto.OrderMaterialUpdates[i].Value,
                //                  OrdersValueRon = orderDto.OrderMaterialUpdates[i].ValueRon,
                //                  Value = orderDto.OrderMaterialUpdates[i].Value,
                //                  ValueRon = orderDto.OrderMaterialUpdates[i].ValueRon,
                //                  // RateId = contract.ContractAmount.RateId,
                //                  RateId = offerMaterial.RateId,
                //                  RequestId = offerMaterial.RequestId,
                //                  OfferMaterialId = offerMaterial.Id,
                //                  PreAmount = orderDto.OrderMaterialUpdates[i].PreAmount,
                //                  PreAmountRon = orderDto.OrderMaterialUpdates[i].PreAmount * offerMaterial.Rate.Value,
                //                  Guid = offerMaterial.Guid

                //              };

                //              _context.Add(orderMaterial);
                //              _context.SaveChanges();
                //          }

                for (int i = 0; i < orderDto.StockMaterialUpdates.Count; i++)
                {

                    //for (int q = 0; q < orderDto.OrderMaterialUpdates[i].Quantity; q++)
                    //{

                    //}

     //               if(orderDto.PlantInitialId != null)
					//{

					//}

     //               if (orderDto.PlantFinalId != null)
     //               {

     //               }

     //               if (orderDto.CategoryId != null)
     //               {

     //               }

                    stock = await _context.Set<Model.Stock>().Where(a => a.Id == orderDto.StockMaterialUpdates[i]).SingleAsync();

					Dto.AddStockAsset newAssetFromStock = new AddStockAsset
					{
						Id = 0,
						Name = stock.Name,
						SerialNumber = "",
						Name2 = stock.LongName,
						StockId = orderDto.StockMaterialUpdates[i],
                        OrderId = order.Id
					};

				    subCategories = await _context.Set<Model.SubCategory>().Where(a => a.CategoryId == stock.CategoryId && a.IsDeleted == false).ToListAsync();
                    

                    if (subCategories.Count == 0)
                    {
                        category = await _context.Set<Model.Category>().AsNoTracking().Where(a => a.Id == stock.CategoryId).SingleAsync();

                        subCategory = new Model.SubCategory()
                        {
                            CategoryENId = null,
                            CategoryId = stock.CategoryId,
                            Code = category.Code,
                            CompanyId = stock.CompanyId,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _context.UserId,
                            IsDeleted = false,
                            ModifiedAt = DateTime.Now,
                            ModifiedBy = _context.UserId,
                            Name = category.Name
                        };

                        _context.Add(subCategory);

                        _context.SaveChanges();

                        // return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Sub-Category-Missing", EntityId = stock.Id };
                    }

                    newAssetFromStock.SubCategoryId = subCategories.Count > 0 ? subCategories[0].Id : subCategory.Id;

                    var res = await AddAssetStockCheck(newAssetFromStock);

                    if (!res.Success)
                    {
                        Model.Error error = new Model.Error();

                        error.AssetId = null;
                        error.ErrorTypeId = errorType.Id;
                        error.CreatedAt = DateTime.Now;
                        error.CreatedBy = _context.UserId;
                        error.ModifiedAt = DateTime.Now;
                        error.ModifiedBy = _context.UserId;
                        error.Code = "AddAssetStockCheck";
                        error.Request = JsonConvert.SerializeObject(newAssetFromStock, Formatting.Indented).ToString();
                        error.Name = res.ErrorMessage;
                        error.UserId = _context.UserId;
                        error.IsDeleted = false;

                        _context.Add(error);

                        stock.Validated = false;
                        stock.Error = error;
                        _context.Update(stock);
                        _context.SaveChanges();
                    }
                    else
                    {
                        stock.Validated = true;
                        stock.ErrorId = null;
                        _context.Update(stock);
                        _context.SaveChanges();
                    }


                    //offerMaterial = _context.Set<Model.OfferMaterial>()
                    //    .Include(r => r.Rate).AsNoTracking()
                    //    .Where(a => a.Id == orderDto.OrderMaterialUpdates[i].Id)
                    //    .SingleOrDefault();

                    //if (offerMaterial == null)
                    //{
                    //    offerMaterial = new Model.OfferMaterial()
                    //    {
                    //        CreatedAt = DateTime.Now,
                    //        CreatedBy = _context.UserId,
                    //        ModifiedAt = DateTime.Now,
                    //        ModifiedBy = _context.UserId,
                    //        OfferId = orderDto.OfferId.Value,
                    //        MaterialId = 627,
                    //        EmailManagerId = 2076,
                    //        AppStateId = 7,
                    //        Value = value,
                    //        ValueRon = valueRon,
                    //        Price = price,
                    //        PriceRon = priceRon,
                    //        Quantity = (decimal)sum,
                    //        RequestId = offer.RequestId,
                    //        RateId = 2669,
                    //        IsDeleted = true
                    //    };

                    //    _context.Add(offerMaterial);

                    //}

                    //orderMaterial = new Model.OrderMaterial()
                    //{
                    //    AppStateId = appStateOrderMaterial.Id,
                    //    CreatedAt = DateTime.Now,
                    //    CreatedBy = _context.UserId,
                    //    IsDeleted = true,
                    //    MaterialId = offerMaterial.MaterialId,
                    //    ModifiedAt = DateTime.Now,
                    //    ModifiedBy = _context.UserId,
                    //    Order = order,
                    //    Price = orderDto.OrderMaterialUpdates[i].Price,
                    //    PriceRon = orderDto.OrderMaterialUpdates[i].PriceRon,
                    //    PriceIni = orderDto.OrderMaterialUpdates[i].Price,
                    //    PriceIniRon = orderDto.OrderMaterialUpdates[i].Price,
                    //    QuantityIni = (decimal)orderDto.OrderMaterialUpdates[i].Quantity,
                    //    ValueIni = orderDto.OrderMaterialUpdates[i].Value,
                    //    ValueIniRon = orderDto.OrderMaterialUpdates[i].ValueRon,
                    //    OrdersQuantity = orderDto.OrderMaterialUpdates[i].Quantity,
                    //    Quantity = (decimal)orderDto.OrderMaterialUpdates[i].Quantity,
                    //    OrdersPrice = orderDto.OrderMaterialUpdates[i].Price,
                    //    OrdersPriceRon = orderDto.OrderMaterialUpdates[i].PriceRon,
                    //    OrdersValue = orderDto.OrderMaterialUpdates[i].Value,
                    //    OrdersValueRon = orderDto.OrderMaterialUpdates[i].ValueRon,
                    //    Value = orderDto.OrderMaterialUpdates[i].Value,
                    //    ValueRon = orderDto.OrderMaterialUpdates[i].ValueRon,
                    //    // RateId = contract.ContractAmount.RateId,
                    //    RateId = offerMaterial.RateId,
                    //    RequestId = offerMaterial.RequestId,
                    //    OfferMaterialId = offerMaterial.Id,
                    //    PreAmount = orderDto.OrderMaterialUpdates[i].PreAmount,
                    //    PreAmountRon = orderDto.OrderMaterialUpdates[i].PreAmount * offerMaterial.Rate.Value,
                    //    Guid = offerMaterial.Guid

                    //};

                    //_context.Add(orderMaterial);
                    //_context.SaveChanges();
                }

                return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = order.Id.ToString() };
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = order.Id.ToString() };
            }


        }

        public async Task<Model.CreateAssetSAPResult> AddAssetStock(AddStockAsset assetDto)
        {
            Model.Asset asset = null;
            Model.AssetDep assetDep = null;
            Model.AssetDepMD assetDepMD = null;
            Model.AssetInv assetInv = null;
            Model.AssetAC assetAC = null;
            Model.AssetAdmMD assetAdmMD = null;
            Model.AccMonth accMonth = null;
            Model.Document document = null;
            Model.AssetOp assetOp = null;
            Model.OrderOp orderOp = null;
            Model.OfferOp offerOp = null;
            Model.BudgetOp budgetOp = null;
            Model.Inventory inventory = null;
            Model.InventoryAsset inventoryAsset = null;
            DateTime? documentDate = null;
            // Model.Administration administration = null;
            Model.AccSystem accSystem = null;
            Model.AssetClass assetClass = null;
            // Model.AssetCategory assetCategory = null;
            Model.DictionaryItem dictionaryItem = null;
            Model.SubCategory subCategory = null;
            Model.AssetClassType assetClassType = null;
            Model.Partner partner = null;
            Model.CostCenter costCenter = null;
            // Model.Company company = null;
            // Model.Room room = null;
            // Model.AdmCenter admCenter = null;
            Model.AssetState assetState = null;
            // Model.AssetType assetType = null;
            Model.InvState invState = null;
            Model.Employee employee = null;
            // Model.Material material = null;
            Model.DocumentType documentType = null;
            // Model.Order order = null;
            // Model.OfferMaterial offerMaterial = null;
            Model.Stock stock = null;
            Model.Accountancy accountancy = null;
            Model.BudgetManager budgetManager = null;
            Model.CreateAssetSAP createAssetSAP = null;
            Model.TransferInStockSAP transferInStockSAP = null;
            Model.EntityType entityType = null;

            string accSystemDefault = "RON";
            string assetClassTypeDefault = "-";

            documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "PURCHASE").FirstOrDefaultAsync();

            inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).SingleAsync();
            invState = await _context.Set<Model.InvState>().Where(a => a.Code == "F").SingleAsync();
            assetState = await _context.Set<Model.AssetState>().Where(a => a.Code == "STOCK_IT_MFX").SingleAsync();
            assetClassType = await _context.Set<Model.AssetClassType>().Where(a => (a.Code == assetClassTypeDefault)).FirstOrDefaultAsync();
            assetClass = await _context.Set<Model.AssetClass>().Where(a => (a.Code == "")).FirstOrDefaultAsync();
            accSystem = await _context.Set<Model.AccSystem>().Where(a => (a.Code == accSystemDefault)).FirstOrDefaultAsync();
            budgetManager = await _context.Set<Model.BudgetManager>().Where(a => (a.Id == inventory.BudgetManagerId)).FirstOrDefaultAsync();

            employee = await _context.Set<Model.Employee>().Where(a => a.InternalCode == "VIRTUAL").SingleAsync();

            costCenter = await _context.Set<Model.CostCenter>()
                    .Include(c => c.Company)
                    .Include(c => c.Room)
                    .Include(c => c.AdmCenter)
                    .Include(c => c.Region)
                    .Include(c => c.Administration)
                    .Include(c => c.Division)
                        .ThenInclude(d => d.Department)
                    .Include(a => a.Storage)
                        .ThenInclude(p => p.Plant)
                    .Where(a => a.Code == "10RO700310").SingleAsync();


            int quantity = 1;

            for (int j = 0; j < quantity; j++)
            {
                entityType = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWASSET").FirstOrDefaultAsync();

                asset = new Model.Asset()
                {
                    Document = document
                };
                _context.Add(asset);

                stock = await _context.Set<Model.Stock>().Include(c => c.Company).Include(m => m.Material).Where(a => a.Id == assetDto.StockId).SingleAsync();

                if(assetDto.SubCategoryId == null || assetDto.SubCategoryId == 0)
				{
                    assetDto.SubCategoryId = 131;
                }

                subCategory = await _context.Set<Model.SubCategory>()
                    .Include(c => c.Category)
                    .Where(a => a.Id == assetDto.SubCategoryId).SingleAsync();



                if (stock.Value > 2500)
                {
                    accountancy = await _context.Set<Model.Accountancy>()

                      .Include(a => a.Account)
                      .Include(a => a.ExpAccount)
                      .Include(a => a.AssetCategory)
                      .Include(a => a.AssetType)
                      .Where(a => a.SubCategoryId == assetDto.SubCategoryId && a.Value > 2500 && a.IsDeleted == false).SingleOrDefaultAsync();

                    if (accountancy == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Lipsa mapare cont GL pentru subCategoria: {subCategory.Code + " - " + subCategory.Name}!" };
                }
                else
                {
                    accountancy = await _context.Set<Model.Accountancy>()

                     .Include(a => a.Account)
                     .Include(a => a.ExpAccount)
                     .Include(a => a.AssetCategory)
                     .Include(a => a.AssetType)
                     .Where(a => a.SubCategoryId == assetDto.SubCategoryId && a.Value == 2500 && a.IsDeleted == false).SingleOrDefaultAsync();

                    if (accountancy == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Lipsa mapare cont GL pentru subCategoria: {subCategory.Code + " - " + subCategory.Name}!" };
                }

                dictionaryItem = await _context.Set<Model.DictionaryItem>().Where(a => a.Name == subCategory.Name).FirstOrDefaultAsync();

                if (dictionaryItem == null)
                {
                    dictionaryItem = new Model.DictionaryItem()
                    {
                        Code = "NEW",
                        Name = subCategory.Name,
                        IsDeleted = false,
                        AssetCategoryId = accountancy.AssetCategoryId,
                        DictionaryTypeId = 1
                    };

                    _context.Add(dictionaryItem);
                }

                document = new Model.Document
                {
                    Approved = true,
                    DocumentTypeId = documentType != null ? documentType.Id : 1,
                    DocNo1 = stock.Invoice != null ? stock.Invoice : string.Empty,
                    DocNo2 = string.Empty,
                    DocumentDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    CostCenterId = costCenter != null ? costCenter.Id : (int?)null,
                    RegisterDate = DateTime.Now,
                    Details = string.Empty,
                    ParentDocumentId = inventory.DocumentId
                };


                partner = await _context.Set<Model.Partner>().Where(a => a.Id == stock.PartnerId).FirstOrDefaultAsync();

                document.Partner = partner;

                _context.Add(document);

                int? value = null;

                asset.InvNo = costCenter.Company.Code + "OPT0000000000" + entityType.Name;
                asset.Name = assetDto.Name;
                asset.PurchaseDate = DateTime.Now;
                asset.SerialNumber = assetDto.SerialNumber;
                asset.ERPCode = costCenter.Company.Code + "OPT0000000000" + entityType.Name;
                asset.Document = document;
                asset.Validated = true;
                asset.SAPCode = costCenter.Company.Code + "OPT0000000000" + entityType.Name;
                asset.AssetStateId = assetState.Id;
                asset.AssetTypeId = accountancy.AssetTypeId;
                asset.CostCenterId = costCenter.Id;

                costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == asset.CostCenterId).SingleOrDefault();

                asset.AssetCategoryId = accountancy.AssetCategoryId;
                asset.EmployeeId = employee.Id;
                asset.RoomId = costCenter.RoomId;
                asset.ValueInv = stock.Value;
                asset.ValueRem = stock.Value;
                asset.InvStateId = invState.Id;
                asset.Quantity = 1;
                asset.DepartmentId = costCenter.Division.DepartmentId;
                asset.Custody = false;
                //asset.UomId = assetDto.UomId;
                asset.CompanyId = costCenter.CompanyId;
                asset.Validated = true;
                asset.ArticleId = 151;
                asset.BudgetManager = budgetManager;
                //asset.AssetNatureId = assetDto.AssetNatureId;
                //asset.SubTypeId = assetDto.SubTypeId;
                //asset.InsuranceCategoryId = assetDto.InsuranceCategoryId;
                //asset.BrandId = assetDto.BrandId;
                //asset.ModelId = assetDto.ModelId;
                //asset.InterCompanyId = assetDto.InterCompanyId;
                //asset.ProjectId = order.ProjectId;
                //asset.InvoiceDate = assetDto.InvoiceDate;
                //asset.PODate = assetDto.PODate;
                //asset.ReceptionDate = assetDto.ReceptionDate;
                //asset.RemovalDate = assetDto.RemovalDate;
                asset.IsTemp = false;
                //asset.DimensionId = assetDto.DimensionId;
                asset.AdministrationId = costCenter.AdministrationId;
                asset.DictionaryItem = dictionaryItem;
                asset.IsAccepted = true;
                asset.IsReconcile = false;
                asset.AccountId = accountancy.AccountId;
                asset.OrderId = assetDto.OrderId;
                //asset.BudgetId = order.BudgetId;
                asset.DivisionId = costCenter.DivisionId;
                //asset.ProjectTypeId = assetDto.ProjectTypeId;
                // asset.AgreementNo = assetDto.PlateNo;
                asset.Name = subCategory.Name;
                asset.StockId = assetDto.StockId;
                asset.MaterialId = stock.MaterialId;
                asset.SubCategoryId = subCategory.Id;
                asset.CostCenterEmpId = employee != null ? employee.CostCenterId : null;
                asset.ExpAccountId = accountancy.ExpAccountId;

                assetAC = new Model.AssetAC
                {
                    AssetClassTypeId = assetClassType.Id,
                    Asset = asset,
                    AssetClassId = assetClass.Id,
                    AssetClassIdIn = assetClass.Id
                };


                _context.Set<Model.AssetAC>().Add(assetAC);

                var monthSum = 0;


                assetDep = new Model.AssetDep
                {
                    AccSystem = accSystem,
                    Asset = asset,
                    DepPeriod = (int)monthSum,
                    DepPeriodIn = (int)monthSum,
                    DepPeriodMonth = (int)monthSum,
                    DepPeriodMonthIn = 0,
                    DepPeriodRem = (int)monthSum,
                    DepPeriodRemIn = (int)monthSum,
                    UsageStartDate = documentDate,
                    ValueDep = stock.Value,
                    ValueDepIn = stock.Value,
                    ValueDepPU = stock.Value,
                    ValueDepPUIn = stock.Value,
                    ValueDepYTD = stock.Value,
                    ValueDepYTDIn = stock.Value,
                    ValueInv = stock.Value,
                    ValueInvIn = stock.Value,
                    ValueRem = stock.Value,
                    ValueRemIn = stock.Value
                };

                _context.Set<Model.AssetDep>().Add(assetDep);


                assetInv = new Model.AssetInv
                {
                    Asset = asset,
                    AllowLabel = true,
                    Barcode = entityType.Code + entityType.Name,
                    InvName = asset.Name,
                    InvNoOld = string.Empty,
                    InvStateId = invState.Id
                };

                _context.Set<Model.AssetInv>().Add(assetInv);


                accMonth = await _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).FirstOrDefaultAsync();

                assetDepMD = new Model.AssetDepMD
                {
                    AccMonthId = accMonth.Id,
                    AccSystem = accSystem,
                    Asset = asset,
                    UsefulLife = (int)monthSum,
                    TotLifeInpPeriods = (int)monthSum,
                    RemLifeInPeriods = 0,
                    AccumulDep = stock.Value,
                    BkValFYStart = stock.Value,
                    DepForYear = stock.Value,
                    CurrentAPC = stock.Value,
                    PosCap = stock.Value
                };

                _context.Set<Model.AssetDepMD>().Add(assetDepMD);


                assetAdmMD = new Model.AssetAdmMD
                {
                    AccMonthId = accMonth.Id,
                    Asset = asset,
                    DepartmentId = null
                };

                assetAdmMD.AssetStateId = assetState.Id;
                assetAdmMD.AssetTypeId = accountancy.AssetTypeId;
                assetAdmMD.CostCenterId = costCenter.Id;
                assetAdmMD.EmployeeId = employee.Id;
                assetAdmMD.RoomId = costCenter.RoomId;
                assetAdmMD.DepartmentId = costCenter.Division.DepartmentId;
                assetAdmMD.AssetCategoryId = accountancy.AssetCategoryId;
                assetAdmMD.AssetClass = assetClass;
                assetAdmMD.ArticleId = 151;
                assetAdmMD.AdministrationId = costCenter.AdministrationId;

                //assetAdmMD.AssetNatureId = assetDto.AssetNatureId;
                assetAdmMD.BudgetManager = budgetManager;
                //assetAdmMD.SubTypeId = assetDto.SubTypeId;
                //assetAdmMD.InsuranceCategoryId = assetDto.InsuranceCategoryId;
                //assetAdmMD.ModelId = assetDto.ModelId;
                //assetAdmMD.BrandId = assetDto.BrandId;
                //
                //assetAdmMD.ProjectId = order.ProjectId;

                assetAdmMD.AdmCenterId = costCenter.AdmCenterId;
                assetAdmMD.RegionId = costCenter.RegionId;
                assetAdmMD.DivisionId = costCenter.DivisionId;
                // assetAdmMD.ProjectTypeId = order.Budget.ProjectTypeId;
                assetAdmMD.SubCategoryId = subCategory.Id;


                _context.Set<Model.AssetAdmMD>().Add(assetAdmMD);

                entityType.Name = StringsADD(entityType.Name, "1");
                _context.Update(entityType);

                if (inventory != null)
                {
                    inventoryAsset = new Model.InventoryAsset
                    {
                        QInitial = 1,
                        QFinal = 0,
                        InventoryId = inventory.Id,
                        Asset = asset,
                        EmployeeIdInitial = employee.Id,
                        RoomIdInitial = costCenter.RoomId,
                        SerialNumber = assetDto.SerialNumber,
                        StateIdInitial = invState.Id,
                        CostCenterInitial = costCenter,
                        AdministrationIdInitial = costCenter.AdministrationId
                    };

                    _context.Add(inventoryAsset);

                    assetOp = new Model.AssetOp()
                    {
                        Asset = asset,
                        Document = document,
                        RoomIdInitial = costCenter.RoomId,
                        RoomIdFinal = costCenter.RoomId,
                        EmployeeIdInitial = employee.Id,
                        EmployeeIdFinal = employee.Id,

                        AssetCategoryIdInitial = accountancy.AssetCategoryId,
                        AssetCategoryIdFinal = accountancy.AssetCategoryId,
                        InvStateIdInitial = invState.Id,
                        InvStateIdFinal = invState.Id,
                        AdministrationIdInitial = costCenter.AdministrationId,
                        AdministrationIdFinal = costCenter.AdministrationId,
                        AccSystem = accSystem,
                        DocumentId = document.Id
                    };

                    assetOp.AssetStateIdInitial = assetState.Id;
                    assetOp.AssetStateIdFinal = assetState.Id;
                    assetOp.CostCenterInitial = costCenter;
                    assetOp.CostCenterFinal = costCenter;
                    assetOp.DepartmentIdInitial = null;
                    assetOp.DepartmentIdFinal = null;
                    assetOp.InvName = assetDto.Name;
                    assetOp.AssetTypeIdInitial = accountancy.AssetTypeId;
                    assetOp.AssetTypeIdFinal = accountancy.AssetTypeId;
                    _context.Add(assetOp);

                    //_context.Add(offerOp);

                    //var lastInventoryDoc = "";
                    //var lastInventoryDate = inventory.End.Value.ToString("yyyyMMdd");
                    var lastInventoryDate = DateTime.Now.ToString("yyyyMMdd");
                    //var committee = _context.Committees.Where(a => a.IsDeleted == false && a.CostCenterId == assetDto.CostCenterId).FirstOrDefault();

                    //if (committee != null)
                    //{
                    //    lastInventoryDoc = committee.Document1;
                    //}

                    var names = SplitToLines(stock.Material.Name, 50);
                    var countNames = names.Count();
                    var guid = Guid.NewGuid();

                    createAssetSAP = new Model.CreateAssetSAP();

                    createAssetSAP.XSUBNO = "";
                    createAssetSAP.COMPANYCODE = costCenter.Company.Code;
                    createAssetSAP.ASSET = asset.InvNo;
                    createAssetSAP.SUBNUMBER = "0000";
                    createAssetSAP.ASSETCLASS = accountancy.ExpAccount.Name;
                    createAssetSAP.POSTCAP = "";
                    createAssetSAP.DESCRIPT = countNames > 0 ? names.ElementAt(0) : "";
                    createAssetSAP.DESCRIPT2 = countNames > 1 ? names.ElementAt(1) : "";
                    createAssetSAP.INVENT_NO = asset.InvNo;
                    createAssetSAP.SERIAL_NO = assetDto.SerialNumber == null || assetDto.SerialNumber == "" ? "" : assetDto.SerialNumber;
                    createAssetSAP.QUANTITY = 0;
                    createAssetSAP.BASE_UOM = "ST";
                    createAssetSAP.LAST_INVENTORY_DATE = "00000000";
                    createAssetSAP.LAST_INVENTORY_DOCNO = "";
                    //createAssetSAP.CAP_DATE = assetDto.CapitalizationDate != null ? assetDto.CapitalizationDate.ToString("yyyyMMdd") : "00000000";
                    createAssetSAP.CAP_DATE = "00000000";
                    createAssetSAP.COSTCENTER = employee != null && employee.CostCenter != null ? employee.CostCenter.Code : costCenter.Code;
                    createAssetSAP.RESP_CCTR = costCenter.Code;
                    createAssetSAP.INTERN_ORD = "";
                    createAssetSAP.PLANT = stock.Plant;
                    createAssetSAP.LOCATION = "";
                    createAssetSAP.ROOM = "";
                    createAssetSAP.PERSON_NO = employee != null ? employee.InternalCode : "";
                    createAssetSAP.PLATE_NO = "";
                    createAssetSAP.ZZCLAS = accountancy.AssetCategory.Code;
                    createAssetSAP.IN_CONSERVATION = "";
                    createAssetSAP.PROP_IND = "1";
                    createAssetSAP.OPTIMA_ASSET_NO = asset.InvNo;
                    createAssetSAP.OPTIMA_ASSET_PARENT_NO = "";
                    createAssetSAP.TESTRUN = "";
                    createAssetSAP.VENDOR_NO = partner.RegistryNumber;
                    createAssetSAP.Asset = asset;
                    createAssetSAP.NotSync = true;
                    createAssetSAP.SyncErrorCount = 0;
                    createAssetSAP.BudgetManagerId = budgetManager.Id;
                    createAssetSAP.AccMonthId = inventory.AccMonthId.Value;
                    createAssetSAP.CreatedBy = _context.UserId;
                    createAssetSAP.ModifiedBy = _context.UserId;
                    createAssetSAP.Guid = guid;
                    createAssetSAP.FromStock = true;
                    createAssetSAP.INVOICE = stock.Invoice == null ? "" : stock.Invoice;
                    asset.NotSync = true;
                    asset.Name = countNames > 0 ? names.ElementAt(0) : "";

                    _context.Add(createAssetSAP);

                    transferInStockSAP = new Model.TransferInStockSAP()
                    {
                        Doc_Date = lastInventoryDate,
                        Pstng_Date = lastInventoryDate,
                        Material = stock.Material.Code,
                        Plant = stock.Plant,
                        Storage_Location = stock.Storage_Location,
                        Quantity = 1,
                        Uom = stock.UM,
                        Batch = stock.Code,
                        Gl_Account = accountancy.ExpAccount.Name,
                        Item_Text = stock.Name,
                        Asset = asset.InvNo,
                        SubNumber = asset.SubNo,
                        AssetStock = asset,
                        NotSync = true,
                        SyncErrorCount = 0,
                        BudgetManagerId = budgetManager.Id,
                        AccMonthId = inventory.AccMonthId.Value,
                        CreatedBy = _context.UserId,
                        ModifiedBy = _context.UserId,
                        Guid = guid,
                        CreateAssetSAP = createAssetSAP,
                        Ref_Doc_No = "MFX IT",
                        Header_Txt = "Transfer din stoc MFX in asset",
                        Storno = "",
                        Storno_Doc = "",
                        Storno_Year = "0000",
                        Storno_Date = "00000000",
                        Storno_User = ""
                    };

                    _context.Add(transferInStockSAP);
                }


                _context.SaveChanges();
            }

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = "OK" };
        }

        public async Task<Model.CreateAssetSAPResult> AddAssetStockCheck(AddStockAsset assetDto)
        {
            Model.Asset asset = null;
            Model.AssetDep assetDep = null;
            Model.AssetDepMD assetDepMD = null;
            Model.AssetInv assetInv = null;
            Model.AssetAC assetAC = null;
            Model.AssetAdmMD assetAdmMD = null;
            Model.Document document = null;
            Model.AssetOp assetOp = null;
            Model.OrderOp orderOp = null;
            Model.OfferOp offerOp = null;
            Model.BudgetOp budgetOp = null;
            Model.Inventory inventory = null;
            Model.InventoryAsset inventoryAsset = null;
            DateTime? documentDate = null;
            // Model.Administration administration = null;
            Model.AccSystem accSystem = null;
            Model.AssetClass assetClass = null;
            // Model.AssetCategory assetCategory = null;
            Model.DictionaryItem dictionaryItem = null;
            Model.SubCategory subCategory = null;
            Model.AssetClassType assetClassType = null;
            Model.Partner partner = null;
            Model.CostCenter costCenter = null;
            // Model.Company company = null;
            // Model.Room room = null;
            // Model.AdmCenter admCenter = null;
            Model.AssetState assetState = null;
            // Model.AssetType assetType = null;
            Model.InvState invState = null;
            Model.Employee employee = null;
            // Model.Material material = null;
            Model.DocumentType documentType = null;
            // Model.Order order = null;
            // Model.OfferMaterial offerMaterial = null;
            Model.Stock stock = null;
            Model.Accountancy accountancy = null;
            //Model.BudgetManager budgetManager = null;
            Model.CreateAssetSAP createAssetSAP = null;
            Model.TransferInStockSAP transferInStockSAP = null;
            Model.EntityType entityType = null;

            string accSystemDefault = "RON";
            string assetClassTypeDefault = "-";

            documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "PURCHASE").FirstOrDefaultAsync();
            if(documentType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Document-Type-Missing" };

            inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).FirstOrDefaultAsync();
            if (inventory == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Inventory-Missing" };

            invState = await _context.Set<Model.InvState>().Where(a => a.Code == "F").FirstOrDefaultAsync();
            if (invState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "State-Missing" };

            assetState = await _context.Set<Model.AssetState>().Where(a => a.Code == "STOCK_IT").FirstOrDefaultAsync();
            if (assetState == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Asset-State-Missing" };

            assetClassType = await _context.Set<Model.AssetClassType>().Where(a => (a.Code == assetClassTypeDefault)).FirstOrDefaultAsync();
            if (assetClassType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Asset-Class-Type-Missing" };

            assetClass = await _context.Set<Model.AssetClass>().Where(a => (a.Code == "")).FirstOrDefaultAsync();
            if (assetClass == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Asset-Class-Missing" };


            accSystem = await _context.Set<Model.AccSystem>().Where(a => (a.Code == accSystemDefault)).FirstOrDefaultAsync();
            if (accSystem == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Acc-System-Missing" };

            employee = await _context.Set<Model.Employee>().Where(a => a.InternalCode == "VIRTUAL").FirstOrDefaultAsync();
            if (employee == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Employee-Missing" };

            costCenter = await _context.Set<Model.CostCenter>()
                    .Include(c => c.Company)
                    .Include(c => c.Room)
                    .Include(c => c.AdmCenter)
                    .Include(c => c.Region)
                    .Include(c => c.Administration)
                    .Include(c => c.Division)
                        .ThenInclude(d => d.Department)
                    .Include(a => a.Storage)
                        .ThenInclude(p => p.Plant)
                    .Where(a => a.Code == "10RO700310").FirstOrDefaultAsync();
            if (costCenter == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Cost-Center-Missing" };

            int quantity = 1;

            for (int j = 0; j < quantity; j++)
            {
                entityType = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWASSETCHECK").FirstOrDefaultAsync();
                if (entityType == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Entity-Type-Missing" };

                stock = await _context.Set<Model.Stock>()
                    .Include(c => c.Company)
                    .Include(m => m.Material)
                    .Where(a => a.Id == assetDto.StockId)
                    .SingleAsync();


                subCategory = await _context.Set<Model.SubCategory>()
                    .Include(c => c.Category)
                    .Where(a => a.Id == assetDto.SubCategoryId)
                    .SingleAsync();

                if (stock.Value > 2500)
                {
                    accountancy = await _context.Set<Model.Accountancy>()

                      .Include(a => a.Account)
                      .Include(a => a.ExpAccount)
                      .Include(a => a.AssetCategory)
                      .Include(a => a.AssetType)
                      .Where(a => a.SubCategoryId == assetDto.SubCategoryId && a.Value > 2500 && a.IsDeleted == false)
                      .FirstOrDefaultAsync();

                    if (accountancy == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Lipsa mapare SubCategoria: {subCategory.Code + " - " + subCategory.Name}!" };
                }
                else
                {
                    accountancy = await _context.Set<Model.Accountancy>()

                     .Include(a => a.Account)
                     .Include(a => a.ExpAccount)
                     .Include(a => a.AssetCategory)
                     .Include(a => a.AssetType)
                     .Where(a => a.SubCategoryId == assetDto.SubCategoryId && a.Value == 2500 && a.IsDeleted == false)
                     .FirstOrDefaultAsync();

                    if (accountancy == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Lipsa mapare SubCategoria: {subCategory.Code + " - " + subCategory.Name}!" };
                }

                asset = new Model.Asset()
                {
                    Document = document
                };

                _context.Add(asset);

                dictionaryItem = await _context.Set<Model.DictionaryItem>().Where(a => a.Name == subCategory.Name).FirstOrDefaultAsync();

                if (dictionaryItem == null)
                {
                    dictionaryItem = new Model.DictionaryItem()
                    {
                        Code = "NEW",
                        Name = subCategory.Name,
                        IsDeleted = true,
                        AssetCategoryId = accountancy.AssetCategoryId,
                        DictionaryTypeId = 1
                    };

                    _context.Add(dictionaryItem);
                }

                document = new Model.Document
                {
                    Approved = true,
                    DocumentTypeId = documentType != null ? documentType.Id : 1,
                    DocNo1 = stock.Invoice != null ? stock.Invoice : string.Empty,
                    DocNo2 = string.Empty,
                    DocumentDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    CostCenterId = costCenter != null ? costCenter.Id : (int?)null,
                    RegisterDate = DateTime.Now,
                    Details = string.Empty,
                    ParentDocumentId = inventory.DocumentId,
                    IsDeleted = true
                };


                partner = await _context.Set<Model.Partner>().Where(a => a.Id == stock.PartnerId).FirstOrDefaultAsync();

                if (partner == null) return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "Partner-Missing" };

                document.Partner = partner;

                _context.Add(document);

                int? value = null;

                asset.InvNo = costCenter.Company.Code + "CHECK00000000" + entityType.Name;
                asset.Name = assetDto.Name;
                asset.PurchaseDate = DateTime.Now;
                asset.SerialNumber = assetDto.SerialNumber;
                asset.ERPCode = costCenter.Company.Code + "CHECK00000000" + entityType.Name;
                asset.Document = document;
                asset.Validated = true;
                asset.SAPCode = costCenter.Company.Code + "CHECK00000000" + entityType.Name;
                asset.AssetStateId = assetState.Id;
                asset.AssetStateId = 1;
                asset.AssetTypeId = accountancy.AssetTypeId;
                asset.CostCenterId = costCenter.Id;
                asset.AssetCategoryId = accountancy.AssetCategoryId;
                asset.EmployeeId = employee.Id;
                asset.RoomId = costCenter.RoomId;
                asset.ValueInv = stock.Value;
                asset.ValueRem = stock.Value;
                asset.InvStateId = invState.Id;
                asset.Quantity = 1;
                asset.DepartmentId = costCenter.Division.DepartmentId;
                asset.Custody = false;
                //asset.UomId = assetDto.UomId;
                asset.CompanyId = costCenter.CompanyId;
                asset.Validated = true;
                asset.ArticleId = 151;
                asset.BudgetManagerId = inventory.BudgetManagerId;
                //asset.AssetNatureId = assetDto.AssetNatureId;
                //asset.SubTypeId = assetDto.SubTypeId;
                //asset.InsuranceCategoryId = assetDto.InsuranceCategoryId;
                //asset.BrandId = assetDto.BrandId;
                //asset.ModelId = assetDto.ModelId;
                //asset.InterCompanyId = assetDto.InterCompanyId;
                //asset.ProjectId = order.ProjectId;
                //asset.InvoiceDate = assetDto.InvoiceDate;
                //asset.PODate = assetDto.PODate;
                //asset.ReceptionDate = assetDto.ReceptionDate;
                //asset.RemovalDate = assetDto.RemovalDate;
                asset.IsTemp = false;
                //asset.DimensionId = assetDto.DimensionId;
                asset.AdministrationId = costCenter.AdministrationId;
                asset.DictionaryItem = dictionaryItem;
                asset.IsAccepted = true;
                asset.IsReconcile = false;
                asset.AccountId = accountancy.AccountId;
                asset.OrderId = assetDto.OrderId;
                //asset.BudgetId = order.BudgetId;
                asset.DivisionId = costCenter.DivisionId;
                //asset.ProjectTypeId = assetDto.ProjectTypeId;
                // asset.AgreementNo = assetDto.PlateNo;
                asset.Name = subCategory.Name;
                asset.StockId = assetDto.StockId;
                asset.MaterialId = stock.MaterialId;
                asset.SubCategoryId = subCategory.Id;
                asset.CostCenterEmpId = employee != null ? employee.CostCenterId : null;
                asset.ExpAccountId = accountancy.ExpAccountId;
                //asset.InvoiceDate = DateTime.Now;
                asset.IsDeleted = true;

                assetAC = new Model.AssetAC
                {
                    AssetClassTypeId = assetClassType.Id,
                    Asset = asset,
                    AssetClassId = assetClass.Id,
                    AssetClassIdIn = assetClass.Id
                };


                _context.Set<Model.AssetAC>().Add(assetAC);

                var monthSum = 0;


                assetDep = new Model.AssetDep
                {
                    AccSystem = accSystem,
                    Asset = asset,
                    DepPeriod = (int)monthSum,
                    DepPeriodIn = (int)monthSum,
                    DepPeriodMonth = (int)monthSum,
                    DepPeriodMonthIn = 0,
                    DepPeriodRem = (int)monthSum,
                    DepPeriodRemIn = (int)monthSum,
                    UsageStartDate = documentDate,
                    ValueDep = stock.Value,
                    ValueDepIn = stock.Value,
                    ValueDepPU = stock.Value,
                    ValueDepPUIn = stock.Value,
                    ValueDepYTD = stock.Value,
                    ValueDepYTDIn = stock.Value,
                    ValueInv = stock.Value,
                    ValueInvIn = stock.Value,
                    ValueRem = stock.Value,
                    ValueRemIn = stock.Value
                };

                _context.Set<Model.AssetDep>().Add(assetDep);


                assetInv = new Model.AssetInv
                {
                    Asset = asset,
                    AllowLabel = true,
                    Barcode = entityType.Code + entityType.Name,
                    InvName = asset.Name,
                    InvNoOld = string.Empty,
                    InvStateId = invState.Id
                };

                _context.Set<Model.AssetInv>().Add(assetInv);

                assetDepMD = new Model.AssetDepMD
                {
                    AccMonthId = inventory.AccMonthId.Value,
                    AccSystem = accSystem,
                    Asset = asset,
                    UsefulLife = (int)monthSum,
                    TotLifeInpPeriods = (int)monthSum,
                    RemLifeInPeriods = 0,
                    AccumulDep = stock.Value,
                    BkValFYStart = stock.Value,
                    DepForYear = stock.Value,
                    CurrentAPC = stock.Value,
                    PosCap = stock.Value
                };

                _context.Set<Model.AssetDepMD>().Add(assetDepMD);


                assetAdmMD = new Model.AssetAdmMD
                {
                    AccMonthId = inventory.AccMonthId.Value,
                    Asset = asset,
                    DepartmentId = null
                };

                assetAdmMD.AssetStateId = assetState.Id;
                assetAdmMD.AssetTypeId = accountancy.AssetTypeId;
                assetAdmMD.CostCenterId = costCenter.Id;
                assetAdmMD.EmployeeId = employee.Id;
                assetAdmMD.RoomId = costCenter.RoomId;
                assetAdmMD.DepartmentId = costCenter.Division.DepartmentId;
                assetAdmMD.AssetCategoryId = accountancy.AssetCategoryId;
                assetAdmMD.AssetClass = assetClass;
                assetAdmMD.ArticleId = 151;
                assetAdmMD.AdministrationId = costCenter.AdministrationId;

                //assetAdmMD.AssetNatureId = assetDto.AssetNatureId;
                assetAdmMD.BudgetManagerId = inventory.BudgetManagerId;
                //assetAdmMD.SubTypeId = assetDto.SubTypeId;
                //assetAdmMD.InsuranceCategoryId = assetDto.InsuranceCategoryId;
                //assetAdmMD.ModelId = assetDto.ModelId;
                //assetAdmMD.BrandId = assetDto.BrandId;
                //
                //assetAdmMD.ProjectId = order.ProjectId;

                assetAdmMD.AdmCenterId = costCenter.AdmCenterId;
                assetAdmMD.RegionId = costCenter.RegionId;
                assetAdmMD.DivisionId = costCenter.DivisionId;
                // assetAdmMD.ProjectTypeId = order.Budget.ProjectTypeId;
                assetAdmMD.SubCategoryId = subCategory.Id;


                _context.Set<Model.AssetAdmMD>().Add(assetAdmMD);

                entityType.Name = StringsADD(entityType.Name, "1");
                _context.Update(entityType);

                inventoryAsset = new Model.InventoryAsset
                {
                    QInitial = 1,
                    QFinal = 0,
                    InventoryId = inventory.Id,
                    Asset = asset,
                    EmployeeIdInitial = employee.Id,
                    RoomIdInitial = costCenter.RoomId,
                    SerialNumber = assetDto.SerialNumber,
                    StateIdInitial = invState.Id,
                    CostCenterInitial = costCenter,
                    AdministrationIdInitial = costCenter.AdministrationId
                };

                _context.Add(inventoryAsset);

                assetOp = new Model.AssetOp()
                {
                    Asset = asset,
                    Document = document,
                    RoomIdInitial = costCenter.RoomId,
                    RoomIdFinal = costCenter.RoomId,
                    EmployeeIdInitial = employee.Id,
                    EmployeeIdFinal = employee.Id,

                    AssetCategoryIdInitial = accountancy.AssetCategoryId,
                    AssetCategoryIdFinal = accountancy.AssetCategoryId,
                    InvStateIdInitial = invState.Id,
                    InvStateIdFinal = invState.Id,
                    AdministrationIdInitial = costCenter.AdministrationId,
                    AdministrationIdFinal = costCenter.AdministrationId,
                    AccSystem = accSystem,
                    DocumentId = document.Id
                };

                assetOp.AssetStateIdInitial = assetState.Id;
                assetOp.AssetStateIdFinal = assetState.Id;
                assetOp.CostCenterInitial = costCenter;
                assetOp.CostCenterFinal = costCenter;
                assetOp.DepartmentIdInitial = null;
                assetOp.DepartmentIdFinal = null;
                assetOp.InvName = assetDto.Name;
                assetOp.AssetTypeIdInitial = accountancy.AssetTypeId;
                assetOp.AssetTypeIdFinal = accountancy.AssetTypeId;
                assetOp.IsDeleted = true;
                _context.Add(assetOp);

                //_context.Add(offerOp);

                //var lastInventoryDoc = "";
                //var lastInventoryDate = inventory.End.Value.ToString("yyyyMMdd");
                var lastInventoryDate = DateTime.Now.ToString("yyyyMMdd");
                //var committee = _context.Committees.Where(a => a.IsDeleted == false && a.CostCenterId == assetDto.CostCenterId).FirstOrDefault();

                //if (committee != null)
                //{
                //    lastInventoryDoc = committee.Document1;
                //}

                var names = SplitToLines(stock.Material.Name, 50);
                var countNames = names.Count();
                var guid = Guid.NewGuid();

                createAssetSAP = new Model.CreateAssetSAP();

                createAssetSAP.XSUBNO = "";
                createAssetSAP.COMPANYCODE = costCenter.Company.Code;
                createAssetSAP.ASSET = asset.InvNo;
                createAssetSAP.SUBNUMBER = "0000";
                createAssetSAP.ASSETCLASS = accountancy.ExpAccount.Name;
                createAssetSAP.POSTCAP = "";
                createAssetSAP.DESCRIPT = countNames > 0 ? names.ElementAt(0) : "";
                createAssetSAP.DESCRIPT2 = countNames > 1 ? names.ElementAt(1) : "";
                createAssetSAP.INVENT_NO = asset.InvNo;
                createAssetSAP.SERIAL_NO = assetDto.SerialNumber == null || assetDto.SerialNumber == "" ? "" : assetDto.SerialNumber;
                createAssetSAP.QUANTITY = 0;
                createAssetSAP.BASE_UOM = "ST";
                createAssetSAP.LAST_INVENTORY_DATE = "00000000";
                createAssetSAP.LAST_INVENTORY_DOCNO = "";
                //createAssetSAP.CAP_DATE = assetDto.CapitalizationDate != null ? assetDto.CapitalizationDate.ToString("yyyyMMdd") : "00000000";
                createAssetSAP.CAP_DATE = "00000000";
                createAssetSAP.COSTCENTER = employee != null && employee.CostCenter != null ? employee.CostCenter.Code : costCenter.Code;
                createAssetSAP.RESP_CCTR = costCenter.Code;
                createAssetSAP.INTERN_ORD = "";
                createAssetSAP.PLANT = stock.Plant;
                createAssetSAP.LOCATION = "";
                createAssetSAP.ROOM = "";
                createAssetSAP.PERSON_NO = employee != null ? employee.InternalCode : "";
                createAssetSAP.PLATE_NO = "";
                createAssetSAP.ZZCLAS = accountancy.AssetCategory.Code;
                createAssetSAP.IN_CONSERVATION = "";
                createAssetSAP.PROP_IND = "1";
                createAssetSAP.OPTIMA_ASSET_NO = asset.InvNo;
                createAssetSAP.OPTIMA_ASSET_PARENT_NO = "";
                createAssetSAP.TESTRUN = "";
                createAssetSAP.VENDOR_NO = partner.RegistryNumber;
                createAssetSAP.Asset = asset;
                createAssetSAP.NotSync = true;
                createAssetSAP.SyncErrorCount = 0;
                createAssetSAP.BudgetManagerId = inventory.BudgetManagerId.Value;
                createAssetSAP.AccMonthId = inventory.AccMonthId.Value;
                createAssetSAP.CreatedBy = _context.UserId;
                createAssetSAP.ModifiedBy = _context.UserId;
                createAssetSAP.Guid = guid;
                createAssetSAP.FromStock = true;
                createAssetSAP.INVOICE = stock.Invoice == null ? "" : stock.Invoice;
                createAssetSAP.IsDeleted = true;
                asset.NotSync = true;
                asset.Name = countNames > 0 ? names.ElementAt(0) : "";

                _context.Add(createAssetSAP);

                transferInStockSAP = new Model.TransferInStockSAP()
                {
                    Doc_Date = lastInventoryDate,
                    Pstng_Date = lastInventoryDate,
                    Material = stock.Material.Code,
                    Plant = stock.Plant,
                    Storage_Location = stock.Storage_Location,
                    Quantity = 1,
                    Uom = stock.UM,
                    Batch = stock.Code,
                    Gl_Account = accountancy.ExpAccount.Name,
                    Item_Text = stock.Name,
                    Asset = asset.InvNo,
                    SubNumber = asset.SubNo,
                    AssetStock = asset,
                    NotSync = true,
                    SyncErrorCount = 0,
                    BudgetManagerId = inventory.BudgetManagerId.Value,
                    AccMonthId = inventory.AccMonthId.Value,
                    CreatedBy = _context.UserId,
                    ModifiedBy = _context.UserId,
                    Guid = guid,
                    CreateAssetSAP = createAssetSAP,
                    IsDeleted = true,
                    Ref_Doc_No = "MFX IT",
                    Header_Txt = "Transfer din stoc MFX in asset",
                    Storno = "",
                    Storno_Doc = "",
                    Storno_Year ="0000",
                    Storno_Date = "00000000",
                    Storno_User = ""
                };

                _context.Add(transferInStockSAP);


                _context.SaveChanges();
            }

            return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = "OK" };
        }

        public static string StringsADD(string s1, string s2)
        {
            int l1 = s1.Count();
            int l2 = s2.Count();

            int[] l3 = { l1, l2 };
            int minlength = l3.Min();
            int maxlength = l3.Max();
            int komsu = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < maxlength; i++)
            {

                Int32 e1 = Convert.ToInt32(s1.PadLeft(maxlength, '0').ElementAt(maxlength - 1 - i).ToString());
                Int32 e2 = Convert.ToInt32(s2.PadLeft(maxlength, '0').ElementAt(maxlength - 1 - i).ToString());
                Int32 sum = e1 + e2 + komsu;
                if (sum >= 10)
                {
                    sb.Append(sum - 10);
                    komsu = 1;
                }
                else
                {
                    sb.Append(sum);
                    komsu = 0;
                }
                if (i == maxlength - 1 && komsu == 1)
                {
                    sb.Append("1");
                }

            }

            return new string(sb.ToString().Reverse().ToArray());
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

        public Model.Order GetDetailsById(int id, string includes)
        {
            IQueryable<Model.Order> query = null;
            query = GetOrderQuery(includes);

            return query.AsNoTracking().Where(a => a.Id == id)
                .Include(b => b.Offer)
                    .ThenInclude(b => b.Partner)
				.Include(b => b.Offer)
					.ThenInclude(b => b.Request)
					    .ThenInclude(b => b.Owner)
				.Include(b => b.BudgetBase)
                .Include(b => b.Company)
                .Include(b => b.Offer)
				    .ThenInclude(b => b.Request)
					    .ThenInclude(b => b.ProjectType)
			    .Include(b => b.Offer)
					.ThenInclude(b => b.Request)
						.ThenInclude(b => b.Project)
				.Include(b => b.Offer)
					.ThenInclude(b => b.Request)
						.ThenInclude(b => b.AssetType)
				.Include(b => b.Administration)
                //.Include(b => b.SubType)
                //    .ThenInclude(t => t.Type)
                //        .ThenInclude(m => m.MasterType)
                .Include(b => b.Employee)
                .Include(b => b.AccMonth)
                .Include(b => b.InterCompany)
                .Include(b => b.Partner)
                .Include(b => b.Account)
                .Include(b => b.CostCenter)
                .Include(b => b.Uom)
                .Include(b => b.OrderType)
                .Include(b => b.Contract)
				.Include(b => b.AppState)
				.Include(b => b.Division)
				.SingleOrDefault();
        }

        private IQueryable<Model.Order> GetOrderQuery(string includes)
        {
            IQueryable<Model.Order> query = null;
            query = _context.Orders.AsNoTracking();

            return query;
        }


        public int SendEmail(int budgetId, string userName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut)
        {
            Model.Order budget = null;
            Model.EmailType emailTypeAsset = null;
            var emailIni = "";
            var emailCC = "";
            var htmlBodyEmail = "";
            var htmlBodyEnd = @"</tbody>
                                                </table>
                                            </body>
                                        </html> ";
            var htmlBody = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    New Order
                                                </title>
                                                <style type=""text/css"">
                                                    <style type=""text/css"">
                                                    HTML{background-color: #e8e8e8;}
                                                    .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
                                                    .courses-table .description {color: #fefefe !important;}
                                                    .courses-table .description a{color: #ffffff !important; font: bold 11px Arial;
                                                      text-decoration: none;
                                                      background-color: #fe0000;
                                                      padding: 2px 6px 2px 6px;
                                                      border-top: 1px solid #CCCCCC;
                                                      border-right: 1px solid #333333;
                                                      border-bottom: 1px solid #333333;
                                                      border-left: 1px solid #CCCCCC;}
                                                    .courses-table td{border: 1px solid #ffffff; background-color: #000000; text-align: center; padding: 8px;}
                                                    .courses-table th{border: 1px solid #ffffff; color: #030804;text-align: center; padding: 8px;}
                                                    .red{background-color: #FFDD04;}
                                                    .header{background-color: #fe0000; color: #000000 !important;}
                                                    .msg{background-color: #000000; color: #fefefe !important;}
                                                    .table-header{background-color: #000000; color: #fefefe !important;}
                                                    .table-body{background-color: #000000; color: #fefefe !important;}
                                                    .green{background-color: #6B9852;}
                                                   
                                                </style>
                                                </style>
                                            </head>
                                            <body>
                                                <h4>Order details:</h4>
                                                <table class=""courses-table"">
                                                    <thead>
                                                        <tr>
                                                            <th class=""red"">Company</th>
                                                            <th class=""red"">Project ID</th>
                                                            <th class=""red"">Project</th>
                                                            <th class=""red"">Supplier</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                        ";
            var subject = "New order validation";

            if (budgetId > 0)
            {

                var user = _context.Users.Where(u => u.UserName == userName).Single();

                budget = _context.Set<Model.Order>()
                    .Include(b => b.BudgetBase)
                        .ThenInclude(e => e.Company)
                    .Include(b => b.BudgetBase)
                        .ThenInclude(e => e.Project)
                    .Include(b => b.BudgetBase)
                        .ThenInclude(e => e.Department)
                    .Include(b => b.BudgetBase)
                        .ThenInclude(e => e.Region)
                    .Include(b => b.Offer)
                        .ThenInclude(e => e.SubType)
                            .ThenInclude(e => e.Type)
                                .ThenInclude(e => e.MasterType)
                    .Include(b => b.Offer)
                            .ThenInclude(d => d.Employee)
                    .Include(b => b.Offer)
                        .ThenInclude(e => e.Partner)
                    .Include(b => b.BudgetBase)
                        .ThenInclude(e => e.AccMonth)
                    .Include(b => b.BudgetBase)
                        .ThenInclude(e => e.BudgetManager)
                    .Where(a => a.Id == budgetId).Single();

                if (budget.Employee != null && budget.Employee.Email != "" && budget.Employee.Email != null)
                {
                    emailIni = budget.Employee.Email;

                    if (budget.Employee.Department != null && budget.Employee.Department.Name != null && budget.Employee.Department.Name != "")
                    {
                        emailCC = budget.Employee.Department.Name;
                    }
                    else
                    {
                        emailCC = "adrian.cirnaru@optima.ro";
                    }
                }
                else
                {
                    emailIni = "adrian.cirnaru@optima.ro";
                }

                //htmlBodyEmail = htmlBodyEmail + @"
                //                                        <tr>
                //                                            <td class=""description"">" + budget.Company.Name + @" </ td >
                //                                            <td class=""description"">" + budget.Project.Code + @" </ td >
                //                                            <td class=""description"">" + budget.Project.Name + @" </ td >
                //                                            <td class=""description"">" + budget.Partner.Name + @" </td >
                //                                        </tr>
                //                                        <tfoot>
                //                                            <br>
                //                                                <thead>
                //                                                <tr>
                //                                                    <th class=""red"">Owner</th>
                //                                                    <th class=""red"">Description</th>
                //                                                    <th class=""red"">Quantiy</th>
                //                                                    <th class=""red"">Price</th>
                //                                                    <th class=""red"">Value</th>
                //                                                </tr>
                //                                            </thead>
                //                                            <tr>
                //                                             <td class=""description"">" + budget.Offer.Employee.FirstName + " " + budget.Offer.Employee.LastName + @" </td >
                //                                             <td class=""description"">" + budget.Name + @" </ td >
                //                                             <td class=""description"">" + budget.Quantity + @" </ td >
                //                                             <td class=""description"">" + budget.Price + @" </ td >
                //                                             <td class=""description"">" + budget.ValueIni + @" </ td >
                //                                            </tr>
                //                                        </tfoot>
                //                        ";
            }


            //emailTypeAsset = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE NEW BUDGET").Single();
            //headerMsg = emailTypeAsset.HeaderMsg;
            //footerMsg = emailTypeAsset.FooterMsg;
             var budgetLink = "https://service.inventare.ro/Emag/#/budgetvalidate/" + budget.Guid.ToString();
            //var budgetLink = "http://localhost:3100/#/budgetvalidate/" + budget.Guid.ToString();
            var link = @"<h4><span style=""font-family: Roboto,Montserrat,helvetica neue,Helvetica,Arial,sans-serif;font-size: 12px;color: rgb(115, 115, 115)"">To review and validate new order, please access the following link: <a style=""color: red; font-size: 12px;"" href = '" + budgetLink + "'" + "' >  VALIDATE ORDER: " + budget.Code + "</a>" + @"</span></h4>";
            var linkInfo = @"<h4><span style=""font-family: Roboto,Montserrat,helvetica neue,Helvetica,Arial,sans-serif;font-size: 12px;color: rgb(115, 115, 115)"">(If the link cannot be accessed with IE, please try using Chrome)" + @"</span></h4>";

            emailIniOut = emailIni;
            emailCCOut = emailCC;
            // bodyHtmlOut = htmlBody + htmlBodyEmail + htmlBodyEnd;
            bodyHtmlOut = htmlBody + htmlBodyEmail + htmlBodyEnd + link + linkInfo;
            subjectOut = subject;

            _context.SaveChanges();

            return budgetId;


        }

        public int SendValidatedEmail(int orderId, string userName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut)
        {
            Model.Order budget = null;
            Model.EmailType emailTypeAsset = null;
            var emailIni = "";
            var emailCC = "";
            var htmlBodyEmail = "";
            var htmlBodyEnd = @"</tbody>
                                                </table>
                                            </body>
                                        </html> ";
            var htmlBody = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    Order Validate Level 1
                                                </title>
                                                <style type=""text/css"">
                                                    <style type=""text/css"">
                                                    HTML{background-color: #e8e8e8;}
                                                    .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
                                                    .courses-table .description {color: #fefefe !important;}
                                                    .courses-table .description a{color: #ffffff !important; font: bold 11px Arial;
                                                      text-decoration: none;
                                                      background-color: #fe0000;
                                                      padding: 2px 6px 2px 6px;
                                                      border-top: 1px solid #CCCCCC;
                                                      border-right: 1px solid #333333;
                                                      border-bottom: 1px solid #333333;
                                                      border-left: 1px solid #CCCCCC;}
                                                    .courses-table td{border: 1px solid #ffffff; background-color: #000000; text-align: center; padding: 8px;}
                                                    .courses-table th{border: 1px solid #ffffff; color: #030804;text-align: center; padding: 8px;}
                                                    .red{background-color: #FFDD04;}
                                                    .header{background-color: #fe0000; color: #000000 !important;}
                                                    .msg{background-color: #000000; color: #fefefe !important;}
                                                    .table-header{background-color: #000000; color: #fefefe !important;}
                                                    .table-body{background-color: #000000; color: #fefefe !important;}
                                                    .green{background-color: #6B9852;}
                                                   
                                                </style>
                                                </style>
                                            </head>
                                            <body>
                                                <h4>Order details:</h4>
                                                <table class=""courses-table"">
                                                    <thead>
                                                        <tr>
                                                            <th class=""red"">Company</th>
                                                            <th class=""red"">Project ID</th>
                                                            <th class=""red"">Project</th>
                                                            <th class=""red"">Activity</th>
                                                            <th class=""red"">CC</th>
                                                            <th class=""red"">PC</th>
                                                            <th class=""red"">Expence Type</th>
                                                            <th class=""red"">Details</th>
                                                            <th class=""red"">Supplier</th>
                                                            <th class=""red"">Account</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                        ";
            var subject = "New order to validate";

            if (orderId > 0)
            {

                var user = _context.Users.Where(u => u.UserName == userName).Single();

                budget = _context.Set<Model.Order>()
                    .Include(b => b.BudgetBase)
                        .ThenInclude(e => e.Company)
                    .Include(b => b.BudgetBase)
                        .ThenInclude(e => e.Project)
                    .Include(b => b.BudgetBase)
                        .ThenInclude(e => e.CostCenter)
                    .Include(b => b.Offer)
                        .ThenInclude(e => e.SubType)
                            .ThenInclude(e => e.Type)
                                .ThenInclude(e => e.MasterType)
                    .Include(b => b.BudgetBase)
                        .ThenInclude(e => e.Employee)
                            .ThenInclude(d => d.Department)
                    .Include(b => b.Offer)
                        .ThenInclude(e => e.Partner)
                    .Include(b => b.BudgetBase)
                        .ThenInclude(e => e.AccMonth)
                    .Where(a => a.Id == orderId).Single();

                if (budget.Employee != null && budget.Employee.Email != "" && budget.Employee.Email != null)
                {
                    emailIni = budget.Employee.Email;

                    if (budget.Employee.Department != null && budget.Employee.Department.Name != null && budget.Employee.Department.Name != "")
                    {
                        emailCC = budget.Employee.Department.Name;
                    }
                    else
                    {
                        emailCC = "adrian.cirnaru@optima.ro";
                    }
                }
                else
                {
                    emailIni = "adrian.cirnaru@optima.ro";
                }

                //htmlBodyEmail = htmlBodyEmail + @"
                //                                        <tr>
                //                                            <td class=""description"">" + budget.Company.Name + @" </ td >
                //                                            <td class=""description"">" + budget.Project.Code + @" </ td >
                //                                            <td class=""description"">" + budget.Project.Name + @" </ td >
                //                                            <td class=""description"">" + budget.Partner.Name + @" </td >
                //                                        </tr>
                //                                        <tfoot>
                //                                            <br>
                //                                                <thead>
                //                                                <tr>
                //                                                    <th class=""red"">Owner</th>
                //                                                    <th class=""red"">Description</th>
                //                                                    <th class=""red"">Quantiy</th>
                //                                                    <th class=""red"">Price</th>
                //                                                    <th class=""red"">Value</th>
                //                                                </tr>
                //                                            </thead>
                //                                            <tr>
                //                                             <td class=""description"">" + budget.Employee.FirstName + " " + budget.Employee.LastName + @" </td >
                //                                             <td class=""description"">" + budget.Name + @" </ td >
                //                                             <td class=""description"">" + budget.Quantity + @" </ td >
                //                                             <td class=""description"">" + budget.Price + @" </ td >
                //                                             <td class=""description"">" + budget.ValueIni + @" </ td >
                //                                            </tr>
                //                                        </tfoot>
                //                        ";
            }


            //emailTypeAsset = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE NEW BUDGET").Single();
            //headerMsg = emailTypeAsset.HeaderMsg;
            //footerMsg = emailTypeAsset.FooterMsg;
            var budgetLink = "https://service.inventare.ro/Emag/#/ordervalidate/" + budget.Guid.ToString();
            //var budgetLink = "http://localhost:3100/#/ordervalidate/" + budget.Guid.ToString();
            var link = @"<h4><span style=""font-family: Roboto,Montserrat,helvetica neue,Helvetica,Arial,sans-serif;font-size: 12px;color: rgb(115, 115, 115)"">To review and validate new order, please access the following link: <a style=""color: red; font-size: 12px;"" href = '" + budgetLink + "'" + "' >  VALIDATE ORDER: " + budget.Code + "</a>" + @"</span></h4>";
            var linkInfo = @"<h4><span style=""font-family: Roboto,Montserrat,helvetica neue,Helvetica,Arial,sans-serif;font-size: 12px;color: rgb(115, 115, 115)"">(If the link cannot be accessed with IE, please try using Chrome)" + @"</span></h4>";

            emailIniOut = emailIni;
            emailCCOut = emailCC;
            // bodyHtmlOut = htmlBody + htmlBodyEmail + htmlBodyEnd;
            bodyHtmlOut = htmlBody + htmlBodyEmail + htmlBodyEnd + link + linkInfo;
            subjectOut = subject;

            _context.SaveChanges();

            return orderId;


        }

        public IEnumerable<Model.OrderDetail> BudgetValidate(OrderFilter budgetFilter, string includes, string userId, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Order> budgetQuery = null;
            IQueryable<OrderDetail> query = null;

            budgetQuery = _context.Orders.AsNoTracking().AsQueryable();

            if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Name.Contains(budgetFilter.Filter)));


            includes = includes ?? string.Empty;

            foreach (var includeProperty in includes.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int dotIndex = includeProperty.IndexOf(".");
                string prefix = string.Empty;
                string property = string.Empty;

                if (dotIndex > 0)
                {
                    prefix = includeProperty.Substring(0, dotIndex);
                    property = includeProperty.Length > dotIndex ? includeProperty.Substring(dotIndex + 1) : string.Empty;
                }
                else
                {
                    prefix = includeProperty;
                }


                switch (prefix)
                {
                    case "Order":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new OrderDetail { Order = budget });

            if (userId != "" && userId != null)
            {
                query = query.Where(a => a.Order.Guid.ToString() == userId);
            }
            else
            {
                query = query.Where(a => a.Order.Guid.ToString() == "1234");
            }

            query = query.Where(a => a.Order.IsDeleted == false && a.Order.Validated == true);



            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();
            //catTotal.AssetCategoryDeskPhone = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 42 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryMonitor = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 72 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryThinClient = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 1035 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.OrderDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.OrderDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

		public async Task<Model.OrderResult> OrderEdit(OrderEdit orderDto)
		{
			Model.Order order = null;
			//Model.RequestOp requestOp = null;
			//Model.Document document = null;
			//Model.DocumentType documentType = null;
			//Model.AppState appState = null;
			Model.ApplicationUser applicationUser = null;

			order = await _context.Set<Model.Order>().Where(c => c.Id == orderDto.Id).FirstOrDefaultAsync();
			if (order == null) return new Model.OrderResult { Success = false, Message = "Nu exista PO!", OrderId = 0 };

			//appState = await _context.Set<Model.AppState>().Where(c => c.Code == "NEW_REQUEST").FirstOrDefaultAsync();
			//if (appState == null) return new Model.RequestResult { Success = false, Message = "Nu exista stare!", RequestId = 0 };

			applicationUser = await _context.Set<Model.ApplicationUser>().Where(a => a.Id == orderDto.UserId).FirstOrDefaultAsync();
			if (applicationUser == null) return new Model.OrderResult { Success = false, Message = "Nu exista user!", OrderId = 0 };

			//documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_REQUEST").FirstOrDefaultAsync();
			//if (documentType == null) return new Model.RequestResult { Success = false, Message = "Nu exista tip de document!", RequestId = 0 };

			//document = new Model.Document()
			//{
			//	Approved = true,
			//	CreatedAt = DateTime.Now,
			//	CreatedBy = applicationUser.Id,
			//	CreationDate = DateTime.Now,
			//	Details = requestDto.Info != null ? requestDto.Info : string.Empty,
			//	DocNo1 = requestDto.Info != null ? requestDto.Info : string.Empty,
			//	DocNo2 = requestDto.Info != null ? requestDto.Info : string.Empty,
			//	DocumentDate = DateTime.Now,
			//	DocumentTypeId = documentType.Id,
			//	Exported = true,
			//	IsDeleted = false,
			//	ModifiedAt = DateTime.Now,
			//	ModifiedBy = applicationUser.Id,
			//	ParentDocumentId = inventory.DocumentId,
			//	PartnerId = null,
			//	RegisterDate = DateTime.Now,
			//	ValidationDate = DateTime.Now
			//};

			//_context.Add(document);

			order.Info = orderDto.Info;

			_context.Update(order);

			//requestOp = new Model.RequestOp()
			//{
			//	AccMonthId = inventory.AccMonthId,
			//	AccSystemId = null,
			//	Request = request,
			//	BudgetManagerId = inventory.BudgetManagerId,
			//	RequestStateId = appState.Id,
			//	CreatedAt = DateTime.Now,
			//	CreatedBy = applicationUser.Id,
			//	Document = document,
			//	DstConfAt = DateTime.Now,
			//	DstConfBy = applicationUser.Id,
			//	InfoIni = requestDto.Info ?? string.Empty,
			//	InfoFin = requestDto.Info ?? string.Empty,
			//	IsAccepted = false,
			//	IsDeleted = false,
			//	ModifiedAt = DateTime.Now,
			//	ModifiedBy = applicationUser.Id,
			//	Validated = true,
			//	Guid = Guid.NewGuid(),
			//	CompanyId = company.Id,
			//};

			//_context.Add(requestOp);

			_context.SaveChanges();


			return new Model.OrderResult { Success = true, Message = "P.R.- ul a fost actualizat cu succes!", OrderId = order.Id };
		}

		public async Task<Model.OrderResult> OrderBudgetForecastUpdate(OrderBudgetForecastUpdate orderDto)
		{
			Model.Order order = null;
            Model.BudgetForecast budgetForecast = null;
			//Model.RequestBudgetForecast requestBudgetForecast = null;
			//List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
			List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
			List<Model.RequestBudgetForecast> requestBFs = null;
			List<string> materials = null;
			Model.Contract contract = null;
			//Model.RequestOp requestOp = null;
			//Model.Document document = null;
			//Model.DocumentType documentType = null;
			//Model.AppState appState = null;
			Model.ApplicationUser applicationUser = null;
			int sumQuantity = 0;
			int sumTotalOrderQuantity = 0;
            bool orderNeedsBudget = false;

            order = await _context.Set<Model.Order>().Include(a => a.Offer).Where(c => c.Id == orderDto.OrderId).FirstOrDefaultAsync();
			if (order == null) return new Model.OrderResult { Success = false, Message = "Nu exista PO!", OrderId = 0 };

			budgetForecast = await _context.Set<Model.BudgetForecast>().Where(c => c.Id == orderDto.BudgetForecastId).FirstOrDefaultAsync();
			if (budgetForecast == null) return new Model.OrderResult { Success = false, Message = "Nu exista BUGET!", OrderId = 0 };

			applicationUser = await _context.Set<Model.ApplicationUser>().Where(a => a.Id == orderDto.UserId).FirstOrDefaultAsync();
			if (applicationUser == null) return new Model.OrderResult { Success = false, Message = "Nu exista user!", OrderId = 0 };


			requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>().Include(a => a.OfferType).Where(a => a.RequestId == order.Offer.RequestId).ToListAsync();
			if (requestBudgetForecasts.Count == 0) return new Model.OrderResult { Success = false, Message = "Nu exista BUGETE!", OrderId = 0 };

			for (int i = 0; i < requestBudgetForecasts.Count; i++)
            {
                requestBudgetForecasts[i].BudgetForecastId = budgetForecast.Id;
				materials = await _context.Set<Model.RequestBudgetForecastMaterial>()
				   .Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecasts[i].Id)
				   .Select(a => a.Material.Code)
				   .ToListAsync();

				//requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
    //                .Include(a => a.OfferType)
    //                .Where(a => a.RequestBudgetForecastId == requestBudgetForecasts[i].Id)
    //                .ToListAsync();


				var mat = string.Join(", ", materials);

				requestBudgetForecasts[i].Materials = mat;
				_context.Update(requestBudgetForecasts[i]);
                _context.SaveChanges();


                // UPDATE VALUES //

               

                if (requestBudgetForecasts[i].OfferType.Code == "O-V")
				{
					sumQuantity = 1;
				}
				else
				{
					sumQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecasts[i].Id).Sum(a => a.Quantity);
				}



				decimal sumValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecasts[i].Id).Sum(a => a.Value);
				decimal sumValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecasts[i].Id).Sum(a => a.ValueRon);

				requestBudgetForecasts[i].Quantity = sumQuantity;
				requestBudgetForecasts[i].Value = sumValue;
				requestBudgetForecasts[i].ValueRon = sumValueRon;

				_context.Update(requestBudgetForecasts[i]);
				_context.SaveChanges();

				requestBFs = await _context.Set<Model.RequestBudgetForecast>().Include(b => b.BudgetForecast).Where(a => a.Guid == requestBudgetForecasts[i].Guid).ToListAsync();

				if (requestBudgetForecasts[i].OfferType.Code == "O-V")
				{
					sumTotalOrderQuantity = 1;
				}
				else
				{
					sumTotalOrderQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecasts[i].Guid).Sum(a => a.Quantity);
				}

				decimal sumTotalOrderValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecasts[i].Guid).Sum(a => a.Value);
				decimal sumTotalOrderValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecasts[i].Guid).Sum(a => a.ValueRon);

				for (int o = 0; o < requestBFs.Count; o++)
				{
					requestBFs[o].NeedBudget = false;
					requestBFs[o].NeedBudgetValue = 0;
					requestBFs[o].TotalOrderQuantity = sumTotalOrderQuantity;
					requestBFs[o].TotalOrderValue = sumTotalOrderValue;
					requestBFs[o].TotalOrderValueRon = sumTotalOrderValueRon;

					requestBFs[o].NeedContract = false;
					requestBFs[o].NeedContractValue = 0;

					contract = await _context.Set<Model.Contract>().Include(c => c.ContractAmount).Where(u => u.Id == requestBFs[o].ContractId).SingleAsync();

					if (contract.ContractAmount.AmountRonRem < requestBFs[0].TotalOrderValueRon)
					{
						requestBFs[o].NeedContract = true;
						requestBFs[o].NeedContractValue = requestBFs[o].TotalOrderValueRon - contract.ContractAmount.AmountRonRem;
					}

					if (requestBFs[o].BudgetForecast.TotalRem < requestBFs[o].ValueRon)
					{
						requestBFs[o].NeedBudget = true;
                        orderNeedsBudget = true;
						requestBFs[o].NeedBudgetValue = requestBFs[o].ValueRon - requestBFs[o].BudgetForecast.TotalRem;
					}

					_context.Update(requestBFs[o]);
					_context.SaveChanges();
				}
			}

            Model.AppState needBudgetAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "NEED_BUDGET").FirstOrDefaultAsync();

            if (!orderNeedsBudget && order.AppState.Id == needBudgetAppState.Id)
            {
                Model.AppState appstate = await _context.Set<Model.AppState>().Where(a => a.Code == "ORDER_LEVELB1").FirstOrDefaultAsync();
                if (appstate != null)
                {
                    order.AppStateId = appstate.Id;
                    _context.Update(order);
                    _context.SaveChanges();
                }
            }

			//documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_REQUEST").FirstOrDefaultAsync();
			//if (documentType == null) return new Model.RequestResult { Success = false, Message = "Nu exista tip de document!", RequestId = 0 };

			//document = new Model.Document()
			//{
			//	Approved = true,
			//	CreatedAt = DateTime.Now,
			//	CreatedBy = applicationUser.Id,
			//	CreationDate = DateTime.Now,
			//	Details = requestDto.Info != null ? requestDto.Info : string.Empty,
			//	DocNo1 = requestDto.Info != null ? requestDto.Info : string.Empty,
			//	DocNo2 = requestDto.Info != null ? requestDto.Info : string.Empty,
			//	DocumentDate = DateTime.Now,
			//	DocumentTypeId = documentType.Id,
			//	Exported = true,
			//	IsDeleted = false,
			//	ModifiedAt = DateTime.Now,
			//	ModifiedBy = applicationUser.Id,
			//	ParentDocumentId = inventory.DocumentId,
			//	PartnerId = null,
			//	RegisterDate = DateTime.Now,
			//	ValidationDate = DateTime.Now
			//};

			//_context.Add(document);

			//_context.Update(order);

			//requestOp = new Model.RequestOp()
			//{
			//	AccMonthId = inventory.AccMonthId,
			//	AccSystemId = null,
			//	Request = request,
			//	BudgetManagerId = inventory.BudgetManagerId,
			//	RequestStateId = appState.Id,
			//	CreatedAt = DateTime.Now,
			//	CreatedBy = applicationUser.Id,
			//	Document = document,
			//	DstConfAt = DateTime.Now,
			//	DstConfBy = applicationUser.Id,
			//	InfoIni = requestDto.Info ?? string.Empty,
			//	InfoFin = requestDto.Info ?? string.Empty,
			//	IsAccepted = false,
			//	IsDeleted = false,
			//	ModifiedAt = DateTime.Now,
			//	ModifiedBy = applicationUser.Id,
			//	Validated = true,
			//	Guid = Guid.NewGuid(),
			//	CompanyId = company.Id,
			//};

			//_context.Add(requestOp);

			//_context.SaveChanges();
			return new Model.OrderResult { Success = true, Message = "P.R.- ul a fost actualizat cu succes!", OrderId = order.Id };
		}

        public async Task UpdateAllBudgetBaseAsync()
        {
            var result = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
            return;
        }


        public async Task UpdateAllBudgetBasesAsync()
        {
            var result = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();
            return;
        }

        private int AddOrderExportHeader(ExcelWorksheet sheet)
        {
            int rIndex = 1;
            int cIndex = 0;
            sheet.Cells[rIndex, ++cIndex].Value = "Nr. crt";
            sheet.Cells[rIndex, ++cIndex].Value = "Cod PO";
            sheet.Cells[rIndex, ++cIndex].Value = "Asset";
            sheet.Cells[rIndex, ++cIndex].Value = "Tip P.O.";
            sheet.Cells[rIndex, ++cIndex].Value = "Numar factura";
            sheet.Cells[rIndex, ++cIndex].Value = "Vendor";
            sheet.Cells[rIndex, ++cIndex].Value = "Stare P.O.";
            sheet.Cells[rIndex, ++cIndex].Value = "L4";
            sheet.Cells[rIndex, ++cIndex].Value = "L3";
            sheet.Cells[rIndex, ++cIndex].Value = "L2";
            sheet.Cells[rIndex, ++cIndex].Value = "L1";
            sheet.Cells[rIndex, ++cIndex].Value = "S3";
            sheet.Cells[rIndex, ++cIndex].Value = "S2";
            sheet.Cells[rIndex, ++cIndex].Value = "S1";
            sheet.Cells[rIndex, ++cIndex].Value = "Data creare";
            sheet.Cells[rIndex, ++cIndex].Value = "Data modificare";
            sheet.Cells[rIndex, ++cIndex].Value = "Owner";
            sheet.Cells[rIndex, ++cIndex].Value = "Cod oferta";
            sheet.Cells[rIndex, ++cIndex].Value = "Cod companie";
            return cIndex;
        }

        private static void ApplyFileFormat(int columnCount, ExcelWorksheet sheet)
        {
            for (int i = 1; i <= columnCount; i++)
            {
                sheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(132, 178, 249));
                sheet.Column(i).AutoFit();
            }

            sheet.Row(1).Height = 45.00;
            sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.View.FreezePanes(2, 1);

            using (var cells = sheet.Cells[1, 1, 1, columnCount])
            {
                cells.Style.Font.Bold = true;
                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(132, 178, 249));
                cells.Style.Font.Color.SetColor(Color.Black);
            }
        }

        private static void AddOrderInList(int rIndex, ExcelWorksheet sheet, OrderReport orderReport)
        {

            int cIndex = 0;
            sheet.Cells[rIndex, ++cIndex].Value = (rIndex - 1).ToString();
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.OrderCode != null ? orderReport.OrderCode : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.InvNo != null ? orderReport.InvNo : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.POTypeName != null ? orderReport.POTypeName : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.InvoiceNumber != null ? orderReport.InvoiceNumber : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.Supplier != null ? orderReport.Supplier : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.AssetStateName != null ? orderReport.AssetStateName : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.L4 != null ? orderReport.L4 : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.L3 != null ? orderReport.L3 : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.L2 != null ? orderReport.L2 : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.L1 != null ? orderReport.L1 : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.S3 != null ? orderReport.S3 : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.S2 != null ? orderReport.S2 : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.S1 != null ? orderReport.S1 : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.CreatedAt.ToString("dd/MM/yyyy") != null ? orderReport.CreatedAt.ToString("dd/MM/yyyy") : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.ModifiedAt.ToString("dd/MM/yyyy") != null ? orderReport.ModifiedAt.ToString("dd/MM/yyyy") : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.OwnerEmail != null ? orderReport.OwnerEmail : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.OfferCode != null ? orderReport.OfferCode : string.Empty;
            sheet.Cells[rIndex, ++cIndex].Value = orderReport.CompanyCode != null ? orderReport.CompanyCode : string.Empty;
        }

        public async Task<byte[]> ExportOrderStatusAsync(int? orderId, int? documentId, int? assetStateId, int? partnerId)
        {
            int rIndex = 1;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("PO");

                int columnCount = AddOrderExportHeader(sheet);

                var auditResult = await _context.Set<OrderReport>().FromSql("OrderReport {0}, {1}, {2}, {3}", orderId, documentId, assetStateId, partnerId).ToListAsync();

                foreach (var order in auditResult)
                {
                    rIndex++;
                    AddOrderInList(rIndex, sheet, order);
                }

                ApplyFileFormat(columnCount, sheet);

                return package.GetAsByteArray();
            }
        }
    }
}
