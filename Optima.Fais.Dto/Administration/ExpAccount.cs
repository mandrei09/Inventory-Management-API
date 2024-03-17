using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class ExpAccount
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool RequireSN { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
