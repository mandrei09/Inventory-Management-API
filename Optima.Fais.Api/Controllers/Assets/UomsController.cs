using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using Optima.Fais.Repository;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/uoms")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class UomsController : GenericApiController<Model.Uom, Dto.Uom>
    {
        public UomsController(ApplicationDbContext context, IUomsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }
    }
}
