using System;

namespace Optima.Fais.Dto
{
    public class AssetImportV12
    {
        public string RegionName { get; set; }
        public string AssetType { get; set; }
        public string AssetCategory { get; set; }
        public string InvNo { get; set; }
        public string InvNoParent { get; set; }
        public string AssetName { get; set; }
        public decimal Quantity { get; set; }
        public string Uom { get; set; }
        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }
        public decimal ValueRem { get; set; }
        public DateTime PifDate { get; set; }
        public int TimeLenght { get; set; }
        public string CostCenter { get; set; }
        public string AssetState { get; set; }
        public string EmployeeFull { get; set; }
        public string InternalCode { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string AdministrationCode { get; set; }
        public string AdministrationName { get; set; }
        public string RoomCode { get; set; }
        public string RoomName { get; set; }
        public string AllowLabel { get; set; }
        public string SerialNumber { get; set; }
        public string RegionCode { get; set; }

    }
}
