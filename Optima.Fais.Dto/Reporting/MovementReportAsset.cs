using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class MovementReportAsset
    {
        public string Name { get; set; }
        public string InvNo { get; set; }
        public string Barcode { get; set; }
        public string SAPnumber { get; set; }
        public string HolderName { get; set; }
        public string SourceDocumentDate { get; set; }
        public string SourceDocumentNumber { get; set; }
        public string OldSegment { get; set; }
        public string NewSegment { get; set; }
        public string OldAdministration { get; set; }
        public string NewAdministration { get; set; }
        public string OldCostCenter { get; set; }
        public string NewCostCenter { get; set; }

    }
}
