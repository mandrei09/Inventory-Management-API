using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class PartnersRepository : Repository<Partner>, IPartnersRepository
    {
        public PartnersRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (p) => (p.Name.Contains(filter) || p.FiscalCode.Contains(filter) || p.RegistryNumber.Contains(filter)); })
        { }

        private Expression<Func<Model.Partner, bool>> GetFiltersPredicate(string filter, List<int> partnerIds, bool showAll)
        {
            Expression<Func<Model.Partner, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


			//if ((partnerIds != null) && (partnerIds.Count > 0))
			//{
			//    var inListPredicate = ExpressionHelper.GetInListPredicate<Model.Partner, int>((id) => { return c => c.Id == id; }, partnerIds);
			//    inListPredicate = ExpressionHelper.Or<Model.Partner>(inListPredicate, c => c.Id > 0);

			//    predicate = predicate != null
			//        ? ExpressionHelper.And<Model.Partner>(predicate, inListPredicate)
			//        : inListPredicate;
			//}

			if (!showAll)
			{
                if ((partnerIds != null) && (partnerIds.Count > 0))
                {
                    predicate = predicate != null
                      ? ExpressionHelper.And<Model.Partner>(predicate, r => partnerIds.Contains(r.Id))
                      : r => partnerIds.Contains(r.Id);
                }
            }

            return predicate;
        }

        public IEnumerable<Model.Partner> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> partnerIds, bool showAll)
        {
            var predicate = GetFiltersPredicate(filter, partnerIds, showAll);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> partnerIds, bool showAll)
        {
            var predicate = GetFiltersPredicate(filter, partnerIds, showAll);

            return GetQueryable(predicate).Count();
        }
    }
}
