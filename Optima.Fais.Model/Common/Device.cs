using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class Device : Entity
    {
      
        public string Code { get; set; }

        public string Name { get; set; }

		public string Model { get; set; }

		public string Producer { get; set; }

		public string UUI { get; set; }

		public string Serial { get; set; }

		public string Platform { get; set; }

		public string Version { get; set; }

		public string Type { get; set; }

		public int? AssetId { get; set; }

		public virtual Asset Asset { get; set; }

		public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

		public int? EmployeeId { get; set; }

		public virtual Employee Employee { get; set; }

		public int? DeviceTypeId { get; set; }

        public DeviceType DeviceType { get; set; }
    }
}
