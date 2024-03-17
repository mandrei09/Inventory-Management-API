using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto
{
    public class Document : DocumentMainDetail
    {
        //public CodeNameEntity DocumentType { get; set; }

        public CodeNameEntity CostCenter { get; set; }

        //public CodeNameEntity Partner { get; set; }

        public string Details { get; set; }

        public bool? Exported { get; set; }

        public bool? Approved { get; set; }

        public DateTime? ValidationDate { get; set; }
    }
}
