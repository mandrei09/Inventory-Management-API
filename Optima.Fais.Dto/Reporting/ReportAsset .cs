using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class ReportAsset
    {
        public string Name { get; set; }
        public string InvNo { get; set; }
        public int Quantity { get; set; }
        public float Value { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string RoomCode { get; set; }
        public string RoomName { get; set; }
        public string SerialNumber { get; set; }
        public string DocumentName { get; set; }
        public DateTime? DocumentDate { get; set; }

        public List<ReportAsset> ReportAssets;
    }
}
