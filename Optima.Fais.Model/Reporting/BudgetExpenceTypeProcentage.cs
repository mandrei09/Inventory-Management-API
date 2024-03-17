using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class BudgetExpenceTypeProcentage
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
		public int Total { get; set; }
		public int Approved { get; set; }
		public int Denied { get; set; }
		public int Waiting { get; set; }
		public decimal Procentage { get; set; }
		public decimal ValueIni { get; set; }
		public decimal ValueFin { get; set; }
		public double Quantity { get; set; }
		public double QuantityRem { get; set; }

	}
}