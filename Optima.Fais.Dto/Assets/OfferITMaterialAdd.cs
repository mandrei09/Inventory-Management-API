using System;

namespace Optima.Fais.Dto
{
    public class OfferITMaterialAdd
    {
        public int[] MaterialIds { get; set; }
        public int OfferId { get; set; }
        public int EmailManagerId { get; set; }
		public int RateId { get; set; }
		public Guid Guid { get; set; }
	}
}
