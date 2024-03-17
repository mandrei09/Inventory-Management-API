using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class County : Entity
    {
        public County()
        {
            Cities = new HashSet<City>();
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

    }
}
