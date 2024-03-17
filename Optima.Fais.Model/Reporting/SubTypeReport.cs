using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class SubTypeReport
	{
		[Key]
		public int Id { get; set; }
		public string Code { get; set; }
		public int Total { get; set; }
		
	}
}
