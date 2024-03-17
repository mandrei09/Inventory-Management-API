using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/rates")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class RatesController : GenericApiController<Model.Rate, Dto.Rate>
    {
        public RatesController(ApplicationDbContext context, IRatesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string uomIds, string date, bool showLast, string includes)
        {
            List<Model.Rate> items = null;
            IEnumerable<Dto.Rate> itemsResult = null;
            List<int?> uIds = null;

            includes = "Uom";

            if ((uomIds != null) && (uomIds.Length > 0)) uIds = JsonConvert.DeserializeObject<string[]>(uomIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IRatesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, uIds, date, showLast).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Rate>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IRatesRepository).GetCountByFilters(filter, uIds,date, showLast);
                var pagedResult = new Dto.PagedResult<Dto.Rate>(itemsResult, new Dto.PagingInfo()
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

        [HttpGet("getCurrency/{currency}")]
        public async Task<Model.Rate> GetBnrCurrency(string currency)
        {
            //var dateToday = DateTime.Now.ToString("yyyy-MM-dd");

            //var dateYesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            //var dateYesterdayMinus1 = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");

            //var dateYesterdayMinus2 = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");

            Model.Rate rate = null;

            // rate =  _context.Set<Model.Rate>().Where(a => a.IsDeleted == false && a.Uom.Code == currency && a.Code == dateToday && a.IsDeleted == false).SingleOrDefault();

            rate = _context.Set<Model.Rate>().Where(a => a.IsDeleted == false && a.Uom.Code == currency && a.IsLast == true).SingleOrDefault();

   //         if (rate == null)
			//{
   //             rate = _context.Set<Model.Rate>().Include(u => u.Uom).Include(u => u.AccMonth).Where(a => a.IsDeleted == false && a.Uom.IsDeleted == false && a.Uom.Code == currency && a.Code == dateYesterday && a.AccMonth.Year == DateTime.Now.Year).SingleOrDefault();

   //             if (rate == null)
   //             {
   //                 rate = _context.Set<Model.Rate>().Include(u => u.Uom).Include(u => u.AccMonth).Where(a => a.IsDeleted == false && a.Uom.IsDeleted == false && a.Uom.Code == currency && a.Code == dateYesterdayMinus1 && a.AccMonth.Year == DateTime.Now.Year).SingleOrDefault();

   //                 if (rate == null)
   //                 {
   //                     rate = _context.Set<Model.Rate>().Include(u => u.Uom).Include(u => u.AccMonth).Where(a => a.IsDeleted == false && a.Uom.IsDeleted == false && a.Uom.Code == currency && a.Code == dateYesterdayMinus2 && a.AccMonth.Year == DateTime.Now.Year).SingleOrDefault();
   //                 }
   //             }
   //         }

            if(rate != null)
			{
                return rate;
			}
			else
			{
                return new Model.Rate();
            }
            
        }


        [HttpGet("getRateByDate/{day}")]
        public async Task<Model.Rate> GetRate(string day)
        {

            Model.Rate rate = null;

            rate = await _context.Set<Model.Rate>().Where(a => a.IsDeleted == false && a.Uom.Code == day && a.IsLast == true).SingleOrDefaultAsync();

            if (rate != null)
            {
                return rate;
            }
            else
            {
                return new Model.Rate();
            }

        }
    }
}
