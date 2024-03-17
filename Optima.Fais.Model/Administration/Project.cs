using System.Collections.Generic;

namespace Optima.Fais.Model
{
	public partial class Project : Entity
	{
		public Project()
		{
			
		}

		public string Code { get; set; }

		public string Name { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

		public int? ProjectTypeId { get; set; }

		public virtual ProjectType ProjectType { get; set; }

		public int? ERPId { get; set; }

	}
}
