namespace Optima.Fais.Model
{
    public partial class AssetAC
    {
        public int AssetId { get; set; }

        public virtual Asset Asset { get; set; }

        public int AssetClassTypeId { get; set; }

        public virtual AssetClassType AssetClassType { get; set; }

        public int AssetClassIdIn { get; set; }

        public virtual AssetClass AssetClassIn { get; set; }

        public int AssetClassId { get; set; }

        public virtual AssetClass AssetClass { get; set; }
    }
}
