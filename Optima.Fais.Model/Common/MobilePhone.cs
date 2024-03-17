namespace Optima.Fais.Model
{
    public class MobilePhone : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

		public int? EmployeeId { get; set; }

		public virtual Employee Employee { get; set; }
	}
}
