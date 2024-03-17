using System;

namespace Optima.Fais.Dto
{
    public class AssetImportV8
    {
        public string InvNo { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public float Quantity { get; set; }
        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }
        public decimal ValueCassation { get; set; }
        public decimal MonthCassationRate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string RoomCode { get; set; }
        public string RoomName { get; set; }
        public string InternalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AssetCategoryCode { get; set; }
        public string AssetCategoryName { get; set; }
        public string AssetClassCode { get; set; }
        public string AssetClassName { get; set; }
        public decimal Years { get; set; }
        public decimal Months { get; set; }
        public string InvNoParent { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
        public string AssetType { get; set; }
        public string AssetState { get; set; }
        public string BookRate { get; set; }
        public DateTime ChangeDate { get; set; }
        public string TransactionTypeCode { get; set; }
        public string TransactionType { get; set; }
        public int QuantityChange { get; set; }
        public decimal ValueChange { get; set; }
        public string AssetIdUnique { get; set; }






        //public string SupplierCode { get; set; }
        //public string SupplierName { get; set; }







    }
}
