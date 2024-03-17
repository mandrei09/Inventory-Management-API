using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class PrintLabel : Entity
    {

        public string Code { get; set; }

        public string Name { get; set; }

		public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

		public int AssetId { get; set; }

		public virtual Asset Asset { get; set; }

        public DateTime? PrintDate { get; set; }

		public DateTime UploadDate { get; set; }

        public bool Hidden { get; set; }
    }
}
