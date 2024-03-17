using System;

namespace Optima.Fais.Dto
{
    public class EmployeeDivision
    {
        public int Id { get; set; }
        public CodeNameEntity Division { get; set; }

        public CodeNameEntity Department { get; set; }
        public EmployeeResource Employee { get; set; }
	}
}
