using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto
{
    public class DocumentMainDetail //: DocumentBase
    {
        public CodeNameEntity DocumentType { get; set; }

        public Partner Partner { get; set; }

        public int Id { get; set; }

        public string DocNo1 { get; set; }

        public string DocNo2 { get; set; }

        public string Details { get; set; }

        public DateTime DocumentDate { get; set; }

        public DateTime RegisterDate { get; set; }

        public DateTime? CreationDate { get; set; }


    }
}
