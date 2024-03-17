namespace Optima.Fais.Model
{
    public partial class LocationType : Entity
    {
        public LocationType()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
