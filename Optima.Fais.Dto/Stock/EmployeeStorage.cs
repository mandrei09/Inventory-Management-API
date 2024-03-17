using System;

namespace Optima.Fais.Dto
{
    public class EmployeeStorage
    {
        public int Id { get; set; }
        public CodeNameEntity Storage { get; set; }
        public EmployeeResource Employee { get; set; }
	}
}
