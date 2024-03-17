namespace Optima.Fais.Dto
{
    public class AssetComponent
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public int? AssetId { get; set; }
        public AssetEntity Asset { get; set; }
        public int? EmployeeId { get; set; }
        public EmployeeResource Employee { get; set; }
        public CodeNameEntity SubType { get; set; }
        public CodeNameEntity Type { get; set; }
        public CodeNameEntity MasterType { get; set; }
        public CodeNameEntity InvState { get; set; }
        public bool IsAccepted { get; set; }
        public string ImageName { get; set; }
    }
}
