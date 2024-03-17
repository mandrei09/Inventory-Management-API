using System;

namespace Optima.Fais.Dto
{
    public class AssetImportV9
    {
        public string ERPCode { get; set; }
        public string InvNo { get; set; }
        public string Name { get; set; }
        public string AssetCategory { get; set; }
        public string LocationCodeInitial { get; set; }
        public string LocationNameInitial { get; set; }
        public string RoomCodeInitial { get; set; }
        public string RoomNameInitial { get; set; }
        public string InternalCodeInitial { get; set; }
        public string EmployeeNameInitial { get; set; }
        public string LocationCodeFinal { get; set; }
        public string LocationNameFinal { get; set; }
        public string RoomCodeFinal { get; set; }
        public string RoomNameFinal { get; set; }
        public string InternalCodeFinal { get; set; }
        public string EmployeeNameFinal { get; set; }
        public string DocNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal ValueInv { get; set; }
        public string SerialNumber { get; set; }
        public string AssetStateInitial { get; set; }
        public string AssetStateFinal { get; set; }
    }
}
