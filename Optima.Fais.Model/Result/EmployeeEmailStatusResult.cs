using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
	public class EmployeeEmailStatusResult
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public int? EntityId { get; set; }
	}
}
