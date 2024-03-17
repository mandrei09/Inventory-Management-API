using System;

namespace Optima.Fais.Dto
{
    public class EmployeeCompany
    {
        public int Id { get; set; }
        public CodeNameEntity Company { get; set; }
        public EmployeeResource Employee { get; set; }
	}
}
