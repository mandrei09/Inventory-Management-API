using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
	public class SGScrabList
	{
		public string InvNo { get; set; }
		public string Description { get; set; }
		public decimal ValueInv { get; set; }
		public decimal ValueDep { get; set; }
		public decimal ValueRem { get; set; }
		public string DocumentNumber { get; set; }
		public string UserName { get; set; }
	}
}
