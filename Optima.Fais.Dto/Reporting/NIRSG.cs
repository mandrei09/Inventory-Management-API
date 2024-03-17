using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
	public class NIRSG
	{
		public string Username { get; set; }
		public string DocumentNumber { get; set; }
		public string DocumentDate { get; set; }

		public List<NIRSGList> NIRSGList;
	}
}
