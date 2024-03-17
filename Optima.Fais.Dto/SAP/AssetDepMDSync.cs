using Optima.Fais.Model;

namespace Optima.Fais.Dto
{
    public class AssetDepMDSync
    {
        public int Id { get; set; }

        public CodeNameEntity AccSystem { get; set; }

        public CodeNameEntity BudgetManager { get; set; }

        public AccMonth AccMonth { get; set; }

        public CodeNameEntity Company { get; set; }

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
