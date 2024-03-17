using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public partial class OrderMaterial : Entity
    {
        public OrderMaterial()
        {
        }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int MaterialId { get; set; }

        public Material Material { get; set; }

        public int AppStateId { get; set; }

        public AppState AppState { get; set; }

		public decimal Value { get; set; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }

        public decimal ValueIni { get; set; }

        public decimal PriceIni { get; set; }

        public decimal QuantityIni { get; set; }

        public decimal OrdersValue { get; set; }

        public decimal OrdersPrice { get; set; }

        public decimal OrdersQuantity { get; set; }

        public decimal ReceptionsValue { get; set; }

        public decimal ReceptionsPrice { get; set; }

        public decimal ReceptionsQuantity { get; set; }

        public int? RateId { get; set; }

        public virtual Rate Rate { get; set; }

        public int? RequestId { get; set; }

        public virtual Request Request { get; set; }

        public int? OfferMaterialId { get; set; }

        public virtual OfferMaterial OfferMaterial { get; set; }

        public decimal PreAmount { get; set; }

        public int? OrderTypeId { get; set; }

        public virtual OrderType OrderType { get; set; }

        public bool WIP { get; set; }


        public decimal ValueRon { get; set; }

        public decimal PriceRon { get; set; }

        public decimal ValueIniRon { get; set; }

        public decimal PriceIniRon { get; set; }

        public decimal OrdersValueRon { get; set; }

        public decimal OrdersPriceRon { get; set; }

        public decimal ReceptionsValueRon { get; set; }

        public decimal ReceptionsPriceRon { get; set; }

        public decimal PreAmountRon { get; set; }

        public Guid Guid { get; set; }
        public bool Validated { get; set; }

        public int? BudgetForecastId { get; set; }

        public virtual BudgetForecast BudgetForecast { get; set; }
    }
}
