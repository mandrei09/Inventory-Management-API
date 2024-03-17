using System;

namespace Optima.Fais.Dto
{
    public class OfferSave
    {
        public int Id { get; set; }
        //public string Code { get; set; }
        //public string Name { get; set; }
        //public int? CompanyId { get; set; }
        //public int? ProjectId { get; set; }
        //public int? AdministrationId { get; set; }
        //public int? MasterTypeId { get; set; }
        //public int? TypeId { get; set; }
        //public int? SubTypeId { get; set; }
        public int? EmployeeId { get; set; }
        //public int? AccMonthId { get; set; }
        //public int? InterCompanyId { get; set; }
        //public int? PartnerId { get; set; }
        //public int? AccountId { get; set; }
        //public int? CostCenterId { get; set; }
        //public decimal ValueIni { get; set; }
        //public decimal ValueFin { get; set; }
        //public float Quantity { get; set; }
        //public string Info { get; set; }
        //public bool Validated { get; set; }
		public string UserId { get; set; }
        //public int? AdmCenterId { get; set; }
        //public int? RegionId { get; set; }
        //public int? AssetTypeId { get; set; }
        //public int? ProjectTypeId { get; set; }
        //public int? BudgetBaseId { get; set; }
        // public int? BudgetForecastId { get; set; }
        public int RequestId { get; set; }
        public int? OfferCloneId { get; set; }
        //public DateTime? StartDate { get; set; }
        //public DateTime? EndDate { get; set; }
        public CodePartnerEntity[] PartnerIds { get; set; }
		public int OfferTypeId { get; set; }
		public Guid Guid { get; set; }

	}
}
