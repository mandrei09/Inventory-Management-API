using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class BudgetDashboardTable
    {
		[Key]
		public int Id { get; set; }
		public decimal Total { get; set; }
		public decimal ValueYTG { get; set; }
		public decimal ValueYTD { get; set; }
		public decimal TotalRem { get; set; }
		public decimal ValueAsset { get; set; }
		public decimal? ValueRon { get; set; }

		public int TotalCount { get; set; }
		public string Prnubmer { get; set; }
		public string ProjectCode { get; set; }
        public string ProjectName { get; set; }

    }
}