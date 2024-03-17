using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class Administration : Entity
    {
        public Administration()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? DivisionId { get; set; }

        public Division Division { get; set; }

        //public int? CostCenterId { get; set; }

        //public virtual CostCenter CostCenter { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

		public int? ERPId { get; set; }

		public int AssetCount { get; set; }

	}
}
