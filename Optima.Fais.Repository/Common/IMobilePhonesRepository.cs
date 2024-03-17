using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IMobilePhonesRepository : IRepository<MobilePhone>
    {
        int GetCountByFilters(string filter);
        IEnumerable<MobilePhone> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        Task<Model.ImportITModelResult> Import(Dto.ImportMobilePhone import);
    }
}
