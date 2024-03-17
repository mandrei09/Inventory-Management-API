namespace Optima.Fais.Model
{
    public class AssetCategory : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Prefix { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
