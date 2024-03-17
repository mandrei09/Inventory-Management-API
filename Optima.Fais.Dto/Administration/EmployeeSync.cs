using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class EmployeeSync
	{
		public int Id { get; set; }
		public string InternalCode { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ErpCode { get; set; }
		public string Email { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime ModifiedAt { get; set; }
	}
}
