using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    public class GenericApiController<T, V> : Controller where T : class, IEntity
    {
        protected ApplicationDbContext _context = null;
        protected IRepository<T> _itemsRepository = null;
        protected readonly IMapper _mapper;

        public GenericApiController(ApplicationDbContext context,
            IRepository<T> itemsRepository, IMapper mapper)
        {
            _context = context;
            _itemsRepository = itemsRepository;
            _mapper = mapper;
        }

        //[HttpGet]
        //public virtual IActionResult Get()
        //{
        //    return Ok(_itemsRepository.GetAll(null, null, null));
        //}

        [HttpGet]
        [Route("", Order = 0)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter)
        {

            //System.Threading.Thread.Sleep(1500);

            List<T> items = null;
            IEnumerable<V> itemsResult = null;

            items = _itemsRepository.GetByFilter(filter, null, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<V>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = _itemsRepository.GetCountByFilter(filter);
                var pagedResult = new PagedResult<V>(itemsResult, new PagingInfo()
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

        [HttpGet("{id:int}")]
        public virtual IActionResult GetById(int id)
        {
            return Ok(_itemsRepository.GetById(id));
        }


        //[HttpGet]
        //[Route("api/apipaged")]
        //public PagedResult<V> GetFilteredPaged(int? page, int? pageSize, string sortColumn, string sortDirection, string filter)
        //{
        //    return GetPaged(page, pageSize, sortColumn, sortDirection, filter);
        //}

        //protected virtual PagedResult<V> GetPaged(int? page, int? pageSize, string sortColumn, string sortDirection, string filter)
        //{
        //    int totalItems = 0;

        //    List<T> items = _itemsRepository.GetByFilter(filter, null, sortColumn, sortDirection, page, pageSize).ToList();
        //    totalItems = _itemsRepository.GetCountByFilter(filter);

        //    return new PagedResult<V>(items.Select(i => _mapper.Map<V>(i)), new PagingInfo()
        //    {
        //        TotalItems = totalItems,
        //        CurrentPage = page.Value,
        //        PageSize = pageSize.Value
        //    });
        //}

        [HttpPost]
        public async Task<V> Post([FromBody] V vm)
        {
            var item = _mapper.Map<T>(vm);
            _itemsRepository.Create(item);

            //if (_notifyChange) _wsService.NotifyNewData(string.Empty, typeof(T).Name, "INSERT", item);
            //if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
            //_context.SaveChanges();

            // ORIGINAL //

            // MODIFICAT //

            var userId = HttpContext.User.Claims.First(c => c.Type.EndsWith("sub")).Value;

            await _context.SaveChangesAsync(userId);

            // MODIFICAT //

            return _mapper.Map<V>(item);
        }

        [HttpPut]
        public async Task Put([FromBody] V vm)
        {
            var item = _mapper.Map<T>(vm);
            _itemsRepository.Update(item);
            // ORIGINAL //

            //if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
            //_context.SaveChanges();

            // ORIGINAL //

            // MODIFICAT //

            var userId = HttpContext.User.Claims.First(c => c.Type.EndsWith("sub")).Value;

            await _context.SaveChangesAsync(userId);

            // MODIFICAT //

            var updatedItem = this.GetById(item.Id);

            //if (_notifyChange) _wsService.NotifyNewData<V>(string.Empty, typeof(T).Name.ToUpper(), "UPDATE", updatedItem);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            _itemsRepository.Delete(id);
            // ORIGINAL //

            //if (HttpContext != null) _context.UserName = HttpContext.User.Identity.Name;
            //_context.SaveChanges();

            // ORIGINAL //

            // MODIFICAT //

            var userId = HttpContext.User.Claims.First(c => c.Type.EndsWith("sub")).Value;

            await _context.SaveChangesAsync(userId);

            // MODIFICAT //
        }
    }
}
