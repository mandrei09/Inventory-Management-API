using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IRequestsRepository : IRepository<Model.Request>
    {

        IEnumerable<Model.RequestDetail> GetRequest(RequestFilter budgetFilter, ColumnRequestFilter colFilters, string includes, Sorting sorting, Paging paging, List<int?> budgetForecastIds, List<int?> requestIds, bool newBudget, bool needBudget, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        IEnumerable<Model.RequestDetail> GetRequestUI(RequestFilter budgetFilter, string includes, Sorting sorting, Paging paging, bool showExisting, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        IEnumerable<Model.RequestDetail> GetRequestBudgetUI(RequestFilter budgetFilter, string includes, Sorting sorting, Paging paging, bool showExisting, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        Task<Model.RequestResult> CreateOrUpdateRequest(RequestSave asset);
		Task<Model.RequestResult> UpdateRequest(RequestUpdate asset);
		Model.Request GetDetailsById(int id, string includes);
        IEnumerable<Model.Division> GetProjectTypeDivisionsWithBudgetBases(RequestFilter requestFilter);
        Task<Model.RequestResult> NeedBudget(Dto.NeedBudget needBudget);
		Task<Model.RequestResult> RequestBudgetForecastUpdate(RequestBudgetForecastUpdate asset);
	}
}
