namespace Optima.Fais.Model
{
    public partial class ConfigValue : Entity
    {
        public ConfigValue()
        {
        }

        public string Group { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string ValueType { get; set; }

        public string TextValue { get; set; }

        public bool? BoolValue { get; set; }

        public System.DateTime? DateValue { get; set; }

        public decimal? NumericValue { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public string RoleId { get; set; }

        public ApplicationRole AspNetRole { get; set; }
    }
}
