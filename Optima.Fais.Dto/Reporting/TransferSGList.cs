using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
	public class TransferSGList
	{
		public string UserName { get; set; }
		public string InvNo { get; set; }
		public string Description { get; set; }
		public string SerialNumber { get; set; }
		public string RoomInitial { get; set; }
		public string RoomFinal { get; set; }
		public string CostCenterInitial { get; set; }
		public string CostCenterFinal { get; set; }
		public string EmployeeInitial { get; set; }
		public string EmployeeFinal { get; set; }
		public string BMInitial { get; set; }
		public string BMFinal { get; set; }
	}
}
