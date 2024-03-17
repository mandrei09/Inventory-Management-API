using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Dto
{
    public class InventoryAssetResource
    {
		public InventoryAssetResource()
		{
			entityFiles = new List<string>();
		}
		public int Id { get; set; }

        public string InvNo { get; set; }
        public string Name { get; set; }
        public DateTime? PurchaseDate { get; set; }

        public CodeNameEntity AdmCenterInitial { get; set; }
        public CodeNameEntity CostCenterInitial { get; set; }
        public CodeNameEntity RegionInitial { get; set; }
        public CodeNameEntity LocationInitial { get; set; }
        public CodeNameEntity RoomInitial { get; set; }
        public EmployeeResource EmployeeInitial { get; set; }
        public CodeNameEntity AdministrationInitial { get; set; }
        public CodeNameEntity DivisionInitial { get; set; }
        
        public CodeNameEntity AdmCenterFinal { get; set; }
        public CodeNameEntity CostCenterFinal { get; set; }
        public CodeNameEntity RegionFinal { get; set; }
        public CodeNameEntity LocationFinal { get; set; }
        public CodeNameEntity RoomFinal { get; set; }
        public EmployeeResource EmployeeFinal { get; set; }
        public CodeNameEntity AdministrationFinal { get; set; }
        public CodeNameEntity DivisionFinal { get; set; }
        //public CodeNameEntity Model { get; set; }
        //public CodeNameEntity Brand { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }


        public string SerialNumberInitial { get; set; }
        public string SerialNumberFinal { get; set; }
		public string SerialNumber { get; set; }
		public string Producer { get; set; }
        public Model Model { get; set; }
		public Brand Brand { get; set; }
        public DictionaryItem DictionaryItem { get; set; }
        public string Info { get; set; }

        public float QIntial { get; set; }
        public float QFinal { get; set; }
        public CodeNameEntity Uom { get; set; }

        public CodeNameEntity AssetType { get; set; }
        public CodeNameEntity MasterType { get; set; }
        public CodeNameEntity Type { get; set; }
        public CodeNameEntity InvStateInitial { get; set; }
        public CodeNameEntity InvStateFinal { get; set; }
        public CodeNameEntity InvDetailState { get; set; }

        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }

        public bool? Custody { get; set; }

        public bool? AllowLabel { get; set; }

        public DateTime ModifiedAt { get; set; }
        public EmployeeResource ModifiedBy { get; set; }

        public CodeNameEntity AssetCategory { get; set; }
        public CodeNameEntity Partner { get; set; }
		public CodeNameEntity AppState { get; set; }
        public CodeNameEntity State { get; set; }
        public string ERPCode { get; set; }
        public string SAPCode { get; set; }
        public string InvName { get; set; }
        public string TempReco { get; set; }
        public string TempName { get; set; }
        public string DocNo1 { get; set; }
        public string UserName { get; set; }
        public int ImageCount { get; set; }

		public bool IsMinus { get; set; }

		public bool IsPlus { get; set; }

		public string InfoMinus { get; set; }

		public string InfoPlus { get; set; }

		public int AssetId { get; set; }

		public ApplicationUser InventoryTeamManager { get; set; }
		public ApplicationUser InventoryResponsable { get; set; }

		public decimal CurrentAPC { get; set; }

		public decimal AccumulDep { get; set; }

		public decimal CurrBkValue { get; set; }

		[NotMapped]
		public List<string> entityFiles { get; set; }
	}
}