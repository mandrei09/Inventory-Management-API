using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto
{
    public class DocumentType
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string ParentCode { get; set; }

        public string Mask { get; set; }

        public bool IsActive { get; set; }

        public string Prefix { get; set; }

        public string Suffix { get; set; }
    }
}
