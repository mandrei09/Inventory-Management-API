using Microsoft.EntityFrameworkCore;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Optima.Fais.Api.Services.Flow
{
    public class OrderFlowService : IOrderFlowService
    {
        protected ApplicationDbContext _context = null;
        private readonly IDocumentTypesRepository _documentTypesRepository;
        private readonly IDocumentsRepository _documentsRepository;
        private readonly IAppStatesRepository _appStatesRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IRequestBudgetForecastsRepository _requestBudgetForecastsRepository;
        private readonly IRequestBudgetForecastMaterialsRepository _requestBudgetForecastMaterialsRepository;
        public readonly IEmailOrderStatusRepository _emailOrderStatusRepository;
        public readonly IOrderMaterialsRepository _orderMaterialsRepository;
        public readonly IOrderOpsRepository _orderOpsRepository;
        public readonly IOfferMaterialsRepository _offerMaterialsRepository;

        private readonly string _documentTypeOrderRequest;
        private readonly string _appStateOrderRequest;
        public OrderFlowService(IConfiguration configuration, ApplicationDbContext context, IDocumentTypesRepository documentTypesRepository, 
            IDocumentsRepository documentsRepository, IAppStatesRepository appStatesRepository, IOrdersRepository ordersRepository,
            IRequestBudgetForecastsRepository requestBudgetForecastsRepository, 
            IRequestBudgetForecastMaterialsRepository requestBudgetForecastMaterialsRepository, IEmailOrderStatusRepository emailOrderStatusRepository,
            IOrderMaterialsRepository orderMaterialsRepository, IOrderOpsRepository orderOpsRepository, IOfferMaterialsRepository offerMaterialsRepository)
        {
            _context = context;
            _documentTypesRepository = documentTypesRepository;
            _documentTypeOrderRequest = configuration.GetSection("DocumentType").GetValue<string>("OrderRequest");
            _appStateOrderRequest = configuration.GetSection("DocumentType").GetValue<string>("AppState");
            _documentsRepository = documentsRepository;
            _appStatesRepository = appStatesRepository;
            _ordersRepository = ordersRepository;
            _requestBudgetForecastsRepository = requestBudgetForecastsRepository;
            _requestBudgetForecastMaterialsRepository = requestBudgetForecastMaterialsRepository;
            _emailOrderStatusRepository = emailOrderStatusRepository;
            _orderMaterialsRepository = orderMaterialsRepository;
            _orderOpsRepository = orderOpsRepository;
            _offerMaterialsRepository = offerMaterialsRepository;
        }

        //public async Task<OrderResult> DeleteOrder(OrderDelete orderDto)
        //{
        //    Model.Order order = null;
        //    Model.OrderOp orderOp = null;
        //    Model.Document document = null;
        //    Model.AppState appState = null;
        //    Model.DocumentType documentType = null;
        //    List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
        //    List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
        //    List<Model.EmailOrderStatus> emailOrderStatuses = null;
        //    List<Model.OrderMaterial> orderMaterials = null;
        //    List<Model.OfferMaterial> offerMaterials = null;

        //    documentType = await this._documentTypesRepository.GetByCodeAsync(this._documentTypeOrderRequest);
        //    if (documentType == null) return new OrderResult { Success = false, Message = "Nu exista tip de document" };

        //    appState = await this._appStatesRepository.GetByCodeAsync(this._appStateOrderRequest);
        //    if (appState == null) return new OrderResult { Success = false, Message = "Nu exista stare" };

        //    document = new Model.Document
        //    {
        //        Approved = true,
        //        DocumentType = documentType,
        //        DocNo1 = string.Empty,
        //        DocNo2 = string.Empty,
        //        DocumentDate = DateTime.Now,
        //        RegisterDate = DateTime.Now,
        //        Partner = null
        //    };

        //    this._documentsRepository.Create(document);

        //    order = this._ordersRepository.GetDetailsById(orderDto.Id,"");

        //    if (order == null) return new OrderResult { Success = false, Message = "Nu a fost gasit P.O. - ul" };

        //    requestBudgetForecasts = await this._requestBudgetForecastsRepository.GetAllRequestBFByRequestId(order.Offer.RequestId);

        //    for (int i = 0; i < requestBudgetForecasts.Count; i++)
        //    {

        //        requestBudgetForecasts[i].ContractId = null;
        //        requestBudgetForecasts[i].MaxQuantity = 0;
        //        requestBudgetForecasts[i].MaxValue = 0;
        //        requestBudgetForecasts[i].MaxValueRon = 0;
        //        requestBudgetForecasts[i].NeedBudget = false;
        //        requestBudgetForecasts[i].NeedBudgetValue = 0;
        //        requestBudgetForecasts[i].NeedContract = false;
        //        requestBudgetForecasts[i].NeedContractValue = 0;
        //        requestBudgetForecasts[i].Quantity = 0;
        //        requestBudgetForecasts[i].TotalOrderQuantity = 0;
        //        requestBudgetForecasts[i].TotalOrderValue = 0;
        //        requestBudgetForecasts[i].TotalOrderValueRon = 0;
        //        requestBudgetForecasts[i].Value = 0;
        //        requestBudgetForecasts[i].ValueRon = 0;

        //        this._requestBudgetForecastsRepository.Update(requestBudgetForecasts[i]);

        //        requestBudgetForecastMaterials = await this._requestBudgetForecastMaterialsRepository.GetAllRequestBFMaterialByRequestId(requestBudgetForecasts[i].Id);

        //        for (int m = 0; m < requestBudgetForecastMaterials.Count; m++)
        //        {
        //            this._requestBudgetForecastMaterialsRepository.Delete(requestBudgetForecastMaterials[m].Id);
        //        }

        //        //emailOrderStatuses = await _context.Set<Model.EmailOrderStatus>().Where(a => a.RequestBudgetForecastId == requestBudgetForecasts[i].Id).ToListAsync();

        //        emailOrderStatuses = await this._emailOrderStatusRepository.GetAllEmailOrderStatusesByRequestBFId(requestBudgetForecasts[i].Id);

        //        for (int e = 0; e < emailOrderStatuses.Count; e++)
        //        {
        //            this._emailOrderStatusRepository.Delete(emailOrderStatuses[e].Id);
        //        }
        //    }

        //    //orderMaterials = await _context.Set<Model.OrderMaterial>().Where(a => a.OrderId == order.Id).ToListAsync();
        //    orderMaterials = await this._orderMaterialsRepository.GetAllOrderMaterialsByOrderId(order.Id);

        //    for (int i = 0; i < orderMaterials.Count; i++)
        //    {
        //        orderMaterials[i].IsDeleted = true;
        //        this._orderMaterialsRepository.Delete(orderMaterials[i].Id);

        //    }

        //    orderOp = new Model.OrderOp()
        //    {
        //        AccMonthId = order.AccMonthId,
        //        AccSystemId = null,
        //        Order = order,
        //        BudgetStateId = appState.Id,
        //        CostCenterIdInitial = order.CostCenterId,
        //        CostCenterIdFinal = order.CostCenterId,
        //        CreatedAt = DateTime.Now,
        //        CreatedBy = order.UserId,
        //        Document = document,
        //        DstConfAt = DateTime.Now,
        //        DstConfBy = order.UserId,
        //        EmployeeIdInitial = order.EmployeeId,
        //        EmployeeIdFinal = order.EmployeeId,
        //        InfoIni = order.Info,
        //        InfoFin = order.Info,
        //        IsAccepted = false,
        //        IsDeleted = false,
        //        ModifiedAt = DateTime.Now,
        //        ModifiedBy = order.UserId,
        //        ProjectIdInitial = order.ProjectId,
        //        ProjectIdFinal = order.ProjectId,
        //        Validated = true,
        //        Guid = Guid.NewGuid(),
        //        BudgetIdInitial = order.BudgetId,
        //        BudgetIdFinal = order.BudgetId,
        //    };

        //    this._orderOpsRepository.Create(orderOp);

        //    order.Offer.ValueFin = order.Offer.ValueIni;
        //    order.Offer.QuantityRem = order.Offer.Quantity;
        //    order.Offer.QuantityUsed = 0;
        //    order.Offer.ValueUsed = 0;
        //    order.Offer.ValueFinRon = order.Offer.ValueIniRon;
        //    order.Offer.ValueUsedRon = 0;

        //    //offerMaterials = await _context.Set<Model.OfferMaterial>().Where(a => a.OfferId == order.OfferId).ToListAsync();
        //    offerMaterials = await this._offerMaterialsRepository.GetAllOfferMaterialsByOfferId(order.OfferId);

        //    for (int i = 0; i < offerMaterials.Count; i++)
        //    {
        //        offerMaterials[i].OrdersPrice = 0;
        //        offerMaterials[i].OrdersPriceRon = 0;
        //        offerMaterials[i].OrdersQuantity = 0;
        //        offerMaterials[i].OrdersValue = 0;
        //        offerMaterials[i].OrdersValueRon = 0;

        //        this._offerMaterialsRepository.Update(offerMaterials[i]);
        //    }

        //    order.AppStateId = appState.Id;
        //    order.IsAccepted = true;
        //    order.Validated = true;
        //    order.IsDeleted = true;

        //    this._ordersRepository.Update(order);
        //    _context.SaveChanges();

        //    await this._ordersRepository.UpdateAllBudgetBaseAsync();
        //    await this._ordersRepository.UpdateAllBudgetBasesAsync();

        //    return new OrderResult { Success = true, Message = "P.O. - ul a fost sters cu sucess!" };
        //}

        public async Task<OrderResult> DeleteOrder(OrderDelete orderDto)
        {
            await Task.Delay(0);
            Model.Order order = null;
            Model.OrderOp orderOp = null;
            Model.Document document = null;
            Model.AppState appState = null;
            Model.DocumentType documentType = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
            List<Model.EmailOrderStatus> emailOrderStatuses = null;
            List<Model.OrderMaterial> orderMaterials = null;
            List<Model.OfferMaterial> offerMaterials = null;

            documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == "ORDER_REQUEST").FirstOrDefaultAsync();
            if (documentType == null) return new OrderResult { Success = false, Message = "Nu exista tip de document" };

            appState = await _context.Set<Model.AppState>().Where(d => d.Code == "ORDER_REQUEST").FirstOrDefaultAsync();
            if (appState == null) return new OrderResult { Success = false, Message = "Nu exista stare" };

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

            order = await _context.Set<Model.Order>()
                .Include(o => o.Offer).ThenInclude(r => r.Request)
                .Where(a => a.Id == orderDto.Id)
                .FirstOrDefaultAsync();

            if (order == null) return new OrderResult { Success = false, Message = "Nu a fost gasit P.O. - ul" };


            requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>().Where(a => a.RequestId == order.Offer.RequestId).ToListAsync();

            for (int i = 0; i < requestBudgetForecasts.Count; i++)
            {

                requestBudgetForecasts[i].ContractId = null;
                requestBudgetForecasts[i].MaxQuantity = 0;
                requestBudgetForecasts[i].MaxValue = 0;
                requestBudgetForecasts[i].MaxValueRon = 0;
                requestBudgetForecasts[i].NeedBudget = false;
                requestBudgetForecasts[i].NeedBudgetValue = 0;
                requestBudgetForecasts[i].NeedContract = false;
                requestBudgetForecasts[i].NeedContractValue = 0;
                requestBudgetForecasts[i].Quantity = 0;
                requestBudgetForecasts[i].TotalOrderQuantity = 0;
                requestBudgetForecasts[i].TotalOrderValue = 0;
                requestBudgetForecasts[i].TotalOrderValueRon = 0;
                requestBudgetForecasts[i].Value = 0;
                requestBudgetForecasts[i].ValueRon = 0;
                //requestBudgetForecasts[i].OfferTypeId = null;
                _context.Update(requestBudgetForecasts[i]);

                requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>().Where(a => a.RequestBudgetForecastId == requestBudgetForecasts[i].Id).ToListAsync();

                for (int m = 0; m < requestBudgetForecastMaterials.Count; m++)
                {
                    requestBudgetForecastMaterials[m].IsDeleted = true;
                    _context.Update(requestBudgetForecastMaterials[m]);
                }

                emailOrderStatuses = await _context.Set<Model.EmailOrderStatus>().Where(a => a.RequestBudgetForecastId == requestBudgetForecasts[i].Id).ToListAsync();

                for (int e = 0; e < emailOrderStatuses.Count; e++)
                {
                    emailOrderStatuses[e].IsDeleted = true;
                    _context.Update(emailOrderStatuses[e]);
                }
            }

            orderMaterials = await _context.Set<Model.OrderMaterial>().Where(a => a.OrderId == order.Id).ToListAsync();

            for (int i = 0; i < orderMaterials.Count; i++)
            {
                orderMaterials[i].IsDeleted = true;
                _context.Update(orderMaterials[i]);

            }


            orderOp = new Model.OrderOp()
            {
                AccMonthId = order.AccMonthId,
                AccSystemId = null,
                Order = order,
                BudgetStateId = appState.Id,
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

            order.Offer.ValueFin = order.Offer.ValueIni;
            order.Offer.QuantityRem = order.Offer.Quantity;
            order.Offer.QuantityUsed = 0;
            order.Offer.ValueUsed = 0;
            order.Offer.ValueFinRon = order.Offer.ValueIniRon;
            order.Offer.ValueUsedRon = 0;

            offerMaterials = await _context.Set<Model.OfferMaterial>().Where(a => a.OfferId == order.OfferId).ToListAsync();

            for (int i = 0; i < offerMaterials.Count; i++)
            {
                offerMaterials[i].OrdersPrice = 0;
                offerMaterials[i].OrdersPriceRon = 0;
                offerMaterials[i].OrdersQuantity = 0;
                offerMaterials[i].OrdersValue = 0;
                offerMaterials[i].OrdersValueRon = 0;

                _context.Update(offerMaterials[i]);

            }

            order.AppStateId = appState.Id;
            order.IsAccepted = true;
            order.Validated = true;
            order.IsDeleted = true;

            _context.Update(order);



            _context.SaveChanges();

            var countBudgetBase = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
            var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();

            return new OrderResult { Success = true, Message = "P.O. - ul a fost sters cu sucess!" };
        }

    }
}
