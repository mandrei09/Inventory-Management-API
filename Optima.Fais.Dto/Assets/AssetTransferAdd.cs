namespace Optima.Fais.Dto
{
    public class AssetTransferAdd
    {
        public int[] AssetOldIds { get; set; }
        public int[] AssetNewIds { get; set; }
        public int EmployeeId { get; set; }
        public int InvStateId { get; set; }
    }
}
