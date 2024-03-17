using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/assetstates")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AssetStatesController : GenericApiController<Model.AssetState, Dto.AssetState>
    {
        public AssetStatesController(ApplicationDbContext context, IAssetStatesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("sync")]
        public virtual IActionResult GetSyncDetails(int pageSize, int lastId, DateTime lastModifiedAt)
        {
            List<Model.AssetState> items = (_itemsRepository as IAssetStatesRepository).GetSyncDetails(pageSize, lastId, lastModifiedAt).ToList();

            return Ok(items.Select(i => _mapper.Map<Dto.AssetState>(i)));
        }
    }
}
