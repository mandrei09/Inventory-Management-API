using System;

namespace Optima.Fais.Dto
{
    public class CostCenter
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? AdmCenterId { get; set; }
        public int? RegionId { get; set; }
        public int? DivisionId { get; set; }
        public CodeNameEntity AdmCenter { get; set; }
        public CodeNameEntity Region { get; set; }
        public CodeNameEntity Division { get; set; }
        public CodeNameEntity Department { get; set; }
        public CodeNameEntity Company { get; set; }

        //public int? EmployeeId { get; set; }
        //public EmployeeResource Employee { get; set; }
        //public int? EmployeeId2 { get; set; }
        //public EmployeeResource Employee2 { get; set; }
        //public int? EmployeeId3 { get; set; }
        //public EmployeeResource Employee3 { get; set; }
        public int? AdministrationId { get; set; }
        public CodeNameEntity Administration { get; set; }
        public CodeNameEntity Storage { get; set; }
        public int? RoomId { get; set; }
        public CodeNameEntity Room { get; set; }

        public System.DateTime ModifiedAt { get; set; }

		public bool IsFinished { get; set; }

		public DateTime? DateFinished { get; set; }

		public bool AllowLabelList { get; set; }

		public bool InventoryList { get; set; }

		public bool BookBefore { get; set; }

		public bool BookAfter { get; set; }

		public bool PvBook { get; set; }

		public int AssetCount { get; set; }
	}

    public class CostCenterViewResource : CodeNameEntity
    {
        public AdmCenterViewResource AdmCenter { get; set; }
    }
}
