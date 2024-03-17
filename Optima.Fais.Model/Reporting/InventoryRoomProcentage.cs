using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class InventoryRoomProcentage
	{
		[Key]
		public int Id { get; set; }
		public string AdministrationName { get; set; }
		public string AdministrationCode { get; set; }
		public int Total { get; set; }
		public int Scanned { get; set; }
		public decimal Procentage { get; set; }
		public int IsFinished { get; set; }
	}
}