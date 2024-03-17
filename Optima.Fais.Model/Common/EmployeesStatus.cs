using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class EmployeesStatus
	{
		[Key]
		public int Id { get; set; }
		public int Count { get; set; }
		public int EmailSend { get; set; }
		public int IsConfirmed { get; set; }
		public decimal Procentage { get; set; }
	}
}