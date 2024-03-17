using System.Collections.Generic;

namespace Optima.Fais.Model
{
	public partial class InterCompanyEN : Entity
	{
		public InterCompanyEN()
		{

		}

		public string Code { get; set; }

		public string Name { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

	}
}
