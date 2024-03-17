using System;

namespace Optima.Fais.Model
{
    public partial class CostCenter : Entity
    {
        public CostCenter()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? AdmCenterId { get; set; }

        public AdmCenter AdmCenter { get; set; }

        public int? RegionId { get; set; }

        public Region Region { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public string ERPCode { get; set; }

		public int? ERPId { get; set; }

        public int? EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        //public int? EmployeeId2 { get; set; }

        //public virtual Employee Employee2 { get; set; }

        //public int? EmployeeId3 { get; set; }

        //public virtual Employee Employee3 { get; set; }

        //public int? EmployeeId4 { get; set; }

        //public virtual Employee Employee4 { get; set; }

        //public int? EmployeeId5 { get; set; }

        //public virtual Employee Employee5 { get; set; }

        //public int? EmployeeId6 { get; set; }

        //public virtual Employee Employee6 { get; set; }

        //public int? EmployeeId7 { get; set; }

        //public virtual Employee Employee7 { get; set; }

        public int? DivisionId { get; set; }

        public virtual Division Division { get; set; }

		public int? AdministrationId { get; set; }

		public virtual Administration Administration { get; set; }

		public int? LocationId { get; set; }

		public virtual Location Location { get; set; }

        public int? RoomId { get; set; }

        public virtual Room Room { get; set; }

        public int? StorageId { get; set; }

        public virtual Storage Storage { get; set; }

		public int? ArticleId { get; set; }

		public virtual Article Article { get; set; }

        public bool IsFinished { get; set; }

		public DateTime? DateFinished { get; set; }

		public bool AllowLabelList { get; set; }

		public bool InventoryList { get; set; }

		public bool BookBefore { get; set; }

		public bool BookAfter { get; set; }

		public bool PvBook { get; set; }

        public int AssetCount { get; set; } 
    }
}
