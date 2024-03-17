using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
	public class SGPIFList
	{
		public string InvNo { get; set; }
		public string Description { get; set; }
		public string SerialNumber { get; set; }
		public string CostCenter { get; set; }
		public string AssetNature { get; set; }
		public string Employee { get; set; }
		public string BudgetManager { get; set; }
		public string Document { get; set; }
		public string Partner { get; set; }
		public float Quantity { get; set; }
		public decimal ValueInv { get; set; }
		public string UserName { get; set; }
	}
}
