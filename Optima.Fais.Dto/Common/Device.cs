using Optima.Fais.Model;

namespace Optima.Fais.Dto
{
    public class Device
	{
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
		public string Model { get; set; }

		public string Producer { get; set; }

		public string UUI { get; set; }

		public string Serial { get; set; }

		public string Platform { get; set; }

		public string Version { get; set; }

		//public string Type { get; set; }

		public int? DeviceTypeId { get; set; }

		public DeviceType DeviceType { get; set; }

		public EmployeeResource Employee { get; set; }
	}
}
