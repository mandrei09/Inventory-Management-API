namespace Optima.Fais.Dto
{
    public class EmployeeResource
    {
        public int Id { get; set; }
        public string InternalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ErpCode { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
		public bool Validate { get; set; }

        public bool IsConfirmed { get; set; }
        public CodeNameEntity Company { get; set; }
		public CodeNameEntity CostCenter { get; set; }
		public CodeNameEntity Division { get; set; }
		public CodeNameEntity Department { get; set; }
		public EmployeeResource Manager { get; set; }
	}
}
