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
using System.Data;

namespace Optima.Fais.EfRepository
{
    public class DashboardWHsRepository : Repository<Model.Dashboard>, IDashboardWHsRepository
	{
        public DashboardWHsRepository(ApplicationDbContext context)
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

        private IQueryable<Model.InventoryAsset> GetAssetMonthDetailQuery(DashboardFilter dashboardFilter, string includes)
		{
			IQueryable<Model.InventoryAsset> query = null;

			query = GetInventoryAssetsQuery2(14, includes);

			return query;
        }

        public IEnumerable<Model.InventoryAsset> GetReportData(DashboardFilter dashboardFilter, string includes, Sorting sorting, Paging paging, out DashboardTotal depTotal)
        {
            IQueryable<Model.InventoryAsset> query = null;

            query = GetAssetMonthDetailQuery(dashboardFilter, null);


			//var queryGroupBy = query
			//	.GroupBy(x => new { x.InventoryTeamManager, x.InventoryResponsable })
	  //          .Select(y => new AssetStatusWHDetail()
	  //          {
		 //           InventoryTeamManager = y.Key.InventoryTeamManager,
		 //           InventoryResponsable = y.Key.InventoryResponsable,
		 //           Assets = y.Select(a => a.Asset).ToList()
	  //          }
	  //          );



            //var queryGroupBy =
            //           from IA in _context.InventoryAssets
            //           join teamManager in _context.ApplicationUsers on IA.InventoryTeamManagerId equals teamManager.Id
            //           where IA.InventoryTeamManagerId != null && IA.InventoryId == 14
            //           group teamManager by new
            //           {
            //               //teamManager.Id,
            //               //IA.ModifiedBy,
            //               //IA.ModifiedAt,
            //               teamManager.UserName

            //           } into IAGROUP

            //           orderby IAGROUP.Key.UserName descending

            //           select new AssetStatusWHDetail
            //           {
            //               TeamManagerUserName = IAGROUP.Key.UserName,
            //           };

            depTotal = new DashboardTotal();
            //depTotal.Count = queryGroupBy.Select(a => a.Assets.Count).Count();
            //if (joinDep)
            //{
            //         depTotal.CurrentAPC = query.Sum(a => a.Dep.CurrentAPC);
            //depTotal.CurrBkValue = query.Sum(a => a.Dep.CurrBkValue);
            //depTotal.AccumulDep = query.Sum(a => a.Dep.AccumulDep);
            //depTotal.Quantity = query.Sum(a => a.Asset.Quantity);
            //depTotal.ValueRem = query.Sum(a => a.Dep.PosCap);
            //depTotal.ValueDep = query.Sum(a => a.Dep.ValueDep);
            //depTotal.ValueDepYTD = query.Sum(a => a.Dep.ValueDepYTD);
            //depTotal.ValueDepPu = query.Sum(a => a.Dep.ValueDepPU);
            //}

            if (sorting != null)
            {
				query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.InventoryAsset>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.InventoryAsset>(sorting.Column));
            }

    //        if (paging != null)
				//query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public IEnumerable<Model.CostCenter> GetCostCentersWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.CostCenterInitial).Distinct();

            return list;
        }

        public IEnumerable<Model.Division> GetDivisionsWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.CostCenterInitial.Division).Distinct();

            return list;
        }


        public IEnumerable<Model.Department> GetDepartmentsWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.CostCenterInitial.Division.Department).Where(a => a.Code !=null).Distinct();

            return list;
        }


        public IEnumerable<Model.SubType> GetSubTypesWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.Asset.SubType).Distinct();

            return list;
        }

        public IEnumerable<Model.Type> GetTypesWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.Asset.SubType.Type).Distinct();

            return list;
        }

        public IEnumerable<Model.AssetType> GetAssetTypesWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.Asset.AssetType).Distinct();

            return list;
        }

        public IEnumerable<Model.Project> GetProjectsWithAssets(DashboardFilter dashboardFilter)
        {
            var query = GetAssetMonthDetailQuery(dashboardFilter, "Adm");
            var list = query.Select(a => a.Asset.Project).Distinct();

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

		public IQueryable<Model.InventoryAsset> GetInventoryAssetsQuery2(int inventoryId, string includes)
		{
			IQueryable<Model.InventoryAsset> query = null;
			query = _context.InventoryAssets.AsNoTracking();

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
                    .Include(i => i.InventoryTeamManager)
					.Include(i => i.InventoryResponsable)
					.Include(i => i.Asset);
					//	.ThenInclude(i => i.AssetCategory)
					// .Include(i => i.Asset)
					//	.ThenInclude(i => i.Project)
					// .Include(i => i.Asset)
					//	.ThenInclude(i => i.InsuranceCategory)
					// .Include(i => i.Asset)
					//	.ThenInclude(i => i.InterCompany)
					// .Include(i => i.Asset)
					//	.ThenInclude(i => i.Material)
					// .Include(i => i.Asset)
					//	.ThenInclude(i => i.AssetNature)
					//.Include(i => i.Asset)
					//	.ThenInclude(i => i.Account)
					// .Include(i => i.Asset)
					//	.ThenInclude(i => i.ExpAccount)
					//  .Include(i => i.Asset)
					//	.ThenInclude(i => i.BudgetManager)
					// .Include(i => i.Asset)
					//	.ThenInclude(i => i.Division)
					// .Include(i => i.Asset)
					//	.ThenInclude(i => i.Department)
					//  .Include(i => i.Asset)
					//	.ThenInclude(i => i.Article)
					//.Include(i => i.Asset)
					// .ThenInclude(i => i.SubType)
					//		.ThenInclude(i => i.Type)
					// .Include(i => i.Asset)
					//	.ThenInclude(i => i.AssetType)
					//.Include(i => i.Asset)
					//   .ThenInclude(i => i.Document)
					//		.ThenInclude(i => i.Partner)
					//.Include(i => i.RoomInitial)
					//	.ThenInclude(r => r.Location)
					//		.ThenInclude(l => l.City)
					//			.ThenInclude(l => l.County)
					//				.ThenInclude(l => l.Country)
					//.Include(i => i.EmployeeInitial)
					//.Include(i => i.AdministrationFinal)
					//	.ThenInclude(i => i.Division)
					//.Include(i => i.RoomFinal)
					//	.ThenInclude(r => r.Location)
					//		.ThenInclude(l => l.City)
					//			.ThenInclude(l => l.County)
					//				 .ThenInclude(l => l.Country)
					//.Include(i => i.EmployeeFinal)
					//.Include(i => i.StateInitial)
					//.Include(i => i.CostCenterFinal)
					//		.ThenInclude(c => c.AdmCenter)
					// .Include(i => i.CostCenterFinal)
					//		.ThenInclude(c => c.Region)
					//  .Include(i => i.CostCenterInitial)
					//		.ThenInclude(c => c.AdmCenter)
					// .Include(i => i.CostCenterInitial)
					//		.ThenInclude(c => c.Region)
					//.Include(i => i.StateFinal);
			}

			query = query.Where(i => i.InventoryId == inventoryId && i.Asset.IsDeleted == false && i.Asset.Validated == true && i.IsTemp == false && i.InventoryTeamManagerId != null);

			return query;
		}

	}
}
