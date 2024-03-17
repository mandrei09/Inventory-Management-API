using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class LocationSync
	{
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? RegionId { get; set; }
        public int? AdmCenterId { get; set; }
        public int? CostCenterId { get; set; }
        public int? CityId { get; set; }
        public int? LocationTypeId { get; set; }
		public bool IsFinished { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
