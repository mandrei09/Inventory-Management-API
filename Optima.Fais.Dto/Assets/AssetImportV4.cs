using System;

namespace Optima.Fais.Dto
{
    public class AssetImportV4
    {
        public string AssetName { get; set; }
        public string InvNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string LocationName { get; set; }
        public string EmployeeFullName { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
     
    }
}
