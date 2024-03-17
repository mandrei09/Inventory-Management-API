using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class AssetAdmIn
    {
        public int AssetId { get; set; }

        public int? RoomId { get; set; }

        public int? EmployeeId { get; set; }

        public int? CostCenterId { get; set; }

        public int? AssetCategoryId { get; set; }

        public int? DepartmentId { get; set; }

        public int? AdministrationId { get; set; }

        public int? AssetTypeId { get; set; }

        public int? AssetStateId { get; set; }

        public int? InvStateId { get; set; }

        public virtual Asset Asset { get; set; }

        public virtual Room Room { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual CostCenter CostCenter { get; set; }

        public virtual AssetCategory AssetCategory { get; set; }

        public virtual Department Department { get; set; }

        public virtual Administration Administration { get; set; }

        public virtual AssetType AssetType { get; set; }

        public virtual AssetState AssetState { get; set; }

        public virtual InvState InvState { get; set; }
    }
}
