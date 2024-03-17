using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/offerops")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class OfferOpsController : GenericApiController<Model.OfferOp, Dto.OfferOp>
    {
        private readonly IWebHostEnvironment hostingEnvironment;

        private readonly UserManager<Model.ApplicationUser> _userManager;

        public OfferOpsController(ApplicationDbContext context, IOfferOpsRepository itemsRepository, IMapper mapper, IWebHostEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        //[HttpGet]
        //[Route("filtered")]
        //public virtual IActionResult GetOperationDetails(string assetOpState, DateTime startDate, DateTime endDate )
        //{
        //    List<Dto.AssetOpSd> items = (_itemsRepository as IAssetOpsRepository).GetFiltered(assetOpState, startDate, endDate).ToList();
        //    return Ok(items);
        //}

        [HttpGet]
        [Route("details")]
        public virtual IActionResult GetOperationDetailsFull(int? page, int? pageSize, string sortColumn, string sortDirection,
            string includes, int? assetId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string jsonFilter)
        {
            AssetFilter assetFilter = null;
            int totalItems = 0;
            string userName = string.Empty;
            string userId = null;
            string employeeId = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + "Offer";


            List<Model.OfferOp> items = (_itemsRepository as IOfferOpsRepository)
                .GetFiltered(assetFilter, includes, assetId, documentTypeCode, assetOpState, startDate, endDate, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var itemsResource = _mapper.Map<List<Model.OfferOp>, List<Dto.OfferOp>>(items);
            var result = new PagedResult<Dto.OfferOp>(itemsResource, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });



            return Ok(result);
        }

    }


}
