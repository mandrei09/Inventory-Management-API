using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class UserStatus
	{
		[Key]
		public int Id { get; set; }
		public int RequestCount { get; set; }
		public int OfferCount { get; set; }
		public int OrderCount { get; set; }
		public int ReceptionCount { get; set; }
	}
}