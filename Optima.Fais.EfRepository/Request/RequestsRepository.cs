using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
	public class RequestsRepository : Repository<Model.Request>, IRequestsRepository
    {

        public RequestsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Name.Contains(filter) || a.Info.Contains(filter)); })
        {
           
        }

        public IEnumerable<Model.RequestDetail> GetRequest(RequestFilter requestFilter, ColumnRequestFilter columnFilters, string includes, Sorting sorting, Paging paging, List<int?> budgetForecastIds, List<int?> requestIds, bool newBudget, bool needBudget, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Request> budgetQuery = null;
            IQueryable<RequestDetail> query = null;
            Model.AppState appState = null;

            budgetQuery = _context.Requests.AsNoTracking().AsQueryable();

            if (requestFilter.Filter != "" && requestFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Name.Contains(requestFilter.Filter) || a.Code.Contains(requestFilter.Filter) || a.Info.Contains(requestFilter.Filter)));
			if (columnFilters != null && columnFilters.Code != "" && columnFilters.Code != null) budgetQuery = budgetQuery.Where(a => (a.Code.Contains(columnFilters.Code)));

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
                    case "Request":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new RequestDetail { Request = budget });


            if (requestFilter.Role != null && requestFilter.Role != "")
            {
                if (requestFilter.Role.ToUpper() == "ADMINISTRATOR")
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
                else if (requestFilter.Role.ToUpper() == "PROCUREMENT")
				{
                    List<int?> divisionIds = new List<int?>();
                    divisionIds.Add(1482);

                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => a.Request.DivisionId != id; }, divisionIds));

                    query = query.Where(a => a.Request.AssetType.Code != "STOCK_IT");
                }
                else if (requestFilter.Role.ToUpper() == "PROC-IT")
                {
                    //if (requestFilter.InInventory)
                    //{
                    //    List<int?> costCenterIds = _context.Set<Model.EmployeeCostCenter>().AsNoTracking().Where(e => e.EmployeeId == requestFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.CostCenterId).ToList();

                    //    if (costCenterIds.Count == 0)
                    //    {
                    //        costCenterIds = new List<int?>();
                    //        costCenterIds.Add(-1);
                    //    }

                    //    query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => a.Request.CostCenterId == id; }, costCenterIds));


                    //    if ((requestFilter.CostCenterIds != null) && (requestFilter.CostCenterIds.Count > 0))
                    //    {
                    //        query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => a.Request.CostCenterId == id; }, requestFilter.CostCenterIds));
                    //    }

                        query = query.Where(a => a.Request.AssetType.Code != "STOCK_IT");
                    //}
                    //else
                    //{
                    //    List<int?> divisionIds = _context.Set<Model.EmployeeDivision>().AsNoTracking().Where(e => e.EmployeeId == requestFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.DivisionId).ToList();

                    //    if (divisionIds.Count == 0)
                    //    {
                    //        divisionIds = new List<int?>();
                    //        divisionIds.Add(-1);
                    //    }

                    //    query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => a.Request.DivisionId == id; }, divisionIds));
                    //}
                }
                else
                {
					if (requestFilter.InInventory)
					{
                        List<int?> costCenterIds = _context.Set<Model.EmployeeCostCenter>().AsNoTracking().Where(e => e.EmployeeId == requestFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.CostCenterId).ToList();

                        if (costCenterIds.Count == 0)
                        {
                            costCenterIds = new List<int?>();
                            costCenterIds.Add(-1);
                        }

                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => a.Request.CostCenterId == id; }, costCenterIds));


                        if ((requestFilter.CostCenterIds != null) && (requestFilter.CostCenterIds.Count > 0))
                        {
                            query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => a.Request.CostCenterId == id; }, requestFilter.CostCenterIds));
                        }
                    }
					else
					{
                        List<int?> divisionIds = _context.Set<Model.EmployeeDivision>().AsNoTracking().Where(e => e.EmployeeId == requestFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.DivisionId).ToList();

                        if (divisionIds.Count == 0)
                        {
                            divisionIds = new List<int?>();
                            divisionIds.Add(-1);
                        }

                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => a.Request.DivisionId == id; }, divisionIds));
                    }
                    
                }
            }

            if ((requestFilter.CompanyIds != null) && (requestFilter.CompanyIds.Count > 0))
            {
                query = query.Where(a => requestFilter.CompanyIds.Contains(a.Request.CompanyId));
            }

			if ((requestFilter.EmployeeIds != null) && (requestFilter.EmployeeIds.Count > 0))
			{
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => a.Request.EmployeeId == id; }, requestFilter.EmployeeIds));
			}

			if ((requestFilter.ReqEmployeeIds != null) && (requestFilter.ReqEmployeeIds.Count > 0))
			{
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => a.Request.OwnerId == id; }, requestFilter.ReqEmployeeIds));
			}

			if ((budgetForecastIds != null) && (budgetForecastIds.Count > 0))
			{
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => a.Request.BudgetForecastId == id; }, budgetForecastIds));
			}

			if ((requestFilter.RequestIds != null) && (requestFilter.RequestIds.Count > 0))
			{
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.RequestDetail, int?>((id) => { return a => a.Request.Id == id; }, requestFilter.RequestIds));
			}

			if (requestFilter.Type != null && requestFilter.Type != "")
            {
                if (requestFilter.Type.ToUpper() == "ME")
                {
                    query = query.Where(a => a.Request.EmployeeId == requestFilter.EmployeeId);
                }
            }

            if (newBudget)
            {
                appState = _context.Set<Model.AppState>().Where(a => a.Code == "NEED_BUDGET").FirstOrDefault();
				query = query.Where(a => a.Request.IsDeleted == false && a.Request.Validated == true && a.Request.AppStateId == appState.Id);

			} 
            else if (needBudget)
			{
				appState = _context.Set<Model.AppState>().Where(a => a.Code == "NEED_PLUS_BUDGET").FirstOrDefault();
				query = query.Where(a => a.Request.IsDeleted == false && a.Request.Validated == true && a.Request.AppStateId == appState.Id);

            }
            else
            {
				query = query.Where(a => a.Request.IsDeleted == false && a.Request.Validated == true);
			}

			

         

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
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.RequestDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.RequestDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public IEnumerable<Model.RequestDetail> GetRequestUI(RequestFilter budgetFilter, string includes, Sorting sorting, Paging paging, bool showExisting, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Request> budgetQuery = null;
            IQueryable<RequestDetail> query = null;

            budgetQuery = _context.Requests.AsNoTracking().AsQueryable();

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
                    case "Request":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new RequestDetail { Request = budget });

			if (showExisting)
			{
                query = query.Where(a => a.Request.IsDeleted == false && a.Request.IsAccepted == true);
            }
			else
			{
                query = query.Where(a => a.Request.IsDeleted == false && a.Request.IsAccepted == true && a.Request.AppStateId == 34);
            }

            



            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.RequestDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.RequestDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public IEnumerable<Model.RequestDetail> GetRequestBudgetUI(RequestFilter budgetFilter, string includes, Sorting sorting, Paging paging, bool showExisting, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Request> budgetQuery = null;
            IQueryable<RequestDetail> query = null;

            budgetQuery = _context.Requests.AsNoTracking().AsQueryable();

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
                    case "Request":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new RequestDetail { Request = budget });

            query = query.Where(a => a.Request.IsDeleted == false && a.Request.AppStateId == 40);



            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.RequestDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.RequestDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public async Task<Model.RequestResult> CreateOrUpdateRequest(RequestSave requestDto)
        {
            Model.Request request = null;
            Model.RequestOp requestOp = null;
            Model.Document document = null;
            Model.DocumentType documentType = null;
            Model.EntityType entityType = null;
			Model.EntityType entityTypeNB = null;
			Model.AppState appState = null;
			Model.AppState appStateNB = null;
			Model.AppState appStateAddBudget = null;
			Model.ApplicationUser applicationUser = null;
            Model.Inventory inventory = null;
            Model.RequestBudgetForecast requestBudgetForecast = null;
            Model.Company company = null;
            Model.EmailRequestStatus emailRequestStatus = null;
            Model.EmailType emailType = null;
            Model.Employee employee = null;
            int needBudgetCount = 0;

			company = await _context.Set<Model.Company>().Where(c => c.Code == "RO10").FirstOrDefaultAsync();
            if (company == null) return new Model.RequestResult { Success = false, Message = "Nu exista compania!", RequestId = 0 };

            inventory = await _context.Set<Model.Inventory>().Where(c => c.Active == true).FirstOrDefaultAsync(); 
            if (inventory == null) return new Model.RequestResult { Success = false, Message = "Nu exista inventar activ!", RequestId = 0 };
            
            entityType = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWREQUEST").FirstOrDefaultAsync();
            if (entityType == null) return new Model.RequestResult { Success = false, Message = "Nu exista entityType!", RequestId = 0 };

            appState = await _context.Set<Model.AppState>().Where(c => c.Code == "NEW_REQUEST").FirstOrDefaultAsync();
            if (appState == null) return new Model.RequestResult { Success = false, Message = "Nu exista stare!", RequestId = 0 };

            applicationUser = await _context.Set<Model.ApplicationUser>().Where(a => a.Id == requestDto.UserId).FirstOrDefaultAsync();
            if (applicationUser == null) return new Model.RequestResult { Success = false, Message = "Nu exista user!", RequestId = 0 };

            documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_REQUEST").FirstOrDefaultAsync();
            if (documentType == null) return new Model.RequestResult { Success = false, Message = "Nu exista tip de document!", RequestId = 0 };

			appStateNB = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "NEED_BUDGET").FirstOrDefaultAsync();
			if (appStateNB == null) return new Model.RequestResult { Success = false, Message = "Nu exista stare!", RequestId = 0 };

			appStateAddBudget = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Code == "ORDER_BOOK").FirstOrDefaultAsync();
			if (appStateAddBudget == null) return new Model.RequestResult { Success = false, Message = "Nu exista stare!", RequestId = 0 };

			DateTime? startExecution = null;
            DateTime? endExecution = null;

            //startExecution = requestDto.RangeDates != null && requestDto.RangeDates.Length > 0 && requestDto.RangeDates.ElementAt(0).HasValue ? requestDto.RangeDates.ElementAt(0).Value.AddDays(1) : (DateTime?)null;
            //endExecution = requestDto.RangeDates != null && requestDto.RangeDates.Length > 1 && requestDto.RangeDates.ElementAt(1).HasValue ? requestDto.RangeDates.ElementAt(1).Value.AddDays(1) : (DateTime?)null;

			startExecution = requestDto.StartPeriodDate;
			endExecution = requestDto.EndPeriodDate;

			var lastCode = int.Parse(entityType.Name);
            var newBudgetCode = string.Empty;

            if (lastCode.ToString().Length == 1)
            {
                newBudgetCode = "PR000000" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 2)
            {
                newBudgetCode = "PR00000" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 3)
            {
                newBudgetCode = "PR0000" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 4)
            {
                newBudgetCode = "PR000" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 5)
            {
                newBudgetCode = "PR00" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 6)
            {
                newBudgetCode = "PR0" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 7)
            {
                newBudgetCode = "PR" + entityType.Name;
            }

            document = new Model.Document()
            {
                Approved = true,
                CreatedAt = DateTime.Now,
                CreatedBy = applicationUser.Id,
                CreationDate = DateTime.Now,
                Details = requestDto.Info != null ? requestDto.Info : string.Empty,
                DocNo1 = requestDto.Info != null ? requestDto.Info : string.Empty,
                DocNo2 = requestDto.Info != null ? requestDto.Info : string.Empty,
                DocumentDate = DateTime.Now,
                DocumentTypeId = documentType.Id,
                Exported = true,
                IsDeleted = false,
                ModifiedAt = DateTime.Now,
                ModifiedBy = applicationUser.Id,
                ParentDocumentId = inventory.DocumentId,
                PartnerId = null,
                RegisterDate = DateTime.Now,
                ValidationDate = DateTime.Now
            };

            _context.Add(document);

            request = new Model.Request()
            {
                AccMonthId = inventory.AccMonthId,
                AppStateId = appState.Id,
                BudgetManagerId = inventory.BudgetManagerId,
                Code = newBudgetCode,
                CreatedAt = DateTime.Now,
                CreatedBy = applicationUser.Id,
                EndDate = requestDto.EndDate,
                StartDate = DateTime.Now,
                Info = requestDto.Info ?? string.Empty,
                IsAccepted = false,
                IsDeleted = false,
                ModifiedAt = DateTime.Now,
                ModifiedBy = applicationUser.Id,
                Name = "",
                UserId = applicationUser.Id,
                Validated = true,
                ProjectTypeId = requestDto.ProjectTypeId,
                AssetTypeId = requestDto.AssetTypeId,
                EmployeeId = applicationUser.EmployeeId,
                OwnerId = requestDto.OwnerId,
                BudgetValueNeed = requestDto.BudgetValueNeed.HasValue ? requestDto.BudgetValueNeed.Value : 0,
                CompanyId = company.Id,
                StartAccMonthId = requestDto.StartAccMonthId,
                Guid = Guid.NewGuid(),
                DivisionId = requestDto.DivisionId,
                StartExecution = startExecution,
                EndExecution = endExecution,
                Quantity = 0

            };
            _context.Add(request);

            requestOp = new Model.RequestOp()
            {
                AccMonthId = inventory.AccMonthId,
                AccSystemId = null,
                Request = request,
                BudgetManagerId = inventory.BudgetManagerId,
                RequestStateId = appState.Id,
                CreatedAt = DateTime.Now,
                CreatedBy = applicationUser.Id,
                Document = document,
                DstConfAt = DateTime.Now,
                DstConfBy = applicationUser.Id,
                InfoIni = requestDto.Info ?? string.Empty,
                InfoFin = requestDto.Info ?? string.Empty,
                IsAccepted = false,
                IsDeleted = false,
                ModifiedAt = DateTime.Now,
                ModifiedBy = applicationUser.Id,
                Validated = true,
                Guid = Guid.NewGuid(),
                CompanyId = company.Id,
            };

            _context.Add(requestOp);

            entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
            _context.Update(entityType);

            _context.SaveChanges();

            var guid = Guid.NewGuid();

            for (int i = 0; i < requestDto.BudgetForecastIds.Length; i++)
            {
                requestBudgetForecast = new Model.RequestBudgetForecast()
                {
                    RequestId = request.Id,
                    BudgetForecastId = requestDto.BudgetForecastIds[i].Id,
                    NeedBudget = requestDto.BudgetForecastIds[i].NeedBudgetValue != null && requestDto.BudgetForecastIds[i].NeedBudgetValue > 0 ? true : false,
                    NeedBudgetValue = requestDto.BudgetForecastIds[i].NeedBudgetValue ?? 0,
					AccMonthId = inventory.AccMonthId.Value,
                    BudgetManagerId = inventory.BudgetManagerId.Value,
                    Guid = guid
                };

                if (requestBudgetForecast.NeedBudget)
                {
                    needBudgetCount++;
					emailType = await _context.Set<Model.EmailType>().Where(c => c.Code == "NEED_BUDGET").FirstOrDefaultAsync();
					entityTypeNB = await _context.Set<Model.EntityType>().Where(c => c.Code == "NEED_BUDGET").FirstOrDefaultAsync();
					
					//employee = await _context.Set<Model.Employee>().AsNoTracking().Where(c => c.Email == "madalina.udrea@emag.ro" && c.IsDeleted == false).FirstOrDefaultAsync();

					emailRequestStatus = new Model.EmailRequestStatus()
					{
						AppStateId = appStateNB.Id,
						Completed = false,
						CreatedAt = DateTime.Now,
						CreatedBy = _context.UserId,
						DocumentNumber = int.Parse(entityTypeNB.Name),
						EmailSend = false,
						EmailTypeId = emailType.Id,
						EmployeeL1EmailSend = false,
						EmployeeL1ValidateAt = null,
						EmployeeL1ValidateBy = null,
						EmployeeL2EmailSend = false,
						EmployeeL2ValidateAt = null,
						EmployeeL2ValidateBy = null,
						EmployeeL3EmailSend = false,
						EmployeeL3ValidateAt = null,
						EmployeeL3ValidateBy = null,
						EmployeeL4EmailSend = false,
						EmployeeL4ValidateAt = null,
						EmployeeL4ValidateBy = null,
						EmployeeS1EmailSend = false,
						EmployeeS1ValidateAt = null,
						EmployeeS1ValidateBy = null,
						EmployeeS2EmailSend = false,
						EmployeeS2ValidateAt = null,
						EmployeeS2ValidateBy = null,
						EmployeeS3EmailSend = false,
						EmployeeS3ValidateAt = null,
						EmployeeS3ValidateBy = null,
						ErrorId = null,
						Exported = false,
						FinalValidateAt = null,
						FinalValidateBy = null,
						Guid = Guid.NewGuid(),
						GuidAll = Guid.NewGuid(),
						Info = string.Empty,
						IsAccepted = false,
						IsDeleted = false,
						ModifiedAt = DateTime.Now,
						ModifiedBy = _context.UserId,
						NotCompletedSync = false,
						NotEmployeeL1Sync = false,
						NotEmployeeL2Sync = false,
						NotEmployeeL3Sync = false,
						NotEmployeeL4Sync = false,
						NotEmployeeS1Sync = false,
						NotEmployeeS2Sync = false,
						NotEmployeeS3Sync = false,
						NotSync = false,
						Request = request,
                        RequestBudgetForecast = requestBudgetForecast,
						SyncCompletedErrorCount = 0,
						SyncEmployeeL1ErrorCount = 0,
						SyncEmployeeL2ErrorCount = 0,
						SyncEmployeeL3ErrorCount = 0,
						SyncEmployeeL4ErrorCount = 0,
						SyncEmployeeS1ErrorCount = 0,
						SyncEmployeeS2ErrorCount = 0,
						SyncEmployeeS3ErrorCount = 0,
						SyncErrorCount = 0,
						EmployeeL1EmailSkip = true,
						EmployeeL2EmailSkip = true,
						EmployeeL3EmailSkip = true,
						EmployeeL4EmailSkip = true,
						EmployeeS1EmailSkip = true,
						EmployeeS2EmailSkip = true,
						EmployeeS3EmailSkip = true,
                        NeedBudgetEmailSend = false,
                        NotNeedBudgetSync = true,
					};

					_context.Add(emailRequestStatus);
					entityTypeNB.Name = (int.Parse(entityTypeNB.Name) + 1).ToString();
					_context.Update(entityTypeNB);
				}

                if(needBudgetCount> 0)
                {
                    request.AppStateId = appStateAddBudget.Id;
                    _context.Update(request);
                }

                _context.Add(requestBudgetForecast);
                _context.SaveChanges();

            }

            return new Model.RequestResult { Success = true, Message = "P.R.- ul a fost salvat cu succes!", RequestId = request.Id };
        }

		public async Task<Model.RequestResult> UpdateRequest(RequestUpdate requestDto)
		{
			Model.Request request = null;
			//Model.RequestOp requestOp = null;
			//Model.Document document = null;
			//Model.DocumentType documentType = null;
			//Model.AppState appState = null;
			Model.ApplicationUser applicationUser = null;

			request = await _context.Set<Model.Request>().Where(c => c.Id == requestDto.Id).FirstOrDefaultAsync();
			if (request == null) return new Model.RequestResult { Success = false, Message = "Nu exista PR!", RequestId = 0 };

			//appState = await _context.Set<Model.AppState>().Where(c => c.Code == "NEW_REQUEST").FirstOrDefaultAsync();
			//if (appState == null) return new Model.RequestResult { Success = false, Message = "Nu exista stare!", RequestId = 0 };

			applicationUser = await _context.Set<Model.ApplicationUser>().Where(a => a.Id == requestDto.UserId).FirstOrDefaultAsync();
			if (applicationUser == null) return new Model.RequestResult { Success = false, Message = "Nu exista user!", RequestId = 0 };

			//documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_REQUEST").FirstOrDefaultAsync();
			//if (documentType == null) return new Model.RequestResult { Success = false, Message = "Nu exista tip de document!", RequestId = 0 };

			//document = new Model.Document()
			//{
			//	Approved = true,
			//	CreatedAt = DateTime.Now,
			//	CreatedBy = applicationUser.Id,
			//	CreationDate = DateTime.Now,
			//	Details = requestDto.Info != null ? requestDto.Info : string.Empty,
			//	DocNo1 = requestDto.Info != null ? requestDto.Info : string.Empty,
			//	DocNo2 = requestDto.Info != null ? requestDto.Info : string.Empty,
			//	DocumentDate = DateTime.Now,
			//	DocumentTypeId = documentType.Id,
			//	Exported = true,
			//	IsDeleted = false,
			//	ModifiedAt = DateTime.Now,
			//	ModifiedBy = applicationUser.Id,
			//	ParentDocumentId = inventory.DocumentId,
			//	PartnerId = null,
			//	RegisterDate = DateTime.Now,
			//	ValidationDate = DateTime.Now
			//};

			//_context.Add(document);

            request.Info = requestDto.Info;

            _context.Update(request);

			//requestOp = new Model.RequestOp()
			//{
			//	AccMonthId = inventory.AccMonthId,
			//	AccSystemId = null,
			//	Request = request,
			//	BudgetManagerId = inventory.BudgetManagerId,
			//	RequestStateId = appState.Id,
			//	CreatedAt = DateTime.Now,
			//	CreatedBy = applicationUser.Id,
			//	Document = document,
			//	DstConfAt = DateTime.Now,
			//	DstConfBy = applicationUser.Id,
			//	InfoIni = requestDto.Info ?? string.Empty,
			//	InfoFin = requestDto.Info ?? string.Empty,
			//	IsAccepted = false,
			//	IsDeleted = false,
			//	ModifiedAt = DateTime.Now,
			//	ModifiedBy = applicationUser.Id,
			//	Validated = true,
			//	Guid = Guid.NewGuid(),
			//	CompanyId = company.Id,
			//};

			//_context.Add(requestOp);

			_context.SaveChanges();


			return new Model.RequestResult { Success = true, Message = "P.R.- ul a fost actualizat cu succes!", RequestId = request.Id };
		}

		public Model.Request GetDetailsById(int id, string includes)
        {
            IQueryable<Model.Request> query = null;
            query = GetBudgetQuery(includes);

            return query.AsNoTracking().Where(a => a.Id == id)
                .Include(b => b.Division)
                .Include(b => b.Company)
                .Include(b => b.Project)
                .Include(b => b.Employee)
				.Include(b => b.Owner)
				.Include(b => b.AccMonth)
				.Include(b => b.AppState)
				.Include(b => b.BudgetManager)
				.Include(b => b.ProjectType)
				.Include(b => b.AssetType)
				.Include(b => b.User)
				.SingleOrDefault();
        }

        private IQueryable<Model.Request> GetBudgetQuery(string includes)
        {
            IQueryable<Model.Request> query = null;
            query = _context.Requests.AsNoTracking();

            return query;
        }
        public IEnumerable<Model.RequestDetail> BudgetValidate(RequestFilter budgetFilter, string includes, string userId, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Request> budgetQuery = null;
            IQueryable<RequestDetail> query = null;

            budgetQuery = _context.Requests.AsNoTracking().AsQueryable();

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
                    case "Request":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new RequestDetail { Request = budget });

            query = query.Where(a => a.Request.IsDeleted == false && a.Request.Validated == true);



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
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.RequestDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.RequestDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public IEnumerable<Model.Division> GetProjectTypeDivisionsWithBudgetBases(RequestFilter requestFilter)
        {
            var query = GetBudgetBaseQuery(requestFilter, "");
            var list = query.Select(a => a.BudgetBase.Division).Distinct();

            return list;
        }

        private IQueryable<BudgetBaseDetail> GetBudgetBaseQuery(Model.RequestFilter requestFilter, string includes)
        {
            IQueryable<Model.BudgetBase> budgetQuery = null;
            IQueryable<BudgetBaseDetail> query = null;

            budgetQuery = _context.BudgetBases.AsNoTracking().AsQueryable();

            if (requestFilter.Filter != "" && requestFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Name.Contains(requestFilter.Filter) || a.Code.Contains(requestFilter.Filter)));

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
                    case "BudgetBase":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new BudgetBaseDetail { BudgetBase = budget });


            query = query.Where(a => a.BudgetBase.IsDeleted == false && a.BudgetBase.Validated == true);

            return query;
        }

		public async Task<RequestResult> NeedBudget(NeedBudget needBudget)
		{
            Model.ProjectTypeDivision projectTypeDivision = null;
            // Model.CostCenter costCenter = null;
            Model.Inventory inventory = null;
            Model.EntityType entityType = null;
            Model.ApplicationUser applicationUser = null;
            Model.DocumentType documentType = null;
            Model.Document document = null;
            Model.Request request = null;
            Model.RequestOp requestOp = null;
            Model.AppState appState = null;
            Model.AccSystem accSystem = null;

            inventory = await _context.Set<Model.Inventory>().Where(a => a.Active == true).SingleAsync();
            projectTypeDivision = await _context.Set<Model.ProjectTypeDivision>().Where(a => a.Id == needBudget.ProjectTypeDivisionId).SingleAsync();
            // costCenter = await _context.Set<Model.CostCenter>().Where(a => a.Id == needBudget.CostCenterId).SingleAsync();
            //assetType = await _context.Set<Model.AssetType>().Where(a => a.Id == needBudget.AssetTypeId).SingleOrDefaultAsync();
            appState = await _context.Set<Model.AppState>().Where(a => a.Code == "REQUEST_BOOK" && a.IsDeleted == false).SingleAsync();
            accSystem = await _context.Set<Model.AccSystem>().Where(a => a.Code == "RON" && a.IsDeleted == false).SingleAsync();

            entityType = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWREQUEST").SingleAsync();
            applicationUser = await _context.Set<Model.ApplicationUser>().Where(a => a.Id == needBudget.UserId).SingleAsync();

            var lastCode = int.Parse(entityType.Name);
            var newBudgetCode = string.Empty;

            if (lastCode.ToString().Length == 1)
            {
                newBudgetCode = "PR000000" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 2)
            {
                newBudgetCode = "PR00000" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 3)
            {
                newBudgetCode = "PR0000" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 4)
            {
                newBudgetCode = "PR000" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 5)
            {
                newBudgetCode = "PR00" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 6)
            {
                newBudgetCode = "PR0" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 7)
            {
                newBudgetCode = "PR" + entityType.Name;
            }

            documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_REQUEST").SingleAsync();

            document = new Model.Document()
            {
                Approved = true,
                CostCenterId = needBudget.CostCenterId,
                CreatedAt = DateTime.Now,
                CreatedBy = needBudget.UserId,
                CreationDate = DateTime.Now,
                Details = needBudget.Info != null ? needBudget.Info : string.Empty,
                DocNo1 = needBudget.Info != null ? needBudget.Info : string.Empty,
                DocNo2 = needBudget.Info != null ? needBudget.Info : string.Empty,
                DocumentDate = DateTime.Now,
                DocumentTypeId = documentType.Id,
                Exported = true,
                IsDeleted = false,
                ModifiedAt = DateTime.Now,
                ModifiedBy = needBudget.UserId,
                ParentDocumentId = null,
                PartnerId = null,
                RegisterDate = DateTime.Now,
                ValidationDate = DateTime.Now
            };

            _context.Add(document);

            request = new Model.Request()
            {
                AccMonthId = inventory.AccMonthId,
                AppStateId = appState.Id,
                BudgetManagerId = inventory.BudgetManagerId,
                Code = newBudgetCode,
                CostCenterId = needBudget.CostCenterId,
                CreatedAt = DateTime.Now,
                CreatedBy = needBudget.UserId,
                EndDate = needBudget.EndDate,
                StartDate = DateTime.Now,
                Info = needBudget.Info,
                IsAccepted = false,
                IsDeleted = false,
                ModifiedAt = DateTime.Now,
                ModifiedBy = needBudget.UserId,
                Name = "",
                UserId = needBudget.UserId,
                Validated = true,
                BudgetId = null,
                BudgetBaseId = null,
                ProjectId = null,
                ProjectTypeId = projectTypeDivision.ProjectTypeId,
                AssetTypeId = needBudget.AssetTypeId,
                EmployeeId = needBudget.EmployeeId,
                // CompanyId = costCenter.CompanyId,
                Guid = Guid.NewGuid(),
                OwnerId = needBudget.OwnerId != null && needBudget.OwnerId > 0 ? needBudget.OwnerId : needBudget.EmployeeId,
                BudgetValueNeed = needBudget.BudgetValueNeed,
                StartAccMonthId = needBudget.StartAccMonthId,
                DivisionId = projectTypeDivision.DivisionId

            };
            _context.Add(request);

            requestOp = new Model.RequestOp()
            {
                AccMonthId = inventory.AccMonthId,
                AccSystemId = accSystem.Id,
                Request = request,
                BudgetManagerId = null,
                RequestStateId = appState.Id,
                CostCenterIdInitial = needBudget.CostCenterId,
                CostCenterIdFinal = needBudget.CostCenterId,
                CreatedAt = DateTime.Now,
                CreatedBy = needBudget.UserId,
                Document = document,
                DstConfAt = DateTime.Now,
                DstConfBy = needBudget.UserId,
                InfoIni = needBudget.Info,
                InfoFin = needBudget.Info,
                IsAccepted = false,
                IsDeleted = false,
                ModifiedAt = DateTime.Now,
                ModifiedBy = needBudget.UserId,
                Validated = true,
                Guid = Guid.NewGuid(),
                BudgetIdInitial = null,
                BudgetIdFinal = null,
            };

            _context.Add(requestOp);

            entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
            _context.Update(entityType);

            _context.SaveChanges();

            if(request.Id > 0)
			{
                return new Model.RequestResult { Success = true, Message = "Ticketul a fost salvat cu succes!", RequestId = request.Id };
            }
			else
			{
                return new Model.RequestResult { Success = true, Message = "Eroare salvare ticket!", RequestId = 0 };
            }

        }

		public async Task<Model.RequestResult> RequestBudgetForecastUpdate(RequestBudgetForecastUpdate orderDto)
		{
			Model.Order order = null;
			Model.Request request = null;
			Model.BudgetForecast budgetForecast = null;
			//Model.RequestBudgetForecast requestBudgetForecast = null;
			//List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
			List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
			List<Model.RequestBudgetForecast> requestBFs = null;
			List<string> materials = null;
			Model.Contract contract = null;
			//Model.RequestOp requestOp = null;
			//Model.Document document = null;
			//Model.DocumentType documentType = null;
			//Model.AppState appState = null;
			Model.ApplicationUser applicationUser = null;
			int sumQuantity = 0;
			int sumTotalOrderQuantity = 0;

			request = await _context.Set<Model.Request>().Where(c => c.Id == orderDto.RequestId).FirstOrDefaultAsync();
			if (request == null) return new Model.RequestResult { Success = false, Message = "Nu exista P.R.!", RequestId = 0 };

			order = await _context.Set<Model.Order>().Include(a => a.Offer).Where(c => c.Offer.RequestId == request.Id).FirstOrDefaultAsync();

			budgetForecast = await _context.Set<Model.BudgetForecast>().Where(c => c.Id == orderDto.BudgetForecastId).FirstOrDefaultAsync();
			if (budgetForecast == null) return new Model.RequestResult { Success = false, Message = "Nu exista BUGET!", RequestId = 0 };

			applicationUser = await _context.Set<Model.ApplicationUser>().Where(a => a.Id == orderDto.UserId).FirstOrDefaultAsync();
			if (applicationUser == null) return new Model.RequestResult { Success = false, Message = "Nu exista user!", RequestId = 0 };

			requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>().Include(a => a.OfferType).Where(a => a.RequestId == request.Id).ToListAsync();
			if (requestBudgetForecasts.Count == 0) return new Model.RequestResult { Success = false, Message = "Nu exista BUGETE!", RequestId = 0 };

			for (int i = 0; i < requestBudgetForecasts.Count; i++)
			{
				requestBudgetForecasts[i].BudgetForecastId = budgetForecast.Id;
				materials = await _context.Set<Model.RequestBudgetForecastMaterial>()
				   .Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecasts[i].Id)
				   .Select(a => a.Material.Code)
				   .ToListAsync();

				//requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
				//                .Include(a => a.OfferType)
				//                .Where(a => a.RequestBudgetForecastId == requestBudgetForecasts[i].Id)
				//                .ToListAsync();


				var mat = string.Join(", ", materials);

				requestBudgetForecasts[i].Materials = mat;
				_context.Update(requestBudgetForecasts[i]);
				_context.SaveChanges();


				// UPDATE VALUES //



				if (requestBudgetForecasts[i].OfferType != null && requestBudgetForecasts[i].OfferType.Code == "O-V")
				{
					sumQuantity = 1;
				}
				else
				{
					sumQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecasts[i].Id).Sum(a => a.Quantity);
				}



				decimal sumValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecasts[i].Id).Sum(a => a.Value);
				decimal sumValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecasts[i].Id).Sum(a => a.ValueRon);

				requestBudgetForecasts[i].Quantity = sumQuantity;
				requestBudgetForecasts[i].Value = sumValue;
				requestBudgetForecasts[i].ValueRon = sumValueRon;

				_context.Update(requestBudgetForecasts[i]);
				_context.SaveChanges();

				requestBFs = await _context.Set<Model.RequestBudgetForecast>().Include(b => b.BudgetForecast).Where(a => a.Guid == requestBudgetForecasts[i].Guid).ToListAsync();

				if (requestBudgetForecasts[i].OfferType.Code == "O-V")
				{
					sumTotalOrderQuantity = 1;
				}
				else
				{
					sumTotalOrderQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecasts[i].Guid).Sum(a => a.Quantity);
				}


				decimal sumTotalOrderValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecasts[i].Guid).Sum(a => a.Value);
				decimal sumTotalOrderValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecasts[i].Guid).Sum(a => a.ValueRon);

				for (int o = 0; o < requestBFs.Count; o++)
				{
					requestBFs[o].NeedBudget = false;
					requestBFs[o].NeedBudgetValue = 0;
					requestBFs[o].TotalOrderQuantity = sumTotalOrderQuantity;
					requestBFs[o].TotalOrderValue = sumTotalOrderValue;
					requestBFs[o].TotalOrderValueRon = sumTotalOrderValueRon;

					requestBFs[o].NeedContract = false;
					requestBFs[o].NeedContractValue = 0;

					contract = await _context.Set<Model.Contract>().Include(c => c.ContractAmount).Where(u => u.Id == requestBFs[o].ContractId).SingleAsync();

					if (contract.ContractAmount.AmountRonRem < requestBFs[0].TotalOrderValueRon)
					{
						requestBFs[o].NeedContract = true;
						requestBFs[o].NeedContractValue = requestBFs[o].TotalOrderValueRon - contract.ContractAmount.AmountRonRem;
					}

					if (requestBFs[o].BudgetForecast.TotalRem < requestBFs[o].ValueRon)
					{
						requestBFs[o].NeedBudget = true;
						requestBFs[o].NeedBudgetValue = requestBFs[o].ValueRon - requestBFs[o].BudgetForecast.TotalRem;
					}

					_context.Update(requestBFs[o]);
					_context.SaveChanges();
				}

				// UPDATE VALUES //

			}
			//documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_REQUEST").FirstOrDefaultAsync();
			//if (documentType == null) return new Model.RequestResult { Success = false, Message = "Nu exista tip de document!", RequestId = 0 };

			//document = new Model.Document()
			//{
			//	Approved = true,
			//	CreatedAt = DateTime.Now,
			//	CreatedBy = applicationUser.Id,
			//	CreationDate = DateTime.Now,
			//	Details = requestDto.Info != null ? requestDto.Info : string.Empty,
			//	DocNo1 = requestDto.Info != null ? requestDto.Info : string.Empty,
			//	DocNo2 = requestDto.Info != null ? requestDto.Info : string.Empty,
			//	DocumentDate = DateTime.Now,
			//	DocumentTypeId = documentType.Id,
			//	Exported = true,
			//	IsDeleted = false,
			//	ModifiedAt = DateTime.Now,
			//	ModifiedBy = applicationUser.Id,
			//	ParentDocumentId = inventory.DocumentId,
			//	PartnerId = null,
			//	RegisterDate = DateTime.Now,
			//	ValidationDate = DateTime.Now
			//};

			//_context.Add(document);

			//_context.Update(order);

			//requestOp = new Model.RequestOp()
			//{
			//	AccMonthId = inventory.AccMonthId,
			//	AccSystemId = null,
			//	Request = request,
			//	BudgetManagerId = inventory.BudgetManagerId,
			//	RequestStateId = appState.Id,
			//	CreatedAt = DateTime.Now,
			//	CreatedBy = applicationUser.Id,
			//	Document = document,
			//	DstConfAt = DateTime.Now,
			//	DstConfBy = applicationUser.Id,
			//	InfoIni = requestDto.Info ?? string.Empty,
			//	InfoFin = requestDto.Info ?? string.Empty,
			//	IsAccepted = false,
			//	IsDeleted = false,
			//	ModifiedAt = DateTime.Now,
			//	ModifiedBy = applicationUser.Id,
			//	Validated = true,
			//	Guid = Guid.NewGuid(),
			//	CompanyId = company.Id,
			//};

			//_context.Add(requestOp);

			//_context.SaveChanges();





			return new Model.RequestResult { Success = true, Message = "P.R.- ul a fost actualizat cu succes!", RequestId = request.Id };
		}
	}
}
