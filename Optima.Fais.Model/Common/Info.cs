using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class Info : Entity
    {
      
        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? InfoTypeId { get; set; }

        public InfoType InfoType { get; set; }
    }
}
