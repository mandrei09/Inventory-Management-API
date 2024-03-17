using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IPrintLabelsRepository : IRepository<PrintLabel>
    {
        int GetCountByFilters(string filter);
        IEnumerable<PrintLabel> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
