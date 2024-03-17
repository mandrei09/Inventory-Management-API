using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IBudgetBasesRepository : IRepository<Model.BudgetBase>
    {

        IEnumerable<Model.BudgetBaseDetail> GetBuget(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        IEnumerable<Model.BudgetBaseDetail> GetBugetUI(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        IEnumerable<Model.BudgetBaseDetail> GetBugetFreezeUI(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        Model.BudgetBase GetDetailsById(int id, string includes);
		Task<Model.ImportBudgetResult> BudgetBaseImport(Dto.BudgetBaseImport budgetImport);
        int CreateBudget(BudgetSave asset);
		Task<Model.ImportBudgetResult> CreateRequestBudget(BudgetAddSave asset);
        int UpdateBudget(BudgetSave asset);
    }
}
