using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class AssetOperation
    {

        public string Confirm { get; set; }
        public DateTime? OperationDate { get; set; }
        public int DocumentTransferNumber { get; set; }
        public int OperationsCount { get; set; }

        public List<AssetOperationDetail> Operations;

        public List<AssetOperationDetail> OperationsDetails()
        {
            return Operations;
        }
    }
}
