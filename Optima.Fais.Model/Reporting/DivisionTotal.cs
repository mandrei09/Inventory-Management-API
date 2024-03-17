using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class DivisionTotal
	{
		[Key]
		public int Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public int Initial { get; set; }
		public int Scanned { get; set; }
		public decimal Procentage { get; set; }
		public decimal ValueInv { get; set; }
		public decimal ValueRem { get; set; }
		public DateTime? LastScan { get; set; }

	}
}