using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class Contract : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string ContractId { get; set; }

        public string Description { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? AgreementDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime? CreationDate { get; set; }

        public string Version { get; set; }

        public string TemplateId { get; set; }

        public string AmendmentType { get; set; }

        public string AmendmentReason { get; set; }

        public int Origin { get; set; }

        public string HierarchicalType { get; set; }

        public string ExpirationTermType { get; set; }

        public string RelatedId { get; set; }

        public int MaximumNumberOfRenewals { get; set; }

        public int AutoRenewalInterval { get; set; }

		public bool IsTestProject { get; set; }

        public virtual ICollection<Commodity> Commodities { get; set; }

        public virtual ICollection<ContractRegion> ContractRegions { get; set; }

        public virtual ICollection<ContractDivision> ContractDivisions { get; set; }

        public int? EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        public int? OwnerId { get; set; }

        public virtual Owner Owner { get; set; }

        public int? PartnerId { get; set; }

        public virtual Partner Partner { get; set; }

        public int? BusinessSystemId { get; set; }

        public virtual BusinessSystem BusinessSystem { get; set; }

        public int? ContractAmountId { get; set; }

        public virtual ContractAmount ContractAmount { get; set; }

        public bool Validated { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public int? AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public bool IsAccepted { get; set; }

        public string Info { get; set; }

        public bool IsEnabled { get; set; }

        public decimal PaymentTerms { get; set; }
    }
}
