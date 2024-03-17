using System;

namespace Optima.Fais.Dto
{
    public class RequestBudgetForecastMaterial
    {
        public int Id { get; set; }
        public RequestBudgetForecast RequestBGTF { get; set; }
        public Material Material { get; set; }
        public int Quantity { get; set; }

        public decimal Value { get; set; }

        public decimal ValueRon { get; set; }

        public decimal Price { get; set; }

        public decimal PriceRon { get; set; }

		public Order Order { get; set; }

        public int QuantityRem { get; set; }

        public decimal ValueRem { get; set; }

        public decimal ValueRemRon { get; set; }

        public AppState AppState { get; set; }

        public decimal ReceptionsValue { get; set; }

        public decimal ReceptionsValueRon { get; set; }

        public decimal ReceptionsPrice { get; set; }

        public decimal ReceptionsPriceRon { get; set; }

        public decimal ReceptionsQuantity { get; set; }

        public decimal PreAmount { get; set; }

        public decimal PreAmountRon { get; set; }

        public bool WIP { get; set; }

        public decimal BudgetValueNeed { get; set; }

        public OfferMaterial OfferMaterial { get; set; }

        public OrderMaterial OrderMaterial { get; set; }

        public decimal BudgetForecastTimeStamp { get; set; }

        public int MaxQuantity { get; set; }

        public int TotalCostCenterQuantity { get; set; }

        public decimal MaxValue { get; set; }

        public decimal MaxValueRon { get; set; }

        public decimal TotalCostCenterValue { get; set; }

        public decimal TotalCostCenterValueRon { get; set; }

        public CodeNameEntity OfferType { get; set; }

        public bool HasParentAsset { get; set; }

        public string ParentAsset { get; set; }

		public decimal ValueUsed { get; set; }

		public decimal ValueUsedRon { get; set; }
        public bool Multiple { get; set; }
    }
}
