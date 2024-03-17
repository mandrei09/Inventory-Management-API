using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class EmailManagersRepository : Repository<EmailManager>, IEmailManagersRepository
    {
        public EmailManagersRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Offer.Code.Contains(filter) || a.Offer.Name.Contains(filter) || a.Offer.Request.Code.Contains(filter)); })
        { }

        private Expression<Func<Model.EmailManager, bool>> GetFiltersPredicate(string employeeId, string role, bool inInventory, string filter, List<int?> emailTypeIds, List<int?> appStateIds, List<int?> assetCategoryIds, List<int> divisionIds, string type)
        {
            Expression<Func<Model.EmailManager, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

			if ((type != null) && (type != ""))
			{
                if (type == "pending")
                {
					predicate = predicate != null
					? ExpressionHelper.And<Model.EmailManager>(predicate, r => r.AppStateId == 6)
					: r => r.AppStateId == 6;
				}
				else if (type == "accepted")
				{
					predicate = predicate != null
					? ExpressionHelper.And<Model.EmailManager>(predicate, r => r.AppStateId == 7)
					: r => r.AppStateId == 7;
				}
				else if (type == "declined")
				{
					predicate = predicate != null
					? ExpressionHelper.And<Model.EmailManager>(predicate, r => r.AppStateId ==8)
					: r => r.AppStateId == 8;
				}
				
			}

			if ((emailTypeIds != null) && (emailTypeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmailManager>(predicate, r => emailTypeIds.Contains(r.EmailTypeId))
                    : r => emailTypeIds.Contains(r.EmailTypeId);
            }

            if ((appStateIds != null) && (appStateIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmailManager>(predicate, r => appStateIds.Contains(r.AppStateId))
                    : r => appStateIds.Contains(r.AppStateId);
            }

            if (role != null && role != "")
            {
                if (role.ToUpper() == "ADMINISTRATOR")
                {
                    //if ((orderFilter.CostCenterIds != null) && (orderFilter.CostCenterIds.Count > 0))
                    //{
                    //    query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => { return a => ((a.Adm.CostCenterId == id)); }, orderFilter.CostCenterIds));
                    //}

                    //if ((orderFilter.EmployeeIds != null) && (orderFilter.EmployeeIds.Count > 0))
                    //{
                    //    query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => { return a => ((a.Adm.EmployeeId == id)); }, orderFilter.EmployeeIds));
                    //}
                }
                else if (role.ToUpper() == "PROCUREMENT")
                {
                    List<int?> divIds = new List<int?>();
                    divIds.Add(1482);

                    predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmailManager>(predicate, r => !divIds.Contains(r.Offer.DivisionId) && r.Offer.AssetType.Code != "STOCK_IT")
                    : r => !divIds.Contains(r.Offer.DivisionId) && r.Offer.AssetType.Code != "STOCK_IT";
                }
                else if (role.ToUpper() == "PROC-IT")
                {
                    List<int?> divIds = _context.Set<Model.EmployeeDivision>().AsNoTracking().Where(e => e.EmployeeId == int.Parse(employeeId) && e.IsDeleted == false).Select(c => (int?)c.DivisionId).ToList();

                    if (divIds.Count == 0)
                    {
                        divIds = new List<int?>();
                        divIds.Add(-1);
                    }

                    predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmailManager>(predicate, r => divIds.Contains(r.Offer.DivisionId) && r.Offer.AssetType.Code != "STOCK_IT")
                    : r => divIds.Contains(r.Offer.DivisionId) && r.Offer.AssetType.Code != "STOCK_IT";
                }
                else
                {
                    if (role.ToUpper() != "ADMINISTRATOR")
                    {

                        if (role.ToUpper() == "APPROVERS")
                        {
                            List<int?> employeeIds = new List<int?>();
                            employeeIds.Add(int.Parse(employeeId));


                            //query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => {
                            //    return a => (
                            //(
                            //(a.Order.EmployeeL4Id == id && a.Order.AppState.Code == "ORDER_LEVEL4") ||
                            //(a.Order.EmployeeL3Id == id && a.Order.AppState.Code == "ORDER_LEVEL3") ||
                            //(a.Order.EmployeeL2Id == id && a.Order.AppState.Code == "ORDER_LEVEL2") ||
                            //(a.Order.EmployeeL1Id == id && a.Order.AppState.Code == "ORDER_LEVEL1") ||
                            //(a.Order.EmployeeS1Id == id && a.Order.AppState.Code == "ORDER_LEVELS1") ||
                            //(a.Order.EmployeeS2Id == id && a.Order.AppState.Code == "ORDER_LEVELS2") ||
                            //(a.Order.EmployeeS3Id == id && a.Order.AppState.Code == "ORDER_LEVELS3")) || a.Order.AppState.Code == "NEED_CONTRACT");
                            //}, employeeIds));
                        }
                        else
                        {
                            List<int?> divIds = _context.Set<Model.EmployeeDivision>().AsNoTracking().Where(e => e.EmployeeId == int.Parse(employeeId) && e.IsDeleted == false).Select(c => (int?)c.DivisionId).ToList();

                            if (divIds.Count == 0)
                            {
                                divIds = new List<int?>();
                                divIds.Add(-1);
                            }

                            predicate = predicate != null
                            ? ExpressionHelper.And<Model.EmailManager>(predicate, r => divIds.Contains(r.Offer.DivisionId) && r.Offer.AssetType.Code != "STOCK_IT")
                            : r => divIds.Contains(r.Offer.DivisionId) && r.Offer.AssetType.Code != "STOCK_IT";
                        }



                        //if ((orderFilter.CostCenterIds != null) && (orderFilter.CostCenterIds.Count > 0))
                        //{
                        //    query = query.Where(ExpressionHelper.GetInListPredicate<Model.OrderDetail, int?>((id) => { return a => ((a.Adm.CostCenterId == id)); }, orderFilter.CostCenterIds));
                        //}

                    }
                }
            }

            //if ((divisionIds != null) && (divisionIds.Count > 0))
            //{
            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Model.EmailManager>(predicate, r => divisionIds.Contains(r.co))
            //        : r => divisionIds.Contains(r.DivisionId);
            //}


            return predicate;
        }

        public IEnumerable<Model.EmailManager> GetByFilters(string employeeId, string role, bool inInventory, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> emailTypeIds, List<int?> appStateIds, List<int?> assetCategoryIds, List<int> divisionIds, string type)
        {
            var predicate = GetFiltersPredicate(employeeId, role, inInventory, filter, emailTypeIds, appStateIds, assetCategoryIds, divisionIds, type);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string employeeId, string role, bool inInventory, string filter, List<int?> emailTypeIds, List<int?> appStateIds, List<int?> assetCategoryIds, List<int> divisionIds, string type)
        {
            var predicate = GetFiltersPredicate(employeeId, role, inInventory, filter, emailTypeIds, appStateIds, assetCategoryIds, divisionIds, type);

            return GetQueryable(predicate).Count();
        }
    }
}
