using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
	public class CompanyDashboard
	{
		public CompanyGroupBase[] Values { get; set; }
		public CompanyGroupBase[] ValueDeps { get; set; }
	}
}
