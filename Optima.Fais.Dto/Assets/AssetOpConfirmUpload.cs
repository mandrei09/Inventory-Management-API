namespace Optima.Fais.Dto
{
    public class AssetOpConfirmUpload
    {
        public int AssetOpId { get; set; }
        public int Id { get; set; }
        public string InvNo { get; set; }
        public string Name { get; set; }
        public float Quantity { get; set; }
        public decimal ValueInv { get; set; }
        public string LocationCodeInitial { get; set; }
        public string LocationNameInitial { get; set; }
        public string RoomCodeInitial { get; set; }
        public string RoomNameInitial { get; set; }
        public string EmployeeFullNameInitial { get; set; }
        public string EmployeeInternalCodeInitial { get; set; }
        public string LocationCodeFinal { get; set; }
        public string LocationNameFinal { get; set; }
        public string RoomCodeFinal { get; set; }
        public string RoomNameFinal { get; set; }
        public string EmployeeFullNameFinal { get; set; }
        public string EmployeeInternalCodeFinal { get; set; }
        public string CostCenterCodeInitial { get; set; }
        public string CostCenterNameInitial { get; set; }
        public string CostCenterCodeFinal { get; set; }
        public string CostCenterNameFinal { get; set; }
        public string Confirm { get; set; }
    }
}
