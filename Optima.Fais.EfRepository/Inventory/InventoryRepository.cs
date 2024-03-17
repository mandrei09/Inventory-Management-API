using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class InventoryRepository : Repository<Model.Inventory>, IInventoryRepository
    {
        public InventoryRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (i) => i.Description.Contains(filter); })
        { }

        public override void Create(Model.Inventory item, string createdBy = null)
        {
            Document document = null;
            DocumentType documentType = null;

            documentType = _context.Set<DocumentType>().Where(d => d.Code == "INVENTORY").Single();
            document = new Document()
            {
                DocNo1 = "-",
                DocNo2 = "-",
                DocumentDate = item.Start.GetValueOrDefault(),
                CreationDate = item.Start.GetValueOrDefault(),
                RegisterDate = item.Start.GetValueOrDefault(),
                DocumentType = documentType
            };
            _context.Set<Document>().Add(document);

            item.CreatedBy = createdBy;
            item.Document = document;
            _context.Set<Model.Inventory>().Add(item);
        }

        private Expression<Func<Model.Inventory, bool>> GetFiltersPredicate(string filter, List<int> accMonthIds)
        {
            Expression<Func<Model.Inventory, bool>> predicate = null;

            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((accMonthIds != null) && (accMonthIds.Count > 0))
            {
                //predicate = predicate != null
                //    ? ExpressionHelper.And<Model.Inventory>(predicate, r => accMonthIds.Contains(r.AccMonthId))
                //    : r => accMonthIds.Contains(r.AccMonthId);
            }

            return predicate;
        }


        public IEnumerable<Model.Inventory> GetSyncDetails(int pageSize, int lastId, System.DateTime lastModifiedAt)
        {
            var query = _context.Set<Model.Inventory>()
                .Where(r => (((r.ModifiedAt == lastModifiedAt) && (r.Id > lastId)) || (r.ModifiedAt > lastModifiedAt)))
                .OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.Inventory>("modifiedAt"))
                .ThenBy(ExpressionHelper.GenericEvaluateOrderBy<Model.Inventory>("id"))
                .Take(pageSize);

            return query.ToList();
        }
        public IEnumerable<Model.Inventory> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> accMonthIds)
        {
            var predicate = GetFiltersPredicate(filter, accMonthIds);

            includes = includes ?? "AccMonth";
            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }



        public int GetCountByFilters(string filter, List<int> accMonthIds)
        {
            var predicate = GetFiltersPredicate(filter, accMonthIds);

            return GetQueryable(predicate).Count();
        }
    }
}
