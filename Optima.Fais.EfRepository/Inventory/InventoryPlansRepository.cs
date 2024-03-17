using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Repository.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InventoryPlan = Optima.Fais.Model.InventoryPlan;

namespace Optima.Fais.EfRepository.Inventory
{
    public class InventoryPlansRepository : Repository<InventoryPlan>, IInventoryPlansRepository
    {
        public InventoryPlansRepository(ApplicationDbContext context)
            : base(context, (filter) => { 
                return (a) => ((a.Administration.Name.Contains(filter) || a.Administration.Code.Contains(filter)) ||
                (a.CostCenter.Name.Contains(filter) || a.CostCenter.Code.Contains(filter))
                ); 
            })
        {}

        private Expression<Func<InventoryPlan, bool>> GetFiltersPredicate(int? administrationId, int? costCenterId, string filter)
        {
            Expression<Func<InventoryPlan, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if (administrationId > 0)
            {
                predicate = predicate != null
                   ? ExpressionHelper.And<InventoryPlan>(predicate, p => p.AdministrationId == administrationId)
                   : p => p.AdministrationId == administrationId;
            }

            if (costCenterId > 0)
            {
                predicate = predicate != null
                   ? ExpressionHelper.And<InventoryPlan>(predicate, p => p.CostCenterId == costCenterId)
                   : p => p.CostCenterId == costCenterId;
            }

            return predicate;
        }

        public IEnumerable<InventoryPlan> GetByFilters(int? administrationId, int? costCenterId, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(administrationId, costCenterId, filter);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(int? administrationId, int? costCenterId, string filter)
        {
            var predicate = GetFiltersPredicate(administrationId, costCenterId, filter);

            return GetQueryable(predicate).Count();
        }

        public async Task<InventoryPlan> GetByAdministrationAndCostCenterAsync(int? administrationId, int? costCenterId)
        {
            InventoryPlan result = new InventoryPlan();
            if (costCenterId != null && costCenterId > 0)
                 result = await _context.Set<InventoryPlan>().Where(i => i.CostCenterId == costCenterId).FirstOrDefaultAsync();
            else if (administrationId != null && administrationId > 0)
                result = await _context.Set<InventoryPlan>().Where(i => i.AdministrationId == administrationId).FirstOrDefaultAsync();
            
            return result;
        }

        public async Task<InventoryPlan> GetByFinalReportAsync()
        {
            InventoryPlan result = new InventoryPlan();

            Model.Administration administrationPVG = await _context.Set<Model.Administration>().Where(a => a.Code == "PVG").FirstOrDefaultAsync();

            result = await _context.Set<InventoryPlan>().Where(i => i.CostCenterId == null && i.AdministrationId == administrationPVG.Id).FirstOrDefaultAsync();
            return result;
        }

        public int UpdateInventoryPlan(Dto.InventoryPlan item)
        {
            InventoryPlan inventoryPlan = null;

            if (item.Id > 0)
            {
                inventoryPlan = _context.Set<InventoryPlan>().Where(a => a.Id == item.Id).Single();
                inventoryPlan.DateStarted = item.DateStarted.Value.ToLocalTime();
                inventoryPlan.DateFinished = item.DateFinished.Value.ToLocalTime();
            }

            _context.Set<InventoryPlan>().Update(inventoryPlan);
            _context.SaveChanges();
            return inventoryPlan.Id;
        }
    }
}
