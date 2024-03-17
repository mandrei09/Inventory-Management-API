using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class EmployeeValidate
    {
        public int AssetId { get; set; }
		public Guid Guid { get; set; }
		public bool Accepted { get; set; }
        public string Reason { get; set; }
    }
}
