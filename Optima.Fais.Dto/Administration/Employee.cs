namespace Optima.Fais.Dto
{
    public class Employee
    {
        public int Id { get; set; }
        public string InternalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? DepartmentId { get; set; }
        public int? CostCenterId { get; set; }
        public int? DivisionId { get; set; }
        public string ErpCode { get; set; }
        public string Email { get; set; }
        public System.DateTime ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
		public bool IsConfirmed { get; set; }
		public int AssetCount { get; set; }
		public CodeNameEntity Department { get; set; }
        public CodeNameEntity CostCenter { get; set; }
        public CodeNameEntity Company { get; set; }
        public EmployeeResource Manager { get; set; }
    }

    public class EmployeeBase
    {
        public int Id { get; set; }
        public string InternalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }
		
	}


    public class EmployeeViewResource : EmployeeBase
    {
        public DepartmentViewResource Department { get; set; }
    }
}
