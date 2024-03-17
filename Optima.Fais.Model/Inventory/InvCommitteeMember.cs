using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Faia.Model
{
    public partial class InvCommitteeMember : Entity
    {
        public InvCommitteeMember(){ }
        public int? InvCommitteeId { get; set; }
        public virtual InvCommittee InvCommittee { get; set; }
        public int? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public int? InvCommitteePositionId { get; set; }
        public virtual InvCommitteePosition InvCommitteePosition { get; set; }
    }
}
