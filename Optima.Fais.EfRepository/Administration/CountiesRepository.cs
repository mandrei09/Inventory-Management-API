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
    public class CountiesRepository : Repository<County>, ICountiesRepository
    {
        public CountiesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (c) => (c.Code.Contains(filter) || c.Name.Contains(filter)); })
        { }

        private Expression<Func<County, bool>> GetFiltersPredicate(string filter, List<int> countryIds)
        {
            Expression<Func<County, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            //if ((admCenterIds != null) && (admCenterIds.Count > 0))
            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Location>(predicate, l => admCenterIds.Contains(l.AdmCenterId.GetValueOrDefault()))
            //        : l => admCenterIds.Contains(l.AdmCenterId.GetValueOrDefault());

            if ((countryIds != null) && (countryIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.County, int>((id) => { return c => c.CountryId == id; }, countryIds);
                inListPredicate = ExpressionHelper.Or<Model.County>(inListPredicate, c => c.CountryId == null);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.County>(predicate, inListPredicate)
                    : inListPredicate;
            }

            return predicate;
        }

        public IEnumerable<County> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> countryIds)
        {
            var predicate = GetFiltersPredicate(filter, countryIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> countryIds)
        {
            var predicate = GetFiltersPredicate(filter, countryIds);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.County> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.Counties.AsNoTracking();

            if (lastId.HasValue)
            {
                query = query
                    .Where(r => (((r.ModifiedAt == lastModifiedAt) && (r.Id > lastId)) || (r.ModifiedAt > lastModifiedAt)));
                totalItems = query.Count();
                query = query
                    .OrderBy(a => a.ModifiedAt)
                    .ThenBy(a => a.Id)
                    .Take(pageSize);
            }
            else
            {
                totalItems = query.Count();
                query = query
                    .OrderBy(a => a.ModifiedAt)
                    .ThenBy(a => a.Id)
                    .Take(pageSize);
            }

            return query.ToList();
        }
    }
}
