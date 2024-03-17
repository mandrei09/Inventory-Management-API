using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class Country : Entity
    {
        public Country()
        {
            Counties = new HashSet<County>();
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public virtual ICollection<County> Counties { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
