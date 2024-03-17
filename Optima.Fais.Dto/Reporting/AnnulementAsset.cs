using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class AnnulementAsset
    {
        public string Name { get; set; }
        public string InvNo { get; set; }
        public string Barcode { get; set; }
        public string MeasureUnit { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set;  }
        public decimal Value { get; set;  }
        public decimal RemainingValue { get; set; }
    }
}
