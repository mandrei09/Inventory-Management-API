using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class AssetStatesRepository : Repository<AssetState>, IAssetStatesRepository
    {
        public AssetStatesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (c) => (c.Code.Contains(filter) || c.Name.Contains(filter)); })
        { }

        public IEnumerable<AssetState> GetSyncDetails(int pageSize, int lastId, DateTime lastModifiedAt)
        {
            var query = _context.Set<Model.AssetState>()
                .Where(r => (((r.ModifiedAt == lastModifiedAt) && (r.Id > lastId)) || (r.ModifiedAt > lastModifiedAt)))
                .OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetState>("modifiedAt"))
                .ThenBy(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetState>("id"))
                .Take(pageSize);

            return query.ToList();
        }
    }
}
