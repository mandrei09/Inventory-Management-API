namespace Optima.Fais.Model
{
    public partial class AssetNature : Entity
    {
        public AssetNature()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? AssetTypeId { get; set; }

        public virtual AssetType AssetType { get; set; }
    }
}
