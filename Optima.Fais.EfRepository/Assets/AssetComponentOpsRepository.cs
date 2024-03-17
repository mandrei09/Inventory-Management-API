using System;
using System.Collections.Generic;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Linq;
using Optima.Fais.Model;
using Microsoft.EntityFrameworkCore;
using Optima.Fais.Model.Utils;

namespace Optima.Fais.EfRepository
{
    public class AssetComponentOpsRepository : Repository<Model.AssetComponentOp>, IAssetComponentOpsRepository
    {
        public AssetComponentOpsRepository(ApplicationDbContext context)
            : base(context, null)
        {
        }

        public IEnumerable<Model.AssetComponentOp> GetFiltered(AssetFilter assetFilter, string includes, int? employeeId, string documentTypeCode, string sortColumn, string sortDirection, int? page, int? pageSize, out int count)
        {

            IQueryable<Model.AssetComponentOp> query = null;
            query = _context.AssetComponentOps.AsNoTracking();

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
                    .Include(i => i.AssetComponent)
                    .Include(i => i.Document)
                        .ThenInclude(d => d.DocumentType)
                    .Include(i => i.EmployeeInitial)
                    .Include(i => i.EmployeeFinal)
                    .AsQueryable();
            }

            //if ((assetFilter.EmployeeIds != null) && (assetFilter.EmployeeIds.Count > 0))
            //{
            //    query = query.Where(a => assetFilter.EmployeeIds.Contains(a.EmployeeIdInitial) || assetFilter.EmployeeIds.Contains(a.EmployeeIdFinal));
            //}


          

            if (employeeId.HasValue) query = query.Where(a => a.EmployeeIdFinal == employeeId);

            if (assetFilter.Filter != "" && assetFilter.Filter != null) query = query.Where(a => (a.AssetComponent.Code.Contains(assetFilter.Filter) || a.AssetComponent.Name.Contains(assetFilter.Filter)));

            if ((documentTypeCode != null) && (documentTypeCode.Length > 0))
            {
                Model.DocumentType documentType = _context.Set<DocumentType>().Where(d => d.Code == documentTypeCode).Single();
              
                query = query.Where(a => a.Document.DocumentTypeId == 16 || a.Document.DocumentTypeId == 17);
                                                                                                                                              

            }


            count = query.Count();

            if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetComponentOp>(sortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetComponentOp>(sortColumn));
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }




            return query.ToList();
        }


    }

}
