using System;

namespace Optima.Fais.Dto
{
    public class ColumnFilter
    {
        public int Id { get; set; }
		public string Property { get; set; }

		public bool Active { get; set; }

		public string Type { get; set; }

		public string Placeholder { get; set; }

		public string Model { get; set; }
	}
}
