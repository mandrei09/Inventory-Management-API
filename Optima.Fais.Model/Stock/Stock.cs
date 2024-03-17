using System;

namespace Optima.Fais.Model
{
    public partial class Stock : Entity
    {
        public Stock()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public DateTime? Last_Incoming_Date { get; set; }

        public string LongName { get; set; }

        public string Plant { get; set; }

        public string Storage_Location { get; set; }

        public string UM { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public int? UomId { get; set; }

        public virtual Uom Uom { get; set; }

        public int? MaterialId { get; set; }

        public virtual Material Material { get; set; }

        public int? BrandId { get; set; }

        public virtual Brand Brand { get; set; }

		public decimal Quantity { get; set; }

        public decimal Value { get; set; }

        public int? PartnerId { get; set; }

        public virtual Partner Partner { get; set; }

		public string Invoice { get; set; }

        public string EAN { get; set; }

		public bool Validated { get; set; }

        public int? ErrorId { get; set; }

        public virtual Error Error { get; set; }

        public bool Imported { get; set; }

        public int? StorageInitialId { get; set; }

        public virtual Storage StorageInitial { get; set; }

        public int? StorageId { get; set; }

        public virtual Storage Storage { get; set; }

        public int? PlantInitialId { get; set; }

        public virtual Plant PlantInitial { get; set; }

        public int? PlantActualId { get; set; }

        public virtual Plant PlantActual { get; set; }

    }
}
