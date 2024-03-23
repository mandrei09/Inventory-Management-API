using System.Collections.Generic;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Linq;
using Optima.Fais.Dto;
using System;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Optima.Fais.Model.Utils;
using System.Text;
using Optima.Fais.Model;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
    public class OffersRepository : Repository<Model.Offer>, IOffersRepository
    {

        public OffersRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Name.Contains(filter) || a.Code.Contains(filter)); })
        {
           
        }

        public IEnumerable<Model.OfferDetail> GetOffer(OfferFilter offerFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Offer> budgetQuery = null;
            IQueryable<OfferDetail> query = null;

            budgetQuery = _context.Offers.AsNoTracking().AsQueryable();

            if (offerFilter.Filter != "" && offerFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Name.Contains(offerFilter.Filter) || a.Code.Contains(offerFilter.Filter) || a.Partner.Name.Contains(offerFilter.Filter)));


            includes = includes ?? string.Empty;

            foreach (var includeProperty in includes.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int dotIndex = includeProperty.IndexOf(".");
                string prefix = string.Empty;
                string property = string.Empty;

                if (dotIndex > 0)
                {
                    prefix = includeProperty.Substring(0, dotIndex);
                    property = includeProperty.Length > dotIndex ? includeProperty.Substring(dotIndex + 1) : string.Empty;
                }
                else
                {
                    prefix = includeProperty;
                }


                switch (prefix)
                {
                    case "Offer":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new OfferDetail { Offer = budget });


            if (offerFilter.Role != null && offerFilter.Role != "")
            {
                if (offerFilter.Role.ToUpper() == "ADMINISTRATOR")
                {
                    //if ((requestFilter.CostCenterIds != null) && (requestFilter.CostCenterIds.Count > 0))
                    //{
                    //    query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => ((a.Adm.CostCenterId == id)); }, assetFilter.CostCenterIds));
                    //}

                    //if ((requestFilter.EmployeeIds != null) && (requestFilter.EmployeeIds.Count > 0))
                    //{
                    //    query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => ((a.Adm.EmployeeId == id)); }, assetFilter.EmployeeIds));
                    //}
                }
                else if (offerFilter.Role.ToUpper() == "PROCUREMENT")
                {
                    List<int?> divisionIds = new List<int?>();
                    divisionIds.Add(1482);

                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.OfferDetail, int?>((id) => { return a => a.Offer.DivisionId != id; }, divisionIds));

                    query = query.Where(a => a.Offer.AssetType.Code != "STOCK_IT");
                }
                else if (offerFilter.Role.ToUpper() == "PROC-IT")
                {
                    if (offerFilter.InInventory)
                    {
                        List<int?> costCenterIds = _context.Set<Model.EmployeeCostCenter>().AsNoTracking().Where(e => e.EmployeeId == offerFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.CostCenterId).ToList();

                        if (costCenterIds.Count == 0)
                        {
                            costCenterIds = new List<int?>();
                            costCenterIds.Add(-1);
                        }

                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.OfferDetail, int?>((id) => { return a => a.Offer.CostCenterId == id; }, costCenterIds));


                        if ((offerFilter.CostCenterIds != null) && (offerFilter.CostCenterIds.Count > 0))
                        {
                            query = query.Where(ExpressionHelper.GetInListPredicate<Model.OfferDetail, int?>((id) => { return a => a.Offer.CostCenterId == id; }, offerFilter.CostCenterIds));
                        }

                        query = query.Where(a => a.Offer.AssetType.Code != "STOCK_IT");
                    }
                    else
                    {
                        List<int?> divisionIds = _context.Set<Model.EmployeeDivision>().AsNoTracking().Where(e => e.EmployeeId == offerFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.DivisionId).ToList();

                        if (divisionIds.Count == 0)
                        {
                            divisionIds = new List<int?>();
                            divisionIds.Add(-1);
                        }

                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.OfferDetail, int?>((id) => { return a => a.Offer.DivisionId == id; }, divisionIds));
                    }
                }
                else
                {
                    if (offerFilter.InInventory)
                    {
                        List<int?> costCenterIds = _context.Set<Model.EmployeeCostCenter>().AsNoTracking().Where(e => e.EmployeeId == offerFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.CostCenterId).ToList();

                        if (costCenterIds.Count == 0)
                        {
                            costCenterIds = new List<int?>();
                            costCenterIds.Add(-1);
                        }

                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.OfferDetail, int?>((id) => { return a => a.Offer.CostCenterId == id; }, costCenterIds));


                        if ((offerFilter.CostCenterIds != null) && (offerFilter.CostCenterIds.Count > 0))
                        {
                            query = query.Where(ExpressionHelper.GetInListPredicate<Model.OfferDetail, int?>((id) => { return a => a.Offer.CostCenterId == id; }, offerFilter.CostCenterIds));
                        }
                    }
                    else
                    {
                        List<int?> divisionIds = _context.Set<Model.EmployeeDivision>().AsNoTracking().Where(e => e.EmployeeId == offerFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.DivisionId).ToList();

                        if (divisionIds.Count == 0)
                        {
                            divisionIds = new List<int?>();
                            divisionIds.Add(-1);
                        }

                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.OfferDetail, int?>((id) => { return a => a.Offer.DivisionId == id; }, divisionIds));
                    }

                }
            }

            if ((offerFilter.CompanyIds != null) && (offerFilter.CompanyIds.Count > 0))
            {
                //query = query.Where(a => offerFilter.CompanyIds.Contains(a.Offer.CompanyId));
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.OfferDetail, int?>((id) => { return a => a.Offer.CompanyId == id; }, offerFilter.CompanyIds));
			}


            if ((offerFilter.PartnerIds != null) && (offerFilter.PartnerIds.Count > 0))
            {
                //query = query.Where(a => offerFilter.PartnerIds.Contains(a.Offer.PartnerId));
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.OfferDetail, int?>((id) => { return a => a.Offer.PartnerId == id; }, offerFilter.PartnerIds));
			}

            if ((offerFilter.EmployeeIds != null) && (offerFilter.EmployeeIds.Count > 0))
            {
                //query = query.Where(a => offerFilter.EmployeeIds.Contains(a.Offer.EmployeeId));
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.OfferDetail, int?>((id) => { return a => a.Offer.EmployeeId == id; }, offerFilter.EmployeeIds));
			}

			if ((offerFilter.RequestIds != null) && (offerFilter.RequestIds.Count > 0))
			{
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.OfferDetail, int?>((id) => { return a => a.Offer.RequestId == id; }, offerFilter.RequestIds));
			}

			if (offerFilter.Type != null && offerFilter.Type != "")
            {
                if (offerFilter.Type.ToUpper() == "ME")
                {
                    query = query.Where(a => a.Offer.EmployeeId == offerFilter.EmployeeId);
                }
            }

            query = query.Where(a => a.Offer.IsDeleted == false && a.Offer.Validated == true);

         

            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();
            //catTotal.AssetCategoryDeskPhone = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 42 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryMonitor = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 72 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryThinClient = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 1035 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.OfferDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.OfferDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }
        public IEnumerable<Model.OfferDetail> GetOfferUI(OfferFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Offer> budgetQuery = null;
            IQueryable<OfferDetail> query = null;

            budgetQuery = _context.Offers.AsNoTracking().AsQueryable();

            if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Name.Contains(budgetFilter.Filter) || a.Code.Contains(budgetFilter.Filter)));


            includes = includes ?? string.Empty;

            foreach (var includeProperty in includes.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int dotIndex = includeProperty.IndexOf(".");
                string prefix = string.Empty;
                string property = string.Empty;

                if (dotIndex > 0)
                {
                    prefix = includeProperty.Substring(0, dotIndex);
                    property = includeProperty.Length > dotIndex ? includeProperty.Substring(dotIndex + 1) : string.Empty;
                }
                else
                {
                    prefix = includeProperty;
                }


                switch (prefix)
                {
                    case "Offer":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new OfferDetail { Offer = budget });

            query = query.Where(a => a.Offer.IsDeleted == false && a.Offer.Validated == true && a.Offer.IsAccepted == true && (a.Offer.QuantityRem > 0 || a.Offer.Code == "STOCK IT"));



            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();
            //catTotal.AssetCategoryDeskPhone = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 42 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryMonitor = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 72 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryThinClient = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 1035 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.OfferDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.OfferDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public IEnumerable<Model.OfferDetail> GetAllOfferUI(OfferFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Offer> budgetQuery = null;
            IQueryable<OfferDetail> query = null;

            budgetQuery = _context.Offers.AsNoTracking().AsQueryable();

            if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Name.Contains(budgetFilter.Filter) || a.Code.Contains(budgetFilter.Filter)));


            includes = includes ?? string.Empty;

            foreach (var includeProperty in includes.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int dotIndex = includeProperty.IndexOf(".");
                string prefix = string.Empty;
                string property = string.Empty;

                if (dotIndex > 0)
                {
                    prefix = includeProperty.Substring(0, dotIndex);
                    property = includeProperty.Length > dotIndex ? includeProperty.Substring(dotIndex + 1) : string.Empty;
                }
                else
                {
                    prefix = includeProperty;
                }


                switch (prefix)
                {
                    case "Offer":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new OfferDetail { Offer = budget });

            query = query.Where(a => a.Offer.IsDeleted == false && a.Offer.Validated == true);



            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();
            //catTotal.AssetCategoryDeskPhone = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 42 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryMonitor = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 72 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryThinClient = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 1035 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.OfferDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.OfferDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public async Task<Model.OfferResult> CreateOrUpdateOffer(OfferSave offerSave)
        {
            Model.Offer offer = null;
            Model.OfferType offerType = null;
            Model.OfferOp offerOp = null;
            Model.Document document = null;
            Model.DocumentType documentType = null;
            Model.EntityType entityType = null;
            Model.Request request = null;
            Model.Inventory inventory = null;
            Model.AppState appState = null;

            inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).FirstOrDefaultAsync();
            if (inventory == null) return new Model.OfferResult { Success = false, Message = "Nu exista inventar activ!", OfferId = 0 };
            appState = await _context.Set<Model.AppState>().Where(c => c.Code == "NEW").FirstOrDefaultAsync();
            if (appState == null) return new Model.OfferResult { Success = false, Message = "Nu exista stare!", OfferId = 0 };
            entityType = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWOFFER").FirstOrDefaultAsync();
            if (entityType == null) return new Model.OfferResult { Success = false, Message = "Nu exista entityType!", OfferId = 0 };
            documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_OFFER").FirstOrDefaultAsync();
            if (documentType == null) return new Model.OfferResult { Success = false, Message = "Nu exista tip de document!", OfferId = 0 };
            request = await _context.Set<Model.Request>().Where(d => d.Id == offerSave.RequestId).FirstOrDefaultAsync();
            if (request == null) return new Model.OfferResult { Success = false, Message = "Nu exista P.R.!", OfferId = 0 };
            offerType = await _context.Set<Model.OfferType>().Where(d => d.Id == offerSave.OfferTypeId).FirstOrDefaultAsync();
            if (offerType == null) return new Model.OfferResult { Success = false, Message = "Nu exista tip oferta!", OfferId = 0 };

            if(offerType.Code == "O" || offerType.Code == "O-C" || offerType.Code == "O-V")
			{
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

                document = new Model.Document()
                {
                    Approved = true,
                    CompanyId = request.CompanyId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = offerSave.UserId,
                    CreationDate = DateTime.Now,
                    Details = string.Empty,
                    DocNo1 = string.Empty,
                    DocNo2 = string.Empty,
                    DocumentDate = DateTime.Now,
                    DocumentTypeId = documentType.Id,
                    Exported = true,
                    IsDeleted = false,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = offerSave.UserId,
                    ParentDocumentId = null,
                    PartnerId = null,
                    RegisterDate = DateTime.Now,
                    ValidationDate = DateTime.Now
                };

                _context.Add(document);


                offer = new Model.Offer()
                {
                    AccMonthId = inventory.AccMonthId,
                    AccountId = null,
                    AdministrationId = null,
                    AppStateId = appState.Id,
                    BudgetManagerId = inventory.BudgetManagerId,
                    Code = newBudgetCode,
                    CompanyId = request.CompanyId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _context.UserId,
                    EmployeeId = offerSave.EmployeeId,
                    EndDate = request.StartDate,
                    StartDate = request.EndDate,
                    Info = request.Info,
                    IsAccepted = false,
                    IsDeleted = false,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = _context.UserId,
                    PartnerId = null,
                    Quantity = 0,
                    SubTypeId = null,
                    UserId = offerSave.UserId,
                    Validated = true,
                    ValueFin = 0,
                    ValueIni = 0,
                    Guid = Guid.NewGuid(),
                    QuantityRem = 0,
                    AdmCenterId = null,
                    RegionId = null,
                    RequestId = offerSave.RequestId,
                    AssetTypeId = request.AssetTypeId,
                    ProjectTypeId = request.ProjectTypeId,
                    OfferTypeId = offerSave.OfferTypeId,
                    DivisionId = request.DivisionId

                };
                _context.Add(offer);

                offerOp = new Model.OfferOp()
                {
                    AccMonthId = inventory.AccMonthId,
                    AccSystemId = null,
                    
                    
                    AdministrationIdInitial = null,
                    AdministrationIdFinal = null,
                    Offer = offer,
                    BudgetManagerIdInitial = inventory.BudgetManagerId,
                    BudgetManagerIdFinal = inventory.BudgetManagerId,
                    BudgetStateId = appState.Id,
                    CompanyIdInitial = request.CompanyId,
                    CompanyIdFinal = request.CompanyId,
                    CostCenterIdInitial = null,
                    CostCenterIdFinal = null,
                    CreatedAt = DateTime.Now,
                    CreatedBy = offerSave.UserId,
                    Document = document,
                    DstConfAt = DateTime.Now,
                    DstConfBy = offerSave.UserId,
                    EmployeeIdInitial = offerSave.EmployeeId,
                    EmployeeIdFinal = offerSave.EmployeeId,
                    InfoIni = request.Info,
                    InfoFin = request.Info,
                    IsAccepted = false,
                    IsDeleted = false,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = offerSave.UserId,
                    PartnerIdInitial = null,
                    PartnerIdFinal = null,
                    ProjectIdInitial = null,
                    ProjectIdFinal = null,
                    QuantityIni = 0,
                    QuantityFin = 0,
                    SubTypeIdInitial = null,
                    SubTypeIdFinal = null,
                    Validated = true,
                    ValueFin1 = 0,
                    ValueIni1 = 0,
                    ValueFin2 = 0,
                    ValueIni2 = 0,
                    Guid = Guid.NewGuid(),
                    AdmCenterIdInitial = null,
                    AdmCenterIdFinal = null,
                    RegionIdInitial = null,
                    RegionIdFinal = null,
                    AssetTypeIdInitial = request.AssetTypeId,
                    AssetTypeIdFinal = request.AssetTypeId,
                    ProjectTypeIdInitial = request.ProjectTypeId,
                    ProjectTypeIdFinal = request.ProjectTypeId,
                    //BudgetIdInitial = budgetDto.BudgetId,
                    //BudgetIdFinal = budgetDto.BudgetId,
                    RequestId = offerSave.RequestId
                };

                _context.Add(offerOp);

                entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
                _context.Update(entityType);

                request.AppStateId = _context.Set<Model.AppState>().Where(a => a.Code == "USED_REQUEST").Select(A => A.Id).FirstOrDefault();

                _context.Update(request);
            }
			else
			{
                offer = await _context.Set<Model.Offer>().Where(a => a.Id == offerSave.OfferCloneId).SingleOrDefaultAsync();

                return new Model.OfferResult { Success = true, Message = "Oferta a fost salvata cu succes!", OfferId = offer.Id };
            }

            
            _context.SaveChanges();

            return new Model.OfferResult { Success = true, Message = "Oferta a fost salvata cu succes!", OfferId = offer.Id };
        }

        public Model.Offer GetDetailsById(int id, string includes)
        {
            IQueryable<Model.Offer> query = null;
            query = GetBudgetQuery(includes);

            return query.AsNoTracking().Where(a => a.Id == id)
                .Include(b => b.Company)
                .Include(b => b.Project)
                .Include(b => b.AdmCenter)
                .Include(b => b.Region)
                .Include(b => b.AssetType)
                .Include(b => b.ProjectType)
                .Include(b => b.Budget)
                .Include(b => b.BudgetBase)
                //.Include(b => b.SubType)
                //    .ThenInclude(t => t.Type)
                //        .ThenInclude(m => m.MasterType)
                .Include(b => b.Employee)
                .Include(b => b.AccMonth)
                
                .Include(b => b.Partner)
                .Include(b => b.CostCenter)
                .SingleOrDefault();
        }

        private IQueryable<Model.Offer> GetBudgetQuery(string includes)
        {
            IQueryable<Model.Offer> query = null;
            query = _context.Offers.AsNoTracking();

            return query;
        }


        public async Task<Model.EmailResult> SendEmail(int offerId, CodePartnerEntity selectedPartner)
        {
            Model.Offer offer = null;
            Model.Partner partner = null;
            var emailIni = "";
            var emailCC = "";
            var htmlBodyEmail = "";
            var htmlBodyEnd = @"
                                            </body>
                                        </html> ";
            var htmlBody = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    New Offer
                                                </title>
                                                <style type=""text/css"">
                                                    <style type=""text/css"">
                                                    HTML{background-color: #e8e8e8;}
                                                    .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
                                                    .courses-table .description {color: #fefefe !important;}
                                                    .courses-table .description a{color: #ffffff !important; font: bold 11px Arial;
                                                      text-decoration: none;
                                                      background-color: #fe0000;
                                                      padding: 2px 6px 2px 6px;
                                                      border-top: 1px solid #CCCCCC;
                                                      border-right: 1px solid #333333;
                                                      border-bottom: 1px solid #333333;
                                                      border-left: 1px solid #CCCCCC;}
                                                    .courses-table td{border: 1px solid #ffffff; background-color: #000000; text-align: center; padding: 8px;}
                                                    .courses-table th{border: 1px solid #ffffff; color: #030804;text-align: center; padding: 8px;}
                                                    .red{background-color: #FFDD04;}
                                                    .header{background-color: #fe0000; color: #000000 !important;}
                                                    .msg{background-color: #000000; color: #fefefe !important;}
                                                    .table-header{background-color: #000000; color: #fefefe !important;}
                                                    .table-body{background-color: #000000; color: #fefefe !important;}
                                                    .green{background-color: #6B9852;}
                                                   
                                                </style>
                                                </style>
                                            </head>
                                            <body>
                                        ";
            var subject = "Oferta noua";

            partner = await _context.Set<Model.Partner>().AsNoTracking().Where(u => u.Id == selectedPartner.Id).SingleAsync();

            offer = await _context.Set<Model.Offer>()
                .Include(e => e.Company)
                .Include(e => e.AssetType)
                .Include(e => e.Employee)
                    .ThenInclude(d => d.Department)
                .Where(a => a.Id == offerId).SingleAsync();

            if (partner.ContactInfo != null && partner.ContactInfo != "")
            {
                emailIni = partner.ContactInfo;
                emailCC = partner.ContactInfo;

                //if (offer.Employee.Department != null && offer.Employee.Department.Name != null && offer.Employee.Department.Name != "")
                //{
                //    emailCC = offer.Employee.Department.Name;
                //}
                //else
                //{
                //    emailCC = "adrian.cirnaru@optima.ro";
                //}
            }
            else
            {
                emailIni = "adrian.cirnaru@optima.ro";
            }

            return new Model.EmailResult { Success = true, Message = "", Subject = subject, To = emailIni, Cc = emailCC, Body = htmlBody + htmlBodyEmail + htmlBodyEnd +

                 @"<PRE style=""font - family: Roboto,Montserrat,helvetica neue,Helvetica,Arial, sans - serif; font - size: 12px"">" 
                + selectedPartner.Name + "<PRE>",  EmailManagerId = 0 };


        }

        public int SendValidatedEmail(int budgetId, string userName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut)
        {
            Model.Budget budget = null;
            Model.EmailType emailTypeAsset = null;
            var emailIni = "";
            var emailCC = "";
            var htmlBodyEmail = "";
            var htmlBodyEnd = @"</tbody>
                                                </table>
                                            </body>
                                        </html> ";
            var htmlBody = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    New budget
                                                </title>
                                                <style type=""text/css"">
                                                    <style type=""text/css"">
                                                    HTML{background-color: #e8e8e8;}
                                                    .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
                                                    .courses-table .description {color: #fefefe !important;}
                                                    .courses-table .description a{color: #ffffff !important; font: bold 11px Arial;
                                                      text-decoration: none;
                                                      background-color: #fe0000;
                                                      padding: 2px 6px 2px 6px;
                                                      border-top: 1px solid #CCCCCC;
                                                      border-right: 1px solid #333333;
                                                      border-bottom: 1px solid #333333;
                                                      border-left: 1px solid #CCCCCC;}
                                                    .courses-table td{border: 1px solid #ffffff; background-color: #000000; text-align: center; padding: 8px;}
                                                    .courses-table th{border: 1px solid #ffffff; color: #030804;text-align: center; padding: 8px;}
                                                    .red{background-color: #FFDD04;}
                                                    .header{background-color: #fe0000; color: #000000 !important;}
                                                    .msg{background-color: #000000; color: #fefefe !important;}
                                                    .table-header{background-color: #000000; color: #fefefe !important;}
                                                    .table-body{background-color: #000000; color: #fefefe !important;}
                                                    .green{background-color: #6B9852;}
                                                   
                                                </style>
                                                </style>
                                            </head>
                                            <body>
                                                <h4>Budget details:</h4>
                                                <table class=""courses-table"">
                                                    <thead>
                                                        <tr>
                                                            <th class=""red"">Company</th>
                                                            <th class=""red"">Project ID</th>
                                                            <th class=""red"">Project</th>
                                                            <th class=""red"">Activity</th>
                                                            <th class=""red"">CC</th>
                                                            <th class=""red"">PC</th>
                                                            <th class=""red"">Expence Type</th>
                                                            <th class=""red"">Details</th>
                                                            <th class=""red"">Supplier</th>
                                                            <th class=""red"">Account</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                        ";
            var subject = "Budget was validated";

            if (budgetId > 0)
            {

                var user = _context.Users.Where(u => u.UserName == userName).Single();

                budget = _context.Set<Model.Budget>()
                    .Include(e => e.Company)
                    .Include(e => e.Project)
                    .Include(e => e.Administration)
                    .Include(e => e.CostCenter)
                    .Include(e => e.SubType)
                        .ThenInclude(e => e.Type)
                            .ThenInclude(e => e.MasterType)
                    .Include(e => e.Employee)
                    .Include(e => e.Partner)
                    .Include(e => e.AccMonth)
                    .Where(a => a.Id == budgetId).Single();

                if (budget.Employee != null && budget.Employee.Email != "" && budget.Employee.Email != null)
                {
                    emailIni = budget.Employee.Email;

                    if (budget.Employee.Department != null && budget.Employee.Department.Name != null && budget.Employee.Department.Name != "")
                    {
                        emailCC = budget.Employee.Department.Name;
                    }
                    else
                    {
                        emailCC = "adrian.cirnaru@optima.ro";
                    }
                }
                else
                {
                    emailIni = "adrian.cirnaru@optima.ro";
                }




                htmlBodyEmail = htmlBodyEmail + @"
                                                        <tr>
                                                            <td class=""description"">" + budget.Company.Name + @" </ td >
                                                            <td class=""description"">" + budget.Project.Code + @" </ td >
                                                            <td class=""description"">" + budget.Project.Name + @" </ td >
                                                            <td class=""description"">" + budget.Administration.Name + @" </ td >
                                                            <td class=""description"">" + budget.CostCenter.Name + @" </ td >
                                                            <td class=""description"">" + budget.SubType.Type.MasterType.Name + @" </ td >
                                                            <td class=""description"">" + budget.SubType.Type.Name + @" </ td >
                                                            <td class=""description"">" + budget.SubType.Name + @" </ td >
                                                            <td class=""description"">" + budget.Partner.Name + @" </td >
                                                        </tr>
                                                        <tfoot>
                                                            <br>
                                                                <thead>
                                                                <tr>
                                                                    <th class=""red"">Owner</th>
                                                                    <th class=""red"">FY</th>
                                                                    <th class=""red"">Tot. life in periods</th>
                                                                    <th class=""red"">Budget value</th>
                                                                    <th class=""red"">Quantity</th>
                                                                    <th class=""red"">Comment</th>
                                                                </tr>
                                                            </thead>
                                                            <tr>
                                                             <td class=""description"">" + budget.Employee.FirstName + " " + budget.Employee.LastName + @" </td >
                                                             <td class=""description"">" + budget.AccMonth.Year + @" </ td >
                                                             <td class=""description"">" + budget.ValueIni + @" </ td >
                                                             <td class=""description"">" + budget.Quantity + @" </ td >
                                                             <td class=""description"">" + budget.Info + @" </ td >
                                                            </tr>
                                                        </tfoot>
                                        ";
            }


            //emailTypeAsset = _context.Set<Model.EmailType>().Where(d => d.Code == "VALIDATE NEW BUDGET").Single();
            //headerMsg = emailTypeAsset.HeaderMsg;
            //footerMsg = emailTypeAsset.FooterMsg;
            //var budgetLink = "https://service.inventare.ro/Emag/#/budgetvalidate/" + budget.Guid.ToString();
            //var budgetLink = "http://localhost:3100/#/budgetvalidate/" + budget.Guid.ToString();
            // var link = @"<h4><span style=""font-family: Roboto,Montserrat,helvetica neue,Helvetica,Arial,sans-serif;font-size: 12px;color: rgb(115, 115, 115)"">To review and validate new budget, please access the following link: <a style=""color: red; font-size: 16px;"" href = '" + budgetLink + "'" + "' >  VALIDATE BUDGET: " + budget.Code + "</a>" + @"</span></h4>";
            //var linkInfo = @"<h4><span style=""font-family: Roboto,Montserrat,helvetica neue,Helvetica,Arial,sans-serif;font-size: 12px;color: rgb(115, 115, 115)"">(If the link cannot be accessed with IE, please try using Chrome)" + @"</span></h4>";

            emailIniOut = emailIni;
            emailCCOut = emailCC;
            // bodyHtmlOut = htmlBody + htmlBodyEmail + htmlBodyEnd;
            bodyHtmlOut = htmlBody + htmlBodyEmail + htmlBodyEnd;// + link + linkInfo;
            subjectOut = subject;

            _context.SaveChanges();

            return budgetId;


        }

        public IEnumerable<Model.OfferDetail> BudgetValidate(OfferFilter budgetFilter, string includes, string userId, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Offer> budgetQuery = null;
            IQueryable<OfferDetail> query = null;

            budgetQuery = _context.Offers.AsNoTracking().AsQueryable();

            if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Name.Contains(budgetFilter.Filter)));


            includes = includes ?? string.Empty;

            foreach (var includeProperty in includes.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int dotIndex = includeProperty.IndexOf(".");
                string prefix = string.Empty;
                string property = string.Empty;

                if (dotIndex > 0)
                {
                    prefix = includeProperty.Substring(0, dotIndex);
                    property = includeProperty.Length > dotIndex ? includeProperty.Substring(dotIndex + 1) : string.Empty;
                }
                else
                {
                    prefix = includeProperty;
                }


                switch (prefix)
                {
                    case "Offer":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new OfferDetail { Offer = budget });

            if (userId != "" && userId != null)
            {
                query = query.Where(a => a.Offer.Guid.ToString() == userId);
            }
            else
            {
                query = query.Where(a => a.Offer.Guid.ToString() == "1234");
            }

            query = query.Where(a => a.Offer.IsDeleted == false && a.Offer.Validated == true);



            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();
            //catTotal.AssetCategoryDeskPhone = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 42 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryMonitor = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 72 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryThinClient = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 1035 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.OfferDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.OfferDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

    }
}
