using System;

namespace Optima.Fais.Model
{
    public partial class AccMonth : Entity
    {
        public AccMonth()
        {
        }

        public int Month { get; set; }

        public int Year { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

		public int FiscalMonth { get; set; }

		public int FiscalYear { get; set; }
	}
}
