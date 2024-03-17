using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class OfferUI
    {
		public int Id { get; set; }

		public string Code { get; set; }

        public string Name { get; set; }

        public CodeNameEntity Company { get; set; }

        public CodeNameEntity Division { get; set; }

        public Request Request { get; set; }

        public EmployeeResource Employee { get; set; }

        public CodePartnerEntity Partner { get; set; }

        public int? AppStateId { get; set; }

        public CodeNameEntity AppState { get; set; }

        public float Quantity { get; set; }

        public float QuantityRem { get; set; }

        public float QuantityUsed { get; set; }

        public decimal ValueIni { get; set; }

        public decimal ValueIniRon { get; set; }

        public decimal ValueFin { get; set; }

        public decimal ValueFinRon { get; set; }

        public decimal ValueUsed { get; set; }

        public decimal ValueUsedRon { get; set; }

        public CodeNameEntity AssetType { get; set; }

        public CodeNameEntity ProjectType { get; set; }

        public CodeNameEntity Uom { get; set; }

        public CodeNameEntity OfferType { get; set; }
    }
}
