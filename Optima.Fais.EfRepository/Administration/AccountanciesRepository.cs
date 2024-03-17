using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class AccountanciesRepository : Repository<Accountancy>, IAccountanciesRepository
    {
        public AccountanciesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Account.Code.Contains(filter) || 
            a.Account.Name.Contains(filter) ||
            a.ExpAccount.Code.Contains(filter) ||
            a.ExpAccount.Name.Contains(filter) ||
            a.AssetCategory.Code.Contains(filter) ||
            a.AssetCategory.Name.Contains(filter) ||
            a.AssetType.Code.Contains(filter) ||
            a.AssetType.Name.Contains(filter) ||
            a.SubCategory.Code.Contains(filter) ||
            a.SubCategory.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.Accountancy, bool>> GetFiltersPredicate(string filter, List<int?> accountIds, List<int?> expAccountIds, List<int?> assetCategoryIds)
        {
            Expression<Func<Model.Accountancy, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((accountIds != null) && (accountIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Accountancy>(predicate, r => accountIds.Contains(r.AccountId))
                    : r => accountIds.Contains(r.AccountId);
            }

            if ((expAccountIds != null) && (expAccountIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Accountancy>(predicate, r => expAccountIds.Contains(r.ExpAccountId))
                    : r => expAccountIds.Contains(r.ExpAccountId);
            }

            if ((assetCategoryIds != null) && (assetCategoryIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Accountancy>(predicate, r => assetCategoryIds.Contains(r.AssetCategoryId))
                    : r => assetCategoryIds.Contains(r.AssetCategoryId);
            }

            return predicate;
        }

        public IEnumerable<Model.Accountancy> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> offerIds, List<int?> materialIds, List<int?> subCategoryIds)
        {
            var predicate = GetFiltersPredicate(filter, offerIds, materialIds, subCategoryIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> offerIds, List<int?> materialIds, List<int?> subCategoryIds)
        {
            var predicate = GetFiltersPredicate(filter, offerIds, materialIds, subCategoryIds);

            return GetQueryable(predicate).Count();
        }
    }
}
