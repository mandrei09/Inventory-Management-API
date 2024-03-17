using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IErrorsRepository : IRepository<Error>
    {
        int GetCountByFilters(string filter, List<int> errorTypeIds);
        IEnumerable<Error> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> errorTypeIds);
    }
}
