using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class Storage : Entity
    {
        public Storage()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? PlantId { get; set; }

        public virtual Plant Plant { get; set; }

    }
}
