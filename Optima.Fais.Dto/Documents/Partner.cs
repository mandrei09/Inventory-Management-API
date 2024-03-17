namespace Optima.Fais.Dto
{
    public class Partner : PartnerBase
    {
        public string Address { get; set; }
        public string ContactInfo { get; set; }
        public string Bank { get; set; }
        public string BankAccount { get; set; }
        public string PayingAccount { get; set; }
        public virtual PartnerLocation PartnerLocation { get; set; }
		public EntityFile[] EntityFiles { get; set; }
	}

    public class PartnerViewResource : CodeNameEntity
    {
    }
}
