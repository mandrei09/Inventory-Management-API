using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class InvCommitteeBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? CompanyId { get; set; }
    }

    public class InvCommittee : InvCommitteeBase
    {
        public int Id { get; set; }
        public Company Company { get; set; }
    }
}
