using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
    public class EmailOrderStatusRepository : Repository<EmailOrderStatus>, IEmailOrderStatusRepository
    {
        public EmailOrderStatusRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Order.Code.Contains(filter) || a.Order.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.EmailOrderStatus, bool>> GetFiltersPredicate(string filter, List<int?> emailTypeIds, List<int?> appStateIds)
        {
            Expression<Func<Model.EmailOrderStatus, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((emailTypeIds != null) && (emailTypeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmailOrderStatus>(predicate, r => emailTypeIds.Contains(r.EmailTypeId))
                    : r => emailTypeIds.Contains(r.EmailTypeId);
            }

            if ((appStateIds != null) && (appStateIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmailOrderStatus>(predicate, r => appStateIds.Contains(r.AppStateId))
                    : r => appStateIds.Contains(r.AppStateId);
            }


            return predicate;
        }

        public IEnumerable<Model.EmailOrderStatus> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> emailTypeIds, List<int?> appStateIds)
        {
            var predicate = GetFiltersPredicate(filter, emailTypeIds, appStateIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> emailTypeIds, List<int?> appStateIds)
        {
            var predicate = GetFiltersPredicate(filter, emailTypeIds, appStateIds);

            return GetQueryable(predicate).Count();
        }

        public async Task<List<Model.EmailOrderStatus>> GetAllEmailOrderStatusesByRequestBFId(int? requestId)
        {
            return await _context.Set<Model.EmailOrderStatus>().Where(a => a.RequestBudgetForecastId == requestId).ToListAsync();
        }
    }
}
