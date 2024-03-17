using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/errortypes")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class ErrorTypesController : GenericApiController<Model.ErrorType, Dto.ErrorType>
    {
        public ErrorTypesController(ApplicationDbContext context, IErrorTypesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }


        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
        {
            List<Model.ErrorType> items = null;
            IEnumerable<Dto.ErrorType> itemsResult = null;


            items = (_itemsRepository as IErrorTypesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.ErrorType>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IErrorTypesRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.ErrorType>(itemsResult, new Dto.PagingInfo()
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
