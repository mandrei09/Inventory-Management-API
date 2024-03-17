using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class Tax : Entity
    {
        public Tax()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

		public decimal Value { get; set; }

		public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

	}
}
