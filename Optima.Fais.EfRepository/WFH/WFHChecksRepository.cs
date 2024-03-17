using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class WFHChecksRepository : Repository<WFHCheck>, IWFHChecksRepository
	{
        public WFHChecksRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.DictionaryItem.Name.Contains(filter) || a.Brand.Name.Contains(filter) ||  a.Model.Name.Contains(filter) || a.SerialNumber.Contains(filter) || a.Imei.Contains(filter) || a.InventoryNumber.Contains(filter)); })
        { }

        private Expression<Func<WFHCheck, bool>> GetFiltersPredicate(string filter, List<int> infoTypeIds)
        {
            Expression<Func<WFHCheck, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


            if ((infoTypeIds != null) && (infoTypeIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.WFHCheck, int>((id) => { return c => c.DictionaryItemId == id; }, infoTypeIds);
                inListPredicate = ExpressionHelper.Or<Model.WFHCheck>(inListPredicate, c => c.DictionaryItemId > 0);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.WFHCheck>(predicate, inListPredicate)
                    : inListPredicate;
            }

            return predicate;
        }

        public IEnumerable<WFHCheck> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> infoTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, infoTypeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> infoTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, infoTypeIds);

            return GetQueryable(predicate).Count();
        }
    }
}
