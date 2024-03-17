namespace Optima.Fais.Dto
{
    public class Division
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
		public CodeNameEntity Department { get; set; }
		public int DepartmentId { get; set; }
		//public CodeNameEntity Location { get; set; }
		//public int LocationId { get; set; }
		public int AssetCount { get; set; }

		public System.DateTime ModifiedAt { get; set; }
    }
}
