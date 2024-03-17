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
    [Route("api/assetcomponentops")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AssetComponentOpsController : GenericApiController<Model.AssetComponentOp, Dto.AssetComponentOp>
    {
        private readonly IHostingEnvironment hostingEnvironment;

        private readonly UserManager<Model.ApplicationUser> _userManager;

        public AssetComponentOpsController(ApplicationDbContext context, IAssetComponentOpsRepository itemsRepository, IMapper mapper, IHostingEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }


        [HttpGet]
        [Route("details")]
        public virtual IActionResult GetOperationDetailsFull(int? page, int? pageSize, string sortColumn, string sortDirection,
            string includes, int? employeeId, string documentTypeCode, string jsonFilter)
        {
            AssetFilter assetFilter = null;
            int totalItems = 0;
            string userName = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();
            includes = includes + "AssetComponent";

          
            List<Model.AssetComponentOp> items = (_itemsRepository as IAssetComponentOpsRepository)
                .GetFiltered(assetFilter, includes, employeeId, documentTypeCode, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var itemsResource = _mapper.Map<List<Model.AssetComponentOp>, List<Dto.AssetComponentOp>>(items);
            var result = new PagedResult<Dto.AssetComponentOp>(itemsResource, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });



            return Ok(result);
        }

        [HttpDelete("remove/{assetComponentOpId}")]
        public virtual IActionResult DeleteAsset(int assetComponentOpId)
        {

            var assetComponentOp = _context.Set<Model.AssetComponentOp>().Where(a => a.Id == assetComponentOpId).Single();

            if (assetComponentOp != null)
            {
                assetComponentOp.IsDeleted = true;
                _context.Update(assetComponentOp);
                _context.SaveChanges();
            }
            else
            {
                return Ok(StatusCode(404));
            }

            return Ok(StatusCode(200));
        }



    }


}
