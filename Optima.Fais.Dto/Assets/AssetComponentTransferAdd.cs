namespace Optima.Fais.Dto
{
    public class AssetComponentTransferAdd
    {
        public int[] AssetComponentOldIds { get; set; }
        public int[] AssetComponentNewIds { get; set; }
        public int EmployeeId { get; set; }
        public int InvStateId { get; set; }
    }
}
