using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Optima.Fais.EfRepository
{
    public class AquisitionAssetSAPsRepository : Repository<AcquisitionAssetSAP>, IAquisitionAssetSAPsRepository
    {
        public AquisitionAssetSAPsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Asset.InvNo.Contains(filter) || a.Asset.Name.Contains(filter)); })
        { }


        private Expression<Func<AcquisitionAssetSAP, bool>> GetFiltersPredicate(AssetReceptionFilter assetFilter, string filter, bool isTesting)
        {
            Expression<Func<AcquisitionAssetSAP, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

			if (isTesting)
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.AcquisitionAssetSAP>(predicate, r => r.IsTesting == true)
					: r => r.IsTesting == true;
			}

			if ((assetFilter.AssetIds != null) && (assetFilter.AssetIds.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.AcquisitionAssetSAP>(predicate, r => assetFilter.AssetIds.Contains(r.AssetId))
                : r => assetFilter.AssetIds.Contains(r.AssetId);
            }
            return predicate;
        }

        public IEnumerable<AcquisitionAssetSAP> GetByFilters(AssetReceptionFilter assetFilter, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, bool isTesting)
        {
            var predicate = GetFiltersPredicate(assetFilter, filter, isTesting);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(AssetReceptionFilter assetFilter, string filter, bool isTesting)
        {
            var predicate = GetFiltersPredicate(assetFilter, filter, isTesting);

            return GetQueryable(predicate).Count();
        }
    }
}
