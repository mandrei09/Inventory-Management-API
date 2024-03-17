using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class BudgetForecastUI
    {
		public int Id { get; set; }

		//public string Code { get; set; }

  //      public string Name { get; set; }

        public MonthEntity AccMonth { get; set; }

        public decimal Total { get; set; }

        public decimal ValueAsset { get; set; }

        public decimal ValueOrder { get; set; }

        public BudgetBase BudgetBase { get; set; }

        public decimal TotalRem { get; set; }

		public CodeNameEntity Project { get; set; }

        public CodeNameEntity ProjectType { get; set; }

        public CodeNameEntity Region { get; set; }

        public CodeNameEntity AdmCenter { get; set; }

        public CodeNameEntity AppState { get; set; }

        public CodeNameEntity AssetType { get; set; }

		public CodeNameEntity Department { get; set; }

		public CodeNameEntity Division { get; set; }

		public decimal ImportValueOrder { get; set; }

		public bool InTransfer { get; set; }
	}
}
