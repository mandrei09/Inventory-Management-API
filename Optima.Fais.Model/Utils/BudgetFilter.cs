using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class BudgetFilter
    {

        public int? AccMonthId { get; set; }
        public List<int?> AdministrationIds { get; set; }
        public List<int?> AdmCenterIds { get; set; }
        public List<int?> AssetTypeIds { get; set; }
        public List<int?> ProjectTypeIds { get; set; }
        public List<int?> DepartmentIds { get; set; }
        public List<int?> DivisionIds { get; set; }
        public List<int?> CompanyIds { get; set; }
        public List<int?> EmployeeIds { get; set; }
        public List<int?> CostCenterIds { get; set; }
        public List<int?> PartnerIds { get; set; }
        public List<int?> BudgetManagerIds { get; set; }
        public List<int?> ProjectIds { get; set; }
        public List<int?> AccMonthIds { get; set; }
        public List<int?> ActivityIds { get; set; }
        public List<int?> BudgetBaseIds { get; set; }
        public List<int?> BudgetForecastIds { get; set; }
        public string Filter { get; set; }
		public bool? HasChange { get; set; }
        public bool? IsFirst { get; set; }

		public string Role { get; set; }
		public int EmployeeId { get; set; }
		public bool InInventory { get; set; }
		public string UserId { get; set; }
		public DateTime? MonthYear { get; set; }
        public int? ProjectId { get; set; }
    }
}
