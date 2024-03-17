using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
    public partial class InvCommittee : Entity
    {
        public InvCommittee() { }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
