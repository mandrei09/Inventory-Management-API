using System;

namespace Optima.Fais.Dto
{
    public class AssetImportV5
    {
        public string AssetName { get; set; }
        public string SerialNumber { get; set; }
        public string InvNo { get; set; }
        public string AssetCategory { get; set; }
        public string AssetState { get; set; }
        public float ValueInv { get; set; }
        public float ValueRem { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string EmployeeFullName { get; set; }
        public string CostCenterName { get; set; }
        public string RegionName { get; set; }
        public string LocationName { get; set; }
      
     
    }
}
