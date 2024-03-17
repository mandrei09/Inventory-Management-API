namespace Optima.Fais.Model
{
    public partial class Account : Entity
    {
        public Account()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
