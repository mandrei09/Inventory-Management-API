using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class EmployeeDivision : Entity
    {
        public EmployeeDivision()
        {
        }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public int DivisionId { get; set; }

        public Division Division { get; set; }
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }

    }
}
