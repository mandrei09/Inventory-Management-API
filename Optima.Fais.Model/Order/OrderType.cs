using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class OrderType : Entity
    {

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
