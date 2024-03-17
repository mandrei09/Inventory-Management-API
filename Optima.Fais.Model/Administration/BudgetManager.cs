namespace Optima.Fais.Model
{
    public partial class BudgetManager : Entity
    {
        public BudgetManager()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? UomId { get; set; }

        public virtual Uom Uom { get; set; }

		public int? ERPId { get; set; }
	}
}
