using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class StocksRepository : Repository<Stock>, IStocksRepository
    {
        public StocksRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter) || a.Material.Code.Contains(filter)); })
        { }

        private Expression<Func<Stock, bool>> GetFiltersPredicate(string filter, List<int?> categoryIds, List<int?> plantInitialIds, List<int?> plantActualIds, List<int?> exceptMaterialIds, List<int?> storageInitialIds, bool showStock)
        {
            Expression<Func<Stock, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            //if ((admCenterIds != null) && (admCenterIds.Count > 0))
            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Location>(predicate, l => admCenterIds.Contains(l.AdmCenterId.GetValueOrDefault()))
            //        : l => admCenterIds.Contains(l.AdmCenterId.GetValueOrDefault());

            //if ((categoryIds != null) && (categoryIds.Count > 0))
            //{
            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Model.Stock>(predicate, r => categoryIds.Contains(r.CategoryId))
            //        : r => categoryIds.Contains(r.CategoryId);
            //}

            if ((plantInitialIds != null) && (plantInitialIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Stock>(predicate, r => plantInitialIds.Contains(r.PlantInitialId))
                    : r => plantInitialIds.Contains(r.PlantInitialId);
            }

            if ((plantActualIds != null) && (plantActualIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Stock>(predicate, r => plantActualIds.Contains(r.PlantActualId))
                    : r => plantActualIds.Contains(r.PlantActualId);
            }

            if ((exceptMaterialIds != null) && (exceptMaterialIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Stock>(predicate, r => !exceptMaterialIds.Contains(r.Id))
                    : r => !exceptMaterialIds.Contains(r.Id);
            }

            //if ((storageInitialIds != null) && (storageInitialIds.Count > 0))
            //{
            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Model.Stock>(predicate, r => storageInitialIds.Contains(r.StorageInitialId))
            //        : r => storageInitialIds.Contains(r.StorageInitialId);
            //}

            predicate = predicate != null
                    ? ExpressionHelper.And<Model.Stock>(predicate, r => r.Imported == showStock)
                    : r => r.Imported == showStock;

            return predicate;
        }

        public IEnumerable<Stock> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> categoryIds, List<int?> plantInitialIds, List<int?> plantActualIds, List<int?> exceptMaterialIds, List<int?> storageInitialIds, bool showStock)
        {
            var predicate = GetFiltersPredicate(filter, categoryIds, plantInitialIds, plantActualIds, exceptMaterialIds, storageInitialIds, showStock);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> categoryIds, List<int?> plantInitialIds, List<int?> plantActualIds, List<int?> exceptMaterialIds, List<int?> storageInitialIds, bool showStock)
        {
            var predicate = GetFiltersPredicate(filter, categoryIds, plantInitialIds, plantActualIds, exceptMaterialIds, storageInitialIds, showStock);

            return GetQueryable(predicate).Count();
        }
    }
}
