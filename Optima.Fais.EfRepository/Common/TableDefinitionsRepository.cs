using System.Collections.Generic;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace Optima.Fais.EfRepository
{
    public class TableDefinitionsRepository : Repository<TableDefinition>, ITableDefinitionsRepository
    {
        public TableDefinitionsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter) || a.Description.Contains(filter)); })
        { }

        public async Task<List<TableDefinition>> GetAllIncludingColumnDefinitionsAsync()
        {
            return await _context.Set<TableDefinition>().Include(i => i.ColumnDefinitions).ToListAsync();
        }

        private Expression<Func<Model.TableDefinition, bool>> GetFiltersPredicate(string filter)
        {
            Expression<Func<Model.TableDefinition, bool>> predicate = null;

            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            return predicate;
        }

        public IEnumerable<Model.TableDefinition> GetByFilters(string filter, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter);

            return GetQueryable(predicate, null, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter)
        {
            var predicate = GetFiltersPredicate(filter);

            return GetQueryable(predicate).Count();
        }
    }
}
