using System;

namespace Optima.Fais.Dto
{
    public class AssetImportV1
    {
        public string InvNo1 { get; set; }
        public string InvNo2 { get; set; }
        public string InvNo3 { get; set; }
        public string AssetCategoryCode { get; set; }
        public string AssetCategoryName { get; set; }
        public int Quantity { get; set; }
        public string LocationCode { get; set; }
        public string CostCenterCode { get; set; }
        public string AssetName { get; set; }
        public string AssetState { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int DepPeriod { get; set; }
        public int DepPeriodDays { get; set; }
        public decimal ValueInv { get; set; }
        public string PartnerName { get; set; }
        public string FiscalCode { get; set; }
        public string DocNo1 { get; set; }
        public string SerialNumber { get; set; }
        public string AssetType { get; set; }
        public decimal ValueRem { get; set; }
        //public bool Validated { get; set; }
        //public string RoomName { get; set; }
        //public string Uom { get; set; }
        //public string InternalCode { get; set; }
        //public string EmployeeFullName { get; set; }
        //public int DeprecationValue { get; set; }
    }
}
