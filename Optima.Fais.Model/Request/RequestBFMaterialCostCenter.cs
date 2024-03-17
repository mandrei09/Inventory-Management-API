using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public class RequestBFMaterialCostCenter : Entity
    {
        
        public int RequestBudgetForecastMaterialId { get; set; }
        [ForeignKey("RequestBudgetForecastMaterialId")]
        public virtual RequestBudgetForecastMaterial RequestBudgetForecastMaterial { get; set; }

        
        public int CostCenterId { get; set; }
        [ForeignKey("CostCenterId")]
        public virtual CostCenter CostCenter { get; set; }

        public int Quantity { get; set; }

        public decimal Value { get; set; }

        public decimal ValueRon { get; set; }

        public decimal Price { get; set; }

        public decimal PriceRon { get; set; }

        public Guid Guid { get; set; }

        public int? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public int QuantityRem { get; set; }

        public decimal ValueRem { get; set; }

        public decimal ValueRemRon { get; set; }

        public int? OfferMaterialId { get; set; }

        public virtual OfferMaterial OfferMaterial { get; set; }

        public int? OrderMaterialId { get; set; }

        public virtual OrderMaterial OrderMaterial { get; set; }

        public decimal ReceptionsValue { get; set; }

        public decimal ReceptionsValueRon { get; set; }

        public decimal ReceptionsPrice { get; set; }

        public decimal ReceptionsPriceRon { get; set; }

        public decimal ReceptionsQuantity { get; set; }

        public decimal PreAmount { get; set; }

        public decimal PreAmountRon { get; set; }

        public bool WIP { get; set; }

        public decimal BudgetValueNeed { get; set; }

        public decimal BudgetForecastTimeStamp { get; set; }

		public bool Multiple { get; set; }

        public int? OfferTypeId { get; set; }

        public virtual OfferType OfferType { get; set; }

        public int MaxQuantity { get; set; }

        public decimal MaxValue { get; set; }

        public decimal MaxValueRon { get; set; }

		public int? EmployeeId { get; set; }

		public virtual Employee Employee { get; set; }

		public bool Storno { get; set; }

		public decimal StornoValue { get; set; }

		public int StornoQuantity { get; set; }

	}
}
