using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class CommissioningAsset
    {
        public string Name { get; set; }
        public string InvNo { get; set; }
        public string Barcode { get; set; }
        public float Value { get; set; }
        public string SourceDocumentNumber { get; set; }
        public string SourceDocumentDate { get; set; }
        public string SourceCompanyName { get; set; }
    }
}
