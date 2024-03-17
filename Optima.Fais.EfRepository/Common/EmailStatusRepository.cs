using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class EmailStatusRepository : Repository<EmailStatus>, IEmailStatusRepository
    {
        public EmailStatusRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Offer.Code.Contains(filter) || a.Offer.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.EmailStatus, bool>> GetFiltersPredicate(string filter, List<int?> emailTypeIds, List<int?> appStateIds, List<int?> assetCategoryIds)
        {
            Expression<Func<Model.EmailStatus, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((emailTypeIds != null) && (emailTypeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmailStatus>(predicate, r => emailTypeIds.Contains(r.EmailTypeId))
                    : r => emailTypeIds.Contains(r.EmailTypeId);
            }

            if ((appStateIds != null) && (appStateIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmailStatus>(predicate, r => appStateIds.Contains(r.AppStateId))
                    : r => appStateIds.Contains(r.AppStateId);
            }


            return predicate;
        }

        public IEnumerable<Model.EmailStatus> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> emailTypeIds, List<int?> appStateIds, List<int?> assetCategoryIds)
        {
            var predicate = GetFiltersPredicate(filter, emailTypeIds, appStateIds, assetCategoryIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> emailTypeIds, List<int?> appStateIds, List<int?> assetCategoryIds)
        {
            var predicate = GetFiltersPredicate(filter, emailTypeIds, appStateIds, assetCategoryIds);

            return GetQueryable(predicate).Count();
        }
    }
}
