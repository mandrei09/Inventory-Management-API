using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IInvStatesRepository : IRepository<InvState>
    {
        IEnumerable<Model.InvState> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
        IEnumerable<Model.InvState> GetInvStatesInUseWithAssets(AssetFilter assetFilter, List<PropertyFilter> propFilters);
    }
}
