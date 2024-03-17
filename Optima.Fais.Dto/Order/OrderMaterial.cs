using System;

namespace Optima.Fais.Dto
{
    public class OrderMaterial
    {
        public int Id { get; set; }
        public OrderUI Order { get; set; }
        public Material Material { get; set; }
        public CodeNameEntity AppState { get; set; }
        public Rate Rate { get; set; }
        public int AppStateId { get; set; }
        public decimal Value { get; set; }
        public decimal ValueIni { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityIni { get; set; }
        public decimal OrdersValue { get; set; }
        public decimal OrdersPrice { get; set; }
        public decimal OrdersQuantity { get; set; }
        public decimal ReceptionsValue { get; set; }
        public decimal ReceptionsPrice { get; set; }
        public decimal ReceptionsQuantity { get; set; }
    }
}
