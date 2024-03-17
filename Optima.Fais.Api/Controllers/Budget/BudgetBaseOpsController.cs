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
    [Route("api/budgetbaseops")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class BudgetBaseOpsController : GenericApiController<Model.BudgetBaseOp, Dto.BudgetBaseOp>
    {
        private readonly IWebHostEnvironment hostingEnvironment;

        private readonly UserManager<Model.ApplicationUser> _userManager;

        public BudgetBaseOpsController(ApplicationDbContext context, IBudgetBaseOpsRepository itemsRepository, IMapper mapper, IWebHostEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager)
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
            string includes, int? budgetForecastId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string jsonFilter)
        {
            AssetFilter assetFilter = null;
            int totalItems = 0;
            string userName = string.Empty;
            string userId = null;
            string employeeId = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + "BudgetForecast.BudgetBase";


            List<Model.BudgetBaseOp> items = (_itemsRepository as IBudgetBaseOpsRepository)
                .GetFiltered(assetFilter, includes, budgetForecastId, documentTypeCode, assetOpState, startDate, endDate, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var itemsResource = _mapper.Map<List<Model.BudgetBaseOp>, List<Dto.BudgetBaseOp>>(items);
            var result = new PagedResult<Dto.BudgetBaseOp>(itemsResource, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });



            return Ok(result);
        }

		[HttpGet]
		[Route("validationdetails")]
		public virtual IActionResult GetValidationDetailsFull(int? page, int? pageSize, string sortColumn, string sortDirection,
		   string includes, int? budgetForecastId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string jsonFilter)
		{
			AssetFilter assetFilter = null;
			int totalItems = 0;
			string userName = string.Empty;
			string userId = null;
			string employeeId = string.Empty;

			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

			includes = includes + "BudgetForecast.BudgetBase";


			List<Model.BudgetBaseOp> items = (_itemsRepository as IBudgetBaseOpsRepository)
				.GetValidationFiltered(assetFilter, includes, budgetForecastId, documentTypeCode, assetOpState, startDate, endDate, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

			var itemsResource = _mapper.Map<List<Model.BudgetBaseOp>, List<Dto.BudgetBaseOp>>(items);
			var result = new PagedResult<Dto.BudgetBaseOp>(itemsResource, new PagingInfo()
			{
				TotalItems = totalItems,
				CurrentPage = page.Value,
				PageSize = pageSize.Value
			});



			return Ok(result);
		}

	}


}
