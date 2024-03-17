using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class InventoryRegionProcentage
	{
		[Key]
		public int Id { get; set; }
		public string RegionName { get; set; }
		public string RegionCode { get; set; }
		public int Total { get; set; }
		public int Scanned { get; set; }
		public decimal Procentage { get; set; }
		public int IsFinished { get; set; }
	}
}