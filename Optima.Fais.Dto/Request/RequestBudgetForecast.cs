using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class RequestBudgetForecast
    {
        public int Id { get; set; }
        public Request Request { get; set; }
        public BudgetForecast BudgetForecast { get; set; }
		public CodeNameEntity Project { get; set; }
        public ContractAmountEntity Contract { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
        public int TotalOrderQuantity { get; set; }
        public decimal Value { get; set; }
        public decimal ValueRon { get; set; }
        public decimal MaxValue { get; set; }
        public decimal MaxValueRon { get; set; }
        public decimal TotalOrderValue { get; set; }
        public decimal TotalOrderValueRon { get; set; }
        public decimal Price { get; set; }

        public decimal PriceRon { get; set; }
        public string Materials { get; set; }
        public IEnumerable<RequestBudgetForecastMaterial> RequestBudgetForecastMaterials { get; set; }

        public bool NeedContract { get; set; }

        public bool NeedBudget { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public string  Info { get; set; }

        public decimal NeedBudgetValue { get; set; }

        public decimal NeedContractValue { get; set; }

        public CodeNameEntity OfferType { get; set; }

		public decimal ValueUsed { get; set; }

		public decimal ValueUsedRon { get; set; }

        public bool Multiple { get; set; }
    }
}
