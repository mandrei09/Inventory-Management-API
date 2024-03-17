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
    public class ContractOpsRepository : Repository<Model.ContractOp>, IContractOpsRepository
    {
        public ContractOpsRepository(ApplicationDbContext context)
            : base(context, null)
        {
        }

        public override void Create(ContractOp item, string createdBy = null)
        {
            base.Create(item, createdBy);
        }

        public IEnumerable<Model.ContractOp> GetFiltered(ContractFilter assetFilter, string includes, int? assetId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string sortColumn, string sortDirection, int? page, int? pageSize, out int count)
        {

            IQueryable<Model.ContractOp> query = null;
            query = _context.ContractOps.AsNoTracking();

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
                    .Include(i => i.Contract)
                    .Include(i => i.Document)
                        .ThenInclude(d => d.DocumentType)
                    .Include(i => i.Employee)
                    .AsQueryable();
            }

            if ((assetFilter.EmployeeIds != null) && (assetFilter.EmployeeIds.Count > 0))
            {
                query = query.Where(a => assetFilter.EmployeeIds.Contains(a.EmployeeId));
            }

            if (assetId.HasValue) query = query.Where(a => a.ContractId == assetId);

            //if (assetOpState != null) query = query.Where(a => a.AssetOpState.Name == assetOpState);

            //if (assetOpState != null)
            //{
            //    switch (assetOpState)
            //    {
            //        case "All":
            //            query = query.Where(a => a.AssetOpStateId == 3 || a.AssetOpStateId == 4 || a.AssetOpStateId == 5);
            //            break;
            //        case "Waiting":
            //            query = query.Where(a => a.AssetOpStateId == 3);
            //            break;
            //        case "Accepted":
            //            query = query.Where(a => a.AssetOpStateId == 4 || a.AssetOpStateId == 5);
            //            break;
            //        default:
            //            //query = query.Where(a => a.AssetOpState.Id > 0);
            //            break;
            //    }
            //}


            if (assetFilter.Filter != "" && assetFilter.Filter != null) query = query.Where(a => (a.Contract.Code.Contains(assetFilter.Filter) || a.Contract.Name.Contains(assetFilter.Filter)));

            if ((documentTypeCode != null) && (documentTypeCode.Length > 0))
            {
                Model.DocumentType documentType = _context.Set<DocumentType>().Where(d => d.Code == documentTypeCode).Single();
                query = query.Where(a => a.Document.DocumentTypeId == 2 || a.Document.DocumentTypeId == 6 || a.Document.DocumentTypeId == 11);  // BNR

            }

            count = query.Count();

            if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.ContractOp>(sortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.ContractOp>(sortColumn));
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }




            return query.ToList();
        }

    }

}
