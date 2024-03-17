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
    public class DictionaryTypesRepository : Repository<DictionaryType>, IDictionaryTypesRepository
    {
        public DictionaryTypesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        private Expression<Func<DictionaryType, bool>> GetFiltersPredicate(string filter)
        {
            Expression<Func<DictionaryType, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            //if ((admCenterIds != null) && (admCenterIds.Count > 0))
            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Location>(predicate, l => admCenterIds.Contains(l.AdmCenterId.GetValueOrDefault()))
            //        : l => admCenterIds.Contains(l.AdmCenterId.GetValueOrDefault());

            //if ((regionIds != null) && (regionIds.Count > 0))
            //{
            //    var inListPredicate = ExpressionHelper.GetInListPredicate<Model.Division, int>((id) => { return c => c.RegionId == id; }, regionIds);
            //    inListPredicate = ExpressionHelper.Or<Model.Division>(inListPredicate, c => c.RegionId == null);

            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Model.Division>(predicate, inListPredicate)
            //        : inListPredicate;
            //}

            return predicate;
        }

        public IEnumerable<DictionaryType> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filters)
        {
            var predicate = GetFiltersPredicate(filters);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.DictionaryType> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.DictionaryTypes.AsNoTracking();

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
