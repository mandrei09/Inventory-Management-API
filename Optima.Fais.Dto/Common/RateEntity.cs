using System;

namespace Optima.Fais.Dto
{
    public class RateEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Multiplier { get; set; }
    }
}
