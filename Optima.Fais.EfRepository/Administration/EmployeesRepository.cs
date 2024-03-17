using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Optima.Fais.Model.Utils;
using Optima.Fais.Model;
using System.Globalization;

namespace Optima.Fais.EfRepository
{
    public class EmployeesRepository : Repository<Model.Employee>, IEmployeesRepository
    {
        public EmployeesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (e) => (
            e.InternalCode.Contains(filter) || 
            e.FirstName.Contains(filter) || 
            e.LastName.Contains(filter) ||
            e.Email.Contains(filter) ||
            e.Manager.InternalCode.Contains(filter) ||
            e.Manager.FirstName.Contains(filter) ||
            e.Manager.LastName.Contains(filter) ||
            e.Manager.Email.Contains(filter)); })
        { }

        public IEnumerable<Dto.EmployeeDetail> GetDetailsByFilters(int? departmentId, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, out int count)
        {
            var query = _context.Set<Model.Employee>().Include("Department").Include("CostCenter").Where(e => e.IsDeleted == false).Select(e => new Dto.EmployeeDetail()
            {
                Id = e.Id,
                InternalCode = e.InternalCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                DepartmentId = e.DepartmentId,
                DepartmentCode = e.Department.Code,
                DepartmentName = e.Department.Name,
                CostCenterId = e.CostCenter.Id,
                CostCenterCode= e.CostCenter.Code,
                CostCenterName = e.CostCenter.Name,
                Email = e.Email
            });

            if (departmentId.HasValue) query = query.Where(e => e.DepartmentId == departmentId);
            if (filter != null) query = query.Where(e => (e.InternalCode.Contains(filter) || e.FirstName.Contains(filter) || e.LastName.Contains(filter) || e.DepartmentCode.Contains(filter) || e.DepartmentName.Contains(filter)));

            count = query.Count();

            if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Dto.EmployeeDetail>(sortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Dto.EmployeeDetail>(sortColumn));
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query.ToList();
        }

        public async Task<IEnumerable<Dto.EmployeeDetail>> GetDetailsByFiltersAsync(int? departmentId, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.Employee> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            
            var query = _context.Employees.AsNoTracking();

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
        public Model.Employee GetDetailsById(int employeeId, string includes)
        {
            IQueryable<Model.Employee> query = null;
            query = GetEmployeeQuery(includes);

            return query.Where(a => a.Id == employeeId).SingleOrDefault();
        }

        private IQueryable<Model.Employee> GetEmployeeQuery(string includes)
        {
            IQueryable<Model.Employee> query = null;
            query = _context.Employees.AsNoTracking();

            if (includes != null)
            {
                foreach (var includeProperty in includes.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query;
        }

        public void  EmployeeImport(Dto.EmployeeImport employeeImport, out bool updated, out bool userExist, out Model.ApplicationUser user, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut)
        {
            Model.Employee employee = null;
            updated = false;
            userExist = true;
            Model.ApplicationUser existingUser = null;
            string internalCode = employeeImport.InternalCode.Trim();
            string firstName = string.Empty;
            string lastName = string.Empty;
            var emailIni = "";
            var emailCC = "";
            var htmlBodyEmail = "";
            var link = string.Format(@" <a href=\\VAIMUC04\WGAI\BRANCH-RO\Team\Guides%20and%20procedures%20IT\PV%20IN%20-%20Proces%20verbal%20New%20Joiner.pdf>this document</a>");
            var htmlBodyEnd = @"</tbody>
                                                </table>
                                        <h5>This is an automated message – Please do not replay directly to this email!</h5>
                                        <h5>For more details please contact the IT Administration at following email: allianztechnology-ro-it@allianz.com</h5>
                                            </body>
                                        </html> ";
            var htmlBody = @"
                                        <html lang=""en"">
                                            <head>    
                                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                                <title>
                                                    Upcoming topics
                                                </title>
                                                <style type=""text/css"">
                                                    HTML{background-color: #e8e8e8;}
                                                    .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
                                                    .courses-table .description{color: #505050;}
                                                    .courses-table td{border: 1px solid #D1D1D1; background-color: #F3F3F3; padding: 0 10px;}
                                                    .courses-table th{border: 1px solid #424242; color: #FFFFFF;text-align: left; padding: 0 10px;}
                                                    .red{background-color: #003880;}
                                                    .yellow{color: #FF0000;}
                                                    .black{background-color: #003880, color: #FF0000;}
                                                    .green{background-color: #6B9852;}
                                                </style>
                                            </head>
                                            <body>
                                                <h2>Dear employee, <span>please complete"  + link  + @" with the inventory numbers of the assets you received: </span> </h2>
                                                <h3></h2>
                                                <table class=""courses-table"">
                                                    <thead>
                                                        <tr>
                                                            <th class=""red""></th>
                                                            <th class=""red"">BenSl/BID</th>
                                                            <th class=""red"">First Name</th>
                                                            <th class=""red"">Last Name</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                        ";
            var subject = "New Employee";

            employee = _context.Set<Model.Employee>().Where(a => a.InternalCode == internalCode).FirstOrDefault();

            if (employee == null)
            {
                updated = false;
                int spaceIndex = employeeImport.FullName.IndexOf(" ");

                firstName = employeeImport.FullName.Substring(0, spaceIndex);
                lastName = employeeImport.FullName.Substring(spaceIndex + 1);

                employee = new Model.Employee
                {
                    InternalCode = internalCode,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = employeeImport.Email,
                    IsDeleted = employeeImport.Status != "" ? (employeeImport.Status.ToUpper() == "IN FORCE" ? false : true) : false
                };

                _context.Set<Model.Employee>().Add(employee);

                existingUser = _context.Set<Model.ApplicationUser>().Where(a => a.UserName.ToUpper() == employeeImport.Email.ToUpper()).FirstOrDefault();

                if(existingUser == null)
                {
                    userExist = false;
                }
                else
                {
                    existingUser.EmployeeId = employee.Id;
                    _context.Set<Model.ApplicationUser>().Update(existingUser);
                }

                //  NEW EMPLOYEE

                var eIniFirstName = employee != null ? employee.FirstName : "";
                var eIniLastName = employee != null ? employee.LastName : "";
                var eIniInternalCode = employee != null ? employee.InternalCode : "";

                emailIni = employee != null ? employee.Email != null ? employee.Email : "adrian.cirnaru@optima.ro" : "adrian.cirnaru@optima.ro";

                //emailCC = "dvladoiu@stanleybet.ro";

                //emailIni = "adrian.cirnaru@optima.ro";
                emailCC = "ALIN.CERNATESCU@ALLIANZ.COM";

                htmlBodyEmail = htmlBodyEmail + @"
                                                        <tr>
                                                            <td class=""description"">Employee Detail</ td >
                                                            <td class=""description"">" + eIniInternalCode + @" </ td >
                                                            <td class=""description"">" + eIniFirstName + @" </ td >
                                                            <td class=""description"">" + eIniLastName + @" </ td >
                                                        </tr>
                                                       
                                        ";

            // NEW EMPLOYEE

            }
            else
            {
                int spaceIndex = employeeImport.FullName.IndexOf(" ");

                firstName = employeeImport.FullName.Substring(0, spaceIndex);
                lastName = employeeImport.FullName.Substring(spaceIndex + 1);


                employee.FirstName = firstName;
                employee.LastName = lastName;
                employee.Email = employeeImport.Email;
                employee.ModifiedAt = DateTime.Now;
                employee.IsDeleted = employeeImport.Status != "" ? employeeImport.Status.ToUpper() == "IN FORCE" ? false : true : false;
                updated = true;

                if(employee.ERPCode != null)
                {
                    employee.ERPCode = Guid.NewGuid().ToString("n").Substring(0, 6).ToUpper() + "T3";
                }

                if(employee.Email.Length > 0)
                {
                    existingUser = _context.Set<Model.ApplicationUser>().Where(a => a.UserName.ToUpper() == employeeImport.Email.ToUpper()).FirstOrDefault();
                   

                    if (existingUser == null)
                    {
                        userExist = false;
                    }
                    else
                    {
                        existingUser.EmployeeId = employee.Id;
                        _context.Set<Model.ApplicationUser>().Update(existingUser);
                    }
                }

                _context.Set<Model.Employee>().Update(employee);

            }

            user = new Model.ApplicationUser
            {
                UserName = employee.Email,
                Email = employee.Email,
                GivenName = employee.Email.ToUpper(),
                FamilyName = employee.Email.ToUpper()
            };

            emailIniOut = emailIni;
            emailCCOut = emailCC;
            bodyHtmlOut = htmlBody + htmlBodyEmail + htmlBodyEnd;
            subjectOut = subject;

            _context.SaveChanges();
        }

        //public void EmployeeImport(Dto.EmployeeImport employeeImport)
        //{
        //    Model.Employee employee = null;

        //    string internalCode = employeeImport.InternalCode.Trim();
        //    string fullName = employeeImport.FullName.Trim();
        //    string email = employeeImport.Email.Trim();
        //    string status = employeeImport.Status.Trim();

        //    string firstNameTl = string.Empty;
        //    string lastNameTl = string.Empty;

        //    int spaceIndex = employeeImport.FullName.IndexOf(" ");
        //    if (spaceIndex >= 0)
        //    {
        //        firstNameTl = employeeImport.FullNameTl.Substring(0, spaceIndex);
        //        lastNameTl = employeeImport.FullNameTl.Substring(spaceIndex + 1);
        //    }
        //    else
        //    {
        //        firstNameTl = employeeImport.FullNameTl;
        //    }

        //    teamLeader = _context.Set<Model.Employee>().Where(a => a.InternalCode == internalCodeTl).FirstOrDefault();
        //    if (teamLeader == null) teamLeader = new Model.Employee();

        //    teamLeader.FirstName = firstNameTl;
        //    teamLeader.LastName = lastNameTl;

        //    if (teamLeader.Id > 0)
        //    {
        //        _context.Set<Model.Employee>().Update(teamLeader);
        //    }
        //    else
        //    {
        //        teamLeader.InternalCode = internalCodeTl;
        //        _context.Set<Model.Employee>().Add(teamLeader);
        //    }

        //    department = _context.Set<Model.Department>().Where(d => d.Name == departmentName).FirstOrDefault();

        //    if (department != null)
        //    {
        //        department.TeamLeader = teamLeader;
        //        department.IsDeleted = false;
        //        _context.Set<Model.Department>().Update(department);
        //    }
        //    else
        //    {
        //        department = new Model.Department();
        //        department.Code = string.Empty;
        //        department.Name = departmentName;
        //        department.IsDeleted = false;
        //        department.TeamLeader = teamLeader;

        //        _context.Set<Model.Department>().Add(department);
        //    }

        //    employee = _context.Set<Model.Employee>().Where(a => a.InternalCode == internalCode).FirstOrDefault();
        //    if (employee == null) employee = new Model.Employee();

        //    employee.FirstName = employeeImport.FirstName;
        //    employee.LastName = employeeImport.LastName;
        //    employee.Email = employeeImport.Email;
        //    employee.Department = department;
        //    employee.IsDeleted = false;

        //    if (employee.Id > 0)
        //    {
        //        _context.Set<Model.Employee>().Update(employee);
        //    }
        //    else
        //    {
        //        employee.InternalCode = internalCode;
        //        _context.Set<Model.Employee>().Add(employee);
        //    }

        //    _context.SaveChanges();
        //}

        public IEnumerable<Model.Employee> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> companyIds, List<int?> costCenterIds, string email, bool deleted, bool isBudgetOwner, bool teamStatus)
        {
            var predicate = GetFiltersPredicate(filter, companyIds, costCenterIds, email, deleted, isBudgetOwner, teamStatus);

            includes = includes ?? "AdmCenter,CostCenter,Department";
            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize, deleted).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> companyIds, List<int?> costCenterIds, string email, bool deleted, bool isBudgetOwner, bool teamStatus)
        {
            var predicate = GetFiltersPredicate(filter, companyIds, costCenterIds, email, deleted, isBudgetOwner, teamStatus);

            return GetQueryable(predicate, null, null, null, null, null, deleted).Count();
            //return GetQueryable(predicate).Count();
        }

        private Expression<Func<Model.Employee, bool>> GetFiltersPredicate(string filter, List<int?> companyIds, List<int?> costCenterIds, string email, bool deleted, bool isBudgetOwner, bool teamStatus)
        {
            Expression<Func<Model.Employee, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((companyIds != null) && (companyIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Employee>(predicate, r => companyIds.Contains(r.CompanyId))
                    : r => companyIds.Contains(r.CompanyId);
            }

            if ((costCenterIds != null) && (costCenterIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Employee>(predicate, r => costCenterIds.Contains(r.CostCenterId))
                    : r => costCenterIds.Contains(r.CostCenterId);
            }

			if (isBudgetOwner)
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Employee>(predicate, r => r.IsBudgetOwner)
					: r => r.IsBudgetOwner;
			}

			if (teamStatus)
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Employee>(predicate, r => r.Manager.Email == email)
					: r => r.Manager.Email == email;
			}

			predicate = predicate != null
              ? ExpressionHelper.And<Model.Employee>(predicate, r => r.IsDeleted == deleted)
              : r => r.IsDeleted == deleted;

            return predicate;
        }

        public IEnumerable<Model.Employee> GetTransferEmployeesInUseWithAssets(AssetFilter assetFilter, List<PropertyFilter> propFilters)
        {
            var query = GetAssetMonthDetailQuery(assetFilter, propFilters);
            var list = query.Select(a => a.Asset.EmployeeTransfer).Distinct();

            return list;
        }

        public IQueryable<AssetMonthDetail> GetAssetMonthDetailQuery(AssetFilter assetFilter, List<PropertyFilter> propFilters)
        {
            IQueryable<Model.Asset> assetQuery = null;
            IQueryable<AssetDepMD> depQuery = null;
            IQueryable<AssetAdmMD> admQuery = null;
            IQueryable<AssetMonthDetail> query = null;

            assetQuery = _context.Assets.AsQueryable();

            int assetStateId = _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "IN_USE").Select(a => a.Id).SingleOrDefault();

            int? accSystemId = assetFilter.AccSystemId;
            int? accMonthId = assetFilter.AccMonthId;

            if (!accMonthId.HasValue || accMonthId.Value <= 0)
            {
                accMonthId = _context.Set<Model.AccMonth>().AsNoTracking().Where(a => a.IsActive == true).Select(a => a.Id).SingleOrDefault();
            }

            if (!accSystemId.HasValue || accSystemId.Value <= 0)
            {
                Model.AccSystem accSystem = _context.AccSystems.FirstOrDefault();
                if (accSystem != null) accSystemId = accSystem.Id;
            }

            depQuery = _context.AssetDepMDs.AsQueryable().Where(a => a.AccSystemId == accSystemId && (a.AccMonthId == accMonthId));
            admQuery = _context.AssetAdmMDs.AsQueryable().Where(a => (a.AccMonthId == accMonthId));

            foreach (var prop in propFilters)
            {
                if (prop.Property == "Asset.ErpCode" && prop.Filter != "")
                {
                    assetQuery = assetQuery.Where(a => a.ERPCode.Contains(prop.Filter));
                }
                else if (prop.Property == "Asset.InvNo" && prop.Filter != "")
                {
                    assetQuery = assetQuery.Where(a => a.InvNo.Contains(prop.Filter));
                }
                else if (prop.Property == "Asset.SubNo" && prop.Filter != "")
                {
                    assetQuery = assetQuery.Where(a => a.SubNo.Contains(prop.Filter));
                }
                else if (prop.Property == "AssetName" && prop.Filter != "")
                {
                    assetQuery = assetQuery.Where(a => a.Name.Contains(prop.Filter));
                }
                else if (prop.Property == "AssetSerialNumber" && prop.Filter != "")
                {
                    assetQuery = assetQuery.Where(a => a.SerialNumber.Contains(prop.Filter));
                }
            }


            query = assetQuery.Select(asset => new AssetMonthDetail { Asset = asset });

            query = query
                   .Join(admQuery, q => q.Asset.Id, adm => adm.AssetId, (q, adm) => new AssetMonthDetail { Asset = q.Asset, Adm = adm });

            query = query
                    .Join(depQuery, q => q.Asset.Id, dep => dep.AssetId, (q, dep) => new AssetMonthDetail { Asset = q.Asset, Adm = q.Adm, Dep = dep });

            if (assetFilter.Role != null && assetFilter.Role != "")
            {
                if (assetFilter.Role.ToUpper() == "ADMINISTRATOR")
                {
                    if ((assetFilter.CostCenterIds != null) && (assetFilter.CostCenterIds.Count > 0))
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => ((a.Adm.CostCenterId == id)); }, assetFilter.CostCenterIds));
                    }

                    if ((assetFilter.EmployeeIds != null) && (assetFilter.EmployeeIds.Count > 0))
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => ((a.Adm.EmployeeId == id)); }, assetFilter.EmployeeIds));
                    }
                }
                else if (assetFilter.Role.ToUpper() == "PROCUREMENT")
                {
                    List<int?> divisionIds = new List<int?>();
                    divisionIds.Add(1482);

                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Asset.Order.DivisionId != id; }, divisionIds));

                    query = query.Where(a => a.Asset.Order.Offer.AssetType.Code != "STOCK_IT");
                }
                else if (assetFilter.Role.ToUpper() == "PROC-IT")
                {
                    List<int?> divisionIds = _context.Set<Model.EmployeeDivision>().AsNoTracking().Where(e => e.EmployeeId == assetFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.DivisionId).ToList();

                    if (divisionIds.Count == 0)
                    {
                        divisionIds = new List<int?>();
                        divisionIds.Add(-1);
                    }

                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Asset.Order.DivisionId == id; }, divisionIds));

                    query = query.Where(a => a.Asset.AssetType.Code != "STOCK_IT");
                }
                else if (assetFilter.Role.ToUpper() == "USER")
                {

                    List<int?> costCenterIds = _context.Set<Model.EmployeeCostCenter>().AsNoTracking().Where(e => e.EmployeeId == assetFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.CostCenterId).ToList();

                    if (costCenterIds.Count == 0)
                    {
                        costCenterIds = new List<int?>();
                        costCenterIds.Add(-1);
                    }


                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => ((a.Adm.CostCenterId == id)); }, costCenterIds));


                    if ((assetFilter.CostCenterIds != null) && (assetFilter.CostCenterIds.Count > 0))
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => ((a.Adm.CostCenterId == id)); }, assetFilter.CostCenterIds));
                    }

                }
                else
                {
                    if (assetFilter.Role.ToUpper() != "ADMINISTRATOR")
                    {

                        if (assetFilter.Role.ToUpper() == "APPROVERS")
                        {
                            List<int?> employeeIds = new List<int?>();
                            employeeIds.Add(assetFilter.EmployeeId);


                            query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => {
                                return a => (
                            (
                            (a.Asset.Order.EmployeeL4Id == id && a.Asset.Order.AppState.Code == "ORDER_LEVEL4") ||
                            (a.Asset.Order.EmployeeL3Id == id && a.Asset.Order.AppState.Code == "ORDER_LEVEL3") ||
                            (a.Asset.Order.EmployeeL2Id == id && a.Asset.Order.AppState.Code == "ORDER_LEVEL2") ||
                            (a.Asset.Order.EmployeeL1Id == id && a.Asset.Order.AppState.Code == "ORDER_LEVEL1") ||
                            (a.Asset.Order.EmployeeS1Id == id && a.Asset.Order.AppState.Code == "ORDER_LEVELS1") ||
                            (a.Asset.Order.EmployeeS2Id == id && a.Asset.Order.AppState.Code == "ORDER_LEVELS2") ||
                            (a.Asset.Order.EmployeeS3Id == id && a.Asset.Order.AppState.Code == "ORDER_LEVELS3")) || a.Asset.Order.AppState.Code == "NEED_CONTRACT");
                            }, employeeIds));
                        }
                        else
                        {
                            List<int?> divisionIds = _context.Set<Model.EmployeeDivision>().AsNoTracking().Where(e => e.EmployeeId == assetFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.DivisionId).ToList();

                            if (divisionIds.Count == 0)
                            {
                                divisionIds = new List<int?>();
                                divisionIds.Add(-1);
                            }

                            query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Asset.Order.DivisionId == id; }, divisionIds));
                        }

                    }
                }
            }


            if ((assetFilter.AssetNatureIds != null) && (assetFilter.AssetNatureIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.AssetNatureId == id; }, assetFilter.AssetNatureIds));
            }

            if ((assetFilter.CompanyIds != null) && (assetFilter.CompanyIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Asset.CompanyId == id; }, assetFilter.CompanyIds));
            }

            if ((assetFilter.InterCompanyIds != null) && (assetFilter.InterCompanyIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.InterCompanyId == id; }, assetFilter.InterCompanyIds));
            }

            if ((assetFilter.InsuranceCategoryIds != null) && (assetFilter.InsuranceCategoryIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.InsuranceCategoryId == id; }, assetFilter.InsuranceCategoryIds));
            }

            if ((assetFilter.DimensionIds != null) && (assetFilter.DimensionIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Asset.DimensionId == id; }, assetFilter.DimensionIds));
            }

            if ((assetFilter.ExpAccountIds != null) && (assetFilter.ExpAccountIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Asset.ExpAccountId == id; }, assetFilter.ExpAccountIds));
            }

            //if ((assetFilter.EmployeeIds != null) && (assetFilter.EmployeeIds.Count > 0))
            //{
            //	query = query.Where(a => assetFilter.EmployeeIds.Contains(a.Adm.EmployeeId));
            //}

            if ((assetFilter.ProjectIds != null) && (assetFilter.ProjectIds.Count > 0))
            {
                query = query.Where(a => assetFilter.ProjectIds.Contains(a.Adm.ProjectId));
            }

            if ((assetFilter.DictionaryItemIds != null) && (assetFilter.DictionaryItemIds.Count > 0))
            {
                query = query.Where(a => assetFilter.DictionaryItemIds.Contains(a.Asset.DictionaryItemId));
            }

            if ((assetFilter.DivisionIds != null) && (assetFilter.DivisionIds.Count > 0))
            {
                query = query.Where(a => assetFilter.DivisionIds.Contains(a.Asset.DivisionId));
            }

            if ((assetFilter.DepartmentIds != null) && (assetFilter.DepartmentIds.Count > 0))
            {
                query = query.Where(a => assetFilter.DepartmentIds.Contains(a.Asset.DepartmentId));
            }


            if ((assetFilter.BrandIds != null) && (assetFilter.BrandIds.Count > 0))
            {
                query = query.Where(a => assetFilter.BrandIds.Contains(a.Adm.BrandId));
            }

            if ((assetFilter.RoomIds != null) && (assetFilter.RoomIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.RoomId == id; }, assetFilter.RoomIds));
            }
            else
            {
                if ((assetFilter.LocationIds != null) && (assetFilter.LocationIds.Count > 0))
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.Room.Location.Id == id; }, assetFilter.LocationIds));
                }
                else
                {
                    if ((assetFilter.RegionIds != null) && (assetFilter.RegionIds.Count > 0))
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.Room.Location.Region.Id == id; }, assetFilter.RegionIds));
                    }
                }
            }

            //if ((assetFilter.AdministrationIds != null) && (assetFilter.AdministrationIds.Count > 0))
            //{
            //    query = query.Where(a => assetFilter.AdministrationIds.Contains(a.Adm.AdministrationId));
            //}
            //else
            //{
            //    if ((assetFilter.DivisionIds != null) && (assetFilter.DivisionIds.Count > 0))
            //    {
            //        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.Administration.Division.Id == id; }, assetFilter.DivisionIds));
            //    }

            //}

            //if (assetFilter.ShowReco)
            //{
            //    query = query.Where(a => a.Asset.ERPCode == null || a.Asset.ERPCode == "");
            //}

            if ((assetFilter.InvStateIds != null) && (assetFilter.InvStateIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Asset.InvStateId == id; }, assetFilter.InvStateIds));
            }

            query = query.Where(a => a.Asset.IsDeleted == false && a.Asset.Validated == true && a.Asset.AssetStateId == assetStateId);

            if (assetFilter.FilterPurchaseDate != "false" && assetFilter.FilterPurchaseDate != null)
            {


                var monthYear = DateTime.ParseExact(assetFilter.FilterPurchaseDate,
                                  "yyyy-MM-dd",
                                   CultureInfo.InvariantCulture);

                var firstDayOfMonth = new DateTime(monthYear.Year, monthYear.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                query = query.Where(a => a.Asset.PurchaseDate >= firstDayOfMonth && a.Asset.PurchaseDate <= lastDayOfMonth);
                // Console.Write("Iesiri: " + query.Count());
            }

            //if (assetFilter.FromDate != null)
            //{
            //    query = query.Where(a => a.Asset.PurchaseDate >= assetFilter.FromDate);
            //}

            //if (assetFilter.ToDate != null)
            //{
            //    query = query.Where(a => a.Asset.PurchaseDate <= assetFilter.ToDate);
            //}


            //if (assetFilter.FromReceptionDate != null)
            //{
            //    query = query.Where(a => a.Asset.ReceptionDate >= assetFilter.FromReceptionDate);
            //}

            //if (assetFilter.ToReceptionDate != null)
            //{
            //    query = query.Where(a => a.Asset.ReceptionDate <= assetFilter.ToReceptionDate);
            //}

            //if (assetFilter.ErpCode != null && assetFilter.ErpCode == true)
            //{
            //    assetQuery = assetQuery.Where(a => a.ERPCode != null);
            //}
            //else if (assetFilter.ErpCode != null && assetFilter.ErpCode == false)
            //{
            //    assetQuery = assetQuery.Where(a => a.ERPCode == null);
            //}

            return query;
        }
    }
}
