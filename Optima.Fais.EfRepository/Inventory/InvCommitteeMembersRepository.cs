using Microsoft.EntityFrameworkCore;
using Optima.Faia.Model;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Optima.Fais.EfRepository.Inventory
{
    public class InvCommitteeMembersRepository : Repository<InvCommitteeMember>, IInvCommitteeMembersRepository
    {
        public InvCommitteeMembersRepository(ApplicationDbContext context)
           : base(context, null)
        { }

        private Expression<Func<InvCommitteeMember, bool>> GetFiltersPredicate(int? invCommitteeId, string filter)
        {
            Expression<Func<InvCommitteeMember, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if (invCommitteeId > 0)
            {
                predicate = predicate != null
                   ? ExpressionHelper.And<InvCommitteeMember>(predicate, p => p.InvCommitteeId == invCommitteeId)
                   : p => p.InvCommitteeId == invCommitteeId;
            }

            return predicate;
        }

        public IEnumerable<InvCommitteeMember> GetByFilters(int? invCommitteeId, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(invCommitteeId, filter);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(int? invCommitteeId, string filter)
        {
            var predicate = GetFiltersPredicate(invCommitteeId, filter);

            return GetQueryable(predicate).Count();
        }

        public async Task<IEnumerable<InvCommitteeMember>> GetInInvCommitteeAsync(int invCommitteeId)
        {
            var query = GetQueryable()
                .Include(p => p.Employee)
                .Include(p => p.InvCommitteePosition)
                .Where(p => p.InvCommitteeId == invCommitteeId && !p.IsDeleted);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<InvCommitteeMember>> GetAllInvCommitteeMemberAsync()
        {
            var query = GetQueryable()
                .Include(p => p.Employee)
                .Include(p => p.InvCommitteePosition)
                .Where(p => !p.IsDeleted);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<InvCommitteeMember>> GetPVGInvCommitteeMemberAsync()
        {
            int pvgAdministrationId = _context.Set<Model.Administration>().Where(a => a.Code == "PVG").Single().Id;
            int invCommitteeId = _context.Set<Model.InventoryPlan>()
                   .Where(ip => ip.AdministrationId == pvgAdministrationId && ip.CostCenterId == null && !ip.IsDeleted)
                   .Single().Id; 
            var query = GetQueryable()
                .Include(p => p.Employee)
                .Include(p => p.InvCommitteePosition)
                .Where(p => !p.IsDeleted && p.InvCommitteeId == invCommitteeId);
            return await query.ToListAsync();
        }
    }
}
