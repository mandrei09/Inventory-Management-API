using System;

namespace Optima.Fais.Dto
{
    public class ImportITMFX
    {
		public string ERPCode { get; set; }
		public string Description { get; set; }
		public string CostCenterCode { get; set; }
		public string CostCenter { get; set; }
		public string AdmCenter { get; set; }
		public string Material { get; set; }
		public string MaterialUnique { get; set; }
		public string SerialNumber { get; set; }
		public string InternalCode { get; set; }
		public string Email { get; set; }
		public string Project { get; set; }
		public string BudgetBase { get; set; }
		public int? EmployeeId { get; set; }
	}
}
