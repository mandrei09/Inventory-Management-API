using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
	public partial class Commodity : Entity
	{
		public Commodity()
		{
			
		}

		public string Code { get; set; }

		public string Name { get; set; }

		public string Domain { get; set; }

		public string UniqueName { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

		public int ContractId { get; set; }

		[ForeignKey("ContractId")]
		public virtual Contract Contract { get; set; }

	}
}
