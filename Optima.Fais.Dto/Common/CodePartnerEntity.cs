using System;

namespace Optima.Fais.Dto
{
    public class CodePartnerEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RegistryNumber { get; set; }
        public string ContactInfo { get; set; }
        public string Bank { get; set; }
        public string BankAccount { get; set; }
		public EntityFile[] EntityFiles { get; set; }
	}
}
