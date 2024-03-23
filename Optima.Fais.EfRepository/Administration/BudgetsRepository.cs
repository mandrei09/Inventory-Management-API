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
    public class BudgetsRepository : Repository<Model.Budget>, IBudgetsRepository
    {

        public BudgetsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Name.Contains(filter)); })
        {
           
        }

        public IEnumerable<Model.BudgetDetail> GetBuget(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Budget> budgetQuery = null;
            IQueryable<BudgetDetail> query = null;

            budgetQuery = _context.Budgets.AsNoTracking().AsQueryable();

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
                    case "Budget":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new BudgetDetail { Budget = budget });

            if ((budgetFilter.CompanyIds != null) && (budgetFilter.CompanyIds.Count > 0))
            {
                query = query.Where(a => budgetFilter.CompanyIds.Contains(a.Budget.CompanyId));
            }

            if ((budgetFilter.EmployeeIds != null) && (budgetFilter.EmployeeIds.Count > 0))
            {
                query = query.Where(a => budgetFilter.EmployeeIds.Contains(a.Budget.EmployeeId));
            }

            query = query.Where(a => a.Budget.IsDeleted == false && a.Budget.Validated == true);

            //query = query.GroupBy(item => item.Budget.ProjectId)
            //        .Select(group => new BudgetDetail()
            //        {
            //            Budget = query.First().Budget,
            //              //= group.Key,
            //              //Orders = group.ToList()
            //          })
            //        .AsQueryable();


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
				//query = sorting.Direction.ToLower() == "asc"
				//	? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetDetail>(sorting.Column))
				//	: query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetDetail>(sorting.Column));
			}

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            //list = list.GroupBy(item => item.Budget.ProjectId)
            //        .Select(group => new BudgetDetail()
            //        {
            //            Budget = list.First().Budget,
            //              // = group.Key,
            //              //Orders = group.ToList()
            //          })
            //        .ToList();

            return list;
        }
        public IEnumerable<Model.BudgetDetail> GetBugetUI(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Budget> budgetQuery = null;
            IQueryable<BudgetDetail> query = null;

            budgetQuery = _context.Budgets.AsNoTracking().AsQueryable();

            if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Project.Code.Contains(budgetFilter.Filter) || a.Code.Contains(budgetFilter.Filter)));


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
                    case "Budget":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new BudgetDetail { Budget = budget });

            if ((budgetFilter.CompanyIds != null) && (budgetFilter.CompanyIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetDetail, int?>((id) => { return a => a.Budget.CompanyId == id; }, budgetFilter.CompanyIds));
            }

            if ((budgetFilter.EmployeeIds != null) && (budgetFilter.EmployeeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetDetail, int?>((id) => { return a => a.Budget.EmployeeId == id; }, budgetFilter.EmployeeIds));
            }

            if ((budgetFilter.AdmCenterIds != null) && (budgetFilter.AdmCenterIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetDetail, int?>((id) => { return a => a.Budget.AdmCenterId == id; }, budgetFilter.AdmCenterIds));
            }

            if ((budgetFilter.AssetTypeIds != null) && (budgetFilter.AssetTypeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetDetail, int?>((id) => { return a => a.Budget.AssetTypeId == id; }, budgetFilter.AssetTypeIds));
            }

            query = query.Where(a => a.Budget.IsDeleted == false && a.Budget.Validated == true && a.Budget.IsAccepted == true && a.Budget.AccMonthId == 36);



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
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public int CreateOrUpdateBudget(BudgetSave budgetDto)
        {
            Model.Budget budget = null;
            Model.BudgetOp budgetOp = null;
            Model.Document document = null;
            Model.DocumentType documentType = null;
            Model.EntityType entityType = null;

   //         if (budgetDto.Id > 0)
   //         {
   //             budget = _context.Set<Model.Budget>().Where(a => a.Id == budgetDto.Id).Single();

   //             budget.AccMonthId = budgetDto.AccMonthId;
   //             budget.AccountId = budgetDto.AccountId;
   //             budget.AdministrationId = budgetDto.AdministrationId;
   //             budget.Code = budgetDto.Code;
   //             budget.CompanyId = budgetDto.CompanyId;
   //             budget.CostCenterId = budgetDto.CostCenterId;
   //             budget.EmployeeId = budgetDto.EmployeeId;
   //             budget.EndDate = budgetDto.EndDate;
   //             budget.Info = budgetDto.Info;
   //             budget.InterCompanyId = budgetDto.AccMonthId;
   //             budget.ModifiedAt = DateTime.Now;
   //             budget.ModifiedBy = budgetDto.UserId;
   //             budget.Name = budgetDto.Name;
   //             budget.PartnerId = budgetDto.PartnerId;
   //             budget.ProjectId = budgetDto.ProjectId;
   //             budget.Quantity = budgetDto.Quantity;
   //             budget.StartDate = budgetDto.StartDate;
   //             budget.SubTypeId = budgetDto.SubTypeId;
   //             budget.UserId = budgetDto.UserId;
   //             budget.ValueIni = budgetDto.ValueIni;
   //             budget.ValueFin = budgetDto.ValueFin;

   //             _context.Set<Model.Budget>().Update(budget);
			//}
   //         else
   //         {
   //             entityType = _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWBUDGET").FirstOrDefault();

   //             var lastCode = int.Parse(entityType.Name);
   //             var newBudgetCode = entityType.Code + lastCode.ToString();


   //             documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_BUDGET").SingleOrDefault();

   //             document = new Model.Document()
   //             {
   //                 Approved = true,
   //                 CompanyId = budgetDto.CompanyId,
   //                 CostCenterId = budgetDto.CostCenterId,
   //                 CreatedAt = DateTime.Now,
   //                 CreatedBy = budgetDto.UserId,
   //                 CreationDate = DateTime.Now,
   //                 Details = budgetDto.Info != null ? budgetDto.Info : string.Empty,
   //                 DocNo1 = budgetDto.Info != null ? budgetDto.Info : string.Empty,
   //                 DocNo2 = budgetDto.Info != null ? budgetDto.Info : string.Empty,
   //                 DocumentDate = DateTime.Now,
   //                 DocumentTypeId = documentType.Id,
   //                 Exported = true,
   //                 IsDeleted = false,
   //                 ModifiedAt = DateTime.Now,
   //                 ModifiedBy = budgetDto.UserId,
   //                 ParentDocumentId = null,
   //                 PartnerId = budgetDto.PartnerId,
   //                 RegisterDate = DateTime.Now,
   //                 ValidationDate = DateTime.Now
   //             };

   //             _context.Add(document);


   //             budget = new Model.Budget()
   //             {
   //                 AccMonthId = budgetDto.AccMonthId,
   //                 AccountId = budgetDto.AccountId,
   //                 AdministrationId = budgetDto.AdministrationId,
   //                 AppStateId = 1,
   //                 BudgetManagerId = null,
   //                 Code = newBudgetCode,
   //                 CompanyId = budgetDto.CompanyId,
   //                 CostCenterId = budgetDto.CostCenterId,
   //                 CreatedAt = DateTime.Now,
   //                 CreatedBy = budgetDto.UserId,
   //                 EmployeeId = budgetDto.EmployeeId,
   //                 EndDate = budgetDto.StartDate,
   //                 StartDate = budgetDto.EndDate,
   //                 Info = budgetDto.Info,
   //                 InterCompanyId = budgetDto.InterCompanyId,
   //                 IsAccepted = false,
   //                 IsDeleted = false,
   //                 ModifiedAt = DateTime.Now,
   //                 ModifiedBy = budgetDto.UserId,
   //                 // Name = newBudgetCode,
   //                 PartnerId = budgetDto.PartnerId,
   //                 ProjectId = budgetDto.ProjectId,
   //                 Quantity = budgetDto.Quantity,
   //                 SubTypeId = budgetDto.SubTypeId,
   //                 UserId = budgetDto.UserId,
   //                 Validated = true,
   //                 ValueFin = budgetDto.ValueFin,
   //                 ValueIni = budgetDto.ValueIni,
   //                 Guid = Guid.NewGuid()


   //             };
   //             _context.Add(budget);

   //             budgetOp = new Model.BudgetOp()
   //             {
   //                 AccMonthId = budgetDto.AccMonthId,
   //                 AccSystemId = null,
   //                 AccountIdInitial = budgetDto.AccountId,
   //                 AccountIdFinal = budgetDto.AccountId,
   //                 AdministrationIdInitial = budgetDto.AdministrationId,
   //                 AdministrationIdFinal = budgetDto.AdministrationId,
   //                 Budget = budget,
   //                 BudgetManagerIdInitial = null,
   //                 BudgetManagerIdFinal = null,
   //                 BudgetStateId = 1,
   //                 CompanyIdInitial = budgetDto.CompanyId,
   //                 CompanyIdFinal = budgetDto.CompanyId,
   //                 CostCenterIdInitial = budgetDto.CostCenterId,
   //                 CostCenterIdFinal = budgetDto.CostCenterId,
   //                 CreatedAt = DateTime.Now,
   //                 CreatedBy = budgetDto.UserId,
   //                 Document = document,
   //                 DstConfAt = DateTime.Now,
   //                 DstConfBy = budgetDto.UserId,
   //                 EmployeeIdInitial = budgetDto.EmployeeId,
   //                 EmployeeIdFinal = budgetDto.EmployeeId,
   //                 InfoIni = budgetDto.Info,
   //                 InfoFin = budgetDto.Info,
   //                 InterCompanyIdInitial = budgetDto.InterCompanyId,
   //                 InterCompanyIdFinal = budgetDto.InterCompanyId,
   //                 IsAccepted = false,
   //                 IsDeleted = false,
   //                 ModifiedAt = DateTime.Now,
   //                 ModifiedBy = budgetDto.UserId,
   //                 PartnerIdInitial = budgetDto.PartnerId,
   //                 PartnerIdFinal = budgetDto.PartnerId,
   //                 ProjectIdInitial = budgetDto.ProjectId,
   //                 ProjectIdFinal = budgetDto.ProjectId,
   //                 QuantityIni = budgetDto.Quantity,
   //                 QuantityFin = budgetDto.Quantity,
   //                 SubTypeIdInitial = budgetDto.SubTypeId,
   //                 SubTypeIdFinal = budgetDto.SubTypeId,
   //                 Validated = true,
   //                 ValueFin1 = budgetDto.ValueFin,
   //                 ValueIni1 = budgetDto.ValueIni,
   //                 ValueFin2 = budgetDto.ValueFin,
   //                 ValueIni2 = budgetDto.ValueIni,
   //                 Guid = Guid.NewGuid()
   //             };

   //             _context.Add(budgetOp);

   //         }

            entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
            _context.Update(entityType);

            _context.SaveChanges();

            return budget.Id;
        }

        public Model.Budget GetDetailsById(int id, string includes)
        {
            IQueryable<Model.Budget> query = null;
            query = GetBudgetQuery(includes);

            return query.AsNoTracking().Where(a => a.Id == id)
                .Include(b => b.Company)
                .Include(b => b.Project)
                .Include(b => b.Administration)
                .Include(b => b.SubType)
                    .ThenInclude(t => t.Type)
                        .ThenInclude(m => m.MasterType)
                .Include(b => b.Employee)
                .Include(b => b.AccMonth)
                
                .Include(b => b.Partner)
                .Include(b => b.CostCenter)
                .SingleOrDefault();
        }

        private IQueryable<Model.Budget> GetBudgetQuery(string includes)
        {
            IQueryable<Model.Budget> query = null;
            query = _context.Budgets.AsNoTracking();

            return query;
        }


        public int SendEmail(int budgetId, string userName, out string emailIniOut, out string emailCCOut, out string bodyHtmlOut, out string subjectOut)
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
            var subject = "New budget validation";

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
                        .ThenInclude(d => d.Department)
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
             var budgetLink = "https://service.inventare.ro/Emag/#/budgetvalidate/" + budget.Guid.ToString();
            //var budgetLink = "http://localhost:3100/#/budgetvalidate/" + budget.Guid.ToString();
            var link = @"<h4><span style=""font-family: Roboto,Montserrat,helvetica neue,Helvetica,Arial,sans-serif;font-size: 12px;color: rgb(115, 115, 115)"">To review and validate new budget, please access the following link: <a style=""color: red; font-size: 16px;"" href = '" + budgetLink + "'" + "' >  VALIDATE BUDGET: " + budget.Code + "</a>" + @"</span></h4>";
            var linkInfo = @"<h4><span style=""font-family: Roboto,Montserrat,helvetica neue,Helvetica,Arial,sans-serif;font-size: 12px;color: rgb(115, 115, 115)"">(If the link cannot be accessed with IE, please try using Chrome)" + @"</span></h4>";

            emailIniOut = emailIni;
            emailCCOut = emailCC;
            // bodyHtmlOut = htmlBody + htmlBodyEmail + htmlBodyEnd;
            bodyHtmlOut = htmlBody + htmlBodyEmail + htmlBodyEnd + link + linkInfo;
            subjectOut = subject;

            _context.SaveChanges();

            return budgetId;


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

        public IEnumerable<Model.BudgetDetail> BudgetValidate(BudgetFilter budgetFilter, string includes, string userId, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.Budget> budgetQuery = null;
            IQueryable<BudgetDetail> query = null;

            budgetQuery = _context.Budgets.AsNoTracking().AsQueryable();

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
                    case "Budget":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new BudgetDetail { Budget = budget });

            if (userId != "" && userId != null)
            {
                query = query.Where(a => a.Budget.Guid.ToString() == userId);
            }
            else
            {
                query = query.Where(a => a.Budget.Guid.ToString() == "1234");
            }

            query = query.Where(a => a.Budget.IsDeleted == false && a.Budget.Validated == true);



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
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

		public int BudgetImport(BudgetImport budgetDto)
		{
			Model.Budget budget = null;
			Model.BudgetOp budgetOp = null;
			Model.Document document = null;
			Model.DocumentType documentType = null;
			Model.EntityType entityType = null;
			Model.Company company = null;
			Model.Country country = null;
			Model.Project project = null;
			Model.Activity activity = null;
			Model.AdmCenter admCenter = null;
			Model.Region region = null;
			Model.AssetType assetType = null;
			Model.ProjectType projectType = null;
			Model.AppState appState = null;
			// Model.Inventory inventory = null;
			Model.BudgetType budgetType = null;
			Model.BudgetType budgetTypeTotal = null;
			Model.AccMonth accMonth = null;
			Model.Department department = null;
			Model.Division division = null;
			Model.Uom uom = null;

			Model.BudgetMonth budgetMonth1 = null;
			Model.BudgetMonth budgetMonth2 = null;
			Model.BudgetMonth budgetMonth3 = null;
			Model.BudgetMonth budgetMonth4 = null;
			Model.BudgetMonth budgetMonth5 = null;
			Model.BudgetMonth budgetMonth6 = null;
			Model.BudgetMonth budgetMonth7 = null;
			Model.BudgetMonth budgetMonth8 = null;
			Model.BudgetMonth budgetMonth9 = null;
			Model.BudgetMonth budgetMonth10 = null;
			Model.BudgetMonth budgetMonth11 = null;
			Model.BudgetMonth budgetMonth12 = null;
			//Model.BudgetMonth budgetTotal = null;

			_context.UserId = budgetDto.UserId;

			accMonth = _context.Set<Model.AccMonth>().Where(i => i.IsActive == true && i.IsDeleted == false).SingleOrDefault();

			budgetType = _context.Set<Model.BudgetType>().Where(i => i.Code == "A" && i.IsDeleted == false).SingleOrDefault();

			uom = _context.Set<Model.Uom>().Where(i => i.Code == "RON" && i.IsDeleted == false).SingleOrDefault();

			if (budgetType == null)
			{
				budgetType = new Model.BudgetType
				{
					Code = "A".Trim(),
					Name = "A".Trim(),
					IsDeleted = false
				};
				_context.Set<Model.BudgetType>().Add(budgetType);
			}
			//budgetTypeTotal = _context.Set<Model.BudgetType>().Where(i => i.Code == "B" && i.IsDeleted == false).SingleOrDefault();

			company = _context.Set<Model.Company>().Where(c => c.Code == budgetDto.Company && c.IsDeleted == false).SingleOrDefault();

			if (company == null)
			{
				company = new Model.Company
				{
					Code = budgetDto.Company.Trim(),
					Name = budgetDto.Company.Trim(),
					IsDeleted = false
				};
				_context.Set<Model.Company>().Add(company);
			}


			country = _context.Set<Model.Country>().Where(c => c.Name == budgetDto.Country && c.IsDeleted == false).SingleOrDefault();

			if (country == null)
			{
				country = new Model.Country
				{
					Code = budgetDto.Country.Trim(),
					Name = budgetDto.Country.Trim(),
					IsDeleted = false
				};
				_context.Set<Model.Country>().Add(country);
			}

			project = _context.Set<Model.Project>().Where(c => c.Name == budgetDto.Project && c.IsDeleted == false).SingleOrDefault();

			if (project == null)
			{
				project = new Model.Project
				{
					Code = budgetDto.Project.Trim(),
					Name = budgetDto.Project.Trim(),
					IsDeleted = false
				};
				_context.Set<Model.Project>().Add(project);
			}

			activity = _context.Set<Model.Activity>().Where(c => c.Name == budgetDto.Activity && c.IsDeleted == false).SingleOrDefault();

			if (activity == null)
			{
				activity = new Model.Activity
				{
					Code = budgetDto.Activity.Trim(),
					Name = budgetDto.Activity.Trim(),
					IsDeleted = false
				};
				_context.Set<Model.Activity>().Add(activity);


			}

			department = _context.Set<Model.Department>().Where(c => c.Name == budgetDto.Activity && c.IsDeleted == false).SingleOrDefault();

			if (department == null)
			{
				department = new Model.Department
				{
					Code = budgetDto.Activity.Trim(),
					Name = budgetDto.Activity.Trim(),
					IsDeleted = false
				};
				_context.Set<Model.Department>().Add(department);


			}

			division = _context.Set<Model.Division>().Where(c => c.Name == budgetDto.Activity && c.IsDeleted == false).SingleOrDefault();

			if (division == null)
			{
				division = new Model.Division
				{
					Code = budgetDto.Activity.Trim(),
					Name = budgetDto.Activity.Trim(),
					IsDeleted = false
				};
				_context.Set<Model.Division>().Add(division);


			}


			admCenter = _context.Set<Model.AdmCenter>().Where(c => c.Name == budgetDto.AdmCenter && c.IsDeleted == false).SingleOrDefault();

			if (admCenter == null)
			{
				admCenter = new Model.AdmCenter
				{
					Code = budgetDto.AdmCenter,
					Name = budgetDto.AdmCenter,
					IsDeleted = false
				};
				_context.Set<Model.AdmCenter>().Add(admCenter);
			}


			region = _context.Set<Model.Region>().Where(c => c.Name == budgetDto.Region && c.IsDeleted == false).SingleOrDefault();

			if (region == null)
			{
				region = new Model.Region
				{
					Code = budgetDto.Region,
					Name = budgetDto.Region,
					IsDeleted = false
				};
				_context.Set<Model.Region>().Add(region);
			}


			assetType = _context.Set<Model.AssetType>().Where(c => c.Name == budgetDto.AssetType && c.IsDeleted == false).SingleOrDefault();

			if (assetType == null)
			{
				assetType = new Model.AssetType
				{
					Code = budgetDto.AssetType.Trim(),
					Name = budgetDto.AssetType.Trim(),
					IsDeleted = false
				};
				_context.Set<Model.AssetType>().Add(assetType);
			}


			projectType = _context.Set<Model.ProjectType>().Where(c => c.Name == budgetDto.ProjectType && c.IsDeleted == false).SingleOrDefault();

			if (projectType == null)
			{
				projectType = new Model.ProjectType
				{
					Code = budgetDto.ProjectType.Trim(),
					Name = budgetDto.ProjectType.Trim(),
					IsDeleted = false
				};
				_context.Set<Model.ProjectType>().Add(projectType);
			}


			appState = _context.Set<Model.AppState>().Where(c => c.Name == budgetDto.AppState && c.IsDeleted == false).SingleOrDefault();


			if (appState == null)
			{
				appState = new Model.AppState
				{
					Code = budgetDto.AppState.Trim(),
					Name = budgetDto.AppState.Trim(),
					IsDeleted = false
				};
				_context.Set<Model.AppState>().Add(appState);
			}

			if (budgetDto.BudgetCode != "" && budgetDto.BudgetCode != null)
			{
				//budget = _context.Set<Model.Budget>().Where(a => a.Id == budgetDto.Id).Single();

				//budget.AccMonthId = budgetDto.AccMonthId;
				//budget.AccountId = budgetDto.AccountId;
				//budget.AdministrationId = budgetDto.AdministrationId;
				//budget.Code = budgetDto.Code;
				//budget.CompanyId = budgetDto.CompanyId;
				//budget.CostCenterId = budgetDto.CostCenterId;
				//budget.EmployeeId = budgetDto.EmployeeId;
				//budget.EndDate = budgetDto.EndDate;
				//budget.Info = budgetDto.Info;
				//budget.InterCompanyId = budgetDto.AccMonthId;
				//budget.ModifiedAt = DateTime.Now;
				//budget.ModifiedBy = budgetDto.UserId;
				//budget.Name = budgetDto.Name;
				//budget.PartnerId = budgetDto.PartnerId;
				//budget.ProjectId = budgetDto.ProjectId;
				//budget.Quantity = budgetDto.Quantity;
				//budget.StartDate = budgetDto.StartDate;
				//budget.SubTypeId = budgetDto.SubTypeId;
				//budget.UserId = budgetDto.UserId;
				//budget.ValueIni = budgetDto.ValueIni;
				//budget.ValueFin = budgetDto.ValueFin;

				//_context.Set<Model.Budget>().Update(budget);
			}
			else
			{
				entityType = _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWBUDGET").FirstOrDefault();

				var lastCode = int.Parse(entityType.Name);
				var newBudgetCode = entityType.Code + lastCode.ToString();

				documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_BUDGET").SingleOrDefault();

				document = new Model.Document()
				{
					Approved = true,
					CompanyId = company.Id,
					CostCenterId = null,
					CreatedAt = DateTime.Now,
					CreatedBy = budgetDto.UserId,
					CreationDate = DateTime.Now,
					Details = string.Empty,
					DocNo1 = string.Empty,
					DocNo2 = string.Empty,
					DocumentDate = DateTime.Now,
					DocumentTypeId = documentType.Id,
					Exported = true,
					IsDeleted = false,
					ModifiedAt = DateTime.Now,
					ModifiedBy = budgetDto.UserId,
					ParentDocumentId = null,
					PartnerId = null,
					RegisterDate = DateTime.Now,
					ValidationDate = DateTime.Now
				};

				_context.Add(document);

				budget = new Model.Budget()
				{
					AccMonthId = accMonth.Id,
					AdministrationId = null,
					AppState = appState,
					BudgetManagerId = null,
					Code = newBudgetCode,
					Company = company,
					CostCenterId = null,
					CreatedAt = DateTime.Now,
					CreatedBy = budgetDto.UserId,
					EmployeeId = null,
					EndDate = null,
					StartDate = null,
					Info = budgetDto.Info,
					IsAccepted = true,
					IsDeleted = false,
					ModifiedAt = DateTime.Now,
					ModifiedBy = budgetDto.UserId,
					Name = newBudgetCode,
					PartnerId = null,
					Project = project,
					Country = country,
					Activity = activity,
					Department = department,
					AdmCenter = admCenter,
					Region = region,
					AssetType = assetType,
					ProjectType = projectType,
					DepPeriod = budgetDto.DepPeriod,
					DepPeriodRem = budgetDto.DepPeriodRem,
					Quantity = 0,
					SubTypeId = null,
					UserId = budgetDto.UserId,
					Validated = true,
					ValueFin = 0,
					ValueIni = 0,
					Guid = Guid.NewGuid(),
					Total = budgetDto.ValueRem,
					Division = division,
					Uom = uom


				};
				_context.Add(budget);

				budgetOp = new Model.BudgetOp()
				{
					AccMonthId = accMonth.Id,
					AccSystemId = null,
					AdministrationIdInitial = null,
					AdministrationIdFinal = null,
					Budget = budget,
					BudgetManagerIdInitial = null,
					BudgetManagerIdFinal = null,
					CompanyInitial = company,
					CompanyFinal = company,
					CostCenterIdInitial = null,
					CostCenterIdFinal = null,
					CreatedAt = DateTime.Now,
					CreatedBy = budgetDto.UserId,
					Document = document,
					DstConfAt = DateTime.Now,
					DstConfBy = budgetDto.UserId,
					EmployeeIdInitial = null,
					EmployeeIdFinal = null,
					InfoIni = budgetDto.Info,
					InfoFin = budgetDto.Info,
					IsAccepted = true,
					IsDeleted = false,
					ModifiedAt = DateTime.Now,
					ModifiedBy = budgetDto.UserId,
					PartnerIdInitial = null,
					PartnerIdFinal = null,
					ProjectInitial = project,
					ProjectFinal = project,
					QuantityIni = 0,
					QuantityFin = 0,
					SubTypeIdInitial = null,
					SubTypeIdFinal = null,
					Validated = true,
					ValueFin1 = 0,
					ValueIni1 = 0,
					ValueFin2 = 0,
					ValueIni2 = 0,
					CountryInitial = country,
					CountryFinal = country,
					ActivityInitial = activity,
					ActivityFinal = activity,
					AdmCenterInitial = admCenter,
					AdmCenterFinal = admCenter,
					RegionInitial = region,
					RegionFinal = region,
					AssetTypeInitial = assetType,
					AssetTypeFinal = assetType,
					ProjectTypeInitial = projectType,
					ProjectTypeFinal = projectType,
					BudgetStateFinal = appState,
					BudgetState = appState,
					Guid = Guid.NewGuid()
				};

				_context.Add(budgetOp);

				var accMonth1 = _context.Set<Model.AccMonth>().Where(a => a.Month == 4 && a.Year == 2021).SingleOrDefault();
				var accMonth2 = _context.Set<Model.AccMonth>().Where(a => a.Month == 5 && a.Year == 2021).SingleOrDefault();
				var accMonth3 = _context.Set<Model.AccMonth>().Where(a => a.Month == 6 && a.Year == 2021).SingleOrDefault();
				var accMonth4 = _context.Set<Model.AccMonth>().Where(a => a.Month == 7 && a.Year == 2021).SingleOrDefault();
				var accMonth5 = _context.Set<Model.AccMonth>().Where(a => a.Month == 8 && a.Year == 2021).SingleOrDefault();
				var accMonth6 = _context.Set<Model.AccMonth>().Where(a => a.Month == 9 && a.Year == 2021).SingleOrDefault();
				var accMonth7 = _context.Set<Model.AccMonth>().Where(a => a.Month == 10 && a.Year == 2021).SingleOrDefault();
				var accMonth8 = _context.Set<Model.AccMonth>().Where(a => a.Month == 11 && a.Year == 2021).SingleOrDefault();
				var accMonth9 = _context.Set<Model.AccMonth>().Where(a => a.Month == 12 && a.Year == 2021).SingleOrDefault();
				var accMonth10 = _context.Set<Model.AccMonth>().Where(a => a.Month == 1 && a.Year == 2022).SingleOrDefault();
				var accMonth11 = _context.Set<Model.AccMonth>().Where(a => a.Month == 2 && a.Year == 2022).SingleOrDefault();
				var accMonth12 = _context.Set<Model.AccMonth>().Where(a => a.Month == 3 && a.Year == 2022).SingleOrDefault();

				var sumMonth1 = 0;
				var sumMonth2 = budgetDto.ValueMonth1;
				var sumMonth3 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2;
				var sumMonth4 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3;
				var sumMonth5 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4;
				var sumMonth6 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5;
				var sumMonth7 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6;
				var sumMonth8 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7;
				var sumMonth9 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8;
				var sumMonth10 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8 + budgetDto.ValueMonth9;
				var sumMonth11 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8 + budgetDto.ValueMonth9 + budgetDto.ValueMonth10;
				var sumMonth12 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8 + budgetDto.ValueMonth9 + budgetDto.ValueMonth10 + budgetDto.ValueMonth11;

				var startMonth = budgetDto.StartMonth;
				int month = 0;

				if (startMonth != null && startMonth != "")
				{
					if (startMonth.ToUpper() == "APRILIE")
					{
						month = 1;

					}
					else if (startMonth.ToUpper() == "MAI")
					{
						month = 2;
					}
					else if (startMonth.ToUpper() == "IUNIE")
					{
						month = 3;
					}
					else if (startMonth.ToUpper() == "IULIE")
					{
						month = 4;
					}
					else if (startMonth.ToUpper() == "AUGUST")
					{
						month = 5;
					}
					else if (startMonth.ToUpper() == "SEPTEMBRIE")
					{
						month = 6;
					}
					else if (startMonth.ToUpper() == "OCTOMBRIE")
					{
						month = 7;
					}
					else if (startMonth.ToUpper() == "NOIEMBRIE")
					{
						month = 8;
					}
					else if (startMonth.ToUpper() == "DECEMBRIE")
					{
						month = 9;
					}
					else if (startMonth.ToUpper() == "IANUARIE")
					{
						month = 10;
					}
					else if (startMonth.ToUpper() == "FABRUARIE")
					{
						month = 11;
					}
					else if (startMonth.ToUpper() == "MARTIE")
					{
						month = 12;
					}

				}

				budgetMonth1 = new Model.BudgetMonth()
				{
					BudgetId = budget.Id,
					BudgetType = budgetType,
					Value = budgetDto.ValueMonth1,
					ValueDep =
						budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
						budgetDto.DepPeriodRem == 1 ? budgetDto.ValueMonth1 :
						budgetDto.DepPeriodRem > 0 && sumMonth1 > 0 ? sumMonth1 / budgetDto.DepPeriodRem :
						budgetDto.DepPeriod > 0 && sumMonth1 > 0 ? sumMonth1 / budgetDto.DepPeriod : 0,
					//CreatedAt = DateTime.Now,
					//CreatedBy = budgetDto.UserId,
					//ModifiedAt = DateTime.Now,
					//ModifiedBy = budgetDto.UserId,
					//IsDeleted = false,
					AccMonth = accMonth1
				};

				_context.Add(budgetMonth1);



				budgetMonth2 = new Model.BudgetMonth()
				{
					BudgetId = budget.Id,
					BudgetType = budgetType,
					Value = budgetDto.ValueMonth2,
					ValueDep =
						budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
						budgetDto.DepPeriodRem == 1 ? budgetDto.ValueMonth2 :
						budgetDto.DepPeriodRem > 0 && sumMonth2 > 0 ? sumMonth2 / budgetDto.DepPeriodRem :
						budgetDto.DepPeriod > 0 && sumMonth2 > 0 ? sumMonth2 / budgetDto.DepPeriod : 0,
					//CreatedAt = DateTime.Now,
					//CreatedBy = budgetDto.UserId,
					//ModifiedAt = DateTime.Now,
					//ModifiedBy = budgetDto.UserId,
					//IsDeleted = false,
					AccMonth = accMonth2
				};

				_context.Add(budgetMonth2);

				budgetMonth3 = new Model.BudgetMonth()
				{
					BudgetId = budget.Id,
					BudgetType = budgetType,
					Value = budgetDto.ValueMonth3,
					ValueDep =
						budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
						budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth3 :
						budgetDto.DepPeriodRem > 0 && sumMonth3 > 0 ? sumMonth3 / budgetDto.DepPeriodRem :
						budgetDto.DepPeriod > 0 && sumMonth3 > 0 ? sumMonth3 / budgetDto.DepPeriod : 0,
					//CreatedAt = DateTime.Now,
					//CreatedBy = budgetDto.UserId,
					//ModifiedAt = DateTime.Now,
					//ModifiedBy = budgetDto.UserId,
					//IsDeleted = false,
					AccMonth = accMonth3
				};

				_context.Add(budgetMonth3);

				budgetMonth4 = new Model.BudgetMonth()
				{
					BudgetId = budget.Id,
					BudgetType = budgetType,
					Value = budgetDto.ValueMonth4,
					ValueDep =
						budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
						budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth4 :
						budgetDto.DepPeriodRem > 0 && sumMonth4 > 0 ? sumMonth4 / budgetDto.DepPeriodRem :
						budgetDto.DepPeriod > 0 && sumMonth4 > 0 ? sumMonth4 / budgetDto.DepPeriod : 0,
					//CreatedAt = DateTime.Now,
					//CreatedBy = budgetDto.UserId,
					//ModifiedAt = DateTime.Now,
					//ModifiedBy = budgetDto.UserId,
					//IsDeleted = false,
					AccMonth = accMonth4
				};

				_context.Add(budgetMonth4);

				budgetMonth5 = new Model.BudgetMonth()
				{
					BudgetId = budget.Id,
					BudgetType = budgetType,
					Value = budgetDto.ValueMonth5,
					ValueDep =
						budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
						budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth5 :
						budgetDto.DepPeriodRem > 0 && sumMonth5 > 0 ? sumMonth5 / budgetDto.DepPeriodRem :
						budgetDto.DepPeriod > 0 && sumMonth5 > 0 ? sumMonth5 / budgetDto.DepPeriod : 0,
					//CreatedAt = DateTime.Now,
					//CreatedBy = budgetDto.UserId,
					//ModifiedAt = DateTime.Now,
					//ModifiedBy = budgetDto.UserId,
					//IsDeleted = false,
					AccMonth = accMonth5
				};

				_context.Add(budgetMonth5);

				budgetMonth6 = new Model.BudgetMonth()
				{
					BudgetId = budget.Id,
					BudgetType = budgetType,
					Value = budgetDto.ValueMonth6,
					ValueDep =
						budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
						budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth6 :
						budgetDto.DepPeriodRem > 0 && sumMonth6 > 0 ? sumMonth6 / budgetDto.DepPeriodRem :
						budgetDto.DepPeriod > 0 && sumMonth6 > 0 ? sumMonth6 / budgetDto.DepPeriod : 0,
					//CreatedAt = DateTime.Now,
					//CreatedBy = budgetDto.UserId,
					//ModifiedAt = DateTime.Now,
					//ModifiedBy = budgetDto.UserId,
					//IsDeleted = false,
					AccMonth = accMonth6
				};

				_context.Add(budgetMonth6);

				budgetMonth7 = new Model.BudgetMonth()
				{
					BudgetId = budget.Id,
					BudgetType = budgetType,
					Value = budgetDto.ValueMonth7,
					ValueDep =
						budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
						budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth7 :
						budgetDto.DepPeriodRem > 0 && sumMonth7 > 0 ? sumMonth7 / budgetDto.DepPeriodRem :
						budgetDto.DepPeriod > 0 && sumMonth7 > 0 ? sumMonth7 / budgetDto.DepPeriod : 0,
					//CreatedAt = DateTime.Now,
					//CreatedBy = budgetDto.UserId,
					//ModifiedAt = DateTime.Now,
					//ModifiedBy = budgetDto.UserId,
					//IsDeleted = false,
					AccMonth = accMonth7
				};

				_context.Add(budgetMonth7);

				budgetMonth8 = new Model.BudgetMonth()
				{
					BudgetId = budget.Id,
					BudgetType = budgetType,
					Value = budgetDto.ValueMonth8,
					ValueDep =
						budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
						budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth8 :
						budgetDto.DepPeriodRem > 0 && sumMonth8 > 0 ? sumMonth8 / budgetDto.DepPeriodRem :
						budgetDto.DepPeriod > 0 && sumMonth8 > 0 ? sumMonth8 / budgetDto.DepPeriod : 0,
					//CreatedAt = DateTime.Now,
					//CreatedBy = budgetDto.UserId,
					//ModifiedAt = DateTime.Now,
					//ModifiedBy = budgetDto.UserId,
					//IsDeleted = false,
					AccMonth = accMonth8
				};

				_context.Add(budgetMonth8);

				budgetMonth9 = new Model.BudgetMonth()
				{
					BudgetId = budget.Id,
					BudgetType = budgetType,
					Value = budgetDto.ValueMonth9,
					ValueDep =
						budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
						budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth9 :
						budgetDto.DepPeriodRem > 0 && sumMonth9 > 0 ? sumMonth9 / budgetDto.DepPeriodRem :
						budgetDto.DepPeriod > 0 && sumMonth9 > 0 ? sumMonth9 / budgetDto.DepPeriod : 0,
					//CreatedAt = DateTime.Now,
					//CreatedBy = budgetDto.UserId,
					//ModifiedAt = DateTime.Now,
					//ModifiedBy = budgetDto.UserId,
					//IsDeleted = false,
					AccMonth = accMonth9
				};

				_context.Add(budgetMonth9);

				budgetMonth10 = new Model.BudgetMonth()
				{
					BudgetId = budget.Id,
					BudgetType = budgetType,
					Value = budgetDto.ValueMonth10,
					ValueDep =
						budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
						budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth10 :
						budgetDto.DepPeriodRem > 0 && sumMonth10 > 0 ? sumMonth10 / budgetDto.DepPeriodRem :
						budgetDto.DepPeriod > 0 && sumMonth10 > 0 ? sumMonth10 / budgetDto.DepPeriod : 0,
					//CreatedAt = DateTime.Now,
					//CreatedBy = budgetDto.UserId,
					//ModifiedAt = DateTime.Now,
					//ModifiedBy = budgetDto.UserId,
					//IsDeleted = false,
					AccMonth = accMonth10
				};

				_context.Add(budgetMonth10);

				budgetMonth11 = new Model.BudgetMonth()
				{
					BudgetId = budget.Id,
					BudgetType = budgetType,
					Value = budgetDto.ValueMonth11,
					ValueDep =
						budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
						budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth11 :
						budgetDto.DepPeriodRem > 0 && sumMonth11 > 0 ? sumMonth11 / budgetDto.DepPeriodRem :
						budgetDto.DepPeriod > 0 && sumMonth11 > 0 ? sumMonth11 / budgetDto.DepPeriod : 0,
					//CreatedAt = DateTime.Now,
					//CreatedBy = budgetDto.UserId,
					//ModifiedAt = DateTime.Now,
					//ModifiedBy = budgetDto.UserId,
					//IsDeleted = false,
					AccMonth = accMonth11
				};

				_context.Add(budgetMonth11);

				budgetMonth12 = new Model.BudgetMonth()
				{
					BudgetId = budget.Id,
					BudgetType = budgetType,
					Value = budgetDto.ValueMonth12,
					ValueDep =
						budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
						budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth12 :
						budgetDto.DepPeriodRem > 0 && sumMonth12 > 0 ? sumMonth12 / budgetDto.DepPeriodRem :
						budgetDto.DepPeriod > 0 && sumMonth12 > 0 ? sumMonth12 / budgetDto.DepPeriod : 0,

                    //budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    //    budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth12 :
                    //    budgetDto.DepPeriodRem > 0 && sumMonth12 > 0 ? sumMonth12 / budgetDto.DepPeriodRem :
                    //    budgetDto.DepPeriod > 0 && sumMonth12 > 0 ? sumMonth12 / budgetDto.DepPeriod : 0
                    //CreatedAt = DateTime.Now,
                    //CreatedBy = budgetDto.UserId,
                    //ModifiedAt = DateTime.Now,
                    //ModifiedBy = budgetDto.UserId,
                    //IsDeleted = false,
                    AccMonth = accMonth12
				};

				_context.Add(budgetMonth12);

				//var totalValue = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 +
				//                 budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8 +
				//                 budgetDto.ValueMonth9 + budgetDto.ValueMonth10 + budgetDto.ValueMonth11 + budgetDto.ValueMonth12;

				//budgetTotal = new Model.BudgetMonth()
				//{
				//    BudgetId = budget.Id,
				//    BudgetType = budgetTypeTotal,
				//    Value = totalValue,
				//    ValueDep = totalValue,
				//    //CreatedAt = DateTime.Now,
				//    //CreatedBy = budgetDto.UserId,
				//    //ModifiedAt = DateTime.Now,
				//    //ModifiedBy = budgetDto.UserId,
				//    //IsDeleted = false,
				//    AccMonth = accMonth12
				//};

				//_context.Add(budgetTotal);

				if (month > 0)
				{
					if (month == 2)
					{
						budgetMonth1.ValueDep = 0;

					}
					else if (month == 3)
					{
						budgetMonth1.ValueDep = 0;
						budgetMonth2.ValueDep = 0;

					}
					else if (month == 4)
					{
						budgetMonth1.ValueDep = 0;
						budgetMonth2.ValueDep = 0;
						budgetMonth3.ValueDep = 0;

					}
					else if (month == 5)
					{
						budgetMonth1.ValueDep = 0;
						budgetMonth2.ValueDep = 0;
						budgetMonth3.ValueDep = 0;
						budgetMonth4.ValueDep = 0;

					}
					else if (month == 6)
					{
						budgetMonth1.ValueDep = 0;
						budgetMonth2.ValueDep = 0;
						budgetMonth3.ValueDep = 0;
						budgetMonth4.ValueDep = 0;
						budgetMonth5.ValueDep = 0;

					}
					else if (month == 7)
					{
						budgetMonth1.ValueDep = 0;
						budgetMonth2.ValueDep = 0;
						budgetMonth3.ValueDep = 0;
						budgetMonth4.ValueDep = 0;
						budgetMonth5.ValueDep = 0;
						budgetMonth6.ValueDep = 0;

					}
					else if (month == 8)
					{
						budgetMonth1.ValueDep = 0;
						budgetMonth2.ValueDep = 0;
						budgetMonth3.ValueDep = 0;
						budgetMonth4.ValueDep = 0;
						budgetMonth5.ValueDep = 0;
						budgetMonth6.ValueDep = 0;
						budgetMonth7.ValueDep = 0;

					}
					else if (month == 9)
					{
						budgetMonth1.ValueDep = 0;
						budgetMonth2.ValueDep = 0;
						budgetMonth3.ValueDep = 0;
						budgetMonth4.ValueDep = 0;
						budgetMonth5.ValueDep = 0;
						budgetMonth6.ValueDep = 0;
						budgetMonth7.ValueDep = 0;
						budgetMonth8.ValueDep = 0;

					}
					else if (month == 10)
					{
						budgetMonth1.ValueDep = 0;
						budgetMonth2.ValueDep = 0;
						budgetMonth3.ValueDep = 0;
						budgetMonth4.ValueDep = 0;
						budgetMonth5.ValueDep = 0;
						budgetMonth6.ValueDep = 0;
						budgetMonth7.ValueDep = 0;
						budgetMonth8.ValueDep = 0;
						budgetMonth9.ValueDep = 0;

					}
					else if (month == 11)
					{
						budgetMonth1.ValueDep = 0;
						budgetMonth2.ValueDep = 0;
						budgetMonth3.ValueDep = 0;
						budgetMonth4.ValueDep = 0;
						budgetMonth5.ValueDep = 0;
						budgetMonth6.ValueDep = 0;
						budgetMonth7.ValueDep = 0;
						budgetMonth8.ValueDep = 0;
						budgetMonth9.ValueDep = 0;
						budgetMonth10.ValueDep = 0;

					}
					else if (month == 12)
					{
						budgetMonth1.ValueDep = 0;
						budgetMonth2.ValueDep = 0;
						budgetMonth3.ValueDep = 0;
						budgetMonth4.ValueDep = 0;
						budgetMonth5.ValueDep = 0;
						budgetMonth6.ValueDep = 0;
						budgetMonth7.ValueDep = 0;
						budgetMonth8.ValueDep = 0;
						budgetMonth9.ValueDep = 0;
						budgetMonth10.ValueDep = 0;
						budgetMonth11.ValueDep = 0;

					}

				}

			}

			entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
			_context.Update(entityType);

			_context.SaveChanges();

			return budget.Id;

		}

		public int BudgetBaseImport(BudgetBaseImport budgetDto)
        {
            Model.BudgetBase budget = null;
            // Model.BudgetOp budgetOp = null;
            Model.Document document = null;
            Model.DocumentType documentType = null;
            Model.EntityType entityType = null;
            Model.Company company = null;
            Model.Country country = null;
            Model.Project project = null;
            Model.Activity activity = null;
            Model.AdmCenter admCenter = null;
            Model.Region region = null;
            Model.AssetType assetType = null;
            Model.ProjectType projectType = null;
            Model.AppState appState = null;
            // Model.Inventory inventory = null;
            Model.BudgetManager budgetManager = null;
            Model.BudgetType budgetType = null;
            Model.BudgetType budgetTypeTotal = null;
            Model.AccMonth accMonth = null;
            Model.AccMonth startAccMonth = null;
            Model.Department department = null;
            Model.Division division = null;
            Model.Uom uom = null;
            Model.Employee employee = null;

            Model.BudgetMonthBase budgetMonth1 = null;
            Model.BudgetMonthBase budgetMonth2 = null;
            Model.BudgetMonthBase budgetMonth3 = null;
            Model.BudgetMonthBase budgetMonth4 = null;
            Model.BudgetMonthBase budgetMonth5 = null;
            Model.BudgetMonthBase budgetMonth6 = null;
            Model.BudgetMonthBase budgetMonth7 = null;
            Model.BudgetMonthBase budgetMonth8 = null;
            Model.BudgetMonthBase budgetMonth9 = null;
            Model.BudgetMonthBase budgetMonth10 = null;
            Model.BudgetMonthBase budgetMonth11 = null;
            Model.BudgetMonthBase budgetMonth12 = null;

            Model.BudgetForecast budgetForecast = null;
            Model.BudgetMonthBase budgetMonthBase = null;
            //Model.BudgetMonth budgetTotal = null;

            _context.UserId = budgetDto.UserId;

            accMonth = _context.Set<Model.AccMonth>().Where(i => i.Id == 37).SingleOrDefault();

            budgetType = _context.Set<Model.BudgetType>().Where(i => i.Code == "A" && i.IsDeleted == false).SingleOrDefault();

            budgetManager = _context.Set<Model.BudgetManager>().Where(i => i.Name == "2022" && i.IsDeleted == false).SingleOrDefault();

            uom = _context.Set<Model.Uom>().Where(i => i.Code == "RON" && i.IsDeleted == false).SingleOrDefault();

            if (budgetType == null)
            {
                budgetType = new Model.BudgetType
                {
                    Code = "A".Trim(),
                    Name = "A".Trim(),
                    IsDeleted = false
                };
                _context.Set<Model.BudgetType>().Add(budgetType);
            }
            //budgetTypeTotal = _context.Set<Model.BudgetType>().Where(i => i.Code == "B" && i.IsDeleted == false).SingleOrDefault();

   //         if(budgetDto.StartMonth != "" && budgetDto.StartMonth != null)
			//{
   //             startMonth = _context.Set<Model.AccMonth>().Where(i => i.GetHashCode == 37).SingleOrDefault();
   //         }
            
            company = _context.Set<Model.Company>().Where(c => c.Code == "RO10" && c.IsDeleted == false).Single();

            //if (company == null)
            //{
            //    company = new Model.Company
            //    {
            //        Code = budgetDto.Company.Trim(),
            //        Name = budgetDto.Company.Trim(),
            //        IsDeleted = false
            //    };
            //    _context.Set<Model.Company>().Add(company);
            //}

            employee = _context.Set<Model.Employee>().Where(c => c.Email == budgetDto.Employee && c.IsDeleted == false).Single();

            //if (employee == null)
            //{
            //    employee = new Model.Employee
            //    {
            //        FirstName = budgetDto.Project,
            //        Name = budgetDto.Project.Trim(),
            //        IsDeleted = false
            //    };
            //    _context.Set<Model.Employee>().Add(employee);
            //}

            project = _context.Set<Model.Project>().Where(c => c.Name == budgetDto.Project && c.IsDeleted == false).SingleOrDefault();

            //if (project == null)
            //{
            //    project = new Model.Project
            //    {
            //        Code = budgetDto.Project,
            //        Name = budgetDto.Project.Trim(),
            //        IsDeleted = false
            //    };
            //    _context.Set<Model.Project>().Add(project);
            //}

            country = _context.Set<Model.Country>().Where(c => c.Name == budgetDto.CountryName && c.IsDeleted == false).SingleOrDefault();

            //if (country == null)
            //{
            //    country = new Model.Country
            //    {
            //        Code = budgetDto.CountryCode.Trim(),
            //        Name = budgetDto.CountryName.Trim(),
            //        IsDeleted = false
            //    };
            //    _context.Set<Model.Country>().Add(country);
            //}

           

            activity = _context.Set<Model.Activity>().Where(c => c.Name == budgetDto.Activity && c.IsDeleted == false).SingleOrDefault();

            if (activity == null)
            {
                activity = new Model.Activity
                {
                    Code = budgetDto.Activity.Trim(),
                    Name = budgetDto.Activity.Trim(),
                    IsDeleted = false
                };
                _context.Set<Model.Activity>().Add(activity);
            }

            department = _context.Set<Model.Department>().Where(c => c.Code == budgetDto.DepartmentCode && c.IsDeleted == false).SingleOrDefault();

            //if (department == null)
            //{
            //    department = new Model.Department
            //    {
            //        Code = budgetDto.DepartmentCode.Trim(),
            //        Name = budgetDto.DepartmentName.Trim(),
            //        IsDeleted = false
            //    };
            //    _context.Set<Model.Department>().Add(department);


            //}

            admCenter = _context.Set<Model.AdmCenter>().Where(c => c.Name == budgetDto.AdmCenter && c.IsDeleted == false).SingleOrDefault();

            //if (admCenter == null)
            //{
            //    admCenter = new Model.AdmCenter
            //    {
            //        Code = budgetDto.AdmCenter,
            //        Name = budgetDto.AdmCenter,
            //        IsDeleted = false
            //    };
            //    _context.Set<Model.AdmCenter>().Add(admCenter);
            //}


            region = _context.Set<Model.Region>().Where(c => c.Name == budgetDto.Region && c.IsDeleted == false).SingleOrDefault();

            //if (region == null)
            //{
            //    region = new Model.Region
            //    {
            //        Code = budgetDto.Region,
            //        Name = budgetDto.Region,
            //        IsDeleted = false
            //    };
            //    _context.Set<Model.Region>().Add(region);
            //}

            division = _context.Set<Model.Division>().Where(c => c.Name == budgetDto.DivisionCode && c.IsDeleted == false).SingleOrDefault();

            //if (division == null)
            //{
            //    division = new Model.Division
            //    {
            //        Code = budgetDto.DivisionCode.Trim(),
            //        Name = budgetDto.DivisionName.Trim(),
            //        IsDeleted = false
            //    };
            //    _context.Set<Model.Division>().Add(division);


            //}


            projectType = _context.Set<Model.ProjectType>().Where(c => c.Code == budgetDto.ProjectTypeCode && c.IsDeleted == false).SingleOrDefault();

            //if (projectType == null)
            //{
            //    projectType = new Model.ProjectType
            //    {
            //        Code = budgetDto.ProjectTypeCode.Trim(),
            //        Name = budgetDto.ProjectTypeName.Trim(),
            //        IsDeleted = false
            //    };
            //    _context.Set<Model.ProjectType>().Add(projectType);
            //}


            assetType = _context.Set<Model.AssetType>().Where(c => c.Name == budgetDto.AssetTypeCode && c.IsDeleted == false).SingleOrDefault();

            //if (assetType == null)
            //{
            //    assetType = new Model.AssetType
            //    {
            //        Code = budgetDto.AssetTypeCode.Trim(),
            //        Name = budgetDto.AssetTypeName.Trim(),
            //        IsDeleted = false
            //    };
            //    _context.Set<Model.AssetType>().Add(assetType);
            //}


            appState = _context.Set<Model.AppState>().Where(c => c.Name == budgetDto.AppState && c.IsDeleted == false).SingleOrDefault();


            if (appState == null)
            {
                appState = new Model.AppState
                {
                    Code = budgetDto.AppState.Trim(),
                    Name = budgetDto.AppState.Trim(),
                    IsDeleted = false
                };
                _context.Set<Model.AppState>().Add(appState);
            }

			entityType = _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWBUDGETBASE").FirstOrDefault();

			var lastCode = int.Parse(entityType.Name);
			var newBudgetCode = entityType.Code + lastCode.ToString();

			documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_BUDGET").SingleOrDefault();

			document = new Model.Document()
			{
				Approved = true,
				CompanyId = company.Id,
				CostCenterId = null,
				CreatedAt = DateTime.Now,
				CreatedBy = budgetDto.UserId,
				CreationDate = DateTime.Now,
				Details = string.Empty,
				DocNo1 = string.Empty,
				DocNo2 = string.Empty,
				DocumentDate = DateTime.Now,
				DocumentTypeId = documentType.Id,
				Exported = true,
				IsDeleted = false,
				ModifiedAt = DateTime.Now,
				ModifiedBy = budgetDto.UserId,
				ParentDocumentId = null,
				PartnerId = null,
				RegisterDate = DateTime.Now,
				ValidationDate = DateTime.Now
			};

			_context.Add(document);

			budget = new Model.BudgetBase()
			{
				AccMonthId = accMonth.Id,
				EmployeeId = employee.Id,
				Project = project,
				Country = country,
				Activity = activity,
				Department = department,
				AdmCenter = admCenter,
				Region = region,
				Division = division,
				ProjectType = projectType,
				Info = budgetDto.Info,
				AssetType = assetType,
				AppState = appState,
				StartMonth = null,
				DepPeriod = budgetDto.DepPeriod,
				DepPeriodRem = budgetDto.DepPeriodRem,
				Code = newBudgetCode,
				Company = company,
				CreatedAt = DateTime.Now,
				CreatedBy = budgetDto.UserId,
				IsAccepted = true,
				IsDeleted = false,
				ModifiedAt = DateTime.Now,
				ModifiedBy = budgetDto.UserId,
				Name = newBudgetCode,
				UserId = budgetDto.UserId,
				Validated = true,
				ValueFin = 0,
				ValueIni = 0,
				Total = budgetDto.ValueRem,
				Uom = uom,
				//BudgetForecast = budgetForecast,
				//BudgetMonthBase = budgetMonthBase,
				BudgetType = budgetType,
				BudgetManager = budgetManager

			};
			_context.Add(budget);


			var sumMonth1 = 0;
			var sumMonth2 = budgetDto.ValueMonth1;
			var sumMonth3 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2;
			var sumMonth4 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3;
			var sumMonth5 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4;
			var sumMonth6 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5;
			var sumMonth7 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6;
			var sumMonth8 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7;
			var sumMonth9 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8;
			var sumMonth10 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8 + budgetDto.ValueMonth9;
			var sumMonth11 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8 + budgetDto.ValueMonth9 + budgetDto.ValueMonth10;
			var sumMonth12 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8 + budgetDto.ValueMonth9 + budgetDto.ValueMonth10 + budgetDto.ValueMonth11;

			var startMonth = budgetDto.StartMonth;
			int month = 0;

			if (startMonth != null && startMonth != "")
			{
				DateTime date = Convert.ToDateTime(startMonth, CultureInfo.InvariantCulture);

				month = date.Month;
				//if (startMonth.ToUpper() == "APRILIE")
				//{
				//    month = 1;

				//}
				//else if (startMonth.ToUpper() == "MAI")
				//{
				//    month = 2;
				//}
				//else if (startMonth.ToUpper() == "IUNIE")
				//{
				//    month = 3;
				//}
				//else if (startMonth.ToUpper() == "IULIE")
				//{
				//    month = 4;
				//}
				//else if (startMonth.ToUpper() == "AUGUST")
				//{
				//    month = 5;
				//}
				//else if (startMonth.ToUpper() == "SEPTEMBRIE")
				//{
				//    month = 6;
				//}
				//else if (startMonth.ToUpper() == "OCTOMBRIE")
				//{
				//    month = 7;
				//}
				//else if (startMonth.ToUpper() == "NOIEMBRIE")
				//{
				//    month = 8;
				//}
				//else if (startMonth.ToUpper() == "DECEMBRIE")
				//{
				//    month = 9;
				//}
				//else if (startMonth.ToUpper() == "IANUARIE")
				//{
				//    month = 10;
				//}
				//else if (startMonth.ToUpper() == "FABRUARIE")
				//{
				//    month = 11;
				//}
				//else if (startMonth.ToUpper() == "MARTIE")
				//{
				//    month = 12;
				//}


				if (month == 4)
				{
					month = 1;

				}
				else if (month == 5)
				{
					month = 2;
				}
				else if (month == 6)
				{
					month = 3;
				}
				else if (month == 7)
				{
					month = 4;
				}
				else if (month == 8)
				{
					month = 5;
				}
				else if (month == 9)
				{
					month = 6;
				}
				else if (month == 10)
				{
					month = 7;
				}
				else if (month == 11)
				{
					month = 8;
				}
				else if (month == 12)
				{
					month = 9;
				}
				else if (month == 1)
				{
					month = 10;
				}
				else if (month == 2)
				{
					month = 11;
				}
				else if (month == 3)
				{
					month = 12;
				}

				startAccMonth = _context.Set<Model.AccMonth>().Where(a => a.Month == month && a.Year == 2023).Single();

			}


			budgetMonthBase = new Model.BudgetMonthBase()
			{
				BudgetBaseId = budget.Id,
				BudgetManagerId = budgetManager.Id,
				BudgetType = budgetType,
				IsFirst = true,
				IsLast = true,
				April = budgetDto.ValueMonth1,
				May = budgetDto.ValueMonth2,
				June = budgetDto.ValueMonth3,
				July = budgetDto.ValueMonth4,
				August = budgetDto.ValueMonth5,
				September = budgetDto.ValueMonth6,
				Octomber = budgetDto.ValueMonth7,
				November = budgetDto.ValueMonth8,
				December = budgetDto.ValueMonth9,
				January = budgetDto.ValueMonth10,
				February = budgetDto.ValueMonth11,
				March = budgetDto.ValueMonth12
			};

			_context.Add(budgetMonthBase);


			budgetForecast = new Model.BudgetForecast()
			{
				BudgetBaseId = budget.Id,
				BudgetManagerId = budgetManager.Id,
				BudgetType = budgetType,
				IsFirst = true,
				IsLast = true,
				April =
					budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
					budgetDto.DepPeriodRem == 1 ? budgetDto.ValueMonth1 :
					budgetDto.DepPeriodRem > 0 && sumMonth1 > 0 ? sumMonth1 / budgetDto.DepPeriodRem :
					budgetDto.DepPeriod > 0 && sumMonth1 > 0 ? sumMonth1 / budgetDto.DepPeriod : 0,
				May = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
					budgetDto.DepPeriodRem == 1 ? budgetDto.ValueMonth2 :
					budgetDto.DepPeriodRem > 0 && sumMonth2 > 0 ? sumMonth2 / budgetDto.DepPeriodRem :
					budgetDto.DepPeriod > 0 && sumMonth2 > 0 ? sumMonth2 / budgetDto.DepPeriod : 0,
				June = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
					budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth3 :
					budgetDto.DepPeriodRem > 0 && sumMonth3 > 0 ? sumMonth3 / budgetDto.DepPeriodRem :
					budgetDto.DepPeriod > 0 && sumMonth3 > 0 ? sumMonth3 / budgetDto.DepPeriod : 0,
				July = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
					budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth4 :
					budgetDto.DepPeriodRem > 0 && sumMonth4 > 0 ? sumMonth4 / budgetDto.DepPeriodRem :
					budgetDto.DepPeriod > 0 && sumMonth4 > 0 ? sumMonth4 / budgetDto.DepPeriod : 0,
				August = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
					budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth5 :
					budgetDto.DepPeriodRem > 0 && sumMonth5 > 0 ? sumMonth5 / budgetDto.DepPeriodRem :
					budgetDto.DepPeriod > 0 && sumMonth5 > 0 ? sumMonth5 / budgetDto.DepPeriod : 0,
				September = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
					budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth6 :
					budgetDto.DepPeriodRem > 0 && sumMonth6 > 0 ? sumMonth6 / budgetDto.DepPeriodRem :
					budgetDto.DepPeriod > 0 && sumMonth6 > 0 ? sumMonth6 / budgetDto.DepPeriod : 0,
				Octomber = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
					budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth7 :
					budgetDto.DepPeriodRem > 0 && sumMonth7 > 0 ? sumMonth7 / budgetDto.DepPeriodRem :
					budgetDto.DepPeriod > 0 && sumMonth7 > 0 ? sumMonth7 / budgetDto.DepPeriod : 0,
				November = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
					budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth8 :
					budgetDto.DepPeriodRem > 0 && sumMonth8 > 0 ? sumMonth8 / budgetDto.DepPeriodRem :
					budgetDto.DepPeriod > 0 && sumMonth8 > 0 ? sumMonth8 / budgetDto.DepPeriod : 0,
				December = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
					budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth9 :
					budgetDto.DepPeriodRem > 0 && sumMonth9 > 0 ? sumMonth9 / budgetDto.DepPeriodRem :
					budgetDto.DepPeriod > 0 && sumMonth9 > 0 ? sumMonth9 / budgetDto.DepPeriod : 0,
				January = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
					budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth10 :
					budgetDto.DepPeriodRem > 0 && sumMonth10 > 0 ? sumMonth10 / budgetDto.DepPeriodRem :
					budgetDto.DepPeriod > 0 && sumMonth10 > 0 ? sumMonth10 / budgetDto.DepPeriod : 0,
				February = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
					budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth11 :
					budgetDto.DepPeriodRem > 0 && sumMonth11 > 0 ? sumMonth11 / budgetDto.DepPeriodRem :
					budgetDto.DepPeriod > 0 && sumMonth11 > 0 ? sumMonth11 / budgetDto.DepPeriod : 0,
				March = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
					budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.ValueMonth12 :
					budgetDto.DepPeriodRem > 0 && sumMonth12 > 0 ? sumMonth12 / budgetDto.DepPeriodRem :
					budgetDto.DepPeriod > 0 && sumMonth12 > 0 ? sumMonth12 / budgetDto.DepPeriod : 0
			};

			_context.Add(budgetForecast);


			if (month > 0)
			{
				if (month == 2)
				{
					budgetForecast.January = 0;

				}
				else if (month == 3)
				{
					budgetForecast.January = 0;
					budgetForecast.February = 0;

				}
				else if (month == 4)
				{
					budgetForecast.January = 0;
					budgetForecast.February = 0;
					budgetForecast.March = 0;

				}
				else if (month == 5)
				{
					budgetForecast.January = 0;
					budgetForecast.February = 0;
					budgetForecast.March = 0;
					budgetForecast.April = 0;

				}
				else if (month == 6)
				{
					budgetForecast.January = 0;
					budgetForecast.February = 0;
					budgetForecast.March = 0;
					budgetForecast.April = 0;
					budgetForecast.May = 0;

				}
				else if (month == 7)
				{
					budgetForecast.January = 0;
					budgetForecast.February = 0;
					budgetForecast.March = 0;
					budgetForecast.April = 0;
					budgetForecast.May = 0;
					budgetForecast.June = 0;

				}
				else if (month == 8)
				{
					budgetForecast.January = 0;
					budgetForecast.February = 0;
					budgetForecast.March = 0;
					budgetForecast.April = 0;
					budgetForecast.May = 0;
					budgetForecast.June = 0;
					budgetForecast.July = 0;

				}
				else if (month == 9)
				{
					budgetForecast.January = 0;
					budgetForecast.February = 0;
					budgetForecast.March = 0;
					budgetForecast.April = 0;
					budgetForecast.May = 0;
					budgetForecast.June = 0;
					budgetForecast.July = 0;
					budgetForecast.August = 0;

				}
				else if (month == 10)
				{
					budgetForecast.January = 0;
					budgetForecast.February = 0;
					budgetForecast.March = 0;
					budgetForecast.April = 0;
					budgetForecast.May = 0;
					budgetForecast.June = 0;
					budgetForecast.July = 0;
					budgetForecast.August = 0;
					budgetForecast.September = 0;

				}
				else if (month == 11)
				{
					budgetForecast.January = 0;
					budgetForecast.February = 0;
					budgetForecast.March = 0;
					budgetForecast.April = 0;
					budgetForecast.May = 0;
					budgetForecast.June = 0;
					budgetForecast.July = 0;
					budgetForecast.August = 0;
					budgetForecast.September = 0;
					budgetForecast.Octomber = 0;

				}
				else if (month == 12)
				{
					budgetForecast.January = 0;
					budgetForecast.February = 0;
					budgetForecast.March = 0;
					budgetForecast.April = 0;
					budgetForecast.May = 0;
					budgetForecast.June = 0;
					budgetForecast.July = 0;
					budgetForecast.August = 0;
					budgetForecast.September = 0;
					budgetForecast.Octomber = 0;
					budgetForecast.November = 0;

				}

			}

			budget.StartMonth = startAccMonth;

            entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
            _context.Update(entityType);

            _context.SaveChanges();

            return budget.Id;

        }

        public async Task<List<Model.Budget>> GetAllIncludingBudgetMonthsAsync()
        {
            return await _context.Set<Model.Budget>()
                //.Include(b => b.Badge)
                .Include(i => i.BudgetMonths)
                //    .ThenInclude(b => b.Badge)
                //.Include(r => r.Role)

                .Where(r => r.IsDeleted == false).ToListAsync();
        }

        public async Task<List<Model.Budget>> GetAllByProjectIncludingBudgetMonthsAsync()
        {


            return await _context.Set<Model.BudgetMonth>()
                .Include(b => b.Budget)
                .Where(c => c.Budget.IsDeleted == false)
                .GroupBy(c => c.Budget.ProjectId)
                .Select(a => new Model.Budget() { BudgetMonths = a.ToList() }).ToListAsync();

            //	var query =
            //				_context.Set<Model.Budget>()
            //					.SelectMany(p =>
            //						(p.BudgetMonths.Count > 0 ?? new Model.BudgetDetail[] { })
            //							.DefaultIfEmpty(new Model.BudgetDetail() { Budget = null }),
            //						(p, c) => new { p, c })
            //.GroupBy(x => x.c.Value, x => x.p);

            //var result = await _context.Set<Model.Budget>().Where(a => a.IsDeleted == false)
            //		.SelectMany(parent => parent.BudgetMonths,
            //				   (parent, child) => new { Key = child.Value, Parent = parent })
            //		.GroupBy(x => x.Key,
            //				 x => x.Parent).ToListAsync();


            //return await _context.Set<Model.Budget>()
            //    //.Include(b => b.Badge)
            //    .Include(i => i.BudgetMonths)
            //    //    .ThenInclude(b => b.Badge)
            //    //.Include(r => r.Role)

            //    .Where(r => r.IsDeleted == false).ToListAsync();
        }

    }
}
