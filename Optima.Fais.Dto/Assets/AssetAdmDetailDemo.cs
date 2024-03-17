namespace Optima.Fais.Dto
{
    public class AssetAdmBaseDetailDemo
    {
        public CodeNameEntity CostCenter { get; set; }
        public CodeNameEntity AdmCenter { get; set; }

        public CodeNameEntity Room { get; set; }
        public CodeNameEntity Location { get; set; }
        public CodeNameEntity Region { get; set; }

        public EmployeeResource Employee { get; set; }
        public CodeNameEntity Department { get; set; }
    }

    public class AssetAdmDetailDemo : AssetAdmBaseDetailDemo
    {
        public CodeNameEntity AssetType { get; set; }
        public CodeNameEntity AssetState { get; set; }
        public CodeNameEntity AssetCategory { get; set; }
        public CodeNameEntity Administration { get; set; }
    }

    public class AssetAdmBaseViewResource
    {
        public RoomViewResource Room { get; set; }
        public EmployeeViewResource Employee { get; set; }
        public CostCenterViewResource CostCenter { get; set; }
    }
}
