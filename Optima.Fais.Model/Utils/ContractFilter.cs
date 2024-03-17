using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class ContractFilter
    {

        public int? AccMonthId { get; set; }
        public List<int?> AdministrationIds { get; set; }
        public List<int?> CompanyIds { get; set; }
        public List<int?> EmployeeIds { get; set; }
        public List<int?> CostCenterIds { get; set; }
        public List<int?> PartnerIds { get; set; }
        public List<int?> UomIds { get; set; }
        public string Filter { get; set; }

    }
}
