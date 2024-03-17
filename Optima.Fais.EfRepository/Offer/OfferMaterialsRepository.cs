using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
    public class OfferMaterialsRepository : Repository<OfferMaterial>, IOfferMaterialsRepository
    {
        public OfferMaterialsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Offer.Code.Contains(filter) || a.Offer.Name.Contains(filter) || a.Material.Code.Contains(filter) || a.Material.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.OfferMaterial, bool>> GetFiltersPredicate(string filter, Guid guid, List<int?> offerIds, List<int?> materialIds, List<int?> requestIds, List<int?> subCategoryIds, List<int?> partnerIds)
        {
            Expression<Func<Model.OfferMaterial, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((offerIds != null) && (offerIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.OfferMaterial>(predicate, r => offerIds.Contains(r.OfferId))
                    : r => offerIds.Contains(r.OfferId);
            }

            if ((materialIds != null) && (materialIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.OfferMaterial>(predicate, r => materialIds.Contains(r.MaterialId))
                    : r => materialIds.Contains(r.MaterialId);
            }

            if ((subCategoryIds != null) && (subCategoryIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.OfferMaterial>(predicate, r => subCategoryIds.Contains(r.Material.SubCategoryId))
                    : r => subCategoryIds.Contains(r.Material.SubCategoryId);
            }

            if ((partnerIds != null) && (partnerIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.OfferMaterial>(predicate, r => partnerIds.Contains(r.EmailManager.PartnerId))
                    : r => partnerIds.Contains(r.EmailManager.PartnerId);
            }

			if ((requestIds != null) && (requestIds.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.OfferMaterial>(predicate, r => requestIds.Contains(r.RequestId))
					: r => requestIds.Contains(r.RequestId);
			}

			if (guid != Guid.Empty)
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.OfferMaterial>(predicate, r => r.Guid == guid)
                    : r => r.Guid == guid;
            }

            return predicate;
        }

        public IEnumerable<Model.OfferMaterial> GetByFilters(string filter, Guid guid, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> offerIds, List<int?> materialIds, List<int?> requestIds, List<int?> subCategoryIds, List<int?> partnerIds)
        {
            var predicate = GetFiltersPredicate(filter, guid, offerIds, materialIds, requestIds, subCategoryIds, partnerIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, Guid guid, List<int?> offerIds, List<int?> materialIds, List<int?> requestIds, List<int?> subCategoryIds, List<int?> partnerIds)
        {
            var predicate = GetFiltersPredicate(filter, guid, offerIds, materialIds, requestIds, subCategoryIds, partnerIds);

            return GetQueryable(predicate).Count();
        }

        public async Task<List<Model.OfferMaterial>> GetAllOfferMaterialsByOfferId(int? offerId)
        {
            return await _context.Set<Model.OfferMaterial>().Where(a => a.OfferId == offerId).ToListAsync();
        }
    }
}
