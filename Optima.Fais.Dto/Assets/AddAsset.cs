using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class AddAsset
    {
        public int Id { get; set; }
        //public string Name { get; set; }
        public string SerialNumber { get; set; }
        //public string ERPCode { get; set; }
        //public string SAPCode { get; set; }
        // public int? DocumentTypeId { get; set; }
        public string DocNo1 { get; set; }
        //public int? AssetCategoryId { get; set; }
        //public int? DictionaryItemId { get; set; }
        // public float Quantity { get; set; }
        // public bool Validated { get; set; }
        // public int? CostCenterId { get; set; }
        //public int? PartnerId { get; set; }
        //public int? CompanyId { get; set; }
        //public int? OrderId { get; set; }
        //public int? EmployeeId { get; set; }
        //public int? MaterialId { get; set; }
        //public int? ExpAccountId { get; set; }
		//public bool PostCap { get; set; }
  //      public bool InConservation { get; set; }
        // public string Name2 { get; set; }
        //public string InventoryNumber { get; set; }
        //public string LastInventoryDate { get; set; }
        //public string LastInventoryDoc { get; set; }
		//public string PlateNo { get; set; }
        //public DateTime CapitalizationDate { get; set; }
		//public int? StockId { get; set; }
        //public int? SubCategoryId { get; set; }
        // public int? OfferMaterialId { get; set; }
        //public int? OrderMaterialId { get; set; }
        //public int? RequestBudgetForecastMaterialId { get; set; }
        public List<int> RequestBudgetForecasts { get; set; }
        //public Guid Guid { get; set; }
		//public decimal ReceptionPrice { get; set; }
        //public int? AssetEntityId { get; set; }
		// public bool MultipleQuantity { get; set; }
	}
}
