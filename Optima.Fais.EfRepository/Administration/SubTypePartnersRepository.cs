using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class SubTypePartnersRepository : Repository<SubTypePartner>, ISubTypePartnersRepository
    {
        public SubTypePartnersRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.SubType.Code.Contains(filter) || a.Partner.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.SubTypePartner, bool>> GetFiltersPredicate(string filter, List<int?> partnerId)
        {
            Expression<Func<Model.SubTypePartner, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((partnerId != null) && (partnerId.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.SubTypePartner>(predicate, r => partnerId.Contains(r.PartnerId))
                    : r => partnerId.Contains(r.PartnerId);
            }

          

            return predicate;
        }

        public IEnumerable<Model.SubTypePartner> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> partnerId)
        {
            var predicate = GetFiltersPredicate(filter, partnerId);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> partnerId)
        {
            var predicate = GetFiltersPredicate(filter, partnerId);

            return GetQueryable(predicate).Count();
        }
    }
}
