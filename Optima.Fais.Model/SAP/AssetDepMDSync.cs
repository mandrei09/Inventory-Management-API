namespace Optima.Fais.Model
{
    public class AssetDepMDSync : Entity
    {
        public int AccSystemId { get; set; }

        public virtual AccSystem AccSystem { get; set; }

        public int BudgetManagerId { get; set; }

        public virtual BudgetManager BudgetManager { get; set; }

        public int AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public string InvNo { get; set; }

        public string SubNumber { get; set; }

		public string ANLN1 { get; set; }
		public string ANLN2 { get; set; }
		public string NDJAR { get; set; }
		public string NDPER { get; set; }
		public string NDABJ { get; set; }
		public decimal APC_FY_START { get; set; }
		public decimal DEP_FY_START { get; set; }
		public decimal ACQUISITION { get; set; }
		public decimal DEP_FY { get; set; }
		public decimal ANBTR_R { get; set; }
		public decimal DEPRET { get; set; }
		public decimal ANBTR_T { get; set; }
		public decimal DEPTRANS { get; set; }

		public bool IsImported { get; set; }
	}
}
