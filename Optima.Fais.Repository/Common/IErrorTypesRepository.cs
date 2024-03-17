using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IErrorTypesRepository : IRepository<ErrorType>
    {
        int GetCountByFilters(string filter);
        IEnumerable<ErrorType> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
