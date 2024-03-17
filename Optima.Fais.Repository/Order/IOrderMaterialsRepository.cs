using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IOrderMaterialsRepository : IRepository<OrderMaterial>
    {
        Task<List<OrderMaterial>> GetAllOrderMaterialsByOrderId(int? orderID);
        IEnumerable<OrderMaterial> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> orderIds, List<int?> materialIds, List<int?> subCategoryIds);
        int GetCountByFilters(string filter, List<int?> orderIds, List<int?> materialIds, List<int?> subCategoryIds);
    }
}
