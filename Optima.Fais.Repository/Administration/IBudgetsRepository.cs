using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IBudgetsRepository : IRepository<Model.Budget>
    {

        IEnumerable<Model.BudgetDetail> GetBuget(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        IEnumerable<Model.BudgetDetail> GetBugetUI(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        int CreateOrUpdateBudget(BudgetSave asset);
        Model.Budget GetDetailsById(int id, string includes);
        int SendEmail(int budgetId, string userName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut);
        int SendValidatedEmail(int budgetId, string userName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut);

        // int BudgetImport(Dto.BudgetImport budgetImport);
        int BudgetBaseImport(Dto.BudgetBaseImport budgetImport);

        IEnumerable<Model.BudgetDetail> BudgetValidate(BudgetFilter budgetFilter, string includes, string userId, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        Task<List<Model.Budget>> GetAllIncludingBudgetMonthsAsync();
        Task<List<Model.Budget>> GetAllByProjectIncludingBudgetMonthsAsync();
    }
}
