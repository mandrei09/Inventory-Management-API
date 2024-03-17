using System;

namespace Optima.Fais.Dto
{
    public class SaveAssetCloneTransfer
    {
        public string ItemText { get; set; }
        public string RefDocNo { get; set; }
        //public bool CompleteTransfer { get; set; }
        public bool PriorYearAcquisitions { get; set; }
        public bool CurrentYearAcquisitions { get; set; }
        //public decimal Amount { get; set; }
        //public decimal Percent { get; set; }
        //public decimal Quantity { get; set; }
		public int UomId { get; set; }
		public int FromAssetId { get; set; }
        public int ToAssetId { get; set; }

        public string ToAssetInvNo { get; set; }
        public string ToAssetSubNo { get; set; }
        public decimal ToAssetQuantity { get; set; }
        public string ToAssetName { get; set; }
        public int ToAssetExpAccountId { get; set; }
        public int ToAssetCompanyId { get; set; }
        public string ToAssetSerialNumber { get; set; }
        public int ToAssetCostCenterId { get; set; }
        public int ToAssetEmployeeId { get; set; }
        public int? ToAssetPlantId { get; set; }
        public int? ToAssetLocationId { get; set; }
        public int ToAssetRoomId { get; set; }
        public int ToAssetAssetCategoryId { get; set; }
        public int ToAssetPartnerId { get; set; }
        public string ToAssetInvoice { get; set; }
    }
}
