using System;
using System.Collections.Generic;

namespace Optima.Fais.Model.Utils
{
    public class DashboardFilter
    {
        public List<int?> BudgetManagerIds { get; set; }
        public List<int?> DepartmentIds { get; set; }
        public List<int?> DivisionIds { get; set; }
        public List<int?> CostCenterIds { get; set; }
        public List<int?> AdministrationIds { get; set; }
        public List<int?> TypeIds { get; set; }
        public List<int?> AssetTypeIds { get; set; }
        public List<int?> ProjectIds { get; set; }

        public List<int?> EmployeeIds { get; set; }
        //public string FilterDepartment { get; set; }
        //public string FilterDivision { get; set; }
        //public string FilterCostCenter { get; set; }
        //public string FilterType { get; set; }
        //public string FilterAssetType { get; set; }
        public string Filter { get; set; }
        public int ReportType { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public int EmployeeId { get; set; }

    }
}
