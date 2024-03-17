using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public class RequestBudgetForecastMaterial : Entity
    {
        
        public int RequestBudgetForecastId { get; set; }
        [ForeignKey("RequestBudgetForecastId")]
        public virtual RequestBudgetForecast RequestBudgetForecast { get; set; }

        
        public int MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }

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

        public int MaxQuantity { get; set; }

        public int TotalCostCenterQuantity { get; set; }

        public decimal MaxValue { get; set; }

        public decimal MaxValueRon { get; set; }

        public decimal TotalCostCenterValue { get; set; }

        public decimal TotalCostCenterValueRon { get; set; }

        public int? OfferTypeId { get; set; }

        public virtual OfferType OfferType { get; set; }

		public bool HasParentAsset { get; set; }

        public string ParentAsset { get; set; }

		public decimal ValueUsed { get; set; }

		public decimal ValueUsedRon { get; set; }
        public bool Multiple { get; set; }
    }
}
