namespace Optima.Fais.Dto
{
    public class Matrix
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
  //      public int? AssetTypeId { get; set; }
		//public CodeNameEntity AssetType { get; set; }
  //      public int? AreaId { get; set; }
  //      public CodeNameEntity Area { get; set; }
  //      public int? CountryId { get; set; }
  //      public CodeNameEntity Country { get; set; }
  //      public int? CompanyId { get; set; }
        public CodeNameEntity Company { get; set; }
        //public int? CostCenterId { get; set; }
        //public CodeNameEntity CostCenter { get; set; }
        public CodeNameEntity Division { get; set; }
        public CodeNameEntity Department { get; set; }
		//public CodeNameEntity AdmCenter { get; set; }
		//public int? ProjectId { get; set; }
		//public CodeNameEntity Project { get; set; }
		//public CodeNameEntity ProjectType { get; set; }
		public EmployeeResource EmployeeB1 { get; set; }
		public EmployeeResource EmployeeL1 { get; set; }
        public EmployeeResource EmployeeL2 { get; set; }
        public EmployeeResource EmployeeL3 { get; set; }
        public EmployeeResource EmployeeL4 { get; set; }
        public EmployeeResource EmployeeS1 { get; set; }
        public EmployeeResource EmployeeS2 { get; set; }
        public EmployeeResource EmployeeS3 { get; set; }
		public decimal AmountL1 { get; set; }
        public decimal AmountL2 { get; set; }
        public decimal AmountL3 { get; set; }
        public decimal AmountL4 { get; set; }
        public decimal AmountS1 { get; set; }
        public decimal AmountS2 { get; set; }
        public decimal AmountS3 { get; set; }
	}
}
