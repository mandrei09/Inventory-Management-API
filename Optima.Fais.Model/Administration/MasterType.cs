using System.Collections.Generic;

namespace Optima.Fais.Model
{
	public partial class MasterType : Entity
	{
		public MasterType()
		{
			Types = new HashSet<Type>();
		}

		public string Code { get; set; }

		public string Name { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

		public virtual ICollection<Type> Types { get; set; }

	}
}
