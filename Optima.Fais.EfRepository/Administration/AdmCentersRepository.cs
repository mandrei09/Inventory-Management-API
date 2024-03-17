using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class AdmCentersRepository : Repository<AdmCenter>, IAdmCentersRepository
    {
        public AdmCentersRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }


        public IEnumerable<AdmCenter> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> employeeIds)
        {
            var predicate = GetFiltersPredicate(filter, employeeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        private Expression<Func<AdmCenter, bool>> GetFiltersPredicate(string filter, List<int> employeeIds)
        {
            Expression<Func<AdmCenter, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


            if ((employeeIds != null) && (employeeIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.AdmCenter, int>((id) => { return c => c.EmployeeId == id; }, employeeIds);
                inListPredicate = ExpressionHelper.Or<Model.AdmCenter>(inListPredicate, c => c.EmployeeId == null);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.AdmCenter>(predicate, inListPredicate)
                    : inListPredicate;
            }

            return predicate;

        }


        public int GetCountByFilters(string filter, List<int> employeeIds)
        {
            var predicate = GetFiltersPredicate(filter, employeeIds);

            return GetQueryable(predicate).Count();
        }
    }
}
