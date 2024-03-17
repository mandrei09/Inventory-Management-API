namespace Optima.Fais.Dto
{
    public class Department
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? TeamLeaderId { get; set; }
		public bool AssetCount { get; set; }
		//public string TeamLeader { get; set; }
	}

    public class DepartmentViewResource : CodeNameEntity
    {
    }
}
