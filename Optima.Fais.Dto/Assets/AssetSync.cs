using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class AssetSync
	{
        public int Id { get; set; }

        public string InvNo { get; set; }

        public string InvNoOld { get; set; }

        public string Name { get; set; }

        public string SerialNumber { get; set; }

        public string Info { get; set; }

        public float Quantity { get; set; }

        public bool AllowLabel { get; set; }

        public int RoomId { get; set; }

        //public int AdministrationId { get; set; }

        public int InvStateId { get; set; }

        public DateTime ModifiedAt { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public decimal ValueInv { get; set; }

        //public int DimensionId { get; set; }

        //public int UomId { get; set; }

        //public int AssetTypeId { get; set; }

        //public int AssetCategoryId { get; set; }

        public int ImageCount { get; set; }

		public bool IsDeleted { get; set; }

        //public bool IsMinus { get; set; }

        //public string InfoMinus { get; set; }

        public string ErpCode { get; set; }

        public string SapCode { get; set; }

        public string SubNo { get; set; }

        //public int DictionaryItemId { get; set; }

        //public int CompanyId { get; set; }

        //public int AssetNatureId { get; set; }

        //public int InterCompanyId { get; set; }

        //public int InsuranceCategoryId { get; set; }

        public int EmployeeId { get; set; }

        public int CostCenterId { get; set; }

        public int MaterialId { get; set; }

        public bool IsPrinted { get; set; }

        public string TempUserId { get; set; }

		public bool IsTemp { get; set; }

	}
}
