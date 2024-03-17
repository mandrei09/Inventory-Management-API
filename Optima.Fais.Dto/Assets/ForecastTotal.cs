using System;

namespace Optima.Fais.Dto
{
    public class ForecastTotal
    {
        public int Count { get; set; }

        public decimal April { get; set; }

        public decimal May { get; set; }

        public decimal June { get; set; }

        public decimal July { get; set; }

        public decimal August { get; set; }

        public decimal September { get; set; }

        public decimal Octomber { get; set; }

        public decimal November { get; set; }

        public decimal December { get; set; }

        public decimal January { get; set; }

        public decimal February { get; set; }

        public decimal March { get; set; }

        public decimal Total { get; set; }

        public decimal TotalRem { get; set; }

		public decimal ImportValueOrder { get; set; }

		public decimal ValueAsset { get; set; }

		public decimal ValueAssetYTD { get; set; }

		public decimal ValueAssetYTG { get; set; }

		public decimal ValueOrder { get; set; }

		public decimal ValueOrderPending { get; set; }

		public decimal ValueOrderApproved { get; set; }

		public decimal ValueRequest { get; set; }
	}
}
