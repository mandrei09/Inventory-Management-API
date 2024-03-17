using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{

    public class AssetInventoryBaseDetail
    {
        public AssetInvDetail Inv { get; set; }
        public AssetAdmBaseViewResource Adm { get; set; }
    }

    public class AssetInventoryDetail
    {
        public AssetInventoryBaseDetail Initial { get; set; }
        public AssetInventoryBaseDetail Final { get; set; }
        public EmployeeResource ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? ScanDate { get; set; }
    }
}
