using System;
using System.Collections.Generic;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System.Linq;

namespace Optima.Fais.EfRepository
{
    public class RegionsRepository : Repository<Region>, IRegionsRepository
    {
        public RegionsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        public IEnumerable<Region> GetSyncDetails(int pageSize, int lastId, DateTime lastModifiedAt)
        {
            var query = _context.Set<Model.Region>()
                .Where(r => (((r.ModifiedAt == lastModifiedAt) && (r.Id > lastId)) || (r.ModifiedAt > lastModifiedAt)))
                .OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.Region>("modifiedAt"))
                .ThenBy(ExpressionHelper.GenericEvaluateOrderBy<Model.Region>("id"))
                .Take(pageSize);

            return query.ToList();
        }
    }
}
