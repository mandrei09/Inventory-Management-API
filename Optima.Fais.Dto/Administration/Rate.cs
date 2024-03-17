namespace Optima.Fais.Dto
{
    public class Rate
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Value { get; set; }
        public decimal Multiplier { get; set; }
        public CodeNameEntity Uom { get; set; }
    }
}
