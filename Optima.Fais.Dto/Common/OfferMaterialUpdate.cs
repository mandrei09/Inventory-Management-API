using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Common
{
    public class OfferMaterialUpdate
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
		public bool WIP { get; set; }
        public decimal RateValue { get; set; }
		public DateTime? RateDate { get; set; }
		public int UomId { get; set; }
	}
}
