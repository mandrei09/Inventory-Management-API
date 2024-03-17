using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class Contract
    {
		public int Id { get; set; }

        public string Title { get; set; }

        public string ContractID { get; set; }

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

        public EmployeeResource Owner { get; set; }

        public CodePartnerEntity Partner { get; set; }

        public CodeNameEntity BusinessSystem { get; set; }

        public ContractAmountEntity ContractAmount { get; set; }

        public AppState AppState { get; set; }

        public int? AppStateId { get; set; }

        public string Code { get; set; }
	}
}
