using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryLabelDetail
    {
        public string CostCenter { get; set; }
        public string Room { get; set; }
        public string InvNo { get; set; }
        public string Name { get; set; }
        public string Employee { get; set; }
        public float Quantity { get; set; }
    }
}
