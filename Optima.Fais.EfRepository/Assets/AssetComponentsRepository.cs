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
    public class AssetComponentsRepository : Repository<AssetComponent>, IAssetComponentsRepository
    {
        public AssetComponentsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.AssetComponent, bool>> GetFiltersPredicate(string filter, List<int> assetIds, List<int> employeeIds, List<int> subTypeIds)
        {
            Expression<Func<Model.AssetComponent, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((assetIds != null) && (assetIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.AssetComponent>(predicate, r => assetIds.Contains(r.AssetId.Value))
                    : r => assetIds.Contains(r.AssetId.Value);
            }

            if ((employeeIds != null) && (employeeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.AssetComponent>(predicate, r => employeeIds.Contains(r.EmployeeId.Value))
                    : r => employeeIds.Contains(r.EmployeeId.Value);
            }

            if ((subTypeIds != null) && (subTypeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.AssetComponent>(predicate, r => subTypeIds.Contains(r.SubTypeId.Value))
                    : r => subTypeIds.Contains(r.SubTypeId.Value);
            }



            return predicate;
        }

        private Expression<Func<Model.AssetComponent, bool>> GetComponentFiltersPredicate(string filter, List<int> assetIds, List<int> assetId, List<int> employeeIds, List<int> subTypeIds, List<int> employeeId)
        {
            Expression<Func<Model.AssetComponent, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((assetIds != null) && (assetIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.AssetComponent>(predicate, r => assetIds.Contains(r.AssetId.Value))
                    : r => assetIds.Contains(r.AssetId.Value);
            }

            if ((assetId != null) && (assetId.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.AssetComponent>(predicate, r => assetId.Contains(r.AssetId.Value))
                    : r => assetIds.Contains(r.AssetId.Value);
            }


            if ((employeeIds != null) && (employeeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.AssetComponent>(predicate, r => employeeIds.Contains(r.EmployeeId.Value))
                    : r => employeeIds.Contains(r.EmployeeId.Value);
            }

            if ((subTypeIds != null) && (subTypeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.AssetComponent>(predicate, r => subTypeIds.Contains(r.SubTypeId.Value))
                    : r => subTypeIds.Contains(r.SubTypeId.Value);
            }

            if ((employeeId != null) && (employeeId.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.AssetComponent>(predicate, r => employeeId.Contains(r.EmployeeId.Value))
                    : r => employeeIds.Contains(r.EmployeeId.Value);
            }



            return predicate;
        }

        public IEnumerable<Model.AssetComponent> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> assetIds, List<int> employeeIds, List<int> subTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, assetIds, employeeIds, subTypeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> assetIds, List<int> employeeIds, List<int> subTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, assetIds, employeeIds, subTypeIds);

            return GetQueryable(predicate).Count();
        }


        public IEnumerable<Model.AssetComponent> GetFiltered(string includes, int? assetId, int? employeeId, string sortColumn, string sortDirection, int? page, int? pageSize, out int count)
        {

            IQueryable<Model.AssetComponent> query = null;
            query = _context.AssetComponents.AsNoTracking();

            if (includes != null)
            {
                foreach (var includeProperty in includes.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            else
            {
                query = query
                    .Include(i => i.Asset)
                    .AsQueryable();

            }



            if (assetId.HasValue) query = query.Where(a => a.AssetId == assetId);
            if (employeeId.HasValue) query = query.Where(a => a.EmployeeId == employeeId);


            // if (assetFilter.Filter != "" && assetFilter.Filter != null) query = query.Where(a => (a.Asset.InvNo.Contains(assetFilter.Filter) || a.Asset.ERPCode.Contains(assetFilter.Filter) || a.Asset.Name.Contains(assetFilter.Filter) || a.Asset.SerialNumber.Contains(assetFilter.Filter)));






            query = query.Where(op => op.IsDeleted == false);

            count = query.Count();

            if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetComponent>(sortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetComponent>(sortColumn));
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }




            return query.ToList();
        }


        public IEnumerable<Model.AssetComponent> GetFilteredDetailUI(string includes, int? assetId, int? employeeId, string sortColumn, string sortDirection, int? page, int? pageSize, out int count)
        {

            IQueryable<Model.AssetComponent> query = null;
            query = _context.AssetComponents.AsNoTracking();

            if (includes != null)
            {
                foreach (var includeProperty in includes.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            else
            {
                query = query
                    .Include(i => i.Asset)
                    .AsQueryable();

            }



            if (assetId.HasValue) query = query.Where(a => a.AssetId == assetId);
            if (employeeId.HasValue) query = query.Where(a => a.EmployeeId == employeeId);




            query = query.Where(op => op.IsDeleted == false && op.EmployeeId == null);

            count = query.Count();

            if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetComponent>(sortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetComponent>(sortColumn));
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }




            return query.ToList();
        }
    }
}
