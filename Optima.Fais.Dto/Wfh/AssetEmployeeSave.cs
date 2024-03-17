using System;

namespace Optima.Fais.Dto
{
	public class AssetEmployeeSave
	{
		public int Id { get; set; }
		public string InvNo { get; set; }
		public string Name { get; set; }
		public string SerialNumber { get; set; }
		public string Info { get; set; }
		public string Info2019 { get; set; }
		public string BrandName { get; set; }
		public string ModelName { get; set; }
		public bool Validated { get; set; }
		public int DictionaryItemId { get; set; }
		public int BrandId { get; set; }
		public int ModelId { get; set; }
		public string SapCode { get; set; }
		public int EmployeeId { get; set; }
		public string Imei { get; set; }
		public string PhoneNumber { get; set; }
	}
}
