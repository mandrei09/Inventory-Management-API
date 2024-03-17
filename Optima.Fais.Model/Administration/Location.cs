using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class Location : Entity
    {
        public Location()
        {
            Rooms = new HashSet<Room>();
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Prefix { get; set; }

        public string ERPCode { get; set; }

        //public int? CostCenterId { get; set; }

        //public virtual CostCenter CostCenter { get; set; }

        public int? CityId { get; set; }

        public virtual City City { get; set; }

        public int? RegionId { get; set; }

        public virtual Region Region { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public int? AdmCenterId { get; set; }

        public virtual AdmCenter AdmCenter { get; set; }

        public int? LocationTypeId { get; set; }

        public virtual LocationType LocationType { get; set; }

        public string Member1 { get; set; }

        public string Member2 { get; set; }

        public string Member3 { get; set; }

		public int AssetCount { get; set; }

        public int ImageCount { get; set; }
    }
}
