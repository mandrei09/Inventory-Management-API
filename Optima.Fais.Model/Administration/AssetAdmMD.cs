namespace Optima.Fais.Model
{
    public class AssetAdmMD
    {
        public int AssetId { get; set; }

        public virtual Asset Asset { get; set; }

        public int AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public int? RoomId { get; set; }

        public int? EmployeeId { get; set; }

        public int? CostCenterId { get; set; }

        public int? AssetCategoryId { get; set; }

        //public int? CompanyId { get; set; }

        public int? DepartmentId { get; set; }

        public int? AdministrationId { get; set; }

		public int? SubTypeId { get; set; }

		public int? InsuranceCategoryId { get; set; }

		public int? ModelId { get; set; }

		public int? BrandId { get; set; }

		public int? ProjectId { get; set; }
		public int? AssetTypeId { get; set; }

        public int? AssetStateId { get; set; }

        

        

        public int? AssetNatureId { get; set; }

        public int? BudgetManagerId { get; set; }

        

        public int? AssetClassId { get; set; }

        public int? AdmCenterId { get; set; }

        public int? RegionId { get; set; }

        public virtual Room Room { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual CostCenter CostCenter { get; set; }

        public virtual AssetCategory AssetCategory { get; set; }

        public virtual Department Department { get; set; }

        //public virtual Company Company { get; set; }

        public virtual Administration Administration { get; set; }

		public virtual SubType SubType { get; set; }

		

		public virtual Model Model { get; set; }

		public virtual Brand Brand { get; set; }

		public virtual Project Project { get; set; }

		public virtual AssetType AssetType { get; set; }

        public virtual AssetState AssetState { get; set; }

        

        public virtual AssetNature AssetNature { get; set; }

        public virtual BudgetManager BudgetManager { get; set; }

        

        public virtual AssetClass AssetClass { get; set; }

        public virtual AdmCenter AdmCenter { get; set; }

        public virtual Region Region { get; set; }

        public int? DivisionId { get; set; }

        public virtual Division Division { get; set; }

        public int? ProjectTypeId { get; set; }

        public virtual ProjectType ProjectType { get; set; }

        public int? RequestId { get; set; }

        public virtual Request Request { get; set; }
    }
}
