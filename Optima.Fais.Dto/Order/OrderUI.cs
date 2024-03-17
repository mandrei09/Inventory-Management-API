using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class OrderUI
    {
		public int Id { get; set; }

		public string Code { get; set; }

        public string Name { get; set; }

        public int? AppStateId { get; set; }

        public CodeNameEntity AppState { get; set; }

        public float Quantity { get; set; }

        public float QuantityRem { get; set; }

        public float QuantityUsed { get; set; }

        public decimal ValueIni { get; set; }

        public decimal ValueFin { get; set; }

        public decimal ValueUsed { get; set; }

        public decimal Price { get; set; }

        public Offer Offer { get; set; }

        public bool IsAccepted { get; set; }

        public string Info { get; set; }

        public CodeNameEntity Division { get; set; }

        public CodeNameEntity AssetType { get; set; }

        public CodeNameEntity ProjectType { get; set; }

        public EmployeeResource Employee { get; set; }

        public decimal BudgetValueNeed { get; set; }

        public Contract Contract { get; set; }
    }
}
