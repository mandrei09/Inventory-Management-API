using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Optima.Fais.EfRepository
{
    public class AssetNiRepository : Repository<Model.AssetNi>, IAssetNiRepository
    {
        public AssetNiRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (i) => (i.Code1.Contains(filter) || i.Code2.Contains(filter) || i.Name1.Contains(filter) || i.Name2.Contains(filter)); })
        {
        }

        public IQueryable<Dto.AssetNiInvDet> GetAssetNiInvDetQuery(int inventoryId)
        {
            IQueryable<Dto.AssetNiInvDet> query = null;
            IQueryable<Model.AssetNi> assetsNi = _context.Set<Model.AssetNi>().AsQueryable();

            IQueryable<Model.CostCenter> costCenters = _context.Set<Model.CostCenter>().AsQueryable();
            IQueryable<Model.Employee> employees = _context.Set<Model.Employee>().AsQueryable();
            IQueryable<Model.AdmCenter> admCenters = _context.Set<Model.AdmCenter>().AsQueryable();
            IQueryable<Model.Location> locations = _context.Set<Model.Location>().AsQueryable();
            IQueryable<Model.Region> regions = _context.Set<Model.Region>().AsQueryable();
            IQueryable<Model.Room> rooms = _context.Set<Model.Room>().AsQueryable();
          

            query =
                from assetNi in assetsNi
                //join uom in uoms on asset.UomId equals uom.Id

                join employee in employees on assetNi.EmployeeId equals employee.Id
                join costCenter in costCenters on assetNi.CostCenterId equals costCenter.Id into costCentersAll
                from costCenter in costCentersAll.DefaultIfEmpty()
                join room in rooms on assetNi.RoomId equals room.Id
                join location in locations on room.LocationId equals location.Id

                join reg in regions on location.RegionId equals reg.Id into regionsAll
               
                from region in regionsAll.DefaultIfEmpty()

                    //join e in employees on assetNi.EmployeeId equals e.Id into employeesAll
                    //from employee in employeesAll.DefaultIfEmpty()

                where assetNi.InventoryId == inventoryId

                select new Dto.AssetNiInvDet()
                {
                    Id = assetNi.Id,
                    Code1 = assetNi.Code1,
                    Code2 = assetNi.Code2,
                    Name1 = assetNi.Name1,
                    Name2 = assetNi.Name2,
                    IsDeleted=assetNi.IsDeleted,
                   
                    SerialNumber = assetNi.SerialNumber,
                    Uom = "BUC",
                    Quantity = assetNi.Quantity,

                    EmployeeId = employee.Id,
                    InternalCode = employee.InternalCode,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    LocationId = location.Id,
                    LocationCode = location.Code,
                    LocationName = location.Name,
                    RoomId = room.Id,
                    RoomCode = room.Code,
                    RoomName = room.Name,

                    RegionId = region != null ? (int?)region.Id : null,
                    RegionCode = region != null ? region.Code : string.Empty,
                    RegionName = region != null ? region.Name : string.Empty,

                    CostCenterId = costCenter != null ? costCenter.Id : 0,
                    CostCenterCode = costCenter != null ? costCenter.Code : string.Empty,
                    CostCenterName = costCenter != null ? costCenter.Name : string.Empty,

                    InvStateId = assetNi.InvStateId,
                    Custody = false
                };
            Console.WriteLine("AssetNi: " + query.Count());
            return query;
        }

        public IEnumerable<Dto.AssetNiInvDet> GetAssetNiInvDetByFilters(int inventoryId, string filter, string reportType, bool? custody,
            List<int> assetCategoryIds, List<int> assetTypeIds, List<int> partnerIds,
            List<int> costCenterIds, List<int> admCenterIds, List<int> departmentIds, List<int> employeeIds, List<int> locationIds, List<int> roomIds,
            string sortColumn, string sortDirection, int? page, int? pageSize, out int count)
        {
            IQueryable<Dto.AssetNiInvDet> query = null;

            query = GetAssetNiInvDetQuery(inventoryId);

            if (custody.HasValue) query = query.Where(a => a.Custody == custody.Value);

            if ((costCenterIds != null) && (costCenterIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Dto.AssetNiInvDet, int>((id) => { return i => i.CostCenterId == id; }, costCenterIds));
            }
            else
            {
                if ((admCenterIds != null) && (admCenterIds.Count > 0))
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Dto.AssetNiInvDet, int>((id) => { return i => i.AdmCenterId == id; }, admCenterIds));
                }
            }

            if ((employeeIds != null) && (employeeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Dto.AssetNiInvDet, int>((id) => { return i => i.EmployeeId == id; }, employeeIds));
            }

            if ((roomIds != null) && (roomIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Dto.AssetNiInvDet, int>((id) => { return i => i.RoomId == id; }, roomIds));
            }
            else
            {
                if ((locationIds != null) && (locationIds.Count > 0))
                {
                    query = query.Where(ExpressionHelper.GetInListPredicate<Dto.AssetNiInvDet, int>((id) => { return i => i.LocationId == id; }, locationIds));
                }
            }

            if (filter != null) query = query.Where(a => (a.Code1.Contains(filter) || a.Code2.Contains(filter) || a.Name1.Contains(filter) || a.Name2.Contains(filter)));

            count = query.Count();

            if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Dto.AssetNiInvDet>(sortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Dto.AssetNiInvDet>(sortColumn));
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query.ToList();
        }

        public IEnumerable<Dto.AssetNi> GetAssetNiByFilters(
            //string filter,
            //int? wordCount, int? letterCount, string searchType, string conditionType, 
            List<string> filters, string conditionType,
            int? inventoryId, int? assetId, 
            List<int> departmentIds, List<int> employeeIds, List<int> locationIds, List<int> roomIds, List<int> regionIds, List<int> admCenterIds,
            string sortColumn, string sortDirection, int? page, int? pageSize, out int count)
        {
            IQueryable<Dto.AssetNi> query = null;
            IQueryable<Model.AssetNi> assetNis = _context.Set<Model.AssetNi>().AsQueryable();
            IQueryable<Model.InvState> invStates = _context.Set<Model.InvState>().AsQueryable();
            IQueryable<Model.Asset> assets = _context.Set<Model.Asset>().AsQueryable();
            IQueryable<Model.Department> departments = _context.Set<Model.Department>().AsQueryable();
            IQueryable<Model.Employee> employees = _context.Set<Model.Employee>().AsQueryable();
            IQueryable<Model.Location> locations = _context.Set<Model.Location>().AsQueryable();
            IQueryable<Model.Room> rooms = _context.Set<Model.Room>().AsQueryable();
            IQueryable<Model.Region> regions = _context.Set<Model.Region>().AsQueryable();
            IQueryable<Model.AdmCenter> admCenters = _context.Set<Model.AdmCenter>().AsQueryable();

            Func<string, Expression<Func<Model.AssetNi, bool>>> filterPredicate = 
                (filter) => { return (i) => (i.Name1.Contains(filter) || i.Name2.Contains(filter)); };

            //if ((invCompStateIds != null) && (invCompStateIds.Count > 0))
            //    invComps = invComps.Where(i => invCompStateIds.Contains(i.Id));
            if ((departmentIds != null) && (departmentIds.Count > 0))
                departments = departments.Where(d => departmentIds.Contains(d.Id));
            if ((employeeIds != null) && (employeeIds.Count > 0))
                employees = employees.Where(e => employeeIds.Contains(e.Id));
            if ((locationIds != null) && (locationIds.Count > 0))
                locations = locations.Where(l => locationIds.Contains(l.Id));
            if ((roomIds != null) && (roomIds.Count > 0))
                rooms = rooms.Where(r => roomIds.Contains(r.Id));
            if ((regionIds != null) && (regionIds.Count > 0))
                regions = regions.Where(l => regionIds.Contains(l.Id));
            if ((admCenterIds != null) && (admCenterIds.Count > 0))
                admCenters = admCenters.Where(l => admCenterIds.Contains(l.Id));

            Expression<Func<Model.AssetNi, bool>> predicate = null;

            if ((filters != null) && (filters.Count > 0))
            {
                predicate = _filterPredicate(filters[0]);

                for (int i = 1; i < filters.Count; i++)
                {
                    predicate = (conditionType.ToUpper() == "OR") ? ExpressionHelper.Or(predicate, filterPredicate(filters[i])) : ExpressionHelper.And(predicate, filterPredicate(filters[i]));
                }
            }

            if (predicate != null) assetNis = assetNis.Where(predicate);

            query =
                from assetNi in assetNis
                    //join asset in assets on assetNi.AssetId equals asset.Id

                join employee in employees on assetNi.EmployeeId equals employee.Id
                //join department in departments on employee.DepartmentId equals department.Id
                join room in rooms on assetNi.RoomId equals room.Id
                join location in locations on room.LocationId equals location.Id
                join region in regions on room.Location.RegionId equals region.Id
                //join admCenter in admCenters on room.Location.AdmCenterId equals admCenter.Id
                //join invState in invStates on assetNi.InvStateId equals invState.Id
                where (assetNi.AssetId == null)

                select new Dto.AssetNi()
                {
                    Id = assetNi.Id,
                    Code1 = assetNi.Code1,
                    Code2 = assetNi.Code2,
                    Name1 = assetNi.Name1,
                    Name2 = assetNi.Name2,
                    AssetId = assetNi.AssetId,
                    EmployeeId = employee.Id,
                    InternalCode = employee.InternalCode,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    LocationId = location.Id,
                    LocationCode = location.Code,
                    LocationName = location.Name,
                    RegionId = location.Region.Id,
                    RegionCode = location.Region.Code,
                    RegionName = location.Region.Name,
                    RoomId = room.Id,
                    RoomCode = room.Code,
                    RoomName = room.Name,

                    SerialNumber = assetNi.SerialNumber,
                    Producer = assetNi.Producer,
                    Model = assetNi.Model,
                    Quantity = assetNi.Quantity,
                    Info = assetNi.Info,
                    AllowLabel = assetNi.AllowLabel
                    //,InvState = invState.Name

                };

            if (inventoryId.HasValue) query = query.Where(i => i.AssetId == null);
          //  if (assetId.HasValue) query = query.Where(i => i.AssetId == assetId);  //  ORIGINAL
          //  if (assetId.HasValue) query = query.Where(i => i.AssetId == null);  //  OTP

            count = query.Count();

            if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Dto.AssetNi>(sortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Dto.AssetNi>(sortColumn));
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query.ToList();
        }

        public int RecoverAssetNi(int assetId, int inventoryId)
        {
            List<Model.EntityFile> entityFiles = null;
            var inventory = _context.Set<Model.Inventory>().Where(a => a.Active == true).Single();

            if (assetId > 0)
            {
                var asset = _context.Set<Model.Asset>().Where(a => a.Id == assetId).Single();

                if (asset != null)
                {




                    entityFiles = _context.Set<Model.EntityFile>().Where(a => a.EntityId == asset.Id).ToList();

                    if (entityFiles.Count() > 0)
                    {
                        foreach (var item in entityFiles)
                        {
                            item.IsDeleted = true;

                            _context.Update(item);



                        }

                    }

                    var assetInv = _context.Set<Model.InventoryAsset>().Where(a => a.AssetId == assetId).Where(a => a.InventoryId == inventoryId).OrderByDescending(o => o.AssetId).LastOrDefault();


                    if (assetInv != null)
                    {
                        assetInv.CostCenterIdFinal = null;
                        assetInv.EmployeeIdFinal = null;
                        assetInv.RoomIdFinal = null;
                        assetInv.QFinal = 0;
                        assetInv.StateIdFinal = null;
                        assetInv.SerialNumber = string.Empty;
                        assetInv.Info = string.Empty;
                        assetInv.ModifiedAt = null;
                        assetInv.ModifiedBy = null;
                        assetInv.AdministrationIdFinal = null;


                        _context.Update(assetInv);
                    }


                    var assetNi = _context.Set<Model.AssetNi>().Where(a => a.Code1 == asset.TempReco).FirstOrDefault();

                    if (assetNi != null)
                    {
                        assetNi.IsDeleted = false;
                        assetNi.AssetId = null;
                        asset.TempReco = null;
                        asset.TempName = null;
                        _context.Update(assetNi);
                        _context.Update(asset);
                        _context.SaveChanges();
                        return assetId;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }


            }
            else
            {
                return 0;
            }



        }
    }
}
