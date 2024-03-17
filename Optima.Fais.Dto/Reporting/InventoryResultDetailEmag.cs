using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryResultDetailEmag
    {
        public string Description { get; set; }
        public string InvNo { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string Key { get; set; }
        public float Initial { get; set; }
        public float Actual { get; set; }
        public string Administration { get; set; }
        public decimal ValueInv { get; set; }
        public string SerialNumber { get; set; }
        public string Info { get; set; }
		public int AssetId { get; set; }
	}
}
