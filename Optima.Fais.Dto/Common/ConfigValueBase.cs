namespace Optima.Fais.Dto
{
    public class ConfigValueBase
    {
        public string Group { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string ValueType { get; set; }

        public string TextValue { get; set; }

        public bool? BoolValue { get; set; }

        public System.DateTime? DateValue { get; set; }

        public decimal? NumericValue { get; set; }

        public int? CompanyId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
