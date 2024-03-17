using System;

namespace Optima.Fais.Dto
{
    public class AccMonth
    {
        public int Id { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? EndDate { get; set; }

		public int FiscalMonth { get; set; }

		public int FiscalYear { get; set; }
	}
}
