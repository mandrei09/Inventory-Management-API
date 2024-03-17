using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class AssetOperationDetail
    {
        public string Description { get; set; }
        public string InvNo { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string CostCenterCodeInitial { get; set; }
        public string CostCenterNameInitial { get; set; }
        public string LocationCodeInitial { get; set; }
        public string LocationNameInitial { get; set; }
        public string CostCenterCodeFinal { get; set; }
        public string CostCenterNameFinal { get; set; }
        public string LocationCodeFinal { get; set; }
        public string LocationNameFinal { get; set; }
        public DateTime? OperationDate { get; set; }
        public int AssetOpId { get; set; }
        public int TransferDocumentId { get; set; }

    }
}
