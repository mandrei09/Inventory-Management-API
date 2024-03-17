using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IRequestBFMaterialCostCentersRepository : IRepository<RequestBFMaterialCostCenter>
    {
        IEnumerable<RequestBFMaterialCostCenter> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> materialIds, List<int?> budgetForecastMaterialIds, int? orderId, bool reception);
        int GetCountByFilters(string filter, List<int?> materialIds, List<int?> budgetForecastMaterialIds, int? orderId, bool reception);

        IEnumerable<RequestBFMaterialCostCenter> GetByReceptionFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> orderIds, bool reception);
        int GetCountByReceptionFilters(string filter, List<int?> orderIds, bool reception);
    }
}
