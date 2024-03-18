using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class InvStatesRepository : Repository<InvState>, IInvStatesRepository
    {
        public InvStatesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (c) => (c.Code.Contains(filter) || c.Name.Contains(filter)); })
        { }

        public IEnumerable<Model.InvState> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.InvStates.AsNoTracking();

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

        public IEnumerable<Model.InvState> GetInvStatesInUseWithAssets(AssetFilter assetFilter, List<PropertyFilter> propFilters)
        {
            var query = GetAssetMonthDetailQuery(assetFilter, propFilters);
            var list = query.Select(a => a.Asset.InvState).Distinct();

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
