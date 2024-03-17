using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
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
using Optima.Fais.Api.Helpers;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.EfRepository;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/budgetmonthbases")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class BudgetMonthBasesController : GenericApiController<Model.BudgetMonthBase, Dto.BudgetMonthBase>
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<Model.ApplicationUser> userManager;

        private readonly IEmailSender _emailSender;

        public BudgetMonthBasesController(ApplicationDbContext context,
            IBudgetMonthBasesRepository itemsRepository, 
            //IRepository<Model.Budget> itemsRepository,
            IMapper mapper, IWebHostEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager, IEmailSender emailSender)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            this._emailSender = emailSender;
        }

		[HttpGet]
		[Route("", Order = -1)]
		public virtual async Task<IActionResult> GetDepDetails(int page, int pageSize, string sortColumn, string sortDirection,
			string includes, string jsonFilter)
		{
			AssetDepTotal depTotal = null;
			AssetCategoryTotal catTotal = null;
			BudgetFilter assetFilter = null;
			Paging paging = null;
			Sorting sorting = null;

			// includes ??= "BudgetBase.AssetType,BudgetBase.ProjectType,BudgetBase.Region,BudgetBase.AdmCenter,BudgetBase.Activity,BudgetBase.Country,BudgetBase.Company,BudgetBase.Employee,BudgetBase.AccMonth,BudgetBase.AppState,";
			//includes = "Company,Project,Administration,CostCenter,SubType.Type.MasterType,SubType.Type,SubType,Employee,AccMonth,InterCompany,Partner,Account,AppState";

			if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
				|| (page <= 0) || (pageSize <= 0))
				return BadRequest();

			paging = new Paging() { Page = page, PageSize = pageSize };
			sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<BudgetFilter>(jsonFilter) : new BudgetFilter();


			includes = includes + "BudgetMonthBase.BudgetBase";

            var items = (_itemsRepository as IBudgetMonthBasesRepository)
               .GetBuget(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();

			var itemsResource = _mapper.Map<List<Model.BudgetMonthBaseDetail>, List<Dto.BudgetMonthBase>>(items);


			var result = new BudgetMonthBasePagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal, catTotal);

			return Ok(result);
		}


    }
}
