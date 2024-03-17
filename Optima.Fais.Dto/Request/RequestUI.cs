using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class RequestUI
    {
		public int Id { get; set; }

		public string Code { get; set; }

        public string Name { get; set; }

        public string Info { get; set; }

        public CodeNameEntity Company { get; set; }

        public CodeNameEntity Division { get; set; }

		public CodeNameEntity Department { get; set; }

		public CodeNameEntity AssetType { get; set; }

        public CodeNameEntity ProjectType { get; set; }

        public DateTime? EndDate { get; set; }

        public EmployeeResource Employee { get; set; }

        public int? AppStateId { get; set; }

        public CodeNameEntity AppState { get; set; }

        public bool IsAccepted { get; set; }

        public decimal BudgetValueNeed { get; set; }

        public MonthEntity StartAccMonth { get; set; }

		public decimal Quantity { get; set; }

        public DateTime? CreatedAt { get; set; }

		public DateTime? ModifiedAt { get; set; }
	}
}
