using System;

namespace Optima.Fais.Dto
{
    public class ContractAmountEntity
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }

        public decimal AmountRon { get; set; }

        public decimal AmountRem { get; set; }

        public decimal AmountRonRem { get; set; }

        public decimal AmountUsed { get; set; }

        public decimal AmountRonUsed { get; set; }
        public CodeNameEntity Uom { get; set; }
        public RateEntity Rate { get; set; }
        public RateEntity RateRon { get; set; }
		public string ContractId { get; set; }
        public string Title { get; set; }
    }
}
