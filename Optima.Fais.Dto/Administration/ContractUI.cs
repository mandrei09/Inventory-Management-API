using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class ContractUI
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ContractID { get; set; }

        public EmployeeResource Owner { get; set; }

        public CodePartnerEntity Partner { get; set; }

        public CodeNameEntity BusinessSystem { get; set; }

        public ContractAmountEntity ContractAmount { get; set; }

        //public int? CompanyId { get; set; }

        // public CodeNameEntity Company { get; set; }

        //public int? AdministrationId { get; set; }

        //public CodeNameEntity Administration { get; set; }

        //public int? CostCenterId { get; set; }

        //public CodeNameEntity CostCenter { get; set; }

        //public int? SubTypeId { get; set; }

        //public CodeNameEntity MasterType { get; set; }

        //public CodeNameEntity Type { get; set; }

        //public CodeNameEntity SubType { get; set; }

        //[MaxLength(450)]
        //public string UserId { get; set; }

        //public ApplicationUser User { get; set; }

        //public int? EmployeeId { get; set; }

        // public EmployeeResource Employee { get; set; }

        //public int? AccMonthId { get; set; }

        //public MonthEntity AccMonth { get; set; }

        //public int? InterCompanyId { get; set; }

        //public CodeNameEntity InterCompany { get; set; }

        //public int? PartnerId { get; set; }

        // public CodePartnerEntity Partner { get; set; }

        //

        //public CodeNameEntity Account { get; set; }

        public int? AppStateId { get; set; }

        public CodeNameEntity AppState { get; set; }
    }
}
