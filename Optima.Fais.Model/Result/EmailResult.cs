using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
	public class EmailResult
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public string Subject { get; set; }
		public string To { get; set; }
		public string Cc { get; set; }
		public string Body { get; set; }
		public int EmailManagerId { get; set; }
	}
}
