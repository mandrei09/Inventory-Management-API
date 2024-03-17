using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class FixedAssetReportAsset
    {
        public string DocumentType { get; set; }
        public string DocumentDate { get; set; }
        public string DocumentNumber { get; set; }
        public string Operation { get; set; }
        public int Quantity { get; set; }
        public float DebitValue { get; set; }
        public float CreditValue { get; set; }
        public float RemainingValue { get; set; }
    }
}
