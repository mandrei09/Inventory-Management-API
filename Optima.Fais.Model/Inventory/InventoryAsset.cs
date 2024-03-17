using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public class InventoryAsset// : Entity
	{
		public InventoryAsset()
		{
			entityFiles = new List<string>();
		}

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }
        //[Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public DateTime? UpdatedAt { get; set; }

        public int InventoryId { get; set; }
        public virtual Inventory Inventory { get; set; }

        public int AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public int? CostCenterIdInitial { get; set; }
        public virtual CostCenter CostCenterInitial { get; set; }
        public int? AdministrationIdInitial { get; set; }
        public virtual Administration AdministrationInitial { get; set; }
        public int? RoomIdInitial { get; set; }
        public virtual Room RoomInitial { get; set; }
        public int? RoomIdFinal { get; set; }
        public virtual Room RoomFinal { get; set; }

        public int? CostCenterIdFinal { get; set; }
        public virtual CostCenter CostCenterFinal { get; set; }
        public int? AdministrationIdFinal { get; set; }
        public virtual Administration AdministrationFinal { get; set; }
        public int? EmployeeIdInitial { get; set; }
        public virtual Employee EmployeeInitial { get; set; }
        public int? EmployeeIdFinal { get; set; }
        public virtual Employee EmployeeFinal { get; set; }

        public string SerialNumber { get; set; }
        public string Producer { get; set; }
        public string Model { get; set; }

        public float QInitial { get; set; }
        public float QFinal { get; set; }

        public int? StateIdInitial { get; set; }
        public virtual InvState StateInitial { get; set; }

        public int? StateIdFinal { get; set; }
        public virtual InvState StateFinal { get; set; }

        public int? DetailStateId { get; set; }
        public virtual InvState DetailState { get; set; }

        [MaxLength(200)]
        public string Info { get; set; }

        public int? DimensionIdFinal { get; set; }
        public virtual Dimension DimensionFinal { get; set; }

        public string Info2019 { get; set; }

        public int? UomIdFinal { get; set; }
        public virtual Uom UomFinal { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedAt { get; set; }

        [MaxLength(450)]
        public string ModifiedBy { get; set; }

        public ApplicationUser ModifiedByUser { get; set; }

		public bool IsMinus { get; set; }

		public bool IsPlus { get; set; }

		public string InfoMinus { get; set; }

		public string InfoPlus { get; set; }

		public bool? IsTemp { get; set; }

		public int ImageCount { get; set; }

		public bool IsReconcile { get; set; }

		public string TempReco { get; set; }

		public string TempName { get; set; }

		public int? AssetRecoStateId { get; set; }

		public string SNInitial { get; set; }

		public bool AllowLabelInitial { get; set; }

		public virtual AppState AssetRecoState { get; set; }

        public string TempRecoSerialNumber { get; set; }

		[MaxLength(450)]
		public string TempUserId { get; set; }

		public ApplicationUser TempUser { get; set; }

		//[MaxLength(450)]
		//public string TempUserId { get; set; }

		//public ApplicationUser TempUser { get; set; }

		//public bool InInventory { get; set; }

		//public string Version { get; set; }

		//[DataType(DataType.DateTime)]
		//public DateTime? ScanDate { get; set; }

		//[MaxLength(450)]
		//public string ScanBy { get; set; }

		//public ApplicationUser ScanByUser { get; set; }
		[MaxLength(450)]
		public string InventoryResponsableId { get; set; }

		public ApplicationUser InventoryResponsable { get; set; }

		[MaxLength(450)]
        public string InventoryTeamManagerId { get; set; }

        public ApplicationUser InventoryTeamManager { get; set; }

		public decimal CurrentAPC { get; set; }

		public decimal AccumulDep { get; set; }

		public decimal CurrBkValue { get; set; }

		[NotMapped]
		public List<string> entityFiles { get; set; }
	}
}
