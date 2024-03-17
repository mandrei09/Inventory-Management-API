namespace Optima.Fais.Model
{
    public partial class MaterialType : Entity
    {
        public MaterialType()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
