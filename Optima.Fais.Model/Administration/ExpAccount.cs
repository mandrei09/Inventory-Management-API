namespace Optima.Fais.Model
{
    public partial class    ExpAccount : Entity
    {
        public ExpAccount()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public string Description { get; set; }

		public bool RequireSN { get; set; }
	}
}
