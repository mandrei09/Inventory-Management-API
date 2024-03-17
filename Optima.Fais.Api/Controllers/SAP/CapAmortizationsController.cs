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
    [Route("api/capamortizations")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class CapAmortizationsController : GenericApiController<Model.AssetDepMDCapSync, Dto.AssetDepMDCapDTOSync>
    {
        public CapAmortizationsController(ApplicationDbContext context, ICapAmortizationsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }


        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
        {
            List<Model.AssetDepMDCapSync> items = null;
            IEnumerable<Dto.AssetDepMDCapDTOSync> itemsResult = null;

            includes = includes + "AccMonth,BudgetManager";


            items = (_itemsRepository as ICapAmortizationsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.AssetDepMDCapDTOSync>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as ICapAmortizationsRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.AssetDepMDCapDTOSync>(itemsResult, new Dto.PagingInfo()
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
        [HttpPost("erpcapdatasync")]
        public async Task<IActionResult> SyncDataFromSAP([FromBody] Dto.AssetDepMDCapSync capData)
        {
            Model.AssetDepMDCapSync assetDepMDCapSync = null;
            Model.AccMonth accMonth = null;
            Model.BudgetManager budgetManager = null;

            if(capData != null)
			{
                accMonth = await _context.Set<Model.AccMonth>().AsNoTracking().Where(a => a.IsActive == true).SingleAsync();
                budgetManager = await _context.Set<Model.BudgetManager>().AsNoTracking().Where(a => a.Name == "2022").SingleAsync();

                for (int i = 0; i < capData.Tt_Item.Count; i++)
				{
                    assetDepMDCapSync = new Model.AssetDepMDCapSync()
                    {
                        BUKRSH = capData.Header.BUKRS,
                        BLDAT = capData.Header.BLDAT,
                        BUDAT = capData.Header.BUDAT,
                        SGTXT = capData.Header.SGTXT,
                        XBLNR = capData.Header.XBLNR,
                        TCODE = capData.Header.TCODE,
                        ANLN1 = capData.Tt_Item[i].ANLN1,
                        ANLN2 = capData.Tt_Item[i].ANLN2,
                        ANBTR = capData.Tt_Item[i].ANBTR,
                        ANLKL = capData.Tt_Item[i].FA_MD.ANLKL,
                        BUKRST = capData.Tt_Item[i].FA_MD.BUKRS,
                        TXT50 = capData.Tt_Item[i].FA_MD.TXT50,
                        TXA50 = capData.Tt_Item[i].FA_MD.TXA50,
                        SERNR = capData.Tt_Item[i].FA_MD.SERNR,
                        AKTIV = capData.Tt_Item[i].FA_MD.AKTIV,
                        KOSTL = capData.Tt_Item[i].FA_MD.KOSTL,
                        CAUFN = capData.Tt_Item[i].FA_MD.CAUFN,
                        KOSTLV = capData.Tt_Item[i].FA_MD.KOSTLV,
                        WERKS = capData.Tt_Item[i].FA_MD.WERKS,
                        STORT = capData.Tt_Item[i].FA_MD.STORT,
                        KFZKZ = capData.Tt_Item[i].FA_MD.KFZKZ,
                        ZZCLAS = capData.Tt_Item[i].FA_MD.ZZCLAS,
                        XSTIL = capData.Tt_Item[i].FA_MD.XSTIL,
                        AccMonthId = accMonth.Id,
                        BudgetManagerId = budgetManager.Id

                    };

                    _context.Add(assetDepMDCapSync);
                    _context.SaveChanges();

                }
                

            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors) .Where(y => y.Count > 0) .ToList();

                List<string> err = new List<string>();

                for (int i = 0; i < errors.Count; i++)
                {
                    err.Add(errors[i][0].Exception.Message);
                }

                return Ok(err);
            }

            return Ok(StatusCode(200));
        }
    }
}
