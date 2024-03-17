namespace Optima.Fais.Model
{
    public partial class Uom : Entity
    {
        public Uom()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

		public int? PartnerLocationId { get; set; }

		public virtual PartnerLocation PartnerLocation { get; set; }

		public int? ERPId { get; set; }

		public int? ERPInitialId { get; set; }
	}
}
