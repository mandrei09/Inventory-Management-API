using System;

namespace Optima.Fais.Model
{
    public class ReportBook : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public Guid Guid { get; set; }

		public int? CostCenterId { get; set; }

		public virtual CostCenter CostCenter { get; set; }
	}
}
