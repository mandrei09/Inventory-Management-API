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
using Optima.Fais.Model;
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
    [Route("api/requestops")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class RequestOpsController : GenericApiController<Model.RequestOp, Dto.RequestOp>
    {
        private readonly IWebHostEnvironment hostingEnvironment;

        private readonly UserManager<Model.ApplicationUser> _userManager;

        public RequestOpsController(ApplicationDbContext context, IRequestOpsRepository itemsRepository, IMapper mapper, IWebHostEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager)
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
            string includes, int? requestId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string jsonFilter)
        {
            RequestFilter assetFilter = null;
            int totalItems = 0;
            string userName = string.Empty;
            string userId = null;
            string employeeId = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<RequestFilter>(jsonFilter) : new RequestFilter();

            includes = includes + "Request";


            List<Model.RequestOp> items = (_itemsRepository as IRequestOpsRepository)
                .GetFiltered(assetFilter, includes, requestId, documentTypeCode, assetOpState, startDate, endDate, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var itemsResource = _mapper.Map<List<Model.RequestOp>, List<Dto.RequestOp>>(items);
            var result = new PagedResult<Dto.RequestOp>(itemsResource, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });



            return Ok(result);
        }

    }


}
