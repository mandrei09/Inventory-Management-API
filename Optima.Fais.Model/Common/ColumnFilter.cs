using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
	public class ColumnFilter : Entity
	{
        public string Property { get; set; }

		public bool Active { get; set; }

		public string Type { get; set; }

		public string Placeholder { get; set; }

		public string Model { get; set; }
	}
}
