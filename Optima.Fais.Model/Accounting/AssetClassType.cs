using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class AssetClassType : Entity
    {
        public AssetClassType()
        {
            AssetClasses = new HashSet<AssetClass>();
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<AssetClass> AssetClasses { get; set; }
    }
}
