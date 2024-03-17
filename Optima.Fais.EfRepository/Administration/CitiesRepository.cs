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
    public class CitiesRepository : Repository<City>, ICitiesRepository
    {
        public CitiesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (c) => (c.Code.Contains(filter) || c.Name.Contains(filter)); })
        { }

        private Expression<Func<City, bool>> GetFiltersPredicate(string filter, List<int> countyIds)
        {
            Expression<Func<City, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            //if ((admCenterIds != null) && (admCenterIds.Count > 0))
            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Location>(predicate, l => admCenterIds.Contains(l.AdmCenterId.GetValueOrDefault()))
            //        : l => admCenterIds.Contains(l.AdmCenterId.GetValueOrDefault());

            if ((countyIds != null) && (countyIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.City, int>((id) => { return c => c.CountyId == id; }, countyIds);
                inListPredicate = ExpressionHelper.Or<Model.City>(inListPredicate, c => c.CountyId == null);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.City>(predicate, inListPredicate)
                    : inListPredicate;
            }

            return predicate;
        }

        public IEnumerable<City> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> countyIds)
        {
            var predicate = GetFiltersPredicate(filter, countyIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> countyIds)
        {
            var predicate = GetFiltersPredicate(filter, countyIds);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.City> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.Cities.AsNoTracking();

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
