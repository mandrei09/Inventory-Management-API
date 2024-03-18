namespace Optima.Fais.Model
{
    public class Partner : Entity
    {
        public string Name { get; set; }

        public string FiscalCode { get; set; }

        public string RegistryNumber { get; set; }

        public string Address { get; set; }

        public string ContactInfo { get; set; }

        public string Bank { get; set; }

        public string BankAccount { get; set; }

        public string PayingAccount { get; set; }

        public string ErpCode { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? PartnerLocationId { get; set; }

        public virtual PartnerLocation PartnerLocation { get; set; }

		public int? ERPId { get; set; }

        public int? SubTypeId { get; set; }

        public virtual SubType SubType { get; set; }

        public int? AddressDetailId { get; set; }

        public virtual Address AddressDetail { get; set; }
    }
}
