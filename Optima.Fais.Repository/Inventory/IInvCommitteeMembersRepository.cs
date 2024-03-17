using Optima.Faia.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IInvCommitteeMembersRepository : IRepository<InvCommitteeMember>
    {
        IEnumerable<InvCommitteeMember> GetByFilters(int? invCommitteeId, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        int GetCountByFilters(int? invCommitteeId, string filter);
        //Task<IEnumerable<InvCommitteeMember>> GetInCommitteeMemberAsync(int? invCommitteeId, int? costCenterId);
        Task<IEnumerable<InvCommitteeMember>> GetInInvCommitteeAsync(int invCommitteeId);
        Task<IEnumerable<InvCommitteeMember>> GetAllInvCommitteeMemberAsync();
        Task<IEnumerable<InvCommitteeMember>> GetPVGInvCommitteeMemberAsync();
    }
}
