using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAdmCentersRepository : IRepository<AdmCenter>
    {
        int GetCountByFilters(string filter, List<int> employeeIds);
        IEnumerable<AdmCenter> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> employeeIds);
    }
}
