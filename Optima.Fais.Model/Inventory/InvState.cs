namespace Optima.Fais.Model
{
    public class InvState : Entity
    {
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string Name { get; set; }
        public string Mask { get; set; }

        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }

		public string BadgeColor { get; set; }

		public string BadgeIcon { get; set; }
	}
}
