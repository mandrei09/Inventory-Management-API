using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class DifferenceListAsset
    {
        public string Name { get; set; }
        public string InvNo { get; set; }
        public string Barcode { get; set; }
        public string CostCenter { get; set; }
        public string HolderName { get; set; }
        public float Value { get; set; }
        public string Observations { get; set; }
        public string AccountingGroup { get; set; }
    }
}
