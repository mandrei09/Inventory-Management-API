using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Common
{
    public class OrderMaterialUpdate
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceRon { get; set; }
        public decimal Value { get; set; }
        public decimal ValueRon { get; set; }
        public decimal PreAmount { get; set; }
    }
}
