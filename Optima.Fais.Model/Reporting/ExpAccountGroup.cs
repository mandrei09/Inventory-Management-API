using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class ExpAccountGroup
	{
		[Key]
		public int Id { get; set; }
		public string ExpAccount { get; set; }
		public decimal ApcfyStart { get; set; }
		public decimal DepFYStart { get; set; }
		public decimal BkValFYStart { get; set; }
		public decimal Acquisition { get; set; }
		public decimal DepForYear { get; set; }
		public decimal Retirement { get; set; }
		public decimal DepRetirement { get; set; }
		public decimal CurrBkValue { get; set; }
		public decimal CurrentAPC { get; set; }
		public decimal AccumulDep { get; set; }
	}

}