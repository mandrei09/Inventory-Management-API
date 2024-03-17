using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class BudgetTotalProcentage
	{
		[Key]
		public int Id { get; set; }
		public decimal Value { get; set; }
		public int Total { get; set; }
		public int Approved { get; set; }
		public int Denied { get; set; }
		public decimal Procentage { get; set; }
		public decimal ValueIni { get; set; }
		public decimal ValueFin { get; set; }
		public double Quantity { get; set; }
		public double QuantityRem { get; set; }
		//public int FinishedAdministration { get; set; }
		//public int TotalAdministration { get; set; }
		//public int TotalTemp { get; set; }
		//public decimal ProcentageAdministration { get; set; }
	}
}