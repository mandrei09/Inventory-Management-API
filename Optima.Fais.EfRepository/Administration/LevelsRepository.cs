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
    public class LevelsRepository : Repository<Level>, ILevelsRepository
    {
        public LevelsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }


        public IEnumerable<Level> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter, null);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public Task<Level> GetByCodeAsync(string code)
        {
            var predicate = GetFiltersPredicate(null, code);

            return GetQueryable(predicate).FirstOrDefaultAsync();
        }

        public Level GetByCode(string code)
        {
            var predicate = GetFiltersPredicate(null, code);

            return GetQueryable(predicate).FirstOrDefault();
        }

        private Expression<Func<Level, bool>> GetFiltersPredicate(string filter, string code)
        {
            Expression<Func<Level, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);
            if (!String.IsNullOrEmpty(code)) predicate = ExpressionHelper.And<Level>(predicate, p => p.Code == code);

            return predicate;

        }


        public int GetCountByFilters(string filter)
        {
            var predicate = GetFiltersPredicate(filter, null);

            return GetQueryable(predicate).Count();
        }
    }
}
