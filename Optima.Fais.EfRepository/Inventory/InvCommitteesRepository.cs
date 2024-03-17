using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.EfRepository
{
    public class InvCommitteesRepository : Repository<InvCommittee>, IInvCommitteesRepository
    {
        public InvCommitteesRepository(ApplicationDbContext context)
           : base(context, null)
        { }


    }
}
