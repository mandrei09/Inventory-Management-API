using System.Collections.Generic;

namespace Optima.Fais.Model
{
	public partial class SubCategory : Entity
	{
		public SubCategory()
		{

		}

		public string Code { get; set; }

		public string Name { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }


	}
}
