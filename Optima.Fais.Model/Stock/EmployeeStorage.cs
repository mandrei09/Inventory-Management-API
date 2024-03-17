using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class EmployeeStorage : Entity
    {
        public EmployeeStorage()
        {
        }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public int StorageId { get; set; }

        public Storage Storage { get; set; }

	}
}
