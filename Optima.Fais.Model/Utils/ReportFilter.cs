using System;
using System.Collections.Generic;

namespace Optima.Fais.Model.Utils
{
    public class ReportFilter
    {
        public List<int?> DivisionIds { get; set; }
        public List<int?> AdministrationIds { get; set; }
        public List<int?> DepartmentIds { get; set; }
        public List<int?> CostCenterIds { get; set; }
        public List<int?> EmployeeIds { get; set; }
        public bool? IsPrinted { get; set; }
        public bool? IsDuplicate { get; set; }
		public bool? IsTemp { get; set; }
        public DateTime? InventoryDateStart { get; set; }
        public DateTime? InventoryDateEnd { get; set; }
    }
}
