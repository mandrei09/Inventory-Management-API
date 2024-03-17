using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IOfferTypesRepository : IRepository<OfferType>
    {
        IEnumerable<OfferType> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        int GetCountByFilters(string filter);
    }
}
