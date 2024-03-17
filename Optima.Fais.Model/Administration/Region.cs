using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class Region : Entity
    {
        public Region()
        {
            Locations = new HashSet<Location>();
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Location> Locations { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

		public int AssetCount { get; set; }
	}
}
