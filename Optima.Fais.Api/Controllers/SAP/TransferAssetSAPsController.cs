using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/transferassetsaps")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class TransferAssetSAPsController : GenericApiController<Model.TransferAssetSAP, Dto.TransferAssetSAP>
    {
        public TransferAssetSAPsController(ApplicationDbContext context, ITransferAssetSAPsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }


        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
        {
            List<Model.TransferAssetSAP> items = null;
            IEnumerable<Dto.TransferAssetSAP> itemsResult = null;

            items = (_itemsRepository as ITransferAssetSAPsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.TransferAssetSAP>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as ITransferAssetSAPsRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.TransferAssetSAP>(itemsResult, new Dto.PagingInfo()
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
    }
}
