﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class BudgetBaseUI
    {
		public int Id { get; set; }

		public string Code { get; set; }

        public string Name { get; set; }

        //public int? CompanyId { get; set; }

        public CodeNameEntity Company { get; set; }

        //public int? ProjectId { get; set; }

        public CodeNameEntity Project { get; set; }

        public CodeNameEntity Uom { get; set; }

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

        //public EmployeeResource Employee { get; set; }

        //public int? AccMonthId { get; set; }

        //public MonthEntity AccMonth { get; set; }

        //public int? InterCompanyId { get; set; }

        //public CodeNameEntity InterCompany { get; set; }

        //public int? PartnerId { get; set; }

        //public CodePartnerEntity Partner { get; set; }

        //public int? AccountId { get; set; }

        //public CodeNameEntity Account { get; set; }

        public int? AppStateId { get; set; }

        public CodeNameEntity AppState { get; set; }

        //public DateTime? StartDate { get; set; }

        //public DateTime? EndDate { get; set; }

        //public bool Validated { get; set; }

        public float Quantity { get; set; }

        public float QuantityRem { get; set; }

        public float QuantityUsed { get; set; }

        public float QuantityOrder { get; set; }

        public decimal ValueIni { get; set; }

        public decimal ValueFin { get; set; }

        public decimal ValueUsed { get; set; }

        public decimal ValueOrder { get; set; }

        //public int? BudgetManagerId { get; set; }

        //public CodeNameEntity BudgetManager { get; set; }

        //public bool IsAccepted { get; set; }

        //public string Info { get; set; }

        //public string InfoMinus { get; set; }

        //public bool IsDeleted { get; set; }

        public CodeNameEntity Region { get; set; }

        public CodeNameEntity AdmCenter { get; set; }

        public CodeNameEntity AssetType { get; set; }

		public BudgetMonthBase BudgetMonthBase { get; set; }
	}
}