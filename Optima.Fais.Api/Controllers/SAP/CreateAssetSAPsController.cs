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
    [Route("api/createassetsaps")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class CreateAssetSAPsController : GenericApiController<Model.CreateAssetSAP, Dto.CreateAssetSAPDTO>
    {
        public CreateAssetSAPsController(ApplicationDbContext context, ICreateAssetSAPsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }


        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
        {
            List<Model.CreateAssetSAP> items = null;
            IEnumerable<Dto.CreateAssetSAPDTO> itemsResult = null;

            includes = includes ?? "Error,Asset";

            items = (_itemsRepository as ICreateAssetSAPsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.CreateAssetSAPDTO>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as ICreateAssetSAPsRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.CreateAssetSAPDTO>(itemsResult, new Dto.PagingInfo()
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
