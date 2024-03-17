using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IProjectTypeDivisionsRepository : IRepository<ProjectTypeDivision>
    {
        IEnumerable<ProjectTypeDivision> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> projectTypeIds, List<int> divisionIds, List<int?> divIds);
        int GetCountByFilters(string filter, List<int?> projectTypeIds, List<int> divisionIds, List<int?> divIds);
    }
}
