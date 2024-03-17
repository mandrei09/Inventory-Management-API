using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
	public class ProjectDashboard
	{
		public ProjectGroupBase [] Values { get; set; }
		public ProjectGroupBase [] ValueDeps { get; set; }
	}
}
