namespace Optima.Fais.Dto
{
    public class DepartmentDetail : Department
    {
        //public int Id { get; set; }
        //public string Code { get; set; }
        //public string Name { get; set; }
        //public int? TeamLeaderId { get; set; }
        public string InternalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
		public int AssetCount { get; set; }
	}
}
