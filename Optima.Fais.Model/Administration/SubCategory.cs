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

		public int? CategoryId { get; set; }

		public virtual Category Category { get; set; }

		public int? CategoryENId { get; set; }

		public virtual CategoryEN CategoryEN { get; set; }

	}
}
