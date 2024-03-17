using System.Collections.Generic;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using System.Linq;
using Optima.Fais.Model.Utils;
using Optima.Fais.Dto;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing;

namespace Optima.Fais.EfRepository
{
    public class DashboardsRepository : Repository<Model.Dashboard>, IDashboardsRepository
	{
        public DashboardsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Name.Contains(filter) || a.Code.Contains(filter)); })
        { }

        public IEnumerable<Model.Dashboard> GetData(DashboardFilter dashboardFilter, string includes, Sorting sorting, Paging paging, out DashboardTotal depTotal)
        {
            IQueryable<Model.InventoryAsset> assetQuery = null;
            IQueryable<Model.Dashboard> query = null;

            assetQuery = _context.InventoryAssets
                .Include(a => a.Asset)
                    .ThenInclude(d => d.AssetType)
                 .Include(a => a.Asset)
                    .ThenInclude(d => d.Project)
                 .Include(a => a.Asset)
                    .ThenInclude(d => d.SubType)
                       .ThenInclude(d => d.Type)
                .Include(a => a.Asset)
                    .ThenInclude(d => d.CostCenter)
                       .ThenInclude(d => d.Division)
                            .ThenInclude(d => d.Department)
                .Include(c => c.CostCenterInitial)
                    .ThenInclude(d =>d.Division)
                       .ThenInclude(d => d.Department)
                .Include(c => c.CostCenterFinal)
                        .ThenInclude(d => d.Division)
                            .ThenInclude(d => d.Department)
                            .Where(i => i.InventoryId == 14).AsQueryable();

            if (dashboardFilter.Filter != "" && dashboardFilter.Filter != null) assetQuery = assetQuery.Where(a => (a.Asset.InvNo.Contains(dashboardFilter.Filter) || a.Asset.ERPCode.Contains(dashboardFilter.Filter) || a.Asset.Name.Contains(dashboardFilter.Filter) || a.Asset.SerialNumber.Contains(dashboardFilter.Filter)));

            query = assetQuery.Select(inv => new Model.Dashboard { InventoryAsset = inv });


            if (dashboardFilter.Role != null && dashboardFilter.Role != "")
            {
                if (dashboardFilter.Role.ToUpper() == "USER")
                {

                    List<int?> costCenterIds = _context.Set<Model.EmployeeCostCenter>().AsNoTracking().Where(e => e.EmployeeId == dashboardFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.CostCenterId).ToList();

                    if (costCenterIds.Count == 0)
                    {
                        costCenterIds = new List<int?>();
                        costCenterIds.Add(10000000);
                    }

                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => ((a.InventoryAsset.CostCenterIdInitial == id)); }, costCenterIds));

                }
            }


            if ((dashboardFilter.CostCenterIds != null) && (dashboardFilter.CostCenterIds.Count > 0))
            {
                if (dashboardFilter.ReportType == 1)
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.CostCenterIdInitial == id; }, dashboardFilter.CostCenterIds));
                }
                else
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.CostCenterIdFinal == id; }, dashboardFilter.CostCenterIds));
                }

			}
			else
			{
                if ((dashboardFilter.DivisionIds != null) && (dashboardFilter.DivisionIds.Count > 0))
                {
                    if (dashboardFilter.ReportType == 1)
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.CostCenterInitial.DivisionId == id; }, dashboardFilter.DivisionIds));
                    }
                    else
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.CostCenterFinal.DivisionId == id; }, dashboardFilter.DivisionIds));
                    }

				}
				else
				{
                    if ((dashboardFilter.DepartmentIds != null) && (dashboardFilter.DepartmentIds.Count > 0))
                    {
                        if (dashboardFilter.ReportType == 1)
                        {
                            query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.CostCenterInitial.Division.DepartmentId == id; }, dashboardFilter.DepartmentIds));
                        }
                        else
                        {
                            query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.CostCenterFinal.Division.DepartmentId == id; }, dashboardFilter.DepartmentIds));
                        }

                    }
                }
            }


            if ((dashboardFilter.TypeIds != null) && (dashboardFilter.TypeIds.Count > 0))
            {
                if (dashboardFilter.ReportType == 1)
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.SubType.TypeId == id; }, dashboardFilter.TypeIds));
                }
                else
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.SubType.TypeId == id && a.InventoryAsset.CostCenterIdFinal != null; }, dashboardFilter.TypeIds));
                }

            }

            if ((dashboardFilter.AssetTypeIds != null) && (dashboardFilter.AssetTypeIds.Count > 0))
            {
                if (dashboardFilter.ReportType == 1)
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.AssetTypeId == id; }, dashboardFilter.AssetTypeIds));
                }
                else
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.AssetTypeId == id && a.InventoryAsset.CostCenterIdFinal != null; }, dashboardFilter.AssetTypeIds));
                }

            }

            if ((dashboardFilter.ProjectIds != null) && (dashboardFilter.ProjectIds.Count > 0))
            {
                if (dashboardFilter.ReportType == 1)
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.ProjectId == id; }, dashboardFilter.ProjectIds));
                }
                else
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.ProjectId == id && a.InventoryAsset.CostCenterIdFinal != null; }, dashboardFilter.ProjectIds));
                }

            }


            query = query.Where(a => a.InventoryAsset.Asset.IsDeleted == false && a.InventoryAsset.Asset.Validated == true);


            depTotal = new DashboardTotal();
            depTotal.Count = query.Count();
            

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.Dashboard>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.Dashboard>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);
            var list = query.ToList();

            return list;
        }

        public IEnumerable<Model.Dashboard> GetAssetStatusData(DashboardFilter dashboardFilter, string includes, Sorting sorting, Paging paging, out DashboardTotal depTotal)
        {
            IQueryable<Model.InventoryAsset> assetQuery = null;
            IQueryable<Model.Dashboard> query = null;

            assetQuery = _context.InventoryAssets
                .Include(a => a.Asset)
                    .ThenInclude(d => d.AssetType)
                 .Include(a => a.Asset)
                    .ThenInclude(d => d.BudgetManager)
                 .Include(a => a.Asset)
                    .ThenInclude(d => d.Project)
                 .Include(a => a.Asset)
                    .ThenInclude(d => d.SubType)
                       .ThenInclude(d => d.Type)
                .Include(a => a.Asset)
                    .ThenInclude(d => d.CostCenter)
                       .ThenInclude(d => d.Division)
                            .ThenInclude(d => d.Department)
                .Include(c => c.CostCenterInitial)
                    .ThenInclude(d => d.Division)
                       .ThenInclude(d => d.Department)
                .Include(c => c.CostCenterFinal)
                        .ThenInclude(d => d.Division)
                            .ThenInclude(d => d.Department)
                            .Where(i => i.InventoryId == 14).AsQueryable();

            if (dashboardFilter.Filter != "" && dashboardFilter.Filter != null) assetQuery = assetQuery.Where(a => (a.Asset.InvNo.Contains(dashboardFilter.Filter) || a.Asset.ERPCode.Contains(dashboardFilter.Filter) || a.Asset.Name.Contains(dashboardFilter.Filter) || a.Asset.SerialNumber.Contains(dashboardFilter.Filter)));

            query = assetQuery.Select(inv => new Model.Dashboard { InventoryAsset = inv });


            if (dashboardFilter.Role != null && dashboardFilter.Role != "")
            {
                if (dashboardFilter.Role.ToUpper() == "USER")
                {

                    List<int?> costCenterIds = _context.Set<Model.EmployeeCostCenter>().AsNoTracking().Where(e => e.EmployeeId == dashboardFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.CostCenterId).ToList();

                    if (costCenterIds.Count == 0)
                    {
                        costCenterIds = new List<int?>();
                        costCenterIds.Add(10000000);
                    }

                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => ((a.InventoryAsset.CostCenterIdInitial == id)); }, costCenterIds));

                }
            }


            if ((dashboardFilter.CostCenterIds != null) && (dashboardFilter.CostCenterIds.Count > 0))
            {
                if (dashboardFilter.ReportType == 1)
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.CostCenterIdInitial == id; }, dashboardFilter.CostCenterIds));
                }
                else
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.CostCenterIdFinal == id; }, dashboardFilter.CostCenterIds));
                }

            }
            else
            {
                if ((dashboardFilter.DivisionIds != null) && (dashboardFilter.DivisionIds.Count > 0))
                {
                    if (dashboardFilter.ReportType == 1)
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.CostCenterInitial.DivisionId == id; }, dashboardFilter.DivisionIds));
                    }
                    else
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.CostCenterFinal.DivisionId == id; }, dashboardFilter.DivisionIds));
                    }

                }
                else
                {
                    if ((dashboardFilter.DepartmentIds != null) && (dashboardFilter.DepartmentIds.Count > 0))
                    {
                        if (dashboardFilter.ReportType == 1)
                        {
                            query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.CostCenterInitial.Division.DepartmentId == id; }, dashboardFilter.DepartmentIds));
                        }
                        else
                        {
                            query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.CostCenterFinal.Division.DepartmentId == id; }, dashboardFilter.DepartmentIds));
                        }

                    }
                }
            }


            if ((dashboardFilter.TypeIds != null) && (dashboardFilter.TypeIds.Count > 0))
            {
                if (dashboardFilter.ReportType == 1)
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.SubType.TypeId == id; }, dashboardFilter.TypeIds));
                }
                else
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.SubType.TypeId == id && a.InventoryAsset.CostCenterIdFinal != null; }, dashboardFilter.TypeIds));
                }

            }

            if ((dashboardFilter.AssetTypeIds != null) && (dashboardFilter.AssetTypeIds.Count > 0))
            {
                if (dashboardFilter.ReportType == 1)
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.AssetTypeId == id; }, dashboardFilter.AssetTypeIds));
                }
                else
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.AssetTypeId == id && a.InventoryAsset.CostCenterIdFinal != null; }, dashboardFilter.AssetTypeIds));
                }

            }

            if ((dashboardFilter.ProjectIds != null) && (dashboardFilter.ProjectIds.Count > 0))
            {
                if (dashboardFilter.ReportType == 1)
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.ProjectId == id; }, dashboardFilter.ProjectIds));
                }
                else
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.ProjectId == id && a.InventoryAsset.CostCenterIdFinal != null; }, dashboardFilter.ProjectIds));
                }

            }

            if ((dashboardFilter.BudgetManagerIds != null) && (dashboardFilter.BudgetManagerIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.Dashboard, int?>((id) => { return a => a.InventoryAsset.Asset.BudgetManagerId == id; }, dashboardFilter.BudgetManagerIds));
            }


            query = query.Where(a => a.InventoryAsset.Asset.IsDeleted == false && a.InventoryAsset.Asset.Validated == true);


            depTotal = new DashboardTotal();
            depTotal.Count = query.Count();


            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.Dashboard>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.Dashboard>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);
            var list = query.ToList();

            return list;
        }

        private IQueryable<AssetMonthDetail> GetAssetMonthDetailQuery(DashboardFilter dashboardFilter, string includes)
		{
            IQueryable<Model.Asset> assetQuery = null;
            IQueryable<AssetDepMD> depQuery = null;
            IQueryable<AssetAdmMD> admQuery = null;
            IQueryable<AssetAC> assetClassQuery = null;
            IQueryable<AssetMonthDetail> query = null;

            assetQuery = _context.Assets.AsQueryable();
            int accMonthId = _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Active == true && a.IsDeleted == false).Select(a => a.AccMonthId.Value).FirstOrDefault();

            depQuery = _context.AssetDepMDs.AsQueryable().Where(a => a.AccSystemId == 3 && a.AccMonthId == accMonthId);
            admQuery = _context.AssetAdmMDs.AsQueryable().Where(a => a.AccMonthId == accMonthId);
            assetClassQuery = _context.AssetACs.AsQueryable();

            //if (assetFilter.Custody != null) assetQuery = assetQuery.Where(a => a.Custody == assetFilter.Custody);
            if (dashboardFilter.Filter != "" && dashboardFilter.Filter != null) assetQuery = assetQuery.Where(a => (a.InvNo.Contains(dashboardFilter.Filter) || a.ERPCode.Contains(dashboardFilter.Filter) || a.Name.Contains(dashboardFilter.Filter) || a.SerialNumber.Contains(dashboardFilter.Filter)));
            //if (assetFilter.FilterDoc != "" && assetFilter.FilterDoc != null) assetQuery = assetQuery.Where(a => a.Document.DocNo1.Contains(assetFilter.FilterDoc));
            //if (assetFilter.IsPrinted != null) assetQuery = assetQuery.Where(a => a.IsPrinted == assetFilter.IsPrinted);
            //if (assetFilter.IsDuplicate != null) assetQuery = assetQuery.Where(a => a.IsDuplicate == assetFilter.IsDuplicate);

            bool joinAdm = false;
            bool joinDep = false;
            bool joinAssetClass = false;

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

                //prefix = dotIndex > 0 ? property.Substring(0, dotIndex).ToLower() : string.Empty;

                switch (prefix)
                {
                    case "Asset":
                        if (property.Length > 0) assetQuery = assetQuery.Include(property);
                        break;
                    case "Dep":
                        if (property.Length > 0) depQuery = depQuery.Include(property);
                        joinDep = true;
                        break;
                    case "Adm":
                        if (property.Length > 0) admQuery = admQuery.Include(property);
                        joinAdm = true;
                        break;
                    case "AssetClass":
                        if (property.Length > 0) assetClassQuery = assetClassQuery.Include(property);
                        joinAssetClass = true;
                        break;
                    default:
                        break;
                }
            }

            query = assetQuery.Select(asset => new AssetMonthDetail { Asset = asset });

            if (joinAdm)
            {
                query = query
                    .Join(admQuery, q => q.Asset.Id, adm => adm.AssetId, (q, adm) => new AssetMonthDetail { Asset = q.Asset, Adm = adm });
            }

            if (joinDep)
            {
                query = query
                    .Join(depQuery, q => q.Asset.Id, dep => dep.AssetId, (q, dep) => new AssetMonthDetail { Asset = q.Asset, Adm = q.Adm, Dep = dep });
            }

            if (dashboardFilter.Role != null && dashboardFilter.Role != "")
            {
                if (dashboardFilter.Role.ToUpper() == "USER")
                {

                    List<int?> costCenterIds = _context.Set<Model.EmployeeCostCenter>().AsNoTracking().Where(e => e.EmployeeId == dashboardFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.CostCenterId).ToList();

                    if (costCenterIds.Count == 0)
                    {
                        costCenterIds = new List<int?>();
                        costCenterIds.Add(10000000);
                    }


                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => ((a.Adm.CostCenterId == id)); }, costCenterIds));


                    if ((dashboardFilter.CostCenterIds != null) && (dashboardFilter.CostCenterIds.Count > 0))
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => ((a.Adm.CostCenterId == id)); }, dashboardFilter.CostCenterIds));
                    }

                }
            }

            if ((dashboardFilter.CostCenterIds != null) && (dashboardFilter.CostCenterIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.CostCenterId == id; }, dashboardFilter.CostCenterIds));

            }
            else
            {
                if ((dashboardFilter.DivisionIds != null) && (dashboardFilter.DivisionIds.Count > 0))
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.CostCenter.DivisionId == id; }, dashboardFilter.DivisionIds));

                }
                else
                {
                    if ((dashboardFilter.DepartmentIds != null) && (dashboardFilter.DepartmentIds.Count > 0))
                    {
                        query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.CostCenter.Division.DepartmentId == id; }, dashboardFilter.DepartmentIds));

                    }
                }
            }

            if ((dashboardFilter.TypeIds != null) && (dashboardFilter.TypeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.SubType.TypeId == id; }, dashboardFilter.TypeIds));
            }

            if ((dashboardFilter.AssetTypeIds != null) && (dashboardFilter.AssetTypeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.AssetTypeId == id; }, dashboardFilter.AssetTypeIds));
            }

            if ((dashboardFilter.ProjectIds != null) && (dashboardFilter.ProjectIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => a.Adm.ProjectId == id; }, dashboardFilter.ProjectIds));
            }


            if ((dashboardFilter.BudgetManagerIds != null) && (dashboardFilter.BudgetManagerIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.AssetMonthDetail, int?>((id) => { return a => ((a.Adm.BudgetManagerId == id)); }, dashboardFilter.BudgetManagerIds));
            }


            query = query.Where(a => a.Asset.IsDeleted == false && a.Asset.Validated == true && a.Asset.IsTemp == false);

			return query;
        }

        public IEnumerable<AssetMonthDetail> GetReportData(DashboardFilter dashboardFilter, string includes, Sorting sorting, Paging paging, out DashboardTotal depTotal)
        {
            IQueryable<AssetMonthDetail> query = null;

            query = GetAssetMonthDetailQuery(dashboardFilter, includes);

            depTotal = new DashboardTotal();
            depTotal.Count = query.Count();
            //if (joinDep)
            //{
                depTotal.CurrentAPC = query.Sum(a => a.Dep.CurrentAPC);
			    depTotal.CurrBkValue = query.Sum(a => a.Dep.CurrBkValue);
			    depTotal.AccumulDep = query.Sum(a => a.Dep.AccumulDep);
			    depTotal.Quantity = query.Sum(a => a.Asset.Quantity);
			//depTotal.ValueRem = query.Sum(a => a.Dep.PosCap);
			//depTotal.ValueDep = query.Sum(a => a.Dep.ValueDep);
			//depTotal.ValueDepYTD = query.Sum(a => a.Dep.ValueDepYTD);
			//depTotal.ValueDepPu = query.Sum(a => a.Dep.ValueDepPU);
			//}

			if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<AssetMonthDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<AssetMonthDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);
            var list = query.ToList();

            return list;
        }

        public IEnumerable<Model.CostCenter> GetCostCentersWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.Adm.CostCenter).Distinct();

            return list;
        }

        public IEnumerable<Model.Division> GetDivisionsWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.Adm.CostCenter.Division).Distinct();

            return list;
        }


        public IEnumerable<Model.Department> GetDepartmentsWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.Adm.CostCenter.Division.Department).Where(a => a.Code !=null).Distinct();

            return list;
        }


        public IEnumerable<Model.SubType> GetSubTypesWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.Adm.SubType).Distinct();

            return list;
        }

        public IEnumerable<Model.Type> GetTypesWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.Adm.SubType.Type).Distinct();

            return list;
        }

        public IEnumerable<Model.AssetType> GetAssetTypesWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.Adm.AssetType).Distinct();

            return list;
        }

        public IEnumerable<Model.Project> GetProjectsWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.Adm.Project).Distinct();

            return list;
        }

		public IEnumerable<RequestKanban> GetRequests(DashboardFilter dashboardFilter)
		{
			IQueryable<Model.Request> requests = null;
			Color color1 = (Color)new ColorConverter().ConvertFromString("#28a745");
			string hexcolor1 = "#" + color1.Name.ToString().Substring(2);
			Color color2 = (Color)new ColorConverter().ConvertFromString("#208eed");
			string hexcolor2 = "#" + color2.Name.ToString().Substring(2);
			Color color3 = (Color)new ColorConverter().ConvertFromString("#343a40");
			string hexcolor3 = "#" + color3.Name.ToString().Substring(2);
			Color color4 = (Color)new ColorConverter().ConvertFromString("#ffc107");
			string hexcolor4 = "#" + color4.Name.ToString().Substring(2);
			Color color5 = (Color)new ColorConverter().ConvertFromString("#6c757d");
			string hexcolor5 = "#" + color5.Name.ToString().Substring(2);
			Color color6 = (Color)new ColorConverter().ConvertFromString("#FFFFFF");
			string hexcolor6 = "#" + color6.Name.ToString().Substring(2);

			requests = _context.Requests.Include(a => a.AppState).AsQueryable();

			requests = requests.Where(a => a.IsDeleted == false && a.Validated == true);

            var list = requests.Where(com => com.IsDeleted == false && com.Validated == true).AsEnumerable()
							.GroupBy(item => item.AppState.Name,
								(key, group) => new RequestKanban()
								{
									Status = key,
									Color = key == "Nou" || key == "Accepted" ? hexcolor1 :
									        key == "Alocat" ? hexcolor2 :
									        key == "Utilizate" ? hexcolor3 : 
									        key == "Lipsa buget" ? hexcolor4 : 
									        key == "Lipsa dovada document" ? hexcolor5 : hexcolor6,
									List = group.ToList().Select(a => new Card
									{
										Id = a.Id,
										Text = a.Code,
										Like = a.Info,
										Comments = new List<Comment>()
						                    {
							                    new Comment()
							                    {
								                    Id = a.Id,
								                    Text = a.Code
							                    }
						                    }
									}).ToList()
								})
							.ToList();
			return list;
		}

		public IEnumerable<RequestListKanban> GetListRequests(DashboardFilter dashboardFilter)
		{
			IQueryable<Model.Request> requests = null;
			

			requests = _context.Requests.Include(a => a.AppState).Include(a => a.Employee).AsQueryable();

			var list = requests.Where(a => a.IsDeleted == false && a.Validated == true).Select(a => new RequestListKanban()
            {
                Id = a.Id,
                Title = a.Info,
                Description = a.Code,
                Type = "Task",
                Status = "ACCEPTED",
                Priority = "Highest",
                ListPosition = a.Id,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.ModifiedAt,
                Reporter = new List<Reporter>()
                {
                    new Reporter()
                    {
                        Id = a.Employee.Id,
                        Name = a.Employee.FirstName + " " + a.Employee.LastName,
                        Email = a.Employee.Email,
                        AvatarUrl = "https://res.cloudinary.com/comparte/image/upload/c_scale,w_48/v1620181672/richard-hendricks.png"
					}
                },
				Assignees = new List<Reporter>()
				{
					new Reporter()
					{
						Id = a.Employee.Id,
						Name = a.Employee.FirstName + " " + a.Employee.LastName,
						Email = a.Employee.Email,
						AvatarUrl = "https://res.cloudinary.com/comparte/image/upload/c_scale,w_48/v1620181672/richard-hendricks.png"
					}
				},
				ProjectId = a.ProjectId
			}).ToList();

			return list;
		}

	}
}
