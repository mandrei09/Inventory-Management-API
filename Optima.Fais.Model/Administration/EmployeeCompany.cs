using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class EmployeeCompany : Entity
    {
        public EmployeeCompany()
        {
        }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

	}
}
