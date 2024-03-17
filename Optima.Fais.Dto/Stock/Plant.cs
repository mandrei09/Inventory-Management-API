using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class Plant
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime? ModifiedAt { get; set; }
    }
}
