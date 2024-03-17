using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
	public class BudgetForecastCorrectionResult
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public int SourceId { get; set; }
		public int DestinationId { get; set; }
		public int BudgetBaseOpId { get; set; }
	}
}
