using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class DivisionReport
	{
		[Key]
		public int Id { get; set; }
		public string InvNo { get; set; }
		public string Name { get; set; }
		public string SerialNumber { get; set; }
		public string Division { get; set; }
		//public string Room { get; set; }
		//public string Employee { get; set; }
		public DateTime PurchaseDate { get; set; }
	}
}