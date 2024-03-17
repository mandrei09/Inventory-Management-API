using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class DevicesRepository : Repository<Device>, IDevicesRepository
    {
        public DevicesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter) ||  a.UUI.Contains(filter) || a.Employee.InternalCode.Contains(filter) || a.Employee.FirstName.Contains(filter) || a.Employee.LastName.Contains(filter)); })
        { }

        private Expression<Func<Device, bool>> GetFiltersPredicate(string filter, List<int> infoTypeIds)
        {
            Expression<Func<Device, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


            if ((infoTypeIds != null) && (infoTypeIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.Device, int>((id) => { return c => c.DeviceTypeId == id; }, infoTypeIds);
                inListPredicate = ExpressionHelper.Or<Model.Device>(inListPredicate, c => c.DeviceTypeId == null);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Device>(predicate, inListPredicate)
                    : inListPredicate;
            }

            return predicate;
        }

        public IEnumerable<Device> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> infoTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, infoTypeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> infoTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, infoTypeIds);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.Device> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.Devices.Where(a => a.IsDeleted == false);

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
