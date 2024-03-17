using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class CompanyDynamicGroupMonth
	{
		[Key]
		public long Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public decimal Value { get; set; }
		public decimal ValueDep { get; set; }
		public string Month { get; set; }
	}

}