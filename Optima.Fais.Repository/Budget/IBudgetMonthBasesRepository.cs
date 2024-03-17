using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IBudgetMonthBasesRepository : IRepository<Model.BudgetMonthBase>
    {

        IEnumerable<Model.BudgetMonthBaseDetail> GetBuget(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
    }
}
