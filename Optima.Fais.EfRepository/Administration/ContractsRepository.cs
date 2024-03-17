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

namespace Optima.Fais.EfRepository
{
    public class ContractsRepository : Repository<Model.Contract>, IContractsRepository
    {

        public ContractsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Name.Contains(filter) || a.ContractId.Contains(filter) || a.Partner.Name.Contains(filter)); })
        {
           
        }

        public IEnumerable<Model.ContractDetail> GetContract(ContractFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Contract> budgetQuery = null;
            IQueryable<ContractDetail> query = null;

            budgetQuery = _context.Contracts.AsNoTracking().AsQueryable();

            int companyId = _context.Set<Model.Company>().AsNoTracking().Where(c => c.Code == "RO10").Select(c => c.Id).FirstOrDefault();

            if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Title.Contains(budgetFilter.Filter) || a.ContractId.Contains(budgetFilter.Filter)));


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
                    case "Contract":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new ContractDetail { Contract = budget });

            if ((budgetFilter.CompanyIds != null) && (budgetFilter.CompanyIds.Count > 0))
            {
                query = query.Where(a => budgetFilter.CompanyIds.Contains(a.Contract.CompanyId));
            }

            if ((budgetFilter.EmployeeIds != null) && (budgetFilter.EmployeeIds.Count > 0))
            {
                query = query.Where(a => budgetFilter.EmployeeIds.Contains(a.Contract.EmployeeId));
            }


            if ((budgetFilter.PartnerIds != null) && (budgetFilter.PartnerIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.ContractDetail, int?>((id) => { return a => a.Contract.PartnerId == id; }, budgetFilter.PartnerIds));
            }

			//query = query.Where(a => a.Contract.IsDeleted == false).Where(b => b.Contract.Commodities
			//.Any(s => (
   //         (s.Code == "D1402001" && s.IsDeleted == false) || 
   //         (s.Code == "D1402002" && s.IsDeleted == false) || 
   //         (s.Code == "D1402003" && s.IsDeleted == false) || 
   //         (s.Code == "D1701004" && s.IsDeleted == false))));

            query = query.Where(a => a.Contract.IsDeleted == false && a.Contract.Code == "Capex" && a.Contract.CompanyId == companyId);



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
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.ContractDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.ContractDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public IEnumerable<Model.ContractDetail> GetContractUI(ContractFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Contract> budgetQuery = null;
            IQueryable<ContractDetail> query = null;

            budgetQuery = _context.Contracts.AsNoTracking().AsQueryable();

            int companyId = _context.Set<Model.Company>().AsNoTracking().Where(c => c.Code == "RO10").Select(c => c.Id).FirstOrDefault();

            int appStateId = _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "Published").Select(a => a.Id).Single();

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
                    case "Contract":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }


            query = budgetQuery.Select(budget => new ContractDetail { Contract = budget });

            var a = query.Count();

            if ((budgetFilter.PartnerIds != null) && (budgetFilter.PartnerIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.ContractDetail, int?>((id) => { return a => a.Contract.PartnerId == id || a.Contract.ContractId == "NO-C"; }, budgetFilter.PartnerIds));
            }

            a = query.Count();

            //if ((budgetFilter.UomIds != null) && (budgetFilter.UomIds.Count > 0))
            //{
            //	query = query.Where(ExpressionHelper.GetInListPredicate<Model.ContractDetail, int?>((id) => { return a => a.Contract.ContractAmount.Uom.Id == id; }, budgetFilter.UomIds));
            //}

            // query = query.Where(a => ((a.Contract.IsDeleted == false && a.Contract.AppStateId == 27) || (a.Contract.ContractId == "NO-C")));
            //query = query.Where(a => ((a.Contract.IsDeleted == false && a.Contract.AppStateId == appStateId) || (a.Contract.ContractId == "NO-C")))
            //                      .Where(b => b.Contract.Commodities
            //                      .Any(s => (
            //                      (s.Code == "D1402001" && s.IsDeleted == false) || 
            //                      (s.Code == "D1402002" && s.IsDeleted == false) ||
            //                      (s.Code == "D1402003" && s.IsDeleted == false) ||
            //                      (s.Code == "D1701004" && s.IsDeleted == false))));

            query = query.Where(a => ((a.Contract.IsDeleted == false && a.Contract.AppStateId == appStateId && a.Contract.CompanyId == companyId && a.Contract.Code == "Capex") || (a.Contract.ContractId == "NO-C")));

            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.ContractDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.ContractDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public int CreateOrUpdateContract(ContractSave budgetDto)
        {
            Model.Contract budget = null;
            Model.ContractOp budgetOp = null;
            Model.ContractOp offerOp = null;
            Model.Document document = null;
            Model.DocumentType documentType = null;
            Model.EntityType entityType = null;

            if (budgetDto.Id > 0)
            {
                budget = _context.Set<Model.Contract>().Where(a => a.Id == budgetDto.Id).Single();

                budget.AccMonthId = budgetDto.AccMonthId;
                budget.Code = budgetDto.Code;
                budget.CompanyId = budgetDto.CompanyId;
                budget.EmployeeId = budgetDto.EmployeeId;
                //budget.EndDate = budgetDto.EndDate;
                budget.Info = budgetDto.Info;
                budget.ModifiedAt = DateTime.Now;
                budget.ModifiedBy = budgetDto.UserId;
                budget.Name = budgetDto.Name;
                budget.PartnerId = budgetDto.PartnerId;
                //budget.Quantity = budgetDto.Quantity;
                //budget.StartDate = budgetDto.StartDate;
                //budget.UserId = budgetDto.UserId;
                //budget.ValueIni = budgetDto.ValueIni;
                //budget.ValueFin = budgetDto.ValueFin;
                //budget.Price = budgetDto.Price;
                //budget.QuantityRem = budgetDto.Quantity;
                //budget.UomId = budgetDto.UomId;

                _context.Set<Model.Contract>().Update(budget);
			}
            else
            {
                entityType = _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWCONTRACT").FirstOrDefault();

                var lastCode = int.Parse(entityType.Name);
                var newBudgetCode = entityType.Code + lastCode.ToString();


                documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_CONTRACT").SingleOrDefault();

                document = new Model.Document()
                {
                    Approved = true,
                    CompanyId = budgetDto.CompanyId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = budgetDto.UserId,
                    CreationDate = DateTime.Now,
                    Details = budgetDto.Info != null ? budgetDto.Info : string.Empty,
                    DocNo1 = budgetDto.Info != null ? budgetDto.Info : string.Empty,
                    DocNo2 = budgetDto.Info != null ? budgetDto.Info : string.Empty,
                    DocumentDate = DateTime.Now,
                    DocumentTypeId = documentType.Id,
                    Exported = true,
                    IsDeleted = false,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = budgetDto.UserId,
                    ParentDocumentId = null,
                    PartnerId = budgetDto.PartnerId,
                    RegisterDate = DateTime.Now,
                    ValidationDate = DateTime.Now
                };

                _context.Add(document);

                //budget = new Model.Contract()
                //{
                   
                //    AppStateId = 12,
                //    Code = newBudgetCode,
                //    CompanyId = budgetApproved.CompanyId,
                //    CostCenterId = budgetApproved.CostCenterId,
                //    CreatedAt = DateTime.Now,
                //    CreatedBy = budgetDto.UserId,
                //    EmployeeId = budgetApproved.EmployeeId,
                //    EndDate = budgetApproved.StartDate,
                //    StartDate = budgetApproved.EndDate,
                //    Info = offer.Info,
                //    InterCompanyId = budgetApproved.InterCompanyId,
                //    IsAccepted = false,
                //    IsDeleted = false,
                //    ModifiedAt = DateTime.Now,
                //    ModifiedBy = budgetDto.UserId,
                //    Name = budgetDto.Name,
                //    PartnerId = offer.PartnerId,
                //    ProjectId = budgetApproved.ProjectId,
                //    Quantity = budgetDto.Quantity,
                //    SubTypeId = offer.SubTypeId,
                //    UserId = budgetDto.UserId,
                //    Validated = true,
                //    ValueFin = budgetDto.ValueIni,
                //    ValueIni = budgetDto.ValueIni,
                //    Guid = Guid.NewGuid(),
                //    QuantityRem = budgetDto.Quantity,
                //    Price = budgetDto.Price,
                //    OfferId = budgetDto.OfferId,
                //    BudgetId = budgetDto.BudgetId,
                //    UomId = budgetDto.UomId
                    


                //};
                //_context.Add(budget);

                //budgetOp = new Model.OrderOp()
                //{
                //    AccMonthId = budgetApproved.AccMonthId,
                //    AccSystemId = null,
                //    AccountIdInitial = budgetApproved.AccountId,
                //    AccountIdFinal = budgetApproved.AccountId,
                //    AdministrationIdInitial = budgetApproved.AdministrationId,
                //    AdministrationIdFinal = budgetApproved.AdministrationId,
                //    Order = budget,
                //    BudgetManagerIdInitial = null,
                //    BudgetManagerIdFinal = null,
                //    BudgetStateId = 12,
                //    CompanyIdInitial = budgetApproved.CompanyId,
                //    CompanyIdFinal = budgetApproved.CompanyId,
                //    CostCenterIdInitial = budgetApproved.CostCenterId,
                //    CostCenterIdFinal = budgetApproved.CostCenterId,
                //    CreatedAt = DateTime.Now,
                //    CreatedBy = budgetDto.UserId,
                //    Document = document,
                //    DstConfAt = DateTime.Now,
                //    DstConfBy = budgetDto.UserId,
                //    EmployeeIdInitial = budgetApproved.EmployeeId,
                //    EmployeeIdFinal = budgetApproved.EmployeeId,
                //    InfoIni = budgetDto.Info,
                //    InfoFin = budgetDto.Info,
                //    InterCompanyIdInitial = budgetApproved.InterCompanyId,
                //    InterCompanyIdFinal = budgetApproved.InterCompanyId,
                //    IsAccepted = false,
                //    IsDeleted = false,
                //    ModifiedAt = DateTime.Now,
                //    ModifiedBy = budgetDto.UserId,
                //    PartnerIdInitial = offer.PartnerId,
                //    PartnerIdFinal = offer.PartnerId,
                //    ProjectIdInitial = budgetApproved.ProjectId,
                //    ProjectIdFinal = budgetApproved.ProjectId,
                //    QuantityIni = budgetDto.Quantity,
                //    QuantityFin = budgetDto.Quantity,
                //    SubTypeIdInitial = offer.SubTypeId,
                //    SubTypeIdFinal = offer.SubTypeId,
                //    Validated = true,
                //    ValueFin1 = budgetDto.ValueIni,
                //    ValueIni1 = budgetDto.ValueIni,
                //    ValueFin2 = budgetDto.ValueIni,
                //    ValueIni2 = budgetDto.ValueIni,
                //    Guid = Guid.NewGuid(),
                //    BudgetIdInitial = budgetDto.BudgetId,
                //    BudgetIdFinal = budgetDto.BudgetId,
                //    OfferIdInitial = budgetDto.OfferId,
                //    OfferIdFinal = budgetDto.OfferId,
                //    UomId = budgetDto.UomId
                //};

                //_context.Add(budgetOp);


                //offerOp = new Model.OfferOp()
                //{
                //    AccMonthId = offer.AccMonthId,
                //    AccSystemId = null,
                //    AccountIdInitial = offer.AccountId,
                //    AccountIdFinal = offer.AccountId,
                //    AdministrationIdInitial = offer.AdministrationId,
                //    AdministrationIdFinal = offer.AdministrationId,
                //    Offer = offer,
                //    BudgetManagerIdInitial = null,
                //    BudgetManagerIdFinal = null,
                //    BudgetStateId = offer.AppStateId,
                //    CompanyIdInitial = offer.CompanyId,
                //    CompanyIdFinal = offer.CompanyId,
                //    CostCenterIdInitial = offer.CostCenterId,
                //    CostCenterIdFinal = offer.CostCenterId,
                //    CreatedAt = DateTime.Now,
                //    CreatedBy = offer.UserId,
                //    Document = document,
                //    DstConfAt = DateTime.Now,
                //    DstConfBy = offer.UserId,
                //    EmployeeIdInitial = offer.EmployeeId,
                //    EmployeeIdFinal = offer.EmployeeId,
                //    InfoIni = offer.Info,
                //    InfoFin = offer.Info,
                //    InterCompanyIdInitial = offer.InterCompanyId,
                //    InterCompanyIdFinal = offer.InterCompanyId,
                //    IsAccepted = false,
                //    IsDeleted = false,
                //    ModifiedAt = DateTime.Now,
                //    ModifiedBy = offer.UserId,
                //    PartnerIdInitial = offer.PartnerId,
                //    PartnerIdFinal = offer.PartnerId,
                //    ProjectIdInitial = offer.ProjectId,
                //    ProjectIdFinal = offer.ProjectId,
                //    QuantityIni = offer.Quantity,
                //    QuantityFin = offer.QuantityRem,
                //    SubTypeIdInitial = offer.SubTypeId,
                //    SubTypeIdFinal = offer.SubTypeId,
                //    Validated = true,
                //    ValueFin1 = offer.ValueFin,
                //    ValueIni1 = offer.ValueIni,
                //    ValueFin2 = offer.ValueFin,
                //    ValueIni2 = offer.ValueIni,
                //    Guid = Guid.NewGuid()
                //};

                //_context.Add(offerOp);

            }

            entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
            _context.Update(entityType);

            _context.SaveChanges();

            return budget.Id;
        }

        public Model.Contract GetDetailsById(int id, string includes)
        {
            IQueryable<Model.Contract> query = null;
            query = GetContractQuery(includes);

            return query.AsNoTracking().Where(a => a.Id == id)
                .Include(b => b.Company)
                .Include(b => b.Employee)
                .Include(b => b.AccMonth)
                .Include(b => b.Partner)
                .SingleOrDefault();
        }

        private IQueryable<Model.Contract> GetContractQuery(string includes)
        {
            IQueryable<Model.Contract> query = null;
            query = _context.Contracts.AsNoTracking();

            return query;
        }

    }
}
