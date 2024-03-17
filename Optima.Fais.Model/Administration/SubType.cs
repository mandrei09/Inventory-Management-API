using System.Collections.Generic;

namespace Optima.Fais.Model
{
	public partial class SubType : Entity
	{
		public SubType()
		{
		}

		public string Code { get; set; }

		public string Name { get; set; }

		public int? TypeId { get; set; }

		public virtual Type Type { get; set; }

	}
}
