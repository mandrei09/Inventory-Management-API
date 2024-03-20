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
    public class OrderMaterialsRepository : Repository<OrderMaterial>, IOrderMaterialsRepository
    {
        public OrderMaterialsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Order.Code.Contains(filter) || a.Order.Name.Contains(filter) || a.Material.Code.Contains(filter) || a.Material.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.OrderMaterial, bool>> GetFiltersPredicate(string filter, List<int?> orderIds, List<int?> materialIds, List<int?> subCategoryIds)
        {
            Expression<Func<Model.OrderMaterial, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((orderIds != null) && (orderIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.OrderMaterial>(predicate, r => orderIds.Contains(r.OrderId))
                    : r => orderIds.Contains(r.OrderId);
            }

            if ((materialIds != null) && (materialIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.OrderMaterial>(predicate, r => materialIds.Contains(r.MaterialId))
                    : r => materialIds.Contains(r.MaterialId);
            }

            //if ((subCategoryIds != null) && (subCategoryIds.Count > 0))
            //{
            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Model.OrderMaterial>(predicate, r => subCategoryIds.Contains(r.Material.SubCategoryId))
            //        : r => subCategoryIds.Contains(r.Material.SubCategoryId);
            //}

            return predicate;
        }

        public IEnumerable<Model.OrderMaterial> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> orderIds, List<int?> materialIds, List<int?> subCategoryIds)
        {
            var predicate = GetFiltersPredicate(filter, orderIds, materialIds, subCategoryIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> orderIds, List<int?> materialIds, List<int?> subCategoryIds)
        {
            var predicate = GetFiltersPredicate(filter, orderIds, materialIds, subCategoryIds);

            return GetQueryable(predicate).Count();
        }

        public async Task<List<Model.OrderMaterial>> GetAllOrderMaterialsByOrderId(int? orderID)
        {
            return await _context.Set<Model.OrderMaterial>().Where(a => a.OrderId == orderID).ToListAsync();
        }
    }
}
