using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Common;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IOrdersRepository : IRepository<Model.Order>
    {

        IEnumerable<Model.OrderDetail> GetOrder(OrderFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        IEnumerable<Model.OrderDetail> GetOrderUI(OrderFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        IEnumerable<Model.OrderDetail> GetOrderNeedBudgetUI(OrderFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        Task<Model.OrderResult> CreateOrUpdateOrder(OrderSave asset);
        Task<Model.OrderResult> CreateOrUpdateOrderStock(OrderStockSave asset);
        Task<Model.CreateAssetSAPResult> CreateOrUpdateOrderCheck(OrderStockSave order);
        Task<Model.CreateAssetSAPResult> OrderContractUpdate(OrderContractUpdate order);
        Model.Order GetDetailsById(int id, string includes);
		Task<Model.OrderResult> OrderEdit(OrderEdit asset);
		Task<Model.OrderResult> OrderBudgetForecastUpdate(OrderBudgetForecastUpdate asset);
		int SendEmail(int budgetId, string userName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut);
        int SendValidatedEmail(int budgetId, string userName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut);

        IEnumerable<Model.OrderDetail> BudgetValidate(OrderFilter budgetFilter, string includes, string userId, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        Task UpdateAllBudgetBaseAsync();
        Task UpdateAllBudgetBasesAsync();
        Task<byte[]> ExportOrderStatusAsync(int? orderId, int? documentId, int? assetStateId, int? partnerId);
    }
}
