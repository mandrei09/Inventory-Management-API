using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using Optima.Fais.Repository;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/partnerlocations")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class PartnerLocationsController : GenericApiController<Model.PartnerLocation, Dto.PartnerLocation>
    {
        public PartnerLocationsController(ApplicationDbContext context, IPartnerLocationsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }
    }
}
