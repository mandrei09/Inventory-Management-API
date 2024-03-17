using System;

namespace Optima.Fais.Dto
{
    public class EmployeeCostCenter
    {
        public int Id { get; set; }
        public CodeNameEntity CostCenter { get; set; }
        public CodeNameEntity Storage { get; set; }
        public EmployeeResource Employee { get; set; }
	}
}
