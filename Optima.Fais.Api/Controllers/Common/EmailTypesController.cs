using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/emailtypes")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class EmailTypesController : GenericApiController<Model.EmailType, Dto.EmailType>
    {
        public EmailTypesController(ApplicationDbContext context, IEmailTypesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
        {
            List<Model.EmailType> items = null;
            IEnumerable<Dto.EmailType> itemsResult = null;


            items = (_itemsRepository as IEmailTypesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.EmailType>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IEmailTypesRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.EmailType>(itemsResult, new Dto.PagingInfo()
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
