using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IBudgetForecastsRepository : IRepository<Model.BudgetForecast>
    {

        IEnumerable<Model.BudgetForecastDetail> GetBuget(BudgetFilter budgetFilter, string filter, string includes, Sorting sorting, Paging paging, out ForecastTotal depTotal);
        IEnumerable<Model.BudgetForecastDetail> GetBugetUI(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out ForecastTotal depTotal);
        Model.BudgetForecast GetDetailsById(int id, string includes);
        Task<Model.ImportBudgetResult> BudgetForecastImport(Dto.BudgetForecastImport budgetImport);
        Task<Model.RequestResult> UpdateBudgetForecast(BudgetSave asset);
		Task<Model.BudgetForecastCorrectionResult> CorrectionBudgetForecast(BudgetCorrectionSave asset);
		Task<Model.BudgetForecastCorrectionResult> CorrectionValidateBudgetForecast(BudgetCorrectionValidate asset);
		Task<Model.BudgetForecastCorrectionResult> TransferBudgetForecast(BudgetTransferSave asset);
		Task<Model.RequestResult> UpdateAssetBudgetForecast(BudgetForecastUpdate asset);
	}
}
