using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/offermaterials")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class OfferMaterialsController : GenericApiController<Model.OfferMaterial, Dto.OfferMaterial>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public OfferMaterialsController(ApplicationDbContext context, IOfferMaterialsRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public async virtual Task<IActionResult> Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string offerIds, Guid guid, string materialIds, string requestIds, string subCategoryIds, string partnerIds, bool readOnly, string includes, bool editPanel = false)
        {
            List<Model.OfferMaterial> items = null;
            IEnumerable<Dto.OfferMaterial> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;
            List<int?> catIds = null;
            List<int?> pIds = null;
			List<int?> reqIds = null;

			var userName = HttpContext.User.Identity.Name;
			var user = await _userManager.FindByEmailAsync(userName);
			if (user == null)
			{
				user = await _userManager.FindByNameAsync(userName);
			}

#if DEBUG
            user.Email = "bogdan.pirvulescu@emag.ro";
#endif
			includes = "Offer.AppState,Material,AppState,Rate.Uom";


			if (offerIds != null && !offerIds.StartsWith("["))
			{
                offerIds = "[" + offerIds + "]";
			}

            if (offerIds == null && !editPanel)
            {
                offerIds = "[-1]";
            }

            if (materialIds != null && !materialIds.StartsWith("["))
			{
                materialIds = "[" + materialIds + "]";
			}

			if (requestIds != null && !requestIds.StartsWith("["))
			{
				requestIds = "[" + requestIds + "]";
			}

			if (subCategoryIds != null && !subCategoryIds.StartsWith("["))
            {
                subCategoryIds = "[" + subCategoryIds + "]";
            }

            if (partnerIds != null && !partnerIds.StartsWith("["))
            {
                partnerIds = "[" + partnerIds + "]";
            }

            if ((offerIds != null) && (offerIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(offerIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((materialIds != null) && (materialIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(materialIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((requestIds != null) && (requestIds.Length > 0)) reqIds = JsonConvert.DeserializeObject<string[]>(requestIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
			if ((subCategoryIds != null) && (subCategoryIds.Length > 0)) catIds = JsonConvert.DeserializeObject<string[]>(subCategoryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((partnerIds != null) && (partnerIds.Length > 0)) pIds = JsonConvert.DeserializeObject<string[]>(partnerIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IOfferMaterialsRepository).GetByFilters(filter, guid, includes, sortColumn, sortDirection, page, pageSize, cIds, eIds,reqIds, catIds, pIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.OfferMaterial>(i));

            if (user != null && user.Email != "bogdan.pirvulescu@emag.ro" && readOnly && itemsResult.Any())
            {
                itemsResult = itemsResult.Where(c => c.ReadOnly == false).Select(c => { c.ReadOnly = true; return c; });
            }



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IOfferMaterialsRepository).GetCountByFilters(filter, guid, cIds, eIds, reqIds, catIds, pIds);
                var pagedResult = new Dto.PagedResult<Dto.OfferMaterial>(itemsResult, new Dto.PagingInfo()
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

        [HttpPost]
        [Route("addFromAsset")]
        public async virtual Task<IActionResult> AddOfferMaterialFromAsset([FromBody] Dto.OfferMaterialAdd offerMatAdd)
        {
            Model.OfferMaterial offerMaterial = null;
            Model.Offer offer = null;
            Model.Rate rate = null;

            offer = await _context.Set<Model.Offer>().Where(u => u.Id == offerMatAdd.OfferId).FirstOrDefaultAsync();

			rate = new Model.Rate()
			{
				AccMonthId = 50,
				Code = "",
				CompanyId = null,
				CreatedAt = DateTime.Now,
				CreatedBy = _context.UserId,
				ModifiedAt = DateTime.Now,
				ModifiedBy = _context.UserId,
				Name = "",
				UomId = 358,
				Value = 0,
				IsLast = false,
				Multiplier = 1
			};

			_context.Add(rate);

			//rate = await _context.Set<Model.Rate>().Where(u => u.Id == offerMatAdd.RateId).FirstOrDefaultAsync();

            //if (rate == null)
            //{
            //    rate = await _context.Set<Model.Rate>().Where(u => u.Id == offer.RateId).FirstOrDefaultAsync();
            //    offerMatAdd.RateId = rate.Id;
            //}

            if (offerMatAdd.EmailManagerId == 0)
            {
                offerMaterial = await _context.Set<Model.OfferMaterial>().AsNoTracking().Where(o => o.OfferId == offerMatAdd.OfferId && o.IsDeleted == false && o.RateId == rate.Id).FirstOrDefaultAsync();
                offerMatAdd.EmailManagerId = offerMaterial.EmailManagerId;
            }

            for (int i = 0; i < offerMatAdd.MaterialIds.Length; i++)
            {
                var material = await _context.Set<Model.Material>().Where(u => u.Id == offerMatAdd.MaterialIds[i]).FirstOrDefaultAsync();


                offerMaterial = new Model.OfferMaterial()
                {
                    OfferId = offerMatAdd.OfferId,
                    MaterialId = offerMatAdd.MaterialIds[i],
                    EmailManagerId = offerMatAdd.EmailManagerId,
                    AppStateId = 6,
                    //Value = material.Price * material.Quantity,
                    //Price = material.Price,
                    //Quantity = material.Quantity,
                    Value = 0,
                    Price = 0,
                    Quantity = 0,
                    RequestId = offer.RequestId,
                    Rate = rate
                };

                _context.Add(offerMaterial);

                _context.SaveChanges();
            }

            offer.Rate = rate;
            offer.UomId = rate.UomId;

            _context.Update(offer);
            _context.SaveChanges();

            return Ok(StatusCode(200));
        }

        [HttpPost]
        [Route("add")]
        public async virtual Task<IActionResult> AddOfferMaterial([FromBody] Dto.OfferMaterialAdd offerMatAdd)
        {
            Model.OfferMaterial offerMaterial = null;
            Model.Offer offer = null;
            Model.Rate rate = null;
            Model.AccMonth accMonth = null;

            var userName = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(userName);
            }
            _context.UserId = user.Id.ToString();

            offer = await _context.Set<Model.Offer>().Where(u => u.Id == offerMatAdd.OfferId).FirstOrDefaultAsync();

            accMonth = await _context.Set<Model.AccMonth>().Where(u => u.IsActive == true).FirstOrDefaultAsync();

			rate = new Model.Rate()
			{
				AccMonthId = accMonth.Id,
				Code = "",
				CompanyId = null,
				CreatedAt = DateTime.Now,
				CreatedBy = _context.UserId,
				ModifiedAt = DateTime.Now,
				ModifiedBy = _context.UserId,
				Name = "",
				UomId = 358,
				Value = 1,
				IsLast = false,
				Multiplier = 1
			};

			_context.Add(rate);

			//rate = await _context.Set<Model.Rate>().Where(u => u.Id == offerMatAdd.RateId).FirstOrDefaultAsync();

            for (int i = 0; i < offerMatAdd.MaterialIds.Length; i++)
            {
                var material = await _context.Set<Model.Material>().Where(u => u.Id == offerMatAdd.MaterialIds[i]).FirstOrDefaultAsync();


                offerMaterial = new Model.OfferMaterial()
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserId,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = _context.UserId,
                    OfferId = offerMatAdd.OfferId,
                    MaterialId = offerMatAdd.MaterialIds[i],
                    EmailManagerId = offerMatAdd.EmailManagerId,
                    AppStateId = 6,
                    //Value = material.Price * material.Quantity,
                    //Price = material.Price,
                    //Quantity = material.Quantity,
                    Value = 0,
                    Price = 0,
                    Quantity = 1,
                    RequestId = offer.RequestId,
                    Rate = rate
                };

                _context.Add(offerMaterial);

                _context.SaveChanges();
            }

            offer.Rate = rate;
            offer.UomId = rate.UomId;

            _context.Update(offer);
            _context.SaveChanges();

            return Ok(StatusCode(200));
        }

        [HttpDelete("remove/{id}")]
        public async virtual Task<IActionResult> DeleteOfferMaterial(int id)
        {
            var userName = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(userName);
            }
            _context.UserId = user.Id.ToString();

            Model.OfferMaterial offerMaterial = _context.Set<Model.OfferMaterial>().Where(a => a.Id == id).Single();

            if (offerMaterial != null)
            {
                offerMaterial.ModifiedBy = _context.UserId;
                offerMaterial.IsDeleted = true;
                offerMaterial.ModifiedAt = DateTime.Now;
                _context.Update(offerMaterial);
                _context.SaveChanges();
            }
            else
            {
                return Ok(StatusCode(404));
            }

            return Ok(StatusCode(200));
        }

        [HttpPost]
        [Route("addByOrder")]
        public async virtual Task<IActionResult> AddOfferMaterialByOrder([FromBody] Dto.OfferITMaterialAdd offerMatAdd)
        {
            Model.OfferMaterial offerMaterial = null;
            Model.Offer offer = null;
            Model.Rate rate = null;

            Model.OfferOp offerOp = null;
            Model.Document document = null;
            Model.DocumentType documentType = null;
            Model.EntityType entityType = null;
            Model.Request request = null;
            Model.Inventory inventory = null;
            Model.BudgetManager budgetManager = null;
            Model.AppState appState = null;

            var userName = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(userName);
            }
            _context.UserId = user.Id.ToString();

            if(offerMatAdd.OfferId > 0)
			{
                offer = _context.Set<Model.Offer>().AsNoTracking().Where(d => d.Id == offerMatAdd.OfferId).SingleOrDefault();
			}
			else
			{
                inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).FirstOrDefault();
                appState = _context.Set<Model.AppState>().Where(c => c.Code == "NEW").Single();
                entityType = _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWOFFER").FirstOrDefault();
                budgetManager = _context.Set<Model.BudgetManager>().Where(c => c.Code == "2023").Single();
                documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_OFFER").SingleOrDefault();

                var lastCode = int.Parse(entityType.Name);
                var newBudgetCode = string.Empty;

                if (lastCode.ToString().Length == 1)
                {
                    newBudgetCode = "OFFER000000" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 2)
                {
                    newBudgetCode = "OFFER00000" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 3)
                {
                    newBudgetCode = "OFFER0000" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 4)
                {
                    newBudgetCode = "OFFER000" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 5)
                {
                    newBudgetCode = "OFFER00" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 6)
                {
                    newBudgetCode = "OFFER0" + entityType.Name;
                }
                else if (lastCode.ToString().Length == 7)
                {
                    newBudgetCode = "OFFER" + entityType.Name;
                }

                request = _context.Set<Model.Request>().AsNoTracking().Where(d => d.Id == 282).SingleOrDefault();

                offer = new Model.Offer()
                {
                    AccMonthId = inventory.AccMonthId,
                    //AccountId = budgetDto.AccountId,
                    //AdministrationId = budgetDto.AdministrationId,
                    AppStateId = appState.Id,
                    BudgetManagerId = budgetManager.Id,
                    Code = newBudgetCode,
                    CompanyId = request.CompanyId,

                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserId,
                    EmployeeId = request.EmployeeId,
                    EndDate = request.StartDate,
                    StartDate = request.EndDate,
                    Info = request.Info,
                    // InterCompanyId = request.InterCompanyId,
                    IsAccepted = false,
                    IsDeleted = false,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = _context.UserId,
                    // Name = newBudgetCode,
                    // PartnerId = request.PartnerId,

                    // Quantity = request.Quantity,
                    // SubTypeId = request.SubTypeId,
                    UserId = request.UserId,
                    Validated = true,
                    // ValueFin = request.ValueIni,
                    //ValueIni = request.ValueIni,
                    Guid = Guid.NewGuid(),
                    // QuantityRem = request.Quantity,
                    //AdmCenterId = request.AdmCenterId,
                    // RegionId = request.RegionId,

                    //ProjectTypeId = request,
                    //BudgetId = budgetDto.BudgetId,
                    BudgetBaseId = request.BudgetBaseId,
                    RequestId = request.Id,
                    ProjectId = request.ProjectId,
                    AssetTypeId = request.AssetTypeId,
                    ProjectTypeId = request.ProjectTypeId,
                    CostCenterId = request.CostCenterId,

                };
                _context.Add(offer);
                entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
                _context.Update(entityType);

                _context.SaveChanges();
            }

            rate = await _context.Set<Model.Rate>().Where(u => u.Id == offerMatAdd.RateId).FirstOrDefaultAsync();

            for (int i = 0; i < offerMatAdd.MaterialIds.Length; i++)
            {
                var material = await _context.Set<Model.Material>().Where(u => u.Id == offerMatAdd.MaterialIds[i]).FirstOrDefaultAsync();


                offerMaterial = new Model.OfferMaterial()
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserId,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = _context.UserId,
                    OfferId = offer.Id,
                    MaterialId = offerMatAdd.MaterialIds[i],
                    EmailManagerId = 2078,
                    AppStateId = 7,
                    //Value = material.Price * material.Quantity,
                    //Price = material.Price,
                    //Quantity = material.Quantity,
                    Value = 0,
                    Price = 0,
                    Quantity = 0,
                    RequestId = offer.RequestId,
                    RateId = offerMatAdd.RateId,
                    Guid = offerMatAdd.Guid,
                    Validated = false
                };

                _context.Add(offerMaterial);

                _context.SaveChanges();
            }

            offer.RateId = offerMatAdd.RateId;
            offer.UomId = rate.UomId;

            _context.Update(offer);
            _context.SaveChanges();

            return Ok(StatusCode(200));
        }
    }
}
