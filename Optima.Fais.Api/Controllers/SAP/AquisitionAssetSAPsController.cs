using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model.Utils;
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
    [Route("api/acquisitionassetsaps")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AcquisitionAssetSAPsController : GenericApiController<Model.AcquisitionAssetSAP, Dto.AcquisitionAssetSAP>
    {
        public AcquisitionAssetSAPsController(ApplicationDbContext context, IAquisitionAssetSAPsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes, string jsonFilter, bool isTesting = false)
        {
            List<Model.AcquisitionAssetSAP> items = null;
            IEnumerable<Dto.AcquisitionAssetSAPView> itemsResult = null;
			AssetReceptionFilter assetFilter = null;
			decimal totalAmount = 0;
			decimal totalTax = 0;

			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetReceptionFilter>(jsonFilter) : new AssetReceptionFilter();

			items = (_itemsRepository as IAquisitionAssetSAPsRepository).GetByFilters(assetFilter, filter, includes, sortColumn, sortDirection, page, pageSize, isTesting).ToList();

			totalAmount = items.Sum(a => a.NET_AMOUNT);
			totalTax = items.Sum(a => a.TAX_AMOUNT);

			itemsResult = items.Select(i => _mapper.Map<Dto.AcquisitionAssetSAPView>(i));

			
			if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IAquisitionAssetSAPsRepository).GetCountByFilters(assetFilter, filter, isTesting);
                var pagedResult = new Dto.PagedResult<Dto.AcquisitionAssetSAPView>(itemsResult, new Dto.PagingInfo()
                {
                    TotalItems = totalItems,
                    CurrentPage = page.Value,
                    PageSize = pageSize.Value,
                    TotalNetAmount = totalAmount,
                    TotalTaxAmount = totalTax,
                    ShowFooter = true
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
