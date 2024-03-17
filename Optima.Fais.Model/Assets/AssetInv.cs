namespace Optima.Fais.Model
{
    public class AssetInv // : Asset
    {
        public int AssetId { get; set; }

        public virtual Asset Asset { get; set; }

        public string InvNoOld { get; set; }

        public string InvName { get; set; }

        public string Barcode { get; set; }

        public string Producer { get; set; }

        public string Model { get; set; }

        public bool? AllowLabel { get; set; }

        public string Info { get; set; }

        public int InvStateId { get; set; }
        public virtual InvState InvState { get; set; }

    }
}
