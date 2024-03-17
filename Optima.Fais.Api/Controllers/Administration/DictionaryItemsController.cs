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
    [Authorize]
    [Route("api/dictionaryitems")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class DictionaryItemsController : GenericApiController<Model.DictionaryItem, Dto.DictionaryItem>
    {
        public DictionaryItemsController(ApplicationDbContext context, IDictionaryItemsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string dictionaryTypeIds, string assetCategoryIds, string includes, bool showWFH = false)
        {
            List<Model.DictionaryItem> items = null;
            IEnumerable<Dto.DictionaryItem> itemsResult = null;
            List<int?> dIds = null;
            List<int?> aIds = null;

            includes = "DictionaryType,AssetCategory";

            if ((dictionaryTypeIds != null) && (dictionaryTypeIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(dictionaryTypeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((assetCategoryIds != null) && (assetCategoryIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(assetCategoryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IDictionaryItemsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, dIds, aIds, showWFH).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.DictionaryItem>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IDictionaryItemsRepository).GetCountByFilters(filter, dIds, aIds, showWFH);
                var pagedResult = new Dto.PagedResult<Dto.DictionaryItem>(itemsResult, new Dto.PagingInfo()
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
        [Route("allowAnonymous", Order = -1)]
        public virtual IActionResult GetAllowAnonymous(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string dictionaryTypeIds, string assetCategoryIds, string includes)
        {
            List<Model.DictionaryItem> items = null;
            IEnumerable<Dto.DictionaryItem> itemsResult = null;
            List<int?> dIds = null;
            List<int?> aIds = null;

            includes = "DictionaryType,AssetCategory";

            // List<int> dictIds = _context.Set<Model.DictionaryItem>().Where(a => a.Id == 466 || a.Id == 364 || a.Id == 334 || a.Id == 337 || a.Id == 363).Select(a => a.Id).ToList();

            if ((dictionaryTypeIds != null) && (dictionaryTypeIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(dictionaryTypeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((assetCategoryIds != null) && (assetCategoryIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(assetCategoryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IDictionaryItemsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, dIds, aIds, false).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.DictionaryItem>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IDictionaryItemsRepository).GetCountByFilters(filter, dIds, aIds, false);
                var pagedResult = new Dto.PagedResult<Dto.DictionaryItem>(itemsResult, new Dto.PagingInfo()
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
            List<Model.DictionaryItem> items = (_itemsRepository as IDictionaryItemsRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<DictionaryItemSync>(i));
            var pagedResult = new Dto.PagedResult<DictionaryItemSync>(itemsResult, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }
    }
}
