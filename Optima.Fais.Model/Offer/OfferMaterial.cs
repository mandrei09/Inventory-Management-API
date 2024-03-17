using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public partial class OfferMaterial : Entity
    {
        public OfferMaterial()
        {
        }

        public int OfferId { get; set; }

        [ForeignKey("OfferId")]
        public Offer Offer { get; set; }

        public int MaterialId { get; set; }

        public Material Material { get; set; }

        public int EmailManagerId { get; set; }

        public EmailManager EmailManager { get; set; }

        public int AppStateId { get; set; }

        public AppState AppState { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Value { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ValueIni { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal PriceIni { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal QuantityIni { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal OrdersValue { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal OrdersPrice { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal OrdersQuantity { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ReceptionsValue { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ReceptionsPrice { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ReceptionsQuantity { get; set; }

        public int? RateId { get; set; }

        public virtual Rate Rate { get; set; }

        public int? RequestId { get; set; }

        public virtual Request Request { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal PreAmount { get; set; }

        public int? OrderTypeId { get; set; }

        public virtual OrderType OrderType { get; set; }

        public bool WIP { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal PriceRon { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ValueRon { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal OrdersPriceRon { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal OrdersValueRon { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal PriceIniRon { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ValueIniRon { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ReceptionsValueRon { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ReceptionsPriceRon { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal PreAmountRon { get; set; }

		public Guid Guid { get; set; }
        public bool Validated { get; set; }

    }
}
