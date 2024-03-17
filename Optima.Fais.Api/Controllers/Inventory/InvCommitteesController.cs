using AutoMapper;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using Optima.Fais.Repository.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace Optima.Fais.Api.Controllers.Inventory
{
    [Route("api/invcommittee")]
    public partial class InvCommitteesController : GenericApiController<Model.InvCommittee, Dto.InvCommittee>
    {
        public InvCommitteesController(ApplicationDbContext context, IInvCommitteesRepository itemsRepository,
            IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }



    }
}
