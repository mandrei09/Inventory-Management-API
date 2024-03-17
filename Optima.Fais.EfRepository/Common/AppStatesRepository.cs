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
    public class AppStatesRepository : Repository<AppState>, IAppStatesRepository
    {
        public AppStatesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (d) => (d.Code.Contains(filter) || d.Name.Contains(filter)); })
        { }

        private Expression<Func<AppState, bool>> GetFiltersPredicate(string filter, string parentCode)
        {
            Expression<Func<AppState, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((parentCode != null) && (parentCode.Length > 0))
            {
                predicate = d => d.ParentCode == parentCode;
            }

            return predicate;
        }

        public IEnumerable<AppState> GetByFilters(string filter, string parentCode, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter, parentCode);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, string parentCode)
        {
            var predicate = GetFiltersPredicate(filter, parentCode);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<AppState> GetAppStatesByFilters(string parentCode)
        {
            Expression<Func<AppState, bool>> predicate = null;

            if ((parentCode != null) && (parentCode.Length > 0))
            {
                predicate = d => d.ParentCode == parentCode;
            }

            return Get(predicate, null, null, null, null, null);
        }

        public IEnumerable<AppState> GetAppStatesByParentCode(string parentCode)
        {
            Expression<Func<AppState, bool>> predicate = null;

            if ((parentCode != null) && (parentCode.Length > 0))
            {
                predicate = d => d.ParentCode == parentCode;
            }

            return Get(predicate, null, null, null, null, null);
        }

        public async Task<AppState> GetByCodeAsync(string code)
        {
            return await GetQueryable(p => p.Code == code).SingleOrDefaultAsync();
        }
    }
}
