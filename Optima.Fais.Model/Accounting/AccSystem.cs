namespace Optima.Fais.Model
{
    public partial class AccSystem : Entity
    {
        public AccSystem()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int AssetClassTypeId { get; set; }

        public virtual AssetClassType AssetClassType { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
