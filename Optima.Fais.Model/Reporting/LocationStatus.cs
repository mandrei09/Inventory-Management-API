using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class LocationStatus
	{
		[Key]
		public int Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public int Value { get; set; }
	}
}