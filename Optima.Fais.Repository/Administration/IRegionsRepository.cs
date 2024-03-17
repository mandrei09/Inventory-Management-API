using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IRegionsRepository : IRepository<Region>
    {
        IEnumerable<Region> GetSyncDetails(int pageSize, int lastId, DateTime lastModifiedAt);
    }
}
