using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class DocumentUpload
    {
        public int Id { get; set; }

        public int DocumentTypeId { get; set; }

        public string DocNo1 { get; set; }

        public string DocNo2 { get; set; }

        public DateTime DocumentDate { get; set; }

        public DateTime RegisterDate { get; set; }

        public Boolean Validated { get; set; }

        public string Details { get; set; }

        public string SerialNumber { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public List<OperationUpload> Operations { get; set; }

        public int? OperationEmpId { get; set; }
    }
}
