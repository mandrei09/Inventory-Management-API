using System;

namespace Optima.Fais.Dto
{
    public class AddStockAsset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        //public int? CostCenterId { get; set; }
        //public int? EmployeeId { get; set; }
        public string Name2 { get; set; }
		public int? StockId { get; set; }
        public int? SubCategoryId { get; set; }
		public int? OrderId { get; set; }
	}
}
