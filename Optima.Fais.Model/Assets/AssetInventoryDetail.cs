namespace Optima.Fais.Model
{
    public class AssetInventoryDetail
    {
        public Asset Asset { get; set; }
        public InventoryAsset Inventory { get; set; }
        public AssetDepMD Dep { get; set; }
    }
}
