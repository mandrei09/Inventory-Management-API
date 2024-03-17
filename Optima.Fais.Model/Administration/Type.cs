using System.Collections.Generic;

namespace Optima.Fais.Model
{
	public partial class Type : Entity
	{
		public Type()
		{
			SubTypes = new HashSet<SubType>();
		}

		public string Code { get; set; }

		public string Name { get; set; }

		public int? MasterTypeId { get; set; }

		public virtual MasterType MasterType { get; set; }

		public virtual ICollection<SubType> SubTypes { get; set; }

	}
}
