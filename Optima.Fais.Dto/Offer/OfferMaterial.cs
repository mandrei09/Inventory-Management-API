using System;

namespace Optima.Fais.Dto
{
    public class OfferMaterial
    {
        public int Id { get; set; }
        public OfferUI Offer { get; set; }
        public Material Material { get; set; }
        public AppState AppState { get; set; }
        public Rate Rate { get; set; }
        public int AppStateId { get; set; }
        public decimal Value { get; set; }
        public decimal ValueRon { get; set; }
        public decimal ValueIni { get; set; }
        public decimal ValueIniRon { get; set; }
        public decimal Price { get; set; }
        public decimal PriceIni { get; set; }
        public decimal PriceRon { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityIni { get; set; }
        public decimal OrdersValue { get; set; }
        public decimal OrdersPrice { get; set; }
        public decimal OrdersQuantity { get; set; }
        public decimal ReceptionsValue { get; set; }
        public decimal ReceptionsPrice { get; set; }
        public decimal ReceptionsQuantity { get; set; }
		public bool ReadOnly { get; set; }
        public decimal PreAmount { get; set; }
		public bool WIP { get; set; }
        public decimal RateValue { get; set; }
		public string RateDate { get; set; }
        public CodeNameEntity Uom { get; set; }
    }
}
