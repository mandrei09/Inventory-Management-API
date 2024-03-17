using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class OfferFilter
    {

        public int? AccMonthId { get; set; }
        public List<int?> AdministrationIds { get; set; }
        public List<int?> CompanyIds { get; set; }
        public List<int?> EmployeeIds { get; set; }
        public List<int?> CostCenterIds { get; set; }
        public List<int?> PartnerIds { get; set; }
		public List<int?> RequestIds { get; set; }
		public string Filter { get; set; }
        public string Role { get; set; }
        public int EmployeeId { get; set; }
        public bool InInventory { get; set; }
		public string Type { get; set; }
        public string UserId { get; set; }
    }
}
