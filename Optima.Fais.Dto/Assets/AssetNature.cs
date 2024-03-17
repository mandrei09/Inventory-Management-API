namespace Optima.Fais.Dto
{
    public class AssetNature
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? AssetTypeId { get; set; }
        public CodeNameEntity AssetType { get; set; }
        public System.DateTime ModifiedAt { get; set; }


    }

    public class AssetNatureBase
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class AssetNatureViewResource : AssetNatureBase
    {
        public AssetTypeViewResource AssetType { get; set; }
    }
}
