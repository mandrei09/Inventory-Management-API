namespace Optima.Fais.Dto
{
    public class EmployeeDetail : Employee
    {
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
    }
}
