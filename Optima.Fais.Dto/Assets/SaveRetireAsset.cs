using System;

namespace Optima.Fais.Dto
{
    public class SaveRetireAsset
    {
        public string ItemText { get; set; }
        public string RefDocNo { get; set; }
        public bool CompleteRetire { get; set; }
        public bool PriorYearAcquisitions { get; set; }
        public bool CurrentYearAcquisitions { get; set; }
        public decimal Amount { get; set; }
        public decimal Percent { get; set; }
        public decimal Quantity { get; set; }
		public int UomId { get; set; }
		public int AssetId { get; set; }
    }
}
