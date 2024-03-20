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
    public class MaterialsRepository : Repository<Material>, IMaterialsRepository
    {
        public MaterialsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        private Expression<Func<Material, bool>> GetFiltersPredicate(string filter, List<int> countyIds, List<int?> materialIds, List<int?> exceptMaterialIds, bool hasSubCategory)
        {
            Expression<Func<Material, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            //if ((admCenterIds != null) && (admCenterIds.Count > 0))
            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Location>(predicate, l => admCenterIds.Contains(l.AdmCenterId.GetValueOrDefault()))
            //        : l => admCenterIds.Contains(l.AdmCenterId.GetValueOrDefault());

            if ((countyIds != null) && (countyIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.Material, int>((id) => { return c => c.AccountId == id; }, countyIds);
                inListPredicate = ExpressionHelper.Or<Model.Material>(inListPredicate, c => c.AccountId == null);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Material>(predicate, inListPredicate)
                    : inListPredicate;
            }

            if ((exceptMaterialIds != null) && (exceptMaterialIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Material>(predicate, r => !exceptMaterialIds.Contains(r.Id))
                    : r => !exceptMaterialIds.Contains(r.Id);
            }

            if ((materialIds != null) && (materialIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Material>(predicate, r => materialIds.Contains(r.Id))
                    : r => materialIds.Contains(r.Id);
            }

            //if (hasSubCategory)
            //{
            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Model.Material>(predicate, r => r.SubCategoryId != null)
            //        : r => r.SubCategoryId != null;
            //}

            return predicate;
        }

        public IEnumerable<Material> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> countyIds, List<int?> materialIds, List<int?> exceptMaterialIds, bool hasSubCategory)
        {
            var predicate = GetFiltersPredicate(filter, countyIds, materialIds, exceptMaterialIds, hasSubCategory);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> countyIds, List<int?> materialIds, List<int?> exceptMaterialIds, bool hasSubCategory)
        {
            var predicate = GetFiltersPredicate(filter, countyIds, materialIds, exceptMaterialIds, hasSubCategory);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.Material> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.Materials.AsNoTracking();

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
