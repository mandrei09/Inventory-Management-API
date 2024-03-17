using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IOfferMaterialsRepository : IRepository<OfferMaterial>
    {
        Task<List<OfferMaterial>> GetAllOfferMaterialsByOfferId(int? offerId);
        IEnumerable<OfferMaterial> GetByFilters(string filter, Guid guid, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> offerIds, List<int?> materialIds, List<int?> requestIds, List<int?> subCategoryIds, List<int?> partnerIds);
        int GetCountByFilters(string filter, Guid guid, List<int?> offerIds, List<int?> materialIds, List<int?> requestIds, List<int?> subCategoryIds, List<int?> partnerIds);
    }
}
