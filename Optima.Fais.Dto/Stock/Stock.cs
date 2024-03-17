using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class Stock
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
		public CodeNameEntity Uom { get; set; }
        public CodeNameEntity Material { get; set; }
        public CodeNameEntity Category { get; set; }
        public CodeNameEntity Company { get; set; }
        public CodeNameEntity Brand { get; set; }
        public CodePartnerEntity Partner { get; set; }
        public DateTime? Last_Incoming_Date { get; set; }
        public string LongName { get; set; }
        public string Plant { get; set; }
        public string Storage_Location { get; set; }
        public string UM { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
        public string Invoice { get; set; }
        public string EAN { get; set; }
        public CodeNameEntity Error { get; set; }
        public bool Validated { get; set; }
        public bool Imported { get; set; }
        public CodeNameEntity PlantInitial { get; set; }
        public CodeNameEntity PlantActual { get; set; }
        public CodeNameEntity StorageInitial { get; set; }
        public CodeNameEntity Storage { get; set; }
    }
}
