using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class EmployeeCostCenter : Entity
    {
        public EmployeeCostCenter()
        {
        }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public int CostCenterId { get; set; }

        public CostCenter CostCenter { get; set; }

	}
}
