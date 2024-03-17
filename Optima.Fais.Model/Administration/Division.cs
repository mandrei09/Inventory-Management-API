using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class Division : Entity
    {
        public Division()
        {
            Administrations = new HashSet<Administration>();
        }


        public string Code { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Administration> Administrations { get; set; }

        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public int? LocationId { get; set; }

        public virtual Location Location { get; set; }

        public int? ActivityId { get; set; }

        public virtual Activity Activity { get; set; }

		//public int? CompanyId { get; set; }

		//public virtual Company Company { get; set; }

		public int AssetCount { get; set; }
	}
}
