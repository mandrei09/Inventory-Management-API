using System;

namespace Optima.Fais.Dto
{
    public class MatrixLevel
    {
        public int Id { get; set; }
        public MatrixUI Matrix { get; set; }
        public CodeNameEntity Level { get; set; }
        public CodeNameEntity Uom { get; set; }
        public EmployeeResource Employee { get; set; }
		public decimal Amount { get; set; }
	}
}
