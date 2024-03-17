using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
	public class TrasferSG
	{
		public string Username { get; set; }
		public string DocumentNumber { get; set; }
		public string DocumentDate { get; set; }

		public List<TransferSGList> TransferSGList;
	}
}
