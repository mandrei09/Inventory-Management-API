using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class AdmCenter : Entity
    {
        public AdmCenter()
        {
            CostCenters = new HashSet<CostCenter>();
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CostCenter> CostCenters { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public int? EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        
    }
}
