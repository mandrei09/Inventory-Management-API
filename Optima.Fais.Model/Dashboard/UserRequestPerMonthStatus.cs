using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class UserRequestPerMonthStatus
	{
		[Key]
		public long Id { get; set; }
		public int Count { get; set; }
		public string Month { get; set; }
	}
}