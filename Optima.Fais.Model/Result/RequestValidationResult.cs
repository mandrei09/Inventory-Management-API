using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
	public class RequestValidationResult
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public int? RequestId { get; set; }
		public int? RequestBudgetForecastId { get; set; }
	}
}
