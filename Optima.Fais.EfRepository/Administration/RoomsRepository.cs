using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class RoomsRepository : Repository<Model.Room>, IRoomsRepository
    {
        public RoomsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (c) => (c.Code.Contains(filter) || c.Name.Contains(filter)); })
        { }

        //public IEnumerable<Dto.RoomDetail> GetDetailsByFilters(int? locationId, string filter, string sortColumn, string sortDirection, int? page, int? pageSize, out int count)
        //{
        //    var query = _context.Set<Model.Room>().Include("Location").Select(r => new Dto.RoomDetail()
        //    {
        //        Id = r.Id,
        //        Code = r.Code,
        //        Name = r.Name,
        //        LocationId = r.LocationId,
        //        LocationCode = r.Location.Code,
        //        LocationName = r.Location.Name
        //    });

        //    if (locationId.HasValue) query = query.Where(r => r.LocationId == locationId);
        //    if (filter != null) query = query.Where(r => (r.Code.Contains(filter) || r.Name.Contains(filter) || r.LocationCode.Contains(filter) || r.LocationName.Contains(filter)));

        //    count = query.Count();

        //    if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
        //    {
        //        query = sortDirection.ToLower() == "asc"
        //            ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Dto.RoomDetail>(sortColumn))
        //            : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Dto.RoomDetail>(sortColumn));
        //    }

        //    if (page.HasValue && pageSize.HasValue)
        //    {
        //        query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        //    }

        //    return query.ToList();
        //}

        private Expression<Func<Model.Room, bool>> GetFiltersPredicate(string filter, List<int> locationIds, List<int> regionIds, List<int> admCenterIds)
        {
            Expression<Func<Model.Room, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((locationIds != null) && (locationIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Room>(predicate, r => locationIds.Contains(r.LocationId))
                    : r => locationIds.Contains(r.LocationId);
            }

            if ((regionIds != null) && (regionIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Room>(predicate, r => regionIds.Contains(r.Location.RegionId.Value))
                    : r => regionIds.Contains(r.Location.RegionId.Value);
            }

            if ((admCenterIds != null) && (admCenterIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Room>(predicate, r => admCenterIds.Contains(r.Location.AdmCenterId.Value))
                    : r => admCenterIds.Contains(r.Location.AdmCenterId.Value);
            }

            return predicate;
        }

        public IEnumerable<Model.Room> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> locationIds, List<int> regionIds, List<int> admCenterIds)
        {
            var predicate = GetFiltersPredicate(filter, locationIds, regionIds, admCenterIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> locationIds, List<int> regionIds, List<int> admCenterIds)
        {
            var predicate = GetFiltersPredicate(filter, locationIds, regionIds, admCenterIds);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.Room> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.Rooms.AsNoTracking();

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
