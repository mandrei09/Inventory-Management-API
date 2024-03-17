using System;

namespace Optima.Fais.Dto
{
    public class DashboardTotal
    {
        public int Count { get; set; }

        public decimal CurrentAPC { get; set; }

        public decimal CurrBkValue { get; set; }

		public decimal AccumulDep { get; set; }

		public float Quantity { get; set; }
	}
}
