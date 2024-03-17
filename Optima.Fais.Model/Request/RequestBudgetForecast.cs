using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class RequestBudgetForecast : Entity
    {
        public RequestBudgetForecast()
        {
        }

        public int? RequestId { get; set; }

        public virtual Request Request { get; set; }

        public int? BudgetForecastId { get; set; }

        public virtual BudgetForecast BudgetForecast { get; set; }

        public int AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public int BudgetManagerId { get; set; }

        public virtual BudgetManager BudgetManager { get; set; }

        public int? ContractId { get; set; }

        public virtual Contract Contract { get; set; }

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

		public virtual ICollection<RequestBudgetForecastMaterial> RequestBudgetForecastMaterials { get; set; }

		public Guid Guid { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

		public bool NeedContract { get; set; }

        public bool NeedBudget { get; set; }

        public decimal NeedBudgetValue { get; set; }

        public decimal NeedContractValue { get; set; }

        public int? OfferTypeId { get; set; }

        public virtual OfferType OfferType { get; set; }

		public decimal ValueUsed { get; set; }

		public decimal ValueUsedRon { get; set; }

	}
}
