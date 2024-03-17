using System;

namespace Optima.Fais.Dto
{
    public class AssetImportV2
    {
        public string InvNo { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string CostCenterCode { get; set; }
        public string AdmCenterName { get; set; }
        //public string LocationName { get; set; }
        public string RoomName { get; set; }
        public string PurchaseDate { get; set; }
        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }
        public string Uom { get; set; }
        public float Quantity { get; set; }
        public string Custody { get; set; }
        public string InternalCode { get; set; }
        public string EmployeeFullName { get; set; }
    }
}
