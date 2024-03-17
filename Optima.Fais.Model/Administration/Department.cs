namespace Optima.Fais.Model
{
    public partial class Department : Entity
    {
        public Department()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? TeamLeaderId { get; set; }

        public virtual Employee TeamLeader { get; set; }

        public string ERPCode { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? ActivityId { get; set; }

        public virtual Activity Activity { get; set; }

		public int AssetCount { get; set; }
	}
}
