using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAssetStatesRepository : IRepository<AssetState>
    {
        IEnumerable<AssetState> GetSyncDetails(int pageSize, int lastId, DateTime lastModifiedAt);
    }
}
