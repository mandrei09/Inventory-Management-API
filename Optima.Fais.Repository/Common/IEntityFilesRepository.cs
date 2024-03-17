using System;
using System.Collections.Generic;
using Optima.Fais.Model;

namespace Optima.Fais.Repository
{
    public interface IEntityFilesRepository : IRepository<EntityFile>
    {
        IEnumerable<Model.EntityFile> GetByEntity(string entityTypeCode, int entityId, Guid? guid, int? partnerId);
    }
}
