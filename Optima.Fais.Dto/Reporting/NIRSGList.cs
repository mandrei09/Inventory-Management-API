using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
	public class NIRSGList
	{
		public string InvNo { get; set; }
		public string Description { get; set; }
		public string SerialNumber { get; set; }
		public decimal ValueInv { get; set; }
		public string DocumentNumber { get; set; }
		public string Partner { get; set; }
		public string Location { get; set; }
		public string UserName { get; set; }
	}
}
