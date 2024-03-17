using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class InventoryPieChartByDay
	{
		[Key]
		public int Id { get; set; }
		public DateTime? ScanDate { get; set; }
		public int Scanned { get; set; }
	}
}