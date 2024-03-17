using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
	public class OrderResult
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public int OrderId { get; set; }
	}
}
