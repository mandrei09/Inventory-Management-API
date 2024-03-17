using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
	public class ExpAccountGroupBase
	{
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
