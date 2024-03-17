using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryLabel
    {
        public string Year { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string CostCenter { get; set; }
        public string CostCenterCode { get; set; }
        public string Room { get; set; }

        public List<InventoryLabelDetail> Details;

        public List<InventoryLabelDetail> InventoryLabelDetail()
        {
            return Details;
        }
    }
}
