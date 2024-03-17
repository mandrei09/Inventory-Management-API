using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class InvCommitteeMemberBase
    {
        public int? InvCommitteeId { get; set; }
        public int? EmployeeId { get; set; }
        public int? InvCommitteePositionId { get; set; }
    }

    public class InvCommitteeMember : InvCommitteeMemberBase
    {
        public int Id { get; set; }
        public InvCommittee InvCommittee { get; set; }
        public Employee Employee { get; set; }
        public InvCommitteePosition InvCommitteePosition { get; set; } 
    }

}
