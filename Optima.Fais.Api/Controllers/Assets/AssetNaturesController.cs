using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/assetnatures")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AssetNaturesController : GenericApiController<Model.AssetNature, Dto.AssetNature>
    {
        public AssetNaturesController(ApplicationDbContext context, IAssetNaturesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string assetTypeIds, string includes)
        {
            List<Model.AssetNature> items = null;
            IEnumerable<Dto.AssetNature> itemsResult = null;
            List<int?> dIds = null;

            includes = includes ?? "AssetType";

            if (assetTypeIds != null && !assetTypeIds.StartsWith("["))
            {
                assetTypeIds = "[" + assetTypeIds + "]";
            }


            if ((assetTypeIds != null) && (assetTypeIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(assetTypeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IAssetNaturesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, dIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.AssetNature>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IAssetNaturesRepository).GetCountByFilters(filter, dIds);
                var pagedResult = new Dto.PagedResult<Dto.AssetNature>(itemsResult, new Dto.PagingInfo()
                {
                    TotalItems = totalItems,
                    CurrentPage = page.Value,
                    PageSize = pageSize.Value
                });
                return Ok(pagedResult);
            }
            else
            {
                return Ok(itemsResult);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sync")]
        public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.AssetNature> items = (_itemsRepository as IAssetNaturesRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<AssetNatureSync>(i));
            var pagedResult = new Dto.PagedResult<AssetNatureSync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }
    }
}
