using System;
using System.Collections.Generic;

namespace Optima.Fais.Model.Utils
{
    public class ColumnAssetFilter
    {
		public List<int?> InvStateIds { get; set; }
		public List<int?> MaterialIds { get; set; }
		public List<int?> CostCenterIds { get; set; }
        public List<int?> AccountIds { get; set; }
        public List<int?> LocationIds { get; set; }
        public List<int?> PartnerIds { get; set; }
        public string InvNo { get; set; }
        public string Quantity { get; set; }
    }
}
