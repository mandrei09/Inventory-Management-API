using System;

namespace Optima.Fais.Dto
{
    public class AssetImportV3
    {
        public string InvNo { get; set; }
        public string Name { get; set; }

        public string AssetType { get; set; }
        public string AssetCategory { get; set; }
        public string AssetState { get; set; }

        public string RoomCode { get; set; }
        public string RoomName { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }

        public string InternalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public decimal ValueInv { get; set; }
        public decimal ValueRem { get; set; }
        public float Quantity { get; set; }
    }
}
