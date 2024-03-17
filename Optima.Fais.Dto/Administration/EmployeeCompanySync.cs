using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class EmployeeCompanySync
	{
		public int Id { get; set; }
		public string EmployeeId { get; set; }
		public string CompanyId { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime ModifiedAt { get; set; }
	}
}
