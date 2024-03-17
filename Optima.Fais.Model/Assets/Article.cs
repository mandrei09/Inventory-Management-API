namespace Optima.Fais.Model
{
    public partial class Article : Entity
    {
        public Article()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
