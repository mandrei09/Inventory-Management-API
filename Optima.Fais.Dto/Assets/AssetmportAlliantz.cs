using System;

namespace Optima.Fais.Dto
{
    public class AssetImportAlliantz
    {
        public string AssetName { get; set; }
        public string InvNo { get; set; }
        public string ErpCode { get; set; }
        public string AssetState { get; set; }
        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }
        public string EmployeeFull { get; set; }
        public string CostCenter { get; set; }
        public string AssetCategory { get; set; }
        public string Location { get; set; }
        public string Room { get; set; }
        public string Region { get; set; }
        public string Administration { get; set; }
        public string Division { get; set; }
        public DateTime? PIFDate { get; set; }
    }
}
