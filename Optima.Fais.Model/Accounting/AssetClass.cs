using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class AssetClass : Entity
    {
        public AssetClass()
        {
            ChildAssetClasses = new HashSet<AssetClass>();
        }

        public int AssetClassTypeId { get; set; }

        public virtual AssetClassType AssetClassType { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int DepPeriodMin { get; set; }

        public int DepPeriodMax { get; set; }

        public int DepPeriodDefault { get; set; }

        public int? ParentAssetClassId { get; set; }

        public virtual AssetClass ParentAssetClass { get; set; }

        public virtual ICollection<AssetClass> ChildAssetClasses { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
