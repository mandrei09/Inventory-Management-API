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
    public class LocationsRepository : Repository<Location>, ILocationsRepository
    {
        public LocationsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (c) => (c.Code.Contains(filter) || c.Name.Contains(filter)); })
        { }

        private Expression<Func<Location, bool>> GetFiltersPredicate(string filter, List<int> cityIds, List<int> regionIds, List<int> admCenterIds)
        {
            Expression<Func<Location, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            //if ((admCenterIds != null) && (admCenterIds.Count > 0))
            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Location>(predicate, l => admCenterIds.Contains(l.AdmCenterId.GetValueOrDefault()))
            //        : l => admCenterIds.Contains(l.AdmCenterId.GetValueOrDefault());

            if ((cityIds != null) && (cityIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.Location, int>((id) => { return c => c.CityId == id; }, cityIds);
                inListPredicate = ExpressionHelper.Or<Model.Location>(inListPredicate, c => c.CityId == null);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Location>(predicate, inListPredicate)
                    : inListPredicate;
            }

            if ((regionIds != null) && (regionIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.Location, int>((id) => { return c => c.RegionId == id; }, regionIds);
                inListPredicate = ExpressionHelper.Or<Model.Location>(inListPredicate, c => c.RegionId == null);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Location>(predicate, inListPredicate)
                    : inListPredicate;
            }

            if ((admCenterIds != null) && (admCenterIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.Location, int>((id) => { return c => c.AdmCenterId == id; }, admCenterIds);
                inListPredicate = ExpressionHelper.Or<Model.Location>(inListPredicate, c => c.AdmCenterId == null);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Location>(predicate, inListPredicate)
                    : inListPredicate;
            }

            return predicate;
        }

        public IEnumerable<Location> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> cityIds, List<int> regionIds, List<int> admCenterIds)
        {
            var predicate = GetFiltersPredicate(filter, cityIds, regionIds, admCenterIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> cityIds, List<int> regionIds, List<int> admCenterIds)
        {
            var predicate = GetFiltersPredicate(filter, cityIds, regionIds, admCenterIds);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.Location> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.Locations.AsNoTracking();

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
