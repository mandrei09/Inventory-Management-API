using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Repository.Inventory
{
    public interface IInvCommitteePositionsRepository : IRepository<InvCommitteePosition>
    {
        IEnumerable<InvCommitteePosition> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        int GetCountByFilters(string filter);
    }
}
