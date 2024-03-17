using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class City : Entity
    {
        public City()
        {
            Locations = new HashSet<Location>();
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Location> Locations { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? CountyId { get; set; }

        public virtual County County { get; set; }
    }
}
