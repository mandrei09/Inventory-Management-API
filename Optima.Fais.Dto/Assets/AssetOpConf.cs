using System;

namespace Optima.Fais.Dto
{
    public class AssetOpConf
    {
        public int AssetOpId { get; set; }
        public int Index { get; set; }
        public string InvNo { get; set; }
        public string AssetName { get; set; }
        public string RoomCodeIni { get; set; }
        public string LocationCodeIni { get; set; }
        public string RoomCodeFin { get; set; }
        public string LocationCodeFin { get; set; }
        public string AssetTypeName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SerialNumber { get; set; }
        public float Quantity { get; set; }
        public decimal ValueInv { get; set; }
        public string EmployeeInternalCodeInitial { get; set; }
        public string EmployeeInternalCodeFinal { get; set; }
        public string EmployeeFirstNameInitial { get; set; }
        public string EmployeeFirstNameFinal { get; set; }
        public string EmployeeLastNameInitial { get; set; }
        public string EmployeeLastNameFinal { get; set; }

    }
}
