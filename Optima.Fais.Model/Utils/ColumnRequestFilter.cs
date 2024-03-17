using System;
using System.Collections.Generic;

namespace Optima.Fais.Model.Utils
{
    public class ColumnRequestFilter
    {
		public List<int?> InvStateIds { get; set; }
		public List<int?> MaterialIds { get; set; }
		public List<int?> CostCenterIds { get; set; }
        public string Code { get; set; }
    }
}
