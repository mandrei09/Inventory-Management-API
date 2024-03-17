using System;

namespace Optima.Fais.Dto
{
    public class AssetImportV7
    {
        public string AssetName { get; set; }
        public string InvNo { get; set; }
        public string SerialNumber { get; set; }
        public string AssetCategory { get; set; }
        public string AssetState { get; set; }
        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string EmployeeFullName { get; set; }
        public string CostCenter { get; set; }
        public string Location { get; set; }
        public string Region { get; set; }
        public string Room { get; set; }

    }
}
