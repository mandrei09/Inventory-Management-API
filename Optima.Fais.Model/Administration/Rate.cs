using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
	public partial class Rate : Entity
	{
		public Rate()
		{
			
		}

		public string Code { get; set; }

		public string Name { get; set; }

		public decimal Value { get; set; }

		public int? UomId { get; set; }

		public virtual Uom Uom { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

		public int? AccMonthId { get; set; }

		public virtual AccMonth AccMonth { get; set; }

		public bool IsLast { get; set; }

		public decimal Multiplier { get; set; }

	}
}
