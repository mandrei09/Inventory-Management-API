using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Optima.Fais.EfRepository
{
    public class DepartmentsRepository : Repository<Model.Department>, IDepartmentsRepository
    {
        public DepartmentsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (c) => (c.Code.Contains(filter) || c.Name.Contains(filter)); })
        { }

        public IEnumerable<Dto.DepartmentDetail> GetDetailsByFilters(int? teamLeaderId, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, out int count)
        {
            ////Expression<Func<Model.Department, bool>> predicate = null;
            //Expression<Func<Dto.DepartmentDetail, bool>> predicate = null;

            ////if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);
            //if ((filter != null) && (filter.Length > 0))
            //    predicate = ExpressionHelper.GetAllPartsFilter<Dto.DepartmentDetail>(filter, 
            //        (search) => { return (d) => (d.Code.Contains(search) || d.Name.Contains(search) || (d.InternalCode.Contains(search) || (d.FirstName.Contains(search)) || (d.LastName.Contains(search)))); });

            ////if (teamLeaderId.HasValue) predicate = ExpressionHelper.And(predicate, (d) => d.TeamLeaderId == teamLeaderId);
            //predicate = (!teamLeaderId.HasValue) ? predicate : ((predicate == null) ? ((d) => d.TeamLeaderId == teamLeaderId) : ExpressionHelper.And(predicate, (d) => d.TeamLeaderId == teamLeaderId));

            //var query = GetQueryable(predicate, "TeamLeader", sortColumn, sortDirection, page, pageSize).Select(d => new Dto.DepartmentDetail()
            //var query = GetQueryable(predicate, "TeamLeader", null, null, null, null).Select(d => new Dto.DepartmentDetail()
            var query = _context.Set<Model.Department>().Include("TeamLeader").Where(d => d.IsDeleted == false).Select(d => new Dto.DepartmentDetail()
            {
                Id = d.Id,
                Code = d.Code,
                Name = d.Name,
                TeamLeaderId = d.TeamLeaderId,
                InternalCode = d.TeamLeader.InternalCode,
                FirstName = d.TeamLeader.FirstName,
                LastName = d.TeamLeader.LastName
            });

            if (teamLeaderId.HasValue) query = query.Where(d => d.TeamLeaderId == teamLeaderId);
            if (filter != null) query = query.Where(d => (d.Code.Contains(filter) || d.Name.Contains(filter) || d.InternalCode.Contains(filter) || d.FirstName.Contains(filter) || d.LastName.Contains(filter)));

            count = query.Count();

            if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Dto.DepartmentDetail>(sortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Dto.DepartmentDetail>(sortColumn));
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query.ToList();
        }

        public IEnumerable<Model.Department> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.Departments.AsNoTracking();

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
