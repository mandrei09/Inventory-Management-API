using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IOffersRepository : IRepository<Model.Offer>
    {

        IEnumerable<Model.OfferDetail> GetOffer(OfferFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        IEnumerable<Model.OfferDetail> GetOfferUI(OfferFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        IEnumerable<Model.OfferDetail> GetAllOfferUI(OfferFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        Task<Model.OfferResult> CreateOrUpdateOffer(OfferSave asset);
        Model.Offer GetDetailsById(int id, string includes);
        Task<Model.EmailResult> SendEmail(int offerId, CodePartnerEntity partner);
        int SendValidatedEmail(int budgetId, string userName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut);

        IEnumerable<Model.OfferDetail> BudgetValidate(OfferFilter budgetFilter, string includes, string userId, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
    }
}
