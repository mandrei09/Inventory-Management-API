using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
	public partial class ContractAmount : Entity
	{
		public ContractAmount()
		{
			
		}

		public string Code { get; set; }

		public string Name { get; set; }

		public decimal Amount { get; set; }

		public decimal AmountRon { get; set; }

		public decimal AmountRem { get; set; }

		public decimal AmountRonRem { get; set; }

		public decimal AmountUsed { get; set; }

		public decimal AmountRonUsed { get; set; }

		public int? UomId { get; set; }

		public virtual Uom Uom { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

		public int? RateId { get; set; }

		public virtual Rate Rate { get; set; }

		public int? RateRonId { get; set; }

		public virtual Rate RateRon { get; set; }

	}
}
