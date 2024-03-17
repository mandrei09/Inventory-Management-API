using System;
using System.Collections.Generic;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Linq;
using Optima.Fais.Model;
using Microsoft.EntityFrameworkCore;
using Optima.Fais.Model.Utils;
using System.Collections;

namespace Optima.Fais.EfRepository
{
    public class AssetOpsRepository : Repository<Model.AssetOp>, IAssetOpsRepository
    {
        public AssetOpsRepository(ApplicationDbContext context)
            : base(context, null)
        {
            _context= context;
        }

        public override void Create(AssetOp item, string createdBy = null)
        {
            base.Create(item, createdBy);
        }

        //public IEnumerable<Dto.AssetOpSd> GetByAsset(int assetId)
        //{
        //    IQueryable<Dto.AssetOpSd> query = null;
        //    IQueryable<Model.Asset> assets = _context.Set<Model.Asset>().Where(a => a.Id == assetId).AsQueryable();
        //    IQueryable<Model.Document> documents = _context.Set<Model.Document>().AsQueryable();
        //    IQueryable<Model.DocumentType> documentTypes = _context.Set<Model.DocumentType>().AsQueryable();
        //    IQueryable<Model.AssetOp> assetOps = _context.Set<Model.AssetOp>().AsQueryable();
        //    IQueryable<Model.Department> departmentsFinal = _context.Set<Model.Department>().AsQueryable();
        //    IQueryable<Model.Employee> esF = _context.Set<Model.Employee>().AsQueryable();
        //    IQueryable<Model.Location> lsF = _context.Set<Model.Location>().AsQueryable();
        //    IQueryable<Model.Room> rsF = _context.Set<Model.Room>().AsQueryable();
        //    IQueryable<Model.AssetState> asS = _context.Set<Model.AssetState>().AsQueryable();

        //    query =
        //        from assetOp in assetOps
        //        join asset in assets on assetOp.AssetId equals asset.Id
        //        join document in documents on assetOp.DocumentId equals document.Id
        //        join documentType in documentTypes on document.DocumentTypeId equals documentType.Id

        //        join eF in esF on assetOp.EmployeeIdFinal equals eF.Id into employeesFinal
        //        from employeeFinal in employeesFinal.DefaultIfEmpty()

        //        join rF in rsF on assetOp.RoomIdFinal equals rF.Id into roomsFinal
        //        from roomFinal in roomsFinal.DefaultIfEmpty()
        //        join lF in lsF on roomFinal.LocationId equals lF.Id into locationsFinal
        //        from locationFinal in locationsFinal.DefaultIfEmpty()

        //        join aS in asS on assetOp.AssetStateIdFinal equals aS.Id into assetStatesFinal
        //        from assetState in assetStatesFinal.DefaultIfEmpty()

        //        select new Dto.AssetOpSd()
        //        {
        //            Id = assetOp.Id,

        //            DocumentId = document.Id,
        //            DocumentType = documentType.Name,
        //            DocumentTypeCode = documentType.Code,
        //            DocumentDetails = document.Details,

        //            EmployeeId = assetOp.EmployeeIdFinal,
        //            InternalCode = (employeeFinal != null ? employeeFinal.InternalCode : ""),
        //            FirstName = (employeeFinal != null ? employeeFinal.FirstName : ""),
        //            LastName = (employeeFinal != null ? employeeFinal.LastName : ""),

        //            LocationId = (locationFinal != null ? (int?)locationFinal.Id : null),
        //            LocationCode = (locationFinal != null ? locationFinal.Code : null),
        //            LocationName = (locationFinal != null ? locationFinal.Name : null),
        //            RoomId = assetOp.RoomIdFinal,
        //            RoomCode = (roomFinal != null ? roomFinal.Code : null),
        //            RoomName = (roomFinal != null ? roomFinal.Name : null),

        //            StateCode = assetState != null ? assetState.Code : string.Empty,
        //            State = assetState != null ? assetState.Name : string.Empty,
        //            ValidationDate = document.ValidationDate
        //        };

        //    return query.ToList();
        //}

        public IEnumerable<Model.AssetOp> GetByAsset(int assetId)
        {
            var query = _context.AssetOps
                .AsNoTracking()
                .Include(i => i.Asset)
                .Include(i => i.AssetOpState)
                .Include(i => i.Document)
                    .ThenInclude(d => d.DocumentType)
                .Include(i => i.RoomInitial)
                    //.ThenInclude(r => r.Location)
                    //    .ThenInclude(l => l.Region)
                .Include(i => i.EmployeeInitial)
                .Include(i => i.CostCenterInitial)
                    //.ThenInclude(c => c.AdmCenter)
                .Include(i => i.RoomFinal)
                    //.ThenInclude(r => r.Location)
                    //    .ThenInclude(l => l.Region)
                .Include(i => i.EmployeeFinal)
                .Include(i => i.CostCenterFinal)
                    //.ThenInclude(c => c.AdmCenter)
                .AsQueryable()
                .Where(a => a.AssetId == assetId);

            return query.ToList();
        }

       
        public IEnumerable<AssetOpExport> ExportAssetOp(AssetFilter assetFilter, List<PropertyFilter> propFilters, string assetOpState)
        {
            IQueryable<Model.AssetOp> query = null;
            query = _context.AssetOps.AsNoTracking();
            query = query
                   .Include(i => i.Asset)
                   .Include(i => i.AssetOpState)
                   .Include(i => i.Document)
                       .ThenInclude(d => d.DocumentType)
                   .Include(i => i.RoomInitial)
                       .ThenInclude(r => r.Location)
                           .ThenInclude(l => l.Region)
                   .Include(i => i.CostCenterInitial)
                   .Include(i => i.EmployeeInitial)
                   .Include(i => i.RoomFinal)
                       .ThenInclude(r => r.Location)
                           .ThenInclude(l => l.Region)
                   .Include(i => i.CostCenterFinal)
                   .Include(i => i.BudgetManagerInitial)
                   .Include(i => i.ProjectInitial)
                   .Include(i => i.DimensionInitial)
                   .Include(i => i.AssetNatureInitial)
                   .Include(i => i.BudgetManagerFinal)
                   .Include(i => i.ProjectFinal)
                   .Include(i => i.DimensionFinal)
                   .Include(i => i.AssetNatureFinal)
                   .Include(i => i.EmployeeFinal)
                   .AsQueryable();
            if ((assetFilter.EmployeeIds != null) && (assetFilter.EmployeeIds.Count > 0))
            {
                query = query.Where(a => assetFilter.EmployeeIds.Contains(a.EmployeeIdInitial) || assetFilter.EmployeeIds.Contains(a.EmployeeIdFinal));
            }


            if ((assetFilter.RoomIds != null) && (assetFilter.RoomIds.Count > 0))
            {
                query = query.Where(a => assetFilter.RoomIds.Contains(a.RoomIdInitial) || assetFilter.RoomIds.Contains(a.RoomIdFinal));
            }

            else
            {
                if ((assetFilter.LocationIds != null) && (assetFilter.LocationIds.Count > 0))
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.RoomInitial.Location.Id == id || a.RoomFinal.Location.Id == id; }, assetFilter.LocationIds));
                }
                else
                {
                    if ((assetFilter.RegionIds != null) && (assetFilter.RegionIds.Count > 0))
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.RoomInitial.Location.Region.Id == id || a.RoomFinal.Location.Region.Id == id; }, assetFilter.RegionIds));
                    }

                    if ((assetFilter.AdmCenterIds != null) && (assetFilter.AdmCenterIds.Count > 0))
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.RoomInitial.Location.AdmCenter.Id == id || a.RoomFinal.Location.AdmCenter.Id == id; }, assetFilter.AdmCenterIds));
                    }
                }
            }


            if ((assetFilter.CostCenterIds != null) && (assetFilter.CostCenterIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.CostCenterIdInitial == id || a.CostCenterIdInitial == id; }, assetFilter.CostCenterIds));
            }
            if ((assetFilter.DepartmentIds != null) && (assetFilter.DepartmentIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.DepartmentIdInitial == id || a.DepartmentIdInitial == id; }, assetFilter.DepartmentIds));
            }
            if ((assetFilter.ProjectIds != null) && (assetFilter.ProjectIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.ProjectIdInitial == id || a.ProjectIdInitial == id; }, assetFilter.ProjectIds));
            }
            if ((assetFilter.DivisionIds != null) && (assetFilter.DivisionIds.Count > 0))
            {
                var depIds = (from a in _context.AssetOps
                              join dp in _context.Departments on a.DepartmentIdInitial equals dp.Id into dpJoin
                              from dp in dpJoin.DefaultIfEmpty()
                              join dv in _context.Divisions on dp.Id equals dv.DepartmentId into dvJoin
                              from dv in dvJoin.DefaultIfEmpty()
                              where assetFilter.DivisionIds.Contains(dv.Id)
                              select a.DepartmentIdInitial).ToList();
                query = query.Where(a => a.DepartmentIdInitial != null && depIds.Contains(a.DepartmentIdInitial.Value));

            }
            //string assetOpState = "Toate";
             if (assetOpState != null)
            {
                switch (assetOpState)
                {
                    case "Toate":
                        query = query.Where(a => a.AssetOpStateId == 3 || a.AssetOpStateId == 4 || a.AssetOpStateId == 43 || a.AssetOpStateId == 44 || a.AssetOpStateId == 45 || a.AssetOpStateId == 46 || a.AssetOpStateId == 50);
                        break;
                    case "In Transfer":
                        query = query.Where(a => a.AssetOpStateId == 3);
                        break;
                    case "Validate":
                        query = query.Where(a => a.AssetOpStateId == 4);
                        break;
                    case "Finalizat":
                        query = query.Where(a => a.AssetOpStateId == 44);
                        break;
                    case "Aprobare Manager":
                        query = query.Where(a => a.AssetOpStateId == 46);
                        break;
                    case "Acceptare primitor":
                        query = query.Where(a => a.AssetOpStateId == 45);
                        break;
                    case "Acceptare stock":
                        query = query.Where(a => a.AssetOpStateId == 43);
                        break;
                    case "Refuz primitor":
                        query = query.Where(a => a.AssetOpStateId == 50);
                        break;
                    default:
                        //query = query.Where(a => a.AssetOpState.Id > 0);
                        break;
                }
            }

                Model.DocumentType documentType = _context.Set<DocumentType>().Where(d => d.Code == "TRANSFER").Single();
                query = query.Where(a => a.Document.DocumentTypeId == 2 || a.Document.DocumentTypeId == 6 || a.Document.DocumentTypeId == 11 || a.Document.DocumentTypeId == 20);  // BNR
            
            if (assetFilter.Filter != "" && assetFilter.Filter != null) query = query.Where(a => (a.Asset.InvNo.Contains(assetFilter.Filter) || a.Asset.ERPCode.Contains(assetFilter.Filter) || a.Asset.Name.Contains(assetFilter.Filter) || a.Asset.SerialNumber.Contains(assetFilter.Filter)));

            

            if (assetFilter.FromDate != null)
            {
                query = query.Where(a => a.ModifiedAt >= assetFilter.FromDate);
            }

            if (assetFilter.ToDate != null)
            {
                query = query.Where(a => a.ModifiedAt <= assetFilter.ToDate);
            }

            query = query.Where(a => a.IsDeleted == false);
            string sortColumn = "modifiedAt";
            string sortDirection = "asc";

            if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetOp>(sortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetOp>(sortColumn));
            }

            var list = query.Select(a => new AssetOpExport()
            {
                Nbon = Convert.ToInt64(a.ValueAdd).ToString() != "0" ? Convert.ToInt64(a.ValueAdd).ToString() : "",
                Type = a.Document.DocumentType.Name.ToString(),
                DateTransfer = a.SrcConfAt.ToString(),
                Stare = a.AssetOpState.Name.ToString(),
                Description = a.Asset.Name.ToString(),
                Asset = a.Asset.InvNo.ToString(),
                FromDateTemporary = "",
                ToDateTemporary = "",
                EmailPredator = a.EmployeeInitial.Email.ToString(),
                EmailTaker = a.EmployeeFinal.Email.ToString(),
                ManageTaker = a.EmployeeFinal.Manager.Email.ToString(),
                CCPredator = a.CostCenterInitial.Code.ToString(),
                CCTaker = a.CostCenterFinal.Code.ToString(),

            }).ToList();

            return list;
        }

        public IEnumerable<Model.AssetOp> GetFiltered(AssetFilter assetFilter,string includes, int? assetId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string sortColumn, string sortDirection, int? page, int? pageSize, out int count)
        {

            IQueryable<Model.AssetOp> query = null;
            IQueryable<Model.InventoryAsset> queryInv = null;
            query = _context.AssetOps.AsNoTracking();

            if (includes != null)
            {
                foreach (var includeProperty in includes.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            else
            {
                query = query
                    .Include(i => i.Asset)
                    .Include(i => i.AssetOpState)
                    .Include(i => i.Document)
                        .ThenInclude(d => d.DocumentType)
                    .Include(i => i.RoomInitial)
                        .ThenInclude(r => r.Location)
                            .ThenInclude(l => l.Region)
                    .Include(i => i.CostCenterInitial)
                    .Include(i => i.EmployeeInitial)
                    .Include(i => i.RoomFinal)
                        .ThenInclude(r => r.Location)
                            .ThenInclude(l => l.Region)
                    .Include(i => i.CostCenterFinal)
                    .Include(i => i.BudgetManagerInitial)
                    .Include(i => i.ProjectInitial)
                    .Include(i => i.DimensionInitial)
                    .Include(i => i.AssetNatureInitial)
                    .Include(i => i.BudgetManagerFinal)
                    .Include(i => i.ProjectFinal)
                    .Include(i => i.DimensionFinal)
                    .Include(i => i.AssetNatureFinal)
                    .Include(i => i.EmployeeFinal)
                    .AsQueryable();
             
            }

            if ((assetFilter.EmployeeIds != null) && (assetFilter.EmployeeIds.Count > 0))
            {
                query = query.Where(a => assetFilter.EmployeeIds.Contains(a.EmployeeIdInitial) || assetFilter.EmployeeIds.Contains(a.EmployeeIdFinal));
            }


            if ((assetFilter.RoomIds != null) && (assetFilter.RoomIds.Count > 0))
            {
                query = query.Where(a => assetFilter.RoomIds.Contains(a.RoomIdInitial) || assetFilter.RoomIds.Contains(a.RoomIdFinal));
            }
            else
            {
                if ((assetFilter.LocationIds != null) && (assetFilter.LocationIds.Count > 0))
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.RoomInitial.Location.Id == id || a.RoomFinal.Location.Id == id; }, assetFilter.LocationIds));
                }
                else
                {
                    if ((assetFilter.RegionIds != null) && (assetFilter.RegionIds.Count > 0))
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.RoomInitial.Location.Region.Id == id || a.RoomFinal.Location.Region.Id == id; }, assetFilter.RegionIds));
                    }

                    if ((assetFilter.AdmCenterIds != null) && (assetFilter.AdmCenterIds.Count > 0))
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.RoomInitial.Location.AdmCenter.Id == id || a.RoomFinal.Location.AdmCenter.Id == id; }, assetFilter.AdmCenterIds));
                    }
                }
            }

            if (assetId.HasValue) query = query.Where(a => a.AssetId == assetId);


            if ((assetFilter.CostCenterIds != null) && (assetFilter.CostCenterIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.CostCenterIdInitial == id || a.CostCenterIdInitial == id; }, assetFilter.CostCenterIds));
            }
            if ((assetFilter.DepartmentIds != null) && (assetFilter.DepartmentIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.DepartmentIdInitial == id || a.DepartmentIdInitial == id; }, assetFilter.DepartmentIds));
            }
            if ((assetFilter.ProjectIds != null) && (assetFilter.ProjectIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.ProjectIdInitial == id || a.ProjectIdInitial == id; }, assetFilter.ProjectIds));
            }
            if ((assetFilter.DivisionIds != null) && (assetFilter.DivisionIds.Count > 0))
            {
                var depIds = (from a in _context.AssetOps
                        join dp in _context.Departments on a.DepartmentIdInitial equals dp.Id into dpJoin
                        from dp in dpJoin.DefaultIfEmpty()
                        join dv in _context.Divisions on dp.Id equals dv.DepartmentId into dvJoin
                        from dv in dvJoin.DefaultIfEmpty()
                        where assetFilter.DivisionIds.Contains(dv.Id)
                        select a.DepartmentIdInitial).ToList();
                query = query.Where(a => a.DepartmentIdInitial != null && depIds.Contains(a.DepartmentIdInitial.Value));
               
            }
            //if (assetOpState != null) query = query.Where(a => a.AssetOpState.Name == assetOpState);

            if (assetOpState != null)
            {
                switch (assetOpState)
                {
                    case "Toate":
                        //query = query.Where(a => a.AssetOpStateId == 3 || a.AssetOpStateId == 4 || a.AssetOpStateId == 43 || a.AssetOpStateId == 44 || a.AssetOpStateId == 45 || a.AssetOpStateId == 46 || a.AssetOpStateId == 50);
                        query = query.Where(a => a.AssetOpStateId == 3 || a.AssetOpStateId == 4 || a.AssetOpStateId == 43 || a.AssetOpStateId == 44 || a.AssetOpStateId == 45 || a.AssetOpStateId == 46 || a.AssetOpStateId == 50);
                        break;
                    case "In Transfer":
                        query = query.Where(a => a.AssetOpStateId == 3);
                        break;
                    case "Validate":
                        query = query.Where(a => a.AssetOpStateId == 4);
                        break;
					case "Finalizat":
						query = query.Where(a => a.AssetOpStateId == 44);
						break;
					case "Aprobare Manager":
						query = query.Where(a => a.AssetOpStateId == 46);
						break;
					case "Acceptare primitor":
						query = query.Where(a => a.AssetOpStateId == 45);
						break;
					case "Acceptare stock":
						query = query.Where(a => a.AssetOpStateId == 43);
						break;
					case "Refuz primitor":
						query = query.Where(a => a.AssetOpStateId == 50);
						break;
					default:
                        //query = query.Where(a => a.AssetOpState.Id > 0);
                        break;
                }
            }

            if ((documentTypeCode != null) && (documentTypeCode.Length > 0))
            {
                Model.DocumentType documentType = _context.Set<DocumentType>().Where(d => d.Code == documentTypeCode).Single();
                // query = query.Where(a => a.Document.DocumentTypeId == documentType.Id);  // ORIGINAL

                // query = query.Where(a => a.Document.DocumentTypeId == documentType.Id || a.Document.DocumentTypeId == 6);  //  PIRAEUS
                // query = query.Where(a => (a.RoomIdInitial != a.RoomIdFinal)  || (a.AssetOpStateId == 3  && a.InvStateIdFinal != 1)); // PIRAEUS
                //query = query.Where(a => a.Document.DocumentTypeId == documentType.Id || a.Document.DocumentTypeId == 2 || a.Document.DocumentTypeId == 7);  // BNR\
                //query = query.Where(a => a.Document.DocumentTypeId == 2 || a.Document.DocumentTypeId == 6 || a.Document.DocumentTypeId == 11 || a.Document.DocumentTypeId == 20);  // BNR
                query = query.Where(a => a.Document.DocumentTypeId == 2 || a.Document.DocumentTypeId == 6 || a.Document.DocumentTypeId == 11 || a.Document.DocumentTypeId == 20);  // BNR                                                                                                                                                              //  query = query.Where(a => a.Document.DocumentTypeId == documentType.Id || a.Document.DocumentTypeId != 1 || a.Document.DocumentTypeId != 5 || a.Document.DocumentTypeId != 4);  // RINGIER
                query = query.Where(a => a.Document.DocumentTypeId == 2 || a.Document.DocumentTypeId == 6 || a.Document.DocumentTypeId == 11 || a.Document.DocumentTypeId == 20);  // BNR                                                                                                                                                                                                                 // query = query.Where(a => (a.RoomIdFinal != a.RoomIdInitial && a.RoomFinal.Location.Id != a.RoomInitial.Location.Id && a.RoomFinal.Location.Region.Id != a.RoomInitial.Location.Region.Id) || (a.EmployeeIdFinal != a.EmployeeIdInitial)); // BNR
                                                                                                                                                                                   //query = query.Where(a => (a.RoomIdFinal != a.RoomIdInitial) || (a.EmployeeIdFinal != a.EmployeeIdInitial) || (a.InvStateIdInitial != a.InvStateIdFinal)); // BNR
                                                                                                                                                                                  // query = query.Where(a => ((a.RoomIdFinal != a.RoomIdInitial) && a.Document.DocumentTypeId != 5) || a.Document.DocumentTypeId == 6); // OTP
            }


            if (assetFilter.Filter != "" && assetFilter.Filter != null) query = query.Where(a => (a.Asset.InvNo.Contains(assetFilter.Filter) || a.Asset.ERPCode.Contains(assetFilter.Filter) || a.Asset.Name.Contains(assetFilter.Filter) || a.Asset.SerialNumber.Contains(assetFilter.Filter)));

            //if ((documentTypeCode != null) && (documentTypeCode.Length > 0))
            // {
            //    Model.DocumentType documentType = _context.Set<DocumentType>().Where(d => d.Code == documentTypeCode).Single();
            //    // query = query.Where(a => a.Document.DocumentTypeId == documentType.Id);  // ORIGINAL

            //     // query = query.Where(a => a.Document.DocumentTypeId == documentType.Id || a.Document.DocumentTypeId == 6);  //  PIRAEUS
            //      // query = query.Where(a => (a.RoomIdInitial != a.RoomIdFinal)  || (a.AssetOpStateId == 3  && a.InvStateIdFinal != 1)); // PIRAEUS
            //     //query = query.Where(a => a.Document.DocumentTypeId == documentType.Id || a.Document.DocumentTypeId == 2 || a.Document.DocumentTypeId == 7);  // BNR\
            //     query = query.Where(a => a.Document.DocumentTypeId == 2 || a.Document.DocumentTypeId == 6 || a.Document.DocumentTypeId == 9 || a.Document.DocumentTypeId == 11);  // BNR
            //   //  query = query.Where(a => a.Document.DocumentTypeId == documentType.Id || a.Document.DocumentTypeId != 1 || a.Document.DocumentTypeId != 5 || a.Document.DocumentTypeId != 4);  // RINGIER
            //    // query = query.Where(a => (a.RoomIdFinal != a.RoomIdInitial && a.RoomFinal.Location.Id != a.RoomInitial.Location.Id && a.RoomFinal.Location.Region.Id != a.RoomInitial.Location.Region.Id) || (a.EmployeeIdFinal != a.EmployeeIdInitial)); // BNR
            //    //query = query.Where(a => (a.RoomIdFinal != a.RoomIdInitial) || (a.EmployeeIdFinal != a.EmployeeIdInitial) || (a.InvStateIdInitial != a.InvStateIdFinal)); // BNR
            //   // query = query.Where(a => ((a.RoomIdFinal != a.RoomIdInitial) && a.Document.DocumentTypeId != 5) || a.Document.DocumentTypeId == 6); // OTP
                
            //}

            //if (startDate.Year > 1900 || endDate.Year > 1900)
            //{
            //    query = query.Where(a => a.ModifiedAt >= startDate && a.ModifiedAt <= (endDate.Year > 1900 ? endDate : DateTime.Now));
            //}

            //if (startDate.Year > 1900) query = query.Where(a => a.ModifiedAt >= startDate);
            //if (endDate.Year > 1900) query = query.Where(a => a.ModifiedAt <= endDate);

            if (assetFilter.FromDate != null)
            {
                query = query.Where(a => a.ModifiedAt >= assetFilter.FromDate);
            }

            if (assetFilter.ToDate != null)
            {
                query = query.Where(a => a.ModifiedAt <= assetFilter.ToDate);
            }

            query = query.Where(a => a.IsDeleted == false);

            count = query.Count();

            if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetOp>(sortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetOp>(sortColumn));
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }


           

            return query.ToList();
        }


        public IEnumerable<Model.AssetOp> GetRecoFiltered(AssetFilter assetFilter, string includes, int? assetId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string sortColumn, string sortDirection, int? page, int? pageSize, out int count)
        {

            IQueryable<Model.AssetOp> query = null;
            IQueryable<Model.InventoryAsset> queryInv = null;
            query = _context.AssetOps.AsNoTracking();

            if (includes != null)
            {
                foreach (var includeProperty in includes.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            else
            {
                query = query
                    .Include(i => i.Asset)
                    //.ThenInclude(i => i.AssetType)
                    .Include(i => i.AssetOpState)
                    .Include(i => i.Document)
                        .ThenInclude(d => d.DocumentType)
                    .Include(i => i.RoomInitial)
                        .ThenInclude(r => r.Location)
                            .ThenInclude(l => l.Region)
                    .Include(i => i.CostCenterInitial)
                    //.ThenInclude(l => l.AdmCenter)
                    .Include(i => i.EmployeeInitial)
                    //.Include(i => i.CostCenterInitial)
                    //    .ThenInclude(c => c.AdmCenter)
                    .Include(i => i.RoomFinal)
                        .ThenInclude(r => r.Location)
                            .ThenInclude(l => l.Region)
                    .Include(i => i.CostCenterFinal)
                    //.ThenInclude(l => l.AdmCenter)
                    .Include(i => i.BudgetManagerInitial)
                    .Include(i => i.ProjectInitial)
                    .Include(i => i.DimensionInitial)
                    .Include(i => i.AssetNatureInitial)
                    .Include(i => i.BudgetManagerFinal)
                    .Include(i => i.ProjectFinal)
                    .Include(i => i.DimensionFinal)
                    .Include(i => i.AssetNatureFinal)
                    .Include(i => i.EmployeeFinal)
                    //.Include(i => i.CostCenterFinal)
                    //    .ThenInclude(c => c.AdmCenter)
                    .AsQueryable();


                //queryInv = queryInv
                //   .Include(i => i.Asset)
                //       .ThenInclude(i => i.AssetType)
                //    .Include(i => i.Asset)
                //       .ThenInclude(i => i.Administration)
                //   .Include(i => i.RoomInitial)
                //       .ThenInclude(r => r.Location)
                //           .ThenInclude(l => l.Region)
                //   .Include(i => i.EmployeeInitial)
                //   .Include(i => i.CostCenterInitial)
                //       .ThenInclude(c => c.AdmCenter)
                //   .Include(i => i.RoomFinal)
                //       .ThenInclude(r => r.Location)
                //           .ThenInclude(l => l.Region)
                //   .Include(i => i.EmployeeFinal)
                //   .Include(i => i.CostCenterFinal)
                //       .ThenInclude(c => c.AdmCenter)
                //       .Where(a =>  a.AdministrationIdInitial != a.AdministrationIdFinal)
                //   .AsQueryable();
            }

            if ((assetFilter.EmployeeIds != null) && (assetFilter.EmployeeIds.Count > 0))
            {
                query = query.Where(a => assetFilter.EmployeeIds.Contains(a.EmployeeIdInitial) || assetFilter.EmployeeIds.Contains(a.EmployeeIdFinal));
            }


            if ((assetFilter.RoomIds != null) && (assetFilter.RoomIds.Count > 0))
            {
                query = query.Where(a => assetFilter.RoomIds.Contains(a.RoomIdInitial) || assetFilter.RoomIds.Contains(a.RoomIdFinal));
            }
            else
            {
                if ((assetFilter.LocationIds != null) && (assetFilter.LocationIds.Count > 0))
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.RoomInitial.Location.Id == id || a.RoomFinal.Location.Id == id; }, assetFilter.LocationIds));
                }
                else
                {
                    if ((assetFilter.RegionIds != null) && (assetFilter.RegionIds.Count > 0))
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.RoomInitial.Location.Region.Id == id || a.RoomFinal.Location.Region.Id == id; }, assetFilter.RegionIds));
                    }

                    if ((assetFilter.AdmCenterIds != null) && (assetFilter.AdmCenterIds.Count > 0))
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetOp, int?>((id) => { return a => a.RoomInitial.Location.AdmCenter.Id == id || a.RoomFinal.Location.AdmCenter.Id == id; }, assetFilter.AdmCenterIds));
                    }
                }
            }

            if (assetId.HasValue) query = query.Where(a => a.AssetId == assetId);

            //if (assetOpState != null) query = query.Where(a => a.AssetOpState.Name == assetOpState);

            if (assetOpState != null)
            {
                switch (assetOpState)
                {
                    case "Toate":
                        query = query.Where(a => a.AssetOpStateId == 79 || a.AssetOpStateId == 80);
                        break;
                    case "Propunere reconciliere":
                        query = query.Where(a => a.AssetOpStateId == 79);
                        break;
                    case "Validate":
                        query = query.Where(a => a.AssetOpStateId == 80);
                        break;
                    default:
                        //query = query.Where(a => a.AssetOpState.Id > 0);
                        break;
                }
            }

            query = query.Where(a => a.IsDeleted == false);


			query = query.Where(a => a.Document.DocumentTypeId == 55 || a.Document.DocumentTypeId == 56);  // BNR


			if (assetFilter.Filter != "" && assetFilter.Filter != null) query = query.Where(a => (a.Asset.InvNo.Contains(assetFilter.Filter) || a.Asset.ERPCode.Contains(assetFilter.Filter) || a.Asset.Name.Contains(assetFilter.Filter) || a.Asset.SerialNumber.Contains(assetFilter.Filter)));


            if (assetFilter.FromDate != null)
            {
                query = query.Where(a => a.ModifiedAt >= assetFilter.FromDate);
            }

            if (assetFilter.ToDate != null)
            {
                query = query.Where(a => a.ModifiedAt <= assetFilter.ToDate);
            }

            count = query.Count();

            if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetOp>(sortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.AssetOp>(sortColumn));
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }




            return query.ToList();
        }

        public IEnumerable<Dto.AssetOpSd> GetAll()
         {
            IQueryable<Dto.AssetOpSd> query = null;
            IQueryable<Model.Asset> assets = _context.Set<Model.Asset>().AsQueryable();
            IQueryable<Model.Document> documents = _context.Set<Model.Document>().AsQueryable();
            IQueryable<Model.DocumentType> documentTypes = _context.Set<Model.DocumentType>().AsQueryable();
            IQueryable<Model.AssetOp> assetOps = _context.Set<Model.AssetOp>().Where(assetop => assetop.DstConfAt !=null).AsQueryable();  // trebuie modificata conditia
            IQueryable<Model.Department> departmentsFinal = _context.Set<Model.Department>().AsQueryable();
            IQueryable<Model.Employee> esF = _context.Set<Model.Employee>().AsQueryable();
            IQueryable<Model.Location> lsF = _context.Set<Model.Location>().AsQueryable();
            IQueryable<Model.Room> rsF = _context.Set<Model.Room>().AsQueryable();
            IQueryable<Model.AssetState> asS = _context.Set<Model.AssetState>().AsQueryable();
            IQueryable<Model.AppState> appS = _context.Set<Model.AppState>().AsQueryable();

            query =
                from assetOp in assetOps
                join asset in assets on assetOp.AssetId equals asset.Id
                join document in documents on assetOp.DocumentId equals document.Id
                join documentType in documentTypes on document.DocumentTypeId equals documentType.Id

                join eF in esF on assetOp.EmployeeIdFinal equals eF.Id into employeesFinal
                from employeeFinal in employeesFinal.DefaultIfEmpty()

                join rF in rsF on assetOp.RoomIdFinal equals rF.Id into roomsFinal
                from roomFinal in roomsFinal.DefaultIfEmpty()
                join lF in lsF on roomFinal.LocationId equals lF.Id into locationsFinal
                from locationFinal in locationsFinal.DefaultIfEmpty()

                join aS in asS on assetOp.AssetStateIdFinal equals aS.Id into assetStatesFinal
                from assetState in assetStatesFinal.DefaultIfEmpty()

                join apS in appS on assetOp.AssetOpStateId equals apS.Id into assetOpState
                from appState in assetOpState.DefaultIfEmpty()

                select new Dto.AssetOpSd()
                {
                    Id = assetOp.Id,

                    DocumentId = document.Id,
                    DocumentType = documentType.Name,
                    DocumentTypeCode = documentType.Code,
                    DocumentDetails = document.Details,

                    EmployeeId = assetOp.EmployeeIdFinal,
                    InternalCode = (employeeFinal != null ? employeeFinal.InternalCode : ""),
                    FirstName = (employeeFinal != null ? employeeFinal.FirstName : ""),
                    LastName = (employeeFinal != null ? employeeFinal.LastName : ""),

                    LocationId = (locationFinal != null ? (int?)locationFinal.Id : null),
                    LocationCode = (locationFinal != null ? locationFinal.Code : null),
                    LocationName = (locationFinal != null ? locationFinal.Name : null),
                    RoomId = assetOp.RoomIdFinal,
                    RoomCode = (roomFinal != null ? roomFinal.Code : null),
                    RoomName = (roomFinal != null ? roomFinal.Name : null),

                    StateCode = assetState != null ? assetState.Code : string.Empty,
                    State = assetState != null ? assetState.Name : string.Empty,
                    ValidationDate = document.ValidationDate,
                    InvNo=asset.InvNo,
                    ReleaseConfAt= assetOp.ReleaseConfAt,
                    ReleaseConfBy = assetOp.ReleaseConfBy,
                    SrcConfAt = assetOp.SrcConfAt,
                    SrcConfBy = assetOp.SrcConfBy,
                    DstConfAt = assetOp.DstConfAt,
                    DstConfBy = assetOp.DstConfBy,
                    RegisterConfAt = assetOp.RegisterConfAt,
                    RegisterConfBy = assetOp.RegisterConfBy,
                    AssetOpStateId=assetOp.AssetOpStateId,
                    AssetOpStateCode = (appState != null ? appState.Code : null),
                    AssetOpStateName = (appState != null ? appState.Name : null)


                };

            return query.ToList();
        }

        public IEnumerable<Model.AssetOp> GetSyncDetails(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var document = _context.Set<Model.Inventory>().Where(i => i.Active == true && i.IsDeleted == false).SingleOrDefault();

            var query = _context.AssetOps.AsNoTracking().Where(a => a.AssetOpStateId == 3 && a.DocumentId == document.Id);

            if (lastId.HasValue)
            {
                query = query
                    .Where(r => (((r.ModifiedAt == lastModifiedAt) && (r.Id > lastId)) || (r.ModifiedAt > lastModifiedAt)));
                totalItems = query.Count();
                query = query
                    .OrderBy(a => a.ModifiedAt)
                    .ThenBy(a => a.Id)
                    .Take(pageSize);
            }
            else
            {
                totalItems = query.Count();
                query = query
                    .OrderBy(a => a.ModifiedAt)
                    .ThenBy(a => a.Id)
                    .Take(pageSize);
            }

            return query.ToList();
        }

        public int AssetOpConfImport(Dto.AssetOpConfirmUpload assetOpConfImport, string userName)
        {

            Model.AssetOp assetOp = null;
            var confirm = assetOpConfImport.Confirm;
          

            if (confirm == null)
            {

            }
            else
            {
                if (confirm.ToUpper() == "DA" && assetOpConfImport.AssetOpId > 0)
                {
                    assetOp = _context.Set<Model.AssetOp>().Where(a => a.Id == assetOpConfImport.AssetOpId).SingleOrDefault();

                    if (assetOp == null || assetOp.AssetOpStateId > 3)
                    {

                    }
                    else
                    {

                        var user = _context.Users.Where(u => u.UserName == userName).Single();

                        assetOp.AssetOpStateId = 4;
                        assetOp.RegisterConfAt = DateTime.Now;
                        assetOp.RegisterConfBy = user.Id;
                        assetOp.AssetCategoryIdFinal = assetOp.AssetCategoryIdFinal;
                        assetOp.AssetCategoryIdInitial = assetOp.AssetCategoryIdInitial;
                        assetOp.CostCenterIdInitial = assetOp.CostCenterIdInitial;
                        assetOp.CostCenterIdFinal = assetOp.CostCenterIdFinal;

                        _context.SaveChanges();
                    }
                }

            }


            return 1;

        }

        public int AssetOpConfImportBnr(Dto.AssetOpConfirmUpload assetOpConfImport, string userName)
        {

            Model.AssetOp assetOp = null;
            Model.Asset asset = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.Inventory inventory = null;
            Model.AccMonth accMonth = null;
            Model.AssetAdmMD assetAdmMD = null;
            var confirm = assetOpConfImport.Confirm;


            if (confirm == null)
            {

            }
            else
            {
                if (confirm.ToUpper() == "DA" && assetOpConfImport.AssetOpId > 0)
                {
                    assetOp = _context.Set<Model.AssetOp>().Where(a => a.Id == assetOpConfImport.AssetOpId).SingleOrDefault();
                    asset = _context.Set<Model.Asset>().Where(a => a.Id == assetOp.AssetId).Single();
                    inventory = _context.Set<Model.Inventory>().Where(inv => inv.Active == true).Where(inv => inv.IsDeleted != true).OrderByDescending(a => a.Id).Take(1).SingleOrDefault();
                    inventoryAsset = _context.Set<Model.InventoryAsset>().Where(a => a.AssetId == assetOp.AssetId).Where(a => a.InventoryId == inventory.Id).OrderByDescending(a => a.ModifiedAt).Take(1).SingleOrDefault();
                    accMonth = _context.Set<Model.AccMonth>().Where(inv => inv.IsActive == true).Where(inv => inv.IsDeleted != true).OrderByDescending(a => a.Id).Take(1).SingleOrDefault();
                    assetAdmMD = _context.Set<Model.AssetAdmMD>().Where(a => a.AssetId == assetOp.AssetId).Where(a => a.AccMonthId == accMonth.Id).SingleOrDefault();

                    if (assetOp == null || assetOp.AssetOpStateId > 3)
                    {

                    }
                    else
                    {

                        var user = _context.Users.Where(u => u.UserName == userName).Single();

                        asset.AdministrationId = assetOp.AdministrationIdFinal;
                        asset.AssetCategoryId = assetOp.AssetCategoryIdFinal;
                        asset.AssetStateId = assetOp.AssetStateIdFinal;
                        asset.CostCenterId = assetOp.CostCenterIdFinal;

                        Model.CostCenter costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == asset.CostCenterId).SingleOrDefault();

                        asset.DepartmentId = costCenter.Division.DepartmentId;
                        asset.DivisionId = costCenter.DivisionId;

                        asset.EmployeeId = assetOp.EmployeeIdFinal;
                        asset.InvStateId = assetOp.InvStateIdFinal;
                        asset.RoomId = assetOp.RoomIdFinal;

                        inventoryAsset.CostCenterIdFinal = assetOp.CostCenterIdFinal;

                        inventoryAsset.EmployeeIdInitial = assetOp.EmployeeIdFinal;
                        inventoryAsset.RoomIdInitial = assetOp.RoomIdFinal;
                        inventoryAsset.StateIdInitial = assetOp.InvStateIdFinal;
                        inventoryAsset.Info = assetOp.Info;

                        assetAdmMD.AssetCategoryId = assetOp.AssetCategoryIdFinal;
                        assetAdmMD.AssetStateId = assetOp.AssetStateIdFinal;
                        assetAdmMD.CostCenterId = assetOp.CostCenterIdFinal;

                        costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == assetAdmMD.CostCenterId).SingleOrDefault();

                        assetAdmMD.DepartmentId = costCenter.Division.DepartmentId;
                        assetAdmMD.DivisionId = costCenter.DivisionId;

                        assetAdmMD.EmployeeId = assetOp.EmployeeIdFinal;
                        assetAdmMD.RoomId = assetOp.RoomIdFinal;

                        assetOp.AssetOpStateId = 4;
                        assetOp.RegisterConfAt = DateTime.Now;
                        assetOp.RegisterConfBy = user.Id;
                        assetOp.AssetCategoryIdFinal = assetOp.AssetCategoryIdFinal;
                        assetOp.AssetCategoryIdInitial = assetOp.AssetCategoryIdInitial;
                        assetOp.CostCenterIdInitial = assetOp.CostCenterIdInitial;
                        assetOp.CostCenterIdFinal = assetOp.CostCenterIdFinal;

                        _context.SaveChanges();
                    }
                }

            }


            return 1;

        }

        public int AssetOpConfImportPiraeus(Dto.AssetOpConfirmUpload assetOpConfImport, string userName)
        {

            Model.AssetOp assetOp = null;
            Model.Asset asset = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.Inventory inventory = null;
            Model.AccMonth accMonth = null;
            Model.AssetAdmMD assetAdmMD = null;
            var confirm = assetOpConfImport.Confirm;


            if (confirm == null)
            {

            }
            else
            {
                if (confirm.ToUpper() == "DA" && assetOpConfImport.AssetOpId > 0)
                {
                    assetOp = _context.Set<Model.AssetOp>().Where(a => a.Id == assetOpConfImport.AssetOpId).SingleOrDefault();
                    asset = _context.Set<Model.Asset>().Where(a => a.Id == assetOp.AssetId).Single();
                    inventory = _context.Set<Model.Inventory>().Where(inv => inv.Active == true).Where(inv => inv.IsDeleted != true).OrderByDescending(a => a.Id).Take(1).SingleOrDefault();
                    inventoryAsset = _context.Set<Model.InventoryAsset>().Where(a => a.AssetId == assetOp.AssetId).Where(a => a.InventoryId == inventory.Id).OrderByDescending(a => a.ModifiedAt).Take(1).SingleOrDefault();
                    accMonth = _context.Set<Model.AccMonth>().Where(inv => inv.IsActive == true).Where(inv => inv.IsDeleted != true).OrderByDescending(a => a.Id).Take(1).SingleOrDefault();
                    assetAdmMD = _context.Set<Model.AssetAdmMD>().Where(a => a.AssetId == assetOp.AssetId).Where(a => a.AccMonthId == accMonth.Id).SingleOrDefault();

                    if (assetOp == null || assetOp.AssetOpStateId > 3)
                    {

                    }
                    else
                    {

                        var user = _context.Users.Where(u => u.UserName == userName).Single();

                        asset.AdministrationId = assetOp.AdministrationIdFinal;
                        asset.AssetCategoryId = assetOp.AssetCategoryIdFinal;
                        asset.AssetStateId = assetOp.AssetStateIdFinal;
                        asset.CostCenterId = assetOp.CostCenterIdFinal;

                        Model.CostCenter costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == asset.CostCenterId).SingleOrDefault();

                        asset.DepartmentId = costCenter.Division.DepartmentId;
                        asset.DivisionId = costCenter.DivisionId;

                        asset.EmployeeId = assetOp.EmployeeIdFinal;
                        asset.InvStateId = assetOp.InvStateIdFinal;
                        asset.RoomId = assetOp.RoomIdFinal;

                        inventoryAsset.CostCenterIdFinal = assetOp.CostCenterIdFinal;
                        inventoryAsset.EmployeeIdInitial = assetOp.EmployeeIdFinal;
                        inventoryAsset.RoomIdInitial = assetOp.RoomIdFinal;
                        inventoryAsset.StateIdInitial = assetOp.InvStateIdFinal;
                        inventoryAsset.Info = assetOp.Info;

                        assetAdmMD.AssetCategoryId = assetOp.AssetCategoryIdFinal;
                        assetAdmMD.AssetStateId = assetOp.AssetStateIdFinal;
                        assetAdmMD.CostCenterId = assetOp.CostCenterIdFinal;

                        costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == assetAdmMD.CostCenterId).SingleOrDefault();

                        assetAdmMD.DepartmentId = costCenter.Division.DepartmentId;
                        assetAdmMD.DivisionId = costCenter.DivisionId;

                        assetAdmMD.EmployeeId = assetOp.EmployeeIdFinal;
                        assetAdmMD.RoomId = assetOp.RoomIdFinal;

                        assetOp.AssetOpStateId = 4;
                        assetOp.RegisterConfAt = DateTime.Now;
                        assetOp.RegisterConfBy = user.Id;
                        assetOp.AssetCategoryIdFinal = assetOp.AssetCategoryIdFinal;
                        assetOp.AssetCategoryIdInitial = assetOp.AssetCategoryIdInitial;
                        assetOp.CostCenterIdInitial = assetOp.CostCenterIdInitial;
                        assetOp.CostCenterIdFinal = assetOp.CostCenterIdFinal;

                        _context.SaveChanges();
                    }
                }

            }


            return 1;

        }


        public int DeleteAssetOp(int assetOpId)
        {

            var assetOp = _context.Set<Model.AssetOp>().Where(a => a.Id == assetOpId).Single();

            if (assetOp != null)
            {
                var asset = _context.Set<Model.Asset>().Where(a => a.Id == assetOp.AssetId).Single();

                if (asset != null)
                {
                    asset.IsInTransfer = false;
                    _context.Update(asset);
                }

            }

            _context.Remove(assetOp);


            _context.SaveChanges();

            return assetOpId;
        }

        public int ValidateAssetOp(int assetOpId)
        {

            var assetOp = _context.Set<Model.AssetOp>().Where(a => a.Id == assetOpId).Single();

            assetOp.IsDeleted = true;

            _context.Update(assetOp);

            _context.SaveChanges();

            return assetOpId;
        }

        public int DeleteAssetOpReco(int assetOpId)
        {
            Model.Inventory inventory = null;
            Model.AssetOp assetOp = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.InventoryAsset inventoryAssetTemp = null;
            Model.Asset asset = null;
            Model.Asset assetTemp = null;
            Model.EntityType entityType = null;

            inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Active == true).Single();
			entityType = _context.Set<Model.EntityType>().AsNoTracking().Where(a => a.Name == inventory.DocumentId.ToString()).Single();

			assetOp = _context.Set<Model.AssetOp>().Where(a => a.Id == assetOpId).SingleOrDefault();
            asset = _context.Set<Model.Asset>().Where(a => a.Id == assetOp.AssetId).SingleOrDefault();
            inventoryAsset = _context.Set<Model.InventoryAsset>().Where(a => a.AssetId == assetOp.AssetId && a.InventoryId == inventory.Id).SingleOrDefault();

            var invNoTemp = inventoryAsset.TempReco;

            assetTemp = _context.Set<Model.Asset>().Where(a => a.InvNo == invNoTemp).SingleOrDefault();
            inventoryAssetTemp = _context.Set<Model.InventoryAsset>().Where(a => a.AssetId == assetTemp.Id && a.InventoryId == inventory.Id).SingleOrDefault();

            asset.TempReco = string.Empty;
            asset.TempName = string.Empty;
            asset.TempSerialNumber = string.Empty;
            asset.AssetRecoStateId = null;

            inventoryAsset.TempReco = string.Empty;
            inventoryAsset.TempName = string.Empty;
            inventoryAsset.TempRecoSerialNumber = string.Empty;
            inventoryAsset.AssetRecoStateId = null;

            assetTemp.TempReco = string.Empty;
            assetTemp.TempName = string.Empty;
            assetTemp.TempSerialNumber = string.Empty;
            assetTemp.AssetRecoStateId = null;

            inventoryAssetTemp.TempReco = string.Empty;
            inventoryAssetTemp.TempName = string.Empty;
            inventoryAssetTemp.TempRecoSerialNumber = string.Empty;
            inventoryAssetTemp.AssetRecoStateId = null;

            assetOp.IsDeleted = true;

            var entityFiles = _context.EntityFiles.Where(e => e.EntityId == asset.Id && e.EntityTypeId == entityType.Id).ToList();

            foreach (Model.EntityFile entityFile in entityFiles)
            {
                entityFile.IsDeleted = true;

                _context.EntityFiles.Update(entityFile);
            }

            _context.SaveChanges();

            if (assetOp != null)
            {
                return assetOp.Id;
            }
            else
            {
                return 0;
            }
        }

    }

}
