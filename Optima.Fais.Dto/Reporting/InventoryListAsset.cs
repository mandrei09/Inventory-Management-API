using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryListAsset
    {
        public string Name { get; set; }
        public string InvNo { get; set; }
        public string Barcode { get; set; }
        public string CostCenter { get; set; }
        public string HolderName { get; set; }
        public int IsReal { get; set; }
        public int IsScriptic { get; set; }
        public int IsPlus { get; set; }
        public int IsMinus { get; set; }
        public float PricePerUnit { get; set; }
        public float AccountingValue { get; set; }
        public float PlusDifference { get; set; }
        public float MinusDifference { get; set; }
        public float Value { get; set; }
        public float Depreciation { get; set; }
        public string Observations { get; set; }
        public string AccountingGroup { get; set; }
    }
}
