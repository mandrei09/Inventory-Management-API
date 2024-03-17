namespace Optima.Fais.Dto
{
    public class AssetClass
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public int AssetClassTypeId { get; set; }
        public int? ParentAssetClassId { get; set; }

        public int DepPeriodMin { get; set; }
        public int DepPeriodMax { get; set; }
        public int DepPeriodDefault { get; set; }
    }
}
