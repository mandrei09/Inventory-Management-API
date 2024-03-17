using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class BudgetStatus
	{
		[Key]
		public int Id { get; set; }
		public string User { get; set; }
		public DateTime Activity { get; set; }
		public decimal ValueIni { get; set; }
		public decimal ValueFin { get; set; }
		public decimal Procentage { get; set; }
	}
}