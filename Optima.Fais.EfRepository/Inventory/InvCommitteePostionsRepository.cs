using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using Optima.Fais.Repository.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Optima.Fais.EfRepository.Inventory
{
    public class InvCommitteePostionsRepository : Repository<InvCommitteePosition>, IInvCommitteePositionsRepository
    {
        public InvCommitteePostionsRepository(ApplicationDbContext context)
          :     base(context, (filter) => { return (a) => ((a.Name.Contains(filter) || a.Code.Contains(filter)));
          }) { }

        private Expression<Func<InvCommitteePosition, bool>> GetFiltersPredicate(string filter)
        {
            Expression<Func<InvCommitteePosition, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            return predicate;
        }

        public IEnumerable<InvCommitteePosition> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter)
        {
            var predicate = GetFiltersPredicate(filter);

            return GetQueryable(predicate).Count();
        }
    }
}
