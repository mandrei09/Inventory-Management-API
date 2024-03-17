using System;

namespace Optima.Fais.Dto
{
    public class AssetImportV10
    {
        public string InvNo { get; set; }
        public string InvNoParent { get; set; }
        public string IsParent { get; set; }
        public string Description { get; set; }
        public string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal ValueInv { get; set; }
        public float QuantityInitial { get; set; }
        public float QuantityFinal { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
        public string LocationCodeInitial { get; set; }
        public string LocationCodeFinal { get; set; }
        public string DocumentName { get; set; }
        public DateTime DocumentDate { get; set; }
        public string LocationNameInitial { get; set; }
        public string LocationNameFinal { get; set; }
        public string IsTransfer { get; set; }
        public string ReconcileInvNo { get; set; }
        public string AssetType { get; set; }
        public string allowLabel { get; set; }
        public string Info { get; set; }

    }
}
